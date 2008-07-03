<%@ WebService Language="C#" Class="I_ProcessAgent" %>

using System;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;
using System.Threading;

using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
[XmlType(Namespace = "http://ilab.mit.edu/iLabs/type")]
[WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services")]
[WebService(Name = "ProcessAgentProxy", Namespace = "http://ilab.mit.edu/iLabs/Services")]
public abstract class I_ProcessAgent  : System.Web.Services.WebService {


    /// <summary>
    /// 
    /// </summary>
    public InitAuthHeader initAuthHeader = new InitAuthHeader();
    public AgentAuthHeader agentAuthHeader = new AgentAuthHeader();
    public BrokerAuthHeader brokerAuthHeader = new BrokerAuthHeader();

    ////////////////////////////////////////////////////
    ///    IProcessAgent Methods                     ///
    ////////////////////////////////////////////////////

    
    /// <summary>
    /// Get the services local DateTime.
    /// </summary>
    /// <returns></returns>
    [WebMethod,
    SoapDocumentMethod(Binding = "IProcessAgent")]
    public abstract DateTime GetServiceTime();
   
    
    /// <summary>
    /// Get a simple StatusReport from the service.
    /// </summary>
    [WebMethod,
    SoapDocumentMethod(Binding = "IProcessAgent")]
    public abstract StatusReport GetStatus();
    
    
    /// <summary>
    /// Send a status notification to this service, it is up to the service to take action.
    /// </summary>
    [WebMethod,
    SoapDocumentMethod(Binding = "IProcessAgent"),
    SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
    public abstract void StatusNotification(StatusNotificationReport report);
  
    /// <summary>
    /// Install the Domain credentials on this static process agent.
    /// Please note this method depends on a 'bootstrap' ProcessAgent record in the 
    /// database with the same agentGUID as the serviceGUID specified in Web.config.
    /// Also uses the defaultPasskey value from Web.config.
    /// </summary>
    /// <param name="service" Description="used to provide information for the registering service"></param>
    /// <param name="inIdentCoupon" Description="For messages from the Service"></param>
    /// <param name="outIdentCoupon" Description="For messages to the Service"></param>
    /// <returns>The bootstrap ProcessAgent 'record' which describes this service.</returns>
    [WebMethod,
    SoapDocumentMethod(Binding = "IProcessAgent"),
    SoapHeader("initAuthHeader", Direction = SoapHeaderDirection.In)]
    public abstract ProcessAgent InstallDomainCredentials(ProcessAgent service, Coupon inIdentCoupon, Coupon outIdentCoupon);
  
    
    /// <summary>
    /// Cancel the ticket. This call is always from a serviceBroker, 
    /// if the receiver is a serviceBroker but is not the redeemer the 
    /// header is repackaged and forwarded to the local redeemer.
    /// </summary>
    /// <param name="coupon"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [WebMethod,
    SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In),
    SoapDocumentMethod(Binding = "IProcessAgent")]
    public abstract bool CancelTicket(Coupon coupon, string type, string redeemer);


    [WebMethod(Description = "Register, an optional method"),
    SoapDocumentMethod(Binding = "IProcessAgent"),
    SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In)]
    public abstract void Register(string registerId, ServiceDescription[] info);
   
}

