using System;
using System.Collections.Generic;
using System.Text;

namespace ODL.Common
{
    public class DatabaseUtils
    {
        public class Postgres
        {
            public static Npgsql.NpgsqlConnection ConnectToPostGRES(DBConnectionDetails dbConnectionInfo)
            {
                Npgsql.NpgsqlConnectionStringBuilder _connString = new Npgsql.NpgsqlConnectionStringBuilder();
                _connString.Username = dbConnectionInfo.DBUsername;
                _connString.Password = dbConnectionInfo.DBPassword;
                _connString.Database = dbConnectionInfo.DBCatalog;
                _connString.Host = dbConnectionInfo.DBServer;
                Npgsql.NpgsqlConnection _dbConnection = new Npgsql.NpgsqlConnection(_connString.ConnectionString);
                _dbConnection.Open();

                return _dbConnection;
            }
        }

    }
}
