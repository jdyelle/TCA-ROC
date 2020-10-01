using ODL.Common;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHandler LogObject = new LogHandler();
            ODL.Common.DBConnectionDetails DbConnectionInf = new DBConnectionDetails() {
                DBServer = "",
                DBCatalog = "",
                DBUsername = "",
                DBPassword = "",
                DBType = SupportedDatabases.PostgreSQL
            };
            String FileName = "C:\\Users\\Zach\\Downloads\\productDownload_2020-10-01T101650.zip";
            IngestBase x = new USCensusS0901(LogObject, DbConnectionInf, FileName);
            x.LoadRecordsFromFile();
        }
    }
}
