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
using iLabs.Proxies.PAgent;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.Proxies.LSS;
using iLabs.Proxies.Ticketing;
using iLabs.Proxies.USS;

using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.DataStorage;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker.Mapping;
using iLabs.Ticketing;
using iLabs.TicketIssuer;
using iLabs.UtilLib;
using iLabs.Web;

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
    WebService(Name = "InteractiveServiceBroker", Namespace = "http://ilab.mit.edu/iLabs/Services",
        Description="ServiceBrokerService contains all of the Service Broker Web Service Calls."
	+ " All of the Service Broker to Lab Server passthrough calls run in the context of a user's "
	+ " Service Broker Web Interface session, and consequently have session enabled. This works"
	+ " because they are submitted from the Java Client, through one or more of the Service Broker"
	+ " passthrough methods, on to the corresponding method on the Lab Server. "
	+ " There is one Method, Notify(), that is called from the Lab Server directly. This"
	+ " runs outside of the session context.")]
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
        /// Modify the specified services Domain credentials on this static process agent. 
        /// Agent_guid is key to the service to be modified. The agentAuthorizationHeader must use the old values.
        /// </summary>
        /// <param name="credentials" Description="Used to provide information for the service to be modified, the agent guid must remain the same as the original for the specified service"></param>
        /// <returns>A DomainCredentials with the results of the requested change, depending on he service type these results may be used or ignored as needed.</returns>
        [WebMethod(Description = "Modify the specified services Domain credentials on this static process agent. "
            + "Agent_guid is key to the service to be modified and may not be modiied. The agentAuthorizationHeader must use the old values."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public override int ModifyDomainCredentials(string originalGuid, ProcessAgent agent, string extra, 
            Coupon inCoupon, Coupon outCoupon)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                BrokerDB brokerDB = new BrokerDB();
                status = brokerDB.ModifyDomainCredentials(originalGuid, agent, inCoupon, outCoupon, extra);

                //Notify all ProcessAgents about the change
                ProcessAgentInfo[] domainServices = dbTicketing.GetProcessAgentInfos();
                ProcessAgentProxy proxy = null;
                foreach (ProcessAgentInfo pi in domainServices)
                {
                    // Do not send if retired this service or the service being modified since this is
                    if (!pi.retired && (pi.agentGuid.CompareTo(ProcessAgentDB.ServiceGuid) != 0)
                        && (pi.agentGuid.CompareTo(agent.agentGuid) != 0))
                    {
                        proxy = new ProcessAgentProxy();
                        proxy.AgentAuthHeaderValue = new AgentAuthHeader();
                        proxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                        proxy.AgentAuthHeaderValue.coupon = pi.identOut;
                        proxy.Url = pi.webServiceUrl;

                        status += proxy.ModifyDomainCredentials(originalGuid, agent, extra, inCoupon, outCoupon);
                    }
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
                BrokerDB brokerDB = new BrokerDB();
                status = brokerDB.ModifyProcessAgent(originalGuid, agent, extra);

             
            }
            return status;
        }

        /// <summary>
        /// Informs a processAgent that it should retire/un-retire all references to a specific processAent. 
        /// This may be used to propigate retire calls.
        /// </summary>
        /// <param name="domainGuid">The guid of the services domain ServiceBroker</param>
        /// <param name="serviceGuid">The guid of the service</param>
        /// <param name="state">The retired state to be set, true sets retired.</param>
        /// <returns>A status value, negative values indicate errors, zero indicates unknown service, positive indicates level of success.</returns>
        [WebMethod(Description = "Informs a processAgent that it should retire/un-retire all references to a specific processAent."
        + " This may be used to propigate retire calls."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public override int RetireProcessAgent(string domainGuid, string serviceGuid, bool state)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                status = retireProcessAgent(domainGuid, serviceGuid, state);
            }
            return status;
        }

        /// <summary>
        /// Install the Domain credentials on this static process agent
        /// </summary>
        /// <param name="initialPasskey"></param>
        /// <param name="agent" Description="used to provide the service address of the the agent not stored on the agent"></param>
        /// <param name="agentIdentCoupon" Description="For messages from the PA_Service"></param>
        /// <param name="serviceBroker" Description="service information stored on PA_Service"></param>
        /// <param name="sbIdentCoupon" Description="For messages from the SB"></param>
       
        protected override ProcessAgent installDomainCredentials(ProcessAgent service,
            Coupon inIdentCoupon, Coupon outIdentCoupon)
        {
            // if the remote process agent is a Service Broker, register it as a remote service broker
            if (service.type.Equals(ProcessAgentType.SERVICE_BROKER))
            {
                service.type = ProcessAgentType.REMOTE_SERVICE_BROKER;
            }
            //else{
            return base.installDomainCredentials(service, inIdentCoupon, outIdentCoupon);


        }
/*
        protected override int modifyProcessAgent(string originalGuid, ProcessAgent agent, string extra)
        {
            int status = 0;
            BrokerDB brokerDB = new BrokerDB();
            int id = dbTicketing.GetProcessAgentID(originalGuid);
            if(id > 1){
                status = brokerDB.ModifyProcessAgent(originalGuid, agent, extra);
            }
          

            return status;
        }

*/

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
                        if (target.retired)
                        {
                            throw new Exception("The ProcessAgent is retired");
                        }
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
                                if (remoteSB.retired)
                                {
                                    throw new Exception("The ProcessAgent is retired");
                                }
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

      


        protected override void register(string registerGuid, ServiceDescription[] info)
        {

            StringBuilder message = new StringBuilder();
            message.AppendLine("Service " + ProcessAgentDB.ServiceAgent.codeBaseUrl + " recieved a 'Register' webService call.");
            

            if (info == null)
            {
                //message.AppendLine("Register called without any ServiceDescriptions");
                throw new ArgumentNullException("Register called without any ServiceDescriptions");
            }

            try
            {
                base.register(registerGuid, info);
            }
            catch (Exception e)
            {
                message.AppendLine("Error in base.register" + Utilities.DumpException(e));
                throw new Exception(message.ToString(), e);
            }
            bool hasProvider = false;
            bool hasConsumer = false;
            string ns = "";
            BrokerDB brokerDB = new BrokerDB();
           
            int lssID = 0;
            int lsID = 0;
            ProcessAgentInfo ls = null;
            ProcessAgentInfo lss = null;
            ProcessAgentInfo uss = null;
            LabClient labClient = null;
            GroupCredential credential = null;
            try
            {
                ResourceDescriptorFactory rFactory = ResourceDescriptorFactory.Instance();
                string jobGuid = registerGuid;
                message.AppendLine(" Register called at " + DateTime.UtcNow + " UTC \t registerGUID: " + registerGuid);
                ProcessAgent sourceAgent = brokerDB.GetProcessAgent(agentAuthHeader.agentGuid);
                message.AppendLine("Source Agent: " + sourceAgent.agentName);

               

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
                                    //message.AppendLine("Reference to existing LSS: " + lssID + " GUID: " + paGuid);

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
                                    //message.AppendLine("Reference to existing LS: " + lsID + " GUID: " + paGuid);
                                    
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
                                message.AppendLine("Adding LabClient: GUID " + clientGuid);
                            }
                        }
                        else if (descriptorType.Equals("systemSupport"))
                        {
                            // Already handled in base.register
                          
                        }
                        // Add Relationships: LSS, LS Client
                    } // end of ServiceProvider
                    if (info[i].consumerInfo != null && info[i].consumerInfo.Length > 0)
                    {
                          // requestSystemSupport Handled by base register & there is no xml dcument
                        if (info[i].consumerInfo.CompareTo("requestSystemSupport") != 0)
                        {
                            message.AppendLine("Consumer Info: " + info[i].consumerInfo);
                            //if (!hasConsumer)
                            //    message.AppendLine("Consumer Info: " + info[i].consumerInfo);
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
                                    if (paInfo.retired)
                                    {
                                        throw new Exception("The ProcessAgent is retired");
                                    }
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
                                    message.AppendLine("Adding Lab Client GUID: " + clientGuid);
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
                    }
                } // End of info loop
            } // End of Try
            catch (Exception ex)
            {
                message.Append("Exception in Register: " + Utilities.DumpException(ex));

                throw new Exception(message.ToString(),ex);
            }
            finally
            {
                // Send a mail Message
                StringBuilder sb = new StringBuilder();

                MailMessage mail = new MailMessage();
                mail.To = ConfigurationSettings.AppSettings["supportMailAddress"];
                //mail.To = "pbailey@mit.edu";
                mail.From = ConfigurationSettings.AppSettings["genericFromMailAddress"];
                mail.Subject = "Register called on " + ProcessAgentDB.ServiceAgent.agentName;
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
                    Logger.WriteLine(smtpErrorMsg.ToString());
                }
            }
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
                        if (paInfo.retired)
                        {
                            throw new Exception("The ProcessAgent is retired");
                        }
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
                        if (paInfo.retired)
                        {
                            throw new Exception("The ProcessAgent is retired");
                        }
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

        
        /// <summary>
        /// Request authorization for the specified types of access, for the specified group and optional user. At this time remote SB's are not supported.
        /// </summary>
        /// <param name="types">An array of requested ticket types</param>
        /// <param name="duration">minimum duration of the created tickets</param>
        /// <param name="groupName">group name on the ticketIssuer service, may in the future support validation from the service making the request</param>
        /// <param name="userName">User name on the ticketIssuer service, may in the future support validation from the service making the request, may be null</param>
        /// <param name="serviceGuid">May be null</param>
        /// <param name="clientGuid">May be null</param>
        /// <returns>An operationCoupon or null</returns>
        [WebMethod(Description = "Request authorization for the specified types of access, for the specified group, user, server and  client.")]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        [SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public Coupon RequestAuthorization(string[] types, long duration, string group, string user, string serviceGuid, string clientGuid)
        {
            bool ok = false;
            BrokerDB brokerDB = new BrokerDB();
            Coupon coupon = brokerDB.CreateCoupon();
            //if (brokerDB.AuthenticateAgentHeader(agentAuthHeader))
            //{
            if (ProcessAgentDB.ServiceGuid.CompareTo(agentAuthHeader.coupon.issuerGuid) == 0)
            {
                if (types != null && types.Length > 0)
                {
                    int userID = -1;
                    int groupID = -1;
                    int lcID = -1;
                    int lsID = -1;
                    int lssID = -1;
                    int ussID = -1;
                    int essID = -1;
                    ProcessAgent lss = null;
                    ProcessAgent uss = null;
                    ProcessAgent ess = null;
                    if (clientGuid != null && clientGuid.Length > 0)
                        lcID = AdministrativeAPI.GetLabClientID(clientGuid);
                    if (serviceGuid != null && serviceGuid.Length > 0)
                        lsID = brokerDB.GetProcessAgentID(serviceGuid);
                    if (lsID > 0)
                        lssID = ResourceMapManager.FindResourceProcessAgentID(ResourceMappingTypes.PROCESS_AGENT, lsID, ProcessAgentType.LAB_SCHEDULING_SERVER);
                    if (lcID > 0)
                    {
                        ussID = ResourceMapManager.FindResourceProcessAgentID(ResourceMappingTypes.CLIENT, lcID, ProcessAgentType.SCHEDULING_SERVER);
                        essID = ResourceMapManager.FindResourceProcessAgentID(ResourceMappingTypes.CLIENT, lcID, ProcessAgentType.EXPERIMENT_STORAGE_SERVER);
                        if (lssID > 0)
                            lss = brokerDB.GetProcessAgent(lssID);
                        if (ussID > 0)
                            uss = brokerDB.GetProcessAgent(ussID);
                        if (essID > 0)
                            ess = brokerDB.GetProcessAgent(essID);

                        TicketLoadFactory factory = TicketLoadFactory.Instance();

                        foreach (string str in types)
                        {
                            switch (str)
                            {
                                case TicketTypes.CREATE_EXPERIMENT:
                                    break;
                                case TicketTypes.EXECUTE_EXPERIMENT:
                                    break;
                                case TicketTypes.ALLOW_EXPERIMENT_EXECUTION:
                                    break;
                                case TicketTypes.RETRIEVE_RECORDS:
                                    break;
                                case TicketTypes.STORE_RECORDS:
                                    break;
                                case TicketTypes.SCHEDULE_SESSION:
                                    // Create the Authority's SCHEDULE_SESSION ticket
                                    string payloadUss = factory.makeReservationPayload(ProcessAgentDB.ServiceGuid, user, group, serviceGuid, clientGuid, uss.webServiceUrl);
                                    brokerDB.AddTicket(coupon, TicketTypes.SCHEDULE_SESSION, uss.agentGuid, agentAuthHeader.agentGuid, duration, payloadUss);
                                    // Create the USS to LSS REQUEST_RESERVATION Ticket
                                    brokerDB.AddTicket(coupon, TicketTypes.REQUEST_RESERVATION, lss.agentGuid, uss.agentGuid, duration, null);
                                    // Create the USS to LSS
                                    brokerDB.AddTicket(coupon, TicketTypes.SCHEDULE_SESSION, lss.agentGuid, uss.agentGuid, duration, null);
                                    ok = true;
                                    break;
                                case TicketTypes.REDEEM_RESERVATION:
                                    break;
                                case TicketTypes.REVOKE_RESERVATION:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            if (ok)
                return coupon;
            else
                return null;
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
        + "or BLOBs can be written to the ESS.", EnableSession = true)]
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
                        if (ess.retired)
                        {
                            throw new Exception("The ESS is retired");
                        }
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
                            if (ess.retired)
                            {
                                throw new Exception("The ProcessAgent is retired");
                            }
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
                        if (paInfo.retired)
                        {
                            throw new Exception("The ProcessAgent is retired");
                        }
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
                            if (ess.retired)
                            {
                                throw new Exception("The ProcessAgent is retired");
                            }
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

        /// <summary>
        /// Revokes reservations that intersect the specifications, may be called from the USS or a RemoteServiceBroker
    /// </summary>
/// <param name="userName"></param>
/// <param name="groupName"></param>
/// <param name="labServerGuid"></param>
/// <param name="clientCuid"></param>
/// <param name="startTime"></param>
/// <param name="endTime"></param>
/// <param name="message"></param>
/// <returns></returns>
        [WebMethod(Description = "Revokes reservations that intersect the specifications, may be called from the USS or a Remote ServiceBroker", EnableSession = true)]
        [SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        [SoapDocumentMethod(Binding = "IServiceBroker")]
        public bool RevokeReservation(string serviceBrokerGuid, string userName, string groupName, string labServerGuid, string labClientGuid,
            DateTime startTime, DateTime endTime, string message)
        {
            TicketIssuerDB ticketIssuer = new TicketIssuerDB();
            bool status = false;
            if (ticketIssuer.AuthenticateAgentHeader(agentAuthHeader))
            {
                if (agentAuthHeader.coupon.issuerGuid == ProcessAgentDB.ServiceGuid)
                {
                    try
                    {
                        int userId = AdministrativeAPI.GetUserID(userName);
                        if (userId > 0)
                        {
                            User[] users = AdministrativeAPI.GetUsers(new int[] { userId });
                            if (users != null && users.Length > 0)
                            {
                                SmtpMail.SmtpServer = "127.0.0.1";
                                if (users[0].email != null)
                                {
                                    MailMessage uMail = new MailMessage();
                                    uMail.To = users[0].email;
                                    uMail.From = ConfigurationSettings.AppSettings["supportMailAddress"];
                                    uMail.Subject = "[iLabs] A Reservation has been revoked!";
                                    StringBuilder buf = new StringBuilder();
                                    buf.Append("Your scheduled reservation for ");
                                    buf.Append(AdministrativeAPI.GetLabClientName(AdministrativeAPI.GetLabClientID(labClientGuid)));
                                    buf.Append(", from " + DateUtil.ToUtcString(startTime) + " to " + DateUtil.ToUtcString(endTime));
                                    buf.AppendLine(" has been removed by an external service for the following reason: ");
                                    buf.AppendLine(message);
                                    buf.AppendLine("Please make a new reservation.");

                                    uMail.Body = buf.ToString(); ;
                                    SmtpMail.Send(uMail);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Report detailed SMTP Errors
                        StringBuilder smtpErrorMsg = new StringBuilder();
                        smtpErrorMsg.Append("Exception: SMTP in InterativeSB:" + ex.Message);
                        //check the InnerException
                        if (ex.InnerException != null)
                            smtpErrorMsg.Append("<br>Inner Exceptions:");
                        while (ex.InnerException != null)
                        {
                            smtpErrorMsg.Append("<br>" + ex.InnerException.Message);
                            ex = ex.InnerException;
                        }
                       Logger.WriteLine(smtpErrorMsg.ToString());
                       
                    }
                    status = true;
                }
            }
            else
            {
                ProcessAgentInfo paInfo = ticketIssuer.GetProcessAgentInfo(agentAuthHeader.coupon.issuerGuid);
                if (paInfo != null)
                {
                    if (paInfo.retired)
                    {
                        throw new Exception("The ProcessAgent is retired");
                    }
                    InteractiveSBProxy proxy = new InteractiveSBProxy();
                    AgentAuthHeader authHeader = new AgentAuthHeader();
                    authHeader.coupon = paInfo.identOut;
                    authHeader.agentGuid = ProcessAgentDB.ServiceGuid;
                    proxy.AgentAuthHeaderValue = authHeader;
                    proxy.Url = paInfo.webServiceUrl;
                    status = proxy.RevokeReservation(serviceBrokerGuid, userName, groupName, labServerGuid, labClientGuid,
                        startTime, endTime, message);
                }
                else
                {
                    throw new Exception("Unknown TicketIssuerDB in RedeemTicket Request");
                }
            }
            return status;
        }

    }

}
