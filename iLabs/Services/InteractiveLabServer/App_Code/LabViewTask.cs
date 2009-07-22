/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */

#define LabVIEW_86

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

using iLabs.Core;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.Proxies.Ticketing;
using iLabs.UtilLib;
using iLabs.Ticketing;
using iLabs.LabServer.Interactive;

#if LabVIEW_82
using LabVIEW.lv821;
using iLabs.LabView.LV82;
#endif
#if LabVIEW_86
using LabVIEW.lv86;
using iLabs.LabView.LV86;
#endif

namespace iLabs.LabServer.LabView
{
    /// <summary>
    /// Summary description for LabViewTask
    /// </summary>
    public class LabViewTask : LabTask
    {
        public LabViewTask()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Copy constructor to create LabView Task from task retrieved from database
        /// </summary>
        /// <param name="task"></param>
        public LabViewTask(LabTask task)
        {
            taskID = task.taskID;
            couponID = task.couponID;
            experimentID = task.experimentID;
            labAppID = task.labAppID;
            status = task.Status;
            groupName = task.groupName;
            issuerGUID = task.issuerGUID;
            startTime = task.startTime;
            endTime = task.endTime;
            data = task.data;
            storage = task.storage;
        }

        public override eStatus Expire()
        {
            Utilities.WriteLog("Task expired: " + taskID);
            LabDB dbService = new LabDB();
            LabViewInterface lvi = null;
            try
            {
                if (data != null)
                {
                    XmlQueryDoc taskDoc = new XmlQueryDoc(data);
                    string viName = taskDoc.Query("task/application");
                    string statusVI = taskDoc.Query("task/status");
                    string server = taskDoc.Query("task/server");
                    string portStr = taskDoc.Query("task/serverPort");
                    if ((portStr != null) && (portStr.Length > 0))
                    {
                        lvi = new LabViewRemote(server, Convert.ToInt32(portStr));
                    }
                    else
                    {
                        lvi = new LabViewInterface();
                    }
                    // Status VI not used 
                    if ((statusVI != null) && statusVI.Length != 0)
                    {
                        try
                        {
                            lvi.DisplayStatus(statusVI, "You are out of time!", "0:00");
                        }
                        catch (Exception ce)
                        {
                            Utilities.WriteLog("Trying StatusVI: " + ce.Message);
                        }
                    }


                    //Get the VI and send version specfic call to get control of the VI
                    VirtualInstrument vi = lvi.GetVI(viName);
                    // LV 8.2.1
                    //Server takes control of RemotePanel, connection not broken
                    lvi.SubmitAction("lockvi", lvi.qualifiedName(vi));
                    int stopStatus = lvi.StopVI(vi);
                    if (stopStatus == 0)
                    { //VI found but no stop control
                        vi.Abort();
                        Utilities.WriteLog("Expire: AbortVI() called because no stop control");
                    }

                    // Also required for LV 8.2.0 and 7.1, force disconnection of RemotePanel
                    //lvi.SubmitAction("closevi", lvi.qualifiedName(vi));

                    vi = null;

                }
                Utilities.WriteLog("TaskID = " + taskID + " has expired");
                dbService.SetTaskStatus(taskID, (int)eStatus.Expired);
                status = eStatus.Expired;
                if (Global.taskTable.ContainsKey(taskID))
                {
                    DataSourceManager dsManager = (DataSourceManager)Global.taskTable[taskID];
                    dsManager.CloseDataSources();
                    Global.taskTable.Remove(taskID);
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
                        Utilities.WriteLog("Can not retrieve ServiceBroker!");
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
                        Utilities.WriteLog("Can not retrieve ServiceBroker!");
                        throw new Exception("Can not retrieve ServiceBroker!");
                    }
                    InteractiveSBProxy iuProxy = new InteractiveSBProxy();
                    iuProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                    iuProxy.AgentAuthHeaderValue.coupon = sbs[0].identOut;
                    iuProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                    iuProxy.Url = sbs[0].webServiceUrl;
                    StorageStatus storageStatus = iuProxy.AgentCloseExperiment(expCoupon, experimentID);
                    Utilities.WriteLog("AgentCloseExperiment status: " + storageStatus.status + " records: " + storageStatus.recordCount);


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
                        Utilities.WriteLog("Canceled ticket: " + expCoupon.couponId);
                    }
                    else
                    {
                        Utilities.WriteLog("Unable to cancel ticket: " + expCoupon.couponId);
                    }
                }
            }
            catch (Exception e1)
            {
                Utilities.WriteLog("ProcessTasks Expired: exception:" + e1.Message + e1.StackTrace);
            }
            finally
            {
                lvi = null;
            }

            return status;
        }


        public override eStatus HeartBeat()
        {

            try
            {
                if (status == eStatus.Running)
                {
                    if (data != null)
                    {
                        XmlQueryDoc taskDoc = new XmlQueryDoc(data);
                        string vi = taskDoc.Query("task/application");
                        string statusVI = taskDoc.Query("task/status");

                        if ((statusVI != null) && (statusVI.Length > 0))
                        {
                            I_LabViewInterface lvi = null;
                            try
                            {
                                string server = taskDoc.Query("task/server");
                                string portStr = taskDoc.Query("task/serverPort");

                                if (((server != null) && (server.Length > 0)) && ((portStr != null) && (portStr.Length > 0)))
                                {
                                    lvi = new LabViewRemote(server, Convert.ToInt32(portStr));
                                }
                                else
                                {
                                    lvi = new LabViewInterface();
                                }
                                long ticks = endTime.Ticks - DateTime.UtcNow.Ticks;
                                TimeSpan val = new TimeSpan(ticks);
                                lvi.DisplayStatus(statusVI, "TaskID: " + taskID, val.Minutes + ":" + val.Seconds);
                            }
                            catch (Exception ce2)
                            {
                                Utilities.WriteLog("Status: " + ce2.Message);
                                throw;
                            }
                            finally
                            {
                                lvi = null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.WriteLog("ProcessTasks Status: " + e.Message);
            }
            return status;
        }

    }
#if LabVIEW_82
}
#endif
#if LabVIEW_86
}
#endif
