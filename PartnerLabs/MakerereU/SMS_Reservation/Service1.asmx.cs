using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

using iLabs.DataTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.Proxies.Ticketing;
using iLabs.Proxies.ISB;
using iLabs.Proxies.LSS;
using iLabs.Proxies.USS;
using iLabs.UtilLib;

namespace TextReservation
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    //[SoapHeader("opHeader", Direction = SoapHeaderDirection.In)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        Coupon inSBcoupon = new Coupon("AISB-179CD2BE-41-99CF-EAABB1D90A82",14,"SendMe");
        Coupon outSBcoupon = new Coupon("AISB-179CD2BE-41-99CF-EAABB1D90A82", 15, "GOT_ME");
        string sbGuid = "AISB-179CD2BE-41-99CF-EAABB1D90A82";
        string clientGuid = "ATSDS-F24ED401-E50A-42D1-8F7F-C47167A48247";
        string lsGuid = "AILS-F8AF2DDB-6F88-484C-A67E-5095E8FA9A8A";
        string smsGuid = "SMS-1234567890";
        string sbURL = "http://dexter.mit.edu/iLabServiceBroker/iLabServiceBroker.asmx";
       string testGroup = "test_group";
        string testUser = "pbailey";
        [WebMethod]
        public string UserInterface()
        {
           // userGroup newUser = new userGroup();
            //string username = "test";
            //string userfeedBack = Convert.ToString(newUser.getUserGroups(username));

            myDoSchedule newUser2 = new myDoSchedule();
            string userfeedback2 = newUser2.doScheduling();
            

            MyMakeReservationClass newUSer3 = new MyMakeReservationClass();
            string userfeedback3 = newUSer3.SMSMakeReservation();
            return  userfeedback2 ;
        }

        [WebMethod]
        public string incomingMessage(string newMessage)
        {
            /*
            //this returns what is in the class of userDictionary
            //for testing purposes only
            userDictionary newUser = new userDictionary();
            newUser.userInterface(newMessage);
            newUser.isaInterface();
            return newUser.replyMessage();
             * */
            DateTime start = DateTime.UtcNow.AddDays(1);
            DateTime end = start.AddMinutes(15);
            string message = null;

            message = MakeReservation(testUser, testGroup, lsGuid, clientGuid, start, end);
            return message;
        }

        protected string MakeReservation(string userName, string groupName, string labServerGuid,
                string clientGuid, DateTime start, DateTime end)
        {
            int status = -1;
            string message = null;
            InteractiveSBProxy isbProxy = new InteractiveSBProxy();
            
            isbProxy.AgentAuthHeaderValue = new AgentAuthHeader();
            isbProxy.AgentAuthHeaderValue.agentGuid = smsGuid;
            isbProxy.AgentAuthHeaderValue.coupon = inSBcoupon; 
            isbProxy.Url = sbURL;

            string[] types = new string[] {TicketTypes.SCHEDULE_SESSION};

            Coupon opCoupon = isbProxy.RequestAuthorization(types, 600, groupName, userName, labServerGuid, clientGuid);
            if (opCoupon != null)
            {
                TicketIssuerProxy ticketProxy = new TicketIssuerProxy();
                ticketProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                ticketProxy.AgentAuthHeaderValue.agentGuid = smsGuid;
                ticketProxy.AgentAuthHeaderValue.coupon = inSBcoupon;
                ticketProxy.Url=sbURL;
                Ticket ticketSMS = ticketProxy.RedeemTicket(opCoupon, TicketTypes.SCHEDULE_SESSION, smsGuid);
                if (ticketSMS != null)
                {
                    if (ticketSMS.payload != null || ticketSMS.payload.Length > 0)
                    {
                        XmlQueryDoc xdoc = new XmlQueryDoc(ticketSMS.payload);
                        string ussURL = xdoc.Query("MakeReservationPayload/ussURL");
                        UserSchedulingProxy ussProxy = new UserSchedulingProxy();
                        ussProxy.OperationAuthHeaderValue.coupon = opCoupon;
                        ussProxy.Url = ussURL;

                        TimePeriod[] times = ussProxy.RetrieveAvailableTimePeriods(sbGuid, groupName, lsGuid,
                            clientGuid, start, end);
                        // Logic to check for final time
                        DateTime resStart = start;
                        DateTime resEnd = end;

                        message = ussProxy.AddReservation(sbGuid, userName, groupName, lsGuid, clientGuid, resStart, resEnd);
                    }
                }
            }
            else
            {
                message = "coupon is null";
            }
            
            
            return message;
        }
    }
}
