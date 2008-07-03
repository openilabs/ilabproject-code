/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: SelfRegistration.aspx.cs,v 1.9 2007/12/26 05:27:30 pbailey Exp $
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;

namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Summary description for administration.
	/// </summary>
	public partial class SelfRegistration : System.Web.UI.Page
	{
        private Color disabled = Color.FromArgb(243, 239, 229);
        private Color enabled = Color.White;
  
	    protected bool secure = false; // Backdoor
        ProcessAgentDB dbTicketing = null;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            string couponId;
            string passkey;
            string issuerGuid;
            dbTicketing = new ProcessAgentDB();
           

            if (!IsPostBack)
            {
          
                // try & test for local access or not configured
                if (secure)
                {
                    // retrieve parameters from URL
                    couponId = Request.QueryString["coupon_id"];
                    passkey = Request.QueryString["passkey"];
                    issuerGuid = Request.QueryString["issuer_guid"];
                    Ticket allowAdminTicket = null;
                    if (couponId != null && passkey != null && issuerGuid != null)
                    {
                        allowAdminTicket = dbTicketing.RetrieveAndVerify(
                            new Coupon(issuerGuid, Int64.Parse(couponId), passkey),
                            TicketTypes.ADMINISTER_LS);
                    }
                    else
                    {
                        Response.Redirect("AccessDenied.aspx", true);
                    }
                }

                string returnUrl = Request.QueryString["returnURL"];
                if (returnUrl != null && returnUrl.Length > 0)
                    Session["returnURL"] = returnUrl;
                txtDomainServer.ReadOnly = true;
                txtDomainServer.BackColor = disabled;
                lblServiceType.Text = ConfigurationManager.AppSettings["serviceType"];
                DisplayForm();
            }
            String returnURL = (string)Session["returnURL"];
            if ((returnURL != null) && (returnURL.Length > 0))
            {
                lnkBackSB.NavigateUrl = returnURL;
                lnkBackSB.Visible = true;
            }
            else
            {
                lnkBackSB.Visible = false;
            }


        }

        protected void DisplayForm()
        {
            lblResponse.Visible = false;
            StringBuilder message = new StringBuilder();
            ProcessAgent pai = ProcessAgentDB.ServiceAgent;
            if (pai != null)
            {
               
                txtServiceName.Text = pai.agentName;
                txtServiceGUID.Text = pai.agentGuid;

                txtCodebaseUrl.Text = pai.codeBaseUrl;
                txtWebServiceUrl.Text = pai.webServiceUrl;

                if (lblServiceType.Text.Equals(ProcessAgentType.SERVICE_BROKER))
                {
                    trDomainSB.Visible = false;
                    IntTag[] pas = dbTicketing.GetProcessAgentTags();
                    // If additional processAgents are registered the Fields may not be modified
                    SetFormMode(pas != null && pas.Length > 1);
                }
                else
                { 
                    // Check to see if a ServiceBroker is registered
                    trDomainSB.Visible = true;
                    ProcessAgentInfo sbInfo = dbTicketing.GetServiceBrokerInfo();
                    if (sbInfo != null)
                    {

                        txtDomainServer.Text = sbInfo.ServiceUrl;

                        // May not modify any fields that define the service since
                        // It is registered with its domainServiceBroker

                        SetFormMode(true);

                    }
                    else
                    {
                        // May modify fields that define the service since
                        // It is not registered registered with a domainServiceBroker
                        message.Append("A domain ServiceBroker has not been registered!");
                        lblResponse.Text = Utilities.FormatWarningMessage(message.ToString());
                        lblResponse.Visible = true;
                        Utilities.WriteLog("administration: DomainServerNotFound");
                        SetFormMode(false);
                    }
                }
            }
            else
            {
                message.Append("The self Registration information has not been saved to the database.");
                message.Append(" Displaying default values from Web.Config. Please modify & save.");
                // Need to call selfRegister
                string serviceGUID = ConfigurationManager.AppSettings["serviceGUID"];
                if(serviceGUID != null)
                txtServiceGUID.Text = serviceGUID;

                string serviceURL = ConfigurationManager.AppSettings["serviceURL"];
                if (serviceURL != null)
                txtWebServiceUrl.Text = serviceURL;

                string serviceName = ConfigurationManager.AppSettings["serviceName"];
                if (serviceName != null)
                txtServiceName.Text = serviceName;

                string codebaseURL = ConfigurationManager.AppSettings["codebaseURL"];
                if (codebaseURL != null)
                txtCodebaseUrl.Text = codebaseURL;
            lblResponse.Text = Utilities.FormatWarningMessage(message.ToString());
            lblResponse.Visible = true;
            }
            txtOutPassKey.Text = ConfigurationManager.AppSettings["defaultPasskey"];
           
        }

        protected void SetFormMode(bool hasDomain)
        {
	    lblResponse.Visible = false;
            txtServiceName.ReadOnly = hasDomain;
            txtServiceName.BackColor = hasDomain ? disabled : enabled;
            txtServiceGUID.ReadOnly = hasDomain;
            txtServiceGUID.BackColor = hasDomain ? disabled : enabled;
            txtCodebaseUrl.ReadOnly = hasDomain;
            txtCodebaseUrl.BackColor = hasDomain ? disabled : enabled;
            txtWebServiceUrl.ReadOnly = hasDomain;
            txtWebServiceUrl.BackColor = hasDomain ? disabled : enabled;
            btnGuid.Visible = !hasDomain;
        }


        protected void btnGuid_Click(object sender, System.EventArgs e)
        {
            Guid guid = System.Guid.NewGuid();
            txtServiceGUID.Text = Utilities.MakeGuid();
            valGuid.Validate();
        }

        protected void checkGuid(object sender, ServerValidateEventArgs args)
        {
            if(args.Value.Length >0 && args.Value.Length <=50)
                args.IsValid = true;
            else 
                args.IsValid = false;
        }

        protected void btnSaveChanges_Click(object sender, System.EventArgs e)
        {
            bool error = false;
            StringBuilder message = new StringBuilder();
            //Check fields for valid input
            if (!(txtServiceName.Text != null && txtServiceName.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter a Service Name<br/>");
            }
            if (!(txtServiceGUID.Text != null && txtServiceGUID.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter a Guid for the service<br/>");
            }
            if (!(txtCodebaseUrl.Text != null && txtCodebaseUrl.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter the base URL for the Web Site<br/>");
            }
            if (!(txtWebServiceUrl.Text != null && txtWebServiceUrl.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter full URL of the Web Service page<br/>");
            }

            if(error){
                lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
		        lblResponse.Visible = true;
                return;
            }
            else{
                // Check if domain is set if so only update mutable Fields
                dbTicketing.SelfRegisterProcessAgent(txtServiceGUID.Text,
                    txtServiceName.Text, lblServiceType.Text, txtServiceGUID.Text, 
                    txtCodebaseUrl.Text, txtWebServiceUrl.Text);
        
                DisplayForm();
            }
        }


        protected void btnNew_Click(object sender, System.EventArgs e)
        {
            txtServiceName.Text = "";
            txtServiceGUID.Text = "";
            txtCodebaseUrl.Text = "";
            txtWebServiceUrl.Text = "";
            lblResponse.Text = "";
	        lblResponse.Visible = false;
        }

        protected void btnRefresh_Click(object sender, System.EventArgs e)
        {
            txtServiceName.Text = "";
            txtServiceGUID.Text = "";
            txtCodebaseUrl.Text = "";
            txtWebServiceUrl.Text = "";
            
            DisplayForm();
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
