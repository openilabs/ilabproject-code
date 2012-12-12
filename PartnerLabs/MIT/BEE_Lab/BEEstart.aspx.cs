/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */

using System;
using System.Collections;
using System.Collections.Generic;
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
                clearSessionInfo();
                //hdnExpId.Value = Request.QueryString["expid"];
                hdnCoupon.Value = Request.QueryString["coupon_id"];
                hdnPasscode.Value = Request.QueryString["passkey"];
                hdnIssuer.Value = Request.QueryString["issuer_guid"];
                hdnSbUrl.Value = Request.QueryString["sb_url"];
                
                string userName = null;
                string userIdStr = null;


                int tz = 0;
                if (Session["userTZ"] != null)
                    tz = Convert.ToInt32(Session["userTZ"]);
                if (Session["returnURL"] != null)
                {
                    String returnURL = (string)Session["returnURL"];
                }

                //// this should be the RedeemSession & Experiment Coupon data
                if (!(hdnPasscode.Value != null && hdnPasscode.Value != ""
                    && hdnCoupon.Value != null && hdnCoupon.Value != ""
                    && hdnIssuer.Value != null && hdnIssuer.Value != ""))
                {
                    Logger.WriteLine("BEEstart: " + "AccessDenied missing credentials");
                    Response.Redirect("AccessDenied.aspx?text=missing+credentials.", true);
                }
               
                Coupon expCoupon = new Coupon(hdnIssuer.Value, Convert.ToInt64(hdnCoupon.Value), hdnPasscode.Value);

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
                    hdnExpID.Value = expIdStr;
                    string tzStr = expDoc.Query("ExecuteExperimentPayload/userTZ");
                    //string userIdStr = expDoc.Query("ExecuteExperimentPayload/userID");
                    string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
                    Session["groupName"] = groupName;
                    string sbStr = expDoc.Query("ExecuteExperimentPayload/sbGuid");
                    Session["brokerGUID"] = sbStr;

                    if ((tzStr != null) && (tzStr.Length > 0))
                    {
                        Session["userTZ"] = tzStr;
                    }

                }
            }
        }



        protected void clearSessionInfo()
        {
            Session.Remove("opCouponID");
            Session.Remove("opIssuer");
            Session.Remove("opPasscode");
        }





        protected void goButton_Click(object sender, System.EventArgs e)
        {
            LabDB labDB = new LabDB();

            // Update Task data for graph page. Note XmlQueryDocs are read-only
            LabTask task = labDB.GetTask(Convert.ToInt64(hdnExpID.Value),hdnIssuer.Value);
            if (task != null)
            {
                    Coupon opCoupon = new Coupon(task.issuerGUID, task.couponID, hdnPasscode.Value);
                    ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                    essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                    essProxy.OperationAuthHeaderValue.coupon = opCoupon;
                    essProxy.Url = task.storage;
                    essProxy.AddRecord(task.experimentID, "BEElab", "profile", false, hdnProfile.Value, null);

                    // send The program
                    sendProfile("CR1000", hdnProfile.Value,"beelab2.mit.edu");
                    sendFile("CR1000_Test_Chamber","c:\\logs\\programs\\test_chamber.CR1","beelab2.mit.edu");
                    StringBuilder buf = new StringBuilder("BEEgraph.aspx?expid=");
                    buf.Append(task.experimentID);
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

        private void sendFile(string loggerName, string filePath, string server){
             Server cr1000 = new Server(server);
            cr1000.sendProgramFile(loggerName,filePath);
        }

        private void sendProfile(string loggerName, string profile, string serverName)
        {
            bool status = false;
            string outputDirectory =  @"c:\logs\test";
            string programPath = outputDirectory + @"\" + "program" + hdnExpID.Value + ".CR1";
            // read the template
            // parse it & render it & save it to tmp/file
            // send that file

            Hashtable hashtable = new Hashtable();
            hashtable["experimentLength"] = "24";
            hashtable["profile"] = profile;
            hashtable["totalLoads"] = 4;
            
            string temp = File.ReadAllText(@"c:\logs\programs\basicExperimentTemplate.txt");
            //string temp = ReadUrl(@"programs\basicExperimentTemplate.txt");
            File.WriteAllText(programPath, iLabParser.Parse(temp, hashtable));
            sendFile(loggerName,programPath,serverName);
        }

    //    string ReadUrl(string urlStr){
    //      HttpRequest request = new HttpRequest(null,urlStr,null);
    //      HttpResponse response = request.GetResponse();
    //      Stream stream = response.GetResponseStream();
    //      StreamReader reader = new StreamReader(stream);
    //      string text = reader.ReadToEnd();
    //      return text;
    //    }

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
