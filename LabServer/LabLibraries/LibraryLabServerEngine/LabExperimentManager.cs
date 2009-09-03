using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using Library.Lab;

namespace Library.LabServerEngine
{
    public class LabExperimentManager : IDisposable
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "LabExperimentManager";

        //
        // String constants for log messages
        //
        private const string STRLOG_farmSize = " farmSize: ";
        private const string STRLOG_LabExperimentManagerIsReady = " Lab experiment manager is ready.";
        private const string STRLOG_experimentId = " experimentId: ";
        private const string STRLOG_sbName = " sbName: ";
        private const string STRLOG_statusCode = " statusCode: ";
        private const string STRLOG_remainingRuntime = " remainingRuntime: ";
        private const string STRLOG_estRuntime = " estRuntime: ";
        private const string STRLOG_QueuePosition = " QueuePosition: ";
        private const string STRLOG_QueueWaitTime = " QueueWaitTime: ";
        private const string STRLOG_QueuedExperimentCancelled = " Queued experiment was cancelled.";
        private const string STRLOG_RunningExperimentCancelled = " Running experiment was cancelled.";
        private const string STRLOG_ExperimentNotCancelled = " Experiment was not cancelled.";

        //
        // String constants for exception messages
        //
        private const string STRERR_appData = "appData";
        private const string STRERR_allowedCallers = "allowedCallers";
        private const string STRERR_experimentQueue = "experimentQueue";
        private const string STRERR_experimentResults = "experimentResults";
        private const string STRERR_experimentStatistics = "experimentStatistics";
        private const string STRERR_labConfiguration = "labConfiguration";
        private const string STRERR_signalCompleted = "signalCompleted";
        private const string STRERR_statusLock = "statusLock";
        private const string STRERR_signalSubmitted = "signalSubmitted";
        private const string STRERR_threadLabExperimentManager = "threadLabExperimentManager";
        private const string STRERR_LabExperimentManagerFailedReady = "Lab experiment manager failed to become ready!";
        private const string STRERR_FailedToQueueExperiment = "Failed to queue experiment!";
        private const string STRERR_FarmSizeMinimum = "Farm size minimum is 1";
        private const string STRERR_FarmSizeMaximum = "Farm size exceeds maximum of ";
        private const string STRERR_FarmSizeInvalid = "Farm size is invalid";
        private const string STRERR_UserGroupNotSpecified = "User group is not specified";

        //
        // Local constants
        //
        private const int MAX_FARM_SIZE = 5;

        //
        // Local variables
        //
        private ExperimentQueue experimentQueue;
        private ExperimentResults experimentResults;
        private ExperimentStatistics experimentStatistics;
        private object signalCompleted;
        private bool disposed;
        private Object signalSubmitted;
        private Object statusLock;
        private Thread threadLabExperimentManager;
        private bool running;

        //
        // Local variables available to a derived class
        //
        protected AppData appData;

        //
        // Private properties
        //
        private bool Running
        {
            get
            {
                lock (this.statusLock)
                {
                    return this.running;
                }
            }
        }

        #endregion

        #region Public Properties

        private bool online;
        private string labStatusMessage;
        private StatusCodes status;

        public bool Online
        {
            get { return this.online; }
        }

        public string LabStatusMessage
        {
            get { return this.labStatusMessage; }
        }

        public StatusCodes Status
        {
            get
            {
                lock (this.statusLock)
                {
                    return this.status;
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public LabExperimentManager(AllowedCallers allowedCallers, LabConfiguration labConfiguration)
            : this(allowedCallers, labConfiguration, 0)
        {
        }

        //-------------------------------------------------------------------------------------------------//

        public LabExperimentManager(AllowedCallers allowedCallers, LabConfiguration labConfiguration, int farmSize)
        {
            const string STRLOG_MethodName = "LabExperimentManager";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            try
            {
                // Thread objects have not been created yet
                this.disposed = true;

                //
                // Initialise local variables
                //
                this.appData = new AppData();
                if (this.appData == null)
                {
                    throw new ArgumentNullException(STRERR_appData);
                }

                if (allowedCallers == null)
                {
                    throw new ArgumentNullException(STRERR_allowedCallers);
                }
                this.appData.allowedCallers = allowedCallers;

                if (labConfiguration == null)
                {
                    throw new ArgumentNullException(STRERR_labConfiguration);
                }
                this.appData.labConfiguration = labConfiguration;

                //
                // Get the farm size
                //
                try
                {
                    if (farmSize == 0)
                    {
                        // Get the farm size from the Application's configuration file
                        appData.farmSize = Utilities.GetIntAppSetting(Consts.STRCFG_FarmSize);
                    }
                    else
                    {
                        appData.farmSize = farmSize;
                    }
                }
                catch (ArgumentNullException)
                {
                    // Farm size is not specified, default to 1
                    appData.farmSize = 1;
                }
                catch (Exception)
                {
                    throw new ArgumentException(STRERR_FarmSizeInvalid);
                }
                if (appData.farmSize < 1)
                {
                    throw new ArgumentException(STRERR_FarmSizeMinimum);
                }
                if (appData.farmSize > MAX_FARM_SIZE)
                {
                    throw new ArgumentException(STRERR_FarmSizeMaximum + MAX_FARM_SIZE.ToString());
                }

                Logfile.Write(STRLOG_farmSize + appData.farmSize);

                //
                // Create class instances and objects that are not derived
                //
                this.experimentResults = new ExperimentResults();
                if (this.experimentResults == null)
                {
                    throw new ArgumentNullException(STRERR_experimentResults);
                }
                appData.experimentResults = this.experimentResults;

                this.experimentStatistics = new ExperimentStatistics();
                if (this.experimentStatistics == null)
                {
                    throw new ArgumentNullException(STRERR_experimentStatistics);
                }
                appData.experimentStatistics = this.experimentStatistics;

                this.signalCompleted = new object();
                if (this.signalCompleted == null)
                {
                    throw new ArgumentNullException(STRERR_signalCompleted);
                }
                appData.signalCompleted = this.signalCompleted;

                this.experimentQueue = new ExperimentQueue();
                if (this.experimentQueue == null)
                {
                    throw new ArgumentNullException(STRERR_experimentQueue);
                }

                //
                // Initialise property variables
                //
                this.online = false;
                this.labStatusMessage = null;
                this.status = StatusCodes.Unknown;

                //
                // Create thread objects
                //
                this.statusLock = new Object();
                if (this.statusLock == null)
                {
                    throw new ArgumentNullException(STRERR_statusLock);
                }
                this.signalSubmitted = new Object();
                if (this.signalSubmitted == null)
                {
                    throw new ArgumentNullException(STRERR_signalSubmitted);
                }
                this.threadLabExperimentManager = new Thread(new ThreadStart(LabExperimentManagerThread));
                if (this.threadLabExperimentManager == null)
                {
                    throw new ArgumentNullException(STRERR_threadLabExperimentManager);
                }

                //
                // Don't start the thread yet, the method Start() must be called to start the thread
                // after the derived class has completed its initialisation.
                //

            }
            catch (Exception ex)
            {
                // Log the message and throw the exception back to the caller
                Logfile.WriteError(ex.Message);
                throw;
            }
            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public bool Cancel(int experimentID, string sbName)
        {
            //
            // Try cancelling the experiment on the queue
            //
            bool cancelled = this.experimentQueue.Cancel(experimentID, sbName);
            if (cancelled == true)
            {
                Logfile.Write(STRLOG_QueuedExperimentCancelled);
            }
            else
            {
                //
                // Experiment may be currently running, try cancelling it there
                //
                for (int i = 0; i < this.appData.farmSize; i++)
                {
                    LabExperimentEngine labExperimentEngine = this.appData.labExperimentEngines[i];

                    cancelled = labExperimentEngine.Cancel(experimentID, sbName);
                    if (cancelled == true)
                    {
                        Logfile.Write(STRLOG_RunningExperimentCancelled);
                        break;
                    }
                }
            }

            if (cancelled == false)
            {
                Logfile.Write(STRLOG_ExperimentNotCancelled);
            }

            return cancelled;
        }

        //-------------------------------------------------------------------------------------------------//

        public WaitEstimate GetEffectiveQueueLength(string userGroup, int priorityHint)
        {
            //
            // NOTE: This implementation does not consider the group or priority of the user
            //

            // Get queue wait estimate
            WaitEstimate waitEstimate = this.experimentQueue.GetWaitEstimate();

            // Add in time remaining before the next experiment can run
            LabExperimentStatus labExperimentStatus = this.GetLabExperimentStatus(0, null);
            waitEstimate.estWait += labExperimentStatus.statusReport.estRemainingRuntime;

            return waitEstimate;
        }

        //-------------------------------------------------------------------------------------------------//

        public LabExperimentStatus GetExperimentStatus(int experimentId, string sbName)
        {
            const string STRLOG_MethodName = "GetExperimentStatus";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            //
            // Check if an experiment is currently running
            //
            LabExperimentStatus labExperimentStatus = this.GetLabExperimentStatus(experimentId, sbName);
            if ((StatusCodes)labExperimentStatus.statusReport.statusCode != StatusCodes.Unknown)
            {
                logMessage = null;
            }
            else
            {
                //
                // Experiment is not currently running, maybe waiting on the queue
                //
                QueuedExperimentInfo queuedExperimentInfo = this.experimentQueue.GetExperimentInfo(experimentId, sbName);
                if (queuedExperimentInfo != null && queuedExperimentInfo.position > 0)
                {
                    // Set the experiment status
                    labExperimentStatus.statusReport.statusCode = (int)StatusCodes.Waiting;

                    // Get the queue position and wait time
                    labExperimentStatus.statusReport.wait =
                        new WaitEstimate(queuedExperimentInfo.position, queuedExperimentInfo.waitTime);

                    // Add in time for any currently running experiment
                    labExperimentStatus.statusReport.wait.estWait += GetMinRemainingRuntime();

                    // Get the time it takes to run the experiment 
                    labExperimentStatus.statusReport.estRuntime = queuedExperimentInfo.estExecutionTime;
                    labExperimentStatus.statusReport.estRemainingRuntime = queuedExperimentInfo.estExecutionTime;

                    logMessage =
                        Logfile.STRLOG_Spacer + STRLOG_QueuePosition + labExperimentStatus.statusReport.wait.effectiveQueueLength.ToString() +
                        Logfile.STRLOG_Spacer + STRLOG_QueueWaitTime + labExperimentStatus.statusReport.wait.estWait.ToString() +
                        Logfile.STRLOG_Spacer + STRLOG_estRuntime + labExperimentStatus.statusReport.estRuntime.ToString() +
                        Logfile.STRLOG_Spacer + STRLOG_remainingRuntime + labExperimentStatus.statusReport.estRemainingRuntime.ToString();
                }
                else
                {
                    //
                    // Experiment is not waiting on the queue, try loading experiment result from file
                    //
                    ResultReport resultReport = this.experimentResults.Load(experimentId, sbName);

                    // Fill in lab experiment status information
                    labExperimentStatus.statusReport.statusCode = resultReport.statusCode;

                    logMessage = null;
                }
            }

            logMessage = STRLOG_statusCode + ((StatusCodes)labExperimentStatus.statusReport.statusCode).ToString() +
                Logfile.STRLOG_Spacer + logMessage;

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return labExperimentStatus;
        }

        //-------------------------------------------------------------------------------------------------//

        public string GetLabConfiguration(string userGroup)
        {
            //
            // Load the lab configuration from the specified file and convert to a string
            //
            XmlDocument xmlDocument = XmlUtilities.GetXmlDocumentFromFile(this.appData.labConfiguration.Filename);
            XmlNode xmlNodeLabConfiguration = XmlUtilities.GetXmlRootNode(xmlDocument, Consts.STRXML_labConfiguration);

            //
            // Write the Xml document to a string
            //
            StringWriter sw = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(sw);
            xtw.Formatting = Formatting.Indented;
            xmlDocument.WriteTo(xtw);
            xtw.Flush();

            return sw.ToString();
        }

        //-------------------------------------------------------------------------------------------------//

        public string GetLabInfo()
        {
            return null;
        }

        //-------------------------------------------------------------------------------------------------//

        public LabStatus GetLabStatus()
        {
            LabStatus labStatus = new LabStatus(this.online, this.labStatusMessage);

            return (labStatus);
        }

        //-------------------------------------------------------------------------------------------------//

        public ResultReport RetrieveResult(int experimentID, string sbName)
        {
            //
            // Try loading the experiment result from file
            //
            ResultReport resultReport = this.experimentResults.Load(experimentID, sbName);
            if (resultReport.statusCode == (int)StatusCodes.Unknown)
            {
                //
                // Experiment has not completed, maybe waiting on the queue
                //
                QueuedExperimentInfo queuedExperimentInfo = this.experimentQueue.GetExperimentInfo(experimentID, sbName);
                if (queuedExperimentInfo != null)
                {
                    // Experiment is waiting on the queue, therefore no results
                    resultReport.statusCode = (int)StatusCodes.Waiting;
                }
                else
                {
                    //
                    // Experiment is not queued, maybe currently running
                    //
                    LabExperimentStatus labExperimentStatus = this.GetLabExperimentStatus(experimentID, sbName);
                    if (labExperimentStatus.statusReport.statusCode != (int)StatusCodes.Unknown)
                    {
                        // Experiment is still running, therefore no results
                        resultReport.statusCode = labExperimentStatus.statusReport.statusCode;
                    }
                }
            }

            return resultReport;
        }

        //-------------------------------------------------------------------------------------------------//

        public SubmissionReport Submit(int experimentID, string sbName, string experimentSpecification,
            string userGroup, int priorityHint)
        {
            //
            // Create a SubmissionReport object ready to fill in and return
            //
            SubmissionReport submissionReport = new SubmissionReport(experimentID);

            //
            // Validate the experiment specification before submitting
            //
            ValidationReport validationReport = Validate(experimentSpecification, userGroup);
            if (validationReport.accepted == false)
            {
                // Experiment specification is invalid, cannot submit
                submissionReport.vReport = validationReport;
                return submissionReport;
            }

            //
            // Create an instance of the experiment
            //
            ExperimentInfo experimentInfo = new ExperimentInfo(experimentID, sbName,
                userGroup, priorityHint, experimentSpecification, (int)validationReport.estRuntime);

            // Get a wait estimate to return in the submission report before putting the experiment on the queue
            WaitEstimate waitEstimate = GetEffectiveQueueLength(userGroup, priorityHint);

            Logfile.Write(" BEFORE: effectiveQueueLength=" + waitEstimate.effectiveQueueLength.ToString() +
                " estWait=" + waitEstimate.estWait.ToString());

            //
            // Add the experiment to the queue
            //
            QueuedExperimentInfo queuedExperimentInfo = this.experimentQueue.Enqueue(experimentInfo);
            if (queuedExperimentInfo != null)
            {
                //
                // Update submission report
                //
                submissionReport.vReport.accepted = true;
                submissionReport.vReport.estRuntime = queuedExperimentInfo.estExecutionTime;
                submissionReport.wait = waitEstimate;

                //
                // Get minimum remaining runtime of any currently running experiments and add into the wait estimate
                //
                int minRemainingRuntime = GetMinRemainingRuntime();
                submissionReport.wait.estWait += minRemainingRuntime;

                //
                // Update the statistics with revised wait estimate
                //
                queuedExperimentInfo.waitTime = (int)submissionReport.wait.estWait;
                this.experimentStatistics.Submitted(queuedExperimentInfo, DateTime.Now);

                // Tell lab experiment manager thread that an experiment has been submitted
                this.SignalSubmitted();
            }
            else
            {
                //
                // Failed to add experiment to the queue
                //
                submissionReport.vReport.accepted = true;
                submissionReport.vReport.errorMessage = STRERR_FailedToQueueExperiment;
            }

            return submissionReport;
        }

        //-------------------------------------------------------------------------------------------------//

        public ValidationReport Validate(string xmlSpecification, string userGroup)
        {
            const string STRLOG_MethodName = "Validate";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            ValidationReport validationReport;

            //
            // Check that usergroup is specified
            //
            if (userGroup == null || userGroup.Trim().Length == 0)
            {
                validationReport = new ValidationReport(false);
                validationReport.errorMessage = STRERR_UserGroupNotSpecified;
            }
            else
            {
                validationReport = this.appData.labExperimentEngines[0].Validate(xmlSpecification);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return validationReport;
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual void Create()
        {
            const string STRLOG_MethodName = "Create";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Create local class instances just to check that all is in order
            //
            ExperimentSpecification experimentSpecification = new ExperimentSpecification(this.appData.labConfiguration);
            LabExperimentResult labExperimentResult = new LabExperimentResult(this.appData.labConfiguration);

            //
            // Create instances of lab experiment engines
            //
            this.appData.labExperimentEngines = new LabExperimentEngine[this.appData.farmSize];
            for (int i = 0; i < this.appData.farmSize; i++)
            {
                this.appData.labExperimentEngines[i] = new LabExperimentEngine(i, this.appData);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public void Start(bool deleteXmlFiles)
        {
            //
            // Check if we want to delete the XML files - use for unit test debugging only
            //
            if (deleteXmlFiles == true)
            {
                //
                // Delete XML experiment queue file so that it does not exist yet
                //
                if (File.Exists(this.experimentQueue.Filename) == true)
                {
                    File.Delete(this.experimentQueue.Filename);
                }

                //
                // Delete XML results file so that it does not exist yet
                //
                if (File.Exists(appData.experimentResults.Filename) == true)
                {
                    File.Delete(appData.experimentResults.Filename);
                }

                //
                // Delete XML statistics file so that it does not exist yet
                //
                if (File.Exists(appData.experimentStatistics.Filename) == true)
                {
                    File.Delete(appData.experimentStatistics.Filename);
                }

                //
                // Delete XML queue files so that they don't exist yet
                //
                for (int i = 0; i < appData.farmSize; i++)
                {
                    if (File.Exists(appData.labExperimentEngines[i].ExperimentQueue.Filename) == true)
                    {
                        File.Delete(appData.labExperimentEngines[i].ExperimentQueue.Filename);
                    }
                }
            }

            //
            // Now start the lab experiment manager
            //
            this.Start();
        }

        //-------------------------------------------------------------------------------------------------//

        public void Start()
        {
            const string STRLOG_MethodName = "Start";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Start the lab experiment manager thread running
            //
            if (this.threadLabExperimentManager.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                this.disposed = false;
                this.running = true;
                this.threadLabExperimentManager.Start();
            }

            //
            // Wait for thread to become ready, with timeout
            //
            for (int i = 0; i < 3; i++)
            {
                if (this.Status == StatusCodes.Ready)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            if (this.Status != StatusCodes.Ready)
            {
                Logfile.WriteError(STRERR_LabExperimentManagerFailedReady);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Signal the waiting thread that experiment has been submitted and is ready for execution.
        /// </summary>
        public bool SignalSubmitted()
        {
            const string STRLOG_MethodName = "SignalSubmitted";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool ok = false;

            if (this.disposed == false)
            {
                // Signal waiting thread
                lock (this.signalSubmitted)
                {
                    Monitor.Pulse(this.signalSubmitted);
                }
                ok = true;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return ok;
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Do not make this method virtual. A derived class should not be allowed to override this method.
        /// </summary>
        public void Close()
        {
            const string STRLOG_MethodName = "Close";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Calls the Dispose method without parameters
            Dispose();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Implement IDisposable. Do not make this method virtual. A derived class should not be able
        /// to override this method.
        /// </summary>
        public void Dispose()
        {
            const string STRLOG_MethodName = "Dispose";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Dispose(true);

            // Take yourself off the Finalization queue to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Use C# destructor syntax for finalization code. This destructor will run only if the Dispose
        /// method does not get called. It gives your base class the opportunity to finalize. Do not provide
        /// destructors in types derived from this class.
        /// </summary>
        ~LabExperimentManager()
        {
            Trace.WriteLine("~LabExperimentManager():");

            //
            // Do not re-create Dispose clean-up code here. Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            //
            Dispose(false);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios:
        /// 1. If disposing equals true, the method has been called directly or indirectly by a user's code.
        ///    Managed and unmanaged resources can be disposed.
        /// 2. If disposing equals false, the method has been called by the runtime from inside the finalizer
        ///    and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            const string STRLOG_MethodName = "Dispose";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Write("Dispose():  disposing: " + disposing.ToString() + "  disposed: " + this.disposed.ToString());

            //
            // Check to see if Dispose has already been called
            //
            if (this.disposed == false)
            {
                //
                // If disposing equals true, dispose all managed and unmanaged resources.
                //
                if (disposing == true)
                {
                    // Dispose managed resources here. Anything that has a Dispose() method.
                }

                //
                // Release unmanaged resources here. If disposing is false, only the following
                // code is executed.
                //
                for (int i = 0; i < this.appData.farmSize; i++)
                {
                    if (this.appData != null && this.appData.labExperimentEngines != null && this.appData.labExperimentEngines[i] != null)
                    {
                        this.appData.labExperimentEngines[i].Close();
                    }
                }

                //
                // Tell LabExperimentThread() that it is no longer running
                //
                lock (this.statusLock)
                {
                    this.running = false;
                }

                //
                // Lab experiment manager thread may be waiting for an experiment submission signal
                //
                lock (this.signalSubmitted)
                {
                    Monitor.Pulse(this.signalSubmitted);
                }

                //
                // ... or could be waiting for an experiment engine completion signal
                //
                lock (this.signalCompleted)
                {
                    Monitor.Pulse(this.signalCompleted);
                }

                //
                // Wait for LabExperimentThread() to terminate
                //
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (this.threadLabExperimentManager.Join(1000) == true)
                        {
                            // Thread has terminated
                            Trace.WriteLine(" OK");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }

                this.disposed = true;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //=================================================================================================//

        private enum States
        {
            sInit, sIdle, sCheckExperimentQueue, sFindAvailableEngine, sWaitForAvailableEngine, sIssue
        }

        //-------------------------------------------------------------------------------------------------//

        private void LabExperimentManagerThread()
        {
            const string STRLOG_MethodName = "LabExperimentManagerThread";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            States state = States.sIdle;
            States lastState = States.sInit;
            int lastEngineIndex = this.appData.farmSize - 1;

            //
            // State machine loop
            //
            try
            {
                while (Running == true)
                {
                    //
                    // Display message on each state change
                    //
                    if (state != lastState)
                    {
                        Logfile.Write(" " + lastState.ToString() + "->" + state.ToString());
                        lastState = state;
                    }

                    switch (state)
                    {
                        case States.sIdle:

                            //
                            // Update lab manager status
                            //
                            lock (this.statusLock)
                            {
                                this.online = true;
                                this.labStatusMessage = StatusCodes.Ready.ToString();
                                this.status = StatusCodes.Ready;
                            }

                            Logfile.Write(STRLOG_LabExperimentManagerIsReady);

                            //
                            // Wait for an experiment to be submitted
                            //
                            lock (this.signalSubmitted)
                            {
                                if (Monitor.Wait(this.signalSubmitted) == true)
                                {
                                    // An experiment has been submitted
                                    Trace.WriteLine(STRLOG_MethodName + ": signalSubmitted received");

                                    // Check if thread is still running
                                    if (Running == true)
                                    {
                                        // Still running
                                        state = States.sCheckExperimentQueue;
                                    }
                                }
                            }
                            break;

                        case States.sCheckExperimentQueue:

                            // Check the queue to make sure an experiment was submitted
                            if (this.experimentQueue.Peek() != null)
                            {
                                state = States.sFindAvailableEngine;
                            }
                            else
                            {
                                // False alarm
                                state = States.sIdle;
                            }
                            break;

                        case States.sFindAvailableEngine:

                            //
                            // Find an available experiment engine to pass the experiment on to
                            //
                            for (int i = 0; i < this.appData.farmSize; i++)
                            {
                                //
                                // Determine which engine to look at
                                //
                                if (++lastEngineIndex == this.appData.farmSize)
                                {
                                    lastEngineIndex = 0;
                                }

                                LabExperimentEngine labExperimentEngine = this.appData.labExperimentEngines[lastEngineIndex];

                                if (labExperimentEngine.Status == StatusCodes.Ready)
                                {
                                    //
                                    // This experiment engine is ready to accept an experiment
                                    //
                                    ExperimentInfo experimentInfo = this.experimentQueue.Dequeue();
                                    if (experimentInfo != null)
                                    {
                                        // Pass on the experiment to the experiment engine
                                        QueuedExperimentInfo queuedExperimentInfo = labExperimentEngine.ExperimentQueue.Enqueue(experimentInfo);
                                        if (queuedExperimentInfo != null)
                                        {
                                            //
                                            // Start the lab experiment engine to run the experiment
                                            //
                                            bool ok = labExperimentEngine.Start();
                                        }
                                    }

                                    state = States.sCheckExperimentQueue;
                                    break;
                                }
                            }

                            //
                            // Check if an available engine was found
                            //
                            if (state == States.sFindAvailableEngine)
                            {
                                // An available engine has not been found
                                state = States.sWaitForAvailableEngine;
                            }
                            break;

                        case States.sWaitForAvailableEngine:

                            //
                            // Wait for an experiment engine to complete execution
                            //
                            lock (this.signalCompleted)
                            {
                                if (Monitor.Wait(this.signalCompleted) == true)
                                {
                                    // An experiment has been submitted
                                    Trace.WriteLine(STRLOG_MethodName + ": signalCompleted received");

                                    // Check if thread is still running
                                    if (Running == true)
                                    {
                                        // Still runnning
                                        state = States.sFindAvailableEngine;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(STRLOG_ClassName + "-" + ex.Message);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        private LabExperimentStatus GetLabExperimentStatus(int experimentId, string sbName)
        {
            const string STRLOG_MethodName = "GetLabExperimentStatus";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            LabExperimentStatus labExperimentStatus = null;

            //
            // Check experiment staus of each experiment engine
            //
            for (int i = 0; i < this.appData.farmSize; i++)
            {
                LabExperimentEngine labExperimentEngine = this.appData.labExperimentEngines[i];

                labExperimentStatus = labExperimentEngine.GetLabExperimentStatus(experimentId, sbName);
                if ((StatusCodes)labExperimentStatus.statusReport.statusCode != StatusCodes.Unknown)
                {
                    // This engine is running the experiment
                    break;
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return (labExperimentStatus);
        }

        //-------------------------------------------------------------------------------------------------//

        private int GetMinRemainingRuntime()
        {
            const string STRLOG_MethodName = "GetLabExperimentStatus";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            int minRemainingRuntime = Int32.MaxValue;

            //
            // Check experiment staus of each experiment engine
            //
            for (int i = 0; i < this.appData.farmSize; i++)
            {
                LabExperimentEngine labExperimentEngine = this.appData.labExperimentEngines[i];

                //
                // Get the remaining runtime for this experiment engine
                //
                int remainingRuntime = labExperimentEngine.GetRemainingRuntime();

                // Check if this is a smaller value
                if (remainingRuntime < minRemainingRuntime)
                {
                    minRemainingRuntime = remainingRuntime;
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return (minRemainingRuntime);
        }
    }
}
