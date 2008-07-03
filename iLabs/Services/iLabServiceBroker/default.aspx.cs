/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: default.aspx.cs,v 1.1.1.1 2006/02/07 22:10:57 pbailey Exp $ */

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
using System.Configuration;

namespace iLabs.ServiceBroker.iLabSB
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public partial class _default : System.Web.UI.Page
	{
		protected string showUrl;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			bool requireSSL = Convert.ToBoolean(ConfigurationSettings.AppSettings["haveSSL"]);
			if (requireSSL)
				showUrl = Global.FormatSecureURL(Request,"home.aspx");
			else
				showUrl = Global.FormatRegularURL(Request,"home.aspx");
			
			if(Request.Params["login"] != null)
				showUrl = "login.aspx";
			// Put user code to initialize the page here
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
