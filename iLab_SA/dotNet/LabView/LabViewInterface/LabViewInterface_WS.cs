/**
 * Copyright (c) 2013 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
**/

/* $Id: LabViewInterface_W.cs 732 2013-08-05 aheifetz $ */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.IO;

using System.Runtime.InteropServices;

using iLabs.Core;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.Proxies.Ticketing;
using iLabs.UtilLib;

using iLabs.LabServer.Interactive;
using iLabs.LabView;

/// <summary>
/// Rewriting of the LabViewInterface class to implement the functions specified in I_LabViewInterface using RESTful Web Services. 
/// The use of this class assumes that the user has properly deployed the ILab_W suite of VIs to localhost on port 8080.
/// This class also assumes that all VIs that the user wishes to access are either on the same server as the ILab_W suite is hosted on
/// or are accessible from that server through a file path.
/// </summary>
/// 
namespace iLabs.LabView.LVWS
{

    public class LabViewInterface : I_LabViewInterface
    {
        protected string url_base = "http://localhost:8080/Ilab_WebService/";
        protected string lvVersion = null;
        //protected _Application viServer;

        protected string appDir = null;
        protected string viPath = @"\user.lib\iLabs";


        public LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket)
        {
            // set defaults
            DateTime startTime = DateTime.UtcNow;
            long duration = -1L;
            long experimentID = 0;
            int status = -1;

            string statusViName = null;
            string statusTemplate = null;
            string templatePath = null;
            LabDB dbManager = new LabDB();
            string qualName = null;
            string fullName = null;  // set defaults
            string viName = null;

            //CHeck that a labVIEW interface revision is set
            //if (appInfo.rev == null || appInfo.rev.Length < 2)
            //{
            //    appInfo.rev = ConfigurationManager.AppSettings["LabViewVersion"];
            //}

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

            if (!IsLoaded(appInfo.application))
            {
                viName = LoadVI(appInfo.path, appInfo.application);
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
                    SetControlValues(viName, names, values);
                }
                OpenFrontPanel(viName, true, LabViewTypes.eFPState.eVisible);
            }
            else
            {
                viName = LoadVI(appInfo.path, appInfo.application);
            }
            if (viName == null)
            {
                status = -1;
                string err = "Unable to Find: " + appInfo.path + @"\" + appInfo.application;
                Logger.WriteLine(err);
                throw new Exception(err);
            }
            // Get qualifiedName
            qualName = qualifiedName(viName);
            fullName = appInfo.path + @"\" + appInfo.application;


            status = GetVIStatus(viName);

            Logger.WriteLine("CreateLabTask - " + qualName + ": VIstatus: " + status);
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
                    //LabViewTypes.eFPState fpState = GetFPStatus(viName);
                   //if (fpState != LabViewTypes.eFPState.eVisible)
                    //{
                        OpenFrontPanel(viName, true, LabViewTypes.eFPState.eVisible);
                    //}
                    ResetVI(viName);
                    break;
                case 2: // eRunTopLevel: this should be the LabVIEW application
                    break;
                case 3: // eRunning
                    //Unless the Experiment is reentrant it should be stopped and be reset.
                    if (!appInfo.reentrant)
                    {
                        int stopStatus = StopVI(viName);
                        if (stopStatus != 0)
                        {
                            AbortVI(viName);
                        }
                        ResetVI(viName);
                    }
                    break;
                default:
                    throw new Exception("Error GetVIStatus: unknown status: " + status);
                    break;
            }
            try
            {
                SetBounds(viName, 0, 0, appInfo.width, appInfo.height);
                Logger.WriteLine("SetBounds: " + appInfo.application);
            }
            catch (Exception sbe)
            {
                Logger.WriteLine("SetBounds exception: " + Utilities.DumpException(sbe));
            }
            SubmitAction("unlockvi", qualifiedName(viName));
            Logger.WriteLine("unlockvi Called: ");

            // Create the labTask & store in database;
            LabViewTask task = new LabViewTask();
            task.labAppID = appInfo.appID;
            task.experimentID = experimentID;
            task.groupName = groupName;
            task.startTime = startTime;
            if (duration > 0)
                task.endTime = startTime.AddTicks(duration * TimeSpan.TicksPerSecond);
            else
                task.endTime = DateTime.MinValue;
            task.Status = LabTask.eStatus.Scheduled;
            task.couponID = expTicket.couponId;
            task.storage = essService;
            task.data = task.constructTaskXml(appInfo.appID, fullName, appInfo.rev, statusViName, essService);
            long taskID = dbManager.InsertTaskLong(task);
            task.taskID = taskID;

            if ((statusTemplate != null) && (statusTemplate.Length > 0))
            {
                statusViName = CreateFromTemplate(templatePath, statusTemplate, task.taskID.ToString());
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
           
            TaskProcessor.Instance.Add(task);
            return task;
        }

        public void DisplayStatus(string viName, string message, string time)
        {
            viName = StripName(viName);
            if (IsLoaded(viName))
            {
                try
                {
                    string response = CallHttpWebRequest(url_base + "ILABW_DisplayStatus.vi?viName=" + viName + "&message=" + message + "&time=" + time);
                }
                catch (Exception e)
                {
                    //do nothing
                }
            }
        }

        public string GetViServerStatus()
        {
            string output = SubmitAction("appstatus", "");
            string[] response = ParseXML(output, "app_info");

            StringBuilder message = new StringBuilder("AppStatus: ");
            try
            {
                message.Append("\tAppName: " + response[0]);
                message.Append("\tDirectory: " + response[1]);
                message.Append("\tVersion: " + response[2]);
                message.Append("\tUserName: " + response[3]);
            }
            catch (Exception ex)
            {
                message.Append("\tError: " + ex.Message + ex.StackTrace);
            }
            return message.ToString();
        }

        public string GetLabViewVersion()
        {
            string output = SubmitAction("appversion", "");
            string response = GetXML(output, "version");
            if (response.Length > 0)
                return response;
            return "Error: Version Not Found";
        }

        public bool IsLabViewOpen()
        {
            string output = SubmitAction("listvis", "");
            string response = GetXML(output, "vi_name");
            return (response.Length > 0);
        }

        public string QuitLabView()
        {
            StringBuilder message = new StringBuilder("QuitLabview: ");
            if (IsLabViewOpen())
            {
                try
                {
                    string temp = SubmitAction("exit", "");
                    message.Append(" Success");
                }
                catch (Exception e)
                {
                    message.Append(" ERROR on Quit(): " + e.Message);
                }
            }
            else
            {
                message.Append(" Already Quit");
            }
            return message.ToString();
        }

        //TODO: implement Method
        public int OpenFrontPanel(string viName, bool reserveForCall, LabViewTypes.eFPState state)
        {
            int status = -1;
            return status;
        }

        public int SetControlValues(string viName, string[] names, object[] values)
        {
            int status = -1;
            return status;
        }

        public string SubmitAction(string action, string data)
        {
            action = StripName(action);
            data = StripName(data);
            string output = CallHttpWebRequest(url_base + "ILabW_CaseHandlerNoPath/?data=" + data + "&action=" + action);
            StringBuilder message = new StringBuilder(action + ": ");
            string response = GetXML(output, "case_response");
            string error = GetXML(output, "error");
            if (data != "")
            {
                message.Append("Data=" + data + " ");
            }
            message.Append(response);
            message.Append("<error>" + error + "</error>");
            return message.ToString();
        }

        public bool IsLoaded(string viName)
        {
            viName = StripName(viName);
            string output = CallHttpWebRequest(url_base + "ILabW_IsLoaded/?name=" + viName);
            string response = GetXML(output, "status");
            return response.Contains("true");
        }

        public int GetVIStatus(string viName)
        {
            viName = StripName(viName);
            if (IsLoaded(viName))
            {
                string response = GetSubmitAction("statusvi", viName, "status");
                try
                {
                    return System.Convert.ToInt32(response);
                }
                catch (Exception e)
                {
                    return -10;
                }
            }
            else
            {
                return -1;
            }
        }

        public string CreateFromTemplate(string templatePath, string templateBase, string suffix)
        {
            templatePath = StripName(templatePath);
            templateBase = StripName(templateBase);
            suffix = StripName(suffix);
            string output = CallHttpWebRequest(url_base + "ILabW_CreateFromTemplate?path=" + templatePath + "&templateBase=" + templateBase + "&suffix=" + suffix);
            string response = GetXML(output, "vi_name");
            if (response.Length > 0)
            {
                return response;
            }
            return "Error: VI Not Created";
        }

        public int AbortVI(string viName)
        {
            string response = GetSubmitAction("abortvi", viName, "status");
            if (response.Length > 0)
            {
                try
                {
                    return System.Convert.ToInt32(response);
                }
                catch (Exception e)
                {
                    return -10;
                }
            }
            return -1;
        }

        public int CloseVI(string viName)
        {
            viName = StripName(viName);
            SubmitAction("closevi", viName);
            string response = GetSubmitAction("statusvi", viName, "status");
            if (response.Length > 0)
            {
                try
                {
                    return System.Convert.ToInt32(response);
                }
                catch (Exception e)
                {
                    return -10;
                }
            }
            return -1;
        }

        public string LoadVI(string viPath, string viName)
        {
            if (IsLoaded(viName))
                return viName;
            string output = GetSubmitAction("loadvi", viName, "vi_name");
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viName"></param>
        /// <returns></returns>
        public int ReleaseVI(string viName)
        {
            int status = -1;
            if (IsLoaded(viName))
            {
                status = 0;
                    // LV 8.2.1
                    //Server takes control of RemotePanel, connection not broken
                    SubmitAction("lockvi", viName);
                    int stopStatus = StopVI(viName);
                
                    if (stopStatus != 0)
                    { //VI found but no stop control
                        AbortVI(viName);
                        status = 2;
                        Logger.WriteLine("Expire: AbortVI() called because no stop control");
                    }
                    else{
                        status = 1;
                    }
                    // Also required for LV 8.2.0 and 7.1, force disconnection of RemotePanel
#if LabVIEW_82
                    SubmitAction("closevi", viName);              
#endif
            }
            return status;
        }

        public int ResetVI(string viName)
        {
            string output = GetSubmitAction("resetvi", viName, "status");
            try
            {
                return System.Convert.ToInt32(output);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public int RunVI(string viName)
        {
            try
            {
                return System.Convert.ToInt32(GetSubmitAction("runvi", viName, "status"));
            }
            catch (Exception e)
            {
                return -10;
            }
        }

        public int RemoveVI(string viName)
        {
            return CloseVI(viName);
        }

        public int StopVI(string viName)
        {
            try
            {
                return Convert.ToInt32(GetSubmitAction("stopvi", viName, "status"));
            }
            catch (Exception e)
            {
                return -10;
            }
        }

        public int SetBounds(string viName, int left, int top, int right, int bottom)
        {
            string output = CallHttpWebRequest(url_base + "ILabW_SetBounds/?viName=" + viName + "&left=" + left.ToString() + "&top=" + top.ToString() + "&right=" + right.ToString() + "&bottom=" + bottom.ToString());
            try
            {
                return System.Convert.ToInt32(GetXML(output, "status"));
            }
            catch
            {
                return -1;
            }
        }

        public int GetLockState(string viName)
        {
            int status = 0;
            try
            {
                status = Convert.ToInt32(GetSubmitAction("getlockstate", viName, "status"));
            }
            catch (Exception e)
            {
                status = -1;
            }
            return status;
        }

        public int SetLockState(string viName, Boolean state)
        {
            //TODO: getStatus
            int status = 1;
            string viPath = GetPath(viName);
            if (state)
            {
                SubmitAction("lockvi", viPath);
            }
            else
            {
                SubmitAction("unlockvi", viPath);
            }
            return status;
        }

        //Convert any ASCII representations of < and > to be literals of "<" and ">"
        private string UnescapeURL(string input)
        {
            input.Replace("&lt;", "<");
            input.Replace("&gt;", ">");
            return input;
        }

        //Convert any ampersands (&) in a name or any other query parameter to their %ASCII versions so as not to break the query string
        private string StripName(string input)
        {
            input.Replace("&", "%26");
            return input;
        }

        //Returns an array containing all strings in input that are between opening and closing tags of tag_name
        private string[] ParseXML(string input, string tag_name)
        {
            input = UnescapeURL(input);
            string start = "<" + tag_name + ">";
            string end = "</" + tag_name + ">";
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            string[] outputs = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                outputs[i] = matches[i].Value;
            }
            return outputs;
        }

        //Version of ParseXML to get all data within multiple tags. Mapped as a dictionary with each tag corresponding to an array of returned data.
        private Dictionary<string, string[]> ParseXMLMultiTag(string input, string[] tag_names)
        {
            Dictionary<string, string[]> outputs = new Dictionary<string, string[]>();
            for (int i = 0; i < tag_names.Length; i++)
            {
                string key = tag_names[i];
                string[] value = ParseXML(input, key);
                outputs.Add(key, value);
            }
            return outputs;
        }

        //Gets the data (as a string) within the first instance of a tag. Useful when it is known that only one thing is returned per tag.
        public string GetXML(string input, string tag_name)
        {
            string[] response = ParseXML(input, tag_name);
            if (response.Length > 0)
                return response[0];
            return "";
        }

        //Takes param of VI name and returns the absolute path to that VI on disk.
        private string GetPath(string viName)
        {
            viName = StripName(viName);
            string path = GetXML(CallHttpWebRequest(url_base + "ILabW_GetPath/?name=" + viName), "vi_path");
            return path;
        }

        //Version of SubmitAction that takes a name instead of a path and also takes a tag name, returning the already-parsed info.
        private string GetSubmitAction(string action, string viName, string tagName)
        {
            string viPath = GetPath(viName);
            string output = GetXML(SubmitAction(action, viPath), tagName);
            return output;
        }

        //Queries a web page and returns the HTML/XML from that page as a string.
        private string CallHttpWebRequest(string URL)
        {
            string sAddress = URL;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sAddress);
            req.Accept = "text/xml,text/plain,text/html";
            req.Method = "GET";
            HttpWebResponse result = (HttpWebResponse)req.GetResponse();
            Stream ReceiveStream = result.GetResponseStream();
            StreamReader reader = new StreamReader(ReceiveStream, System.Text.Encoding.ASCII);
            string respHTML = reader.ReadToEnd();
            return respHTML;
        }

        //TODO: fix qualifiedName for WS
        public string qualifiedName(string viName)
        {
            //string qName = null;
            //if (vi != null)
            //{
            //    StringBuilder buf = new StringBuilder();
            //    string libraryName = null;
            //    try
            //    {
            //        Library lib = vi.Library;
            //        Project proj = null;
            //        if (lib != null)
            //        {
            //            libraryName = lib.Name;
            //            proj = lib.Project;
            //            if (proj != null)
            //            {
            //                if ((proj.Name != null) && (proj.Name.Length > 0))
            //                    buf.Append(proj.Name + "/");
            //                if ((proj.MyComputer != null) && (proj.MyComputer.Name != null) && (proj.MyComputer.Name.Length > 0))
            //                    buf.Append(proj.MyComputer.Name + "/");
            //            }
            //            if ((libraryName != null) && libraryName.Length > 0)
            //            {
            //                buf.Append(libraryName + ":");
            //            }
            //        }
            //    }
            //    catch (InvalidCastException ivcEx)
            //    {
            //        Logger.WriteLine("Library error: " + ivcEx.Message);
            //    }
            //    buf.Append(vi.Name);
            //    qName = buf.ToString();

            //}
            //return qName;
            return viName;
        }
    }
}