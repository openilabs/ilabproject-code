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

using System.Runtime.InteropServices;

using iLabs.UtilLib;

/// <summary>
/// Rewriting of the LabViewInterface class to implement the functions specified in I_LabViewInterface using RESTful Web Services. 
/// The use of this class assumes that the user has properly deployed the ILab_W suite of VIs to localhost on port 8080.
/// This class also assumes that all VIs that the user wishes to access are either on the same server as the ILab_W suite is hosted on
/// or are accessible from that server through a file path.
/// </summary>
/// 
namespace iLabs.LabView
{

    public class LabViewInterface_W : I_LabViewInterface
    {
        protected string url_base = "http://localhost:8080/Ilab_WebService/";
        protected string lvVersion = null;
        //protected _Application viServer;

        protected string appDir = null;
        protected string viPath = @"\user.lib\iLabs";

        public void DisplayStatus(string viName, string message, string time)
        {
            viName = StripName(viName);
			if (IsLoaded(viName))
            {
                try
                {
                    string response = CallHTTPWebRequest(url_base + "ILABW_DisplayStatus.vi?viName=" + viName + "&message=" + message + "&time=" + time);
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
            string[] response = GetXML(output, "vi_name");
            return (response.Length > 0);
        }

        public string QuitLabView()
        {
            StringBuilder message = new StringBuilder("QuitLabview: ");
            if (IsLabViewOpen())
            {
                try
                {
                    temp = SubmitAction("exit", "");
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
                    return System.Convert.ToDecimal(response);
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
           int  status = 1;
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
        private Dictionary<string,string[]> ParseXMLMultiTag(string input, string[] tag_names)
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
            string[] response = parseXML(input, tag_name);
            if (response.Length > 0)
                return response[0];
            return "";
        }

        //Takes param of VI name and returns the absolute path to that VI on disk.
        private string GetPath(string viName)
        {
            viName = StripName(viName);
            string path = queryXML(CallHttpWebRequest(url_base + "ILabW_GetPath/?name=" + viName), "vi_path");
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

    }
}