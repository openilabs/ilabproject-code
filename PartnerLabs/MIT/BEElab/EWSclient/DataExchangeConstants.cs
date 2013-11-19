using System;
using System.Text;

namespace schneiderelectric.services.ews
{
    /// <summary>
    ///   Constants used to define the environment for data exchange in EWS  
    ///  </summary>                   
    /// <remarks> <p/>UML Diagram: <img src="file:///D:/CWSGeneratedDocu/EA/html/EARoot/EA2/EA1/EA19.png" alt="UML Diagram" /></remarks>
    public sealed class DataExchangeConstants
    {
        /// <summary>
        ///  namespace used for EWS
        /// </summary>
        public const string Namespace = "http://www.schneider-electric.com/common/dataexchange/2011/05";

        /// <summary>
        /// prefix used for all actions
        /// </summary>
        public const string ActionPrefix = "http://www.schneider-electric.com/common/dataexchange/2011/05/";
        /// <summary>
        /// postfix used in request actions
        /// </summary>
        public const string ActionRequest = "In";

        /// <summary>
        /// postfix used in response actions
        /// </summary>
        public const string ActionResponse = "Out";

        /// <summary>
        /// namespace used for SOAP faults
        /// </summary>
        /// 
        public const string FaultNamespace = "http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterface/Fault";

        /// <summary>
        /// prefix for fault actions
        /// </summary>
        public const string FaultActionPrefix = "http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterface/Fault";
    }
}
