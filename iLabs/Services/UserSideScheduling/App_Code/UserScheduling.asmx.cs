using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using iLabs.Core;
using iLabs.Ticketing;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Services;



namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Summary description for SchedulingService.
	/// </summary>
    [WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebServiceBinding(Name = "IUSS", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "UserSideScheduling", Namespace = "http://ilab.mit.edu/iLabs/Services")]
	public class UserScheduling : WS_ILabCore
	{
		public UserScheduling()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

        public OperationAuthHeader opHeader = new OperationAuthHeader();
 
		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		// WEB SERVICE EXAMPLE
		// The HelloWorld() example service returns the string Hello World
		// To build, uncomment the following lines then save and build the project
		// To test this web service, press F5


        /// <summary>
        /// List all the reservations for the user for the specified time, any intersection.
        /// </summary>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime">UTC</param>
        /// <param name="endTime">UTC</param>
        /// <returns>true if all the reservations have been 
        /// removed successfully</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public Reservation[] ListReservations(string userName, string serviceBrokerGuid,
            string labClientGuid, string labServerGuid,
            DateTime startTime, DateTime endTime)
        {
            return new Reservation[] { new Reservation() };
        }
        

		/// <summary>
		/// remove all the reservation for certain lab server being covered by the revocation time 
		/// </summary>
        /// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>local time of USS
		/// <param name="endTime"></param>local time of USS
		/// true if all the reservations have been removed successfully
		[WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
		public bool RevokeReservation(string labClientGuid,string labServerGuid, 
            DateTime startTime, DateTime endTime)
		{
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.REVOKE_RESERVATION;
            try
            {

                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);
                return USSSchedulingAPI.RevokeReservation(labServerGuid, startTime, endTime);
            }
            catch 
            {
                throw;
            }
			
		}
		/// <summary>
		/// Returns an Boolean indicating whether it the right time for a particular user to execute a particular experiment
		/// </summary>
		/// <param name="userName"></param>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
		/// <returns></returns>true if it is the right time for a particular user to execute a particular experiment
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public ReservationInfo RedeemReservation(String userName, String serviceBrokerGuid, String clientGuid, String labServerGuid)
		{
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.EXECUTE_EXPERIMENT;
            try
            {
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);

                return USSSchedulingAPI.RedeemReservation(userName, serviceBrokerGuid, clientGuid, labServerGuid);
            }
            catch
            {
                throw;
            }
			

		}
		
		/// <summary>
		///  add a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>true the credential set has been added successfully, false otherwise
		[WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool AddCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName)
		{
            bool add = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    int test = USSSchedulingAPI.GetCredentialSetID(serviceBrokerGuid,groupName);
                    if (test > 0)
                    {
                      
                        add = true;
                    }
                    else{
                        int i = USSSchedulingAPI.AddCredentialSet(serviceBrokerGuid, serviceBrokerName, groupName);
                        if (i != -1)
                        {
                            add = true;
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            return add;
		}
		///  Remove a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>true, the credentialset is removed successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool RemoveCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName)
        {
            bool removed = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    removed = USSSchedulingAPI.RemoveCredentialSet(serviceBrokerGuid, serviceBrokerName, groupName);
                }
                catch
                {
                    throw;
                }
            }
            return removed;
        }

		/// <summary>
		/// add information of a particular experiment
		/// </summary>
        /// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="labClientName"></param>
		/// <param name="providerName"></param>
        /// <param name="lssGuid"></param>
		/// <returns></returns>true, the experimentInfo is added successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool AddExperimentInfo(string labServerGuid, string labServerName, string labClientGuid, string labClientName, string labClientVersion, string providerName, string lssGuid)
        {
            bool added = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    int eID = USSSchedulingAPI.AddExperimentInfo(labServerGuid, labServerName,labClientGuid,  labClientName, labClientVersion, providerName, lssGuid);
                    if (eID != -1)
                        added = true;
                }
                catch
                {
                    throw;
                }
            }
            return added;
        }

		/// <summary>
		/// add information of a particular lab side scheduling server identified by lssID
		/// </summary>
        /// <param name="lssGuid"></param>
		/// <param name="lssUrl"></param>
		/// <returns></returns>true, the LSSInfo is removed successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool AddLSSInfo(string lssGuid, string lssName, string lssUrl)
        {
            bool added = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {                 
                    int lID = USSSchedulingAPI.AddLSSInfo(lssGuid, lssName, lssUrl);
                    if (lID != -1)
                        added = true;
                }
                catch
                {
                    throw;
                }
            }
            return added;
        }
  
	}
}

