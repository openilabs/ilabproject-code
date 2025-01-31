/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: BEEstart.aspx.cs 689 2013-05-08 16:51:41Z phbailey $
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.LabServer.Interactive;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.LabServer;

using CR1000Connection;

namespace iLabs.LabServer.BEE
{
	/// <summary>
    /// BEEstart is a page for creating a load profile for the BEE lab
	/// </summary>
	public partial class BEEstart : System.Web.UI.Page
	{
        protected bool showTime;
		protected System.Web.UI.WebControls.Label lblCoupon;
		protected System.Web.UI.WebControls.Label lblTicket;
		protected System.Web.UI.WebControls.Label lblGroupNameTitle;
        protected string title = "Building Energy Efficiency iLab";

        int userTZ;
        int userID = -1;
        int groupID;
        long couponID = -1;
        string passKey = null;
        string issuerID = null;
        CultureInfo culture = null;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            LabDB dbManager = new LabDB();
            if (!IsPostBack)
            {
                // Query values from the request
                //clearSessionInfo();
                hdnExpId.Value = Request.QueryString["expid"];
                hdnCoupon.Value = Request.QueryString["coupon_id"];
                hdnPasscode.Value = Request.QueryString["passkey"];
                hdnIssuer.Value = Request.QueryString["issuer_guid"];
                hdnSbUrl.Value = Request.QueryString["sb_url"];
               string expLen  = Request.QueryString["explen"];
               string timeUnit = Request.QueryString["tu"];
               if (timeUnit != null && timeUnit.Length > 0)
                   hdnTimeUnit.Value = timeUnit;
                
                string userName = null;
                string userIdStr = null;


                int tz = 0;
                if (Session["userTZ"] != null)
                    tz = Convert.ToInt32(Session["userTZ"]);
                if (Session["sbUrl"] != null)
                {
                    String returnURL = (string)Session["sbUrl"];
                }

                //// this should be the RedeemSession & Experiment Coupon data
                if (!(Session["opPasscode"] != null && Session["opPasscode"] != ""
                    && Session["opCouponID"] != null && Session["opCouponID"] != ""
                    && Session["opIssuer"] != null && Session["opIssuer"] != ""))
                {
                    Logger.WriteLine("BEEstart: " + "AccessDenied missing credentials");
                    Response.Redirect("AccessDenied.aspx?text=missing+credentials.", true);
                }

                Coupon expCoupon = new Coupon(Session["opIssuer"].ToString(), Convert.ToInt64(Session["opCouponID"]), Session["opPasscode"].ToString());

                //Check the database for ticket and coupon, if not found Redeem Ticket from
                // issuer and store in database.
                //This ticket should include group, experiment id and be valid for this moment in time??
                Ticket expTicket = dbManager.RetrieveAndVerify(expCoupon, TicketTypes.EXECUTE_EXPERIMENT);

                if (expTicket != null)
                {
                    if (expTicket.IsExpired())
                    {
                        Response.Redirect("AccessDenied.aspx?text=The ExperimentExecution+ticket+has+expired.", true);

                    }
                    Session["exCoupon"] = expCoupon;

                    ////Parse experiment payload, only get what is needed 	
                    string payload = expTicket.payload;
                    XmlQueryDoc expDoc = new XmlQueryDoc(payload);
                    string expIdStr = expDoc.Query("ExecuteExperimentPayload/experimentID");
                    hdnExpId.Value = expIdStr;
                    string tzStr = expDoc.Query("ExecuteExperimentPayload/userTZ");
                    //string userIdStr = expDoc.Query("ExecuteExperimentPayload/userID");
                    string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
                    Session["groupName"] = groupName;
                    string sbStr = expDoc.Query("ExecuteExperimentPayload/sbGuid");
                    Session["brokerGUID"] = sbStr;
                    string startStr = expDoc.Query("ExecuteExperimentPayload/startExecution");
                    string durStr = expDoc.Query("ExecuteExperimentPayload/duration");
                    writeExpLength(startStr,durStr);
                    if ((tzStr != null) && (tzStr.Length > 0))
                    {
                        Session["userTZ"] = tzStr;
                    }

                }
            }
        }

        protected void writeExpLength(string startStr, string durStr)
        {
            if (startStr != null && durStr != null && startStr.Length > 0 && durStr.Length > 0)
            {
                DateTime start = DateUtil.ParseUtc(startStr);
                int duration = Convert.ToInt32(durStr);
                DateTime end = start.AddSeconds(duration);
                double hours = end.Subtract(DateTime.UtcNow).TotalHours;
                int runTime = (int) Math.Truncate(hours);
                if (runTime < 0)
                {
                    throw new Exception("Remaining time to run experiment is negative!");
                }
                if (runTime < 1)
                {
                    runTime = 1; ;
                }
                hdnExpLength.Value = runTime.ToString();
            }
            else
            {
                hdnExpLength.Value = "24";
            }
            StringBuilder buf = new StringBuilder("<script type=\"text/javascript\">");
            buf.Append(" window.labLength = '" + hdnExpLength.Value + "';");
            buf.AppendLine("</script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "labLength", buf.ToString(), false);
        }

        /*
        protected void clearSessionInfo()
        {
            Session.Remove("opCouponID");
            Session.Remove("opIssuer");
            Session.Remove("opPasscode");
        }
        */




        protected void goButton_Click(object sender, System.EventArgs e)
        {
            LabDB labDB = new LabDB();

            // Update Task data for graph page. Note XmlQueryDocs are read-only
            LabTask task = labDB.GetTask(Convert.ToInt64(hdnExpId.Value), Session["opIssuer"].ToString());
            if (task != null)
            {
                Coupon opCoupon = new Coupon(task.issuerGUID, task.couponID, Session["opPasscode"].ToString());
               ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
               essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
               essProxy.OperationAuthHeaderValue.coupon = opCoupon;
               essProxy.Url = task.storage;
               essProxy.AddRecord(task.experimentID, "BEElab", "profile", false, hdnProfile.Value, null);

               // send The program
               sendProfile(ConfigurationManager.AppSettings["climateController"],
                   hdnProfile.Value, ConfigurationManager.AppSettings["climateServer"]);
               sendFile(ConfigurationManager.AppSettings["chamberController"],
                  ConfigurationManager.AppSettings["chamberFile"], 
                  ConfigurationManager.AppSettings["chamberServer"]);
                StringBuilder buf = new StringBuilder("BEEgraph.aspx?expid=");
                buf.Append(task.experimentID);
                task.Status = LabTask.eStatus.Running;
                TaskProcessor.Instance.Modify(task);
                labDB.SetTaskStatus(task.taskID, (int) LabTask.eStatus.Running);
                Session["opCouponID"] = hdnCoupon.Value;
                Session["opIssuer"] = hdnIssuer.Value;
                Session["opPasscode"] = hdnPasscode.Value;
                Response.Redirect(buf.ToString(), true);
            }
            else
            {
                throw new Exception("Task was not found.");
            }
        }


        private void sendFile(string loggerName, string filePath, string serverName){
             Server cr1000 = new Server(serverName);
            cr1000.sendProgramFile(loggerName,filePath);
        }

        private void sendProfile(string loggerName, string profile, string serverName)
        {
            bool status = false;
            
            string programPath = ConfigurationManager.AppSettings["climateProgramDir"] + @"\" + "program" + hdnExpId.Value + ".CR1";
            // read the template
            // parse it & render it & save it to tmp/file
            // send that file

            Hashtable hashtable = new Hashtable();
            hashtable["experimentLength"] = hdnExpLength.Value;
            hashtable["profile"] = profile;
            hashtable["totalLoads"] = 4;
            hashtable["timeUnit"] = hdnTimeUnit.Value;
            hashtable["sampleRate"] = hdnSampleRate.Value;
            iLabParser parser = new iLabParser();
            parser.SetEndToken("}$");
            string temp = File.ReadAllText(ConfigurationManager.AppSettings["climateTemplate"]);
            //string temp = ReadUrl(@"programs\basicExperimentTemplate.txt");
            File.WriteAllText(programPath, parser.Parse(temp, hashtable));
            sendFile(loggerName,programPath,serverName);
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
