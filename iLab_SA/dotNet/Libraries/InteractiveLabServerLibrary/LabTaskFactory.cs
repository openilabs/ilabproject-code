using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.Proxies.ISB;
using iLabs.Proxies.ESS;
using iLabs.UtilLib;


namespace iLabs.LabServer.Interactive
{

    /// <summary>
    /// Summary description for LabTaskFactory
    /// </summary>
    public class LabTaskFactory : I_TaskFactory
    {
        public LabTaskFactory()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        
        /// <summary>
        /// Parses the appInfo and experiment ticket, inserts the task into the database and
        /// creates a dataManager and dataSources defined in the appInfo.
        /// </summary>
        /// <param name="appInfo"></param>
        /// <param name="expCoupon"></param>
        /// <param name="expTicket"></param>
        /// <returns></returns>
        public virtual LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        {
            LabTask labTask = CreateLabTask(appInfo, expTicket);

            if (((labTask.storage != null) && (labTask.storage.Length > 0)))
            {
                // Create DataSourceManager to manage dataSources
                DataSourceManager dsManager = new DataSourceManager();

                // set up an experiment storage handler
                ExperimentStorageProxy ess = new ExperimentStorageProxy();
                ess.OperationAuthHeaderValue = new OperationAuthHeader();
                ess.OperationAuthHeaderValue.coupon = expCoupon;
                ess.Url = labTask.storage;
                dsManager.essProxy = ess;
                dsManager.ExperimentID = labTask.experimentID;
                dsManager.AppKey = appInfo.appKey;
                // Note these dataSources are written to by the application and sent to the ESS
                if ((appInfo.dataSources != null) && (appInfo.dataSources.Length > 0))
                {
                    string[] sources = appInfo.dataSources.Split(',');
                    // Use the experimentID as the storage parameter
                    foreach (string s in sources)
                    {
                       // dsManager.AddDataSource(createDataSource(s));
                    }
                }
                TaskProcessor.Instance.AddDataManager(labTask.taskID, dsManager);
            }
            TaskProcessor.Instance.Add(labTask);
            return labTask;
        }

        /// <summary>
        /// Parses the appInfo and experiment ticket, inserts the task into the database, 
        /// but does not create the DataManager.
        /// </summary>
        /// <param name="appInfo"></param>
        /// <param name="expTicket"></param>
        /// <returns></returns>
        public virtual LabTask CreateLabTask(LabAppInfo appInfo, Ticket expTicket)
        {
            // set defaults
            DateTime startTime = DateTime.UtcNow;
            long experimentID = 0;

            string statusName = null;
            string statusTemplate = null;
            string templatePath = null;

            string qualName = null;
            string fullName = null;  // set defaults
            long duration = -1L;

            LabTask labTask = null;

            LabDB dbManager = new LabDB();

            ////Parse experiment payload, only get what is needed 	
            string payload = expTicket.payload;
            XmlQueryDoc expDoc = new XmlQueryDoc(payload);
            string essService = expDoc.Query("ExecuteExperimentPayload/essWebAddress");
            string startStr = expDoc.Query("ExecuteExperimentPayload/startExecution");
            string durationStr = expDoc.Query("ExecuteExperimentPayload/duration");
            string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
            string userName = expDoc.Query("ExecuteExperimentPayload/userName");
            string expIDstr = expDoc.Query("ExecuteExperimentPayload/experimentID");

            if ((startStr != null) && (startStr.Length > 0))
            {
                startTime = DateUtil.ParseUtc(startStr);
            }
            if ((durationStr != null) && (durationStr.Length > 0) && !(durationStr.CompareTo("-1") == 0))
            {
                duration = Convert.ToInt64(durationStr);
            }
            if ((expIDstr != null) && (expIDstr.Length > 0))
            {
                experimentID = Convert.ToInt64(expIDstr);
            }

            if (appInfo.extraInfo != null && appInfo.extraInfo.Length > 0)
            {
                // Note should have either statusVI or template pair
                // Add Option for VNCserver access
                try
                {
                    XmlQueryDoc viDoc = new XmlQueryDoc(appInfo.extraInfo);
                    statusName = viDoc.Query("extra/status");
                    statusTemplate = viDoc.Query("extra/statusTemplate");
                    templatePath = viDoc.Query("extra/templatePath");
                }
                catch (Exception e)
                {
                    string err = e.Message;
                }
            }

            // log the experiment for debugging

            Logger.WriteLine("Experiment: " + experimentID + " Start: " + DateUtil.ToUtcString(startTime) + " \tduration: " + duration);
            long statusSpan = DateUtil.SecondsRemaining(startTime, duration);
            //launchClient(experimentID, essService, appInfo);
           

            // Create  & store the labTask in database and return an LabTask object;
            labTask = dbManager.InsertTask(appInfo.appID, experimentID,
                        groupName, startTime, duration, LabTask.eStatus.Scheduled,
                        expTicket.couponId, expTicket.issuerGuid, essService,null);
            labTask.data = labTask.constructTaskXml(appInfo.appID, fullName, appInfo.rev, statusName, essService);
            dbManager.SetTaskData(labTask.taskID, labTask.data);
            return labTask;
        }
     

    }
}