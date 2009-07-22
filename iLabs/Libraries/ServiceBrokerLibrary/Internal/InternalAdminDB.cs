/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: InternalAdminDB.cs,v 1.22 2008/03/14 16:10:27 pbailey Exp $ */

using System;
using System.Data ;
using System.Data .SqlClient ;
using System.Configuration ;
using System.Collections ;
using System.Globalization;
using System.Text;

using iLabs.Core;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
//using iLabs.DataTypes.BatchTypes;
using iLabs.Ticketing;
using iLabs.TicketIssuer;
using iLabs.UtilLib;
using iLabs.ServiceBroker;

namespace iLabs.ServiceBroker.Internal
{
	/// <summary>
	/// Summary description for InternalAdminDB.
	/// </summary>
	public class InternalAdminDB
	{
		public InternalAdminDB()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR ProcessAgents and Services
		 * !------------------------------------------------------------------------------!
		 */

	
		/// <summary>
		/// to delete all lab servers records specified by the array of lab server IDs
		/// </summary>
		public static int[] DeleteProcessAgents ( int[] agentIDs )
		{
			
			SqlConnection myConnection = FactoryDB.GetConnection();
			SqlCommand myCommand = new SqlCommand ("DeleteProcessAgents", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@agentID",null));

			/*
			 * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
			 * extremely inefficient and hence this method was chosen
			 */
			 
			ArrayList arrayList = new ArrayList ();

			try 
			{
				myConnection.Open ();
											
				foreach (int agentID in agentIDs) 
				{
					
					// Deleting from table LabServers
					/*	IMPORTANT ! - The database if currently set to Cascade delete, where deleting an experiment will automatically
					 *  delete the relevant Lab_Server_to_Client_Map records. If Cascade Delete is not to be used, then the code to delete the extra records
					 *  in the map table when a lab server is deleted should be added in the stored procedure
					 * 
					 * Also, the qualifiers pertaining to the lab server are automatically
					 * deleted in the 'DeleteLabServer' stored procedure. This preserves consistency
					 * and gets rid of unnecessary rollback mechanisms that would otherwise have 
					 * to be implemented. - CV, 4/29/05
					 */

					myCommand.Parameters["@agentID"].Value = agentID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (agentID);
					}
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteLabServers",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// refresh in memory Q & Q-H cache - since the lab server qualifiers are automatically deleted
			AuthCache.QualifierSet  = InternalAuthorizationDB.RetrieveQualifiers ();
			AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy ();

			int[] lsIDs = Utilities.ArrayListToIntArray(arrayList);
			return lsIDs;
		}


        //public static ResourceMapping SelectResourceMappingByID(int mappingID)
        //{
        //    BrokerDB issuer = new BrokerDB();
        //    return issuer.GetResourceMapping(mappingID);
        //}

        //public static void DeleteResourceMapping(ResourceMapping mapping)
        //{
        //    BrokerDB issuer = new BrokerDB();
        //    issuer.DeleteResourceMapping(mapping);
        //    // refresh in memory Q & Q-H cache - since the lab server qualifiers are automatically deleted
        //    AuthCache.QualifierSet = InternalAuthorizationDB.RetrieveQualifiers();
        //    AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy();
        //}

		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR LAB SERVERS
		 * !------------------------------------------------------------------------------!
		 */
		/// <summary>
		/// to add a new lab server
		/// </summary>

		public static int InsertLabServer(LabServer ls)
		{
			int labServerID = -1;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddLabServer", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@labServerName", ls.labServerName ));
			myCommand.Parameters.Add(new SqlParameter("@labServerGUID", ls.labServerGUID));
			myCommand.Parameters.Add(new SqlParameter("@webServiceURL", ls.webServiceURL));
			myCommand.Parameters.Add(new SqlParameter("@labServerDescription", ls.labServerDescription));
			myCommand.Parameters.Add(new SqlParameter("@labInfoURL", ls.labInfoURL));
			myCommand.Parameters.Add(new SqlParameter("@contactFirstName", ls.contactFirstName ));
			myCommand.Parameters.Add(new SqlParameter("@contactLastName", ls.contactLastName ));
			myCommand.Parameters.Add(new SqlParameter("@contactEmail",ls.contactEmail ));
			
			try
			{
				myConnection.Open();
				labServerID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in inserting lab server",ex);
			}
			finally
			{
				myConnection.Close();
			}

			return labServerID;
		}

		/// <summary>
		/// to modify an existing lab server's information
		/// </summary>
		public static void UpdateLabServer(LabServer ls)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifyLabServer", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@labServerID", ls.labServerID ));
			myCommand.Parameters.Add(new SqlParameter("@labServerName", ls.labServerName ));
			myCommand.Parameters.Add(new SqlParameter("@labServerGUID", ls.labServerGUID));
			myCommand.Parameters.Add(new SqlParameter("@webServiceURL", ls.webServiceURL));
			myCommand.Parameters.Add(new SqlParameter("@labServerDescription", ls.labServerDescription));
			myCommand.Parameters.Add(new SqlParameter("@labInfoURL", ls.labInfoURL));
			myCommand.Parameters.Add(new SqlParameter("@contactFirstName", ls.contactFirstName ));
			myCommand.Parameters.Add(new SqlParameter("@contactLastName", ls.contactLastName ));
			myCommand.Parameters.Add(new SqlParameter("@contactEmail",ls.contactEmail ));

			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
				if(i == 0)
					throw new Exception ("UpdateLabServer: No record modified exception");
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in modifying lab server",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to delete all lab servers records specified by the array of lab server IDs
		/// </summary>
		public static int[] DeleteLabServers ( int[] labServerIDs )
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteLabServer", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@labServerID",null));

			/*
			 * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
			 * extremely inefficient and hence this method was chosen
			 */
			 
			ArrayList arrayList = new ArrayList ();

			try 
			{
				myConnection.Open ();
											
				foreach (int labServerID in labServerIDs) 
				{
					
					// Deleting from table LabServers
					/*	IMPORTANT ! - The database if currently set to Cascade delete, where deleting an experiment will automatically
					 *  delete the relevant Lab_Server_to_Client_Map records. If Cascade Delete is not to be used, then the code to delete the extra records
					 *  in the map table when a lab server is deleted should be added in the stored procedure
					 * 
					 * Also, the qualifiers pertaining to the lab server are automatically
					 * deleted in the 'DeleteLabServer' stored procedure. This preserves consistency
					 * and gets rid of unnecessary rollback mechanisms that would otherwise have 
					 * to be implemented. - CV, 4/29/05
					 */

					myCommand.Parameters["@labServerID"].Value = labServerID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (labServerID);
					}
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteLabServers",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// refresh in memory Q & Q-H cache - since the lab server qualifiers are automatically deleted
			AuthCache.QualifierSet  = InternalAuthorizationDB.RetrieveQualifiers ();
			AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy ();

			int[] lsIDs = Utilities.ArrayListToIntArray(arrayList);
			return lsIDs;
		}

		/// <summary>
		/// to retrieve a list of all the lab servers in the database
		/// </summary>

		public static int[] SelectLabServerIDs ()
		{
			ProcessAgentDB ticketing = new ProcessAgentDB();
			return ticketing.GetProcessAgentIDsByType((int) ProcessAgentType.AgentType.LAB_SERVER);
		}

		/// <summary>
		/// to retrieve lab server metadata for lab servers specified by array of lab server IDs 
		/// </summary>
		public static LabServer[] SelectLabServers ( int[] labServerIDs )
		{
			LabServer[] ls = new LabServer[labServerIDs.Length ];
			for (int i=0; i<labServerIDs.Length ; i++)
			{
				ls[i] = new LabServer();
			}

			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveLabServer", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@labServerID",SqlDbType.Int));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < labServerIDs.Length ; i++) 
				{
					myCommand.Parameters["@labServerID"].Value = labServerIDs[i];

					// get labserver info from table lab_servers
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						ls[i].labServerID = labServerIDs[i];

						if(myReader["lab_server_name"] != System.DBNull.Value )
							ls[i].labServerName = (string) myReader["lab_server_name"];
						if(myReader["GUID"] != System.DBNull.Value )
							ls[i].labServerGUID = (string) myReader["GUID"];
						if(myReader["web_service_URL"] != System.DBNull.Value )
							ls[i].webServiceURL= (string) myReader["web_service_URL"];
						if(myReader["description"] != System.DBNull.Value )
							ls[i].labServerDescription= (string) myReader["description"];
						if(myReader["info_URL"] != System.DBNull.Value )
							ls[i].labInfoURL= (string) myReader["info_URL"];
						if(myReader["contact_first_name"] != System.DBNull.Value )
							ls[i].contactFirstName = (string) myReader["contact_first_name"];
						if(myReader["contact_last_name"] != System.DBNull .Value )
							ls[i].contactLastName = (string) myReader["contact_last_name"];
						if(myReader["contact_email"] != System.DBNull .Value )
							ls[i].contactEmail = (string) myReader["contact_email"];
						//if(myReader["date_created"] != System.DBNull .Value )
						//	ls[i].dateCreated = (DateTime) myReader["date_created"];
					}
					myReader.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectLabServers",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return ls;
		}

		/// <summary>
		/// to retrieve lab server metadata for lab servers specified by array of lab server IDs 
		/// </summary>
		public static ProcessAgent[] SelectLabServers ()
		{
			ProcessAgentDB ticketing = new ProcessAgentDB();
			return ticketing.GetProcessAgentsByType(ProcessAgentType.LAB_SERVER);
		}

		/// <summary>
		/// Gets the integer LabServerID given labServerGUID
		/// </summary>
		public static int SelectLabServerID(string labServerGUID)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveLabServerID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@GUID", labServerGUID));
			
			try
			{
				myConnection.Open();
				int labServerID = Convert.ToInt32(myCommand.ExecuteScalar());

				//If labServer record doesn't exist return -1
				if (labServerID == 0)
					labServerID=-1;
				
				return labServerID;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving labServerID",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// Gets the integer LabServerID given the experimentID
		/// </summary>
		public static int SelectLabServerID(long experimentID)
		{
			
			int intLabServerID;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveExperimentRawData", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@experimentID", experimentID));
			
			try
			{
				myConnection.Open();
				SqlDataReader r = myCommand.ExecuteReader();
				if (r.Read()) 
				{
					intLabServerID = r.GetInt32(2);
				}
				else
				{
					throw new Exception("Cannot retrieve Lab Server ID from the database");
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving labServerID", ex);
			}
			finally
			{
				myConnection.Close();
			}
			return intLabServerID;
		}
/*
		/// <summary>
		/// to modify the outgoing passkey 
		/// Method name changed from UpdateSBOutgoingPasskey
		/// </summary>
		public static void UpdateLSOutgoingPasskey(int labServerID,string passKey)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifyOutPasskey", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@lsID", labServerID));
			myCommand.Parameters.Add(new SqlParameter("@passKey", passKey));
			
			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
				if(i == 0)
					throw new Exception ("No record modified exception");
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in updating outgoing passkey",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to modify the incoming passkey
		/// Method name changed from UpdateSBIncomingPasskey
		/// </summary>
		public static void UpdateLSIncomingPasskey(int labServerID,string passKey)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifyInPasskey", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@lsID", labServerID));
			myCommand.Parameters.Add(new SqlParameter("@passKey", passKey));
			
			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
				if(i == 0)
					throw new Exception ("No record modified exception");
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in updating incoming passkey",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}


		/// <summary>
		/// to select the passkey
		/// </summary>
		public static string selectSBOutgoingPasskey(int labServerID)
		{
			string passKey = null;
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("SelectOutPasskey", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@lsID", labServerID));
			
			
			try
			{
				myConnection.Open();
				passKey = myCommand.ExecuteScalar().ToString ();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in selecting passkey",ex);
			}
			finally
			{
				myConnection.Close();
			}
			return passKey;
		}

		/// <summary>
		/// to select the passkey
		/// </summary>
		public static string selectSBIncomingPasskey(int labServerID)
		{
			string passKey = null;
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("SelectInPasskey", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters.Add(new SqlParameter("@lsID", labServerID));
			
			try
			{
				myConnection.Open();
				passKey = myCommand.ExecuteScalar().ToString ();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in selecting passkey",ex);
			}
			finally
			{
				myConnection.Close();
			}
			return passKey;
		}
		*/
		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR LAB CLIENTS
		 * !------------------------------------------------------------------------------!
		 */
		/// <summary>
		/// to add a lab client
		/// The order in which lab servers are associated with this client is the array order
		/// </summary>
		
		public static int InsertLabClient(LabClient lc)
		{
			int clientID = -1;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddLabClient", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@guid", lc.clientGuid));
			myCommand.Parameters.Add(new SqlParameter("@labClientName", lc.clientName ));
			myCommand.Parameters.Add(new SqlParameter("@shortDescription", lc.clientShortDescription ));
			myCommand.Parameters.Add(new SqlParameter("@longDescription", lc.clientLongDescription ));
			myCommand.Parameters.Add(new SqlParameter("@version", lc.version ));
			myCommand.Parameters.Add(new SqlParameter("@loaderScript", lc.loaderScript));
			myCommand.Parameters.Add(new SqlParameter("@clientType", lc.clientType));
			myCommand.Parameters.Add(new SqlParameter("@email",lc.contactEmail ));	
			myCommand.Parameters.Add(new SqlParameter("@firstName",lc.contactFirstName ));	
			myCommand.Parameters.Add(new SqlParameter("@lastName",lc.contactLastName ));
			myCommand.Parameters.Add(new SqlParameter("@notes", lc.notes));
            myCommand.Parameters.Add(new SqlParameter("@needsScheduling", lc.needsScheduling));
            myCommand.Parameters.Add(new SqlParameter("@needsESS", lc.needsESS));
            myCommand.Parameters.Add(new SqlParameter("@isReentrant", lc.IsReentrant));
	
			//Encoding transaction here
			//Alternatively a dataset can be used, but it's not preferred bcos it's inefficient?
			SqlTransaction transaction;
		
			myConnection.Open();
			transaction = myConnection.BeginTransaction();
			myCommand.Transaction = transaction;

			try
			{

				clientID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
				lc.clientID = clientID;

				/*-- ASSOCIATED LAB SERVERS --*/
				if(lc.labServerIDs != null)
				{
					myCommand.Parameters.Clear();
					myCommand.CommandText = "AddLabServerClient";
					myCommand.CommandType = CommandType.StoredProcedure ;

					myCommand.Parameters.Add(new SqlParameter("@labClientID", clientID));
					myCommand.Parameters.Add(new SqlParameter("@labServerID", SqlDbType.Int));
					myCommand.Parameters.Add(new SqlParameter("@displayOrder", SqlDbType.Int));
				
					int count=1;
					foreach (int id in lc.labServerIDs ) 
					{
						myCommand.Parameters["@labServerID"].Value = id;
						myCommand.Parameters["@displayOrder"].Value = count;

						int i = myCommand.ExecuteNonQuery();
						
						if (i!=0)
							count+=1;
					}
				}

				/*-- ASSOCIATED INFO URLS --*/
				if(lc.clientInfos != null)
				{
					myCommand.Parameters.Clear();
					myCommand.CommandText = "AddClientInfo";
					myCommand.CommandType = CommandType.StoredProcedure ;

					myCommand.Parameters.Add(new SqlParameter("@labClientID", lc.clientID));
					myCommand.Parameters.Add(new SqlParameter("@infoURL", SqlDbType.VarChar));
					myCommand.Parameters.Add(new SqlParameter("@infoName", SqlDbType.VarChar));
					myCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar));
					myCommand.Parameters.Add(new SqlParameter("@displayOrder", SqlDbType.Int));
				
					int count=1;
					foreach (ClientInfo c in lc.clientInfos ) 
					{
						//if (c != null) - this doesn't work for some reason
						if ((c.infoURL!=null)&&(c.infoURL.CompareTo("")!=0))
						{
							myCommand.Parameters["@infoURL"].Value = c.infoURL;
							myCommand.Parameters["@infoName"].Value = c.infoURLName;
							myCommand.Parameters["@description"].Value = c.description;
							myCommand.Parameters["@displayOrder"].Value = count;
							//myCommand.Parameters["@displayOrder"].Value = c.displayOrder;

							int i = myCommand.ExecuteNonQuery();
						
							if (i!=0)
								count+=1;
						}
					}
				}

				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();	
				throw new Exception("Exception thrown in inserting lab server to client",ex);
			}
			finally
			{
				myConnection.Close();
			}
			return clientID;
		}

		/// <summary>
		/// to modify a lab client
		/// </summary>
		
		public static void UpdateLabClient(LabClient lc)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifyLabClient", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@labClientID", lc.clientID ));
			myCommand.Parameters.Add(new SqlParameter("@labClientName", lc.clientName ));
			myCommand.Parameters.Add(new SqlParameter("@shortDescription", lc.clientShortDescription ));
			myCommand.Parameters.Add(new SqlParameter("@longDescription", lc.clientLongDescription ));
			myCommand.Parameters.Add(new SqlParameter("@version", lc.version ));
			myCommand.Parameters.Add(new SqlParameter("@loaderScript", lc.loaderScript));
			myCommand.Parameters.Add(new SqlParameter("@clientType", lc.clientType));
			myCommand.Parameters.Add(new SqlParameter("@email",lc.contactEmail ));	
			myCommand.Parameters.Add(new SqlParameter("@firstName",lc.contactFirstName ));	
			myCommand.Parameters.Add(new SqlParameter("@lastName",lc.contactLastName ));
			myCommand.Parameters.Add(new SqlParameter("@notes", lc.notes));
            myCommand.Parameters.Add(new SqlParameter("@needsScheduling", lc.needsScheduling));
            myCommand.Parameters.Add(new SqlParameter("@needsESS", lc.needsESS));
            myCommand.Parameters.Add(new SqlParameter("@isReentrant", lc.IsReentrant));

			//Encoding transaction here
			//Alternatively a dataset can be used, but it's not preferred bcos it's inefficient?
			SqlTransaction transaction;
		
			myConnection.Open();
			transaction = myConnection.BeginTransaction();
			myCommand.Transaction = transaction;

			try
			{
				int i = myCommand.ExecuteNonQuery();
				if(i == 0)
					throw new Exception ("No record modified exception");
				//bool inserted = Boolean.Parse ( myCommand.ExecuteScalar().ToString ());

				/*-- ASSOCIATED LAB SERVERS --*/
				// update table Lab_Server_To_Client_Map
				
				/* First delete all the lab_server_ids that exist for a particular client */
				myCommand.Parameters.Clear();
				myCommand.CommandText = "DeleteLabServerClient";
				myCommand.CommandType = CommandType.StoredProcedure ;

				myCommand.Parameters.Add(new SqlParameter("@labClientID", lc.clientID));
				myCommand.ExecuteNonQuery();

				myCommand.Parameters.Clear();
				myCommand.CommandText = "AddLabServerClient";
				myCommand.CommandType = CommandType.StoredProcedure ;

				myCommand.Parameters.Add(new SqlParameter("@labClientID", lc.clientID));
				myCommand.Parameters.Add(new SqlParameter("@labServerID", null));
				myCommand.Parameters.Add(new SqlParameter("@displayOrder", SqlDbType.Int));
				
				if(lc.labServerIDs != null)
				{
					int count=1;
					foreach (int id in lc.labServerIDs ) 
					{
						myCommand.Parameters["@labServerID"].Value = id;
						myCommand.Parameters["@displayOrder"].Value = count;

						int x = myCommand.ExecuteNonQuery();
						
						if (x!=0)
							count+=1;
					}
				}

				/*-- ASSOCIATED INFO URLS --*/
				/* First delete all the lab_server_ids that exist for a particular client */
				myCommand.Parameters.Clear();
				myCommand.CommandText = "DeleteClientInfo";
				myCommand.CommandType = CommandType.StoredProcedure ;

				myCommand.Parameters.Add(new SqlParameter("@labClientID", lc.clientID));
				myCommand.ExecuteNonQuery();

				myCommand.Parameters.Clear();
				myCommand.CommandText = "AddClientInfo";
				myCommand.CommandType = CommandType.StoredProcedure ;

				myCommand.Parameters.Add(new SqlParameter("@labClientID", lc.clientID));
				myCommand.Parameters.Add(new SqlParameter("@infoURL", SqlDbType.VarChar));
				myCommand.Parameters.Add(new SqlParameter("@infoName", SqlDbType.VarChar));
				myCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar));
				myCommand.Parameters.Add(new SqlParameter("@displayOrder", SqlDbType.Int));
				
				if(lc.clientInfos != null)
				{
					int count=1;
					foreach (ClientInfo c in lc.clientInfos ) 
					{
						//if (c != null) - this doesn't work for some reason
						if ((c.infoURL!=null)&&(c.infoURL.CompareTo("")!=0))
						{
							myCommand.Parameters["@infoURL"].Value = c.infoURL;
							myCommand.Parameters["@infoName"].Value = c.infoURLName;
							myCommand.Parameters["@description"].Value = c.description;
							myCommand.Parameters["@displayOrder"].Value = count;
							//myCommand.Parameters["@displayOrder"].Value = c.displayOrder;

							int x = myCommand.ExecuteNonQuery();
						
							if (x!=0)
								count+=1;
						}
					}
				}

				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();	
				throw new Exception("Exception thrown in inserting lab server to client: " + ex.Message, ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to delete all lab clients records specified by the array of lab client IDs
		/// </summary>
		public static int[] DeleteLabClients ( int[] clientIDs )
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteLabClient", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@labClientID",null));

			/*
			 * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
			 * extremely inefficient and hence this method was chosen
			 */
			 
			ArrayList arrayList = new ArrayList ();

			try 
			{
				myConnection.Open ();
											
				foreach (int clientID in clientIDs) 
				{
					// Deleting from table LabClients
					/*	IMPORTANT ! - The database if currently set to Cascade delete, where deleting an experiment will automatically
					 *  delete the relevant Lab_Server_to_Client_Map records. If Cascade Delete is not to be used, then the code to delete the extra records
					 *  in the map table when a lab client is deleted should be added in the stored procedure
					 *  
					 * Also, the qualifiers pertaining to the lab client are automatically
					 * deleted in the 'DeleteLabClient' stored procedure. This preserves consistency
					 * and gets rid of unnecessary rollback mechanisms that would otherwise have 
					 * to be implemented. - CV, 4/29/05
					 */
					myCommand.Parameters["@labClientID"].Value = clientID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (clientID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteLabClients",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// refresh in memory Q & Q-H cache - since the lab server qualifiers are automatically deleted
			AuthCache.QualifierSet  = InternalAuthorizationDB.RetrieveQualifiers ();
			AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy ();

			int[] lcIDs = Utilities.ArrayListToIntArray(arrayList);
						
			return lcIDs;
		}

        public static int DeleteLabClient(int clientID)
        {
            int count = -1;
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["sqlConnection"]);
            SqlCommand myCommand = new SqlCommand("DeleteLabClient", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@labClientID", null));

            /*
             * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
             * extremely inefficient and hence this method was chosen
             */

            try
            {
                myConnection.Open();

                // Deleting from table LabClients
                /*	IMPORTANT ! - The database if currently set to Cascade delete, where deleting an experiment will automatically
                 *  delete the relevant Lab_Server_to_Client_Map records. If Cascade Delete is not to be used, then the code to delete the extra records
                 *  in the map table when a lab client is deleted should be added in the stored procedure
                 *  
                 * Also, the qualifiers pertaining to the lab client are automatically
                 * deleted in the 'DeleteLabClient' stored procedure. This preserves consistency
                 * and gets rid of unnecessary rollback mechanisms that would otherwise have 
                 * to be implemented. - CV, 4/29/05
                 */
                myCommand.Parameters["@labClientID"].Value = clientID;
                count = myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in DeleteLabClients", ex);
            }
            finally
            {
                myConnection.Close();
            }
            if (count > 0)
            {
                // refresh in memory Q & Q-H cache - since the lab server qualifiers are automatically deleted
                AuthCache.QualifierSet = InternalAuthorizationDB.RetrieveQualifiers();
                AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy();
            }
            return count;
        }

        public static int SelectLabClientId(string guid)
        {
            int id = -1;
            SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveLabClientID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter guidParam = myCommand.Parameters.Add("@guid", SqlDbType.VarChar, 50);
		    guidParam.Value = guid;
            try
            {
                myConnection.Open();

                // get labclient id from table lab_clients
                SqlDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    if (myReader["client_ID"] != System.DBNull.Value)
                        id = myReader.GetInt32(0);

                }
            }
            catch (Exception e)
            {
                Utilities.WriteLog("Error getting clientID: " + e.Message);
            }
            finally
            {
                myConnection.Close();
            }
            return id;

        }

		/// <summary>
		/// to retrieve a list of all the lab clients in the database
		/// </summary>

		public static int[] SelectLabClientIDs ()
		{
			int[] clientIDs;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveLabClientIDs", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			try 
			{
				myConnection.Open ();
				

				// get labclient ids from table lab_clients
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList lcIDs = new ArrayList();

				while(myReader.Read ())
				{	
					if(myReader["client_ID"] != System.DBNull.Value )
						lcIDs.Add(Convert.ToInt32(myReader["client_ID"]));
					//	clientIDs [i] = (string) myReader["client_id"];
				}
				myReader.Close ();

				// Converting to an int array
				clientIDs = Utilities.ArrayListToIntArray(lcIDs);

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectLabClientIDs",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return clientIDs;
		}

		/// <summary>
		/// to retrieve lab client metadata for lab clients specified by array of lab client IDs 
		/// </summary>
		public static LabClient[] SelectLabClients ( int[] clientIDs )
		{
			LabClient[] lc = new LabClient[clientIDs.Length];
			for (int i=0; i<clientIDs.Length ; i++)
			{
				lc[i] = new LabClient();
			}

			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveLabClient", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@labClientID",SqlDbType.Int));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < clientIDs.Length ; i++) 
				{
					myCommand.Parameters["@labClientID"].Value = clientIDs[i];

					// get labclient info from table lab_clients
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						lc[i].clientID = clientIDs[i];

						if(myReader["lab_client_name"] != System.DBNull.Value )
							lc[i].clientName = (string) myReader["lab_client_name"];
						if(myReader["short_description"] != System.DBNull.Value )
							lc[i].clientShortDescription = (string) myReader["short_description"];
						if(myReader["long_description"] != System.DBNull.Value )
							lc[i].clientLongDescription = (string) myReader["long_description"];
						if(myReader["version"] != System.DBNull.Value )
							lc[i].version = (string) myReader["version"];
						if(myReader["loader_script"] != System.DBNull.Value )
							lc[i].loaderScript= (string) myReader["loader_script"];
						if(myReader["description"] != System.DBNull.Value )
							lc[i].clientType= (string) myReader["description"];
						if(myReader["contact_email"] != System.DBNull .Value )
							lc[i].contactEmail= (string) myReader["contact_email"];
						if(myReader["contact_first_name"]!= System.DBNull.Value)
							lc[i].contactFirstName = (string) myReader["contact_first_name"];
						if(myReader["contact_last_name"]!= System.DBNull.Value)
							lc[i].contactLastName = (string) myReader["contact_last_name"];
						if(myReader["notes"]!= System.DBNull.Value)
							lc[i].notes = (string) myReader["notes"];
                        lc[i].needsScheduling = Convert.ToBoolean(myReader["needsScheduling"]);
                        lc[i].needsESS = Convert.ToBoolean(myReader["needsESS"]);
                        lc[i].IsReentrant = Convert.ToBoolean(myReader["isReentrant"]);
                        if (myReader["Client_Guid"] != System.DBNull.Value)
                            lc[i].clientGuid = (string)myReader["Client_Guid"];
						/*if(myReader["date_created"] != System.DBNull .Value )
							lc[i].= (string) myReader["date_created"];*/

						//added by Karim
						//if(myReader["info_URL"] != System.DBNull .Value )
						//	lc[i].infoURL= (string) myReader["info_URL"];

					}
					myReader.Close ();
				}
				//Retrieve  lab servers for a client
						
				ArrayList lsIDs = new ArrayList();

				SqlCommand myCommand2 = new SqlCommand ("RetrieveClientServerIDs", myConnection);
				myCommand2.CommandType = CommandType.StoredProcedure;
				myCommand2.Parameters .Add (new SqlParameter ("@labClientID",SqlDbType.Int));
				
				for (int i =0; i < clientIDs.Length ; i++) 
				{
					myCommand2.Parameters["@labClientID"].Value = clientIDs[i];
					SqlDataReader myReader2 = myCommand2.ExecuteReader ();
					
					while(myReader2.Read ())
					{	
						if(myReader2["agent_id"] != System.DBNull.Value )
							lsIDs.Add(Convert.ToInt32(myReader2["agent_id"]));
					}
				
					myReader2.Close();

					// Convert to an int array and add to the current LabClient object
					lc[i].labServerIDs = Utilities.ArrayListToIntArray(lsIDs);
					lsIDs.Clear();
				}

				//Retrieve info urls for a client
						
				ArrayList infoURLs = new ArrayList();

				myCommand2 = new SqlCommand ("RetrieveClientInfo", myConnection);
				myCommand2.CommandType = CommandType.StoredProcedure;
				myCommand2.Parameters .Add (new SqlParameter ("@labClientID",SqlDbType.Int));
				
				for (int i =0; i < clientIDs.Length ; i++) 
				{
					myCommand2.Parameters["@labClientID"].Value = clientIDs[i];
		
					SqlDataReader myReader2 = myCommand2.ExecuteReader ();
					
					while(myReader2.Read ())
					{	
						ClientInfo c = new ClientInfo();
						if(myReader2["info_url"] != System.DBNull.Value )
							c.infoURL = (string) myReader2["info_url"];
						if(myReader2["info_name"] != System.DBNull.Value )
							c.infoURLName  = (string) myReader2["info_name"];
						if(myReader2["client_info_id"] != System.DBNull.Value )
							c.clientInfoID = Convert.ToInt32( myReader2["client_info_id"]);
						if(myReader2["description"] != System.DBNull.Value )
							c.description = ((string) myReader2["description"]);
						if(myReader2["display_order"] != System.DBNull.Value )
							c.displayOrder = (int) myReader2["display_order"];
						
						infoURLs.Add(c);
					}
				
					myReader2.Close();

					// Converting to a clientInfo array
					lc[i].clientInfos = new ClientInfo [infoURLs.Count];
					for (int j=0;j <infoURLs.Count ; j++) 
					{
						lc[i].clientInfos[j] = (ClientInfo) (infoURLs[j]);
					}
					infoURLs.Clear();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectLabClients",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return lc;
		}

        public static int CountServerClients(int groupID, int labServerID){
            SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["sqlConnection"]);
            SqlCommand myCommand = new SqlCommand("CountServerClients", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter group = myCommand.Parameters.Add(new SqlParameter("@groupID", SqlDbType.Int));
            group.Value = groupID;
            SqlParameter server = myCommand.Parameters.Add(new SqlParameter("@serverID", SqlDbType.Int));
            server.Value = labServerID;
            try
            {
                myConnection.Open();
                return Convert.ToInt32(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
        }

        /// <summary>
        /// Get the local labClientID from it's name. Note this does not use a stored procedure
        /// as it was added as part of the service patch and did not want to require a database update.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ClientID</returns>
        public static int GetLabClientID(string name)
        {
            int id = -1;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand("SELECT Client_ID from Lab_Clients where Lab_Client_Name ='" + name + "';", myConnection);


            try
            {
                int value = Convert.ToInt32(myCommand.ExecuteScalar());
                if (value > 0)
                {
                    id = value;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return id;
        }


 /// <summary>
        /// Get the local labClientName from it's ID. Note this does not use a stored procedure
        /// as it was added as part of the service patch and did not want to require a database update.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ClientID</returns>
        public static string GetLabClientName(int clientID)
        {
            string name = null;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SELECT Lab_Client_Name from Lab_Clients where Client_ID ='" + clientID + "';", myConnection);
            myConnection.Open();
            try
            {
                name = Convert.ToString(myCommand.ExecuteScalar());
             
            }
            catch
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return name;
        }

        /// <summary>
        /// Get the local labClient Guid from it's ID. Note this does not use a stored procedure
        /// as it was added as part of the service patch and did not want to require a database update.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ClientID</returns>
        public static string GetLabClientGUID(int clientID)
        {
            string guid = null;
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SELECT Client_GUID from Lab_Clients where Client_ID ='" + clientID + "';", myConnection);
            myConnection.Open();
            try
            {
                guid = Convert.ToString(myCommand.ExecuteScalar());

            }
            catch
            {
                throw;
            }
            finally
            {
                myConnection.Close();
            }
            return guid;
        }
		/// <summary>
		/// to retrieve a list of all the client types in the database
		/// </summary>

		public static string[] SelectLabClientTypes ()
		{
			string[] clientTypes;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveLabClientTypes", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			try 
			{
				myConnection.Open ();
				

				// get labclient types from table clients_types
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList lcTypes = new ArrayList();

				while(myReader.Read ())
				{	
					if(myReader["description"] != System.DBNull.Value )
						lcTypes.Add((string)(myReader["description"]));
				}
				myReader.Close ();

				// Converting to a string array
				clientTypes = Utilities.ArrayListToStringArray(lcTypes);

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectLabClientTypes",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return clientTypes;
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



        /* !------------------------------------------------------------------------------!
         *							CALLS FOR USERS
         * !------------------------------------------------------------------------------!
         */

		/// <summary>
		/// to add a user
		/// The previous call used to be
		/// public static void InsertUser(User user, string principalID, string authenticationType, string initialGroupID)
		/// principal ID is automatically generated by the principals table.
		/// </summary>

		public static int InsertUser(User user, string principalString, string authenticationType, int initialGroupID)
		{
			// The Add User stored procedure first inserts a user into the Agents table
			//& then Agent Hierarchy, with the specified parent group id
			// The AgentID is then used as the primary key in the Users table
			// Finally user is also entered into the principals table

			int userID = -1;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddUser", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@userName", user.userName));
			myCommand.Parameters.Add(new SqlParameter("@firstName",user.firstName ));
			myCommand.Parameters.Add(new SqlParameter("@lastName", user.lastName));
			myCommand.Parameters.Add(new SqlParameter("@email", user.email));
			myCommand.Parameters.Add(new SqlParameter("@affiliation", user.affiliation));
			myCommand.Parameters.Add(new SqlParameter("@reason", user.reason));
			if ((user.xmlExtension == null)||(user.xmlExtension.CompareTo("")==0))
				myCommand.Parameters.Add(new SqlParameter("@XMLExtension", System.DBNull.Value));
			else
				myCommand.Parameters.Add(new SqlParameter("@XMLExtension", user.xmlExtension));
			
			myCommand.Parameters.Add (new SqlParameter("@lockUser",SqlDbType.Bit));
			if (user.lockAccount)
				myCommand.Parameters["@lockUser"].Value = 1;
			else
				myCommand.Parameters["@lockUser"].Value = 0;
		
			myCommand.Parameters.Add(new SqlParameter("@principalString", principalString));
			myCommand.Parameters.Add(new SqlParameter("@authenType", authenticationType));
			myCommand.Parameters.Add(new SqlParameter("@initialGroupID", initialGroupID));

			try
			{
				myConnection.Open();
				userID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
			}
			catch (SqlException sex)
			{
				throw new Exception("SQLException thrown in inserting user", sex);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in inserting user",ex);
			}
			finally
			{
				myConnection.Close();
			}

			// refresh A & A-H in memory cache
			AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();
			AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();

			return userID;
		}

		/// <summary>
		/// to modify a user
		/// The previous call used to be
		/// public static void UpdateUser(User user, string principalID, string authenticationType)
		/// principalID was changed to principalString a while ago
		/// </summary>
		public static void UpdateUser(User user, string principalString, string authenticationType)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifyUser", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@userID", user.userID));
			myCommand.Parameters.Add(new SqlParameter("@userName",user.userName ));
			myCommand.Parameters.Add(new SqlParameter("@firstName",user.firstName ));
			myCommand.Parameters.Add(new SqlParameter("@lastName", user.lastName));
			myCommand.Parameters.Add(new SqlParameter("@email", user.email));
			myCommand.Parameters.Add(new SqlParameter("@affiliation", user.affiliation));
			myCommand.Parameters.Add(new SqlParameter("@reason", user.reason));
			if ((user.xmlExtension == null)||(user.xmlExtension.CompareTo("")==0))
				myCommand.Parameters.Add(new SqlParameter("@XMLExtension", System.DBNull.Value));
			else
				myCommand.Parameters.Add(new SqlParameter("@XMLExtension", user.xmlExtension));
			
			myCommand.Parameters.Add (new SqlParameter("@lockUser",SqlDbType.Bit));
			if (user.lockAccount)
				myCommand.Parameters["@lockUser"].Value = 1;
			else
				myCommand.Parameters["@lockUser"].Value = 0;
		
			myCommand.Parameters.Add(new SqlParameter("@principalString", principalString));
			myCommand.Parameters.Add(new SqlParameter("@authenType", authenticationType));
			
			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
				if (i == 0)
					throw new Exception ("No record exists exception");   //throws an exception if No records can be modified
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in updating user record",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to delete all user records specified by the array of user IDs
		/// </summary>
		public static int[] DeleteUsers ( int[] userIDs )
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteUser", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@userID",SqlDbType.Int));

			/*
			 * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
			 * extremely inefficient and hence this method was chosen
			 */
			 
			ArrayList arrayList = new ArrayList ();

			try 
			{
				myConnection.Open ();
											
				foreach (int userID in userIDs) 
				{
					// Deleting from table Users
					/*	
					 * IMPORTANT ! - The database if currently not set to Cascade delete for Agents and Users.
					 * Hence the stored procedure implements the following functionality:
					 * 1. When a user (specified by userID) is to be deleted, the agent is first deleted.
					 * 2. This cascade deletes the records in the Agent Hierarchy and Grants tables (which has to be manually done in the code if cascade delete
																				doesn't work)
					 * 3. Then the User is deleted from the Users Table
					 * 4. This cascade deletes the entries in the Principals table (which has to be manually done in the code if cascade delete doesn't work).
					 * 5. This cascade deletes the entries in the Experiment_Information 
					 *		Client_Items & User_sessions tables
					 */
					myCommand.Parameters["@userID"].Value = userID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (userID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteUsers",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// need refresh agent datasets in memory
			AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
			AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();

			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			
			return uIDs;
		}

		/// <summary>
		/// to retrieve a list of all the users in the database
		/// </summary>
		public static int[] SelectUserIDs ()
		{
			int[] userIDs;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveUserIDs", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			try 
			{
				myConnection.Open ();

				// get User ids from table users
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList uIDs = new ArrayList();

				while(myReader.Read ())
				{	
					if(myReader["user_id"] != System.DBNull.Value )
						uIDs.Add(Convert.ToInt32(myReader["user_id"]));
				}
				myReader.Close ();

				// Converting to an int array
				userIDs = Utilities.ArrayListToIntArray(uIDs);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectUserIDs",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return userIDs;
		}

		/// <summary>
		/// to retrieve a list of all the orphaned users in the database i.e. users which no longer belong to a group
		/// </summary>
		public static int[] SelectOrphanedUserIDs ()
		{
			int[] userIDs;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveOrphanedUserIDs", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			try 
			{
				myConnection.Open ();
				

				// get User ids from table users
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList uIDs = new ArrayList();

				while(myReader.Read ())
				{	
					if(myReader["user_id"] != System.DBNull.Value )
						uIDs.Add(Convert.ToInt32(myReader["user_id"]));
				}
				myReader.Close ();

				// Converting to an int array
				userIDs = Utilities.ArrayListToIntArray(uIDs);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectOrphanedUserIDs",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return userIDs;
		}


		/// <summary>
		/// to retrieve user metadata for users specified by array of users 
		/// </summary>
		public static User[] SelectUsers ( int[] userIDs )
		{
			User[] u = new User[userIDs.Length ];
			for (int i=0; i<userIDs.Length ; i++)
			{
				u[i] = new User();
			}

			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveUser", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@userID",SqlDbType.Int));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < userIDs.Length ; i++) 
				{
					myCommand.Parameters["@userID"].Value = userIDs[i];

					// get User info from table users 
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						u[i].userID = userIDs[i];

						if(myReader["user_name"] != System.DBNull.Value )
							u[i].userName= (string) myReader["user_name"];
						if(myReader["first_name"] != System.DBNull.Value )
							u[i].firstName= (string) myReader["first_name"];
						if(myReader["last_name"] != System.DBNull.Value )
							u[i].lastName= (string) myReader["last_name"];
						if(myReader["email"] != System.DBNull .Value )
							u[i].email= (string) myReader["email"];
						if(myReader["affiliation"] != System.DBNull.Value )
							u[i].affiliation= (string) myReader["affiliation"];
						if(myReader["xml_extension"] != System.DBNull.Value )
							u[i].xmlExtension = (string) myReader["xml_extension"];
						if(myReader["signup_reason"] != System.DBNull .Value )
							u[i].reason= (string) myReader["signup_reason"];
						if(myReader["date_created"] != System.DBNull .Value)
							u[i].registrationDate = (DateTime) myReader["date_created"];
						if (Convert.ToInt16(myReader["lock_user"]) == 0)
							u[i].lockAccount = false;
						else u[i].lockAccount = true;
					}
					myReader.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectUsers",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return u;
		}

		/// <summary>
		/// to get a user's email given userName
		/// </summary>
		public static string SelectUserEmail(string userName)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveUserEmail", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@userName", userName));
			
			try
			{
				myConnection.Open();
				return  myCommand.ExecuteScalar().ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving user email",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to get a user's ID given userName
		/// </summary>
		public static int SelectUserID(string userName)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveUserID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@userName", userName));
			
			try
			{
				myConnection.Open();
				int userID = Convert.ToInt32(myCommand.ExecuteScalar());

				//If user record doesn't exist return -1
				if (userID == 0)
						userID=-1;
				
				return userID;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving user ID",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}


		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR USER SESSIONS
		 * !------------------------------------------------------------------------------!
		 */

		/// <summary>
		/// to insert a user session record. returns a database generated session id.
		/// </summary>
		public static long InsertUserSession(UserSession us)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddUserSession", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@userID", us.userID));
			myCommand.Parameters.Add(new SqlParameter("@groupID", us.groupID));
            myCommand.Parameters.Add(new SqlParameter("@tzOffset", us.tzOffset));
			myCommand.Parameters.Add(new SqlParameter("@sessionKey", us.sessionKey));
			
			try
			{
				myConnection.Open();
				return Convert.ToInt64( myCommand.ExecuteScalar());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in inserting user session",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

        /// <summary>
        /// to insert a user session record. returns a database generated session id.
        /// </summary>
        public static bool ModifyUserSession(long sessionID,int groupID,int clientID, int tzOffset, string sessionKey)
        {

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("ModifyUserSession", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@sessionID", sessionID));
            myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
            myCommand.Parameters.Add(new SqlParameter("@clientID", clientID));
            myCommand.Parameters.Add(new SqlParameter("@tzOffset", tzOffset));
            myCommand.Parameters.Add(new SqlParameter("@sessionKey", sessionKey));

            try
            {
                myConnection.Open();
                return Convert.ToBoolean(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in inserting user session", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

        /// <summary>
        /// to insert a user session record. returns a database generated session id.
        /// </summary>
        public static bool SetSessionGroup(long sessionID, int groupID)
        {

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SetSessionGroup", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@sessionID", sessionID));
            myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
            try
            {
                myConnection.Open();
                return Convert.ToBoolean(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in inserting user session", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }
        /// <summary>
        /// to insert a user session record. returns a database generated session id.
        /// </summary>
        public static bool SetSessionClient(long sessionID, int clientID)
        {
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SetSessionClient", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@sessionID", sessionID));
            myCommand.Parameters.Add(new SqlParameter("@clientID", clientID));
            try
            {
                myConnection.Open();
                return Convert.ToBoolean(myCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown setting user session client", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

        /// <summary>
        /// to insert a user session record. returns a database generated session id.
        /// </summary>
        public static bool SetSessionKey(long sessionID, string key)
        {

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SetSessionKey", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@sessionID", sessionID));
            myCommand.Parameters.Add(new SqlParameter("@sessionKey", key));
            try
            {
                myConnection.Open();
                object obj = myCommand.ExecuteScalar();
                return (Convert.ToInt32(obj) > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in inserting user session", ex);
            }
            finally
            {
                myConnection.Close();
            }
        }

		/// <summary>
		/// to update the session end time in the user's session record -returns the user's session end time.
		/// </summary>
		public static DateTime SaveUserSessionEndTime(long sessionID)
		{
			
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("SaveUserSessionEndTime", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@sessionID", sessionID));
			
			try
			{
				myConnection.Open();
				return Convert.ToDateTime( myCommand.ExecuteScalar());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in updating user's session end time",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

        /// <summary>
        /// to select a user's sessions given the session IDs
        /// </summary>
        public static SessionInfo SelectSessionInfo(long sessionID)
        {
            SessionInfo sessionInfo = null;
           
            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("SelectSessionInfo", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.Add(new SqlParameter("@sessionID", SqlDbType.BigInt));
            myCommand.Parameters["@sessionID"].Value = sessionID;
            try
            {
                myConnection.Open();
                // get session info from table user_sessions
                SqlDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    sessionInfo = new SessionInfo();

                    sessionInfo.sessionID = sessionID;
                    sessionInfo.userID = myReader.GetInt32(0);
                    sessionInfo.groupID = myReader.GetInt32(1);
                    if (!myReader.IsDBNull(2))
                        sessionInfo.clientID = myReader.GetInt32(2);
                    else
                        sessionInfo.clientID = 0;
                    sessionInfo.userName = myReader.GetString(3);
                    sessionInfo.groupName = myReader.GetString(4);
                    sessionInfo.tzOffset = myReader.GetInt32(5);
                }
                myReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in selecting sessions given sessionIDs", ex);
            }
            finally
            {
                myConnection.Close();
            }
            return sessionInfo;
        }

		/// <summary>
		/// to select a user's sessions given the session IDs
		/// </summary>
		public static UserSession[] SelectUserSessions(long[] sessionIDs)
		{
			UserSession[] us = new UserSession[sessionIDs.Length ];
			for (int i=0; i<sessionIDs.Length ; i++)
			{
				us[i] = new UserSession();
			}

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("SelectUserSession", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@sessionID",SqlDbType.BigInt));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < sessionIDs.Length ; i++) 
				{
					myCommand.Parameters["@sessionID"].Value = sessionIDs[i];

					// get session info from table user_sessions
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						us[i].sessionID = sessionIDs[i];
                        if (myReader["user_id"] != System.DBNull.Value)
                            us[i].userID = Convert.ToInt32(myReader["user_id"]);
                        if (myReader["effective_group_id"] != System.DBNull.Value)
                            us[i].groupID = Convert.ToInt32(myReader["effective_group_id"]);
                        if (myReader["client_id"] != System.DBNull.Value)
                            us[i].groupID = Convert.ToInt32(myReader["client_id"]);
						if(myReader["session_start_time"] != System.DBNull.Value )
							us[i].sessionStartTime = DateUtil.SpecifyUTC((DateTime) myReader["session_start_time"]);
						if(myReader["session_end_time"] != System.DBNull.Value )
							us[i].sessionEndTime= DateUtil.SpecifyUTC((DateTime) myReader["session_end_time"]);
						if(myReader["session_key"] != System.DBNull.Value )
							us[i].sessionKey= ((string)myReader["session_key"]);
                        if (myReader["tz_offset"] != System.DBNull.Value)
                            us[i].tzOffset = Convert.ToInt32(myReader["tz_offset"]);
					}
					myReader.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in selecting sessions given sessionIDs",ex);
			}
			finally
			{
				myConnection.Close();
			}
			return us;
		}

		/// <summary>
		/// to select all the sessions of a given user
		/// </summary>
		public static UserSession[] SelectUserSessions(int userID, int groupID, DateTime timeAfter, DateTime timeBefore)
		{
			UserSession[] userSessions = null;
			ArrayList sessions = new ArrayList();
			string sqlQuery = "";
						
			sqlQuery = "select session_ID, session_start_time, session_end_time,user_ID, effective_group_ID"+
						", session_key from user_sessions ";
			if (userID!=-1)
			{
				sqlQuery += "where user_ID = "+userID;
			}
			if (groupID !=-1)
			{
				if (userID!=-1)
					sqlQuery +=" and effective_group_ID = "+groupID;
				else
					sqlQuery +=" where effective_group_ID ="+groupID;
			}

			if (timeBefore.CompareTo(DateTime.MinValue)!=0)
			{
				if ((userID==-1)&&(groupID==-1))
					sqlQuery +=" where ";
				else
					sqlQuery +=" and ";
				sqlQuery +="session_start_time <= '"+timeBefore+"'";;
			}

			if (timeAfter.CompareTo(DateTime.MinValue)!=0)
			{
				if ((userID==-1)&&(groupID==-1)&&(timeBefore.CompareTo(DateTime.MinValue)==0))
					sqlQuery +=" where ";
				else
					sqlQuery +=" and ";
				sqlQuery +="session_start_time >= '"+timeAfter+"'";
			}

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand ();
			myCommand.Connection = myConnection;
			myCommand.CommandType = CommandType.Text;
			myCommand.CommandText = sqlQuery;

//			SqlConnection myConnection = ProcessAgentDB.GetConnection();
//			SqlCommand myCommand = new SqlCommand("SelectAllUserSessions", myConnection);
//			myCommand.CommandType = CommandType.StoredProcedure;
//			myCommand.Parameters .Add (new SqlParameter ("@userID",userID));
//			myCommand.Parameters .Add (new SqlParameter ("@groupID",groupID));
//			myCommand.Parameters .Add (new SqlParameter ("@TimeAfter",timeAfter));
//			myCommand.Parameters .Add (new SqlParameter ("@TimeBefore",timeBefore));

			try 
			{
				myConnection.Open ();
				
					// get session info from table user_sessions
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						UserSession us = new UserSession();
						us.sessionID = Convert.ToInt64( myReader["session_id"]); //casting to (long) didn't work
						if(myReader["session_start_time"] != System.DBNull.Value )
							us.sessionStartTime = DateUtil.SpecifyUTC((DateTime) myReader["session_start_time"]);
						if(myReader["session_end_time"] != System.DBNull.Value )
							us.sessionEndTime= DateUtil.SpecifyUTC((DateTime) myReader["session_end_time"]);
						if(myReader["user_id"]!=System.DBNull.Value)
							us.userID=Convert.ToInt32(myReader["user_id"]);
						if(myReader["effective_group_id"] != System.DBNull.Value )
							us.groupID= Convert.ToInt32(myReader["effective_group_id"]);
						if(myReader["session_key"] != System.DBNull.Value )
							us.sessionKey= ((string)myReader["session_key"]);
							
						sessions.Add(us);

					}
					myReader.Close ();
				
				userSessions = new UserSession[sessions.Count];
				for (int i=0;i <sessions.Count ; i++) 
				{
					userSessions[i] = (UserSession) sessions[i];
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in selecting user session",ex);
			}
			finally
			{
				myConnection.Close();
			}

			return userSessions;
		}
		
		// Will probably need new methods for user sessions by group and by time

		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR GROUPS
		 * !------------------------------------------------------------------------------!
		 */

		/// <summary>
		/// to add a group
		/// </summary>
		public static int InsertGroup(Group grp, int parentGroupID, int associatedGroupID)
		{

			// The Add Group stored procedure first inserts a group into the Agents table
			//& then Agent Hierarchy, with the specified parent group id
			// The AgentID is then used as the primary key in the groups table

			// Corresponding qualifiers  are NOT added here since they
			//	usually only created after the actual group record has been created 

			int groupID = -1;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddGroup", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupName", grp.groupName));
			myCommand.Parameters.Add(new SqlParameter("@description", grp.description));
			myCommand.Parameters.Add(new SqlParameter("@email", grp.email));
			myCommand.Parameters.Add(new SqlParameter("@parentGroupID",parentGroupID));
			myCommand.Parameters.Add(new SqlParameter("@groupType", grp.groupType));
			myCommand.Parameters.Add(new SqlParameter("@associatedGroupID", associatedGroupID));
			
			try
			{
				myConnection.Open();
				groupID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in inserting group",ex);
			}
			finally
			{
				myConnection.Close();
			}

			// refresh A & A-H in memory
			AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();
			AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
				
			return groupID;
		}

		/// <summary>
		/// to modify a group
		/// </summary>
		public static void UpdateGroup(Group grp)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifyGroup", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupID", grp.groupID));
			myCommand.Parameters.Add(new SqlParameter("@groupName", grp.groupName));
			myCommand.Parameters.Add(new SqlParameter("@description", grp.description));
			myCommand.Parameters.Add(new SqlParameter("@email",grp.email));
			
			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
				if(i == 0)
					throw new Exception ("No record modified exception");
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in modifying group",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to delete all group records specified by the array of group IDs
		/// </summary>
		/* IMPORTANT NOTE !
		 *  This method assumes that a group is empty. 
		 * So the admin API is responsible for calling the 
		 * RemoveMembersFromGroup method before this*/
		public static int[] DeleteGroups ( int[] groupIDs )
		{
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteGroup", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@groupID",null));

			/*
			 * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
			 * extremely inefficient and hence this method was chosen
			 */
			 
			ArrayList arrayList = new ArrayList ();

			try 
			{
				myConnection.Open ();
											
				foreach (int groupID in groupIDs) 
				{
					// Deleting from table Groups
					/*	
					 * IMPORTANT ! - The database if currently not set to Cascade delete for Agents and Groups.
					 * Hence the stored procedure implements the following functionality:
					 * 1. When a group (specified by groupID) is to be deleted, the agent is first deleted.
					 * 2. This cascade deletes the records in the Agent Hierarchy and Grants tables (which has to be manually done in the code if cascade delete
																				doesn't work)
					 * 3. Then the Group is deleted from the Groups Table
					 * 4. This cascade deletes the entries in the Experiment_Information 
					 *		System_Messages & User_sessions tables
					 * 5. The corresponding group entries are deleted from the Qualifiers Table (
					 *		& hence Qualifier_Hierarchy by cascade delete)
					 * 6. The Experiment_Collection qualifier of the group is also deleted from the Qualifiers Table
					 */
					myCommand.Parameters["@groupID"].Value = groupID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (groupID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteGroups",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// refresh A-H in memory
			AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
			AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();

			// refresh in memory Q & Q-H
			AuthCache.QualifierSet  = InternalAuthorizationDB.RetrieveQualifiers ();
			AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy ();

			//Converting to int array
			int[] gIDs = Utilities.ArrayListToIntArray(arrayList);
				
			return gIDs;
		}

		/// <summary>
		/// to retrieve a list of all the group IDs in the database
		/// </summary>

		public static int[] SelectGroupIDs ()
		{
			int[] groupIDs;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveGroupIDs", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			try 
			{
				myConnection.Open ();
				

				// get group ids from table groups
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList grpIDs = new ArrayList();

				while(myReader.Read ())
				{	
					if(myReader["group_id"] != System.DBNull.Value )
						grpIDs.Add(Convert.ToInt32(myReader["group_id"]));
				}
				myReader.Close ();

				// Converting to an int array
				groupIDs = Utilities.ArrayListToIntArray(grpIDs);
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectGroupIDs",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return groupIDs;
		}
        /// <summary>
        /// to retrieve a list of all the admin group IDs in the database
        /// </summary>

        public static int[] SelectAdminGroupIDs()
        {
            int[] groupIDs;

            SqlConnection myConnection = ProcessAgentDB.GetConnection();
            SqlCommand myCommand = new SqlCommand("RetrieveAdminGroupIDs", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                myConnection.Open();


                // get group ids from table groups
                SqlDataReader myReader = myCommand.ExecuteReader();
                ArrayList grpIDs = new ArrayList();

                while (myReader.Read())
                {
                    if (myReader["group_id"] != System.DBNull.Value)
                        grpIDs.Add(Convert.ToInt32(myReader["group_id"]));
                }
                myReader.Close();

                // Converting to an int array
                groupIDs = Utilities.ArrayListToIntArray(grpIDs);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown SelectAdminGroupIDs", ex);
            }
            finally
            {
                myConnection.Close();
            }

            return groupIDs;
        }

		/// <summary>
		/// to retrieve group metadata for groups specified by array of group IDs 
		/// </summary>
		public static Group[] SelectGroups ( int[] groupIDs )
		{
			Group[] g = new Group[groupIDs.Length ];
			for (int i=0; i<groupIDs.Length ; i++)
			{
				g[i] = new Group();
			}

			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveGroup", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@groupID",SqlDbType.Int));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < groupIDs.Length ; i++) 
				{
					myCommand.Parameters["@groupID"].Value = groupIDs[i];

					// get labserver info from table lab_servers
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						g[i].groupID = groupIDs[i];

						if(myReader["group_name"] != System.DBNull.Value )
							g[i].groupName= (string) myReader["group_name"];
						if(myReader["description"] != System.DBNull.Value )
							g[i].description= (string) myReader["description"];
						if(myReader["email"] != System.DBNull.Value )
							g[i].email= (string) myReader["email"];
						if(myReader["group_type"] != System.DBNull.Value )
							g[i].groupType= (string) myReader["group_type"];
						/*if(myReader["date_created"] != System.DBNull .Value )
							g[i].= (string) myReader["date_created"];*/
					}
					myReader.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectGroups",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return g;
		}

		/// <summary>
		/// to get a group's ID given groupName
		/// </summary>
		public static int SelectGroupID(string groupName)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveGroupID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupName", groupName));
			
			try
			{
				myConnection.Open();

				int groupID = Convert.ToInt32(myCommand.ExecuteScalar());

				//If group record doesn't exist return -1
				if (groupID == 0)
					groupID=-1;

				return groupID;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving group id",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to get a group's associated ID given groupID
		/// </summary>
		public static int SelectAssociatedGroupID(int groupID)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveAssociatedGroupID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
			
			try
			{
				myConnection.Open();

				int associatedGroupID = Convert.ToInt32(myCommand.ExecuteScalar());

				//If group record doesn't exist return -1
				if (associatedGroupID == 0)
					associatedGroupID=-1;

				return associatedGroupID;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving associated group id",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to get a group's request group ID given groupID
		/// </summary>
		public static int SelectGroupRequestGroup(int groupID)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveGroupRequestGroupID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
			
			try
			{
				myConnection.Open();

				int reqGroupID = Convert.ToInt32(myCommand.ExecuteScalar());

				//If group record doesn't exist return -1
				if (reqGroupID == 0)
					reqGroupID=-1;

				return reqGroupID;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving reqGroup id",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to get a group's course staff group ID given groupID
		/// </summary>
		public static int SelectGroupAdminGroup(int groupID)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("RetrieveGroupAdminGroupID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
			
			try
			{
				myConnection.Open();

				int adminGroupID = Convert.ToInt32(myCommand.ExecuteScalar());

				//If group record doesn't exist return -1
				if (adminGroupID == 0)
					adminGroupID=-1;

				return adminGroupID;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieving adminGroup id",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR GROUPS - MEMBERS
		 * !------------------------------------------------------------------------------!
		 */

		/// <summary>
		/// to add a member to a group
		/// </summary>
		public static bool InsertMemberInGroup(int memberID, int groupID)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddMemberToGroup", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
			myCommand.Parameters.Add(new SqlParameter("@memberID", memberID));
			
			try
			{
				myConnection.Open();

				// Adding a member to the group
				/*	
				* IMPORTANT ! 
				* The stored procedure implements the following functionality:
				* 1. When a user is to be added a group, he/she is
					- Removed into orphaned users group (if they exist there), AND
					- Added to the group in the agent hierarchy
						
				* 2. When a subgroup is to be added to a group it is,
					- The qualifier corresponding to the subgroup is added under the qualifier corresponding to the parent group
					- If the subgroup has an experiment collection node, it is moved under the experiment collection node of the parent(if one exists)
						except if the parent is ROOT, in which case the subgroup experiment collection node is added under Qualifier ROOT
					- Added to the group in the agent hierarchy
				*/

				int i = myCommand.ExecuteNonQuery();
				if (i<=0) 
					return false;
				else
				{
					// refresh Agents & A-H in memory
					AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
					AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();

					// refresh in memory Q & Q-H
					AuthCache.QualifierSet  = InternalAuthorizationDB.RetrieveQualifiers ();
					AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy ();

					return true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in inserting member to group. ",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to remove a member from a group
		/// </summary>
		public static bool DeleteMemberFromGroup(int memberID, int groupID)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("DeleteMemberFromGroup", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
			myCommand.Parameters.Add(new SqlParameter("@memberID", memberID));
			
			try
			{
				myConnection.Open();

				// Deleting a member from the group
				/*	
				* IMPORTANT ! 
				* The stored procedure implements the following functionality:
				* 1. When a user is to be removed from a group, he/she is either
					- Moved into orphaned users group (if they are part of only 1 group,
						 from which they're being removed) OR,
					- Just removed from the group (relationship severed in agent hierarchy table)
						if they're part of multiple groups
						
				* 2. When a subgroup is to be removed from a group it is either,
					- Moved to ROOT (if it is part of only one group from which it's being removed).
						Consequently, the corresponding group qualifier is also moved under the 
						Qualifier root & the corresponding experiment collection qualifier 
						is also moved under Qualifier root, OR
					- Just removed from the group (relationship severed in agent hierarchy table)
						if they're part of multiple groups. The corresponding group and
						experiment collection qualifier relationships are also removed.
				*/
				int i = myCommand.ExecuteNonQuery();
				if (i <=0)
					return false;
				else
				{
					// refresh Agents & A-H in memory
					AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
					AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();

					// refresh in memory Q & Q-H
					AuthCache.QualifierSet  = InternalAuthorizationDB.RetrieveQualifiers ();
					AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy ();

					return true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in removing member from group. ",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

        /// <summary>
        /// to add a member to a group
        /// </summary>
        public static bool MoveMemberToGroup(int memberID, int fromID, int groupID)
        {
            bool status = false;
            try
            {
                status = InsertMemberInGroup(memberID, groupID);
                if (!status)
                {
                    return false;
                }
                else
                {
                    int orphanID = -1;
                    SqlConnection myConnection = ProcessAgentDB.GetConnection();
                    try
                    {
                        SqlCommand isOrphanCommand = new SqlCommand("RetrieveGroupID", myConnection);
                        isOrphanCommand.CommandType = CommandType.StoredProcedure;
                        isOrphanCommand.Parameters.Add(new SqlParameter("@groupName", Group.ORPHANEDGROUP));
                        myConnection.Open();
                        orphanID = Convert.ToInt32(isOrphanCommand.ExecuteScalar());
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Exception thrown in moving member to group. ", e);
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                    if (orphanID == fromID)
                    {
                        // if the from group is Orphan the member should already have been removed
                        // Rebuild the Cache
                        status = true;
                        // refresh Agents & A-H in memory
                        AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
                        AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();

                        // refresh in memory Q & Q-H
                        AuthCache.QualifierSet = InternalAuthorizationDB.RetrieveQualifiers();
                        AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy();
                    }
                    else
                    {
                        status = DeleteMemberFromGroup(memberID, fromID);
                    }
                    return status;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in moving member to group. ", ex);
            }

        }

		/// <summary>
		/// to retrieve a list of all the members of a group
		/// </summary>
		public static Agent[] SelectMembersInGroup (int groupID)
		{
			Agent[] members;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveGroupMembers", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add (new SqlParameter("@groupID",groupID));

			try 
			{
				myConnection.Open ();

				// get Member IDs from table Agent_Hierarchy
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList mIDs = new ArrayList();

				while(myReader.Read ())
				{	
					Agent a = new Agent();
					if(myReader["agent_id"] != System.DBNull.Value )
						a.id = Convert.ToInt32(myReader["agent_ID"]);
					if(myReader["agent_name"] != System.DBNull.Value )
						a.name = ((string)myReader["agent_name"]);
					if(Convert.ToInt16(myReader["is_group"]) ==1 )
						a.type = "Group";
					else
						a.type = "User";
				//	memberIDs [i] = (string) myReader["agent_id"];

					mIDs.Add(a);
				}
				myReader.Close ();

				// Converting to an Agent array
				members = new Agent[mIDs.Count];
				for (int i=0;i <mIDs.Count ; i++) 
				{
					members[i] = (Agent) mIDs[i];
				}
											
				//mIDs.ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown Select Group's Members",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return members;
		}
		/// <summary>
		/// Method is equivalent to "ListMemberIDsInGroup", except that it acts upon a DataSet rather than on the Database
		/// </summary>
		/// <param name="groupID">the ID of the Group whose members are to be listed</param>
		/// <returns>a string array containing the member IDs of all members of the Group. A subgroup is listed by the subgroup ID, not by enumerating all members of the subgroup</returns>
		public static int[] ListMemberIDsInGroupFromDS(int groupID)
		{
			DataTable hierarchyTable = AuthCache.AgentHierarchySet.Tables[0];
			ArrayList memberIDsList = new ArrayList();
			foreach(DataRow dataRow in hierarchyTable.Rows)
			{
				if(Convert.ToInt32(dataRow["parent_group_id"])== groupID)
				{
					memberIDsList.Add(Convert.ToInt32(dataRow["agent_id"]));
				}
			}
			int[] memberIDs = new int[memberIDsList.Count];
			int i = 0;
			foreach(int memberID in memberIDsList)
			{
				memberIDs[i] = memberID;
				i++;
			}
			return memberIDs;
		}
	

		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR SYSTEM MESSAGES
		 * !------------------------------------------------------------------------------!
		 */
		/// <summary>
		/// to add a system message
		/// </summary>
		
		public static int InsertSystemMessage(SystemMessage sm)
		{
			int messageID =0;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("AddSystemMessage", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@messageType", sm.messageType ));
			myCommand.Parameters.Add(new SqlParameter("@messageTitle", sm.messageTitle ));
			myCommand.Parameters.Add(new SqlParameter("@toBeDisplayed", SqlDbType.Bit));
			if (sm.toBeDisplayed) // is true
				myCommand.Parameters["@toBeDisplayed"].Value = 1;
			else 
				myCommand.Parameters["@toBeDisplayed"].Value = 0;
            if(sm.groupID == 0)
                myCommand.Parameters.Add(new SqlParameter("@groupID", System.DBNull.Value));
            else
			    myCommand.Parameters.Add (new SqlParameter("@groupID",sm.groupID));
            if(sm.labServerID == 0)
                myCommand.Parameters.Add(new SqlParameter("@labServerID", System.DBNull.Value));
            else
			    myCommand.Parameters.Add (new SqlParameter("@labServerID",sm.labServerID));
			myCommand.Parameters.Add(new SqlParameter("@messageBody",sm.messageBody ));	

			try
			{
				myConnection.Open();
				messageID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in inserting system message",ex);
			}
			finally
			{
				myConnection.Close();
			}

			return messageID;
		}

		/// <summary>
		/// to modify a system message
		/// </summary>
		
		public static void UpdateSystemMessage(SystemMessage sm)
		{
			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("ModifySystemMessage", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			myCommand.Parameters.Add(new SqlParameter("@messageID", sm.messageID ));
			myCommand.Parameters.Add(new SqlParameter("@messageType", sm.messageType ));
			myCommand.Parameters.Add(new SqlParameter("@messageTitle", sm.messageTitle ));
			myCommand.Parameters.Add(new SqlParameter("@toBeDisplayed", SqlDbType.Bit));
			if (sm.toBeDisplayed) // is true
				myCommand.Parameters["@toBeDisplayed"].Value = 1;
			else 
				myCommand.Parameters["@toBeDisplayed"].Value = 0;
            if(sm.groupID == 0)
			myCommand.Parameters.Add (new SqlParameter("@groupID",System.DBNull.Value));
            else
            myCommand.Parameters.Add(new SqlParameter("@groupID", sm.groupID));
            if(sm.labServerID ==0)
                myCommand.Parameters.Add(new SqlParameter("@labServerID", System.DBNull.Value));
            else
			myCommand.Parameters.Add (new SqlParameter("@labServerID",sm.labServerID));
			myCommand.Parameters.Add(new SqlParameter("@messageBody",sm.messageBody ));	

			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
	
				if(i == 0)
					throw new Exception ("No record modified exception");
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in updating system message",ex);
			}
			finally
			{
				myConnection.Close();
			}
		}

		/// <summary>
		/// to delete all system message records specified by the array of system message IDs
		/// </summary>
		public static int[] DeleteSystemMessages ( int[] systemMessageIDs )
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteSystemMessageByID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@messageID",null));

			/*
			 * Note : Alternately ADO.NET could be used. However, the disconnected  DataAdapter object might prove
			 * extremely inefficient and hence this method was chosen
			 */
			 
			ArrayList arrayList = new ArrayList ();

			try 
			{
				myConnection.Open ();
											
				foreach (int messageID in systemMessageIDs) 
				{
					// Deleting from table SystemMessages
					
					myCommand.Parameters["@messageID"].Value = messageID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (messageID);
					}
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteSystemMessages",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			int[] smIDs = Utilities.ArrayListToIntArray(arrayList);

			return smIDs;
		}

		/// <summary>
		/// to delete all system message records specified by the messageType and groupID and labServerID
		/// not sure if this method is being used though
		/// </summary>
		public static void DeleteSystemMessages ( string messageType,int groupID, int labServerID )
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteSystemMessages", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@messageType",messageType));
            if (groupID == 0)
                myCommand.Parameters.Add(new SqlParameter("@groupID", System.DBNull.Value));
            else
			myCommand.Parameters.Add (new SqlParameter("@groupID",groupID));
            if(labServerID==0)
                myCommand.Parameters.Add(new SqlParameter("@labServerID", System.DBNull.Value));
                else
			myCommand.Parameters.Add (new SqlParameter("@labServerID",labServerID));

			try
			{
				myConnection.Open();
				int i = myCommand.ExecuteNonQuery();
				if(i == 0)
					throw new Exception ("No record deleted exception");
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteSystemMessages",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

		}

		/// <summary>
		/// to retrieve system message metadata for systemMessages specified by array of systemMessage IDs 
		/// This method only displays the messages where the to_be_displayed is set to true (0)
		/// </summary>
		public static SystemMessage[] SelectSystemMessages ( int[] systemMessageIDs )
		{
			SystemMessage[] sm = new SystemMessage[systemMessageIDs.Length];
			for (int i=0; i<systemMessageIDs.Length ; i++)
			{
				sm[i] = new SystemMessage();
			}

			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveSystemMessageByID", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@messageID",SqlDbType.Int));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < systemMessageIDs.Length ; i++) 
				{
					myCommand.Parameters["@messageID"].Value = systemMessageIDs[i];

					// get systemMessage info from table system_messages
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						sm[i].messageID = systemMessageIDs[i];

						if(myReader["message_body"] != System.DBNull.Value )
							sm[i].messageBody = (string) myReader["message_body"];
						byte tbd = 0;
						if(myReader["to_be_displayed"] != System.DBNull.Value ) 
						{
							tbd = Convert.ToByte( myReader["to_be_displayed"]);
						}
						if (tbd ==1)
							sm[i].toBeDisplayed = true;
						else 
							sm[i].toBeDisplayed = false;
						if(myReader["description"] != System.DBNull.Value )
							sm[i].messageType = (string) myReader["description"];
						if(myReader["last_modified"] != System.DBNull.Value )
							sm[i].lastModified = Convert.ToDateTime(myReader["last_modified"]);
                        if (myReader["group_id"] != System.DBNull.Value)
						sm[i].groupID = Convert.ToInt32(myReader["group_id"]);
                    if (myReader["lab_server_id"] != System.DBNull.Value)
						sm[i].labServerID = Convert.ToInt32(myReader["lab_server_id"]);
						if(myReader["message_title"] != System.DBNull.Value )
							sm[i].messageTitle = (string) myReader["message_title"];
					}
					myReader.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectSystemMessages",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return sm;
		}

		/// <summary>
		/// to retrieve system message metadata for systemMessages specified by messageType and group and labServerID
		/// </summary>
		public static SystemMessage[] SelectSystemMessages ( string messageType, int groupID, int labServerID )
		{
			ArrayList arrayList = new ArrayList();
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveSystemMessages", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;

			myCommand.Parameters.Add (new SqlParameter("@messageType",messageType));
            if(groupID == 0)
                myCommand.Parameters.Add(new SqlParameter("@groupID", System.DBNull.Value));
            else
			myCommand.Parameters.Add (new SqlParameter("@groupID",groupID));
            if(labServerID == 0)
                myCommand.Parameters.Add(new SqlParameter("@labServerID", System.DBNull.Value));
                else
			myCommand.Parameters.Add (new SqlParameter("@labServerID",labServerID));

			try 
			{
				myConnection.Open ();
				

				// get systemMessage info from table system_messages
				SqlDataReader myReader = myCommand.ExecuteReader ();
				while(myReader.Read ())
				{	
					SystemMessage sm = new SystemMessage();
					sm.messageType = messageType;
					sm.groupID = groupID;
					sm.labServerID = labServerID;

					if(myReader["system_message_id"] != System.DBNull.Value )
						sm.messageID = Convert.ToInt32(myReader["system_message_id"]);

					if(myReader["message_body"] != System.DBNull.Value )
						sm.messageBody = (string) myReader["message_body"];
					byte tbd = 0;
					if(myReader["to_be_displayed"] != System.DBNull.Value ) 
					{
						tbd = Convert.ToByte( myReader["to_be_displayed"]);
					}
					if (tbd ==1)
						sm.toBeDisplayed = true;
					else 
						sm.toBeDisplayed = false;
					if(myReader["last_modified"] != System.DBNull.Value )
						sm.lastModified = Convert.ToDateTime(myReader["last_modified"]);
					if(myReader["message_title"] != System.DBNull.Value )
						sm.messageTitle = (string) myReader["message_title"];

					arrayList.Add(sm);
						
				}
				myReader.Close ();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectSystemMessages",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// Converting to a SystemMessage array
			SystemMessage[] systemMessages = new SystemMessage[arrayList.Count];
			for (int i=0;i <arrayList.Count ; i++) 
			{
				systemMessages[i] = (SystemMessage) arrayList[i];
			}
			
			return systemMessages;
		}

		
		/// <summary>
		/// to retrieve system message metadata for systemMessages specified by array of systemMessage IDs 
		/// This was a method added later by Shaomin - it was required to display all the system messages for the admin pages
		/// 
		/// Its name was changed from SelectSystemMessagesSuperUser to SelectAdminSystemMessages by Charu on 5/22/04 during the converstion
		/// to the new database
		/// </summary>
		public static SystemMessage[] SelectAdminSystemMessages ( int[] systemMessageIDs )
		{
			SystemMessage[] sm = new SystemMessage[systemMessageIDs.Length];
			for (int i=0; i<systemMessageIDs.Length ; i++)
			{
				sm[i] = new SystemMessage();
			}

			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveSystemMessageByIDForAdmin", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters .Add (new SqlParameter ("@messageID",SqlDbType.Int));

			try 
			{
				myConnection.Open ();
				
				for (int i =0; i < systemMessageIDs.Length ; i++) 
				{
					myCommand.Parameters["@messageID"].Value = systemMessageIDs[i];

					// get systemMessage info from table system_messages
					SqlDataReader myReader = myCommand.ExecuteReader ();
					while(myReader.Read ())
					{	
						sm[i].messageID = systemMessageIDs[i];

						if(myReader["message_body"] != System.DBNull.Value )
							sm[i].messageBody = (string) myReader["message_body"];
						byte tbd = 0;
						if(myReader["to_be_displayed"] != System.DBNull.Value ) 
						{
							tbd = Convert.ToByte( myReader["to_be_displayed"]);
						}
						if (tbd ==1)
							sm[i].toBeDisplayed = true;
						else 
							sm[i].toBeDisplayed = false;
						if(myReader["description"] != System.DBNull.Value )
							sm[i].messageType = (string) myReader["description"];
						if(myReader["last_modified"] != System.DBNull.Value )
							sm[i].lastModified = Convert.ToDateTime(myReader["last_modified"]);
                        if (myReader["group_id"] != System.DBNull.Value)
						sm[i].groupID = Convert.ToInt32(myReader["group_id"]);
                    if (myReader["lab_server_id"] != System.DBNull.Value)
						sm[i].labServerID = Convert.ToInt32(myReader["lab_server_id"]);
						if(myReader["message_title"] != System.DBNull.Value )
							sm[i].messageTitle= (string) myReader["message_title"];
					}
					myReader.Close ();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectSystemMessages",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return sm;
		}

		/// <summary>
		/// to retrieve system message metadata for systemMessages specified by messageType and group for a superUser (gets all system messages)
		/// This was a method added later by Shaomin - it was required to display all the system messages for the admin pages
		/// 
		/// Its name was changed from SelectSystemMessagesSuperUser to SelectAdminSystemMessages by Charu on 5/22/04, during the conversion
		/// to the new database
		/// </summary>
		public static SystemMessage[] SelectAdminSystemMessages ( string messageType, int groupID, int labServerID )
		{
			ArrayList arrayList = new ArrayList();
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveSystemMessagesForAdmin", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			
			myCommand.Parameters.Add (new SqlParameter("@messageType",messageType));
            if (groupID==0)
			myCommand.Parameters.Add (new SqlParameter("@groupID",System.DBNull.Value));
            else
            myCommand.Parameters.Add(new SqlParameter("@groupID", groupID));
            if (labServerID==0)
                myCommand.Parameters.Add(new SqlParameter("@labServerID", System.DBNull.Value));
            else
			myCommand.Parameters.Add (new SqlParameter("@labServerID",labServerID));

			try 
			{
				myConnection.Open ();
				

				// get systemMessage info from table system_messages
				SqlDataReader myReader = myCommand.ExecuteReader ();
				while(myReader.Read ())
				{	
					SystemMessage sm = new SystemMessage();
					sm.messageType = messageType;
					sm.groupID = groupID;
					sm.labServerID = labServerID;

					if(myReader["system_message_id"] != System.DBNull.Value )
						sm.messageID = Convert.ToInt32(myReader["system_message_id"]);

					if(myReader["message_body"] != System.DBNull.Value )
						sm.messageBody = (string) myReader["message_body"];
					byte tbd = 0;
					if(myReader["to_be_displayed"] != System.DBNull.Value ) 
					{
						tbd = Convert.ToByte( myReader["to_be_displayed"]);
					}
					if (tbd ==1)
						sm.toBeDisplayed = true;
					else 
						sm.toBeDisplayed = false;
					if(myReader["last_modified"] != System.DBNull.Value )
						sm.lastModified = Convert.ToDateTime(myReader["last_modified"]);
					if(myReader["message_title"] != System.DBNull.Value )
						sm.messageTitle= (string) myReader["message_title"];

					arrayList.Add(sm);
						
				}
				myReader.Close ();
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectSystemMessages",ex);
			}
			finally 
			{
				myConnection.Close ();
			}

			// Converting to a SystemMessage array
			SystemMessage[] systemMessages = new SystemMessage[arrayList.Count];
			for (int i=0;i <arrayList.Count ; i++) 
			{
				systemMessages[i] = (SystemMessage) arrayList[i];
			}
			
			return systemMessages;
		}

		
		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR EXPERIMENT INFORMATION
		 * !------------------------------------------------------------------------------!
		 */
        /*
		/// <summary>
		/// to add experiment information into the index table. Returns the experimentIndexID
		/// </summary>
		/// 
		public static int CreateExperimentInIndex(long experimentID, int userID, int effectiveGroupID)
		{
			int experimentIndexID = -1;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("CreateExperimentIndex", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters.Add(new SqlParameter("@experimentID",experimentID));
			myCommand.Parameters.Add(new SqlParameter("@userID", userID));
			myCommand.Parameters.Add(new SqlParameter("@effectiveGroupID", effectiveGroupID));

			try
			{
				myConnection.Open();
				experimentIndexID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown creating experiment index",ex);
			}
			finally
			{
				myConnection.Close();
			}

			return experimentIndexID;
		}

         * 
*/


	}

}

