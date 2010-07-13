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
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace LabProxy.Data
{
    /// <summary>
    /// When a laboratory wants to retrieve reservation information
    /// </summary>
    [XmlRootAttribute(ElementName = "Reservation", IsNullable = false)]
    public class Reservation
    {
        /// <summary>
        /// DateTime of Reservation start
        /// </summary>
        [XmlElement(ElementName = "ReservationExpiration")]
        public string ReservationExpiration { get; set; }

        /// <summary>
        /// The group membership of the user which executes the laboratory
        /// This information can help you to integrate different levels of
        /// execution in your laboratory
        /// </summary>
        [XmlElement(ElementName = "GroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// The ID of the experiment which was called on the Servicebroker.
        /// With the help of the experiment ID different Experiment settings can be 
        /// loaded when a user connects to the laboratory
        /// </summary>
        [XmlElement(ElementName = "ExperimentID")]
        public string ExperimentID { get; set; }

        #region XML Serialization

        /// <summary>
        /// Deserialize the object
        /// </summary>
        /// <param name="xml">XML representation as string</param>
        /// <returns>the object</returns>
        public static Reservation FromXMLString(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Reservation));
            StringReader sr = new StringReader(xml);
            object result = null;
            result = serializer.Deserialize(sr);

            if (result != null)
                return (Reservation)result;
            return null;
        }

        public string ToXMLString()
        {

            XmlSerializer serializer = new XmlSerializer(this.GetType());
            StringWriterWithEncoding sw = new StringWriterWithEncoding(Encoding.UTF8);
            //StringWriter sw = new StringWriter();
            serializer.Serialize(sw, this);
            return sw.ToString();
        }

        #endregion
        
    }

}
