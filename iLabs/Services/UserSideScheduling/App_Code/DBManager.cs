using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.UtilLib;

namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Summary description for DBManager.
	/// </summary>
	public class DBManager
	{
		 static string connectionStr;

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
         *							CALLS FOR USSPolicy
         * !------------------------------------------------------------------------------!
         */
        /// <summary>
        /// add user side scheduling policy that governs whether a reservation request to execute an experiment at a certain time will be accept from a student with a particular credential set
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="experimentInfoId"></param>
        /// <param name="rule"></param>
        /// <returns></returns>the index ID of the USSPolicy added,>0 successfully added, ==-1 otherwise
        public static int AddUSSPolicy(string groupName, string serviceBrokerGuid, int experimentInfoId, string rule)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "addUSSPolicy" store procedure
            SqlCommand cmd = new SqlCommand("AddUSSPolicy", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar, 50);
            groupNameParam.Value = groupName;
            SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar, 50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
            SqlParameter experimentInfoIdParam = cmd.Parameters.Add("@experimentInfoId", SqlDbType.Int);
            experimentInfoIdParam.Value = experimentInfoId;
            SqlParameter ruleParam = cmd.Parameters.Add("@rule", SqlDbType.VarChar, 1024);
            ruleParam.Value = rule;
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
                throw new Exception("Exception thrown in add USSPolicy", ex);
            }
            finally
            {
                connection.Close();
            }
            return i;

        }
        /// <summary>
        /// delete the user side scheduling policies specified by the ussPolicyIds
        /// </summary>
        /// <param name="ussPolicyIds"></param>
        /// <returns></returns>the IDs of all UssPolicies not successfully removed

        public static int[] RemoveUSSPolicy(int[] ussPolicyIds)
        {
            ArrayList arrayList = new ArrayList();
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "deleteUSSPolicy" store procedure
            SqlCommand cmd = new SqlCommand("DeleteUSSPolicy", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ussPolicyId", SqlDbType.Int));
            // execute the command
            try
            {
                connection.Open();
                //populate the parameters
                foreach (int ussPolicyId in ussPolicyIds)
                {
                    cmd.Parameters["@ussPolicyId"].Value = ussPolicyId;
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        arrayList.Add(ussPolicyId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in remove usspolicies", ex);
            }
            finally
            {
                connection.Close();
            }
            int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
            return uIDs;
        }
        /// <summary>
        /// update the data fields for the user side scheduling policy specified by the ussPolicyId
        /// </summary>
        /// <param name="ussPolicyId"></param>
        /// <param name="experimentInfoId"></param>
        /// <param name="rule"></param>
        /// <returns></returns>if updated succesfully, return ture, otherwise, return false

        public static bool ModifyUSSPolicy(int ussPolicyId, int experimentInfoId, string rule, int credentialSetId)
        {
            //create a connection
            SqlConnection connection = CreateConnection();
            //create a command
            //command executes the "modifyUSSPolicy" store procedure
            SqlCommand cmd = new SqlCommand("ModifyUSSPolicy", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            //populate the parameters
            SqlParameter ussPolicyIdParam = cmd.Parameters.Add("@ussPolicyId", SqlDbType.Int);
            ussPolicyIdParam.Value = ussPolicyId;
            SqlParameter experimentInfoIdParam = cmd.Parameters.Add("@experimentInfoID", SqlDbType.Int);
            experimentInfoIdParam.Value = experimentInfoId;
            SqlParameter ruleParam = cmd.Parameters.Add("@rule", SqlDbType.VarChar, 1024);
            ruleParam.Value = rule;
            SqlParameter credentialSetIdParam = cmd.Parameters.Add("@credentialSetId", SqlDbType.Int);
            credentialSetIdParam.Value = credentialSetId;
            bool i = false;

            // execute the command
            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                int m = 0;
                if (ob != null && ob != System.DBNull.Value)
                {
                    m = Int32.Parse(ob.ToString());
                }
                if (m != 0)
                {
                    i = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception thrown in add ModifyUSSPolicy", ex);
            }
            finally
            {
                connection.Close();
            }
            return i;

        }

        /// <summary>
        /// enumerates all IDs of the User side Scheduling Policies 
        /// </summary>
        /// <returns></returns>an array of ints containing the IDs of all the USSPolicies
        public static int[] ListUSSPolicyIDs()
        {
            int[] ussPolicyIds;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveUSSPolicyIDsByGroup" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveUSSPolicyIDs", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the ussPolicyIds retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["USS_Policy_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["USS_Policy_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve USSPolicyIDs", ex);
            }
            finally
            {
                connection.Close();
            }
            ussPolicyIds = Utilities.ArrayListToIntArray(arrayList);
            return ussPolicyIds;
        }
        /// <summary>
        /// enumerates all IDs of the User side Scheduling Policies applying for the users with a particular credential set identified by the combination of groupName and serviceBrokerID
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceBrokerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the USSPolicies applying for the users with a particular credential set
        public static int[] ListUSSPolicyIDsByGroup(string groupName, string serviceBrokerGuid)
        {
            int[] ussPolicyIds;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveUSSPolicyIDsByGroup" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveUSSPolicyIDsByGroup", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar, 50);
            groupNameParam.Value = groupName;
            SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar, 50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the ussPolicyIds retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["USS_Policy_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["USS_Policy_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve USSPolicyIDs by group", ex);
            }
            finally
            {
                connection.Close();
            }
            ussPolicyIds = Utilities.ArrayListToIntArray(arrayList);
            return ussPolicyIds;
        }

        /// <summary>
        /// enumerates the ID of the User side Scheduling Policies applying for the users with a particular credential set identified by the combination of groupName and serviceBrokerID to execute a particular experiment
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="serviceBrokerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the USSPolicies applying for the users with a particular credential set to execute a particular experiment
        public static int[] ListUSSPolicyIDsByGroupAndExperiment(string groupName, string serviceBrokerGuid, string clientGuid, string labServerGuid)
        {
            int[] ussPolicyIds;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveUSSPolicyIDsByGroupandExp" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveUSSPolicyIDsByGroupandExp", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar, 50);
            groupNameParam.Value = groupName;
            SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar, 50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
            SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar, 50);
            labClientNameParam.Value = clientGuid;
            SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar, 50);
            labClientVersionParam.Value = labServerGuid;

            // execute the command
            SqlDataReader dataReader = null;
            ArrayList arrayList = new ArrayList();
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                //store the ussPolicyIds retrieved in arraylist

                while (dataReader.Read())
                {
                    if (dataReader["USS_Policy_ID"] != System.DBNull.Value)
                        arrayList.Add(Convert.ToInt32(dataReader["USS_Policy_ID"]));
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve USSPolicyIDs by group and experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            ussPolicyIds = Utilities.ArrayListToIntArray(arrayList);
            return ussPolicyIds;
        }
        /// <summary>
        /// returns an array of the immutable USSPolicy objects that correspond to the supplied ussPolicyIds
        /// </summary>
        /// <param name="ussPolicyIds"></param>
        /// <returns></returns>
        public static USSPolicy[] GetUSSPolicies(int[] ussPolicyIds)
        {
            USSPolicy[] ussPolicies = new USSPolicy[ussPolicyIds.Length];
            for (int i = 0; i < ussPolicyIds.Length; i++)
            {
                ussPolicies[i] = new USSPolicy();
            }
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveUSSPolicyByID" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveUSSPolicyByID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ussPolicyId", SqlDbType.Int));
            //execute the command
            try
            {
                connection.Open();
                for (int i = 0; i < ussPolicyIds.Length; i++)
                {
                    // populate the parameters
                    cmd.Parameters["@ussPolicyId"].Value = ussPolicyIds[i];
                    SqlDataReader dataReader = null;
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ussPolicies[i].ussPolicyId = ussPolicyIds[i];
                        if (dataReader[0] != System.DBNull.Value)
                            ussPolicies[i].experimentInfoId = (int)dataReader.GetInt32(0);
                        if (dataReader[1] != System.DBNull.Value)
                            ussPolicies[i].rule = dataReader.GetString(1);
                        if (dataReader[2] != System.DBNull.Value)
                            ussPolicies[i].credentialSetId = (int)dataReader.GetInt32(2);
                    }
                    dataReader.Close();
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in get usspolicies", ex);
            }
            finally
            {
                connection.Close();
            }
            return ussPolicies;
        }
	    	/* !------------------------------------------------------------------------------!
			 *							CALLS FOR ReservationInfo
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add reservation by user
		/// </summary>
		/// <param name="userName"></param>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <param name="experimentInfoId"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>the unique id which identifies the reservation added by the user,>0 successfully added, ==-1 otherwise
        public static int AddReservation(string userName, string serviceBrokerGuid, string groupName, int experimentInfoId, DateTime startTime, DateTime endTime)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddReservation" store procedure
			SqlCommand cmd=new SqlCommand("AddReservation",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter userNameParam = cmd.Parameters.Add("@userName", SqlDbType.VarChar,50);
			userNameParam.Value = userName;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter experimentInfoIdParam = cmd.Parameters.Add("@experimentInfoId", SqlDbType.Int);
			experimentInfoIdParam.Value = experimentInfoId;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;
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
				throw new Exception("Exception thrown in add ReservationInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		/// <summary>
		/// delete the reservations specified by the reservationIDs
		/// </summary>
		/// <param name="reservationIDs"></param>
		/// <returns></returns>an array of ints containing the IDs of all reservation not successfully removed
		public static int[] RemoveReservation(int[] reservationIDs)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteReservation" store procedure
			SqlCommand cmd=new SqlCommand("DeleteReservation",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@reservationID", SqlDbType.Int));
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int reservationID in reservationIDs)
				{
					cmd.Parameters["@reservationID"].Value = reservationID;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(reservationID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove reservations",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
/// <summary>
/// updates the data fields for the reservation specified by the reservationID
/// </summary>
/// <param name="reservationID"></param>
/// <param name="experimentInfoId"></param>
/// <param name="startTime"></param>
/// <param name="endTime"></param>
/// <returns></returns>ture if updated successfully, false otherwise
		public static bool ModifyReservation(int reservationID,int experimentInfoId,DateTime startTime,DateTime endTime)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyReservation" store procedure
			SqlCommand cmd=new SqlCommand("ModifyReservation",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter reservationIDParam = cmd.Parameters.Add("@reservationID", SqlDbType.Int);
			reservationIDParam.Value = reservationID;
			SqlParameter experimentInfoIdParam = cmd.Parameters.Add("@experimentInfoId", SqlDbType.Int);
			experimentInfoIdParam.Value = experimentInfoId;
			SqlParameter startTimeParam = cmd.Parameters.Add("@startTime", SqlDbType.DateTime);
			startTimeParam.Value = startTime;
			SqlParameter endTimeParam = cmd.Parameters.Add("@endTime", SqlDbType.DateTime);
			endTimeParam.Value = endTime;
			bool i=false;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				int m=0;
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
				throw new Exception("Exception thrown in add ModifyReservation",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
		/// <summary>
		/// enumerates all IDs of the reservations made on a particular experiment by a particular user identified by the combination of userName and serviceBrokerID
		/// </summary>
		/// <param name="userName"></param>
        /// <param name="serviceBrokerGuid"></param>
		/// <returns></returns>an array of ints containing the IDs of all the reservation made by the specified user

        public static int[] ListReservationIDsByUser(string userName, string serviceBrokerGuid, int experimentInfoId)
		{
			int[] reservationIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReservationIDsByUser" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReservationIDsByUser", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter userNameParam = cmd.Parameters.Add("@userName", SqlDbType.VarChar,50);
			userNameParam.Value = userName;
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter experimentInfoIdParam = cmd.Parameters.Add("@experimentInfoId", SqlDbType.Int);
			experimentInfoIdParam.Value = experimentInfoId;
			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the reservationIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Reservation_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Reservation_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve reservationIDs by user",ex);
			}
			finally
			{
				connection.Close();
			}
			reservationIDs=Utilities.ArrayListToIntArray(arrayList);
			return reservationIDs;
		}
		/// <summary>
		/// enumerates all IDs of the reservations with a credential sets identified by the combination of groupName and serviceBrokerID
		/// </summary>
		/// <param name="groupName"></param>
        /// <param name="serviceBrokerGuid"></param>
		/// <returns></returns>an array of ints containing the IDs of all the reservations for the specified lab server made by the specified user
        public static int[] ListReservationIDsByGroup(string groupName, string serviceBrokerGuid)
		{
			int[] reservationIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReservationIDsByGroup" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReservationIDsByGroup", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the reservationIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Reservation_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Reservation_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve reservationIDs by group",ex);
			}
			finally
			{
				connection.Close();
			}
			reservationIDs=Utilities.ArrayListToIntArray(arrayList);
			return reservationIDs;
		}

		/// <summary>
		/// enumerates all IDs of the reservations made on the particular lab server during a paticular time period
		/// </summary>
        /// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>/* An array of ints containing the IDs of the reservations made on the particular lab server during a particular time period 
        public static int[] ListReservationIDsByLabServer(string labServerGuid, DateTime startTime, DateTime endTime)
		{
			int[] reservationIDs;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReservationIDsByGroup" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReservationIDsByUser", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labServerIDParam = cmd.Parameters.Add("@labServerGUID", SqlDbType.VarChar,50);
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
				//store the reservationIDs retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Reservation_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Reservation_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve reservationIDs by group",ex);
			}
			finally
			{
				connection.Close();
			}
			reservationIDs=Utilities.ArrayListToIntArray(arrayList);
			return reservationIDs;
			
		}
        /// <summary>
        /// retrieve the ID of the reservation on a particular experiment for a particular user at a specific time 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="labClientName"></param>
        /// <param name="labClientVersion"></param>
        /// <param name="time"></param>
        /// <returns></returns>the ID of the reservation if the requested reservation does exist, -1 otherwise
        public static int ListReservationIDByUser(string userName, string serviceBrokerGuid, string clientGuid, string labServerGuid, DateTime time)
		{
			int i= 0;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReservationIDByUser" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReservationIDByUser", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter userNameParam = cmd.Parameters.Add("@userName", SqlDbType.VarChar,50);
			userNameParam.Value = userName;
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,50);
			labClientNameParam.Value = clientGuid;
			SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
			labClientVersionParam.Value = labServerGuid;
			SqlParameter timeParam = cmd.Parameters.Add("@time", SqlDbType.DateTime);
			timeParam.Value = time;


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
				throw new Exception("Exception thrown in retrieve reservationIDs by group",ex);
			}
			finally
			{
				connection.Close();
			}
			
			return i;
			
		}


		/// <summary>
		/// returns an array of the immutable reservation objects that correspond to the supplied reservation IDs
		/// </summary>
		/// <param name="reservationIDs"></param>
		/// <returns></returns>
		public static ReservationInfo[] GetReservations(int[] reservationIDs)
		{
			ReservationInfo[] reservations=new ReservationInfo[reservationIDs.Length];
			for(int i=0; i<reservationIDs.Length;i++)
			{
				reservations[i]=new  ReservationInfo();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveReservationByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveReservationByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@reservationID", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<reservationIDs.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@reservationID"].Value = reservationIDs[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						reservations[i].reservationId=reservationIDs[i];
						if(dataReader[0] != System.DBNull.Value )
							reservations[i].userName=dataReader.GetString(0);
						if(dataReader[1] != System.DBNull.Value )
							reservations[i].startTime= DateUtil.SpecifyUTC(dataReader.GetDateTime(1));
						if(dataReader[2] != System.DBNull.Value )
                            reservations[i].endTime = DateUtil.SpecifyUTC(dataReader.GetDateTime(2));
						if(dataReader[3] != System.DBNull.Value )
							reservations[i].credentialSetId=(int)dataReader.GetInt32(3);
						if(dataReader[4] != System.DBNull.Value )
							reservations[i].experimentInfoId=(int)dataReader.GetInt32(4);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get reservations",ex);
			}
			finally
			{
				connection.Close();
			}	
			return reservations;
		}

		/// <summary>
		/// to select reservation accorrding to given criterion
		/// </summary>
		public static ReservationInfo[] SelectReservation(string userName, int experimentInfoId, int credentialSetId, DateTime timeAfter, DateTime timeBefore)
		{
			ReservationInfo[] reservations = null;
			ArrayList reInfos = new ArrayList();
			string sqlQuery = "";
			sqlQuery = "select Reservation_ID, User_Name, Start_Time, End_Time, Experiment_Info_ID, Credential_Set_ID from Reservations where Credential_Set_ID =" + credentialSetId.ToString() ;
			if(userName != null && userName != "")
			{
				sqlQuery += " and [User_Name] = " +"'"+ userName+"'";

			}
			if (experimentInfoId!=-1)
			{
				
					sqlQuery += " and Experiment_Info_ID = " + experimentInfoId;
				
			}
			

			if (timeBefore.CompareTo(DateTime.MinValue)!=0)
			{
				
				sqlQuery +=" and Start_Time <= '"+timeBefore+"'";;
			}

			if (timeAfter.CompareTo(DateTime.MinValue)!=0)
			{
				
				sqlQuery +=" and Start_Time >= '"+timeAfter+"'";
			}

			sqlQuery += " ORDER BY Start_Time asc";

			SqlConnection myConnection = CreateConnection();
			SqlCommand myCommand = new SqlCommand ();
			myCommand.Connection = myConnection;
			myCommand.CommandType = CommandType.Text;
			myCommand.CommandText = sqlQuery;

			try 
			{
				myConnection.Open ();
				
				// get ReservationInfo info from table reservation
				SqlDataReader myReader = myCommand.ExecuteReader ();
				while(myReader.Read ())
				{	
					ReservationInfo res = new ReservationInfo();
					res.reservationId = Convert.ToInt32( myReader["Reservation_ID"]); //casting to (long) didn't work
					if(myReader["Start_Time"] != System.DBNull.Value )
						res.startTime = DateUtil.SpecifyUTC((DateTime) myReader["Start_Time"]);
					if(myReader["End_Time"] != System.DBNull.Value )
						res.endTime= DateUtil.SpecifyUTC((DateTime) myReader["End_Time"]);
					if(myReader["Experiment_Info_ID"]!=System.DBNull.Value)
						res.experimentInfoId=Convert.ToInt32(myReader["Experiment_Info_ID"]);
					if(myReader["Credential_Set_ID"] != System.DBNull.Value )
						res.credentialSetId = Convert.ToInt32(myReader["Credential_Set_ID"]);
					if(myReader["User_Name"] != System.DBNull.Value )
						res.userName = Convert.ToString(myReader["User_Name"]);
							
					reInfos.Add(res);

				}
				myReader.Close ();
				
				reservations = new ReservationInfo[reInfos.Count];
				for (int i=0;i <reInfos.Count ; i++) 
				{
					reservations[i] = (ReservationInfo)reInfos[i];
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in selecting reservation",ex);
			}
			finally
			{
				myConnection.Close();
			}

			return reservations;
		}

		    /* !------------------------------------------------------------------------------!
			 *							CALLS FOR Experiment Information
			 * !------------------------------------------------------------------------------!
			 * */
		/// <summary>
		/// add information of a particular experiment
		/// </summary>
        /// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="labClientName"></param>
		/// <param name="providerName"></param>
        /// <param name="lssGuid"></param>
		/// <returns></returns>the unique ID which identifies the experiment added,>0 successfully added,==-1 otherwise
        public static int AddExperimentInfo(string labServerGuid, string labServerName,
            string labClientGuid, string labClientName, string labClientVersion, string providerName, string lssGuid)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddExperimentInfo" store procedure
			SqlCommand cmd=new SqlCommand("AddExperimentInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
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
			SqlParameter lssIDParam = cmd.Parameters.Add("@lssGUID", SqlDbType.VarChar,50);
            lssIDParam.Value = lssGuid;
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
				throw new Exception("Exception thrown in add Experiment Information",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
/// <summary>
/// delete the experiment information specified by the experimentInfoIds
/// </summary>
/// <param name="experimentInfoIds"></param>
/// <returns></returns>an array of ints containing the IDs of all experiments not successfully removed
		public static int[] RemoveExperimentInfo(int[] experimentInfoIds)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteExperimentInfo" store procedure
			SqlCommand cmd=new SqlCommand("DeleteExperimentInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@experimentInfoId", SqlDbType.Int));
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int experimentInfoId in experimentInfoIds)
				{
					cmd.Parameters["@experimentInfoId"].Value = experimentInfoId;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(experimentInfoId);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove experiment infomation",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
		/// <summary>
		///Enumerates IDs of all the experimentInfos 
		/// </summary>
		/// <returns></returns> An array of ints containing the IDs of all the experimentInfos 
		public static int[] ListExperimentInfoIDs()
		{
			int[] experimentInfoIds;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the experimentInfoIds retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Experiment_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Experiment_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve experimentInfoIds",ex);
			}
			finally
			{
				connection.Close();
			}
			experimentInfoIds=Utilities.ArrayListToIntArray(arrayList);
			return experimentInfoIds;
		}
		/// <summary>
		/// Returns an array of the immutable ExperimentInfo objects that correspond to the supplied experimentInfo IDs. 
		/// </summary>
		/// <param name="experimentInfoIds"></param>
		/// <returns></returns>
		public static UssExperimentInfo[] GetExperimentInfos(int[] experimentInfoIds)
		{
			UssExperimentInfo[] experimentInfos=new UssExperimentInfo[experimentInfoIds.Length];
			for(int i=0; i<experimentInfoIds.Length;i++)
			{
				experimentInfos[i]=new  UssExperimentInfo();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentInfoByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentInfoByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@experimentInfoId", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<experimentInfoIds.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@experimentInfoId"].Value = experimentInfoIds[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						experimentInfos[i].experimentInfoId=experimentInfoIds[i];
                        if (dataReader[0] != System.DBNull.Value)
                            experimentInfos[i].labClientGuid = dataReader.GetString(0);
						if(dataReader[1] != System.DBNull.Value )
							experimentInfos[i].labServerGuid=dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							experimentInfos[i].labServerName=dataReader.GetString(2);
						if(dataReader[3] != System.DBNull.Value )
							experimentInfos[i].labClientVersion=dataReader.GetString(3);
						if(dataReader[4] != System.DBNull.Value )
							experimentInfos[i].labClientName=dataReader.GetString(4);
						if(dataReader[5] != System.DBNull.Value )
							experimentInfos[i].providerName=dataReader.GetString(5);
						if(dataReader[6] != System.DBNull.Value )
							experimentInfos[i].lssGuid=dataReader.GetString(6);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get experimentInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			return experimentInfos;
		}
		/// <summary>
		/// enumerates the ID of the information of a particular experiment specified by labClientName and labClientVersion
		/// </summary>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <returns></returns>the ID of the information of a particular experiment, -1 if such a experiment info can not be retrieved
		public static int ListExperimentInfoIDByExperiment(string clientGuid,string labServerGuid)
		{
			int experimentInfoId=-1;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveExperimentInfoIDByExperiment" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveExperimentInfoIDByExperiment", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,50);
			labClientNameParam.Value = clientGuid;
			SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
			labClientVersionParam.Value = labServerGuid;

			// execute the command
			
			try 
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					experimentInfoId=Int32.Parse(ob.ToString());
				}
				
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve experimentInfoIds by experiment",ex);
			}
			finally
			{
				connection.Close();
			}
			return experimentInfoId;
		}
		/// <summary>
		/// enumerates the URL of LSS which is in charge of a particular experiment specified by labClientName and labClientVersion
		/// </summary>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <returns></returns>the url of the requested LSS, null if such a experiment info can not be retrieved
		public static string ListLssUrlByExperiment(string clientGuid,string labServerGuid)
		{
			string url = null;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveLSSURLbyExperiment" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveLSSURLbyExperiment", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar,50);
			labClientNameParam.Value = clientGuid;
			SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar,50);
			labClientVersionParam.Value = labServerGuid;

			// execute the command
			
			try 
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
				{
					url = ob.ToString();
				}
				
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve LSS url by experiment",ex);
			}
			finally
			{
				connection.Close();
			}
			return url;
		}
        /// <summary>
        ///  enumerates the GUID of LSS which is in charge of a particular experiment specified by labClientName and labClientVersion
        /// </summary>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>
        public static string ListLssIdByExperiment(string clientGuid, string labServerGuid)
        {
            string lssID = null;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveLSSURLbyExperiment" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveLSSIDbyExperiment", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter labClientNameParam = cmd.Parameters.Add("@clientGuid", SqlDbType.VarChar, 50);
            labClientNameParam.Value = clientGuid;
            SqlParameter labClientVersionParam = cmd.Parameters.Add("@labServerGuid", SqlDbType.VarChar, 50);
            labClientVersionParam.Value = labServerGuid;

            // execute the command

            try
            {
                connection.Open();
                Object ob = cmd.ExecuteScalar();
                if (ob != null && ob != System.DBNull.Value)
                {
                    lssID = ob.ToString();
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in retrieve LSS url by experiment", ex);
            }
            finally
            {
                connection.Close();
            }
            return lssID;
        }
/// <summary>
/// modify the experimentInfo
/// </summary>
/// <param name="experimentInfoId"></param>
/// <param name="labServerGuid"></param>
/// <param name="labServerName"></param>
/// <param name="labClientVersion"></param>
/// <param name="labClientName"></param>
/// <param name="providerName"></param>
        /// <param name="lssGuid"></param>
/// <returns></returns>true modified successfully, false otherwise
        public static bool ModifyExperimentInfo(int experimentInfoId, string labServerGuid, string labServerName, 
            string labClientGuid, string labClientName, string labClientVersion, string providerName, string lssGuid)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyExperimentInfo" store procedure
			SqlCommand cmd=new SqlCommand("ModifyExperimentInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter experimentInfoIdParam = cmd.Parameters.Add("@experimentInfoId", SqlDbType.Int);
		    experimentInfoIdParam.Value = experimentInfoId;
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
			SqlParameter lssIDParam = cmd.Parameters.Add("@lssGUID", SqlDbType.VarChar,50);
            lssIDParam.Value = lssGuid;
			bool i=false;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				int m=0;
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
				throw new Exception("Exception thrown in add ModifyExperimentInfo",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
        
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR LSSInfo
			 * !------------------------------------------------------------------------------!
			 * */
		/// <summary>
		/// add information of a particular lab side scheduling server identified by lssID
		/// </summary>
        /// <param name="lssGuid"></param>
		/// <param name="lssUrl"></param>
		/// <returns></returns>the unique ID which identifies the LSSInfo added,>0 successfully added, ==-1 otherwise
        public static int AddLSSInfo(string lssGuid, string lssName, string lssUrl)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "AddLSSInfo" store procedure
			SqlCommand cmd=new SqlCommand("AddLSSInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter lssIDParam = cmd.Parameters.Add("@lssGUID", SqlDbType.VarChar,50);
            lssIDParam.Value = lssGuid;
			SqlParameter lssNameParam = cmd.Parameters.Add("@lssName", SqlDbType.VarChar,256);
			lssNameParam.Value = lssName;
			SqlParameter lssURLParam = cmd.Parameters.Add("@lssURL", SqlDbType.VarChar,256);
			lssURLParam.Value = lssUrl;
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
				throw new Exception("Exception thrown in add LSS Information",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}

        /// <summary>
        /// Updates the data fields for the LSSInfo specified by the lssInfoId; note lssInfoId may not be changed
        /// </summary>
        /// <param name="lssInfoId"></param>
        /// <param name="lssGuid"></param>
        /// <param name="lssName"></param>
        /// <param name="lssUrl"></param>
        /// <returns></returns>true if lssInfo was successfully modified, ==false otherwise
        public static bool ModifyLSSInfo(int lssInfoId, string lssGuid, string lssName, string lssUrl)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyLSSInfo" store procedure
			SqlCommand cmd=new SqlCommand("ModifyLssInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter lssInfoIdParam = cmd.Parameters.Add("@lssInfoId", SqlDbType.Int);
			lssInfoIdParam.Value = lssInfoId;
			SqlParameter lssIDParam = cmd.Parameters.Add("@lssGUID", SqlDbType.VarChar,50);
            lssIDParam.Value = lssGuid;
			SqlParameter lssNameParam = cmd.Parameters.Add("@lssName", SqlDbType.VarChar,256);
			lssNameParam.Value = lssName;
			SqlParameter lssURLParam = cmd.Parameters.Add("@lssURL", SqlDbType.VarChar,256);
			lssURLParam.Value = lssUrl;
			bool i=false;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				int m=0;
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
				throw new Exception("Exception thrown in add ModifyLSSInfo",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
		/// <summary>
		/// delete the information of lab side scheduling servers identified by lssInfoIds
		/// </summary>
		/// <param name="lssInfoIds"></param>
		/// <returns></returns>An array of ints containing the IDs of all LSS whose informations not successfully removed, i.e., those for which the operation failed. 
		public static int[] RemoveLSSInfo(int[] lssInfoIds)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeletLSSInfo" store procedure
			SqlCommand cmd=new SqlCommand("DeletLSSInfo",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@lssInfoId", SqlDbType.Int));
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int lssInfoId in lssInfoIds)
				{
					cmd.Parameters["@lssInfoId"].Value = lssInfoId;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(lssInfoId);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove LSS infomation",ex);
			}
			finally
			{
				connection.Close();
			}	
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			return uIDs;			
		}
		/// <summary>
		/// Enumerates IDs of all the lssInfos 
		/// </summary>
		/// <returns></returns>An array of ints containing the IDs of all the lssInfos 
		public static int[] ListLSSInfoIDs()
		{
			int[] lssInfoIds;
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveRetrieveLssInfoIDs" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveLssInfoIDs", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// execute the command
			SqlDataReader dataReader = null;
			ArrayList arrayList=new ArrayList();
			try 
			{
				connection.Open();
				dataReader = cmd.ExecuteReader ();
				//store the lssInfoIds retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["LSS_Info_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["LSS_Info_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve lssInfoIds",ex);
			}
			finally
			{
				connection.Close();
			}
			lssInfoIds=Utilities.ArrayListToIntArray(arrayList);
			return lssInfoIds;
		}
		/// <summary>
		/// Returns an array of the immutable LSSInfo objects that correspond to the supplied lssInfo IDs. 
		/// </summary>
		/// <param name="lssInfoIds"></param>
		/// <returns></returns>
		public static LSSInfo[] GetLSSInfos(int[] lssInfoIds)
		{
			LSSInfo[] lssInfos=new LSSInfo[lssInfoIds.Length];
			for(int i=0; i<lssInfoIds.Length;i++)
			{
				lssInfos[i]=new LSSInfo();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveLSSInfoByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveLSSInfoByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@lssInfoId", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<lssInfoIds.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@lssInfoId"].Value = lssInfoIds[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						lssInfos[i].lssInfoId=lssInfoIds[i];
						if(dataReader[0] != System.DBNull.Value )
							lssInfos[i].lssGuid=dataReader.GetString(0);
						if(dataReader[1] != System.DBNull.Value )
							lssInfos[i].lssName=dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							lssInfos[i].lssUrl=dataReader.GetString(2);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get lssInfo",ex);
			}
			finally
			{
				connection.Close();
			}	
			return lssInfos;
		}

        /// <summary>
        /// Returns an array of the immutable LSSInfo objects that correspond to the supplied lssInfo IDs. 
        /// </summary>
        /// <param name="lssInfoIds"></param>
        /// <returns></returns>
        public static LSSInfo GetLSSInfo(string lssGuid)
        {
            LSSInfo lssInfo = new LSSInfo();

            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            SqlCommand cmd = new SqlCommand("RetrieveLSSInfoByGUID", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@lssGuid", SqlDbType.VarChar, 50));
            cmd.Parameters["@lssGuid"].Value = lssGuid;
            //execute the command
            try
            {
                connection.Open();
                SqlDataReader dataReader = null;
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    lssInfo.lssInfoId = dataReader.GetInt32(0);
                    if (dataReader[1] != System.DBNull.Value)
                        lssInfo.lssGuid = dataReader.GetString(1);
                    if (dataReader[2] != System.DBNull.Value)
                        lssInfo.lssName = dataReader.GetString(2);
                    if (dataReader[3] != System.DBNull.Value)
                        lssInfo.lssUrl = dataReader.GetString(3);
                }
                dataReader.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("Exception thrown in get lssInfo", ex);
            }
            finally
            {
                connection.Close();
            }
            return lssInfo;
        }


		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR Credential Set
			 * !------------------------------------------------------------------------------!
			 * */
		/// <summary>
		///  add a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>return the unique ID identifying the credential set added,>0 successfully added, ==-1 otherwise
        public static int AddCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName)
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
				throw new Exception("Exception thrown in add credential set",ex);
			}
			finally
			{
				connection.Close();
			}	
			return i;		
		}
		/// <summary>
		/// Updates the data fields for the credential set specified by the credentialSetId; note credentialSetId may not be changed
		/// </summary>
		/// <param name="credentialSetId"></param>
		/// <param name="groupName"></param>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="serviceBrokerName"></param>
		/// <returns></returns>true if reservation was successfully modified, ==false otherwise
        public static bool ModifyCredentialSet(int credentialSetId, string groupName, string serviceBrokerGuid, string serviceBrokerName)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "ModifyCredentialSet" store procedure
			SqlCommand cmd=new SqlCommand("ModifyCredentialSet",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter credentialSetIdParam = cmd.Parameters.Add("@credentialSetId", SqlDbType.Int);
			credentialSetIdParam.Value = credentialSetId;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter serviceBrokerNameParam = cmd.Parameters.Add("@serviceBrokerName", SqlDbType.VarChar,256);
			serviceBrokerNameParam.Value = serviceBrokerName;
			bool i=false;

			// execute the command
			try
			{
				connection.Open();
				Object ob=cmd.ExecuteScalar();
				int m=0;
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
				throw new Exception("Exception thrown in add ModifyCredentialSet",ex);
			}
			finally
			{
				connection.Close();
			}		
			return i;
             
		}
		/// <summary>
		/// delete the credential sets specified by the credentialSetIds
		/// </summary>
		/// <param name="credentialSetIds"></param>
		/// <returns></returns>an array of ints containing the IDs of all credentail sets not successfully removed
		public static int[] RemoveCredentialSets(int[] credentialSetIds)
		{
			ArrayList arrayList=new ArrayList();
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteCredentialSet" store procedure
			SqlCommand cmd=new SqlCommand("DeleteCredentialSetByID",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@credentialSetId", SqlDbType.Int));
			
			// execute the command
			try
			{
				connection.Open();
				//populate the parameters
				foreach(int credentialSetId in credentialSetIds)
				{			
					cmd.Parameters["@credentialSetId"].Value = credentialSetId;
					if (cmd.ExecuteNonQuery()==0)
					{
						arrayList.Add(credentialSetId);
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
		///  Remove a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>true, the credentialset is removed successfully, false otherwise
        public static bool RemoveCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName)
		{
			//create a connection
			SqlConnection connection= CreateConnection();
			//create a command
			//command executes the "DeleteCredentialSet" store procedure
			SqlCommand cmd=new SqlCommand("DeleteCredentialSet",connection);
			cmd.CommandType=CommandType.StoredProcedure;
			//populate the parameters
			SqlParameter serviceBrokerIDParam = cmd.Parameters.Add("@serviceBrokerGUID", SqlDbType.VarChar,50);
            serviceBrokerIDParam.Value = serviceBrokerGuid;
			SqlParameter serviceBrokerNameParam = cmd.Parameters.Add("@serviceBrokerName", SqlDbType.VarChar,256);
			serviceBrokerNameParam.Value = serviceBrokerName;
			SqlParameter groupNameParam = cmd.Parameters.Add("@groupName", SqlDbType.VarChar,50);
			groupNameParam.Value = groupName;
			bool removed = false;

			// execute the command
			try
			{
				connection.Open();
                if (cmd.ExecuteNonQuery() > 0)
                    removed = true;
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in remove credential set",ex);
			}
			finally
			{
				connection.Close();
			}	
			return removed;		
		}
		
/// <summary>
/// Enumerates IDs of all the credential sets 
/// </summary>
/// <returns></returns>An array of ints containing the IDs of all the credential sets 
		public static int[] ListCredentialSetIds()
		{
			int[] credentialSetIds;
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
				//store the credentialSetIds retrieved in arraylist
				
				while(dataReader.Read ())
				{	
					if(dataReader["Credential_Set_ID"] != System.DBNull.Value )
						arrayList.Add(Convert.ToInt32(dataReader["Credential_Set_ID"]));
				}
				dataReader.Close();
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in retrieve credentialSetIds",ex);
			}
			finally
			{
				connection.Close();
			}
			credentialSetIds=Utilities.ArrayListToIntArray(arrayList);
			return credentialSetIds;
		}
/// <summary>
/// Returns an array of the immutable credential set objects that correspond to the supplied credential set IDs
/// </summary>
/// <param name="credentialSetIds"></param>
/// <returns></returns>
		public static UssCredentialSet[] GetCredentialSets(int[] credentialSetIds)
		{
			UssCredentialSet[] credentialSets=new UssCredentialSet[credentialSetIds.Length];
			for(int i=0; i<credentialSetIds.Length;i++)
			{
				credentialSets[i]=new UssCredentialSet();
			}
			// create sql connection
			SqlConnection connection = CreateConnection();

			// create sql command
			// command executes the "RetrieveCredentialSetByID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveCredentialSetByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add(new SqlParameter("@credentialSetID", SqlDbType.Int));
			//execute the command
			try 
			{
				connection.Open();
				for(int i=0;i<credentialSetIds.Length;i++)
				{
					// populate the parameters
					cmd.Parameters["@credentialSetID"].Value = credentialSetIds[i];	
					SqlDataReader dataReader = null;
					dataReader=cmd.ExecuteReader();
					while(dataReader.Read())
					{	
						credentialSets[i].credentialSetId=credentialSetIds[i];
						if(dataReader[0] != System.DBNull.Value )
							credentialSets[i].groupName=dataReader.GetString(0);
						if(dataReader[1] != System.DBNull.Value )
							credentialSets[i].serviceBrokerGuid=dataReader.GetString(1);
						if(dataReader[2] != System.DBNull.Value )
							credentialSets[i].serviceBrokerName=dataReader.GetString(2);
					}
					dataReader.Close();
				}	
			} 

			catch (Exception ex)
			{
				throw new Exception("Exception thrown in get credential set",ex);
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
        public static int GetCredentialSetID(string serviceBrokerGuid, string groupName)
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
		}
	}

