using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace ODL.Common
{
    public class TeacherEvaluations : IngestBase
    {
        public TeacherEvaluations(LogHandler LogObject, ODL.Common.DBConnectionDetails DbConnectionInfo, String FileName) : base(LogObject, DbConnectionInfo, FileName)
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
                //THE START OF SOME EXTREMELY BREAKABLE CODE
                string schoolYear = Regex.Replace(base.DataFile.FullName, @"[^\d]", ""); //remove non-numeric characters
                string schoolYearFormatted = schoolYear + "-" + (short.Parse(schoolYear.Substring(2, 2)) + 1);
                //END OF EXTREMELY BREAKABLE CODE

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (VerifyDesirableFile(entry))
                    {
                        string tempPath = IngestBase.TEMP_FOLDER + "\\" + entry.FullName;

                        if (ExtractFileToTempLocation(entry, tempPath))
                        {
                            DataSet data = RetrieveFromMDBFile(tempPath);

                            foreach (DataTable table in data.Tables)
                            {
                                if (table.TableName.EndsWith("_SCHEMA"))
                                {
                                    continue;
                                }

                                //THE START OF SOME EXTREMELY BREAKABLE CODE
                                if (table.Columns.Contains("SCHOOL_YEAR"))
                                {
                                    //do nothing, date is already recorded
                                }
                                else
                                {
                                    table.Columns.Add("SCHOOL_YEAR").Expression = "'" + schoolYearFormatted + "'";
                                }
                                //END OF EXTREMELY BREAKABLE CODE

                                string postGresTableName = table.TableName.Substring(0, table.TableName.LastIndexOf("_DATA"));

                                this.PreviouslyLoadedRecords.Add(CreatePrimaryKey(postGresTableName, schoolYear));
                                recordCount++;

                                WriteToPostgres(postGresTableName, GetMatchingSchemaTable(table.TableName.Replace("_DATA", "_SCHEMA"), data), table);
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
            List<string> previousRecords = new List<string>();

            using (NpgsqlCommand command = new NpgsqlCommand(string.Empty, this.PostgresConnection))
            {
                foreach (string table in new string[]{
                    "APPR_DISTRICT_RESEARCHER_FILE_DATA", "APPR_SCHOOL_RESEARCHER_FILE_DATA", "APPR_STATEWIDE_RESEARCHER_FILE_DATA", "SPG_DISTRICT_RESEARCHER_FILE_DATA", "SPG_SCHOOL_RESEARCHER_FILE_DATA", "SPG_STATEWIDE_RESEARCHER_FILE_DATA", "APPR_RESEARCHER_DATA_PART_C_ORIGINAL_DISTRICT", "APPR_RESEARCHER_DATA_PART_C_ORIGINAL_SCHOOL", "APPR_RESEARCHER_DATA_PART_C_ORIGINAL_STATEWIDE", "APPR_RESEARCHER_DATA_PART_C_TRANSITION_DISTRICT", "APPR_RESEARCHER_DATA_PART_C_TRANSITION_SCHOOL", "APPR_RESEARCHER_DATA_PART_C_TRANSITION_STATEWIDE", "APPR_RESEARCHER_DATA_PART_D_ORIGINAL_DISTRICT" , "APPR_RESEARCHER_DATA_PART_D_ORIGINAL_SCHOOL", "APPR_RESEARCHER_DATA_PART_D_ORIGINAL_STATEWIDE", "APPR_RESEARCHER_DATA_PART_D_TRANSITION_DISTRICT", "APPR_RESEARCHER_DATA_PART_D_TRANSITION_SCHOOL", "APPR_RESEARCHER_DATA_PART_D_TRANSITION_STATEWIDE"})
                {
                    command.CommandText = String.Format(@"IF EXISTS( SELECT FROM pg_tables WHERE tablename='{0}') THEN 
                                                              SELECT '{0}' as TABLE, SCHOOL_YEAR FROM {0} GROUP BY SCHOOL_YEAR;
                                                          END IF;", table);

                    //TODO fix error "syntax error at or near "IF"'"
                    //"Why are we not doing this as a UNION?" ... because I can't get this already simple case to work and I don't want to add more complexity until I can do the simple case.
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            previousRecords.Add(CreatePrimaryKey(reader["TABLE"].ToString(), reader["SCHOOL_YEAR"].ToString()));
                        }
                    }
                }
            }

            return previousRecords;
        }

        /// <summary>
        /// Takes a file and determines whether or not it is a desirable file to parse.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private bool VerifyDesirableFile(ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith(".mdb", StringComparison.OrdinalIgnoreCase);
        }

        private string CreatePrimaryKey(string tableName, string schoolYear)
        {
            return tableName + "|" + schoolYear;
        }
    }
}
