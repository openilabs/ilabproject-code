using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using ProxyClient.ReservationService;
using System.ServiceModel;

namespace LabProxyService
{

    /// <summary>
    /// Summary description for ProxyWS
    /// This class allows you to check the Reservation details of your laboratory
    /// integrated within the ISA
    /// </summary>

    public class LabProxyClient
    {
        /// <summary>
        /// Calls a webservice to retrieve reservation information and synchronizes the time of the Labserver / Labproxy
        /// </summary>
        public static void RetrieveReservation(string ProxyWebService)
        {
                // pass current request params to this class
                HttpRequest currentRequest = HttpContext.Current.Request;
                // get posted values
                HttpContext.Current.Session["ilab"] = currentRequest.QueryString["passkey"];
                HttpContext.Current.Session["sbUrl"] = currentRequest.QueryString["sbUrl"];
                
                // create webservice with posted URL
                LabProxySoapClient clientWS = new LabProxySoapClient(new BasicHttpBinding(),
                   new EndpointAddress(ProxyWebService));

                // call webservice and retrieve synchronized reservation results
                string reservation = clientWS.GetReservationInfo(HttpContext.Current.Session["ilab"].ToString(), Convert.ToString(DateTime.Now));

                // extracts all reservation data to make them available 
                XmlDocument reservationXml = new XmlDocument();
                reservationXml.LoadXml(reservation);

                //da salvare in sessione
                HttpContext.Current.Session["ReservationExpiration"] = reservationXml.GetElementsByTagName("ReservationExpiration")[0].InnerText.ToString();
                HttpContext.Current.Session["GroupName"] = reservationXml.GetElementsByTagName("GroupName")[0].InnerText.ToString();
                HttpContext.Current.Session["ExperimentID"] = reservationXml.GetElementsByTagName("ExperimentID")[0].InnerText.ToString();
        }
        

        /// <summary>
        /// Checks wehther the current registration is still valid
        /// </summary>
        /// <returns>true or false</returns>
        public static bool isResgistrationValid()
        {
            DateTime expiration = new DateTime();
            try
            {
                expiration = Convert.ToDateTime(HttpContext.Current.Session["ReservationExpiration"]);
            }
            catch (Exception)
            {
                HttpContext.Current.Session.Remove("ilab");
                return false;
            }

            if (HttpContext.Current.Session["ilab"] != null && HttpContext.Current.Request["passkey"] != null)
            {
                //check whether the session is still the same
                if (HttpContext.Current.Session["ilab"] != HttpContext.Current.Request["passkey"])
                {
                    HttpContext.Current.Session.Remove("ilab");
                    return false;
                }
            }

            if (expiration != null && DateTime.Now <= expiration)
            {
                return true;
            }
            else
            {
                HttpContext.Current.Session.Remove("ilab");
                return false;
            }
        }

        #region properties

        public static string ReservationExpiration
        {
            get
            {
                return HttpContext.Current.Session["ReservationExpiration"].ToString();
            }
        }

        public static string ExperimentID
        {
            get
            {
                return HttpContext.Current.Session["ExperimentID"].ToString();
            }
        }

        public static string GroupName
        {
            get
            {
                return HttpContext.Current.Session["GroupName"].ToString();
            }
        }

        #endregion
    }
}