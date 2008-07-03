/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: loginRecords.aspx.cs,v 1.2 2006/08/11 14:26:19 pbailey Exp $
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authentication;
using iLabs.ServiceBroker.Authorization;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.admin
{
	/// <summary>
	/// Summary description for loginRecords.
	/// </summary>
	public partial class loginRecords : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlAnchor navLogout;

		AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Session["UserID"]==null)
				Response.Redirect("../login.aspx");

			if (ddlTimeIs.SelectedIndex!=4)
			{
				txtTime2.ReadOnly=true;
				txtTime2.BackColor=Color.Lavender;
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
	 
		protected void ddlTimeIs_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtTime1.Text=null;
			txtTime2.Text=null;
			if(ddlTimeIs.SelectedIndex==4)
			{
				txtTime2.ReadOnly=false;
				txtTime2.BackColor=Color.White;
			}
		}
		//list the log-in sessions according to the selected criterion
		private void BuildLoginListBox(int userID, int groupID, DateTime time1, DateTime time2)
		{
			
			try
			{
				UserSession[] sessions=wrapper.GetUserSessionsWrapper(userID,groupID,time1,time2);						
				if (sessions.Length==0)
				{
					lblResponse.Text =Utilities.FormatErrorMessage("No sessions found.");
					lblResponse.Visible=true;
				}
				else
				{
					for(int j = sessions.Length-1; j > -1  ; j--)
					{
						string userName=wrapper.GetUsersWrapper(new int[]{sessions[j].userID})[0].userName;
						string groupName=wrapper.GetGroupsWrapper(new int[]{sessions[j].groupID})[0].groupName;
						txtLoginDisplay.Text+="User: "+userName+" \n" + "Session group: "+groupName+" \n" + "Login time: " + sessions[j].sessionStartTime.ToString () + " \n" + "Logout time:  " + sessions[j].sessionEndTime .ToString ()+"\n\n";
					}
				}
			}
			catch(Exception ex)
			{
				lblResponse.Text ="<div class=errormessage><p>Cannot retrieve UserSessions. "+ex.GetBaseException()+"</p></div>";
				lblResponse.Visible=true;
			}
		}
		protected void btnGo_Click(object sender, System.EventArgs e)
		{
			txtLoginDisplay.Text=null;
			if(txtGroupName.Text=="" && txtUserName.Text=="" && txtTime1.Text=="" && txtTime2.Text=="")
			{
				UserSession[] sessions=wrapper.GetUserSessionsWrapper(-1,-1,DateTime.MinValue,DateTime.MaxValue);
				for(int j = sessions.Length-1; j > -1  ; j--)
				{
					string userName=wrapper.GetUsersWrapper(new int[]{sessions[j].userID})[0].userName;
					string groupName=wrapper.GetGroupsWrapper(new int[]{sessions[j].groupID})[0].groupName;
					txtLoginDisplay.Text+="User: "+userName+" \n" + "Session group: "+groupName+" \n" + "Login time: " + sessions[j].sessionStartTime.ToString () + " \n" + "Logout time:  " + sessions[j].sessionEndTime .ToString ()+"\n\n";
				}			
			}
			else
			{
				if(txtUserName.Text!="" && wrapper.GetUserIDWrapper(txtUserName.Text)==-1)
				{
					lblResponse.Text ="<div class=errormessage><p>no user with the username '"+txtUserName.Text+"'found</p></div>";
					lblResponse.Visible=true;
					return;
				}
				if(txtGroupName.Text!="" && wrapper.GetGroupIDWrapper(txtGroupName.Text)==-1)
				{
					lblResponse.Text =Utilities.FormatErrorMessage("Group '"+txtGroupName.Text+"' not found.");
					lblResponse.Visible=true;
					return;
				}
				if (ddlTimeIs.SelectedIndex<1)
				{
					BuildLoginListBox(wrapper.GetUserIDWrapper(txtUserName.Text),wrapper.GetGroupIDWrapper(txtGroupName.Text),DateTime.MinValue,DateTime.MinValue);

				}
				else 
				{
					DateTime time1;
					try
					{
						time1 = DateTime.Parse (txtTime1.Text);
					}
					catch
					{	
						lblResponse.Text = "<div class=errormessage><p>Please enter a valid time</p></div>";
						lblResponse.Visible=true;
						return;
					}
					if(ddlTimeIs.SelectedIndex==1)
					{
                        BuildLoginListBox(wrapper.GetUserIDWrapper(txtUserName.Text),wrapper.GetGroupIDWrapper(txtGroupName.Text),time1,time1);

					}
					else if(ddlTimeIs.SelectedIndex==2)
					{
						BuildLoginListBox(wrapper.GetUserIDWrapper(txtUserName.Text),wrapper.GetGroupIDWrapper(txtGroupName.Text),DateTime.MinValue,time1);

					}				
					else if(ddlTimeIs.SelectedIndex==3)
					{
						BuildLoginListBox(wrapper.GetUserIDWrapper(txtUserName.Text),wrapper.GetGroupIDWrapper(txtGroupName.Text),time1,DateTime.MinValue);

					}
					else if(ddlTimeIs.SelectedIndex==4)
					{
						DateTime time2;
						try
						{
							time2 = DateTime.Parse (txtTime2.Text);
						}
						catch
						{	
							lblResponse.Text = "<div class=errormessage><p>Please enter a valid time</p></div>";
							lblResponse.Visible=true;
							return;
						}
						BuildLoginListBox(wrapper.GetUserIDWrapper(txtUserName.Text),wrapper.GetGroupIDWrapper(txtGroupName.Text),time1,time2);

					}
				}
			}		
		}
	}
	}

