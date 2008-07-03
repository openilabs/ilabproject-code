/* $Id: LabScheduling.asmx.cs,v 1.9 2007/06/27 22:45:02 pbailey Exp $ */
using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

using iLabs.Core;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Services;
using iLabs.Ticketing;
using iLabs.UtilLib;


namespace iLabs.Scheduling.LabSide
{
	/// <summary>
	/// Summary description for SchedulingService.
	/// </summary>
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/Type")]
    [WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebServiceBinding(Name = "ILSS", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "LabSideScheduling", Namespace = "http://ilab.mit.edu/iLabs/Services")]
	public class LabScheduling : WS_ILabCore
	{
    
 
		public LabScheduling()
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
		/// retrieve available time periods(local time of LSS) overlaps with a time chrunk for a particular group and particular experiment,
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>the local time of LSS
		/// <param name="endTime"></param>the local time of LSS
		/// <returns></returns>return an array of time periods (local time), each of the time periods is longer than the experiment's minimum time 
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public TimePeriod[] RetrieveAvailableTimePeriods(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            try
            {
               
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, TicketTypes.REQUEST_RESERVATION);
                TimePeriod[] array = LSSSchedulingAPI.RetrieveAvailableTimePeriods(serviceBrokerGuid, groupName, ussGuid, 
                    clientGuid, labServerGuid, startTime, endTime);
                return array;
            }
            catch
            {
                throw;
            }
			
			
		}
        /// <summary>
        /// retrieve available time blocks overlaps with a time chrunk for a particular group and particular experiment,
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
       /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>return an array of time blocks, each of the time blocks is longer than the experiment's minimum time 
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public TimeBlock[] RetrieveAvailableTimeBlocks(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
        {
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.REQUEST_RESERVATION;
            try
            {

                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);
                TimeBlock[] array = LSSSchedulingAPI.RetrieveAvailableTimeBlocks(serviceBrokerGuid, groupName, ussGuid, clientGuid, labServerGuid, startTime, endTime);
                return array;
            }
            catch(Exception e)
            {
                iLabs.UtilLib.Utilities.WriteLog("LSS: RetrieveAvailableTimeBlocks: " + e.Message);
                throw;
            }


        }

		/// <summary>
		/// Returns an Boolean indicating whether a particular reservation from a USS is confirmed and added to the database in LSS successfully. If it fails, exception will be throw out indicating
		///	the reason for rejection.
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>the notification whether the reservation is confirmed. If not, notification will give a reason
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public string ConfirmReservation(string serviceBrokerGuid, string groupName, string ussGuid, 
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.REQUEST_RESERVATION;
            try
            {
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);

                return LSSSchedulingAPI.ConfirmReservation(serviceBrokerGuid, groupName, ussGuid, 
                    clientGuid, labServerGuid, startTime, endTime);
            }
            catch
            {
                throw;
            }
			
		}
		/// <summary>
		/// remove the reservation information
		/// </summary>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>true remove successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public bool RemoveReservation(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            string type = TicketTypes.REQUEST_RESERVATION;
            try
            {
                //                Ticket retrievedTicket = ticketRetrieval.RetrieveAndVerify(opCoupon, type, "LAB SCHEDULING SERVER");

                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, type);

                DateTime startTimeUTC = startTime.ToUniversalTime();
                DateTime endTimeUTC = endTime.ToUniversalTime();
                bool removed = LSSSchedulingAPI.RemoveReservationInfo(serviceBrokerGuid, groupName, ussGuid,
                    clientGuid, labServerGuid, startTimeUTC, endTimeUTC);
                return removed;
            }
            catch
            {
                throw;
            }
			
		}

        /// <summary>
        /// given a time period defined by the start time and the end tiime, return the time slots defined by the quatum of the experiment during this time period
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public TimePeriod[] RetrieveTimeSlots(string serviceBrokerGuid, string groupName, 
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
        {
            Coupon opCoupon = new Coupon();
            opCoupon.couponId = opHeader.coupon.couponId;
            opCoupon.passkey = opHeader.coupon.passkey;
            opCoupon.issuerGuid = opHeader.coupon.issuerGuid;
            
            try
            {
                Ticket retrievedTicket = dbTicketing.RetrieveAndVerify(opCoupon, TicketTypes.REQUEST_RESERVATION);
                TimePeriod[] array = LSSSchedulingAPI.RetrieveTimeSlots(serviceBrokerGuid, groupName, 
                    clientGuid, labServerGuid,startTime, endTime);
                return array;
            }
            catch
            {
                throw;
            }

        }

		/// <summary>
		/// Add information of a particular user side scheduling server identified by ussGuid. 
        /// This may be called several times with the same USS Info, due to the nature 
        /// of Scheduling this is not an error. If a revokeTicket already exists for the
        /// USS use the existing TicketCoupon.
		/// </summary>
        /// <param name="ussGuid"></param>
		/// <param name="ussName"></param>
		/// <param name="ussUrl"></param>
        /// <param name="coupon"></param>
        /// <returns>true if the USSInfo is added successfully or is already in the database, false otherwise</returns>
		[WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool AddUSSInfo(string ussGuid, string ussName, string ussUrl, Coupon coupon)
		{
            bool found = false;
            try
            {
                if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
                {
                    int ussId = LSSSchedulingAPI.ListUSSInfoID(ussGuid);
                    if (ussId > 0)
                    {
                        USSInfo[] info = LSSSchedulingAPI.GetUSSInfos(new int[] { ussId });
                        if(info != null && info.Length > 0){
                            if(info[0].ussGuid.Equals(ussGuid) // && info[0].ussUrl.Equals(ussUrl) 
                                && info[0].domainGuid.Equals(coupon.issuerGuid)){
                                if(info[0].couponId != coupon.couponId){
                                    // A new revokeTicket coupon has been created,
                                    // Add it to the database & update USSinfo
                                    if (!dbTicketing.AuthenticateCoupon(coupon))
                                        dbTicketing.InsertCoupon(coupon);
                                    LSSSchedulingAPI.ModifyUSSInfo(ussId, ussGuid,ussName,ussUrl,
                                        coupon.couponId,coupon.issuerGuid);
                                }
                                found = true;
                            }
                        }

                    }
                    if(!found)
                    {
                        if( !dbTicketing.AuthenticateCoupon(coupon))
                            dbTicketing.InsertCoupon(coupon);
                        int uID = LSSSchedulingAPI.AddUSSInfo(ussGuid, ussName, ussUrl, coupon.couponId, coupon.issuerGuid);
                        if (uID > 0)
                            found = true;
                    }

                }
            }
            catch
            {
                throw;
            }
            return found;
		}
		/// <summary>
		/// Add a credential set of a particular group, may be called multiple times with same data. 
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <returns></returns>true if the CredentialSet is added successfully, or already exists, false otherwise
		[WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool AddCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName, string ussGuid)
		{
            bool added = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                int test = LSSSchedulingAPI.GetCredentialSetID(serviceBrokerGuid, groupName, ussGuid);
                if (test > 0)
                {
                    added = true;
                }
                else
                {
                    int cID = LSSSchedulingAPI.AddCredentialSet(serviceBrokerGuid, serviceBrokerName, groupName, ussGuid);
                    if (cID != -1)
                        added = true;
                }
            }
			return added;
		}

        /// <summary>
        /// remove a credential set of a particular group
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <returns></returns>true if the CredentialSet is removed successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool RemoveCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName, string ussGuid)
        {
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                return LSSSchedulingAPI.RemoveCredentialSet(serviceBrokerGuid, serviceBrokerName, groupName, ussGuid);
            }
            else 
                return false;
        }
		
          /// <summary>
        /// add information of a particular experiment
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <param name="labServerName"></param>
        /// <param name="labClientGuid"></param>
        /// <param name="labClientVersion"></param>
        /// <param name="labClientName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>true, the experimentInfo is added 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool AddExperimentInfo(string labServerGuid, string labServerName,
            string clientGuid, string clientName, string clientVersion,
            string providerName)
        {
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                int id = LSSSchedulingAPI.AddExperimentInfo(labServerGuid, labServerName, clientGuid, clientName,
                    clientVersion, providerName, 0, 0, 0, 0, 0);
                int ok = LSSSchedulingAPI.CheckForLSResource(labServerGuid, labServerName);
                return id > 0;
            }
            else
                return false;
        }
  
	}
}
