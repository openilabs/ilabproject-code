/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: InteractiveSB.asmx.cs,v 1.52 2008/03/17 21:23:18 pbailey Exp $ */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

using System.Web;
using System.Web.Mail;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.SessionState;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.BatchTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;

using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.DataStorage;
using iLabs.ServiceBroker.Internal;
using iLabs.Services;
using iLabs.Ticketing;
using iLabs.TicketIssuer;

using iLabs.UtilLib;


namespace iLabs.ServiceBroker.iLabSB
{
	/// <summary>
	/// ServiceBrokerService contains all of the Service Broker Web Service Calls.
	/// All of the Service Broker to Lab Server passthrough calls run in the context of a user's 
	/// Service Broker Web Interface session, and consequently have session enabled. This works
	/// because they are submitted from the Java Client, through one or more of the Service Broker
	/// passthrough methods, on to the corresponding method on the Lab Server. 
	/// There is one Method, Notify(), that is called from the Lab Server directly. This
	/// runs outside of the session context. 
	/// </summary>
	/// 
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/Type")]
    [WebServiceBinding(Name = "IServiceBroker", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebServiceBinding(Name = "ITicketIssuer", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "InteractiveServiceBroker", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public class InteractiveSB : WS_ILabCore
    {

        /// <summary>
        /// 
        /// </summary>
        public OperationAuthHeader opHeader = new OperationAuthHeader();
        protected AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();

        /// <summary>
        /// Instantiated to send sbAuthHeader objects in SOAP requests
        /// </summary>

        public InteractiveSB()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();

        }

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
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion



        /// <summary>
        /// Install the Domain credentials on this static process agent
        /// </summary>
        /// <param name="initialPasskey"></param>
        /// <param name="agent" Description="used to provide the service address of the the agent not stored on the agent"></param>
        /// <param name="agentIdentCoupon" Description="For messages from the PA_Service"></param>
        /// <param name="serviceBroker" Description="service information stored on PA_Service"></param>
        /// <param name="sbIdentCoupon" Description="For messages from the SB"></param>
        [WebMethod,
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("initAuthHeader", Direction = SoapHeaderDirection.In)]
        public override ProcessAgent InstallDomainCredentials(ProcessAgent service,
            Coupon inIdentCoupon, Coupon outIdentCoupon)
        {
            // if the remote process agent is a Service Broker, register it as a remote service broker
            if (service.type.Equals(ProcessAgentType.SERVICE_BROKER))
                service.type = ProcessAgentType.REMOTE_SERVICE_BROKER;
            return InstallDomainCredentials(initAuthHeader, service, inIdentCoupon, outIdentCoupon);

        }


        [WebMethod(Description = "CancelTicket -- Try to cancel a cached ticket, should return true if cancelled or not found."
          + " The serviceBroker version - If not the redeemer the call needs to be repackaged and forwarded to the redeemer."),
       SoapDocumentMethod(Binding = "IProcessAgent"),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public override bool CancelTicket(Coupon coupon, string type, string redeemer)
        {
            bool status = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                // Move all this into BrokerDB
                BrokerDB agentDB = new BrokerDB();
                if (ProcessAgentDB.ServiceGuid.Equals(redeemer))
                {
                    // this ServiceBroker is the redeemer
                    status = agentDB.CancelTicket(coupon, type, redeemer);
                }
                else
                {
                    ProcessAgentInfo target = agentDB.GetProcessAgentInfo(redeemer);
                    if (target != null)
                    {

                        if (ProcessAgentDB.ServiceGuid.Equals(target.domainGuid))
                        {
                            // Its a local domain processAgent
                            ProcessAgentProxy paProxy = new ProcessAgentProxy();
                            paProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                            paProxy.AgentAuthHeaderValue.coupon = target.identOut;
                            paProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                            paProxy.Url = target.webServiceUrl;
                            status = paProxy.CancelTicket(coupon, type, redeemer);

                        }
                        else
                        {
                            ProcessAgentInfo remoteSB = agentDB.GetProcessAgentInfo(target.domainGuid);
                            if (remoteSB != null)
                            {
                                // Its from a known domain
                                ProcessAgentProxy sbProxy = new ProcessAgentProxy();
                                sbProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                                sbProxy.AgentAuthHeaderValue.coupon = remoteSB.identOut;
                                sbProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                                sbProxy.Url = remoteSB.webServiceUrl;
                                status = sbProxy.CancelTicket(coupon, type, redeemer);

                            }
                        }
                    }

                }
            }
            return status;
        }

        [WebMethod(Description = "Register"),
 SoapDocumentMethod(Binding = "IProcessAgent"),
 SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public override void Register(string registerId, ServiceDescription[] info)
        {
            bool hasProvider = false;
            bool hasConsumer = false;
            string ns = "";
            BrokerDB brokerDB = new BrokerDB();
            StringBuilder message = new StringBuilder();
            int lssID = 0;
            int lsID = 0;
            ProcessAgentInfo ls = null;
            ProcessAgentInfo lss = null;
            ProcessAgentInfo uss = null;
            LabClient labClient;
            GroupCredential credential = null;

            if (brokerDB.AuthenticateAgentHeader(agentAuthHeader))
            {
                try
                {
                    ResourceDescriptorFactory rFactory = ResourceDescriptorFactory.Instance();
                    string jobGuid = registerId;
                    message.AppendLine(" Register called at " + DateTime.UtcNow + " UTC \t registerGUID: " + registerId);
                    ProcessAgent sourceAgent = brokerDB.GetProcessAgent(agentAuthHeader.agentGuid);
                    message.AppendLine("Source Agent: " + sourceAgent.agentName);

                    if (info == null)
                    {
                        message.AppendLine("Register called without any ServiceDescriptions");
                        throw new ArgumentNullException("Register called without any ServiceDescriptions");
                    }

                    for (int i = 0; i < info.Length; i++)
                    {
                        
                        Coupon coupon = null;
                        if (info[i].coupon != null)
                        {
                            coupon = info[i].coupon;
                        }
                        if (info[i].serviceProviderInfo != null && info[i].serviceProviderInfo.Length > 0)
                        {
                            // ProviderInfo is simple add to database and create qualifier
                            if (!hasProvider)
                            {
                                message.AppendLine("Provider Info:");
                                hasProvider = true;
                            }
                            XmlQueryDoc xdoc = new XmlQueryDoc(info[i].serviceProviderInfo);
                            string descriptorType = xdoc.GetTopName();
                            if (descriptorType.Equals("processAgentDescriptor"))
                            {
                                string paGuid = xdoc.Query("/processAgentDescriptor/agentGuid");
                                string paType = xdoc.Query("/processAgentDescriptor/type");
                                if (paType.Equals(ProcessAgentType.LAB_SCHEDULING_SERVER))
                                {
                                    lssID = brokerDB.GetProcessAgentID(paGuid);
                                    if (lssID > 0)
                                    {
                                        // Already in database
                                        message.AppendLine("Reference to existing LSS: " + lssID + " GUID: " + paGuid);
                                        
                                    }
                                    else
                                    {
                                        lss = rFactory.LoadProcessAgent(xdoc, ref message);
                                        lssID = lss.agentId;
                                    }
                                }
                                else if (paType.Equals(ProcessAgentType.LAB_SERVER))
                                {
                                    lsID = brokerDB.GetProcessAgentID(paGuid);
                                    if (lsID > 0)
                                    {
                                        // Already in database
                                        message.AppendLine("Reference to existing LS: " + lsID + " GUID: " + paGuid);
                                        //ls = brokerDB.GetProcessAgentInfo(paId);
                                    }
                                    else
                                    {
                                        ls = rFactory.LoadProcessAgent(xdoc, ref message);
                                        lsID = ls.agentId;
                                    }
                                    int myLssID = brokerDB.FindProcessAgentIdForAgent(lsID, ProcessAgentType.LAB_SCHEDULING_SERVER);
                                    if ((lssID > 0) && (myLssID <= 0) && (lssID != myLssID))
                                    {
                                        brokerDB.AssociateLSS(lsID, lssID);
                                    }
                                }
                            }
                            else if (descriptorType.Equals("clientDescriptor"))
                            {
                                int clientId = -1;
                                string clientGuid = xdoc.Query("/clientDescriptor/clientGuid");
                                clientId = AdministrativeAPI.GetLabClientID(clientGuid);
                                if (clientId > 0)
                                {
                                    // Already in database
                                    message.Append(" Attempt to Register a LabClient that is already in the database. ");
                                    message.AppendLine(" GUID: " + clientGuid);
                                }
                                else
                                {
                                    // LabServer should already be in the Database, once multiple LS supported may need work
                                    // LS is specified in clientDescriptor
                                    int clientID = rFactory.LoadLabClient(xdoc, ref message);
                                }
                            }
                            // Add Relationships: LSS, LS Client
                        } // end of ServiceProvider
                        if (info[i].consumerInfo != null && info[i].consumerInfo.Length > 0)
                        {
                            if (!hasConsumer)
                                message.AppendLine("Consumer Info:");
                            hasConsumer = true;
                            XmlQueryDoc xdoc = new XmlQueryDoc(info[i].consumerInfo);
                            string descriptorType = xdoc.GetTopName();
                            if (descriptorType.Equals("processAgentDescriptor"))
                            {
                                string paGuid = xdoc.Query("/processAgentDescriptor/agentGuid");
                                ProcessAgentInfo paInfo = brokerDB.GetProcessAgentInfo(paGuid);
                                if (paInfo == null)
                                {
                                    // Not in database
                                    paInfo = rFactory.LoadProcessAgent(xdoc, ref message);
                                    message.Append("Loaded new ");
                                }
                                else
                                {
                                    message.Append("Reference to existing ");
                                }
                                if (paInfo.agentType == ProcessAgentType.AgentType.LAB_SCHEDULING_SERVER)
                                {
                                    lss = paInfo;
                                    message.AppendLine("LSS: " + paGuid);
                                }
                                else if (paInfo.agentType == ProcessAgentType.AgentType.LAB_SERVER)
                                {
                                    ls = paInfo;
                                    message.AppendLine("LS: " + paGuid);
                                }
                                else if (paInfo.agentType == ProcessAgentType.AgentType.SCHEDULING_SERVER)
                                {
                                    uss = paInfo;
                                    message.AppendLine("USS: " + paGuid);
                                    if (lss != null)
                                    {
                                        if (lss.domainGuid.Equals(ProcessAgentDB.ServiceGuid))
                                        {
                                            message.AppendLine("Registering USSinfo on LSS: " + lss.agentName);
                                            LabSchedulingProxy lssProxy = new LabSchedulingProxy();
                                            lssProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                                            lssProxy.AgentAuthHeaderValue.coupon = lss.identOut;
                                            lssProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                                            lssProxy.Url = lss.webServiceUrl;
                                            lssProxy.AddUSSInfo(uss.agentGuid, uss.agentName, uss.webServiceUrl, coupon);
                                        }
                                        else
                                        {
                                            message.AppendLine("LSS is not from this domain");
                                        }


                                    }
                                }

                            }
                            else if (descriptorType.Equals("clientDescriptor"))
                            {
                                int newClientId = -1;
                                string clientGuid = xdoc.Query("/clientDescriptor/clientGuid");
                                int clientId = AdministrativeAPI.GetLabClientID(clientGuid);
                                if (clientId > 0)
                                {
                                    // Already in database
                                    message.Append(" Attempt to Register a LabClient that is already in the database. ");
                                    message.AppendLine(" GUID: " + clientGuid);

                                }
                                else
                                {
                                    clientId = rFactory.LoadLabClient(xdoc, ref message);
                                }
                            }
                            else if (descriptorType.Equals("credentialDescriptor"))
                            {

                                credential = rFactory.ParseCredential(xdoc, ref message);
                                if (lss != null)
                                {
                                    if (lss.domainGuid.Equals(ProcessAgentDB.ServiceGuid))
                                    {
                                        message.AppendLine("Registering Group Credentials on LSS: " + lss.agentName);
                                        message.AppendLine("Group:  " + credential.groupName + " DomainServer: " + credential.domainServerName);
                                        LabSchedulingProxy lssProxy = new LabSchedulingProxy();
                                        lssProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                                        lssProxy.AgentAuthHeaderValue.coupon = lss.identOut;
                                        lssProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                                        lssProxy.Url = lss.webServiceUrl;
                                        lssProxy.AddCredentialSet(credential.domainGuid, credential.domainServerName, credential.groupName, credential.ussGuid);
                                    }
                                    else
                                    {
                                        message.AppendLine("LSS is not from this domain");
                                    }


                                }

                            }
                        }

                    } // End of info loop

                } // End of Try
                catch (Exception ex)
                {
                    message.Append("Exception in Register: " + Utilities.DumpException(ex));
                    throw;
                }
                finally
                {
                    // Send a mail Message
                    StringBuilder sb = new StringBuilder();

                    MailMessage mail = new MailMessage();
                    mail.To = ConfigurationSettings.AppSettings["supportMailAddress"];
                    //mail.To = "pbailey@mit.edu";
                    mail.From = ConfigurationSettings.AppSettings["genericFromMailAddress"];
                    mail.Subject = ProcessAgentDB.ServiceAgent.agentName + " new Service Registration: " + registerId;
                    mail.Body = message.ToString();
                    SmtpMail.SmtpServer = "127.0.0.1";

                    try
                    {
                        SmtpMail.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        // Report detailed SMTP Errors
                        StringBuilder smtpErrorMsg = new StringBuilder();
                        smtpErrorMsg.Append("Exception: " + ex.Message);
                        //check the InnerException
                        if (ex.InnerException != null)
                            smtpErrorMsg.Append("<br>Inner Exceptions:");
                        while (ex.InnerException != null)
                        {
                            smtpErrorMsg.Append("<br>" + ex.InnerException.Message);
                            ex = ex.InnerException;
                        }
                        Utilities.WriteLog(smtpErrorMsg.ToString());
                    }
                }

            } //End of if AthenticateHeader
        }


        /////////////////////////////////////
        ///   ITickerIssuer Methods           ///
        /////////////////////////////////////

        /// <summary>
        /// Attempts to add a ticket of the requested type
        /// to the existing coupon, fails if permissions 
        /// are not available, or the coupon was not issued 
        /// by this serviceBroker.
        /// </summary>
        /// <param> name="coupon"></param>
        /// <param name="redeemer_gid"></param>
        /// <param name="type"></param>
        /// <param name="duration"></param>
        /// <param name="payload"></param>
        /// <param name="sponsor_gid"></param>
        /// <param name="identCoupon"></param>
        /// <returns>the created Ticket or null if creation fails</returns>
        [WebMethod,
        SoapDocumentMethod(Binding = "ITicketIssuer"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public Ticket AddTicket(Coupon coupon, string type, string redeemerGuid,
            long duration, string payload)
        {
            BrokerDB brokerDB = new BrokerDB();
            Ticket ticket = null;
            if (brokerDB.AuthenticateAgentHeader(agentAuthHeader))
            {

                ticket = brokerDB.AddTicket(coupon,
                    type, redeemerGuid, agentAuthHeader.agentGuid, duration, payload);
            }
            return ticket;
        }

        /// <summary>
        /// Request the creation of a ticket of the specified type,
        /// by the Ticketing service. If the credentials pass a 
        /// ticket will be created and accessable by the returned coupon.
        /// Sponsor will be requesting agent ( derive from authHeader the 
        /// agent that was issued the idCoupon ).
        /// </summary>
        /// 
        /// <param name="type"></param>
        /// <param name="redeemerGuid">string GUID of the  requesting agent</param>
        /// <param name="duration">-1 for never, in seconds</param>
        /// <param name="payload"></param>
        /// used to Identify the requester</param>
        /// <returns>Coupon on success, null if Ticket creation is refused</returns>
        [WebMethod,
        SoapDocumentMethod(Binding = "ITicketIssuer"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public Coupon CreateTicket(string type, string redeemerGuid,
            long duration, string payload)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            Coupon coupon = null;
            if (ticketIssuer.AuthenticateAgentHeader(agentAuthHeader))
            {
                if (agentAuthHeader.coupon.issuerGuid == ProcessAgentDB.ServiceGuid)
                {
                    // Note: may need to find requesting service for sponsor.
                    coupon = ticketIssuer.CreateTicket(type, redeemerGuid, agentAuthHeader.agentGuid,
                        duration, payload);
                }
            }
            return coupon;

        }


        /// <summary>
        /// Redeem a ticket from this service,
        /// or if the issuer is known from the remote Issuer.
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="redeemer"></param>
        /// <param name="type"></param>
        /// <returns>the ticket or null</returns>
        [WebMethod,
        SoapDocumentMethod(Binding = "ITicketIssuer"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public Ticket RedeemTicket(Coupon coupon, string type, string redeemerGuid)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            Ticket ticket = null;
            if (ticketIssuer.AuthenticateAgentHeader(agentAuthHeader))
            {
                if (coupon.issuerGuid == ProcessAgentDB.ServiceGuid)
                {
                    ticket = ticketIssuer.RetrieveIssuedTicket(coupon, type, redeemerGuid);
                }
                else
                {
                    ProcessAgentInfo paInfo = ticketIssuer.GetProcessAgentInfo(coupon.issuerGuid);
                    if (paInfo != null)
                    {
                        TicketIssuerProxy ticketProxy = new TicketIssuerProxy();
                        AgentAuthHeader authHeader = new AgentAuthHeader();
                        authHeader.coupon = paInfo.identOut;
                        authHeader.agentGuid = ProcessAgentDB.ServiceGuid;
                        ticketProxy.AgentAuthHeaderValue = authHeader;
                        ticketProxy.Url = paInfo.webServiceUrl;
                        ticket = ticketProxy.RedeemTicket(coupon, type, redeemerGuid);
                    }
                    else
                    {
                        throw new Exception("Unknown TicketIssuerDB in RedeemTicket Request");
                    }
                }

            }
            return ticket;
        }


        /// <summary>
        /// Request The cancellation of an individual ticket, if the coupon 
        /// was not issued by this serviceBroker it will be forwarded, if the issuer is known.
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="redeemer_guid"></param>
        /// <param name="type"></param>
        /// <returns>True if the ticket has been cancelled successfully.</returns>
        [WebMethod,
        SoapDocumentMethod(Binding = "ITicketIssuer"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public bool RequestTicketCancellation(Coupon coupon,
            string type, string redeemerGuid)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            bool status = false;
            if (ticketIssuer.AuthenticateAgentHeader(agentAuthHeader))
            {
                if (coupon.issuerGuid == ProcessAgentDB.ServiceGuid)
                {
                    return ticketIssuer.RequestTicketCancellation(coupon,
                        type, redeemerGuid);
                }
                else
                {
                    ProcessAgentInfo paInfo = ticketIssuer.GetProcessAgentInfo(coupon.issuerGuid);
                    if (paInfo != null)
                    {
                        TicketIssuerProxy ticketProxy = new TicketIssuerProxy();
                        AgentAuthHeader authHeader = new AgentAuthHeader();
                        authHeader.coupon = paInfo.identOut;
                        authHeader.agentGuid = ProcessAgentDB.ServiceGuid;
                        ticketProxy.AgentAuthHeaderValue = authHeader;
                        ticketProxy.Url = paInfo.webServiceUrl;
                        status = ticketProxy.RequestTicketCancellation(coupon, type, redeemerGuid);
                    }
                    else
                    {
                        throw new Exception("Unknown TicketIssuerDB in RedeemTicket Request");
                    }
                }
            }

            return status;

        }




        //////////////////////////////////////////////////////
        ///// BATCH SERVICE BROKER TO LAB SERVER API     /////
        //////////////////////////////////////////////////////


        //////////////////////////////////////////////////////
        ///// INTERACTIVE SERVICE BROKER API               /////
        /////////////////////////////////////////////////////


       

        /// <summary>
        /// Sets a client item value in the user's opaque data store.
        /// </summary>
        /// <param name="name">The name of the client item whose value is to be saved.</param>
        /// <param name="itemValue">The value that is to be saved with name.</param>
        /// <remarks>Web Method</remarks>
        [WebMethod(Description = "Sets a client item value in the user's opaque data store", EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In, Required = true)]        
        [SoapDocumentMethod("http://ilab.mit.edu/iLabs/Type/SaveClientItem",Binding = "IServiceBroker")]      
        public void SaveClientData(string name, string itemValue)
        {
            //first try to recreate session if using a html client
            if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
                 wrapper.SetServiceSession(opHeader.coupon);

            int userID = Convert.ToInt32(Session["UserID"]);
            int clientID = Convert.ToInt32(Session["ClientID"]);
            DataStorageAPI.SaveClientItemValue(clientID, userID, name, itemValue);
                
        }

       
        


        /// <summary>
        /// Returns the value of a client item in the user's opaque data store.  
        /// </summary>
        /// <param name="name">The name of the client item whose value is to be returned.</param>
        /// <returns>The value of a client item in the user's opaque data store.</returns>
        /// <remarks>Web Method</remarks>
        [WebMethod(Description = "Returns the value of an client item in the user's opaque data store", EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In, Required = true)]
        [SoapDocumentMethod("http://ilab.mit.edu/iLabs/Type/LoadClientItem", Binding = "IServiceBroker")]
        public string LoadClientData(string name)
        {
            //first try to recreate session if using a html client
            if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
                wrapper.SetServiceSession(opHeader.coupon);

            int userID = Convert.ToInt32(Session["UserID"]);
            int clientID = Convert.ToInt32(Session["ClientID"]);

            return DataStorageAPI.GetClientItemValue(clientID, userID, new string[] { name })[0].ToString();
        }

        

      
        /// <summary>
        /// Removes a client item from the user's opaque data store.
        /// </summary>
        /// <param name="name">The name of the client item to be removed.</param>
        /// <remarks>Web Method</remarks>
        [WebMethod(Description = "Removes an client item from the user's opaque data store", EnableSession = true)]
      
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In, Required = true)]
        [SoapDocumentMethod("http://ilab.mit.edu/iLabs/Type/DeleteClientItem", Binding = "IServiceBroker")]
        public void DeleteClientData(string name)
        {
            //first try to recreate session if using a html client
            if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
                wrapper.SetServiceSession(opHeader.coupon);

            int userID = Convert.ToInt32(Session["UserID"]);
            int clientID = Convert.ToInt32(Session["ClientID"]);

            DataStorageAPI.RemoveClientItems(clientID, userID, new string[] { name });
        }



        /// <summary>
        /// Enumerates the names of all client items in the user's opaque data store.
        /// </summary>
        /// <returns>An array of client items.</returns>
        /// <remarks>Web Method</remarks>
        [WebMethod(Description = "Enumerates the names of all client items in the user's opaque data store", EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In, Required = true)]
        [SoapDocumentMethod("http://ilab.mit.edu/iLabs/Type/ListAllClientItems", Binding = "IServiceBroker")]
        public string[] ListClientDataItems()
        {
            //first try to recreate session if using a html client
            if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
                wrapper.SetServiceSession(opHeader.coupon);

            int userID = Convert.ToInt32(Session["UserID"]);
            int clientID = Convert.ToInt32(Session["ClientID"]);

            return DataStorageAPI.ListClientItems(clientID, userID);
        }
        

        /////////////////////////////////////////////////////////////////////
        ////  ExperimentStorage Methods                            //////////
        /////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Opens an Experiment on the ServiceBroker, if an ESS is
        /// associated with this experiment the ESS experiment record is configured so that ExperimentRecords
        /// or BLOBs can be written to the ESS.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [WebMethod(Description = "Opens an Experiment on the ServiceBroker, if an ESS is "
        + "associated with this experiment the ESS experiment record is configured so that ExperimentRecords "
        + "or BLOBs can be written to the ESS.",
        EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public StorageStatus OpenExperiment(long experimentId, long duration)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            StorageStatus status = null;
            if (ticketIssuer.AuthenticateAgentHeader(agentAuthHeader))
            {
                Ticket essTicket = ticketIssuer.RetrieveTicket(opHeader.coupon, TicketTypes.ADMINISTER_EXPERIMENT);
                // Check for ESS use
                if (essTicket != null)
                {
                    XmlDocument payload = new XmlDocument();
                    payload.LoadXml(essTicket.payload);
                    string essURL = payload.GetElementsByTagName("essURL")[0].InnerText;

                    long sbExperimentId = Int64.Parse(payload.GetElementsByTagName("experimentID")[0].InnerText);
                    //
                    ExperimentSummary summary = InternalDataDB.SelectExperimentSummary(experimentId);
                    if (summary.HasEss)
                    {
                        // Retrieve the ESS Status info and update as needed
                        ProcessAgentInfo ess = ticketIssuer.GetProcessAgentInfo(summary.essGuid);

                        ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                        essProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                        essProxy.AgentAuthHeaderValue.coupon = ess.identOut;
                        essProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                        essProxy.Url = essURL;
                        status = essProxy.OpenExperiment(sbExperimentId, duration);
                    }

                    // Note: store and retrieve tickets are not cancelled.
                }
            }
            if (status != null)
            {
                DataStorageAPI.UpdateExperimentStatus(status);
            }
            return status;
        }

        /// <summary>
        /// Closes an Experiment on the ServiceBroker, if an ESS is
        /// associated with this experiment the ESS experiment is closed so that no further ExperimentRecords
        /// or BLOBs can be written to the ESS.
        /// </summary>
        /// <param name="coupon">coupon issued as part of ExperimentExecution collection</param>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Closes an Experiment on the ServiceBroker, if the SB is not the "
        + "issuer of the coupon the call is forwarded to the issuer. If an ESS is "
     + "associated with this experiment the ESS experiment is closed so that no further ExperimentRecords "
     + "or BLOBs can be written to the ESS.",
     EnableSession = true)]
        [SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public StorageStatus AgentCloseExperiment(Coupon coupon, long experimentId)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            StorageStatus status = null;
            bool experimentClosed = false;

            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                if (coupon.issuerGuid == ProcessAgentDB.ServiceGuid)
                {

                    // Check for ESS use
                    Ticket essTicket = ticketIssuer.RetrieveTicket(coupon, TicketTypes.ADMINISTER_EXPERIMENT);
                    if (essTicket != null)
                    {
                        ProcessAgentInfo ess = ticketIssuer.GetProcessAgentInfo(essTicket.redeemerGuid);
                        if (ess != null)
                        {
                            ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                            essProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                            essProxy.AgentAuthHeaderValue.coupon = ess.identOut;
                            essProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                            essProxy.Url = ess.webServiceUrl;
                            status = essProxy.CloseExperiment(experimentId);
                            DataStorageAPI.UpdateExperimentStatus(status);
                        }
                        ticketIssuer.CancelIssuedTicket(coupon, essTicket);
                    }
                    else
                    {
                        // Close the local Experiment records
                        // Note: store and retrieve tickets are not cancelled.
                        experimentClosed = DataStorageAPI.CloseExperiment(experimentId, StorageStatus.CLOSED_USER);
                        status = DataStorageAPI.RetrieveExperimentStatus(experimentId);
                    }


                }
                else
                {
                    ProcessAgentInfo paInfo = ticketIssuer.GetProcessAgentInfo(coupon.issuerGuid);
                    if (paInfo != null)
                    {
                        InteractiveSBProxy ticketProxy = new InteractiveSBProxy();
                        AgentAuthHeader authHeader = new AgentAuthHeader();
                        authHeader.coupon = paInfo.identOut;
                        authHeader.agentGuid = ProcessAgentDB.ServiceGuid;
                        ticketProxy.AgentAuthHeaderValue = authHeader;
                        ticketProxy.Url = paInfo.webServiceUrl;
                        status = ticketProxy.AgentCloseExperiment(coupon, experimentId);
                    }
                    else
                    {
                        throw new Exception("Unknown TicketIssuerDB in RedeemTicket Request");
                    }
                }

            }
            return status;
        }

        /// <summary>
        /// Closes an Experiment on the ServiceBroker, if an ESS is
        /// associated with this experiment the ESS experiment is closed so that no further ExperimentRecords
        /// or BLOBs can be written to the ESS.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Closes an Experiment on the ServiceBroker, if an ESS is "
     + "associated with this experiment the ESS experiment is closed so that no further ExperimentRecords "
     + "or BLOBs can be written to the ESS.",
     EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public StorageStatus ClientCloseExperiment(long experimentId)
        {
            BrokerDB ticketIssuer = new BrokerDB();
            StorageStatus status = null;
            bool experimentClosed = false;

            //Coupon opCoupon = new Coupon(opHeader.coupon.issuerGuid, opHeader.coupon.couponId,
            //     opHeader.coupon.passkey);
            if (ticketIssuer.AuthenticateIssuedCoupon(opHeader.coupon))
            {
                Ticket expTicket = ticketIssuer.RetrieveTicket(opHeader.coupon, TicketTypes.EXECUTE_EXPERIMENT);
                if (expTicket != null)
                {
                    // Check for ESS use
                    Ticket essTicket = ticketIssuer.RetrieveTicket(opHeader.coupon, TicketTypes.ADMINISTER_EXPERIMENT);
                    if (essTicket != null)
                    {
                        ProcessAgentInfo ess = ticketIssuer.GetProcessAgentInfo(essTicket.redeemerGuid);
                        if (ess != null)
                        {

                            ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                            essProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                            essProxy.AgentAuthHeaderValue.coupon = ess.identOut;
                            essProxy.Url = ess.webServiceUrl;
                            status = essProxy.CloseExperiment(experimentId);
                            DataStorageAPI.UpdateExperimentStatus(status);
                        }
                        ticketIssuer.CancelIssuedTicket(opHeader.coupon, essTicket);
                    }
                    else
                    {
                        // Close the local Experiment records
                        // Note: store and retrieve tickets are not cancelled.
                        experimentClosed = DataStorageAPI.CloseExperiment(experimentId, StorageStatus.CLOSED_USER);
                        status = DataStorageAPI.RetrieveExperimentStatus(experimentId);
                    }

                }
            }
            return status;
        }


        [WebMethod(Description = "Uses the ExecuteExperimentTicket to derive client,user and group IDs to authorize access. Criteria may be specified"
        + " valid field names include; userName, groupName, labServerName,clientName, scheduledStart,creationTime, status "
        + " annotation, and experimentID."
        + " If an ESS is specified, record_Type, contents and record attributes may also be checked.",
        EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public long[] RetrieveExperimentIds(Criterion[] carray)
        {
            BrokerDB brokerDB = new BrokerDB();

            long[] expIds = null;
            if (brokerDB.AuthenticateIssuedCoupon(opHeader.coupon))
            {
                expIds = getExperimentIDs(opHeader.coupon, carray);
            }
            return expIds;
        }


        protected long[] getExperimentIDs(Coupon opCoupon, Criterion[] carray)
        {
            BrokerDB brokerDB = new BrokerDB();
            int userID = 0;
            int groupID = 0;
            long[] expIDs = null;
            Ticket expTicket = brokerDB.RetrieveTicket(opCoupon, TicketTypes.REDEEM_SESSION);
            if (expTicket != null && !expTicket.IsExpired())
            {
                //Parse payload, only get what is needed 	

                XmlQueryDoc expDoc = new XmlQueryDoc(expTicket.payload);
                long expID = -1;

                string userStr = expDoc.Query("RedeemSessionPayload/userID");
                if ((userStr != null) && (userStr.Length > 0))
                    userID = Convert.ToInt32(userStr);
                string groupStr = expDoc.Query("RedeemSessionPayload/groupID");
                if ((groupStr != null) && (groupStr.Length > 0))
                    groupID = Convert.ToInt32(groupStr);

                if (userID > 0)
                {

                    expIDs = DataStorageAPI.RetrieveAuthorizedExpIDs(userID, groupID, carray);
                }
            }
            return expIDs;
        }

        [WebMethod(Description = "Uses the users qualifiers to authorize access, forwards call to ESS",
        EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public Experiment RetrieveExperiment(long experimentID)
        {
            Experiment experiment = null;
            BrokerDB brokerDB = new BrokerDB();
            int roles = 0;
            int userID = 0;
            int groupID = 0;
            long[] expIDs = null;
            Ticket expTicket = brokerDB.RetrieveTicket(opHeader.coupon, TicketTypes.REDEEM_SESSION);
            if (expTicket != null && !expTicket.IsExpired())
            {
                //Parse payload, only get what is needed 	

                XmlQueryDoc expDoc = new XmlQueryDoc(expTicket.payload);
                long expID = -1;

                string userStr = expDoc.Query("RedeemSessionPayload/userID");
                if ((userStr != null) && (userStr.Length > 0))
                    userID = Convert.ToInt32(userStr);
                string groupStr = expDoc.Query("RedeemSessionPayload/groupID");
                if ((groupStr != null) && (groupStr.Length > 0))
                    groupID = Convert.ToInt32(groupStr);

                if (userID > 0)
                {

                    AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
                    roles = wrapper.GetExperimentAuthorizationWrapper(experimentID, userID, groupID);
                }
                if ((roles | ExperimentAccess.READ) == ExperimentAccess.READ)
                {
                    experiment = new Experiment();
                    experiment.experimentId = experimentID;
                    experiment.issuerGuid = ProcessAgentDB.ServiceGuid;
                    ProcessAgentInfo ess = brokerDB.GetExperimentESS(experimentID);
                    if (ess != null)
                    {
                        ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                        Coupon opCoupon = brokerDB.GetEssOpCoupon(experimentID, TicketTypes.RETRIEVE_RECORDS, 60, ess.agentGuid);
                        if (opCoupon == null)
                        {
                            string payload = TicketLoadFactory.Instance().RetrieveRecordsPayload(experimentID, ess.webServiceUrl);
                            opCoupon = brokerDB.CreateTicket(TicketTypes.RETRIEVE_RECORDS, ess.agentGuid, ProcessAgentDB.ServiceGuid,
                                60, payload);
                        }
                        essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                        essProxy.OperationAuthHeaderValue.coupon = opCoupon;
                        essProxy.Url = ess.webServiceUrl;
                        essProxy.GetRecords(experimentID, null);
                    }

                }
                else
                {
                    throw new AccessDeniedException("You do not have permission to read this experiment");
                }
            }
            return experiment;
        }


        [WebMethod(Description = "Uses the users qualifiers to select Experiment summaries, no write permissions are created."
        + " Valid field names include; userName, groupName, labServerName,clientName, scheduledStart,creationTime, status "
        + " and experimentID.",
        EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public ExperimentSummary[] RetrieveExperimentSummary(Criterion[] carray)
        {
            BrokerDB brokerDB = new BrokerDB();
            ExperimentSummary[] summaries = null;
            if (brokerDB.AuthenticateIssuedCoupon(opHeader.coupon))
            {
                long[] expIds = getExperimentIDs(opHeader.coupon, carray);
                summaries = InternalDataDB.SelectExperimentSummaries(expIds);

            }
            return summaries;
        }

        [WebMethod(Description = "Uses the users qualifiers to select Experiment summaries, no write permissions are created."
+ " Valid field names include; userName, groupName, labServerName,clientName, scheduledStart,creationTime, status "
+ " and experimentID.",
EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public ExperimentRecord[] RetrieveExperimentRecords(long experimentID, Criterion[] carray)
        {
                  ExperimentRecord[]  records = null;
            BrokerDB brokerDB = new BrokerDB();
            int roles = 0;
            int userID = 0;
            int groupID = 0;
            long[] expIDs = null;
            Ticket expTicket = brokerDB.RetrieveTicket(opHeader.coupon, TicketTypes.REDEEM_SESSION);
            if (expTicket != null && !expTicket.IsExpired())
            {
                //Parse payload, only get what is needed 	

                XmlQueryDoc expDoc = new XmlQueryDoc(expTicket.payload);
                long expID = -1;

                string userStr = expDoc.Query("RedeemSessionPayload/userID");
                if ((userStr != null) && (userStr.Length > 0))
                    userID = Convert.ToInt32(userStr);
                string groupStr = expDoc.Query("RedeemSessionPayload/groupID");
                if ((groupStr != null) && (groupStr.Length > 0))
                    groupID = Convert.ToInt32(groupStr);

                if (userID > 0)
                {

                    AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
                    roles = wrapper.GetExperimentAuthorizationWrapper(experimentID, userID, groupID);
                }
                if ((roles | ExperimentAccess.READ) == ExperimentAccess.READ)
                {
                    records = brokerDB.RetrieveExperimentRecords(experimentID, carray);
                }
                else
                {
                    throw new AccessDeniedException("You do not have the required permission to access the experiment");
                }
            }
                return records;
        }

        [WebMethod(Description = "Uses the cridentials granted the experiment specified by the opHeader to check "
            + "access to the requested experiment, if allowed a new ticket collection is started "
        + "to access the requested experiment and optional ESS records. Returns null if access denied.",
        EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public Coupon RequestExperimentAccess(long experimentID)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            Coupon coupon = null;
            //first try to recreate session if using a html client
            //if ((Session == null) || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))


            if (ticketIssuer.AuthenticateIssuedCoupon(opHeader.coupon))
            {
                Ticket sessionTicket = ticketIssuer.RetrieveTicket(opHeader.coupon, TicketTypes.REDEEM_SESSION);
                if (sessionTicket != null)
                {


                    if (sessionTicket.IsExpired())
                    {
                        throw new AccessDeniedException("The ticket has expired.");
                    }

                    //Parse payload 	
                    XmlQueryDoc expDoc = new XmlQueryDoc(sessionTicket.payload);
                    // Get User & Group
                    int userID = -1;
                    int groupID = -1;
                    string group = expDoc.Query("RedeemSessionPayload/groupID");
                    string user = expDoc.Query("RedeemSessionPayload/userID");
                    if (group != null && group.Length > 0)
                    {
                        groupID = Convert.ToInt32(group);
                    }
                    if (user != null && user.Length > 0)
                    {
                        userID = Convert.ToInt32(user);
                    }



                    //Check Qualifiers on experiment
                    int status = wrapper.GetExperimentAuthorizationWrapper(experimentID, userID, groupID);
                    //if accessable by user create new TicketCollection
                }
            }
            return coupon;
        }
        /// <summary>
        /// Saves or modifies an optional user defined annotation to the experiment record.
        /// </summary>
        /// <param name="experimentID">A token which identifies the experiment.</param>
        /// <param name="annotation">The annotation to be saved with the experiment.</param>
        /// <returns>The previous annotation or null if there wasn't one.</returns>
        /// <remarks>Web Method</remarks>
        [WebMethod(Description = "Saves or modifies an optional user defined annotation to the experiment record.", EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public string SetAnnotation(int experimentID, string annotation)
        {
            //first try to recreate session if using an html client
            if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
            {
                wrapper.SetServiceSession(opHeader.coupon);
            }
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            if (ticketIssuer.AuthenticateIssuedCoupon(opHeader.coupon))
            {
                try
                {
                    //first try to recreate session if using a html client
                    if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
                        wrapper.SetServiceSession(opHeader.coupon);


                    return wrapper.SaveExperimentAnnotationWrapper(experimentID, annotation);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Retrieves a previously saved experiment annotation.
        /// </summary>
        /// <param name="experimentID">A token which identifies the experiment.</param>
        /// <returns>The annotation, a string originally created by the user via the Lab Client.</returns>
        /// <remarks>Web Method</remarks>
        [WebMethod(Description = "Retrieves a previously saved experiment annotation.", EnableSession = true)]
        [SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public string GetAnnotation(int experimentID)
        {
            //first try to recreate session if using an html client
            if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
            {
                wrapper.SetServiceSession(opHeader.coupon);
            }
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            if (ticketIssuer.AuthenticateIssuedCoupon(opHeader.coupon))
            {
                try
                {
                    //first try to recreate session if using a html client
                    if (Session == null || (Session["UserID"] == null) || (Session["UserID"].ToString() == ""))
                        wrapper.SetServiceSession(opHeader.coupon);

                    return wrapper.SelectExperimentAnnotationWrapper(experimentID);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                return null;
        }

    }

}
