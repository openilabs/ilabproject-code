using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Timers;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace CommonTypes.DataExchange
{
    // Events only used in the server NOT in the client
    #region Events
    // Define an Event Callback Delegate
    public delegate void CbkMethod_GetContainerItems(List<ContainerItemType> data, List<Tuple<string, string>> errors);
    public delegate void CbkMethod_GetItems(List<ValueItemResponse> data, List<Tuple<string, string>> errors);
    public delegate void CbkMethod_GetValues(List<ValueTypeStateful> data, List<Tuple<string, string>> errors);
    public delegate void CbkMethod_SetValues(List<ResultType> data);
    public delegate void CbkMethod_GetHistory(HistoryResponseStatusType status, HistoryRecordsType data);
    public delegate void CbkMethod_GetAlarmEvents(AlarmEventGroup group);
    public delegate void CbkMethod_AcknowledgeAlarmEvents(Tuple<string, string> result);
    public delegate void CbkMethod_GetAlarmEventTypes(List<string> types);

    // Define an Event Delegate
    public delegate void EventHandlerEWS_GetContainerItems(List<string> ids, CbkMethod_GetContainerItems cbk);
    public delegate void EventHandlerEWS_GetItems(List<string> ids, CbkMethod_GetItems cbk);
    public delegate void EventHandlerEWS_GetValues(List<string> ids, CbkMethod_GetValues cbk);
    public delegate void EventHandlerEWS_SetValues(List<ValueTypeStateless> values, CbkMethod_SetValues cbk);
    public delegate void EventHandlerEWS_GetHistory(HistoryParameter param, HistoryFilter filter, CbkMethod_GetHistory cbk);
    public delegate void EventHandlerEWS_GetAlarmEvents(AlarmEventsParameter param, AlarmEventsFilter filter, CbkMethod_GetAlarmEvents cbk);
    public delegate void EventHandlerEWS_GetUpdatedAlarmEvents(UpdatedAlarmEventsParameter param, AlarmEventsFilter filter, CbkMethod_GetAlarmEvents eventCbk);
    public delegate void EventHandlerEWS_AcknowledgeAlarmEvents(string id, CbkMethod_AcknowledgeAlarmEvents eventCbk);
    public delegate void EventHandlerEWS_GetAlarmEventTypes(CbkMethod_GetAlarmEventTypes eventCbk);
    #endregion

    #region DataExchnageBaseTypes

    /// <summary>
    /// Base type for types included in a EWS message.
    /// All properties will are mandatory for dervided classes 
    /// except the property 'Description'
    /// </summary>
    public class BaseItemType
    {
        /// <summary>
        /// Identifier of an item
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// A user chosen Name for the specific AlarmItem. 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Provides a human readable text that describes the ContainerItem. 
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Represents an AlarmEventItem of EWS DataTypes
    /// See http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_alarms
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public class AlarmEventsType
    {
        /// <summary>
        /// A unique identifier to access an element unambiguously.
        /// The Id shall be unambiguously within an EcoStruxure L2 solution, 
        /// which means the Ids have to be unique over all items contained in a system. 
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Identifies unambiguously the source where the alarm was originated. 
        /// </summary>
        public string SourceID { get; set; }
        /// <summary>
        /// A human readable text that describes the source of the alarm. 
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// True or False depending on the authorization and the AlarmEvent condition 
        /// No (default) 0 	An acknowledgement is not possible 
        /// Yes 	1 	An acknowledgement is possible but not necessary 
        /// Required 	2 	An acknowledgement is possible and required 
        /// </summary>
        public int Acknowledgeable { get; set; }
        /// <summary>
        /// The time the AlarmEvent occurred. 
        /// </summary>
        public System.DateTime TimeStampOccurrence { get; set; }
        /// <summary>
        /// The time the AlarmEvent changed the transition the last time 
        /// </summary>
        public System.DateTime TimeStampTransition { get; set; }
        /// <summary>
        /// Priority is an indicator of the urgency of the alarm. (Value: 0-1000) 
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Describes the status of the alarm within the state-machine, e.g. whether the alarm is
        /// Normal 	0 	The alarm and the situation it is representing went into a normal condition 
        /// Active 	1 	An alarm occurred, which has not be acknowledged yet and the situation it is representing still exists
        /// Acknowledged 	2 	The active alarm was acknowledged by an user, but the situation it is representing still exists 
        /// Reset 	3 	The alarm is still present, but the situation it is representing does not exists anymore 
        /// Disabled 	4 	The alarm was disabled. There is no statement possible about the situation the alarm represented. 
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// Describes the type of the AlarmEvent, e.g. system, diagnosis, process alarm, etc. 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// A human readable text that describes the AlarmEvent. 
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Base type for alarm items.
    /// All properties are mandatory.
    /// </summary>
    public class AlarmItemType: BaseItemType
    {
        /// <summary>
        /// Describes the status of the alarm within the state-machine, e.g. whether the alarm is
        /// Normal 	0 	The alarm and the situation it is representing went into a normal condition 
        /// Active 	1 	An alarm occurred, which has not be acknowledged yet and the situation it is representing still exists
        /// Acknowledged 	2 	The active alarm was acknowledged by an user, but the situation it is representing still exists 
        /// Reset 	3 	The alarm is still present, but the situation it is representing does not exists anymore 
        /// Disabled 	4 	The alarm was disabled. There is no statement possible about the situation the alarm represented. 
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// A unique identifier, which indicates the Id of the related ValueItem, which is related to the occurring AlarmEvents. 
        /// </summary>
        public string ValueItemId { get; set; }
    }

    /// <summary>
    /// Describe the status of a server response including a status 
    /// description of the server database.
    /// </summary>
    public class AlarmResponseStatusType
    {
        /// <summary>
        /// Indicates if only an extract of data was transmitted due to limitations and 
        /// more data are available in the server (false=no more data; true= more data 
        /// available) 
        /// Since there could be a large number of AlarmEvents, the number of the 
        /// AlarmEvents returned per request can be limited by the server. 
        /// The MoreDataAvailable parameter indicates that one message was not enough 
        /// to transmit all AlarmEvent elements. 
        /// </summary>
        public bool MoreDataAvailable { get; set; }
        /// <summary>
        /// Server indication, where the next data are located, if the MoreDataAvailable 
        /// parameter is “TRUE”. If the MoreDataAvailable element indicates that not all 
        /// data were transmitted, this parameter can be used to request the missing data 
        /// with an additional request. In the previous response, the server responses with 
        /// a system specific reference, that value shall be used to request the missing 
        /// data in another request. 
        /// </summary>
        public string MoreDataRef { get; set; }
        /// <summary>
        /// The point in the server where the last update of alarm events was done. This 
        /// could be either a time or a pointer. a point in the server, from which on all 
        /// alarms will be responded with the next request. This value will be used later 
        /// on to request all alarms, which changed their state since the last request. 
        /// The LastUpdate element within the request shall include the responded value 
        /// of the LastUpdate element within the previous response. 
        /// </summary>
        public string LastUpdate { get; set; }
        /// <summary>
        /// Sends by the server, which indicates that the client has to refresh all 
        /// AlarmEvents. An element to indicate to the client that the alarms transmitted 
        /// by the server may not anymore reasonable. A fully upload of the alarm list from 
        /// the server is needed.
        /// </summary>
        public bool NeedsRefresh { get; set; }
    }

    /// <summary>
    /// Represent a structure containing list of different items
    /// </summary>
    public class ArrayOfItemType
    {
        /// <summary>
        /// List of value items
        /// </summary>
        public List<ValueItemType> ValueItems { get; set; }
        /// <summary>
        /// List of history items
        /// </summary>
        public List<HistoryItemType> HistoryItems { get; set; }
        /// <summary>
        /// List of alarm items
        /// </summary>
        public List<AlarmItemType> AlarmItems { get; set; }
    }

    /// <summary>
    /// An representing object of the type ContainerItem which DOES NOT contains
    /// other ContainerItems as well as other Items like ValueItems, HistoryItems, and/or AlarmItems.
    /// This simple type is used to avoid recursion.
    /// </summary>
    public class ContainerItemSimpleType : BaseItemType
    {
        /// <summary>
        /// Describes the type of the ContainerItem, which is represented by this 
        /// object. The defined common types of the ContainerItem can 
        /// be found below. Other types can be used, but if a system is not 
        /// aware of a specific type, it has to be represented as a “Folder” type.
        /// see http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data
        /// or the EWS Specification on http://cws:8000
        /// 
        /// Examples
        /// Folder 	folder 	A folder is the generic container, which represents one ContainerItem within a hierarchy
        /// Server 	server 	A server is a physical equipment, which manages several devices
        /// Device 	device 	A device is a physical equipment, which manages ValueItems, HistoryItems as well as AlarmItems
        /// Structure 	structure 	A structure is a grouping element for several ValueItems, HistoryItems as well as AlarmItems
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// An object of the type ContainerItem can contains other ContainerItems as 
    /// well as other Items like ValueItems, HistoryItems, and/or AlarmItems.
    /// With the ContainerItem type, the hierarchical view inside the system is 
    /// represented in a tree structure. The system root ContainerItem includes 
    /// further ContainerItems and other items, which again can include further 
    /// ContainerItems and other items. The system root ContainerItem can be 
    /// requested by an Id = NULL. 
    /// 
    /// see http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public class ContainerItemType : BaseItemType
    {
        /// <summary>
        /// Describes the type of the ContainerItem, which is represented by this 
        /// object. The defined common types of the ContainerItem can 
        /// be found below. Other types can be used, but if a system is not 
        /// aware of a specific type, it has to be represented as a “Folder” type.
        /// see http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data
        /// or the EWS Specification on http://cws:8000
        /// 
        /// Examples
        /// Folder 	folder 	A folder is the generic container, which represents one ContainerItem within a hierarchy
        /// Server 	server 	A server is a physical equipment, which manages several devices
        /// Device 	device 	A device is a physical equipment, which manages ValueItems, HistoryItems as well as AlarmItems
        /// Structure 	structure 	A structure is a grouping element for several ValueItems, HistoryItems as well as AlarmItems
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// A list of children lists (list[ContainerItem], list[ValuesItems], list[HistoryItems], list[AlarmItems])
        /// </summary>
        public ContainerItemTypeItems Items { get; set; }
    }

    /// <summary>
    /// List of children lists (list[ContainerItem], list[ValuesItems], 
    /// list[HistoryItems], list[AlarmItems]) used by container. 
    /// 
    /// see http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public class ContainerItemTypeItems
    {
        /// <summary>
        /// List of container types
        /// </summary>
        public List<ContainerItemSimpleType> ContainerItems { get; set; }
        /// <summary>
        /// List of value item types
        /// </summary>
        public List<ValueItemTypeBase> ValueItems { get; set; }
        /// <summary>
        /// List of history item types
        /// </summary>
        public List<HistoryItemType> HistoryItems { get; set; }
        /// <summary>
        /// List of alarm item types
        /// </summary>
        public List<AlarmItemType> AlarmItems { get; set; }
    }

    /// <summary>
    /// Error result returned by the server. 
    /// See EWS Specification on http://cws:8000
    /// </summary>
    public class ErrorResultType
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents a filter object which is used in web service opration to
    /// load alarm events (e.g. GetUpdatedAlarmEvents) 
    /// see http://139.158.104.53/ecostruxurewiki/index.php?title=CWS_V1.1_-_GetUpdatedAlarmEvents%28%29
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public class AlarmEventsFilter
    {
        /// <summary>
        /// Start time point for filter alarms
        /// </summary>
        public int PriorityFrom { get; set; }
        /// <summary>
        /// End time point for filter alarms
        /// </summary>
        public int PriorityTo { get; set; }
        /// <summary>
        /// A list of alarm event type name for filtering
        /// </summary>
        public List<string> Types { get; set; }
    }   

    /// <summary>
    /// Used to read alarm events from a server (e.g. EWSClientCom.GetAlarmEvents(...))
    /// </summary>
    public class AlarmEventsParameter
    {
        /// <summary>
        /// Server indication, where the next data are located (pointer)
        /// </summary>
        public string MoreDataRef { get; set; }
    }

    /// <summary>
    /// Represents the settings to set a filter for requesting history items
    /// from a server.
    /// See http://139.158.104.53/ecostruxurewiki/index.php?title=CWS_V1.1_-_GetHistory%28%29
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public class HistoryFilter
    {
        /// <summary>
        /// Start time point for filter history items
        /// </summary>
        public System.DateTime TimeFrom { get; set; }
        /// <summary>
        /// (internal) Flag to see if the TimeFrom property was set.
        /// This was created to handle the optional flag in the EWS WSDL file
        /// see http://139.158.104.53/ecostruxurewiki/images/5/54/CS_V1.1.wsdl
        /// </summary>
        public bool TimeFromSpecified { get; set; }
        /// <summary>
        /// End time point for filter history items
        /// </summary>
        public System.DateTime TimeTo { get; set; }
        /// <summary>
        /// (internal) Flag to see if the TimeTo property was set.
        /// This was created to handle the optional flag in the EWS WSDL file
        /// see http://139.158.104.53/ecostruxurewiki/images/5/54/CS_V1.1.wsdl
        /// </summary>
        public bool TimeToSpecified { get; set; }
    }

    /// <summary>
    /// Used in history queries
    /// </summary>
    public class HistoryParameter
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the more data ref.
        /// </summary>
        /// <value>The more data ref.</value>
        public string MoreDataRef { get; set; }
    }

    /// <summary>
    /// parameters used by GetUpdatedAlarmEvents
    /// </summary>
    public class UpdatedAlarmEventsParameter
    {
        /// <summary>
        /// Gets or sets the last update.
        /// </summary>
        /// <value>The last update.</value>
        public string LastUpdate { get; set; }
        /// <summary>
        /// Gets or sets the more data ref.
        /// </summary>
        /// <value>The more data ref.</value>
        public string MoreDataRef { get; set; }
    }

    /// <summary>
    /// Conatains information about the web service.
    /// </summary>
    public class WebServiceInformationVersion
    {
        /// <summary>
        /// Major version number of the service.
        /// </summary>
        public int MajorVersion { get; set; }

        /// <summary>
        /// Minor version number of the service.
        /// </summary>
        public string MinorVersion { get; set; }

        /// <summary>
        /// Used target name space of the service.
        /// </summary>
        public string UsedNameSpace { get; set; }
    }

    /// <summary>
    /// Represents the information of history item.
    /// </summary>
    public class HistoryItemType : BaseItemType
    {
        /// <summary>
        /// Type of the history item.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Unit of the history item
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// ID fo the source value item
        /// </summary>
        public string ValueItemId { get; set; }
    }

    /// <summary>
    /// Contains hisory records and additional information about it.  
    /// </summary>
    public class HistoryRecordsType
    {
        /// <summary>
        /// ID fo the source value item.
        /// </summary>
        public string ValueItemId { get; set; }

        /// <summary>
        /// Unit of the values stored in the records.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Type of the values stored in the records.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// List of records related to the source ID.
        /// </summary>
        public List<HistoryRecordType> List { get; set; }
    }

    /// <summary>
    /// Used in the history item to represent the data of an history record.
    /// </summary>
    public class HistoryRecordType
    {
        /// <summary>
        /// Value of the history record
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// State of the history recored
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Timestamp of the history record 
        /// </summary>
        public System.DateTime TimeStamp { get; set; }
    }

    /// <summary>
    /// Used to give feedback about getting the history data form a value item.
    /// </summary>
    public class HistoryResponseStatusType
    {
        /// <summary>
        /// Signals if more history data about a value item are available.
        /// </summary>
        public bool MoreDataAvailable { get; set; }

        /// <summary>
        /// Reference to the next table of history data.
        /// </summary>
        public string MoreDataRef { get; set; }

        /// <summary>
        /// Defines the start time of the transmitted history records.
        /// </summary>
        public System.DateTime TimeFrom { get; set; }

        /// <summary>
        /// Defines the end time of the transmitted history records.
        /// </summary>
        public System.DateTime TimeTo { get; set; }
    }

    /// <summary>
    /// Is used in a SetValue operation.
    /// It represent the result of setting an item value in the server.
    /// </summary>
    public class ResultType
    {
        /// <summary>
        /// Gets or sets the id  of a value item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Shows if set operation was successfull.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the text of the error message about what was going wrong. 
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents the full information about a value item.
    /// </summary>
    public partial class ValueItemType : ValueItemTypeBase
    {
        /// <summary>
        /// Gets or sets the value  of a value item.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the state of a value item.
        /// </summary>
        public int State { get; set; }
    }

    /// <summary>
    /// Represents the base information about a value item.
    /// </summary>
    public partial class ValueItemTypeBase : BaseItemType
    {
        /// <summary>
        /// Gets or sets the type of a value item.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the unit of a value item. 
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the writable state of a value item.
        /// </summary>
        public int Writeable { get; set; }
    }

    /// <summary>
    /// Contains information about a value item including a state.
    /// </summary>
    public partial class ValueTypeStateful
    {
        /// <summary>
        /// Gets or sets the id of a value item. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the state of a value item. 
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Gets or sets the value of a value item. 
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Contains information about a stateless value item.
    /// </summary>
    public partial class ValueTypeStateless
    {
        /// <summary>
        /// Gets or sets the id of a value item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///Gets or sets the value of a value item. 
        /// </summary>
        public string Value { get; set; }
    }

    #endregion

    #region DataExchangeExtentedTypes

    /// <summary>
    /// Extention for alarm events.
    /// </summary>
    public class AlarmEventGroup
    {
        /// <summary>
        /// Time of the latest.
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Reference for more available events if transmitting limit is crossed.
        /// </summary>
        public string MoreDataRef { get; set; }

        /// <summary>
        /// List of alarm events.
        /// </summary>
        public List<AlarmEventsType> AlarmEvents { get; set; }
    }

    /// <summary>
    /// Used in the response of getting value items. 
    /// Sperates the items by the types.
    /// </summary>
    public class ValueItemResponse
    {
        /// <summary>
        /// List of value items.
        /// </summary>
        public List<ValueItemType> ValueItems { get; set; }

        /// <summary>
        /// List of history items.
        /// </summary>
        public List<HistoryItemType> HistoryItem { get; set; }

        /// <summary>
        /// List of alarm items.
        /// </summary>
        public List<AlarmItemType> AlarmItem { get; set; }
    }

    #endregion

    #region ClientSide

    /// <summary>
    /// Contains ews information about the connect server.
    /// </summary>
    public class EwsServerInfo
    {
        /// <summary>
        /// List of the supported service operations of the server.
        /// </summary>
        private List<string> _supportedOperations = new List<string>();

        /// <summary>
        /// MajorVersion of the server.
        /// </summary>
        public string MajorVersion { get; set; }
        /// <summary>
        /// MinorVersion of the server.
        /// </summary>
        public string MinorVersion { get; set; }
        /// <summary>
        /// Used targed name space of the server.
        /// </summary>
        public string TargetNamespace { get; set; }
        /// <summary>
        /// Endpoint address url of the server.
        /// </summary>
        public string EndpointAddress { get; set; }

        /// <summary>
        /// Sets the supported operations.
        /// </summary>
        /// <param name="supportedOperations">The supported operations.</param>
        public void SetSupportedOperations(List<string> supportedOperations)
        {
            this._supportedOperations = supportedOperations;
        }

        /// <summary>
        /// Gets the supported operations.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSupportedOperations()
        {
            return this._supportedOperations;
        }

        /// <summary>
        /// Determines whether [is supported operation] [the specified operation].
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// 	<c>true</c> if [is supported operation] [the specified operation]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSupportedOperation(string operation)
        {
            return _supportedOperations != null && _supportedOperations.Contains(operation);
        }
    }

    /// <summary>
    /// Contains the security data which are used by the client instant to communicate with the server
    /// </summary>
    public class EwsSecurity
    {
        /// <summary>
        /// Gets or sets the authentication scheme.
        /// </summary>
        /// <value>The authentication scheme.</value>
        public System.Net.AuthenticationSchemes AuthenticationScheme { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Contains the configuration parameter of the client instance to the communication layer.
    /// </summary>
    public class EwsBindingConfig
    {
        /// <summary>
        /// Represents the used message version of SOAP
        /// </summary>
        public MessageVersion MessageVersion { get; set; }

        /// <summary>
        /// HTTP: 
        /// </summary>
        public bool AllowCookies { get; set; }

        /// <summary>
        /// Represents the used authentication scheme.
        /// </summary>
        public System.Net.AuthenticationSchemes AuthenticationScheme { get; set; }

        /// <summary>
        /// Max. buffer size of the pool.
        /// </summary>
        public long MaxBufferPoolSize { get; set; }

        /// <summary>
        /// Max. buffer size
        /// </summary>
        public int MaxBufferSize { get; set; }

        /// <summary>
        /// Max. buffer size of a incomming message.
        /// </summary>
        public long MaxReceivedMessageSize { get; set; }

        /// <summary>
        ///  Timeout for sending a message.
        /// </summary>
        public Double SendTimeout { get; set; }

        /// <summary>
        /// Timeout for open a connection.
        /// </summary>
        public Double OpenTimeout { get; set; }

        /// <summary>
        /// Timeout for receiving a message.
        /// </summary>
        public Double ReceiveTimeout { get; set; }

        /// <summary>
        /// Timeout for closing a connection.
        /// </summary>
        public Double CloseTimeout { get; set; }
    }


    /// <summary>
    /// Contains the behaviour definition of the client for general content dependencies.
    /// In case of specification violation and name space missmatches during the processing time of the data. 
    /// </summary>
    public class EwsContentTolerance
    {
        // General Ccontent dependencies.
        /// <summary>
        /// Ignore namespaces [on/off].
        /// </summary>
        public bool IgnoreNamespace { get; set; }

        /// <summary>
        /// Strict following of the rules in the specification [on/off].
        /// </summary>
        public bool StrictContentProcessing { get; set; }
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// Definition of the different existing folder types.
    /// </summary>
    public enum ContainerType
    {
        /// <summary>
        /// Container represents a folder.
        /// </summary>
        [XmlEnum("Folder")]
        Folder,
        /// <summary>
        /// Container represents a server.
        /// </summary>
        [XmlEnum("Server")]
        Server,
        /// <summary>
        /// Container represents a device.
        /// </summary>
        [XmlEnum("Device")]
        Device,
        /// <summary>
        /// Container represents a structure.
        /// </summary>
        [XmlEnum("Structure")]
        Structure
    }

    /// <summary>
    /// Definition of status for acknowleding an alarm.
    /// </summary>
    public enum AcknowledgeableType
    {
        /// <summary>
        /// No acknowledgment.
        /// </summary>
        [XmlEnum("No")]
        No = 0,
        /// <summary>
        /// Alarm is acknowledged.
        /// </summary>
        [XmlEnum("Yes")]
        Yes = 1,
        /// <summary>
        /// Acknowledging the alarm is required.
        /// </summary>
        [XmlEnum("Required")]
        Required = 2
    }

    /// <summary>
    /// Represent the type of the alarm item state mode
    /// as enum/integer representation.
    /// </summary>
    public enum AlarmState
    {
        /// <summary>
        /// Alarm item has the state normal
        /// </summary>
        [XmlEnum("Normal")]
        Normal = 0,
        /// <summary>
        /// Alarm item is active
        /// </summary>
        [XmlEnum("Active")]
        Active = 1,
        /// <summary>
        /// Alarm is acknowledged
        /// </summary>
        [XmlEnum("Acknowledged")]
        Acknowledged = 2,
        /// <summary>
        /// Alarm item is reseted
        /// </summary>
        [XmlEnum("Reset")]
        Reset = 3,
        /// <summary>
        /// Alarm item is disabled
        /// </summary>
        [XmlEnum("Disabled")]
        Disabled = 4
    }

    /// <summary>
    /// Represent the type of the value item state mode
    /// as enum/integer representation. It is needed inside the
    /// EWS Test Client
    /// </summary>
    public enum ValueItemState
    {
        /// <summary>
        /// Good
        /// </summary>
        [XmlEnum("Good")]
        Good = 0,
        /// <summary>
        /// Uncertain
        /// </summary>
        [XmlEnum("Uncertain")]
        Uncertain = 1,
        /// <summary>
        /// Forced
        /// </summary>
        [XmlEnum("Forced")]
        Forced = 2,
        /// <summary>
        /// Offline
        /// </summary>
        [XmlEnum("Offline")]
        Offline = 3,
        /// <summary>
        /// Error
        /// </summary>
        [XmlEnum("Error")]
        Error = 4
    }

    /// <summary>
    /// Represent the type of the value item access mode (read/ read-write)
    /// as enum/integer representation
    /// </summary>
    public enum ValueItemAccess
    {
        /// <summary>
        /// Read only
        /// </summary>
        [XmlEnum("r")]
        ReadOnly = 0,
        /// <summary>
        /// Read Write
        /// </summary>
        [XmlEnum("rw")]
        ReadWrite = 1
    }

    /// <summary>
    /// Represent the type of the value item content as enum/integer
    /// representation
    /// </summary>
    public enum ValueItemDataType
    {
        /// <summary>
        /// No/Unknown type
        /// </summary>
        [XmlEnum("None")]
        None = 0,
        /// <summary>
        /// DateTime Item
        /// </summary>
        [XmlEnum("DateTime")]
        DateTime,
        /// <summary>
        /// Boolean Item
        /// </summary>
        [XmlEnum("Boolean")]
        Boolean,
        /// <summary>
        /// String Item
        /// </summary>
        [XmlEnum("String")]
        String,
        /// <summary>
        /// Double Item
        /// </summary>
        [XmlEnum("Double")]
        Double,
        /// <summary>
        /// Long Item
        /// </summary>
        [XmlEnum("Long")]
        Long,
        /// <summary>
        /// Integer Item
        /// </summary>
        [XmlEnum("Int")]
        Integer
    }

    /// <summary>
    /// Represents the children items of an container as enum/integer
    /// representation
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Container Item
        /// </summary>
        [XmlEnum("Container")]
        Container = 0,
        /// <summary>
        /// Value Item
        /// </summary>
        [XmlEnum("Value")]
        Value,
        /// <summary>
        /// History Item
        /// </summary>
        [XmlEnum("History")]
        History,
        /// <summary>
        /// Alarm Item
        /// </summary>
        [XmlEnum("Alarm")]
        Alarm 
    }

    #endregion
}
