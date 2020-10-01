using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Linq;

namespace ODL.Common
{
    public class USCensusS0901 : IngestBase
    {
        public USCensusS0901(LogHandler LogObject, ODL.Common.DBConnectionDetails DbConnectionInfo, String FileName) : base(LogObject, DbConnectionInfo, FileName)
        {

        }

        /// <summary>
        /// Load Records From selected ZIP file.  Each ZIP will be unique, data might be csv or mdb or accdb or xls.
        /// This method will open the zip, figure out the best file, extract it, parse it, create DB tables (if necessary) and load the data.
        /// </summary>
        /// <returns>(Int32)Count of Records in the File</returns>
        public override Int32 LoadRecordsFromFile()
        {
            Int32 recordCount = 0;

            using (ZipArchive archive = ZipFile.OpenRead(base.DataFile.FullName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    //Find the matching files
                    if (VerifyDesirableFile(entry))
                    {
                        string tempPath = IngestBase.TEMP_FOLDER + "\\" + entry.Name.Replace(".","").Replace(".csv", "") + ".csv";

                        //Find the matching files
                        //Extract file to a temporary location
                        if (ExtractFileToTempLocation(entry, tempPath))
                        {
                            //Read Access (.mdb) files
                            string connectionString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source=" + IngestBase.TEMP_FOLDER + ";" + "Extended Properties=\"Text;HDR=Yes;FMT=Delimited\"";

                            using (OleDbConnection connection = new OleDbConnection(connectionString))
                            {
                                try
                                {
                                    connection.Open();
                                    DataTable schema = null;
                                    DataTable data = new DataTable();

                                    using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + entry.Name.Replace(".", "").Replace(".csv", "") + ".csv" + "]", connection))
                                    {
                                        adapter.Fill(data);
                                    }

                                    recordCount++;

                                    WriteToPostgres(entry.Name, schema, data);

                                }
                                catch (Exception ex)
                                {
                                    base.Logger.LogError("Can't connect to the database with specified parameters: " + ex.Message);
                                }
                            }

                            File.Delete(tempPath);
                        }
                    }
                }
            }

            return recordCount;
        }

        /// <summary>
        /// This method should verify/create a database table that fits the data format that is being loaded for the table type.
        /// Ideally, there would be a hashid of each record as the PK that will prevent duplicate entries if the same file is loaded twice.
        /// </summary>
        public override void CreateDatabaseTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method should select the common PK (if one exists in the source) or the generated PK/hash from the DB to ensure
        /// that no duplicate records are loaded to the repository from the source files.
        /// </summary>
        public override List<String> PopulatePreviouslyLoadedRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes a file and determines whether or not it is a desirable file to parse.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private bool VerifyDesirableFile(ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) && !entry.FullName.Contains("_metadata_");
        }

        /// <summary>
        /// Takes a file within a zip archive and safely extracts it to the target path
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="targetPath"></param>
        /// <returns>sucess = true, or failure</returns>
        private bool ExtractFileToTempLocation(ZipArchiveEntry entry, string targetPath)
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
                base.Logger.LogError(ex, "Error Extracting File to Temp Location");
            }

            return success;
        }

        /// <summary>
        /// Converts from .Net Type DataRow to PostGres SQL column definition type (hopefully)
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <remarks>Not using NpgsqlDbType because it cannot be cleaning converted</remarks>
        private string ConvertToPostGresType(DataRow row)
        {
            string columnDefinition = row.Field<string>("ColumnName") + " ";

            switch (row.Field<Type>("DataType").FullName)
            {
                case "System.Int64":
                    columnDefinition += "int8 (" + row.Field<int>("ColumnSize") + ")";
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
                    columnDefinition += "float8 (" + row.Field<int>("NumericPrecision") + ", " + row.Field<int>("NumericScale") + ")";
                    break;
                case "System.Int32":
                    columnDefinition += "int4 (" + row.Field<int>("ColumnSize") + ")";
                    break;
                case "System.Decimal":
                    columnDefinition += "numeric (" + row.Field<int>("NumericPrecision") + ", " + row.Field<int>("NumericScale") + ")";
                    break;
                case "System.Single":
                    columnDefinition += "float4 (" + row.Field<int>("NumericPrecision") + ", " + row.Field<int>("NumericScale") + ")";
                    break;
                case "System.Int16":
                    columnDefinition += "int2 (" + row.Field<int>("ColumnSize") + ")";
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
                    columnDefinition += "varchar (" + row.Field<int>("ColumnSize") + ")";
                    break;
            }

            return columnDefinition;
        }

        /// <summary>
        /// Takes in-memory information (datatables) and creates a Postgres table and populates that table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <param name="data"></param>
        private void WriteToPostgres(string tableName, DataTable schema, DataTable data)
        {
            try
            {
                base.PostgresConnection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(string.Empty, base.PostgresConnection))
                {
                    command.CommandText = "CREATE TABLE " + tableName + " (";

                    foreach (DataRow row in schema.Rows)
                    {
                        command.CommandText += ConvertToPostGresType(row) + ", ";
                    }

                    command.CommandText = command.CommandText.TrimEnd(new char[] { ',', ' ' }) + " );";

                    command.ExecuteNonQuery();
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);

                    byte[] bytes = stream.GetBuffer();

                    using (var outStream = base.PostgresConnection.BeginRawBinaryCopy("COPY " + tableName + " FROM STDIN (FORMAT BINARY)"))
                    {
                        outStream.Write(bytes, 0, data.Rows.Count);
                    }
                }
            }
            finally
            {
                base.PostgresConnection.Close();
            }
        }
    }
}
