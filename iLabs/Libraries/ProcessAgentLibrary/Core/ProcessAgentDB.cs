/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: ProcessAgentDB.cs,v 1.16 2007/12/26 05:27:22 pbailey Exp $
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using iLabs.UtilLib;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.Ticketing;

using iLabs.Proxies.PAgent;
using iLabs.Proxies.Ticketing;

namespace iLabs.Core
{
	/// <summary>
	/// Provides database access and table to object marshalling.
	/// </summary>
	public class ProcessAgentDB 
	{
		protected static string connectionStr;
        protected static ProcessAgent serviceAgent;
        protected static ProcessAgentInfo domainSB;

        static ProcessAgentDB()
        {
            // read connection string from the app settings
            connectionStr = ConfigurationManager.AppSettings["sqlConnection"];
            if (connectionStr == null)
            {
                throw new NoNullAllowedException(" The connection string is not specified, check configuration");
            }
            RefreshServiceAgent();
        }

        /// <summary>
        /// Loads the static varable serviceAgent with the processAgent's ProcessAgent data.
        /// </summary>
        /// <returns></returns>
        public static bool RefreshServiceAgent()
        {
            ProcessAgentDB padb = new ProcessAgentDB();
            ProcessAgent tmp = padb.GetSelfProcessAgent();
            serviceAgent = tmp;
            return (tmp != null);
        }

        public static bool CheckServiceAgent()
        {
            if (serviceAgent == null)
                RefreshServiceAgent();
            return (serviceAgent != null);
        }

        /// <summary>
        /// Creates an unopened connection to the database
        /// </summary>
        /// <returns></returns>
        public static  SqlConnection GetConnection()
        {
            SqlConnection connection = null;
            try
            {
                if (connectionStr == null || connectionStr.Equals(""))
                {
                    // read connection string from the app settings
                    connectionStr= ConfigurationManager.AppSettings["sqlConnection"];
                }
                // create an SqlConnection
                connection = new SqlConnection(connectionStr);
            }
            catch (Exception e)
            {
                Utilities.WriteLog(Utilities.DumpException(e));
                throw;
            }
            return connection;
        }


        /// <summary>
        /// this serviceAgent, null if selfRecord has not been saved to database.
        /// </summary>
        public static ProcessAgent ServiceAgent
        {
            get
            {
                return serviceAgent;
            }
        }
      
        /// <summary>
        /// this serviceAgent's Guid, null if selfRecord has not been saved to database.
        /// </summary>
        public static string ServiceGuid
        {
            get
            {
                if (serviceAgent != null)
                {
                    return serviceAgent.agentGuid;
                }
                else
                    return null;
            }
        }

        public ProcessAgentDB()
        {

        }

        /// <summary>
        /// Inserts or updates the domain ServiceBroker record in the database.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool SetDomainGuid(string guid)
        {
            bool status = false;
            SqlConnection connection = FactoryDB.GetConnection();
           SqlCommand cmd = new SqlCommand("UpdateDomainGuid", connection);
			 cmd.CommandType = CommandType.StoredProcedure;

				// populate parameters
				SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
				guidParam.Value = guid;
				
			try{
                connection.Open();
				status = Convert.ToBoolean(cmd.ExecuteScalar());
                RefreshServiceAgent();
			}
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}
            finally{
                connection.Close();
            }
			return status;
		}
        
        /// <summary>
        /// Creates an open connection to the database
        /// </summary>
        /// <returns></returns>
		protected SqlConnection CreateConnection() 
		{
			try 
			{
				if (connectionStr == null || connectionStr.Equals("")) 
				{
					// read connection string from the app settings
					connectionStr
                        = ConfigurationManager.AppSettings["sqlConnection"];
					Console.WriteLine("DB_CONNECTION: " + connectionStr);
				}
			} 
			catch (Exception e) 
			{
				writeEx(e);
				throw;
			}

			// create an SqlConnection
			SqlConnection connection = new SqlConnection(connectionStr);

			try 
			{
				// open connection
				connection.Open();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}
			catch (InvalidOperationException e) 
			{
				writeEx(e);
			}

			return connection;
		}

        /// <summary>
        ///  This agent's Web Service URL
        /// </summary>
		public string GetServiceURL()
		{
            if (serviceAgent != null)
                return serviceAgent.webServiceUrl;
            else 
                return null;
		}
        /// <summary>
        /// Check that the coupon is in the database.
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
		public bool AuthenticateCoupon(Coupon coupon)
		{
			bool status = false;
			SqlConnection connection = FactoryDB.GetConnection();
            connection.Open();
			status = AuthenticateCoupon(connection,coupon);
			connection.Close();
			return status;
		}

        /// <summary>
        /// Check that the coupon is in the database.
        /// </summary>
        /// <param name="connection">an open connection</param>
        /// <param name="coupon"></param>
        /// <returns></returns>
		public bool AuthenticateCoupon(SqlConnection connection, Coupon coupon){

			bool status = false;
			try 
			{
				SqlCommand cmd = new SqlCommand("AuthenticateCoupon", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				// populate parameters
				SqlParameter idParam = cmd.Parameters.Add("@couponid", SqlDbType.BigInt);
				idParam.Value = coupon.couponId;
				SqlParameter issuerParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.VarChar, 50);
				issuerParam.Value = coupon.issuerGuid;
				SqlParameter passKeyParam = cmd.Parameters.Add("@passKey", SqlDbType.VarChar, 100);
				passKeyParam.Value = coupon.passkey;
			
				status = Convert.ToBoolean(cmd.ExecuteScalar());
			}
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}
			return status;
		}

        /// <summary>
        /// Inserts a coupon in the database
        /// </summary>
        /// <param name="coupon"></param>
		public void InsertCoupon(Coupon coupon)
        {
            SqlConnection connection = null;

            try
            {
                connection = FactoryDB.GetConnection();
                connection.Open();
                InsertCoupon(connection, coupon);
            }

            catch
            {
                throw;
            }

            finally
            {
                connection.Close();
            }

		}

		//////////////////////////////////////////////////////////////////
		/// <summary>
		/// Insert a coupon into the database
		/// </summary>
		/// <param name="couponID"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public void InsertCoupon(SqlConnection connection, long id,string issuerGuid,string pass) 
		{

			// create sql command
			// command executes the "GetCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("InsertCoupon", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponIdParam.Value = id;
			SqlParameter couponGuidParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.VarChar, 50);
			couponGuidParam.Value = issuerGuid;
			SqlParameter passKeyParam = cmd.Parameters.Add("@passKey", SqlDbType.VarChar, 100);
			passKeyParam.Value = pass;
			
			
			try 
			{
				
				cmd.ExecuteNonQuery();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}
	
		}
		
		protected void InsertCoupon(SqlConnection connection, Coupon coupon)
		{
				InsertCoupon(connection,coupon.couponId,coupon.issuerGuid,coupon.passkey);
		}


		/// <summary>
		/// Mark the coupon as cancelled, if the coupon is found and 
		/// not already cancelled any tickets in the Ticket collection are also cancelled.
		/// </summary>
		/// <param name="coupon"></param>
		/// <returns><code>true</code> if the coupon is found, is not currently cancelled and cancelation completed </returns>
		public bool CancelCoupon(Coupon coupon)
		{
			bool status = false;
			// create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "CancelCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("CancelCoupon", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponIdParam.Value = coupon.couponId;
			SqlParameter issuerParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.BigInt);
			couponIdParam.Value = coupon.issuerGuid;
			// execute the command
            try
            {
                connection.Open();
                status = Convert.ToBoolean(cmd.ExecuteScalar());
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
        public int DeleteCoupons(string guid)
        {
            int status = 0;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "DeleteCoupons" stored procedure
            SqlCommand cmd = new SqlCommand("DeleteCoupons", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);
            guidParam.Value = guid;
            
            // execute the command
            try
            {
                connection.Open();
                status = Convert.ToInt32(cmd.ExecuteScalar());
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


		/// <summary>
		/// Retrieve a coupon from the database given its id and issuer
		/// </summary>
		/// <param name="couponID"></param>
		/// <param name="issuerGuid"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public Coupon GetCoupon(long couponID, string issuerGuid) 
		{
            SqlConnection connection = null;
            Coupon coupon = null;

            try
            {
                // create sql connection
                connection = FactoryDB.GetConnection();
                connection.Open();
                coupon = GetCoupon(connection, couponID, issuerGuid);
            }
            catch
            {
                throw;
            }

            finally
            {
                connection.Close();
            }

			return coupon;
		}



		/// <summary>
		/// Retrieve a coupon from the database given its id and issuerGuid.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="issuerGuid"></param>
		/// <param name="couponID"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found or has been cancelled</returns>
		public Coupon GetCoupon(SqlConnection connection,long couponID, string issuerGuid) 
		{
            bool cancelled = false;
            Coupon coupon = null;

			// command executes the "GetCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetCoupon", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponIdParam.Value = couponID;
			SqlParameter couponGuidParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.VarChar, 50);
			couponGuidParam.Value = issuerGuid;

			// read the result
			SqlDataReader dataReader = null;
			try 
			{
				dataReader = cmd.ExecuteReader();
                
                // Create the coupon object
                // from the dataReader

                while (dataReader.Read())
                {
                    coupon = new Coupon();
                    coupon.couponId = couponID;
                    coupon.issuerGuid = issuerGuid;
                    cancelled = dataReader.GetBoolean(0);
                    coupon.passkey = dataReader.GetString(1).Trim();
                }
			} 
			catch (Exception e) 
			{
				writeEx(e);
				throw;
			}
            if (cancelled)
                return null;
            else
			    return coupon;
		}
        /// <summary>
        /// Get the expected incoming Identity coupon for the specified agent.
        /// </summary>
        /// <param name="agentGUID"></param>
        /// <returns>The coupon or null</returns>
		public Coupon GetIdentityInCoupon(string agentGUID)
        {
			Coupon coupon = null;
            bool cancelled = true;
			SqlConnection connection = FactoryDB.GetConnection();
			SqlCommand cmd = new SqlCommand("GetIdentInCoupon", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			SqlParameter guidParam = cmd.Parameters.Add("@agentGUID", SqlDbType.VarChar,50);
			guidParam.Value = agentGUID;
			
			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    coupon = new Coupon();
                    cancelled = dataReader.GetBoolean(0);
                    coupon.couponId = dataReader.GetInt64(1);
                    if (!DBNull.Value.Equals(dataReader.GetValue(2)))
                    coupon.issuerGuid = dataReader.GetString(2);
                    if (!DBNull.Value.Equals(dataReader.GetValue(3)))
                    coupon.passkey = dataReader.GetString(3);
                }
            }
            catch (Exception e)
            {
                writeEx(e);
                throw;
            }

            finally
            {
                connection.Close();
            }
            if (cancelled)
            {
                return null;
            }
            else
                return coupon;	
		}
        /// <summary>
        /// Get the Identity coupon to be used in outgoing messages to the specified agent.
        /// </summary>
        /// <param name="agentGUID"></param>
        /// <returns>The coupon or null</returns>
		public Coupon GetIdentityOutCoupon(string agentGUID)
		{
			Coupon coupon = null;
            bool cancelled = true;
			SqlConnection connection = FactoryDB.GetConnection();
			SqlCommand cmd = new SqlCommand("GetIdentOutCoupon", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			SqlParameter guidParam = cmd.Parameters.Add("@agentGUID", SqlDbType.VarChar,50);
			guidParam.Value = agentGUID;
			
			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    cancelled = dataReader.GetBoolean(0);
                    coupon.couponId = dataReader.GetInt64(1);
                    coupon.issuerGuid = dataReader.GetString(2);
                    coupon.passkey = dataReader.GetString(2);
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
            if (cancelled)
            {
                return null;
            }
            else
                return coupon;	
		}
        /// <summary>
        /// Checks the AgentAuthHeader to make sure the coupon matches the local IdentityIn Coupon for the specified agent.
        /// </summary>
        /// <param name="agentHeader"></param>
        /// <returns></returns>
        public bool AuthenticateAgentHeader(AgentAuthHeader agentHeader)
        {
            if (agentHeader.agentGuid == null)
            {
                throw new TicketingAutheticationFailedException("AgentGuid is null");
            }
            Coupon inCoupon = GetIdentityInCoupon(agentHeader.agentGuid);
            
            if (inCoupon == null)
            {
                return false;
            }
            else
            {
                return agentHeader.coupon == inCoupon;
            }
        }

       

        /// <summary>
        /// Retrieves ticket from local database or the SB. Ticket type is needed to retrieve ticket from SB, 
        /// </summary>
        /// <param name="coupon">operation coupon sent by the Process Agent</param>
        /// <param name="ticketType">type of the ticket to be retrieved</param>
        /// <returns>Retrieved ticket</returns>
        public Ticket RetrieveAndVerify(Coupon coupon, string ticketType)
        {
            Ticket theTicket = RetrieveTicket(coupon, ticketType, ServiceGuid);
            if (theTicket == null)
            {
                TicketIssuerProxy ticketIssuerProxy = new TicketIssuerProxy();

                //Get the Service Broker info from the database
                ProcessAgentInfo sbInfo = GetServiceBrokerInfo();

                //get the SB web service URL, and set the proxy's URL accordingly
                ticketIssuerProxy.Url = sbInfo.webServiceUrl;

                iLabs.DataTypes.SoapHeaderTypes.AgentAuthHeader agentAuthHeader = new iLabs.DataTypes.SoapHeaderTypes.AgentAuthHeader();

                //set the SOAP header (of the proxy class) to the agentCoupon
                agentAuthHeader.coupon = sbInfo.identOut;
                agentAuthHeader.agentGuid = ServiceGuid;
                ticketIssuerProxy.AgentAuthHeaderValue = agentAuthHeader;
                
                //call the RetrieveTicket web service method on the SB (ticket issuer)
                theTicket = ticketIssuerProxy.RedeemTicket(coupon, ticketType, ServiceGuid);
                if (theTicket != null)
                {
                    // If the ticket is no longer valid do not insert it
                    if (!theTicket.IsExpired() && !theTicket.isCancelled)
                    {
                        if (!AuthenticateCoupon(coupon))
                        {
                            // coupon is not in the database
                            InsertCoupon(coupon);
                        }
                        InsertTicket(theTicket);
                    }
                }   
            }
            //if ticket not found locally or in in the Ticket issuer (SB) database, throw exception
            if (theTicket == null)
            {
                throw new TicketNotFoundException("The requested ticket was not found. "
                    + "Ticket type: " + ticketType + ". Access denied.");
            }
            if (theTicket.IsExpired() || theTicket.isCancelled)
            {
                     throw new TicketExpiredException("The Retrieved ticket has expired. "
                    + "Ticket type: " + ticketType + ". Access denied.");
            }
            return theTicket;
        }

    	/// <summary>
		/// Mark the ticket as cancelled in the DB, this cancels the local copy of the ticket if found.
		/// Should not be directly called but as the result of a successful RequestTicketCancellation call.
		/// </summary>
		/// <param name="ticket"></param>
		/// <returns><code>true</code> if the ticket has been cancelled successfully</returns>
		public bool CancelTicket(Ticket ticket)
		{
			bool status = false;
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "CancelTicket" stored procedure
			SqlCommand cmd = new SqlCommand("CancelTicketByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			// populate the parameters
			SqlParameter couponIdParam = cmd.Parameters.Add("@ticketID", SqlDbType.BigInt);
			couponIdParam.Value = ticket.ticketId;
            SqlParameter issuerParam = cmd.Parameters.Add("@issuer", SqlDbType.VarChar,50);
            issuerParam.Value = ticket.issuerGuid;
			
			// execute the command
			//SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                status = Convert.ToBoolean(cmd.ExecuteScalar());
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

		/// <summary>
		/// Mark the ticket as cancelled in the DB, this cancels the local copy of the ticket if found.
		/// Should not be directly called but as the result of a successful RequestTicketCancellation call.
		/// </summary>
		/// <param name="coupon"></param>
        /// <param name="type"></param>
        /// <param name="redeemerGUID"></param>
		/// <returns><code>true</code> if the ticket has been cancelled successfully</returns>
		public bool CancelTicket(Coupon coupon,String type,String redeemerGUID)
		{
			int status = -1;
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "CancelTicket" stored procedure
			SqlCommand cmd = new SqlCommand("CancelTicket", connection);
			cmd.CommandType = CommandType.StoredProcedure;
			// populate the parameters
			SqlParameter typeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar,100);
			typeParam.Value = type;
			SqlParameter redeemerParam = cmd.Parameters.Add("@redeemerGuid", SqlDbType.VarChar,50);
			redeemerParam.Value = redeemerGUID;
			SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponIdParam.Value = coupon.couponId;
			SqlParameter issuerParam = cmd.Parameters.Add("@issuerGuid", SqlDbType.VarChar,50);
			issuerParam.Value = coupon.issuerGuid;
			
			// execute the command
			
            try
            {
                connection.Open();
                status = Convert.ToInt32(cmd.ExecuteScalar());
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

			return status >=0;	
		}

        /// <summary>
        /// Delete the ticket from the DB, this deletes the local copy of the ticket if found.
        /// Should not be directly called but as the result of a successful RequestTicketCancellation call.
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="type"></param>
        /// <param name="redeemerGUID"></param>
        /// <returns><code>true</code> if the ticket has been cancelled successfully</returns>
        public bool DeleteTicket(Coupon coupon, String type, String redeemerGUID)
        {
            int status = -1;
            int ccount = 0;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "CancelTicket" stored procedure
            SqlCommand cmd = new SqlCommand("DeleteTicket", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            // populate the parameters
            SqlParameter typeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            typeParam.Value = type;
            SqlParameter redeemerParam = cmd.Parameters.Add("@redeemerGuid", SqlDbType.VarChar, 50);
            redeemerParam.Value = redeemerGUID;
            SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIdParam.Value = coupon.couponId;
            SqlParameter issuerParam = cmd.Parameters.Add("@issuerGuid", SqlDbType.VarChar, 50);
            issuerParam.Value = coupon.issuerGuid;

            // execute the command

            try
            {
                connection.Open();
                status = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
                if(status > 0){
                    cmd = new SqlCommand("GetCouponCollectionCount", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter coupIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
                    SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);
                    coupIDParam.Value = coupon.couponId;
                    guidParam.Value = coupon.issuerGuid;
                    int collectioncount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (collectioncount == 0)
                    {
                        cmd.Dispose();
                 
                        cmd = new SqlCommand("DeleteCoupon", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Transaction = transaction;
                        coupIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
                        guidParam = cmd.Parameters.Add("@issuerGuid", SqlDbType.VarChar, 50);
                        coupIDParam.Value = coupon.couponId;
                        guidParam.Value = coupon.issuerGuid;
                        ccount = Convert.ToInt32(cmd.ExecuteScalar());
                          
                    }

                }
                Utilities.WriteLog("DeleteTicket: ticketCount=" + status + " \tcouponCount=" + ccount);
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

            return status >= 0;
        }

        public int DeleteTickets(string guid)
        {
            int status = 0;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            SqlCommand cmd = new SqlCommand("DeleteTickets", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            guidParam.Value = guid;

            // execute the command
            try
            {
                connection.Open();
                status = Convert.ToInt32(cmd.ExecuteScalar());
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
            /// <summary>
        /// Insert a ticket in the Ticket table
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public void InsertTicket(Ticket ticket)
        {

            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // command executes the "InsertTicket" stored procedure
            SqlCommand cmd = new SqlCommand("InsertTicket", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter ticketIDParam = cmd.Parameters.Add("@ticketID", SqlDbType.BigInt);
            ticketIDParam.Value = ticket.ticketId;
            SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            ticketTypeParam.Value = ticket.type;
            SqlParameter couponIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIDParam.Value = ticket.couponId;
            SqlParameter issuerIDParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.VarChar, 50);
            issuerIDParam.Value = ticket.issuerGuid.ToString();
            SqlParameter redeemerIDParam = cmd.Parameters.Add("@redeemerGUID", SqlDbType.VarChar, 50);
            redeemerIDParam.Value = ticket.redeemerGuid.ToString();
            SqlParameter sponsorIDParam = cmd.Parameters.Add("@sponsorGUID", SqlDbType.VarChar, 50);
            sponsorIDParam.Value = ticket.sponsorGuid.ToString();
            SqlParameter payloadParam = cmd.Parameters.Add("@payload", SqlDbType.Text);
            payloadParam.Value = ticket.payload;

            SqlParameter cancelledParam = cmd.Parameters.Add("@cancelled", SqlDbType.Bit);
            cancelledParam.Value = 0;
            SqlParameter creationTimeParam = cmd.Parameters.Add("@creationTime", SqlDbType.DateTime);
            creationTimeParam.Value = ticket.creationTime;

            SqlParameter durationParam = cmd.Parameters.Add("@duration", SqlDbType.BigInt);
            durationParam.Value = ticket.duration;
            long id = -1;
            try
            {
                connection.Open();
                id = Convert.ToInt64(cmd.ExecuteScalar());
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

        
      
		/// <summary>
		/// Retrieve a ticket from the database. ProcessAgent version
		/// The triple (couponID, redeemerID, type) uniquely identifies the ticket.
        /// Note the ProcessAgent must store the tickets, a null return is a valid value.
		/// </summary>
		/// <param name="coupon"></param>
		/// <param name="type"></param>
        /// <param name="redeemerGUID"></param>
		/// <returns>Retrieved Ticket, or null if  the ticket cannot be found</returns>
		public virtual Ticket RetrieveTicket(Coupon coupon, string ticketType, string redeemerGUID)
		{
			Ticket result = null;
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveTicket" stored procedure
			SqlCommand cmd = new SqlCommand("GetTicket", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate the parameters
			SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponIdParam.Value = coupon.couponId;
			SqlParameter issuerIdParam = cmd.Parameters.Add("@issuer", SqlDbType.VarChar, 50);
			issuerIdParam.Value = coupon.issuerGuid;
            SqlParameter redeemerIdParam = cmd.Parameters.Add("@redeemer", SqlDbType.VarChar, 50);
            redeemerIdParam.Value = redeemerGUID;
			SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
			ticketTypeParam.Value = ticketType;    

			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Ticket ticket = new Ticket();
                    // read ticket id
                    ticket.ticketId = (long)dataReader.GetInt64(0);
                    ticket.type = dataReader.GetString(1);
                    // read coupon id
                    ticket.couponId = (long)dataReader.GetInt64(2);
                    ticket.issuerGuid = dataReader.GetString(3);
                    // read redeemer id
                    ticket.redeemerGuid = dataReader.GetString(4);
                    // read sponsor id
                    ticket.sponsorGuid = dataReader.GetString(5);
                    // read expiration
                    ticket.creationTime = dataReader.GetDateTime(6);
                    ticket.duration = dataReader.GetInt64(7);
                    // read payload
                    ticket.payload = dataReader.GetString(8);
                    // read Cancelled
                    bool cancelled = dataReader.GetBoolean(9);
                    if (!cancelled)
                        result = ticket;
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

			return result;
		}

		/// <summary>
		/// Retrieve all the tickets of a certain type from the database 
		/// </summary>
		/// <param name="ticketType"></param>
		/// <returns>Array of ticket objects</returns>
		public Ticket[] RetrieveTicketsByType(String ticketType) 
		{
            ArrayList ticketsList = new ArrayList();
            Ticket ticket = null;

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveTicketsByType" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTicketsByType", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar,100);
			ticketTypeParam.Value = ticketType;

			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    ticket = new Ticket();
                    // read ticket id
                    ticket.ticketId = (long)dataReader.GetInt64(1);
                    ticket.type = dataReader.GetString(2);
                    // read coupon id
                    long couponID = (long)dataReader.GetInt64(3);
                    string issuerGUID = dataReader.GetString(4);
                    // read redeemer id
                    ticket.redeemerGuid = dataReader.GetString(5);
                    // read sponsor id
                    ticket.sponsorGuid = dataReader.GetString(6);

                    // read expiration
                    ticket.creationTime = dataReader.GetDateTime(7);
                    ticket.duration = dataReader.GetInt64(8);
                    // read payload
                    ticket.payload = dataReader.GetString(9);
                    // read Cancelled
                    bool cancelled = dataReader.GetBoolean(10);

                    // add to tickets list
                    if (!cancelled)
                        ticketsList.Add(ticket);
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

			Ticket[] tickets = (Ticket[])ticketsList.ToArray(ticket.GetType());
			return tickets;
		}

		/// <summary>
		/// Retrieve all the tickets that belong to the given coupon
		/// </summary>
		/// <param name="ticketType"></param>
		/// <returns>Array of ticket objects</returns>
		public Ticket[] RetrieveTickets(Coupon coupon) 
		{

            ArrayList ticketsList = new ArrayList();
            Ticket ticket = new Ticket();

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveTickets" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveTickets", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			SqlParameter couponIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
			couponIDParam.Value = coupon.couponId;
			SqlParameter issuerParam = cmd.Parameters.Add("@issuer", SqlDbType.VarChar,50);
			issuerParam.Value = coupon.issuerGuid;

			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    ticket = new Ticket();
                    // read ticket id
                    ticket.ticketId = (long)dataReader.GetInt64(1);
                    ticket.type = dataReader.GetString(2);
                    // read coupon id
                    long couponID = (long)dataReader.GetInt64(3);
                    string issuerGUID = dataReader.GetString(4);
                    // read redeemer id
                    ticket.redeemerGuid = dataReader.GetString(5);
                    // read sponsor id
                    ticket.sponsorGuid = dataReader.GetString(6);

                    // read expiration
                    ticket.creationTime = dataReader.GetDateTime(7);
                    ticket.duration = dataReader.GetInt64(8);
                    // read payload
                    ticket.payload = dataReader.GetString(9);
                    // read Cancelled
                    bool cancelled = dataReader.GetBoolean(10);

                    // add to tickets list
                    if (!cancelled)
                        ticketsList.Add(ticket);
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
     	
			Ticket[] tickets = (Ticket[])ticketsList.ToArray(ticket.GetType());
			return tickets;
		}

        /// <summary>
        /// Retrieve all the ticket types present in the DB
        /// </summary>
        /// <returns>Array of ticket type objects</returns>
        public TicketType[] RetrieveTicketTypes()
        {
            ArrayList ticketTypesList = new ArrayList();
            TicketType ticketType = null;

            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "RetrieveTicketsByType" stored procedure
            SqlCommand cmd = new SqlCommand("RetrieveTicketTypes", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            
            // execute the command
            SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    ticketType = new TicketType();
                    // read ticket type id
                    ticketType.ticketTypeId = (int)dataReader.GetInt32(0);
                    // read ticket type name
                    ticketType.name = dataReader.GetString(1);
                    // read ticket type short description
                    ticketType.shortDescription = dataReader.GetString(2);
                    // read abstact
                    ticketType.isAbstract = dataReader.GetBoolean(3);

                    // add to tickets list
                    ticketTypesList.Add(ticketType);
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

            // sort list in ascending order of ticket short description
            ticketTypesList.Sort(new TicketTypeComparer());

            // create array from list
            TicketType[] ticketTypes = (TicketType[])ticketTypesList.ToArray(ticketType.GetType());
            return ticketTypes;
        }




        /// <summary>
        /// Finds all expired and not cancelled tickets in the Ticket table and deletes them.
        /// IF the ticket's coupon no longer has any tickets the coupon is deleted. This should
        /// only process tickets that have a valid duration.
        /// </summary>
        /// <returns></returns>
        public int ProcessExpiredTickets()
        {
            List<Coupon> coupons = new List<Coupon>();
            List<Coupon> remove = new List<Coupon>();
            List<LongTag> tickets = new List<LongTag>();

            int ticketCount = 0;
            int couponCount = 0;

            // Get all Expired Ticket information
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            SqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                // create sql command
                SqlCommand cmd = new SqlCommand("GetExpiredTickets", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = transaction;

                // execute the command
                SqlDataReader dataReader = null;


                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    long ticketID = dataReader.GetInt64(0);
                    long couponID = dataReader.GetInt64(1);
                    string guid = dataReader.GetString(2);
                    string pass = dataReader.GetString(3);
                    tickets.Add(new LongTag(ticketID, guid));
                    Coupon coupon = new Coupon(guid, couponID, pass);
                    if (!coupons.Contains(coupon))
                    {
                        coupons.Add(coupon);
                    }
                }
                dataReader.Close();
                cmd.Dispose();

                if (tickets.Count > 0)
                {
                    Utilities.WriteLog("ProcessExpiredTickets: expired count = " + tickets.Count);
                    cmd = new SqlCommand("DeleteTicketByID", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;
                    SqlParameter ticketIDParam = cmd.Parameters.Add("@ticketID", SqlDbType.BigInt);
                    SqlParameter issuerParam = cmd.Parameters.Add("@issuer", SqlDbType.VarChar, 50);
                    foreach (LongTag tag in tickets)
                    {
                        ticketIDParam.Value = tag.id;
                        issuerParam.Value = tag.tag;
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        ticketCount++;

                    }
                    cmd.Dispose();
                    cmd = new SqlCommand("GetCouponCollectionCount", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;
                    SqlParameter couponIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
                    SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);

                    foreach (Coupon c in coupons)
                    {
                        couponIDParam.Value = c.couponId;
                        guidParam.Value = c.issuerGuid;
                        Object objCount = cmd.ExecuteScalar();
                        int collectioncount = 0;
                        if (objCount != null)
                            collectioncount = Convert.ToInt32(objCount);
                        if (collectioncount == 0)
                        {
                            remove.Add(c);
                        }
                    }
                    cmd.Dispose();
                    if (remove.Count > 0)
                    {
                        cmd = new SqlCommand("DeleteCoupon", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = transaction;
                        couponIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
                        guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                        foreach (Coupon co in remove)
                        {
                            couponIDParam.Value = co.couponId;
                            guidParam.Value = co.issuerGuid;
                            int ccount = Convert.ToInt32(cmd.ExecuteScalar());
                            if (ccount > 0)
                            {
                                couponCount++;
                            }
                        }
                    }
                }
                transaction.Commit();
                Utilities.WriteLog("ProcessExpiredTickets: ticketCount=" + ticketCount + " \tcouponCount=" + couponCount);

            }
            catch (Exception e)
            {
                transaction.Rollback();
                Utilities.WriteLog("ProcessExpiredTickets:  transaction Failed - Rollback- ticketCount=" + ticketCount + " \tcouponCount=" + couponCount +"\n" + e.Message);
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return ticketCount;
        }

        /// <summary>
        /// Set the coupon id of the identification in coupon in the record of the process agent
        /// </summary>
        public void SetIdentCoupons(string agentGuid, long inId, long outId, string issuerGuid)
        {
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "SetIdentificationCouponID" stored procedure
            SqlCommand cmd = new SqlCommand("SetIdentCoupons", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate params
            SqlParameter processAgentGuidParam = cmd.Parameters.Add("@agentGUID", SqlDbType.VarChar, 50);
            processAgentGuidParam.Value = agentGuid;

            SqlParameter inIdParam = cmd.Parameters.Add("@inID", SqlDbType.BigInt);
            if(inId > 0)
            inIdParam.Value = inId;
            else
            inIdParam.Value = null;
            SqlParameter outIdParam = cmd.Parameters.Add("@outID", SqlDbType.BigInt);
            if(outId > 0)
            outIdParam.Value = outId;
            else
                outIdParam.Value = null;

            SqlParameter issuerGuidParam = cmd.Parameters.Add("@issuerGUID", SqlDbType.VarChar, 50);
            if (issuerGuid != null && issuerGuid.Length > 0)
                issuerGuidParam.Value = issuerGuid;
            else
                issuerGuidParam.Value = null;
            // execute the command
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Set the coupon id of the identification in coupon in the record of the process agent
        /// </summary>
        public void SetIdentInCouponID(string guid, long id)
        {
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "SetIdentificationCouponID" stored procedure
            SqlCommand cmd = new SqlCommand("SetIdentInCouponID", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate GUID param
            SqlParameter processAgentGuidParam = cmd.Parameters.Add("@agentGUID", SqlDbType.VarChar, 50);
            processAgentGuidParam.Value = guid;

            // populate coupon id param
            SqlParameter couponIdParam = cmd.Parameters.Add("@ID", SqlDbType.BigInt);
            couponIdParam.Value = id;

            // execute the command
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// Set the coupon id of the identification in coupon in the record of the process agent
        /// </summary>
        public void SetIdentOutCouponID(string guid, long id)
        {
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "SetIdentificationCouponID" stored procedure
            SqlCommand cmd = new SqlCommand("SetIdentOutCouponID", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate GUID param
            SqlParameter processAgentGuidParam = cmd.Parameters.Add("@agentGUID", SqlDbType.VarChar, 50);
            processAgentGuidParam.Value = guid;

            // populate coupon id param
            SqlParameter couponIdParam = cmd.Parameters.Add("@ID", SqlDbType.BigInt);
            couponIdParam.Value = id;

            // execute the command
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                connection.Close();
            }
        }

        public virtual int ModifyDomainCredentials(string originalGuid, ProcessAgent service, Coupon inCoupon, Coupon outCoupon, string extra)
        {
            int status = 0;
            bool error = false;
            StringBuilder buf = new StringBuilder();

            Coupon couponIn = null;
            Coupon couponOut = null;
            if (service.domainGuid == null)
            {
                error = true;
                buf.Append(" The domain is not set.");
            }
            if (inCoupon == null || inCoupon.issuerGuid == null || (inCoupon.issuerGuid.CompareTo(service.domainGuid) != 0))
            {
                error = true;
                buf.Append(" The inCoupon is not valid.");
            }
            else
            {
                couponIn = GetCoupon(inCoupon.couponId, inCoupon.issuerGuid);
                if (couponIn == null)
                {
                    InsertCoupon(inCoupon);
                }
                else if (couponIn.passkey.CompareTo(inCoupon.passkey) != 0)
                {
                    error = true;
                    buf.Append(" inCoupon conflits with coupon in database.");
                }
            }

            if (outCoupon == null || outCoupon.issuerGuid == null || (outCoupon.issuerGuid.CompareTo(service.domainGuid) != 0))
            {
                error = true;
                buf.Append(" The outCoupon is not valid.");
            }
            else
            {
                couponOut = GetCoupon(outCoupon.couponId, outCoupon.issuerGuid);
                if (couponOut == null)
                {
                    InsertCoupon(outCoupon);
                }
                else if (couponOut.passkey.CompareTo(outCoupon.passkey) != 0)
                {
                    error = true;
                    buf.Append(" outCoupon conflits with coupon in database.");
                }
            }
            if (error)
            {
                throw new Exception(buf.ToString());
            }

            status = ModifyProcessAgent(originalGuid, service, extra);
            SetIdentInCouponID(service.agentGuid, inCoupon.couponId);
            SetIdentOutCouponID(service.agentGuid, outCoupon.couponId);
            return status;
        }

        

        public int RemoveDomainCredentials(string domainGuid, string agentGuid){
            int status = -1;
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "RetrieveTickets" stored procedure
                SqlCommand cmd = new SqlCommand("RemoveDomainCredentials", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter domainParam = cmd.Parameters.Add("@domainGuid", SqlDbType.VarChar,50);
                domainParam.Value = domainGuid;
                SqlParameter agentParam = cmd.Parameters.Add("@agentGuid", SqlDbType.VarChar,50);
                agentParam.Value = agentGuid;
                // execute the command
                connection.Open();
                Object obj = cmd.ExecuteScalar();
                if (obj != null)
                    status = Convert.ToInt32(obj);
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return status;
        }

        public virtual int ModifyProcessAgent(string originalGuid, ProcessAgent agent, string extra)
        {
            int status = 0;
            int id = GetProcessAgentID(originalGuid);
            if (id > 1)
            {
                status = UpdateProcessAgent(id, agent.agentGuid, agent.agentName, agent.type, agent.domainGuid,
                    agent.codeBaseUrl, agent.webServiceUrl);
            }

            return status;
        }

        public string GetProcessAgentName(int id)
        {
            string name = null;
            // create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try
			{
				// create sql command
				// command executes the "RetrieveTickets" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentName", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				
                SqlParameter idParam = cmd.Parameters.Add("@agentID", SqlDbType.Int);
            idParam.Value = id;
				// execute the command
                connection.Open();
				Object obj = cmd.ExecuteScalar();
                if(obj != null)
                name = obj.ToString();
			}
			catch( SqlException)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
			return name;
		}

        public string GetProcessAgentNameWithType(int id)
        {
            string name = null;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "RetrieveTickets" stored procedure
                SqlCommand cmd = new SqlCommand("GetProcessAgentNameWithType", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter idParam = cmd.Parameters.Add("@agentID", SqlDbType.Int);
                idParam.Value = id;
                // execute the command

                // execute the command
                SqlDataReader dataReader = null;
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    name = dataReader.GetString(0) +": " + dataReader.GetString(1);
                }			
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return name;
        }

        /// <summary>
        /// Return all processAgent tags, agentName is the tag, local processAgent ID is the id.
        /// </summary>
        /// <returns></returns>
		public IntTag[] GetProcessAgentTags()
		{
			ArrayList list = new ArrayList();
			IntTag tag = new IntTag();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try
			{
				// create sql command
				// command executes the "RetrieveTickets" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentTags", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				
				// execute the command
				SqlDataReader dataReader = null;
                connection.Open();
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read()) 
				{
					tag = new IntTag();
					tag.id = dataReader.GetInt32(0);
					tag.tag = dataReader.GetString(1);
					list.Add(tag);
				}			
			}
			catch( SqlException)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
			IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
			return info;
		}


        /// <summary>
        /// Return all processAgent tags for the specified domain, agentName is the tag.
        /// </summary>
        /// <returns></returns>
        public IntTag[] GetProcessAgentTagsForDomain(string domainGuid)
        {
            ArrayList list = new ArrayList();
            IntTag tag = new IntTag();
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "RetrieveTickets" stored procedure
                SqlCommand cmd = new SqlCommand("GetDomainProcessAgentTags", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 50);
                domainParam.Value = domainGuid;
                // execute the command
                SqlDataReader dataReader = null;
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);
                    list.Add(tag);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
            return info;
        }
        /// <summary>
        /// Return all ProcessAgent tags with Type appended to the agent name.
        /// </summary>
        /// <returns></returns>
		public IntTag[] GetProcessAgentTagsWithType()
		{
			ArrayList list = new ArrayList();
			IntTag tag = new IntTag();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
           
			try
			{
				// create sql command
				// command executes the "RetrieveTickets" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentTagsWithType", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				
				// execute the command
				SqlDataReader dataReader = null;
                connection.Open();
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read()) 
				{
					tag = new IntTag();
					tag.id = dataReader.GetInt32(0);
					tag.tag = dataReader.GetString(1) + ": " + dataReader.GetString(2);
					list.Add(tag);
				}			
			}
			catch( SqlException)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
			IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
			return info;
		}

        /// <summary>
        /// Return all ProcessAgent tags from the specified domain with Type
        /// </summary>
        /// <returns></returns>
        public IntTag[] GetProcessAgentTagsWithTypeForDomain(string domainGuid)
        {
            ArrayList list = new ArrayList();
            IntTag tag = new IntTag();
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            try
            {
                // create sql command
                // command executes the "RetrieveTickets" stored procedure
                SqlCommand cmd = new SqlCommand("GetDomainProcessAgentTagsWithType", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 50);
                domainParam.Value = domainGuid;

                // execute the command
                SqlDataReader dataReader = null;
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1) + ": " + dataReader.GetString(2);
                    list.Add(tag);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
            return info;
        }

        /// <summary>
        /// Return the ProcessAgent Tag, with type, for the specified processAgentID.
        /// </summary>
        /// <param name="paIds"></param>
        /// <returns></returns>
        public IntTag GetProcessAgentTag(string guid)
        {

            IntTag tag = null;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            SqlCommand cmd = new SqlCommand("GetProcessAgentTagByGuid", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter processAgentIdParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            processAgentIdParam.Value = guid;
            SqlDataReader dataReader = null;
            try
            {
                // execute the command
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);

                }
                dataReader.Close();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return tag;
        }

        /// <summary>
        /// Return the ProcessAgent Tag, with type, for the specified processAgentID.
        /// </summary>
        /// <param name="paIds"></param>
        /// <returns></returns>
        public IntTag GetProcessAgentTagWithType(string guid)
        {

            IntTag tag = null;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            SqlCommand cmd = new SqlCommand("GetProcessAgentTagsWithTypeByGuid", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter processAgentIdParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
            processAgentIdParam.Value = guid;
            SqlDataReader dataReader = null;
            try
            {

                

                // execute the command
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1) + ": " + dataReader.GetString(2);

                }
                dataReader.Close();
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return tag;
        }

        /// <summary>
        /// Return the ProcessAgent Tag, with type, for the specified processAgentID.
        /// </summary>
        /// <param name="paIds"></param>
        /// <returns></returns>
        public IntTag GetProcessAgentTagWithType(int paId)
        {
         
            IntTag tag = null;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            SqlCommand cmd = new SqlCommand("GetProcessAgentTagsWithTypeById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter processAgentIdParam = cmd.Parameters.Add("@agentID", SqlDbType.VarChar, 50);

            SqlDataReader dataReader = null;
            try
            {
             
                    processAgentIdParam.Value = paId;

                    // execute the command
                    connection.Open();
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        tag = new IntTag();
                        tag.id = dataReader.GetInt32(0);
                        tag.tag = dataReader.GetString(1) + ": " + dataReader.GetString(2);
                       
                    }
                    dataReader.Close();   
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return tag;
        }

        /// <summary>
        /// Return the ProcessAgent Tags, with type, for the specified processAgentIDs.
        /// </summary>
        /// <param name="paIds"></param>
        /// <returns></returns>
        public IntTag[] GetProcessAgentTagsWithType(int []paIds)
        {
            ArrayList list = new ArrayList();
            IntTag tag = new IntTag();
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            SqlCommand cmd = new SqlCommand("GetProcessAgentTagsWithTypeById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter processAgentIdParam = cmd.Parameters.Add("@agentID", SqlDbType.VarChar, 50);
            
            SqlDataReader dataReader = null;
            try
            {
                for (int i = 0; i < paIds.Length; i++)
                {
                    processAgentIdParam.Value = paIds[i];

                    // execute the command
                    connection.Open();
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        tag = new IntTag();
                        tag.id = dataReader.GetInt32(0);
                        tag.tag = dataReader.GetString(1) + ": " + dataReader.GetString(2);
                        list.Add(tag);
                    }
                    dataReader.Close();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
            return info;
        }
        /// <summary>
        /// return tags for all aprocessAgents of typeID
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
		public IntTag[] GetProcessAgentTagsByType(int typeID)
		{
			ArrayList list = new ArrayList();
			IntTag tag = new IntTag();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try
			{
				// create sql command
				// command executes the "RetrieveTickets" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentTagsByTypeID", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter typeParam = cmd.Parameters.Add("@typeID", SqlDbType.Int);
				typeParam.Value = typeID;
				// execute the command
				SqlDataReader dataReader = null;
                connection.Open();
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read()) 
				{
					tag = new IntTag();
					tag.id = dataReader.GetInt32(0);
					tag.tag = dataReader.GetString(1);
					list.Add(tag);
				}			
			}
			catch( SqlException)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
			IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
			return info;
		}

        /// <summary>
        /// Returns all ProcessAgent tags for the specified typeIDs.
        /// </summary>
        /// <param name="typeIDs"></param>
        /// <returns></returns>
		public IntTag[] GetProcessAgentTagsByType(int[] typeIDs)
		{
			ArrayList list = new ArrayList();
			IntTag tag = new IntTag();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try
			{
				// create sql command
				// command executes the "RetrieveTickets" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentTagsByTypeID", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter typeParam = cmd.Parameters.Add("@typeID", SqlDbType.Int);
				
				// execute the command
				SqlDataReader dataReader = null;
                connection.Open();
				foreach( int i in typeIDs)
				{
					typeParam.Value = i;
					dataReader = cmd.ExecuteReader();
					while (dataReader.Read()) 
					{
						tag = new IntTag();
						tag.id = dataReader.GetInt32(0);
						tag.tag = dataReader.GetString(1);
						list.Add(tag);
					}
				}
			}
			catch( SqlException)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
			IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
			return info;
		}
        /// <summary>
        /// Return all ProcessAgent tags for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
		public IntTag[] GetProcessAgentTagsByType(string type)
		{
			ArrayList list = new ArrayList();
			IntTag tag = new IntTag();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try
			{
				// create sql command
                // command executes the "GetProcessAgentTagsByType" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentTagsByType", connection);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar,100);
				typeParam.Value = type;
				
				// execute the command
				SqlDataReader dataReader = null;
                connection.Open();
				dataReader = cmd.ExecuteReader();
				while (dataReader.Read()) 
				{
					tag = new IntTag();
					tag.id = dataReader.GetInt32(0);
					tag.tag = dataReader.GetString(1);
					list.Add(tag);
				}			
			}
			catch( SqlException)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
			IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
			return info;
		}
        /// <summary>
        /// Return all ProcessAgent tags for the specified type and domain.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IntTag[] GetProcessAgentTagsByType(string type, string domainGuid)
        {
            ArrayList list = new ArrayList();
            IntTag tag = new IntTag();
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "GetProcessAgentTagsByType" stored procedure
                SqlCommand cmd = new SqlCommand("GetDomainProcessAgentTagsByType", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
                typeParam.Value = type;
                SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 50);
                domainParam.Value = domainGuid;
                // execute the command
                SqlDataReader dataReader = null;
                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    tag = new IntTag();
                    tag.id = dataReader.GetInt32(0);
                    tag.tag = dataReader.GetString(1);
                    list.Add(tag);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            IntTag[] info = (IntTag[])list.ToArray(tag.GetType());
            return info;
        }
   
		/// <summary>
		/// Check that an agent type exits in the database.
		/// </summary>
		/// <param name="processAgentType"></param>
		/// <returns><code>true</code> if processAgentType is a name of a static process agent type in the database</returns>
		bool ProcessAgentTypeExists(string processAgentType)
		{
            int typeID = -1;

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveProcessAgentTypeID" stored procedure
			SqlCommand cmd = new SqlCommand("RetrieveProcessAgentTypeID", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameter
			// 1. type
			SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
			typeParam.Value = processAgentType;

			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    // read ticket id
                    typeID = Convert.ToInt32(dataReader.GetValue(0));
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

			if (typeID == -1)
                return false;

			return true;
		}

       
        /// <summary>
        /// Copies the standard agent select results into a ProcessAgent object
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        protected ProcessAgent readProcessAgent(SqlDataReader dataReader)
        {
            ProcessAgent agent = new ProcessAgent();
            long id = dataReader.GetInt32(0);
            agent.agentGuid = dataReader.GetString(1);
            agent.agentName = dataReader.GetString(2);
            // get process agent type
            if (!DBNull.Value.Equals(dataReader.GetValue(3)))
                agent.type = dataReader.GetString(3);
            // get the description
            if (!DBNull.Value.Equals(dataReader.GetValue(4)))
                agent.domainGuid = dataReader.GetString(4);
            // Get CodeBase
            if (!DBNull.Value.Equals(dataReader.GetValue(5)))
                agent.codeBaseUrl = dataReader.GetString(5);
            // get Webservice_URL
            if (!DBNull.Value.Equals(dataReader.GetValue(6)))
                agent.webServiceUrl = dataReader.GetString(6);
            
            return agent;
        }

		/// <summary>
		/// Retrieve all process agent records from the database
		/// </summary>
		/// <param name="guid">The Guid id of the static process agent</param>
		/// <returns>Object that represents the retrieved static process agent</returns>
		public ProcessAgent[] GetProcessAgents()
		{
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveProcessAgentDescriptor" stored procedure
			SqlCommand cmd = new SqlCommand("GetProcessAgents", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// execute the command
			SqlDataReader dataReader = null;
			try 
			{
                connection.Open();
				dataReader = cmd.ExecuteReader();
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}

			ArrayList list = new ArrayList();
			ProcessAgent agent = null;
			while (dataReader.Read()) 
			{
				agent = readProcessAgent(dataReader);
				list.Add(agent);
			}
															  
			connection.Close();
			ProcessAgent dummy = new ProcessAgent();
			ProcessAgent[] agents = (ProcessAgent[])list.ToArray(dummy.GetType());
			return agents;
		}

	/// <summary>
	/// Retrieve all the process agents of a given type from the database
	/// </summary>
	/// <returns>Array of objects that represent the retrieved static process agents</returns>
	public ProcessAgent[] GetProcessAgentsByType(string agentType)
    {
        ArrayList list = new ArrayList();
        ProcessAgent agent = null;

	    // create sql connection
	    SqlConnection connection = FactoryDB.GetConnection();

	    // create sql command
	    // command executes the "RetrieveProcessAgentDescriptors" stored procedure
	    SqlCommand cmd = new SqlCommand("GetProcessAgentsByType", connection);
	    cmd.CommandType = CommandType.StoredProcedure;	

	    SqlParameter typeParam = cmd.Parameters.Add("@agentType", SqlDbType.VarChar, 100);
	    typeParam.Value = agentType;

	    // execute the command
	    SqlDataReader dataReader = null;
        try
        {
            connection.Open();
            dataReader = cmd.ExecuteReader();

            // add the agent to the list
            while (dataReader.Read())
            {
                agent = readProcessAgent(dataReader);
                list.Add(agent);
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

	    ProcessAgent dummy = new ProcessAgent();
	    ProcessAgent[] agents = (ProcessAgent[])list.ToArray(dummy.GetType());
	    return agents;

    }

		/// <summary>
		/// Retrieve a process agent record from the database by it's local ID
		/// </summary>
		/// <param name="guid">The Guid id of the static process agent</param>
		/// <returns>Object that represents the retrieved static process agent</returns>
		public ProcessAgent GetProcessAgent(int id)
		{
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

            ProcessAgent agent = GetProcessAgent(connection, id);

            connection.Close();

			return agent;
		}

        /// <summary>
        /// Retrieve a process agent record from the database by it's local ID
        /// </summary>
        public ProcessAgent GetProcessAgent(SqlConnection connection, int id)
        {
            ProcessAgent agent = null;

            // create sql command
            // command executes the "RetrieveProcessAgentDescriptor" stored procedure
            SqlCommand cmd = new SqlCommand("GetProcessAgentByID", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter idParam = cmd.Parameters.Add("@ID", SqlDbType.Int);
            idParam.Value = id;

            // execute the command
            SqlDataReader dataReader = null;
            try{
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    agent = readProcessAgent(dataReader);
                }
                dataReader.Close();
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

            return agent;
        }


		/// <summary>
		/// Retrieve a process agent record from the database, by it's Guid
		/// </summary>
		/// <param name="guid">The Guid id of the static process agent</param>
		/// <returns>Object that represents the retrieved static process agent</returns>
		public ProcessAgent GetProcessAgent(string guid)
		{
            ProcessAgent agent = null;

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveProcessAgentDescriptor" stored procedure
			SqlCommand cmd = new SqlCommand("GetProcessAgent", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			SqlParameter guidParam = cmd.Parameters.Add("@agentGUID", SqlDbType.VarChar, 50);
			guidParam.Value = guid.ToString();

			// execute the command
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    agent = readProcessAgent(dataReader);
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
			return agent;
		}

        public int InsertProcessAgent(ProcessAgent paIdent, Coupon inCoupon, Coupon outCoupon)
        {
            return InsertProcessAgent(paIdent.agentGuid, paIdent.agentName, paIdent.type, paIdent.domainGuid,
                    paIdent.codeBaseUrl, paIdent.webServiceUrl, inCoupon, outCoupon);
        }
		/// <summary>
		/// The process agent will not be created if the parameter processAgentType is not the valid name 
		/// of a process agent type
		/// </summary>
		/// <param name="guid"></param>
        /// <param name="name"></param>
		/// <param name="processAgentType"></param>
		/// <param name="issuerGuid"></param>
		/// <param name="codeBaseUrl"></param>
		/// <param name="webServiceUrl"></param>
		/// <param name="inCoupon">Optional, null if from outside the current domain</param>
        /// <param name="outCoupon">Optional, null if from outside the current domain</param>
		/// <returns>The internal database ID of the static process agent created, or null if it cannot be created</returns>
		public int InsertProcessAgent(string guid, string name,
			string processAgentType, string domainGuid,
             string codeBaseUrl, string webServiceUrl,
             Coupon inCoupon, Coupon outCoupon) 
		{
			int id = -1;
            SqlConnection connection = null;

            try
            {
                connection = FactoryDB.GetConnection();
                connection.Open();
                id = InsertProcessAgent(connection, guid, name, processAgentType, domainGuid, codeBaseUrl, webServiceUrl,
                    inCoupon, outCoupon);
            }

            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

			return id;
        }


		/// <summary>
		/// Inserts the database record for a ProcessAgent, the Identity coupons should already be in the database,
        /// except in the case when the original service broker record is being created.
		/// The process agent will not be created if the parameter processAgentType is not the valid name 
		/// of a process agent type.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="guid"></param>
        /// <param name="name"></param>
		/// <param name="processAgentType"></param>
		/// <param name="issuerGuid"></param>
        /// <param name="codeBaseUrl"></param>
        /// <param name="webServiceUrl"></param>
        /// <param name="inCoupon"></param>
        /// <param name="outCoupon"></param>
		/// <returns>An int status flag, negative numbers are errors</returns>
		/// <exception>Throws SqlException</exception>
		protected int InsertProcessAgent(SqlConnection connection, string guid, string name, string processAgentType, 
            string domainGuid, string codeBaseUrl, string webServiceUrl,
            Coupon inCoupon, Coupon outCoupon)
		{
			int typeID = -1;
			int itemID = -1;
            string issuerGuid = null;

            if ((inCoupon != null && outCoupon != null))
            {
                issuerGuid = outCoupon.issuerGuid;
            }
            else if (outCoupon != null)
            {
                issuerGuid = outCoupon.issuerGuid;
            }
            else if (inCoupon != null)
            {
                issuerGuid = inCoupon.issuerGuid;
            }


			try 
			{
			// command executes the "RetrieveProcessAgentTypeID" stored procedure
			SqlCommand cmd = new SqlCommand("GetProcessAgentTypeID", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameter
			// 1. type
			SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
			typeParam.Value = processAgentType;

			// execute the command
			typeID  = Convert.ToInt32(cmd.ExecuteScalar());
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}
			if(typeID >= 0){
				try
				{
                    // if inCoupon is null or outCoupon is null, then the initial SB record is being created
                    if (inCoupon != null)
                        InsertCoupon(connection, inCoupon);
                    if (outCoupon != null)
                        InsertCoupon(connection, outCoupon);

					// create sql command
					// command executes the "InsertProcessAgent" stored procedure
					SqlCommand cmd = new SqlCommand("InsertProcessAgent", connection);
					cmd.CommandType = CommandType.StoredProcedure;

					// populate parameters
					// 2. processAgentType
					SqlParameter processAgentTypeParam = cmd.Parameters.Add("@processAgentType", SqlDbType.VarChar,100);
					processAgentTypeParam.Value = processAgentType;
					// 1. guid
					SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
					guidParam.Value = guid;
					SqlParameter nameParam = cmd.Parameters.Add("@name", SqlDbType.VarChar, 100);
                    if (name != null)
                        nameParam.Value = name;
                    else
                        nameParam.Value = System.DBNull.Value;

                    SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 50);
                    domainParam.Value = domainGuid;
					// 4. webServiceURL
					SqlParameter webServiceParam = cmd.Parameters.Add("@webServiceURL", SqlDbType.VarChar, 256);
					webServiceParam.Value = webServiceUrl;
					// 5. redirectURL
					SqlParameter applicationURLParam = cmd.Parameters.Add("@codeBaseURL", SqlDbType.VarChar, 256);
                    if (codeBaseUrl != null)
                        applicationURLParam.Value = codeBaseUrl;
                    else
                        applicationURLParam.Value = System.DBNull.Value;
                    // 9. issuerGUID
                    SqlParameter issuerParam = cmd.Parameters.Add("@issuer", SqlDbType.VarChar, 50);
                    if (issuerGuid != null)
                        issuerParam.Value = issuerGuid;
                    else
                        issuerParam.Value = System.DBNull.Value;
                    // if inCoupon is null and outCoupon is null, then the processAgent is from another domain
                    // 10. inCouponID
                    SqlParameter idInParam = cmd.Parameters.Add("@inID", SqlDbType.BigInt);
                    if (inCoupon != null)
                        idInParam.Value = inCoupon.couponId;
                    else
                        idInParam.Value = DBNull.Value;

                    // 11. OutCouponID
                    SqlParameter idOutParam = cmd.Parameters.Add("@outID", SqlDbType.BigInt);
                    if (outCoupon != null)    
                        idOutParam.Value = outCoupon.couponId;
                    else
                        idOutParam.Value = DBNull.Value;

					// execute the command
			    	itemID = Convert.ToInt32(cmd.ExecuteScalar());
				} 
				catch (SqlException e) 
				{
					writeEx(e);
					throw;
				}
			}
			return itemID;
		}
        public int SetProcessAgentRetired(string guid, bool state)
        {
             int status = -1;
            SqlConnection connection = null;

            try
            {
                connection = FactoryDB.GetConnection();
                connection.Open();
                SqlCommand cmd = new SqlCommand("SetProcessAgentRetired", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                  SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                guidParam.Value = guid;
                SqlParameter stateParam = cmd.Parameters.Add("@state", SqlDbType.Bit);
                stateParam.Value =state;

                // execute the command
                status = Convert.ToInt32(cmd.ExecuteScalar());
            }
            

            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return status;
        }

        public int SetSelfState(string guid, bool state)
        {
            int status = -1;
            SqlConnection connection = null;

            try
            {
                connection = FactoryDB.GetConnection();
                
                SqlCommand cmd = new SqlCommand("SetSelfState", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                guidParam.Value = guid;
                SqlParameter stateParam = cmd.Parameters.Add("@state", SqlDbType.Bit);
                stateParam.Value = state;

                // execute the command
                connection.Open();
                status = Convert.ToInt32(cmd.ExecuteScalar());
            }


            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return status;
        }

        /// <summary>
        /// Retrieve a process agent record from the database, by it's Guid
        /// </summary>
        /// <param name="guid">The Guid id of the static process agent</param>
        /// <returns>Object that represents the retrieved static process agent</returns>
        public ProcessAgent GetSelfProcessAgent()
        {
            ProcessAgent agent = null;

            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "RetrieveProcessAgentDescriptor" stored procedure
            SqlCommand cmd = new SqlCommand("ReadSelfProcessAgent", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // execute the command
            SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    agent = readProcessAgent(dataReader);
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
            return agent;
        }
       /// <summary>
       /// A specialized method to create or modify the ProcessAgent's ProcessAgent Record.
       /// This record is only used internally, if an existing record has 'self' set  to '1', 
       /// the record will be updated or a new record will be created.
       /// Both agent and domain guids 
       /// may be the same the same. Registering the domainServer will overwrite the domainGuid.
       /// THis method should not be called after a domainServer has been registed.
       /// </summary>
       /// <param name="guid"></param>
       /// <param name="name"></param>
       /// <param name="processAgentType"></param>
       /// <param name="domainGuid"></param>
       /// <param name="applicationURL"></param>
       /// <param name="webserviceURL"></param>
       /// <returns></returns>
        public int SelfRegisterProcessAgent(string guid, string name, 
            string processAgentType, string domainGuid,
            string applicationURL,  string webserviceURL)
        {
            int id = -1;
            SqlConnection connection = null;

            try
            {
                connection = FactoryDB.GetConnection();
                connection.Open();
                id = SelfRegisterProcessAgent(connection, guid, name, processAgentType,
                    domainGuid, applicationURL, webserviceURL);
            }

            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return id;
        }

        /// <summary>
        /// A specialized method to create or modify the ProcessAgent's ProcessAgent Record.
        /// This record is only used internally, has agent_ID of '1', and both agent and domain guids 
        /// may be the same the same. Registering the domainServer will overwrite the domainGuid.
        /// THis method should not be called after a domainServer has been registed.
        /// </summary>
        public int SelfRegisterProcessAgent(SqlConnection connection, string guid, string name,
            string processAgentType, string domainGuid,string codebaseURL, string webserviceURL)
        {
            int typeID = -1;
            int itemID = -1;

            try
            {
                // Check if the type is valid
                // command executes the "RetrieveProcessAgentTypeID" stored procedure
                SqlCommand cmd = new SqlCommand("GetProcessAgentTypeID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameter
                // 1. type
                SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
                typeParam.Value = processAgentType;

                // execute the command
                typeID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
            if (typeID >= 0)
            {
                try
                {
                    // create sql command
                    // command executes the "WriteSelfProcessAgent" stored procedure
                    SqlCommand cmd = new SqlCommand("WriteSelfProcessAgent", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // populate parameters
                    // 2. processAgentType
                    SqlParameter processAgentTypeParam = cmd.Parameters.Add("@processAgentType", SqlDbType.VarChar, 100);
                    processAgentTypeParam.Value = processAgentType;
                    // 1. guid
                    SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                    guidParam.Value = guid;
                    SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 100);
                    if (domainGuid != null)
                        domainParam.Value = domainGuid;
                    else
                        domainParam.Value = "";
                    SqlParameter nameParam = cmd.Parameters.Add("@name", SqlDbType.VarChar, 100);
                    if (name != null)
                        nameParam.Value = name;
                    else
                        nameParam.Value = "";
                    // 4. webServiceURL
                    SqlParameter webServiceParam = cmd.Parameters.Add("@webServiceURL", SqlDbType.VarChar, 256);
                    webServiceParam.Value = webserviceURL;
                    // 5. redirectURL
                    SqlParameter applicationURLParam = cmd.Parameters.Add("@codeBaseURL", SqlDbType.VarChar, 256);
                    if (codebaseURL != null)
                        applicationURLParam.Value = codebaseURL;
                    else
                        applicationURLParam.Value = "";
                  
                    // execute the command
                    itemID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (SqlException e)
                {
                    writeEx(e);
                    throw;
                }
            }
            RefreshServiceAgent();
            return itemID;
        }

		public int UpdateProcessAgent(string guid,string name, string type,
			string domainGuid,string codeBaseURL, string serviceURL)
		{
			int status = 0;
			SqlConnection connection = FactoryDB.GetConnection();
			try 
			{
				SqlCommand cmd = new SqlCommand("UpdateProcessAgent", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				// populate parameters
				SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                guidParam.Value = guid;
				SqlParameter nameParam = cmd.Parameters.Add("@name", SqlDbType.VarChar, 100);
				nameParam.Value = name;
				SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
				typeParam.Value = type;
                SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 50);
                domainParam.Value = domainGuid;
                SqlParameter appParam = cmd.Parameters.Add("@codeBaseUrl", SqlDbType.VarChar, 256);
                appParam.Value = codeBaseURL;
				SqlParameter serviceParam = cmd.Parameters.Add("@webServiceUrl", SqlDbType.VarChar, 256);
				serviceParam.Value = serviceURL;



                connection.Open();
				status = Convert.ToInt32(cmd.ExecuteScalar());
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

        public int UpdateProcessAgent(int id, string guid, string name, string type,
            string domainGuid, string codeBaseURL, string serviceURL)
        {
            int status = 0;
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UpdateProcessAgentByID", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.VarChar, 50);
                idParam.Value = id;
                SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
                guidParam.Value = guid;
                SqlParameter nameParam = cmd.Parameters.Add("@name", SqlDbType.VarChar, 100);
                nameParam.Value = name;
                SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
                typeParam.Value = type;
                SqlParameter domainParam = cmd.Parameters.Add("@domain", SqlDbType.VarChar, 50);
                domainParam.Value = domainGuid;
                SqlParameter appParam = cmd.Parameters.Add("@codeBaseUrl", SqlDbType.VarChar, 256);
                appParam.Value = codeBaseURL;
                SqlParameter serviceParam = cmd.Parameters.Add("@webServiceUrl", SqlDbType.VarChar, 256);
                serviceParam.Value = serviceURL;



                connection.Open();
                status = Convert.ToInt32(cmd.ExecuteScalar());
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


		/// <summary>
		/// Check that a processagent type exisits in the database.
		/// </summary>
		/// <param name="processAgentType"></param>
		/// <returns><code>true</code> if processAgentType is a name of a static process agent type in the database</returns>
		protected bool ProcessAgentTypeExists(SqlConnection connection, string processAgentType)
		{
			bool status = false;
			// create sql command
			// command executes the "RetrieveProcessAgentTypeID" stored procedure
			SqlCommand cmd = new SqlCommand("ProcessAgentTypeExists", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameter
			// 1. type
			SqlParameter typeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
			typeParam.Value = processAgentType;

			// execute the command
		
			try 
			{
				status = Convert.ToBoolean(cmd.ExecuteScalar());
			} 
			catch (SqlException e) 
			{
				writeEx(e);
				throw;
			}
			return status;
		}

		/// <summary>
		/// Retrieve a agent's local ID given a string guid.
		/// </summary>
		/// <param name="guidStr"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public int GetProcessAgentID(string guidStr)
		{
			int id = -1;
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try 
			{
				// create sql command
				// command executes the "RetrieveCoupon" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentID", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				// populate parameters
				SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
				guidParam.Value = guidStr;

				// read the result
				//SqlDataReader dataReader = null;
                connection.Open();
				id = Convert.ToInt32(cmd.ExecuteScalar());
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
			return id;
		}

		/// <summary>
		/// Retrieve a agents serviceURL given a string guid.
		/// </summary>
		/// <param name="guidStr"></param>
		/// <returns>Web Service URL, or null if the agent can not be found</returns>
		public string GetServiceURL(string guidStr)
		{
			string url = null;
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetServiceURL", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
			guidParam.Value = guidStr;

			// read the result
			//SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                url = Convert.ToString(cmd.ExecuteScalar());
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
			return url.Trim();
		}

		
		/// <summary>
		/// Retrieve a agents serviceURL given the local ID.
		/// </summary>
		/// <param name="id"></param>
        /// <returns>Web Service URL, or null if the agent can not be found</returns>
		public string GetServiceURL(int id)
		{
			string url = null;
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetServiceURLByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@agent_id", SqlDbType.Int);
			idParam.Value = id;

			// read the result
			//SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                url = Convert.ToString(cmd.ExecuteScalar());
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

			return url.Trim();
		}

		protected ProcessAgentInfo readAgentInfo(SqlDataReader dataReader)
		{
            long idIn = -1;
            long idOut = -1;
            string passIn = null;
            string passOut = null;
			ProcessAgentInfo info = new ProcessAgentInfo();
			info.agentId = dataReader.GetInt32(0);
			info.agentGuid = dataReader.GetString(1);
            if (!DBNull.Value.Equals(dataReader.GetValue(2)))
			    info.agentName = dataReader.GetString(2);
			info.agentType = (ProcessAgentType.AgentType) dataReader.GetInt32(3);
            if (!DBNull.Value.Equals(dataReader.GetValue(4)))
                info.codeBaseUrl = dataReader.GetString(4);
			info.webServiceUrl = dataReader.GetString(5);
            if (!DBNull.Value.Equals(dataReader.GetValue(6)))
                info.domainGuid = dataReader.GetString(6);
             if (!DBNull.Value.Equals(dataReader.GetValue(7)))
			    info.issuerGuid = dataReader.GetString(7);
            if (!DBNull.Value.Equals(dataReader.GetValue(8)))
			    idIn = dataReader.GetInt64(8);
            if (!DBNull.Value.Equals(dataReader.GetValue(9)))
			    passIn = dataReader.GetString(9);
            if (!DBNull.Value.Equals(dataReader.GetValue(10)))
			    idOut =dataReader.GetInt64(10); 
            if (!DBNull.Value.Equals(dataReader.GetValue(11)))
			    passOut = dataReader.GetString(11);
            if (!DBNull.Value.Equals(dataReader.GetValue(12)))
                info.retired = dataReader.GetBoolean(12);
            if (info.issuerGuid != null)
            {
                if (idIn > 0)
                {
                    Coupon couponIn = new Coupon();
                    couponIn.couponId = idIn;
                    couponIn.issuerGuid = info.issuerGuid;
                    couponIn.passkey = passIn;
                    info.identIn = couponIn;
                }
                else
                {
                    info.identIn = null;
                }
                if (idOut > 0)
                {

                    Coupon couponOut = new Coupon();
                    couponOut.couponId = idOut;
                    couponOut.issuerGuid = info.issuerGuid;
                    couponOut.passkey = passOut;
                    info.identOut = couponOut;
                }
                else
                {
                    info.identOut = null;
                }
            }
			return info;
		}

		/// <summary>
		/// Retrieve a agents information given an id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>agentInfo, or null if the agent</returns>
		public ProcessAgentInfo GetProcessAgentInfo(int id)
		{
            ProcessAgentInfo info = null;

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetProcessAgentInfoById", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
			idParam.Value = id;

			// read the result
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                // from the dataReader
                while (dataReader.Read())
                {
                    info = readAgentInfo(dataReader);
                }

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

			return info;
		}


		/// <summary>
		/// Retrieve a agentInfo given a string guid.
		/// </summary>
		/// <param name="guidStr"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public ProcessAgentInfo GetProcessAgentInfo(string guidStr)
		{
            ProcessAgentInfo info = null;

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetProcessAgentInfo", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter guidParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar, 50);
			guidParam.Value = guidStr;

			// read the result
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                // from the dataReader
                while (dataReader.Read())
                {
                    info = readAgentInfo(dataReader);
                }
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

			return info;
		}

        /// <summary>
        /// Retrieve a agentInfo given a string guid.
        /// </summary>
        /// <param name="guidStr"></param>
        /// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
        public ProcessAgentInfo GetProcessAgentInfoByInCoupon(long couponID, string issuerGuid)
        {
            ProcessAgentInfo info = null;

            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            // command executes the "RetrieveCoupon" stored procedure
            SqlCommand cmd = new SqlCommand("GetProcessAgentInfoByInCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            idParam.Value = couponID;
            SqlParameter guidParam = cmd.Parameters.Add("@issuer", SqlDbType.VarChar, 50);
            guidParam.Value = issuerGuid;

            // read the result
            SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                // from the dataReader
                while (dataReader.Read())
                {
                    info = readAgentInfo(dataReader);
                }
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

            return info;
        }

		/// <summary>
		/// Retrieve all agentInfos.
		/// </summary>
		/// <param name="guidStr"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public ProcessAgentInfo[] GetProcessAgentInfos()
		{	
			ArrayList list = new ArrayList();		
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try 
			{
				// create sql command
				// command executes the "RetrieveCoupon" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentsInfo", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				// read the result
				SqlDataReader dataReader = null;
                connection.Open();
				dataReader = cmd.ExecuteReader();
			

				// from the dataReader
				ProcessAgentInfo info = null;
				while (dataReader.Read()) 
				{
					info = readAgentInfo(dataReader);
					list.Add(info);
				}
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
			ProcessAgentInfo dummy = new ProcessAgentInfo();
			ProcessAgentInfo[] infos = (ProcessAgentInfo[])list.ToArray(dummy.GetType());
			return infos;
		}



		/// <summary>
		/// Retrieve agentInfos given a string agent type.
		/// </summary>
		/// <param name="type">the type of agent</param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public ProcessAgentInfo[] GetProcessAgentInfos(string type)
		{
			ArrayList list = new ArrayList();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try 
			{
				// create sql command
				// command executes the "RetrieveCoupon" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentInfoByType", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				// populate parameters
				SqlParameter typeParam = cmd.Parameters.Add("@agentType", SqlDbType.VarChar, 100);
				typeParam.Value = type;

				// read the result
				SqlDataReader dataReader = null;
                connection.Open();
				dataReader = cmd.ExecuteReader();
			
				// from the dataReader
				
				ProcessAgentInfo info = null;
				while (dataReader.Read()) 
				{
					info = readAgentInfo(dataReader);
					list.Add(info);
				}
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

			ProcessAgentInfo dummy = new ProcessAgentInfo();
			ProcessAgentInfo[] infos = (ProcessAgentInfo[])list.ToArray(dummy.GetType());
			return infos;
		}

		/// <summary>
		/// Retrieve agentInfos given an array of agent IDs.
		/// </summary>
		/// <param name="IDs"></param>
		/// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
		public ProcessAgentInfo[] GetProcessAgentInfos(int[] ids)
		{
			ProcessAgentInfo[] agents = new ProcessAgentInfo[ids.Length];

			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetProcessAgentInfoByID", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@id",SqlDbType.BigInt);

			// read the result
			SqlDataReader dataReader = null;
			try 
			{
                connection.Open();
				for(int i=0;i<ids.Length;i++)
				{
					idParam.Value = ids[i];
					dataReader = cmd.ExecuteReader();
		
					// from the dataReader
					while (dataReader.Read()) 
					{
						agents[i] = readAgentInfo(dataReader);
					}

                    dataReader.Close();
				}
			}
			catch(SqlException e)
			{
				Console.WriteLine(e);
				throw;
			}
			finally
			{
				// close the sql connection
				connection.Close();
			}
			return agents;
		}

        /// <summary>
        /// Retrieve agentInfos given an array of agent IDs.
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns>Retrieved Coupon, or null if the ticket cannot be found</returns>
        public ProcessAgentInfo[] GetProcessAgentInfosForDomain(string domainGuid)
        {
            List<ProcessAgentInfo> agents = new List<ProcessAgentInfo>(); ;

            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();

            // create sql command
            SqlCommand cmd = new SqlCommand("GetProcessAgentInfosForDomain", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);

            // read the result
            SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                
                    dataReader = cmd.ExecuteReader();

                    // from the dataReader
                    while (dataReader.Read())
                    {
                        agents.Add(readAgentInfo(dataReader));
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
            return agents.ToArray();
        }

        public ProcessAgentInfo GetServiceBrokerInfo()
        {
            // Should use a better model to make sure we have the correct SB
            ProcessAgentInfo[] infos = GetProcessAgentInfos(ProcessAgentType.SERVICE_BROKER);
            foreach(ProcessAgentInfo info in infos){
                if (!info.retired)
                {
                    return info;
                }
            }
            return null;
        }


		/// <summary>
		/// Retrieve an array of processAgent IDs given an agent typeID.
		/// </summary>
		/// <param name="IDs"></param>
		/// <returns>array of IDs</returns>
		public int[] GetProcessAgentIDsByType(int typeID)
		{	
			List<int> list = new List<int>();
			// create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
			try 
			{
				// create sql command
				// command executes the "RetrieveCoupon" stored procedure
				SqlCommand cmd = new SqlCommand("GetProcessAgentIdsByTypeID", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				// populate parameters
				SqlParameter idParam = cmd.Parameters.Add("@typeid",SqlDbType.Int);
				idParam.Value = typeID;
				// read the result
				SqlDataReader dataReader = null;
                connection.Open();		
				dataReader = cmd.ExecuteReader();
		
				// from the dataReader
				while (dataReader.Read()) 
				{
					list.Add(dataReader.GetInt32(0));
				}
			}
			catch(SqlException e)
			{
				Console.WriteLine(e);
				throw;
			}
			finally
			{
				// close the sql connection
				connection.Close();
			}
			return list.ToArray();
		}
        /// <summary>
        /// Returns all ProcessAgent tags for the specified typeIDs.
        /// </summary>
        /// <param name="typeIDs"></param>
        /// <returns></returns>
        public int[] GetProcessAgentIDsByType(int[] typeIDs)
        {
            List<int> ids = new List<int>();
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "RetrieveTickets" stored procedure
                SqlCommand cmd = new SqlCommand("GetProcessAgentIdsByTypeID", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter typeParam = cmd.Parameters.Add("@typeID", SqlDbType.Int);

                // execute the command
                SqlDataReader dataReader = null;
                connection.Open();
                foreach (int i in typeIDs)
                {
                    typeParam.Value = i;
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ids.Add(dataReader.GetInt32(0));
                    }
                    dataReader.Close();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return ids.ToArray(); ;
        }
/*
        /// <summary>
        /// Modifies the information related to the specified service the service's Guid must exist and the typ of service may not be modified,
        /// in and out coupons may be changed.
        /// <param name="service"></param>
        /// <param name="inIdentCoupon"></param>
        /// <param name="outIdentCoupon"></param>
        /// <returns>the resulting modified domain credentials if the agent is in the local processAgent table, or null</returns>
        public int ModifyLocalDomainCredentials(string originalGuid, ProcessAgent agent, Coupon inCoupon, Coupon outCoupon)
        {
            int status = 0;
            try
            {
                ProcessAgentInfo paInfo = GetProcessAgentInfo(originalGuid);
                if (paInfo == null)
                {
                    Utilities.WriteLog("modifyDomainCredentials: The specified processAgent is unknown.");
                    return status;
                }
                if (paInfo.retired)
                {
                    Utilities.WriteLog("modifyDomainCredentials: The specified processAgent is retired.");
                    throw new Exception("modifyDomainCredentials: The specified processAgent is retired.");
                }
                status = UpdateProcessAgent(paInfo.agentId,agent.agentGuid, agent.agentName, agent.type,
                    agent.domainGuid, agent.codeBaseUrl, agent.webServiceUrl);
                if (paInfo.identIn != inCoupon)
                {
                    if (inCoupon != null)
                    {
                        InsertCoupon(inCoupon);
                        SetIdentInCouponID(agent.agentGuid, inCoupon.couponId);
                    }
                }
                if (paInfo.identOut != outCoupon)
                {
                    if (outCoupon != null)
                    {
                        InsertCoupon(outCoupon);
                        SetIdentOutCouponID(agent.agentGuid, outCoupon.couponId);
                    }
                }
               
            }
            catch (Exception ex)
            {
                throw new Exception("ModifyLocalDomainCredentials: ", ex);
            }
            return status;
        }

*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainGuid"></param>
        /// <param name="serviceGuid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public  int RetireProcessAgent(string domainGuid, string serviceGuid, bool state)
        {
            int status = -1;
            ProcessAgent pa = GetProcessAgent(serviceGuid);
            if (pa != null)
            {
                status = SetProcessAgentRetired(serviceGuid, state);
            }
            else
                status = 0;
            return status;

        }
        public SystemSupport RetrieveSystemSupport(int id)
        {
            SystemSupport  ss = null;
            // create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetSystemSupportById", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
			idParam.Value = id;

			// read the result
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                // from the dataReader
                while (dataReader.Read())
                {
                    ss = new SystemSupport();
                    if (!dataReader.IsDBNull(0))
			            ss.contactEmail = dataReader.GetString(0);
                    else ss.contactEmail = null;
                     if (!dataReader.IsDBNull(1))
			            ss.infoUrl = dataReader.GetString(1);
                    else ss.infoUrl = null;
                     if (!dataReader.IsDBNull(2))
			            ss.description = dataReader.GetString(2);
                    else ss.description = null;
                }
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

			return ss;
		}

        
        public SystemSupport RetrieveSystemSupport(string guid)
        {
                      SystemSupport  ss = null;
            // create sql connection
			SqlConnection connection = FactoryDB.GetConnection();

			// create sql command
			// command executes the "RetrieveCoupon" stored procedure
			SqlCommand cmd = new SqlCommand("GetSystemSupport", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			// populate parameters
			SqlParameter idParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);
			idParam.Value = guid;

			// read the result
			SqlDataReader dataReader = null;
            try
            {
                connection.Open();
                dataReader = cmd.ExecuteReader();

                // from the dataReader
                while (dataReader.Read())
                {
                    ss = new SystemSupport();
                    if (!dataReader.IsDBNull(0))
                        ss.contactEmail = dataReader.GetString(0);
                    else ss.contactEmail = null;
                    if (!dataReader.IsDBNull(1))
                        ss.infoUrl = dataReader.GetString(1);
                    else ss.infoUrl = null;
                    if (!dataReader.IsDBNull(2))
                        ss.description = dataReader.GetString(2);
                    else ss.description = null;
                }
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

			return ss;

        }

        public int SaveSystemSupport(int id, string contactEmail, string infoUrl, string description)
        {
            int status = -1;
            // create sql connection
			SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "RetrieveCoupon" stored procedure
                SqlCommand cmd = new SqlCommand("SaveSystemSupportById", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@id", SqlDbType.Int);
                idParam.Value = id;
                
                SqlParameter emailParam = cmd.Parameters.Add("@email", SqlDbType.VarChar, 100);
                if (contactEmail != null)
                    emailParam.Value = contactEmail;
                else emailParam.Value = null;

                SqlParameter infoParam = cmd.Parameters.Add("@info", SqlDbType.VarChar, 256);
                if(infoUrl != null)
                infoParam.Value = infoUrl;
            else emailParam.Value = null;
                
                SqlParameter descParam = cmd.Parameters.Add("@desc", SqlDbType.Text);
                if (description != null)
                descParam.Value = description;
                 else descParam.Value = null;


                // read the result
                SqlDataReader dataReader = null;
                connection.Open();
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                    status = Convert.ToInt32(obj);
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

			return status;

        }

        public int SaveSystemSupport(string guid, string contactEmail, string infoUrl, string description)
        {
            int status = -1;
            // create sql connection
            SqlConnection connection = FactoryDB.GetConnection();
            try
            {
                // create sql command
                // command executes the "RetrieveCoupon" stored procedure
                SqlCommand cmd = new SqlCommand("SaveSystemSupport", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@guid", SqlDbType.VarChar,50);
                idParam.Value = guid;

                SqlParameter emailParam = cmd.Parameters.Add("@email", SqlDbType.VarChar, 100);
                if (contactEmail != null)
                    emailParam.Value = contactEmail;
                else emailParam.Value = null;

                SqlParameter infoParam = cmd.Parameters.Add("@info", SqlDbType.VarChar, 256);
                if (infoUrl != null)
                    infoParam.Value = infoUrl;
                else emailParam.Value = null;

                SqlParameter descParam = cmd.Parameters.Add("@desc", SqlDbType.Text);
                if (description != null)
                    descParam.Value = description;
                else descParam.Value = null;


                // read the result
                SqlDataReader dataReader = null;
                connection.Open();
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                    status = Convert.ToInt32(obj);
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

            return status;

        }
            
 
		protected void writeEx(Exception e)
		{
			Console.WriteLine("DB_CONNECTION: " + connectionStr);
			Console.WriteLine(e.Message + "\n" + e.StackTrace);
		}

	}
 }