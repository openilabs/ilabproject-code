<%@ WebService Language="c#" Class="I_USS" %>

using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using iLabs.DataTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SchedulingTypes;

	/// <summary>
	/// Summary description for SchedulingService.
	/// </summary>
    [WebServiceBinding(Name = "IUSS", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "UserSchedulingProxy", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public abstract class I_USS : System.Web.Services.WebService
	{

        public OperationAuthHeader opHeader = new OperationAuthHeader();
        public AgentAuthHeader agentAuthHeader = new AgentAuthHeader();

        /// <summary>
        /// List all the reservations for certain lab server for the specified time 
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
        public abstract Reservation[] ListReservations(string serviceBrokerGuid, string userName,
            string labServerGuid, string labClientGuid, DateTime startTime, DateTime endTime);
        
		/// <summary>
		/// remove all the reservation for certain lab server
        /// covered by the revocation time 
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
		/// <param name="clientGuid"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="labClientGuid"></param>
		/// <param name="startTime">UTC</param>
		/// <param name="endTime">UTC</param>
        /// <param name="message"></param>
        /// <returns>true if all the reservations have been 
        /// removed successfully</returns>
		[WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RevokeReservation(string serviceBrokerGuid, string groupName,
            string labServerGuid, string labClientGuid, DateTime startTime, DateTime endTime,
            string message);
		
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
        public abstract Reservation RedeemReservation(string serviceBrokerGuid, string userName, 
            string labServerGuid, string clientGuid);
		
		
		/// <summary>
		///  add a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>true the credential set has been added 
        /// successfully, false otherwise
		[WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
		public abstract int AddCredentialSet(string serviceBrokerGuid,
            string serviceBrokerName,string groupName);

        /// <summary>
        ///  Modify a credential set
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>true the credential set has been added 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int ModifyCredentialSet(string serviceBrokerGuid,
            string serviceBrokerName, string groupName);
		
		///  Remove a credential set
		/// </summary>
        /// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <returns></returns>true, the credentialset is removed 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RemoveCredentialSet(string serviceBrokerGuid, string groupName);
        
		/// <summary>
		/// add information of a particular experiment
		/// </summary>
        /// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
        /// <param name="labClientGuid"></param>
        /// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>		
		/// <param name="providerName"></param>
        /// <param name="lssGuid"></param>
		/// <returns></returns>true, the experimentInfo is added 
        /// successfully, false otherwise
		[WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
		public abstract int AddExperimentInfo(string labServerGuid, string labServerName,
            string labClientGuid, string labClientName, string labClientVersion,
            string providerName, string lssGuid);


        /// <summary>
        /// add information of a particular experiment
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <param name="labServerName"></param>
        /// <param name="labClientGuid"></param>
        /// <param name="labClientName"></param>
        /// <param name="labClientVersion"></param>		
        /// <param name="providerName"></param>
        /// <param name="lssGuid"></param>
        /// <returns></returns>true, the experimentInfo is added 
        /// successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int ModifyExperimentInfo(string labServerGuid, string labServerName,
            string labClientGuid, string labClientName, string labClientVersion,
            string providerName, string lssGuid);
        
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
        public abstract int RemoveExperimentInfo(string labServerGuid, string labClientGuid,string lssGuid);
		
		/// <summary>
		/// add information of a particular lab side scheduling server identified by lssID
		/// </summary>
        /// <param name="lssGuid"></param>
        /// <param name="lssName"></param>
		/// <param name="lssUrl"></param>
		/// <returns></returns>true, the LSSInfo is removed successfully, false otherwise
		[WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int AddLSSInfo(string lssGuid, string lssName, string lssUrl);
        
        /// <summary>
        /// add information of a particular lab side scheduling server identified by lssID
        /// </summary>
        /// <param name="lssGuid"></param>
        /// <param name="lssName"></param>
        /// <param name="lssUrl"></param>
        /// <returns></returns>true, the LSSInfo is removed successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int ModifyLSSInfo(string lssGuid, string lssName, string lssUrl);
        
        /// <summary>
        /// add information of a particular lab side scheduling server identified by lssID
        /// </summary>
        /// <param name="lssGuid"></param>
        /// <param name="lssName"></param>
        /// <param name="lssUrl"></param>
        /// <returns></returns>true, the LSSInfo is removed successfully, false otherwise
        [WebMethod]
        [SoapDocumentMethod(Binding = "IUSS"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public abstract int RemoveLSSInfo(string lssGuid);


	}


