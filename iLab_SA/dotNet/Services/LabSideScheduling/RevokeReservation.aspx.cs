
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Proxies.USS;
using iLabs.UtilLib;
using iLabs.Ticketing;



namespace iLabs.Scheduling.LabSide
{
	/// <summary>
	/// Summary description for RevokeReservation.
	/// </summary>
    public partial class RevokeReservation : System.Web.UI.Page
	{
		string labServerGuid = null;
        string labServerName = null;
        ProcessAgentDB dbTicketing = new ProcessAgentDB();

        
        string couponID = null, passkey = null, issuerID = null, sbUrl = null;
        CultureInfo culture;
        int userTZ = 0;

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);
			// Put user code to initialize the page here
            txtStartDate.Attributes.Add("OnKeyPress", "return false;");
            txtEndDate.Attributes.Add("OnKeyPress", "return false;");

            // Add the JavaScript code to the page.
            if (!ClientScript.IsClientScriptBlockRegistered("DisableRevoke"))
            {
                StringBuilder jsBuf = new StringBuilder();
                jsBuf.AppendLine("<script> function DisableRevoke() {");
                //jsBuf.AppendLine("debugger");
                jsBuf.AppendLine("document.getElementById('btnRevoke').disabled = true; ");
                jsBuf.AppendLine("document.getElementById('btnReserve').disabled = true; ");
                //jsBuf.AppendLine("return false;");
                jsBuf.AppendLine("}</script>");
                ClientScript.RegisterClientScriptBlock(this.GetType(), "DisableRevoke", jsBuf.ToString());
            }
            //string disableFunction = "javascript:disableRevoke();";
            string disableFunction = "DisableRevoke();";
            ddlResource.Attributes.Add("onchange", disableFunction);
            ddlExperiment.Attributes.Add("onchange", disableFunction);
            ddlGroup.Attributes.Add("onchange", disableFunction);
            txtStartDate.Attributes.Add("onchange", disableFunction);
            ddlStartHour.Attributes.Add("onchange", disableFunction);
            txtStartMin.Attributes.Add("onchange", disableFunction);
            ddlStartAM.Attributes.Add("onchange", disableFunction);
            txtEndDate.Attributes.Add("onchange", disableFunction);
            ddlEndHour.Attributes.Add("onchange", disableFunction);
            txtEndMin.Attributes.Add("onchange", disableFunction);
            ddlEndAM.Attributes.Add("onchange", disableFunction);

            if (!IsPostBack)
            {

                if (Session["couponID"] == null || Request.QueryString["coupon_id"] != null)
                    couponID = Request.QueryString["coupon_id"];
                else
                    couponID = Session["couponID"].ToString();

                if (Session["passkey"] == null || Request.QueryString["passkey"] != null)
                    passkey = Request.QueryString["passkey"];
                else
                    passkey = Session["passkey"].ToString();

                if (Session["issuerID"] == null || Request.QueryString["issuer_guid"] != null)
                    issuerID = Request.QueryString["issuer_guid"];
                else
                    issuerID = Session["issuerID"].ToString();

                if (Session["sbUrl"] == null || Request.QueryString["sb_url"] != null)
                    sbUrl = Request.QueryString["sb_url"];
                else
                    sbUrl = Session["sbUrl"].ToString();

                bool unauthorized = false;

                if (couponID != null && passkey != null && issuerID != null)
                {
                    try
                    {
                        Coupon coupon = new Coupon(issuerID, long.Parse(couponID), passkey);
                        

                        Ticket ticket = dbTicketing.RetrieveAndVerify(coupon, TicketTypes.MANAGE_LAB);

                        if (ticket == null || ticket.IsExpired() || ticket.isCancelled)
                        {
                            unauthorized = true;
                            Response.Redirect("Unauthorized.aspx", false);
                        }

                        Session["couponID"] = couponID;
                        Session["passkey"] = passkey;
                        Session["issuerID"] = issuerID;
                        Session["sbUrl"] = sbUrl;

                        XmlDocument payload = new XmlDocument();
                        payload.LoadXml(ticket.payload);

                        Session["labServerGuid"] = payload.GetElementsByTagName("labServerGuid")[0].InnerText;
                        Session["labServerName"] = payload.GetElementsByTagName("labServerName")[0].InnerText;
                        Session["adminGroup"] = payload.GetElementsByTagName("adminGroup")[0].InnerText;
                        userTZ = Convert.ToInt32(payload.GetElementsByTagName("userTZ")[0].InnerText);
                        Session["userTZ"] = userTZ;

                        LoadResourceListBox(Session["labServerGuid"].ToString());
                        // Load the Group list box
                        LoadGroupListBox(Session["labServerGuid"].ToString());
                        // Load the Experiment list box
                        LoadExperimentListBox(Session["labServerGuid"].ToString());
                        LoadStartLists();
                        // load the reservation List box.
                        //BuildReservationListBox(Session["labServerGuid"].ToString());

                        DateTime curTime = DateTime.UtcNow;
                        txtStartDate.Text = DateUtil.ToUserDate(curTime, culture, userTZ);
                        curTime = curTime.AddMinutes(userTZ);
                        int hour = curTime.Hour;
                        if (hour > 12)
                        {
                            ddlStartHour.SelectedValue = (hour - 12).ToString();
                            ddlStartAM.SelectedValue = "PM";
                        }
                        else
                        {
                            ddlStartHour.SelectedValue = hour.ToString();
                            ddlStartAM.SelectedValue = "AM";
                        }
                        int minute = curTime.Minute;
                        txtStartMin.Text = minute.ToString();

                        //txtEndDate.Text = culture.DateTimeFormat.ShortDatePattern;

                        lblDescription.Text = "All reservations for LabServer: " + Session["labServerName"].ToString()
                            + "<br/>for the time span you select will be revoked."
                            + "<br/><br/>Times shown are GMT:&nbsp;&nbsp;&nbsp;" + userTZ/60.0;
                    }

                    catch (Exception ex)
                    {
                        unauthorized = true;
                        Response.Redirect("Unauthorized.aspx", false);
                    }
                }

                else
                {
                    unauthorized = true;
                    Response.Redirect("Unauthorized.aspx", false);
                }

                if (!unauthorized)
                {
                   
                }
            }
            labServerGuid = (string) Session["labServerGuid"];
            labServerName = (string) Session["labServerName"];
            if(Session["userTZ"] != null)
                userTZ  = (int) Session["userTZ"];
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

        private void LoadResourceListBox(string lsGuid)
        {
            ddlResource.Items.Clear();
            ddlResource.Items.Add(new ListItem(" ---------- All Resources ---------- "));
            IntTag[] resources = DBManager.GetLSResourceTags(lsGuid);
            foreach (IntTag it in resources)
            {
                ddlResource.Items.Add(new ListItem(it.tag, it.id.ToString()));
            }

        }

        private void LoadGroupListBox(string lsGuid)
        {
            ddlGroup.Items.Clear();
            try
            {
                ddlGroup.Items.Add(new ListItem(" ---------- All Groups ---------- "));
                LssCredentialSet[] credentialSets = LSSSchedulingAPI.GetCredentialSetsByLS(lsGuid);
                for (int i = 0; i < credentialSets.Length; i++)
                {
                    string cred = credentialSets[i].groupName + " : " + credentialSets[i].serviceBrokerName;
                    ddlGroup.Items.Add(new ListItem(cred, credentialSets[i].credentialSetId.ToString()));
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("can not load the Group List Box" + ex.Message);
                lblErrorMessage.Visible = true;
            }
        }

        private void LoadExperimentListBox(string labServerID)
        {
            ddlExperiment.Items.Clear();
            try
            {
                ddlExperiment.Items.Add(new ListItem(" ---------- Any Experiment ---------- "));
                int[] experimentInfoIDs = LSSSchedulingAPI.ListExperimentInfoIDsByLabServer(labServerID);
                LssExperimentInfo[] experimentInfos = LSSSchedulingAPI.GetExperimentInfos(experimentInfoIDs);
                for (int i = 0; i < experimentInfoIDs.Length; i++)
                {

                    string exper = experimentInfos[i].labClientName + "  " + experimentInfos[i].labClientVersion;
                    ddlExperiment.Items.Add(new ListItem(exper, experimentInfos[i].experimentInfoId.ToString()));
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("can not load the Experiment List Box" + ex.Message);
                lblErrorMessage.Visible = true;
            }

        }
        private void LoadStartLists()
        {
            ddlStartHour.Items.Clear();
            ddlStartHour.Items.Add(new ListItem("12","0"));
             ddlStartHour.Items.Add(new ListItem("1","1"));
             ddlStartHour.Items.Add(new ListItem("2","2"));
             ddlStartHour.Items.Add(new ListItem("3","3"));
             ddlStartHour.Items.Add(new ListItem("4","4"));
             ddlStartHour.Items.Add(new ListItem("5","5"));
            ddlStartHour.Items.Add(new ListItem("6","6"));
            ddlStartHour.Items.Add(new ListItem("7","7"));
            ddlStartHour.Items.Add(new ListItem("8","8"));
            ddlStartHour.Items.Add(new ListItem("9","9"));
            ddlStartHour.Items.Add(new ListItem("10", "10"));
            ddlStartHour.Items.Add(new ListItem("11","11"));

            ddlStartAM.Items.Clear();
            ddlStartAM.Items.Add(new ListItem("AM", "AM"));
            ddlStartAM.Items.Add(new ListItem("PM", "PM"));
        }

        //list the reservation information according to the selected criterion
        private void BuildReservationListBox(int resourceID, int ExperimentInfoID, int CredentialSetID, DateTime time1, DateTime time2)
        {

            try
            {
                txtDisplay.Text = null;
                IntTag[] reservations = LSSSchedulingAPI.ListReservations(resourceID, ExperimentInfoID, CredentialSetID, time1, time2, culture, userTZ);
                if (reservations == null || reservations.Length == 0)
                {
                    lblErrorMessage.Text = Utilities.FormatConfirmationMessage("no reservations have been found.");
                    lblErrorMessage.Visible = true;
                }
                else
                {
                    StringBuilder buf = new StringBuilder();
                    foreach (IntTag t in reservations)
                    {
                        buf.AppendLine(t.tag);
                    }
                    txtDisplay.Text = buf.ToString();
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("can not retrieve reservationInfos  " + ex.Message);
                lblErrorMessage.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, System.EventArgs e)
        {
            lblErrorMessage.Text = "";
            lblErrorMessage.Visible = false;
            int resourceID = 0;
            int expID = 0;
            int credID = 0;
            DateTime start = getStartTime();
            DateTime end = getEndTime();

            if (ddlResource.SelectedIndex > 0)
            {
                resourceID = Convert.ToInt32(ddlResource.SelectedValue);
            }
            if (ddlGroup.SelectedIndex > 0)
            {
                credID = Convert.ToInt32(ddlGroup.SelectedValue);
            }
            if (ddlExperiment.SelectedIndex > 0)
            {
                expID = Convert.ToInt32(ddlExperiment.SelectedValue);
            }
            hdnResource.Value = resourceID.ToString();
            hdnExpID.Value = expID.ToString();
            hdnCredID.Value = credID.ToString();
            hdnStart.Value = DateUtil.ToUtcString(start);
            hdnEnd.Value = DateUtil.ToUtcString(end);
            BuildReservationListBox(resourceID, expID, credID, start, end);
            trRevoke.Visible = true;
            btnRevoke.Enabled = true;
            //btnRevoke.Attributes.Add("onclick", "btnRevoke_Click");
            btnReserve.Enabled = true;
            //btnReserve.Attributes.Add("onclick", "btnReserve_Click");

            
        }

        private DateTime getStartTime()
        {
            DateTime tmp = DateTime.MinValue;
            if (txtStartDate.Text != null && txtStartDate.Text.Length > 0)
            {
                tmp = DateUtil.ParseUserToUtc(txtStartDate.Text, culture, 0);

                tmp = tmp.AddHours(Convert.ToDouble(ddlStartHour.SelectedValue));
                if (ddlStartAM.SelectedValue.Contains("PM"))
                {
                    tmp = tmp.AddHours(12);
                }
                tmp = tmp.AddMinutes(Convert.ToDouble(txtStartMin.Text) + userTZ);
            }
            return tmp;
        }

        private DateTime getEndTime()
        {
            DateTime tmpE = DateTime.MaxValue;
            if (txtEndDate.Text != null && txtEndDate.Text.Length > 0)
            {
                tmpE = DateUtil.ParseUserToUtc(txtEndDate.Text, culture, 0);
                tmpE = tmpE.AddHours(Convert.ToDouble(ddlEndHour.SelectedValue));
                if (ddlEndAM.SelectedValue.Contains("PM"))
                {
                    tmpE = tmpE.AddHours(12);
                }
                tmpE = tmpE.AddMinutes(Convert.ToDouble(txtStartMin.Text) + userTZ);
            }
            return tmpE;
        }


        protected void btnReserve_Click(object sender, System.EventArgs e)
        {
            int status = -1;
            int resourceId = 0;
            int expId = 0;
            int credId = 0;
            DateTime start = FactoryDB.MinDbDate;
            DateTime end = FactoryDB.MaxDbDate;
            lblErrorMessage.Visible = false;
            if (hdnResource.Value.CompareTo("0") != 0) { resourceId = Convert.ToInt32(hdnResource.Value); }
            if (hdnExpID.Value.CompareTo("0") != 0) { expId = Convert.ToInt32(hdnExpID.Value); }
            if (hdnCredID.Value.CompareTo("0") != 0) { credId = Convert.ToInt32(hdnCredID.Value); }
            start = DateUtil.ParseUtc(hdnStart.Value);
            end = DateUtil.ParseUtc(hdnEnd.Value);
            if (start < DateTime.UtcNow)
            {
                start = DateTime.UtcNow;
            }
            if (start > end)
            {
                lblErrorMessage.Text = Utilities.FormatWarningMessage("The end time is less than the current time. Only future reservations may be revoked!");
                lblErrorMessage.Visible = true;
                return;
            }
            status = revokeReservations(resourceId, expId,credId,start, end);
            int adminCredId = LSSSchedulingAPI.GetCredentialSetID(Session["adminSbGuid"].ToString(),Session["adminGroup"].ToString());
            status = LSSSchedulingAPI.AddReservationInfo(start, end, adminCredId, expId, resourceId, 0, 0);
            if (status > 0)
            {
                lblErrorMessage.Text = "The existing reservations have been revoked, and the time reserved for " + Session["adminGroup"].ToString();
                lblErrorMessage.Visible = true;
            }
        }


		protected void btnRevoke_Click(object sender, System.EventArgs e)
   		{
            int status = -1;
            int resourceId = 0;
            int expId = 0;
            int credId = 0;
            DateTime start = FactoryDB.MinDbDate;
            DateTime end = FactoryDB.MaxDbDate;
            lblErrorMessage.Visible = false;
            if (hdnResource.Value.CompareTo("0") != 0) { resourceId = Convert.ToInt32(hdnResource.Value); }
            if (hdnExpID.Value.CompareTo("0") != 0) { expId = Convert.ToInt32(hdnExpID.Value); }
            if (hdnCredID.Value.CompareTo("0") != 0) { credId = Convert.ToInt32(hdnCredID.Value); }
            start = DateUtil.ParseUtc(hdnStart.Value);
            end = DateUtil.ParseUtc(hdnEnd.Value);
            if (start < DateTime.UtcNow)
            {
                start = DateTime.UtcNow;
            }
            if (start > end)
            {
                lblErrorMessage.Text = Utilities.FormatWarningMessage("The end time is less than the current time. Only future reservations may be revoked!");
                lblErrorMessage.Visible = true;
                return;
            }
            status = revokeReservations(resourceId, expId, credId,start, end);
            if (status >= 0)
            {
                lblErrorMessage.Text = Utilities.FormatConfirmationMessage(status.ToString() + " reservations have been revoked.");
            }
            else
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("An error in revoking reservations.");
            }
            lblErrorMessage.Visible = true;
		}

        protected int revokeReservations(int resourceId, int expId, int credId, DateTime start, DateTime end) 
        {
            int count = 0;
            Dictionary<int,List<ReservationData>> reservations = new Dictionary<int,List<ReservationData>>();
            ReservationData[] data = DBManager.RetrieveReservationData(resourceId, expId, credId, start, end);
            if (data != null && data.Length > 0)
            {

                // get  list for each USS
                foreach (ReservationData rd in data)
                {
                    if (!reservations.ContainsKey(rd.ussId))
                    {
                        List<ReservationData> lst = new List<ReservationData>();
                        reservations.Add(rd.ussId, lst);
                    }
                    reservations[rd.ussId].Add(rd);
                }
                foreach (int uid in reservations.Keys)
                {


                    USSInfo uss = DBManager.GetUSSInfo(uid);
                    if (uss != null)
                    {
                        ProcessAgentDB paDB = new ProcessAgentDB();
                        UserSchedulingProxy ussProxy = new UserSchedulingProxy();
                        OperationAuthHeader header = new OperationAuthHeader();
                        header.coupon = paDB.GetCoupon(uss.couponId, uss.domainGuid);
                        ussProxy.OperationAuthHeaderValue = header;
                        ussProxy.Url = uss.ussUrl;
                        foreach (ReservationData res in reservations[uid])
                        {

                            int num = ussProxy.RevokeReservation(res.sbGuid, res.groupName,
                                res.labServerGuid, res.clientGuid, res.start, res.end, txtMessage.Text);
                            if (num > 0)
                            {
                                LSSSchedulingAPI.RemoveReservationInfoByIDs(new int[] { res.reservationID });
                                count += num;
                            }
                        }
                    }
                }
            }

            return count;
        }

        protected void makeReservation(int resourceID, int expId, string groupName, string sbGuid, DateTime start, DateTime end)
        {
        }

        void JunkCode()
        {
            string revoked = "revoke";
            int count = 0;
            DateTime startDate = DateTime.MinValue;
            int startHours = -1;
            int startMinutes = -1;
            DateTime endDate = DateTime.MinValue;
            int endHours = -1;
            int endMinutes = -1;


            // input error check
            try
            {
                if (txtStartMin.Text.Length > 0)
                    startMinutes = int.Parse(txtStartMin.Text);
                if (startMinutes >= 60 || startMinutes < 0)
                {
                    string msg = "Please input right form of minute in the start time ";
                    lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                    lblErrorMessage.Visible = true;
                }
                if (txtEndMin.Text.Length > 0)
                    endMinutes = int.Parse(txtEndMin.Text);
                if (endMinutes > 60 || endMinutes < 0)
                {
                    string msg = "Please input right form of minute in the end time ";
                    lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                    lblErrorMessage.Visible = true;
                }


                if (txtEndDate.Text.Length == 0 || txtEndDate.Text.CompareTo(culture.DateTimeFormat.ShortDatePattern) == 0)
                {
                    lblErrorMessage.Text = Utilities.FormatWarningMessage("You must enter the end date of the time block.");
                    lblErrorMessage.Visible = true;
                    return;
                }
                endDate = DateTime.Parse(txtEndDate.Text, culture);
                if (txtStartDate.Text.Length == 0 || txtStartDate.Text.CompareTo(culture.DateTimeFormat.ShortDatePattern) == 0)
                {
                    lblErrorMessage.Text = Utilities.FormatWarningMessage("You must enter the end date of the time block.");
                    lblErrorMessage.Visible = true;
                    return;
                }
                startDate = DateTime.Parse(txtStartDate.Text, culture);
                if (endDate < startDate)
                {
                    lblErrorMessage.Text = Utilities.FormatWarningMessage("The end date must be greater than or equal to the start date.");
                    lblErrorMessage.Visible = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                lblErrorMessage.Text = Utilities.FormatErrorMessage(msg);
                lblErrorMessage.Visible = true;
            }

            startHours = int.Parse(ddlStartHour.SelectedItem.Text);
            if (ddlStartAM.Text.CompareTo("PM") == 0)
            {
                startHours += 12;
            }

            endHours = int.Parse(ddlEndHour.SelectedItem.Text);
            if (ddlEndAM.Text.CompareTo("PM") == 0)
            {
                endHours += 12;
            }

            DateTime startTime = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                startHours, startMinutes, 0, DateTimeKind.Utc);
            startTime.AddMinutes(userTZ);
            DateTime endTime = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                endHours, endMinutes, 0, DateTimeKind.Utc);
            endTime.AddMinutes(userTZ);

            //Ticket ticketforRevo = ticketRetrieval.RetrieveAndVerify(coupon, TicketTypes.REVOKE_RESERVATION);

            // the removed reservations on LSS
            ArrayList removedRes = new ArrayList();
            ArrayList ussGuids = new ArrayList();

            if (startTime > endTime)
            {
                string msg = "the start time should be earlier than the end time";
                lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                lblErrorMessage.Visible = true;
                return;
            }
            try
            {
                //the reservations going to be removed
                int[] resIDs = DBManager.ListReservationInfoIDsByLabServer(Session["lsGuid"].ToString(), startTime, endTime);
                if (resIDs != null && resIDs.Length > 0)
                {
                    count = LSSSchedulingAPI.RevokeReservations(resIDs, txtMessage.Text);
                    lblErrorMessage.Text = Utilities.FormatConfirmationMessage("For the time period "
                        + DateUtil.ToUserTime(startTime, culture, userTZ) + " to "
                    + DateUtil.ToUserTime(endTime, culture, userTZ) + ", " + count + " out of " + resIDs.Length + " reservations have been revoked successfully.");
                    lblErrorMessage.Visible = true;
                }

            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("The related reservations have not been revoked successfully." + ex.Message);
                lblErrorMessage.Visible = true;
                // rollback
                foreach (ReservationInfo resInfo in removedRes)
                {
                    LSSSchedulingAPI.AddReservationInfo(resInfo.startTime, resInfo.endTime,
                        resInfo.credentialSetId, resInfo.experimentInfoId, resInfo.resourceId, resInfo.ussId, resInfo.statusCode);

                }
                return;
            }

            try
            {

                foreach (string uGuid in ussGuids)
                {
                    int uInfoID = LSSSchedulingAPI.ListUSSInfoID(uGuid);
                    USSInfo[] ussArray = LSSSchedulingAPI.GetUSSInfos(new int[] { uInfoID });
                    if (ussArray.Length > 0)
                    {
                        Coupon revokeCoupon = dbTicketing.GetCoupon(ussArray[0].couponId, ussArray[0].domainGuid);

                        UserSchedulingProxy ussProxy = new UserSchedulingProxy();
                        ussProxy.Url = ussArray[0].ussUrl;

                        //assign the coupon from ticket to the soap header;
                        OperationAuthHeader opHeader = new OperationAuthHeader();
                        opHeader.coupon = revokeCoupon;
                        ussProxy.OperationAuthHeaderValue = opHeader;

                        //if (ussProxy.RevokeReservation(Session["lsGuid"].ToString(), startTime, endTime))
                        //{
                        //    lblErrorMessage.Text = Utilities.FormatConfirmationMessage(" The related reservations have been revoked successfully !");
                        //    lblErrorMessage.Visible = true;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = Utilities.FormatErrorMessage("The related reservation have not been revoked successfully." + ex.Message);
                lblErrorMessage.Visible = true;
                // rollback
                foreach (ReservationInfo resInfo in removedRes)
                {
                    LSSSchedulingAPI.AddReservationInfo(resInfo.startTime, resInfo.endTime, resInfo.credentialSetId, resInfo.experimentInfoId, resInfo.resourceId, resInfo.ussId, resInfo.statusCode);

                }
            }
        }
      
	}
}
