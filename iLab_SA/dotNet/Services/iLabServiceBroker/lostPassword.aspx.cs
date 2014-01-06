/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Mail;
using System.Web.Security;


using iLabs.Core;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authentication;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Internal;
using iLabs.Ticketing;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.iLabSB
{
	/// <summary>
	/// iLabs Service Broker Lost Password Page
	/// </summary>
	public partial class lostPassword : System.Web.UI.Page
	{

		

		protected void Page_Load(object sender, System.EventArgs e)
		{
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

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
            string registrationMailAddress = ConfigurationManager.AppSettings["registrationMailAddress"];
			AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
			if(txtUsername.Text == "")
			{
				lblResponse.Text = Utilities.FormatErrorMessage("Missing user name.");
				lblResponse.Visible = true;
				return;
			}
			else
			{
				string userName = txtUsername.Text;
				int userID = AdministrativeAPI.GetUserID(userName, 0);
                if(userID <= 0)
                {
                    lblResponse.Text = Utilities.FormatErrorMessage("User name was not found.");
                    lblResponse.Visible = true;
                    return;
                }
				if (txtEmail.Text == null || txtEmail.Text == "")
				{
					lblResponse.Text = Utilities.FormatErrorMessage("Missing email field.");
					lblResponse.Visible = true;
					return;				
				}
				else
				{
					string email = txtEmail.Text ;
					User lostPassUser = AdministrativeAPI.GetUser(userID);

                    if (lostPassUser == null || lostPassUser.userID == 0)
					{
						// userID does not exist in the database
						lblResponse.Text = Utilities.FormatErrorMessage("The user does not exist.");
						lblResponse.Visible = true;

					}
                    else if (email.ToLower() != lostPassUser.email.ToLower())
					{
						// email does not match email record in our database
						lblResponse.Text = Utilities.FormatErrorMessage("Please use the user name AND email you were registered with.");
						lblResponse.Visible = true;
					}
                    else if (lostPassUser.lockAccount)
                    {
                        // email does not match email record in our database
                        lblResponse.Text = Utilities.FormatErrorMessage("The user account is locked out! Please send a help message to resolve this issue.");
                        lblResponse.Visible = true;
                    }
                    else if (lostPassUser.userName.ToLower().CompareTo("superuser") ==0)
                    {
                        // email does not match email record in our database
                        lblResponse.Text = Utilities.FormatErrorMessage("Password reset is not supported for this user. Please use the help system to contact a system administrator.");
                        lblResponse.Visible = true;

                        MailMessage mail = new MailMessage();
                        mail.From = registrationMailAddress;
                        // bugReportMailAddress
                        mail.To = ConfigurationManager.AppSettings["bugReportMailAddress"]; ;

                        mail.Subject = "[iLab] Attempt to reset password failed";
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("An attempt was made to change the password for the following user!\n\r");
                        sb.AppendLine("Username: " + lostPassUser.userName);
                        sb.AppendLine("Email:  " + lostPassUser.email);
                        sb.Append("Site URL: " + ProcessAgentDB.ServiceAgent.codeBaseUrl + "\n\r");
                        sb.Append("Site GUID: " + ProcessAgentDB.ServiceAgent.agentGuid + "\n\r");
                        sb.Append("\n\r\n\r");
                        sb.Append("Additional Information:\n\r");
                        sb.Append("User Host Name: " + Request.UserHostName + "\n\r");
                        sb.Append("User Host Address: " + Request.UserHostAddress + "\n\r");
                        sb.Append("User Browser: " + Request.Browser.Type + "\n\r");
                        sb.Append("User Browser Agent: " + Request.UserAgent + "\n\r");
                        sb.Append("User Platform: " + Request.Browser.Platform + "\n\r");
                        sb.Append("URL used to access page: " + Request.Url + "\n\r");
                        sb.Append("URL Referrer: " + Request.UrlReferrer + "\n\r");
                        sb.Append("Machine Name: " + Server.MachineName + "\n\r");
                        sb.Append("Server Type: " + Server.GetType() + "\n\r");
                        sb.Append("iLab Release: " + iLabGlobal.Release + "\n\r");
                        mail.Body = sb.ToString();

                        SmtpMail.SmtpServer = "127.0.0.1";
                        try
                        {
                            SmtpMail.Send(mail);

                            // email sent message
                            lblResponse.Text = Utilities.FormatConfirmationMessage("Your request has been submitted. A new password has been created and emailed to the email address you entered below.");
                            lblResponse.Visible = true;
                        }
                        catch (Exception ex)
                        {
                            // trouble sending request for password
                            // Report detailed SMTP Errors
                            string smtpErrorMsg;
                            smtpErrorMsg = "Exception: " + ex.Message;
                            //check the InnerException
                            if (ex.InnerException != null)
                                smtpErrorMsg += "<br>Inner Exceptions:";
                            while (ex.InnerException != null)
                            {
                                smtpErrorMsg += "<br>" + ex.InnerException.Message;
                                ex = ex.InnerException;
                            }

                            lblResponse.Text = Utilities.FormatErrorMessage("Trouble sending email. Your request could not be submitted - please inform an administrator.<br>" + smtpErrorMsg);
                            lblResponse.Visible = true;
                        }
                    }
                    else // send password to requestor's email address
					{
						MailMessage mail = new MailMessage();
						mail.From = registrationMailAddress;
                        mail.To = lostPassUser.email;
                       
						mail.Subject = "[iLabs] Service Broker Password Reset" ;
                        StringBuilder buf = new StringBuilder();
                        buf.AppendLine("Username: " + userName);
						buf.AppendLine("Email:  " + email);
						buf.AppendLine("Your old password has been reset to the following password. For security reasons, please login and use the 'My Account' page to reset your password.");
						buf.AppendLine("Password: " + resetPassword(userID));//InternalAuthenticationDB.ReturnNativePassword (userID);
                        mail.Body = buf.ToString();

						SmtpMail.SmtpServer = "127.0.0.1";
						try
						{	
							SmtpMail.Send(mail);

							// email sent message
                            lblResponse.Text = Utilities.FormatConfirmationMessage("Your request has been submitted. A new password has been created and emailed to the email address you entered below.");
							lblResponse.Visible = true;
						}
						catch (Exception ex)
						{
							// trouble sending request for password
							// Report detailed SMTP Errors
							string smtpErrorMsg;
							smtpErrorMsg = "Exception: " + ex.Message;
							//check the InnerException
							if (ex.InnerException != null)
								smtpErrorMsg += "<br>Inner Exceptions:";
							while( ex.InnerException != null )
							{
								smtpErrorMsg += "<br>" +  ex.InnerException.Message;
								ex = ex.InnerException;
							}

							lblResponse.Text = Utilities.FormatErrorMessage("Trouble sending email. Your request could not be submitted - please inform an administrator.<br>" + smtpErrorMsg);
							lblResponse.Visible = true;
						}
					}
				}			
			}
		}

		/* This is what happens in the following method:
			1) A random number is generated
			2) This number is hashed to create a long alphanumeric string
			3) The resulting string is truncated to 8 positions. This becomes the new
				password.
			4) The 8-position password is hashed and stored in the database.
			5) The un-hashed 8-position password is returned (& then emailed to the student.)
			When the student logs in, the un-hashed 8-position password is presented.
			It is then hashed and compared to the hashed value that has been stored in
			the database.
		*/
		public string resetPassword(int userID)
		{
			// 1. generate random number
			Random rnd = new Random();
			long rndNo = rnd.Next();

			//2. hash the random number 
			string hashed = FormsAuthentication.HashPasswordForStoringInConfigFile(rndNo.ToString(), "sha1");

			//3. get any 8 characters out of this & make this the new password.
			//ends with 24 since there are 32 characters in hashed string.
			string newPassword = hashed.Substring(rnd.Next(24), 8);
           
			//4. hash this password and store it into the database.
			string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "sha1");

			DbConnection myConnection =FactoryDB.GetConnection();
			try 
			{

                DbCommand cmd = myConnection.CreateCommand();
                cmd.CommandText = "UPDATE Users SET Password = '" + hashedPassword + "' WHERE User_ID = " + userID.ToString();
				myConnection.Open();
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				lblResponse.Text = Utilities.FormatErrorMessage("Error retrieving lost password. "+ex.GetBaseException());
			}
			finally
			{
				myConnection.Close();
			}

			//5. return password
			return newPassword;
		}
	}
}
