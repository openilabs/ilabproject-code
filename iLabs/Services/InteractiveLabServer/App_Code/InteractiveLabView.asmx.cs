/* $Id: InteractiveLabView.asmx.cs,v 1.13 2008/03/17 18:50:42 pbailey Exp $ */

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
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;


using iLabs.LabView;
using iLabs.LabView.LV82;

namespace iLabs.LabServer.LabView
{

    /// <summary>
    /// This class is a LabView LabServer interface, The methods in this 
    /// WebService were defined for testing. The code has not been updated 
    /// and should only be used for debuging the state of the LabView processes.
    /// </summary>
   // [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[SoapDocumentService(RoutingStyle=SoapServiceRoutingStyle.RequestElement)]
    [WebService(Name = "InteractiveLabView", Namespace = "http://ilab.mit.edu/iLabs/Services", Description = "LabVIEW Lab Server"),
    WebServiceBindingAttribute(Name = "LabViewLS", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public class InteractiveLabView : InteractiveLabServer
    {

        //private LabDB dbManager;

        public InteractiveLabView()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();

            //dbManager = new LabDB();

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
		 * Lab Server API Web Service Methods
		 * ***********************************************************************************************************************/


        /// <summary>
        /// Checks on the status of the lab server.  
        /// </summary>
        [WebMethod(Description = @"Submits an action to the Labview Interface: loadvi, closevi, publishvi, monitorvi, disconnectuser, listvis. C:\Program Files\National Instruments\LabVIEW 7.1\examples\apps\tankmntr.llb\Tank Simulation.vi"),
            //SoapHeader("authHeader", Direction=SoapHeaderDirection.In), 
        SoapDocumentMethod(Binding = "LabViewLS")]
        public string SubmitCommand(string actionStr, string dataStr)
        {
            return new LabViewInterface().SubmitAction(actionStr, dataStr);
        }

        /// <summary>
        /// Checks on the status of the lab server.  
        /// </summary>
        [WebMethod(Description = @"Submits an action to the Labview Interface: loadvi, closevi, publishvi, monitorvi, disconnectuser, listvis. C:\Program Files\National Instruments\LabVIEW 7.1\examples\apps\tankmntr.llb\Tank Simulation.vi"),
            //SoapHeader("authHeader", Direction=SoapHeaderDirection.In), 
        SoapDocumentMethod(Binding = "LabViewLS")]
        public string SubmitCommandRemote(string actionStr, string dataStr, string host, int port)
        {
            return new LabViewRemote(host,port).SubmitAction(actionStr, dataStr);
        }

/*
        /// <summary>
        /// Checks on the status of the lab server.  
        /// </summary>
        [WebMethod(Description = "Lab Server Method: Returns the current status of the VIServer connection. "
        + "Members of struct LabStatus are 'online' (boolean) and 'labStatusMessage' (string)."),
       SoapHeader("agentAuthHeader", Direction = SoapHeaderDirection.In),
        SoapDocumentMethod(Binding = "ILabServer")]
        public LabStatus GetLabStatus()
        {
            return getLabStatus();

        }

        
        protected LabStatus getLabStatus()
        {
            string report = new LabViewInterface().GetViServerStatus();
            LabStatus status = new LabStatus();
            status.online = true;
            status.labStatusMessage = report;
            return status;

        }


        [WebMethod(Description = @"Submits an action to the Labview Interface: loadvi, closevi, publishvi, monitorvi, disconnectuser, listvis. C:\Program Files\National Instruments\LabVIEW 7.1\examples\apps\tankmntr.llb\Tank Simulation.vi"),
            //SoapHeader("authHeader", Direction=SoapHeaderDirection.In), 
       SoapDocumentMethod(Binding = "LabViewLS")]
        public LabStatus getLabStatusRemote(String host, int port)
        {
            string report = new LabViewRemote(host, port).GetViServerStatus();
            LabStatus status = new LabStatus();
            status.online = true;
            status.labStatusMessage = report;
            return status;

        }
*/
        [WebMethod(Description = @"Test the remote  SetLockState"),
            //SoapHeader("authHeader", Direction=SoapHeaderDirection.In), 
      SoapDocumentMethod(Binding = "LabViewLS")]
        public void TestLockState(String host, int port, string viName, bool state)
        {
            I_LabViewInterface lvi = new LabViewRemote(host, port);
            lvi.SetLockState(viName, state);
            
        }


        protected string getConfiguration(string group)
        {
            return new LabViewInterface().GetLabConfiguration(group);

        }

    } // END OF LabViewLS Class

}


