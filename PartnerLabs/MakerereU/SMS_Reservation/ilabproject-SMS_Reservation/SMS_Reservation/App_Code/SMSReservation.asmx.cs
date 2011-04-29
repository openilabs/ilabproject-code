using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
//using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;

using iLabs.Proxies.Ticketing;
using iLabs.Proxies.ISB;
using iLabs.Proxies.LSS;
using iLabs.Proxies.USS;

using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Mapping;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.Web;
//using iLabs.Web.Linq;

namespace iLabs.Generic.TextReservation
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [WebService(Name = "SMSResrevations", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    [WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SMSReservation : WS_ILabCore
    {
          public OperationAuthHeader opHeader = new OperationAuthHeader();


        //this is the timeOfDay Server
        string lsGuid = "TOD-BC2D1D79-7229-4F42-ABF7-E200B05E3730";
         //clientGuid is the interactive Time of Day Client Guid
        string clientGuid = "TOD-12345";
        //the group for the time of day experiment
        string testGroup = "Test_Group";
        //a typical user that was created for testing purposes
        string testUser = "test"; 

         /*** END OF DEXTER Values **/    
        
             
 

        /*** ORIGINAL info
         //The inSBcoupon and outSBcoupon id (20 and 21 respectively were obtained from the
         //service broker database, particulary from the 
         
        string sbGuid = "57371F64A9D24C2F8013E434DA768027";     
        //my service broker Guid


        Coupon inSBcoupon = new Coupon("57371F64A9D24C2F8013E434DA768027", 20, "SendMe");
        Coupon outSBcoupon = new Coupon("57371F64A9D24C2F8013E434DA768027", 21, "GOT_ME");

        string clientGuid = "4CA0A781BE70452C876D76A3061AA690"; 
        //clientGuid is the interactive Time of Day Client Guid that i created as a test client
        string lsGuid = "0E9FD54639F7452690808B03BB9FC315";     
        //this is the default timeOfDay Server
        string smsGuid = "C53CC3DADF034d67978075F3F4E24F0E"; 
        //this is the batched "lab server" that i made which i termed as SMS_Reservation
        string sbURL = "http://10.0.2.26/iLabServiceBroker/iLabServiceBroker.asmx"; //mine
        string testGroup = "Experiment_Group";      
        //the group for the time of day experiment
        string testUser = "test";      
        //a typical user that was created for testing purposes
        ****/

        [WebMethod]
        public string incomingMessage(string newMessage)
        {
            DateTime targetTime;
            /* The rest of these values were not altered at all
             */
            DateTime start = DateTime.UtcNow.AddDays(1.3);
            DateTime end;
            string message = null;
            try
            {
                targetTime = DateTime.Parse(newMessage);
                start = targetTime;
            }
            catch( Exception E){ }
            end = start.AddMinutes(15);
            message = MakeReservation(testUser, testGroup, lsGuid, clientGuid, start, end);
            return message;
        }

        protected string MakeReservation(string userName, string groupName, string labServerGuid,
                string clientGuid, DateTime start, DateTime end)
        {
            int status = -1;
            string message = null;
            if (ProcessAgentDB.ServiceAgent != null && ProcessAgentDB.ServiceAgent.domainGuid != null)
            {
                ProcessAgentDB paDb = new ProcessAgentDB();
                ProcessAgentInfo domainServer = paDb.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.domainGuid);

                InteractiveSBProxy isbProxy = new InteractiveSBProxy();
                isbProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                isbProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                isbProxy.AgentAuthHeaderValue.coupon = domainServer.identOut;
                isbProxy.Url = domainServer.ServiceUrl;

                string[] types = new string[] { TicketTypes.SCHEDULE_SESSION };

                Coupon opCoupon = isbProxy.RequestAuthorization(types, 600, groupName, userName, labServerGuid, clientGuid);
                if (opCoupon != null)
                {
                    TicketIssuerProxy ticketProxy = new TicketIssuerProxy();
                    ticketProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                    ticketProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                    ticketProxy.AgentAuthHeaderValue.coupon = domainServer.identOut;
                    ticketProxy.Url = domainServer.ServiceUrl;
                    //the method call below is one which does not returns a null value
                    //in otherwards the ticket value is not created.
                    Ticket ticketSMS = ticketProxy.RedeemTicket(opCoupon, TicketTypes.SCHEDULE_SESSION, ProcessAgentDB.ServiceGuid);
                    if (ticketSMS != null)
                    {
                        if (ticketSMS.payload != null || ticketSMS.payload.Length > 0)
                        {
                            XmlQueryDoc xdoc = new XmlQueryDoc(ticketSMS.payload);
                            string ussURL = xdoc.Query("MakeReservationPayload/ussURL");
                            UserSchedulingProxy ussProxy = new UserSchedulingProxy();
                            ussProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                            ussProxy.OperationAuthHeaderValue.coupon = opCoupon;
                            ussProxy.Url = ussURL;

                            TimePeriod[] times = ussProxy.RetrieveAvailableTimePeriods(ProcessAgentDB.ServiceAgent.domainGuid, groupName, lsGuid,
                                clientGuid, start, end);
                            // Logic to check for final time
                            DateTime resStart = start;
                            DateTime resEnd = end;

                            message = ussProxy.AddReservation(ProcessAgentDB.ServiceAgent.domainGuid, userName, groupName, lsGuid, clientGuid, resStart, resEnd);
                        }
                    }
                }
                else
                {
                    message = "coupon is null";
                }
            }
            else
            {
                message = "This service is not part of a domain, please contact the administrator!";

            }
            return message;
        }

       
    }
}
