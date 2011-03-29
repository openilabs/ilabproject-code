using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker.iLabSB;


using iLabs.UtilLib;
using iLabs.Ticketing;
//using iLabs.Services;

namespace iLabs.ServiceBroker.admin 
{

    public partial class ClientMetadata : System.Web.UI.Page
    {
        protected BrokerDB brokerDb = new BrokerDB();


        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();

        int labClientID;
        int[] labClientIDs;
        LabClient[] labClients;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session.Remove("CrossRegLSS");
                initialDropDownList();
            }
        }

        protected void initialDropDownList()
        {
           
            ddlClient.Items.Clear();
            labClientIDs = wrapper.ListLabClientIDsWrapper();

            ddlClient.Items.Add(new ListItem(" ------------- select Client ------------ ", "0"));
            if (labClientIDs != null)
            {
                labClients = wrapper.GetLabClientsWrapper(labClientIDs);
                foreach (LabClient lc in labClients)
                {
                    // Check that the client is not a 6.1 Batch client and that it is from this domain
                    if ((lc.clientType.CompareTo(LabClient.BATCH_APPLET) != 0) && (lc.clientType.CompareTo(LabClient.BATCH_APPLET) != 0))
                    {
                        ProcessAgentInfo[] labServers = AdministrativeAPI.GetLabServersForClient(lc.clientID);
                        if (labServers != null && (labServers.Length > 0) && (labServers[0].agentId > 0))
                        {
                            if (labServers[0].domainGuid.Equals(ProcessAgentDB.ServiceGuid))
                            {
                                ddlClient.Items.Add(new ListItem(lc.clientName, lc.clientID.ToString()));
                            }
                        }
                    }
                }
            }
        }

        protected void ddlServiceBroker_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblResponse.Visible = false;
        }
        protected void ddlClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblResponse.Visible = false;
       
        }


        protected void btnRegister_Click(object sender, System.EventArgs e)
        {
            lblResponse.Visible = false;
            bool error = false;
            int lssID = 0;
            StringBuilder message = new StringBuilder();

            try
            {
                // Check for enough information to perform the register

             
                if (Session["crossRegLSS"] != null)
                {
                    lssID = (int)Session["CrossRegLSS"];
                }
                if (!(ddlClient.SelectedIndex > 0))
                {

                    message.AppendLine("You must select a Lab Client!");
                    error = true;
                }
                if (error)
                {
                    lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
                    lblResponse.Visible = true;
                }
                else
                {
                 
                }


            }
            catch (Exception ex)
            {
               Logger.WriteLine("Error in cross-Register: " + ex.Message);
                throw;
            }

        }

    }

}
