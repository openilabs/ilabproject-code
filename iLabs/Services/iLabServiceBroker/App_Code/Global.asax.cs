/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: Global.asax.cs,v 1.11 2007/05/31 20:58:47 pbailey Exp $ */

using System;
using System.Collections;

using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Web.Security;
using System.Web;
using System.Web.SessionState;

using iLabs.ServiceBroker.DataStorage;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker;
using iLabs.UtilLib;


namespace iLabs.ServiceBroker.iLabSB 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
        private SBTicketRemover ticketRemover;

        static Global()
        {
            Utilities.WriteLog("ISB: Global Static started");
        }
		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			string path = ConfigurationSettings.AppSettings["logPath"];
            if (path != null && path.Length > 0)
            {
                Utilities.LogPath = path;
                Utilities.WriteLog("ISB Application_Start: starting");
            }
			// The AuthCache class is defined in the Authorization
			AuthCache.GrantSet = InternalAuthorizationDB.RetrieveGrants();
			AuthCache.QualifierSet = InternalAuthorizationDB.RetrieveQualifiers();
			AuthCache.QualifierHierarchySet = InternalAuthorizationDB.RetrieveQualifierHierarchy();
			AuthCache.AgentHierarchySet = InternalAuthorizationDB.RetrieveAgentHierarchy();
			AuthCache.AgentsSet = InternalAuthorizationDB.RetrieveAgents();
			//Application["ServerID"] = ConfigurationSettings.AppSettings["sbGID"];
            ticketRemover = new SBTicketRemover();
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
            object obj = Request;
            // Check for cookie added to Applet page
            HttpCookie cookie = Request.Cookies[ConfigurationManager.AppSettings["isbAuthCookieName"]];
            if (cookie != null)
            {
                object cValue = cookie.Value;
                if (cValue != null)
                {
                    long sesID = Convert.ToInt64(cValue);
                    SessionInfo info = AdministrativeAPI.GetSessionInfo(sesID);
                    if (info != null)
                    {
                        Session["SessionID"] = sesID;
                        Session["UserID"] = info.userID;
                        Session["UserName"] = info.userName;
                        if (info.groupID > 0)
                        {
                            Session["GroupID"] = info.groupID;
                            Session["GroupName"] = info.groupName;
                        }
                        if (info.clientID != null && info.clientID > 0)
                            Session["ClientID"] = info.clientID;
                        AdministrativeAPI.SetSessionKey(sesID, Session.SessionID);
                    }

                }
            }
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
            
			if (Request.Path.IndexOf('\\') >= 0 ||
				System.IO.Path.GetFullPath(Request.PhysicalPath) != Request.PhysicalPath) 
			{
				throw new HttpException(404, "not found");
			}
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
			if(ex is HttpUnhandledException)
			{
				EventLog.WriteEntry(this.Application.ToString(),ex.Message,EventLogEntryType.Error);
				
				Server.Transfer("reportBug.aspx?ex=true");
			}

		}

		protected void Session_End(Object sender, EventArgs e)
		{
			AdministrativeAPI.SaveUserSessionEndTime (Convert.ToInt64 (Session["SessionID"]));
			Session.RemoveAll();
			FormsAuthentication.SignOut ();
		}

		protected void Application_End(Object sender, EventArgs e)
		{
            if (ticketRemover != null)
                ticketRemover.Stop();
            Utilities.WriteLog("ISB Application_End:");

		}

		public static string FormatRegularURL(HttpRequest r, string relativePath)
		{
			string protocol = ConfigurationSettings.AppSettings["regularProtocol"];
			string serverName = 
				HttpUtility.UrlEncode(r.ServerVariables["SERVER_NAME"]);
			string vdirName = r.ApplicationPath;
			string formattedURL = protocol+"://"+serverName+vdirName+"/"+relativePath;
			return formattedURL;
		}

		public static string FormatSecureURL(HttpRequest r, string relativePath)
		{
			string protocol = ConfigurationSettings.AppSettings["secureProtocol"];
			string serverName = 
				HttpUtility.UrlEncode(r.ServerVariables["SERVER_NAME"]);
			string vdirName = r.ApplicationPath;
			string formattedURL = protocol+"://"+serverName+vdirName+"/"+relativePath;
			return formattedURL;
		}


			
		#region Web Form Designer generated code
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

