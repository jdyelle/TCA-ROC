using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;

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

                                foreach (System.Data.DataRow row in schema.Rows)
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
                                    Console.WriteLine(tableName);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        File.Delete(tempPath);
                    }
                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// This method should verify/create a database table that fits the data format that is being loaded for the table type.
        /// Ideally, there would be a hashid of each record as the PK that will prevent duplicate entries if the same file is loaded twice.
        /// </summary>
        public override void CreateDatabaseTable() {

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
