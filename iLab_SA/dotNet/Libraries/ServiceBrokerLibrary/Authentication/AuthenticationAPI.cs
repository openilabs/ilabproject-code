/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id$ */

using System;
using System.Web.Security;

using iLabs.Core;
using iLabs.DataTypes.TicketingTypes;
using iLabs.ServiceBroker.Internal;
using iLabs.Ticketing;


namespace iLabs.ServiceBroker.Authentication
{
	public class AuthenticationType
	{
		public const string NativeAuthentication = "Native";
		public const string Kerberos = "Kerberos_MIT";
	}


	/// <summary>
	/// Summary description for Authentication.
	/// </summary>
	public class AuthenticationAPI
	{
		public AuthenticationAPI()
		{
		}

		/// <summary>
		/// performs whatever actions including GUI interaction to identify and authenticate the user who has initiated the current session
		/// </summary>
		/// <param name="userID">the ID of user</param>
		/// <param name="password">the user's password</param>
		/// <returns>true if the user has been authenticated; false otherwise</returns>
		public static bool Authenticate (int userID, string password)
		{
            bool status = false;
			string hashedDBPassword = InternalAuthenticationDB.ReturnNativePassword (userID);
            if (hashedDBPassword != null && hashedDBPassword.Length > 0)
            {
                string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
                if (hashedPassword == hashedDBPassword)
                {
                    status = true;
                }
            }
            return status;
		}

        /// <summary>
        /// performs whatever actions including GUI interaction to identify and authenticate the user who has initiated the current session
        /// </summary>
        /// <param name="userName">the local of user</param>
        /// <param name="authGuid">an optional authority guid, if empty use local pasword</param>
        /// <param name="password">the user's password</param>
        /// <returns>true if the user has been authenticated; false otherwise</returns>
        public static bool Authenticate(string userName, string authGuid, string password)
        {
            bool status = false;
            int userID = -1;
            if (authGuid == null || authGuid.Length == 0)
            {
                userID = InternalAdminDB.SelectUserID(userName);
                string hashedDBPassword = InternalAuthenticationDB.ReturnNativePassword(userID);
                if (hashedDBPassword != null && hashedDBPassword.Length > 0)
                {
                    string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
                    if (hashedPassword == hashedDBPassword)
                    {
                        status = true;
                    }
                }
            }
            return status;
        }

        public static int AuthenticateAuthority(string authGuid, string passkey)
        {
            int id = -1;
            BrokerDB brokerDB = new BrokerDB();
            Coupon authCoupon = brokerDB.GetIssuedCoupon(passkey);
            if (authCoupon != null)
            {
                Ticket authTicket = brokerDB.RetrieveTicket(authCoupon, TicketTypes.AUTHENTICATE_AGENT);
                if(authTicket != null){

                // Parse Ticket
                    //todo
                }
            }
            else
            {
                throw new AccessDeniedException("AccessDenied!");
            }
            return id;
        }
		/*		
		public static string Authenticate (string type)
		{
			// how to do this?
		}

		public static bool CreateNativePrincipal (string principalID)
		{

		}

		public static string[] RemoveNativePrincipals (string[] principalIDs)
		{
		
		}

		public static string[] ListNativePrincipals()
		{

		}
*/
		/// <summary>
		/// Set the password of the specified native principal
		/// </summary>
		/// <param name="userID">the ID of the native principal whose password is to be changed</param>
		/// <param name="password">the new password</param>
		/// <returns>true if the change was successful; false if the new password was of inappropriate form or the native userID unknown.</returns>
		public static bool SetNativePassword (int userID, string password)
		{
			string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
			try
			{
				InternalAuthenticationDB.SaveNativePassword (userID, hashedPassword);

			}
			catch
			{
				return false;
			}
			return true;

		}

	
	}
}
