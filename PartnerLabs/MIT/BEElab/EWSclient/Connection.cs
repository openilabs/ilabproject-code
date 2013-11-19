using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using ews = schneiderelectric.services.ews;
using com = CommonTypes.DataExchange;

namespace ClientSoapCom
{
    /// <summary>
    /// Represents a intance of connection over the client communication stack.
    /// </summary>
    class EwsConnection
    {
        /// <summary>
        /// Unique reference number to identiy a connection.
        /// </summary>
        public string ConnectReference { get; set; }

        /// <summary>
        /// Information about the connect server.
        /// </summary>
        public com.EwsServerInfo Server { get; set; }

        /// <summary>
        /// Information about the security configuration of the connection.
        /// </summary>
        public com.EwsSecurity Security { get; set; }

        /// <summary>
        /// Information about the connection specific parameters.
        /// </summary>
        public com.EwsBindingConfig Binding { get; set; }

        /// <summary>
        /// Information about the behaviour about specification violation.
        /// </summary>
        public com.EwsContentTolerance Tolerance { get; set; }

        /// <summary>
        /// Reference to the client communication stack.
        /// </summary>
        public ews.DataExchangeClient Client { get; set; }
    }
}
