/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
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
using System.Configuration;
using iLabs.UtilLib;

namespace iLabs.LabServer.BEE
{
	/// <summary>
	/// iLabs Home Page
	/// </summary>
	public partial class home : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label Label1;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Session["UserID"] == null)
			{
				bool requireSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["haveSSL"]);
				string Url;
				if ((requireSSL)&&(!Request.IsSecureConnection))
				{
					Url = Global.FormatSecureURL(Request,"home.aspx");
					Response.Redirect(Url);
				}
				else 
				if ((!requireSSL)&&(Request.IsSecureConnection))
				{
					Url = Global.FormatRegularURL(Request,"home.aspx");
					Response.Redirect(Url);
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

	}
}
