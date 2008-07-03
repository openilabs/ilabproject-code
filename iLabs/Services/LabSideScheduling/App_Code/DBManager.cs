/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: DBManager.cs,v 1.19 2007/06/27 22:45:02 pbailey Exp $
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using iLabs.DataTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.UtilLib;

namespace iLabs.Scheduling.LabSide
{
	public class DBManager
	{
		static String connectionStr;

		public DBManager()
		{
		}
		/// <summary>
		/// create the connection to the database
		/// </summary>
		/// <returns></returns>
		protected static SqlConnection CreateConnection()
		{
			try
			{
				if (connectionStr==null || connectionStr.Equals(""))
				{
					connectionStr=System.Configuration.ConfigurationSettings.AppSettings["sqlConnection"];
				}
			}
			catch (Exception e) 
			{
				throw new Exception(e.Message + "\n" + e.StackTrace);
			}
			// create an SqlConnection
			SqlConnection connection = new SqlConnection(connectionStr);
			return connection;
		}
		/* !------------------------------------------------------------------------------!
		 *							CALLS FOR LSSPolicy
		 * !------------------------------------------------------------------------------!
		 */
		/// <summary>
		/// ass lab side scheduling policy to determine whether a reservation from a particular group for a particular experiment shoulc be accepted or not
		/// </summary>
		/// <param name="credentialSetID"></param>
		/// <param name="experimentInfoID"></param>
		/// <param name="rule"></param>
		/// <returns></returns>the unique ID which identifies the lab scheduling policy added,>0 was successfully added,==-1 otherwise
		public static int AddLSSPolicy(int credentialSetID, int experimentInfoID,string rule)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddLSSPolicy" store procedure
			SqlCommand cmd=new SqlCommand("AddLSSPolicy",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
			credentialSetIDParam.Value = credentialSetID;
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;
			SqlParameter ruleParam = cmd.Parameters.Add("@rule", SqlDbType.VarChar, 1024);
			ruleParam.Value = rule;
			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob != null && ob!=System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add LSSPolicy",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;
			
		}
		/// <summary>
		/// delete the lab side scheduling policies specified by the lssPolicyIDs
		/// </summary>
		/// <param name="lssPolicyIDs"></param>
		/// <returns></returns>the IDs of all LssPolicies not successfully removed
		
		public static int[] RemoveLSSPolicy(int[] lssPolicyIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "deleteLSSPolicy" store procedure
			SqlCommand cmd=new SqlCommand("DeleteLSSPolicy",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@lssPolicyID", SqlDbType.Int));
			
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int lssPolicyID in lssPolicyIDs)
				{
					cmd.Parameters["@lssPolicyID"].Value = lssPolicyID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(lssPolicyID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove LSS policies",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
		/// <summary>
		/// update the data fields for the lab side scheduling policy specified by the lssPolicyID
		/// </summary>
		/// <param name="lssPolicyID"></param>
		/// <param name="credentialSetID"></param>
		/// <param name="experimentInfoID"></param>
		/// <param name="rule"></param>
		/// <returns></returns>if updated succesfully, return ture, otherwise, return false

		public static bool ModifyLSSPolicy(int lssPolicyID,int credentialSetID,int experimentInfoID,string rule)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "modifyLSSPolicy" store procedure
			SqlCommand cmd=new SqlCommand("ModifyLSSPolicy",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter lssPolicyIDParam = cmd.Parameters.Add("@lssPolicyID", SqlDbType.Int);
			lssPolicyIDParam.Value = lssPolicyID;
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
			credentialSetIDParam.Value = credentialSetID;
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;
			SqlParameter ruleParam = cmd.Parameters.Add("@rule", SqlDbType.VarChar, 1024);
			ruleParam.Value = rule;
			bool i=false;

			// execute the command
			try
			{
				int m=0;
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob != null && ob!=System.DBNull.Value)
				{
					m = Int32.Parse(ob.ToString());
				}
				if (m!=0)
				{
					i=true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add ModifyLSSPolicy",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
		/// <summary>
		/// enumerates all the IDs of the lab side scheduling policies for  a particular experiment identified by the experimentInfoIDs
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <returns></returns>an array of ints containing the IDs of all the lab side scheduling policies of specified experiment
		public static int[] ListLSSPolicyIDsByExperiment(int experimentInfoID)
		{
			int[] lssPolicyIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveLSSPolicyIDsByExperiment" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveLSSPolicyIDsByExperiment", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the lssPolicyIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["LSS_Policy_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["LSS_Policy_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve lSSPolicyIDs by group",ex);
			}
			finally
			{
				connection.Close();
			}
			lssPolicyIDs=Utilities.ArrayListToIntArray(arrayList);
			return lssPolicyIDs;
		}
		/// <summary>
		/// returns an array of the immutable LSSPolicy objects that correspond to the supplied lssPolicyIDs
		/// </summary>
		/// <param name="lssPolicyIDs"></param>
		/// <returns></returns>
		public static LSSPolicy[] GetLSSPolicies(int[] lssPolicyIDs)
		{
			LSSPolicy[] lssPolicies=new LSSPolicy[lssPolicyIDs.Length];
			for(int i=0; i<lssPolicyIDs.Length;i++)
			{
				lssPolicies[i]=new LSSPolicy();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveLSSPolicyByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveLSSPolicyByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@lssPolicyID", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<lssPolicyIDs.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@lssPolicyID"].Value = lssPolicyIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						lssPolicies[i].lssPolicyId=lssPolicyIDs[i];
						if(dataReader[0] != System.DBNull.Value )
							lssPolicies[i].experimentInfoId=(int)dataReader.GetInt32(0);
						if(dataReader[1] != System.DBNull.Value )
							lssPolicies[i].rule=dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							lssPolicies[i].credentialSetId=(int)dataReader.GetInt32(2);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get lsspolicies",ex);
			}
			finally
			{
				connection.Close();
			}	
			return lssPolicies;
		}
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR Time Block
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add a time block in which users with a particular credential set are allowed to access a particular lab server
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <param name="resourceID"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
        /// <param name="recurrenceID"></param>
        /// <returns>the uniqueID which identifies the time block added, >0 was successfully added; ==-1 otherwise</returns>
		public static int AddTimeBlock(string labServerGuid,int resourceID, DateTime startTime, DateTime endTime, int recurrrenceID)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddTimeBlock" store procedure
			SqlCommand cmd=new SqlCommand("AddTimeBlock",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerGuid;
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@resourceID", SqlDbType.Int);
			credentialSetIDParam.Value = resourceID;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = recurrrenceID;
			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob != null && ob!=System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add time block",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		
		/// <summary>
		/// delete the time blocks specified by the timeBlockIDs
		/// </summary>
		/// <param name="timeBlockIDs"></param>
		/// <returns></returns>an array of ints containning the IDs of all time blocks not successfully removed
		public static int[] RemoveTimeBlock(int[] timeBlockIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteTimeBlock" store procedure
			SqlCommand cmd=new SqlCommand("DeleteTimeBlock",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@timeBlockID", SqlDbType.Int));
			
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int timeBlockID in timeBlockIDs)
				{
					cmd.Parameters["@timeBlockID"].Value = timeBlockID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(timeBlockID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove time block",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeBlockID"></param>
		/// <param name="labServerID"></param>
		/// <param name="resourceID"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns> true if updated sucessfully, fals otherwise
		public static bool ModifyTimeBlock(int timeBlockID, string labServerGuid,int resourceID, DateTime startTime, DateTime endTime)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyTimeBlock" store procedure
			SqlCommand cmd=new SqlCommand("ModifyTimeBlock",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter timeBlockIDParam = cmd.Parameters.Add("@timeBlockID", SqlDbType.Int);
			timeBlockIDParam.Value = timeBlockID;
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerGuid;
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@resourceID", SqlDbType.Int);
			credentialSetIDParam.Value = resourceID;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;
			bool i=false;

			// execute the command
			try
			{
				connection.Open();
				int m=0 ;
				Object ob=cmd.ExecuteScalar();
				if(ob != null && ob!=System.DBNull.Value)
				{
					m= Int32.Parse(ob.ToString());
				}
				if (m!=0)
				{
					i=true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add ModifyTime",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
		
		/// <summary>
		/// enumerates all IDs of the time blocks belonging to a particular lab server identified by the labserverID
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
		public static int[] ListTimeBlockIDsByLabServer(string labServerGuid)
		{
			int[] timeBlockIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveTimeBlockIDsByLabServer" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTimeBlockIDsByLabServer", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerGuid;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the timeBlockIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Time_Block_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Time_Block_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve timeBlockIDs by lab server",ex);
			}
			finally
			{
				connection.Close();
			}
			timeBlockIDs=Utilities.ArrayListToIntArray(arrayList);
			return timeBlockIDs;
		}
        
		/// <summary>
		/// enumerates all IDs of the time blocks during which the members of a particular group identified by the credentialSetID are allowed to access a particular lab server identified by the labServerID
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <param name="credentialSetID"></param>
		/// <returns></returns>an array of ints containing the IDs of all the time blocks during which the members of a particular grou[ are allowed to access a particular lab server
		public static int[] ListTimeBlockIDsByGroup(string labServerGuid,int credentialSetID)
		{
			int[] timeBlockIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveTimeBlockIDsByGroup" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTimeBlockIDsByGroup", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerGuid;
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
			credentialSetIDParam.Value = credentialSetID;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the timeBlockIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Time_Block_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Time_Block_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve timeBlockIDs by group",ex);
			}
			finally
			{
				connection.Close();
			}
			timeBlockIDs=Utilities.ArrayListToIntArray(arrayList);
			return timeBlockIDs;
		}
		/// <summary>
		/// list the IDs of a particular lab server's time blocks which are assigned to particular group in the time chunk defined by the start time and end time
		/// </summary>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
        public static int[] ListTimeBlockIDsByTimeChunk(string serviceBrokerGuid, string groupName, string ussGuid, 
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			int[] timeBlockIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveTimeBlockIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTimeBlockIDsByTimeChunk", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
			serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,128);
			groupNameParam.Value = groupName;
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
            ussIDParam.Value = ussGuid;
            SqlParameter clientParam = cmd.Parameters.Add("@labClientGuid", SqlDbType.VarChar, 50);
            clientParam.Value = clientGuid;
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
            labServerIDParam.Value = labServerGuid;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;

		

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the timeBlockIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Time_Block_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Time_Block_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve timeBlockIDs ",ex);
			}
			finally
			{
				connection.Close();
			}
			timeBlockIDs=Utilities.ArrayListToIntArray(arrayList);
			return timeBlockIDs;
		}

		/// Enumerates the IDs of the information of all the time blocks 
		/// </summary>
		/// <returns></returns>the array  of ints contains the IDs of the information of all the time blocks
		public static int[] ListTimeBlockIDs()
		{
			int[] timeBlockIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveTimeBlockIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTimeBlockIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the timeBlockIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Time_Block_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Time_Block_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve timeBlockIDs ",ex);
			}
			finally
			{
				connection.Close();
			}
			timeBlockIDs=Utilities.ArrayListToIntArray(arrayList);
			return timeBlockIDs;
		}
		/// <summary>
		/// Returns an array of the immutable TimeBlock objects that correspond ot the supplied time block IDs
		/// </summary>
		/// <param name="timeBlockIDs"></param>
		/// <returns></returns>an array of immutable objects describing the specified time blocks
		public static TimeBlock[] GetTimeBlocks(int[] timeBlockIDs)
		{
			TimeBlock[] timeBlocks=new TimeBlock[timeBlockIDs.Length];
			for(int i=0; i<timeBlockIDs.Length;i++)
			{
				timeBlocks[i]=new TimeBlock();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveTimeBlockByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTimeBlockByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add("@timeBlockID", SqlDbType.Int);
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0; i<timeBlockIDs.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@timeBlockID"].Value = timeBlockIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						//timeBlocks[i].timeBlockId=timeBlockIDs[i];
                        if (dataReader[0] != System.DBNull.Value)
                        {
                            DateTime temp = dataReader.GetDateTime(0);
                            timeBlocks[i].startTime = DateUtil.SpecifyUTC(temp);
                        }
						if(dataReader[1] != System.DBNull.Value )
							timeBlocks[i].endTime=DateUtil.SpecifyUTC(dataReader.GetDateTime(1));
						if(dataReader[2] != System.DBNull.Value )
							timeBlocks[i].labServerGuid=dataReader.GetString(2);
                        //if(dataReader[3] != System.DBNull.Value )
                        //    timeBlocks[i].credentialSetId=(int)dataReader.GetInt32(3);
                        //if (dataReader[4] != System.DBNull.Value)
                        //    timeBlocks[i].recurrenceID = (int)dataReader.GetInt32(4);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get time blocks",ex);
			}
			finally
			{
				connection.Close();
			}	
			return timeBlocks;
		}
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR Experiment Information
			 * !------------------------------------------------------------------------------!
			 */
		// <summary>
		/// add information of particular experiment
		/// </summary>
        /// <param name="labClientGuid"></param>
        /// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="labClientName"></param>
		/// <param name="providerName"></param>
		/// <param name="quantum"></param>
		/// <param name="prepareTime"></param>
		/// <param name="recoverTime"></param>
		/// <param name="minimumTime"></param>
		/// <param name="earlyArriveTime"></param>
		/// <returns></returns>the unique ID which identifies the experiment information added, >0 was successfully added, ==-1 otherwise
        public static int AddExperimentInfo(string labServerGuid, string labServerName, string labClientGuid, string labClientName, string labClientVersion, string providerName, int quantum, int prepareTime, int recoverTime, int minimumTime, int earlyArriveTime)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddExperimentInfo" store procedure
			SqlCommand cmd=new SqlCommand("AddExperimentInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
           
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerGuid;
			SqlParameter labServerNameParam = cmd.Parameters.Add("@labServerName", SqlDbType.VarChar,50);
			labServerNameParam.Value = labServerName;
            SqlParameter labClientIDParam = cmd.Parameters.Add("@labClientGUID", SqlDbType.VarChar, 50);
            labClientIDParam.Value = labClientGuid;
			SqlParameter labClientNameParam = cmd.Parameters.Add("@labClientName", SqlDbType.VarChar,50);
			labClientNameParam.Value = labClientName;
            SqlParameter labClientVersionParam = cmd.Parameters.Add("@labClientVersion", SqlDbType.VarChar, 50);
            labClientVersionParam.Value = labClientVersion;
			SqlParameter providerNameParam = cmd.Parameters.Add("@providerName", SqlDbType.VarChar,50);
			providerNameParam.Value = providerName;
			SqlParameter quantumParam = cmd.Parameters.Add("@quantum", SqlDbType.Int);
			quantumParam.Value = quantum;
			SqlParameter prepareTimeParam = cmd.Parameters.Add("@prepareTime", SqlDbType.Int);
			prepareTimeParam.Value = prepareTime;
			SqlParameter minimumTimeParam = cmd.Parameters.Add("@minimumTime", SqlDbType.Int);
			minimumTimeParam.Value = minimumTime;
			SqlParameter recoverTimeParam = cmd.Parameters.Add("@recoverTime", SqlDbType.Int);
			recoverTimeParam.Value = recoverTime;
			SqlParameter earlyArriveTimeParam = cmd.Parameters.Add("@earlyArriveTime", SqlDbType.Int);
			earlyArriveTimeParam.Value = earlyArriveTime;

			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob!=null && ob!=System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add experiment Infomation",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		
		/// <summary>
		/// delete the experiment information specified by the experimentInfoIDs
		/// </summary>
		/// <param name="experimentInfoIDs"></param>
		/// <returns></returns>an array of ints containing the IDs of all experiment information not successfully removed
		public static int[] RemoveExperimentInfo(int[] experimentInfoIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteExperimentInfo" store procedure
			SqlCommand cmd=new SqlCommand("DeleteExperimentInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@experimentInfoID", SqlDbType.Int));
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int experimentInfoID in experimentInfoIDs)
				{
					cmd.Parameters["@experimentInfoID"].Value = experimentInfoID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(experimentInfoID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove experiment information",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
		/// <summary>
		/// update the data fields for the experiment information specified by the experimentInfoID
		/// </summary>
		/// <param name="experimentInfoID"></param>
        /// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="labClientName"></param>
		/// <param name="providerName"></param>
		/// <param name="quantum"></param>
		/// <param name="prepareTime"></param>
		/// <param name="recoverTime"></param>
		/// <param name="minimumTime"></param>
		/// <param name="earlyArriveTime"></param>
		/// <returns></returns>true if modified successfully, falso otherwise
        public static bool ModifyExperimentInfo(int experimentInfoID, string labServerGuid, string labServerName, string labClientGuid, string labClientName, string labClientVersion, string providerName, int quantum, int prepareTime, int recoverTime, int minimumTime, int earlyArriveTime)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyExperimentInfo" store procedure
			SqlCommand cmd=new SqlCommand("ModifyExperimentInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;
            SqlParameter labClientIDParam = cmd.Parameters.Add("@labClientGUID", SqlDbType.VarChar, 50);
            labClientIDParam.Value = labClientGuid;
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
            labServerIDParam.Value = labServerGuid;
			SqlParameter labServerNameParam = cmd.Parameters.Add("@labServerName", SqlDbType.VarChar,256);
			labServerNameParam.Value = labServerName;
			SqlParameter labClientVersionParam = cmd.Parameters.Add("@labClientVersion", SqlDbType.VarChar,50);
			labClientVersionParam.Value = labClientVersion;
			SqlParameter labClientNameParam = cmd.Parameters.Add("@labClientName", SqlDbType.VarChar,256);
			labClientNameParam.Value = labClientName;
			SqlParameter providerNameParam = cmd.Parameters.Add("@providerName", SqlDbType.VarChar,256);
			providerNameParam.Value = providerName;
			SqlParameter quantumParam = cmd.Parameters.Add("@quantum", SqlDbType.Int);
			quantumParam.Value = quantum;
			SqlParameter prepareTimeParam = cmd.Parameters.Add("@prepareTime", SqlDbType.Int);
			prepareTimeParam.Value = prepareTime;
			SqlParameter recoverTimeParam = cmd.Parameters.Add("@recoverTime", SqlDbType.Int);
			recoverTimeParam.Value = recoverTime;
			SqlParameter minimumTimeParam = cmd.Parameters.Add("@minimumTime", SqlDbType.Int);
			minimumTimeParam.Value = minimumTime;
			SqlParameter earlyArriveTimeParam = cmd.Parameters.Add("@earlyArriveTime", SqlDbType.Int);
			earlyArriveTimeParam.Value = earlyArriveTime;
			bool i=false;

			// execute the command
			try
			{
				connection.Open();
				int m=0;
				Object ob=cmd.ExecuteScalar();
				if(ob!=null && ob!=System.DBNull.Value)
				{
					m= Int32.Parse(ob.ToString());
				}
				if (m!=0)
				{
					i=true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add Modify Experiment Infomation",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
		
		/// <summary>
		/// enumerates IDs of the information of all the experiments belonging to certain lab server identified by the labserverID
		/// </summary>
		/// <param name="labServerID"></param>
		/// <returns></returns>an array of ints containing the IDs of the information of all the experiments belonging to specified lab server
		public static int[] ListExperimentInfoIDsByLabServer(string labServerGuid)
		{
			int[] experimentInfoIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentInfoIDsByLabServer" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentInfoIDsByLabServer", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerGuid;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader();
				//store the experimetnInfoIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Experiment_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Experiment_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve experimentInfoIDs by lab server",ex);
			}
			finally
			{
				connection.Close();
			}
			experimentInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return experimentInfoIDs;
		}

        /// <summary>
        /// get the labserver name according to the labserver ID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>
        public static string RetrieveLabServerName(string labServerGuid)
        {
            string labServerName = null;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveExperimentInfoIDsByLabServer" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveLabServerName", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar, 50);
            labServerIDParam.Value = labServerGuid;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob!=null && ob != System.DBNull.Value)
                {
                    labServerName = ob.ToString();
                }
                return labServerName;
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve experimentInfoIDs by lab server", ex);
            }
            finally
            {
                connection.Close();
            }
            
           
        }
		/// <summary>
		/// retrieve the ids of all the experiment information
		/// </summary>
		/// <returns></returns>an array of ints containing the IDs of the information of all the experiments 
		public static int[] ListExperimentInfoIDs()
		{
			int[] experimentInfoIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentInfoIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentInfoIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the experimetnInfoIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Experiment_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Experiment_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve experimentInfoIDs ",ex);
			}
			finally
			{
				connection.Close();
			}
			experimentInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return experimentInfoIDs;
		}
		/// <summary>
		/// enumerates the ID of the information of a particular experiment specified by labClientName and labClientVersion
		/// </summary>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <returns></returns>the ID of the information of a particular experiment, -1 if such a experiment info can not be retrieved
		public static int ListExperimentInfoIDByExperiment(string clientGuid,string labServerGuid)
		{
			int experimentInfoID=-1;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentInfoIDByExperiment" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentInfoIDByExperiment", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labClientGuidParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,50);
			labClientGuidParam.Value = clientGuid;
			SqlParameter labServerParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
			labServerParam.Value = labServerGuid;

			// execute the command
			
			try 
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					experimentInfoID=Int32.Parse(ob.ToString());
				}
				
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve experimentInfoIDs by experiment",ex);
			}
			finally
			{
				connection.Close();
			}
			return experimentInfoID;
		}
        
		/// <summary>
		/// Return an array of the immutable USSInfo objects thta correspond to the supplied USS information IDs
		/// </summary>
		/// <param name="experimentInfoIDs"></param>
		/// <returns></returns>
		
		public static LssExperimentInfo[] GetExperimentInfos(int[] experimentInfoIDs)
		{
			LssExperimentInfo[] experimentInfos=new LssExperimentInfo[experimentInfoIDs.Length];
			for(int i=0; i<experimentInfoIDs.Length;i++)
			{
				experimentInfos[i]=new LssExperimentInfo();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentInfoByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentInfoByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters .Add (new SqlParameter("@experimentInfoID", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<experimentInfoIDs.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@experimentInfoID"].Value = experimentInfoIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						experimentInfos[i].experimentInfoId=experimentInfoIDs[i];
						if(dataReader[0] != System.DBNull.Value )
							experimentInfos[i].labClientGuid=dataReader.GetString(0);
                        if (dataReader[1] != System.DBNull.Value)
                            experimentInfos[i].labServerGuid = dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							experimentInfos[i].labServerName=dataReader.GetString(2);
						if(dataReader[3] != System.DBNull.Value )
							experimentInfos[i].labClientVersion=dataReader.GetString(3);
						if(dataReader[4] != System.DBNull.Value )
							experimentInfos[i].labClientName=dataReader.GetString(4);
						if(dataReader[5] != System.DBNull.Value )
							experimentInfos[i].providerName=dataReader.GetString(5);
						if(dataReader[6] != System.DBNull.Value )
							experimentInfos[i].quantum=dataReader.GetInt32(6);
						if(dataReader[7] != System.DBNull.Value )
							experimentInfos[i].prepareTime=dataReader.GetInt32(7);
						if(dataReader[8] != System.DBNull.Value )
							experimentInfos[i].recoverTime=dataReader.GetInt32(8);
						if(dataReader[9] != System.DBNull.Value )
							experimentInfos[i].minimumTime=dataReader.GetInt32(9);
						if(dataReader[10] != System.DBNull.Value )
							experimentInfos[i].earlyArriveTime=dataReader.GetInt32(10);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get experiment inforamtion",ex);
			}
			finally
			{
				connection.Close();
			}	
			return experimentInfos;
		}

        /* !------------------------------------------------------------------------------!
			 *							CALLS FOR LabServer Resources
			 * !------------------------------------------------------------------------------!
			 */

        public static int CheckForLSResource(string guid, string labServerName){
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_AddGetID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);
            guidParam.Value = guid;
            SqlParameter nameParam = cmd.Parameters.Add("@name", SqlDbType.VarChar,256);
            nameParam.Value = labServerName;
            int i = -1;

            // execute the command
            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    i = Convert.ToInt32(ob);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add permitted experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return i;
        }
   /*      */

        public static LSResource[] GetLSResources()
        {
            List<LSResource> resources = null;
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_GetAll", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            // execute the command
            try
            {
                connection.Open();
                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    resources = new List<LSResource>();
                    while (dataReader.Read())
                    {
                        LSResource resource = new LSResource();
                        resource.resourceID = dataReader.GetInt32(0);
                        resource.labServerGuid = dataReader.GetString(1);
                        resource.labServerName = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3))
                            resource.description = dataReader.GetString(3);
                        resources.Add(resource);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add permitted experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return resources.ToArray();
        }
        public static LSResource GetLSResource(int id)
        {
            LSResource resource = null;
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_Get", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
            idParam.Value = id;
            // execute the command
            try
            {
                connection.Open();
                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        resource = new LSResource();
                        resource.resourceID = dataReader.GetInt32(0);
                        resource.labServerGuid = dataReader.GetString(1);
                        resource.labServerName = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3))
                            resource.description = dataReader.GetString(3);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add permitted experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return resource;
        }

        public static LSResource GetLSResource(string guid)
        {
            LSResource resource = null;
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_GetByGuid", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            guidParam.Value = guid;
            // execute the command
            try
            {
                connection.Open();
                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        resource = new LSResource();
                        resource.resourceID = dataReader.GetInt32(0);
                        resource.labServerGuid = dataReader.GetString(1);
                        resource.labServerName = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3))
                            resource.description = dataReader.GetString(3);
                    }
                }
               
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add permitted experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return resource;
        }

        public static IntTag[] GetLSResourceTags()
        {
            List<IntTag> tags = new List<IntTag>();
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_GetTags", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            
            // execute the command
            try
            {
                connection.Open();
               SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        IntTag tag = new IntTag();
                        tag.id = dataReader.GetInt32(0);
                        tag.tag = dataReader.GetString(1);
                        tags.Add(tag);
                    }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in GetResourceTags", ex);
            }
            finally
            {
                connection.Close();
            }
            return tags.ToArray();
        }

        public static IntTag[] GetLSResourceTags(string guid)
        {
            List<IntTag> tags = new List<IntTag>();
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_GetTagsByGuid", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            guidParam.Value = guid;

            // execute the command
            try
            {
                connection.Open();
                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    IntTag tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);
                    tags.Add(tag);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add permitted experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return tags.ToArray();
        }

        public static int InsertLSResource(string guid,string name,string description)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_Insert", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            guidParam.Value = guid;
            SqlParameter nameParam = cmd.Parameters.Add("@name", SqlDbType.VarChar, 256);
            nameParam.Value = name;
            SqlParameter descParam = cmd.Parameters.Add("@description", SqlDbType.VarChar, 1024);
            descParam.Value = description;

            // execute the command
            try
            {
                connection.Open();
                SqlDataReader dataReader = null;
                return Convert.ToInt32(cmd.ExecuteScalar());
               
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add InsertResource", ex);
            }
            finally
            {
                connection.Close();
            }
           
        }

        public static int SetLSResourceDescription(int id, string description)
        {
            
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            SqlCommand cmd = new SqlCommand("Resource_SetDescription", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.VarChar, 50);
            idParam.Value = id;
            SqlParameter descParam = cmd.Parameters.Add("@description", SqlDbType.VarChar, 1024);
            descParam.Value = description;

            // execute the command
            try
            {
                connection.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add InsertResource", ex);
            }
            finally
            {
                connection.Close();
            }

        }

		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR Permited Experiment
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add permission of a particular experiment being executed in a particular recurrence
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <param name="recurrenceID"></param>
		/// <returns></returns>the unique ID which identifies the permission added. >0 was successfully added;==-1 otherwise
		public static int AddPermittedExperiment(int experimentInfoID, int recurrenceID)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddPermittedExperiment" store procedure
			SqlCommand cmd=new SqlCommand("AddPermittedExperiment",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
			recurrenceIDParam.Value = recurrenceID;
			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob!=null && ob!=System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add permitted experiment",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		
		/// <summary>
		/// delete permission of  a particular experiment being executed in a particular time block
		/// </summary>
		/// <param name="permittedExperimentIDs"></param>
		/// <returns></returns>an array of ints containing the IDs of all permissions not successfully removed
		public static int[] RemovePermittedExperiments(int[] permittedExperimentIDs,int recurrenceID)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeletePermittedExperiment" store procedure
			SqlCommand cmd=new SqlCommand("DeletePermittedExperiment",connection);
			cmd.CommandType=CommandType.StoredProcedure;
            SqlParameter expParameter = cmd.Parameters.Add(new SqlParameter("@experimentID", SqlDbType.Int));
            SqlParameter recurrParameter = cmd.Parameters.Add(new SqlParameter("@recurrenceID", SqlDbType.Int));
            recurrParameter.Value = recurrenceID;
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int permittedExperimentID in permittedExperimentIDs)
				{
                    expParameter.Value = permittedExperimentID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(permittedExperimentID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove permitted experiments",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
		
		/// <summary>
		/// enumerates the IDs of information of the permitted experiments for a particular time block identified by the timeBlockID
		/// </summary>
		/// <param name="timeBlockID"></param>
		/// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular time block identified by the timeBlockID
		public static int[] ListPermittedExperimentInfoIDsByTimeBlock(int timeBlockID)
		{
			
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrievePermittedExperimentInfoIDsByTimeBlock" stored procedure
			SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentInfoIDsByTimeBlock", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter timeBlockIDParam = cmd.Parameters.Add("@timeBlockID", SqlDbType.Int);
			timeBlockIDParam.Value = timeBlockID;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the permitteExperimentIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Experiment_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Experiment_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve permittedExperimentInfoIDs by timeBlock",ex);
			}
			finally
			{
				connection.Close();
			}
			int[] permittedExperimentInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return permittedExperimentInfoIDs;
		}
        /// <summary>
        /// enumerates the IDs of information of the permitted experiments for a particular recurrence identified by the recurrenceID
        /// </summary>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular recurrence identified by the recurrenceID
        public static int[] ListExperimentInfoIDsByRecurrence(int recurrenceID)
        {

            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentInfoIDsByRecurrence" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentInfoIDsByRecurrence", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = recurrenceID;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the permitteExperimentIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Experiment_Info_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Experiment_Info_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve permittedExperimentInfoIDs by recurrence", ex);
            }
            finally
            {
                connection.Close();
            }
            int[] permittedExperimentInfoIDs = Utilities.ArrayListToIntArray(arrayList);
            return permittedExperimentInfoIDs;
        }
		/// <summary>
		/// returns an array of the immutable PermittedExperiment objects that correspond to the supplied permittedExperimentIDs
		/// </summary>
		/// <param name="permittedExperimentIDs"></param>
		/// <returns></returns>an array of immutable objects describing the specified PermittedExperiments
		public static PermittedExperiment[] GetPermittedExperiments(int[] permittedExperimentIDs)
		{
			PermittedExperiment[] permittedExperiments=new  PermittedExperiment[permittedExperimentIDs.Length];
			for(int i=0; i<permittedExperimentIDs.Length;i++)
			{
				permittedExperiments[i]=new  PermittedExperiment();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrievePermittedExperimentByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@permittedExperimentID", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<permittedExperimentIDs.Length;i++)
				{
					// populate the parameters
					
					cmd.Parameters["@permittedExperimentID"].Value = permittedExperimentIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						permittedExperiments[i].permittedExperimentId=permittedExperimentIDs[i];
						if(dataReader[0] != System.DBNull.Value )
							permittedExperiments[i].experimentInfoId=(int)dataReader.GetInt32(0);
						if(dataReader[1] != System.DBNull.Value )
							permittedExperiments[i].recurrenceId=(int)dataReader.GetInt32(1);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get permittedExperiment",ex);
			}
			finally
			{
				connection.Close();
			}	
			return permittedExperiments;
		}
		/// <summary>
		/// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular time block
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <param name="timeBlockID"></param>
		/// <returns></returns>-1 if the permission can not be retrieved
		public static int ListPermittedExperimentID(int experimentInfoID, int timeBlockID)
		{
			int permittedExperimentID=-1;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrievePermittedExperimentID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentID", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;
			SqlParameter timeBlockIDParam = cmd.Parameters.Add("@timeBlockID", SqlDbType.Int);
			timeBlockIDParam.Value = timeBlockID;

			// execute the command
			
			try 
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					permittedExperimentID = Int32.Parse(ob.ToString());
				}
					
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve permittedExperimentIDs by timeBlock",ex);
			}
			finally
			{
				connection.Close();
			}
			
			return permittedExperimentID;
		}
        /// <summary>
        /// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular recurrence
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>-1 if the permission can not be retrieved
        public static int ListPermittedExperimentIDByRecur(int experimentInfoID, int recurrenceID)
        {
            int permittedExperimentID = -1;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentID" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentIDByRecur", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
            experimentInfoIDParam.Value = experimentInfoID;
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = recurrenceID;

            // execute the command

            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    permittedExperimentID = Int32.Parse(ob.ToString());
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve permittedExperimentIDs by recurrence", ex);
            }
            finally
            {
                connection.Close();
            }

            return permittedExperimentID;
        }

        /* !------------------------------------------------------------------------------!
			 *							CALLS FOR Permited CredentialSet
			 * !------------------------------------------------------------------------------!
			 */
        /// <summary>
        /// add permission of a particular experiment being executed in a particular recurrence
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>the unique ID which identifies the permission added. >0 was successfully added;==-1 otherwise
        public static int AddPermittedCredentialSet(int credentialSetID, int recurrenceID)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "AddPermittedExperiment" store procedure
            SqlCommand cmd = new SqlCommand("AddPermittedGroup", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter CredentialSetIDParam = cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
            CredentialSetIDParam.Value = credentialSetID;
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = recurrenceID;
            int i = -1;

            // execute the command
            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    i = Int32.Parse(ob.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add permitted experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return i;
        }

        /// <summary>
        /// delete permission of  a particular experiment being executed in a particular time block
        /// </summary>
        /// <param name="permittedExperimentIDs"></param>
        /// <returns></returns>an array of ints containing the IDs of all permissions not successfully removed
        public static int[] RemovePermittedCredentialSets(int[] permittedCredentialSetIDs, int recurrenceID)
        {
            ArrayList arrayList = new ArrayList();
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "DeletePermittedExperiment" store procedure
            SqlCommand cmd = new SqlCommand("DeletePermittedGroup", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@groupID", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@recurrenceID", SqlDbType.Int));
            cmd.Parameters["@recurrenceID"].Value = recurrenceID;
            // execute the command
            try
            {
                connection.Open();
                //populate the parameters
                foreach (int permittedCredentialSetID in permittedCredentialSetIDs)
                {
                    cmd.Parameters["@groupID"].Value = permittedCredentialSetID;
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        arrayList.Add(permittedCredentialSetID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in remove permitted experiments", ex);
            }
            finally
            {
                connection.Close();
            }
            int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
            return uIDs;
        }

        /// <summary>
        /// enumerates the IDs of information of the permitted experiments for a particular time block identified by the timeBlockID
        /// </summary>
        /// <param name="timeBlockID"></param>
        /// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular time block identified by the timeBlockID
        public static int[] ListCredentialSetIDsByTimeBlock(int timeBlockID)
        {

            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentInfoIDsByTimeBlock" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedCredentialSetIDsByTimeBlock", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter timeBlockIDParam = cmd.Parameters.Add("@timeBlockID", SqlDbType.Int);
            timeBlockIDParam.Value = timeBlockID;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the permitteExperimentIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Credential_Set_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Credential_Set_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve permittedCredentialSetIDs by timeBlock", ex);
            }
            finally
            {
                connection.Close();
            }
            int[] permittedCredentialSetIDs = Utilities.ArrayListToIntArray(arrayList);
            return permittedCredentialSetIDs;
        }
        public static bool IsPermittedCredentialSet(int credentialSetID, int timeBlockID)
        {
              // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentInfoIDsByRecurrence" stored procedure
            SqlCommand cmd = new SqlCommand("IsPermittedCredentialSetByRecurrence", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
             SqlParameter groupIDParam = cmd.Parameters.Add("@credentialID", SqlDbType.Int);
            groupIDParam.Value = credentialSetID;
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = timeBlockID;

            int count = 0;
            try
            {
                connection.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return count > 0;
        }

        /// <summary>
        /// enumerates the IDs of information of the permitted experiments for a particular recurrence identified by the recurrenceID
        /// </summary>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular recurrence identified by the recurrenceID
        public static int[] ListCredentialSetIDsByRecurrence(int recurrenceID)
        {

            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentInfoIDsByRecurrence" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedGroupIDsForRecurrence", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = recurrenceID;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the permitteExperimentIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Credential_Set_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Credential_Set_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve permittedCredentialSetIDs by recurrence", ex);
            }
            finally
            {
                connection.Close();
            }
            int[] permittedCredentialSetIDs = Utilities.ArrayListToIntArray(arrayList);
            return permittedCredentialSetIDs;
        }

      /*  
        /// <summary>
        /// returns an array of the immutable PermittedExperiment objects that correspond to the supplied permittedExperimentIDs
        /// </summary>
        /// <param name="permittedExperimentIDs"></param>
        /// <returns></returns>an array of immutable objects describing the specified PermittedExperiments
        public static CredentialSet[] GetPermittedCredentialSet(int[] permittedCredentialSetIDs)
        {
            CredentialSet[] permittedCredentialSets = new PermittedExperiment[permittedCredentialSetIDs.Length];
            for (int i = 0; i < permittedCredentialSetIDs.Length; i++)
            {
                permittedCredentialSets[i] = new PermittedExperiment();
            }
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentByID" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentByID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@permittedExperimentID", SqlDbType.Int));
            //execute the command
            try
            {
                connection.Open();
                for (int i = 0; i < permittedExperimentIDs.Length; i++)
                {
                    // populate the parameters

                    cmd.Parameters["@permittedExperimentID"].Value = permittedExperimentIDs[i];
                    SqlDataReader dataReader = null;
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        permittedExperiments[i].permittedExperimentId = permittedExperimentIDs[i];
                        if (dataReader[0] != System.DBNull.Value)
                            permittedExperiments[i].experimentInfoId = (int)dataReader.GetInt32(0);
                        if (dataReader[1] != System.DBNull.Value)
                            permittedExperiments[i].recurrenceId = (int)dataReader.GetInt32(1);
                    }
                    dataReader.Close();
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in get permittedExperiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return permittedExperiments;
        }
        
        /// <summary>
        /// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular time block
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="timeBlockID"></param>
        /// <returns></returns>-1 if the permission can not be retrieved
        public static int ListPermittedExperimentID(int experimentInfoID, int timeBlockID)
        {
            int permittedExperimentID = -1;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentID" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentID", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
            experimentInfoIDParam.Value = experimentInfoID;
            SqlParameter timeBlockIDParam = cmd.Parameters.Add("@timeBlockID", SqlDbType.Int);
            timeBlockIDParam.Value = timeBlockID;

            // execute the command

            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    permittedExperimentID = Int32.Parse(ob.ToString());
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve permittedExperimentIDs by timeBlock", ex);
            }
            finally
            {
                connection.Close();
            }

            return permittedExperimentID;
        }
         * /
         * 
        /// <summary>
        /// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular recurrence
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>-1 if the permission can not be retrieved
        public static int ListPermittedExperimentIDByRecur(int experimentInfoID, int recurrenceID)
        {
            int permittedExperimentID = -1;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrievePermittedExperimentID" stored procedure
            SqlCommand cmd = new SqlCommand("RetrievePermittedExperimentIDByRecur", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
            experimentInfoIDParam.Value = experimentInfoID;
            SqlParameter recurrenceIDParam = cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            recurrenceIDParam.Value = recurrenceID;

            // execute the command

            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    permittedExperimentID = Int32.Parse(ob.ToString());
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve permittedExperimentIDs by recurrence", ex);
            }
            finally
            {
                connection.Close();
            }

            return permittedExperimentID;
        }
        */

		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR ReservationInfo
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add reservation information
		/// </summary>
		/// <param name="serviceBrokerID"></param>
		/// <param name="groupName"></param>
		/// <param name="ussID"></param>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>the unique ID identifying the reservation information added, >0 successfully added, -1 otherwise
		public static int AddReservationInfo(string serviceBrokerID, string groupName, string ussID,
            string clientGuid,string labServerGuid, DateTime startTime, DateTime endTime)
		{
            int status = -1;
			//create a connection
			SqlConnection connection= CreateConnection();
            try
            {
			//create a command
			//command executes the "AddReservationInfo" store procedure
			SqlCommand cmd=new SqlCommand("AddReservationInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
			serviceBrokerIDParam.Value = serviceBrokerID;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
			ussIDParam.Value = ussID;
			SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,256);
			labClientNameParam.Value = clientGuid;
			SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
			labClientVersionParam.Value = labServerGuid;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;
			

			// execute the command
			
				connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    status = Convert.ToInt32(cmd.ExecuteScalar());
                }
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add ReservationInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			return status;		
		}
		
        /// <summary>
        /// add reservationInfo
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="credentialSetID"></param>
        /// <param name="experimentInfoID"></param>
        /// <returns></returns>the unique ID identifying the reservation information added, >0 successfully added, -1 otherwise
		public static int AddReservationInfo(DateTime startTime, DateTime endTime,int credentialSetID, int experimentInfoID)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddReservation" store procedure
			SqlCommand cmd=new SqlCommand("AddReservation",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
			credentialSetIDParam.Value = credentialSetID;
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;

			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob!=null && ob!=System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add ReservationInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		
		/// <summary>
		/// delete the reservation information specified by the reservationInfoIDs
		/// </summary>
		/// <param name="reservationInfoIDs"></param>
		/// <returns></returns>an array of ints containning the IDs of all reservation information not successfully removed
		public static int[] RemoveReservationInfoByIDs(int[] reservationInfoIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteReservationInfoByID" store procedure
			SqlCommand cmd=new SqlCommand("DeleteReservationInfoByID",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@reservationInfoID", SqlDbType.Int));
			
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int reservationInfoID in reservationInfoIDs)
				{
					cmd.Parameters["@reservationInfoID"].Value = reservationInfoID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(reservationInfoID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove reservationInfobyID",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
        
		/// <summary>
		/// remove the reservation information
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>true remove successfully, false otherwise
        public static bool RemoveReservationInfo(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteReservationInfo" store procedure
			SqlCommand cmd=new SqlCommand("DeleteReservationInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			bool i=false;
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
			
				SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
                serviceBrokerIDParam.Value = serviceBrokerGuid;
				SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
				groupNameParam.Value = groupName;
				SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
                ussIDParam.Value = ussGuid;
				SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,256);
				labClientNameParam.Value = clientGuid;
				SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
				labClientVersionParam.Value = labServerGuid;
				SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
				startTimeParam.Value = startTime;
				SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
				endTimeParam.Value = endTime;
				if (cmd.ExecuteNonQuery()!=0)
				{
					i=true;
				}
				
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove reservationInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			
			return i;			
		}
		
		/// <summary>
		/// enumerates all IDs of the reservations made to a particular experiment identified by the experimentInfoID
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <returns></returns>an array of ints containing the IDs of all the reservation information made to the specified experiment
		public static int[] ListReservationInfoIDsByExperiment(int experimentInfoID)
		{
			int[] reservationInfoIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReserveInfoIDsByExperiment" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReserveInfoIDsByExperiment", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter experimentInfoIDParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
			experimentInfoIDParam.Value = experimentInfoID;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the reservationInfoIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Reservation_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Reservation_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve reservationInfoIDs by experiment",ex);
			}
			finally
			{
				connection.Close();
			}
			reservationInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return reservationInfoIDs;
		}
		/// <summary>
		/// enumerates all IDs of the reservations made to a particular experiment from a particular group between the start time and the end time
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
        public static int[] ListReservationInfoIDs(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			int[] reservationInfoIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReserveInfoIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReserveInfoIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
            ussIDParam.Value = ussGuid;
			SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,256);
			labClientNameParam.Value = clientGuid;
			SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
			labClientVersionParam.Value = labServerGuid;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;

		

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the reservationInfoIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Reservation_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Reservation_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve reservationInfoIDs ",ex);
			}
			finally
			{
				connection.Close();
			}
			reservationInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return reservationInfoIDs;
		}
/// <summary>
/// retrieve reservation made to a particular labserver during a given time chunk.
/// </summary>
/// <param name="labServerID"></param>
/// <param name="startTime"></param>
/// <param name="endTime"></param>
/// <returns></returns>
		public static int[] ListReservationInfoIDsByLabServer(string labServerID, DateTime startTime, DateTime endTime)
		{
			int[] reservationInfoIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReservationInfoIDByLabServer" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReservationInfoIDByLabServer", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
			labServerIDParam.Value = labServerID;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;

		

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the reservationInfoIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Reservation_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Reservation_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve reservationInfoIDsByLabServer ",ex);
			}
			finally
			{
				connection.Close();
			}
			reservationInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return reservationInfoIDs;
		}
		
		/// <summary>
		/// to select reservation Infos accorrding to given criterion
		/// </summary>
        public static ReservationInfo[] SelectReservationInfo(string labServerGuid, int experimentInfoID, int credentialSetID, DateTime timeAfter, DateTime timeBefore)
		{
			ReservationInfo[] reservationInfos = null;
			ArrayList reInfos = new ArrayList();
			string sqlQuery = "";
            sqlQuery = "select Reservation_Info_ID, Start_Time, End_Time, R.Experiment_Info_ID, Credential_Set_ID from Reservation_Info AS R Join Experiment_Info AS E on (R.Experiment_Info_ID = E.Experiment_Info_ID) where E.Lab_server_GUID = " + "'" + labServerGuid + "'";
			if (experimentInfoID!=-1)
			{
				sqlQuery += " and R.Experiment_Info_ID = " + experimentInfoID;
			}
			if (credentialSetID !=-1)
			{
					sqlQuery +=" and R.Credential_Set_ID =" + credentialSetID;
			}

			if (timeBefore.CompareTo(DateTime.MinValue)!=0)
			{
				
				sqlQuery +=" and R.Start_Time <= '"+timeBefore+"'";;
			}

			if (timeAfter.CompareTo(DateTime.MinValue)!=0)
			{
				
				sqlQuery +=" and R.Start_Time >= '"+timeAfter+"'";
			}

			sqlQuery += "ORDER BY R.Start_Time asc";

			SqlConnection myConnection = CreateConnection();
			SqlCommand myCommand = new SqlCommand ();
			myCommand.Connection = myConnection;
			myCommand.CommandType = CommandType.Text;
			myCommand.CommandText = sqlQuery;

			try 
			{
				myConnection.Open ();
				
				// get ReservationInfo info from table reservation_Info
				SqlDataReader myReader = myCommand.ExecuteReader ();
				while(myReader.Read ())
				{	
					ReservationInfo ri = new ReservationInfo();
					ri.reservationInfoId = Convert.ToInt32( myReader["Reservation_Info_ID"]); //casting to (long) didn't work
					if(myReader["Start_Time"] != System.DBNull.Value )
						ri.startTime = (DateTime) myReader["Start_Time"];
					if(myReader["End_Time"] != System.DBNull.Value )
						ri.endTime= (DateTime) myReader["End_Time"];
					if(myReader["Experiment_Info_ID"]!=System.DBNull.Value)
						ri.experimentInfoId=Convert.ToInt32(myReader["Experiment_Info_ID"]);
					if(myReader["Credential_Set_ID"] != System.DBNull.Value )
						ri.credentialSetId = Convert.ToInt32(myReader["Credential_Set_ID"]);
							
					reInfos.Add(ri);

				}
				myReader.Close ();
				
				reservationInfos = new ReservationInfo[reInfos.Count];
				for (int i=0;i <reInfos.Count ; i++) 
				{
					reservationInfos[i] = (ReservationInfo)reInfos[i];
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in selecting reservation information",ex);
			}
			finally
			{
				myConnection.Close();
			}

			return reservationInfos;
		}

		/// <summary>
		/// returns an array of the immutable ReservationInfo objects that correspond to the supplied reservationInfoIDs
		/// </summary>
		/// <param name="reservationInfoIDs"></param>
		/// <returns></returns>an array ofimmutable objects describing the specified reservations
 
		public static ReservationInfo[] GetReservationInfos(int[] reservationInfoIDs)
		{
			ReservationInfo[] reservationInfos=new  ReservationInfo[reservationInfoIDs.Length];
            if (reservationInfoIDs.Length > 0)
            {
                for (int i = 0; i < reservationInfoIDs.Length; i++)
                {
                    reservationInfos[i] = new ReservationInfo();
                }
                // create sql connection
                SqlConnection connection = CreateConnection();

                // create sql command
                // command executes the "RetrieveReservationInfoByID" stored procedure
                SqlCommand cmd = new SqlCommand("RetrieveReservationInfoByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@reservationInfoID", SqlDbType.Int));
                //execute the command
                try
                {
                    connection.Open();
                    for (int i = 0; i < reservationInfoIDs.Length; i++)
                    {
                        // populate the parameters
                        cmd.Parameters["@reservationInfoID"].Value = reservationInfoIDs[i];
                        SqlDataReader dataReader = null;
                        dataReader = cmd.ExecuteReader();
                        while (dataReader.Read())
                        {
                            reservationInfos[i].reservationInfoId = reservationInfoIDs[i];
                            if (dataReader[0] != System.DBNull.Value)
                            {
                                DateTime test = dataReader.GetDateTime(0);
                                reservationInfos[i].startTime = DateUtil.SpecifyUTC(dataReader.GetDateTime(0));
                            }
                            if (dataReader[1] != System.DBNull.Value)
                                reservationInfos[i].endTime = DateUtil.SpecifyUTC(dataReader.GetDateTime(1));
                            if (dataReader[2] != System.DBNull.Value)
                                reservationInfos[i].experimentInfoId = (int)dataReader.GetInt32(2);
                            if (dataReader[3] != System.DBNull.Value)
                                reservationInfos[i].credentialSetId = (int)dataReader.GetInt32(3);

                        }
                        dataReader.Close();
                    }
                }

                catch (Exception ex)
                {
                    throw new Exception("Exception thrown in get reservationInfo", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
			return reservationInfos;
		}
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR CredentialSet
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add a credential set of a particular group
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <returns></returns>the unique ID which identifies the credential set added.>0 was successfully added,-1 otherwise
        public static int AddCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName, string ussGuid)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddCredentialSet" store procedure
			SqlCommand cmd=new SqlCommand("AddCredentialSet",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter serviceBrokerNameParam = cmd.Parameters.Add("@serviceBrokerName", SqlDbType.VarChar,256);
			serviceBrokerNameParam.Value = serviceBrokerName;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
            ussIDParam.Value = ussGuid;
			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob!=null && ob!=System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add credential set",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		/// <summary>
		/// Updates the data fields for the credential set specified by the credentialSetID; note credentialSetID may not be changed 
		/// </summary>
		/// <param name="credentialSetID"></param>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="serviceBrokerName"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <returns></returns>if modified successfully, false otherwise
        public static bool ModifyCredentialSet(int credentialSetID, string serviceBrokerGuid, string serviceBrokerName, string groupName, string ussGuid)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyCredentialSet" store procedure
			SqlCommand cmd=new SqlCommand("ModifyCredentialSet",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter credentialSetIDParam = cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
			credentialSetIDParam.Value = credentialSetID;
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter serviceBrokerNameParam = cmd.Parameters.Add("@serviceBrokerName", SqlDbType.VarChar,256);
			serviceBrokerNameParam.Value = serviceBrokerName;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
            ussIDParam.Value = ussGuid;
			bool i=false;

			// execute the command
			try
			{
				int m=0;
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					m = Int32.Parse(ob.ToString());
				}
				if (m!=0)
				{
					i=true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in  ModifyCredentialSet ",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;      
		}
		/// <summary>
		///  remove a credential set specified by the credentialsetsIDS
		/// </summary>
		/// <param name="credentialSetIDs"></param>
		/// <returns></returns>An array of ints containing the IDs of all credential sets not successfully removed, i.e., those for which the operation failed. 
		public static int[] RemoveCredentialSets(int[] credentialSetIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteCredentialSet" store procedure
			SqlCommand cmd=new SqlCommand("DeleteCredentialSet",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@credentialSetID", SqlDbType.Int));
			
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int credentialSetID in credentialSetIDs)
				{
					cmd.Parameters["@credentialSetID"].Value = credentialSetID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(credentialSetID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove credential set",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
        /// <summary>
        /// remove a credential set of a particular group
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <returns></returns>true, the credentialset is removed successfully, false otherwise
        public static bool RemoveCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName,string ussGuid)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "RemoveCredentialSet" store procedure
            SqlCommand cmd = new SqlCommand("RemoveCredentialSet", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar, 50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
            SqlParameter serviceBrokerNameParam = cmd.Parameters.Add("@serviceBrokerName", SqlDbType.VarChar, 256);
            serviceBrokerNameParam.Value = serviceBrokerName;
            SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar, 50);
            groupNameParam.Value = groupName;
            SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar, 50);
            ussIDParam.Value = ussGuid;
            bool removed = false;

            // execute the command
            try
            {
                connection.Open();
                if(cmd.ExecuteNonQuery()>0)
                removed = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in remove credential set", ex);
            }
            finally
            {
                connection.Close();
            }
            return removed;
        }
		/// <summary>
		/// Enumerates the IDs of the information of all the credential set 
		/// </summary>
		/// <returns></returns>the array  of ints contains the IDs of all the credential set
		public static int[] ListCredentialSetIDs()
		{
			int[] credentialSetIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveCredentialSetIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveCredentialSetIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the credentialSetIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Credential_Set_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Credential_Set_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve credentialSetIDs ",ex);
			}
			finally
			{
				connection.Close();
			}
			credentialSetIDs=Utilities.ArrayListToIntArray(arrayList);
			return credentialSetIDs;
		}
/// <summary>
/// Returns an array of the immutable Credential objects that correspond to the supplied credentialSet IDs. 
/// </summary>
/// <param name="credentialSetIDs"></param>
/// <returns></returns>An array of immutable objects describing the specified Credential Set information; if the nth credentialSetID does not correspond to a valid experiment scheduling property, the nth entry in the return array will be null.
		public static LssCredentialSet[] GetCredentialSets(int[] credentialSetIDs)
		{
			LssCredentialSet[] credentialSets=new LssCredentialSet[credentialSetIDs.Length];
			for(int i=0; i<credentialSetIDs.Length;i++)
			{
				credentialSets[i]=new LssCredentialSet();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveCredentialSetByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveCredentialSetByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add("@credentialSetID", SqlDbType.Int);
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<credentialSetIDs.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@credentialSetID"].Value = credentialSetIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						credentialSets[i].credentialSetId=credentialSetIDs[i];
						if(dataReader[0] != System.DBNull.Value )
							credentialSets[i].serviceBrokerGuid=dataReader.GetString(0);
						if(dataReader[1] != System.DBNull.Value )
							credentialSets[i].serviceBrokerName=dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							credentialSets[i].groupName=dataReader.GetString(2);
						if(dataReader[3] != System.DBNull.Value )
							credentialSets[i].ussGuid=dataReader.GetString(3);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get credentialSets",ex);
			}
			finally
			{
				connection.Close();
			}	
			return credentialSets;
		}
        /// <summary>
        /// Get a credential set ID of a particular group
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <returns></returns>the unique ID which identifies the credential set, 0 otherwise
        public static int GetCredentialSetID(string serviceBrokerGuid,  string groupName, string ussGuid)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "AddCredentialSet" store procedure
            SqlCommand cmd = new SqlCommand("GetCredentialSetID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar, 50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
            SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar, 50);
            groupNameParam.Value = groupName;
            SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar, 50);
            ussIDParam.Value = ussGuid;
            int i = 0;

            // execute the command
            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    i = Convert.ToInt32(ob);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in GetCredentialSetID", ex);
            }
            finally
            {
                connection.Close();
            }
            return i;
        }

		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR USSInfo
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// Add information of a particular user side scheduling server identified by ussID 
		/// </summary>
        /// <param name="ussGuid"></param>
		/// <param name="ussName"></param>
		/// <param name="ussURL"></param>
		/// <returns></returns>The unique ID which identifies the experiment information added. >0 was successfully added; ==-1 otherwise   
        public static int AddUSSInfo(string ussGuid, string ussName, string ussURL,long couponId,string domainGuid)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddUSSInfo" store procedure
			SqlCommand cmd=new SqlCommand("AddUSSInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
            ussIDParam.Value = ussGuid;
			SqlParameter ussNameParam = cmd.Parameters.Add("@ussName", SqlDbType.VarChar,256);
			ussNameParam.Value = ussName;
			SqlParameter ussURLParam = cmd.Parameters.Add("@ussURL", SqlDbType.VarChar,256);
			ussURLParam.Value = ussURL;
            SqlParameter couponParam = cmd.Parameters.Add("@couponId", SqlDbType.BigInt);
            couponParam.Value = couponId;
            SqlParameter domainParam = cmd.Parameters.Add("@domainGuid", SqlDbType.VarChar, 50);
            domainParam.Value = domainGuid;
			int i=-1;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					i= Int32.Parse(ob.ToString());
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in add ussInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}

		/// <summary>
		/// Updates the data fields for the USS information specified by the ussInfoID; note ussInfoID may not be changed 
		/// </summary>
		/// <param name="ussInfoID"></param>
        /// <param name="ussGuid"></param>
		/// <param name="ussName"></param>
		/// <param name="ussURL"></param>
		/// <returns></returns>true if modified successfully, false otherwise
        public static bool ModifyUSSInfo(int ussInfoID, string ussGuid, string ussName, string ussURL,long couponId,string domainGuid)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyUSSInfo" store procedure
			SqlCommand cmd=new SqlCommand("ModifyUSSInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter ussInfoIDParam = cmd.Parameters.Add("@ussInfoID", SqlDbType.Int);
			ussInfoIDParam.Value = ussInfoID;
			SqlParameter ussIDParam = cmd.Parameters.Add("@ussGUID", SqlDbType.VarChar,50);
            ussIDParam.Value = ussGuid;
			SqlParameter ussNameParam = cmd.Parameters.Add("@ussName", SqlDbType.VarChar,256);
			ussNameParam.Value = ussName;
			SqlParameter ussURLParam = cmd.Parameters.Add("@ussURL", SqlDbType.VarChar,256);
			ussURLParam.Value = ussURL;
            SqlParameter couponParam = cmd.Parameters.Add("@couponId", SqlDbType.BigInt);
            couponParam.Value = couponId;
            SqlParameter domainParam = cmd.Parameters.Add("@domainGuid", SqlDbType.VarChar, 50);
            domainParam.Value = domainGuid;
			bool i=false;

			// execute the command
			try
			{
				int m=0;
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					m = Int32.Parse(ob.ToString());
				}
				if (m!=0)
				{
					i=true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in ModifyUSSInfo",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;      
		}
		
		/// <summary>
		/// Delete the uss information specified by the ussInfoIDs. 
		/// </summary>
		/// <param name="ussInfoIDs"></param>
		/// <returns></returns>An array of USS information IDs specifying the USS information to be removed
		public static int[] RemoveUSSInfo(int[] ussInfoIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteUSSInfo" store procedure
			SqlCommand cmd=new SqlCommand("DeleteUSSInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@ussInfoID", SqlDbType.Int));
			
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int ussInfoID in ussInfoIDs)
				{
					cmd.Parameters["@ussInfoID"].Value = ussInfoID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(ussInfoID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove ussInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}

		/// <summary>
		/// Enumerates the IDs of the information of all the USS 
		/// </summary>
		/// <returns></returns>the array  of ints contains the IDs of the information of all the USS
		public static int[] ListUSSInfoIDs()
		{
			int[] ussInfoIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveUSSInfoIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveUSSInfoIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the ussInfoIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["USS_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["USS_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve ussInfoIDs ",ex);
			}
			finally
			{
				connection.Close();
			}
			ussInfoIDs=Utilities.ArrayListToIntArray(arrayList);
			return ussInfoIDs;
		}
		/// <summary>
		/// Enumerates the ID of the information of a particular USS specified by ussID
		/// </summary>
        /// <param name="ussGuid"></param>
		/// <returns></returns>the ID of the information of a particular USS 
        public static int ListUSSInfoID(string ussGuid)
		{
			int ussInfoID=-1;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveUSSInfoID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveUSSInfoID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter ussGuidParam = cmd.Parameters.Add("@ussGuid", SqlDbType.VarChar, 50);
            ussGuidParam.Value = ussGuid;

			// execute the command
			
			try 
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				if(ob != null && ob!=System.DBNull.Value)
				{
					ussInfoID=Int32.Parse(ob.ToString());
				}
				
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve ussInfoID",ex);
			}
			finally
			{
				connection.Close();
			}
			return ussInfoID;
		}
		
      /// <summary>
      /// Returns an array of the immutable USSInfo objects that correspond to the supplied USS information IDs. 
      /// </summary>
      /// <param name="ussInfoIDs"></param>
      /// <returns></returns>An array of immutable objects describing the specified USS information; if the nth ussInfoID does not correspond to a valid experiment scheduling property, the nth entry in the return array will be null
		public static USSInfo[] GetUSSInfos(int[] ussInfoIDs)
		{
			USSInfo[] ussInfos=new USSInfo[ussInfoIDs.Length];
			for(int i=0; i<ussInfoIDs.Length;i++)
			{
				ussInfos[i]=new USSInfo();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveUSSInfoByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveUSSInfoByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add("@ussInfoID", SqlDbType.Int);
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<ussInfoIDs.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@ussInfoID"].Value = ussInfoIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						ussInfos[i].ussInfoId=ussInfoIDs[i];
						if(dataReader[0] != System.DBNull.Value )
							ussInfos[i].ussGuid=dataReader.GetString(0);
						if(dataReader[1] != System.DBNull.Value )
							ussInfos[i].ussName=dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							ussInfos[i].ussUrl=dataReader.GetString(2);
                        if (dataReader[3] != System.DBNull.Value)
                            ussInfos[i].couponId = dataReader.GetInt64(3);
                        if (dataReader[4] != System.DBNull.Value)
                            ussInfos[i].domainGuid = dataReader.GetString(4);
                        
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get ussInfos",ex);
			}
			finally
			{
				connection.Close();
			}	
			return ussInfos;
		}

        /* !------------------------------------------------------------------------------!
         *							CALLS FOR Recurrence
         * !------------------------------------------------------------------------------!
         */
      /// <summary>
      /// add recurrence
      /// </summary>
      /// <param name="recurrenceStartDate"></param>
      /// <param name="recurrenceEndDate"></param>
      /// <param name="recurrenceType"></param>
      /// <param name="recurrenceStartTime"></param>
      /// <param name="recurrenceEndTime"></param>
      /// <param name="labServerGuid"></param>
      /// <param name="credentialSetID"></param>
        /// <returns></returns>the uniqueID which identifies the recurrence added, >0 was successfully added; ==-1 otherwise
        public static int AddRecurrence(DateTime recurrenceStartDate, DateTime recurrenceEndDate,
            string recurrenceType, TimeSpan recurrenceStartTime, TimeSpan recurrenceEndTime,
            string labServerGuid, int resourceID, byte dayMask)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "AddRecurrence" store procedure
            SqlCommand cmd = new SqlCommand("AddRecurrence", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar, 50);
            labServerIDParam.Value = labServerGuid;
            SqlParameter resourceIDParam = cmd.Parameters.Add("@resourceID", SqlDbType.Int);
            resourceIDParam.Value = resourceID;
            SqlParameter recurrenceStartTimeParam = cmd.Parameters.Add("@recurrenceStartTime", SqlDbType.Int);
            recurrenceStartTimeParam.Value = recurrenceStartTime.TotalSeconds;
            SqlParameter recurrenceeEndTimeParam = cmd.Parameters.Add("@recurrenceEndTime", SqlDbType.Int);
            recurrenceeEndTimeParam.Value = recurrenceEndTime.TotalSeconds;
            SqlParameter recurrenceStartDateParam = cmd.Parameters.Add("@recurrenceStartDate", SqlDbType.DateTime);
            recurrenceStartDateParam.Value = recurrenceStartDate;
            SqlParameter recurrenceeEndDateParam = cmd.Parameters.Add("@recurrenceEndDate", SqlDbType.DateTime);
            recurrenceeEndDateParam.Value = recurrenceEndDate;
            SqlParameter recurrenceTypeParam = cmd.Parameters.Add("@recurrenceType", SqlDbType.VarChar, 50);
            recurrenceTypeParam.Value = recurrenceType;
            SqlParameter daysParam = cmd.Parameters.Add("@dayMask", SqlDbType.TinyInt);
            daysParam.Value = dayMask;
            int i = -1;

            // execute the command
            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    i = Int32.Parse(ob.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add recurrence", ex);
            }
            finally
            {
                connection.Close();
            }
            return i;
        }

        /// <summary>
        /// delete the recurrence specified by the recurrenceIDs
        /// </summary>
        /// <param name="recurrenceIDs"></param>
        /// <returns></returns>an array of ints containning the IDs of all recurrence not successfully removed
        public static int[] RemoveRecurrence(int[] recurrenceIDs)
        {
            ArrayList arrayList = new ArrayList();
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "DeleteRecurrence" store procedure
            SqlCommand cmd = new SqlCommand("DeleteRecurrence", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@recurrenceID", SqlDbType.Int));

            // execute the command
            try
            {
                connection.Open();
                //populate the parameters
                foreach (int recurrenceID in recurrenceIDs)
                {
                    cmd.Parameters["@recurrenceID"].Value = recurrenceID;
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        arrayList.Add(recurrenceID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in remove recurrence", ex);
            }
            finally
            {
                connection.Close();
            }
            int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
            return uIDs;
        }
        /// <summary>
        /// Returns an array of the immutable Recurrence objects that correspond ot the supplied Recurrence IDs
        /// </summary>
        /// <param name="timeBlockIDs"></param>
        /// <returns></returns>an array of immutable objects describing the specified Recurrence
        public static Recurrence[] GetRecurrence(int[] recurrenceIDs)
        {
            Recurrence[] recurrences = new Recurrence[recurrenceIDs.Length];
            for (int i = 0; i < recurrenceIDs.Length; i++)
            {
                recurrences[i] = new Recurrence();
            }
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveRecurrenceByID" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveRecurrenceByID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@recurrenceID", SqlDbType.Int);
            //execute the command
            try
            {
                connection.Open();
                for (int i = 0; i < recurrenceIDs.Length; i++)
                {
                    // populate the parameters
                    cmd.Parameters["@recurrenceID"].Value = recurrenceIDs[i];
                    SqlDataReader dataReader = null;
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        recurrences[i].recurrenceId = recurrenceIDs[i];
                        if (dataReader[0] != System.DBNull.Value)
                            recurrences[i].recurrenceStartDate = DateUtil.SpecifyUTC(dataReader.GetDateTime(0));
                        if (dataReader[1] != System.DBNull.Value)
                            recurrences[i].recurrenceEndDate = DateUtil.SpecifyUTC(dataReader.GetDateTime(1));
                        if (dataReader[2] != System.DBNull.Value)
                            recurrences[i].recurrenceType = dataReader.GetString(2);
                        if (dataReader[3] != System.DBNull.Value)
                        {
                            recurrences[i].recurrenceStartTime = TimeSpan.FromSeconds((double)dataReader.GetInt32(3));
                        }
                        if (dataReader[4] != System.DBNull.Value)
                            recurrences[i].recurrenceEndTime = TimeSpan.FromSeconds((double)dataReader.GetInt32(4));
                        if (dataReader[5] != System.DBNull.Value)
                            recurrences[i].labServerGuid= dataReader.GetString(5);
                        if (dataReader[6] != System.DBNull.Value)
                            recurrences[i].resourceId = (int)dataReader.GetInt32(6);
                        if (dataReader[7] != System.DBNull.Value)
                            recurrences[i].dayMask = dataReader.GetByte(7);
                    }
                    dataReader.Close();
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in get recurrence", ex);
            }
            finally
            {
                connection.Close();
            }
            return recurrences;
        }
        /// Enumerates the IDs of the information of all the Recurrence 
        /// </summary>
        /// <returns></returns>the array  of ints contains the IDs of the information of all the Recurrence
        public static int[] ListRecurrenceIDs()
        {
            int[] recurrenceIDs;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveRecurrenceIDs" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveRecurrenceIDs", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the timeBlockIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Recurrence_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Recurrence_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve recurrenceIDs ", ex);
            }
            finally
            {
                connection.Close();
            }
            recurrenceIDs = Utilities.ArrayListToIntArray(arrayList);
            return recurrenceIDs;
        }
        /// <summary>
        /// enumerates all IDs of the recurrences belonging to a particular lab server identified by the labserverID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
        public static int[] ListRecurrenceIDsByLabServer(string labServerGuid)
        {
            int[] recurIDs;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveRecurrenceIDsByLabServer" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveRecurrenceIDsByLabServer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar, 50);
            labServerIDParam.Value = labServerGuid;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the timeBlockIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Recurrence_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Recurrence_ID"]));
                }
                dataReader.Close();
            }


            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve recurrenceIDs by lab server", ex);
            }
            finally
            {
                connection.Close();
            }
            recurIDs = Utilities.ArrayListToIntArray(arrayList);
            return recurIDs;
        }

        /// <summary>
        /// enumerates all IDs of the recurrences belonging to a particular lab server identified by the labserverID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
        public static int[] ListRecurrenceIDsByLabServer(DateTime start, DateTime end, string labServerGuid)
        {
            int[] recurIDs;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveRecurrenceIDsByLabServer" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveRecurrenceIDsByLabServerAndTime", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar, 50);
            labServerIDParam.Value = labServerGuid;
            SqlParameter startParam = cmd.Parameters.Add("@start", SqlDbType.DateTime);
            startParam.Value = start;
            SqlParameter endParam = cmd.Parameters.Add("@end", SqlDbType.DateTime);
            endParam.Value = end;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the timeBlockIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Recurrence_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Recurrence_ID"]));
                }
                dataReader.Close();
            }


            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve recurrenceIDs by lab server", ex);
            }
            finally
            {
                connection.Close();
            }
            recurIDs = Utilities.ArrayListToIntArray(arrayList);
            return recurIDs;
        }

        /// <summary>
        /// enumerates all IDs of the recurrences belonging to a particular lab server identified by the labserverID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
        public static int[] ListRecurrenceIDsByResourceID(DateTime start, DateTime end, int resourceID)
        {
            int[] recurIDs;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveRecurrenceIDsByLabServer" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveRecurrenceIDsByResourceAndTime", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter IDParam = cmd.Parameters.Add("@resourceID", SqlDbType.Int);
            IDParam.Value = resourceID;
            SqlParameter startParam = cmd.Parameters.Add("@start", SqlDbType.DateTime);
            startParam.Value = start;
            SqlParameter endParam = cmd.Parameters.Add("@end", SqlDbType.DateTime);
            endParam.Value = end;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the timeBlockIDs retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["Recurrence_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["Recurrence_ID"]));
                }
                dataReader.Close();
            }


            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve recurrenceIDs by lab server", ex);
            }
            finally
            {
                connection.Close();
            }
            recurIDs = Utilities.ArrayListToIntArray(arrayList);
            return recurIDs;
        }
	
	}
}

