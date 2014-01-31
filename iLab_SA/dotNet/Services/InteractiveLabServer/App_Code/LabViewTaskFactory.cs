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
using iLabs.LabView.LVWS;

namespace iLabs.LabServer.LabView
{

    /// <summary>
    /// Summary description for LabTaskFactory
    /// </summary>
    public class LabViewTaskFactory : LabTaskFactory
    {
        private string lvBinding;
        public LabViewTaskFactory()
        {
            //
            // TODO: Add constructor logic here

            //
            try
            {
                lvBinding = ConfigurationManager.AppSettings["LabViewRevision"];
            }
            catch (ConfigurationErrorsException ce)
            {
                lvBinding = null;
            }
        }

        public override LabTask CreateLabTask()
        {
            LabTask task = null;
            // 8.2, 8.6, 2009, 2010, 2011, 2012, WebService
            switch (lvBinding)
            {

                case null:
                    task = new LabTask();
                    break;
                case "WebService":
                    task = new iLabs.LabView.LVWS.LabViewTask();
                    break;
                case "2012":
                    task = new iLabs.LabView.LV2012.LabViewTask();
                    break;
                case "2011":
                    task = new iLabs.LabView.LV2011.LabViewTask();
                    break;
                case "2010":
                    task = new iLabs.LabView.LV2010.LabViewTask();
                    break;
                case "2009":
                    task = new iLabs.LabView.LV2009.LabViewTask();
                    break;
                case "8.6":
                    task = new iLabs.LabView.LV86.LabViewTask();
                    break;
                case "8.2":
                    task = new iLabs.LabView.LV82.LabViewTask();
                    break;
                default:
                    task = new iLabs.LabView.LVWS.LabViewTask();
                    break;
            }
            return task;
        }

        public LabTask CreateLabTask(LabTask task)
        {
            LabTask newTask = CreateLabTask();
            newTask.taskID = task.taskID;
            newTask.couponID = task.couponID;
            newTask.experimentID = task.experimentID;
            newTask.labAppID = task.labAppID;
            newTask.Status = task.Status;
            newTask.groupName = task.groupName;
            newTask.issuerGUID = task.issuerGUID;
            newTask.startTime = task.startTime;
            newTask.endTime = task.endTime;
            newTask.data = task.data;
            newTask.storage = task.storage;
            return newTask;
        }

        public override LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        {
            long experimentID = -1;
            LabTask task = null;
            string revision = null;
            //Parse experiment payload 	
            string payload = expTicket.payload;
            XmlQueryDoc expDoc = new XmlQueryDoc(payload);

            string experimentStr = expDoc.Query("ExecuteExperimentPayload/experimentID");
            if ((experimentStr != null) && (experimentStr.Length > 0))
            {
                experimentID = Convert.ToInt64(experimentStr);
            }
            string sbStr = expDoc.Query("ExecuteExperimentPayload/sbGuid");

            // Check to see if an experiment with this ID is already running
            LabDB dbManager = new LabDB();
            LabTask.eStatus status = dbManager.ExperimentStatus(experimentID, sbStr);
            if (status == LabTask.eStatus.NotFound)
            {
                // Check for an existing experiment using the same resources, if found Close it

                //Create the new Task
                if (appInfo.rev == null || appInfo.rev.Length < 2)
                {
                    revision = appInfo.rev;
                }
                else
                {
                    revision = ConfigurationManager.AppSettings["LabViewVersion"];
                }
                if (revision != null && revision.Length > 2)
                {
                    if (revision.Contains("8.2"))
                    {
                        task = new iLabs.LabView.LV82.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }
                    else if (revision.Contains("8.6"))
                    {
                        task = new iLabs.LabView.LV86.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }
                    else if (revision.Contains("2009"))
                    {
                        task = new iLabs.LabView.LV2009.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }

                    else if (revision.Contains("2010"))
                    {
                        task = new iLabs.LabView.LV2010.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }
                    else if (revision.Contains("2011"))
                    {
                        task = new iLabs.LabView.LV2011.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }
                    else if (revision.Contains("2012"))
                    {
                        task = new iLabs.LabView.LV2012.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }
                    else if (revision.Contains("WS"))
                    {
                        task = new iLabs.LabView.LVWS.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                    }
                }
                else // Default to LV 2009
                {
                    task = new iLabs.LabView.LV2009.LabViewInterface().CreateLabTask(appInfo, expCoupon, expTicket);
                }
                
            }
            else
            {
                task =  TaskProcessor.Instance.GetTask(experimentID,expCoupon.issuerGuid);
            }
            return task;
        }

        public static I_LabViewInterface GetLabViewInterface()
        {
            I_LabViewInterface lvInterface = null;
            string revision = null;
            revision = ConfigurationManager.AppSettings["LabViewVersion"];

            if (revision != null && revision.Length > 2)
            {
                if (revision.Contains("8.2"))
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
                else if (revision.Contains("WS"))
                {
                    lvInterface = new iLabs.LabView.LVWS.LabViewInterface();
                }
            }
            else // Default to LV 2009
            {
                lvInterface = new iLabs.LabView.LV2009.LabViewInterface();
            }
            return lvInterface;
        }


    }

}


