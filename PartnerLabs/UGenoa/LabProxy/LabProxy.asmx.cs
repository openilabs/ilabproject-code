using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using iLabs.Web;
using LabProxy.Data;
using System.Data.SqlClient;
using System.Configuration;
using LabProxy.iLabSbServiceReference;
using System.ServiceModel;
using System.Xml;


namespace iLabs.LabProxy
{
    /// <summary>
    /// This Web Service is used to retrieve the reservation info.
    /// It is an extended Process agent which fosters the connection
    /// of different laboratory types.
    /// Next to the connection the WS synchronizes also the time between
    /// iLab and the proper Laboratory Server.
    /// </summary>
    [WebService(Namespace = "http://ilab.mit.edu/iLabs/Services")]
    [XmlType(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [WebServiceBinding(Name = "LabProxyWebService", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    [WebServiceBinding(Name = "IProcessAgent", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    [WebServiceBinding(Name = "IProxy", Namespace = "http://ilab.mit.edu/iLabs/Services")]
    public class LabProxy : WS_ILabCore
    {
        /// <summary>
        /// The WS needs the passkey of the current reservation and the timestamp 
        /// of the labserver to synchronize the different systems.
        /// The response is an XML information providing the reservation details
        /// (Reservation Expiration - Experiment ID - UserGroup)
        /// In the case that the current request is not within the reserved timeslot,
        /// null will be returned.
        /// </summary>
        /// <param name="passkey">
        /// Obtained reservation ID from iLab passed through the redirect page to the laboratory
        /// </param>
        /// <param name="labServerTime">
        /// TimeStamp of the laboratory as a string in the format Y-m-dTH:i:s.uZ  --> 2010-07-06T13:41:15.000000Z
        /// </param>
        /// <returns>
        /// Returns the reservation info associated to the provided passkey or null if
        /// the passkey is not valid or not within the reserved timeslot.
        /// Data are encoded as an XML serialized string whith the following structure:
        /// <Reservation>
        ///     <ReservationExpiration>Datetime</ReservationExpiration>
        ///     <GroupName>group of the user</GroupName>
        ///     <ExperimentID>Experiment execution id</ExperimentID>
        /// </Reservation>
        /// ReservationExpiration represents the time when an experiment 
        /// has to be terminated. REMEMBER: This information will be send only if the user
        /// is within the time slot or after!
        /// </returns>
        [WebMethod(Description = "This Web Method is used to retrieve the reservation info.")]
        public string GetReservationInfo(string passkey, string labServerTime)
        {
            DateTime timeNow = DateTime.Now;
            DateTime timeLabServer = Convert.ToDateTime(labServerTime);

            string payloadStr = "";
            Reservation actualReservation = new Reservation();
            // Make database connection with settings from web.config

            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["sqlConnection"]);

            try
            {

                #region retrieves reserv. info (fix)

                // open connection
                con.Open();
                // query payload from actual reservation
                string strSQL = @"
                SELECT Payload
                FROM Coupon c, Ticket t
                WHERE c.Passkey = @passkey and t.Coupon_ID = c.Coupon_ID
                ";
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.Parameters.AddWithValue("@passkey", passkey);
                payloadStr = Convert.ToString(cmd.ExecuteScalar());

                #endregion

                //webservice to get SB time
                #region calculate time diff between SB and labproxy (could be done with local time??)

                string ProxyWebService = "http://black.dibe.unige.it/iLabServiceBroker/iLabServiceBroker.asmx";

                ProcessAgentClient test = new ProcessAgentClient(new BasicHttpBinding(),
                   new EndpointAddress(ProxyWebService));

                timeNow = test.GetServiceTime();

                // convert result to xml
                XmlDocument payload = new XmlDocument();
                payload.LoadXml(payloadStr);
                TimeSpan timeDifferenceLSLPR = timeLabServer - timeNow;

                #endregion

                /// if the difference between labserver and labproxy is greater then 3 seconds or less then -3 seconds no synchronization
                /// will be made because this time could be consumed even by calling the webservice.
                /// If the time differs more then 3 Seconds the reservation time will be adjusted to the laboratory server time.
                if (timeDifferenceLSLPR.TotalSeconds < 3 && timeDifferenceLSLPR.TotalSeconds > -3)
                {
                    //if time differs not more then 3 secs
                    //extract data from xml and add to data container
                    actualReservation.ReservationExpiration = this.checkIfReservationInPeriod(Convert.ToDateTime(payload.GetElementsByTagName("startExecution")[0].InnerText), Convert.ToInt32(payload.GetElementsByTagName("duration")[0].InnerText));

                    actualReservation.ExperimentID = payload.GetElementsByTagName("experimentID")[0].InnerText;
                    actualReservation.GroupName = payload.GetElementsByTagName("groupName")[0].InnerText;
                }
                else
                {
                    //if time differs more then 3 secs
                    // extract data from xml, calculate difference and add to data container
                    DateTime start = Convert.ToDateTime(payload.GetElementsByTagName("startExecution")[0].InnerText) + timeDifferenceLSLPR;
                    actualReservation.ReservationExpiration = this.checkIfReservationInPeriod(start, Convert.ToInt32(payload.GetElementsByTagName("duration")[0].InnerText));
                    actualReservation.ExperimentID = payload.GetElementsByTagName("experimentID")[0].InnerText;
                    actualReservation.GroupName = payload.GetElementsByTagName("groupName")[0].InnerText;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // close connection to database
                con.Close();
            }

            return actualReservation.ToXMLString();
        }

        #region Private Methods

        /// <summary>
        /// This method checks whether the current date of expiration is in the reserved time slot. 
        /// If this is the case the expiration Datetime will be returned, otherwise null.
        /// </summary>
        private string checkIfReservationInPeriod(DateTime start, int duration)
        {
            DateTime end = start;
            end = end.AddSeconds(duration);

            //if reservation in period return expiration date
            if (DateTime.Now > start && DateTime.Now < end)
                return end.ToString();
            // else return nothing
            else
                return null;
        }

        #endregion

    }
}
