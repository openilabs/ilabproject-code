using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ServiceModel.Channels;  // EwsEndpointInformation.MessageVersion

namespace CommonTypes
{
    /// <summary>
    /// This class is the server configuration. It can be serialised/deserialised for
    /// communication/business handling. This class was created for further adaption.
    /// </summary>
    [Serializable]
    [XmlRootAttribute("ServerConfiguration", IsNullable = true)]
    public class ServerConfiguration
    {
        #region Variables
        /// <summary>
        /// Name of the server configuration.
        /// </summary>
        private string name = String.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Name of the server configuration.
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion
    }

    /// <summary>
    /// Contains a value for every possible SOAP Version
    /// </summary>
    public enum SOAPVersions
    {
        /// <summary>
        /// SOAP 1.1
        /// </summary>
        Soap11,
        /// <summary>
        /// SOAP 1.1 with WSAddressing10
        /// </summary>
        Soap11WSAddressing10,
        /// <summary>
        /// SOAP 1.1 with WSAddressingAugust2004
        /// </summary>
        Soap11WSAddressingAugust2004,
        /// <summary>
        /// SOAP 1.2
        /// </summary>
        Soap12,
        /// <summary>
        /// SOAP 1.2 with WSAddressing10
        /// </summary>
        Soap12WSAddressing10,
        /// <summary>
        /// SOAP 1.2 with WSAddressingAugust2004
        /// </summary>
        Soap12WSAddressingAugust2004
    }

    /// <summary>
    /// All information that is needed to connect to an endpoint
    /// (Uri, Authentication-Method, Username, Password, SOAP-Version, Buffer sizes, ...)
    /// Yes, this could have been done by inheritance but in C#, 
    /// one class can only inherit from one base class  
    /// <remarks>
    /// see cref=http://msdn.microsoft.com/en-us/library/x9afc042.aspx
    /// </remarks>
    /// </summary>
    [Serializable, XmlRoot]
    public class EwsEndpointInformation
    {
        /// <summary>
        /// Combined: Uri = Native type; _URI = string type used in XML-representation of object
        /// </summary>
        [XmlIgnore]
        public Uri Uri { get; set; }
        /// <summary>
        /// Combined: Uri = Native type; _URI = string type used in XML-representation of object
        /// </summary>
        [XmlAttribute("Uri")]
        public string _URI
        {
            get { return Uri.ToString(); }
            set { Uri = new Uri(value); }
        }

        /// <summary>
        /// Authentication scheme used for the comunication-transport
        /// </summary>
        [XmlAttribute]
        public System.Net.AuthenticationSchemes AuthenticationScheme { get; set; }
        /// <summary>
        /// Username used for Authentication
        /// </summary>
        [XmlAttribute]
        public string Username { get; set; }
        /// <summary>
        /// Password used for Authentication
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        /// Accept cookies sent by server
        /// </summary>
        [XmlAttribute]
        public bool AllowCookies { get; set; }

        /// <summary>
        /// SOAP version used between server and client
        /// </summary>
        [XmlAttribute]
        public SOAPVersions SOAPVersion { get; set; }

        /// <summary>
        /// Maximun size of the BufferPool
        /// </summary>
        [XmlAttribute]
        public long MaxBufferPoolSize { get; set; }
        /// <summary>
        /// Maximum size of one Buffer
        /// </summary>
        [XmlAttribute]
        public int MaxBufferSize { get; set; }
        /// <summary>
        /// Maximum size for one recieved message
        /// </summary>
        [XmlAttribute]
        public long MaxReceivedMessageSize { get; set; }

        /// <summary>
        /// Timout for sending a message
        /// </summary>
        [XmlAttribute]
        public Double SendTimeout { get; set; }
        /// <summary>
        /// Timout for opening a new connection
        /// </summary>
        [XmlAttribute]
        public Double OpenTimeout { get; set; }
        /// <summary>
        /// Timeout for waiting for a message to be recvieved (response from server)
        /// </summary>
        [XmlAttribute]
        public Double ReceiveTimeout { get; set; }
        /// <summary>
        /// Timeout for closing an existing connection
        /// </summary>
        [XmlAttribute]
        public Double CloseTimeout { get; set; }


        /// <summary>
        /// Set default values on object
        /// (127.0.0.1 as endpoint, Anonymous-Authentication, no Username, no Password)
        /// </summary>
        public void SetDefaultValues()
        {
            Uri = new Uri("http://127.0.0.1");
            AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
            Username = "";
            Password = "";
            AllowCookies = true;
            SOAPVersion = SOAPVersions.Soap12;
            MaxBufferPoolSize = 65536;
            MaxBufferSize = 65536;
            MaxReceivedMessageSize = 65536;
            SendTimeout = 60;
            OpenTimeout = 60;
            ReceiveTimeout = 60;
            CloseTimeout = 60;
        }

        /// <summary>
        /// Initialises new object with default values (see SetDefaultValues())
        /// </summary>
        public EwsEndpointInformation()
        {
            SetDefaultValues();
        }

        /// <summary>
        /// Initialises new object with default values (see SetDefaultValues()) and given Uri as endpoint address
        /// </summary>
        /// <param name="uri">string; Uri of endpoint address</param>
        public EwsEndpointInformation(string uri)
        {
            SetDefaultValues();
            Uri = new Uri(uri);
        }
    }
    /// <summary>
    /// Type of information to log
    /// </summary>
    public enum LoggingTypes : int
    {
        /// <summary>
        /// Log all information
        /// </summary>
        All = 0,
        /// <summary>
        /// Log debug information
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Log user information
        /// </summary>
        User = 2
    }
}
