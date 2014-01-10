using System;
using System.Text;

using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Core;
using iLabs.Ticketing;
using iLabs.UtilLib;

using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.Proxies.Ticketing;

namespace iLabs.LabServer.Interactive
{
	/// <summary>
	/// Summary description for LabTask.
	/// </summary>
	public class LabTask
	{
		public long taskID; // Assigned by database, Primary Key
        public long couponID;
        public long experimentID = -1;
		public int labAppID; // Reference to LabApplication
        protected eStatus status;
		public string groupName; // SB group
        public string issuerGUID;
		public DateTime startTime; // in UTC
		public DateTime endTime; //  == -1 never automaticly ends
		public string data; // any XML encoded data
        public string storage; // information about ESS and data sources ( XML )

        public enum eStatus { NotFound = -1, Unknown = 0, Scheduled=2, Pending =4, Waiting=8, Running = 16, Completed=32, Aborted=64, Expired=128, Closed=256 };

        public virtual string constructSessionPayload(LabAppInfo appInfo, DateTime start, long duration,
            long taskID, string returnTarget, string user, string statusName)
        {
            return constructSessionPayload(appInfo.appID, appInfo.title,appInfo.application,
                appInfo.rev,appInfo.appURL,appInfo.width,appInfo.height,
                start, duration, taskID, returnTarget, user, statusName);
        }

       
        public virtual string constructSessionPayload(int id, string title, string application, string revision, string appUrl,
            int width, int height, DateTime start, long duration, long taskID, string returnTarget, string user, string statusName)
        {
            StringBuilder buf = new StringBuilder("<payload>");
            buf.Append("<appId>" + id + "</appId>");
            buf.Append("<title>" + title + "</title>");
            buf.Append("<application>" + application + "</application>");
            buf.Append("<revision>" + revision + "</revision>");
            buf.Append("<taskId>" + taskID + "</taskId>");
            buf.Append("<width>" + width + "</width>");
            buf.Append("<height>" + height + "</height>");
            buf.Append("<startTime>" + DateUtil.ToUtcString(start) + "</startTime>");
            buf.Append("<duration>" + duration + "</duration>");
            if (appUrl != null)
                buf.Append("<appUrl>" + appUrl + "</appUrl>");
            if (user != null)
                buf.Append("<user>" + user + "</user>");
            if (statusName != null)
                buf.Append("<status>" + statusName + "</status>");
            buf.Append("</payload>");
            return buf.ToString();
        }

        public virtual string constructTaskXml(long appID, string application, string revision, string statusName, string essURL)
        {
            StringBuilder buf = new StringBuilder("<task>");
            buf.Append("<appId>" + appID + "</appId>");
            if (application != null)
                buf.Append("<application>" + application + "</application>");
            if (revision != null)
            buf.Append("<revision>" + revision + "</revision>");
            if (statusName != null)
                buf.Append("<status>" + statusName + "</status>");
            if ((essURL != null) && (essURL.Length > 0))
                buf.Append("<essUrl>" + essURL + "</essUrl>");
            buf.Append("</task>");
            return buf.ToString();
        }
		public LabTask()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        ///// <summary>
        ///// Parses the appInfo and experiment ticket, inserts the task into the database and
        ///// creates a dataManager and dataSources defined in the appInfo.
        ///// </summary>
        ///// <param name="appInfo"></param>
        ///// <param name="expCoupon"></param>
        ///// <param name="expTicket"></param>
        ///// <returns></returns>
        //public LabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        //{
        //    // set defaults
        //    startTime = DateTime.UtcNow;

        //    string statusName = null;
        //    string statusTemplate = null;
        //    string templatePath = null;

        //    string qualName = null;
        //    string fullName = null;  // set defaults
        //    long duration = -1L;

        //    LabTask labTask = null;

        //    LabDB dbManager = new LabDB();

        //    ////Parse experiment payload, only get what is needed 	
        //    string payload = expTicket.payload;
        //    XmlQueryDoc expDoc = new XmlQueryDoc(payload);
        //    string essService = expDoc.Query("ExecuteExperimentPayload/essWebAddress");
        //    string startStr = expDoc.Query("ExecuteExperimentPayload/startExecution");
        //    string durationStr = expDoc.Query("ExecuteExperimentPayload/duration");
        //    string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
        //    string userName = expDoc.Query("ExecuteExperimentPayload/userName");
        //    string expIDstr = expDoc.Query("ExecuteExperimentPayload/experimentID");

        //    if ((startStr != null) && (startStr.Length > 0))
        //    {
        //        startTime = DateUtil.ParseUtc(startStr);
        //    }
        //    if ((durationStr != null) && (durationStr.Length > 0) && !(durationStr.CompareTo("-1") == 0))
        //    {
        //        duration = Convert.ToInt64(durationStr);
        //    }
        //    if ((expIDstr != null) && (expIDstr.Length > 0))
        //    {
        //        experimentID = Convert.ToInt64(expIDstr);
        //    }

        //    if (appInfo.extraInfo != null && appInfo.extraInfo.Length > 0)
        //    {
        //        // Note should have either statusVI or template pair
        //        // Add Option for VNCserver access
        //        try
        //        {
        //            XmlQueryDoc viDoc = new XmlQueryDoc(appInfo.extraInfo);
        //            statusName = viDoc.Query("extra/status");
        //            statusTemplate = viDoc.Query("extra/statusTemplate");
        //            templatePath = viDoc.Query("extra/templatePath");
        //        }
        //        catch (Exception e)
        //        {
        //            string err = e.Message;
        //        }
        //    }

        //    // log the experiment for debugging

        //    Logger.WriteLine("Experiment: " + experimentID + " Start: " + DateUtil.ToUtcString(startTime) + " \tduration: " + duration);
        //    long statusSpan = DateUtil.SecondsRemaining(startTime, duration);
        //    //launchClient(experimentID, essService, appInfo);
           
           
        //    //TODO: convert to InsertTaskLong()
        //    // Create  & store the labTask in database and return an LabTask object;
        //    labTask = dbManager.InsertTask(appInfo.appID, experimentID,
        //                groupName, startTime, duration, LabTask.eStatus.Scheduled,
        //                expTicket.couponId, expTicket.issuerGuid, essService, null);

        //    labTask.data = labTask.constructTaskXml(appInfo.appID, fullName, appInfo.rev, statusName, essService);
        //    dbManager.SetTaskData(labTask.taskID, labTask.data);
        //    if (((labTask.storage != null) && (labTask.storage.Length > 0)))
        //    {
        //        // Create DataSourceManager to manage dataSources
        //        DataSourceManager dsManager = new DataSourceManager();

        //        // set up an experiment storage handler
        //        ExperimentStorageProxy ess = new ExperimentStorageProxy();
        //        ess.OperationAuthHeaderValue = new OperationAuthHeader();
        //        ess.OperationAuthHeaderValue.coupon = expCoupon;
        //        ess.Url = labTask.storage;
        //        dsManager.essProxy = ess;
        //        dsManager.ExperimentID = labTask.experimentID;
        //        dsManager.AppKey = appInfo.appKey;
        //        // Note these dataSources are written to by the application and sent to the ESS
        //        if ((appInfo.dataSources != null) && (appInfo.dataSources.Length > 0))
        //        {
        //            string[] sources = appInfo.dataSources.Split(',');
        //            // Use the experimentID as the storage parameter
        //            foreach (string s in sources)
        //            {
        //                // dsManager.AddDataSource(createDataSource(s));
        //            }
        //        }
        //        TaskProcessor.Instance.AddDataManager(labTask.taskID, dsManager);
        //    }
        //    TaskProcessor.Instance.Add(labTask);
        //    return labTask;
        //}

        ///// <summary>
        ///// Parses the appInfo and experiment ticket, inserts the task into the database, 
        ///// but does not create the DataManager.
        ///// </summary>
        ///// <param name="appInfo"></param>
        ///// <param name="expTicket"></param>
        ///// <returns></returns>
        //public LabTask(LabAppInfo appInfo, Ticket expTicket)
        //{
        //    // set defaults
        //    DateTime startTime = DateTime.UtcNow;
        //    long experimentID = 0;

        //    string statusName = null;
        //    string statusTemplate = null;
        //    string templatePath = null;

        //    string qualName = null;
        //    string fullName = null;  // set defaults
        //    long duration = -1L;

        //    LabTask labTask = null;

        //    LabDB dbManager = new LabDB();

        //    ////Parse experiment payload, only get what is needed 	
        //    string payload = expTicket.payload;
        //    XmlQueryDoc expDoc = new XmlQueryDoc(payload);
        //    string essService = expDoc.Query("ExecuteExperimentPayload/essWebAddress");
        //    string startStr = expDoc.Query("ExecuteExperimentPayload/startExecution");
        //    string durationStr = expDoc.Query("ExecuteExperimentPayload/duration");
        //    string groupName = expDoc.Query("ExecuteExperimentPayload/groupName");
        //    string userName = expDoc.Query("ExecuteExperimentPayload/userName");
        //    string expIDstr = expDoc.Query("ExecuteExperimentPayload/experimentID");

        //    if ((startStr != null) && (startStr.Length > 0))
        //    {
        //        startTime = DateUtil.ParseUtc(startStr);
        //    }
        //    if ((durationStr != null) && (durationStr.Length > 0) && !(durationStr.CompareTo("-1") == 0))
        //    {
        //        duration = Convert.ToInt64(durationStr);
        //    }
        //    if ((expIDstr != null) && (expIDstr.Length > 0))
        //    {
        //        experimentID = Convert.ToInt64(expIDstr);
        //    }

        //    if (appInfo.extraInfo != null && appInfo.extraInfo.Length > 0)
        //    {
        //        // Note should have either statusVI or template pair
        //        // Add Option for VNCserver access
        //        try
        //        {
        //            XmlQueryDoc viDoc = new XmlQueryDoc(appInfo.extraInfo);
        //            statusName = viDoc.Query("extra/status");
        //            statusTemplate = viDoc.Query("extra/statusTemplate");
        //            templatePath = viDoc.Query("extra/templatePath");
        //        }
        //        catch (Exception e)
        //        {
        //            string err = e.Message;
        //        }
        //    }

        //    // log the experiment for debugging

        //    Logger.WriteLine("Experiment: " + experimentID + " Start: " + DateUtil.ToUtcString(startTime) + " \tduration: " + duration);
        //    long statusSpan = DateUtil.SecondsRemaining(startTime, duration);
        //    //launchClient(experimentID, essService, appInfo);
        //    string taskData = null;
        //    taskData = LabTask.constructTaskXml(appInfo.appID, fullName, appInfo.rev, statusName, essService);

        //    // Create  & store the labTask in database and return an LabTask object;
        //    labTask = dbManager.InsertTask(appInfo.appID, experimentID,
        //                groupName, startTime, duration, LabTask.eStatus.Scheduled,
        //                expTicket.couponId, expTicket.issuerGuid, essService, taskData);
        //    return labTask;
        //}

        public virtual void launchClient(long experimentID, string essService, LabAppInfo appInfo)
        {
            // Dummy method
        }

        public virtual LabDataSource createDataSource(string source)
        {
            return null;
        }



        public virtual eStatus Status
        {
            get { return status; }
            set { status = value; }
        }


        public virtual eStatus Expire()
        {
            Logger.WriteLine("Task expired: " + taskID);

            Close(eStatus.Expired);
            status |= eStatus.Expired;
            return status;
        }

        public virtual eStatus Close()
        {
            return Close(eStatus.Closed);
        }

        public virtual eStatus Close(eStatus reason)
        {
            LabDB dbService = new LabDB();
            
            try
            {
                if (data != null)
                {
                    XmlQueryDoc taskDoc = new XmlQueryDoc(data);
                    string app = taskDoc.Query("task/application");
                    string statusName = taskDoc.Query("task/status");
                    string server = taskDoc.Query("task/server");
                    string portStr = taskDoc.Query("task/serverPort");
                    
                    // Status not used 
                    if ((statusName != null) && statusName.Length != 0)
                    {
                        try
                        {
                            if (reason == eStatus.Expired)
                                DisplayStatus(statusName, "You are out of time!", "0:00");
                            else
                                DisplayStatus(statusName, "Your experiment has been cancelled", "0:00");
                        }
                        catch (Exception ce)
                        {
                            Logger.WriteLine("Trying StatusName: " + ce.Message);
                        }
                    }


                   //Stop the application & close ESS sessions

                }
                Logger.WriteLine("TaskID = " + taskID + " is being closed");
                dbService.SetTaskStatus(taskID, (int)reason);
                status = eStatus.Closed;

                DataSourceManager dsManager = TaskProcessor.Instance.GetDataManager(taskID);
                if (dsManager != null)
                {
                    dsManager.CloseDataSources();
                    TaskProcessor.Instance.RemoveDataManager(taskID);
                }

                dbService.SetTaskStatus(taskID, (int)status);
                if (couponID > 0)
                { // this task was created with a valid ticket, i.e. not a test.
                    Coupon expCoupon = dbService.GetCoupon(couponID, issuerGUID);

                    // Only use the domain ServiceBroker, do we need a test
                    // Should only be one
                    ProcessAgentInfo[] sbs = dbService.GetProcessAgentInfos(ProcessAgentType.SERVICE_BROKER);

                    if ((sbs == null) || (sbs.Length < 1))
                    {
                        Logger.WriteLine("Can not retrieve ServiceBroker!");
                        throw new Exception("Can not retrieve ServiceBroker!");
                    }
                    ProcessAgentInfo domainSB = null;
                    foreach (ProcessAgentInfo dsb in sbs)
                    {
                        if (!dsb.retired)
                        {
                            domainSB = dsb;
                            break;
                        }
                    }
                    if (domainSB == null)
                    {
                        Logger.WriteLine("Can not retrieve ServiceBroker!");
                        throw new Exception("Can not retrieve ServiceBroker!");
                    }
                    InteractiveSBProxy iuProxy = new InteractiveSBProxy();
                    iuProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                    iuProxy.AgentAuthHeaderValue.coupon = sbs[0].identOut;
                    iuProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                    iuProxy.Url = sbs[0].webServiceUrl;
                    StorageStatus storageStatus = iuProxy.AgentCloseExperiment(expCoupon, experimentID);
                    Logger.WriteLine("AgentCloseExperiment status: " + storageStatus.status + " records: " + storageStatus.recordCount);


                    // currently RequestTicketCancellation always returns false
                    // Create ticketing service interface connection to TicketService
                    TicketIssuerProxy ticketingInterface = new TicketIssuerProxy();
                    ticketingInterface.AgentAuthHeaderValue = new AgentAuthHeader();
                    ticketingInterface.Url = sbs[0].webServiceUrl;
                    ticketingInterface.AgentAuthHeaderValue.coupon = sbs[0].identOut;
                    ticketingInterface.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                    if (ticketingInterface.RequestTicketCancellation(expCoupon, TicketTypes.EXECUTE_EXPERIMENT, ProcessAgentDB.ServiceGuid))
                    {
                        dbService.CancelTicket(expCoupon, TicketTypes.EXECUTE_EXPERIMENT, ProcessAgentDB.ServiceGuid);
                        Logger.WriteLine("Canceled ticket: " + expCoupon.couponId);
                    }
                    else
                    {
                        Logger.WriteLine("Unable to cancel ticket: " + expCoupon.couponId);
                    }
                }
            }
            catch (Exception e1)
            {
                Logger.WriteLine("ProcessTasks Cancelled: exception:" + e1.Message + e1.StackTrace);
            }
  
            return status;
        }

        public virtual void DisplayStatus(string statusName, string message, string time)
        {
        }

         /// <summary>
         /// This is called by the TaskProcessor during every interation of the process loop, 
         /// for task with one of these LabTask.eStatus: Pending, Scheduled, Running, Waiting.
         /// </summary>
         /// <returns>the task status after the heartbeat.</returns>
        public virtual eStatus HeartBeat()
        {
            try
            {
                if (status == eStatus.Running)
                {
                    if (data != null)
                    {
                        XmlQueryDoc taskDoc = new XmlQueryDoc(data);
                        string app = taskDoc.Query("task/application");
                        string statusName = taskDoc.Query("task/status");

                        if ((statusName != null) && (statusName.Length > 0))
                        {
                                long ticks = endTime.Ticks - DateTime.UtcNow.Ticks;
                                TimeSpan val = new TimeSpan(ticks);
                                DisplayStatus(statusName, "TaskID: " + taskID, val.Minutes + ":" + val.Seconds);
                        }
                    }
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
