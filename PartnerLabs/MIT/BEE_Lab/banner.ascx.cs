/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: banner.ascx.cs 547 2012-04-17 20:20:00Z phbailey $
 */

namespace iLabs.LabServer.LabView
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///	User Control/Include File
	///	Standard header that provides User Name and Effective Group.
	/// </summary>
	public partial class banner : System.Web.UI.UserControl
	{
		string referringPage;
        protected string bannerText = "LabVIEW Interactive iLab Server";
        protected string imageURL = "~/images/MITiCampus_Logo_White.gif";

		//AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();

		protected void Page_Load(object sender, System.EventArgs e)
		{
            lblTextBanner.Text = bannerText;
            imgBanner.ImageUrl = imageURL;
			// Get User Name
			if(Session["userName"] != null)
			{
				lblUserNameBanner.Visible=true;
				lblUserNameBanner.Text="User: " + Session["userName"].ToString();
			}
			else
			{
				lblUserNameBanner.Visible = false;
			}
			
			// Get Group Name
			if(Session["groupName"] != null)
			{
				lblGroupNameBanner.Text="Group: " +Session["groupName"].ToString();
				lblGroupNameBanner.Visible = true;
			}
			else
			{
				lblGroupNameBanner.Visible = false;
			}
		}

        public string ImageURL
        {
            get { return imageURL; }
            set { imageURL = value; }
        }

        public string BannerText
        {
            get { return bannerText; }
            set { bannerText = value; }
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
	}
}
