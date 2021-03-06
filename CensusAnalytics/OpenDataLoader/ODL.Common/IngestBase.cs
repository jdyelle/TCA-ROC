﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Data.OleDb;
using Npgsql;
using System.Text.RegularExpressions;
using NpgsqlTypes;
using System.Linq;

namespace ODL.Common
{
    public abstract class IngestBase
    {
        protected const string TEMP_FOLDER = @".\TEMP\";
        protected LogHandler Logger;
        protected FileInfo DataFile;
        protected List<String> PreviouslyLoadedRecords;
        protected Npgsql.NpgsqlConnection PostgresConnection = null;

        public IngestBase(LogHandler LogObject, ODL.Common.DBConnectionDetails DbConnectionInfo, String FileName)
        {
            this.Logger = LogObject;
            this.DataFile = new FileInfo(FileName);

            try
            {
                //Figure out which type of database we have and test the connection.
                if (DbConnectionInfo.DBType == ODL.Common.SupportedDatabases.PostgreSQL) this.PostgresConnection = DatabaseUtils.Postgres.ConnectToPostGRES(DbConnectionInfo);
            }
            catch (Exception ex)
            {
                LogObject.LogError("Can't connect to the database with specified parameters: " + ex.Message);
            }
        }

        /// <summary>
        /// This is the method (after the constructor) that I should be able to call as a task and have it process everything
        /// in a separate thread.  We will use tasks so that we don't hang up the main UI thread.
        /// </summary>
        /// <returns>Number of new records loaded from selected file.</returns>
        public Int32 StartLoading()
        {
            this.PreviouslyLoadedRecords = PopulatePreviouslyLoadedRecords();
            return LoadRecordsFromFile();
        }

        /// <summary>
        /// Load Records From selected ZIP file.  Each ZIP will be unique, data might be csv or mdb or accdb or xls.
        /// This method will open the zip, figure out the best file, extract it, parse it, create DB tables (if necessary) and load the data.
        /// </summary>
        /// <returns>(Int32)Count of Records in the File</returns>
        public abstract Int32 LoadRecordsFromFile();

        /// <summary>
        /// This method should select the common PK (if one exists in the source) or the generated PK/hash from the DB to ensure
        /// that no duplicate records are loaded to the repository from the source files.
        /// </summary>
        public abstract List<String> PopulatePreviouslyLoadedRecords();

        /// <summary>
        /// Apparently, many organizations like to include periods "." in their file names, so let's do some cleanup
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>a filename where all of the invalid characters have been replaced with underscores "_"</returns>
        /// <remarks>
        /// WHY are we not using string.Join("_", filename.Split(Path.GetInvalidFileNameChars()))?
        /// Because we're going to potentially be using these filenames for table names, and we want them to be as simple (character set-wise) as possible.
        /// WHY are we not using the regex [w\]+ or something?
        /// Screw other character sets; we're running with English characters only.
        /// </remarks>
        public string ReplaceInvalidChars(string filename)
        {
            //return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
            return Regex.Replace(filename, @"[^a-zA-Z0-9_]+", "_");
        }

        /// <summary>
        /// Takes a file within a zip archive and safely extracts it to the target path
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="targetPath"></param>
        /// <returns>sucess = true, or failure</returns>
        protected bool ExtractFileToTempLocation(ZipArchiveEntry entry, string targetPath)
        {
            bool success = false;

            try
            {
                if (!Directory.Exists(IngestBase.TEMP_FOLDER))
                {
                    Directory.CreateDirectory(IngestBase.TEMP_FOLDER);
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

                entry.ExtractToFile(targetPath);

                success = true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error Extracting File to Temp Location");
            }

            return success;
        }

        /// <summary>
        /// Parse CSV file and return data as a datatable
        /// </summary>
        /// <returns></returns>
        protected DataTable RetrieveFromCSVFile(string fileName)
        {
            DataTable csvData = null;

            string connectionString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source=" + IngestBase.TEMP_FOLDER + ";" + "Extended Properties=\"Text;HDR=Yes;FMT=Delimited\"";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //TODO: Handle occurence where CSV file has more than 255 columns
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + fileName + "]", connection))
                    {
                        csvData = new DataTable();
                        adapter.Fill(csvData);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Errors retrieving data from CSV file");
                }
            }

            return csvData;
        }

        /// <summary>
        /// Parse MDB file and return data as a datatable
        /// </summary>
        /// <returns></returns>
        protected DataSet RetrieveFromMDBFile(string source)
        {
            DataSet mdbData = new DataSet();

            //This might only work on machines where the Access Engine exists AccessDatabaseEngine_x64.exe /passive
            //https://www.microsoft.com/en-us/download/details.aspx?id=13255
            string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + source + ";";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    List<string> tableNames = new List<string>();
                    var tables = connection.GetSchema("Tables");

                    foreach (DataRow row in tables.Rows)
                    {
                        var tableName = row["TABLE_NAME"].ToString();

                        //Exclude the system tables
                        if (!tableName.StartsWith("MSys"))
                        {
                            tableNames.Add(tableName);
                        }
                    }

                    foreach (var tableName in tableNames)
                    {
                        DataTable data = mdbData.Tables.Add(tableName + "_DATA");

                        using (OleDbCommand command = new OleDbCommand(string.Empty, connection))
                        {
                            command.CommandText = "SELECT * FROM " + tableName;

                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                DataTable schema = reader.GetSchemaTable();
                                schema.TableName = tableName + "_SCHEMA";
                                mdbData.Tables.Add(schema);

                                data.Load(reader);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Errors retrieving data from MDB file");
                }
            }

            return mdbData;
        }

        /// <summary>
        /// Parse XLS/XLSX file and return data as a datatable
        /// </summary>
        /// <returns></returns>
        protected DataSet RetrieveFromExcelFile(string fileName)
        {
            DataSet excelDataSet = new DataSet();

            string connectionString = string.Empty;

            switch (new FileInfo(fileName).Extension)
            {
                case ".xls": //Excel 97-03  
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source='" + fileName + "'; Extended Properties='Excel 8.0;HDR=YES;';";
                    break;

                case ".xlsx": //Excel 07  
                    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + fileName + "; Extended Properties='Excel 12.0;HDR=YES;';";
                    break;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    DataTable dtExcelSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);


                    foreach (DataRow sheet in dtExcelSchema.Rows)
                    {
                        string sheetName = sheet["TABLE_NAME"].ToString();
                        DataTable data = excelDataSet.Tables.Add(sheetName);

                        //TODO: Handle occurence where Excel file has more than 255 columns
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "]", connection))
                        {
                            adapter.Fill(data);
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Errors retrieving data from XLS/XLSX file");
                }
            }

            return excelDataSet;
        }

        /// <summary>
        /// In the tables for a dataset, find one with the matching name
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected DataTable GetMatchingSchemaTable(string tableName, DataSet data)
        {
            DataTable schema = null;

            foreach (DataTable table in data.Tables)
            {
                if (table.TableName.Equals(tableName))
                {
                    schema = table;

                    break;
                }
            }

            return schema;
        }

        /// <summary>
        /// Converts from .Net Type DataRow to PostGres SQL column definition type (hopefully)
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dataType"></param>
        /// <param name="columnSize"></param>
        /// <param name="numericPrecision"></param>
        /// <param name="numericScale"></param>
        /// <remarks>Not using NpgsqlDbType because it cannot be cleanly converted</remarks>
        private string ConvertToPostGresType(string columnName, string dataType, int columnSize, int numericPrecision, int numericScale)
        {
            string columnDefinition = columnName + " ";

            switch (dataType)
            {
                case "System.Int64":
                    columnDefinition += "int8 (" + columnSize + ")";
                    break;
                case "System.Boolean":
                    columnDefinition += "bool";
                    break;
                case "System.Byte[]":
                    columnDefinition += "bytea";
                    break;
                case "System.DateTime":
                    columnDefinition += "date";
                    break;
                case "System.Double":
                    columnDefinition += "float8 (" + numericPrecision + ", " + numericScale + ")";
                    break;
                case "System.Int32":
                    columnDefinition += "int4 (" + columnSize + ")";
                    break;
                case "System.Decimal":
                    columnDefinition += "numeric (" + numericPrecision + ", " + numericScale + ")";
                    break;
                case "System.Single":
                    columnDefinition += "float4 (" + numericPrecision + ", " + numericScale + ")";
                    break;
                case "System.Int16":
                    columnDefinition += "int2 (" + columnSize + ")";
                    break;
                case "System.TimeSpan":
                    columnDefinition += "interval";
                    break;
                case "System.IPAddress":
                    columnDefinition += "inet";
                    break;
                case "System.Guid":
                    columnDefinition += "uuid";
                    break;
                case "System.Array":
                    columnDefinition += "array";
                    break;
                default:
                    columnDefinition += "varchar (" + columnSize + ")";
                    break;
            }

            return columnDefinition;
        }

        /// <summary>
        /// Converts from .Net Type to PostGres NpgsqlDbType definition type (hopefully)
        /// </summary>
        /// <param name="dataType"></param>
        /// <remarks>Not using NpgsqlDbType because it cannot be cleaning converted</remarks>
        private NpgsqlDbType GetNpgSqlType(string dataType)
        {
            NpgsqlDbType npgType;

            switch (dataType)
            {
                case "System.Int64":
                    npgType = NpgsqlDbType.Bigint;
                    break;
                case "System.Boolean":
                    npgType = NpgsqlDbType.Boolean;
                    break;
                case "System.Byte[]":
                    npgType = NpgsqlDbType.Bytea;
                    break;
                case "System.DateTime":
                    npgType = NpgsqlDbType.Date;
                    break;
                case "System.Double":
                    npgType = NpgsqlDbType.Double;
                    break;
                case "System.Int32":
                    npgType = NpgsqlDbType.Integer;
                    break;
                case "System.Decimal":
                    npgType = NpgsqlDbType.Numeric;
                    break;
                case "System.Single":
                    npgType = NpgsqlDbType.Real;
                    break;
                case "System.Int16":
                    npgType = NpgsqlDbType.Smallint;
                    break;
                case "System.TimeSpan":
                    npgType = NpgsqlDbType.Interval;
                    break;
                case "System.IPAddress":
                    npgType = NpgsqlDbType.Inet;
                    break;
                case "System.Guid":
                    npgType = NpgsqlDbType.Uuid;
                    break;
                case "System.Array":
                    npgType = NpgsqlDbType.Array;
                    break;
                default:
                    npgType = NpgsqlDbType.Varchar;
                    break;
            }

            return npgType;
        }

        /// <summary>
        /// Takes in-memory information (datatables) and creates a Postgres table and populates that table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <param name="data"></param>
        protected void WriteToPostgres(string tableName, DataTable schema, DataTable data)
        {
            List<KeyValuePair<string, NpgsqlDbType>> columns = new List<KeyValuePair<string, NpgsqlDbType>>();

            using (NpgsqlCommand command = new NpgsqlCommand(string.Empty, this.PostgresConnection))
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + " (";

                if (schema == null)
                {
                    foreach (DataColumn column in data.Columns)
                    {
                        //TODO Figure out how to actually choose column sizes
                        int length = column.MaxLength < 1 ? 500 : column.MaxLength;
                        int precision = length;
                        int scale = 6;

                        columns.Add(new KeyValuePair<string, NpgsqlDbType>(column.ColumnName, NpgsqlDbType.Varchar));
                        command.CommandText += ConvertToPostGresType(column.ColumnName, column.DataType.FullName, length, precision, scale) + ", ";
                    }
                }
                else
                {
                    foreach (DataRow row in schema.Rows)
                    {
                        columns.Add(new KeyValuePair<string, NpgsqlDbType>(row.Field<string>("ColumnName"), GetNpgSqlType(row.Field<Type>("DataType").FullName)));
                        command.CommandText += ConvertToPostGresType(row.Field<string>("ColumnName"), row.Field<Type>("DataType").FullName,
                            row.Field<int>("ColumnSize"), row.Field<int>("NumericPrecision"), row.Field<int>("NumericScale")) + ", ";
                    }
                }

                command.CommandText = command.CommandText.TrimEnd(new char[] { ',', ' ' }) + " );";

                command.ExecuteNonQuery();
            }

            using (var writer = this.PostgresConnection.BeginBinaryImport("COPY " + tableName + "(" + string.Join(",", (from kvp in columns select kvp.Key).ToList()) + ") FROM STDIN (FORMAT BINARY)"))
            {
                foreach (DataRow row in data.Rows)
                {
                    writer.StartRow();

                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        writer.Write(row[i], columns[i].Value);
                    }
                }

                writer.Complete();
            }
        }
    }
}
