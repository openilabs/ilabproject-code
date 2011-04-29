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
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SchedulingTypes;

using iLabs.Proxies.ISB;
using iLabs.Proxies.LSS;
using iLabs.Web;

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

        /// <summary>
        /// Modifies the information related to the specified service the service's Guid must exist and the typ of service may not be modified,
        /// in and out coupons may be changed.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="inIdentCoupon"></param>
        /// <param name="outIdentCoupon"></param>
        /// <returns></returns>
        [WebMethod,
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public override int ModifyDomainCredentials(string originalGuid, ProcessAgent agent, string extra, 
            Coupon inCoupon, Coupon outCoupon)
        {
              int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                DBManager dbManager = new DBManager();
            try
            {
                status = dbManager.ModifyDomainCredentials(originalGuid, agent, inCoupon, outCoupon, extra);
            }
            catch (Exception ex)
            {
                throw new Exception("USS: ", ex);
            }

           
            }
            return status;

        }

        /// <summary>
        /// Informs this processAgent that it should modify all references to a specific processAent. 
        /// This is used to propagate modifications, The agentGuid must remain the same.
        /// </summary>
        /// <param name="domainGuid">The guid of the services domain ServiceBroker</param>
        /// <param name="serviceGuid">The guid of the service</param>
        /// <param name="state">The retired state to be set</param>
        /// <returns>A status value, negative values indicate errors, zero indicates unknown service, positive indicates level of success.</returns>
        [WebMethod,
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public override int ModifyProcessAgent(string originalGuid, ProcessAgent agent, string extra)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                DBManager dbManager = new DBManager();
                status = dbManager.ModifyProcessAgent(originalGuid, agent, extra);
            }
            return status;
        }


        /// <summary>
        /// Retrieve available time periods(local time of LSS) This is a pas-through method that gets the information from the LSS.
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime"></param>the local time of LSS
        /// <param name="endTime"></param>the local time of LSS
        /// <returns></returns>return an array of time periods (local time), each of the time periods is longer than the experiment's minimum time 
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public TimePeriod[] RetrieveAvailableTimePeriods(string serviceBrokerGuid, string groupName,
            string labServerGuid, string clientGuid, DateTime startTime, DateTime endTime)
        {
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            try
            {
                Ticket ssTicket = dbTicketing.RetrieveAndVerify(opCoupon, TicketTypes.SCHEDULE_SESSION);
                
                string lssGuid = USSSchedulingAPI.ListLSSIDbyExperiment(clientGuid, labServerGuid);
                ProcessAgentInfo lssInfo = dbTicketing.GetProcessAgentInfo(lssGuid);
                LabSchedulingProxy lssProxy = new LabSchedulingProxy();
                lssProxy.OperationAuthHeaderValue.coupon = opCoupon;
                lssProxy.Url = lssInfo.ServiceUrl;
                TimePeriod[] array = lssProxy.RetrieveAvailableTimePeriods( serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid,
                    labServerGuid, clientGuid, startTime, endTime);
                return array;
            }
            catch
            {
                throw;
            }
        }

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
        public Reservation[] ListReservations(string serviceBrokerGuid, string userName, 
            string labServerGuid, string labClientGuid, DateTime startTime, DateTime endTime)
        {
            
            Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opHeader.coupon, TicketTypes.REDEEM_RESERVATION);

            ReservationInfo[] resInfos = USSSchedulingAPI.GetReservationInfos(serviceBrokerGuid, userName, null,
                labServerGuid, labClientGuid, startTime, endTime);
            if (resInfos != null && resInfos.Length > 0)
            {
                Reservation[] reservations = new Reservation[resInfos.Length];

                for (int i = 0; i < resInfos.Length; i++)
                {
                    reservations[i] = new Reservation(resInfos[i].startTime, resInfos[i].endTime);
                    reservations[i].userName = resInfos[i].userName;
                }
                return reservations;
            }
            else 
                return null;

        }

        /// <summary>
        /// Add a reservation for a lab server for thew specified user, client and time. 
        /// If the reservation is confirmed by the LSS, the reservation will be added to the USS.
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="labClientGuid"></param>
        /// <param name="startTime">UTC</param>
        /// <param name="endTime">UTC</param>
        /// <returns>Returns a message of succes or a simple description of the reason for failure.</returns>
        [WebMethod(Description = "Add a reservation for a lab server for the specified user, client and time. "
        + "If the reservation is confirmed by the LSS, the reservation will be added to the USS.")]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public string AddReservation(string serviceBrokerGuid, string userName, string groupName,
            string labServerGuid, string labClientGuid, DateTime startTime, DateTime endTime)
        {
            string message = null;
             Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.SCHEDULE_SESSION;
            try
            {
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);
                string lssGuid = USSSchedulingAPI.ListLSSIDbyExperiment(labClientGuid, labServerGuid);
                ProcessAgentInfo lssInfo = dbTicketing.GetProcessAgentInfo(lssGuid);
                LabSchedulingProxy lssProxy = new LabSchedulingProxy();
                lssProxy.OperationAuthHeaderValue.coupon = opCoupon;
                lssProxy.Url = lssInfo.ServiceUrl;
                message = lssProxy.ConfirmReservation( serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid,
                    labServerGuid, labClientGuid, startTime, endTime);
                if(message.Contains("Success")){
                    int infoID = USSSchedulingAPI.ListExperimentInfoIDByExperiment(labServerGuid, labClientGuid);
                    USSSchedulingAPI.AddReservation(userName, serviceBrokerGuid,groupName,infoID,startTime,endTime);
                }
                return message;
            }
           
            catch (Exception e)
            {
                throw new Exception("USS: AddReservation -> ", e);
            }
            return message;
        }

		/// <summary>
		/// Remove all the reservations for certain lab server being covered by the revocation time. 
        /// Unless the request is from a LSS the LSS will be forwarded A RemoveReservation request.
        /// For each reservation the service broker is notified, so that an email may be sent to the user.
		/// </summary>
        /// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>local time of USS
		/// <param name="endTime"></param>local time of USS
		/// true if all the reservations have been removed successfully
		[WebMethod(Description = "Remove all the reservations for a lab server that match the specification."
            + " The serviceBroker is notified of each reservation so that an email is sent to each of the owners of the removed reservations."
            + " If the method is not called by an LSS the LSS is forwarded a RemoveReservation request.")]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public int RevokeReservation(string serviceBrokerGuid, string groupName,
            string labServerGuid, string labClientGuid, DateTime startTime, DateTime endTime, string message)
		{
            bool status = false;
            int count = 0;
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.REVOKE_RESERVATION;
            try
            {
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);
                ReservationData[] ris = USSSchedulingAPI.GetReservations(serviceBrokerGuid, null, groupName,
                    labServerGuid, labClientGuid, startTime, endTime);
           
                if (ris != null && ris.Length > 0)
                {

                    InteractiveSBProxy sbProxy = new InteractiveSBProxy();
                    ProcessAgentInfo sbInfo = dbTicketing.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.domainGuid);
                    AgentAuthHeader header = new AgentAuthHeader();
                    header.coupon = sbInfo.identOut;
                    header.agentGuid = ProcessAgentDB.ServiceGuid;
                    sbProxy.AgentAuthHeaderValue = header;
                    sbProxy.Url = sbInfo.webServiceUrl;
                    foreach (ReservationData rd in ris)
                    {
                        status = USSSchedulingAPI.RevokeReservation(rd.sbGuid, rd.groupName, rd.lsGuid, rd.clientGuid,
                              rd.startTime, rd.endTime, message);
                        if (status)
                        {
                            count++;
                            status = sbProxy.RevokeReservation(rd.sbGuid, rd.userName, rd.groupName, rd.lsGuid, rd.clientGuid,
                              rd.startTime, rd.endTime, message);
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                throw new Exception("USS: RevokeReservation -> ",e);
            }
            return count;
			
		}
		/// <summary>
        /// Returns an existing reservation for the current time for 
        /// a particular user to execute a particular experiment. 
        /// This does not create the AllowExperiment ticket.
		/// </summary>
		/// <param name="userName"></param>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <returns>the existing reservation if it is the right time for a particular
        /// user to execute a particular experiment, or null.</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public Reservation RedeemReservation(string serviceBrokerGuid, String userName,  
            String labServerGuid, string clientGuid)
		{
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.REDEEM_RESERVATION;
            try
            {
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);
                ReservationInfo res = USSSchedulingAPI.RedeemReservation(userName, serviceBrokerGuid, clientGuid, labServerGuid);
                if (res != null)
                {
                    Reservation reservation = new Reservation(res.startTime, res.endTime);
                    reservation.userName = res.userName;
                    return reservation;
                }
                else
                {
                    return null;
                }
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
        public int AddCredentialSet(string serviceBrokerGuid, string serviceBrokerName,
            string groupName)
		{
           int add = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    int test = USSSchedulingAPI.GetCredentialSetID(serviceBrokerGuid,groupName);
                    if(test > 0)
                    {

                        add = 1; ;
                    }
                    else{
                        int i = USSSchedulingAPI.AddCredentialSet(serviceBrokerGuid, serviceBrokerName, groupName);
                        add = (i != -1) ? 1 : 0;
                       
                    }
                }
                catch
                {
                    throw;
                }
            }
            return add;
		}

        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int ModifyCredentialSet(string originalGuid, string serviceBrokerGuid, string serviceBrokerName,
            string groupName)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    status = DBManager.ModifyCredentialSetServiceBroker(originalGuid, serviceBrokerGuid, serviceBrokerName);
                }
                catch
                {
                    throw;
                }
            }
            return status;
        }

		///  Remove a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>true, the credentialset is removed successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int RemoveCredentialSet(string serviceBrokerGuid, string serviceBrokerName,
            string groupName)
        {
            int removed = 0;
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
        public int AddExperimentInfo(string labServerGuid, string labServerName, 
            string labClientGuid, string labClientName, string labClientVersion, 
            string providerName, string lssGuid)
        {
           int added = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    int eID = USSSchedulingAPI.AddExperimentInfo(labServerGuid, labServerName,labClientGuid,  labClientName, labClientVersion, providerName, lssGuid);
                    added = (eID != -1) ? 1 : 0;
                }
                catch
                {
                    throw;
                }
            }
            return added;
        }

        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int ModifyExperimentInfo(string labServerGuid, string labServerName,
            string labClientGuid, string labClientName, string labClientVersion,
            string providerName, string lssGuid)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    status = DBManager.ModifyExperimentInfo(labServerGuid, labServerName, labClientGuid, labClientName, labClientVersion, providerName, lssGuid);
                }
                catch
                {
                    throw;
                }
            }
            return status;
        }
         /// <summary>
        /// remove a particular experiment
        /// </summary>
        /// <param name="labServerGuid"></param>      
        /// <param name="labClientGuid"></param>
        /// <param name="lssGuid"></param>
        /// <returns></returns>true, the experimentInfo is removed 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int RemoveExperimentInfo(string labServerGuid,
            string labClientGuid,string lssGuid){
            return 0;
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
        public int AddLSSInfo(string lssGuid, string lssName, string lssUrl)
        {
            int added = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    int lID = USSSchedulingAPI.AddLSSInfo(lssGuid, lssName, lssUrl);
                    added = (lID != -1) ? 1 : 0;
                }
                catch
                {
                    throw;
                }
            }
            return added;
        }

        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int ModifyLSSInfo(string lssGuid, string lssName, string lssUrl)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    status = DBManager.ModifyLSSInfo(lssGuid, lssName, lssUrl);
                    
                   
                }
                catch
                {
                    throw;
                }
            }
            return status;
        }

        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int RemoveLSSInfo(string lssGuid)
        {
            int added = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    added = USSSchedulingAPI.RemoveLSSInfoByGuid(lssGuid);
                  
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

