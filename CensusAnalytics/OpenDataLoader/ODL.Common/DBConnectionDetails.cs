using System;
using System.Collections.Generic;
using System.Text;

namespace ODL.Common
{
    public class DBConnectionDetails
    {
        public string DBServer;
        public string DBCatalog;
        public string DBUsername;
        public string DBPassword;
        public SupportedDatabases DBType;
    }

    public enum SupportedDatabases
    {
        PostgreSQL
    }
}
