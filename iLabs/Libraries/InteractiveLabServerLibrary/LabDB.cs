using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.Services;

//using iLabs.LabServer.LabView;



namespace iLabs.LabServer.Interactive
{
	/// <summary>
	/// Summary description for LabDB.
	/// </summary>
	public class LabDB : ProcessAgentDB
	{
		public LabDB()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        //public void ProcessExpiredTasks()
        //{
        //}
/*
		public LabAppInfo GetVIforGroup(long groupID)
		{
			return new LabAppInfo();
		}
         public int InsertLabApp(LabAppInfo app)
        {
*/

        public int InsertLabApp(LabAppInfo app)
        {
            return InsertLabApp(app.title, app.appGuid,app.version, app.appKey,
                app.path, app.application, app.page, app.appURL, app.width, app.height,
                app.dataSources, app.server, app.port,
                app.contact, app.description, app.comment, app.extraInfo,
                app.rev, app.type);
        }

        /*
@application varchar (100),
@appKey varchar (100),
@path varchar (256),
@version varchar (50),
@rev varchar (50),
@page varchar (256),
@title varchar (256),
@description varchar (2000),
@comment varchar (256),
@width int,
@height int,
@type int,
@server varchar (256),
@port int,
@contact varchar (256),
@cgi varchar (256),
@datasource varchar (2000),
@extra nvarchar (2000)
*/
        public int InsertLabApp(string title,string appGuid, string version, string appKey,
            string path, string application, string page,string cgi,int width, int height,
            string datasource,  string server,int port,
            string contact,string description, string comment, string extra,
            string rev, int type)
            
        {
            int id = -1;
            SqlConnection connection = CreateConnection();
            try
            {
                // create sql command
                SqlCommand cmd = new SqlCommand("InsertLabApp", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter Param = null;
                Param = cmd.Parameters.Add("@title", SqlDbType.VarChar, 256);
                Param.Value = title;
                Param = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                Param.Value = appGuid;
                Param = cmd.Parameters.Add("@version", SqlDbType.VarChar, 50);
                if (version != null && version.Length > 0)
                    Param.Value = version;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@appKey", SqlDbType.VarChar, 100);
                if (appKey != null && appKey.Length > 0)
                    Param.Value = appKey;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@path", SqlDbType.VarChar, 256);
                Param.Value = path;
                Param = cmd.Parameters.Add("@application", SqlDbType.VarChar, 100);
                Param.Value = application;
                Param = cmd.Parameters.Add("@page", SqlDbType.VarChar, 256);
                if (page != null && page.Length > 0)
                    Param.Value = page;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@cgi", SqlDbType.VarChar, 256);
                if (cgi != null && cgi.Length > 0)
                    Param.Value = cgi;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@width", SqlDbType.Int);
                Param.Value = width;
                Param = cmd.Parameters.Add("@height", SqlDbType.Int);
                Param.Value = height;
                Param = cmd.Parameters.Add("@datasource", SqlDbType.VarChar, 2000);
                if (datasource != null && datasource.Length > 0)
                    Param.Value = datasource;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@server", SqlDbType.VarChar, 256);
                if (server != null && server.Length > 0)
                    Param.Value = server;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@port", SqlDbType.Int);
                if (port > 0)
                    Param.Value = port;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@contact", SqlDbType.VarChar, 256);
                if (contact != null && contact.Length > 0)
                    Param.Value = contact;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@description", SqlDbType.VarChar, 2000);
                if (description != null && description.Length > 0)
                    Param.Value = description;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@comment", SqlDbType.VarChar, 256);
                if (comment != null && comment.Length > 0)
                    Param.Value = comment;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@extra", SqlDbType.NVarChar, 2000);
                if (extra != null && extra.Length > 0)
                    Param.Value = extra;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@rev", SqlDbType.VarChar, 50);
                if (rev != null && rev.Length > 0)
                    Param.Value = rev;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@type", SqlDbType.Int);
                Param.Value = type;


                Object obj = cmd.ExecuteScalar();
                if (obj != null)
                    id = Convert.ToInt32(obj);


            }
            catch (Exception ex)
            {
                Utilities.WriteLog(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return id;
        }

        public int ModifyLabApp(LabAppInfo app)
        {
            return ModifyLabApp(app.appID, app.title, app.appGuid, app.version, app.appKey,
                app.path, app.application, app.page, app.appURL, app.width, app.height,
                app.dataSources, app.server, app.port,
                app.contact, app.description, app.comment, app.extraInfo,
                app.rev, app.type);
        }

        public int ModifyLabApp(int appId, string title, string appGuid, string version, string appKey,
            string path, string application, string page, string cgi, int width, int height,
            string datasource, string server, int port,
            string contact, string description, string comment, string extra,
            string rev, int type)
        {
            int count = -1;
            SqlConnection connection = CreateConnection();
            try
            {
                // create sql command
                SqlCommand cmd = new SqlCommand("ModifyLabApp", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter Param = null;
                Param = cmd.Parameters.Add("@appId", SqlDbType.Int);
                Param.Value = appId;
                Param = cmd.Parameters.Add("@title", SqlDbType.VarChar, 256);
                Param.Value = title;
                Param = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                if (version != null && version.Length > 0)
                    Param.Value = appGuid;
                Param = cmd.Parameters.Add("@version", SqlDbType.VarChar, 50);
                if (version != null && version.Length > 0)
                    Param.Value = version;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@appKey", SqlDbType.VarChar, 100);
                if (appKey != null && appKey.Length > 0)
                    Param.Value = appKey;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@path", SqlDbType.VarChar, 256);
                Param.Value = path;
                Param = cmd.Parameters.Add("@application", SqlDbType.VarChar, 100);
                Param.Value = application;
                Param = cmd.Parameters.Add("@page", SqlDbType.VarChar, 256);
                if (page != null && page.Length > 0)
                    Param.Value = page;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@cgi", SqlDbType.VarChar, 256);
                if (cgi != null && cgi.Length > 0)
                    Param.Value = cgi;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@width", SqlDbType.Int);
                Param.Value = width;
                Param = cmd.Parameters.Add("@height", SqlDbType.Int);
                Param.Value = height;
                Param = cmd.Parameters.Add("@datasource", SqlDbType.VarChar, 2000);
                if (datasource != null && datasource.Length > 0)
                    Param.Value = datasource;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@server", SqlDbType.VarChar, 256);
                if (server != null && server.Length > 0)
                    Param.Value = server;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@port", SqlDbType.Int);
                if (port > 0)
                    Param.Value = port;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@contact", SqlDbType.VarChar, 256);
                if (contact != null && contact.Length > 0)
                    Param.Value = contact;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@description", SqlDbType.VarChar, 2000);
                if (description != null && description.Length > 0)
                    Param.Value = description;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@comment", SqlDbType.VarChar, 256);
                if (comment != null && comment.Length > 0)
                    Param.Value = comment;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@extra", SqlDbType.NVarChar, 2000);
                if (extra != null && extra.Length > 0)
                    Param.Value = extra;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@rev", SqlDbType.VarChar, 50);
                if (rev != null && rev.Length > 0)
                    Param.Value = rev;
                else
                    Param.Value = DBNull.Value;
                Param = cmd.Parameters.Add("@type", SqlDbType.Int);
                Param.Value = type;

                Object obj = cmd.ExecuteScalar();
                if (obj != null)
                    count = Convert.ToInt32(obj);


            }
            catch (Exception ex)
            {
                Utilities.WriteLog(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return count;
        }

        public int DeleteLabApp(int appId)
        {
            int count = -1;
              SqlConnection connection = CreateConnection();
            try
            {
                // create sql command
                SqlCommand cmd = new SqlCommand("RemoveLabApp", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter Param = cmd.Parameters.Add("@appId", SqlDbType.Int);
                Param.Value = appId;

                Object obj = cmd.ExecuteScalar();
                if (obj != null)
                    count = Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                Utilities.WriteLog(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return count;
        }


        public LabAppInfo GetLabApp(int appId)
        {
            SqlConnection connection = CreateConnection();
            LabAppInfo info = GetLabApp(connection, appId);
            connection.Close();
            return info;
        }

        public LabAppInfo GetLabApp(string appKey)
        {
            SqlConnection connection = CreateConnection();
            LabAppInfo info = GetLabApp(connection, appKey);
            connection.Close();
            return info;
        }

        public IntTag [] GetLabAppTags()
        {
            ArrayList list = new ArrayList();
            SqlConnection connection = CreateConnection();

           // create sql command
			SqlCommand cmd = new SqlCommand("GetLabAppTags", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) 
			    {
                    IntTag tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);
                    list.Add(tag);
                }
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}

			finally{
                connection.Close();
            }
            IntTag temp = new IntTag();
			return  ( IntTag[]) list.ToArray(temp.GetType());
        }

        public IntTag GetLabAppTag(int appId)
        {
            IntTag tag = null;
            SqlConnection connection = CreateConnection();
            // create sql command
			// command executes the "GetVI" stored procedure
			SqlCommand cmd = new SqlCommand("GetLabAppTag", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter Param = cmd.Parameters.Add("@appId", SqlDbType.Int);
			Param.Value = appId;
			
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
                // id of created coupon
			
			    while (dataReader.Read()) 
			    {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);
                }
			}
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}

			finally{
                connection.Close();
            }
            return tag;
        }

        public IntTag GetLabAppTag(string appKey)
        {
            IntTag tag = null;
            SqlConnection connection = CreateConnection();
           // create sql command
			// command executes the "GetVI" stored procedure
			SqlCommand cmd = new SqlCommand("GetLabAppTagByKey", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter Param = cmd.Parameters.Add("@appKey", SqlDbType.VarChar,100);
			Param.Value = appKey;
			
			SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);
                }
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
            return tag;
        }

        public LabAppInfo GetLabApp(SqlConnection connection, int appId)
        {
	
			// create sql command
			// command executes the "GetVI" stored procedure
			SqlCommand cmd = new SqlCommand("GetLabApp", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter Param = cmd.Parameters.Add("@appId", SqlDbType.Int);
			Param.Value = appId;
			
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}

			// id of created coupon
			LabAppInfo appInfo = new LabAppInfo();
			appInfo.appID = appId;
			while (dataReader.Read()) 
			{
				appInfo.appID = appId;
				appInfo.path = dataReader.GetString(0);
				appInfo.application= dataReader.GetString(1);
				appInfo.page= dataReader.GetString(2);
                if (!dataReader.IsDBNull(3))
				appInfo.title= dataReader.GetString(3);
            if (!dataReader.IsDBNull(4))
				appInfo.description= dataReader.GetString(4);
				if(!dataReader.IsDBNull(5))
					appInfo.extraInfo= dataReader.GetString(5);
                if (!dataReader.IsDBNull(6))
				appInfo.contact= dataReader.GetString(6);
            if (!dataReader.IsDBNull(7))
				appInfo.comment= dataReader.GetString(7);
				appInfo.width= dataReader.GetInt32(8);
				appInfo.height= dataReader.GetInt32(9);
				if(!dataReader.IsDBNull(10))
					appInfo.dataSources= dataReader.GetString(10);
                if (!dataReader.IsDBNull(11))
                    appInfo.server = dataReader.GetString(11);
                if (!dataReader.IsDBNull(12))
                    appInfo.port = dataReader.GetInt32(12);
                if (!dataReader.IsDBNull(13))
                    appInfo.appURL = dataReader.GetString(13);
                if (!dataReader.IsDBNull(14))
                    appInfo.version = dataReader.GetString(14);
                if (!dataReader.IsDBNull(15))
                    appInfo.rev = dataReader.GetString(15);
                if (!dataReader.IsDBNull(16))
                    appInfo.appKey = dataReader.GetString(16);
                if (!dataReader.IsDBNull(17))
                    appInfo.appGuid = dataReader.GetString(17);
			}

			
	return appInfo;
	}

    public LabAppInfo GetLabApp(SqlConnection connection, string appKey)
        {
            LabAppInfo appInfo = null;

            // create sql command
            // command executes the "GetVI" stored procedure
            SqlCommand cmd = new SqlCommand("GetLabAppByKey", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter Param = cmd.Parameters.Add("@appKey", SqlDbType.VarChar,100);
            Param.Value = appKey;

            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }

            // id of created coupon
           
            while (dataReader.Read())
            {
                appInfo = new LabAppInfo();
                appInfo.appID = dataReader.GetInt32(0);
                appInfo.path = dataReader.GetString(1);
                appInfo.application = dataReader.GetString(2);
                appInfo.page = dataReader.GetString(3);
                if (!dataReader.IsDBNull(4))
                    appInfo.title = dataReader.GetString(4);
                if (!dataReader.IsDBNull(5))
                    appInfo.description = dataReader.GetString(5);
                if (!dataReader.IsDBNull(6))
                    appInfo.extraInfo = dataReader.GetString(6);
                if (!dataReader.IsDBNull(7))
                    appInfo.contact = dataReader.GetString(7);
                if (!dataReader.IsDBNull(8))
                    appInfo.comment = dataReader.GetString(8);
                appInfo.width = dataReader.GetInt32(9);
                appInfo.height = dataReader.GetInt32(10);
                if (!dataReader.IsDBNull(11))
                    appInfo.dataSources = dataReader.GetString(11);
                if (!dataReader.IsDBNull(12))
                    appInfo.server = dataReader.GetString(12);
                if (!dataReader.IsDBNull(13))
                    appInfo.port = dataReader.GetInt32(13);
                if (!dataReader.IsDBNull(14))
                    appInfo.appURL = dataReader.GetString(14);
                if (!dataReader.IsDBNull(15))
                    appInfo.version = dataReader.GetString(15);
                if (!dataReader.IsDBNull(16))
                    appInfo.rev = dataReader.GetString(16);
                if (!dataReader.IsDBNull(17))
                    appInfo.appKey = dataReader.GetString(17);
                if (!dataReader.IsDBNull(18))
                    appInfo.appGuid = dataReader.GetString(18);
            }
            return appInfo;
        }

        public LabAppInfo[] GetLabApps()
        {
            SqlConnection connection = CreateConnection();
            LabAppInfo[] labs = GetLabApps(connection);
            connection.Close();
            return labs;
        }

        public LabAppInfo[] GetLabApps(SqlConnection connection)
        {

            // create sql command
            SqlCommand cmd = new SqlCommand("GetLabApps", connection);
            cmd.CommandType = CommandType.StoredProcedure;

         
            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }

            // id of created coupon
            
           ArrayList list = new ArrayList();
            while (dataReader.Read())
            {
                LabAppInfo viInfo = new LabAppInfo();
                viInfo.appID = dataReader.GetInt32(0);
                viInfo.path = dataReader.GetString(1);
                viInfo.application = dataReader.GetString(2);
                viInfo.page = dataReader.GetString(3);
                if (!dataReader.IsDBNull(4))
                    viInfo.title = dataReader.GetString(4);
                if (!dataReader.IsDBNull(5))
                    viInfo.description = dataReader.GetString(5);
                if (!dataReader.IsDBNull(6))
                    viInfo.extraInfo = dataReader.GetString(6);
                if (!dataReader.IsDBNull(7))
                    viInfo.contact = dataReader.GetString(7);
                if (!dataReader.IsDBNull(8))
                    viInfo.comment = dataReader.GetString(8);
                viInfo.width = dataReader.GetInt32(9);
                viInfo.height = dataReader.GetInt32(10);
                if (!dataReader.IsDBNull(11))
                    viInfo.dataSources = dataReader.GetString(11);
                if (!dataReader.IsDBNull(12))
                    viInfo.server = dataReader.GetString(12);
                if (!dataReader.IsDBNull(13))
                    viInfo.port = dataReader.GetInt32(13);
                if (!dataReader.IsDBNull(14))
                    viInfo.appURL = dataReader.GetString(14);
                if (!dataReader.IsDBNull(15))
                    viInfo.version = dataReader.GetString(15);
                if (!dataReader.IsDBNull(16))
                    viInfo.rev = dataReader.GetString(16);
                if (!dataReader.IsDBNull(17))
                    viInfo.appKey = dataReader.GetString(17);
                if (!dataReader.IsDBNull(18))
                    viInfo.appGuid = dataReader.GetString(18);
                list.Add(viInfo);
            }
            LabAppInfo[] labs = new LabAppInfo[list.Count];
            for(int i= 0;i<list.Count;i++){
                labs[i] = ( LabAppInfo) list[i];
            }
            return labs;

            
        }
 
        public LabAppInfo GetLabAppForGroup(string groupName, string serviceGUID)
		{
			SqlConnection connection =  CreateConnection();
			// create sql command
            SqlCommand cmd = new SqlCommand("GetLabAppByGroup", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,256);
			idParam.Value = groupName;
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);
            guidParam.Value = serviceGUID;

          
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}

			LabAppInfo viInfo = new LabAppInfo();
			while (dataReader.Read()) 
			{
				viInfo.appID = dataReader.GetInt32(0);
                viInfo.path = dataReader.GetString(1);
                viInfo.application = dataReader.GetString(2);
                if (!dataReader.IsDBNull(3))
                    viInfo.page = dataReader.GetString(3);
                if (!dataReader.IsDBNull(4))
                    viInfo.title = dataReader.GetString(4);
                if (!dataReader.IsDBNull(5))
                    viInfo.description = dataReader.GetString(4);
                if (!dataReader.IsDBNull(6))
                    viInfo.extraInfo = dataReader.GetString(6);
                if (!dataReader.IsDBNull(7))
                    viInfo.contact = dataReader.GetString(7);
                if (!dataReader.IsDBNull(8))
                    viInfo.comment = dataReader.GetString(8);
                viInfo.width = dataReader.GetInt32(9);
                viInfo.height = dataReader.GetInt32(10);
                if (!dataReader.IsDBNull(11))
                    viInfo.dataSources = dataReader.GetString(11);
                if (!dataReader.IsDBNull(12))
                    viInfo.server = dataReader.GetString(12);
                if (!dataReader.IsDBNull(13))
                    viInfo.port = dataReader.GetInt32(13);
                if (!dataReader.IsDBNull(14))
                    viInfo.appURL = dataReader.GetString(14);
                if (!dataReader.IsDBNull(15))
                    viInfo.version = dataReader.GetString(15);
                if (!dataReader.IsDBNull(16))
                    viInfo.rev = dataReader.GetString(16);
                if (!dataReader.IsDBNull(17))
                    viInfo.appKey = dataReader.GetString(17);
                if (!dataReader.IsDBNull(18))
                    viInfo.appGuid = dataReader.GetString(18);
			}

			// close the sql connection
			connection.Close();
			
			return viInfo;
		}

        public LabTask InsertTask(int app_id, long exp_id, string groupName, DateTime startTime, long duration, LabTask.eStatus status,
			long coupon_ID,string issuerGuidStr, string data)
		{
            LabTask task = new LabTask();

			SqlConnection connection =  CreateConnection();
			// create sql command
			SqlCommand cmd = new SqlCommand("InsertTask", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@appid", SqlDbType.Int);
			idParam.Value = app_id;
            SqlParameter expidParam = cmd.Parameters.Add("@expid", SqlDbType.Int);
            if(exp_id < 1)
                expidParam.Value = DBNull.Value;
            else
                expidParam.Value = exp_id;
			SqlParameter groupParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,128);
			groupParam.Value = groupName;
			SqlParameter startParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
            // This must be in UTC
			startParam.Value = startTime;
			SqlParameter endParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
            if (duration > 0)
                endParam.Value = startTime.AddTicks(duration * TimeSpan.TicksPerSecond);
            else
                endParam.Value = DateTime.MinValue;
			SqlParameter statusParam = cmd.Parameters.Add("@status", SqlDbType.Int);
			statusParam.Value = (int) status;
			SqlParameter couponParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponParam.Value = coupon_ID;
			SqlParameter guidParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.VarChar,50);
			guidParam.Value = issuerGuidStr;
			SqlParameter dataParam = cmd.Parameters.Add("@data", SqlDbType.NVarChar,2000);
			if(data == null)
                dataParam.Value = DBNull.Value;
			else
				dataParam.Value = data;
           
            // id of created task
            long itemID = -1;	
			
			try 
			{
				itemID = Convert.ToInt64(cmd.ExecuteScalar());
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}
            task.taskID = itemID;
            task.labAppID = app_id;
            task.experimentID = exp_id;
            task.groupName = groupName;
            task.startTime = startTime;
            if (duration > 0)
                task.endTime = startTime.AddTicks(duration * TimeSpan.TicksPerSecond);
            else
                task.endTime = DateTime.MinValue;
            task.Status = status;
            task.couponID = coupon_ID;
            task.issuerGUID = issuerGuidStr;
            task.data = data;
            return task;
		}



		public LabTask GetTask(long task_id)
		{
			SqlConnection connection =  CreateConnection();
			// create sql command
			SqlCommand cmd = new SqlCommand("GetTask", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter Param = cmd.Parameters.Add("@taskid", SqlDbType.BigInt);
			Param.Value = task_id;
			
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}

			// id of created coupon
            LabTask taskInfo = new LabTask();
			taskInfo.taskID = task_id;
			while (dataReader.Read()) 
			{
				taskInfo.labAppID = dataReader.GetInt32(0);
                if (!dataReader.IsDBNull(1))
                taskInfo.experimentID = dataReader.GetInt64(1);
				taskInfo.groupName = dataReader.GetString(2);
				taskInfo.startTime= dataReader.GetDateTime(3);
				taskInfo.endTime = dataReader.GetDateTime(4);				
				taskInfo.Status= (LabTask.eStatus) dataReader.GetInt32(5);
				if(!DBNull.Value.Equals(dataReader.GetValue(6)))
				    taskInfo.couponID= dataReader.GetInt64(6);
				if(!DBNull.Value.Equals(dataReader.GetValue(7)))
				    taskInfo.issuerGUID= dataReader.GetString(7);
				if(!dataReader.IsDBNull(8))
				    taskInfo.data= dataReader.GetString(8);
			}

			// close the sql connection
			connection.Close();
			
			return taskInfo;
		}

        public LabTask GetTask(long experiment_id, string sbGUID)
        {
            SqlConnection connection = CreateConnection();
            // create sql command
            SqlCommand cmd = new SqlCommand("GetTaskByExperiment", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter param = cmd.Parameters.Add("@experimentid", SqlDbType.BigInt);
            param.Value = experiment_id;
            SqlParameter paramSB = cmd.Parameters.Add("@sbguid", SqlDbType.VarChar,50);
            paramSB.Value = sbGUID;

            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw e;
            }

            // id of created coupon
            LabTask taskInfo = new LabTask();
            while (dataReader.Read())
            {
                taskInfo.taskID = dataReader.GetInt64(0);
                taskInfo.labAppID = dataReader.GetInt32(1);
                if (!dataReader.IsDBNull(2))
                    taskInfo.experimentID = dataReader.GetInt64(2);
                taskInfo.groupName = dataReader.GetString(3);
                taskInfo.startTime = dataReader.GetDateTime(4);
                taskInfo.endTime = dataReader.GetDateTime(5);
                taskInfo.Status = (LabTask.eStatus)dataReader.GetInt32(6);
                if (!DBNull.Value.Equals(dataReader.GetValue(7)))
                    taskInfo.couponID = dataReader.GetInt64(7);
                if (!DBNull.Value.Equals(dataReader.GetValue(8)))
                    taskInfo.issuerGUID = dataReader.GetString(8);
                if (!dataReader.IsDBNull(9))
                    taskInfo.data = dataReader.GetString(9);
            }

            // close the sql connection
            connection.Close();

            return taskInfo;
        }


/*
CREATE PROCEDURE GetActiveTasks

AS
select taskID,Status 
*/
		public LabTask [] GetActiveTasks()
		{
			SqlConnection connection =  CreateConnection();
			// create sql command
			SqlCommand cmd = new SqlCommand("GetActiveTasks", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}

			// id of created coupon
			
			ArrayList list = new ArrayList();
			while (dataReader.Read()) 
			{
				LabTask taskInfo = new LabTask();
				taskInfo.taskID = dataReader.GetInt64(0);
                taskInfo.labAppID = dataReader.GetInt32(1);
                if (!dataReader.IsDBNull(2))
				    taskInfo.experimentID = dataReader.GetInt64(2);
                if (!dataReader.IsDBNull(3))
				    taskInfo.groupName = dataReader.GetString(3);
				taskInfo.startTime= dataReader.GetDateTime(4);
                taskInfo.endTime = dataReader.GetDateTime(5);			
				taskInfo.Status= (LabTask.eStatus) dataReader.GetInt32(6);
				if(!DBNull.Value.Equals(dataReader.GetValue(7)))
				    taskInfo.couponID= dataReader.GetInt64(7);
				if(!DBNull.Value.Equals(dataReader.GetValue(8)))
				    taskInfo.issuerGUID= dataReader.GetString(8);
				if(!dataReader.IsDBNull(9))
				    taskInfo.data= dataReader.GetString(9);
				list.Add(taskInfo);
			}

			// close the sql connection
			connection.Close();
			LabTask taskInfoTemp = new LabTask();
			return  (LabTask[]) list.ToArray(taskInfoTemp.GetType());
		}

/*
CREATE PROCEDURE GetExpiredTasks
@targetTime datetime

AS
select taskID,VIID,GroupID,StartTime,endTime,Status,CouponID,IssuerID,Data 
*/
		public LabTask [] GetExpiredTasks(DateTime targetTime)
		{
			SqlConnection connection =  CreateConnection();
			// create sql command
			SqlCommand cmd = new SqlCommand("GetExpiredTasks", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter Param = cmd.Parameters.Add("@targetTime", SqlDbType.DateTime);
			Param.Value = targetTime;
			
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}

			// id of created coupon
			
			ArrayList list = new ArrayList();
			while (dataReader.Read()) 
			{
				LabTask taskInfo = new LabTask();
				taskInfo.taskID = dataReader.GetInt64(0);
				taskInfo.labAppID = dataReader.GetInt32(1);
                if (!dataReader.IsDBNull(2))
                taskInfo.experimentID = dataReader.GetInt64(2);
				taskInfo.groupName = dataReader.GetString(3);
				taskInfo.startTime= dataReader.GetDateTime(4);
				taskInfo.endTime= dataReader.GetDateTime(5);
                taskInfo.Status = (LabTask.eStatus)dataReader.GetInt32(6);
				if(!DBNull.Value.Equals(dataReader.GetValue(7)))
				    taskInfo.couponID= dataReader.GetInt64(7);
				if(!dataReader.IsDBNull(8))
				    taskInfo.issuerGUID= dataReader.GetString(8);
				if(!dataReader.IsDBNull(9))
				    taskInfo.data= dataReader.GetString(9);
				list.Add(taskInfo);
			}

			// close the sql connection
			connection.Close();
			LabTask taskInfoTemp = new LabTask();
			return  (LabTask[]) list.ToArray(taskInfoTemp.GetType());
		}

		public void SetTaskData(long task_id,string data)
		{
			SqlConnection connection =  CreateConnection();
			// create sql command
			SqlCommand cmd = new SqlCommand("SetTaskData", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@taskid", SqlDbType.BigInt);
			idParam.Value = task_id;
			SqlParameter dataParam = cmd.Parameters.Add("@data", SqlDbType.NVarChar,2000);
			if(data == null)
				dataParam.Value = DBNull.Value;
			else
				dataParam.Value = data;
				
			try 
			{
				cmd.ExecuteNonQuery();
			
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}
		}



/*
SetTaskStatus taskID, status
AS
update task set status = @status where taskID = @taskID

*/
		public void SetTaskStatus(long task_id, int status)
		{
			SqlConnection connection =  CreateConnection();
			// create sql command
			SqlCommand cmd = new SqlCommand("SetTaskStatus", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@taskID", SqlDbType.BigInt);
			idParam.Value = task_id;
			SqlParameter statusParam = cmd.Parameters.Add("@status", SqlDbType.Int);
			statusParam.Value = status;
			
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw e;
			}
		}
        /*
        // Should move this into the TaskProcessor and have LabVIEW specific methods 
        // happen outside of the database class.
		int count = 0;
		public void ProcessTasks()
		{
			count++;
			if(count >= 10)
			{
				Utilities.WriteLog("ProcessTasks");
				count = 0;
			}
			
			LabTask[] tasks = GetActiveTasks();
			//LabViewInterface lvi = new LabViewInterface();
			DateTime time = DateTime.UtcNow;
			foreach(LabTask task in tasks)
			{
				
				if(time.CompareTo(task.endTime) > 0)
				{ // task has expired
					try
					{

                        Utilities.WriteLog("Found expired task: " + task.taskID);
					
						if(task.data != null)
						{
							XmlQueryDoc taskDoc = new XmlQueryDoc(task.data);
							String vi = taskDoc.Query("task/viname");
							string status = taskDoc.Query("task/statusvi");
							if(status != null)
							{
								try
								{
									//lvi.DisplayStatus(status,"You are out of time!","0:00");
									//lvi.submitAction("stopvi",status);
								}
								catch(Exception ce)
								{
                                    Utilities.WriteLog("Trying Status: " + ce.Message);
								}
							}
							//lvi.submitAction("lockvi",vi);
							//lvi.submitAction("stopvi",vi);
						}
                        Utilities.WriteLog("TaskID = " + task.taskID + " has expired");
						SetTaskStatus(task.taskID,99);
						Coupon expCoupon = this.GetCoupon(task.couponID,task.issuerGUID);
						Coupon identCoupon = this.GetIdentityInCoupon (task.issuerGUID);

						ProcessAgentInfo issuer = GetProcessAgentInfo(task.issuerGUID);
						if(issuer == null)
						{
							//Response.Redirect("AccessDenied.aspx?text=the+specified+ticket+issuer+could+not+be+found.", true);
						}
				
						// Create ticketing service interface connection to TicketService
						ITicketIssuer_Proxy ticketingInterface = new ITicketIssuer_Proxy();
						ticketingInterface.AgentAuthHeaderValue.coupon = identCoupon;
                        ticketingInterface.AgentAuthHeaderValue.agentGuid = ConfigurationManager.AppSettings["ServiceGuid"];
						ticketingInterface.Url = issuer.webServiceUrl;
						if(ticketingInterface.RequestTicketCancellation(expCoupon,TicketTypes.EXECUTE_EXPERIMENT,ConfigurationManager.AppSettings["ServiceGuid"]))
						{
							// Or should this be cancelled from the TicketIssuer
							CancelTicket(expCoupon,TicketTypes.EXECUTE_EXPERIMENT,ConfigurationManager.AppSettings["ServiceGuid"]);
			
						}
						else
						{
                            Utilities.WriteLog("Unable to cancel ticket: " + expCoupon.couponId);
						}
					}
				
					catch(Exception e1)
					{
                        Utilities.WriteLog("ProcessTasks Expired: " + e1.Message);
					}
				}
				else
				{
					try
					{
						if(task.Status == LabTask.eStatus.Running)
						{
							if(task.data != null)
							{
								XmlQueryDoc taskDoc = new XmlQueryDoc(task.data);
								string status = taskDoc.Query("task/statusvi");
								if(status != null)
								{
									try
									{
										//lvi.DisplayStatus(status,time.ToString() , task.endTime.Subtract(time).ToString());
									}
									catch(Exception ce2)
									{
                                        Utilities.WriteLog("Status: " + ce2.Message);
									}
								}
							}
						}
					}
					catch(Exception e2)
					{
                        Utilities.WriteLog("ProcessTasks Status: " + e2.Message);
					}
				}
			}
		}
*/
        public LabTask.eStatus ExperimentStatus(long id, string issuer)
        {
            LabTask.eStatus status = LabTask.eStatus.NotFound;
            int result = -10;
            SqlConnection connection = CreateConnection();
            // create sql command
            // command executes the "GetExperimentStatus" stored procedure
            SqlCommand cmd = new SqlCommand("GetTaskStatusByExperiment", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter ParamID = cmd.Parameters.Add("@id", SqlDbType.BigInt);
            ParamID.Value = id;
            SqlParameter ParamGUID = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            ParamGUID.Value = issuer;

            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
            int count = 0;
            while (dataReader.Read())
            {
                status = (LabTask.eStatus)dataReader.GetInt32(0);
                count++;
            }
            Utilities.WriteLog("ExperimentStatus count: " + count);
            
            return status;
        }


	}
}
