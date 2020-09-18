using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ODL.Common
{
    internal abstract class IngestBase
    {
        private LogHandler logger;
        private IDbConnection dbConnection;
        private FileInfo dataFile;
        private List<String> PreviouslyLoadedRecords;

        public IngestBase(LogHandler LogObject, IDbConnection DbConnection, String FileName)
        {
            this.logger = LogObject;
            this.dbConnection = DbConnection;
            this.dataFile = new FileInfo(FileName);

            if (dbConnection.State != ConnectionState.Open)
            {
                logger.LogCritical("Database Connection State Passed to Ingest Class is Closed, Please Check DB Connection");
            }
        }

        /// <summary>
        /// Load Records From selected ZIP file.  Each ZIP will be unique, data might be csv or mdb or accdb or xls.
        /// This method will open the zip, figure out the best file, extract it, parse it, create DB tables (if necessary) and load the data.
        /// </summary>
        /// <returns>(Int32)Count of Records in the File</returns>
        public abstract Int32 LoadRecordsFromFile();

        /// <summary>
        /// This method should create a database table that fits the data format that is being loaded for the table type.
        /// Ideally, there would be a hashid of each record as the PK that will prevent duplicate entries if the same file is loaded twice.
        /// </summary>
        public abstract void CreateDatabaseTable();

        /// <summary>
        /// This method should select the common PK (if one exists in the source) or the generated PK/hash from the DB to ensure
        /// that no duplicate records are loaded to the repository from the source files.
        /// </summary>
        public abstract void PopulatePreviouslyLoadedRecords();
    }
}
