using System.Collections.Generic;
using System.Xml.Serialization;
using System.ServiceModel;
using System;
using CommonTypes.DataExchange;

/// <summary>
/// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
/// or the EWS Specification on http://cws:8000
/// </summary>
namespace schneiderelectric.services.ews
{
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000

    public partial class AlarmEventsType
    {
    }

    public partial class AlarmResponseStatusType
    {
    }

    public partial class HistoryRecordType
    {
    }

    public partial class HistoryRecordsType
    {
    }

    public partial class HistoryResponseStatusType
    {
    }

    public partial class ResultType
    {
    }

    public partial class ValueTypeStateless
    {
    }

    public partial class ValueTypeStateful
    {
    }

    public partial class ValueItemType
    {
    }

    public partial class ArrayOfItemType
    {
    }

    public partial class HistoryItemType
    {
    }

    /// <summary>
    /// defines an alarm item. 
    /// See also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class AlarmItemType
    {
        public AlarmItemType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmItemType"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="valueItemId">The value item id.</param>
        /// <param name="state">The state.</param>
        public AlarmItemType(string id, string name, string description, string valueItemId, string state)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.ValueItemId = valueItemId;
            this.State = state;
        }
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class ErrorResultType
    {
        public static List<ErrorResultType> ToList(List<Tuple<string, string>> errors)
        {
            var list = new List<ErrorResultType>();
            foreach (var error in errors)
            {
                list.Add(new ErrorResultType()
                {
                    Id = error.Item1,
                    Message = error.Item2
                });
            }
            return list;
        }
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class ValueItemTypeBase
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class ContainerItemSimpleType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItemSimpleType"/> class.
        /// </summary>
        public ContainerItemSimpleType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItemSimpleType"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="containerType">Type of the container.</param>
        public ContainerItemSimpleType(string id, string name, string description, string containerType)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Type = containerType;
        }
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class ContainerItemType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItemType"/> class.
        /// </summary>
        public ContainerItemType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerItemType"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="containerType">Type of the container.</param>
        public ContainerItemType(string id, string name, string description, string containerType)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Type = containerType;
        }

        /// <summary>
        /// Adds the container item.
        /// </summary>
        /// <param name="container">The container.</param>
        public void AddContainerItem(ContainerItemSimpleType container)
        {
            if (this.Items == null) this.Items = new ContainerItemTypeItems();
            if (this.Items.ContainerItems == null) this.Items.ContainerItems = new List<ContainerItemSimpleType>();
            this.Items.ContainerItems.Add(container);
        }

        /// <summary>
        /// Adds the value item.
        /// </summary>
        /// <param name="valueitem">The valueitem.</param>
        public void AddValueItem(ValueItemTypeBase valueitem)
        {
            if (this.Items == null) this.Items = new ContainerItemTypeItems();
            if (this.Items.ValueItems == null) this.Items.ValueItems = new List<ValueItemTypeBase>();
            this.Items.ValueItems.Add(valueitem);
        }

        /// <summary>
        /// Adds the history item.
        /// </summary>
        /// <param name="historyitem">The historyitem.</param>
        public void AddHistoryItem(HistoryItemType historyitem)
        {
            if (this.Items == null) this.Items = new ContainerItemTypeItems();
            if (this.Items.HistoryItems == null) this.Items.HistoryItems = new List<HistoryItemType>();
            this.Items.HistoryItems.Add(historyitem);
        }

        /// <summary>
        /// Adds the alarm item.
        /// </summary>
        /// <param name="alarmitem">The alarmitem.</param>
        public void AddAlarmItem(AlarmItemType alarmitem)
        {
            if (this.Items == null) this.Items = new ContainerItemTypeItems();
            if (this.Items.AlarmItems == null) this.Items.AlarmItems = new List<AlarmItemType>();
            this.Items.AlarmItems.Add(alarmitem);
        }
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class ContainerItemTypeItems
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class GetWebServiceInformationResponse1
    {
        /// <summary>
        /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
        /// or the EWS Specification on http://cws:8000
        /// </summary>
        public GetWebServiceInformationResponse1(string majorVersion, string minorVersion, string usedNameSpace, string[] supportedOperations)
        {
            this.GetWebServiceInformationResponse = new GetWebServiceInformationResponse(majorVersion, minorVersion, usedNameSpace, supportedOperations);
        }
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class GetWebServiceInformationResponse
    {
        public const string Unknown = "Unknown";

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWebServiceInformationResponse"/> class.
        /// </summary>
        public GetWebServiceInformationResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWebServiceInformationResponse"/> class.
        /// </summary>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="minorVersion">The minor version.</param>
        /// <param name="usedNameSpace">The used name space.</param>
        /// <param name="supportedOperations">The supported operations.</param>
        public GetWebServiceInformationResponse(string majorVersion, string minorVersion, string usedNameSpace, string[] supportedOperations)
        {
            this.GetWebServiceInformationVersion = new GetWebServiceInformationResponseGetWebServiceInformationVersion()
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
                UsedNameSpace = usedNameSpace
            };
            this.GetWebServiceInformationSupportedOperations = new List<string>(supportedOperations);
        }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <returns></returns>
        public string GetMajorVersion()
        {
            return this.GetWebServiceInformationVersion != null ? this.GetWebServiceInformationVersion.MajorVersion : Unknown;
        }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <returns></returns>
        public string GetMinorVersion()
        {
            return this.GetWebServiceInformationVersion.MinorVersion != null ? this.GetWebServiceInformationVersion.MinorVersion : Unknown;
        }

        /// <summary>
        /// Gets the used name space.
        /// </summary>
        /// <returns></returns>
        public string GetUsedNameSpace()
        {
            return this.GetWebServiceInformationVersion.UsedNameSpace != null ? this.GetWebServiceInformationVersion.UsedNameSpace : Unknown;
        }

        /// <summary>
        /// Gets the supported operations.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSupportedOperations()
        {
            return this.GetWebServiceInformationSupportedOperations != null ? this.GetWebServiceInformationSupportedOperations : new List<string>();
        }
    }

    public partial class GetContainerItemsRequest
    {
    }

    public partial class GetContainerItemsResponse
    {
    }

    public partial class GetContainerItemsRequest1
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class GetContainerItemsResponse1
    {
        /// <summary>
        /// Adds the container item.
        /// </summary>
        /// <param name="container">The container.</param>
        public void AddContainerItem(ContainerItemType container)
        {
            if (this.GetContainerItemsResponse == null) this.GetContainerItemsResponse = new GetContainerItemsResponse();
            if (this.GetContainerItemsResponse.GetContainerItemsItems == null) this.GetContainerItemsResponse.GetContainerItemsItems = new List<ContainerItemType>();
            this.GetContainerItemsResponse.GetContainerItemsItems.Add(container);
        }

        /// <summary>
        /// Adds the error results.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public void AddErrorResults(List<ErrorResultType> errors)
        {
            if (this.GetContainerItemsResponse == null) this.GetContainerItemsResponse = new GetContainerItemsResponse();
            this.GetContainerItemsResponse.GetContainerItemsErrorResults = errors;
        }
    }

    public partial class GetItemsRequest
    {
    }

    public partial class GetItemsResponse
    {
    }

    public partial class GetItemsRequest1
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class GetItemsResponse1
    {
        /// <summary>
        /// Setups the response.
        /// </summary>
        void SetupResponse()
        {
            if (this.GetItemsResponse == null) this.GetItemsResponse = new GetItemsResponse();
            if (this.GetItemsResponse.GetItemsItems == null) this.GetItemsResponse.GetItemsItems = new ArrayOfItemType();
        }

        /// <summary>
        /// Adds the value item.
        /// </summary>
        /// <param name="valueitem">The valueitem.</param>
        public void AddValueItem(ValueItemType valueitem)
        {
            SetupResponse();
            if (this.GetItemsResponse.GetItemsItems.ValueItems == null) this.GetItemsResponse.GetItemsItems.ValueItems = new List<ValueItemType>();
            this.GetItemsResponse.GetItemsItems.ValueItems.Add(valueitem);
        }

        /// <summary>
        /// Adds the history item.
        /// </summary>
        /// <param name="historyitem">The historyitem.</param>
        public void AddHistoryItem(HistoryItemType historyitem)
        {
            SetupResponse();
            if (this.GetItemsResponse.GetItemsItems.HistoryItems == null) this.GetItemsResponse.GetItemsItems.HistoryItems = new List<HistoryItemType>();
            this.GetItemsResponse.GetItemsItems.HistoryItems.Add(historyitem);
        }

        /// <summary>
        /// Adds the alarm item.
        /// </summary>
        /// <param name="alarmitem">The alarmitem.</param>
        public void AddAlarmItem(AlarmItemType alarmitem)
        {
            SetupResponse();
            if (this.GetItemsResponse.GetItemsItems.AlarmItems == null) this.GetItemsResponse.GetItemsItems.AlarmItems = new List<AlarmItemType>();
            this.GetItemsResponse.GetItemsItems.AlarmItems.Add(alarmitem);
        }

        /// <summary>
        /// Adds the error results.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public void AddErrorResults(List<ErrorResultType> errors)
        {
            if (this.GetItemsResponse == null) this.GetItemsResponse = new GetItemsResponse();
            this.GetItemsResponse.GetItemsErrorResults = errors;
        }
    }

    public partial class GetValuesRequest
    {
    }

    public partial class GetValuesResponse
    {
    }

    public partial class GetValuesRequest1
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class GetValuesResponse1
    {
        /// <summary>
        /// Adds the values.
        /// </summary>
        /// <param name="valueitem">The valueitem.</param>
        public void AddValues(ValueTypeStateful valueitem)
        {
            if (this.GetValuesResponse == null) this.GetValuesResponse = new GetValuesResponse();
            if (this.GetValuesResponse.GetValuesItems == null) this.GetValuesResponse.GetValuesItems = new List<ValueTypeStateful>();
            this.GetValuesResponse.GetValuesItems.Add(valueitem);
        }

        /// <summary>
        /// Adds the error results.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public void AddErrorResults(List<ErrorResultType> errors)
        {
            if (this.GetValuesResponse == null) this.GetValuesResponse = new GetValuesResponse();
            this.GetValuesResponse.GetValuesErrorResults = errors;
        }
    }

    public partial class SetValuesRequest
    {
    }

    public partial class SetValuesResponse
    {
    }

    public partial class SetValuesRequest1
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class SetValuesResponse1
    {
        /// <summary>
        /// Adds the result.
        /// </summary>
        /// <param name="result">The result.</param>
        public void AddResult(ResultType result)
        {
            if (this.SetValuesResponse == null) this.SetValuesResponse = new SetValuesResponse();
            if (this.SetValuesResponse.SetValuesResults == null) this.SetValuesResponse.SetValuesResults = new List<ResultType>();
            this.SetValuesResponse.SetValuesResults.Add(result);
        }
    }

    public partial class GetHistoryRequest
    {
    }

    public partial class GetHistoryRequestGetHistoryParameter
    {
    }

    public partial class GetHistoryRequestGetHistoryFilter
    {
    }

    public partial class GetHistoryResponse
    {
    }

    public partial class GetHistoryRequest1
    {
    }

    public partial class GetHistoryResponse1
    {
    }

    public partial class GetAlarmEventsRequest
    {
    }

    public partial class GetAlarmEventsRequestGetAlarmEventsParameter
    {
    }

    public partial class GetAlarmEventsRequestGetAlarmEventsFilter
    {
    }

    public partial class GetAlarmEventsResponse
    {
    }

    public partial class GetAlarmEventsRequest1
    {
    }

    public partial class GetAlarmEventsResponse1
    {
    }

    public partial class GetUpdatedAlarmEventsRequest
    {
    }

    public partial class GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsParameter
    {
    }

    public partial class GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsFilter
    {
    }

    public partial class GetUpdatedAlarmEventsResponse
    {
    }

    public partial class GetUpdatedAlarmEventsRequest1
    {
    }

    public partial class GetUpdatedAlarmEventsResponse1
    {
    }

    public partial class AcknowledgeAlarmEventsRequest
    {
    }

    public partial class AcknowledgeAlarmEventsResponse
    {
    }

    public partial class AcknowledgeAlarmEventsRequest1
    {
    }

    /// <summary>
    /// For all Type descriptions see also http://139.158.104.53/ecostruxurewiki/index.php?title=Corporate_Web_Services_V1.1#Corporate_Web_Service_V1.1_for_real-time_data 
    /// or the EWS Specification on http://cws:8000
    /// </summary>
    public partial class AcknowledgeAlarmEventsResponse1
    {
        /// <summary>
        /// Adds the result.
        /// </summary>
        /// <param name="result">The result.</param>
        public void AddResult(ResultType result)
        {
            if (this.AcknowledgeAlarmEventsResponse == null) this.AcknowledgeAlarmEventsResponse = new AcknowledgeAlarmEventsResponse();
            if (this.AcknowledgeAlarmEventsResponse.AcknowledgeAlarmEventsResults == null) this.AcknowledgeAlarmEventsResponse.AcknowledgeAlarmEventsResults = new List<ResultType>();
            this.AcknowledgeAlarmEventsResponse.AcknowledgeAlarmEventsResults.Add(result);
        }
    }

    public partial class GetAlarmEventTypesRequest
    {
    }

    public partial class GetAlarmEventTypesResponse
    {
    }

    public partial class GetAlarmEventTypesRequest1
    {
    }

    public partial class GetAlarmEventTypesResponse1
    {
    }
}
