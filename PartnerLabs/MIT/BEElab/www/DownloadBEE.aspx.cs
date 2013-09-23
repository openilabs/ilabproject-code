using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.LabServer.Interactive;

using iLabs.Proxies.ISB;
using iLabs.Proxies.ESS;

namespace iLabs.LabServer.BEE
{
    public partial class DownloadBEE : System.Web.UI.Page
    {

        long experimentID;
        int minRecord = -1;
        int maxRecord = -1;
        string indices = null;
        Coupon opCoupon = null;
        string fileName = "beeLabData.csv";


        protected void Page_Load(object sender, EventArgs e)
        {
            LabDB dbManager = new LabDB();
            if (Session["opCouponID"] != null && Session["opIssuer"] != null && Session["opPasscode"] != null)
            {
                opCoupon = new Coupon(Session["opIssuer"].ToString(),
                    Convert.ToInt64(Session["opCouponID"].ToString()),
                    Session["opPasscode"].ToString());
            }
            else
            {
                throw new AccessDeniedException("Missing credentials for BEE Lab graph.");
            }
            if (Request.QueryString["expid"] != null)
                experimentID = Convert.ToInt64(Request.QueryString["expid"]);
            if (Request.QueryString["min"] != null && Request.QueryString["min"].Length > 0)
                minRecord = Convert.ToInt32(Request.QueryString["min"]);
            if (Request.QueryString["max"] != null && Request.QueryString["max"].Length > 0)
                maxRecord = Convert.ToInt32(Request.QueryString["max"]) +1;
            if (Request.QueryString["index"] != null && Request.QueryString["index"].Length > 0)
                indices = Request.QueryString["index"];

            InteractiveSBProxy sbProxy = new InteractiveSBProxy();
            //ProcessAgentInfo sbInfo = dbManager.GetProcessAgentInfo(Session["opIssuer"].ToString());
            ProcessAgentInfo sbInfo = dbManager.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.domainGuid);
            if (sbInfo != null)
            {
                sbProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                sbProxy.OperationAuthHeaderValue.coupon = opCoupon;
                sbProxy.Url = sbInfo.webServiceUrl;
                List<Criterion> criteria = new List<Criterion>();
                //criteria.Add(new Criterion("experiment_ID", "=", experimentID));
                criteria.Add(new Criterion("Record_Type", "=", "data"));
                if (minRecord > -1)
                    criteria.Add(new Criterion("sequence_no", ">=", minRecord.ToString()));
                if (maxRecord > -1)
                    criteria.Add(new Criterion("sequence_no", "<=", maxRecord.ToString()));
                //ExperimentSummary[] expInfo = sbProxy.RetrieveExperimentSummary(search);
                //if (expInfo.Length > 0)
                //{
                //    theExperiment = expInfo[0];
                //}
                //ExperimentRecord[] profileRecords = getRecords(sbProxy, Convert.ToInt64(hdnExperimentID.Value),
                //  new Criterion[] { new Criterion("Record_Type", "=", "profile") });
                ExperimentRecord[] records = getRecords(sbProxy, experimentID, criteria.ToArray());
                if (records != null)
                {
                    if (records.Length > 0)
                    {

                        //processProfile(profileRecords);
                        StringBuilder buf = processRecords(records, indices);
                        int bufLen = buf.Length * 2;
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                        //Response.AddHeader("Content-Length",bufLen.ToString());
                        Response.ContentType = "application/octet-stream";
                        Response.Write(buf.ToString());
                        Response.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Should use the ServiceBroker to create a ticket to directly connect to the ESS for experiment records
        /// </summary>
        /// <param name="sbProxy"></param>
        /// <param name="expID"></param>
        /// <param name="cList"></param>
        protected ExperimentRecord[] getRecords(InteractiveSBProxy sbProxy, long expID, Criterion[] cList)
        {
            ExperimentRecord[] records = null;
            records = sbProxy.RetrieveExperimentRecords(expID, cList);
            return records;
        }

        protected StringBuilder processRecords(ExperimentRecord[] records, string indices)
        {
            bool useIndex = false;
            StringBuilder buf = new StringBuilder();
            int[] indexes = null;
            char[] delim = ",".ToCharArray();
            if (indices != null && indices.Length > 0)
            {
                useIndex = true;
                string[] idxStr = indices.Split(delim);
                List<int> idx = new List<int>();
                foreach (string s in idxStr)
                {
                    idx.Add(Convert.ToInt32(s));
                }
                idx.Sort();
                indexes = idx.ToArray();
            }
            if (records.Length > 0)
            {
                foreach (ExperimentRecord rec in records)
                {
                    buf.Append(rec.sequenceNum.ToString());
                    if (useIndex)
                    {
                        string[] values = null;
                        values = rec.contents.Split(delim);
                        buf.Append("," + values[0]);
                        for (int i = 0; i < indexes.Length; i++)
                        {
                            // May need an offset
                            buf.Append("," + values[indexes[i]]);
                        }
                        buf.AppendLine();
                    }
                    else
                    {
                        buf.AppendLine("," + rec.contents);
                    }
                }
            }
            return buf;
        }
    }
}
