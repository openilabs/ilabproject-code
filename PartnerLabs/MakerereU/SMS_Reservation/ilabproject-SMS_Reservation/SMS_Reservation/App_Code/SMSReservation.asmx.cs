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

       /** DEXTER.MIT.EDU INFO ***/
         // The inSBcoupon and outSBcoupon id  were obtained from the
         //service broker database, particulary from the issued_coupon table

        long inID = 169L;
        long outID = 170L;
        string sbURL = "http://dexter.mit.edu/iLabServiceBroker/iLabServiceBroker.asmx"; //mine
        string sbGuid = "AISB-179CD2BE-41-99CF-EAABB1D90A82";
        //this is the default timeOfDay Server
        string lsGuid = "ATOD-E8190629-5986-4695-8FED-C5E72B354AD8";
         //clientGuid is the interactive Time of Day Client Guid
        string clientGuid = "TOD-12345";
        //the group for the time of day experiment
        string testGroup = "Experiment_Group";
        //a typical user that was created for testing purposes
        string testUser = "test"; 

      

        //The dummy "lab server" termed as TextReservation which will spoof being a Batch LabServer
        string smsGuid = "0E9FD54639F7452690808B03BB9FC315"; 
       
        
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
            /* The rest of these values were not altered at all
             */
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
            if (ProcessAgentDB.ServiceAgent != null && ProcessAgentDB.ServiceAgent.domainGuid != null)
            {
                ProcessAgentDB paDb = new ProcessAgentDB();
                ProcessAgentInfo domainServer = paDb.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.domainGuid);
                Coupon inSBcoupon = new Coupon(sbGuid, inID, "SendMe");
                Coupon outSBcoupon = new Coupon(sbGuid, outID, "GOT_ME");

                InteractiveSBProxy isbProxy = new InteractiveSBProxy();
                isbProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                isbProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                isbProxy.AgentAuthHeaderValue.coupon = domainServer.identIn;
                isbProxy.Url = domainServer.ServiceUrl;

                string[] types = new string[] { TicketTypes.SCHEDULE_SESSION };

                Coupon opCoupon = isbProxy.RequestAuthorization(types, 600, groupName, userName, labServerGuid, clientGuid);
                if (opCoupon != null)
                {
                    TicketIssuerProxy ticketProxy = new TicketIssuerProxy();
                    ticketProxy.AgentAuthHeaderValue = new AgentAuthHeader();
                    ticketProxy.AgentAuthHeaderValue.agentGuid = ProcessAgentDB.ServiceGuid;
                    ticketProxy.AgentAuthHeaderValue.coupon = domainServer.identIn;
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
            }
            else
            {
                message = "This service is not part of a domain, please contact the administrator!";

            }
            return message;
        }

       
    }
}
