<%@ WebService Language="c#" Class="I_ILS" %>


/* $Id: I_ILS.asmx,v 1.1 2007/06/04 01:27:56 pbailey Exp $ */
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

using iLabs.DataTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;

	/// <summary>
	/// The interface definition for an InteractiveLabServer. Currently only one method, 
    /// additional methods will be added as needed.
	/// </summary>
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/Type")]
    [WebServiceBinding(Name = "I_ILS", Namespace = "http://ilab.mit.edu/iLabs/Services"),
    WebService(Name = "InteractiveLSProxy", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public abstract class I_ILS : System.Web.Services.WebService
	{
    
        public AgentAuthHeader agentAuthHeader = new AgentAuthHeader();
       

        /// <summary>
        /// Alert is used by the LabScheduling Server to notify the lab server about a scheduled event other than an experiment execution. This is currently not implemented.
        /// </summary>
        /// <param name="payload">Defines the alert parameters<wakeup><groupName></groupName><guid></guid><executionTime></executionTime></wakeup></param>
        /// <returns></returns>
        [WebMethod(Description = "Alert is used by the LabScheduling Server to notify the lab server about a scheduled event other than an experiment execution."),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In),
        SoapDocumentMethod(Binding = "I_ILS")]
        public abstract void Alert(string payload);
	}
