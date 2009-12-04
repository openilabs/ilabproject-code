using System;
using System.IO;
using System.Xml;
using Library.Lab;

namespace Library.LabServerEngine
{
    public class ExperimentQueue
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "ExperimentQueue";

        //
        // XML experiment queue template
        //
        private const string STRXMLDOC_XmlTemplate =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            "<experimentQueue>\r\n" +
            "  <experiment>\r\n" +
            "    <experimentId />\r\n" +
            "    <sbName />\r\n" +
            "    <userGroup />\r\n" +
            "    <priorityHint />\r\n" +
            "    <specification />\r\n" +
            "    <estExecutionTime />\r\n" +
            "    <cancelled />\r\n" +
            "  </experiment>\r\n" +
            "</experimentQueue>\r\n";

        //
        // String constants for the XML experiment queue template
        //
        private const string STRXML_experimentQueue = "experimentQueue";
        private const string STRXML_experiment = "experiment";
        private const string STRXML_experimentId = "experimentId";
        private const string STRXML_sbName = "sbName";
        private const string STRXML_userGroup = "userGroup";
        private const string STRXML_priorityHint = "priorityHint";
        private const string STRXML_specification = "specification";
        private const string STRXML_estExecutionTime = "estExecutionTime";
        private const string STRXML_cancelled = "cancelled";

        //
        // String constants for exception messages
        //
        private const string STRERR_XmlTemplate = "xmlQueueTemplate";
        private const string STRERR_ExperimentInfo = "experimentInfo";
        private const string STRERR_FileNotExists = " File does not exist!";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_FileNotExistYet = " File does not exist yet!";
        private const string STRLOG_Filename = " Filename: ";
        private const string STRLOG_experimentId = " experimentId: ";
        private const string STRLOG_sbName = " sbName: ";
        private const string STRLOG_statusCode = " statusCode: ";
        private const string STRLOG_position = " position: ";
        private const string STRLOG_queueLength = " queueLength: ";
        private const string STRLOG_waitTime = " waitTime: ";
        private const string STRLOG_estExecutionTime = " estExecutionTime: ";
        private const string STRLOG_seconds = " seconds";
        private const string STRLOG_removed = " removed: ";
        private const string STRLOG_QueueIsEmpty = "Queue is empty";

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

        /// <summary>
        /// <para>Exceptions:</para>
        /// <para>System.ArgumentNullException</para>
        /// <para>System.Xml.XmlException</para>
        /// </summary>
        public ExperimentQueue()
            : this(STRXMLDOC_XmlTemplate, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// <para>Exceptions:</para>
        /// <para>System.ArgumentNullException</para>
        /// <para>System.Xml.XmlException</para>
        /// </summary>
        public ExperimentQueue(int unitId)
            : this(STRXMLDOC_XmlTemplate, STRLOG_ClassName + unitId.ToString() + Consts.STR_XmlFileExtension)
        {
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Unit testing use only - The specified XML queue template string will contain errors to
        /// test the exception handling  of this class. 
        /// <para>Exceptions:</para>
        /// <para>System.ArgumentNullException</para>
        /// <para>System.Xml.XmlException</para>
        /// </summary>
        public ExperimentQueue(string xmlTemplate)
            : this(xmlTemplate, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Unit testing use only - The specified XML queue template string will contain errors to
        /// test the exception handling  of this class. 
        /// <para>Exceptions:</para>
        /// <para>System.ArgumentNullException</para>
        /// <para>System.Xml.XmlException</para>
        /// </summary>
        /// <param name="xmlQueueTemplate"></param>
        /// <param name="xmlQueueFilename"></param>
        public ExperimentQueue(string xmlTemplate, string filename)
        {
            const string STRLOG_MethodName = "ExperimentQueue";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Set the experiment queue template
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
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentQueue);
                XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experiment);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_experimentId, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_sbName, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_userGroup, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_priorityHint, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_specification, true);
                XmlUtilities.GetXmlValue(xmlNode, STRXML_estExecutionTime, true);
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
                // Check if file path is valid and file exists.
                //
                xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                if (xmlDocument == null)
                {
                    //
                    // Create an empty queue file
                    //
                    xmlDocument = XmlUtilities.GetXmlDocument(this.xmlTemplate);
                    xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentQueue);
                    xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experiment);
                    xmlRootNode.RemoveChild(xmlNode);
                    xmlDocument.Save(this.filename);
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

        /// <summary>
        /// Add an experiment to the end of the queue. Return queue information about the experiment.
        /// </summary>
        /// <param name="experimentInfo"></param>
        /// <returns>Queue information about the experiment.</returns>
        public QueuedExperimentInfo Enqueue(ExperimentInfo experimentInfo)
        {
            const string STRLOG_MethodName = "Enqueue";

            string logMessage;
            if (experimentInfo != null)
            {
                logMessage = STRLOG_experimentId + experimentInfo.experimentId.ToString() +
                    Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + experimentInfo.sbName + Logfile.STRLOG_Quote;

                Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

                Logfile.Write(STRLOG_Filename + this.filename);
            }
            else
            {
                Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);
            }

            //
            // Catch all exceptions thrown and return false if an error occurred.
            //
            QueuedExperimentInfo queuedExperimentInfo = null;
            try
            {
                //
                // Check that the experiment info exists
                //
                if (experimentInfo == null)
                {
                    throw new ArgumentNullException(STRERR_ExperimentInfo);
                }

                Logfile.Write(STRLOG_experimentId + experimentInfo.experimentId.ToString());
                Logfile.Write(STRLOG_sbName + experimentInfo.sbName);

                // Load experiment queue XML template string
                XmlDocument xmlTemplateDocument = XmlUtilities.GetXmlDocument(this.xmlTemplate);

                //
                // Fill in the XML template with values from the experiment information
                //
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlTemplateDocument, STRXML_experimentQueue);
                XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlRootNode, STRXML_experiment);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_experimentId, experimentInfo.experimentId);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_sbName, experimentInfo.sbName, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_userGroup, experimentInfo.userGroup, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_priorityHint, experimentInfo.priorityHint);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_specification, experimentInfo.xmlSpecification, false);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_estExecutionTime, experimentInfo.estExecutionTime);
                XmlUtilities.SetXmlValue(xmlNode, STRXML_cancelled, experimentInfo.cancelled);

                //
                // Get exclusive access to the experiment queue file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML experiment queue document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist, save the template document to file
                        xmlTemplateDocument.Save(this.filename);
                        xmlDocument = xmlTemplateDocument;
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

                    //
                    // Get the queued experiment information to return before releasing the file lock
                    //
                    queuedExperimentInfo = GetExperimentInfo(experimentInfo.experimentId,
                        experimentInfo.sbName, xmlDocument);
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return queuedExperimentInfo;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Removes and returns the experiment at the beginning of the queue.
        /// </summary>
        /// <returns></returns>
        public ExperimentInfo Dequeue()
        {
            const string STRLOG_MethodName = "Dequeue";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Write(STRLOG_Filename + this.filename);

            //
            // Get the next experiment and remove it
            //
            ExperimentInfo experimentInfo = GetNextExperiment(true);

            string logMessage;
            if (experimentInfo == null)
            {
                logMessage = STRLOG_QueueIsEmpty;
            }
            else
            {
                logMessage = STRLOG_experimentId + experimentInfo.experimentId.ToString() +
                    Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + experimentInfo.sbName + Logfile.STRLOG_Quote;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return experimentInfo;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Returns the experiment at the beginning of the queue without removing it.
        /// </summary>
        /// <returns></returns>
        public ExperimentInfo Peek()
        {
            const string STRLOG_MethodName = "Peek";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Write(STRLOG_Filename + this.filename);

            //
            // Get the next experiment without removing it
            //
            ExperimentInfo experimentInfo = GetNextExperiment(false);

            string logMessage;
            if (experimentInfo == null)
            {
                logMessage = STRLOG_QueueIsEmpty;
            }
            else
            {
                logMessage = STRLOG_experimentId + experimentInfo.experimentId.ToString() +
                    Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + experimentInfo.sbName + Logfile.STRLOG_Quote;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return experimentInfo;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Cancel the experiment in the queue. The experiment is not removed from the queue.
        /// </summary>
        /// <param name="experimentId">Experiment ID</param>
        /// <param name="sbName">ServiceBroker's name</param>
        /// <returns></returns>
        public bool Cancel(int experimentId, string sbName)
        {
            const string STRLOG_MethodName = "Cancel";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Write(STRLOG_experimentId + experimentId.ToString());
            Logfile.Write(STRLOG_sbName + sbName);

            bool cancelled = false;
            try
            {
                //
                // Get exclusive access to the experiment queue file. Hold the lock on the file
                // until after the experiment has been cancelled and the file has been saved.
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML experiment queue document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist
                        throw new ArgumentException(STRERR_FileNotExists);
                    }

                        //
                        // Get a list of all the experiments in the queue
                        //
                    XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentQueue);
                    XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlRootNode, STRXML_experiment, true);

                    //
                    // Scan through the list of the experiments
                    //
                    for (int i = 0; i < xmlNodeList.Count; i++)
                    {
                        XmlNode xmlNodeExperiment = xmlNodeList.Item(i);

                        //
                        // Get the experiment id and the ServiceBroker's name and check for a match
                        //
                        int id = XmlUtilities.GetIntValue(xmlNodeExperiment, STRXML_experimentId);
                        string name = XmlUtilities.GetXmlValue(xmlNodeExperiment, STRXML_sbName, false);
                        if (id == experimentId && name.Equals(sbName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Found experiment, now cancel it
                            XmlUtilities.SetXmlValue(xmlNodeExperiment, STRXML_cancelled, true);

                            // Save XML document
                            xmlDocument.Save(this.filename);

                            cancelled = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            Logfile.Write(STRLOG_removed + cancelled.ToString());

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return cancelled;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Remove the experiment from the queue.
        /// </summary>
        /// <param name="experimentId">Experiment ID</param>
        /// <param name="sbName">ServiceBroker's name</param>
        /// <returns></returns>
        public bool Remove(int experimentId, string sbName)
        {
            const string STRLOG_MethodName = "Remove";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool removed = false;
            try
            {
                //
                // Get exclusive access to the experiment queue file. Hold the lock on the file
                // until after the experiment has been removed and the file has been saved.
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML experiment queue document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist
                        throw new ArgumentException(STRERR_FileNotExists);
                    }

                    //
                    // Get a list of all the experiments in the queue
                    //
                    XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentQueue);
                    XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlRootNode, STRXML_experiment, true);

                    //
                    // Scan through the list of the experiments
                    //
                    for (int i = 0; i < xmlNodeList.Count; i++)
                    {
                        XmlNode xmlNodeExperiment = xmlNodeList.Item(i);

                        //
                        // Get the experiment id and the ServiceBroker's name and check for a match
                        //
                        int id = XmlUtilities.GetIntValue(xmlNodeExperiment, STRXML_experimentId);
                        string name = XmlUtilities.GetXmlValue(xmlNodeExperiment, STRXML_sbName, false);
                        if (id == experimentId && name.Equals(sbName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Found experiment, remove from XML document
                            xmlRootNode.RemoveChild(xmlNodeExperiment);

                            // Save XML document
                            xmlDocument.Save(this.filename);

                            removed = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_removed + removed.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return removed;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Get the length of the queue and a total of the estimated execution times of all experiments
        /// on the queue.
        /// </summary>
        /// <returns></returns>
        public WaitEstimate GetWaitEstimate()
        {
            const string STRLOG_MethodName = "GetWaitEstimate";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Create a WaitEstimate ready to fill in
            WaitEstimate waitEstimate = new WaitEstimate();

            // Request a queue wait estimate
            QueuedExperimentInfo queuedExperimentInfo = GetExperimentInfo(0, null, null);

            // Store the wait estimate
            if (queuedExperimentInfo != null)
            {
                waitEstimate.effectiveQueueLength = queuedExperimentInfo.queueLength;
                waitEstimate.estWait = queuedExperimentInfo.waitTime;
            }
            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return waitEstimate;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Get the experiment information for the specified experiment. Return null if the
        /// experiment is not on the queue.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="sbName"></param>
        /// <returns>Null if the experiment is not on the queue.</returns>
        public QueuedExperimentInfo GetExperimentInfo(int experimentId, string sbName)
        {
            const string STRLOG_MethodName = "GetExperimentInfo";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Find the specified experiment in the queue
            //
            QueuedExperimentInfo queuedExperimentInfo = GetExperimentInfo(experimentId, sbName, null);

            if (queuedExperimentInfo == null)
            {
                logMessage = STRLOG_QueueIsEmpty;
            }
            else
            {
                logMessage = STRLOG_position + queuedExperimentInfo.position.ToString() +
                    Logfile.STRLOG_Spacer + STRLOG_queueLength + queuedExperimentInfo.queueLength +
                    Logfile.STRLOG_Spacer + STRLOG_waitTime + queuedExperimentInfo.waitTime.ToString() + STRLOG_seconds +
                    Logfile.STRLOG_Spacer + STRLOG_estExecutionTime + queuedExperimentInfo.estExecutionTime.ToString() + STRLOG_seconds;
            }
            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return queuedExperimentInfo;
        }

        //=======================================================================================//

        /// <summary>
        /// Get the experiment information for the specified experiment. Return null if the
        /// experiment is not on the queue.
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="sbName"></param>
        /// <param name="xmlQueueDocument"></param>
        /// <returns></returns>
        private QueuedExperimentInfo GetExperimentInfo(int experimentId, string sbName, XmlDocument xmlQueueDocument)
        {
            QueuedExperimentInfo queuedExperimentInfo = null;
            try
            {
                //
                // Check if the queue XML document is specified
                //
                if (xmlQueueDocument == null)
                {
                    //
                    // Get the XML experiment queue document from the file
                    //
                    lock (this.fileLock)
                    {
                        // File may not exist yet
                        xmlQueueDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    }
                }

                if (xmlQueueDocument == null)
                {
                    // The experiment queue file does not exist yet
                    throw new ArgumentException(STRERR_FileNotExists);
                }

                //
                // Get a list of all the experiments in the queue
                //
                XmlNode xmlRootNode = XmlUtilities.GetXmlRootNode(xmlQueueDocument, STRXML_experimentQueue);
                XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlRootNode, STRXML_experiment, true);
                if (xmlNodeList.Count > 0)
                {
                    if (sbName != null)
                    {
                        // Trim whitespace
                        sbName = sbName.Trim();
                    }
                    else
                    {
                        // Must not be null
                        sbName = "";
                    }

                    // Initialise the wait time for the specified experiment
                    int waitTime = 0;

                    // Store queue length
                    int queueLength = xmlNodeList.Count;

                    //
                    // Scan through the list of the experiments
                    //
                    for (int i = 0; i < xmlNodeList.Count; i++)
                    {
                        XmlNode xmlNode = xmlNodeList.Item(i);

                        // Get the execution time of this experiment
                        int estExecutionTime = XmlUtilities.GetIntValue(xmlNode, STRXML_estExecutionTime);

                        //
                        // Get the experiment id and the ServiceBroker's name and check for a match
                        //
                        int id = XmlUtilities.GetIntValue(xmlNode, STRXML_experimentId);
                        string name = XmlUtilities.GetXmlValue(xmlNode, STRXML_sbName, false);
                        if (id == experimentId && name.Equals(sbName, StringComparison.OrdinalIgnoreCase))
                        {
                            //
                            // Found the experiment, save experiment information
                            //
                            queuedExperimentInfo = new QueuedExperimentInfo(experimentId, sbName);
                            queuedExperimentInfo.userGroup = XmlUtilities.GetXmlValue(xmlNode, STRXML_userGroup, false);
                            queuedExperimentInfo.priorityHint = XmlUtilities.GetIntValue(xmlNode, STRXML_priorityHint);
                            queuedExperimentInfo.xmlSpecification = XmlUtilities.GetXmlValue(xmlNode, STRXML_specification, false);;
                            queuedExperimentInfo.cancelled = XmlUtilities.GetBoolValue(xmlNode, STRXML_cancelled, false);
                            queuedExperimentInfo.estExecutionTime = estExecutionTime;
                            queuedExperimentInfo.queueLength = queueLength;
                            queuedExperimentInfo.position = i + 1;
                            queuedExperimentInfo.waitTime = waitTime;

                            break;
                        }

                        // Add the wait time for this experiment
                        waitTime += estExecutionTime;
                    }

                    //
                    // Check if the experiment was found
                    //
                    if (queuedExperimentInfo == null)
                    {
                        // Not found
                        queuedExperimentInfo = new QueuedExperimentInfo(0, null);
                        queuedExperimentInfo.queueLength = queueLength;
                        queuedExperimentInfo.waitTime = waitTime;

                        //logMessage = STRLOG_NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return queuedExperimentInfo;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Returns the experiment at the beginning of the queue. If 'remove' is true, the experiment
        /// is also removed from the queue. If the queue is empty or an exception occurrs, null is returned.
        /// </summary>
        /// <returns></returns>
        private ExperimentInfo GetNextExperiment(bool remove)
        {
            ExperimentInfo experimentInfo = null;
            bool saveXmlQueueDocument = false;

            try
            {
                //
                // Get exclusive access to the experiment queue file
                //
                lock (this.fileLock)
                {
                    //
                    // Get the XML experiment queue document, file may not exist yet
                    //
                    XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.filename);
                    if (xmlDocument == null)
                    {
                        // The file does not exist
                        throw new ArgumentException(STRERR_FileNotExists);
                    }

                    //
                    // Get the experiment queue XML node
                    //
                    XmlNode xmlNodeExperimentQueue = XmlUtilities.GetXmlRootNode(xmlDocument, STRXML_experimentQueue);

                    while (experimentInfo == null)
                    {
                        //
                        // Get a list of all the experiments in the queue
                        //
                        XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(xmlNodeExperimentQueue, STRXML_experiment, true);
                        if (xmlNodeList.Count == 0)
                        {
                            // No experiments, exit loop
                            break;
                        }

                        //
                        // Get the experiment information from the first experiment in the queue
                        //
                        XmlNode xmlNodeExperiment = xmlNodeList.Item(0);
                        try
                        {
                            experimentInfo = GetXmlNodeExperimentInfo(xmlNodeExperiment);

                            //
                            // Remove experiment if specified
                            //
                            if (experimentInfo != null && remove == true)
                            {
                                // Remove the experiment
                                xmlNodeExperimentQueue.RemoveChild(xmlNodeExperiment);
                                saveXmlQueueDocument = true;
                            }
                        }
                        catch
                        {
                            //
                            // Error occurred, cannot process ExperimentInfo
                            //
                            xmlNodeExperimentQueue.RemoveChild(xmlNodeExperiment);
                            saveXmlQueueDocument = true;
                        }
                    }

                    if (saveXmlQueueDocument == true)
                    {
                        // An experiment has been removed, save XML document
                        xmlDocument.Save(this.filename);
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return experimentInfo;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Return the ExperimentInfo with information provided in the XML Experiment node.
        /// </summary>
        /// <param name="xmlNodeExperiment"></param>
        /// <returns></returns>
        private ExperimentInfo GetXmlNodeExperimentInfo(XmlNode xmlNode)
        {
            // Create experiment information object ready to fill in
            ExperimentInfo experimentInfo = new ExperimentInfo(0, null);

            try
            {
                //
                // Fill in experiment info
                //
                experimentInfo.experimentId = XmlUtilities.GetIntValue(xmlNode, STRXML_experimentId);
                experimentInfo.sbName = XmlUtilities.GetXmlValue(xmlNode, STRXML_sbName, false);
                experimentInfo.userGroup = XmlUtilities.GetXmlValue(xmlNode, STRXML_userGroup, false);
                experimentInfo.priorityHint = XmlUtilities.GetIntValue(xmlNode, STRXML_priorityHint);
                experimentInfo.xmlSpecification = XmlUtilities.GetXmlValue(xmlNode, STRXML_specification, false);
                experimentInfo.estExecutionTime = XmlUtilities.GetIntValue(xmlNode, STRXML_estExecutionTime);
                experimentInfo.cancelled = XmlUtilities.GetBoolValue(xmlNode, STRXML_cancelled, false);
            }
            catch (Exception)
            {
                throw;
            }

            return experimentInfo;
        }
    }
}
