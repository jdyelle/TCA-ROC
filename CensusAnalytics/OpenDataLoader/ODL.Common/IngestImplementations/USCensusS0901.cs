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
                    if (VerifyDesirableFile(entry))
                    {
                        string tempPath = IngestBase.TEMP_FOLDER + "\\" + entry.Name.Replace(".", "").Replace(".csv", "") + ".csv";
                        
                        if (ExtractFileToTempLocation(entry, tempPath))
                        {
                            DataTable data = RetrieveFromCSVFile(entry.Name.Replace(".", "").Replace(".csv", "") + ".csv");
                                   
                            WriteToPostgres(entry.Name, null, data);
                            
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

    }
}
