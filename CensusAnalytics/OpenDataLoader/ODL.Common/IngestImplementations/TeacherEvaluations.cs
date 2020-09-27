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

namespace ODL.Common
{
    public class TeacherEvaluations:IngestBase
    {                
        public TeacherEvaluations(LogHandler LogObject, ODL.Common.DBConnectionDetails DbConnectionInfo, String FileName) : base(LogObject, DbConnectionInfo, FileName)
        {
            
        }

        /// <summary>
        /// Load Records From selected ZIP file.  Each ZIP will be unique, data might be csv or mdb or accdb or xls.
        /// This method will open the zip, figure out the best file, extract it, parse it, create DB tables (if necessary) and load the data.
        /// </summary>
        /// <returns>(Int32)Count of Records in the File</returns>
        public override Int32 LoadRecordsFromFile() {
            Int32 recordCount = 0;

            using (ZipArchive archive = ZipFile.OpenRead(base.dataFile.FullName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".mdb", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!Directory.Exists(IngestBase.TEMP_FOLDER))
                        {
                            Directory.CreateDirectory(IngestBase.TEMP_FOLDER);
                        }
                        string tempPath = IngestBase.TEMP_FOLDER + "\\" + entry.FullName;

                        if (File.Exists(tempPath)) {
                            File.Delete(tempPath);
                        }

                        entry.ExtractToFile(tempPath);

                        string connectionString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source=" + tempPath + ";";

                        using (OleDbConnection connection = new OleDbConnection(connectionString))
                        {
                            try {
                                connection.Open();
                                List<string> tableNames = new List<string>();
                                var schema = connection.GetSchema("Tables");

                                foreach (DataRow row in schema.Rows)
                                {
                                    var tableName = row["TABLE_NAME"].ToString();

                                    //Exclude the system tables
                                    if (!tableName.StartsWith("MSys"))
                                    {
                                        tableNames.Add(tableName);
                                    }
                                }

                                recordCount = tableNames.Count;

                                foreach (var tableName in tableNames)
                                {
                                    DataTable data = null;

                                    using (OleDbCommand command = new OleDbCommand(string.Empty, connection))
                                    {
                                        command.CommandText = "SELECT * FROM " + tableName;

                                        using (OleDbDataReader reader = command.ExecuteReader())
                                        {
                                            data.Load(reader);
                                        }
                                    }

                                    try
                                    {
                                        base.PostgresConnection.Open();

                                        using (NpgsqlCommand command = new NpgsqlCommand(string.Empty, base.PostgresConnection))
                                        {
                                            command.CommandText = "CREATE TABLE " + tableName;

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

                                    Console.WriteLine(tableName);
                                }
                            }
                            catch (Exception ex)
                            {
                                base.logger.LogError("Can't connect to the database with specified parameters: " + ex.Message);
                            }
                        }

                        File.Delete(tempPath);
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
        public override List<String> PopulatePreviouslyLoadedRecords() {
            throw new NotImplementedException();
        }

    }
}
