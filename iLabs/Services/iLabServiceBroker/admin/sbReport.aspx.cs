using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web.SessionState;
using System.Collections.Generic;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Proxies.PAgent;
using iLabs.ServiceBroker;
using iLabs.Ticketing;
using iLabs.ServiceBroker.DataStorage;
using iLabs.DataTypes.StorageTypes;
using iLabs.Proxies.ESS;

using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authentication;
using iLabs.ServiceBroker.Authorization;
using iLabs.UtilLib;
namespace iLabs.ServiceBroker.admin
{
    /// <summary>
    /// Summary description for service Broker Stats.
    /// </summary>
    /// ***************************************************
    public partial class sbReport : System.Web.UI.Page
    {
        private Color disabled = Color.FromArgb(243, 239, 229);
        private bool secure = false;

        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();

        BrokerDB dbTicketing = new BrokerDB();

        protected void Page_Load(object sender, EventArgs e)
        {
            string couponId;
            string passkey;
            string issuerGuid;

            if (!IsPostBack)
            {

                // try & test for local access or not configured
                if (secure)
                {
                    // retrieve parameters from URL
                    couponId = Request.QueryString["coupon_id"];
                    passkey = Request.QueryString["passkey"];
                    issuerGuid = Request.QueryString["issuer_guid"];
                    Ticket allowAdminTicket = null;
                    if (couponId != null && passkey != null && issuerGuid != null)
                    {
                        allowAdminTicket = dbTicketing.RetrieveAndVerify(
                            new Coupon(issuerGuid, Int64.Parse(couponId), passkey),
                            TicketTypes.ADMINISTER_LS);
                    }
                    else
                    {
                        Response.Redirect("AccessDenied.aspx", true);
                    }
                }

                string returnUrl = Request.QueryString["returnURL"];
                if (returnUrl != null && returnUrl.Length > 0)
                    Session["returnURL"] = returnUrl;

                DisplayForm();

            }
        }

        /// ***************************************************

        protected void DisplayForm()
        {
            try
			{
				ddlGroupTarget.Items .Clear ();
				//ddlGroupTarget.Items .Add (new ListItem("--Select one--","0"));
				int[] groupIDs = wrapper.ListGroupIDsWrapper();
				Group[] groups=wrapper.GetGroupsWrapper(groupIDs);
                foreach (Group g in groups)
                {
                    if (g.groupID > 0)
                    {
                        if (
                            (!g.groupName.Equals(Group.ROOT))
                          && (!g.groupName.Equals(Group.ORPHANEDGROUP)) && (!g.groupName.Equals(Group.NEWUSERGROUP) && (!g.groupName.Equals(Group.SUPERUSER)))
                          && (g.groupType.Equals(GroupType.REGULAR)))
                        {
                            //Response.Write(g.groupID);
                            //Response.Write(g.groupType);
                            ddlGroupTarget.Items.Add(new ListItem(g.groupName, g.groupID.ToString()));
                        }
                    }
                }
				}
				catch(Exception ex)
				{
				    string msg = "Exception: Cannot list groups. "+ex.Message+". "+ex.GetBaseException()+".";
                    lblResponse.Text = Utilities.FormatErrorMessage(msg);
					lblResponse.Visible = true; 
				}
                ddlReportTarget.Items.Add(new ListItem("Group Roster", "1"));
                ddlReportTarget.Items.Add(new ListItem("Group Experiment Report", "2"));
                //ddlReportTarget.Items.Add(new ListItem("Group Stats Report", "3"));

			} //end displayform

        /// ***************************************************
        
        protected void btnSubmit_Click(object sender, System.EventArgs e)
        {
            // check to make sure that both a group and report type are selected
            if (ddlGroupTarget.Text == "" && ddlReportTarget.Text == "")
            {
                lblResponse.Text = "<div class=errormessage><p>Please select both a group and a report </p></div>";
                lblResponse.Visible = true;
                return;
            }
            else
            {
                switch (ddlReportTarget.Text )
                {
                    case "1":
                        ReportDisplayArea.Text = GroupRoster(Convert.ToInt32(ddlGroupTarget.Text));
                        ReportDisplayArea.Visible = true;
                        break;
                    case "2":
                        ReportDisplayArea.Text = GroupExpReport(Convert.ToInt32(ddlGroupTarget.Text));
                        ReportDisplayArea.Visible = true;
                        break;
                    case "3":
                        ReportDisplayArea.Text = GroupStatReport(Convert.ToInt32(ddlGroupTarget.Text));
                        ReportDisplayArea.Visible = true;
                        break;

                } //end switch statement
            } //end else

        } // end btnSubmit_Click

        /// ***************************************************

        protected string GroupRoster(int groupID)
        {
            string reportDate = System.DateTime.Now.ToString();
            string groupReportTXT = "";
            string lSession = "";
            int[] gIDs;
            gIDs = new int[1] { groupID };
            Group[] groups = wrapper.GetGroupsWrapper(gIDs);
            foreach (Group g in groups)
            {
                // Get GroupName from GroupID
                groupReportTXT = "<br><br><hr><br><center><h1><b>" + g.groupName + "</b></h1></center><br><br>Report Date: " + reportDate + " <br>Description: " + g.description + "<br><br>\n";
            }

            groupReportTXT = groupReportTXT + "<p><table border=1 width=650 ><tr><th>Last Name</th><th>First Name</th><th>Username</th><th>Email</th><th>Last Login</th></tr> \n";

            // Get Group user information (name, username, last_login)
            int[] userIDs = wrapper.ListUserIDsInGroupWrapper(groupID);
            User[] users = wrapper.GetUsersWrapper(userIDs);
            foreach (User u in users)
            {
                lSession = GetLastLogin(u.userID, groupID);
                groupReportTXT = groupReportTXT + "<tr><td>"+ u.lastName +"</td><td>"+ u.firstName +"</td><td>"+ u.userName +"</td><td>"+ u.email +"</td><td>"+ lSession +"</td></tr>\n"; 
            } 

            groupReportTXT = groupReportTXT + "</table></p>";
            return groupReportTXT;

        } // end GroupRoster

        /// ***************************************************

        protected string GroupExpReport(int groupID)
        {
            string reportDate = System.DateTime.Now.ToString();
            string groupExpTXT = "";
            string groupExpTXT2 = "";
            int numSession = 0;
            int totalSessions = 0;
            int numExp = 0;
            int totalExp = 0;
            int[] gIDs;
            gIDs = new int[1] { groupID };
            Group[] groups = wrapper.GetGroupsWrapper(gIDs);
            foreach (Group g in groups)
            {
                // Get GroupName from GroupID
                groupExpTXT = "<br><br><hr><br><center><h1><b>" + g.groupName + "</b></h1></center><br><br>Report Date: " + reportDate + " <br>Description: " + g.description +"<br><br>\n";

                // get the associated lab clients
                int[] lcIDsList = AdministrativeUtilities.GetGroupLabClients(g.groupID);
                LabClient[] lcList = wrapper.GetLabClientsWrapper(lcIDsList);
                groupExpTXT += "Associated Clients: <ul>";
                for (int i = 0; i < lcList.Length; i++)
                {
                    groupExpTXT += "<li><strong class=lab>" + lcList[i].clientName + "</strong> - " + lcList[i].clientShortDescription + "</li>";
                }
                groupExpTXT += "</ul><br>";
            }

            // Get the number of user logins
            // Get the number of experiments submitted
            groupExpTXT2 = "<p><table border=1 width=650 ><tr><th>Last Name</th><th>First Name</th><th>Username</th><th>Logins</th><th>Stored Experiments</th></tr> \n";
            // Get Group user information (name, username, last_login, number of experiments submitted)
            int[] userIDs = wrapper.ListUserIDsInGroupWrapper(groupID);
            User[] users = wrapper.GetUsersWrapper(userIDs);
            foreach (User u in users)
            {
                numSession = GetNumLogin(u.userID, groupID);
                totalSessions = totalSessions + numSession;
                numExp = GetNumExp(u.userID, groupID);
                totalExp = totalExp + numExp;
                groupExpTXT2 = groupExpTXT2 + "<tr><td>" + u.lastName + "</td><td>" + u.firstName + "</td><td>" + u.userName + "</td><td>" + numSession + "</td><td>" + numExp + "</td></tr>\n";
            }
            groupExpTXT2 = groupExpTXT2 + "</table></p>";
            groupExpTXT = groupExpTXT + "Number of users: " + users.Length + "\n<br>Number of user logins: " + totalSessions + "\n<br>Number of stored experiments: " + totalExp + "\n<br><br>";
            groupExpTXT = groupExpTXT + groupExpTXT2;
            return groupExpTXT;

        } // end GroupRoster

        /// ***************************************************

        protected string GroupStatReport(int groupID)
        {
            string var1 = "Group Statistic Report " + groupID;
            return var1;

        } // end GroupRoster

        /// ***************************************************

        //last log-in sessions according to the selected criterion
        private string GetLastLogin(int userID, int groupID)
        {
            string lastSession = "";
            try
            {
                UserSession[] sessions = wrapper.GetUserSessionsWrapper(userID, groupID, DateTime.MinValue, DateTime.MaxValue);
                if (sessions.Length == 0)
                {
                    lastSession = "No logins.";
                }
                else
                {
                    int pos = 0;
                    DateTime lastLogin = sessions[0].sessionStartTime;
                    for (int j = sessions.Length - 1; j > -1; j--)
                    {
                        if (lastLogin < sessions[j].sessionStartTime)
                        {
                            lastLogin = sessions[j].sessionStartTime;
                            pos = j;
                        }
                    }
                    //string gName = wrapper.GetGroupsWrapper(new int[] { sessions[pos].groupID })[0].groupName;
                    lastSession += sessions[pos].sessionStartTime.ToString() + " \n\n" ;
                }
            }
            catch (Exception ex)
            {
                lblResponse.Text = "<div class=errormessage><p>Cannot retrieve UserSessions. " + ex.GetBaseException() + "</p></div>";
                lblResponse.Visible = true;
            }
            return lastSession;
        }

        /// ***************************************************

        //return the number log-in sessions based on the selected criterion
        private int GetNumLogin(int userID, int groupID)
        {
            int num = 0;
            try
            {
                UserSession[] sessions = wrapper.GetUserSessionsWrapper(userID, groupID, DateTime.MinValue, DateTime.MaxValue);
                num = sessions.Length;
            }
            catch (Exception ex)
            {
                lblResponse.Text = "<div class=errormessage><p>Cannot retrieve UserSessions. " + ex.GetBaseException() + "</p></div>";
                lblResponse.Visible = true;
            }
            return num;
        }

        /// ***************************************************

        private int GetNumExp(int userID, int groupID)
        {
            int num = 0;
            List<Criterion> cList = new List<Criterion>();
            cList.Add(new Criterion("Group_ID", "=", groupID.ToString()));
            cList.Add(new Criterion("User_ID", "=", userID.ToString()));

            try
            {
                long[] eIDs = DataStorageAPI.RetrieveAuthorizedExpIDs(userID, groupID, cList.ToArray());
                num = eIDs.Length;
            }
            catch (Exception ex)
            {
                lblResponse.Text = "<div class=errormessage><p>Cannot retrieve UserSessions. " + ex.GetBaseException() + "</p></div>";
                lblResponse.Visible = true;
            }
            return num;
        }

        } //end public class sbReport
    } // end Namespace



    // client items  string[] ListClientItems(int clientID, int userID)