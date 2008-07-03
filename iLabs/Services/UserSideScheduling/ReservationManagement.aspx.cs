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
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;
using System.Globalization;
using System.Xml;

namespace iLabs.Scheduling.UserSide
{
	/// <summary>
	/// Summary description for ReservationManagement.
	/// </summary>
	public partial class ReservationManagement : System.Web.UI.Page
	{
        string couponID = null, passkey = null, issuerID = null, sbUrl = null;
        CultureInfo culture;
        int userTZ;
	
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
            // Load the Group list box
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

                        ProcessAgentDB dbTicketing = new ProcessAgentDB();
                        Ticket ticket = dbTicketing.RetrieveAndVerify(coupon, TicketTypes.MANAGE_USS_GROUP);
                        XmlDocument payload = new XmlDocument();
                        payload.LoadXml(ticket.payload);
                        if (ticket.IsExpired() || ticket.isCancelled)
                        {
                            unauthorized = true;
                            Response.Redirect("Unauthorized.aspx", false);
                        }

                        Session["couponID"] = couponID;
                        Session["passkey"] = passkey;
                        Session["issuerID"] = issuerID;
                        Session["sbUrl"] = sbUrl;
                        userTZ = Convert.ToInt32(payload.GetElementsByTagName("userTZ")[0].InnerText);
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
                    LoadGroupListBox();
                    LoadExperimentListBox();
                }
            }

			if (ddlTimeIs.SelectedIndex!=4)
			{
				txtTime2.ReadOnly=true;
				txtTime2.BackColor=Color.Lavender;
			}			
			
		}

	
		public void ddlTimeIs_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtTime1.Text=null;
			txtTime2.Text=null;
			if(ddlTimeIs.SelectedIndex==4)
			{
				txtTime2.ReadOnly=false;
				txtTime2.BackColor=Color.White;
			}
		}
	
		
		//list the reservation information according to the selected criterion
		private void BuildReservationListBox(string userName, int ExperimentInfoID, int credentialSetId, DateTime time1, DateTime time2)
		{
			
			try
			{
				txtDisplay.Text=null;
				ReservationInfo[] reservations = USSSchedulingAPI.SelectReservation(userName, ExperimentInfoID, credentialSetId,  time1,  time2);						
				if (reservations.Length==0)
				{
					lblErrorMessage.Text =Utilities.FormatConfirmationMessage("no reservations have been made.");
					lblErrorMessage.Visible=true;
				}
				else
				{
					
					for(int j = reservations.Length-1; j > -1  ; j--)
					{
						string uName = reservations[j].userName;
						UssExperimentInfo exinfo = USSSchedulingAPI.GetExperimentInfos(new int[]{reservations[j].experimentInfoId})[0];
						string experimentName = exinfo.labClientName + "  " + exinfo.labClientVersion;
						txtDisplay.Text += uName + ", " + experimentName + ", "+"Start time:  " + DateUtil.ToUserTime(reservations[j].startTime, culture, userTZ) + "   " + "End time:  " + DateUtil.ToUserTime(reservations[j].endTime, culture, userTZ)+"\n";
					}
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text =Utilities.FormatErrorMessage("can not retrieve reservations  "+ex.Message);
				lblErrorMessage.Visible=true;
			}
		}
		protected void btnGo_Click(object sender, System.EventArgs e)
		{
			lblErrorMessage.Text ="";
			lblErrorMessage.Visible=false;
			if(ddlGroup.SelectedIndex <= 0)
			{
				lblErrorMessage.Text =Utilities.FormatWarningMessage("please select a group!");
				lblErrorMessage.Visible=true;
				return;
			}
			    int credentialSetId = Int32.Parse(ddlGroup.SelectedValue) ;
				int experimentInfoId = -1;
				string userName = txtUserName.Text;

                if (ddlExperiment.SelectedIndex >= 1)
                {
                    experimentInfoId = Int32.Parse(ddlExperiment.SelectedValue);
                }
                else
                {
                    lblErrorMessage.Text = Utilities.FormatWarningMessage("please select a experiment!");
                    lblErrorMessage.Visible = true;
                    return;
                }
				
				if (ddlTimeIs.SelectedIndex<1)
				{
					BuildReservationListBox(userName, experimentInfoId,credentialSetId, DateTime.MinValue,DateTime.MinValue);

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
						BuildReservationListBox(userName, experimentInfoId,credentialSetId, time1,time1);

					}
					else if(ddlTimeIs.SelectedIndex==2)
					{
						BuildReservationListBox(userName, experimentInfoId,credentialSetId, DateTime.MinValue,time1);

					}				
					else if(ddlTimeIs.SelectedIndex==3)
					{
						BuildReservationListBox(userName, experimentInfoId,credentialSetId,time1,DateTime.MinValue);

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
						BuildReservationListBox(userName, experimentInfoId,credentialSetId,time1,time2);

					}
				}
					
		}
		private void LoadGroupListBox()
		{
			ddlGroup.Items.Clear();
			try
			{
				ddlGroup.Items.Add(new ListItem(" ---------- select Group ---------- "));
				int[] credentialSetIds = USSSchedulingAPI.ListCredentialSetIds();
				UssCredentialSet[] credentialSets=USSSchedulingAPI.GetCredentialSets(credentialSetIds);
				for(int i=0; i< credentialSets.Length; i++)
				{
					
					string cred=credentialSets[i].groupName+" "+credentialSets[i].serviceBrokerName;
					ddlGroup.Items.Add(new ListItem(cred, credentialSets[i].credentialSetId.ToString()));
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text ="can not load the Group List Box"+ex.Message;
				lblErrorMessage.Visible=true;
			}
		}

		private void LoadExperimentListBox()
		{
			ddlExperiment.Items.Clear();
			try
			{
				ddlExperiment.Items.Add(new ListItem(" ---------- select Experiment ---------- "));
				int[] experimentInfoIds = USSSchedulingAPI.ListExperimentInfoIDs();
				UssExperimentInfo[] experimentInfos = USSSchedulingAPI.GetExperimentInfos(experimentInfoIds);
				for(int i=0; i< experimentInfoIds.Length; i++)
				{
					
					string exper=experimentInfos[i].labClientName + "  " + experimentInfos[i].labClientVersion;
					ddlExperiment.Items.Add(new ListItem(exper, experimentInfos[i].experimentInfoId.ToString()));
				}
			}
			catch(Exception ex)
			{
				lblErrorMessage.Text =Utilities.FormatErrorMessage("can not load the Experiment List Box"+ex.Message);
				lblErrorMessage.Visible=true;
			}

		}
	
	}
	}
