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
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Xml.XPath;

using iLabs.Core;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;

using iLabs.LabServer.Interactive;


namespace iLabs.LabServer.BEE
{
    /// <summary>
    /// Summary description for Portal.
    /// </summary>
    public partial class BEElabPortal : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            LabDB dbManager = new LabDB();
            int appID = 0;
            long expID = 0;
            LabTask task = null;
            string pageURL = null;
            // Query values from the request
            
            string appKey = Request.QueryString["app"];
            string coupon_Id = Request.QueryString["coupon_id"];
            string passkey = Request.QueryString["passkey"];
            string issuerGUID = Request.QueryString["issuer_guid"];
            string returnTarget = Request.QueryString["sb_url"];

            if ((returnTarget != null) && (returnTarget.Length > 0))
                Session["returnURL"] = returnTarget;
            
           Logger.WriteLine("BEElab: " + Request.Url.ToString());

            // this should be the Experiment Coupon data
            if (!(passkey != null && passkey != "" && coupon_Id != null && coupon_Id != "" && issuerGUID != null && issuerGUID != ""))
            {
                Logger.WriteLine("BEElab: " + "AccessDenied missing Experiment credentials");
                Response.Redirect("AccessDenied.aspx?text=missing+Experiment+credentials.", true);
            }
            
            long expCoupId = Convert.ToInt64(coupon_Id);
            Coupon expCoupon = new Coupon(issuerGUID, expCoupId, passkey);

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

                ////Parse experiment payload, only get what is needed 	
                string payload = expTicket.payload;
                XmlQueryDoc expDoc = new XmlQueryDoc(payload);
                string tzStr = expDoc.Query("ExecuteExperimentPayload/userTZ");
                if ((tzStr != null) && (tzStr.Length > 0))
                    Session["userTZ"] = tzStr;
                string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
                Session["groupName"] = groupName;
                string sbStr = expDoc.Query("ExecuteExperimentPayload/sbGuid");
                string essUrl = expDoc.Query("ExecuteExperimentPayload/essWebAddress");
                expID = Convert.ToInt64(expDoc.Query("ExecuteExperimentPayload/experimentID"));
                Session["brokerGUID"] = sbStr;

                //Get Lab specific info for this URL or group
                LabAppInfo appInfo = null;
                // Experiment is specified by 'app=appKey'
                if (appKey != null && appKey.Length > 0)
                {
                    appInfo = dbManager.GetLabApp(appKey);
                }
                else // Have to use groupName & Servicebroker THIS REQUIRES groups & permissions are set in database
                {    // This is no longer the case as the USS handles groups and permissions
                    appInfo = dbManager.GetLabAppForGroup(groupName, sbStr);
                }
                if (appInfo == null)
                {
                    Response.Redirect("AccessDenied.aspx?text=Unable+to+find+application+information,+please+notify+your+administrator.", true);
                }

                // Check to see if an experiment with this ID is already running
                // Check for an existing task for this experiment
                // If found redirect to the graph page, do not abort running controller program
                LabTask.eStatus status = dbManager.ExperimentStatus(expID, sbStr);
                if (status == LabTask.eStatus.NotFound)
                { 
                    // Check for existing tasks that may be using resources
                    // For now only close other instances of the lab
                    List<LabTask> curTasks = TaskProcessor.Instance.GetTasks(appInfo.appID);
                    if (curTasks != null && curTasks.Count > 0)
                    {
                        foreach (LabTask t in curTasks)
                        {
                            DataSourceManager dsManager = TaskProcessor.Instance.GetDataManager(t.taskID);
                            if (dsManager != null)
                            {
                                // Need to close existing data Sources
                                TaskProcessor.Instance.Remove(t);
                                t.Close();
                            }
                        }
                    }
                    
                    // Create a new Experiment task
                    // Use taskFactory to create a new task
                    LabTaskFactory factory = new LabTaskFactory();
                    task = factory.CreateLabTask(appInfo, expCoupon, expTicket);

                    if (task != null)
                    {
                        string filePath = "c:\\logs\\Campbell\\LoggerNet";
                        string fileName = "CECI-CR10X_final_storage_1.dat";
                        // Stop the controller and flush the data file

                        //Flush the File
                        FileInfo fInfo = new FileInfo(filePath +"\\" + fileName);
                        using (FileStream inFile = fInfo.Open(FileMode.Truncate)) { }

                        string pushChannel = ChecksumUtil.ToMD5Hash("BEElab" + expID);
                        //Add BEElab specific attributes
                        BeeEventHandler bEvt = new BeeEventHandler(expCoupon,expID,essUrl,
                            "data",ProcessAgentDB.ServiceGuid);
                        bEvt.PusherChannel = pushChannel;
                        DataSourceManager dsManager = TaskProcessor.Instance.GetDataManager(task.taskID);
                        FileWatcherDataSource fds = new FileWatcherDataSource();
                        fds.Path = filePath;
                        fds.Filter = fileName;
                        fds.AddFileSystemEventHandler(bEvt.OnChanged);
                        dsManager.AddDataSource(fds);
                        fds.Start();

                        //Useful for debugging overloads the use of a field in the banner
                        //Session["GroupName"] = "TaskID: " + task.taskID.ToString();

                        //Utilities.WriteLog("TaskXML: " + task.taskID + " \t" + task.data);

                        ////Construct the information to be passed to the target page
                        //TimeSpan taskDur = task.endTime - task.startTime;
                        //string vipayload = LabTask.constructSessionPayload(appInfo,
                        //    task.startTime, taskDur.Ticks / TimeSpan.TicksPerSecond, task.taskID,
                        //    returnTarget, null, null);

                        ////Utilities.WriteLog("sessionPayload: " + payload);
                        ////Store Session information
                        //Session["payload"] = vipayload;
                        //if (appInfo.rev != null && appInfo.rev.Length > 0)
                        //{
                        //    Session["lvversion"] = appInfo.rev;
                        //}
                        //else
                        //{
                        //    Session.Remove("lvversion");
                        //}

                        //set Presentation page tp appPage
                        pageURL = appInfo.page;
                       
                    }
                    else
                    {
                        Response.Redirect("AccessDenied.aspx?text=Unable+to+launch++application,+please+notify+your+administrator.", true);
                    }
                }
                else
                { // An existing Experiment
                    task = TaskProcessor.Instance.GetTask(expID, sbStr);
                    if (task.data != null)
                    {
                        XmlQueryDoc taskDoc = new XmlQueryDoc(task.data);
                        pageURL = taskDoc.Query("task/application");
                    }
                    if (pageURL == null || pageURL.Length == 0)
                    {
                        pageURL = appInfo.page;
                    }
                }
            }
            if(pageURL != null && pageURL.Length >0){
                StringBuilder buf = new StringBuilder(pageURL + "?expid=" + expID);
                Session["opCouponID"] = coupon_Id;
                Session["opPasscode"] = passkey;
                Session["opIssuer"] = issuerGUID;
                buf.Append("&coupon_id=" + coupon_Id);
                buf.Append("&passkey=" + passkey);
                buf.Append("&issuer_guid=" + issuerGUID);
                buf.Append("&sb_url=" + returnTarget );
                Response.Redirect(buf.ToString(), true);
            }
            else{
                 Response.Redirect("AccessDenied.aspx?text=Unable+to+launch++application,+please+notify+your+administrator.", true);
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
