/* $Id: SelectTimeSlots.aspx.cs,v 1.21 2007/12/26 05:27:30 pbailey Exp $ */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using iLabs.Controls.Scheduling;
using iLabs.Core;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Proxies.LSS;
using iLabs.Proxies.Ticketing;
using iLabs.Ticketing;
using iLabs.UtilLib;

namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Summary description for ConfirmReservation.
	/// </summary>
	public partial class SelectTimePeriods: System.Web.UI.Page
	{
		 DateTime startTime;
         DateTime endTime;
         string serviceBrokerGuid = null;
         string groupName = null;
        string clientGuid = null;
        string labServerGuid = null;
         string labClientName = null;
         string labClientVersion = null;
         string userName = null;
         string lssURL = null;
         TimePeriod[] timeSlots;
         DateTime startReserveTime;
         DateTime endReserveTime;
         string lssGuid = null;
         TicketIssuerProxy ticketIssuer;
         Coupon coupon;
         int userTZ;
        CultureInfo culture;
        ProcessAgentDB dbTicketing = new ProcessAgentDB();
        List<TimePeriod> periods = null;
        int defaultRange = 30;
        TimeSpan maxAllowTime;
        TimeSpan minRequiredTime;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);
			serviceBrokerGuid = Session["serviceBrokerGuid"].ToString();
			groupName = Session["groupName"].ToString();
            clientGuid = Session["clientGuid"].ToString();
            labServerGuid = Session["labServerGuid"].ToString();
			labClientName = Session["labClientName"].ToString();
			labClientVersion = Session["labClientVersion"].ToString();
			userName = Session["userName"].ToString();
			lssURL = Session["lssURL"].ToString();
            lssGuid = Session["lssGuid"].ToString();
            userTZ = Convert.ToInt32(Session["userTZ"]);
            coupon = (Coupon) Session["coupon"];

            if (IsPostBack){
                if(Session["startTime"]!= null)
                   startTime = (DateTime)Session["startTime"] ;
                if(Session["endTime"]!= null)
                    endTime = (DateTime)Session["endTime"];
                if (Session["minAllowTime"] != null)
                {
                    minRequiredTime = TimeSpan.FromMinutes((int)Session["minAllowTime"]);
                }
                else
                {
                    minRequiredTime = TimeSpan.FromMinutes(1);
                }
                if (Session["maxAllowTime"] != null)
                {
                    maxAllowTime = TimeSpan.FromMinutes((int)Session["maxAllowTime"]);
                }
                else
                {
                    maxAllowTime = TimeSpan.FromMinutes(42);
                }
            }

            else{
                string value1 = Request.QueryString["start"];
                string value2 = Request.QueryString["end"];
                startTime = DateUtil.ParseUtc(value1);
                Session["startTime"] = startTime;
                endTime = DateUtil.ParseUtc(value2);
                Session["endTime"] = endTime;
                int[] policyIDs = USSSchedulingAPI.ListUSSPolicyIDsByGroupAndExperiment(groupName, serviceBrokerGuid, clientGuid, labServerGuid);
                for (int i = 0; i < policyIDs.Length; i++)
                {
                    USSPolicy pol = USSSchedulingAPI.GetUSSPolicies(new int[] { policyIDs[i] })[0];
                    string maxstr = PolicyParser.getProperty(pol.rule, "Maximum reservable time");
                    if (maxstr != null)
                    {
                        maxAllowTime = TimeSpan.FromMinutes(Int32.Parse(maxstr));
                        Session["maxAllowTime"] = (int) maxAllowTime.TotalMinutes; ;
                    }
                    string minstr = PolicyParser.getProperty(pol.rule, "Minimum time required");
                    if (minstr != null)
                    {
                        minRequiredTime = TimeSpan.FromMinutes(Int32.Parse(minstr));
                        Session["minAllowTime"] = (int) minRequiredTime.TotalMinutes;
                    }
                    lblUssPolicy.Text = "Minimum time required: " + minRequiredTime + "<br />Maximum time allowed: " + maxAllowTime;
                }
            }
            getTimePeriods();
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


        void getTimePeriods()
        {
            OperationAuthHeader opHeader = new OperationAuthHeader();
            opHeader.coupon = coupon;
            LabSchedulingProxy lssProxy = new LabSchedulingProxy();
            lssProxy.Url = lssURL;
            lssProxy.OperationAuthHeaderValue = opHeader;

            TimePeriod[] availablePeriods = lssProxy.RetrieveAvailableTimePeriods(serviceBrokerGuid, groupName,
                "", labServerGuid, clientGuid, startTime, endTime);
            if (availablePeriods == null)
            {
                string msg = "There are no available time slots for this experiment.";
                lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                lblErrorMessage.Visible = true;
                btnMakeReservation.Visible = false;
               // btnMakeReservation1.Visible = false;
                cntrScheduling.Visible=false;
            }
            else{
                cntrScheduling.Visible = true;
                cntrScheduling.StartTime = startTime;
                cntrScheduling.EndTime = endTime;
                cntrScheduling.UserTZ = userTZ;
                cntrScheduling.Culture = culture;
               
                cntrScheduling.DataSource = availablePeriods;
                cntrScheduling.DataBind();
            }
        }

        protected void TimePeriod_Click(object sender, System.EventArgs e)
        {
            AvailableClickEventArgs args = (AvailableClickEventArgs)e;
            int quantum = args.Quantum;
            TimeSpan quantTS = TimeSpan.FromMinutes(quantum);
            TimeSpan duration = TimeSpan.FromSeconds(args.Duration);
            DateTime endTime = args.Start.Add(duration).Subtract(minRequiredTime);
            StringBuilder buf = new StringBuilder();
            //buf.Append("StartTime: " + args.Start.ToString("o") + "<br />&nbsp;&nbsp;Duration: " + duration.ToString() + " Quant: " + quantum + "<br />");
            buf.Append("The minimum time required for this experiment is: " + minRequiredTime.ToString() + ".<br />");
            buf.Append("The maximum time allowed is: " + maxAllowTime.ToString() + ".");
            lblUssPolicy.Text = buf.ToString();

            ddlSelectTime.Items.Clear();
            ddlDuration.Items.Clear();
            DateTime wrkTime = args.Start.AddMinutes((args.Start.Minute % args.Quantum));
            int count = 0;
           
            while ((count <= defaultRange) && (wrkTime <= endTime))
            {
                ddlSelectTime.Items.Add(new ListItem(DateUtil.ToUserTime(wrkTime, culture, userTZ)));
                wrkTime = wrkTime.AddMinutes(quantum);
                count += quantum;
            }
            TimeSpan span = minRequiredTime;
            ddlDuration.Items.Add(new ListItem(DateUtil.TimeSpanTrunc(span), Convert.ToInt32(span.TotalSeconds).ToString()));
            span = span.Add(quantTS);
            span = span.Subtract(TimeSpan.FromMinutes((double)(span.Minutes % quantum)));
            while ((span <= maxAllowTime) && (span <duration))
            {
                ddlDuration.Items.Add(new ListItem(DateUtil.TimeSpanTrunc(span), Convert.ToInt32(span.TotalSeconds).ToString()));
                span = span.Add(quantTS);
            }
            if( (span <= maxAllowTime) && (endTime >= args.Start.Add(span)))
                ddlDuration.Items.Add(new ListItem(DateUtil.TimeSpanTrunc(maxAllowTime), Convert.ToInt32(maxAllowTime.TotalSeconds).ToString()));
        }


        protected void btnMakeReservation_Click(object sender, System.EventArgs e)
        {
            // ToDo: Add error checking
            DateTime startReserveTime = DateUtil.ParseUserToUtc(ddlSelectTime.SelectedItem.Text, culture, userTZ);
            DateTime endReserveTime = startReserveTime.AddSeconds(Double.Parse(ddlDuration.SelectedValue));
            lblErrorMessage.Text = ddlSelectTime.SelectedItem.Text + " " + DateUtil.ToUserTime(endReserveTime, culture, -userTZ);
            lblErrorMessage.Visible = true;
            string notification = null;
            LabSchedulingProxy lssProxy = new LabSchedulingProxy();
            lssProxy.Url = lssURL;
            try
            {
                // create "REQUEST RESERVATION" ticket in SB and get the coupon for the ticket.
                //iLabs.DataTypes.TicketingTypes.Coupon authCoupon = ticketIssuer.CreateTicket(lssGuid, "REQUEST RESERVATION", 300, "");

                //assign the coupon from ticket to the soap header;
                OperationAuthHeader opHeader = new OperationAuthHeader();
                opHeader.coupon = coupon;
                lssProxy.OperationAuthHeaderValue = opHeader;
                notification = lssProxy.ConfirmReservation(serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid, labServerGuid, clientGuid,
                    startReserveTime, endReserveTime);
                if (notification != "The reservation is confirmed successfully")
                {
                    lblErrorMessage.Text = Utilities.FormatErrorMessage(notification);
                }
                else
                {
                    try
                    {
                        int status = USSSchedulingAPI.AddReservation(userName, serviceBrokerGuid, groupName, labServerGuid, clientGuid, 
                            startReserveTime, endReserveTime);
                        lblErrorMessage.Text = Utilities.FormatConfirmationMessage(notification);
                    }
                    catch(Exception insertEx){
                        lblErrorMessage.Text = Utilities.FormatErrorMessage(notification);
                        lblErrorMessage.Visible = true;
                    }
                     getTimePeriods();
                }
                lblErrorMessage.Visible = true;
                    return;
            }
            catch (Exception ex)
            {
                string msg = "Exception: reservation can not be confirmed. " + ex.GetBaseException() + ".";
                lblErrorMessage.Text = Utilities.FormatErrorMessage(msg);
                lblErrorMessage.Visible = true;

            }
            try
            {
                if (notification == "The reservation is confirmed successfully")
                {
                    int experimentInfoId = USSSchedulingAPI.ListExperimentInfoIDByExperiment(labServerGuid, clientGuid);
                    DateTime startTimeUTC = startReserveTime.ToUniversalTime();
                    DateTime endTimeUTC = endReserveTime.ToUniversalTime();
                }
                return;
            }
            catch (Exception ex)
            {
                string msg = "Exception: reservation can not be added successfully. " + ex.GetBaseException() + ".";
                lblErrorMessage.Text = Utilities.FormatErrorMessage(msg);
                lblErrorMessage.Visible = true;
                lssProxy.RemoveReservation(serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid, labServerGuid, clientGuid, startReserveTime, endReserveTime);
            }
        }
        
        protected void btnReturn_Click(Object Src, EventArgs E)
        {
            // This routine will create a javascript block to refresh the client & close the page.
            Page.ClientScript.RegisterStartupScript( this.GetType(), "Success", "ReloadParent();", true);
        }
 
	}
	
}
