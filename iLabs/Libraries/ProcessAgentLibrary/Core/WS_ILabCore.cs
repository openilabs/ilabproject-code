/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: WS_ILabCore.cs,v 1.18 2007/12/26 05:27:22 pbailey Exp $ */

using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.Threading ;
using System.Web.Services.Protocols;
using System.Net;
using System.Web.SessionState;
using System.Reflection;

using iLabs.Core;
using iLabs.Ticketing;
using iLabs.UtilLib;

using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;


namespace iLabs.Core
{
	/// <summary>
	/// WS_ILabCore provides a base WebService implementation of all of the ProcessAgent required Web Service Methods.
	/// </summary>
	/// 
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/Type")]
    [WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "ILabCore", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public class WS_ILabCore : System.Web.Services.WebService
    {

        protected ProcessAgentDB dbTicketing;

        /// <summary>
        /// Instantiated to recieve soap header objects in SOAP requests
        /// </summary>

        public BrokerAuthHeader brokerAuthHeader = new BrokerAuthHeader();
        public AgentAuthHeader agentAuthHeader = new AgentAuthHeader();
        public InitAuthHeader initAuthHeader = new InitAuthHeader();


        public WS_ILabCore()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
            dbTicketing = new ProcessAgentDB();
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

        /////////////////////////////////////////////////////
        /// ProcessAgent Methods                          ///
        ////////////////////////////////////////////////////

        /// <summary>
        /// Get the processAgent's local time, this is not in UTC.
        /// </summary>
        /// <returns></returns>
        [WebMethod,
       SoapDocumentMethod(Binding = "IProcessAgent")]
        public DateTime GetServiceTime()
        {
            //if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            //{
            //    return DateTime.Now;
            //}
            //else
            //{
                return DateTime.Now;
            //}
        }
    
        /// <summary>
        /// Generate a statusReort.
        /// </summary>
        [WebMethod,
       SoapDocumentMethod(Binding = "IProcessAgent")]
        public StatusReport GetStatus()
        {
            StatusReport status = new StatusReport();
            status.online = true;
            status.serviceGuid = ProcessAgentDB.ServiceGuid;
            //status.payload = "Optional payload";
            return status;
        }

        /// <summary>
        /// Process the statusNotificationReport. Currently no processing has been specified.
        /// </summary>
        [WebMethod,
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public virtual void StatusNotification(StatusNotificationReport report)
        {
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                // No default processing of the StatusNotification message has been specified
            }
        }

        /// <summary>
        /// Try to delete a cached ticket.
        /// If the receiver is a serviceBroker and is not the redeemer the call needs to be repackaged and forwarded to the redeemer.
        /// Each ProcessAgent type may need to override this method depending on the ticket type.
        /// </summary>
        /// <param name="coupon">collection coupon</param>
        /// <param name="type">ticket type</param>
        /// <param name="redeemer">the ticket redeemer</param>
        /// <returns>return true if deleted or not found</returns>
        [WebMethod(Description = "CancelTicket -- Try to delete a cached ticket, should return true if deleted or not found."
           + " If the receiver is a serviceBroker and is not the redeemer the call needs to be repackaged and forwarded to the redeemer."
            + " Each ProcessAgent type may need to override this method depending on the ticket type." ),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public virtual bool CancelTicket(Coupon coupon, string type, string redeemer)
        {
            bool status = false;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                if (ProcessAgentDB.ServiceGuid.Equals(redeemer))
                {
                    ProcessAgentDB ticketing = new ProcessAgentDB();
                    status = dbTicketing.DeleteTicket(coupon, type, ProcessAgentDB.ServiceGuid);
                }
                else
                {
                    status = false;
                }
            }
            return status;
        }

        /// <summary>
        /// Install the calling services credentials on this processAgent.
        /// </summary>
        /// <param name="service">The calling processAgents immutable information</param>
        /// <param name="inIdentCoupon">The coupon that will authorize messages from the service</param>
        /// <param name="outIdentCoupon">The coupon to be used when contacting the service</param>
        [WebMethod(Description = "Install the calling services credentials on this processAgent."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("initAuthHeader", Direction = SoapHeaderDirection.In)]
        public ProcessAgent InstallDomainCredentials(ProcessAgent service,
            Coupon inIdentCoupon, Coupon outIdentCoupon)
        {
            if (!initAuthHeader.initPasskey.Equals(ConfigurationManager.AppSettings["defaultPasskey"]))
            {
                throw new Exception("The proper authorization to install the domain credentials, has not been provided.");
            }
            return installDomainCredentials(service, inIdentCoupon, outIdentCoupon);
        }


        /// <summary>
        /// Modify the specified services Domain credentials on this static process agent. 
        /// Agent_guid is key to the service to be modified. The agentAuthorizationHeader must use the old values.
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "Modify the specified services Domain credentials on this static process agent. "
            + "Agent_guid is key to the service to be modified and may not be modiied. The agentAuthorizationHeader must use the old values."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public virtual int ModifyDomainCredentials(string originalGuid, ProcessAgent agent, string extra, 
            Coupon inCoupon, Coupon outCoupon)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                status = dbTicketing.ModifyDomainCredentials(originalGuid, agent, inCoupon, outCoupon, extra);
            }
            return status;
        }
        

        [WebMethod(Description = "Modify the specified services Domain credentials on this static process agent. "
            + "Agent_guid is key to the service to be modified and may not be modiied. The agentAuthorizationHeader must use the old values."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public int RemoveDomainCredentials(string domainGuid, string serviceGuid)
        {
            int status = -1;
            DomainCredentials returnCred = null;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                status = removeDomainCredentials(domainGuid, serviceGuid);
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
        public virtual int ModifyProcessAgent(string originalGuid, ProcessAgent agent, string extra)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                status = dbTicketing.ModifyProcessAgent(originalGuid, agent, extra);
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
        public virtual int RetireProcessAgent(string domainGuid, string serviceGuid, bool state)
        {
            int status = 0;
            if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
                status = retireProcessAgent(domainGuid, serviceGuid, state);
            }
            return status;
        }
    
        [WebMethod(Description = "Register, an optional method, default virtual method is a no-op."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public void Register(string registerGuid, ServiceDescription[] info)
        {
              if (dbTicketing.AuthenticateAgentHeader(agentAuthHeader))
            {
            // This is an optional method, the base method is a no-op.
                  register(registerGuid,info);
              }
        }

        protected virtual ProcessAgent installDomainCredentials(ProcessAgent service,
            Coupon inIdentCoupon, Coupon outIdentCoupon)
        {
            ProcessAgent agent = null;
            try
            {
                if (ProcessAgentDB.ServiceAgent == null)
                {
                    throw new Exception("The specified ProcessAgent has not been configured, please contact the administrator.");
                }
                
                if (service.type.Equals(ProcessAgentType.SERVICE_BROKER))
                {

                    int[] ids = dbTicketing.GetProcessAgentIDsByType((int)ProcessAgentType.AgentType.SERVICE_BROKER);
                    if (ids != null && ids.Length > 0)
                    {
                        throw new Exception("There is already a domain ServiceBroker assigned to this Service!");
                    }
                    else
                    {
                        int id = dbTicketing.InsertProcessAgent(service, inIdentCoupon, outIdentCoupon);

                        if (id > 0)
                        {
                            dbTicketing.SetDomainGuid(service.agentGuid);
                            ProcessAgentDB.RefreshServiceAgent();
                            Utilities.WriteLog("InstallDomainCredentials: " + service.codeBaseUrl);
                            agent = ProcessAgentDB.ServiceAgent;
                        }
                        else
                        {
                            Utilities.WriteLog("Error InstallDomainCredentials: " + service.codeBaseUrl);
                            throw new Exception("Error Installing DomainCredentials on: " + ProcessAgentDB.ServiceAgent.codeBaseUrl);
                        }
                    }
                }
                else
                {
                    int[] ids = dbTicketing.GetProcessAgentIDsByType((int)ProcessAgentType.AgentType.SERVICE_BROKER);
                    if (ids == null || ids.Length == 0)
                    {
                        throw new Exception("This Service is not part of a domain and may not be accessed");
                    }
                    else
                    {
                        int pid = dbTicketing.InsertProcessAgent(service, inIdentCoupon, outIdentCoupon);

                        if (pid > 0)
                        {
                            Utilities.WriteLog("InstallDomainCredentials: " + service.codeBaseUrl);
                            agent = ProcessAgentDB.ServiceAgent;
                        }
                        else
                        {
                            Utilities.WriteLog("Error InstallDomainCredentials: " + service.codeBaseUrl);
                            throw new Exception("Error Installing DomainCredentials on: " + ProcessAgentDB.ServiceAgent.codeBaseUrl);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.WriteLog("Error on InstallDomainCredentials: " + Utilities.DumpException(e));
                throw;
            }
            return agent;
        }

   

        protected virtual int removeDomainCredentials(string domainGuid, string agentGuid)
        {
            int status = 0;
            status = dbTicketing.RemoveDomainCredentials(domainGuid, agentGuid);
            return status;
        }

       

        protected virtual int retireProcessAgent(string domainGuid, string serviceGuid, bool state)
        {
            int status = 0;
            status = dbTicketing.RetireProcessAgent(domainGuid, serviceGuid, state);
            return status;
        }
    

         protected virtual void register(string registerId, ServiceDescription[] info){
         }
}
}