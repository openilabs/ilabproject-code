using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.Core;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;

using iLabs.Proxies.ISB;
using iLabs.Proxies.ESS;
using iLabs.UtilLib;
using iLabs.LabServer.Interactive;

namespace iLabs.LabServer.BEE
{
    /// <summary>
    /// Summary description for BeeAPI
    /// </summary>
    public class BeeAPI : LabTaskFactory
    {
        public BeeAPI()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public BeeTask InsertTask(int appId, long expId, string groupName, DateTime startTime, long duration, LabTask.eStatus status,
            long coupon_ID, string issuerGuid, string essService, string taskData)
        {
            BeeTask beeTask = null;
            LabDB dbManager = new LabDB();
            LabTask labTask = dbManager.InsertTask(appId, expId,
                        groupName, startTime, duration, status,
                        coupon_ID, issuerGuid, essService, taskData);
            beeTask = new BeeTask(labTask);
            return beeTask;
        }


      
                // Create DataSourceManager to manage dataSources
            public DataSourceManager CreateDataSourceManager(LabTask task, Coupon expCoupon){
                DataSourceManager dsManager = new DataSourceManager();

                // set up an experiment storage handler
                ExperimentStorageProxy ess = new ExperimentStorageProxy();
                ess.OperationAuthHeaderValue = new OperationAuthHeader();
                ess.OperationAuthHeaderValue.coupon = expCoupon;
                ess.Url = task.storage;
                dsManager.essProxy = ess;
                dsManager.ExperimentID = task.experimentID;
                //dsManager.AppKey = task.labAppID.;
                return dsManager;
            }

        public FileWatcherDataSource CreateBeeDataSource(Coupon expCoupon, LabTask task, string recordType, bool flushFile)
        {
            string outputDir = ConfigurationManager.AppSettings["chamberOutputDir"];
            string outputFile = ConfigurationManager.AppSettings["chamberOutputFile"];
            string filePath = outputDir + @"\" + outputFile;
            // Stop the controller and flush the data file
            if (flushFile)
            {
                //Flush the File
                FileInfo fInfo = new FileInfo(filePath);
                using (FileStream inFile = fInfo.Open(FileMode.Truncate)) { }
            }
            string pushChannel = ChecksumUtil.ToMD5Hash("BEElab" + task.experimentID);
            //Add BEElab specific attributes
            BeeEventHandler bEvt = new BeeEventHandler(expCoupon, task.experimentID, task.storage,
                recordType, ProcessAgentDB.ServiceGuid);
            bEvt.PusherChannel = pushChannel;
            //DataSourceManager dsManager = TaskProcessor.Instance.GetDataManager(task.taskID);
            FileWatcherDataSource fds = new FileWatcherDataSource();
            fds.Path = outputDir;
            fds.Filter = outputFile;
            fds.AddFileSystemEventHandler(bEvt.OnChanged);
            //dsManager.AddDataSource(fds);
            //fds.Start();
            return fds;
        }

    }
}
