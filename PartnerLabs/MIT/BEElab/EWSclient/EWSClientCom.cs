using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Xml;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using CommonFunctions.Conversion;
using ews = schneiderelectric.services.ews;
using com = CommonTypes.DataExchange;


namespace ClientSoapCom
{
    /// <summary>
    ///   Main class for using the client communication component
    /// </summary>
    public class EWSClientCom
    {
        #region Variable

        /// <summary>
        /// List of the connections
        /// </summary>
        private Dictionary<string, EwsConnection> _connectionList =  new Dictionary<string, EwsConnection>();

        /// <summary>
        /// EWS plugin error codes 
        /// </summary>
        public enum EwsStatus : int
        {
            OK = 0,
            ERROR_COMMUNICATION = -1,
            ERROR_UNKNOWN = -2,
            ERROR_PARAMETER,
            ERROR_SERVER_NOT_AVAILABLE,
            ERROR_INTERNAL_CONNECTIONLIST,
            ERROR_CONNECTION_DOESNOT_EXIST,

            WARNING_PARAMETER_NOT_SPECIFICATION_CONFORM
        }

        #endregion

        #region Properties
        #endregion

        #region Methods

        #region Plugin_Methods

        #endregion

        #region Management

        /// <summary>
        /// Checks if operation is supported by the connected server. 
        /// </summary>
        /// <param name="connectRef">Reference of the connected server to check</param>
        /// <param name="operation">Name of the operation to check</param>
        /// <returns>Returns "true" in the case operation is supported otherwise false</returns>
        public bool IsSupportedOperation(string connectRef, string operation)
        {

            return _connectionList[connectRef].Server.IsSupportedOperation(operation);
        }

        /// <summary>
        /// Get the reference keys for all open connections. 
        /// </summary>
        /// <param name="connectRefsList">Returns a list of all connection references</param>
        /// <returns>Returns status/error code</returns>
        public int ConnectionAllReferences(out List<string> connectRefsList) 
        {
            int status = (int)EwsStatus.OK;

            // Get all reference keys in the connection list
            connectRefsList = new List<string>(_connectionList.Keys);
            if (connectRefsList == null)
                status = (int)EwsStatus.ERROR_INTERNAL_CONNECTIONLIST;

            return status;
        }

        /// <summary>
        /// Get information about a connection binding.
        /// </summary>
        /// <param name="connectRef">The connect ref.</param>
        /// <param name="binding">Returns info about the binding</param>
        /// <returns>Returns status/error code</returns>
        public int ConnectionBindingInfo(string connectRef, out com.EwsBindingConfig binding) 
        {
            int status = (int)EwsStatus.OK;

            // Get item from connection list
            EwsConnection connection = _connectionList[connectRef];
            binding = connection.Binding;

            return status;
        }

        /// <summary>
        /// Get information about a connected server (internal stored data).
        /// </summary>
        /// <param name="connectRef">The connect ref.</param>
        /// <param name="server">Returns info about the connected server</param>
        /// <returns>Returns status/error code</returns>
        public int ConnectionServerInfo(string connectRef, out com.EwsServerInfo server)
        {
            int status = (int)EwsStatus.OK;

            // Get item from connection list
            EwsConnection connection = _connectionList[connectRef];
            server = connection.Server;

            return status;
        }

        /// <summary>
        /// Do a disconnect for all open connections. 
        /// </summary>
        /// <returns>Returns status/error code</returns>
        public int CloseAllConnections()
        {
            int status = (int)EwsStatus.OK;

            // Get item from connection list
            List<string> connectRefsList = new List<string>(_connectionList.Keys);

            // Perfrom disconnect for all open connection
            foreach (string key in connectRefsList)
            {
                status = Disconnect(key);
            }
            return status;
        }

        #endregion

        #region Communication

        /// <summary>
        /// Connect with default communication values and no security. 
        /// </summary>
        /// <param name="address">Address of the server</param>
        /// <param name="connectRef">Returns reference id of the new connection</param>
        /// <returns>Returns status/error code</returns>
        public int ConnectEx(string address, out string connectRef)
        {
            int status = (int)EwsStatus.OK;

            // Set security to anonymous
            com.EwsSecurity security= new com.EwsSecurity() 
            { 
                AuthenticationScheme = AuthenticationSchemes.Anonymous,
                Password = null,
                Username =  null
            };

            // Call main connect method
            status = ConnectEx(address, security, out connectRef);

            return status;
        }

        /// <summary>
        /// Connect with default communication values. 
        /// </summary>
        /// <param name="address">Address of the server</param>
        /// <param name="security">Security information for the connection</param>
        /// <param name="connectRef">Returns reference id of the new connection</param>
        /// <returns>Returns status/error code</returns>
        public int ConnectEx(string address, com.EwsSecurity security, out string connectRef)
        {
            int status = (int)EwsStatus.OK;

            // Set default values for connection
            com.EwsBindingConfig config = new com.EwsBindingConfig()
            {
                MessageVersion = MessageVersion.Soap12,
                AllowCookies = true,
                AuthenticationScheme = AuthenticationSchemes.Anonymous,

                MaxBufferPoolSize = 524288,
                MaxBufferSize = 65536,
                MaxReceivedMessageSize = 65536,

                OpenTimeout = 10,
                ReceiveTimeout = 10,
                SendTimeout = 10,
                CloseTimeout = 10
            };

            // Call main connect method
            status = ConnectEx(address, security, config, out connectRef);

            return status;
        }

        /// <summary>
        /// Connect to a server. 
        /// </summary>
        /// <param name="address">Address of the server</param>
        /// <param name="security">Security information for the connection</param>
        /// <param name="config">Binding configuration information for the connection</param>
        /// <param name="connectRef">Returns reference id of the new c onnection</param>
        /// <returns>int ; Returns status/error code</returns>
        public int ConnectEx(string address, com.EwsSecurity security, com.EwsBindingConfig config, out string connectRef)
        {
            int status = (int)EwsStatus.OK;
            status = ConnectEx(address, security, config, null, out connectRef);
            return status;
        
        }

        /// <summary>
        /// Connect to the server with some content tolerance. 
        /// </summary>
        /// <param name="address">Address of the server</param>
        /// <param name="security">Security information for the connection</param>
        /// <param name="config">Binding configuration information for the connection</param>
        /// <param name="tolerance">Tolerance conditions for the connection</param>
        /// <param name="connectRef">Returns reference id of the new connection</param>
        /// <returns>int ; Returns status/error code</returns>
        public int ConnectEx(string address, com.EwsSecurity security, com.EwsBindingConfig config, com.EwsContentTolerance tolerance, out string connectRef)        
        {
            EwsConnection connection = new EwsConnection();
            connectRef = null;
            
            ews.DataExchangeClient client = null;
            config.AuthenticationScheme = security.AuthenticationScheme;
            var binding = CreateCustomBinding(config);
            client = new ews.DataExchangeClient(binding, new EndpointAddress(address));

            // Set security parameter for the connection
            if (security.AuthenticationScheme == System.Net.AuthenticationSchemes.Basic)
            {
                client.ClientCredentials.UserName.UserName = security.Username;
                client.ClientCredentials.UserName.Password = security.Password;
            }
            else if (security.AuthenticationScheme == System.Net.AuthenticationSchemes.Digest)
            {
                client.ClientCredentials.HttpDigest.ClientCredential = new System.Net.NetworkCredential(security.Username, security.Password);
                client.ClientCredentials.HttpDigest.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            }

            // Check if there is some dependencies conecning the rules of the EWS message content
            if (tolerance != null)
            {
                // MessageInspector for forcing the namespace of the response
                if (tolerance.IgnoreNamespace)
                    client.Endpoint.Behaviors.Add(new RewriteNamespaceBehavior(config.MessageVersion));
            }
            else 
            {
                tolerance = new com.EwsContentTolerance() 
                { 
                    IgnoreNamespace =  false,
                    StrictContentProcessing = false
                };
            }

            // Check if server is online and get the information about the server
            var response = client.GetWebServiceInformation(new ews.GetWebServiceInformationRequest());

            if (response != null && response.GetMajorVersion() != "Unknown")
            {
                // Set server information
                connection.Server = new com.EwsServerInfo();
                connection.Server.EndpointAddress = address;
                connection.Server.MajorVersion = response.GetMajorVersion();
                connection.Server.MinorVersion = response.GetMinorVersion();
                connection.Server.TargetNamespace = response.GetUsedNameSpace();
                connection.Server.SetSupportedOperations(response.GetSupportedOperations());

                // Set connection parameters
                connection.Security = security;
                connection.Binding = config;
                connection.Client = client;
                connection.Tolerance = tolerance;

                // Create reference id and add the connection to the list
                connectRef = Guid.NewGuid().ToString();
                connection.ConnectReference = connectRef;
                _connectionList.Add(connectRef, connection);
            }
            else 
            {
                return (int)EwsStatus.ERROR_SERVER_NOT_AVAILABLE; 
            }

            return (int)EwsStatus.OK;
        }

        /// <summary>
        /// Disconnect from a server. 
        /// </summary>
        /// <param name="connectRef">Reference to the connection</param>
        /// <returns>Returns status/error code</returns>
        public int Disconnect(string connectRef)
        {
            int status = (int)EwsStatus.OK;
            Boolean connectionExist = false;
            
            // Checks if connect still exists
            foreach (var item in _connectionList.Keys)
            {
                if (item == connectRef)
                    connectionExist = true;
            }

            if (connectionExist)
            {
                // Get connection instance
                EwsConnection connection = _connectionList[connectRef];
                ews.DataExchangeClient client = connection.Client;

                // Close connection
                if (client != null)
                {
                    if (client.State != CommunicationState.Closed)
                    {
                        client.Close();
                    }
                    client = null;

                    if (!_connectionList.Remove(connectRef))
                        status = (int)EwsStatus.ERROR_INTERNAL_CONNECTIONLIST;
                    else
                        status = (int)EwsStatus.OK;
                }
                else
                {
                    status = (int)EwsStatus.ERROR_CONNECTION_DOESNOT_EXIST;
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_CONNECTION_DOESNOT_EXIST;
            }
            return status;
        }

        /// <summary>
        /// Creates the binfing for a connection. 
        /// </summary>
        /// <param name="config">Contains the binding configuration parameter</param>
        /// <returns>Binding; Returns the newly created binding</returns>
        private Binding CreateCustomBinding(com.EwsBindingConfig config)
        {
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = config.AuthenticationScheme;
            httpBinding.AllowCookies = config.AllowCookies;
            httpBinding.DecompressionEnabled = true;
            httpBinding.KeepAliveEnabled = true;

            //httpBinding.UseDefaultWebProxy = false;
            //httpBinding.BypassProxyOnLocal = true;
            httpBinding.MaxBufferPoolSize = config.MaxBufferPoolSize;
            httpBinding.MaxBufferSize = config.MaxBufferSize;
            httpBinding.MaxReceivedMessageSize = config.MaxReceivedMessageSize;

            TextMessageEncodingBindingElement txtMsgEncoding = new TextMessageEncodingBindingElement();
            txtMsgEncoding.MessageVersion = config.MessageVersion; // default Soap12
            txtMsgEncoding.WriteEncoding = Encoding.UTF8;

            CustomBinding binding = new CustomBinding(txtMsgEncoding, httpBinding);

            binding.SendTimeout = TimeSpan.FromSeconds(config.SendTimeout);
            binding.OpenTimeout = TimeSpan.FromSeconds(config.OpenTimeout);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(config.ReceiveTimeout);
            binding.CloseTimeout = TimeSpan.FromSeconds(config.CloseTimeout);

            return binding;
        }

        #endregion

        #region EWS_Services

        /// <summary>
        /// Gets WebService Information from server. 
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="info">List of item Ids to read their values of</param>
        /// <returns>Returns status/error code</returns>
        public int GetWebServiceInformation(string connectRef, out com.EwsServerInfo info) 
        { 
            int status = (int)EwsStatus.OK;
            List<string> messages = new List<string>();

            info = new com.EwsServerInfo();

            // Get connection instance
            EwsConnection connection = _connectionList[connectRef];
            ews.DataExchangeClient client = connection.Client;

            try
            {
                // Communicate with server
                var response = client.GetWebServiceInformation(new ews.GetWebServiceInformationRequest());

                info.MajorVersion = response.GetMajorVersion();
                info.MinorVersion = response.GetMinorVersion();
                info.TargetNamespace = response.GetUsedNameSpace();
                info.EndpointAddress = connection.Server.EndpointAddress;
                info.SetSupportedOperations(response.GetSupportedOperations());
            }
            catch (Exception ex)
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
                // TODO: Log error message
            }

            return status;
        }

        /// <summary>
        /// Gets containers form server. 
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="containerIds">List of item Ids to read the value</param>
        /// <param name="containerList">Returns parameter for the read containers</param>
        /// <param name="errorList">Returns parameter for the read error list</param>
        /// <returns>Returns status/error code</returns>
        public int GetContainerItems(string connectRef, List<string> containerIds, out List<com.ContainerItemType> containerList, out List<com.ErrorResultType> errorList)
        {
            int status = (int)EwsStatus.OK;
            
            containerList =  new List<com.ContainerItemType>();
            errorList =  new List<com.ErrorResultType>();
        
            // Communicate with server
            var response = _connectionList[connectRef].Client.GetContainerItems(new ews.GetContainerItemsRequest()
            {
                GetContainerItemsIds = containerIds
            });

            if (response != null)
            {
                foreach (var item in response.GetContainerItemsErrorResults)
                {
                    errorList.Add(new com.ErrorResultType() {
                        Id = item.Id,
                        Message = item.Message
                        });
                }

                foreach (var item in response.GetContainerItemsItems)
                {
                    com.ContainerItemType container = new com.ContainerItemType()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Type = item.Type,
                        Items = new com.ContainerItemTypeItems() 
                    };

                    container.Items.AlarmItems = new List<com.AlarmItemType>();
                    container.Items.ContainerItems = new List<com.ContainerItemSimpleType>();
                    container.Items.HistoryItems = new List<com.HistoryItemType>();
                    container.Items.ValueItems = new List<com.ValueItemTypeBase>();

                    if (item.Items != null)
                    {
                        if (item.Items.AlarmItems != null)
                        {
                            foreach (var entry in item.Items.AlarmItems)
                            {
                                container.Items.AlarmItems.Add(new com.AlarmItemType()
                                {

                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Description = entry.Description,
                                    State = Conversions.BooleanToInt(entry.State, _connectionList[connectRef].Tolerance.StrictContentProcessing, entry.Id),
                                    ValueItemId = entry.ValueItemId
                                });
                            }
                        }

                        if (item.Items.ContainerItems != null)
                        {
                            foreach (var entry in item.Items.ContainerItems)
                            {
                                container.Items.ContainerItems.Add(new com.ContainerItemSimpleType()
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Description = entry.Description,
                                    Type = entry.Type
                                });
                            }
                        }

                        if (item.Items.HistoryItems != null)
                        {
                            foreach (var entry in item.Items.HistoryItems)
                            {
                                container.Items.HistoryItems.Add(new com.HistoryItemType()
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Description = entry.Description,
                                    Type = entry.Type,
                                    Unit = entry.Unit,
                                    ValueItemId = entry.ValueItemId
                                });
                            }
                        }

                        if (item.Items.ValueItems != null)
                        {
                            foreach (var entry in item.Items.ValueItems)
                            {
                                container.Items.ValueItems.Add(new com.ValueItemTypeBase()
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Description = entry.Description,
                                    Type = entry.Type,
                                    Unit = entry.Unit,
                                    Writeable = Int32.Parse(entry.Writeable)
                                });
                            }
                        }
                    }
                    else
                    {
                        // TODO: Logging which parameter is not specification conform 
                        if(_connectionList[connectRef].Tolerance.StrictContentProcessing)
                            throw new Exception(String.Format("Parameter does not conform to the specification\nElement: items in ContainerItemType {0}\nContent processing error: NULL value\n\nUse Paramter NO STRICT CONTENT PROCESSING", item.Id));   
                    }

                    // Add container to response parameter
                    containerList.Add(container);

                }

            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Gets items from server. 
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="itemIds">List of item Ids to read</param>
        /// <param name="itemList">Returns parameter for the read items</param>
        /// <param name="errorList">Returns parameter for the read error list</param>
        /// <returns>int, Returns status/error code</returns>
        public int GetItems(string connectRef, List<string> itemIds, out com.ArrayOfItemType itemList, out List<com.ErrorResultType> errorList)
        {
            int status = (int)EwsStatus.OK;
            itemList = new com.ArrayOfItemType();
            errorList = new List<com.ErrorResultType>();

            // Communicate with server
            var response = _connectionList[connectRef].Client.GetItems(new ews.GetItemsRequest()
            {
                GetItemsIds = itemIds
            });

            if (response != null)
            {
                // Copy data from ews structures in common structures of the host
                foreach (ews.ErrorResultType error in response.GetItemsErrorResults)
                {
                    errorList.Add(new com.ErrorResultType()
                    {
                        Id = error.Id,
                        Message = error.Message
                    });
                }

                // Copy data from ews structures in common structures of the host
                itemList.AlarmItems = new List<com.AlarmItemType>();
                foreach (var entry in response.GetItemsItems.AlarmItems)
                {
                    itemList.AlarmItems.Add(new com.AlarmItemType()
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        Description = entry.Description,
                        State = Conversions.BooleanToInt(entry.State, _connectionList[connectRef].Tolerance.StrictContentProcessing, entry.Id),
                        ValueItemId = entry.ValueItemId
                    });
                }

                itemList.HistoryItems = new List<com.HistoryItemType>();
                foreach (var entry in response.GetItemsItems.HistoryItems)
                {
                    itemList.HistoryItems.Add(new com.HistoryItemType()
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        Description = entry.Description,
                        Type = entry.Type,
                        Unit = entry.Unit,
                        ValueItemId = entry.ValueItemId
                    });
                }

                itemList.ValueItems = new List<com.ValueItemType>();
                foreach (var entry in response.GetItemsItems.ValueItems)
                {
                    itemList.ValueItems.Add(new com.ValueItemType()
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        Description = entry.Description,
                        State = Conversions.BooleanToInt(entry.State, _connectionList[connectRef].Tolerance.StrictContentProcessing, entry.Id),
                        Type = entry.Type,
                        Unit = entry.Unit,
                        Value = entry.Value,
                        Writeable = Int32.Parse(entry.Writeable)
                    });
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }
            return status;
        }

        /// <summary>
        /// Gets values for a defined list of items.
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="valueItemIds">List of item Ids to read the value</param>
        /// <param name="values">Returns parameter for the read values</param>
        /// <param name="errorList">The error list.</param>
        /// <returns>Returns status/error code</returns>
        public int GetValues(string connectRef, List<string> valueItemIds, out List<com.ValueTypeStateful> values, out List<com.ErrorResultType> errorList)
        {
            int status = (int)EwsStatus.OK;
            values =  new List<com.ValueTypeStateful>();
            errorList = new List<com.ErrorResultType>();

            // Communicate with server
            var response = _connectionList[connectRef].Client.GetValues(new ews.GetValuesRequest()
            {
                GetValuesIds = valueItemIds
            });

            if (response != null)
            {
                // Copy data from EWS structures in common structures of the host
                foreach(ews.ErrorResultType error in response.GetValuesErrorResults)
                {
                    errorList.Add(new com.ErrorResultType() {
                            Id = error.Id,
                            Message = error.Message
                        });
                }

                // Copy data from EWS structures in common structures of the host
                foreach(ews.ValueTypeStateful value in response.GetValuesItems)
                {
                    values.Add(new com.ValueTypeStateful() {
                            Id = value.Id,
                            State = Conversions.BooleanToInt(value.State, _connectionList[connectRef].Tolerance.StrictContentProcessing, value.Id),
                            Value = value.Value
                        });
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Write values from a list of items
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="writeItemList">List of items to write</param>
        /// <param name="resultList">The result list.</param>
        /// <returns>Returns status/error code</returns>
        public int SetValue(string connectRef, List<com.ValueTypeStateless> writeItemList, out List<com.ResultType> resultList)
        {
            int status = (int)EwsStatus.OK;
            
            // Copy request data to EWS com specific structure
            List<ews.ValueTypeStateless> values = new List<ews.ValueTypeStateless>();
            foreach (com.ValueTypeStateless item in writeItemList)
            {
                values.Add(new ews.ValueTypeStateless() {
                        Id = item.Id,
                        Value = item.Value
                    });
            }

            resultList = new List<com.ResultType>();

            // Communicate with server
            var response = _connectionList[connectRef].Client.SetValues(new ews.SetValuesRequest()
                {
                    SetValuesItems = values
                });

            // Copy response data to common structure
            if (response.SetValuesResults != null && response.SetValuesResults.Count > 0)
            {
                foreach(ews.ResultType result in response.SetValuesResults)
                {
                    resultList.Add(new com.ResultType() { 
                            Id =  result.Id,
                            Message = result.Message,
                            Success = result.Success
                        });
                    
                }
            }
            else 
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Read history of an item 
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="param">id of the item to read the history</param>
        /// <param name="filter">Specified the time range of the history</param>
        /// <param name="statusResp"> Return parameter for the status</param>
        /// <param name="history"> Return parameter for the history</param>
        /// <returns>Returns status/error code</returns>
        public int GetHistory(string connectRef, com.HistoryParameter param, com.HistoryFilter filter, out com.HistoryResponseStatusType statusResp, out com.HistoryRecordsType history)
        {
            int status = (int)EwsStatus.OK;
            statusResp =  new com.HistoryResponseStatusType();
            history = new com.HistoryRecordsType();

            // Check input parameter
            
            if(filter.TimeFrom == null && filter.TimeTo == null)
            {
                filter.TimeFromSpecified =  false;
                filter.TimeToSpecified =  false;
            }
            else if(filter.TimeFrom != null && filter.TimeTo != null)
            {
                filter.TimeFromSpecified =  true;
                filter.TimeToSpecified =  true;
            }
            else
            {
                return (int)EwsStatus.ERROR_PARAMETER;
            }
                
            if(param.Id == null && param.MoreDataRef == null)
                return (int)EwsStatus.ERROR_PARAMETER;

            // Communicate with server
            var response = _connectionList[connectRef].Client.GetHistory(new ews.GetHistoryRequest()
            {
                GetHistoryParameter = new ews.GetHistoryRequestGetHistoryParameter()
                {
                    Id = param.Id,
                    MoreDataRef = param.MoreDataRef
                },
                GetHistoryFilter = new ews.GetHistoryRequestGetHistoryFilter()
                {
                    TimeFrom = filter.TimeFrom,
                    TimeFromSpecified = filter.TimeFromSpecified,
                    TimeTo = filter.TimeTo,
                    TimeToSpecified = filter.TimeToSpecified
                }
            });

            if (response != null)
            {
                // Copy data from EWS structures in common structures of the host
                statusResp.MoreDataAvailable = response.GetHistoryResponseStatus.MoreDataAvailable;
                statusResp.MoreDataRef = response.GetHistoryResponseStatus.MoreDataRef;
                statusResp.TimeFrom = response.GetHistoryResponseStatus.TimeFrom;
                statusResp.TimeTo = response.GetHistoryResponseStatus.TimeTo;

                history.Type = response.GetHistoryHistoryRecords.Type;
                history.Unit = response.GetHistoryHistoryRecords.Unit;
                history.ValueItemId = response.GetHistoryHistoryRecords.ValueItemId;
                history.List = new List<com.HistoryRecordType>(); ;
                foreach (ews.HistoryRecordType record in response.GetHistoryHistoryRecords.List)
                {
                    history.List.Add(new com.HistoryRecordType()
                    {
                        State = Int32.Parse(record.State),
                        TimeStamp = record.TimeStamp,
                        Value = record.Value
                    });
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Read alarms form a server 
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="param">parameter to get next table of the alarmlist</param>
        /// <param name="filter">Specified the some filter parameters for alarms</param>
        /// <param name="statusResp"> Return paramater for the status</param>
        /// <param name="alarmList"> Return paramater about the list of alarms</param>
        /// <returns>Returns status/error code</returns>
        public int GetAlarmEvents(string connectRef, com.AlarmEventsParameter param, com.AlarmEventsFilter filter, out com.AlarmResponseStatusType statusResp, out List<com.AlarmEventsType> alarmList)
        {
            int status = (int)EwsStatus.OK;
            statusResp = new com.AlarmResponseStatusType();
            alarmList = new List<com.AlarmEventsType>();

            if (filter == null) 
            {
                filter = new com.AlarmEventsFilter();
                filter.PriorityFrom = 0;
                filter.PriorityTo = 1000;
                filter.Types = new List<string>();
            }
            // Communicate with server
            var response = _connectionList[connectRef].Client.GetAlarmEvents(new ews.GetAlarmEventsRequest()
            {
                GetAlarmEventsFilter = new ews.GetAlarmEventsRequestGetAlarmEventsFilter()
                {
                    PriorityFrom = filter.PriorityFrom.ToString(),
                    PriorityTo = filter.PriorityTo.ToString(),
                    Types = filter.Types
                },
                GetAlarmEventsParameter = new ews.GetAlarmEventsRequestGetAlarmEventsParameter()
                {
                    MoreDataRef = param.MoreDataRef
                }
            });

            if (response != null)
            {
                // Copy data from EWS structures in common structures of the host
                statusResp.MoreDataAvailable = response.GetAlarmEventsResponseStatus.MoreDataAvailable;
                statusResp.MoreDataRef = response.GetAlarmEventsResponseStatus.MoreDataRef;
                statusResp.LastUpdate = response.GetAlarmEventsResponseStatus.LastUpdate;
                statusResp.NeedsRefresh = response.GetAlarmEventsResponseStatus.NeedsRefresh;

                
                foreach (ews.AlarmEventsType alarm in response.GetAlarmEventsAlarmEvents)
                {
                    alarmList.Add(new com.AlarmEventsType()
                    {
                        Acknowledgeable = Conversions.BooleanToInt(alarm.Acknowledgeable, _connectionList[connectRef].Tolerance.StrictContentProcessing, alarm.ID),
                        ID =  alarm.ID,
                        Message =  alarm.Message,
                        Priority =  Int32.Parse(alarm.Priority),
                        SourceID =  alarm.SourceID,
                        SourceName =  alarm.SourceName,
                        State = Int32.Parse(alarm.State),
                        TimeStampOccurrence =  alarm.TimeStampOccurrence,
                        TimeStampTransition =  alarm.TimeStampTransition,
                        Type =  alarm.Type
                    });
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Read updated alarms form a server 
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="param">parameter to get next table of the alarmlist</param>
        /// <param name="filter">Specified the some filter parameters for alarms</param>
        /// <param name="statusResp"> Return paramater for the status</param>
        /// <param name="alarmList"> Return paramater about the list of alarms</param>
        /// <returns>Returns status/error code</returns>
        public int GetUpdatedAlarmEvents(string connectRef, com.UpdatedAlarmEventsParameter param, com.AlarmEventsFilter filter, out com.AlarmResponseStatusType statusResp, out List<com.AlarmEventsType> alarmList)
        {
            int status = (int)EwsStatus.OK;
            statusResp = new com.AlarmResponseStatusType();
            alarmList = new List<com.AlarmEventsType>();

            // Communicate with server
            var response = _connectionList[connectRef].Client.GetUpdatedAlarmEvents(new ews.GetUpdatedAlarmEventsRequest()
            {
                GetUpdatedAlarmEventsFilter = new ews.GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsFilter()
                {
                    PriorityFrom = filter.PriorityFrom.ToString(),
                    PriorityTo = filter.PriorityTo.ToString(),
                    Types = filter.Types
                },
                GetUpdatedAlarmEventsParameter = new ews.GetUpdatedAlarmEventsRequestGetUpdatedAlarmEventsParameter()
                {
                    LastUpdate =  param.LastUpdate,
                    MoreDataRef = param.MoreDataRef
                }
            });

            if (response != null)
            {
                // Copy data from EWS structures in common structures of the host
                statusResp.MoreDataAvailable = response.GetUpdatedAlarmEventsResponseStatus.MoreDataAvailable;
                statusResp.MoreDataRef = response.GetUpdatedAlarmEventsResponseStatus.MoreDataRef;
                statusResp.LastUpdate = response.GetUpdatedAlarmEventsResponseStatus.LastUpdate;
                statusResp.NeedsRefresh = response.GetUpdatedAlarmEventsResponseStatus.NeedsRefresh;


                foreach (ews.AlarmEventsType alarm in response.GetUpdatedAlarmEventsAlarmEvents)
                {
                    alarmList.Add(new com.AlarmEventsType()
                    {
                        Acknowledgeable = Conversions.BooleanToInt(alarm.Acknowledgeable, _connectionList[connectRef].Tolerance.StrictContentProcessing, alarm.ID),
                        ID = alarm.ID,
                        Message = alarm.Message,
                        Priority = Int32.Parse(alarm.Priority),
                        SourceID = alarm.SourceID,
                        SourceName = alarm.SourceName,
                        State = Int32.Parse(alarm.State),
                        TimeStampOccurrence = alarm.TimeStampOccurrence,
                        TimeStampTransition = alarm.TimeStampTransition,
                        Type = alarm.Type
                    });
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Acknowledge alarm events in a server
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="alarmItemIds">List of items to write</param>
        /// <param name="resultList">The result list.</param>
        /// <returns>Returns status/error code</returns>
        public int AcknowledgeAlarmEvent(string connectRef, List<string> alarmItemIds, out List<com.ResultType> resultList)
        {
            int status = (int)EwsStatus.OK;
            resultList = new List<com.ResultType>();

            // Communicate with server
            var response = _connectionList[connectRef].Client.AcknowledgeAlarmEvents(new ews.AcknowledgeAlarmEventsRequest()
            {
                AcknowledgeAlarmEventsIds = alarmItemIds
            });

            if (response != null && response.AcknowledgeAlarmEventsResults.Count > 0)
            {
                // Copy data from EWS structures in common structures of the host
                foreach (ews.ResultType result in response.AcknowledgeAlarmEventsResults)
                {
                    resultList.Add(new com.ResultType()
                    {
                        Id = result.Id,
                        Message = result.Message,
                        Success = result.Success
                    });
                }
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }

            return status;
        }

        /// <summary>
        /// Returns the list of the alarm event types in the server
        /// </summary>
        /// <param name="connectRef">Reference to the server</param>
        /// <param name="types"> Return paramater conatins the list of alarm event types</param>
        /// <returns>Returns status/error code</returns>
        public int GetAlarmEventTypes(string connectRef, out List<string> types)
        {
            int status = (int)EwsStatus.OK;
            types = new List<string>();

            // Communicate with server
            var response = _connectionList[connectRef].Client.GetAlarmEventTypes(new ews.GetAlarmEventTypesRequest());

            if (response != null)
            {
                // Copy data from EWS structures in common structures of the host
                types = response.GetAlarmEventTypesTypes;
            }
            else
            {
                status = (int)EwsStatus.ERROR_COMMUNICATION;
            }
            return status;
        }

        #endregion

        #endregion
    }

   
}
