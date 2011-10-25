using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.TicketIssuer;
using iLabs.UtilLib;

using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.DataStorage;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker.Mapping;
using iLabs.Proxies.PAgent;
using iLabs.Proxies.ESS;

namespace iLabs.ServiceBroker
{
    /// <summary>
    /// Interface for the DB Layer class
    /// </summary>
    public class BrokerDB : TicketIssuerDB
    {


        //protected static ResourceMapping[] resourceMappings;

        public BrokerDB()
        {
        }

        public ProcessAgentInfo GetExperimentESS(long experimentID)
        {
            DbConnection myConnection = FactoryDB.GetConnection();
            DbCommand myCommand = FactoryDB.CreateCommand("Experiment_RetrieveEssInfo", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(FactoryDB.CreateParameter( "@experimentID", experimentID, DbType.Int64));
            ProcessAgentInfo ess = null;
            try
            {
                myConnection.Open();
                DbDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    ess = readAgentInfo(myReader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown reading ESSinfo", ex);
            }
            finally
            {
                myConnection.Close();
            }
            return ess;
        }

        public Coupon GetEssOpCoupon(long experimentId, string ticketType,
             long duration, string essGuid)
        {
            Coupon opCoupon = null;
            long[] couponIDs = DataStorageAPI.RetrieveExperimentCouponIDs(experimentId);
            if (couponIDs != null && couponIDs.Length >= 0)
            {   // An experiment ticket collection exists, try and find an active
                // Retrieve_Records ticket
                for (int i = 0; i < couponIDs.Length; i++)
                {
                    Coupon tmpCoupon = GetIssuedCoupon(couponIDs[i]);
                    Ticket ticket = RetrieveTicket(tmpCoupon, ticketType);
                    if (ticket != null && !ticket.IsExpired() && (ticket.SecondsToExpire() > duration))
                    {
                        if (ticket.redeemerGuid.CompareTo(essGuid) == 0)
                        {
                            opCoupon = tmpCoupon;
                            break;
                        }
                    }
                }
            }
            return opCoupon;
        }

        public Experiment RetrieveExperiment(long experimentID, int userID, int groupID)
        {
            int roles = 0;
            Experiment experiment = null;
            AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
            roles = wrapper.GetExperimentAuthorizationWrapper(experimentID, userID, groupID);

            if ((roles | ExperimentAccess.READ) == ExperimentAccess.READ)
            {
                experiment = new Experiment();
                experiment.experimentId = experimentID;
                experiment.issuerGuid = ProcessAgentDB.ServiceGuid;
                ProcessAgentInfo ess = GetExperimentESS(experimentID);
                if (ess != null)
                {
                    ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                    Coupon opCoupon = GetEssOpCoupon(experimentID, TicketTypes.RETRIEVE_RECORDS, 60, ess.agentGuid);
                    if (opCoupon == null)
                    {
                        string payload = TicketLoadFactory.Instance().RetrieveRecordsPayload(experimentID, ess.webServiceUrl);
                        opCoupon = CreateTicket(TicketTypes.RETRIEVE_RECORDS, ess.agentGuid, ProcessAgentDB.ServiceGuid,
                            60, payload);
                    }
                    essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                    essProxy.OperationAuthHeaderValue.coupon = opCoupon;
                    essProxy.Url = ess.webServiceUrl;
                    experiment.records = essProxy.GetRecords(experimentID, null);
                }

            }
            else
            {
                throw new AccessDeniedException("You do not have permission to read this experiment");
            }

            return experiment;
        }

        public ExperimentRecord[] RetrieveExperimentRecords(long experimentID, int userID, int groupID, Criterion[] criteria)
        {
            int roles = 0;
            ExperimentRecord[] records = null;
            AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
            roles = wrapper.GetExperimentAuthorizationWrapper(experimentID, userID, groupID);

            if ((roles | ExperimentAccess.READ) == ExperimentAccess.READ)
            {
                records = RetrieveExperimentRecords(experimentID, criteria);

            }
            else
            {
                throw new AccessDeniedException("You do not have permission to read this experiment");
            }

            return records;
        }

        public ExperimentRecord[] RetrieveExperimentRecords(long experimentID, Criterion[] criteria)
        {
            ExperimentRecord[] records = null;
            ProcessAgentInfo ess = GetExperimentESS(experimentID);
            if (ess != null)
            {
                ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                Coupon opCoupon = GetEssOpCoupon(experimentID, TicketTypes.RETRIEVE_RECORDS, 60, ess.agentGuid);
                if (opCoupon == null)
                {
                    string payload = TicketLoadFactory.Instance().RetrieveRecordsPayload(experimentID, ess.webServiceUrl);
                    opCoupon = CreateTicket(TicketTypes.RETRIEVE_RECORDS, ess.agentGuid, ProcessAgentDB.ServiceGuid,
                        60, payload);
                }
                essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                essProxy.OperationAuthHeaderValue.coupon = opCoupon;
                essProxy.Url = ess.webServiceUrl;
                records = essProxy.GetRecords(experimentID, criteria);
            }
            return records;
        }


        //public Coupon CreateExperimentTicketCollection(ProcessAgentInfo ess, int userId, int groupId, int roles, long duration){
        //    Coupon coupon = CreateCoupon();

        //        TicketLoadFactory factory = TicketLoadFactory.Instance();
        //        string payload = null;
        //        if (ticketType.CompareTo(TicketTypes.ADMINISTER_EXPERIMENT) == 0)
        //        {
        //            payload = factory.createAdministerESSPayload();
        //        }
        //        if (ticketType.CompareTo(TicketTypes.RETRIEVE_RECORDS) == 0)
        //        {
        //            payload = factory.RetrieveRecordsPayload(experimentId, webServiceUrl);
        //        }
        //        if (ticketType.CompareTo(TicketTypes.STORE_RECORDS) == 0)
        //        {
        //            payload = factory.StoreRecordsPayload(true, experimentId, webServiceUrl);
        //        }
        //        // Create a ticket to read records
        //        opCoupon = CreateTicket(ticketType, agentGuid, ProcessAgentDB.ServiceGuid, duration, payload);
        //        DataStorage.InsertExperimentCoupon(experimentId, opCoupon.couponId);

        //    return opCoupon;
        //}

        /** Admin URL **/

        public int InsertAdminURL(DbConnection connection, int id, string url, string ticketType)
        {
            try
            {
                if (!TicketTypes.TicketTypeExists(ticketType))
                    throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");

                // command executes the "InsertAdminURL" stored procedure
                DbCommand cmd = FactoryDB.CreateCommand("AdminURL_Insert", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                // 1. type
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@processAgentID", id, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@adminURL", url, DbType.String, 512));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@ticketType", ticketType, DbType.AnsiString, 100));


                // execute the command
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }
        }

        public int InsertAdminURL(int id, string url, string ticketType)
        {
            if (!TicketTypes.TicketTypeExists(ticketType))
                throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");
            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                connection.Open();
                return InsertAdminURL(connection, id, url, ticketType);
            }

            finally
            {
                connection.Close();
            }
        }

        public void DeleteAdminURL(int Id)
        {

            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                connection.Open();
                DeleteAdminURL(connection, Id);
            }
            finally
            {
                connection.Close();
            }
        }


        public int DeleteAdminURL(DbConnection connection, int Id)
        {
            try
            {

                // command executes the "DeleteAdminURLbyID" stored procedure
                DbCommand cmd = FactoryDB.CreateCommand("AdminURL_DeleteByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@adminURLID", Id, DbType.Int32));

                // execute the command
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }
        }


        public int DeleteAdminURL(DbConnection connection, int processAgentId, string url, string ticketType)
        {
            try
            {
                if (!TicketTypes.TicketTypeExists(ticketType))
                    throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");

                // command executes the "InsertAdminURL" stored procedure
                DbCommand cmd = FactoryDB.CreateCommand("AdminURL_Delete", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                // 1. type
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@processAgentID", processAgentId, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@adminURL", url, DbType.String, 512));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@ticketType", ticketType, DbType.AnsiString, 100));


                // execute the command
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }
        }

        public void DeleteAdminURL(ProcessAgentInfo processAgentInfo, string url, string ticketType)
        {
            if (!TicketTypes.TicketTypeExists(ticketType))
                throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");
            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                connection.Open();
                DeleteAdminURL(connection, processAgentInfo.AgentId, url, ticketType);
            }

            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
        }

        public void DeleteAdminURL(AdminUrl adminURL)
        {
            if (!TicketTypes.TicketTypeExists(adminURL.TicketType.name))
                throw new Exception("\"" + adminURL.TicketType.name + "\" is not a valid ticket type.");
            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                connection.Open();
                DeleteAdminURL(connection, adminURL.ProcessAgentId, adminURL.Url, adminURL.TicketType.name);
            }

            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
        }

        public int ModifyAdminUrls(int agentID, string oldCodebase, string newCodebase)
        {

            int status = 0;
            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();
            // command executes the "InsertAdminURL" stored procedure
            DbCommand cmd = FactoryDB.CreateCommand("AdminURL_UpdateCodebase", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameter
            // 1. type
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@id", agentID, DbType.Int32));
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@old", oldCodebase, DbType.String, 512));
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@new", newCodebase, DbType.String, 512));

            try
            {
                // execute the command
                connection.Open();
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                    status = Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return status;
        }

        public int ModifyClientScripts(int clientID, string oldCodebase, string newCodebase)
        {
            int status = 0;
            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();

            DbCommand cmd = FactoryDB.CreateCommand("Client_RetrieveLoaderScript", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameter
            // 1. type
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@id", clientID, DbType.Int32));

            int count = -1;
            try
            {
                // execute the command
                connection.Open();
                string loaderScript = Convert.ToString(cmd.ExecuteScalar());
                if (loaderScript.Contains(oldCodebase))
                {
                    loaderScript = loaderScript.Replace(oldCodebase, newCodebase);

                    DbCommand cmd2 = FactoryDB.CreateCommand("Client_UpdateLoaderScript", connection);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.Add(FactoryDB.CreateParameter("@id", clientID, DbType.Int32));
                    cmd2.Parameters.Add(FactoryDB.CreateParameter("@script", loaderScript, DbType.String, 2000));

                    count = cmd2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return status;
        }

        public int ModifyResourceInfoURL(int agentID, string oldCodebase, string newCodebase)
        {
            int status = 0;
            Hashtable resources = GetResourceStringTags(agentID, ResourceMappingTypes.PROCESS_AGENT);
            if (resources != null) // Check the current resources
            {
                IntTag resourceTag = null;

                if (resources.ContainsKey("Info URL"))
                {
                    resourceTag = (IntTag)resources["Info URL"];
                    string value = resourceTag.tag.Replace(oldCodebase, newCodebase);
                    status = UpdateResourceMappingString(resourceTag.id, value);
                }
            }
            return status;
        }



        public AdminUrl[] RetrieveAdminURLs(int processAgentID)
        {
            // create sql connection
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                connection.Open();
                return RetrieveAdminURLs(connection, processAgentID);
            }

            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }

        }

        public AdminUrl RetrieveAdminURL(int processAgentID, string function)
        {
            AdminUrl[] adminUrls = RetrieveAdminURLs(processAgentID);
            for (int i = 0; i < adminUrls.Length; i++)
            {
                if (adminUrls[i].TicketType.name.CompareTo(function) == 0)
                    return adminUrls[i];

            }
            return null;
        }

        public AdminUrl RetrieveAdminURL(string processAgentGuid, string function)
        {
            int id = GetProcessAgentID(processAgentGuid);
            if (id > 0)
                return RetrieveAdminURL(id, function);
            else
                return null;
        }

        public AdminUrl[] RetrieveAdminURLs(DbConnection connection, int processAgentID)
        {
            // create sql command
            // command executes the "RetrieveAdminURLs" stored procedure
            DbCommand cmd = FactoryDB.CreateCommand("AdminURLs_Retrieve", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@processAgentID", processAgentID, DbType.Int32));


            // read the result
            ArrayList list = new ArrayList();
            DbDataReader dataReader = cmd.ExecuteReader();
            try
            {
                while (dataReader.Read())
                {
                    int id = (int)dataReader.GetInt32(0);
                    processAgentID = (int)dataReader.GetInt32(1);
                    string url = dataReader.GetString(2).Trim();
                    string ticketType = dataReader.GetString(3);

                    list.Add(new AdminUrl(id, processAgentID, url, ticketType));
                }
                dataReader.Close();
            }
            catch (DbException e)
            {
                Console.WriteLine(e);
                throw;
            }

            finally
            {
                // close the sql connection
                connection.Close();
            }

            AdminUrl dummy = new AdminUrl();
            AdminUrl[] urls = (AdminUrl[])list.ToArray(dummy.GetType());
            return urls;
        }

        public override int ModifyDomainCredentials(string originalGuid, ProcessAgent agent, Coupon inCoupon, Coupon outCoupon, string extra)
        {
            int status = 0;
            ProcessAgentInfo paiOld = GetProcessAgentInfo(agent.agentGuid);
            try
            {
                status = base.ModifyDomainCredentials(originalGuid, agent, inCoupon, outCoupon, extra);
            }
            catch (Exception ex)
            {
                throw new Exception("ISB: ", ex);
            }
            if (paiOld != null)
            {
                if (agent.codeBaseUrl.CompareTo(paiOld.codeBaseUrl) != 0)
                {
                    ModifyAdminUrls(paiOld.agentId, paiOld.codeBaseUrl, agent.codeBaseUrl);
                    ModifyResourceInfoURL(paiOld.agentId, paiOld.codeBaseUrl, agent.codeBaseUrl);
                    if (paiOld.agentType == ProcessAgentType.AgentType.LAB_SERVER)
                    {
                        ModifyClientScripts(paiOld.agentId, paiOld.codeBaseUrl, agent.codeBaseUrl);
                    }
                }
            }
            //Notify all ProcessAgents about the change
            ProcessAgentInfo[] domainServices = GetProcessAgentInfos();
            ProcessAgentProxy proxy = null;
            foreach (ProcessAgentInfo pi in domainServices)
            {
                // Do not send if retired this service or the service being modified since this is
                if (!pi.retired && (pi.agentGuid.CompareTo(ProcessAgentDB.ServiceGuid) != 0)
                    && (pi.agentGuid.CompareTo(agent.agentGuid) != 0))
                {
                    proxy = new ProcessAgentProxy();
                    proxy.AgentAuthHeaderValue = new AgentAuthHeader();
                    proxy.AgentAuthHeaderValue.coupon = pi.identOut;
                    proxy.Url = pi.webServiceUrl;

                    proxy.ModifyDomainCredentials(originalGuid, agent, extra, inCoupon, outCoupon);
                }
            }
            return status;
        }

        public override int ModifyProcessAgent(string originalGuid, ProcessAgent agent, string extra)
        {
            int status = 0;
            ProcessAgentInfo paiOld = GetProcessAgentInfo(originalGuid);

            if (paiOld != null)
            {

                try
                {
                    status = UpdateProcessAgent(paiOld.agentId, agent.agentGuid, agent.agentName, agent.type,
                        agent.domainGuid, agent.codeBaseUrl, agent.webServiceUrl);
                }
                catch (Exception ex)
                {
                    throw new Exception("ISB: ", ex);
                }

                if (agent.codeBaseUrl.CompareTo(paiOld.codeBaseUrl) != 0)
                {
                    status += ModifyAdminUrls(paiOld.agentId, paiOld.codeBaseUrl, agent.codeBaseUrl);
                    status += ModifyResourceInfoURL(paiOld.agentId, paiOld.codeBaseUrl, agent.codeBaseUrl);
                    if (paiOld.agentType == ProcessAgentType.AgentType.LAB_SERVER)
                    {
                        status += ModifyClientScripts(paiOld.agentId, paiOld.codeBaseUrl, agent.codeBaseUrl);
                    }
                }
                if (agent.agentName.CompareTo(paiOld.agentName) != 0)
                {
                    // need to update Qualifier Names
                    AuthorizationAPI.ModifyQualifierName(Qualifier.ToTypeID(agent.type), paiOld.agentId, agent.agentName);
                    int[] resourceMapIds = GetResourceMappingIdsByValue(ResourceMappingTypes.PROCESS_AGENT, paiOld.agentId);
                    foreach (int id in resourceMapIds)
                    {
                        ResourceMapping map = ResourceMapManager.GetMap(id);
                        AuthorizationAPI.ModifyQualifierName(Qualifier.resourceMappingQualifierTypeID, id, ResourceMappingToString(map));
                    }
                }
                //Notify all ProcessAgents about the change
                ProcessAgentInfo[] domainServices = GetProcessAgentInfos();
                ProcessAgentProxy proxy = null;
                foreach (ProcessAgentInfo pi in domainServices)
                {
                    // Do not send if retired this service or the service being modified since this is
                    if (!pi.retired && (pi.agentType != ProcessAgentType.AgentType.BATCH_LAB_SERVER)
                        && (pi.agentGuid.CompareTo(ProcessAgentDB.ServiceGuid) != 0)
                        && (pi.agentGuid.CompareTo(agent.agentGuid) != 0))
                    {
                        proxy = new ProcessAgentProxy();
                        proxy.AgentAuthHeaderValue = new AgentAuthHeader();
                        proxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                        proxy.AgentAuthHeaderValue.coupon = pi.identOut;
                        proxy.Url = pi.webServiceUrl;
                        try
                        {
                            status += proxy.ModifyProcessAgent(originalGuid, agent, extra);
                        }
                        catch (Exception ex)
                        {
                            Exception ex2 = new Exception("ModifyProcessAgent: " + pi.webServiceUrl, ex);
                            throw ex2;
                        }
                    }
                }
            }

            return status;
        }


        /* START OF RESOURCE MAPPING */

        public IntTag[] GetAdminServiceTags(int groupID)
        {
            ArrayList list = new ArrayList();
            DbConnection connection = FactoryDB.GetConnection();
            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ProcesAgent_RetrieveAdminServiceTags", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                // populate stored procedure parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@groupId", groupID, DbType.Int32));


                // read the result
                connection.Open();
                DbDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    int id = dataReader.GetInt32(0);
                    string name = dataReader.GetString(1);
                    list.Add(new IntTag(id, name));
                }
                dataReader.Close();
            }
            catch (DbException e)
            {
                Console.WriteLine(e);
                throw;
            }

            finally
            {
                // close the sql connection
                connection.Close();
            }

            IntTag dummy = new IntTag();
            IntTag[] tags = (IntTag[])list.ToArray(dummy.GetType());
            return tags;
        }


        public IntTag[] GetAdminProcessAgentTags(int groupID)
        {
            ArrayList list = new ArrayList();
            DbConnection connection = FactoryDB.GetConnection();
            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ProcessAgent_RetrieveAdminTags", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                // populate stored procedure parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@groupID", groupID, DbType.Int32));

                // read the result
                connection.Open();
                DbDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    int id = dataReader.GetInt32(0);
                    string name = dataReader.GetString(1);
                    list.Add(new IntTag(id, name));
                }
                dataReader.Close();
            }
            catch (DbException e)
            {
                Console.WriteLine(e);
                throw;
            }

            finally
            {
                // close the sql connection
                connection.Close();
            }

            IntTag dummy = new IntTag();
            IntTag[] tags = (IntTag[])list.ToArray(dummy.GetType());
            return tags;
        }

        public Grant[] GetProcessAgentAdminGrants(int agentID, int groupID)
        {
            ArrayList list = new ArrayList();
            DbConnection myConnection = FactoryDB.GetConnection();
            DbCommand myCommand = FactoryDB.CreateCommand("ProcessAgent_RetrieveAdminGrants", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(FactoryDB.CreateParameter( "@agentID", agentID, DbType.Int32));
            myCommand.Parameters.Add(FactoryDB.CreateParameter( "@groupID", groupID, DbType.Int32));


            try
            {
                myConnection.Open();
                DbDataReader reader = myCommand.ExecuteReader();
                while (reader.Read())
                {
                    Grant grant = new Grant();
                    grant.agentID = reader.GetInt32(0);
                    grant.function = reader.GetString(1);
                    grant.grantID = reader.GetInt32(2);
                    grant.qualifierID = reader.GetInt32(3);
                    list.Add(grant);
                }
                reader.Close();
            }
            catch
            {
            }
            finally
            {
                myConnection.Close();
            }
            Grant dummy = new Grant();
            Grant[] grants = (Grant[])list.ToArray(dummy.GetType());
            return grants;

        }



        public ResourceMapping AddResourceMapping(string keyType, object key, string[] valueTypes, object[] values)
        {
            DbConnection connection = FactoryDB.GetConnection();
            ResourceMapping mapping = null;
            try
            {

                mapping = InsertResourceMapping(connection, keyType, key, valueTypes, values);
                if (mapping != null)
                {

                    // add the new resource mapping to the static resource mappings array
                    ResourceMapManager.Add(mapping);


                }


            }
            catch (DbException sqlEx)
            {
            }
            finally
            {
                connection.Close();
            }

            return mapping;

        }



        protected ResourceMapping InsertResourceMapping(DbConnection connection, string keyType, object key, string[] valueTypes, object[] values)
        {
            if (valueTypes == null || values == null)
                throw new ArgumentException("Arguments cannot be null", "valueTypes and values");

            if (valueTypes.Length != values.Length)
                throw new ArgumentException("Parameter Arrays \"valueTypes\" and \"values\" should be of the same length");

            ResourceMappingKey mappingKey = new ResourceMappingKey(keyType, key);
            // insert key into database

            try
            {
                connection.Open();
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapKey_Insert", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Get the key type id
                int keyTypeID = ResourceMappingTypes.GetResourceMappingTypeID(keyType);
                if (keyTypeID == -1)
                    throw new ArgumentException("Value for key type is invalid");

                int keyID = -1;

                // if the key is a string, add the string to the strings table
                if (keyType.Equals(ResourceMappingTypes.STRING))
                {
                    DbCommand cmd2 = FactoryDB.CreateCommand("ResourceMapString_Insert", connection);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    cmd2.Parameters.Add(FactoryDB.CreateParameter("@string_Value", key, DbType.String, 2048));

                    keyID = Convert.ToInt32(cmd2.ExecuteScalar());
                }

                // if the key is a Resource Type, add the string to the ResourceTypes table
                else if (keyType.Equals(ResourceMappingTypes.RESOURCE_TYPE))
                {
                    DbCommand cmd2 = FactoryDB.CreateCommand("ResourceMapResourceType_Insert", connection);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.Parameters.Add(FactoryDB.CreateParameter("@resourceType_Value", key, DbType.String, 256));

                    keyID = Convert.ToInt32(cmd2.ExecuteScalar());
                }

                else
                    keyID = ResourceMappingEntry.GetId(key);

                if (keyID == -1)
                    throw new ArgumentException("Value for key is invalid");

                // populate stored procedure parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@MappingKey_Type", keyTypeID, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@MappingKey", keyID, DbType.Int32));

                // execute the command
                int mappingID = Convert.ToInt32(cmd.ExecuteScalar());

                //
                // insert mapping values
                //
                ResourceMappingValue[] mappingValues = new ResourceMappingValue[values.Length];
                for (int i = 0; i < mappingValues.Length; i++)
                {
                    mappingValues[i] = new ResourceMappingValue(valueTypes[i], values[i]);

                    // Get the value type id
                    int valueTypeID = ResourceMappingTypes.GetResourceMappingTypeID(valueTypes[i]);
                    if (valueTypeID == -1)
                        throw new ArgumentException("Value for value type \"" + i + "\" is invalid");

                    int valueID = -1;

                    // if the value is a string, add the string to the strings table
                    if (valueTypes[i].Equals(ResourceMappingTypes.STRING))
                    {
                        DbCommand cmd2 = FactoryDB.CreateCommand("ResourceMapString_Insert", connection);
                        cmd2.CommandType = CommandType.StoredProcedure;

                        // populate parameters
                        cmd2.Parameters.Add(FactoryDB.CreateParameter("@string_Value", (string)values[i], DbType.String, 2048));

                        valueID = Convert.ToInt32(cmd2.ExecuteScalar());
                    }

                    // if the key is a Resource Type, add the string to the ResourceTypes table
                    else if (valueTypes[i].Equals(ResourceMappingTypes.RESOURCE_TYPE))
                    {
                        DbCommand cmd2 = FactoryDB.CreateCommand("ResourceMapResourceType_Insert", connection);
                        cmd2.CommandType = CommandType.StoredProcedure;

                        cmd2.Parameters.Add(FactoryDB.CreateParameter("@resourceType_Value", (string)values[i], DbType.String, 256));
                        valueID = Convert.ToInt32(cmd2.ExecuteScalar());
                    }

                    else
                        valueID = ResourceMappingEntry.GetId(values[i]);

                    if (valueID == -1)
                        throw new ArgumentException("Value \"" + i + "\" is invalid");

                    cmd = FactoryDB.CreateCommand("ResourceMapValue_Insert", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Clear();

                    // populate stored procedure parameters
                    cmd.Parameters.Add(FactoryDB.CreateParameter( "@Mapping_ID", mappingID, DbType.Int32));
                    cmd.Parameters.Add(FactoryDB.CreateParameter( "@MappingValue_Type", valueTypeID, DbType.Int32));
                    cmd.Parameters.Add(FactoryDB.CreateParameter( "@MappingValue", valueID, DbType.Int32));

                    // execute the command
                    int mapValueID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // create new mapping object
                ResourceMapping mapping = new ResourceMapping(mappingID, mappingKey, mappingValues);



                return mapping;
            }

            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
        }

        public ResourceMapping AddResourceMapping(ResourceMappingKey key, ResourceMappingValue[] values)
        {
            string[] valueTypes = new string[values.Length];
            object[] valueObjs = new object[values.Length];

            for (int i = 0; i < valueTypes.Length; i++)
            {
                valueTypes[i] = values[i].type;
                valueObjs[i] = values[i].entry;
            }

            return AddResourceMapping(key.type, key.entry, valueTypes, valueObjs);
        }


        /// <summary>
        /// Deletes the resourceMapping and any qualifiers and grants related to the mapping
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(ResourceMapping mapping)
        {
            return DeleteResourceMapping(mapping.MappingID);
        }

        /// <summary>
        /// Deletes the resourceMapping and any qualifiers and grants related to the mapping 
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(int mappingId)
        {
            bool status = false;
            DbConnection connection = FactoryDB.GetConnection();

            try
            {

                // Check for any Qualifiers created for this RM
                int qualifierId = AuthorizationAPI.GetQualifierID(mappingId, Qualifier.resourceMappingQualifierTypeID);
                if (qualifierId > 0)
                {
                    // Any grant associated with this qualifier are removed via a cascading delete
                    AuthorizationAPI.RemoveQualifiers(new int[] { qualifierId });
                }

                DbCommand cmd = FactoryDB.CreateCommand("ResourceMap_Delete", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@mapping_ID", mappingId, DbType.Int32));

                connection.Open();
                cmd.ExecuteNonQuery();

                // Remove the Deleted ResourceMapping
                ResourceMapManager.Remove(mappingId);
                status = true;

                // update resource mappings array
                // find index of deleted mapping
                //int i = 0;
                //while (i < resourceMappings.Length)
                //{
                //    if (resourceMappings[i].MappingID == mappingId)
                //        break;
                //    i++;
                //}
                //// delete the mapping that ws found from the array
                //ResourceMapping[] temp = new ResourceMapping[resourceMappings.Length - 1];
                //Array.Copy(resourceMappings, 0, temp, 0, i);
                //Array.Copy(resourceMappings, i + 1, temp, i, resourceMappings.Length - i - 1);
                //resourceMappings = temp;

            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }

            return status;
        }

        /// <summary>
        /// Deletes the resourceMapping and any qualifiers and grants related to the mapping 
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(string keyType, int keyId, string valueType, int valueId)
        {
            return DeleteResourceMapping(ResourceMappingTypes.GetResourceMappingTypeID(keyType), keyId,
                ResourceMappingTypes.GetResourceMappingTypeID(valueType), valueId);
        }


        /// <summary>
        /// Deletes the resourceMapping and any qualifiers and grants related to the mapping 
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(int keyTypeId, int keyId, int valueTypeId, int valueId)
        {
            bool status = false;
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                int mapId = -1;
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapId_RetrieveByKeyValue", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@keyID", keyId, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@keyType", keyTypeId, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@valueID", valueId, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@valueType", valueTypeId, DbType.Int32));

                connection.Open();
                mapId = (int)cmd.ExecuteScalar();
                if (mapId > 0)
                {
                    status = DeleteResourceMapping(mapId);
                }
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
            return status;
        }

        public ResourceMappingKey GetResourceMappingKey(int mappingID)
        {
            ResourceMapping mapping = ResourceMapManager.GetMap(mappingID);
            if (mapping != null)
                return mapping.key;
            else
                return null;

        }

        public ResourceMapping GetResourceMapping(int mappingID)
        {
            return ResourceMapManager.GetMap(mappingID);
        }

        public int[] GetResourceMappingIdsByValue(string type, int value)
        {
            List<int> list = new List<int>();
            DbConnection connection = FactoryDB.GetConnection();
            DbCommand cmd = FactoryDB.CreateCommand("ResourceMapIDs_RetrieveByValue", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@type", type, DbType.AnsiString, 256));
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@id", value, DbType.Int32));

            // execute the command
            try
            {
                DbDataReader dataReader = null;
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    if (!DBNull.Value.Equals(dataReader.GetValue(0)))
                        list.Add(dataReader.GetInt32(0));
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return list.ToArray();
        }

        /// <summary>
        /// Reads a resource mapping from the database
        /// </summary>
        /// <param name="mappingID">id of the resource mapping</param>
        /// <returns>ResourceMapping object</returns>
        public ResourceMapping ReadResourceMapping(int mappingID)
        {
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                connection.Open();
                ResourceMapping mapping = ReadResourceMapping(mappingID, connection);

                return mapping;
            }

            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
        }

        public ResourceMapping ReadResourceMapping(int mappingID, DbConnection connection)
        {
            DbDataReader dataReader = null;
            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMap_RetrieveByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@mappingID", mappingID, DbType.Int32));

                // execute the command
                dataReader = cmd.ExecuteReader();

                // first row is (key_type, key)
                dataReader.Read();
                int keyTypeID = -1, keyID = -1;
                if (!DBNull.Value.Equals(dataReader.GetValue(0)))
                    keyTypeID = dataReader.GetInt32(0);
                if (!DBNull.Value.Equals(dataReader.GetValue(1)))
                    keyID = dataReader.GetInt32(1);
                ResourceMappingKey key = (ResourceMappingKey)CompleteResourceMappingKeyRead(connection, keyTypeID, keyID, true);

                // subsequent rows are (value_type, value)
                ResourceMappingValue value = null;
                ArrayList valuesList = new ArrayList();
                while (dataReader.Read())
                {
                    int valueTypeID = -1, valueID = -1;
                    if (!DBNull.Value.Equals(dataReader.GetValue(0)))
                        valueTypeID = dataReader.GetInt32(0);
                    if (!DBNull.Value.Equals(dataReader.GetValue(1)))
                        valueID = dataReader.GetInt32(1);
                    value = (ResourceMappingValue)CompleteResourceMappingKeyRead(connection, valueTypeID, valueID, false);
                    valuesList.Add(value);
                }
                if (valuesList.Count > 0)
                {
                    ResourceMappingValue[] values = (ResourceMappingValue[])valuesList.ToArray(value.GetType());

                    // construct resource mapping
                    return new ResourceMapping(mappingID, key, values);
                }
                else
                {
                    return null;
                }
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }
            finally
            {
                dataReader.Close();
            }
        }

        /// <summary>
        /// Return all resource mappings in the database.
        /// If resource mappings array is not null, return it.
        /// Otherwise read resource mappings list from database and return it.
        /// </summary>
        /// <returns></returns>
        //public ResourceMapping[] GetResourceMappings()
        //{          
        //    // check if resourcemappings have been initialized before, in which case they do not need to be re-read from the DB
        //    if (resourceMappings != null)
        //        return resourceMappings;

        //    // otherwise, read mappings from the database    
        //    List<ResourceMapping> mappingList = null;
        //    mappingList = RetrieveResourceMapping();

        //    if (mappingList != null)
        //        resourceMappings = mappingList.ToArray();

        //    else
        //        resourceMappings = new ResourceMapping[0];
        //    return resourceMappings;
        //}

        public List<ResourceMapping> RetrieveResourceMapping()
        {
            // Read mappings from the database            
            DbConnection connection = FactoryDB.GetConnection();
            DbCommand cmd = FactoryDB.CreateCommand("ResourceMapIDs_Retrieve", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            ResourceMapping mapping = null;
            List<ResourceMapping> mappingList = new List<ResourceMapping>();
            connection.Open();
            // execute the command
            DbDataReader dataReader = null;
            dataReader = cmd.ExecuteReader();

            // read mapping ID's
            ArrayList mappingIDs = new ArrayList();
            while (dataReader.Read())
            {
                int mappingID = -1;
                if (!DBNull.Value.Equals(dataReader.GetValue(0)))
                {
                    mappingID = dataReader.GetInt32(0);
                    mappingIDs.Add(mappingID);
                }
            }

            dataReader.Close();
            connection.Close();


            // read mappings
            int ii = 0;
            int[] ids = (int[])mappingIDs.ToArray(ii.GetType());
            for (int i = 0; i < ids.Length; i++)
            {
                mapping = ReadResourceMapping(ids[i]);
                mappingList.Add(mapping);
            }
            if (mappingList.Count > 0)
            {
                return mappingList;
            }
            else
                return null;
        }


        /// <summary>
        /// Complete reading a resource mapping given the mapping id as well an entry id. The entry is could be a key or a value.
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="entryID"></param>
        /// <param name="isKey">Determines whether the method should return a key or a value</param>
        /// <returns></returns>
        private ResourceMappingEntry CompleteResourceMappingKeyRead(DbConnection connection, int typeID, int entryID, bool isKey)
        {
            // get the entry type
            string type = ResourceMappingTypes.GetResourceMappingType(typeID);

            // construct the entry object based on the entry type
            object entry = null;
            if (type.Equals(ResourceMappingTypes.PROCESS_AGENT))
                // read from process agent table
                entry = entryID;
            else if (type.Equals(ResourceMappingTypes.CLIENT))
                // copy client ID
                entry = entryID;
            else if (type.Equals(ResourceMappingTypes.RESOURCE_MAPPING))
                // copy resource mapping ID
                entry = entryID;
            else if (type.Equals(ResourceMappingTypes.STRING))
                // read string from string table
                entry = GetResourceMappingString(entryID);

            else if (type.Equals(ResourceMappingTypes.RESOURCE_TYPE))
                // read string from Resource Types table
                entry = GetResourceMappingResourceType(entryID);

            else if (type.Equals(ResourceMappingTypes.TICKET_TYPE))
                // read the ticket type from the tyckettypes class
                entry = TicketTypes.GetTicketType(entryID);
            else if (type.Equals(ResourceMappingTypes.GROUP))
                // copy group id
                entry = entryID;

            if (isKey)
                return new ResourceMappingKey(type, entry);
            else
                return new ResourceMappingValue(type, entry);
        }

        /// <summary>
        /// Read a resource mapping string from the database
        /// </summary>
        /// <param name="strID"></param>
        /// <returns></returns>
        public String GetResourceMappingString(int strID)
        {
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapString_RetrieveByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@ID", strID, DbType.Int32));

                connection.Open();
                return Convert.ToString(cmd.ExecuteScalar());
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
        }

        public int AddResourceMappingString(string s)
        {
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapString_Insert", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@string_Value", s, DbType.String, 2048));

                connection.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());

            }

            finally
            {
                connection.Close();
            }

        }

        public int UpdateResourceMappingString(int id, string s)
        {
            DbConnection connection = FactoryDB.GetConnection();
            int mappingID = 0;
            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapString_Update", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@id", id, DbType.Int32));
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@string", s, DbType.String, 2048));

                connection.Open();
                mappingID = Convert.ToInt32(cmd.ExecuteScalar());
                if (mappingID > 0)
                {
                    ResourceMapping rm = ReadResourceMapping(mappingID, connection);
                    ResourceMapManager.Update(rm);
                }
            }

            finally
            {
                connection.Close();
            }
            return mappingID;

        }

        public int AddResourceMappingResourceType(string s)
        {
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapResourceType_Insert", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@resourceType_Value", s, DbType.String, 256));

                connection.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());

            }

            finally
            {
                connection.Close();
            }

        }

        public String GetResourceMappingResourceType(int resourceTypeID)
        {
            DbConnection connection = FactoryDB.GetConnection();

            try
            {
                DbCommand cmd = FactoryDB.CreateCommand("ResourceMapType_RetrieveByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter( "@ID", resourceTypeID, DbType.Int32));

                connection.Open();
                return Convert.ToString(cmd.ExecuteScalar());
            }
            catch (DbException e)
            {
                writeEx(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        //Returns a hashtable of (mappingID, ResourceMappingValue[]) pairs
        //public Hashtable GetResourceMappingsForKey(object searchKey, string type)
        //{
        //    ResourceMappingKey key = null;

        //    if (type.Equals(ResourceMappingTypes.CLIENT))
        //    {
        //        key = new ResourceMappingKey(type, (int)searchKey);
        //    }
        //    else if (type.Equals(ResourceMappingTypes.PROCESS_AGENT))
        //    {
        //        key = new ResourceMappingKey(type, (int)searchKey);
        //    }
        //    else if (type.Equals(ResourceMappingTypes.TICKET_TYPE))
        //    {
        //        key = new ResourceMappingKey(type, (TicketType)searchKey);
        //    }
        //    else if (type.Equals(ResourceMappingTypes.GROUP))
        //    {  
        //        key = new ResourceMappingKey(type, (int)searchKey);
        //    }


        //    List<ResourceMapping> list = ResourceMapManager.Get(key);
        //    if (list != null && list.Count > 0)
        //    {
        //        Hashtable mappingsTable = new Hashtable();
        //        foreach (ResourceMapping rm in list)
        //        {
        //            mappingsTable.Add(rm.MappingID, rm);
        //        }
        //        return mappingsTable;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //Gets the resource mapping values as a 2D array from a mappings HashTable
        public ResourceMappingValue[][] GetResourceMappingValues(Hashtable mappingsTable)
        {
            if (mappingsTable == null || mappingsTable.Count == 0)
                return null;

            ResourceMappingValue[][] values = new ResourceMappingValue[mappingsTable.Count][];
            int i = 0;
            foreach (DictionaryEntry entry in mappingsTable)
            {
                values[i++] = ((ResourceMapping)entry.Value).values;

            }
            return values;
        }

        public Hashtable GetResourceStringTags(ResourceMappingKey key)
        {
            return GetResourceStringTags((int)key.Entry, key.Type);
        }

        public Hashtable GetResourceStringTags(int target, string rmType)
        {

            return GetResourceStringTags(target, ResourceMappingTypes.GetResourceMappingTypeID(rmType));
        }

        public Hashtable GetResourceStringTags(int target, int type)
        {
            Hashtable resources = null;
            Hashtable results = null;

            DbConnection connection = FactoryDB.GetConnection();
            DbCommand cmd = FactoryDB.CreateCommand("ResourceMapTypeStrings_Retrieve", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters

            cmd.Parameters.Add(FactoryDB.CreateParameter( "@type", type, DbType.Int32));
            cmd.Parameters.Add(FactoryDB.CreateParameter( "@target", target, DbType.Int32));

            try
            {
                connection.Open();
                DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    resources = new Hashtable();
                    int mid = 0;
                    while (reader.Read())
                    {
                        mid = reader.GetInt32(0);
                        resources.Add(mid, reader.GetString(1));
                    }
                    if (reader.NextResult())
                    {
                        results = new Hashtable();
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            if (resources.ContainsKey(id))
                            {
                                IntTag tag = new IntTag();
                                tag.id = reader.GetInt32(1);
                                tag.tag = reader.GetString(2);
                                results.Add(resources[id], tag);
                            }
                        }
                    }
                }
                reader.Close();
            }

            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                connection.Close();

            }
            if (results != null && results.Count > 0)
                return results;
            else
                return null;
        }


        /// <summary>
        /// Find a ResourceMapping entry, given a matrix of values
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public object FindResourceEntry(ResourceMappingValue searchValue, ResourceMappingValue[][] values, string dataType)
        {
            if (values == null || values.Length == 0 || searchValue == null)
                return null;

            object target = null;
            bool found = false;
            int row = 0;

            //the number of "set of values" (array of values) associated with this client
            int numSetOfValues = values.GetLength(0);

            for (row = 0; row < numSetOfValues && !found; row++)
            {
                int numValues = values[row].Length;
                for (int column = 0; column < numValues; column++)
                {
                    if (values[row][column].Equals(searchValue))
                    {
                        found = true;
                        break;
                    }
                }
            }
            if (found)
            {
                ResourceMappingValue[] mappingValue = values[row - 1];
                for (int i = 0; i < mappingValue.Length; i++)
                {
                    if (mappingValue[i].Type.Equals(dataType))
                    {
                        target = mappingValue[i].Entry;
                        break;
                    }
                }
            }
            return target;
        }
        /// <summary>
        /// Creates the resourceMapping for this relationship and adds a qualifier.
        /// </summary>
        /// <param name="lsId"></param>
        /// <param name="lssId"></param>
        /// <returns>the qualifierID</returns>
        public int AssociateLSS(int lsId, int lssId)
        {
            Object keyObj = lsId;
            string keyType = ResourceMappingTypes.PROCESS_AGENT;

            ArrayList valuesList = new ArrayList();
            Object valueObj = null;

            ResourceMappingValue value = new ResourceMappingValue(ResourceMappingTypes.RESOURCE_TYPE,
                ProcessAgentType.LAB_SCHEDULING_SERVER);
            valuesList.Add(value);

            value = new ResourceMappingValue(ResourceMappingTypes.PROCESS_AGENT,
                lssId);
            valuesList.Add(value);

            value = new ResourceMappingValue(ResourceMappingTypes.TICKET_TYPE,
                TicketTypes.GetTicketType(TicketTypes.MANAGE_LAB));
            valuesList.Add(value);

            ResourceMappingKey key = new ResourceMappingKey(keyType, keyObj);
            ResourceMappingValue[] values = (ResourceMappingValue[])valuesList.ToArray((new ResourceMappingValue()).GetType());
            ResourceMapping newMapping = AddResourceMapping(key, values);

            // add mapping to qualifier list
            int qualifierType = Qualifier.resourceMappingQualifierTypeID;
            string name = ResourceMappingToString(newMapping);
            int qualifierID = AuthorizationAPI.AddQualifier(newMapping.MappingID, qualifierType, name, Qualifier.ROOT);

            // Should a grant be created here

            return qualifierID;
        }

        public int FindProcessAgentIdForAgent(int keyId, string type)
        {
            int result = -1;
            ResourceMappingKey key = new ResourceMappingKey(ResourceMappingTypes.PROCESS_AGENT, keyId);
            ResourceMappingValue[] search = new ResourceMappingValue[1];
            search[0] = new ResourceMappingValue(ResourceMappingTypes.RESOURCE_TYPE, type);
            List<ResourceMapping> found = ResourceMapManager.Find(key, search);
            if (found != null && found.Count > 0)
            {
                foreach (ResourceMapping rm in found)
                {
                    for (int i = 0; i < rm.values.Length; i++)
                    {
                        if (rm.values[i].type.Equals(ResourceMappingTypes.PROCESS_AGENT))
                        {
                            result = (int)rm.values[i].entry;
                            break;
                        }
                    }
                }

            }

            //Hashtable mappingsTable = GetResourceMappingsForKey(keyId, ResourceMappingTypes.PROCESS_AGENT);
            //if (mappingsTable != null)
            //{
            //    ResourceMappingValue[][] values = GetResourceMappingValues(mappingsTable);
            //    result = FindProcessAgentIdForLS(keyId, values,
            //        ProcessAgentType.LAB_SCHEDULING_SERVER);
            //}
            return result;
        }

        /// <summary>
        /// Find a Process Agent (an LSS) associated with a particular LS, given a matrix of values
        /// </summary>
        /// <param name="lsId"></param>
        /// <param name="values"></param>
        /// <param name="processAgentType"></param>
        /// <returns></returns>
        //public int FindProcessAgentIdForLS(int lsId, ResourceMappingValue[][] values,
        //    string processAgentType)
        //{
        //    int paId = 0;
        //    if (values == null || values.Length == 0 || lsId == 0)
        //        return paId;

        //    ResourceMappingValue searchValue = null;


        //    if (processAgentType.Equals(ProcessAgentType.LAB_SCHEDULING_SERVER))
        //        searchValue = new ResourceMappingValue(ResourceMappingTypes.RESOURCE_TYPE, ProcessAgentType.LAB_SCHEDULING_SERVER);

        //    bool foundProcessAgent = false;
        //    int row = 0;

        //    //the number of "set of values" (array of values) associated with this client
        //    int numSetOfValues = values.GetLength(0);

        //    for (row = 0; row < numSetOfValues && !foundProcessAgent; row++)
        //    {
        //        int numValues = values[row].Length;
        //        for (int column = 0; column < numValues; column++)
        //        {
        //            if (values[row][column].Equals(searchValue))
        //            {
        //                foundProcessAgent = true;
        //                break;
        //            }
        //        }
        //    }

        //    if (foundProcessAgent)
        //    {

        //        ResourceMappingValue[] associatedPA = values[row - 1];
        //        for (int i = 0; i < associatedPA.Length; i++)
        //        {
        //            if (associatedPA[i].Type.Equals(ResourceMappingTypes.PROCESS_AGENT))
        //            {
        //                paId = (int)associatedPA[i].Entry;
        //                break;
        //            }
        //        }
        //    }

        //    return paId;
        //}


        /// <summary>
        /// Finds an USS or ESS associated with a particular client, given an matrix of values
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="values"></param>
        /// <param name="processAgentType"></param>
        /// <returns></returns>
        public int FindProcessAgentIdForClient(int clientID, string processAgentType)
        {
            int paId = 0;

            ResourceMappingKey key = new ResourceMappingKey(ResourceMappingTypes.CLIENT, clientID);
            ResourceMappingValue[] searchValue = new ResourceMappingValue[1];


            if (processAgentType.Equals(ProcessAgentType.SCHEDULING_SERVER))
                searchValue[0] = new ResourceMappingValue(ResourceMappingTypes.RESOURCE_TYPE, ProcessAgentType.SCHEDULING_SERVER);
            else if (processAgentType.Equals(ProcessAgentType.EXPERIMENT_STORAGE_SERVER))
                searchValue[0] = new ResourceMappingValue(ResourceMappingTypes.RESOURCE_TYPE, ProcessAgentType.EXPERIMENT_STORAGE_SERVER);
            List<ResourceMapping> found = ResourceMapManager.Find(key, searchValue);

            if (found != null && found.Count > 0)
            {
                foreach (ResourceMapping rm in found)
                {
                    for (int i = 0; i < rm.values.Length; i++)
                    {
                        if (rm.values[i].type.Equals(ResourceMappingTypes.PROCESS_AGENT))
                        {
                            paId = (int)rm.values[i].entry;
                            break;
                        }
                    }
                }

            }



            //bool foundProcessAgent = false;
            //int row = 0;

            ////the number of "set of values" (array of values) associated with this client
            //int numSetOfValues = values.GetLength(0);

            //for (row = 0; row < numSetOfValues && !foundProcessAgent; row++)
            //{
            //    int numValues = values[row].Length;
            //    for (int column = 0; column < numValues; column++)
            //    {
            //        if (values[row][column].Equals(searchValue))
            //        {
            //            foundProcessAgent = true;
            //            break;
            //        }
            //    }
            //}

            //if (foundProcessAgent)
            //{

            //    ResourceMappingValue[] associatedPA = values[row - 1];
            //    for (int i = 0; i < associatedPA.Length; i++)
            //    {
            //        if (associatedPA[i].Type.Equals(ResourceMappingTypes.PROCESS_AGENT))
            //        {
            //            paId = (int)associatedPA[i].Entry;
            //            break;
            //        }
            //    }
            //}

            return paId;


        }

        /// <summary>
        /// Checks if an array of Resource Mapping values is Equal to another one
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public bool EqualMappingValues(ResourceMappingValue[] v1, ResourceMappingValue[] v2)
        {
            int num1Values = v1.GetLength(0);
            int num2Values = v2.GetLength(0);

            if (num1Values != num2Values)
                return false;

            bool areNotEqual = false;
            bool areEqual = false;

            for (int i = 0; i < num1Values; i++)
            {
                if (!v1[i].Equals(v2[i]))
                {
                    areNotEqual = true;
                    break;
                }
            }

            //for (int i = 0; i < num1Values; i++)
            //{
            //    areEqual = false;

            //    for (int j = 0; j < num2Values; j++)
            //    {
            //        if (v1[i].Equals(v1[j]))
            //        {
            //            areEqual = true;
            //            break;
            //        }
            //    }

            //    if (areEqual == false)
            //        break;
            //}

            //return (areEqual);

            return (!areNotEqual);
        }

        // THis is not supported, an attempt to re-do resources using int's instead of strings
        //public int InsertResourceMap(int keyType, int keyValue,
        //    int type0, object value0, int type1, object value1, int type2, object value2)
        //{
        //    int id = -1;
        //    DbConnection connection  = CreateConnection();
        //      // command executes the "InsertResourceMap" stored procedure
        //        DbCommand cmd = FactoryDB.CreateCommand("InsertResourceMap", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        DbParameter paramType = null;
        //        DbParameter paramValue = null;

        //        // Need to do this in pairs 
        //        paramType = cmd.Parameters.Add(FactoryDB.CreateParameter("@keyType", DbType.Int);
        //        paramValue = cmd.Parameters.Add(FactoryDB.CreateParameter("@keyValue", DbType.Int);
        //        paramType.Value = keyType; 
        //        paramValue.Value = keyValue;
        //        paramType = cmd.Parameters.Add(FactoryDB.CreateParameter("@Type0", DbType.Int);
        //        paramValue = cmd.Parameters.Add(FactoryDB.CreateParameter("@value0", DbType.Int);
        //        paramType.Value = type0;
        //        paramValue.Value = value0;

        //    paramType = cmd.Parameters.Add(FactoryDB.CreateParameter("@type1", DbType.Int);
        //    paramValue = cmd.Parameters.Add(FactoryDB.CreateParameter("@value1", DbType.Int);
        //    if(ResourceMap.IsResourceMapType(type1)){
        //        paramType.Value = type1;   
        //        paramValue.Value = value1;
        //    }
        //    else{
        //        paramType.Value = DBNull.Value;   
        //        paramValue.Value =  DBNull.Value;
        //    }
        //    paramType = cmd.Parameters.Add(FactoryDB.CreateParameter("@type2", DbType.Int);
        //    paramValue = cmd.Parameters.Add(FactoryDB.CreateParameter("@value2", DbType.Int);
        //   if(ResourceMap.IsResourceMapType(type2)){
        //        paramType.Value = type2;   
        //        paramValue.Value = value2;
        //    }
        //    else{
        //        paramType.Value = DBNull.Value;   
        //        paramValue.Value =  DBNull.Value;
        //    }
        //    try
        //    {
        //        // execute the command
        //        id = Convert.ToInt32(cmd.ExecuteScalar());
        //    }
        //    catch (DbException e)
        //    {
        //        writeEx(e);
        //        throw;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //    return id;
        //}

        public string ResourceMappingToString(ResourceMapping mapping)
        {
            StringBuilder s = new StringBuilder();
            //s.Append(mapping.MappingID + " ");
            s.Append(GetMappingEntryString(mapping.key, true) + "-> ");

            //if (mapping.values.Length > 1)
            //    s.Append("(");

            // print all values except last
            for (int i = 0; i < mapping.values.Length; i++)
            {
                if (i > 0 && i < mapping.values.Length)
                    s.Append(", ");
                s.Append(GetMappingEntryString(mapping.values[i], true));

            }

            //// print last value
            //if (mapping.values[mapping.values.Length - 1].Type.Equals(ResourceMappingTypes.RESOURCE_MAPPING))
            //    s.Append("(" + mapping.values[mapping.values.Length - 1] + ":" + mapping.values[mapping.values.Length - 1].Entry + ")");
            //else
            //    s.Append("(" + mapping.values[mapping.values.Length - 1].TypeName + ":" + GetMappingEntryString(mapping.values[mapping.values.Length - 1]) + ")");

            //if (mapping.values.Length > 1)
            //    s.Append(")");
            return s.ToString();
        }



        public string GetMappingEntryString(ResourceMappingEntry entry, bool showType)
        {
            StringBuilder buf = new StringBuilder();
            Object o = entry.Entry;

            if (entry == null)
            {
                buf.Append("Entry is null, NOT FOUND");
            }
            else if (entry.Type.Equals(ResourceMappingTypes.PROCESS_AGENT))
            {
                string name = null;
                //if (showType)
                //    name = GetProcessAgentNameWithType((int)o);
                //else
                name = GetProcessAgentName((int)o);
                if (name != null)
                    buf.Append(name);
                else
                    buf.Append("Process Agent not found");
            }
            else if (entry.Type.Equals(ResourceMappingTypes.CLIENT))
            {
                LabClient[] labClients = AdministrativeAPI.GetLabClients(new int[] { (int)o });
                if (labClients.Length == 1)
                {
                    if (showType)
                        buf.Append("Client: ");
                    buf.Append(labClients[0].ClientName);

                }
                else
                {
                    buf.Append("Client not found");
                }
            }

            else if (entry.Type.Equals(ResourceMappingTypes.RESOURCE_MAPPING))
            {
                if (showType)
                    buf.Append("RM: ");
                buf.Append(ResourceMappingToString(GetResourceMapping((int)o)));
            }
            else if (entry.Type.Equals(ResourceMappingTypes.STRING))
            {
                if (showType)
                    buf.Append("String: ");
                buf.Append((string)o);
            }
            else if (entry.Type.Equals(ResourceMappingTypes.RESOURCE_TYPE))
            {
                if (showType)
                    buf.Append("RT: ");
                string type = (string)o;
                if (type.Equals(ProcessAgentType.EXPERIMENT_STORAGE_SERVER))
                    buf.Append("ESS");
                else if (type.Equals(ProcessAgentType.LAB_SCHEDULING_SERVER))
                    buf.Append("LSS");
                else if (type.Equals(ProcessAgentType.SCHEDULING_SERVER))
                    buf.Append("USS");
                else if (type.Equals(ProcessAgentType.LAB_SERVER))
                    buf.Append("LS");
                else
                {
                    buf.Append(type);
                }
            }
            else if (entry.Type.Equals(ResourceMappingTypes.TICKET_TYPE))
            {
                if (showType)
                    buf.Append("TT: ");
                buf.Append(((TicketType)o).shortDescription);
            }
            else if (entry.Type.Equals(ResourceMappingTypes.GROUP))
            {

                Group[] groups = AdministrativeAPI.GetGroups(new int[] { (int)o });
                if (groups.Length == 1)
                {

                    if (showType)
                        buf.Append("Group: ");
                    buf.Append(groups[0].GroupName);
                }
                else
                {
                    buf.Append("Group not found");
                }
            }
            if (buf.Length == 0)
            {
                buf.Append("Entry not Found");
            }
            return buf.ToString(); ;
        }


        //public string GetMappingString(ResourceMapping mapping)
        //{
        //    String s = mapping.MappingID.ToString();
        //    s += " (" + mapping.key.TypeName + ":" + GetMappingEntryString(mapping.key) + ")";
        //    s += "-->";

        //    if (mapping.values.Length > 1)
        //        s += "(";

        //    // print all values except last
        //    for (int i = 0; i < mapping.values.Length - 1; i++)
        //    {
        //        ResourceMappingValue value = mapping.values[i];
        //        if (value.Type.Equals(ResourceMappingTypes.RESOURCE_MAPPING))
        //            s += "(" + value.TypeName + ":" + value.Entry + "), ";
        //        else
        //            s += "(" + value.TypeName + ":" + GetMappingEntryString(value) + "), ";
        //    }

        //    // print last value
        //    if (mapping.values[mapping.values.Length - 1].Type.Equals(ResourceMappingTypes.RESOURCE_MAPPING))
        //        s += "(" + mapping.values[mapping.values.Length - 1] + ":" + mapping.values[mapping.values.Length - 1].Entry + ")";
        //    else
        //        s += "(" + mapping.values[mapping.values.Length - 1].TypeName + ":" + GetMappingEntryString(mapping.values[mapping.values.Length - 1]) + ")";

        //    if (mapping.values.Length > 1)
        //        s += ")";
        //    return s;
        //}
        /*
                public int InsertRegisterRecord(int couponId, string couponGuid, string registerGuid,
                    string sourceGuid, int status, string email, string descriptor)
                {
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("InsertRegistration", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter idParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@couponId", DbType.Int );
                    idParam.Value = couponId;
                    DbParameter couponGuidParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@couponGuid", DbType.AnsiString,50 );
                    couponGuidParam.Value = couponGuid;
                    DbParameter registerGuidParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@registerGuid", DbType.Varchar,50);
                    registerGuidParam.Value = registerGuid;
                    DbParameter sourceParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@sourceGuid", DbType.Varchar,50);
                    sourceParam.Value = sourceGuid;
                    DbParameter statusParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@status", DbType.Int );
                    statusParam.Value = status;
                    DbParameter emailParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@email", DbType.AnsiString,256 );
                    emailParam.Value = email;
                    DbParameter descriptorParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@descriptor", DbType.Text );
                    descriptorParam.Value = descriptor;
                    try
                    {
            
                        return Convert.ToInt32(cmd.ExecuteScalar());

                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }

                public string[] SelectRegisterGuids()
                {
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("InsertRegistration", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter Param = cmd.Parameters.Add(FactoryDB.CreateParameter("@", DbType.AnsiString);
                    Param.Value = (string)s;
                    try
                    {

                        DbDataReader dataReader = null;
                        dataReader = cmd.ExecuteReader();

                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }


                }

                protected RegisterRecord readRegisterRecord(DbDataReader reader){
                    RegisterRecord record = new RegisterRecord();
            
                    record.recordId = reader.GetInt32(0);
                    if(!reader.IsDBNull(1))
                        record.couponId = reader.GetInt32(1);
                    if(!reader.IsDBNull(2))
                        record.couponGuid = reader.GetString(2);
                    if(!reader.IsDBNull(3))
                        record.registerGuid = reader.GetString(3);
                    if(!reader.IsDBNull(4))
                        record.sourceGuid = reader.GetString(4);
                    if(!reader.IsDBNull(5))
                        record.status = reader.GetInt32(5);
                   record.create =  DateUtil.SpecifyUTC(reader.GetDateTime(6));
                     record.lastModified = DateUtil.SpecifyUTC(reader.GetDateTime(7));
                     if(!reader.IsDBNull(8))
                        record.descriptor = reader.GetString(8);
                     if(!reader.IsDBNull(9))
                        record.email = reader.GetString(9);
                    return record;
                }

                public RegisterRecord SelectRegisterRecord(int id)
                {
                    RegisterRecord record = null;
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("SelectRegistrationRecord", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter Param = cmd.Parameters.Add(FactoryDB.CreateParameter("@id", DbType.Int);
                    Param.Value = id;
                    try
                    {
                        DbDataReader dataReader = null;
                        dataReader = cmd.ExecuteReader();
                        while(dataReader.Read()){
                            record = readRegisterRecord(dataReader);


                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }


                }

                public RegisterRecord[] SelectRegister(string registerGuid)
                {
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("InsertRegistration", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter Param = cmd.Parameters.Add(FactoryDB.CreateParameter("@", DbType.AnsiString);
                    Param.Value = (string)s;
                    try
                    {

                        return Convert.ToInt32(cmd.ExecuteScalar());

                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }

                public RegisterRecord[] SelectRegisterByStatus(int status)
                {
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("InsertRegistration", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter Param = cmd.Parameters.Add(FactoryDB.CreateParameter("@", DbType.AnsiString);
                    Param.Value = (string)s;
                    try
                    {

                        return Convert.ToInt32(cmd.ExecuteScalar());

                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
                public RegisterRecord[] SelectRegisterByStatus(int low, int high)
                {
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("InsertRegistration", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter Param = cmd.Parameters.Add(FactoryDB.CreateParameter("@", DbType.AnsiString);
                    Param.Value = (string)s;
                    try
                    {

                        return Convert.ToInt32(cmd.ExecuteScalar());

                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }


                }

                public int SetRegisterStatus(int id, int status)
                {
                    DbConnection connection = CreateConnection();
                    DbCommand cmd = FactoryDB.CreateCommand("UpdateRegistrationStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    DbParameter idParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@id", DbType.Int);
                    idParam.Value = id;
                    DbParameter statusParam = cmd.Parameters.Add(FactoryDB.CreateParameter("@status", DbType.Int);
                    statusParam.Value = status;
                    try
                    {

                        return cmd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                       Logger.WriteLine(e.Message);
                    }

                    finally
                    {
                        connection.Close();
                    }


                }
        */

        /// <summary>
        /// This examines the specified parameters to resolve the available resources for the user
        /// This may only be called after a user is Authenticated.
        /// </summary>
        public int ResolveResources(HttpContext context, string authStr, string userName, string groupName, string serviceGuid, string clientGuid, bool allGroups,
            ref StringBuilder message, out int userID, out Dictionary<string, int[]> groupClientsMap)
        {
            int status = -1;
            int groupID = 0;
            int clientID = 0;
            List<int> grpIDs = new List<int>();
            List<int> clientIds = new List<int>();
            groupClientsMap = new Dictionary<string, int[]>();
            userID = 0;
            if (userName != null && userName.Length > 0)
            {
                status = 0;
                // Note not using the authStr since all users are local at this time. Will need to create logic for checking authentication.
                //if (authStr == null || authStr.Length == 0 || (authStr.CompareTo(ProcessAgentDB.ServiceGuid) == 0))
                //{
                    // Local user
                    userID = AdministrativeAPI.GetUserID(userName);
                    
                //}
            }
            else
            {
                message.Append(" No user was specified!");
                return status;
            }
            if (userID > 0)
            {
                status = 0;
                if (groupName != null && groupName.Length > 0)
                {
                    groupID = AdministrativeAPI.GetGroupID(groupName);
                    if (groupID > 0)
                    {
                        if (AdministrativeAPI.IsAgentMember(userID, groupID))
                        {
                            grpIDs.Add(groupID);
                            //status = 1;
                        }
                        else
                        {
                            message.AppendLine(" The specified user is not a member of the specified group!");
                        }
                    }
                }
                else
                {
                    grpIDs.AddRange(AdministrativeAPI.ListGroupsForAgentRecursively(userID));
                }
                
                if (clientGuid != null && clientGuid.Length > 0)
                {
                    clientID = AdministrativeAPI.GetLabClientID(clientGuid);
                    // Check that the client may be run from an authorized group
                    if (clientID > 0)
                    {
                        int[] clientGroups = AdministrativeUtilities.GetLabClientGroups(clientID);
                        if (clientGroups != null && clientGroups.Length > 0)
                        {
                            foreach (int id in clientGroups)
                            {
                                if (grpIDs.Contains(id))
                                {
                                    groupClientsMap.Add(AdministrativeAPI.GetGroupName(id), new int[] { clientID });
                                    status++;
                                }
                            }
                        }

                    }
                }
                else
                {
                    Group[] groups = AdministrativeAPI.GetGroups(grpIDs.ToArray());
                    foreach (Group g in groups)
                    {
                        if (    (g.groupType.CompareTo(GroupType.REGULAR) == 0)
                            || (allGroups && ((g.groupType.CompareTo(GroupType.COURSE_STAFF) == 0)
                                || (g.groupType.CompareTo(GroupType.SERVICE_ADMIN) == 0)) ||(g.groupName.CompareTo("SuperUserGroup") ==0) ))
                        {
                            int[] cIDs = AdministrativeUtilities.GetGroupLabClients(g.groupID);
                            if (cIDs != null && cIDs.Length > 0)
                            {
                                groupClientsMap.Add(g.groupName, cIDs);
                                status += cIDs.Length;
                            }
                            if (allGroups && (cIDs == null || cIDs.Length == 0))
                            {
                                groupClientsMap.Add(g.groupName, new int[] { });
                            }
                        }
                    }
                }
            }
            else
            {
                message.Append("The specified user could not be authorized!");
            }
            return status;
        }

        /// <summary>
        /// This examines the specified parameters to resove the next action.
        /// This may only be called after a user is Authenticated.
        /// </summary>
        public IntTag ResolveAction( HttpContext context, string clientGuid, string userName, string groupName, DateTime start, long duration, bool autoStart)
        {
            int user_ID = 0;
            int client_ID = 0;
            int group_ID = 0;
            IntTag result = new IntTag();
            result.id = -1;
            StringBuilder buf = new StringBuilder();

            if (userName != null && userName.Length > 0)
            {
                user_ID = AdministrativeAPI.GetUserID(userName);
                if (user_ID <= 0)
                {
                    result.tag = "The user '" + userName + "' does not exist!";
                    return result;
                }

            }
            else
            {
                result.tag = "You must specifiy a user name!";
                return result;
            }
            
            //Get Client_ID
            if (clientGuid != null && clientGuid.Length > 0)
            {
                client_ID = AdministrativeAPI.GetLabClientID(clientGuid);
                if (user_ID <= 0)
                {
                    result.tag = "The clientGUID '" + clientGuid + "' does not exist!";
                    return result;
                }
            }
            else
            {
                result.tag = "You must specifiy a clientGuid!";
                return result;
            }

            // Check that the user & is a member of the group
            if (groupName != null && groupName.Length >0)
            {
                int gid = AdministrativeAPI.GetGroupID(groupName);
                if (gid > 0)
                {
                    if (AdministrativeAPI.IsAgentMember(user_ID, gid))
                    {
                        group_ID = gid;
                    }
                    else
                    {
                        // user is not a member of the group
                        group_ID = -1;
                        groupName = null;
                        result.tag = "The user is not a member of the requested group!";
                        return result;
                    }
                }
                else
                {
                    result.tag = "The group '" + groupName + "' does not exist!";
                    return result;
                }
            }
            else
            {
                result.tag = "You must specifiy a group name!";
                return result;
            }


            // parameters are parsed, do we have enough info to launch
            int[] clientGroupIDs = null;
            int[] userGroupIDs = null;

            // Try and resolve any unspecified parameters
            if (client_ID <= 0 && group_ID <= 0)
            {
                userGroupIDs = AdministrativeAPI.ListGroupsForAgentRecursively(user_ID);
                Group[] groups = AdministrativeAPI.GetGroups(userGroupIDs);
                Dictionary<int, int[]> clientMap = new Dictionary<int, int[]>();
                foreach (Group g in groups)
                {
                    if ((g.groupType.CompareTo(GroupType.REGULAR) == 0) )
                    {
                        int[] clientIDs = AdministrativeUtilities.GetGroupLabClients(g.groupID);
                        if (clientIDs != null & clientIDs.Length > 0)
                        {
                            clientMap.Add(g.groupID, clientIDs);
                        }
                    }
                }
                if (clientMap.Count > 1) //more than one group with clients
                {
                    //modifyUserSession(group_ID, client_ID);
                    buf.Append(FormatRegularURL(context.Request, "myGroups.aspx"));
                }
                else if (clientMap.Count == 1) // get the group with clients
                {
                    Dictionary<int, int[]>.Enumerator en = clientMap.GetEnumerator();
                    int gid = -1;
                    int[] clients = null;
                    while (en.MoveNext())
                    {
                        gid = en.Current.Key;
                        clients = en.Current.Value;
                    }
                    if (AdministrativeAPI.IsAgentMember(user_ID, gid))
                    {
                        group_ID = gid;
                        groupName = AdministrativeAPI.GetGroupName(gid);


                        if (clients == null || clients.Length > 1)
                        {
                           // modifyUserSession(group_ID, client_ID);
                            buf.Append(FormatRegularURL(context.Request, "myLabs.aspx"));
                        }
                        else
                        {
                            client_ID = clients[0];
                        }
                    }
                }
            }

            else if (client_ID > 0 && group_ID <= 0)
            {
                int gid = -1;
                clientGroupIDs = AdministrativeUtilities.GetLabClientGroups(client_ID);
                if (clientGroupIDs == null || clientGroupIDs.Length == 0)
                {
                    //modifyUserSession(group_ID, client_ID);
                    buf.Append(FormatRegularURL(context.Request, "myGroups.aspx"));
                }
                else if (clientGroupIDs.Length == 1)
                {
                    gid = clientGroupIDs[0];
                }
                else
                {
                    userGroupIDs = AdministrativeAPI.ListGroupsForAgentRecursively(user_ID);
                    int count = 0;
                    foreach (int ci in clientGroupIDs)
                    {
                        foreach (int ui in userGroupIDs)
                        {
                            if (ci == ui)
                            {
                                count++;
                                gid = ui;
                            }
                        }
                    }
                    if (count != 1)
                    {
                        gid = -1;
                    }
                }
                if (gid > 0 && AdministrativeAPI.IsAgentMember(user_ID, gid))
                {
                    group_ID = gid;

                }
                else
                {
                    //modifyUserSession(group_ID, client_ID);
                }
            }
            else if (client_ID <= 0 && group_ID > 0)
            {
                int[] clients = AdministrativeUtilities.GetGroupLabClients(group_ID);
                if (clients == null || clients.Length != 1)
                {
                    //modifyUserSession(group_ID, client_ID);
                    buf.Append(FormatRegularURL(context.Request, "myLabs.aspx"));
                }
                else
                {
                    client_ID = clients[0];
                }
            }
            if (user_ID > 0 && group_ID > 0 && client_ID > 0)
            {
                int gid = -1;
                clientGroupIDs = AdministrativeUtilities.GetLabClientGroups(client_ID);
                foreach (int g_id in clientGroupIDs)
                {
                    if (g_id == group_ID)
                    {
                        gid = g_id;
                        break;
                    }
                }
                if (gid == -1)
                {
                    result.tag = "The specified group does not have permission to to run the specified client!";
                    return result;
                }
                if (!AdministrativeAPI.IsAgentMember(user_ID, group_ID))
                {
                    result.tag = "The user does not have permission to to run the specified client!";
                    return result;
                }

                // is authorized ?

                //modifyUserSession(group_ID, client_ID);
                //launchLab(user_ID, group_ID, client_ID, duration, autoStart);
                result.id = 1;
                return result;

            }
            return result;
        }


        public IntTag launchLab(ref HttpContext context, int userID, int groupID, int clientID, long duration, bool autoLaunch)
        {
            // Currently there is not a good solution for checking for an AllowExperiment ticket, will check the USS for reservation
            IntTag results = new IntTag();
            StringBuilder buf = new StringBuilder();
            buf.Append(FormatRegularURL(context.Request,"myClient.aspx"));
            if(autoLaunch)
                buf.Append("?auto=t");

            string userName = null;
            string groupName = null;
            Coupon opCoupon = null;
            Ticket allowTicket = null;
            int effectiveGroupID = 0;
           
            userName = AdministrativeAPI.GetUserName(userID);
            

            LabClient client = AdministrativeAPI.GetLabClient(clientID);
            if (client != null && client.clientID > 0) // need to test for valid value
            {
                DateTime start = DateTime.UtcNow;
                long _duration = 36000L; // default is ten hours
                ProcessAgentInfo[] labServers = null;
                labServers = AdministrativeAPI.GetLabServersForClient(clientID);
                if (labServers.Length > 0)
                {
                    //labServer = labServers[0];
                }
                else
                {
                    throw new Exception("The lab server is not specified for lab client " + client.clientName + " version: " + client.version);
                }
                groupName = AdministrativeAPI.GetGroupName(groupID);
                // Find efective group
                string effectiveGroupName = null;
                effectiveGroupID = AuthorizationAPI.GetEffectiveGroupID(groupID, clientID,
                    Qualifier.labClientQualifierTypeID, Function.useLabClientFunctionType);
                if (effectiveGroupID == groupID)
                {
                        effectiveGroupName = groupName;
                }
                else if (effectiveGroupID > 0)
                {
                    effectiveGroupName = AdministrativeAPI.GetGroupName(effectiveGroupID);
                }

                //Check for Scheduling: Moved to myClient

                //Session["ClientID"] = client.clientID;
                //Response.Redirect(Global.FormatRegularURL(Request, "myClient.aspx"), true);
                //Response.Redirect(buf.ToString(), true);
                results.id = 1;
                results.tag = buf.ToString();
            } // End if valid client
            else
            {
                throw new Exception("The specified lab client could not be found");
            }
            return results;
        }


        //if (clientMap.Count > 1) //more than one group with clients
        //            {
        //                modifyUserSession(group_ID, client_ID);
        //                Response.Redirect(Global.FormatRegularURL(Request, "myGroups.aspx"), true);
        //            }
        //            else if (clientMap.Count == 1) // get the group with clients
        //    {
        //        Dictionary<int, int[]>.Enumerator en = clientMap.GetEnumerator();
        //        int gid = -1;
        //        int[] clients = null;
        //        while (en.MoveNext())
        //        {
        //            gid = en.Current.Key;
        //            clients = en.Current.Value;
        //        }
        //        if (AdministrativeAPI.IsAgentMember(user_ID, gid))
        //        {
        //            group_ID = gid;
        //            group_Name = AdministrativeAPI.GetGroupName(gid);


        //            if (clients == null || clients.Length > 1)
        //            {
        //                modifyUserSession(group_ID, client_ID);
        //                Response.Redirect(Global.FormatRegularURL(Request, "myLabs.aspx"), true);
        //            }
        //            else
        //            {
        //                client_ID = clients[0];
        //            }
        //        }    
        //}

        //    //Get Client_ID
        //    if (hdnClient.Value != null && hdnClient.Value.Length > 0)
        //    {
        //        client_ID = AdministrativeAPI.GetLabClientID(hdnClient.Value);
        //        //Session["clientID"] = client_ID;
        //    }

        //    //{ // Note: The existing session client should not be concidered?

        //    if (hdnGroup.Value != null && hdnGroup.Value.Length > 0)
        //    {
        //        group_Name = hdnGroup.Value;
        //    }

        //    // Check that the user & is a member of the group
        //    if (group_Name != null)
        //    {
        //        int gid = AdministrativeAPI.GetGroupID(group_Name);
        //        if (gid > 0)
        //        {
        //            if (AdministrativeAPI.IsAgentMember(user_ID, gid))
        //            {
        //                group_ID = gid;
        //                //Session["GroupID"] = group_ID;
        //                //Session["GroupName"] = group_Name;
        //            }
        //            else
        //            {
        //                // user is not a member of the group
        //                group_ID = -1;
        //                group_Name = null;

        //            }
        //        }
        //    }

        //    // Session and parameters are parsed, do we have enough info to launch
        //    int[] clientGroupIDs = null;
        //    int[] userGroupIDs = null;

        //    // Try and resolve any unspecified parameters
        //    if (client_ID <= 0 && group_ID <= 0)
        //    {
        //        userGroupIDs = AdministrativeAPI.ListGroupsForAgentRecursively(user_ID);


        //    }

        //    else if (client_ID > 0 && group_ID <= 0)
        //    {
        //        int gid = -1;
        //        clientGroupIDs = AdministrativeUtilities.GetLabClientGroups(client_ID);
        //        if (clientGroupIDs == null || clientGroupIDs.Length == 0)
        //        {
        //            modifyUserSession(group_ID, client_ID);
        //            Response.Redirect(Global.FormatRegularURL(Request, "myGroups.aspx"), true);
        //        }
        //        else if (clientGroupIDs.Length == 1)
        //        {
        //            gid = clientGroupIDs[0];
        //        }
        //        else
        //        {
        //            userGroupIDs = AdministrativeAPI.ListGroupsForAgentRecursively(user_ID);
        //            int count = 0;
        //            foreach (int ci in clientGroupIDs)
        //            {
        //                foreach (int ui in userGroupIDs)
        //                {
        //                    if (ci == ui)
        //                    {
        //                        count++;
        //                        gid = ui;
        //                    }
        //                }
        //            }
        //            if (count != 1)
        //            {
        //                gid = -1;
        //            }
        //        }
        //        if (gid > 0 && AdministrativeAPI.IsAgentMember(user_ID, gid))
        //        {
        //            group_ID = gid;

        //        }
        //        else
        //        {
        //            modifyUserSession(group_ID, client_ID);
        //        }
        //    }
        //    else if (client_ID <= 0 && group_ID > 0)
        //    {
        //        int[] clients = AdministrativeUtilities.GetGroupLabClients(group_ID);
        //        if (clients == null || clients.Length != 1)
        //        {
        //            modifyUserSession(group_ID, client_ID);
        //            Response.Redirect(Global.FormatRegularURL(Request, "myLabs.aspx"), true);
        //        }
        //        else
        //        {
        //            client_ID = clients[0];
        //        }
        //    }
        //    if (user_ID > 0 && group_ID > 0 && client_ID > 0)
        //    {
        //        int gid = -1;
        //        clientGroupIDs = AdministrativeUtilities.GetLabClientGroups(client_ID);
        //        foreach (int g_id in clientGroupIDs)
        //        {
        //            if (g_id == group_ID)
        //            {
        //                gid = g_id;
        //                break;
        //            }
        //        }
        //        if (gid == -1)
        //        {
        //            buf.Append("The specified group does not have permission to to run the specified client!");
        //            lblMessages.Visible = true;
        //            lblMessages.Text = Utilities.FormatErrorMessage(buf.ToString());
        //            return;
        //        }
        //        if (!AdministrativeAPI.IsAgentMember(user_ID, group_ID))
        //        {
        //            buf.Append("You do not have permission to to run the specified client!");
        //            lblMessages.Visible = true;
        //            lblMessages.Text = Utilities.FormatErrorMessage(buf.ToString());
        //            return;
        //        }

        //        // is authorized ?

        //        modifyUserSession(group_ID, client_ID);
        //        launchLab(user_ID, group_ID, client_ID);

        //    }
        //    return status;
        //}
        /*
                /// <summary>
                /// This examines the specified parameters and current session state to resove the next action.
                /// This may only be reached after a user is Authenticated.
                /// </summary>
                private void ResolveAction()
                {
                    int user_ID = 0;
                    int client_ID = 0;
                    int group_ID = 0;
                    string client_Guid = null;
                    string group_Name = null;
                    string user_Name = null;
                    StringBuilder buf = new StringBuilder();
                    Session["IsAdmin"] = false;
                    Session["IsServiceAdmin"] = false;
                    lblMessages.Visible = false;
                    lblMessages.Text = "";

                    if (hdnUser.Value != null && hdnUser.Value.Length > 0)
                    {
                        // Check that the specified user & current user match
                        if (hdnUser.Value.ToLower().CompareTo(Session["UserName"].ToString().ToLower()) == 0)
                        {
                            user_Name = hdnUser.Value;
                            user_ID = AdministrativeAPI.GetUserID(user_Name);
                        }
                        else
                        {
                            //logout();
                            lblMessages.Visible = true;
                            lblMessages.Text = "You are not the user that was specified!";
                            return;
                        }
                    }
                    else // User is current user
                    {
                        user_Name = Session["UserName"].ToString();
                        user_ID = Convert.ToInt32(Session["UserID"]);
                    }

                    //Get Client_ID
                    if (hdnClient.Value != null && hdnClient.Value.Length > 0)
                    {
                        client_ID = AdministrativeAPI.GetLabClientID(hdnClient.Value);
                        //Session["clientID"] = client_ID;
                    }

                    //{ // Note: The existing session client should not be concidered?

                    if (hdnGroup.Value != null && hdnGroup.Value.Length > 0)
                    {
                        group_Name = hdnGroup.Value;
                    }

                    // Check that the user & is a member of the group
                    if (group_Name != null)
                    {
                        int gid = AdministrativeAPI.GetGroupID(group_Name);
                        if (gid > 0)
                        {
                            if (AdministrativeAPI.IsAgentMember(user_ID, gid))
                            {
                                group_ID = gid;
                                //Session["GroupID"] = group_ID;
                                //Session["GroupName"] = group_Name;
                            }
                            else
                            {
                                // user is not a member of the group
                                group_ID = -1;
                                group_Name = null;

                            }
                        }
                    }

                    // Session and parameters are parsed, do we have enough info to launch
                    int[] clientGroupIDs = null;
                    int[] userGroupIDs = null;

                    // Try and resolve any unspecified parameters
                    if (client_ID <= 0 && group_ID <= 0)
                    {
                        userGroupIDs = AdministrativeAPI.ListGroupsForAgentRecursively(user_ID);
                        Group[] groups = AdministrativeAPI.GetGroups(userGroupIDs);
                        Dictionary<int, int[]> clientMap = new Dictionary<int, int[]>();
                        foreach (Group g in groups)
                        {
                            if ((g.groupType.CompareTo(GroupType.REGULAR) == 0) && (g.groupName.CompareTo("ROOT") != 0)
                                && (g.groupName.CompareTo("NewUserGroup") != 0) && (g.groupName.CompareTo("OrphanedUserGroup") != 0)
                                 && (g.groupName.CompareTo("SuperUserGroup") != 0))
                            {
                                int[] clientIDs = AdministrativeUtilities.GetGroupLabClients(g.groupID);
                                if (clientIDs != null & clientIDs.Length > 0)
                                {
                                    clientMap.Add(g.groupID, clientIDs);
                                }
                            }
                        }
                        if (clientMap.Count > 1) //more than one group with clients
                        {
                            modifyUserSession(group_ID, client_ID);
                            Response.Redirect(Global.FormatRegularURL(Request, "myGroups.aspx"), true);
                        }
                        if (clientMap.Count == 1) // get the group with clients
                        {
                            Dictionary<int, int[]>.Enumerator en = clientMap.GetEnumerator();
                            int gid = -1;
                            int[] clients = null;
                            while (en.MoveNext())
                            {
                                gid = en.Current.Key;
                                clients = en.Current.Value;
                            }
                            if (AdministrativeAPI.IsAgentMember(user_ID, gid))
                            {
                                group_ID = gid;
                                group_Name = AdministrativeAPI.GetGroupName(gid);


                                if (clients == null || clients.Length > 1)
                                {
                                    modifyUserSession(group_ID, client_ID);
                                    Response.Redirect(Global.FormatRegularURL(Request, "myLabs.aspx"), true);
                                }
                                else
                                {
                                    client_ID = clients[0];
                                }
                            }
                        }
                    }

                    else if (client_ID > 0 && group_ID <= 0)
                    {
                        int gid = -1;
                        clientGroupIDs = AdministrativeUtilities.GetLabClientGroups(client_ID);
                        if (clientGroupIDs == null || clientGroupIDs.Length == 0)
                        {
                            modifyUserSession(group_ID, client_ID);
                            Response.Redirect(Global.FormatRegularURL(Request, "myGroups.aspx"), true);
                        }
                        else if (clientGroupIDs.Length == 1)
                        {
                            gid = clientGroupIDs[0];
                        }
                        else
                        {
                            userGroupIDs = AdministrativeAPI.ListGroupsForAgentRecursively(user_ID);
                            int count = 0;
                            foreach (int ci in clientGroupIDs)
                            {
                                foreach (int ui in userGroupIDs)
                                {
                                    if (ci == ui)
                                    {
                                        count++;
                                        gid = ui;
                                    }
                                }
                            }
                            if (count != 1)
                            {
                                gid = -1;
                            }
                        }
                        if (gid > 0 && AdministrativeAPI.IsAgentMember(user_ID, gid))
                        {
                            group_ID = gid;

                        }
                        else
                        {
                            modifyUserSession(group_ID, client_ID);
                        }
                    }
                    else if (client_ID <= 0 && group_ID > 0)
                    {
                        int[] clients = AdministrativeUtilities.GetGroupLabClients(group_ID);
                        if (clients == null || clients.Length != 1)
                        {
                            modifyUserSession(group_ID, client_ID);
                            Response.Redirect(Global.FormatRegularURL(Request, "myLabs.aspx"), true);
                        }
                        else
                        {
                            client_ID = clients[0];
                        }
                    }
                    if (user_ID > 0 && group_ID > 0 && client_ID > 0)
                    {
                        int gid = -1;
                        clientGroupIDs = AdministrativeUtilities.GetLabClientGroups(client_ID);
                        foreach (int g_id in clientGroupIDs)
                        {
                            if (g_id == group_ID)
                            {
                                gid = g_id;
                                break;
                            }
                        }
                        if (gid == -1)
                        {
                            buf.Append("The specified group does not have permission to to run the specified client!");
                            lblMessages.Visible = true;
                            lblMessages.Text = Utilities.FormatErrorMessage(buf.ToString());
                            return;
                        }
                        if (!AdministrativeAPI.IsAgentMember(user_ID, group_ID))
                        {
                            buf.Append("You do not have permission to to run the specified client!");
                            lblMessages.Visible = true;
                            lblMessages.Text = Utilities.FormatErrorMessage(buf.ToString());
                            return;
                        }

                        // is authorized ?

                        modifyUserSession(group_ID, client_ID);
                        launchLab(user_ID, group_ID, client_ID);

                    }
                }
        
            }
         * */


    }
}
