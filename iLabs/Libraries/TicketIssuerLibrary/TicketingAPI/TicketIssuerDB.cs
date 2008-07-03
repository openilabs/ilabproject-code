using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes;
using iLabs.Core;
using iLabs.Ticketing;
using iLabs.UtilLib;

//using iLabs.Services;



namespace iLabs.TicketIssuer
{
    /// <summary>
    /// Interface for the DB Layer class
    /// </summary>
    public class TicketIssuerDB : ProcessAgentDB
    {
       

        public TicketIssuerDB()
        {
    
        }



        public string GetIssuerGuid()
        {
            if (serviceAgent != null)
                return serviceAgent.agentGuid;
            else
                return null;
        }

        //public bool AuthenticateProcessAgent(Coupon agentCoupon, string agentGuid)
        //{
        //    // check that the agent record exists
        //    int id = GetProcessAgentID(agentGuid);
        //    if (id == 0)
        //        return false;

        //    // check that the agentCoupon parameter corresponds to the In coupon of the process agent
        //    if (AuthenticateCoupon(agentCoupon) && GetProcessAgentInfo(id).identIn.couponId == agentCoupon.couponId)
        //        return true;

        //    return false;
        //}


        public bool RedeemSessionInfo(Coupon coupon, out int userId, out int groupId, out int clientId)
        {
            bool status = false;
            userId = -1;
            groupId = -1;
            clientId = -1;
            if (String.Compare(coupon.issuerGuid, GetIssuerGuid(), true) == 0)
            {
                status = AuthenticateIssuedCoupon( coupon);
                if (status)
                {
                    XmlQueryDoc xDoc = null;
                    Ticket sessionTicket = RetrieveIssuedTicket(coupon,TicketTypes.REDEEM_SESSION,GetIssuerGuid());
                    if(sessionTicket != null){
                        xDoc = new XmlQueryDoc(sessionTicket.payload);
                        string user = xDoc.Query("RedeemSessionPayload/userID");
                        string group = xDoc.Query("RedeemSessionPayload/groupID");
                        string client = xDoc.Query("RedeemSessionPayload/clientID");
                        if (user != null && user.Length > 0)
                            userId = Convert.ToInt32(user);
                        if (group != null && group.Length > 0)
                            groupId = Convert.ToInt32(group);
                        if (client != null && client.Length > 0)
                            clientId = Convert.ToInt32(client);
                    }
                }
            }
            return status;
        }


        /// <summary>
        /// Verifies that an issued  coupon corresponding to the argument exists, and is not cancelled
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public bool AuthenticateIssuedCoupon(long couponID, string passkey)
        {
            bool status = false;
            SqlConnection connection = CreateConnection();
            try
            {
                status = AuthenticateIssuedCoupon(connection, couponID, passkey);
            }
            finally
            {
                connection.Close();
            }
            return status;
        }

        /// <summary>
        /// Verifies that an issued  coupon corresponding to the argument exists, and is not cancelled
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public bool AuthenticateIssuedCoupon(Coupon coupon)
        {
            bool status = false;
            if (String.Compare(coupon.issuerGuid, GetIssuerGuid(), true) == 0)
            {
                SqlConnection connection = CreateConnection();
                try
                {
                    status = AuthenticateIssuedCoupon(connection, coupon.couponId, coupon.passkey);
                }
                finally
                {
                    connection.Close();
                }
            }
            return status;
        }

        protected bool AuthenticateIssuedCoupon(SqlConnection connection, long couponID, string passkey)
        {
            bool status = false;
            
                SqlCommand cmd = new SqlCommand("AuthenticateIssuedCoupon", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // populate parameters
                SqlParameter idParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
                idParam.Value = couponID;
                SqlParameter passKeyParam = cmd.Parameters.Add("@passKey", SqlDbType.VarChar, 100);
                passKeyParam.Value = passkey;
                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    status = reader.HasRows;
                    reader.Close();
                }
                catch (SqlException e)
                {
                    writeEx(e);
                    throw;
                }
            
            return status;
        }

        protected bool AuthenticateIssuedCoupon(SqlConnection connection, Coupon coupon)
        {
            bool status = false;

            SqlCommand cmd = new SqlCommand("AuthenticateIssuedCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            idParam.Value = coupon.couponId;
            SqlParameter passKeyParam = cmd.Parameters.Add("@passKey", SqlDbType.VarChar, 100);
            passKeyParam.Value = coupon.passkey;
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                status = reader.HasRows;
                reader.Close();
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }

            return status;
        }

       
        public Coupon CreateCoupon()
        {
            SqlConnection connection = CreateConnection();

            try
            {
                Coupon coupon = CreateCoupon(connection);
                return coupon;

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

        public Coupon CreateCoupon(string passcode)
        {
            SqlConnection connection = CreateConnection();

            try
            {
                Coupon coupon = CreateCoupon(connection,passcode);
                return coupon;

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
        /// Create a new coupon in the issued_Coupon Table, the coupon is assigned 
        /// a generated passkey and the service GUID. 
        /// </summary>
        /// <returns>Created Coupon</returns>
        protected Coupon CreateCoupon(SqlConnection connection)
        {
            long couponID = -1;
           

            // create sql command
            // command executes the "CreateCoupon" stored procedure
            SqlCommand cmd = new SqlCommand("CreateCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            string pass = TicketUtil.NewPasskey();
            // populate parameters
            SqlParameter passKeyParam = cmd.Parameters.Add("@passKey", SqlDbType.VarChar, 100);
            passKeyParam.Value = pass;

            //SqlDataReader dataReader = null;
            try
            {
                couponID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
            Coupon coupon = new Coupon(GetIssuerGuid(), couponID, pass);

            return coupon;
        }

        /// <summary>
        /// Create a new coupon in the issued_Coupon Table, the coupon is assigned 
        /// a generated passkey and the service GUID. 
        /// </summary>
        /// <returns>Created Coupon</returns>
        protected Coupon CreateCoupon(SqlConnection connection, string pass)
        {
            long couponID = -1;


            // create sql command
            // command executes the "CreateCoupon" stored procedure
            SqlCommand cmd = new SqlCommand("CreateCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter passKeyParam = cmd.Parameters.Add("@passKey", SqlDbType.VarChar, 100);
            passKeyParam.Value = pass;

            //SqlDataReader dataReader = null;
            try
            {
                couponID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }
            Coupon coupon = new Coupon(GetIssuerGuid(), couponID, pass);

            return coupon;
        }

        public void CancelIssuedCoupon(long couponID)
        {
            bool status = false;
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("CancelIssuedCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            idParam.Value = couponID;

            try
            {
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
        }
        public void DeleteIssuedCoupon(long couponID)
        {
            bool status = false;
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("DeleteIssuedCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            idParam.Value = couponID;

            try
            {
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
        }
         /// <summary>
        /// Checks the IssuedCoupon table and constructs a full Coupon if an 
        /// Issued coupon is found and is not cancelled.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="couponID"></param>
        /// <returns>Coupon if found,  null if cancelled or not found</returns>
        public Coupon GetIssuedCoupon(long couponID)
        {

            Coupon coupon = null;
            SqlConnection connection = CreateConnection();
            try
            {
                
                //connection.Open();
                coupon = GetIssuedCoupon(connection, couponID);
            }
            catch (Exception ex)
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
        /// Checks the IssuedCoupon table and constructs a full Coupon if an 
        /// Issued coupon is found and is not cancelled.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="couponID"></param>
        /// <returns>Coupon if found,  null if cancelled or not found</returns>
        protected Coupon GetIssuedCoupon(SqlConnection connection, long couponID)
        {
            Coupon coupon = null;
            SqlCommand cmd = new SqlCommand("GetIssuedCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            idParam.Value = couponID;

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
            while (dataReader.Read())
            {

                bool cancel = dataReader.GetBoolean(0);
                string pass = dataReader.GetString(1);
                if (!cancel)
                {
                    coupon = new Coupon(GetIssuerGuid(), couponID, pass);
                }
            }

            dataReader.Close();
            return coupon;
        }
        /// <summary>
        /// Counts the number of tickets remaining in the coupon collection, returns -1 on error;
        /// </summary>
        /// <param name="couponID"></param>
        /// <returns>the ticket count, -1 on error</returns>
        public int GetIssuedCouponCollectionCount(long couponID)
        {
            int count = -1;
            SqlConnection connection = CreateConnection();
            try
            {
                count = GetIssuedCouponCollectionCount(connection, couponID);
            }
            catch (Exception e)
            { 
                throw;
            }
            finally{
                connection.Close();
            }
            return count;

        }

        protected int GetIssuedCouponCollectionCount(SqlConnection connection, long couponID)
        {
            int count = -1;
            SqlCommand cmd = new SqlCommand("GetIssuedCollectionCount", connection);
            cmd.CommandType = CommandType.StoredProcedure;


            // populate parameters 
            SqlParameter couponIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIDParam.Value = couponID;
            count = Convert.ToInt32(cmd.ExecuteScalar());
            return count;
        }


        protected Ticket InsertIssuedTicket(SqlConnection connection, long couponID, string redeemerGUID, string sponsorGUID,
            string type, long duration, string payload)
        {

            // creation time in seconds
            DateTime creation = DateTime.UtcNow;

            // command executes the "InsertIssuedTicket" stored procedure
            SqlCommand cmd = new SqlCommand("InsertIssuedTicket", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            ticketTypeParam.Value = type;
            SqlParameter couponIDParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIDParam.Value = couponID;
            SqlParameter redeemerIDParam = cmd.Parameters.Add("@redeemerGUID", SqlDbType.VarChar, 50);
            redeemerIDParam.Value = redeemerGUID.ToString();
            SqlParameter sponsorIDParam = cmd.Parameters.Add("@sponsorGUID", SqlDbType.VarChar, 50);
            sponsorIDParam.Value = sponsorGUID.ToString();
            SqlParameter payloadParam = cmd.Parameters.Add("@payload", SqlDbType.Text);
            payloadParam.Value = payload;

            SqlParameter cancelledParam = cmd.Parameters.Add("@cancelled", SqlDbType.Bit);
            cancelledParam.Value = 0;
            SqlParameter creationTimeParam = cmd.Parameters.Add("@creationTime", SqlDbType.DateTime);
            creationTimeParam.Value = creation;
            SqlParameter durationParam = cmd.Parameters.Add("@duration", SqlDbType.BigInt);
            durationParam.Value = duration;
            long id = -1;
            try
            {
                id = Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (SqlException e)
            {
                writeEx(e);
                throw;
            }

            string issuerGuid = GetIssuerGuid();

            // construct the Ticket object and return it
            return new Ticket (id, type, couponID, issuerGuid, sponsorGUID, redeemerGUID, creation, duration, false, payload);  

        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="type"></param>
        /// <param name="redeemerGuid"></param>
        /// <param name="sponsorGuid"></param>
        /// <param name="expiration"></param>
        /// <param name="payload"></param>
        /// <returns>The added Ticket, or null of the ticket cannot be added</returns>
        public Ticket AddTicket(Coupon coupon, 
            string type, string redeemerGuid, string sponsorGuid, long expiration, string payload)
        {
            Ticket ticket = null;
            // create sql connection
            SqlConnection connection = CreateConnection();

            try
            {
                if (AuthenticateIssuedCoupon(connection, coupon))
                {
                    ticket = InsertIssuedTicket(connection, coupon.couponId, redeemerGuid, sponsorGuid,
                        type, expiration, payload);
                }
            }

            finally
            {
                connection.Close();
            }

            return ticket;
        }
        /// <summary>
        /// Creates a new coupon and adds a new ticket to it.
        /// </summary>
        /// <param name="redeemerInfo"></param>
        /// <param name="ticketType"></param>
        /// <param name="expiration"></param>
        /// <param name="payload"></param>
        /// <returns>Coupon corresponding to the created Ticket</returns>
        public Coupon CreateTicket(string ticketType, string redeemerID, string sponsorID,
             long duration, string payload)
        {
            SqlConnection connection = CreateConnection();

            try
            {
                // create a new coupon
                Coupon newCoupon = CreateCoupon(connection);

                Ticket ticket = InsertIssuedTicket(connection, newCoupon.couponId, redeemerID, sponsorID, ticketType, duration, payload);
                return newCoupon;

            }

            finally
            {
                connection.Close();
            }

        }



        /// <summary>
        /// Mark the ticket as cancelled in the DB
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public bool CancelIssuedTicket(Coupon coupon, Ticket ticket)
        {
            bool status = false;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "CancelTicket" stored procedure
            SqlCommand cmd = new SqlCommand("CancelIssuedTicket", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIdParam.Value = coupon.couponId;
            SqlParameter redeemerIdParam = cmd.Parameters.Add("@redeemer", SqlDbType.VarChar, 50);
            redeemerIdParam.Value = ticket.redeemerGuid.ToString();
            SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            ticketTypeParam.Value = ticket.type;

            // execute the command
            //SqlDataReader dataReader = null;
            try
            {
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

        public void DeleteIssuedTicket(long ticketID)
        {
            bool status = false;
            SqlConnection connection = CreateConnection();
            SqlCommand cmd = new SqlCommand("DeleteIssuedTicket", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate parameters
            SqlParameter idParam = cmd.Parameters.Add("@ticketID", SqlDbType.BigInt);
            idParam.Value = ticketID;

            try
            {
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
        }


        /// <summary>
        /// Retrieve a ticket coupon from the database.
        /// The triple (type,redeemerGuid,sponsorGuid) identifies the ticket.
        /// </summary>
        /// <param name="typeType"></param>
        /// <param name="redeemerGuid"></param>
        /// <param name="sponsorGuid"></param>
        /// <returns>Retrieved Coupon, or null if  the ticket cannot be found</returns>
        public  Coupon []  RetrieveIssuedTicketCoupon(string ticketType, string redeemerGuid,string sponsorGuid)
        {
            List<Coupon> results = new List<Coupon>(); ;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveTicket" stored procedure
            SqlCommand cmd = new SqlCommand("GetIssuedTicketCoupon", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
           
            SqlParameter ticketTypeParam = cmd.Parameters.Add("@type", SqlDbType.VarChar, 100);
            ticketTypeParam.Value = ticketType;
            SqlParameter redeemerParam = cmd.Parameters.Add("@redeemer", SqlDbType.VarChar, 50);
            redeemerParam.Value = redeemerGuid;
            SqlParameter sponsorParam = cmd.Parameters.Add("@sponsor", SqlDbType.VarChar, 50);
            sponsorParam.Value = sponsorGuid;


            // execute the command
            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Coupon coupon = new Coupon();
                  
                    // read id
                    coupon.couponId = dataReader.GetInt64(0);
                    coupon.passkey = dataReader.GetString(1);
                    coupon.issuerGuid = ServiceGuid;
                    results.Add(coupon);
                   
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
            if (results.Count > 0)
            {
                return results.ToArray();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Retrieve a ticket from the database. TicketIssuerDB version
        /// The triple (couponID, redeemerID, type) uniquely identifies the ticket.
        /// If the ticket was issued here try the issuedTickets,
        /// Note the ProcessAgent must store the tickets, a null return is a valid value.
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="redeemerGUID"></param>
        /// <param name="type"></param>
        /// <returns>Retrieved Ticket, or null if  the ticket cannot be found</returns>
        public override Ticket RetrieveTicket(Coupon coupon, string ticketType, string redeemerGUID)
        {
            Ticket result = null;
            if (coupon.issuerGuid == ServiceGuid)
            {
                result = RetrieveIssuedTicket(coupon, ticketType, redeemerGUID);
            }
            else
            {
                base.RetrieveTicket(coupon, ticketType, redeemerGUID);
            }
          
            return result;
        }

        /// <summary>
        /// Retrieve a ticket from the database.
        /// The triple (couponID, redeemerID, type) uniquely identifies the ticket.
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="redeemerID"></param>
        /// <param name="type"></param>
        /// <returns>Retrieved Ticket, or null if  the ticket cannot be found</returns>
        public virtual Ticket RetrieveIssuedTicket(Coupon coupon, string ticketType, string redeemerID)
        {
            Ticket result = null;
            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "RetrieveTicket" stored procedure
            SqlCommand cmd = new SqlCommand("GetIssuedTicketByRedeemer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIdParam.Value = coupon.couponId;
            SqlParameter ticketTypeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            ticketTypeParam.Value = ticketType; 
            SqlParameter redeemerIdParam = cmd.Parameters.Add("@redeemer", SqlDbType.VarChar, 50);
            redeemerIdParam.Value = redeemerID;
            

            // execute the command
            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Ticket ticket = new Ticket();
                    ticket.issuerGuid = ServiceGuid;
                    // read ticket id
                    ticket.ticketId = (long)dataReader.GetInt64(0);
                    ticket.type = dataReader.GetString(1);
                    // read coupon id
                    ticket.couponId = (long)dataReader.GetInt64(2);
                    // read redeemer id
                    ticket.redeemerGuid = dataReader.GetString(3);
                    // read sponsor id
                    ticket.sponsorGuid = dataReader.GetString(4);
                    // read expiration
                    ticket.creationTime = dataReader.GetDateTime(5);
                    ticket.duration = dataReader.GetInt64(6);
                    // read payload
                    ticket.payload = dataReader.GetString(7);
                    // read Cancelled
                    bool cancelled = dataReader.GetBoolean(8);
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

        public Ticket RetrieveTicket(Coupon coupon, string type)
        {
            return GetIssuedTicket(coupon, type);
        }
/*
        public Ticket RedeemTicket(Coupon coupon, string type, string redeemerGuid)
        {
            return GetIssuedTicket(coupon, type, redeemerGuid);
        }
*/
        protected Ticket GetIssuedTicket(Coupon coupon, string type)
        {
            Ticket results = null;

            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "CancelTicket" stored procedure
            SqlCommand cmd = new SqlCommand("GetIssuedTicket", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIdParam.Value = coupon.couponId;
            SqlParameter typeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            typeParam.Value = type;

            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Ticket ticket = new Ticket();
                    ticket.couponId = coupon.couponId;
                    ticket.issuerGuid = coupon.issuerGuid;
                    ticket.ticketId = dataReader.GetInt64(0);
                    ticket.type = dataReader.GetString(1);
                    ticket.redeemerGuid = dataReader.GetString(3);
                    ticket.sponsorGuid = dataReader.GetString(4);
                    ticket.creationTime = dataReader.GetDateTime(5);
                    ticket.duration = dataReader.GetInt64(6);
                    ticket.payload = dataReader.GetString(7);
                    bool cancelled = dataReader.GetBoolean(8);
                    if (!cancelled)
                    {
                        results = ticket;
                    }
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

            return results;
        }

        protected Ticket GetIssuedTicket(Coupon coupon, string type, string redeemer)
        {
            Ticket results = null;

            // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "CancelTicket" stored procedure
            SqlCommand cmd = new SqlCommand("GetIssuedTicketByRedeemer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // populate the parameters
            SqlParameter couponIdParam = cmd.Parameters.Add("@couponID", SqlDbType.BigInt);
            couponIdParam.Value = coupon.couponId;
            SqlParameter typeParam = cmd.Parameters.Add("@ticketType", SqlDbType.VarChar, 100);
            typeParam.Value = type;
            SqlParameter redeemerParam = cmd.Parameters.Add("@redeemer", SqlDbType.VarChar, 50);
            redeemerParam.Value = redeemer;

            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Ticket ticket = new Ticket();
                    ticket.couponId = coupon.couponId;
                    ticket.issuerGuid = coupon.issuerGuid;
                    ticket.ticketId = dataReader.GetInt64(0);
                    ticket.type = dataReader.GetString(1);
                    ticket.redeemerGuid = dataReader.GetString(3);
                    ticket.sponsorGuid = dataReader.GetString(4);
                    ticket.creationTime = dataReader.GetDateTime(5);
                    ticket.duration = dataReader.GetInt64(6);
                    ticket.payload = dataReader.GetString(7);
                    bool cancelled = dataReader.GetBoolean(8);
                    if (!cancelled)
                    {
                        results = ticket;
                    }
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

            return results;
        }

        public bool RequestTicketCancellation(Coupon coupon, string type, string redeemerGuid)
        {
            return false;
        }

        public List<Ticket> GetExpiredIssuedTickets()
        {
            List<Ticket> tickets = new List<Ticket>();

              // create sql connection
            SqlConnection connection = CreateConnection();

            // create sql command
            // command executes the "CancelTicket" stored procedure
            SqlCommand cmd = new SqlCommand("GetExpiredIssuedTickets", connection);
            cmd.CommandType = CommandType.StoredProcedure;

         

            SqlDataReader dataReader = null;
            try
            {
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Ticket ticket = new Ticket();
                    ticket.issuerGuid = this.GetIssuerGuid();
                    ticket.ticketId = dataReader.GetInt64(0);
                    ticket.type = dataReader.GetString(1);
                    ticket.couponId = dataReader.GetInt64(2);
                    ticket.redeemerGuid = dataReader.GetString(3);
                    ticket.sponsorGuid = dataReader.GetString(4);
                    ticket.creationTime = dataReader.GetDateTime(5);
                    ticket.duration = dataReader.GetInt64(6);
                    ticket.payload = dataReader.GetString(7);
                    ticket.isCancelled = !dataReader.GetBoolean(8);
                    tickets.Add(ticket);

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

            return tickets;
        }



    }
}
