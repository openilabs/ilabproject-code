/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: InteractiveLabServer.asmx.cs 450 2011-09-07 20:33:00Z phbailey $
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

using iLabs.Ticketing;
using iLabs.Core;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.LabServer.Interactive;
using iLabs.Web;



namespace iLabs.LabServer
{


    /// <summary>
    /// This is a sample InteractiveLabServer, most of the processing 
    /// will be handled by other classes specific to the functionality.
    /// </summary>
    //[System.Diagnostics.DebuggerStepThroughAttribute()] // Do not use this is stops you from Debugging
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [WebService(Name = "InteractiveLS", Namespace = "http://ilab.mit.edu/iLabs/Services", Description = "Interactive Lab Server")]
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [WebServiceBinding(Name = "I_ILS", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    [WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    //[SoapDocumentService(RoutingStyle=SoapServiceRoutingStyle.RequestElement)]
    public class InteractiveLabServer : WS_ILabCore
    {

        public InteractiveLabServer()
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

        /**************************************************************************************************************************
		 * Interactive Lab Server API Web Service Methods
		 * ***********************************************************************************************************************/



        /// <summary>
        /// Alert is used by the LabScheduling Server to notify the lab server about a scheduled event other than an experiment execution. This is currently not implemented.
        /// </summary>
        /// <param name="payload">Defines the alert parameters<wakeup><groupName></groupName><guid></guid><executionTime></executionTime></wakeup></param>
        /// <returns></returns>
        [WebMethod(Description = "Alert is used by the LabScheduling Server to notify the lab server about a scheduled event other than an experiment execution."),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In),
        SoapDocumentMethod(Binding = "I_ILS")]
        public void Alert(string payload)
        {
            processAlert(payload);

        }
        /// <summary>
        /// Used by the ServiceBroker to notify a LabServer that an experiment should be closed and that all data sources should be closed & released.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="reason"></param>
        /// <returns>an integer which is a StorageStatus value</returns>
        [WebMethod(Description = "Used by the ServiceBroker to notify a LabServer that an experiment should be closed and that all data sources should be closed & released."),
        SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In),
        SoapDocumentMethod(Binding = "I_ILS")]
        public int CloseExperiment(long experimentId, int reason)
        {
            int sstatus = StorageStatus.UNKNOWN;
            LabDB dbManager = new LabDB();
            if (dbManager.AuthenticateAgentHeader(agentAuthHeader))
            {
                LabTask.eStatus estatus = dbManager.ExperimentStatus(experimentId, agentAuthHeader.agentGuid);
                if (estatus == LabTask.eStatus.Running
                    || estatus == LabTask.eStatus.Scheduled
                     || estatus == LabTask.eStatus.Pending
                     || estatus == LabTask.eStatus.Waiting)
                {
                    LabTask task = TaskProcessor.Instance.GetTask(experimentId, agentAuthHeader.agentGuid);
                    TaskProcessor.Instance.Remove(task);
                    // close existing data Sources ln task.Close
                    estatus = task.Close();
                    switch (estatus)
                    {
                        case LabTask.eStatus.Aborted:
                        case LabTask.eStatus.Closed:
                        case LabTask.eStatus.Completed:
                            sstatus = StorageStatus.CLOSED;
                            break;
                        case LabTask.eStatus.Expired:
                            sstatus = StorageStatus.CLOSED_TIMEOUT;
                            break;
                        case LabTask.eStatus.Pending:
                        case LabTask.eStatus.Running:
                        case LabTask.eStatus.Scheduled:
                        case LabTask.eStatus.Waiting:
                            sstatus = StorageStatus.RUNNING;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                sstatus = StorageStatus.ERROR;
            }
            return sstatus;
        }

        protected void processAlert(string payload)
        {
        }

       
 
    } // END OF InteractiveLabServer

}
