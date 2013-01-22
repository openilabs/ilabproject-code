/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
	 /// <summary>
    ///
	/// </summary>
	public partial class BEEgraph : System.Web.UI.Page
	{
        protected bool showTime;
		protected System.Web.UI.WebControls.Label lblCoupon;
		protected System.Web.UI.WebControls.Label lblTicket;
		protected System.Web.UI.WebControls.Label lblGroupNameTitle;
        protected string title = "Building Energy Efficiency iLab";
	    int tz = 0;
        Coupon opCoupon = null;
        ExperimentSummary theExperiment = null;
        ExperimentRecord[] curRecords = null;
        List<string> data = new List<string>();

		protected void Page_Load(object sender, System.EventArgs e)
		{
         
            if(Session["userTZ"] != null)
                tz = Convert.ToInt32(Session["userTZ"]);
            String returnURL = (string)Session["sbUrl"];
            if ((returnURL != null) && (returnURL.Length > 0))
            {
                lnkBackSB.NavigateUrl = returnURL;
                lnkBackSB.Visible = true;
            }
            else{
                lnkBackSB.Visible = false;
            }
            if (Session["opCouponID"] != null && Session["opIssuer"] != null && Session["opPasscode"] != null)
            {
                opCoupon = new Coupon(Session["opIssuer"].ToString(), Convert.ToInt64(Session["opCouponID"].ToString()), Session["opPasscode"].ToString());
            }
            else
            {
                throw new AccessDeniedException("Missing credentials for BEE Lab graph.");
            }
            LabDB dbManager = new LabDB();
            if (!IsPostBack)
            {
                // Query values from the request
                hdnExperimentID.Value = Request.QueryString["expid"];
                if (hdnExperimentID.Value != null && hdnExperimentID.Value.Length > 0)
                    hdnChannelID.Value = ChecksumUtil.ToMD5Hash("BEElab" + hdnExperimentID.Value);
                else
                    hdnChannelID.Value = "experiment-channel";

                InteractiveSBProxy sbProxy = new InteractiveSBProxy();
                ProcessAgentInfo sbInfo = dbManager.GetProcessAgentInfo(Session["opIssuer"].ToString());
                if (sbInfo != null)
                {
                    sbProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                    sbProxy.OperationAuthHeaderValue.coupon = opCoupon;
                    sbProxy.Url = sbInfo.webServiceUrl;
                    
                    //Criterion[] search = new Criterion[] { new Criterion("experiment_ID", "=", hdnExperimentID.Value) };
                    //ExperimentSummary[] expInfo = sbProxy.RetrieveExperimentSummary(search);
                    //if (expInfo.Length > 0)
                    //{
                    //    theExperiment = expInfo[0];
                    //}
                    ExperimentRecord[] profileRecords = getRecords(sbProxy, Convert.ToInt64(hdnExperimentID.Value),
                      new Criterion[] { new Criterion("Record_Type", "=", "profile") });
                    ExperimentRecord[] records = getRecords(sbProxy,Convert.ToInt64(hdnExperimentID.Value),
                       new Criterion[] { new Criterion("Record_Type", "=", "data")});
                    if(hdnChannelID.Value != null)
                        writeChannelID(hdnChannelID.Value);
                    //processProfile(profileRecords);
                    processRecords(records);
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
            records = sbProxy.RetrieveExperimentRecords(expID,cList);
            return records;
        }
        protected void writeChannelID(string channelID)
        {
            StringBuilder buf = new StringBuilder("<script type=\"text/javascript\">");
            buf.Append(" window.pusherChannel = '" + channelID + "';" );
            buf.AppendLine("</script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "pusherChannel", buf.ToString(), false);
        }

        protected void processProfile(ExperimentRecord[] records)
        {
            bool hasProfile = false;
            StringBuilder buf = new StringBuilder();
            char[] delim = ",".ToCharArray();
            buf.AppendLine("<script type=\"text/javascript\">");
            //TODO: Add ChannelName here as window
            buf.AppendLine(" window.profileData = [");
            if (records.Length > 0)
            {
                
                
                //DateTime epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                // This a simple solution it expects that there will be an even number of values
                foreach (ExperimentRecord rec in records)
                {
                   // TODO: Change to new profile format
                    string content = rec.contents;
                    string[] values = content.Split(delim);
                    int i = 0;
                    while( i< values.Length){
                        if(i> 0)
                            buf.Append(",");
                        buf.Append("[" + values[i++]);
                        buf.Append("," + values[i++] + "]");
                    }

                    if (hasProfile)
                    {
                        buf.AppendLine(",");
                    }
                    else
                    {
                        buf.AppendLine();
                        hasProfile = true;
                    }
                }
                
            }
            buf.AppendLine();
            buf.AppendLine("];");
            buf.AppendLine("</script>");
            
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "profileData", buf.ToString(), false);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "profileData", buf.ToString(), false);
        }
        protected void processRecords(ExperimentRecord[] records)
        {
            bool hasRecords = false;
            StringBuilder buf = new StringBuilder();
            char[] delim = ",".ToCharArray();
            buf.AppendLine("<script type=\"text/javascript\">");
            buf.Append(" window.sampleData = [");
            if (records.Length > 0)
            {

                //int i = 1;
                DateTime epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                //for(int i =0; i < 100;i++){
                //    ExperimentRecord rec = records[i];
                DateTime tStamp = new DateTime();
                foreach (ExperimentRecord rec in records)
                {
                    if (hasRecords)
                    {
                        buf.AppendLine(",");
                    }
                    else
                    {
                        buf.AppendLine();
                        hasRecords = true;
                    }
                    //string content = rec.contents;
                    //string[] values = content.Split(delim, 3);
                    //buf.Append("[" + values[0] + "," + values[1] + "," + values[2] + "]");
                    buf.Append("[" + rec.contents + "]");
                }
            }
            buf.AppendLine();
            buf.AppendLine("];");
            buf.AppendLine("</script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "graphData", buf.ToString(), false);

        }

        protected void processRecordsOrig(ExperimentRecord[] records)
        {
            bool hasRecords = false;
           StringBuilder buf = new StringBuilder();
            char[] delim = ",".ToCharArray();
            buf.AppendLine("<script type=\"text/javascript\">");
            buf.Append(" window.sampleData = [");
            if (records.Length > 0)
            {
               
                //int i = 1;
                DateTime epoc = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
                //for(int i =0; i < 100;i++){
                //    ExperimentRecord rec = records[i];
                foreach(ExperimentRecord rec in records){
                    string content = rec.contents;
                    string[] values = content.Split(delim, 5);
                    DateTime tStamp = new DateTime(0L, DateTimeKind.Utc);
                    tStamp = tStamp.AddYears(Convert.ToInt32(values[1]) - 1);
                    tStamp = tStamp.AddDays(Convert.ToDouble(values[2]) - 1.0);
                    string hhmm = values[3].PadLeft(4, '0');
                    tStamp = tStamp.AddHours(Convert.ToDouble(hhmm.Substring(0, 2)));
                    tStamp = tStamp.AddMinutes(Convert.ToDouble(hhmm.Substring(2, 2)));
                  
                    if(hasRecords){
                        buf.AppendLine(",");
                    }
                    else{
                        buf.AppendLine();
                        hasRecords = true;
                    }
                    buf.Append("[" + rec.sequenceNum.ToString() + ",'" + DateUtil.ToUtcString(tStamp) + "'," + values[4] + "]");                  
                }
               
            }
            buf.AppendLine();
            buf.AppendLine("];");
            buf.AppendLine("</script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "graphData", buf.ToString() , false);
            
        }
        protected void downloadClick(object sender, System.EventArgs e)
        {
            StringBuilder buf = new StringBuilder("downloadBEE.aspx?expid=");
            buf.Append(hdnExperimentID.Value);
            buf.Append("&min=" + hdnMin.Value);
            buf.Append("&max=" + hdnMax.Value);
            buf.Append("&index=" + hdnSensors.Value);
            Response.Redirect(buf.ToString(),false);

        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
