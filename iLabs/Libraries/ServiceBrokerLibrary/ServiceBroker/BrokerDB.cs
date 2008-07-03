using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

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
using iLabs.Services;

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
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("getEssInfoForExperiment", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
            ProcessAgentInfo ess = null;
            try
            {
                myConnection.Open();
                SqlDataReader myReader = myCommand.ExecuteReader();
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
                    if (ticket != null && !ticket.IsExpired()&& (ticket.SecondsToExpire() > duration))
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

        public int InsertAdminURL(SqlConnection connection, int id, string url, string ticketType)
        {
            try
            {
                if (!TicketTypes.TicketTypeExists(ticketType))
                    throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");

                // command executes the "InsertAdminURL" stored procedure
                SqlCommand cmd = new SqlCommand("InsertAdminURL", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                // 1. type
                SqlParameter typeParam = cmd.Parameters.Add("@processAgentID", SqlDbType.Int);
                typeParam.Value = id;
                // 2. admin URL
                SqlParameter urlParam = cmd.Parameters.Add("@adminURL", SqlDbType.VarChar, 256);
                urlParam.Value = url;
                // 3. ticket type
                SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
                ticketTypeParam.Value = ticketType;

                // execute the command
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
        }

        public int InsertAdminURL(ProcessAgentInfo processAgentInfo, string url, string ticketType)
        {
            if (!TicketTypes.TicketTypeExists(ticketType))
                throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");
            // create sql connection
            SqlConnection connection = CreateConnection();

            try
            {
                return InsertAdminURL(connection, processAgentInfo.agentId, url, ticketType);
            }

            finally
            {
                connection.Close();
            }
        }

        public int InsertAdminURL(int id, string url, string ticketType)
        {
            if (!TicketTypes.TicketTypeExists(ticketType))
                throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");
            // create sql connection
            SqlConnection connection = CreateConnection();

            try
            {
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
            SqlConnection connection = CreateConnection();

            try
            {
                DeleteAdminURL(connection, Id);
            }
            finally
            {
                connection.Close();
            }
        }


        public int DeleteAdminURL(SqlConnection connection, int Id)
        {
            try
            {

                // command executes the "DeleteAdminURLbyID" stored procedure
                SqlCommand cmd = new SqlCommand("DeleteAdminURLbyID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter

                SqlParameter idParam = cmd.Parameters.Add("@adminURLID", SqlDbType.Int);
                idParam.Value = Id;


                // execute the command
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
        }


        public int DeleteAdminURL(SqlConnection connection, int processAgentId, string url, string ticketType)
        {
            try
            {
                if (!TicketTypes.TicketTypeExists(ticketType))
                    throw new Exception("\"" + ticketType + "\" is not a valid ticket type.");

                // command executes the "InsertAdminURL" stored procedure
                SqlCommand cmd = new SqlCommand("DeleteAdminURL", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                // 1. type
                SqlParameter typeParam = cmd.Parameters.Add("@processAgentID", SqlDbType.Int);
                typeParam.Value = processAgentId;
                // 2. admin URL
                SqlParameter urlParam = cmd.Parameters.Add("@adminURL", SqlDbType.VarChar, 256);
                urlParam.Value = url;
                // 3. ticket type
                SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
                ticketTypeParam.Value = ticketType;

                // execute the command
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
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
            SqlConnection connection = CreateConnection();

            try
            {
                DeleteAdminURL(connection, processAgentInfo.AgentId, url, ticketType);
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
        }

        public void DeleteAdminURL(AdminUrl adminURL)
        {
            if (!TicketTypes.TicketTypeExists(adminURL.TicketType.name))
                throw new Exception("\"" + adminURL.TicketType.name + "\" is not a valid ticket type.");
            // create sql connection
            SqlConnection connection = CreateConnection();

            try
            {
                DeleteAdminURL(connection, adminURL.ProcessAgentId, adminURL.Url, adminURL.TicketType.name);
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
        }


        public AdminUrl[] RetrieveAdminURLs(int processAgentID)
        {
            // create sql connection
            SqlConnection connection = CreateConnection();

            try
            {
                return RetrieveAdminURLs(connection, processAgentID);
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
            return RetrieveAdminURL(GetProcessAgentInfo(processAgentGuid).AgentId, function);
        }

        public AdminUrl[] RetrieveAdminURLs(SqlConnection connection, int processAgentID)
        {
            // create sql command
            // command executes the "RetrieveAdminURLs" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveAdminURLs", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@processAgentID", SqlDbType.Int);
            idParam.Value = processAgentID;

            // read the result
            ArrayList list = new ArrayList();
            SqlDataReader dataReader = cmd.ExecuteReader();
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
            catch (SqlException e)
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

        /* START OF RESOURCE MAPPING */
     
        public IntTag[] GetAdminServiceTags(int groupID)
        {
            ArrayList list = new ArrayList();
            SqlConnection connection = CreateConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("GetAdminServiceTags", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                   // populate stored procedure parameters
                SqlParameter groupParam = cmd.Parameters.Add("@groupId", SqlDbType.Int);
                groupParam.Value = groupID;

                // read the result
                
                SqlDataReader dataReader = cmd.ExecuteReader();
           
                while (dataReader.Read())
                {
                    int id = dataReader.GetInt32(0);
                    string name = dataReader.GetString(1);
                    list.Add(new IntTag(id,name));
                }
                dataReader.Close();
            }
            catch (SqlException e)
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
            SqlConnection connection = CreateConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("GetAdminProcessAgentTags", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                   // populate stored procedure parameters
                SqlParameter groupParam = cmd.Parameters.Add("@groupID", SqlDbType.Int);
                groupParam.Value = groupID;

                // read the result
                
                SqlDataReader dataReader = cmd.ExecuteReader();
           
                while (dataReader.Read())
                {
                    int id = dataReader.GetInt32(0);
                    string name = dataReader.GetString(1);
                    list.Add(new IntTag(id,name));
                }
                dataReader.Close();
            }
            catch (SqlException e)
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
            SqlConnection myConnection = CreateConnection();
            SqlCommand myCommand = new SqlCommand("GetProcessAgentAdminGrants", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@agentID", agentID));
            myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));


            try
            {
               
                SqlDataReader reader = myCommand.ExecuteReader();
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

     
	
       public ResourceMapping AddResourceMapping(string keyType, object key, string[] valueTypes, object[] values){
           SqlConnection connection = CreateConnection();
           ResourceMapping mapping = null;
           try
           {
               mapping = InsertResourceMapping(connection, keyType, key, valueTypes, values);
               if (mapping != null)
               {
                  
                   // add the new resource mapping to the static resource mappings array
                   ResourceMapManager.Add(mapping);
                   //ResourceMapping[] mappings = new ResourceMapping[BrokerDB.resourceMappings.Length + 1];
                   //for (int i = 0; i < BrokerDB.resourceMappings.Length; i++)
                   //    mappings[i] = BrokerDB.resourceMappings[i];
                   //mappings[mappings.Length - 1] = mapping;
                   //// reassign ticket issuer mappings
                   //BrokerDB.resourceMappings = mappings;

               }


           }
           catch (SqlException sqlEx)
           {
           }
           finally
           {
               connection.Close();
           }

           return mapping;

       }

        protected ResourceMapping InsertResourceMapping(SqlConnection connection, string keyType, object key, string[] valueTypes, object[] values)
        {
            if (valueTypes == null || values == null)
                throw new ArgumentException("Arguments cannot be null", "valueTypes and values");

            if (valueTypes.Length != values.Length)
                throw new ArgumentException ("Parameter Arrays \"valueTypes\" and \"values\" should be of the same length");

            ResourceMappingKey mappingKey = new ResourceMappingKey(keyType, key);
            // insert key into database
         
            try
            {
                SqlCommand cmd = new SqlCommand("InsertResourceMappingKey", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Get the key type id
                int keyTypeID = ResourceMappingTypes.GetResourceMappingTypeID(keyType);
                if (keyTypeID == -1)
                    throw new ArgumentException("Value for key type is invalid");

                int keyID = -1;

                // if the key is a string, add the string to the strings table
                if (keyType.Equals(ResourceMappingTypes.STRING))
                {
                    SqlCommand cmd2 = new SqlCommand("AddResourceMappingString", connection);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    SqlParameter idParam = cmd2.Parameters.Add("@string_Value", SqlDbType.VarChar);
                    idParam.Value = (string)key;

                    keyID = Convert.ToInt32(cmd2.ExecuteScalar());
                }

                // if the key is a Resource Type, add the string to the ResourceTypes table
                else if (keyType.Equals(ResourceMappingTypes.RESOURCE_TYPE))
                {
                    SqlCommand cmd2 = new SqlCommand("AddResourceMappingResourceType", connection);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    SqlParameter idParam = cmd2.Parameters.Add("@resourceType_Value", SqlDbType.VarChar);
                    idParam.Value = (string)key;

                    keyID = Convert.ToInt32(cmd2.ExecuteScalar());
                }

                else
                    keyID = ResourceMappingEntry.GetId(key);

                if (keyID == -1)
                    throw new ArgumentException("Value for key is invalid");

                // populate stored procedure parameters
                SqlParameter keyTypeParam = cmd.Parameters.Add("@MappingKey_Type", SqlDbType.Int);
                SqlParameter keyParam = cmd.Parameters.Add("@MappingKey", SqlDbType.Int);
                keyTypeParam.Value = keyTypeID;
                keyParam.Value = keyID;

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
                        SqlCommand cmd2 = new SqlCommand("AddResourceMappingString", connection);
                        cmd2.CommandType = CommandType.StoredProcedure;

                        // populate parameters
                        SqlParameter idParam = cmd2.Parameters.Add("@string_Value", SqlDbType.VarChar);
                        idParam.Value = (string)values[i];

                        valueID = Convert.ToInt32(cmd2.ExecuteScalar());
                    }

                    // if the key is a Resource Type, add the string to the ResourceTypes table
                    else if (valueTypes[i].Equals(ResourceMappingTypes.RESOURCE_TYPE))
                    {
                        SqlCommand cmd2 = new SqlCommand("AddResourceMappingResourceType", connection);
                        cmd2.CommandType = CommandType.StoredProcedure;

                        SqlParameter idParam = cmd2.Parameters.Add("@resourceType_Value", SqlDbType.VarChar);
                        idParam.Value = (string)values[i];

                        valueID = Convert.ToInt32(cmd2.ExecuteScalar());
                    }

                    else
                        valueID = ResourceMappingEntry.GetId(values[i]);

                    if (valueID == -1)
                        throw new ArgumentException("Value \"" + i + "\" is invalid");

                    cmd = new SqlCommand("InsertResourceMappingValue", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    // populate stored procedure parameters
                    SqlParameter mappingIDParam = cmd.Parameters.Add("@Mapping_ID", SqlDbType.Int);
                    SqlParameter valueTypeParam = cmd.Parameters.Add("@MappingValue_Type", SqlDbType.Int);
                    SqlParameter valueParam = cmd.Parameters.Add("@MappingValue", SqlDbType.Int);
                    mappingIDParam.Value = mappingID;
                    valueTypeParam.Value = valueTypeID;
                    valueParam.Value = valueID;

                    // execute the command
                    mappingID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // create new mapping object
                ResourceMapping mapping = new ResourceMapping(mappingID, mappingKey, mappingValues);


         
                return mapping;
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
        /// 
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(ResourceMapping mapping)
        {
            return DeleteResourceMapping(mapping.MappingID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(int mappingId)
        {
            bool status = false;
            SqlConnection connection = CreateConnection();

            try
            {

                // Check for any Qualifiers created for this RM
                int qualifierId = AuthorizationAPI.GetQualifierID(mappingId, Qualifier.resourceMappingQualifierTypeID);
                if (qualifierId > 0)
                {
                    // Any grant associated with this qualifier are removed via a cascading delete
                    AuthorizationAPI.RemoveQualifiers(new int[] { qualifierId });
                }

                SqlCommand cmd = new SqlCommand("DeleteResourceMapping", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@mapping_ID", SqlDbType.Int);
                idParam.Value = mappingId;

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
            catch (SqlException e)
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

        public bool DeleteResourceMapping(string keyType, int keyId, string valueType, int valueId)
        {
            return DeleteResourceMapping(ResourceMappingTypes.GetResourceMappingTypeID(keyType), keyId, 
                ResourceMappingTypes.GetResourceMappingTypeID( valueType), valueId);
    }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapping">Resource Mapping to be deleted</param>
        /// <returns><code>true</code> if the mapping has been deleted successfully</returns>
        public bool DeleteResourceMapping(int keyTypeId, int keyId, int valueTypeId, int valueId)
        {
            bool status = false;
            SqlConnection connection = CreateConnection();

            try
            {
                int mapId = -1;
                SqlCommand cmd = new SqlCommand("GetMappingIdByKeyValue", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter keyIdParam = cmd.Parameters.Add("@keyID", SqlDbType.Int);
                keyIdParam.Value = keyId;
                SqlParameter keyTypeParam = cmd.Parameters.Add("@keyType", SqlDbType.Int);
                keyTypeParam.Value = keyTypeId;
                SqlParameter valueIdParam = cmd.Parameters.Add("@valueID", SqlDbType.Int);
                valueIdParam.Value = valueId;
                SqlParameter valueTypeParam = cmd.Parameters.Add("@valueType", SqlDbType.Int);
                valueTypeParam.Value = valueTypeId;

                mapId  = (int)cmd.ExecuteScalar();
                if (mapId > 0)
                {
                    status = DeleteResourceMapping(mapId);
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
            return status;
        }

        public ResourceMappingKey GetResourceMappingKey(int mappingID)
        {
            ResourceMapping mapping = ResourceMapManager.GetMap(mappingID);
            if(mapping != null)
                return mapping.key;
            else 
                return null;
            
        }

        public ResourceMapping GetResourceMapping(int mappingID)
        {
            return ResourceMapManager.GetMap(mappingID);
        }               

        /// <summary>
        /// Reads a resource mapping from the database
        /// </summary>
        /// <param name="mappingID">id of the resource mapping</param>
        /// <returns>ResourceMapping object</returns>
        public ResourceMapping ReadResourceMapping(int mappingID)
        {
            SqlConnection connection = CreateConnection();

            try
            {
                ResourceMapping mapping = ReadResourceMapping(mappingID, connection);

                return mapping;
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
        }
      
        public ResourceMapping ReadResourceMapping(int mappingID, SqlConnection connection)
        {
            SqlDataReader dataReader = null;
            try
            {
                SqlCommand cmd = new SqlCommand("GetResourceMappingByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@mappingID", SqlDbType.Int);
                idParam.Value = mappingID;

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
                //if (valuesList.Count > 0)
                //{
                    ResourceMappingValue[] values = (ResourceMappingValue[])valuesList.ToArray(value.GetType());

                    // construct resource mapping
                    return new ResourceMapping(mappingID, key, values);
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
            finally{
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
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("GetResourceMappingIDs", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            ResourceMapping mapping = null;
            List<ResourceMapping> mappingList = new List<ResourceMapping>();

            // execute the command
            SqlDataReader dataReader = null;
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
        private ResourceMappingEntry CompleteResourceMappingKeyRead(SqlConnection connection, int typeID, int entryID, bool isKey)
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
            SqlConnection connection = CreateConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("GetResourceStringByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@ID", SqlDbType.Int);
                idParam.Value = strID;

                //connection.Close();
                return Convert.ToString(cmd.ExecuteScalar());
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
        }

        public int AddResourceMappingString(string s)
        {
            SqlConnection connection = CreateConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("AddResourceMappingString", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter param = cmd.Parameters.Add("@string_Value", SqlDbType.VarChar);
                param.Value = (string)s;

                return Convert.ToInt32(cmd.ExecuteScalar());

            }

            finally
            {
                connection.Close();
            }

        }

        public int UpdateResourceMappingString(int id, string s)
        {
            SqlConnection connection = CreateConnection();
            int mappingID = 0;
            try
            {
                SqlCommand cmd = new SqlCommand("UpdateResourceMappingString", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                idParam.Value = id;
                SqlParameter strParam = cmd.Parameters.Add("@string", SqlDbType.VarChar,2000);
                strParam.Value = s;

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
            SqlConnection connection = CreateConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("AddResourceMappingResourceType", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter param = cmd.Parameters.Add("@resourceType_Value", SqlDbType.VarChar);
                param.Value = (string)s;

                return Convert.ToInt32(cmd.ExecuteScalar());

            }

            finally
            {
                connection.Close();
            }

        }

        public String GetResourceMappingResourceType(int resourceTypeID)
        {
            SqlConnection connection = CreateConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("GetResourceTypeByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@ID", SqlDbType.Int);
                idParam.Value = resourceTypeID;

                return Convert.ToString(cmd.ExecuteScalar());
                connection.Close();
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
            foreach(DictionaryEntry entry in mappingsTable)
            {
                values[i++] = ((ResourceMapping)entry.Value).values;
               
            }
            return values;  
        }

        public Hashtable GetResourceStringTags(ResourceMappingKey key)
        {
            return GetResourceStringTags((int) key.Entry, key.Type);
        }

        public Hashtable GetResourceStringTags(int target, string rmType)
        {
            
            return GetResourceStringTags(target, ResourceMappingTypes.GetResourceMappingTypeID(rmType));
        }

        public Hashtable GetResourceStringTags(int target,int type){
            Hashtable resources = null;
            Hashtable results = null;

            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("GetResourceTypeStrings", connection);
           
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
          
            SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.Int);
            typeParam.Value = type;
            SqlParameter targetParam = cmd.Parameters.Add("@target", SqlDbType.Int);
            targetParam.Value = target;
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows){
                    resources = new Hashtable();
                    int mid = 0;                    
                    while(reader.Read()){
                        mid = reader.GetInt32(0);
                        resources.Add(mid,reader.GetString(1));
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
                Utilities.WriteLog(ex.Message);
                throw;
            }
            finally{
                connection.Close();
                
            }
            if(results != null && results.Count > 0)
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
            ResourceMappingValue [] search = new ResourceMappingValue[1];
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
        //    SqlConnection connection  = CreateConnection();
        //      // command executes the "InsertResourceMap" stored procedure
        //        SqlCommand cmd = new SqlCommand("InsertResourceMap", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter paramType = null;
        //        SqlParameter paramValue = null;

        //        // Need to do this in pairs 
        //        paramType = cmd.Parameters.Add("@keyType", SqlDbType.Int);
        //        paramValue = cmd.Parameters.Add("@keyValue", SqlDbType.Int);
        //        paramType.Value = keyType; 
        //        paramValue.Value = keyValue;
        //        paramType = cmd.Parameters.Add("@Type0", SqlDbType.Int);
        //        paramValue = cmd.Parameters.Add("@value0", SqlDbType.Int);
        //        paramType.Value = type0;
        //        paramValue.Value = value0;
            
        //    paramType = cmd.Parameters.Add("@type1", SqlDbType.Int);
        //    paramValue = cmd.Parameters.Add("@value1", SqlDbType.Int);
        //    if(ResourceMap.IsResourceMapType(type1)){
        //        paramType.Value = type1;   
        //        paramValue.Value = value1;
        //    }
        //    else{
        //        paramType.Value = DBNull.Value;   
        //        paramValue.Value =  DBNull.Value;
        //    }
        //    paramType = cmd.Parameters.Add("@type2", SqlDbType.Int);
        //    paramValue = cmd.Parameters.Add("@value2", SqlDbType.Int);
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
        //    catch (SqlException e)
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
            s.Append(mapping.MappingID + " ");
             s.Append(GetMappingEntryString(mapping.key,true) + "-> ");

            //if (mapping.values.Length > 1)
            //    s.Append("(");

            // print all values except last
             for (int i = 0; i < mapping.values.Length; i++)
             {
                 if (i > 0 && i < mapping.values.Length)
                     s.Append(", ");
                 s.Append(GetMappingEntryString(mapping.values[i],true));

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
                if (showType)
                    name = GetProcessAgentNameWithType((int)o);
                else
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
                buf.Append((string)o);
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
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("InsertRegistration", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@couponId", SqlDbType.Int );
            idParam.Value = couponId;
            SqlParameter couponGuidParam = cmd.Parameters.Add("@couponGuid", SqlDbType.VarChar,50 );
            couponGuidParam.Value = couponGuid;
            SqlParameter registerGuidParam = cmd.Parameters.Add("@registerGuid", SqlDbType.Varchar,50);
            registerGuidParam.Value = registerGuid;
            SqlParameter sourceParam = cmd.Parameters.Add("@sourceGuid", SqlDbType.Varchar,50);
            sourceParam.Value = sourceGuid;
            SqlParameter statusParam = cmd.Parameters.Add("@status", SqlDbType.Int );
            statusParam.Value = status;
            SqlParameter emailParam = cmd.Parameters.Add("@email", SqlDbType.VarChar,256 );
            emailParam.Value = email;
            SqlParameter descriptorParam = cmd.Parameters.Add("@descriptor", SqlDbType.Text );
            descriptorParam.Value = descriptor;
            try
            {
            
                return Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }
            finally
            {
                connection.Close();
            }

        }

        public string[] SelectRegisterGuids()
        {
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("InsertRegistration", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter Param = cmd.Parameters.Add("@", SqlDbType.VarChar);
            Param.Value = (string)s;
            try
            {

                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();

            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }
            finally
            {
                connection.Close();
            }


        }

        protected RegisterRecord readRegisterRecord(SqlDataReader reader){
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
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("SelectRegistrationRecord", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter Param = cmd.Parameters.Add("@id", SqlDbType.Int);
            Param.Value = id;
            try
            {
                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();
                while(dataReader.Read()){
                    record = readRegisterRecord(dataReader);


            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }
            finally
            {
                connection.Close();
            }


        }

        public RegisterRecord[] SelectRegister(string registerGuid)
        {
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("InsertRegistration", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter Param = cmd.Parameters.Add("@", SqlDbType.VarChar);
            Param.Value = (string)s;
            try
            {

                return Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }
            finally
            {
                connection.Close();
            }

        }

        public RegisterRecord[] SelectRegisterByStatus(int status)
        {
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("InsertRegistration", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter Param = cmd.Parameters.Add("@", SqlDbType.VarChar);
            Param.Value = (string)s;
            try
            {

                return Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }
            finally
            {
                connection.Close();
            }

        }
        public RegisterRecord[] SelectRegisterByStatus(int low, int high)
        {
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("InsertRegistration", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter Param = cmd.Parameters.Add("@", SqlDbType.VarChar);
            Param.Value = (string)s;
            try
            {

                return Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }
            finally
            {
                connection.Close();
            }


        }

        public int SetRegisterStatus(int id, int status)
        {
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("UpdateRegistrationStatus", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
            idParam.Value = id;
            SqlParameter statusParam = cmd.Parameters.Add("@status", SqlDbType.Int);
            statusParam.Value = status;
            try
            {

                return cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Utilities.WriteLog(e.Message);
            }

            finally
            {
                connection.Close();
            }


        }
*/

        
    }
    //public class RegisterRecord
    //{
    //    public int recordId;
    //    public int status;
    //    public int couponId;
    //    public DateTime create;
    //    public DateTime lastModified;
    //    public string couponGuid;
    //    public string registerGuid;
    //    public string sourceGuid;
    //    public string descriptor;
    //    public string email;
    //}
        
}
