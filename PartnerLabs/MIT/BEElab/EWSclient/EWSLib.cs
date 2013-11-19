namespace schneiderelectric.services.ews
{
    
    /// <summary>
    /// Auto generated Code
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", ConfigurationName="IDataExchange")]
    public interface IDataExchange
    {
        
        // CODEGEN: Generating message contract since the operation GetWebServiceInformation is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetWebServiceInform" +
            "ationIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetWebServiceInform" +
            "ationOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetWebServiceInform" +
            "ationFault_Operation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetWebServiceInform" +
            "ationFault_Permission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetWebServiceInform" +
            "ationFault_TimeOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetWebServiceInformationResponse1 GetWebServiceInformation(GetWebServiceInformationRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetContainerItems is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetContainerItemsIn" +
            "", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetContainerItemsOu" +
            "t")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetContainerItemsFa" +
            "ult_Operation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetContainerItemsFa" +
            "ult_Missing_Id_List", Name="Fault_Missing_Id_List", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetContainerItemsFa" +
            "ult_Permission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetContainerItemsFa" +
            "ult_TimeOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetContainerItemsResponse1 GetContainerItems(GetContainerItemsRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetItems is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetItemsIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetItemsOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetItemsFault_Opera" +
            "tion_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetItemsFault_Missi" +
            "ng_Id_List", Name="Fault_Missing_Id_List", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetItemsFault_Permi" +
            "ssion_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetItemsFault_TimeO" +
            "ut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetItemsResponse1 GetItems(GetItemsRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetValues is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetValuesIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetValuesOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetValuesFault_Oper" +
            "ation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetValuesFault_Miss" +
            "ing_Id_List", Name="Fault_Missing_Id_List", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetValuesFault_Perm" +
            "ission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetValuesFault_Time" +
            "Out", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetValuesResponse1 GetValues(GetValuesRequest1 request);
        
        // CODEGEN: Generating message contract since the operation SetValues is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesFault_Oper" +
            "ation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesFault_Miss" +
            "ing_Id_List", Name="Fault_Missing_Id_List", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesFault_Perm" +
            "ission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesFault_Time" +
            "Out", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesFault_Miss" +
            "ing_Value_List", Name="Fault_Missing_Value_List", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/SetValuesFault_Uneq" +
            "ual_Lists", Name="Fault_Unequal_Lists", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SetValuesResponse1 SetValues(SetValuesRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetHistory is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryFault_Ope" +
            "ration_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryFault_Inv" +
            "alid_Reference", Name="Fault_Invalid_Reference", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryFault_Per" +
            "mission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryFault_Tim" +
            "eOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryFault_Inv" +
            "alid_Id", Name="Fault_Invalid_Id", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetHistoryFault_Inv" +
            "alid_Time", Name="Fault_Invalid_Time", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetHistoryResponse1 GetHistory(GetHistoryRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetAlarmEvents is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsFault" +
            "_Operation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsFault" +
            "_Invalid_Reference", Name="Fault_Invalid_Reference", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsFault" +
            "_Invalid_Priority", Name="Fault_Invalid_Priority", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsFault" +
            "_Permission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsFault" +
            "_TimeOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventsFault" +
            "_Invalid_Type", Name="Fault_Invalid_Type", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetAlarmEventsResponse1 GetAlarmEvents(GetAlarmEventsRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetUpdatedAlarmEvents is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_Operation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_Invalid_Reference", Name="Fault_Invalid_Reference", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_Invalid_Priority", Name="Fault_Invalid_Priority", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_Permission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_TimeOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_Invalid_Type", Name="Fault_Invalid_Type", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetUpdatedAlarmEven" +
            "tsFault_Invalid_Time", Name="Fault_Invalid_Time", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetUpdatedAlarmEventsResponse1 GetUpdatedAlarmEvents(GetUpdatedAlarmEventsRequest1 request);
        
        // CODEGEN: Generating message contract since the operation AcknowledgeAlarmEvents is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/AcknowledgeAlarmEve" +
            "ntsIn", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/AcknowledgeAlarmEve" +
            "ntsOut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/AcknowledgeAlarmEve" +
            "ntsFault_Operation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/AcknowledgeAlarmEve" +
            "ntsFault_Missing_Id_List", Name="Fault_Missing_Id_List", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/AcknowledgeAlarmEve" +
            "ntsFault_Permission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/AcknowledgeAlarmEve" +
            "ntsFault_TimeOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        AcknowledgeAlarmEventsResponse1 AcknowledgeAlarmEvents(AcknowledgeAlarmEventsRequest1 request);
        
        // CODEGEN: Generating message contract since the operation GetAlarmEventTypes is neither RPC nor document wrapped.
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventTypesI" +
            "n", ReplyAction="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventTypesO" +
            "ut")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventTypesF" +
            "ault_Operation_Not_Supported", Name="Fault_Operation_Not_Supported", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventTypesF" +
            "ault_Permission_Denied", Name="Fault_Permission_Denied", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.FaultContractAttribute(typeof(string), Action="http://www.schneider-electric.com/common/dataexchange/2011/05/GetAlarmEventTypesF" +
            "ault_TimeOut", Name="Fault_TimeOut", Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05/DataExchangeInterfa" +
            "ce/Fault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GetAlarmEventTypesResponse1 GetAlarmEventTypes(GetAlarmEventTypesRequest1 request);
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetWebServiceInformationRequest
    {
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class AlarmEventsType
    {
        
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string ID;
        
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string SourceID;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string SourceName;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Acknowledgeable;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeStampOccurrence;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeStampTransition;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Priority;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string State;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Message;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class AlarmResponseStatusType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public bool MoreDataAvailable;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string MoreDataRef;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string LastUpdate;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public bool NeedsRefresh;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class HistoryRecordType
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Value;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string State;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeStamp;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class HistoryRecordsType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string ValueItemId;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Unit;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("HistoryRecord", IsNullable=false)]
        public System.Collections.Generic.List<HistoryRecordType> List;
        
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeList()
        {
            return ((this.List != null) 
                        && (this.List.Count > 0));
        }
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class HistoryResponseStatusType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public bool MoreDataAvailable;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string MoreDataRef;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeFrom;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeTo;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ResultType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public bool Success;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Message;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ValueTypeStateless
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Value;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ValueTypeStateful
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string State;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Value;
    }
    
    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ValueItemType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Name;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Description;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Value;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Unit;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Writeable;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string State;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ArrayOfItemType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ValueItem", IsNullable=false)]
        public System.Collections.Generic.List<ValueItemType> ValueItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("HistoryItem", IsNullable=false)]
        public System.Collections.Generic.List<HistoryItemType> HistoryItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("AlarmItem", IsNullable=false)]
        public System.Collections.Generic.List<AlarmItemType> AlarmItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeValueItems()
        {
            return ((this.ValueItems != null) 
                        && (this.ValueItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeHistoryItems()
        {
            return ((this.HistoryItems != null) 
                        && (this.HistoryItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeAlarmItems()
        {
            return ((this.AlarmItems != null) 
                        && (this.AlarmItems.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class HistoryItemType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Name;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Description;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Unit;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string ValueItemId;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class AlarmItemType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Name;
        
        /// <remarks/>
        public string Description;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string State;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string ValueItemId;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ErrorResultType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Message;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ValueItemTypeBase
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Name;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Description;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Unit;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Writeable;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ContainerItemSimpleType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Name;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Description;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ContainerItemType
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Name;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Description;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Type;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public ContainerItemTypeItems Items;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class ContainerItemTypeItems
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ContainerItem", IsNullable=false)]
        public System.Collections.Generic.List<ContainerItemSimpleType> ContainerItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ValueItem", IsNullable=false)]
        public System.Collections.Generic.List<ValueItemTypeBase> ValueItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("HistoryItem", IsNullable=false)]
        public System.Collections.Generic.List<HistoryItemType> HistoryItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("AlarmItem", IsNullable=false)]
        public System.Collections.Generic.List<AlarmItemType> AlarmItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeContainerItems()
        {
            return ((this.ContainerItems != null) 
                        && (this.ContainerItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeValueItems()
        {
            return ((this.ValueItems != null) 
                        && (this.ValueItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeHistoryItems()
        {
            return ((this.HistoryItems != null) 
                        && (this.HistoryItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeAlarmItems()
        {
            return ((this.AlarmItems != null) 
                        && (this.AlarmItems.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetWebServiceInformationResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetWebServiceInformationResponseGetWebServiceInformationVersion GetWebServiceInformationVersion;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Operation", IsNullable=false)]
        public System.Collections.Generic.List<string> GetWebServiceInformationSupportedOperations;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetWebServiceInformationSupportedOperations()
        {
            return ((this.GetWebServiceInformationSupportedOperations != null) 
                        && (this.GetWebServiceInformationSupportedOperations.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetWebServiceInformationResponseGetWebServiceInformationVersion
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string MajorVersion;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string MinorVersion;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string UsedNameSpace;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetWebServiceInformationRequest1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetWebServiceInformationRequest GetWebServiceInformationRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetWebServiceInformationRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetWebServiceInformationRequest1(GetWebServiceInformationRequest GetWebServiceInformationRequest)
        {
            this.GetWebServiceInformationRequest = GetWebServiceInformationRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetWebServiceInformationResponse1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetWebServiceInformationResponse GetWebServiceInformationResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetWebServiceInformationResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetWebServiceInformationResponse1(GetWebServiceInformationResponse GetWebServiceInformationResponse)
        {
            this.GetWebServiceInformationResponse = GetWebServiceInformationResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetContainerItemsRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable=false)]
        public System.Collections.Generic.List<string> GetContainerItemsIds;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetContainerItemsIds()
        {
            return ((this.GetContainerItemsIds != null) 
                        && (this.GetContainerItemsIds.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetContainerItemsResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ContainerItem", IsNullable=false)]
        public System.Collections.Generic.List<ContainerItemType> GetContainerItemsItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ErrorResult", IsNullable=false)]
        public System.Collections.Generic.List<ErrorResultType> GetContainerItemsErrorResults;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetContainerItemsItems()
        {
            return ((this.GetContainerItemsItems != null) 
                        && (this.GetContainerItemsItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetContainerItemsErrorResults()
        {
            return ((this.GetContainerItemsErrorResults != null) 
                        && (this.GetContainerItemsErrorResults.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetContainerItemsRequest1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetContainerItemsRequest GetContainerItemsRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetContainerItemsRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetContainerItemsRequest1(GetContainerItemsRequest GetContainerItemsRequest)
        {
            this.GetContainerItemsRequest = GetContainerItemsRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetContainerItemsResponse1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetContainerItemsResponse GetContainerItemsResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetContainerItemsResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetContainerItemsResponse1(GetContainerItemsResponse GetContainerItemsResponse)
        {
            this.GetContainerItemsResponse = GetContainerItemsResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetItemsRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable=false)]
        public System.Collections.Generic.List<string> GetItemsIds;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetItemsIds()
        {
            return ((this.GetItemsIds != null) 
                        && (this.GetItemsIds.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetItemsResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public ArrayOfItemType GetItemsItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ErrorResult", IsNullable=false)]
        public System.Collections.Generic.List<ErrorResultType> GetItemsErrorResults;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetItemsErrorResults()
        {
            return ((this.GetItemsErrorResults != null) 
                        && (this.GetItemsErrorResults.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetItemsRequest1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetItemsRequest GetItemsRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetItemsRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetItemsRequest1(GetItemsRequest GetItemsRequest)
        {
            this.GetItemsRequest = GetItemsRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetItemsResponse1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetItemsResponse GetItemsResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetItemsResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetItemsResponse1(GetItemsResponse GetItemsResponse)
        {
            this.GetItemsResponse = GetItemsResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetValuesRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable=false)]
        public System.Collections.Generic.List<string> GetValuesIds;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetValuesIds()
        {
            return ((this.GetValuesIds != null) 
                        && (this.GetValuesIds.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetValuesResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ValueItem", IsNullable=false)]
        public System.Collections.Generic.List<ValueTypeStateful> GetValuesItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ErrorResult", IsNullable=false)]
        public System.Collections.Generic.List<ErrorResultType> GetValuesErrorResults;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetValuesItems()
        {
            return ((this.GetValuesItems != null) 
                        && (this.GetValuesItems.Count > 0));
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetValuesErrorResults()
        {
            return ((this.GetValuesErrorResults != null) 
                        && (this.GetValuesErrorResults.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetValuesRequest1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetValuesRequest GetValuesRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetValuesRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetValuesRequest1(GetValuesRequest GetValuesRequest)
        {
            this.GetValuesRequest = GetValuesRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetValuesResponse1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetValuesResponse GetValuesResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetValuesResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetValuesResponse1(GetValuesResponse GetValuesResponse)
        {
            this.GetValuesResponse = GetValuesResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class SetValuesRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("ValueItem", IsNullable=false)]
        public System.Collections.Generic.List<ValueTypeStateless> SetValuesItems;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeSetValuesItems()
        {
            return ((this.SetValuesItems != null) 
                        && (this.SetValuesItems.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class SetValuesResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Result", IsNullable=false)]
        public System.Collections.Generic.List<ResultType> SetValuesResults;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeSetValuesResults()
        {
            return ((this.SetValuesResults != null) 
                        && (this.SetValuesResults.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SetValuesRequest1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public SetValuesRequest SetValuesRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public SetValuesRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public SetValuesRequest1(SetValuesRequest SetValuesRequest)
        {
            this.SetValuesRequest = SetValuesRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SetValuesResponse1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public SetValuesResponse SetValuesResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public SetValuesResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public SetValuesResponse1(SetValuesResponse SetValuesResponse)
        {
            this.SetValuesResponse = SetValuesResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetHistoryRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryRequestGetHistoryParameter GetHistoryParameter;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryRequestGetHistoryFilter GetHistoryFilter;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetHistoryRequestGetHistoryParameter
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string Id;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string MoreDataRef;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetHistoryRequestGetHistoryFilter
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeFrom;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TimeFromSpecified;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public System.DateTime TimeTo;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TimeToSpecified;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetHistoryResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public HistoryResponseStatusType GetHistoryResponseStatus;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public HistoryRecordsType GetHistoryHistoryRecords;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetHistoryRequest1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetHistoryRequest GetHistoryRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryRequest1(GetHistoryRequest GetHistoryRequest)
        {
            this.GetHistoryRequest = GetHistoryRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetHistoryResponse1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetHistoryResponse GetHistoryResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryResponse1(GetHistoryResponse GetHistoryResponse)
        {
            this.GetHistoryResponse = GetHistoryResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetAlarmEventsRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsRequestGetAlarmEventsParameter GetAlarmEventsParameter;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsRequestGetAlarmEventsFilter GetAlarmEventsFilter;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetAlarmEventsRequestGetAlarmEventsParameter
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string MoreDataRef;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetAlarmEventsRequestGetAlarmEventsFilter
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string PriorityFrom;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string PriorityTo;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Type", IsNullable=false)]
        public System.Collections.Generic.List<string> Types;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeTypes()
        {
            return ((this.Types != null) 
                        && (this.Types.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetAlarmEventsResponse
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AlarmResponseStatusType GetAlarmEventsResponseStatus;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("AlarmEvent", IsNullable=false)]
        public System.Collections.Generic.List<AlarmEventsType> GetAlarmEventsAlarmEvents;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetAlarmEventsAlarmEvents()
        {
            return ((this.GetAlarmEventsAlarmEvents != null) 
                        && (this.GetAlarmEventsAlarmEvents.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAlarmEventsRequest1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetAlarmEventsRequest GetAlarmEventsRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsRequest1(GetAlarmEventsRequest GetAlarmEventsRequest)
        {
            this.GetAlarmEventsRequest = GetAlarmEventsRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAlarmEventsResponse1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetAlarmEventsResponse GetAlarmEventsResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsResponse1(GetAlarmEventsResponse GetAlarmEventsResponse)
        {
            this.GetAlarmEventsResponse = GetAlarmEventsResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetUpdatedAlarmEventsRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsParameter GetUpdatedAlarmEventsParameter;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsFilter GetUpdatedAlarmEventsFilter;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsParameter
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string LastUpdate;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public string MoreDataRef;
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsFilter
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string PriorityFrom;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string PriorityTo;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Type", IsNullable=false)]
        public System.Collections.Generic.List<string> Types;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeTypes()
        {
            return ((this.Types != null) 
                        && (this.Types.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetUpdatedAlarmEventsResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AlarmResponseStatusType GetUpdatedAlarmEventsResponseStatus;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("AlarmEvent", IsNullable=false)]
        public System.Collections.Generic.List<AlarmEventsType> GetUpdatedAlarmEventsAlarmEvents;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetUpdatedAlarmEventsAlarmEvents()
        {
            return ((this.GetUpdatedAlarmEventsAlarmEvents != null) 
                        && (this.GetUpdatedAlarmEventsAlarmEvents.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetUpdatedAlarmEventsRequest1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetUpdatedAlarmEventsRequest GetUpdatedAlarmEventsRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsRequest1(GetUpdatedAlarmEventsRequest GetUpdatedAlarmEventsRequest)
        {
            this.GetUpdatedAlarmEventsRequest = GetUpdatedAlarmEventsRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetUpdatedAlarmEventsResponse1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetUpdatedAlarmEventsResponse GetUpdatedAlarmEventsResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsResponse1(GetUpdatedAlarmEventsResponse GetUpdatedAlarmEventsResponse)
        {
            this.GetUpdatedAlarmEventsResponse = GetUpdatedAlarmEventsResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class AcknowledgeAlarmEventsRequest
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable=false)]
        public System.Collections.Generic.List<string> AcknowledgeAlarmEventsIds;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeAcknowledgeAlarmEventsIds()
        {
            return ((this.AcknowledgeAlarmEventsIds != null) 
                        && (this.AcknowledgeAlarmEventsIds.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class AcknowledgeAlarmEventsResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Result", IsNullable=false)]
        public System.Collections.Generic.List<ResultType> AcknowledgeAlarmEventsResults;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeAcknowledgeAlarmEventsResults()
        {
            return ((this.AcknowledgeAlarmEventsResults != null) 
                        && (this.AcknowledgeAlarmEventsResults.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class AcknowledgeAlarmEventsRequest1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public AcknowledgeAlarmEventsRequest AcknowledgeAlarmEventsRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AcknowledgeAlarmEventsRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AcknowledgeAlarmEventsRequest1(AcknowledgeAlarmEventsRequest AcknowledgeAlarmEventsRequest)
        {
            this.AcknowledgeAlarmEventsRequest = AcknowledgeAlarmEventsRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class AcknowledgeAlarmEventsResponse1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public AcknowledgeAlarmEventsResponse AcknowledgeAlarmEventsResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AcknowledgeAlarmEventsResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AcknowledgeAlarmEventsResponse1(AcknowledgeAlarmEventsResponse AcknowledgeAlarmEventsResponse)
        {
            this.AcknowledgeAlarmEventsResponse = AcknowledgeAlarmEventsResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetAlarmEventTypesRequest
    {
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05")]
    public partial class GetAlarmEventTypesResponse
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.Xml.Serialization.XmlArrayItemAttribute("Type", IsNullable=false)]
        public System.Collections.Generic.List<string> GetAlarmEventTypesTypes;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public virtual bool ShouldSerializeGetAlarmEventTypesTypes()
        {
            return ((this.GetAlarmEventTypesTypes != null) 
                        && (this.GetAlarmEventTypesTypes.Count > 0));
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAlarmEventTypesRequest1
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetAlarmEventTypesRequest GetAlarmEventTypesRequest;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventTypesRequest1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventTypesRequest1(GetAlarmEventTypesRequest GetAlarmEventTypesRequest)
        {
            this.GetAlarmEventTypesRequest = GetAlarmEventTypesRequest;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAlarmEventTypesResponse1
    {

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.schneider-electric.com/common/dataexchange/2011/05", Order=0)]
        public GetAlarmEventTypesResponse GetAlarmEventTypesResponse;

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventTypesResponse1()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventTypesResponse1(GetAlarmEventTypesResponse GetAlarmEventTypesResponse)
        {
            this.GetAlarmEventTypesResponse = GetAlarmEventTypesResponse;
        }
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataExchangeChannel : IDataExchange, System.ServiceModel.IClientChannel
    {
    }

    /// <summary>
    /// Generated code by WSCF.blue
    /// </summary>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataExchangeClient : System.ServiceModel.ClientBase<IDataExchange>, IDataExchange
    {
        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public DataExchangeClient()
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public DataExchangeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public DataExchangeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public DataExchangeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public DataExchangeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetWebServiceInformationResponse1 IDataExchange.GetWebServiceInformation(GetWebServiceInformationRequest1 request)
        {
            return base.Channel.GetWebServiceInformation(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetWebServiceInformationResponse GetWebServiceInformation(GetWebServiceInformationRequest GetWebServiceInformationRequest)
        {
            GetWebServiceInformationRequest1 inValue = new GetWebServiceInformationRequest1();
            inValue.GetWebServiceInformationRequest = GetWebServiceInformationRequest;
            GetWebServiceInformationResponse1 retVal = ((IDataExchange)(this)).GetWebServiceInformation(inValue);
            return retVal.GetWebServiceInformationResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetContainerItemsResponse1 IDataExchange.GetContainerItems(GetContainerItemsRequest1 request)
        {
            return base.Channel.GetContainerItems(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetContainerItemsResponse GetContainerItems(GetContainerItemsRequest GetContainerItemsRequest)
        {
            GetContainerItemsRequest1 inValue = new GetContainerItemsRequest1();
            inValue.GetContainerItemsRequest = GetContainerItemsRequest;
            GetContainerItemsResponse1 retVal = ((IDataExchange)(this)).GetContainerItems(inValue);
            return retVal.GetContainerItemsResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetItemsResponse1 IDataExchange.GetItems(GetItemsRequest1 request)
        {
            return base.Channel.GetItems(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetItemsResponse GetItems(GetItemsRequest GetItemsRequest)
        {
            GetItemsRequest1 inValue = new GetItemsRequest1();
            inValue.GetItemsRequest = GetItemsRequest;
            GetItemsResponse1 retVal = ((IDataExchange)(this)).GetItems(inValue);
            return retVal.GetItemsResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetValuesResponse1 IDataExchange.GetValues(GetValuesRequest1 request)
        {
            return base.Channel.GetValues(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetValuesResponse GetValues(GetValuesRequest GetValuesRequest)
        {
            GetValuesRequest1 inValue = new GetValuesRequest1();
            inValue.GetValuesRequest = GetValuesRequest;
            GetValuesResponse1 retVal = ((IDataExchange)(this)).GetValues(inValue);
            return retVal.GetValuesResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SetValuesResponse1 IDataExchange.SetValues(SetValuesRequest1 request)
        {
            return base.Channel.SetValues(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public SetValuesResponse SetValues(SetValuesRequest SetValuesRequest)
        {
            SetValuesRequest1 inValue = new SetValuesRequest1();
            inValue.SetValuesRequest = SetValuesRequest;
            SetValuesResponse1 retVal = ((IDataExchange)(this)).SetValues(inValue);
            return retVal.SetValuesResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetHistoryResponse1 IDataExchange.GetHistory(GetHistoryRequest1 request)
        {
            return base.Channel.GetHistory(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetHistoryResponse GetHistory(GetHistoryRequest GetHistoryRequest)
        {
            GetHistoryRequest1 inValue = new GetHistoryRequest1();
            inValue.GetHistoryRequest = GetHistoryRequest;
            GetHistoryResponse1 retVal = ((IDataExchange)(this)).GetHistory(inValue);
            return retVal.GetHistoryResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetAlarmEventsResponse1 IDataExchange.GetAlarmEvents(GetAlarmEventsRequest1 request)
        {
            return base.Channel.GetAlarmEvents(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventsResponse GetAlarmEvents(GetAlarmEventsRequest GetAlarmEventsRequest)
        {
            GetAlarmEventsRequest1 inValue = new GetAlarmEventsRequest1();
            inValue.GetAlarmEventsRequest = GetAlarmEventsRequest;
            GetAlarmEventsResponse1 retVal = ((IDataExchange)(this)).GetAlarmEvents(inValue);
            return retVal.GetAlarmEventsResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetUpdatedAlarmEventsResponse1 IDataExchange.GetUpdatedAlarmEvents(GetUpdatedAlarmEventsRequest1 request)
        {
            return base.Channel.GetUpdatedAlarmEvents(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetUpdatedAlarmEventsResponse GetUpdatedAlarmEvents(GetUpdatedAlarmEventsRequest GetUpdatedAlarmEventsRequest)
        {
            GetUpdatedAlarmEventsRequest1 inValue = new GetUpdatedAlarmEventsRequest1();
            inValue.GetUpdatedAlarmEventsRequest = GetUpdatedAlarmEventsRequest;
            GetUpdatedAlarmEventsResponse1 retVal = ((IDataExchange)(this)).GetUpdatedAlarmEvents(inValue);
            return retVal.GetUpdatedAlarmEventsResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        AcknowledgeAlarmEventsResponse1 IDataExchange.AcknowledgeAlarmEvents(AcknowledgeAlarmEventsRequest1 request)
        {
            return base.Channel.AcknowledgeAlarmEvents(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public AcknowledgeAlarmEventsResponse AcknowledgeAlarmEvents(AcknowledgeAlarmEventsRequest AcknowledgeAlarmEventsRequest)
        {
            AcknowledgeAlarmEventsRequest1 inValue = new AcknowledgeAlarmEventsRequest1();
            inValue.AcknowledgeAlarmEventsRequest = AcknowledgeAlarmEventsRequest;
            AcknowledgeAlarmEventsResponse1 retVal = ((IDataExchange)(this)).AcknowledgeAlarmEvents(inValue);
            return retVal.AcknowledgeAlarmEventsResponse;
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        GetAlarmEventTypesResponse1 IDataExchange.GetAlarmEventTypes(GetAlarmEventTypesRequest1 request)
        {
            return base.Channel.GetAlarmEventTypes(request);
        }

        /// <summary>
        /// Generated code by WSCF.blue
        /// </summary>
        public GetAlarmEventTypesResponse GetAlarmEventTypes(GetAlarmEventTypesRequest GetAlarmEventTypesRequest)
        {
            GetAlarmEventTypesRequest1 inValue = new GetAlarmEventTypesRequest1();
            inValue.GetAlarmEventTypesRequest = GetAlarmEventTypesRequest;
            GetAlarmEventTypesResponse1 retVal = ((IDataExchange)(this)).GetAlarmEventTypes(inValue);
            return retVal.GetAlarmEventTypesResponse;
        }
    }
}
