using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.Web;

//required for scheduling
using iLabs.DataTypes.SchedulingTypes;
using iLabs.Proxies.Ticketing;
using iLabs.Proxies.ISB;
using iLabs.Proxies.USS;
using System.Xml.Serialization;

using iLabs.ServiceBroker.Authentication;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Administration;


/// <summary>
/// Summary description for SMSReservation
/// </summary>
[XmlType(Namespace = "http://ilab.mit.edu/iLabs/type")]
[WebService(Name = "SMSResrevations", Namespace = "http://ilab.mit.edu/iLabs/Services")]
[WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services")]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class SMSReservation : iLabs.Web.WS_ILabCore
{

    public SMSReservation()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    
    [WebMethod]
    public string IncomingMessage(string message, string contact)
    {
        FormatMessage SMS = new FormatMessage();

        string response =  SMS.SplittingMessage(message, contact);

        Context.Response.Output.Write(response);
        Context.Response.End();
        return string.Empty;

    }

}

public class FormatMessage : System.Web.Services.WebService
{
    //These are the parameters actually sent by the user during scheduling
    private string telephone = null;    // the telephone contact used by the user while scheduling
    private string recievedRawMsg = null; // the actual message sent by the user
    private string username = null;
    private string labname = null;
    private DateTime GivenDate = DateTime.MinValue.AddYears(1753); //default date and time..
    private DateTime startTimeRange = DateTime.MinValue.AddYears(1753); //default date and time
    private DateTime endTimeRange = DateTime.MinValue.AddYears(1753); //default date and time

    //parameters earlier provided when administrator 
    //registers a lab to be scheduled for via sms
    private int LabConfID; //configuration ID representing a lab requested for
    private string labAcronym = null; //the name the student uses in scheduling the lab by sms, should be short
    private string ExptGrpN = null; //the experiment group name attached to the lab
    private string sbGuid = null; //the service broker guid the user and lab is registered to
    private string sbURL = null; //the service broker url
    private string ClientGUID = null; //lab client guid being registered for
    private string LsGuid = null; //lab server Guid being registered for
    private double MaxDuratn; //maximum duration for the lab to be done as assigned by the user

    //parameters that are being run through this class
    private string[] SplitMessage = null;
    private string messageKey = null;
    private string TimeRecieved = null;
    private string userReply = null;
    private DateTime StartTimeGiven = DateTime.MinValue.AddYears(1753);
    private DateTime EndTimeGiven = DateTime.MinValue.AddYears(1753);

    /// <summary>
    /// constructor
    /// </summary>
    public FormatMessage()
    {


    }

    /// <summary>
    /// Splits the message and asigns more like a session key
    /// </summary>
    /// <param name="recievedMessage">The message Sent by the user</param>
    /// <param name="contact">The user contact number</param>
    /// <returns>returns result from userAuthorize() unless otherwise</returns>
    public string SplittingMessage(string recievedMessage, string contact)
    {
        //storing the recieved elements 
        telephone = contact;
        recievedRawMsg = recievedMessage.TrimEnd();


        //give the message a key by use of random numbers
        //need to check if the given key is already available
        key:
        Random r = new Random();
        messageKey = r.Next(10000001, 99999999).ToString();
        //if (LinqConnections.KeyChecker(messageKey) == true)
        //{
        //    goto key;
        //}

        //split the message into an array basing on the spaces
        SplitMessage = recievedRawMsg.Split(' ');

        
        /*The message needs to be either 4 or 5 words/elements between the spaces
         * If not this if statement is fulfilled and user recieves an approriate error message
         */
        if (SplitMessage.Length < 4 || SplitMessage.Length > 5)
        {
            LinqConnections.storeIncoming(contact, recievedRawMsg, messageKey, DateTime.UtcNow,
                username, labAcronym, startTimeRange, endTimeRange);


            //generating error message for the out going message
            userReply = LinqConnections.errorMessage("MFE01");

            //storing the out going message to the user
            LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
               StartTimeGiven, EndTimeGiven, "MFE01");

            return userReply;
        }


        /* The message is 4 or 5 characters long
         * Username, and labAcronym are picked up from here
         * These are the 1st and 2nd elements in the message respectively
         * Calls on the userAuthorize()
         */
        else
        {
            //store the user elements
            username = SplitMessage[0];
            labAcronym = SplitMessage[1].ToLower();
            //return the next method to be used
            return userAuthorize(username);
        }
    }

    private string userAuthorize(string username)
    {
        //connect to the ilabs ISB database and check if user is actually authorized to schedule
        //use the if statement to check
        //also check if he lies in the group he/ she said was in
        //if true, call on the LabNamePresent()
        //else  - update databases and return  reply:"username not present. the username used is not being recoganized by the system"

        //if (ISAConnect.checkUser(username) == true)
        //{
        //    return LabNamePresent(labAcronym);
        //}

        //else
        //{
        //    return null;
        //}
        return LabNamePresent(labAcronym);
    }

    /// <summary>
    /// Checks if the labAcronym given by the user is actually present
    /// Requires connection to the database
    /// </summary>
    /// <param name="labAcronym">second element as input by the user</param>
    /// <returns>calls the switchUserOptns if true and error otherwise</returns>
    private string LabNamePresent(string labAcronym)
    {
        //check in LabConfig if the labname of the configuration is present
        int value = LinqConnections.isLabnamePresent(labAcronym);
        if (value != 1)
        {
            //allow the user to proceed with the experiment
            return switchUserOptns();
        }
        else
        {
            //storing the incoming message
            LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                   username, labAcronym, startTimeRange, endTimeRange);

            userReply = LinqConnections.errorMessage("LRU01");

            LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                   StartTimeGiven, EndTimeGiven, "LRU01");



            //update incoming and out going message tables in the db and reply user
            return userReply;
        }
    }

    private string switchUserOptns()
    {
        try
        {
            //deal with the start time first
            string res = resources.dateChecker(SplitMessage[2], SplitMessage[3]);

            if (res.Equals("failedTotally"))
            {
                //the date was not successfully formatted
                //provide the date format section and remind him that it should be uptodate

                //update the database
                //storing incoming message
                LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labAcronym, startTimeRange, endTimeRange);

                userReply = LinqConnections.errorMessage("SDT01");

                LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                    StartTimeGiven, EndTimeGiven, "SDT01");

                return userReply;
            }
            else if (res.Equals("formatOny"))
            {
                //the date and start time was not uptodate
                //update DB
                //storing the incoming message
                LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labAcronym, startTimeRange, endTimeRange);

                userReply = LinqConnections.errorMessage("SDT02");

                LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                    StartTimeGiven, EndTimeGiven, "SDT02");

                return userReply;
            }

            else
            {
                //update the startTimeRange
                startTimeRange = Convert.ToDateTime(resources.dateChecker(SplitMessage[2], SplitMessage[3]));

                if (SplitMessage.Length == 4)
                {
                    //add like 4 hours to create the end time
                    //then call on the method which gives the specific time to use for scheduling
                    //which calls on the scheduling method
                    startTimeRange = Convert.ToDateTime(SplitMessage[2] + " " + SplitMessage[3]);
                    endTimeRange = startTimeRange.AddHours(4.0);
                    return specificTime(startTimeRange, endTimeRange);
                }

                else if (SplitMessage.Length == 5)
                {
                    //check if the endtime is fine and move on to ensure it aint before the finish time
                    string resE = resources.dateChecker(SplitMessage[2], SplitMessage[4]);
                    if (resE.Equals("failedTotally"))
                    {
                        //the date was not successfully formatted
                        //provide the date format section and remind him that it should be uptodate

                        //update the database
                        //update incoming message table
                        LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labAcronym, startTimeRange, endTimeRange);

                        userReply = LinqConnections.errorMessage("EDT01");

                        LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                            StartTimeGiven, EndTimeGiven, "EDT01");

                        return userReply;
                    }
                    else if (resE.Equals("formatOny"))
                    {
                        //the date and start time was not uptodate
                        //update DB
                        //updating incoming message
                        LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labAcronym, startTimeRange, endTimeRange);

                        userReply = LinqConnections.errorMessage("EDT02");

                        LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                            StartTimeGiven, EndTimeGiven, "EDT02");

                        return userReply;
                    }

                    else
                    {
                        endTimeRange = Convert.ToDateTime(resources.dateChecker(SplitMessage[2], SplitMessage[4]));
                        if (endTimeRange <= startTimeRange)
                        {
                            //update the database with the possible errors that will arise in the system
                            //we could go on and do the scheduling and point it out to the user after
                            //updating the incoming message
                            LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labAcronym, startTimeRange, endTimeRange);

                            userReply = LinqConnections.errorMessage("SDT00");


                            LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                                StartTimeGiven, EndTimeGiven, "SDT00");

                            return userReply;
                        }
                        else
                        {
                            //call on the method which gives the specific time and range
                            return specificTime(startTimeRange, endTimeRange);
                        }
                    }
                }
            }

            return null;
        }
        catch
        {

            LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
        username, labname, startTimeRange, endTimeRange);

            userReply = LinqConnections.errorMessage("EDT02");

            LinqConnections.storeOutgoing(userReply , messageKey, false, labname,
                StartTimeGiven, EndTimeGiven, "EDT02");

            return userReply;
        }
    }

    private bool LabConfigurationVariables(string sLs)
    {
        try
        {
            //used to pick Guids and other variables required for scheduling
            //from the database
            //these are the ones declared above


            DataClassesDataContext db = new DataClassesDataContext();

            var value = from l in db.LabConfigurations
                        where l.applicationCallName == labAcronym
                        select l;

            LabConfiguration lb = db.LabConfigurations.Where(s => s.applicationCallName == labAcronym).First();

            //picking elements required for scheduling a user
            sbGuid = lb.ServiceBrokerGUID.Trim();
            ClientGUID = lb.ClientGuid.Trim();
            ExptGrpN = lb.ExperimentGroupName;
            sbURL = lb.ServiceBrokerURL.Trim();
            MaxDuratn = Convert.ToDouble(lb.MaximumLabDuration);
            LsGuid = lb.LabServerGuid.Trim();

            return true;
        }
        catch
        {
            return false;
        }

    }

    private string specificTime(DateTime GivenStartTime, DateTime GivenEndTime)
    {

        //put in the logic to check the available time slots to carry out a schedule
        //first, ensure that all the elements have been successfully inserted from the database

        //If the database has failed to conntect then we be honest and tell the user
        if (LabConfigurationVariables(labAcronym) == false)
        {
            //update the database and inform the user what happened of the error
            LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                username, labAcronym, startTimeRange, endTimeRange);

            userReply = LinqConnections.errorMessage("DBF01");

            LinqConnections.storeOutgoing(userReply, messageKey, false, labAcronym,
                StartTimeGiven, EndTimeGiven, "DBF01");

            return userReply;
        }
            //otherwise lets proceed to do connect to the ISA
        else
        {
            //give the specific time period and access the bridge to the ISA
            Double Dur = Convert.ToDouble(MaxDuratn);

            //check if these times are available
            //check if the there is an available timeslot starting from the start of the timeRange
            StartTimeGiven = startTimeRange;
            EndTimeGiven = StartTimeGiven.AddMinutes(Dur);
        Loop:
            try
            {

                string message = ISAConnect.MakeReservation(username, ExptGrpN, LsGuid, ClientGUID, StartTimeGiven, EndTimeGiven);
                
                //when the time slot requested for is first taken
                //use the recieved message and pick out the recommended duration for starting and use it 
                //as the start time
                if(message.Contains("not available") && message.Contains("adjust"))
                {
                   //to get the minutes to be added, split the message first
                    //then from the array, we pick the value and convert it to double
                    string[] ISAmessageSplit = message.Split(' ');
                    double nxtAvail = Convert.ToDouble(ISAmessageSplit[26]); //can also be [17]

                    StartTimeGiven = StartTimeGiven.AddMinutes(nxtAvail);
                    EndTimeGiven = StartTimeGiven.AddMinutes(Dur);

                    //after adjusting, check if the new times given are within the user time range
                    //1. When the starttimeGiven is beyond the The endTimeRange
                    //   Inform the user that he cannot be scheduled and should try another time range
                    if(StartTimeGiven >= endTimeRange)
                    {
                        try
                        {
                            //started error format has not been created for this issue as yet.
                            userReply = "Unfortunately the lab durations are full in this time Range. Please choose another time range";

                            LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                        username, labname, startTimeRange, endTimeRange);

                            LinqConnections.storeOutgoing(userReply, messageKey, true, labname,
                        StartTimeGiven, EndTimeGiven, userReply);

                            return userReply + " while connecting to the ISA";
                        }
                        catch
                        {

                            LinqConnections.storeOutgoing(userReply, messageKey, true, labname,
                                StartTimeGiven, EndTimeGiven, "DEF00");

                            return userReply;
                        }
                    }

                    //2.when the startTimeRange is within the EndtimeGiven but the endTimeGiven is outside.
                    //in this case we just schedule the user but tell him/her of the situation
                    else if (StartTimeGiven < endTimeRange && EndTimeGiven >= endTimeRange)
                    {
                        //go on and schedule the user
                        goto Loop;

                    }
                    
                    //when we cant figure out what happened!!
                    else 
                    {
                        goto Loop;
                        LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labname, startTimeRange, endTimeRange);

                        userReply = LinqConnections.errorMessage("DEF00");

                        LinqConnections.storeOutgoing(userReply, messageKey, false, labname,
                            StartTimeGiven, EndTimeGiven, "DEF00");

                        return userReply;
                    }

            }
                //when the experiment is not availabe to the user at that time...
                else if (message.Contains("your group during this time"))
                {
                    LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                    username, labname, startTimeRange, endTimeRange);

                    userReply = message + "Contact Administrator for more details";

                    LinqConnections.storeOutgoing(userReply, messageKey, false, labname,
                        StartTimeGiven, EndTimeGiven, userReply);

                    return userReply;
                }

                //when the user has actually been scheduled
                else if (message.Contains("confirmed successfully"))
                {
                    try
                    {
                        LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
                        username, labname, startTimeRange, endTimeRange);
                        string format = "f";
                        //userReply = "sucess "+StartTimeGiven.ToString();
                        userReply = "Reservation confirmed successfully for: " + 
                            username  + " for the Period: " + StartTimeGiven.ToString(format) + " to " + EndTimeGiven.ToString(format) + ". Thank you. iLabs@MAK";

                        LinqConnections.storeOutgoing(userReply, messageKey, true, labname,
                            StartTimeGiven, EndTimeGiven, userReply);
                    }
                        //when for some reason we cannot store into the databases but the schedule has been done

                    catch
                    {

                        LinqConnections.storeOutgoing(userReply, messageKey, true, labname,
                            StartTimeGiven, EndTimeGiven, "DEF00");

                        return userReply;
                    }

                    return userReply;
                }

                //when we cant figure out what might have happened!!

                LinqConnections.storeIncoming(telephone, recievedRawMsg, messageKey, DateTime.UtcNow,
            username, labname, startTimeRange, endTimeRange);

                userReply = LinqConnections.errorMessage("DEF00");

                LinqConnections.storeOutgoing(userReply, messageKey, false, labname,
                    StartTimeGiven, EndTimeGiven, "DEF00");

                return userReply;
         }

            catch
            {
                userReply = LinqConnections.errorMessage("DEF00");

                LinqConnections.storeOutgoing(userReply, messageKey, true, labname,
                    StartTimeGiven, EndTimeGiven, "DEF00");

                return userReply;
            }
        }

    }


}
    /// <summary>
    /// Class that is used by the application to connect the database iLab_SMS
    /// Databases connections used in this case are based on Linq to SQL
    /// </summary>
    public static class LinqConnections
    {
       
        /// <summary>
        /// Checks if the message key assigned to an incoming message is valied
        /// valied message keys are the ones which are unique
        /// if a message key is present, the application returns a true
        /// else its a false
        /// </summary>
        /// <param name="messageKey">message key being tasted or checked if available</param>
        /// <returns></returns>
        public static bool KeyChecker(string messageKey)
        {
            try
            {
                DataClassesDataContext db = new DataClassesDataContext();
                InComingMessage InCom = db.InComingMessages.Where(s => s.MessageKey == messageKey).First();
                if (InCom.LabConfigurationID > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// checking if the labname acronym given by the user was the one actually used by the application
        /// requires connection to the database
        /// Note: ID no. 1 is the default value returned when no lab is present
        /// </summary>
        /// <param name="labnameReq">acronynm of the lab to be scheduled for</param>
        /// <returns>returns a value greater than 1 if present and a 1 if not present</returns>
        public static int isLabnamePresent(string labnameReq)
        {
            try
            {
                DataClassesDataContext db = new DataClassesDataContext();
                //string labInDB = null;
                //checks if the labname requested by the user is present in the system
                LabConfiguration lb = db.LabConfigurations.Where(s => s.applicationCallName == labnameReq).First();
                if (lb.LabConfigurationID > 1)
                {

                    return lb.LabConfigurationID;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Stores every incoming message that reaches the application
        /// Values can be null depending on the level of how the message was decoded
        /// </summary>
        /// <param name="contact"> The contact of the user</param>
        /// <param name="RwRecievedMsg">The actual message recieved by the user</param>
        /// <param name="MessageKey">The key assigned to the incoming message</param>
        /// <param name="TimeRecieved">The time when the message has been recieved</param>
        /// <param name="username">The username of the user(is nullable)</param>
        /// <param name="labAcronym">LabAcronym given to the user to schedule for a given lab</param>
        /// <param name="startTimeRange">The star time range within the user would want to do the lab</param>
        /// <param name="EndTimeRange">The End time range within which the lab should be scheduled</param>
        public static void storeIncoming(string contact, string RwRecievedMsg,
            string MessageKey, DateTime TimeRecieved, string username, string labAcronym,
            DateTime startTimeRange, DateTime EndTimeRange)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            InComingMessage InComing = new InComingMessage();

            InComing.Telephone = contact;
            InComing.RwRecievedMsg = RwRecievedMsg;
            InComing.MessageKey = MessageKey;
            InComing.TimeReceived = TimeRecieved;
            InComing.username = username;
            InComing.LabConfigurationID = isLabnamePresent(labAcronym);
            InComing.StartTimeRange = startTimeRange;
            InComing.EndTimeRange = EndTimeRange;

            db.InComingMessages.InsertOnSubmit(InComing);
            db.SubmitChanges();

        }

        /// <summary>
        /// Storing the message replied to the user
        /// </summary>
        /// <param name="RwSentMsg">The Raw message sent to user</param>
        /// <param name="MessageKey"> The key of the message; similar to the incoming message key</param>
        /// <param name="IsScheduled">boolen comfirming if the message has been scheduled or not</param>
        /// <param name="labAcronym">The lab being scheduled for</param>
        /// <param name="GivenStartTimeDB">The given startTime if lab was scheduled</param>
        /// <param name="GivenEndTimeDB">The given Endtime of the given lab</param>
        /// <param name="codeERR">The error incase of not succesfully sent</param>
        public static void storeOutgoing(string RwSentMsg, string MessageKey,
             bool IsScheduled, string labAcronym,
            DateTime GivenStartTimeDB, DateTime GivenEndTimeDB, string codeERR)
        {

            DataClassesDataContext db = new DataClassesDataContext();
            OutGoingMessage OutGo = new OutGoingMessage();

            OutGo.RwSentMsg = RwSentMsg;
            OutGo.MessageKey = MessageKey;
            OutGo.TimeAndDateSent = DateTime.UtcNow;
            OutGo.IsScheduled = IsScheduled;
            OutGo.GivenStartTime = GivenStartTimeDB;
            OutGo.GivenEndTime = GivenEndTimeDB;
            OutGo.LabConfigurationID = isLabnamePresent(labAcronym);
            OutGo.codeError = codeERR;


            db.OutGoingMessages.InsertOnSubmit(OutGo);
            db.SubmitChanges();

        }
        /// <summary>
        /// Retrieves an error message for the corresponding code of the error..
        /// </summary>
        /// <param name="errorCode">The errorCode for a particular error in the message</param>
        /// <returns>Resturns a short description of what has happened and a possible solution</returns>
        public static string errorMessage(string errorCode)
        {
            try
            {
                DataClassesDataContext db = new DataClassesDataContext();
                ErrorDescription err = db.ErrorDescriptions.Where(s => s.codeError == errorCode).First();
                return err.ShortDescription.TrimEnd() + " " + err.PossibleSoln;
            }
            catch
            {
                return "Problem with error description procedures";
            }


        }
    }


    /// <summary>
    /// Class that helps us to connect to the sms application to the ISA
    /// </summary>
    public class ISAConnect 
    {

        /// <summary>
        /// Method that does the actual make reservation for the user. 
        /// makes the basis of the parameters from the message and the application database
        /// </summary>
        /// <param name="userName"> the user who is scheduling for the lab; must be registered with the service broker</param>
        /// <param name="groupName">the group name authorized to do a lab</param>
        /// <param name="labServerGuid">lab server which holds the lab that does the actual scheduling</param>
        /// <param name="clientGuid">Guid of the lab being registered for</param>
        /// <param name="start">Start time of a particular period scheduled for</param>
        /// <param name="end">End time of a particular period being scheduled for</param>
        /// <returns>returns a message string that the application bases on for to give the feedback to the user</returns>
        public static string MakeReservation(string userName, string groupName, string labServerGuid,
                    string clientGuid, DateTime start, DateTime end)
        {
            string lsGuid = null;
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

                Coupon opCoupon = isbProxy.RequestAuthorization(types, 600, 			userName, groupName,  labServerGuid, clientGuid);
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
                            string ussURL = xdoc.Query("ScheduleSessionPayload/ussURL");
                            UserSchedulingProxy ussProxy = new UserSchedulingProxy();
                            ussProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                            ussProxy.OperationAuthHeaderValue.coupon = opCoupon;
                            ussProxy.Url = ussURL;

                            TimePeriod[] times = ussProxy.RetrieveAvailableTimePeriods(ProcessAgentDB.ServiceAgent.domainGuid, groupName, labServerGuid,
                                clientGuid, start, end);
                            // Logic to check for final time
                            DateTime resStart = start;
                            DateTime resEnd = end;

                            message = ussProxy.AddReservation(ProcessAgentDB.ServiceAgent.domainGuid, userName, groupName, labServerGuid, clientGuid, resStart, resEnd);
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

        public static bool checkUser(string username)
        {
            AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
            int userID = -1;
            try
            {
                userID = wrapper.GetUserIDWrapper(username);
                if (userID > 0)
                    return true;
                else
                    return false;

            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// class that gives some utility methods for the application to operate
    /// 
    /// </summary>
    public static class resources
    {
        /// <summary>
        /// The default date and time
        /// This acts like a null value to a string
        /// </summary>
        /// <returns>Date and time considered 'null' as per the system</returns>
        public static DateTime defaultTime()
        {
            return Convert.ToDateTime("01/01/1753 12:00AM").ToUniversalTime();
        }

        /// <summary>
        /// takes in date and time as two seperate strings
        /// it then comforms that these two parameters make a standard dateTime
        /// and comforms that these dates given are up to date
        /// </summary>
        /// <returns>a string that contains the given date and time</returns>
        public static string dateChecker(string date, string time)
        {
            try
            {
                //check if the format input is success then procede
                string[] messageElementsDate = date.Split('/', '.', '-');
                string DateInWords = messageElementsDate[0] + "/" + messageElementsDate[1] + "/" + messageElementsDate[2];
                //DateTime day = Convert.ToDateTime(messageElementsDate[0]).Day;
                //assingning each element a location
                string day = messageElementsDate[0];
                string month = messageElementsDate[1];
                string year = messageElementsDate[2];
                string UtcDateInWords = month + "/" + day + "/" + year;


                string[] messageElementsTime = time.Split(':', ';');
                string TimeInWords = messageElementsTime[0] + ":" + messageElementsTime[1];

                //string format = "dd/mm/yyyy hh:mm";
                string ThisDateDayWord = UtcDateInWords + " " + TimeInWords;
                DateTime ThisDate = Convert.ToDateTime(ThisDateDayWord);

                if (ThisDate >= DateTime.Now)
                {
                    return ThisDate.ToString();

                }
                else
                {
                    return "formatOnly";
                }
            }
            catch
            {
                return "failedTotally";
            }
        }
    }




