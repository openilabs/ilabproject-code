/* $Id: SelectTimeSlots.aspx.cs,v 1.21 2007/12/26 05:27:30 pbailey Exp $ */

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using iLabs.Core;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Services;
using iLabs.Ticketing;
using iLabs.UtilLib;



namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Summary description for ConfirmReservation.
	/// </summary>
	public partial class SelectTimeSlots : System.Web.UI.Page
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
         iLabs.Services.TicketIssuerProxy ticketIssuer;
         Coupon coupon;
         int userTZ;
        CultureInfo culture;
        ProcessAgentDB dbTicketing = new ProcessAgentDB();
        
         int maxNumResTS;
         int minNumResTS;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);
			string value1 = Request.QueryString["StartTime"];
			string value2 = Request.QueryString["EndTime"];
			startTime = DateUtil.ParseUtc(value1);
			endTime = DateUtil.ParseUtc(value2);
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
            //long couponID = coupon.couponId;
            //String passKey = coupon.passkey;
            //string issuerID = coupon.issuerGuid;

            //coupon = new Coupon(issuerID, couponID, passKey);
            int[] policyIDs = USSSchedulingAPI.ListUSSPolicyIDsByGroupAndExperiment(groupName, serviceBrokerGuid, clientGuid, labServerGuid);
            for (int i = 0; i < policyIDs.Length; i++)
            {
                USSPolicy pol = USSSchedulingAPI.GetUSSPolicies(new int[] { policyIDs[i] })[0];
                lblTimeSlotsPolicy.Text = lblTimeSlotsPolicy.Text + pol.rule;
                string maxstr = PolicyParser.getProperty(pol.rule, "Maximum number of reservable Time Slots");
                if (maxstr != null)
                    maxNumResTS = Int32.Parse(maxstr);
                string minstr = PolicyParser.getProperty(pol.rule, "Minimun number of reservable Time Slots");
                if (minstr != null)
                    minNumResTS = Int32.Parse(minstr);
            }

            /*if (!IsPostBack)
            {
                ////create a ticket issuer and assign the soapheader to the ticket issuer
                //iLabs.Services.TicketIssuerWebService ticketIssuer = new iLabs.Services.TicketIssuerWebService();
                //string serviceBrokerWebServiceUrl = ConfigurationManager.AppSettings["serviceBrokerWebServiceUrl"].ToString();
                //ticketIssuer.Url = serviceBrokerWebServiceUrl;
                ////iLabs.DataTypes.TicketingTypes.Coupon agentCoupon = new iLabs.DataTypes.TicketingTypes.Coupon("1be13120-04a3-4306-9b5c-9c184d694c10", 46, "284327138347703");
                //ProcessAgentInfo[] paInfos = dbTicketing.GetProcessAgentInfos("SCHEDULING SERVER");
                //Coupon agentCoupon = paInfos[0].identOut;
                //iLabs.Services.AgentAuthHeader agentAuthHeader = new iLabs.Services.AgentAuthHeader();
                //agentAuthHeader.agentCoupon = agentCoupon;
                //ticketIssuer.AgentAuthHeaderValue = agentAuthHeader;

                //long couponID = long.Parse(Session["couponID"].ToString());
                //String passKey = Session["passKey"].ToString();
                //string issuerID = Session["issuerID"].ToString();

                //coupon = new Coupon(issuerID, couponID, passKey);
                
            }*/
            showDayTimeSlotInfo();
			
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
        private void showDayTimeSlotInfo()
        {
            repTimeSlotsInfo.Controls.Clear();
            // create "REQUEST RESERVATION" ticket in SB and get the coupon for the ticket.
            //iLabs.DataTypes.TicketingTypes.Coupon authCoupon = ticketIssuer.CreateTicket(lssGuid, "REQUEST RESERVATION", 300, "");
            //assign the coupon from ticket to the soap header;

            OperationAuthHeader opHeader = new OperationAuthHeader();
            opHeader.coupon = coupon;
            LabSchedulingProxy lssProxy = new LabSchedulingProxy();
            lssProxy.Url = lssURL;
            lssProxy.OperationAuthHeaderValue = opHeader;

            TimePeriod[] availableTimePeriods = lssProxy.RetrieveAvailableTimePeriods(serviceBrokerGuid, groupName,
                   ProcessAgentDB.ServiceGuid, clientGuid, labServerGuid, startTime, endTime);
            if (availableTimePeriods.Length > 0)
            {
                timeSlots = lssProxy.RetrieveTimeSlots(serviceBrokerGuid, groupName, clientGuid, labServerGuid, startTime, endTime);
                if (timeSlots.Length < minNumResTS)
                {
                    string msg = "not enough time slots for this experiment.";
                    lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                    lblErrorMessage.Visible = true;
                    btnMakeReservation.Visible = false;
                    btnMakeReservation1.Visible = false;

                }
                else
                {

                    int count = 0;
                    foreach (TimePeriod tp in timeSlots)
                    {
                        System.Web.UI.WebControls.CheckBox cbxSelect = new System.Web.UI.WebControls.CheckBox();
                        cbxSelect.Visible = true;
                        cbxSelect.Enabled = false;
                        cbxSelect.BackColor = Color.FromArgb(174, 155, 138);
                        foreach (TimePeriod avaTP in availableTimePeriods)
                        {
                            if (avaTP.startTime <= tp.startTime && avaTP.endTime >= tp.endTime)
                            {
                                cbxSelect.Enabled = true;
                                cbxSelect.BackColor = Color.White;
                                break;
                            }

                        }

                        repTimeSlotsInfo.Controls.AddAt(count, cbxSelect);
                        repTimeSlotsInfo.Controls.AddAt(count + 1, new LiteralControl(" " + DateUtil.ToUserTime(tp.startTime, culture, userTZ) + "---" + DateUtil.ToUserTime(tp.endTime, culture, userTZ) + "         "));
                        repTimeSlotsInfo.Controls.AddAt(count + 2, new LiteralControl("<br></br>"));
                        count += 3;
                    }
                }
            }
            else
            {
                string msg = "There are no available time slots for this experiment.";
                lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                lblErrorMessage.Visible = true;
                btnMakeReservation.Visible = false;
                btnMakeReservation1.Visible = false;
            }
        }

		private void clearCheckBox()
		{
			int cot= repTimeSlotsInfo.Controls.Count;
			for(int i=0; i<cot; i++)
			{
				if (repTimeSlotsInfo.Controls[i].GetType().FullName=="System.Web.UI.WebControls.CheckBox")
				{
					((CheckBox)repTimeSlotsInfo.Controls[i]).Checked = false;
					
				}
			}

		}

		protected void btnMakeReservation_Click(object sender, System.EventArgs e)
		{
			
			int cot= repTimeSlotsInfo.Controls.Count;
			ArrayList checkedIndexes = new ArrayList();
			for(int i=0; i<cot; i++)
			{
				if (repTimeSlotsInfo.Controls[i].GetType().FullName=="System.Web.UI.WebControls.CheckBox")
				{
					if (((CheckBox)repTimeSlotsInfo.Controls[i]).Checked)
					{
						
						int indexOfTS=i/3;
						checkedIndexes.Add(indexOfTS);
					}
				}
			}
			if(checkedIndexes.Count > maxNumResTS || checkedIndexes.Count < minNumResTS)
			{
				lblErrorMessage.Text= Utilities.FormatWarningMessage("please select more than " + minNumResTS.ToString() + " and less than " + maxNumResTS+" time slots!");
				lblErrorMessage.Visible=true;
				clearCheckBox();
				return;
			}
			for(int i=0; i<checkedIndexes.Count-1; i++)
			{
				if ((int)checkedIndexes[i+1]- (int)checkedIndexes[i]!=1)
				{
					lblErrorMessage.Text=Utilities.FormatWarningMessage("please select sequential time slots!");
					lblErrorMessage.Visible=true;
					clearCheckBox();
					return;
				}			
			}
			startReserveTime = ((TimePeriod)timeSlots[(int)checkedIndexes[0]]).startTime;
            endReserveTime = ((TimePeriod)timeSlots[(int)checkedIndexes[checkedIndexes.Count - 1]]).endTime;
			lblErrorMessage.Text= DateUtil.ToUserTime(startReserveTime,culture,userTZ) + " " + DateUtil.ToUserTime(endReserveTime,culture,userTZ) ;
			lblErrorMessage.Visible=true;
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
                notification = lssProxy.ConfirmReservation(serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid, clientGuid, labServerGuid, startReserveTime, endReserveTime);
			    lblErrorMessage.Text = Utilities.FormatConfirmationMessage(notification);
			    lblErrorMessage.Visible=true;
                if (notification != "The reservation is confirmed successfully")
                 return; 
               
				
			}
			catch(Exception ex)
			{
				string msg = "Exception: reservation can not be confirmed. "+ex.GetBaseException()+".";
				lblErrorMessage.Text= Utilities.FormatErrorMessage(msg);
				lblErrorMessage.Visible=true;
              
			}

			try
			{
				if(notification == "The reservation is confirmed successfully")
				{


                    int experimentInfoId = USSSchedulingAPI.ListExperimentInfoIDByExperiment(clientGuid, labServerGuid);
					DateTime startTimeUTC = startReserveTime.ToUniversalTime();
					DateTime endTimeUTC = endReserveTime.ToUniversalTime();
					USSSchedulingAPI.AddReservation( userName, serviceBrokerGuid, groupName, experimentInfoId, startTimeUTC, endTimeUTC);
					
				}
				string jScript;
                jScript = "<script> window.opener.document.forms[0].hiddenPopupOnMakeRev.value='1';";
                //jScript += "window.opener.document.forms[0].txtStartTimePeriod.value = 'imad';";
                //window.opener.document.forms[0].txtStartTimePeriod.value = '";
                jScript += "self.close();</script>";

                RegisterClientScriptBlock("SaveReservation", jScript);
			    return;
			}
			catch(Exception ex)
			{
				string msg = "Exception: reservation can not be added successfully. "+ex.GetBaseException()+".";
				lblErrorMessage.Text= Utilities.FormatErrorMessage(msg);
				lblErrorMessage.Visible=true;
                lssProxy.RemoveReservation(serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid, clientGuid, labServerGuid, startTime, endTime);
			}		
			
		}
		

	}
	
}
