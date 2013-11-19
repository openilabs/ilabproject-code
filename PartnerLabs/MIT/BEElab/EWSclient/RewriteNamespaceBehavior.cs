using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;
using System.IO;
using ews = schneiderelectric.services.ews;

namespace ClientSoapCom
{
    /// <summary>
    ///  Deals with the responses' namespace
    ///  Implements the IEndpointBehavior interface. All methods have to be implemented, but several methods are implemented empty.
    ///  
    /// </summary>
    public class RewriteNamespaceBehavior : IEndpointBehavior
    {
        private MessageVersion _messageVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="RewriteNamespaceBehavior"/> class.
        /// </summary>
        /// <param name="messageVersion">The message version. None,Soap11,Soap12,....</param>
        public RewriteNamespaceBehavior(MessageVersion messageVersion)
        {
            this._messageVersion = messageVersion;
        }

        #region IEndpointBehavior Members

        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new RewriteNamespaceMessageInspector(_messageVersion));
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }

    /// <summary>
    /// Implements IClientMessageInspector interface to view or modify messages.
    /// Can be added to the MessageInspectors collection to view or modify messages. 
    /// </summary>
    public class RewriteNamespaceMessageInspector : IClientMessageInspector
    {
        private MessageVersion _messageVersion;
        private string _envelopeNamespace;
        private string _targetNamespace = ews.DataExchangeConstants.Namespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="RewriteNamespaceMessageInspector"/> class.
        /// </summary>
        /// <param name="messageVersion">The message version.</param>
        public RewriteNamespaceMessageInspector(MessageVersion messageVersion)
        {
            MessageVersion = messageVersion;
        }

        #region IClientMessageInspector Members

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            string action = string.Empty;
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms);
            reply.WriteMessage(writer);
            writer.Flush();

            ms.Position = 0;

            ms = RewriteMessage(ms);

            ms.Position = 0;

            XmlReader reader = XmlReader.Create(ms);
            reply = Message.CreateMessage(reader, int.MaxValue, reply.Version);
        }

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The  client object channel.</param>
        /// <returns>
        /// The object that is returned as the <paramref name="correlationState "/>argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> to ensure that no two <paramref name="correlationState"/> objects are the same.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// Rewrites the message.
        /// </summary>
        /// <param name="ms">The memory stream.</param>
        /// <returns></returns>
        private MemoryStream RewriteMessage(MemoryStream ms)
        {
            MemoryStream ms2 = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms2, new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                OmitXmlDeclaration = true
            });
            int state = 0;

            ms.Position = 0;
            XmlReader reader = XmlReader.Create(ms);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        writer.WriteStartDocument(true);
                        break;
                    case XmlNodeType.Element:
                        if (state == 0 && reader.LocalName == "Envelope" && reader.NamespaceURI == _envelopeNamespace)
                        {
                            writer.WriteStartElement(reader.LocalName, reader.NamespaceURI);
                            state = 1;
                        }
                        else if (state == 1 && reader.LocalName == "Body" && reader.NamespaceURI == _envelopeNamespace)
                        {
                            writer.WriteStartElement(reader.LocalName, reader.NamespaceURI);
                            state = 2;
                        }
                        else if (state == 2)
                        {
                            writer.WriteStartElement(reader.LocalName, _targetNamespace);
                        }
                        else
                            writer.WriteStartElement(reader.LocalName, reader.NamespaceURI);

                        break;
                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        if (state == 2 && reader.LocalName == "Body")
                            state = 3;
                        writer.WriteEndElement();
                        break;
      
                }
            }
            writer.Flush();
            ms2.Position = 0;
            return ms2;
        }

        /// <summary>
        /// Gets or sets the message version and accordingly sets the envelope namespace 
        /// </summary>
        /// <value>The message version. Soap11 or , Soap12</value>
        public MessageVersion MessageVersion
        {
            get
            {
                return this._messageVersion;
            }
            set
            {
                this._messageVersion = value;

                if (this._messageVersion == MessageVersion.Soap11 || this._messageVersion == MessageVersion.Soap11WSAddressing10)
                    this._envelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
                if (this._messageVersion == MessageVersion.Soap12 || this._messageVersion == MessageVersion.Soap12WSAddressing10)
                    this._envelopeNamespace = "http://www.w3.org/2003/05/soap-envelope";
            }
        }
    }
}
