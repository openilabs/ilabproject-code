using System;
using System.IO;
using System.Xml;
using Library.Lab;

namespace Library.LabServerEngine
{
    public class ExperimentResults
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "ExperimentResults";

        //
        // XML experiment results template
        //
        private const string STRXMLDOC_XmlTemplate =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            "<experimentResults>\r\n" +
            "  <experimentResult>\r\n" +
            "    <experimentID />\r\n" +
            "    <sbName />\r\n" +
            "    <userGroup />\r\n" +
            "    <priorityHint />\r\n" +
            "    <statusCode />\r\n" +
            "    <xmlExperimentResult />\r\n" +
            "    <xmlResultExtension />\r\n" +
            "    <xmlBlobExtension />\r\n" +
            "    <warningMessages>\r\n" +
            "      <warningMessage />\r\n" +
            "    </warningMessages>\r\n" +
            "    <errorMessage />\r\n" +
            "  </experimentResult>\r\n" +
            "</experimentResults>\r\n";

        //
        // String constants for the XML elements
        //
        private const string STRXML_experimentResults = "experimentResults";
        private const string STRXML_experimentResult = "experimentResult";
        private const string STRXML_userGroup = "userGroup";
        private const string STRXML_priorityHint = "priorityHint";
        private const string STRXML_statusCode = "statusCode";
        private const string STRXML_xmlExperimentResult = "xmlExperimentResult";
        private const string STRXML_xmlResultExtension = "xmlResultExtension";
        private const string STRXML_xmlBlobExtension = "xmlBlobExtension";
        private const string STRXML_warningMessages = "warningMessages";
        private const string STRXML_warningMessage = "warningMessage";
        private const string STRXML_errorMessage = "errorMessage";

        //
        // String constants for exception messages
        //
        private const string STRERR_XmlTemplate = "xmlResultsTemplate";
        private const string STRERR_ExperimentInfo = "experimentInfo";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_FileNotExistYet = " File does not exist yet!";
        private const string STRLOG_Filename = " Filename: ";
        private const string STRLOG_experimentId = " experimentId: ";
        private const string STRLOG_sbName = " sbName: ";
        private const string STRLOG_statusCode = " statusCode: ";
        private const string STRLOG_saved = " saved: ";

        //
        // Local variables
        //
        private Object fileLock;

        #endregion

        #region Properties

        private string xmlTemplate;
        private string filename;

        public string XmlTemplate
        {
            get { return this.xmlTemplate; }
        }

        public string Filename
        {
            get { return this.filename; }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public ExperimentResults()
            : this(STRXMLDOC_XmlTemplate, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public ExperimentResults(string xmlTemplate)
            : this(xmlTemplate, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// <para>Exceptions:</para>
        /// <para>System.ArgumentNullException</para>
        /// <para>System.Xml.XmlException</para>
        /// </summary>
        /// <param name="xmlResultsTemplate"></param>
        /// <param name="xmlQueueFilename"></param>
        public ExperimentResults(string xmlTemplate, string filename)
        {
            const string STRLOG_MethodName = "ExperimentResults";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Set the experiment results template
            //
            this.xmlTemplate = xmlTemplate;
            if (xmlTemplate == null && filename != null)
            {
                this.xmlTemplate = STRXMLDOC_XmlTemplate;
            }

            //
            // Validate the template
            //
            try
            {
                //
                // Check that the template exists
                //
                if (this.xmlTemplate == null)
                {
                    throw new ArgumentNullException(STRERR_XmlTemplate);
                }

                // Load the XML template string
                XmlDocument xmlDocument = XmlUtilities.GetXmlDocument(this.xmlTemplate);

                //
                // Check that all required XML nodes exists
                //
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentResults);
                XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experimentResult);
                XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_experimentID, true);
                XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_sbName, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_userGroup, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_priorityHint, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_statusCode, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_xmlExperimentResult, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_xmlResultExtension, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_xmlBlobExtension, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_warningMessages, true);
                XmlNode xmlNodeTemp = XmlUtilities.GetXmlNode(xmlNode, STRXML_warningMessages);
                XmlUtilities.GetXmlValues(xmlNodeTemp, STRXML_warningMessage, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_errorMessage, true);

                //
                // Get root filepath from Application's configuration file
                //
                string rootFilePath = Utilities.GetAppSetting(Consts.STRCFG_RootFilePath);

                // Check for trailing backslash and append if missing
                if (rootFilePath.EndsWith("\\") == false)
                {
                    rootFilePath += "\\";
                }

                //
                // Get relative filepath from Application's configuration file
                //
                string filepath = Utilities.GetAppSetting(Consts.STRCFG_ResultsPath);

                // Check for trailing backslash and append if missing
                if (filepath.EndsWith("\\") == false)
                {
                    filepath += "\\";
                }

                //
                // Check if a filename is specified
                //
                if (filename == null)
                {
                    // Create new filename with absolute path
                    this.filename = rootFilePath + filepath + STRLOG_ClassName + Consts.STR_XmlFileExtension;
                }
                else
                {
                    // Append absolute path with specified filename 
                    this.filename = rootFilePath + filepath + filename;
                }

                Logfile.Write(STRLOG_Filename + this.filename);

                //
                // Check if file path is valid and file exists. It is ok not to exist yet
                //
                xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                if (xmlDocument == null)
                {
                    Logfile.Write(STRLOG_FileNotExistYet);
                }
            }
            catch (Exception ex)
            {
                // Log the message and throw the exception back to the caller
                Logfile.WriteError(ex.Message);
                throw;
            }

            // Create file access lock
            this.fileLock = new Object();

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>Load an experiment result from the experiment result file.
        /// </summary>
        /// <param name="experimentID">Experiment number</param>
        /// <param name="sbName">ServiceBroker's name</param>
        /// <returns></returns>
        public ResultReport Load(int experimentID, string sbName)
        {
            //
            // Load experiment results from the experiment results file
            //
            return this.LoadResults(experimentID, sbName, null);
        }

        //-------------------------------------------------------------------------------------------------//

        public ResultReport Load(int experimentID, string sbName, string xmlResultsString)
        {
            //
            // Load experiment results from the specified XML results string, debugging use only
            //
            return this.LoadResults(experimentID, sbName, xmlResultsString);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="sbName"></param>
        /// <param name="xmlResultsString"></param>
        /// <returns></returns>
        private ResultReport LoadResults(int experimentId, string sbName, string xmlResultsString)
        {
            const string STRLOG_MethodName = "Load";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);


            //
            // Create a ResultReport to return (not nullable)
            //
            ResultReport resultReport = new ResultReport();

            //
            // Catch all exceptions so that a valid result report can be returned
            //
            try
            {
                XmlDocument xmlDocument;

                if (xmlResultsString != null)
                {
                    // Load XML experiment results document from an XML string
                    xmlDocument = XmlUtilities.GetXmlDocument(xmlResultsString);
                }
                else
                {
                    //
                    // Get exclusive access to the experiment result file
                    //
                    lock (this.fileLock)
                    {
                        // Get the XML experiment results document, file may not exist yet
                        xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    }
                }

                if (xmlDocument != null)
                {
                    //
                    // Get the list of experiment results
                    //
                    XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentResults);
                    XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlRootNode, STRXML_experimentResult, true);

                    //
                    // Scan through the list of experiments looking for the last occurrence of
                    // the experiment ID. Multiple occurrences of the same experiment ID can be present
                    // during development/debugging but are not after release. Doesn't really matter.
                    //
                    for (int i = 0; i < xmlNodeList.Count; i++)
                    {
                        XmlNode xmlNode = xmlNodeList.Item(i);

                        //
                        // Get the experiment id and the ServiceBroker's name and check for a match
                        //
                        int id = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_experimentID);
                        string name = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_sbName, false);
                        if (id == experimentId && name.Equals(sbName, StringComparison.OrdinalIgnoreCase))
                        {
                            //
                            // Build the result report
                            //
                            resultReport.statusCode = XmlUtilities.GetIntValue(xmlNode, STRXML_statusCode);
                            resultReport.experimentResults = XmlUtilities.GetXmlValue(xmlNode, STRXML_xmlExperimentResult, true);
                            resultReport.xmlResultExtension = XmlUtilities.GetXmlValue(xmlNode, STRXML_xmlResultExtension, true);
                            resultReport.xmlBlobExtension = XmlUtilities.GetXmlValue(xmlNode, STRXML_xmlBlobExtension, true);
                            XmlNode xmlNodeTemp = XmlUtilities.GetXmlNode(xmlNode, STRXML_warningMessages);
                            resultReport.warningMessages = XmlUtilities.GetXmlValues(xmlNodeTemp, STRXML_warningMessage, true);
                            resultReport.errorMessage = XmlUtilities.GetXmlValue(xmlNode, STRXML_errorMessage, true);

                            //
                            // Format the experiment result XML before adding to the report
                            //
                            XmlNode xmlNodeResult = XmlUtilities.GetXmlNode(xmlNode, STRXML_xmlExperimentResult);
                            xmlNodeResult = XmlUtilities.GetXmlNode(xmlNodeResult, STRXML_experimentResult);
                            StringWriter sw = new StringWriter();
                            XmlTextWriter xtw = new XmlTextWriter(sw);
                            xtw.Formatting = Formatting.Indented;
                            xmlNodeResult.WriteTo(xtw);
                            xtw.Flush();
                            resultReport.experimentResults = sw.ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Place the exception message in the result report
                resultReport.errorMessage = ex.Message;

                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_statusCode + ((StatusCodes)resultReport.statusCode).ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return resultReport;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experimentInfo"></param>
        /// <returns>True if the experiment information was saved successfully.</returns>
        public bool Save(ExperimentInfo experimentInfo)
        {
            const string STRLOG_MethodName = "Save";

            string logMessage = null;

            if (experimentInfo != null)
            {
                logMessage = STRLOG_experimentId + experimentInfo.experimentId.ToString() +
                    Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + experimentInfo.sbName + Logfile.STRLOG_Quote;
            }

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Catch all exceptions thrown and return false if an error occurred.
            //
            bool success = false;
            try
            {
                //
                // Check that the experiment info exists
                //
                if (experimentInfo == null)
                {
                    throw new ArgumentNullException(STRERR_ExperimentInfo);
                }

                // Load experiment results XML template string
                XmlDocument xmlTemplateDocument = XmlUtilities.GetXmlDocument(this.xmlTemplate);

                //
                // Fill in the XML template with values from the experiment information
                //
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlTemplateDocument, STRXML_experimentResults);
                XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experimentResult);
                XmlUtilities.SetXmlValue(xmlNode, Consts.STRXML_experimentID, experimentInfo.experimentId);
                XmlUtilities.SetXmlValue(xmlNode, Consts.STRXML_sbName, experimentInfo.sbName, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_userGroup, experimentInfo.userGroup, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_priorityHint, experimentInfo.priorityHint);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_statusCode, experimentInfo.resultReport.statusCode);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_xmlExperimentResult, experimentInfo.resultReport.experimentResults, true);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_xmlResultExtension, experimentInfo.resultReport.xmlResultExtension, true);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_xmlBlobExtension, experimentInfo.resultReport.xmlBlobExtension, true);
                XmlNode xmlNodeTemp = XmlUtilities.GetXmlNode(xmlNode, STRXML_warningMessages);
                XmlUtilities.SetXmlValues(xmlNodeTemp, STRXML_warningMessage, experimentInfo.resultReport.warningMessages, true);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_errorMessage, experimentInfo.resultReport.errorMessage, true);

                //
                // Get exclusive access to the experiment result file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML experiment results document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist, save the template document to file
                        xmlTemplateDocument.Save(this.filename);
                    }
                    else
                    {
                        // The file does exist, create an XML fragment
                        XmlDocumentFragment xmlFragment = xmlDocument.CreateDocumentFragment();

                        //
                        // Append the XML fragment to the document and save the document to file
                        //
                        xmlFragment.InnerXml = xmlNode.OuterXml;
                        xmlDocument.DocumentElement.AppendChild(xmlFragment);
                        xmlDocument.Save(this.filename);
                    }
                }

                // Information saved successfully
                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_saved + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }
    }
}
