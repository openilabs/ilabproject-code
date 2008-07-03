using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.LabServer.Interactive;
using iLabs.Services;
using iLabs.UtilLib;

using LabVIEW;
using iLabs.LabView;
using iLabs.LabView.LV82;



namespace iLabs.LabServer.LabView
{

    /// <summary>
    /// Summary description for LabTaskFactory
    /// </summary>
    public class LabViewTaskFactory : LabTaskFactory
    {
        public LabViewTaskFactory()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public override LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        {

            // set defaults
            DateTime startTime = DateTime.UtcNow;
            long duration = -1L;
            long experimentID = 0;
            int status = -1;
            LabTask labTask = null;
            LabViewTask task = null;
            VirtualInstrument vi = null;
            LabViewInterface lvi = null;
            string statusViName = null;
            string statusTemplate = null;
            string templatePath = null;
            LabDB dbManager = new LabDB();
            string qualName = null;
            string fullName = null;

            if (appInfo.extraInfo != null)
            {
                // Note should have either statusVI or template pair
                XmlQueryDoc viDoc = new XmlQueryDoc(appInfo.extraInfo);
                statusViName = viDoc.Query("extra/status");
                statusTemplate = viDoc.Query("extra/statusTemplate");
                templatePath = viDoc.Query("extra/templatePath");
            }
            //Parse experiment payload 	
            string payload = expTicket.payload;
            XmlQueryDoc expDoc = new XmlQueryDoc(payload);

            string essService = expDoc.Query("ExecuteExperimentPayload/essWebAddress");
            string startStr = expDoc.Query("ExecuteExperimentPayload/startExecution");
            string durationStr = expDoc.Query("ExecuteExperimentPayload/duration");
            string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
            string userName = expDoc.Query("ExecuteExperimentPayload/userName");
            string sbStr = expDoc.Query("ExecuteExperimentPayload/sbGuid");
            string experimentStr = expDoc.Query("ExecuteExperimentPayload/experimentID");
           
            if ((startStr != null) && (startStr.Length > 0))
            {
                startTime = DateUtil.ParseUtc(startStr);
            }
            if ((durationStr != null) && (durationStr.Length > 0) && !(durationStr.CompareTo("-1") == 0))
            {
                duration = Convert.ToInt64(durationStr);
            }
            if ((experimentStr != null) && (experimentStr.Length > 0))
            {
                experimentID = Convert.ToInt64(experimentStr);
            }

            // log the experiment for debugging

            Utilities.WriteLog("Experiment: " + experimentID + " Start: " + DateUtil.ToUtcString(startTime) + " \tduration: " + duration);
            long statusSpan = DateUtil.SecondsRemaining(startTime, duration);



            if ((appInfo.server != null) && (appInfo.port != null) && (appInfo.server.Length > 0) && (appInfo.port > 0))
            {
                lvi = new LabViewRemote(appInfo.server, appInfo.port);
            }
            else
            {
                lvi = new LabViewInterface();
            }
            if (!lvi.IsLoaded(appInfo.application))
            {
                vi = lvi.loadVI(appInfo.path, appInfo.application);
				vi.OpenFrontPanel(true, FPStateEnum.eVisible);
            }
            else
            {
                vi = lvi.GetVI(appInfo.path, appInfo.application);
            }
            if (vi == null)
            {
                status = -1;
                string err = "Unable to Find: " + appInfo.path + @"\" + appInfo.application;
                Utilities.WriteLog(err);
                throw new Exception(err);
            }
            // Get qualifiedName
            qualName = lvi.qualifiedName(vi);
            fullName = appInfo.path + @"\" + appInfo.application;
            
            
            status = lvi.GetVIStatus(vi);

            Utilities.WriteLog("CreateLabTask - VIstatus: " + status);
            switch (status)
            {
                case -10:
                    throw new Exception("Error GetVIStatus: " + status);
                    break;
                case -1:
                    // VI not in memory
					throw new Exception("Error GetVIStatus: " + status);
            
                    break;
                case 0: // eBad == 0
                    break;
                case 1: // eIdle == 1 vi in memory but not running 
					FPStateEnum fpState = vi.FPState;
                    if(fpState != FPStateEnum.eVisible)
                    {
                        vi.OpenFrontPanel(true, FPStateEnum.eVisible);
                    }
                    vi.ReinitializeAllToDefault();      
                    break;
                case 2: // eRunTopLevel: this should be the LabVIEW application
                    break;
                case 3: // eRunning
                    //Unless the Experiment is reentrant it should be stopped and be reset.
                    //Currently reentrant not supported
                    lvi.StopVI(vi);
                    vi.ReinitializeAllToDefault();
                    break;
                default:
                    throw new Exception("Error GetVIStatus: unknown status: " + status);
                    break;
            }
            try
            {
                lvi.SetBounds(vi, 0, 0, appInfo.width, appInfo.height);
                Utilities.WriteLog("SetBounds: " + appInfo.application);
            }
            catch (Exception sbe)
            {
                Utilities.WriteLog("SetBounds exception: " + Utilities.DumpException(sbe));
            }
            lvi.SubmitAction("unlockvi", lvi.qualifiedName(vi));
           

            // Set up in-memory and database task control structures
            DataSourceManager dsManager = null;
          

            // Check to see if the experiment is currently running, 
            // or exists and is in a re-entrient state.
            if (dbManager.ExperimentStatus(experimentID, sbStr) == LabTask.eStatus.NotFound)
            {
                // Create the labTask & store in database;
                labTask = dbManager.InsertTask(appInfo.appID, experimentID,
               groupName, startTime, duration,
               LabTask.eStatus.Scheduled, expTicket.couponId, expTicket.issuerGuid, null);
                if (labTask != null)
                {
                    //Convert the generic LabTask to a LabViewTask
                    task = new LabViewTask(labTask);
                }
                if ((statusTemplate != null) && (statusTemplate.Length > 0))
                {
                    statusViName = lvi.CreateFromTemplate(templatePath, statusTemplate, task.taskID.ToString());
                }

                
                if (((essService != null) && (essService.Length > 0)) && ((appInfo.dataSources != null) && (appInfo.dataSources.Length > 0)))
                {
                    /* currently not working
                    // Try & see if the new CreateESSproxy exists and initialize it
                    VirtualInstrument essCreateVi = null;
                    essCreateVi = lvi.GetSubVI(vi, "CreateESSproxy.vi");
                    if (essCreateVi != null)
                    {
                        essCreateVi.SetControlValue("couponId", expCoupon.couponId);
                        essCreateVi.SetControlValue("issuerGuid", expCoupon.issuerGuid);
                        essCreateVi.SetControlValue("passket", expCoupon.passkey);
                        vi.SetControlValue("experimentID", experimentID);
                    }
                    */

                    // Create DataSourceManager to manage dataSocket connections
                    dsManager = new DataSourceManager();
                 
                    // set up an experiment storage handler
                    ExperimentStorageProxy ess = new ExperimentStorageProxy();
                    ess.OperationAuthHeaderValue = new OperationAuthHeader();
                    ess.OperationAuthHeaderValue.coupon = expCoupon;
                    ess.Url = essService;
                    dsManager.essProxy = ess;
                    dsManager.ExperimentID = experimentID;
                    dsManager.AppKey = qualName;
                     string[] sockets = appInfo.dataSources.Split(';');
                    // Use the experimentID as the storage parameter
                    foreach (string s in sockets)
                    {
                         LVDataSocket reader = new LVDataSocket();
                         dsManager.AddDataSource(reader);
                        if(s.Contains("=")){
                            string [] nv = s.Split('=');
                            reader.Type = nv[1];
                            reader.Connect(nv[0], LabDataSource.READ_AUTOUPDATE);
                           
                        }
                        else{
                            reader.Connect(s, LabDataSource.READ_AUTOUPDATE);
                        }
                        
                    }
                    Global.taskTable.Add(task.taskID, dsManager);
                }
                string taskData = null;
                taskData = LabTask.constructTaskXml(appInfo.appID, fullName, statusViName, essService);
                dbManager.SetTaskData(task.taskID, taskData);
                task.data = taskData;
                if (Global.tasks != null)
                {
                    lock (Global.tasks)
                    {
                        Global.tasks.Add(task);
                    }
                }
            }
            return task;
        }

       
    }
}
