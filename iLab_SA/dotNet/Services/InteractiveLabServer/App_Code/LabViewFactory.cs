/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */

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
using iLabs.Proxies.ESS;
using iLabs.LabServer.Interactive;
using iLabs.UtilLib;

using iLabs.LabView;
using iLabs.LabView.LV82;
using iLabs.LabView.LV86;
using iLabs.LabView.LV2009;
using iLabs.LabView.LV2010;
using iLabs.LabView.LV2011;
using iLabs.LabView.LV2012;

namespace iLabs.LabServer.LabView
{

    /// <summary>
    /// Summary description for LabTaskFactory
    /// </summary>
    public class LabViewFactory : LabTaskFactory
    {
        public LabViewFactory()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        {
            // set defaults
            DateTime startTime = DateTime.UtcNow;
            long duration = -1L;
            long experimentID = 0;
            LabViewTypes.eExecState status = LabViewTypes.eExecState.eUndefined;

            string statusViName = null;
            string statusTemplate = null;
            string templatePath = null;
            LabDB dbManager = new LabDB();
            //string qualName = null;
            string fullName = null;  // set defaults
           
            //VirtualInstrument vi = null;\
            String viStr = null;
            I_LabViewInterface lvi = null;

            LabViewTask task = new LabViewTask();

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
                task.startTime = DateUtil.ParseUtc(startStr);
            }
            if ((durationStr != null) && (durationStr.Length > 0) && !(durationStr.CompareTo("-1") == 0))
            {
                duration = Convert.ToInt64(durationStr);
            }
            if (duration > -1L)
                task.endTime = task.startTime.AddTicks(duration * TimeSpan.TicksPerSecond);
            else
                task.endTime = DateTime.MinValue;
            if ((expIDstr != null) && (expIDstr.Length > 0))
            {
                task.experimentID = Convert.ToInt64(expIDstr);
            }


            if (appInfo.extraInfo != null && appInfo.extraInfo.Length > 0)
            {
                // Note should have either statusVI or template pair
                // Add Option for VNCserver access
                try
                {
                    XmlQueryDoc viDoc = new XmlQueryDoc(appInfo.extraInfo);
                    statusViName = viDoc.Query("extra/status");
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



            if ((appInfo.server != null) && (appInfo.server.Length > 0) && (appInfo.port > 0))
            {
                lvi = GetLabViewInterface(null, appInfo.server, appInfo.port);
            }
            else
            {
                lvi = GetLabViewInterface();
            }
            if (!lvi.IsLoaded(appInfo.application))
            {
                viStr = lvi.LoadVI(appInfo.path, appInfo.application);
                if (false) // Check for controls first
                {
                    string[] names = new string[4];
                    object[] values = new object[4];
                    names[0] = "CouponId";
                    values[0] = expCoupon.couponId;
                    names[1] = "Passcode";
                    values[1] = expCoupon.passkey;
                    names[2] = "IssuerGuid";
                    values[2] = expCoupon.issuerGuid;
                    names[3] = "ExperimentId";
                    values[3] = experimentID;
                    lvi.SetControlValues(viStr, names, values);
                }
                lvi.OpenFrontPanel(viStr, true, LabViewTypes.eFPState.eVisible);
            }
            else
            {
                viStr = lvi.LoadVI(appInfo.path, appInfo.application);
            }
            if (viStr == null)
            {
                status = LabViewTypes.eExecState.eNotInMemory;
                string err = "Unable to Find: " + appInfo.path + @"\" + appInfo.application;
                Logger.WriteLine(err);
                throw new Exception(err);
            }
            // Get qualifiedName
            //qualName = lvi.qualifiedName(viStr);
            fullName = appInfo.path + @"\" + appInfo.application;


            status = lvi.GetVIStatus(viStr);

            Logger.WriteLine("CreateLabTask - " + viStr + ": VIstatus: " + status);
            switch (status)
            {
                case LabViewTypes.eExecState.eUndefined:
                    throw new Exception("Error GetVIStatus: " + status);
                    break;
                case LabViewTypes.eExecState.eNotInMemory:
                    // VI not in memory
                    throw new Exception("Error GetVIStatus: " + status);

                    break;
                case LabViewTypes.eExecState.eBad: // eBad == 0
                    break;
                case LabViewTypes.eExecState.eIdle: // eIdle == 1 vi in memory but not running 
                    LabViewTypes.eFPState fpState = lvi.GetFPStatus(viStr);
                    if (fpState != LabViewTypes.eFPState.eVisible)
                    {
                        lvi.OpenFrontPanel(viStr, true, LabViewTypes.eFPState.eVisible);
                    }
                    lvi.ResetVI(viStr);
                    break;
                case LabViewTypes.eExecState.eRunningTopLevel: // eRunTopLevel: this should be the LabVIEW application
                    break;
                case LabViewTypes.eExecState.eRunning: // eRunning
                    //Unless the Experiment is reentrant it should be stopped and be reset.
                    if (!appInfo.reentrant)
                    {
                        int stopStatus = lvi.StopVI(viStr);
                        if (stopStatus != 0)
                        {
                            lvi.AbortVI(viStr);
                        }
                        lvi.ResetVI(viStr);
                    }
                    break;
                default:
                    throw new Exception("Error GetVIStatus: unknown status: " + status);
                    break;
            }
            try
            {
                lvi.SetBounds(viStr, 0, 0, appInfo.width, appInfo.height);
                Logger.WriteLine("SetBounds: " + appInfo.application);
            }
            catch (Exception sbe)
            {
                Logger.WriteLine("SetBounds exception: " + Utilities.DumpException(sbe));
            }
            lvi.SubmitAction("unlockvi", viStr);
            Logger.WriteLine("unlockvi Called: ");

            task.storage = essService;
            if ((statusTemplate != null) && (statusTemplate.Length > 0))
            {
                statusViName = lvi.CreateFromTemplate(templatePath, statusTemplate, task.taskID.ToString());
            }
            task.data = task.constructTaskXml(appInfo.appID, fullName, appInfo.rev, statusViName, essService);
            task.Status = LabTask.eStatus.Scheduled;

            // Create the labTask & store in database;
            long taskId = dbManager.InsertTaskLong(task);
            if (taskId > 0)
            {
                task.taskID = taskId;
            }
           
            if (((essService != null) && (essService.Length > 0)))
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
                dsManager.AppKey = viStr;
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
       
            TaskProcessor.Instance.Add(task);
            return task;
        }

        //public override LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        //{

        //    long experimentID = -1;
        //    LabTask task = null;
        //    string revision = null;
        //    //Parse experiment payload 	
        //    string payload = expTicket.payload;
        //    XmlQueryDoc expDoc = new XmlQueryDoc(payload);

        //    string experimentStr = expDoc.Query("ExecuteExperimentPayload/experimentID");
        //    if ((experimentStr != null) && (experimentStr.Length > 0))
        //    {
        //        experimentID = Convert.ToInt64(experimentStr);
        //    }
        //    string sbStr = expDoc.Query("ExecuteExperimentPayload/sbGuid");



        //    // Check to see if an experiment with this ID is already running
        //    LabDB dbManager = new LabDB();
        //    LabTask.eStatus status = dbManager.ExperimentStatus(experimentID, sbStr);
        //    if (status == LabTask.eStatus.NotFound)
        //    {
        //        // Check for an existing experiment using the same resources, if found Close it

        //        //Create the new Task
        //        if (appInfo.rev == null || appInfo.rev.Length < 2)
        //        {
        //            revision = appInfo.rev;
        //        }
        //        else
        //        {
        //            revision = ConfigurationManager.AppSettings["LabViewVersion"];
        //        }
        //        if (revision != null && revision.Length > 2)
        //        {
        //            if (revision.Contains("8.2"))
        //            {
        //                task = iLabs.LabView.LV82.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //            }
        //            else if (revision.Contains("8.6"))
        //            {
        //                task = iLabs.LabView.LV86.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //            }
        //            else if (revision.Contains("2009"))
        //            {
        //                task = iLabs.LabView.LV2009.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //            }

        //            else if (revision.Contains("2010"))
        //            {
        //                task = iLabs.LabView.LV2010.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //            }
        //            else if (revision.Contains("2011"))
        //            {
        //                task = iLabs.LabView.LV2011.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //            }
        //            else if (revision.Contains("2012"))
        //            {
        //                task = iLabs.LabView.LV2012.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //            }
        //        }
        //        else // Default to LV 2009
        //        {
        //            task = iLabs.LabView.LV2009.LabViewTask.CreateLabTask(appInfo, expCoupon, expTicket);
        //        }
                
        //    }
        //    else
        //    {
        //        task =  TaskProcessor.Instance.GetTask(experimentID,expCoupon.issuerGuid);
        //    }
        //    return task;
        //}

        public static I_LabViewInterface GetLabViewInterface()
        {
           
            string revision = ConfigurationManager.AppSettings["LabViewVersion"];
            return GetLabViewInterface(revision);
        }
        public static I_LabViewInterface GetLabViewInterface(string revision)
        {
            I_LabViewInterface lvInterface = null;
            revision = ConfigurationManager.AppSettings["LabViewVersion"];

            if (revision != null && revision.Length > 2)
            {
                if (revision.Contains("WS"))
                {
                    lvInterface = new iLabs.LabView.LabViewInterface_W();
                }
                else if (revision.Contains("8.2"))
                {
                    lvInterface = new iLabs.LabView.LV82.LabViewInterface();
                }
                else if (revision.Contains("8.6"))
                {
                    lvInterface = new iLabs.LabView.LV86.LabViewInterface();
                }
                else if (revision.Contains("2009"))
                {
                    lvInterface = new iLabs.LabView.LV2009.LabViewInterface();
                }

                else if (revision.Contains("2010"))
                {
                    lvInterface = new iLabs.LabView.LV2010.LabViewInterface();
                }
                else if (revision.Contains("2011"))
                {
                    lvInterface = new iLabs.LabView.LV2011.LabViewInterface();
                }
                else if (revision.Contains("2012"))
                {
                    lvInterface = new iLabs.LabView.LV2012.LabViewInterface();
                }
            }
            else // Default to LV 2009
            {
                lvInterface = new iLabs.LabView.LV2009.LabViewInterface();
            }
            return lvInterface;
        }

        public static I_LabViewInterface GetLabViewInterface(string revision, string host, int port)
        {
            I_LabViewInterface lvInterface = null;
            revision = ConfigurationManager.AppSettings["LabViewVersion"];

            if (revision != null && revision.Length > 2)
            {
                if (revision.Contains("WS"))
                {
                    lvInterface = new iLabs.LabView.LabViewInterface_W();
                }
                else if (revision.Contains("8.2"))
                {
                    lvInterface = new iLabs.LabView.LV82.LabViewRemote(host,port);
                }
                else if (revision.Contains("8.6"))
                {
                    lvInterface = new iLabs.LabView.LV86.LabViewRemote(host, port);
                }
                else if (revision.Contains("2009"))
                {
                    lvInterface = new iLabs.LabView.LV2009.LabViewRemote(host, port);
                }

                else if (revision.Contains("2010"))
                {
                    lvInterface = new iLabs.LabView.LV2010.LabViewRemote(host, port);
                }
                else if (revision.Contains("2011"))
                {
                    lvInterface = new iLabs.LabView.LV2011.LabViewRemote(host, port);
                }
                else if (revision.Contains("2012"))
                {
                    lvInterface = new iLabs.LabView.LV2012.LabViewRemote(host, port);
                }
            }
            else // Default to LV 2009
            {
                lvInterface = new iLabs.LabView.LV2009.LabViewRemote(host, port);
            }
            return lvInterface;
        }


    }

}


