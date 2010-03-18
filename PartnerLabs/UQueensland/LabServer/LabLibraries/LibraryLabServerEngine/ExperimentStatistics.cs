using System;
using System.IO;
using System.Xml;
using Library.Lab;

namespace Library.LabServerEngine
{
    public class ExperimentStatistics
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "ExperimentStatistics";

        private const string STR_DateTimeFormat = "G";

        //
        // XML statistics template
        //
        private const string STRXMLDOC_XmlTemplate =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            "<statistics>\r\n" +
            "  <experiment>\r\n" +
            "    <experimentId />\r\n" +
            "    <sbName />\r\n" +
            "    <userGroup />\r\n" +
            "    <priorityHint />\r\n" +
            "    <estimatedExecTime />\r\n" +
            "    <timeSubmitted />\r\n" +
            "    <queueLength />\r\n" +
            "    <estimatedWaitTime />\r\n" +
            "    <timeStarted />\r\n" +
            "    <unitId />\r\n" +
            "    <timeCompleted />\r\n" +
            "    <actualExecTime />\r\n" +
            "    <cancelled />\r\n" +
            "  </experiment>\r\n" +
            "</statistics>\r\n";

        //
        // String constants for XML elements
        //
        private const string STRXML_statistics = "statistics";
        private const string STRXML_experiment = "experiment";
        private const string STRXML_experimentId = "experimentId";
        private const string STRXML_sbName = "sbName";
        private const string STRXML_userGroup = "userGroup";
        private const string STRXML_priorityHint = "priorityHint";
        private const string STRXML_estimatedExecTime = "estimatedExecTime";
        private const string STRXML_timeSubmitted = "timeSubmitted";
        private const string STRXML_queueLength = "queueLength";
        private const string STRXML_estimatedWaitTime = "estimatedWaitTime";
        private const string STRXML_timeStarted = "timeStarted";
        private const string STRXML_unitId = "unitId";
        private const string STRXML_timeCompleted = "timeCompleted";
        private const string STRXML_actualExecTime = "actualExecTime";
        private const string STRXML_cancelled = "cancelled";

        //
        // String constants for exception messages
        //
        private const string STRERR_XmlTemplate = "xmlStatisticsTemplate";
        private const string STRERR_queuedExperimentInfo = "queuedExperimentInfo";
        private const string STRERR_FileNotExists = " File does not exist!";
        private const string STRERR_ExperimentNotFound = " Experiment not found!";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_FileNotExistYet = " File does not exist yet";
        private const string STRLOG_Filename = " Filename: ";
        private const string STRLOG_experimentId = "experimentId: ";
        private const string STRLOG_sbName = "sbName: ";
        private const string STRLOG_unitId = "unitId: ";
        private const string STRLOG_success = "success: ";

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

        public ExperimentStatistics()
            : this(STRXMLDOC_XmlTemplate, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public ExperimentStatistics(string xmlTemplate)
            : this(xmlTemplate, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// <para>Exceptions:</para>
        /// <para>System.ArgumentNullException</para>
        /// <para>System.Xml.XmlException</para>
        /// </summary>
        /// <param name="xmlStatisticsTemplate"></param>
        public ExperimentStatistics(string xmlTemplate, string filename)
        {
            const string STRLOG_MethodName = "ExperimentStatistics";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Set the experiment statistics template
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
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_statistics);
                XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experiment);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_experimentId, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_sbName, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_userGroup, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_priorityHint, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_estimatedExecTime, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_timeSubmitted, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_queueLength, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_estimatedWaitTime, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_timeStarted, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_unitId, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_timeCompleted, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_actualExecTime, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_cancelled, true);

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
                    // Create filename with absolute path
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

        //---------------------------------------------------------------------------------------//

        public bool Submitted(QueuedExperimentInfo queuedExperimentInfo, DateTime timeSubmitted)
        {
            const string STRLOG_MethodName = "Submitted";

            string logMessage = null;
            if (queuedExperimentInfo != null)
            {
                logMessage = STRLOG_experimentId + queuedExperimentInfo.experimentId.ToString() +
                                 Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + queuedExperimentInfo.sbName + Logfile.STRLOG_Quote;
            }

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Catch all exceptions thrown and return false if an error occurred.
            //
            bool success = false;
            try
            {
                //
                // Check that the queued experiment info exists
                //
                if (queuedExperimentInfo == null)
                {
                    throw new ArgumentNullException(STRERR_queuedExperimentInfo);
                }

                // Load the XML template string
                XmlDocument xmlTemplateDocument = XmlUtilities.GetXmlDocument(this.xmlTemplate);

                //
                // Fill in the XML template with values from the queued experiment information
                //
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlTemplateDocument, STRXML_statistics);
                XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experiment);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_experimentId, queuedExperimentInfo.experimentId);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_sbName, queuedExperimentInfo.sbName, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_userGroup, queuedExperimentInfo.userGroup, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_priorityHint, queuedExperimentInfo.priorityHint);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_estimatedExecTime, queuedExperimentInfo.estExecutionTime);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_timeSubmitted, timeSubmitted.ToString(), false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_queueLength, queuedExperimentInfo.position - 1);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_estimatedWaitTime, queuedExperimentInfo.waitTime);

                //
                // Get exclusive access to the statistics file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML statistics document, file may not exist yet
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

            logMessage = STRLOG_success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public bool Started(int experimentId, string sbName, int unitId, DateTime timeStarted)
        {
            const string STRLOG_MethodName = "Started";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote +
                Logfile.STRLOG_Spacer + STRLOG_unitId + unitId.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Catch all exceptions thrown and return false if an error occurred.
            //
            bool success = false;
            try
            {
                //
                // Get exclusive access to the statistics file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML statistics document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist
                        throw new ArgumentException(STRERR_FileNotExists);
                    }

                    //
                    // Get the XML node for the specified experiment
                    //
                    XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_statistics);
                    XmlNode xmlNode = FindXmlNodeExperiment(xmlRootNode, experimentId, sbName);
                    if (xmlNode == null)
                    {
                        throw new ArgumentException(STRERR_ExperimentNotFound);
                    }

                    //
                    // Fill in the time started and the unit ID information
                    //
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_timeStarted, timeStarted.ToString(), false);
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_unitId, unitId.ToString(), false);

                    //
                    // Save the time started in binary form to calculate the actual execution time later
                    //
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_actualExecTime, timeStarted.ToBinary().ToString(), false);

                    // Save the XML document to file
                    xmlDocument.Save(this.filename);
                }

                // Information saved successfully
                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public bool Completed(int experimentId, string sbName, DateTime timeCompleted)
        {
            const string STRLOG_MethodName = "Completed";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Catch all exceptions thrown and return false if an error occurred.
            //
            bool success = false;
            try
            {
                //
                // Get exclusive access to the statistics file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML statistics document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist
                        throw new ArgumentException(STRERR_FileNotExists);
                    }

                    //
                    // Get the XML node for the specified experiment
                    //
                    XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_statistics);
                    XmlNode xmlNode = FindXmlNodeExperiment(xmlRootNode, experimentId, sbName);
                    if (xmlNode == null)
                    {
                        throw new ArgumentException(STRERR_ExperimentNotFound);
                    }

                    //
                    // Fill in the time completed information
                    //
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_timeCompleted, timeCompleted.ToString(), false);

                    //
                    // Calculate the actual execution time and save it
                    //
                    string timeStartedBinary = XmlUtilities.GetXmlValue(xmlNode, STRXML_actualExecTime, false);
                    int actualExecTime = -1;
                    try
                    {
                        long timeStarted = Int64.Parse(timeStartedBinary);
                        actualExecTime = (int)(timeCompleted - DateTime.FromBinary(timeStarted)).TotalSeconds;
                    }
                    catch
                    {
                    }
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_actualExecTime, actualExecTime);

                    // Experiment was not cancelled
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_cancelled, false);

                    // Save the XML document to file
                    xmlDocument.Save(this.filename);
                }

                // Information saved successfully
                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public bool Cancelled(int experimentId, string sbName, DateTime timeCancelled)
        {
            const string STRLOG_MethodName = "Cancelled";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Catch all exceptions thrown and return false if an error occurred.
            //
            bool success = false;
            try
            {
                //
                // Get exclusive access to the statistics file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML statistics document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist
                        throw new ArgumentException(STRERR_FileNotExists);
                    }

                    //
                    // Get the XML node for the specified experiment
                    //
                    XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_statistics);
                    XmlNode xmlNode = FindXmlNodeExperiment(xmlRootNode, experimentId, sbName);
                    if (xmlNode == null)
                    {
                        throw new ArgumentException(STRERR_ExperimentNotFound);
                    }

                    //
                    // Fill in the time cancelled information
                    //
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_timeCompleted, timeCancelled.ToString(), false);

                    //
                    // Calculate the actual execution time before being cancelled and save it
                    //
                    string timeStartedBinary = XmlUtilities.GetXmlValue(xmlNode, STRXML_actualExecTime, false);
                    int actualExecTime = -1;
                    try
                    {
                        long timeStarted = Int64.Parse(timeStartedBinary);
                        actualExecTime = (int)(timeCancelled - DateTime.FromBinary(timeStarted)).TotalSeconds;
                    }
                    catch
                    {
                    }
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_actualExecTime, actualExecTime);

                    // Experiment was cancelled
                    XmlUtilities.SetXmlValue(xmlNode, STRXML_cancelled, true.ToString(), false);

                    // Save the XML document to file
                    xmlDocument.Save(this.filename);
                }

                // Information saved successfully
                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        private XmlNode FindXmlNodeExperiment(XmlNode xmlRootNode, int experimentId, string sbName)
        {
            //
            // Scan through the list of experiments looking for the last occurrence of
            // the desired experiment. Multiple occurrences of the same experiment ID can be
            // present during development but are not after release. Doesn't really matter.
            //
            XmlNode xmlNode = null;
            try
            {
                XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlRootNode, STRXML_experiment, true);
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlNode xmlNodeTemp = xmlNodeList.Item(i);

                    //
                    // Get the experiment id and ServiceBroker's name
                    //
                    int id = XmlUtilities.GetIntValue(xmlNodeTemp, STRXML_experimentId);
                    string name = XmlUtilities.GetXmlValue(xmlNodeTemp, STRXML_sbName, false);

                    //
                    // Check if there is a match
                    //
                    if (id == experimentId && name.Equals(sbName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Found the experiment, get the XML node
                        xmlNode = xmlNodeTemp;
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return xmlNode;
        }
    }
}
