using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.BatchTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;

using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.DataStorage;
using iLabs.ServiceBroker;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.Proxies.ESS;


namespace iLabs.ServiceBroker.Internal
{
    public class InternalDataDB
    {
        public static DateTime MinSqlDateTime = new DateTime(1800, 1, 1);

        /// <summary>
        /// Closes an Experiment in the ServiceBroker database
        /// </summary>
        /// <param name="experimentID">the ID of the Experiment to be closed</param>
        /// <returns>true if the Experiment was successfully deleted</returns>
        public static bool CloseExperiment(long experimentID, int status)
        {
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("CloseExperiment", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@status", status));

            try
            {
                myConnection.Open();
                int rows = myCommand.ExecuteNonQuery();

                return (rows != -1);

                //alternatively
                //return true;
            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception("Exception thrown deleting experiment", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }


        /// <summary>
        /// deletes an Experiment object and all its associated ExperimentRecords and BLOBs on the ESS
        /// </summary>
        /// <param name="experimentID">the ID of the Experiment to be deleted</param>
        /// <returns>true if the Experiment was successfully deleted</returns>
        public static bool DeleteExperiment(long experimentID)
        {
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("DeleteExperiment", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));

            try
            {
                myConnection.Open();
                int rows = myCommand.ExecuteNonQuery();

                return (rows != -1);

                //alternatively
                //return true;
            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception("Exception thrown deleting experiment", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }
  
        
        /// <summary>
        /// Returns the current
        /// </summary>
        /// <param name="experimentID"></param>
        /// <returns></returns>
        public Coupon GetExperimentCoupon(long experimentID)
        {
            Coupon expCoupon = null;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetriveExperimentCoupon", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            try
            {
                myConnection.Open();
                SqlDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                   
                    bool cancelled = myReader.GetBoolean(0);
                    if (cancelled)
                        return null;
                    expCoupon = new Coupon();
                    expCoupon.couponId = myReader.GetInt64(1);
                    expCoupon.passkey = myReader.GetString(2);
                    expCoupon.issuerGuid = ConfigurationSettings.AppSettings["serviceGUID"];

                }
            }
            catch(SqlException ex) {
                throw new Exception("Exception thrown retrieving experiment coupon", ex);
            }
            finally
            {
                myConnection.Close();
            }
            return expCoupon;
        }
        
        /// <summary>
        /// Creates the Experiment_information record on the ServiceBroker
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userid"></param>
        /// <param name="groupid"></param>
        /// <param name="ls_id"></param>
        /// <param name="client_id"></param>
        /// <param name="ess_id"></param>
        /// <param name="duration">Time in seconds that the Experiment will be available, normally -1 ?</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>the experiments ID</returns>
        public static long InsertExperiment(int status, int userid, int groupid,
            int ls_id, int client_id, int essID, DateTime start, long duration)
        {
            long experimentID = -1;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("CreateExperiment", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@status", status));
            myCommand.Parameters.Add(new SqlParameter("@user", userid));
            myCommand.Parameters.Add(new SqlParameter("@group", groupid));
            myCommand.Parameters.Add(new SqlParameter("@ls", ls_id));
            myCommand.Parameters.Add(new SqlParameter("@client", client_id));
            SqlParameter essParam = myCommand.Parameters.Add("@ess", SqlDbType.Int);
            if (essID > 0)
                essParam.Value = essID;
            else
                essParam.Value = DBNull.Value;
            SqlParameter startParam = myCommand.Parameters.Add("@start", SqlDbType.DateTime);
            if (start != null)
                startParam.Value = start;
            else
                startParam.Value = DBNull.Value;
            SqlParameter durationParam = myCommand.Parameters.Add("@duration", SqlDbType.BigInt);
            durationParam.Value = duration;

            try
            {
                myConnection.Open();
                experimentID = Convert.ToInt64(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown creating experiment", ex);
            }
            finally
            {
                myConnection.Close();
            }

            
            return experimentID;
        }
        /// <summary>
        /// Saves the record on the experiment's ESS
        /// </summary>
        /// <param name="experimentID"></param>
        /// <param name="submitter"></param>
        /// <param name="type"></param>
        /// <param name="contents"></param>
        /// <param name="xmlSearchable"></param>
        /// <param name="attributes"></param>
        /// <returns>experiment Sequence number or -1 if the record may not be saved</returns>
        public static int SaveExperimentRecord(long experimentID, string submitter,
            string type, bool xmlSearchable, string contents, RecordAttribute[] attributes)
        {
            int seqNum = -1;
            ExperimentStorageProxy essProxy = GetEssProxy(experimentID);
            if (essProxy != null)
            {
                seqNum = essProxy.AddRecord(experimentID, submitter, type, xmlSearchable,
                        contents, attributes);
            }
            return seqNum;
        }

        //public static ExperimentRecord RetrieveExperimentRecord(long experimentID, int sequenceNum)
        //{
        //    ExperimentRecord record = null;
        //    ExperimentStorageProxy essProxy = GetEssProxy(experimentID);
           
        //    if (essProxy != null)
        //    {
        //        record = essProxy.GetRecord(experimentID, sequenceNum);
        //    }
        //    return record;
        //}

        //public static ExperimentRecord[] RetrieveExperimentRecords(long experimentID, Criterion[] criteria)
        //{
        //    ExperimentRecord[] records = null;
        //    ExperimentStorageProxy essProxy = GetEssProxy(experimentID);

        //    if (essProxy != null)
        //    {
        //        records = essProxy.GetRecords(experimentID, criteria);
        //    }
        //    return records;
        //}

        //public static ExperimentRecord[] RetrieveExperimentRecords(long experimentID,int essID, Criterion[] criteria)
        //{
        //    ExperimentRecord [] records = null;

        //    // This operation should happen within the Wrapper
        //    BrokerDB ticketIssuer = new BrokerDB();
        //    ProcessAgentInfo ess = ticketIssuer.GetProcessAgentInfo(essID);
        //    Coupon opCoupon = null;

        //    long[] couponIDs = RetrieveExperimentCouponIDs(experimentID);
        //    if (couponIDs != null && couponIDs.Length >= 0)
        //    {   // An experiment ticket collection exists, try and find an active
        //        // Retrieve_Records ticket
        //        for (int i = 0; i < couponIDs.Length; i++)
        //        {
        //            Coupon tmpCoupon = ticketIssuer.GetIssuedCoupon(couponIDs[i]);
        //            Ticket ticket = ticketIssuer.RetrieveTicket(tmpCoupon, TicketTypes.RETRIEVE_RECORDS);
        //            if (ticket != null && !(ticket.SecondsToExpire() > 60))
        //            {
                        
        //                opCoupon = tmpCoupon;
        //                break;
        //            }
        //        }
        //    }
        //    if (opCoupon == null)
        //    {
        //        TicketLoadFactory factory = TicketLoadFactory.Instance();
        //        string payload = factory.RetrieveRecordsPayload(experimentID, ess.webServiceUrl);
        //        // Create a ticket to read records
        //        opCoupon = ticketIssuer.CreateTicket(TicketTypes.RETRIEVE_RECORDS, ess.agentGuid, ticketIssuer.GetServiceBrokerInfo().agentGuid, 600, payload);
        //        InternalDataDB.InsertExperimentCoupon(experimentID, opCoupon.couponId);
        //    }
        //    ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
        //    OperationAuthHeader header = new OperationAuthHeader();
        //    header.coupon = opCoupon;
        //    essProxy.Url = ess.webServiceUrl;
        //    essProxy.OperationAuthHeaderValue = header;
            
        //    records = essProxy.GetRecords(experimentID, criteria);
        //    return records;
        //}
/*
        /// <summary>
        /// to record an experiment specification
        /// </summary>
        public static void SaveExperimentInformation(long experimentID, int userID, int effectiveGroupID, int labServerID, string annotation)
        {
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SaveExperimentInformation", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            //SQL Server doesn't throw an error if we try to update a row that doesn't exist. Perhaps a better way to check this would be to see if the row 
            // exists(by calling retrieve experiment spec. from where you're calling it and then proceed if not null.
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@userID", userID));
            myCommand.Parameters.Add(new SqlParameter("@effectiveGroupID", effectiveGroupID));
            myCommand.Parameters.Add(new SqlParameter("@labServerID", labServerID));
            myCommand.Parameters.Add(new SqlParameter("@annotation", annotation));

            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown saving experiment information", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }
*/
        /// <summary>
        /// saves or modifies an optional user defined annotation to the experiment record
        /// </summary>
        public static string SaveExperimentAnnotation(long experimentID, string annotation)
        {
            string previousAnnotation = null;
            previousAnnotation = SelectExperimentAnnotation(experimentID);
            if (previousAnnotation != annotation)
            {
                SqlConnection myConnection = ProcessAgentDB.GetConnection();
                SqlCommand myCommand = new SqlCommand("SaveExperimentAnnotation", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
                myCommand.Parameters.Add(new SqlParameter("@annotation", annotation));
                try
                {
                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception thrown SaveExperimentAnnotation", ex);
                }
                finally
                {
                    myConnection.Close();
                }
            }
            return previousAnnotation;
        }

        /// <summary>
        /// to retrieve a previously saved experiment annotation
        /// </summary>
        public static string SelectExperimentAnnotation(long experimentID)
        {
            string annotation = null;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentAnnotation", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            try
            {
                myConnection.Open();
                annotation = myCommand.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown retrieving experiment annotation. " + ex.Message, ex);
            }
            finally
            {
                myConnection.Close();
            }

            return annotation;
        }

        /// <summary>
        /// to retrieve the owner (currently a user) of an experiment
        /// </summary>
        public static int SelectExperimentOwner(long experimentID)
        {
            int userID = -1;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentOwner", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            try
            {
                myConnection.Open();
                userID = Convert.ToInt32(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown retrieving experiment owner", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return userID;
        }

        /// <summary>
        /// to retrieve the effective group an experiment was run under
        /// </summary>
        public static int SelectExperimentGroup(long experimentID)
        {
            int groupID = -1;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentGroup", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            try
            {
                myConnection.Open();
                groupID = Convert.ToInt32(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown retrieving experiment group", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return groupID;
        }

        /// <summary>
        /// to change the owner of an experiment.
        /// </summary>
        public static void UpdateExperimentOwner(long experimentID, int newUserID)
        {
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("ModifyExperimentOwner", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@newUserID", newUserID));

            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown ModifyExperimentOwner", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /*
		/// <summary>
		/// to delete information about an experiment (such as owner, submitted time etc.)
		/// </summary>
		public static void DeleteExperimentInformation (long experimentID)
		{
			SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand("DeleteExperiment", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));

			try 
			{
				myConnection.Open();
				myCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown Deleting Experiment Information",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}
        */
        /// <summary>
        /// deletes all record of the experiments specified by the array of experiment IDs
        /// </summary>
        /// <param name="experimentIDs">an array that identifies the experiments to be deleted</param>
        /// <returns>an array containing the subset of the specified experiment IDs for which the delete operation failed</returns>

        public static long[] DeleteExperiments(long[] experimentIDs)
        {
            /*
            * Note : Alternately ADO.NET could be used. However, the disconnected DataAdapter object might prove
            * extremely inefficient and hence this method was chosen
            */

            ArrayList arrayList = new ArrayList();

            try
            {
                // this is very inefficient and cannot have a transaction

                foreach (long experimentID in experimentIDs)
                {
                    int qualID = Authorization.AuthorizationAPI.GetQualifierID((int)experimentID, Qualifier.experimentQualifierTypeID);
                    if (qualID > 0)
                        Authorization.AuthorizationAPI.RemoveQualifiers(new int[] { qualID });
                    bool deleted = DeleteExperiment(experimentID);

                    // Deleting from table Experiments
                    /*  IMPORTANT ! - The database if currently set to Cascade delete, where deleting an experiment will automatically
                    *  delete the relevant Experiment_Results records and consequentially,that will automatically 
                    *  delete all the Result Message Records. If Cascade Delete is not to be used, then the code to delete the extra records
                    *  in these 2 tables when an experiment is deleted should be added in the stored procedure
                    */
                    if (deleted)
                    {
                        arrayList.Add(experimentID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in Removing Experiments", ex);
            }

            //Shaomin's Code
            ArrayList qualifiersToRemove = new ArrayList();

            for (int i = 0; i < experimentIDs.Length; i++)
            {
                if (!arrayList.Contains(experimentIDs[i]))
                {
                    int qualifierID =
                        Authorization.AuthorizationAPI.GetQualifierID((int)experimentIDs[i],
                        Qualifier.experimentQualifierTypeID);
                    qualifiersToRemove.Add(qualifierID);
                }
            }

            int[] qualifierIDs = new int[qualifiersToRemove.Count];
            for (int i = 0; i < qualifiersToRemove.Count; i++)
            {
                qualifierIDs[i] = Convert.ToInt32(qualifiersToRemove[i]);
            }

            Authorization.AuthorizationAPI.RemoveQualifiers(qualifierIDs);

            long[] expIDs = new long[arrayList.Count];
            for (int i = 0; i < arrayList.Count; i++)
            {
                expIDs[i] = (long)arrayList[i];
            }

            return expIDs;
        }
        /// <summary>
        /// Retrieves the current Experiment summary form the ServiceBroker's database.
        /// </summary>
        /// <param name="experimentID"></param>
        /// <returns></returns>
        public static ExperimentSummary SelectExperimentSummary(long experimentID)
        {
            //select ei.coupon_ID, u.user_Name, g.group_Name, c.Lab_Client_Name,status, essGuid, 
            //   scheduledStart,duration, creationTime, closeTime, annotation

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentSummary", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            ExperimentSummary exp = null;
            try
            {
                myConnection.Open();
                SqlDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                    {
                        exp = new ExperimentSummary();
                        exp.experimentId = experimentID;
                       
                        exp.userName = myReader.GetString(0);
                        exp.groupName = myReader.GetString(1);
                        exp.labServerGuid = myReader.GetString(2);
                        exp.labServerName = myReader.GetString(3);
                        exp.clientName = myReader.GetString(4);
                        exp.clientVersion = myReader.GetString(5);
                        exp.status = myReader.GetInt32(6); 
                        if (!myReader.IsDBNull(7))
                         exp.essGuid = myReader.GetString(7);
                        else 
                          exp.essGuid = null;
                        if (!myReader.IsDBNull(8))
                            exp.scheduledStart = DateUtil.SpecifyUTC(myReader.GetDateTime(8));
                        if (!myReader.IsDBNull(9))
                            exp.duration = myReader.GetInt64(9);
                        if (!myReader.IsDBNull(10))
                        exp.creationTime = DateUtil.SpecifyUTC(myReader.GetDateTime(10));
                        if (!myReader.IsDBNull(11))
                            exp.closeTime = DateUtil.SpecifyUTC(myReader.GetDateTime(11));
                        if (!myReader.IsDBNull(12))
                            exp.annotation = myReader.GetString(12);
                    if (!myReader.IsDBNull(13))
                        exp.recordCount = myReader.GetInt32(13);
                     
                    }
                    myReader.Close();
                }
            
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return exp;
        }


        /// <summary>
        /// retrieves all ExperimentIds that the specified user and group has access to
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"
        /// <returns></returns>
        public static long[] RetrieveExperimentIDs(int userID, int groupID)
        {

            StringBuilder whereClause = null;
            List<long> expIDs = new List<long>();

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand cmd = new SqlCommand("RetrieveAuthorizedExpIDs", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter userParam = cmd.Parameters.Add("@userId", SqlDbType.Int);
            userParam.Value = userID;
            SqlParameter groupParam = cmd.Parameters.Add("@groupId", SqlDbType.Int);
            groupParam.Value = groupID;

            try
            {
                myConnection.Open();
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    expIDs.Add(myReader.GetInt64(0));
                }
            }
            catch { }
            finally
            {
                myConnection.Close();
            }
            return expIDs.ToArray();
        }
/*
        /// <summary>
        /// retrieves all ExperimentIds that the specified user and group has access to
        /// and that match the Criterion. The criterion has been limited to actual ISB experiment fields.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"
        /// <param name="carray">The criterion have been limited to ServiceBroker fields</param>
        /// <returns></returns>
        public static DataSet RetrieveExperimentIds(long[] experiments, Criterion[] carray)
        {
            DataSet results = new DataSet();
            StringBuilder whereClause = null;
            List<long> expIDs = new List<long>();
  
              if(carray != null && carray.Length > 0){     

                whereClause = new StringBuilder();
                   
                for (int i = 0; i < carray.Length; i++)
                {
                    if(i > 0)
                        whereClause.Append(" AND ");

                    switch (carray[i].attribute.ToLower())
                    {
                      
                        case "agent_id":    // Actual SB experiment column names
                        case "annotation":
                        case "client_id":
                        case "creationtime":
                        case "ess_id":
                        case "group_id":
                        case "record_count":
                        case "scheduledstart":
                        case "status":
                        case "user_id":
                            whereClause.Append(carray[i].ToSQL());
                            break;
                        default: // any unhandled attributes are ignored
                            break;
                    }
                }
                if(whereClause.Length > 7000){
                    throw new Exception("Please reduce the number of criteria for this query, too many arguments!");
                }

              }
                SqlConnection myConnection = ProcessAgentDB.GetConnection();
                SqlCommand cmd = new SqlCommand("RetrieveAuthorizedExpIDsCriteria", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter userParam = cmd.Parameters.Add("@userID", SqlDbType.Int);
				userParam.Value = userID;
                SqlParameter groupParam = cmd.Parameters.Add("@groupID", SqlDbType.Int);
				groupParam.Value = userID;
                SqlParameter whereParam = cmd.Parameters.Add(new SqlParameter("@whereClause",SqlDbType.VarChar,7000));
                if(whereClause != null && whereClause.Length == 0)
                    whereParam.Value = DBNull.Value;
                else
                    whereParam.Value = whereClause.ToString();
	
            try
            {
                myConnection.Open();
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    expIDs.Add(myReader.GetInt64(0));
                }
            }
            catch{}
            finally{
                myConnection.Close();
            }
           return expIDs.ToArray();
        }
*/
        /// <summary>
        /// retrieves all ExperimentIds that the specified user and group has access to
        /// and that match the Criterion. The criterion has been limited to actual ISB experiment fields.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"
        /// <param name="carray">The criterion have been limited to ServiceBroker fields</param>
        /// <returns></returns>
        public static DataSet RetrieveExperimentIDsCriteria(int userID, int groupID, Criterion[] carray)
        {
            //if (carray == null || carray.Length == 0)
            //{
            //    throw new Exception("Method requires Criteria");
            //}
            StringBuilder whereClause = null;
            DataSet results = new DataSet();

            if (carray != null && carray.Length > 0)
            {
                whereClause = new StringBuilder();

                for (int i = 0; i < carray.Length; i++)
                {
                    if (i > 0)
                        whereClause.Append(" AND ");

                    switch (carray[i].attribute.ToLower())
                    {
                        case "agent_id":    // Actual SB experiment column names                        
                        case "client_id":                       
                        case "ess_id":
                        case "group_id":
                        case "record_count":
                        case "status":
                        case "user_id":
                            whereClause.Append(carray[i].ToSQL());
                            break;
                        case "annotation":
                        case "creationtime":
                        case "scheduledstart":
                            whereClause.Append(carray[i].ToSQL());
                            break;
                        default: // any unhandled attributes are ignored
                            break;
                    }
                }
                if (whereClause.Length > 7000)
                {
                    throw new Exception("Please reduce the number of criteria for this query, too many arguments!");
                }


                SqlConnection myConnection = ProcessAgentDB.GetConnection();

                SqlCommand cmd = new SqlCommand("RetrieveAuthorizedExpIDsCriteria", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter userParam = cmd.Parameters.Add("@userID", SqlDbType.Int);
                userParam.Value = userID;
                SqlParameter groupParam = cmd.Parameters.Add("@groupID", SqlDbType.Int);
                groupParam.Value = groupID;
                SqlParameter whereParam = cmd.Parameters.Add(new SqlParameter("@criteria", SqlDbType.VarChar, 7000));
                if (whereClause != null && whereClause.Length > 0)
                    whereParam.Value = whereClause.ToString();
                else
                    whereParam.Value = DBNull.Value;
                try
                {
                    myConnection.Open();
                    SqlDataReader myReader = cmd.ExecuteReader();
                    if (myReader.HasRows)
                    {

                        DataTable exp = new DataTable("sbHits");
                        exp.Columns.Add("expid", typeof(System.Int64));
                        exp.Columns.Add("essid", typeof(System.Int32));
                        results.Tables.Add(exp);

                        while (myReader.Read())
                        {
                            DataRow row = exp.NewRow();
                            row["expid"] = myReader.GetInt64(0);
                            if (!myReader.IsDBNull(1))
                                row["essid"] = myReader.GetInt32(1);
                            else
                                row["essid"] = DBNull.Value;
                            exp.Rows.Add(row);
                        }
                    }
                    
                    if (myReader.NextResult())
                    {
                        if (myReader.HasRows)
                        {
                            DataTable ess = new DataTable("ess");
                            ess.Columns.Add("essid", typeof(System.Int32));
                            results.Tables.Add(ess);

                            while (myReader.Read())
                            {
                                if (!myReader.IsDBNull(0))
                                {
                                    DataRow essRow = ess.NewRow();
                                    essRow["essid"] = myReader.GetInt32(0);
                                    ess.Rows.Add(essRow);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Utilities.WriteLog("Error: " + e.Message);
                    throw;
                }
                finally
                {
                    myConnection.Close();
                }
                
            }
            return results;
            
        }

        public static ExperimentAdminInfo SelectExperimentAdminInfo(long experimentID)
        {
            ExperimentAdminInfo[] infos = SelectExperimentAdminInfos(new long[] { experimentID });
            if (infos != null && infos.Length > 0)
                return infos[0];
            else 
                return null;
        }

        public static ExperimentAdminInfo[] SelectExperimentAdminInfos(long[] experimentIDs)
        {
            //select u.user_Name,g.group_Name,pa.Agent_Guid,pa.Agent_Name,
            //c.Lab_Client_Name,c.version, status, ess_ID, scheduledStart, duration, creationTime, closeTime, annotation, record_count
            //from Experiments ei,ProcessAgent pa, Groups g, Lab_Clients c, Users u
            List<ExperimentAdminInfo> list = new List<ExperimentAdminInfo>();
            
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentAdminInfos", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@ids", SqlDbType.VarChar,7000));
            myCommand.Parameters["@ids"].Value = Utilities.ToCSV(experimentIDs);
            //myCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int));
            //myCommand.Parameters["@userID"].Value = userID;
            //myCommand.Parameters.Add(new SqlParameter("@groupId", SqlDbType.Int));
            //myCommand.Parameters["@groupID"].Value = groupID;
            try
            {
                myConnection.Open();

           
                    SqlDataReader myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        ExperimentAdminInfo exp = new ExperimentAdminInfo();
                   
                        exp.experimentID = myReader.GetInt64(0);
                        exp.userID = myReader.GetInt32(1);
                        exp.groupID = myReader.GetInt32(2);
                        exp.agentID = myReader.GetInt32(3);
                        exp.clientID = myReader.GetInt32(4);
                        if (!myReader.IsDBNull(5))
                        exp.essID = myReader.GetInt32(5);
                        exp.status = myReader.GetInt32(6);
                        exp.recordCount = myReader.GetInt32(7);
                        exp.duration = myReader.GetInt64(8);
                        if (!myReader.IsDBNull(9))
                        exp.startTime = DateUtil.SpecifyUTC(myReader.GetDateTime(9));
                        exp.creationTime = DateUtil.SpecifyUTC(myReader.GetDateTime(10));
                        if (!myReader.IsDBNull(11))
                        exp.closeTime = DateUtil.SpecifyUTC(myReader.GetDateTime(11));
                    if (!myReader.IsDBNull(12))
                        exp.annotation = myReader.GetString(12);

                      list.Add(exp);
                       
                    }
                    myReader.Close();
                }
            
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return list.ToArray();
        }

           
        public static ExperimentSummary[] SelectExperimentSummaries(long[] experimentIDs)
        {
            //select u.user_Name,g.group_Name,pa.Agent_Guid,pa.Agent_Name,
            //c.Lab_Client_Name,c.version, status, ess_ID, scheduledStart, duration, creationTime, closeTime, annotation, record_count
            //from Experiments ei,ProcessAgent pa, Groups g, Lab_Clients c, Users u
            ExperimentSummary[] exp = new ExperimentSummary[experimentIDs.Length];
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentSummary", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@experimentID", SqlDbType.BigInt));
            try
            {
                myConnection.Open();

                for (int i = 0; i < experimentIDs.Length; i++)
                {
                    myCommand.Parameters["@experimentID"].Value = experimentIDs[i];

                    // get experimentInfo from table Experiments
                   // select u.user_Name,g.group_Name,pa.Agent_Guid,pa.Agent_Name,
                   // c.Lab_Client_Name,c.version, status, ess_ID, scheduledStart, duration, 
                   // creationTime, closeTime, annotation, record_count
                   // from Experiments ei,ProcessAgent pa, Groups g, Lab_Clients c, Users u
                    SqlDataReader myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        exp[i] = new ExperimentSummary();
                        exp[i].experimentId = experimentIDs[i];
                       
                        exp[i].userName = myReader.GetString(0);
                        exp[i].groupName = myReader.GetString(1);
                        exp[i].labServerGuid = myReader.GetString(2);
                        exp[i].labServerName = myReader.GetString(3);
                        exp[i].clientName = myReader.GetString(4);
                        exp[i].clientVersion = myReader.GetString(5);
                        exp[i].status = myReader.GetInt32(6);
                        if (!myReader.IsDBNull(7))
                            exp[i].essGuid = myReader.GetString(7);
                        else exp[i].essGuid = null;
                        if (!myReader.IsDBNull(8))
                            exp[i].scheduledStart = DateUtil.SpecifyUTC(myReader.GetDateTime(8));
                        if (!myReader.IsDBNull(9))
                            exp[i].duration = myReader.GetInt64(9);
                        if (!myReader.IsDBNull(10))
                        exp[i].creationTime = DateUtil.SpecifyUTC(myReader.GetDateTime(10));
                        if (!myReader.IsDBNull(11))
                            exp[i].closeTime = DateUtil.SpecifyUTC(myReader.GetDateTime(11));
                        if (!myReader.IsDBNull(12))
                            exp[i].annotation = myReader.GetString(12);
                        if (!myReader.IsDBNull(13))
                        exp[i].recordCount = myReader.GetInt32(13);
                    }
                    myReader.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return exp;
        }

        public static bool UpdateExperimentStatus(long experimentID, int statusCode)
        {
            bool ok = false;
            SqlConnection myConnection = FactoryDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("UpdateExperimentStatusCode", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@status", statusCode));
            try{
                myConnection.Open();
                object obj = myCommand.ExecuteScalar();
                if(obj != null){
                    int i = Convert.ToInt32(obj);
                    ok = i > 0;
                }
            }
            catch{
                throw;
            }
            finally{
                myConnection.Close();
            }
            return ok;
        }

        public static bool UpdateExperimentStatus(StorageStatus status)
        {
            bool ok = false;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("UpdateExperimentStatus", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", status.experimentId));
            myCommand.Parameters.Add(new SqlParameter("@status", status.status));
            SqlParameter closeParam = myCommand.Parameters.Add("@closeTime", SqlDbType.DateTime);
            if (status.closeTime != null && (status.closeTime > MinSqlDateTime))
            {
                closeParam.Value = status.closeTime;
            }
            else
            {
                closeParam.Value = DBNull.Value;
            }
            SqlParameter countParam = myCommand.Parameters.Add("@recordCount", SqlDbType.Int);
            if (status.recordCount != null)
            {
                countParam.Value = status.recordCount ;
            }
            else
            {
                countParam.Value = DBNull.Value;
            }

            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                ok = true;
            }
            catch (Exception e)
            {
                Utilities.WriteLog("UpdateExperimentStatus: " + e.Message);
            }
            finally
            {
                myConnection.Close();
            }
            return ok;
        }



 
/*
        
		/// <summary>
		/// to retrieve information about an experiment such as owner, submitted time, etc.
		/// </summary>
		public static ExperimentInformation RetrieveExperimentInformation (long experimentID)
		{
			ExperimentInformation ei = new ExperimentInformation();
            
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveExperimentInformation", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@experimentID",experimentID));

			try 
			{
				myConnection.Open ();
				
				// get experiment info from table Experiment_Information
				SqlDataReader myReader = myCommand.ExecuteReader ();
				while(myReader.Read ())
				{	
					ei.experimentID = experimentID;
                   
			
                    if(myReader["status"] !=System.DBNull.Value)
						ei.statusCode = Convert.ToInt32(myReader["status"]);
					if(myReader["user_id"] != System.DBNull.Value )
						ei.userID = Convert.ToInt32(myReader["user_id"]);
					if(myReader["group_id"] != System.DBNull.Value )
						ei.effectiveGroupID= Convert.ToInt32(myReader["group_id"]);
					if(myReader["lab_server_id"] != System.DBNull.Value )
						ei.labServerID = Convert.ToInt32(myReader["agent_id"]);
                    
					if(myReader["creation_time"] != System.DBNull.Value )
						ei.submissionTime = Convert.ToDateTime(myReader["creation_time"]);
                    if(myReader["duration"] != System.DBNull.Value )
						ei.submissionTime = Convert.ToDateTime(myReader["duration"]);
                    if(myReader["close_time"] != System.DBNull.Value )
						ei.completionTime= Convert.ToDateTime( myReader["close_time"]);
					if(myReader["annotation"] != System.DBNull.Value )
						ei.annotation= (string) myReader["annotation"];
                    ei.
					
					
				}
				myReader.Close ();
				
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown getting experiment information",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return ei;
		}
      */  

        /*
		/// <summary>
		/// to retrieve information about a set of experiments 
		/// </summary>
		public static ExperimentInformation[] RetrieveExperimentInformation (long[] experimentIDs)
		{
			ExperimentInformation[]ei = new ExperimentInformation[experimentIDs.Length];
			for (int i = 0;i<experimentIDs.Length;i++)
			{
				ei[i]= new ExperimentInformation();
			}
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveExperimentInformation", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@experimentID",SqlDbType.BigInt));

			try 
			{
				myConnection.Open ();
				
				for (int i=0;i<experimentIDs.Length;i++)
				{
					myCommand.Parameters["@experimentID"].Value = experimentIDs[i];

												   
					// get experiment info from table Experiment_Information
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						ei[i].experimentID = experimentIDs[i];

						if(myReader["user_id"] != System.DBNull.Value )
							ei[i].userID = Convert.ToInt32(myReader["user_id"]);
						if(myReader["group_id"] != System.DBNull.Value )
							ei[i].effectiveGroupID= Convert.ToInt32(myReader["group_id"]);
						if(myReader["agent_id"] != System.DBNull.Value )
							ei[i].labServerID = Convert.ToInt32(myReader["agent_id"]);
                        //if (myReader["scheduledStart"] != System.DBNull.Value)
                        //    ei[i].submissionTime = Convert.ToDateTime(myReader["scheduledStart"]);
                        //if (myReader["duration"] != System.DBNull.Value)
                        //    ei[i].submissionTime = Convert.ToDateTime(myReader["duration"]);
                        //if(myReader["essID"] != System.DBNull.Value )
                        //    ei[i]. = Convert.ToDateTime(myReader["essID"]);
						if(myReader["annotation"] != System.DBNull.Value )
							ei[i].annotation= (string) myReader["annotation"];
						if(myReader["closeTime"] != System.DBNull.Value )
							ei[i].completionTime= Convert.ToDateTime( myReader["closeTime"]);
						if(myReader["status"] !=System.DBNull.Value)
							ei[i].statusCode = Convert.ToInt32(myReader["status"]);
					}
					myReader.Close ();
				}
				
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown getting experiment information",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return ei;
		}
*/
        /// <summary>
        /// to retrieve all the experiments that were run under the specified groups
        /// </summary>
        public static long[] SelectGroupExperimentIDs(int[] groupIDs)
        {
            ArrayList eIDs = new ArrayList();

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveGroupExperimentIDs", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@groupID", SqlDbType.Int));

            try
            {
                myConnection.Open();

                for (int i = 0; i < groupIDs.Length; i++)
                {
                    myCommand.Parameters["@groupID"].Value = groupIDs[i];

                    // get experiment id from table Experiment_Information
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    while (myReader.Read())
                    {
                        if (myReader["experiment_id"] != System.DBNull.Value)
                            eIDs.Add(Convert.ToInt32(myReader["experiment_id"]));
                    }
                    myReader.Close();
                }

                return Utilities.ArrayListToLongArray(eIDs);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown getting experiments that were run in this group", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

        /// <summary>
        /// finds all of those Experiments specified in experimentIDs which possess attributes that match the logical AND of conditions expressed in the Criterion array. The search is limited to those experiments that the current user created or for which he or she has a “ReadExperiment” grant. The Criterion conditions must be satisfied elements of the Experiment’s administrative data model such as its ownerID or by the RecordAttributes of a single ExperimentRecord belonging to the experiment for it to qualify. 
        /// </summary>
        /// <param name="criteria">The array of Criterion objects that specify the attributes of the requested experiments; all experimentIDs match if null.</param>
        /// <returns>an array of the IDs of Experiments that match the search criteria</returns>
        // Need to stub in alternate predicates and attribute hash maps. - CV 07/08/04
        public static long[] SelectExperimentIDs(Criterion[] criteria)
        {
            StringBuilder sqlQuery = new StringBuilder("select experiment_id from experiments");

            long[] experimentIDs;

          
            for (int i = 0; i < criteria.Length; i++)
            {
                if (i == 0)
                {
                    sqlQuery.Append(" where ");
                }
                else {
                    sqlQuery.Append(" AND ");
                }
                sqlQuery.Append(criteria[i].attribute + " " + criteria[i].predicate + " '" + criteria[i].value + "'");

            }

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandType = CommandType.Text;
            myCommand.CommandText = sqlQuery.ToString();

            try
            {
                myConnection.Open();
                // get experiment ids from table experiments
                SqlDataReader myReader = myCommand.ExecuteReader();
                ArrayList eIDs = new ArrayList();

                while (myReader.Read())
                {
                    if (myReader["experiment_id"] != System.DBNull.Value)
                        eIDs.Add(myReader["experiment_id"]);
                }

                myReader.Close();
                // Converting to a string array
                experimentIDs = new long[eIDs.Count];
                for (int i = 0; i < eIDs.Count; i++)
                {
                    experimentIDs[i] = Convert.ToInt64(eIDs[i]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown finding experiment", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return experimentIDs;


        }
/*
        /// <summary>
        /// finds all of those Experiments specified in experimentIDs which possess attributes that match the logical AND of conditions expressed in the Criterion array. The search is limited to those experiments that the current user created or for which he or she has a “ReadExperiment” grant. The Criterion conditions must be satisfied elements of the Experiment’s administrative data model such as its ownerID or by the RecordAttributes of a single ExperimentRecord belonging to the experiment for it to qualify. 
        /// </summary>
        /// <param name="criteria">The array of Criterion objects that specify the attributes of the requested experiments; all experimentIDs match if null.</param>
        /// <returns>an array of the IDs of Experiments that match the search criteria</returns>
        // Need to stub in alternate predicates and attribute hash maps. - CV 07/08/04
        public static ExperimentSummary[] SelectExperiments(Criterion[] criteria)
        {
            StringBuilder sqlQuery = new StringBuilder();
            long[] experimentIDs;

            sqlQuery.Append("select experiment_id ");
            sqlQuery.Append(" from experiments where ");
            for (int i = 0; i < criteria.Length; i++)
            {
                if (i != 0)
                {
                    sqlQuery.Append(" AND ");
                }
                sqlQuery.Append(criteria[i].attribute + " " + criteria[i].predicate + " '" + criteria[i].value + "'");

            }

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandType = CommandType.Text;
            myCommand.CommandText = sqlQuery.ToString();

            try
            {
                myConnection.Open();
                // get experiment ids from table experiments
                SqlDataReader myReader = myCommand.ExecuteReader();
                ArrayList eIDs = new ArrayList();

                while (myReader.Read())
                {
                    if (myReader["experiment_id"] != System.DBNull.Value)
                        eIDs.Add(myReader["experiment_id"]);
                }

                myReader.Close();
                // Converting to a string array
                experimentIDs = new long[eIDs.Count];
                for (int i = 0; i < eIDs.Count; i++)
                {
                    experimentIDs[i] = Convert.ToInt64(eIDs[i]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown finding experiment", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return SelectExperimentSummary(myConnection, experimentIDs);
        }
 * */
/*
        /// <summary>
        /// finds all of those Experiments specified in experimentIDs which possess attributes that match the logical AND of conditions expressed in the Criterion array. The search is limited to those experiments that the current user created or for which he or she has a “ReadExperiment” grant. The Criterion conditions must be satisfied elements of the Experiment’s administrative data model such as its ownerID or by the RecordAttributes of a single ExperimentRecord belonging to the experiment for it to qualify. 
        /// </summary>
        /// <param name="criteria">The array of Criterion objects that specify the attributes of the requested experiments; all experimentIDs match if null.</param>
        /// <returns>an array of the IDs of Experiments that match the search criteria</returns>
        // Need to stub in alternate predicates and attribute hash maps. - CV 07/08/04
        public static ExperimentSummary[] SelectExperimentInfo(long[] expIDs)
        {
            ExperimentSummary[] experiments = null;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            try
            {
                experiments = SelectExperimentSummary(myConnection, expIDs);
            }
            catch (Exception e)
            {
            }
            finally
            {
                myConnection.Close();
            }
            return experiments;

        }
     */  
        /// <summary>
        /// Creates a populated ESS proxy if the specified experiment has an associated ESS.
        /// </summary>
        /// <param name="experimentID"></param>
        /// <returns>a valid ESS proxy, null if the experiment does not have an ESS</returns>
        public static ExperimentStorageProxy GetEssProxy(long experimentID){

            ExperimentStorageProxy proxy = null;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("getEssInfo", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            
            try
            {
                myConnection.Open();
                SqlDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    proxy = new ExperimentStorageProxy();
                    proxy.AgentAuthHeaderValue = new AgentAuthHeader();
                    Coupon coupon = new Coupon();
                    coupon.couponId = (long) myReader.GetSqlInt64(0);
                    coupon.issuerGuid = (string)myReader.GetSqlString(1);
                    coupon.passkey = (string) myReader.GetSqlString(2);
                    proxy.AgentAuthHeaderValue.coupon = coupon;
                    proxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                    proxy.Url = (string) myReader.GetSqlString(3);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown updateing ESSinfo", ex);
            }
            finally
            {
                myConnection.Close();
            }
            return proxy;
        }
     

        public static bool UpdateExperimentESSInfo(long experimentID, long essExpID, int agentId)
        {
            bool status = false;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("UpdateEssInfo", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@essExpID", essExpID));
            myCommand.Parameters.Add(new SqlParameter("@essID", agentId));
            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                status = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown updateing ESSinfo", ex);
            }
            finally
            {
                myConnection.Close();
            }
            return status;
        }

        /** ExperimentCoupon **/

        public static int DeleteExperimentCoupon(long experimentID, long couponID)
        {
            int count = 0;
            SqlConnection connection = ProcessAgentDB.GetConnection();
            try
            {
                count = DeleteExperimentCoupon(connection, experimentID, couponID);
            }
            catch { }
            finally
            {
                connection.Close();
            }
            return count;
        }

        public static int DeleteExperimentCoupon(SqlConnection connection, long experimentID, long couponID)
        {
            SqlCommand myCommand = new SqlCommand("DeleteExperimentCoupon", connection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@couponID", couponID));
            connection.Open();
            int count = Convert.ToInt32(myCommand.ExecuteScalar());
            return count;
        }

        public static void InsertExperimentCoupon(long experimentID, long couponID)
        {
            SqlConnection connection = ProcessAgentDB.GetConnection();
            try
            {
                InsertExperimentCoupon(connection, experimentID, couponID);
            }
            catch { }
            finally
            {
                connection.Close();
            }
        }
        public static void InsertExperimentCoupon(SqlConnection connection, long experimentID, long couponID)
        {
            SqlCommand myCommand = new SqlCommand("InsertExperimentCoupon", connection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            myCommand.Parameters.Add(new SqlParameter("@couponID", couponID));
            connection.Open();
            myCommand.ExecuteNonQuery();

        }

        public static long[] RetrieveExperimentCouponIDs(long experimentID)
        {
            long[] coupons = null;
            SqlConnection connection = ProcessAgentDB.GetConnection();
            try
            {
                coupons = RetrieveExperimentCouponIDs(connection, experimentID);
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return coupons;
        }

        public static long[] RetrieveExperimentCouponIDs(SqlConnection connection, long experimentID)
        {
            long[] coupons = null;
            ArrayList ids = new ArrayList();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentCouponID", connection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            connection.Open();
            SqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                long id = reader.GetInt64(0);
                ids.Add(id);
            }
            coupons = Utilities.ArrayListToLongArray(ids);
            return coupons;
        }


        /* !------------------------------------------------------------------------------!
         *							CALLS FOR CLIENT ITEMS
         * !------------------------------------------------------------------------------!
         */

        /// <summary>
        /// to add a client item
        /// </summary>
        public static void SaveClientItem(int clientID, int userID, string itemName, string itemValue)
        {
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SaveClientItem", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@clientID", clientID));
            myCommand.Parameters.Add(new SqlParameter("@userID", userID));
            myCommand.Parameters.Add(new SqlParameter("@itemName", itemName));
            myCommand.Parameters.Add(new SqlParameter("@itemValue", itemValue));

            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in inserting client item", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

        /// <summary>
        /// to delete the client item specified by the combination of clientID, userID and itemName
        /// </summary>
        public static void DeleteClientItem(int clientID, int userID, string itemName)
        {
            //string previousItem =  SelectClientItemValue(clientID,userID,itemName);
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("DeleteClientItem", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@clientID", clientID));
            myCommand.Parameters.Add(new SqlParameter("@userID", userID));
            myCommand.Parameters.Add(new SqlParameter("@itemName", itemName));

            try
            {
                myConnection.Open();
                int i = myCommand.ExecuteNonQuery();
                if (i == 0)
                    throw new Exception("No record exists exception");
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in DeleteClientItem", ex);
            }
            finally
            {
                myConnection.Close();
            }
            //return previousItem;
        }

        /// <summary>
        /// to retrieve a list of all the item names in the database for a client -user combo
        /// </summary>
        public static string[] SelectClientItems(int clientID, int userID)
        {
            string[] clientItemIDs;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveClientItemNames", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.Add(new SqlParameter("@clientID", clientID));
            myCommand.Parameters.Add(new SqlParameter("@userID", userID));

            try
            {
                myConnection.Open();


                // get ClientItem Names from table stored_item_summary
                SqlDataReader myReader = myCommand.ExecuteReader();
                ArrayList citems = new ArrayList();

                while (myReader.Read())
                {
                    if (myReader["item_name"] != System.DBNull.Value)
                        citems.Add((string)myReader["item_name"]);
                    //	clientItemIDs [i] = (string) myReader["item_name"];
                }
                myReader.Close();

                // Converting to a string array
                clientItemIDs = Utilities.ArrayListToStringArray(citems);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown SelectClientItemIDs", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return clientItemIDs;
        }


        /// <summary>
        /// to retrieve user the item value for client items specified the combination of clientID, userID and itemName 
        /// </summary>
        public static string SelectClientItemValue(int clientID, int userID, string itemName)
        {
            ArrayList clientItems = new ArrayList();

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveClientItem", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@clientID", clientID));
            myCommand.Parameters.Add(new SqlParameter("@userID", userID));
            myCommand.Parameters.Add(new SqlParameter("@itemName", itemName));

            try
            {
                myConnection.Open();

                // get ClientItem info from table client_items
                return myCommand.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown SelectClientItemValue", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

 
    }
}
