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
using System.Configuration;
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
    public partial class launchBEElab : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            LabDB dbManager = new LabDB();
            int appID = 0;
            long expID = 0;
            LabTask task = null;
            string expLen = null;
            string pageURL = null;
            bool useTaskDuration = true;
            // Query values from the request
            if (!IsPostBack)
            {
                string appKey = Request.QueryString["app"];
                string coupon_Id = Request.QueryString["coupon_id"];
                string passkey = Request.QueryString["passkey"];
                string issuerGUID = Request.QueryString["issuer_guid"];
                string returnTarget = Request.QueryString["sb_url"];
                expLen = Request.QueryString["explen"];
                if ((returnTarget != null) && (returnTarget.Length > 0))
                    Session["sbUrl"] = returnTarget;

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
                    Session[""] = issuerGUID;
                    Session[""] = expCoupId;
                    Session[""] = passkey;
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
                    // This is no longer the case as the USS handles groups and permissions
                    //else // Have to use groupName & Servicebroker THIS REQUIRES groups & permissions are set in database
                    //{   
                    //    appInfo = dbManager.GetLabAppForGroup(groupName, sbStr);
                    //}
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
                                //ToDo: check if the tasks should all be closed
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
                        BeeAPI factory = new BeeAPI();
                        task = factory.CreateLabTask(appInfo, expTicket);

                        if (task != null)
                        {
                            if (task.storage != null && task.storage.Length > 0)
                            {
                                DataSourceManager dsManager = new DataSourceManager(task);
                                FileWatcherDataSource fds = factory.CreateBeeDataSource(expCoupon, task, "data", true);
                                dsManager.AddDataSource(fds);
                                fds.Start();
                                TaskProcessor.Instance.AddDataManager(task.taskID, dsManager);
                            }
                            TaskProcessor.Instance.Add(new BeeTask(task));


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
                        task = dbManager.GetTask(expID, sbStr);
                        if (task.Status == LabTask.eStatus.Scheduled)
                        {
                            pageURL = appInfo.appURL;
                        }
                        else if (task.Status == LabTask.eStatus.Running)
                        {
                            pageURL = ProcessAgentDB.ServiceAgent.codeBaseUrl + @"/BEEgraph.aspx";
                        }
                        else if (task.Status == LabTask.eStatus.Aborted
                            || task.Status == LabTask.eStatus.Closed
                            || task.Status == LabTask.eStatus.Completed
                            || task.Status == LabTask.eStatus.Expired)
                        {
                            Response.Redirect("AccessDenied.aspx?text=The+requested+experiment+is+no+longer+running.", true);
                        }

                    }
                }
                if (pageURL != null && pageURL.Length > 0)
                {
                    StringBuilder buf = new StringBuilder(pageURL + "?expid=" + expID);
                    Session["opCouponID"] = coupon_Id;
                    Session["opPasscode"] = passkey;
                    Session["opIssuer"] = issuerGUID;
                    buf.Append("&coupon_id=" + coupon_Id);
                    buf.Append("&passkey=" + passkey);
                    buf.Append("&issuer_guid=" + issuerGUID);
                    buf.Append("&sb_url=" + returnTarget);
                    if(expLen != null && expLen.Length > 0)
                        buf.Append("&explen=" + expLen);
                    //if (useTaskDuration)
                    //{
                    //    TimeSpan taskDur = task.endTime.Subtract(DateTime.UtcNow);
                    //    int hours = Convert.ToInt32(taskDur.TotalHours);
                    //    buf.Append("&explen=" + hours);
                    //}
                    Response.Redirect(buf.ToString(), true);
                }
                else
                {
                    Response.Redirect("AccessDenied.aspx?text=Unable+to+launch++application,+please+notify+your+administrator.", true);
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
