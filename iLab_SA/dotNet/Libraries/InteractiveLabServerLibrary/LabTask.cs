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

        public static string constructSessionPayload(LabAppInfo appInfo, DateTime start, long duration,
            long taskID, string returnTarget, string user, string statusName)
        {
            return constructSessionPayload(appInfo.appID, appInfo.title,appInfo.application,
                appInfo.rev,appInfo.appURL,appInfo.width,appInfo.height,
                start, duration, taskID, returnTarget, user, statusName);
        }

        //This should be in the factory
        public static string constructSessionPayload(int id, string title, string application, string revision, string appUrl,
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

        public static string constructTaskXml(long appID, string application, string revision, string statusName, string essURL)
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
                Logger.WriteLine("TaskID = " + taskID + " has expired");
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
                Logger.WriteLine("ProcessTasks Status: " + e.Message);
            }
            return status;
        }



	}
}
