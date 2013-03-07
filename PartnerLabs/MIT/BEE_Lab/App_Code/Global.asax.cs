/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: Global.asax.cs 469 2011-10-26 21:13:05Z phbailey $
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using System.Runtime.InteropServices;
using System.Threading;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;

using iLabs.LabServer.Interactive;

namespace iLabs.LabServer.BEE 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		
		static protected Thread threadLV;
        //static public Hashtable taskTable;
        public static TaskHandler taskThread;
        //public static TaskProcessor tasks;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private TicketRemover ticketRemover;
        private ThreadedProcess pinger;
		
        

	

		public static void ThreadProc() 
		{
			//LVDLLStatus(IntPtr.Zero, 0, IntPtr.Zero);
			//PumpMessages();
		}

		static Global()
		{
            if (ConfigurationManager.AppSettings["logPath"] != null
                && ConfigurationManager.AppSettings["logPath"].Length > 0)
            {
               Logger.LogPath = ConfigurationManager.AppSettings["logPath"];
               Logger.WriteLine("");
               Logger.WriteLine("#############################################################################");
               Logger.WriteLine("");
               Logger.WriteLine("Global Static started: " + iLabGlobal.Release + " -- " + iLabGlobal.BuildDate);
            }
            ProcessAgentDB.RefreshServiceAgent();
			//threadLV = new Thread(new ThreadStart(ThreadProc));
			//threadLV.Start();
          
			
            //taskTable = new Hashtable();
            //tasks = new TaskProcessor();
           Logger.WriteLine("Global Static ended");
		}

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
            try
            {
                string path = ConfigurationManager.AppSettings["logPath"];
                if (path != null && path.Length > 0)
                {
                    Logger.LogPath = path;
                    Logger.WriteLine("");
                    Logger.WriteLine("#############################################################################");
                    Logger.WriteLine(iLabGlobal.Release);
                    Logger.WriteLine("Application_Start: starting");
                }
                ProcessAgentDB.RefreshServiceAgent();
                //Should load any active tasks and update any expired tasks
                LabDB dbService = new LabDB();
                pinger = new ThreadedProcess();
                pinger.ProcessName("PingServer");
                pinger.WaitTime = 360; //6 minutes
                pinger.Process = PingServer;
                pinger.Start();

                TaskProcessor.Instance.WaitTime = 60000;
                LabTask[] activeTasks = dbService.GetActiveTasks();
                int count = 0;
                foreach (LabTask task in activeTasks)
                {
                    if (task != null)
                    {
                        if (task.storage != null && task.storage.Length > 0)
                        {
                            Coupon expCoupon = dbService.GetCoupon(task.couponID, task.issuerGUID);
                            DataSourceManager dsManager = new DataSourceManager(task);
                            BeeAPI api = new BeeAPI();
                            FileWatcherDataSource fds = api.CreateBeeDataSource(expCoupon, task, "data", false);
                            dsManager.AddDataSource(fds);
                            fds.Start();
                            TaskProcessor.Instance.AddDataManager(task.taskID, dsManager);
                        }
                        TaskProcessor.Instance.Add(new BeeTask(task));
                        count++;
                    }
                }
                Logger.WriteLine("Added " + count + " active Tasks");
                taskThread = new TaskHandler(TaskProcessor.Instance);
                ticketRemover = new TicketRemover();
            }
            catch (Exception err)
            {
                Logger.WriteLine(Utilities.DumpException(err));
            }
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
            Logger.WriteLine("Session_Start: " + sender.ToString() + " \t EventType: " + e.GetType());
           // Exception ex = new Exception("Session_Start:");
           // string tmp = Utilities.DumpException(ex);
           //Logger.WriteLine(Utilities.DumpException(ex));

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			// In InteractiveLabView
            Logger.WriteLine("Request: " + sender.ToString() + " \t EventType: " + e.GetType());

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{
            
           Logger.WriteLine("Application_Error: " + sender.ToString() + " \t EventType: " + e.GetType());
           Exception ex = null;
            Exception err = Server.GetLastError();
            if(e != null)
                ex = new Exception("Application_Error: ",err);
            else
                ex = new Exception("Application_Error: No Exception returned");
           Logger.WriteLine(Utilities.DumpException(ex));
		}

		protected void Session_End(Object sender, EventArgs e)
		{
           Logger.WriteLine("Session_End: " + sender.ToString() + " \t EventType: " + e.GetType());
           // string tmp = Utilities.DumpException(ex);
           //Logger.WriteLine(Utilities.DumpException(ex));

		}
		

		protected void Application_End(Object sender, EventArgs e)
		{
            if (ticketRemover != null)
                ticketRemover.Stop();
            TaskProcessor.Instance.Stop();
            if (pinger != null)
                pinger.Stop();
			Logger.WriteLine("Application_End Called:");
           Logger.WriteLine("Application_End: closing");
            //try to restart a new Application
           Global.PingServer();
			
		}
		override public void Dispose()
		{
           Logger.WriteLine("GLOBAL:Dispose Called:");
			Application_End(this, null);
			base.Dispose();
		}

        public static void PingServer()
        {
            try
            {
                WebClient http = new WebClient();
                string result = http.DownloadString(ProcessAgentDB.ServiceAgent.codeBaseUrl + "/pingMe.aspx");
                //ToDo: remove after debugging
                Logger.WriteLine("PingServer: OK");
            }
            catch (Exception ex)
            {
                Logger.WriteLine("PingServer: " + ex.Message);
            }
        }

        public static string FormatRegularURL(HttpRequest r, string relativePath)
        {
            string protocol = ConfigurationManager.AppSettings["regularProtocol"];
            string serverName =
                HttpUtility.UrlEncode(r.ServerVariables["SERVER_NAME"]);
            string vdirName = r.ApplicationPath;
            string formattedURL = protocol + "://" + serverName + vdirName + "/" + relativePath;
            return formattedURL;
        }

        public static string FormatSecureURL(HttpRequest r, string relativePath)
        {
            string protocol = ConfigurationManager.AppSettings["secureProtocol"];
            string serverName =
                HttpUtility.UrlEncode(r.ServerVariables["SERVER_NAME"]);
            string vdirName = r.ApplicationPath;
            string formattedURL = protocol + "://" + serverName + vdirName + "/" + relativePath;
            return formattedURL;
        }
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}


	

}

