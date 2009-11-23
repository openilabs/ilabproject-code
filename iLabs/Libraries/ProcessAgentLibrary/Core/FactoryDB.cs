using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace iLabs.Core
{
    public static class FactoryDB
    {
        static private string connectionStr;
        static private string providerStr;
        static private DateTime minDbDate;
        static private DateTime maxDbDate;
        static private DbProviderFactory theFactory;
        static FactoryDB(){
            minDbDate = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            maxDbDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            // read connection string from the app settings
            connectionStr = ConfigurationManager.AppSettings["sqlConnection"];
            if (connectionStr == null || connectionStr.Equals(""))
            {
                throw new NoNullAllowedException(" The connection string is not specified, check configuration");
            }
            providerStr = ConfigurationManager.AppSettings["databaseProvider"];
            if (providerStr == null || providerStr.Equals(""))
            {
                throw new NoNullAllowedException(" The database provider is not specified, check configuration");
            }
            theFactory = DbProviderFactories.GetFactory(providerStr);


        }

        /// <summary>
        /// Creates an unopened connection to the database, should return a DbConnection.
        /// </summary>
        /// <returns></returns>
        public static DbConnection GetConnection()
        {
            DbConnection connection = null;
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
            // create an DbConnection
            connection = theFactory.CreateConnection();
            connection.ConnectionString =connectionStr;
            return connection;
        }

        public static DbCommand CreateCommand(string text, DbConnection connection)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = text;
            return cmd;
        }

        public static DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public static DbParameter CreateParameter(DbCommand cmd, string name, DbType type)
        {
            DbParameter param = cmd.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            return param;
        }
        public static DbParameter CreateParameter(DbCommand cmd, string name, DbType type, int max)
        {
            DbParameter param = cmd.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Size = max;
            return param;
        }
        public static DbParameter CreateParameter(DbCommand cmd, string name, object value, DbType type)
        {
            DbParameter param = cmd.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            if (type == DbType.DateTime)
            {
                if (value == null || ((DateTime)value) < minDbDate)
                {
                    param.Value = minDbDate;
                }
                else if (((DateTime)value) > maxDbDate)
                {
                    param.Value = maxDbDate;
                }
                else
                {
                    param.Value = value;
                }
            }
            else
            {
                if (value == null || value == System.DBNull.Value)
                    param.Value = System.DBNull.Value;
                else
                {
                    if ((value is string) && (((string)value).Length == 0))
                    {
                        param.Value = System.DBNull.Value;
                    }
                    else
                    {
                        param.Value = value;
                    }
                }
            }
            return param;
        }

        public static DbParameter CreateParameter(DbCommand cmd, string name, object value, DbType type, int size)
        {
            DbParameter param = CreateParameter(cmd, name, value, type);
            param.Size = size;
            return param;
        }

        public static DateTime MinDbDate
        {
            get
            {
                return minDbDate;
            }
        }
        public static DateTime MaxDbDate
        {
            get
            {
                return maxDbDate;
            }
        }
    }
}
