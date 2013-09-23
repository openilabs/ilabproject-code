using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.LabServer.Interactive;
using iLabs.UtilLib;


namespace iLabs.LabServer.BEE
{

    /// <summary>
    /// Summary description for BeeTask
    /// </summary>
    public class BeeTask : LabTask
    {
        public static BeeTask[] GetActiveTasks()
        {
            List<BeeTask> list = null;
            LabDB labDb = new LabDB();
            LabTask[] tasks = labDb.GetActiveTasks();
            if (tasks != null && tasks.Length > 0)
            {

                list = new List<BeeTask>();
                foreach (LabTask t in tasks)
                {
                    BeeTask bt = new BeeTask(t);
                    if (bt.Status == eStatus.Running)
                    {
                        //t.createDataSource(
                    }
                    list.Add(bt);
                }
            }
            return list.ToArray(); ;

        }

        /*      if (((essService != null) && (essService.Length > 0)) )
            {
                // Create DataSourceManager to manage dataSocket connections
                DataSourceManager dsManager = new DataSourceManager();

                // set up an experiment storage handler
                ExperimentStorageProxy ess = new ExperimentStorageProxy();
                ess.OperationAuthHeaderValue = new OperationAuthHeader();
                ess.OperationAuthHeaderValue.coupon = expCoupon;
                ess.Url = essService;
                dsManager.essProxy = ess;
                dsManager.ExperimentID = experimentID;
                dsManager.AppKey = qualName;
                // Note these dataSources are written to by the application and sent to the ESS
                if ((appInfo.dataSources != null) && (appInfo.dataSources.Length > 0))
                {
                    string[] sockets = appInfo.dataSources.Split(',');
                    // Use the experimentID as the storage parameter
                    foreach (string s in sockets)
                    {
                        LVDataSocket reader = new LVDataSocket();
                        dsManager.AddDataSource(reader);
                        if (s.Contains("="))
                        {
                            string[] nv = s.Split('=');
                            reader.Type = nv[1];
                            reader.Connect(nv[0], LabDataSource.READ_AUTOUPDATE);

                        }
                        else
                        {
                            reader.Connect(s, LabDataSource.READ_AUTOUPDATE);
                        }

                    }
                }
                TaskProcessor.Instance.AddDataManager(task.taskID, dsManager);
            }
*/
        public BeeTask()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public BeeTask(LabTask task)
        {
            this.couponID = task.couponID;
            this.data = task.data;
            this.endTime = task.endTime;
            this.experimentID = task.experimentID;
            this.groupName = task.groupName;
            this.issuerGUID = task.issuerGUID;
            this.labAppID = task.labAppID;
            this.startTime = task.startTime;
            this.status = task.Status;
            this.storage = task.storage;
            this.taskID = task.taskID;
        }

        public static BeeTask RestoreTask(LabTask task)
        {
            BeeTask bt = new BeeTask(task);

            return bt;
        }

        public override LabDataSource createDataSource(string source)
        {
            return null;
        }

        public override eStatus HeartBeat()
        {
            try
            {
                switch(status){
                    case eStatus.Running:
                    if (data != null)
                    {
                        XmlQueryDoc taskDoc = new XmlQueryDoc(data);
                        string app = taskDoc.Query("task/application");
                        string statusName = taskDoc.Query("task/status");

                    }
                    break;
                    case eStatus.Scheduled:
                        break;
                    default:
                        break;

                }
            }
            catch (Exception e)
            {
                Logger.WriteLine("HeartBeat: " + e.Message);
            }
            return status;
        }
            
    }
}
