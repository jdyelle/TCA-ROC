using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ODL.Common
{
    public abstract class IngestBase
    {
        protected const string TEMP_FOLDER = "D:\\Temp";
        protected LogHandler logger;
        protected FileInfo dataFile;
        protected List<String> PreviouslyLoadedRecords;
        protected Npgsql.NpgsqlConnection PostgresConnection = null;

        public IngestBase(LogHandler LogObject, ODL.Common.DBConnectionDetails DbConnectionInfo, String FileName)
        {
            this.logger = LogObject;
            this.dataFile = new FileInfo(FileName);

            try
            {
               //Figure out which type of database we have and test the connection.
               if (DbConnectionInfo.DBType == ODL.Common.SupportedDatabases.PostgreSQL) PostgresConnection = DatabaseUtils.Postgres.ConnectToPostGRES(DbConnectionInfo);
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
            CreateDatabaseTable();
            PreviouslyLoadedRecords = PopulatePreviouslyLoadedRecords();
            return LoadRecordsFromFile();
        }

        /// <summary>
        /// Load Records From selected ZIP file.  Each ZIP will be unique, data might be csv or mdb or accdb or xls.
        /// This method will open the zip, figure out the best file, extract it, parse it, create DB tables (if necessary) and load the data.
        /// </summary>
        /// <returns>(Int32)Count of Records in the File</returns>
        public abstract Int32 LoadRecordsFromFile();

        /// <summary>
        /// This method should verify/create a database table that fits the data format that is being loaded for the table type.
        /// Ideally, there would be a hashid of each record as the PK that will prevent duplicate entries if the same file is loaded twice.
        /// </summary>
        public abstract void CreateDatabaseTable();

        /// <summary>
        /// This method should select the common PK (if one exists in the source) or the generated PK/hash from the DB to ensure
        /// that no duplicate records are loaded to the repository from the source files.
        /// </summary>
        public abstract List<String> PopulatePreviouslyLoadedRecords();

    }
}
