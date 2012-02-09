/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */
using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Web.Mail;

using iLabs.Ticketing;
using iLabs.DataTypes.TicketingTypes;
using iLabs.UtilLib;

namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Help Page
	/// </summary>
	public partial class help : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlAnchor navLogout;
		protected System.Web.UI.WebControls.TextBox txtBugReport;
	
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (Request.UserAgent.Contains("MSIE 6") || Request.UserAgent.Contains("MSIE 7"))
            {
                recaptcha.EmbedJavascript = true;
            }
			if(! IsPostBack)
			{
				//int[] lsIDs = AdministrativeAPI.ListLabServerIDs();
               
				ddlWhichLab.Items.Add("System-wide error");
			
				String optList = ConfigurationManager.AppSettings["helpOptions"];
				if((optList != null)&& (optList.Length >0)){
					char [] delimiter = {','};
					String [] options =optList.Split(delimiter,100);
					for(int i =0;i< options.Length;i++)
					{
						ddlWhichLab.Items.Add(new ListItem(options[i],"0"));
					}
					if(options.Length > 0)
					{
						ddlWhichLab.Items[0].Selected = false;
					}
				}
			
			}
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

		protected void btnRequestHelp_Click(object sender, System.EventArgs e)
		{
            if (!recaptcha.IsValid)
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("You must respond to the security question!");
                lblErrorMessage.Visible = true;
                return;
            }
			if(ddlWhichLab.SelectedItem.Text.CompareTo("") == 0)
			{
				lblErrorMessage.Text = "<div class=errormessage><p>Please select a lab.</p></div>";
				lblErrorMessage.Visible = true;
			}
            else if (txtProblem.Text == "")
            {
                lblErrorMessage.Text = "<div class=errormessage><p>Enter a description of the problem!</p></div>";
                lblErrorMessage.Visible = true;
            }
            else
            {
                //int userID = Convert.ToInt32(Session["UserID"]);

                string email = txtEmail.Text;

                string lab = ddlWhichLab.SelectedItem.Text;
                
                    //Send email
                    MailMessage mail = new MailMessage();
                    mail.From = email;
                    mail.To = ConfigurationManager.AppSettings["supportMailAddress"];
                    if (email != "")
                    {
                        mail.Cc = email;
                    }
                    mail.Subject = "[iLabs] Bug Report: " + lab;
                    StringBuilder buf = new StringBuilder();
                    //mail.Body = fname + " "+ lname +"\n\r";
                    //mail.Body +="User ID: " + uname + "\n\r";
                    buf.AppendLine("Email:  " + email);
                    buf.Append("Group: ");
                    if( Session["groupName"] != null)
                        buf.AppendLine(Session["groupName"].ToString());
                    else 
                        buf.AppendLine("No group");
                    buf.Append("BrokerGUID: ");
                    if(Session["brokerGUID"] != null)
                      buf.AppendLine(Session["brokerGUID"].ToString());
                    else
                      buf.AppendLine();
                    buf.AppendLine();
                    buf.AppendLine("reports the following bug for the lab '" + lab + "':  ");
                    buf.AppendLine(txtProblem.Text);
                    buf.AppendLine();
                    buf.AppendLine("Additional Information:");
                    buf.AppendLine("User Browser: " + Request.Browser.Type);
                    buf.AppendLine("User Browser Agent: " + Request.UserAgent);
                    buf.AppendLine("User Platform: " + Request.Browser.Platform);
                    buf.AppendLine("URL used to access page: " + Request.Url);
                    buf.AppendLine();
                    buf.AppendLine("-------------------------------------------------");
                    buf.AppendLine("This is an automatically generated message. ");
                    buf.AppendLine("DO NOT reply to the sender.");
                    buf.AppendLine();
                   //buf.AppendLine("For questions regarding this service, email smwang@mit.edu");
                    mail.Body = buf.ToString();
                    SmtpMail.SmtpServer = "127.0.0.1";
                    try
                    {
                        SmtpMail.Send(mail);
                        lblErrorMessage.Text = Utilities.FormatConfirmationMessage("Thank-you! Your request has been submitted. An administrator will contact you within 24-48 hours.");
                        lblErrorMessage.Visible = true;

                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text =  Utilities.FormatErrorMessage("Error sending your help request, please email ilab-debug@mit.edu. " + ex.Message);
                        lblErrorMessage.Visible = true;
                    }
                
            }
		}
   
	}
}
