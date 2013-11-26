using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.DataTypes;
using iLabs.DataTypes.BatchTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;

using iLabs.Core;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Internal;
using iLabs.Ticketing;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.iLabSB
{
    public partial class LaunchClient : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder buf;
            long couponID = -1L;
            int userID = -1;
            int groupID = -1;
            int clientID = -1;
            int labServerID = -1;
            int userTZ = 0;

            string clientGuid = null;
            string serverGuid = null;
            string groupName = null;
            string userName = null;
            string authKey = null;
            string startStr = null;
            string durStr = null;
            Coupon initCoupon = null;
            LabClient client = null;

            BrokerDB brokerDB = new BrokerDB();
            buf = new StringBuilder();

            string cidStr = Request.QueryString["cid"];
            string authStr = Request.QueryString["ss"];

            if (!String.IsNullOrEmpty(cidStr))
            {
                string test =hdnUserTZ.Value;
                couponID = Int64.Parse(cidStr);
                initCoupon = brokerDB.GetIssuedCoupon(couponID);
                if (initCoupon != null)
                {
                    Ticket launchTicket = brokerDB.RetrieveIssuedTicket(initCoupon, TicketTypes.LAUNCH_CLIENT, ProcessAgentDB.ServiceGuid);
                    if (launchTicket != null && !launchTicket.isCancelled && !launchTicket.IsExpired())
                    {
                        XmlQueryDoc xDoc = new XmlQueryDoc(launchTicket.payload);

                        //Check that the coupon matches the passcode in the ticket payload
                        string passChk = xDoc.Query("LaunchClient/passcode");
                        string ssChk = xDoc.Query("LaunchClient/ss");
                        if (String.IsNullOrEmpty(passChk) || passChk.CompareTo(initCoupon.passkey) != 0)
                        {
                            buf.AppendLine("Passkey Error");
                            processError(buf);
                        }
                        if (String.IsNullOrEmpty(ssChk) || passChk.CompareTo(authStr) != 0)
                        {
                            buf.AppendLine("ss_key Error");
                            processError(buf);
                        }
                        clientGuid = xDoc.Query("LaunchClient/clientGuid");
                        serverGuid = xDoc.Query("LaunchClient/serverGuid");
                        groupName = xDoc.Query("LaunchClient/groupName");
                        authKey = xDoc.Query("LaunchClient/authorityKey");
                        string userIdStr = xDoc.Query("LaunchClient/userId");
                        userName = xDoc.Query("LaunchClient/userName");
                        if (String.IsNullOrEmpty(authKey))
                        {
                            buf.AppendLine("authority Error");
                            processError(buf);
                        }
                        if (!String.IsNullOrEmpty(userIdStr))
                        {
                            userID = Int32.Parse(userIdStr);
                            if (userID <= 0)
                            {
                                buf.AppendLine("userID invalid value");
                                processError(buf);
                            }
                        }
                        else
                        {
                            buf.AppendLine("userID not specified");
                            processError(buf);
                        }
                        if (!String.IsNullOrEmpty(groupName))
                        {
                            groupID = AdministrativeAPI.GetGroupID(groupName);
                            if (groupID <= 0)
                            {
                                buf.AppendLine("groupName invalid value");
                                processError(buf);
                            }
                        }
                        else
                        {
                            buf.AppendLine("groupName not specified");
                            processError(buf);
                        }
                        startStr = xDoc.Query("LaunchClient/start");
                        durStr = xDoc.Query("LaunchClient/duration");

                        if (!String.IsNullOrEmpty(clientGuid))
                        {
                            clientID = AdministrativeAPI.GetLabClientID(clientGuid);
                            if (clientID > 0)
                            {
                                // do the real work
                                IntTag results = brokerDB.ProcessLaunchClientRequest(initCoupon, clientID, labServerID, groupID, userID, userTZ);
                                
                                // Checks for different status values
                                //TODO Check for Scheduling redirect -- May be checked before we get here
                                if (results.id > -1)
                                {
                                   long sessionID = AdministrativeAPI.InsertUserSession(userID, groupID, clientID, userTZ, Session.SessionID);
                                    //add Session cookie
                                    HttpCookie cookie = new HttpCookie(ConfigurationManager.AppSettings["isbAuthCookieName"], sessionID.ToString());
                                    Response.AppendCookie(cookie);
                                    Response.AddHeader("Access-Control-Allow-Origin", "*");
                                    if ((results.id & LabClient.APPLET_BIT) == LabClient.APPLET_BIT)
                                    {
                                        Session["LoaderScript"] = results.tag;
                                        Response.Redirect("applet2.aspx", true);
                                    }
                                    else if ((results.id & LabClient.REDIRECT_BIT) == LabClient.REDIRECT_BIT)
                                    {
                                        string url = results.tag;
                                        Response.Redirect(url, true);
                                    }
                                    else
                                    {
                                        buf.AppendLine("Client type not supported");
                                    }
                                }
                                else
                                {
                                    buf.AppendLine("Client not found");
                                }
                            }
                            else
                            {
                                buf.AppendLine("client not specified");
                                processError(buf);
                            }

                        }
                    }
                    else
                    {
                        buf.AppendLine("Ticket Error");
                    }
                }
            }
            else
            {
                buf.AppendLine("Missing cid");
            }
            // If the code above does not complete fall through to processError()
            processError(buf);

        }

        private void processError(StringBuilder buf)
        {
            Response.ContentType = "text/plain";
            Response.Write(buf.ToString());
        }

    }

}
