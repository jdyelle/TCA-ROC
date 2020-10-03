using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace ODL.Common
{
    public class DBConnectionDetails
    {
        public string DBServer = String.Empty;
        public string DBCatalog = String.Empty;
        public string DBUsername = String.Empty;
        public string DBPassword = String.Empty;
        public int DBPort = -1;
        public SupportedDatabases DBType = SupportedDatabases.PostgreSQL;
    }

    public enum SupportedDatabases
    {
        PostgreSQL
    }
}
