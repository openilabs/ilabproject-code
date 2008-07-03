using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using iLabs.Core;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Ticketing;
using iLabs.DataTypes.TicketingTypes;
using System.Xml;
using System.Globalization;
using iLabs.UtilLib;


namespace iLabs.Scheduling.LabSide
{
	/// <summary>
	/// Summary description for RervationManagement.
	/// </summary>
	public partial class ReservationManagement : System.Web.UI.Page
	{
	    string labServerGuid;
        CultureInfo culture;
        string labServerName = null;
        string couponID = null, passkey = null, issuerID = null, sbUrl = null;
        int userTZ = 0;
        


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

		protected void Page_Load(object sender, System.EventArgs e)
		{
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);
            if (!IsPostBack)
            {
                lblDateTimeFormat.Text = culture.DateTimeFormat.ShortDatePattern;
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

                        ProcessAgentDB dbTicketing = new ProcessAgentDB();
                        Ticket ticket = dbTicketing.RetrieveAndVerify(coupon, TicketTypes.MANAGE_LAB);

                        if (ticket.IsExpired() || ticket.isCancelled)
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

                        labServerGuid = payload.GetElementsByTagName("labServerGuid")[0].InnerText;
                        Session["labServerGuid"] = labServerGuid;
                        labServerName = payload.GetElementsByTagName("labServerName")[0].InnerText;
                        Session["labServerName"] = labServerName;
                        userTZ = Convert.ToInt32(payload.GetElementsByTagName("userTZ")[0].InnerText);
                        Session["userTZ"] = userTZ;

                        lblDescription.Text = "Select criteria for the reservations displayed."
                           + "<br/><br/>Times shown are GMT:&nbsp;&nbsp;&nbsp;" + userTZ / 60.0;

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
                    //labServerGuid = Session["labServerGuid"].ToString();
                    LoadGroupListBox();
                    //lblLabServerName.Text = Session["labServerName"].ToString();
                    //lblLabServerName.Text = Session["labServerName"].ToString();
                    // Load the Experiment list box
                    LoadExperimentListBox(Session["labServerGuid"].ToString());
                    // load the reservation List box.
                    BuildReservationListBox(Session["labServerGuid"].ToString());
                }
            }

			if (ddlTimeIs.SelectedIndex!=4)
			{
				txtTime2.ReadOnly=true;
				txtTime2.BackColor=Color.Lavender;
			}
			
			
			// Load the Group list box
			
		}

	
		protected void ddlTimeIs_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtTime1.Text=null;
			txtTime2.Text=null;
			if(ddlTimeIs.SelectedIndex==4)
			{
				txtTime2.ReadOnly=false;
				txtTime2.BackColor=Color.White;
			}
		}
	
		//list the reservation information according to the labserverGuid
		private void BuildReservationListBox(string labServerGuid)
		{
			txtDisplay.Text=null;
			try
			{
				int[] reIDs = LSSSchedulingAPI.ListReservationInfoIDsByLabServer(labServerGuid, DateTime.Parse("1/1/1753 12:00:00 AM"), DateTime.MaxValue);						
				if (reIDs.Length==0)
				{
					lblErrorMessage.Text =Utilities.FormatConfirmationMessage("no reservations have been made.");
					lblErrorMessage.Visible=true;
				}
				else
				{
					ReservationInfo[] reservationInfos = LSSSchedulingAPI.GetReservationInfos(reIDs);
					for(int j = reservationInfos.Length-1; j > -1  ; j--)
					{
						LssExperimentInfo exinfo = LSSSchedulingAPI.GetExperimentInfos(new int[]{reservationInfos[j].experimentInfoId})[0];
						string experimentName = exinfo.labClientName + "  " + exinfo.labClientVersion;
						LssCredentialSet cre = LSSSchedulingAPI.GetCredentialSets(new int[]{reservationInfos[j].credentialSetId})[0];
						string ussName = LSSSchedulingAPI.GetUSSInfos(new int[]{LSSSchedulingAPI.ListUSSInfoID(cre.ussGuid)})[0].ussName;
						string credentialSetName = cre.groupName + "  " +cre.serviceBrokerName + "  " +ussName;
						txtDisplay.Text += experimentName + " , " + credentialSetName + ", "+"Start time:  " + DateUtil.ToUserTime(reservationInfos[j].startTime, culture,userTZ) + "   " + "End time:  " + DateUtil.ToUserTime(reservationInfos[j].endTime, culture, userTZ) +"\n";
					}
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text =Utilities.FormatErrorMessage("can not retrieve reservationInfos  "+ex.Message);
				lblErrorMessage.Visible=true;
			}
		}
		//list the reservation information according to the selected criterion
		private void BuildReservatoinListBox(string labServerID, int ExperimentInfoID, int CredentialSetID, DateTime time1, DateTime time2)
		{
			
			try
			{
				txtDisplay.Text=null;
				ReservationInfo[] reservationInfos = LSSSchedulingAPI.SelectReservationInfo(labServerID, ExperimentInfoID, CredentialSetID,  time1,  time2);						
				if (reservationInfos.Length==0)
				{
					lblErrorMessage.Text =Utilities.FormatWarningMessage("no reservations have been made.");
					lblErrorMessage.Visible=true;
				}
				else
				{
					
					for(int j = reservationInfos.Length-1; j > -1  ; j--)
					{
						LssExperimentInfo exinfo = LSSSchedulingAPI.GetExperimentInfos(new int[]{reservationInfos[j].experimentInfoId})[0];
						string experimentName = exinfo.labClientName + "  " + exinfo.labClientVersion;
						LssCredentialSet cre = LSSSchedulingAPI.GetCredentialSets(new int[]{reservationInfos[j].credentialSetId})[0];
						string ussName = LSSSchedulingAPI.GetUSSInfos(new int[]{LSSSchedulingAPI.ListUSSInfoID(cre.ussGuid)})[0].ussName;
						string credentialSetName = cre.groupName + "  " +cre.serviceBrokerName + "  " +ussName;
						txtDisplay.Text += experimentName + " , " + credentialSetName + ", "+"Start time:  " + DateUtil.ToUserTime(reservationInfos[j].startTime, culture, userTZ) + "   " + "End time:  " + DateUtil.ToUserTime(reservationInfos[j].endTime,culture,userTZ)+"\n";
					}
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text =Utilities.FormatErrorMessage("can not retrieve reservationInfos  " + ex.Message);
				lblErrorMessage.Visible=true;
			}
		}
		protected void btnGo_Click(object sender, System.EventArgs e)
		{
			lblErrorMessage.Text ="";
			lblErrorMessage.Visible=false;
			
			
			if(ddlGroup.SelectedIndex <= 0 && ddlExperiment.SelectedIndex <=0 && txtTime1.Text==null && txtTime2.Text==null)
			{
                BuildReservationListBox(Session["labServerName"].ToString());		
			}
			else
			{
				int experimentInfoID = -1;
				int credentialSetID = -1;
                if (ddlGroup.SelectedIndex >= 1)
                {
                    credentialSetID = Int32.Parse(ddlGroup.SelectedValue);
                }
                else
                {
                    lblErrorMessage.Text = Utilities.FormatWarningMessage("Please select a group");
                    lblErrorMessage.Visible = true;
                    return;
                }
				if (ddlExperiment.SelectedIndex >= 1)
				{
                      experimentInfoID = Int32.Parse(ddlExperiment.SelectedValue);
				}
                else
                {
                    lblErrorMessage.Text = Utilities.FormatWarningMessage("Please select a experiment");
                    lblErrorMessage.Visible = true;
                    return;
                }
				
				if (ddlTimeIs.SelectedIndex<1)
				{
                    BuildReservatoinListBox(Session["labServerName"].ToString(), experimentInfoID, credentialSetID, DateTime.MinValue, DateTime.MinValue);

				}
				else 
				{
					DateTime time1;
					try
					{
						time1 = DateTime.Parse (txtTime1.Text).ToUniversalTime();
					}
					catch
					{	
						lblErrorMessage.Text = Utilities.FormatWarningMessage("Please enter a valid time");
						lblErrorMessage.Visible=true;
						return;
					}
					if(ddlTimeIs.SelectedIndex==1)
					{
                        BuildReservatoinListBox(Session["labServerName"].ToString(), experimentInfoID, credentialSetID, time1, time1);

					}
					else if(ddlTimeIs.SelectedIndex==2)
					{
                        BuildReservatoinListBox(Session["labServerName"].ToString(), experimentInfoID, credentialSetID, DateTime.MinValue, time1);

					}				
					else if(ddlTimeIs.SelectedIndex==3)
					{
                        BuildReservatoinListBox(Session["labServerName"].ToString(), experimentInfoID, credentialSetID, time1, DateTime.MinValue);

					}
					else if(ddlTimeIs.SelectedIndex==4)
					{
						DateTime time2;
						try
						{
							time2 = DateTime.Parse (txtTime2.Text).ToUniversalTime();
						}
						catch
						{	
							lblErrorMessage.Text = Utilities.FormatWarningMessage("Please enter a valid time");
							lblErrorMessage.Visible=true;
							return;
						}
                        BuildReservatoinListBox(Session["labServerName"].ToString(), experimentInfoID, credentialSetID, time1, time2);

					}
				}
			}		
		}
		private void LoadGroupListBox()
		{
			ddlGroup.Items.Clear();
			try
			{
				ddlGroup.Items.Add(new ListItem(" ---------- select Group ---------- "));
				int[] credentialSetIDs = LSSSchedulingAPI.ListCredentialSetIDs();
				LssCredentialSet[] credentialSets=LSSSchedulingAPI.GetCredentialSets(credentialSetIDs);
				for(int i=0; i< credentialSets.Length; i++)
				{
					USSInfo[] uIn=LSSSchedulingAPI.GetUSSInfos(new int[]{LSSSchedulingAPI.ListUSSInfoID(credentialSets[i].ussGuid)});
					string cred=credentialSets[i].groupName+" "+credentialSets[i].serviceBrokerName + " " + uIn[0].ussName;
					ddlGroup.Items.Add(new ListItem(cred, credentialSets[i].credentialSetId.ToString()));
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text = Utilities.FormatErrorMessage("can not load the Group List Box"+ex.Message);
				lblErrorMessage.Visible=true;
			}
		}

		private void LoadExperimentListBox(string labServerID)
		{
			ddlExperiment.Items.Clear();
			try
			{
				ddlExperiment.Items.Add(new ListItem(" ---------- select Experiment ---------- "));
				int[] experimentInfoIDs = LSSSchedulingAPI.ListExperimentInfoIDsByLabServer(labServerID);
				LssExperimentInfo[] experimentInfos = LSSSchedulingAPI.GetExperimentInfos(experimentInfoIDs);
				for(int i=0; i< experimentInfoIDs.Length; i++)
				{
					
					string exper=experimentInfos[i].labClientName + "  " + experimentInfos[i].labClientVersion;
					ddlExperiment.Items.Add(new ListItem(exper, experimentInfos[i].experimentInfoId.ToString()));
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text = Utilities.FormatErrorMessage("can not load the Experiment List Box"+ex.Message);
				lblErrorMessage.Visible=true;
			}

		}
		/*private void btnSelectByLabServer_Click(object sender, System.EventArgs e)
		{
			if (txtLabServerID.Text == "" )
			{
				lblErrorMessage.Text ="Please enter the lab server ID";
				lblErrorMessage.Visible=true;
			}
			else
			{
				labServerIDG = txtLabServerID.Text;
				// Load the Experiment list box
				LoadExperimentListBox(labServerIDG);
				// load the reservation List box.
				BuildReservatoinListBox(labServerIDG);
			}
		}*/

        protected void NavBar1_Load(object sender, EventArgs e)
        {

        }
}
	}

