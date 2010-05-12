using System;
using System.Xml;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServerEngine.Drivers.Setup;
using Library.LabServerEngine.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public partial class DriverFullLoad : DriverEquipmentGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverFullLoad";

        //
        // Local variables
        //
        private int measurementCount;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverFullLoad(EquipmentService equipmentServiceProxy, Configuration configuration)
            : this(equipmentServiceProxy, configuration, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverFullLoad(EquipmentService equipmentServiceProxy, Configuration configuration, CancelExperiment cancelExperiment)
            : base(equipmentServiceProxy, configuration, cancelExperiment)
        {
            const string STRLOG_MethodName = "DriverFullLoad";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Save for use by this driver
            //
            this.measurementCount = configuration.MeasurementCount;

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        private XmlDocument CreateXmlRequestDocument(string command)
        {
            return CreateXmlRequestDocument(command, null);
        }

        //-------------------------------------------------------------------------------------------------//

        private XmlDocument CreateXmlRequestDocument(string command, string[,] args)
        {
            XmlDocument xmlDocument = null;

            try
            {
                xmlDocument = new XmlDocument();
                XmlElement xmlElement = xmlDocument.CreateElement(LabServerEngine.Consts.STRXML_Request);
                xmlDocument.AppendChild(xmlElement);

                //
                // Add command
                //
                xmlElement = xmlDocument.CreateElement(LabServerEngine.Consts.STRXML_Command);
                xmlElement.InnerText = command;
                xmlDocument.DocumentElement.AppendChild(xmlElement);

                //
                // Add arguments
                //
                if (args != null)
                {
                    for (int i = 0; i < args.GetLength(0); i++)
                    {
                        xmlElement = xmlDocument.CreateElement(args[i,0]);
                        xmlElement.InnerText = args[i, 1];
                        xmlDocument.DocumentElement.AppendChild(xmlElement);
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return xmlDocument;
        }

        //-------------------------------------------------------------------------------------------------//

        private XmlNode CreateXmlResponseNode(string xmlResponse)
        {
            XmlDocument xmlDocument = null;
            XmlNode xmlResponseNode = null;

            try
            {
                xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlResponse);
                xmlResponseNode = XmlUtilities.GetXmlRootNode(xmlDocument, LabServerEngine.Consts.STRXML_Response);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return xmlResponseNode.Clone();
        }

    }
}
