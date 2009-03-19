using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;

namespace iLabs.Core
{
    public static class FactoryDB
    {
        static private string connectionStr;
        static private DateTime minDbDate;

        static FactoryDB()
        {
            minDbDate = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // read connection string from the app settings
            connectionStr = ConfigurationManager.AppSettings["sqlConnection"];
            if (connectionStr == null || connectionStr.Equals(""))
            {
                throw new NoNullAllowedException(" The connection string is not specified, check configuration");
            }

        }

        /// <summary>
        /// Creates an unopened connection to the database, should return a DbConnection.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetConnection()
        {
            SqlConnection connection = null;
            if (connectionStr == null || connectionStr.Equals(""))
            {
                // read connection string from the app settings
                connectionStr = ConfigurationManager.AppSettings["sqlConnection"];
                if (connectionStr == null || connectionStr.Equals(""))
                {
                    throw new NoNullAllowedException(" The connection string is not specified, check configuration");
                }
            }
            // Replace with Connection constructor for the Database used
            // create an SqlConnection
            connection = new SqlConnection(connectionStr);
            return connection;
        }

        public static DateTime MinDbDate
        {
            get
            {
                return minDbDate;
            }
        }
    }
}
