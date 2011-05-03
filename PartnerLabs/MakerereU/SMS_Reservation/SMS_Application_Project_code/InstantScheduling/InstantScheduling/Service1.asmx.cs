using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;


namespace InstantScheduling
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string IncomingMessage(string message, string contact)
        {
            FormatMessage SMS = new FormatMessage();
            
            return SMS.SplittingMessage(message, contact);
          
        }
    }
        public class FormatMessage
        {   
            private string telephone;
            private string recievedRawMsg;
            private string username;
            private string labname;
            private DateTime GivenDate;
            private DateTime startTimeRange;
            private DateTime endTimeRange;
            
            private int LabConfID;
            private string LabNm = null;
            private string LabDescpt = null;
            private string ExptGrpN = null;
            private string sbGuid = null;
            private string InPasKeyNm = null;
            private string InPasKyID;
            private string OutPasKyID;
            private string OutPasKyNm = null;
            private string sbURL = null;
            private string ClientNm = null;
            private string ClientGUID = null;
            private string LsName = null;
            private string LsGuid = null;
            private string IMGuid = null;
            private string MinDuratn;
            private string MaxDuratn;

            private string[] SplitMessage = null;
            private string messageKey = null;
            private string TimeRecieved = null;
            private string userReply = null;
            private DateTime StartTimeGiven;
            private DateTime EndTimeGiven;

            public FormatMessage()
            {
             

            }

            public string SplittingMessage(string recievedMessage, string contact)
            {
                //storing the recieved elements 
                telephone = contact;
                recievedRawMsg = recievedMessage;
               
                
                //give the message a key by use of random numbers
                Random r = new Random();
                messageKey = r.Next(10000001, 99999999).ToString();
                //while (DBconnect.KeyChecker(messageKey) == true)
                //{
                //    messageKey = r.Next(10000001, 99999999).ToString();
                //}
                //split the message into an array
                SplitMessage = recievedMessage.Split(' ');

                //check if message has the minimum requirements
                //for now these are username, labname, date and start Time
                //if this is not the case then return an error message
                if (SplitMessage.Length < 4 || SplitMessage.Length > 5)
                {
                    //storing incoming message to database
                   // DBconnect.storeIncoming(contact, recievedRawMsg, messageKey, NowTime(), "null",
                   //     "null", "null", "null");
                    DBconnect.storeIncoming(contact, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));
                       
                    
                    //generating error message for the out going message
                    userReply = DBconnect.errorMessage("MFE01");

                    //storing the out going message to the user
                    DBconnect.storeOutgoing(userReply, messageKey,false, labname,
                        StartTimeGiven.ToString(), EndTimeGiven.ToString(), "MFE01");

                    return userReply;
                }

                else
                {
                    //store the user elements
                    username = SplitMessage[0];
                    labname = SplitMessage[1];
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


                return LabNamePresent(labname);
            }

            private string LabNamePresent(string labname)
           {
               //check in LabConfig if the labname of the configuration is present
             int value =  DBconnect.isLabnamePresent(labname);
             if (value != 5)
             {
                 //allow the user to proceed with the experiment
                 return switchUserOptns();
             }
             else
             {
                 //storing the incoming message
                 DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                 userReply = DBconnect.errorMessage("LRU01");

                 DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                        StartTimeGiven.ToString(), EndTimeGiven.ToString(), "LRU01");


                 
                 //update incoming and out going message tables in the db and reply user
                 return userReply;
             }
           }

            private string switchUserOptns()
            {
                //deal with the start time first
                string res = dateChecker(SplitMessage[2], SplitMessage[3]);
                
                if(res.Equals("failedTotally"))
                {
                    //the date was not successfully formatted
                    //provide the date format section and remind him that it should be uptodate
                    
                    //update the database
                    //storing incoming message
                    DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                    userReply = DBconnect.errorMessage("SDT01");

                    DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                        StartTimeGiven.ToString(), EndTimeGiven.ToString(), "SDT01");

                    return userReply;
                }
                else if(res.Equals("formatOny"))
                {
                    //the date and start time was not uptodate
                    //update DB
                    //storing the incoming message
                    DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                    userReply = DBconnect.errorMessage("SDT02");

                    DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                        StartTimeGiven.ToString(), EndTimeGiven.ToString(), "SDT02");

                    return userReply;
                }

                else
                {
                    //update the startTimeRange
                    startTimeRange = Convert.ToDateTime(dateChecker(SplitMessage[2], SplitMessage[3]));

                    if(SplitMessage.Length == 4)
                    {
                        //add like 4 hours to create the end time
                        //then call on the method which gives the specific time to use for scheduling
                        //which calls on the scheduling method
                        startTimeRange = Convert.ToDateTime(SplitMessage[2] + " " + SplitMessage[3]);
                        endTimeRange = startTimeRange.AddHours(4.0);
                        return specificTime(startTimeRange, endTimeRange);
                    }
                
                    else if(SplitMessage.Length == 5)
                    {
                        //check if the endtime is fine and move on to ensure it aint before the finish time
                        string resE = dateChecker(SplitMessage[2], SplitMessage[4]);
                        if(resE.Equals("failedTotally"))
                        {
                            //the date was not successfully formatted
                            //provide the date format section and remind him that it should be uptodate
                            
                            //update the database
                            //update incoming message table
                            DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                            userReply = DBconnect.errorMessage("EDT01");

                            DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                                StartTimeGiven.ToString(), EndTimeGiven.ToString(), "EDT01");

                            return userReply;
                        }
                        else if (resE.Equals("formatOny"))
                        {
                            //the date and start time was not uptodate
                            //update DB
                            //updating incoming message
                            DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                            userReply = DBconnect.errorMessage("EDT02");

                            DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                                StartTimeGiven.ToString(), EndTimeGiven.ToString(), "EDT02");

                            return userReply;
                        }

                        else
                        {
                            endTimeRange = Convert.ToDateTime(dateChecker(SplitMessage[2], SplitMessage[4]));
                            if (endTimeRange <= startTimeRange)
                            {
                                //update the database with the possible errors that will arise in the system
                                //we could go on and do the scheduling and point it out to the user after
                                //updating the incoming message
                                DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                                userReply = DBconnect.errorMessage("SDT00");


                                DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                                    StartTimeGiven.ToString(), EndTimeGiven.ToString(), "SDT00");

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

                return "Not sure whats up!";

            }

            private string dateChecker(string date, string time)
           {
               try
               {
               //check if the format input is success then procede
               string[] messageElementsDate = date.Split('/','.','-');
               string DateInWords = messageElementsDate[0] + "/" + messageElementsDate[1] + "/" + messageElementsDate[2];
               
               string[] messageElementsTime = time.Split(':', ';');
               string TimeInWords = messageElementsTime[0] +":"+ messageElementsTime[1];


               string ThisDateDayWord = DateInWords + " " + TimeInWords;
               DateTime ThisDate = Convert.ToDateTime(ThisDateDayWord);
               if(ThisDate >= DateTime.Now)
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
           
            private bool LabConfigurationVariables(string sLs)
           {
               try
               {
                   //used to pick Guids and other variables required for scheduling
                   //from the database
                   //these are the ones declared above

                   //DataClassesDataContext db = new DataClassesDataContext();
                   //        //LabConfiguration v = db.LabConfigurations.Where(s => s.LabConfigurationID > 34).First();
                   //        //v.LabDescription = "wefwedf";
                   //        //dx.SubmitChanges();

                   InstantMessageDBDataContext db = new InstantMessageDBDataContext();

                   var value = from l in db.LabConfigurations
                               where l.LabName == labname
                               select l;

                   LabConfiguration lb = db.LabConfigurations.Where(s => s.LabName == labname).First();
                   LabDescpt = lb.LabDescription;
                   sbGuid = lb.ServiceBrokerGUID;
                   ClientGUID = lb.ClientGuid;
                   ExptGrpN = lb.ExperimentGroupName;
                   sbURL = lb.ServiceBrokerURL;

                   InPasKeyNm = lb.InstantMessageInPasskey;
                   InPasKyID = lb.InstantMessageInID.ToString();
                   OutPasKyNm = lb.InstantMessageOutPassKey;
                   OutPasKyID = lb.InstantMessageOutID.ToString();

                   LsGuid = lb.LabServerGuid;
                   MinDuratn = lb.MinimumLabDuration.ToString();
                   MaxDuratn = lb.MaximumLabDuration.ToString();
                   IMGuid = lb.InstantMessageGUID;

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
                if (LabConfigurationVariables(labname) == false)
                {
                    //update the database and inform the user what happened of the error
                    DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                    userReply = DBconnect.errorMessage("DBF01");

                    DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                        StartTimeGiven.ToString(), EndTimeGiven.ToString(), "DBF01");

                    return "Unfortunately we failed to connect to the database, sorry for any inconvinences";
                }
                else
                {
                    //give the specific time period and access the bridge to the ISA
                    Double Dur = Convert.ToDouble(MaxDuratn);

                    DBconnect.storeIncoming(telephone, recievedRawMsg, messageKey, Convert.ToString(DateTime.UtcNow),
                        username, labname, Convert.ToString(startTimeRange), Convert.ToString(endTimeRange));

                    userReply = DBconnect.errorMessage("DEF00");

                    DBconnect.storeOutgoing(userReply, messageKey, false, labname,
                        StartTimeGiven.ToString(), EndTimeGiven.ToString(), "DEF00");

                    return userReply;
                }

            }

            private string NowTime()
            {
                return Convert.ToString(DateTime.UtcNow);
            }

           }

       
        public static class DBconnect
        {
            //this class connects to the database whenever a user wants somethign out of it

            public static bool KeyChecker(string messageKey)
            {
                InstantMessageDBDataContext db = new InstantMessageDBDataContext();
                InComingMessage InCom = db.InComingMessages.Where(s => s.MessageKey == messageKey).First();
                if (InCom.LabConfigurationID > 0)
                    return true;
                else
                    return false;
            }
                                  
           
            public static int isLabnamePresent(string labnameReq)
            {
                try
                {
                    InstantMessageDBDataContext db = new InstantMessageDBDataContext();
                    //string labInDB = null;
                    //checks if the labname requested by the user is present in the system
                    LabConfiguration lb = db.LabConfigurations.Where(s => s.LabName == labnameReq).First();
                    if (lb.LabConfigurationID > 0)
                    {
                        
                        return lb.LabConfigurationID;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 5;
                }
            }

            public static void storeIncoming(string contact, string RwRecievedMsg,
                string MessageKey, string TimeRecieved, string username, string labname,
                string startTimeRange, string EndTimeRange)
            {
                InstantMessageDBDataContext db = new InstantMessageDBDataContext();
                InComingMessage InComing = new InComingMessage();

                InComing.Telephone = contact;
                InComing.RwRecievedMsg = RwRecievedMsg;
                InComing.MessageKey = MessageKey;
                InComing.TimeReceived = TimeRecieved;
                InComing.username = username;
                InComing.LabConfigurationID = isLabnamePresent(labname);
                InComing.StartTimeRange = startTimeRange;
                InComing.EndTimeRange = EndTimeRange;

                db.InComingMessages.InsertOnSubmit(InComing);
                db.SubmitChanges();

            }

            public static void storeOutgoing(string RwSentMsg, string MessageKey,
                 bool IsScheduled, string labname,
                string GivenStartTimeDB, string GivenEndTimeDB, string codeERR)
            {
               
                InstantMessageDBDataContext db = new InstantMessageDBDataContext();
                OutGoingMessage OutGo = new OutGoingMessage();

                OutGo.RwSentMsg = RwSentMsg;
                OutGo.MessageKey = MessageKey;
                OutGo.TimeAndDateSent = Convert.ToString(DateTime.UtcNow);
                OutGo.IsScheduled = IsScheduled;
                OutGo.GivenStartTime = Convert.ToDateTime(GivenStartTimeDB);
                OutGo.GivenEndTime = Convert.ToDateTime(GivenEndTimeDB);
                OutGo.LabConfigurationID = isLabnamePresent(labname);
                OutGo.codeError = codeERR;
                

                db.OutGoingMessages.InsertOnSubmit(OutGo);
                db.SubmitChanges();

            }

            public static string errorMessage(string errorCode)
            {
                try
                {
                    InstantMessageDBDataContext db = new InstantMessageDBDataContext();
                    ErrorDescription err = db.ErrorDescriptions.Where(s => s.codeError == errorCode).First();
                    return err.ShortDescription + " " + err.PossibleSoln;
                }
                catch
                {
                    return "Problem with error description procedures";
                }


            }
       }

        
    
    
   
}
