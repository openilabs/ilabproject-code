/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: userNav.ascx.cs,v 1.6 2007/05/14 22:31:40 pbailey Exp $
 */
namespace iLabs.LabServer
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Web.Security;

	/// <summary>
	///		Summary description for userNav.
	/// </summary>
	public partial class userNav : System.Web.UI.UserControl
	{


		protected string currentPage;
		protected string helpURL = "help.aspx";

		public string HelpURL
		{
			get {return helpURL;}
			set {helpURL = value; }
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Get the current page name w/o path name or slashes
			currentPage = Request.Url.Segments[Request.Url.Segments.Length -1];
			aHelp.HRef = helpURL;
			
			// Only show the link to Home if not logged in
            //if (Session["UserID"] == null)
            //{	
            //    aHome.Attributes.Add("class", "only");
            //    liNavlistMyGroups.Visible = false;
            //    liNavlistMyLabs.Visible = false;
            //    liNavlistExperiments.Visible = false;
            //    liNavlistMyAccount.Visible = false;
            //    liNavlistAdmin.Visible = false;
            //    lbtnLogout.Visible = false;
            //}
            //else
            //{
				lbtnLogout.Visible = true;
				SetNavList();
			//}

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		/// <summary>
		/// Sets the button state on the items in the unorderd list "ulNavList"
		/// </summary>
		private void SetNavList()
		{
			//object adminState = Session["isAdmin"];
			//liNavlistAdmin.Visible = ((adminState != null) && Convert.ToBoolean(adminState));

			// Do not show Labs or Experiments if Effective Group has not been specified
			//if (Session["GroupID"] !=null)
			//{
				//if (!((bool)adminState))
				//{
                    liNavlistAdmin.Visible = true;
					liNavlistMyLabs.Visible = true;
					liNavlistExperiments.Visible = true;
            //    }
            //    else
            //    {
            //        liNavlistMyLabs.Visible = false;
            //        liNavlistExperiments.Visible = false;
            //    }
            //}
            //else
            //{
            //    liNavlistMyLabs.Visible = false;
            //    liNavlistExperiments.Visible = false;
            //}

			// Only show the groups page if the user has more than one lab
			//if (Convert.ToInt32(Session["LabCount"]) != 1)
			//{
				liNavlistMyGroups.Visible = true;
			//}
			//else
			//{
			//	liNavlistMyGroups.Visible = false;
			//}
			
			//Logout Button
			lbtnLogout.CausesValidation = false;

			switch(currentPage)
			{
				case "home.aspx":
					aHome.Attributes.Add("class", "topactive");
					aMyAccount.Attributes.Add("class", "last");
					break;
                case "selfRegistration.aspx":
                    aHome.Attributes.Add("class", "first");
                    aSelfRegistration.Attributes.Add("class", "topactive");
                    aMyAccount.Attributes.Add("class", "last");
                    break;
				case "localGroups.aspx":
					aHome.Attributes.Add("class", "first");
					aMyGroups.Attributes.Add("class", "topactive");
					aMyAccount.Attributes.Add("class", "last");
					break;
				case "groupPermissions.aspx":
					//Note: the myLabs page determines which clients a user/group can access,
					// then redirects to myClient.aspx. So myLabs.aspx is never displayed, though
					// it looks as though it is the page to be linked to.
					aHome.Attributes.Add("class", "first");
					aMyLabs.Attributes.Add("class", "topactive");
					aMyAccount.Attributes.Add("class", "last");
					break;
				case "labExperiments.aspx":
					aHome.Attributes.Add("class", "first");
					aMyExperiments.Attributes.Add("class", "topactive");
					aMyAccount.Attributes.Add("class", "last");
					break;
				case "manageTasks.aspx":
					aHome.Attributes.Add("class", "first");
					aMyAccount.Attributes.Add("class", "last");
					break;
				case "help.aspx":
					aHome.Attributes.Add("class", "first");
					aHelp.Attributes.Add("class", "topactive");
					aMyAccount.Attributes.Add("class", "last");
					break;
				case "myAccount.aspx":
					aHome.Attributes.Add("class", "first");
					aMyAccount.Attributes.Add("class", "topactive");
					break;
				default:
					aHome.Attributes.Add("class", "first");
					aMyAccount.Attributes.Add("class", "last");
					break;
			}
		}
		
		protected void lbtnLogout_Click(object sender, System.EventArgs e)
		{
			//AdministrativeAPI.SaveUserSessionEndTime (Convert.ToInt64 (Session["SessionID"]));
			FormsAuthentication.SignOut();
			Session.Abandon();
			Response.Redirect("login.aspx");
		}
	}
}
