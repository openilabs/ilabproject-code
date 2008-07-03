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
        /// Install the calling services credentials on this processAgent
        /// </summary>
        /// <param name="service">The calling processAgents immutable information</param>
        /// <param name="inIdentCoupon">The coupon that will authorize messages from the service</param>
        /// <param name="outIdentCoupon">The coupon to be used when contacting the service</param>
        [WebMethod,
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("initAuthHeader", Direction = SoapHeaderDirection.In)]
        public virtual ProcessAgent InstallDomainCredentials(ProcessAgent service,
            Coupon inIdentCoupon, Coupon outIdentCoupon)
        {
             ProcessAgent agent = null;
            if (service.type.Equals(ProcessAgentType.SERVICE_BROKER))
            {

                int[] ids = dbTicketing.GetProcessAgentIDsByType((int) ProcessAgentType.AgentType.SERVICE_BROKER);
                if (ids != null && ids.Length > 0)
                {
                    throw new Exception("There is already a domain ServiceBroker assigned to this Service!");
                }
                else
                {
                    agent = InstallDomainCredentials(initAuthHeader, service, inIdentCoupon, outIdentCoupon);
                    
                    
                }
            }
            else if (service.type.Equals(ProcessAgentType.REMOTE_SERVICE_BROKER))
            {
                throw new Exception("You may not register a Remote ServiceBroker with this Service!");
            }
            else
            {
                int[] ids = dbTicketing.GetProcessAgentIDsByType((int) ProcessAgentType.AgentType.SERVICE_BROKER);
                if (ids == null || ids.Length == 0)
                {
                    throw new Exception("This Service is not part of a domain and may not be accessed");
                }
                else
                {
                    agent = InstallDomainCredentials(initAuthHeader, service, inIdentCoupon, outIdentCoupon);
                }
            }
            return agent;
        }

        protected ProcessAgent InstallDomainCredentials(InitAuthHeader initHeader, ProcessAgent service,
            Coupon inIdentCoupon, Coupon outIdentCoupon)
        {
            if (ProcessAgentDB.ServiceAgent == null)
            {
                throw new Exception("The specified ProcessAgent has not been configured, please contact the administrator.");
            }
            if (!initHeader.initPasskey.Equals(ConfigurationManager.AppSettings["defaultPasskey"]))
            {
                throw new Exception("The proper authorization to install the domain credentials, has not been provided.");          
            }
            try
            {
                int id = dbTicketing.InsertProcessAgent(service, inIdentCoupon, outIdentCoupon);
                if (service.type.Equals(ProcessAgentType.SERVICE_BROKER))
                {
                    dbTicketing.SetDomainGuid(service.agentGuid);
                    ProcessAgentDB.RefreshServiceAgent();
                }
                if (id > 0)
                {
                    Utilities.WriteLog("InstallDomainCredentials: " + service.codeBaseUrl);
                    return ProcessAgentDB.ServiceAgent;
                }
                else
                {
                    Utilities.WriteLog("Error InstallDomainCredentials: " + service.codeBaseUrl);
                    throw new Exception("Error Installing DomainCredentials on: " + ProcessAgentDB.ServiceAgent.codeBaseUrl);
                }
            }
            catch(Exception e){
                Utilities.WriteLog("Error on InstallDomainCredentials: " + Utilities.DumpException(e));
                throw;
            }
            
        }

        [WebMethod(Description = "Register, an optional method, default virtual method is a no-op."),
        SoapDocumentMethod(Binding = "IProcessAgent"),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
        public virtual void Register(string registerId, ServiceDescription[] info)
        {
            // This is an optional method, the base method is a no-op.
        }

}
}