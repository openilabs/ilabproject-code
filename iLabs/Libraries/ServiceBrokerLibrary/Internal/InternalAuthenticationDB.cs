/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: InternalAuthenticationDB.cs,v 1.3 2007/12/26 05:27:26 pbailey Exp $ */

using System;
using System.Data ;
using System.Data .SqlClient ;
using System.Configuration ;
using System.Collections ;

using iLabs.Core;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.Ticketing;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.Internal
{
	/// <summary>
	/// Summary description for InternalAuthenticationDB.
	/// </summary>
	public class InternalAuthenticationDB
	{
		public InternalAuthenticationDB()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// to create a native principal
		/// </summary>
		public static int CreateNativePrincipal(string userName)
		{
			int userID=-1;

			SqlConnection myConnection = ProcessAgentDB.GetConnection();
			SqlCommand myCommand = new SqlCommand("CreateNativePrincipal", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			myCommand.Parameters.Add (new SqlParameter("@userName",userName));
			try
			{
				myConnection.Open();
				userID = Int32.Parse ( myCommand.ExecuteScalar().ToString ());
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in creating native principal",ex);
			}
			finally
			{
				myConnection.Close();
			}
			return userID;
		}

		/// <summary>
		/// to delete the principals specified by the array of user IDS
		/// </summary>
		public static int[] DeleteNativePrincipals (int[] userIDs)
		{
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("DeleteNativePrincipal", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure ;
			myCommand.Parameters.Add (new SqlParameter("@userID",null));

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
					// Deleting from table Principals
					
					myCommand.Parameters["@userID"].Value = userID;
					if(myCommand.ExecuteNonQuery () == 0)
					{
						arrayList.Add (userID);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown in DeleteNativePrincipals",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			int[] uIDs = Utilities.ArrayListToIntArray(arrayList);
			
			return uIDs;
		}

		/// <summary>
		/// to retrieve a list of all native principals
		/// </summary>
		public static int[] SelectNativePrincipals ()
		{
			int[] userIDs;
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveNativePrincipals", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;

			try 
			{
				myConnection.Open ();
				

				// get native principal ids from table principals
				SqlDataReader myReader = myCommand.ExecuteReader ();
				ArrayList uIDs = new ArrayList();

				while(myReader.Read ())
				{	
					if(myReader["user_id"] != System.DBNull.Value )
						uIDs.Add( myReader["user_id"]);
				}
				myReader.Close ();
				// Converting to a int array
				userIDs = Utilities.ArrayListToIntArray(uIDs);

			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SelectClientItemIDs",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
			return userIDs;
		}

		/// <summary>
		/// Sets the password of the specified native principal
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="password"></param>
		public static void SaveNativePassword (int userID, string password)
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("SaveNativePassword", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			
			myCommand.Parameters.Add (new SqlParameter("@userID",userID));
			myCommand.Parameters.Add (new SqlParameter("@password",password));

			try 
			{
				myConnection.Open ();
				
				// set user's password
				myCommand.ExecuteNonQuery();
				
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SetNativePassword",ex);
			}
			finally 
			{
				myConnection.Close();
			}
			
		}

		/// <summary>
		/// retrieves the password of the specified user
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static string ReturnNativePassword (int userID)
		{
			
			SqlConnection myConnection = new SqlConnection (ConfigurationSettings.AppSettings ["sqlConnection"]);
			SqlCommand myCommand = new SqlCommand ("RetrieveNativePassword", myConnection);
			myCommand.CommandType = CommandType.StoredProcedure;
			
			myCommand.Parameters.Add (new SqlParameter("@userID",userID));
			
			try 
			{
				myConnection.Open ();
				
				// get user's password
				return (myCommand.ExecuteScalar().ToString());
				
			}
			catch (Exception ex)
			{
				throw new Exception("Exception thrown SetNativePassword",ex);
			}
			finally 
			{
				myConnection.Close ();
			}
			
		}
	}
}
