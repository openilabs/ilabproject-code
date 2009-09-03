using System;
using System.Xml;
using Library.Lab;

namespace Library.LabServerEngine
{
    public class ExperimentSpecification
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "ExperimentSpecification";

        //
        // String constants for error messages
        //
        private const string STRERR_LabConfiguration = "labConfiguration";
        private const string STRERR_XmlSpecification = "xmlSpecification";
        private const string STRERR_SetupIdInvalid = "Setup ID is invalid!";

        //
        // Constants
        //
        private const int EXECUTION_TIME = 15;

        //
        // Local variables
        //
        private LabConfiguration labConfiguration;

        //
        // Local variables available to a derived class
        //
        protected XmlNode xmlNodeConfiguration;
        protected XmlNode xmlNodeSpecification;

        #endregion

        #region Properties

        protected string setupId;
        protected XmlNode xmlNodeSetup;

        public string SetupId
        {
            get { return this.setupId; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public ExperimentSpecification(LabConfiguration labConfiguration)
        {
            const string STRLOG_MethodName = "ExperimentSpecification";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            // Save the lab configuration for use by the Parse() method
            this.labConfiguration = labConfiguration;

            try
            {
                //
                // Load XML specification string from the lab configuration and save a copy of the XML node
                //
                XmlDocument xmlDocument = XmlUtilities.GetXmlDocument(labConfiguration.XmlSpecification);
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, Consts.STRXML_experimentSpecification);
                this.xmlNodeSpecification = xmlRootNode.Clone();

                //
                // Check that all required XML specification nodes exist
                //
                XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_setupId, true);

                //
                // Load XML configuration string from the lab configuration and save a copy of the XML node
                //
                xmlDocument = XmlUtilities.GetXmlDocument(labConfiguration.XmlConfiguration);
                xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, Consts.STRXML_configuration);
                this.xmlNodeConfiguration = xmlRootNode.Clone();
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw;
            }

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Parse the XML specification string to check its validity. No exceptions are thrown back to the
        /// calling method. If an error occurs, 'accepted' is set to false and the error message is placed
        /// in 'errorMessage' where it can be examined by the calling method. Return 'accepted'.
        /// </summary>
        /// <param name="xmlSpecification"></param>
        public virtual ValidationReport Parse(string xmlSpecification)
        {
            const string STRLOG_MethodName = "Parse";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Create a new validation report ready to fill in
            //
            ValidationReport validationReport = new ValidationReport();

            //
            // Process the XML specification string
            //
            try
            {
                // Load XML specification string
                XmlDocument xmlDocument = XmlUtilities.GetXmlDocument(xmlSpecification);

                //
                // Get a copy of the specification XML node
                //
                XmlNode xmlNode = XmlUtilities.GetXmlRootNode(xmlDocument, Consts.STRXML_experimentSpecification);
                this.xmlNodeSpecification = xmlNode.Clone();

                //
                // Get the setup id and check that it exists - search is case-sensitive
                //
                this.setupId = XmlUtilities.GetXmlValue(this.xmlNodeSpecification, Consts.STRXML_setupId, false);
                int setupIndex = Array.IndexOf(this.labConfiguration.SetupIds, this.setupId);
                if (setupIndex < 0)
                {
                    throw new ArgumentException(STRERR_SetupIdInvalid, this.setupId);
                }

                //
                // Get the specified setup XML node
                //
                XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(this.xmlNodeConfiguration, Consts.STRXML_setup, true);
                this.xmlNodeSetup = xmlNodeList.Item(setupIndex);

                // Specification is valid
                validationReport.accepted = true;

                // Calculate execution time
                validationReport.estRuntime = EXECUTION_TIME;
            }
            catch (Exception ex)
            {
                validationReport.errorMessage = ex.Message;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return validationReport;
        }

    }
}
