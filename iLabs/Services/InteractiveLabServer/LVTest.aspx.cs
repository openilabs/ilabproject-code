/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: LVTest.aspx.cs,v 1.9 2007/06/21 21:13:53 pbailey Exp $
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Xml.XPath;

using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;

using iLabs.Services;
using iLabs.LabView;
using iLabs.LabView.LV82;
using LabVIEW;
using iLabs.LabServer.Interactive;



namespace iLabs.LabServer.LabView
{
    /// <summary>
    /// A very simple test of the LabViewInterface will not require scheduling or ESS, does not use tickets.
    /// </summary>
    public partial class LVTest : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            LabDB dbManager = new LabDB();
            int vid = 1;
            string statusViName = null;
            string statusTemplate = null;
            string templatePath = null;
            // These are generated by the ExperimentRecipe
            // And are the experiment collection Coupon
            string vi_Id = Request.QueryString["vid"];
            string durationStr = Request.QueryString["dur"];
            string essService = null;
            string groupName = "LabView Users";
            string returnTarget = "http://thrush.mit.edu/iLabs/LSLS";
            string qualName = null;
            string strRedirect = null;
            int status = -10;

            // Hack for testing
            string sbString = "DV_ISB-824B-4AD4-B5B3-6AF091CCDEEC";
            VirtualInstrument vi = null;

            Utilities.WriteLog("LVTest: " + Request.Url.ToString());




            string tzStr = "-5";
            if ((tzStr != null) && (tzStr.Length > 0))
                Session["userTZ"] = tzStr;

            // set defaults
            DateTime startTime = DateTime.UtcNow;
            long duration = 120L;
            long experimentID = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMinute;
            //int groupID = 0;

            if ((vi_Id != null) && (vi_Id.Length > 0))
            {
                vid = Convert.ToInt32(durationStr);
            }

            if ((durationStr != null) && (durationStr.Length > 0) && !(durationStr.CompareTo("-1") == 0))
            {
                duration = Convert.ToInt64(durationStr);
            }



            // log the experiment for debugging

            Utilities.WriteLog("Experiment: " + experimentID + " Start: " + DateUtil.ToUtcString(startTime) + " \tduration: " + duration);

            try
            {

                //Get Lab specific info for this group
                LabAppInfo viInfo = null;
                if (vid > 0)
                {
                    viInfo = dbManager.GetLabApp(vid);
                }

                if (viInfo.extraInfo != null)
                {
                    // Note should have either statusVI or template pair
                    XmlQueryDoc viDoc = new XmlQueryDoc(viInfo.extraInfo);
                    statusViName = viDoc.Query("extra/statusVi");
                    statusTemplate = viDoc.Query("extra/statusTemplate");
                    templatePath = viDoc.Query("extra/templatePath");
                }
                strRedirect = viInfo.page;


                LabViewInterface lvi = null;

                if ((viInfo.server != null) && (viInfo.server.Length > 0) && (viInfo.port > 0))
                {
                    lvi = new LabViewRemote(viInfo.server, viInfo.port);
                }
                else
                {
                    lvi = new LabViewInterface();
                }

                if (!lvi.IsLoaded(viInfo.application))
                {
                    vi = lvi.loadVI(viInfo.path,viInfo.application, true,0);
                    vi.OpenFrontPanel(true, FPStateEnum.eVisible);
                }
                else
                {
                    vi = lvi.GetVI(viInfo.path, viInfo.application);
                }
                if (vi == null)
                {
                    status = -1;
                    string err = "Unable to Find: " + viInfo.path + @"\" + viInfo.application;
                    Utilities.WriteLog(err);
                    //throw new Exception(err);
                }
                // Hack to force path\Name
                qualName = lvi.qualifiedName(vi);
                //string names = lvi.testNames();
                //Utilities.WriteLog(LVTest names: " + names);
                //lvi.submitRemoteCommand("appStatus", "", vi);
                //lvi.submitRemoteCommand("getControlState", "", vi);
                //lvi.submitRemoteCommand("unlockControl", "", vi);
                //lvi.submitRemoteCommand("getControlState", "", vi);
                //lvi.submitRemoteCommand("lockControl", "", vi);
                //lvi.submitRemoteCommand("getControlState", "", vi);

                status = lvi.GetVIStatus(vi);

                Utilities.WriteLog("LVTest - VIstatus: " + status);
                switch (status)
                {
                    case -10:
                        throw new Exception("Error GetVIStatus: " + status);
                        break;
                    case -1: // Not in memory
                        break;
                    case 0: // eBad == 0
                        break;
                    case 1: // eIdle == 1 vi in memory but not running
                        FPStateEnum fpState = vi.FPState;
                        if (fpState != FPStateEnum.eVisible)
                        {
                            vi.OpenFrontPanel(true, FPStateEnum.eVisible);
                        }
                        break;
                    case 2: // eRunTopLevel
                        
                        break;
                    case 3: // eRunning
                        break;
                    default:
                        throw new Exception("Error GetVIStatus: unknown status: " + status);
                        break;
                }
                lvi.SetBounds(vi, 0, 0, viInfo.width, viInfo.height);
                Utilities.WriteLog("SetBounds: " + qualName);
                lvi.SetLockState(vi.Path, false);
                //statusStr = lvi.SubmitAction("statusvi", appInfo.application);


                // Set up in-memory and database task control structures
                DataSourceManager exp = null;
                LabViewTask task = null;

                // Check to see if the experiment is currently running, 
                // or exists and is in a re-entrient state.
                if (dbManager.ExperimentStatus(experimentID, sbString) == LabTask.eStatus.NotFound)
                {



                    // Create the task & store in database;
                    task = (LabViewTask)dbManager.InsertTask(viInfo.appID, experimentID,
                   groupName, startTime, duration,
                   LabTask.eStatus.Scheduled, -1, sbString, null);
                    if ((statusTemplate != null) && (statusTemplate.Length > 0))
                    {
                        statusViName = lvi.CreateFromTemplate(templatePath, statusTemplate, task.taskID.ToString());
                    }
                    string taskData = null;
                    taskData = LabViewTask.constructTaskXml(viInfo.appID, vi.Path, statusViName, essService);
                    dbManager.SetTaskData(task.taskID, taskData);
                    task.data = taskData;

                    if (((essService != null) && (essService.Length > 0)) && ((viInfo.dataSources != null) && (viInfo.dataSources.Length > 0)))
                    {
                        //exp = new LabViewExp();
                        /*
                        string[] sockets = appInfo.dataSources.Split(';');
                        // set up an experiment storage handler
                        ExperimentStorageProxy ess = new ExperimentStorageProxy();
                        ess.OperationAuthHeaderValue = new OperationAuthHeader();
                        ess.OperationAuthHeaderValue.coupon = expCoupon;
                        ess.Url = essService;
                        exp.ESS = ess;
                        exp.ExperimentID = experimentID;
                        exp.VI = fullName;

                        // Use the experimentID as the storage parameter
                        foreach (string s in sockets)
                        {
                            LVDataSocket reader = new LVDataSocket();
                            reader.ExperimentID = experimentID;
                            reader.ESS = ess;
                            reader.ConnectAutoUpdate(s);
                            exp.AddDataSource(reader);
                        }
                       */
                        //Global.taskTable.Add(task.taskID, exp);  
                    }



                    if (Global.tasks != null)
                    {
                        lock (Global.tasks)
                        {
                            Global.tasks.Add(task);
                        }
                    }
                }
                else
                {
                    task = (LabViewTask)Global.tasks.GetTask(experimentID);
                    if ((statusTemplate != null) && (statusTemplate.Length > 0))
                    {
                        statusViName = statusTemplate + task.taskID.ToString() + ".vi";
                    }
                }

                Session["GroupName"] = "TaskID: " + task.taskID.ToString();
                Utilities.WriteLog("TaskXML: " + task.taskID + " \t" + task.data);




                /*					
                                    //create ticket->cookie->session
                                    FormsAuthenticationTicket sTicket;
                                    string strCookie;
                                    HttpCookie cookie;

                                    sTicket = new FormsAuthenticationTicket(1, UserName, IssueDate, DateTime.Now.AddSeconds(30), false, "custom data");
                                    strCookie = FormsAuthentication.Encrypt(sTicket);
                                    cookie = new HttpCookie(FormsAuthentication.FormsCookieName, strCookie);
                                    cookie.Expires=sTicket.Expiration;
                                    cookie.Path = FormsAuthentication.FormsCookiePath;
                                    Response.Cookies.Add(cookie);
                                    */

                string vipayload = LabViewTask.constructSessionPayload(viInfo.appID,viInfo.title, viInfo.application, viInfo.appURL, viInfo.width, viInfo.height, startTime, duration, task.taskID, essService, returnTarget, null, statusViName);
                //spit out ticket
                //Utilities.WriteLog("sessionPayload: " + payload);
                Session["payload"] = vipayload;




                //} // end Hack for vid == 3

                //redirect to Presentation page...
            }
            catch (Exception ex)
            {
                Utilities.WriteLog("LVTest: " + ex.Message);
            }
            finally
            {
                vi = null;
            }

            Response.Redirect(strRedirect, true);
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
