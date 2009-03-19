<%@ WebService Language="c#" Class="I_LSS" %>


/* $Id: I_LSS.asmx,v 1.10 2007/06/15 23:14:55 pbailey Exp $ */
using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;


//using iLabs.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

//using iLabs.TicketingAPI;
using iLabs.DataTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SchedulingTypes;



	/// <summary>
	/// Summary description for SchedulingService.
	/// </summary>
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/Type")]
    [WebServiceBinding(Name = "ILSS", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "LabSchedulingProxy", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public abstract class I_LSS : System.Web.Services.WebService
	{
        public OperationAuthHeader opHeader = new OperationAuthHeader();
        public AgentAuthHeader agentAuthHeader = new AgentAuthHeader();
       
		/// <summary>
		/// retrieve available time periods(UTC) overlaps 
        /// with a time chunk for a particular group and particular experiment,
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <param name="clientGuid"></param>
		/// <param name="labServerGuid"></param>
        /// <param name="startTime">UTC start time</param>
        /// <param name="endTime">UTC end time</param>
        /// <returns>return an array of time periods (UTC), each of the 
        /// time periods is longer than the experiment's minimum time</returns> 
        [WebMethod(Description="Retrieve available time periods for a particular group and particular experiment")]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public abstract TimePeriod[] RetrieveAvailableTimePeriods(string serviceBrokerGuid,
            string groupName, string ussGuid,
            string labServerGuid, string clientGuid, DateTime startTime, DateTime endTime);
 
		/// <summary>
		/// Returns an Boolean indicating whether a particular reservation from
        /// a USS is confirmed and added to the database in LSS successfully.
        /// If it fails, exception will be throw out indicating
		///	the reason for rejection.
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
		/// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime">UTC start time</param>
        /// <param name="endTime">UTC end time</param>
        /// <returns>the notification whether the reservation is confirmed. If not, 
        /// notification will give a reason</returns>
        [WebMethod(Description="Returns an Boolean indicating whether a particular reservation from a USS is confirmed and added to the database in LSS successfully."
        + "If it fails, a exception will be throw indicating the reason for rejection.")]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public abstract string ConfirmReservation(string serviceBrokerGuid, string groupName,
            string ussGuid, string labServerGuid, string clientGuid, 
            DateTime startTime, DateTime endTime);

        /// <summary>
        /// Update the reservation status
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="startTime">UTC start time</param>
        /// <param name="endTime">UTC end time</param>
        /// <returns>true updated successfully, false otherwise</returns>
        [WebMethod(Description="Used to update reservation status on LSS")]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RedeemReservation(string serviceBrokerGuid, string groupName,
            string ussGuid, string labServerGuid, string clientGuid,
            DateTime startTime, DateTime endTime);
        
		/// <summary>
		/// Removes all resevations which intersect the parameters, called from the USS does not require the LSS to notify the USS.
		/// </summary>
        /// <param name="serviceBrokerGuid">Must be specified if groupName specified</param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid">Must be specified</param>
		/// <param name="clientGuid"></param>
        /// <param name="labServerGuid">Must be specified</param>
        /// <param name="startTime">UTC start time, trucated within the method to UTCnow</param>
        /// <param name="endTime">UTC end time</param>
        /// <returns>true remove successfully, false otherwise</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RemoveReservation(string serviceBrokerGuid, string groupName,
            string ussGuid, string labServerGuid, string clientGuid, 
            DateTime startTime, DateTime endTime);
   
		/// <summary>
		/// Add information of a particular user side scheduling server identified by ussID 
		/// </summary>
        /// <param name="ussGuid"></param>
		/// <param name="ussName"></param>
		/// <param name="ussUrl"></param>
        /// <returns>true if the USSInfo is added successfully, false otherwise</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int AddUSSInfo(string ussGuid, string ussName, string ussUrl, Coupon coupon);

        /// <summary>
        /// Add information of a particular user side scheduling server identified by ussID 
        /// </summary>
        /// <param name="ussGuid"></param>
        /// <param name="ussName"></param>
        /// <param name="ussUrl"></param>
        /// <returns>true if the USSInfo is added successfully, false otherwise</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int ModifyUSSInfo(string ussGuid, string ussName, string ussUrl, Coupon coupon);
        
        /// <summary>
        /// Add information of a particular user side scheduling server identified by ussID 
        /// </summary>
        /// <param name="ussGuid"></param>
        /// <param name="ussName"></param>
        /// <param name="ussUrl"></param>
        /// <returns>true if the USSInfo is added successfully, false otherwise</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RemoveUSSInfo(string ussGuid);
        
		/// <summary>
		/// add a credential set of a particular group
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="serviceBrokerName"></param>
		/// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <returns>true if the CredentialSet is added successfully, false otherwise</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int AddCredentialSet(string serviceBrokerGuid, string serviceBrokerName, 
            string groupName, string ussGuid);


        /// <summary>
        /// add a credential set of a particular group
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="serviceBrokerName"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <returns>true if the CredentialSet is added successfully, false otherwise</returns>
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int ModifyCredentialSet(string serviceBrokerGuid, string serviceBrokerName,
            string groupName, string ussGuid);

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
        public abstract int RemoveCredentialSet(string serviceBrokerGuid,
            string groupName, string ussGuid);

        /// <summary>
        /// add information of a particular experiment
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <param name="labServerName"></param>
        /// <param name="clientGuid"></param>
        /// <param name="clientVersion"></param>
        /// <param name="clientName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>true, the experimentInfo is added 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int AddExperimentInfo(string labServerGuid, string labServerName,
            string clientGuid, string clientName, string clientVersion,
            string providerName);

        /// <summary>
        /// add information of a particular experiment
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <param name="labServerName"></param>
        /// <param name="clientGuid"></param>
        /// <param name="clientVersion"></param>
        /// <param name="clientName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>true, the experimentInfo is added 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int ModifyExperimentInfo(string labServerGuid, string labServerName,
            string clientGuid, string clientName, string clientVersion,
            string providerName);

        /// <summary>
        /// Remove a particular experiment
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <param name="labServerName"></param>
        /// <param name="clientGuid"></param>
        /// <param name="clientVersion"></param>
        /// <param name="clientName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>true, the experimentInfo is removed 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "ILSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RemoveExperimentInfo(string labServerGuid, string clientGuid);


	}
