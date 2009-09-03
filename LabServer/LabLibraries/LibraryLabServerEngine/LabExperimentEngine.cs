using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Library.Lab;
using Library.LabServerEngine;

namespace Library.LabServerEngine
{
    public class LabExperimentEngine : IDisposable
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "LabExperimentEngine";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_unitId = " unitId: ";
        private const string STRLOG_experimentId = " experimentId: ";
        private const string STRLOG_sbName = " sbName: ";
        private const string STRLOG_statusCode = " statusCode: ";
        private const string STRLOG_remainingRuntime = " remainingRuntime: ";
        private const string STRLOG_estRuntime = " estRuntime: ";
        private const string STRLOG_QueuePosition = " Queue Position: ";
        private const string STRLOG_LabExperimentEngineIsReady = " Lab experiment engine is ready.";
        private const string STRLOG_QueuedExperimentCancelled = " Experiment was cancelled while queued";
        private const string STRLOG_ActualExecutionTime = " Actual execution time: ";
        private const string STRLOG_seconds = " seconds";
        private const string STRLOG_LabExperimentEngineAlreadyStarted = "Lab experiment engine is already started!";
        private const string STRLOG_LabExperimentEngineThreadState = " LabExperimentEngine thread state -> ";

        //
        // String constants for exception messages
        //
        private const string STRERR_appData = "appData";
        private const string STRERR_allowedCallers = "allowedCallers";
        private const string STRERR_experimentResults = "experimentResults";
        private const string STRERR_experimentStatistics = "experimentStatistics";
        private const string STRERR_labConfiguration = "labConfiguration";
        private const string STRERR_signalCompleted = "signalCompleted";
        private const string STRERR_labEquipmentEngine = "labEquipmentEngine";
        private const string STRERR_experimentQueue = "experimentQueue";
        private const string STRERR_statusLock = "statusLock";
        private const string STRERR_signalSubmitted = "signalSubmitted";
        private const string STRERR_threadLabExperimentEngine = "threadLabExperimentEngine";
        private const string STRERR_LabExperimentEngineFailedStart = "Lab experiment engine failed to start!";
        private const string STRERR_QueueRemoveFailed = "Failed to remove experiment from the queue!";

        /// <summary>
        /// Information about the currently running experiment.
        /// </summary>
        public class LabExperimentInfo
        {
            public ExperimentInfo experimentInfo;
            public CancelExperiment cancelExperiment;
            public DateTime startDateTime;
            public double minTimeToLive;
            public string errorMessage;

            public LabExperimentInfo(ExperimentInfo experimentInfo, DateTime startDateTime)
            {
                this.experimentInfo = experimentInfo;
                this.cancelExperiment = new CancelExperiment();
                this.startDateTime = startDateTime;
                this.minTimeToLive = 0.0;
                this.errorMessage = null;
            }
        }

        //
        // Local variables
        //
        private AllowedCallers allowedCallers;
        private ExperimentResults experimentResults;
        private ExperimentStatistics experimentStatistics;
        private object signalCompleted;
        private bool disposed;
        private Object statusLock;
        private Thread threadLabExperimentEngine;

        //
        // Local variables available to a derived class
        //
        protected int unitId;
        protected LabConfiguration labConfiguration;
        protected LabExperimentInfo labExperimentInfo;

        #endregion

        #region Private Properties

        private bool running;

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
            set
            {
                lock (this.statusLock)
                {
                    this.running = value;
                }
            }
        }

        #endregion

        #region Public Properties

        private bool online;
        private string labStatusMessage;
        private StatusCodes status;
        private ExperimentQueue experimentQueue;

        public bool Online
        {
            get { return this.online; }
        }

        public string LabStatusMessage
        {
            get  { return this.labStatusMessage; }
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

        public ExperimentQueue ExperimentQueue
        {
            get { return this.experimentQueue; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public LabExperimentEngine(int unitId, AppData appData)
        {
            const string STRLOG_MethodName = "LabExperimentEngine";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            Logfile.Write(STRLOG_unitId + unitId.ToString());

            //
            // Initialise local variables
            //
            this.disposed = true;
            this.unitId = unitId;
            this.labExperimentInfo = null;

            //
            // Initialise private properties
            //
            this.running = false;

            //
            // Initialise public properties
            //
            this.online = false;
            this.labStatusMessage = null;
            this.status = StatusCodes.Unknown;

            try
            {
                //
                // Initialise local variables from application data
                //
                if (appData == null)
                {
                    throw new ArgumentNullException(STRERR_appData);
                }
                this.allowedCallers = appData.allowedCallers;
                if (this.allowedCallers == null)
                {
                    throw new ArgumentNullException(STRERR_allowedCallers);
                }
                this.experimentResults = appData.experimentResults;
                if (this.experimentResults == null)
                {
                    throw new ArgumentNullException(STRERR_experimentResults);
                }
                this.experimentStatistics = appData.experimentStatistics;
                if (this.experimentStatistics == null)
                {
                    throw new ArgumentNullException(STRERR_experimentStatistics);
                }
                this.labConfiguration = appData.labConfiguration;
                if (this.labConfiguration == null)
                {
                    throw new ArgumentNullException(STRERR_labConfiguration);
                }
                this.signalCompleted = appData.signalCompleted;
                if (this.signalCompleted == null)
                {
                    throw new ArgumentNullException(STRERR_signalCompleted);
                }

                //
                // Create the experiment queue object
                //
                this.experimentQueue = new ExperimentQueue(this.unitId);
                if (this.experimentQueue == null)
                {
                    throw new ArgumentNullException(STRERR_experimentQueue);
                }

                //
                // Create thread objects
                //
                this.statusLock = new Object();
                if (this.statusLock == null)
                {
                    throw new ArgumentNullException(STRERR_statusLock);
                }
                this.threadLabExperimentEngine = new Thread(new ThreadStart(LabExperimentEngineThread));
                if (this.threadLabExperimentEngine == null)
                {
                    throw new ArgumentNullException(STRERR_threadLabExperimentEngine);
                }

                //
                // Update lab status
                //
                this.online = true;
                this.labStatusMessage = StatusCodes.Ready.ToString();
                this.status = StatusCodes.Ready;

                Logfile.Write(STRLOG_LabExperimentEngineIsReady + STRLOG_unitId + this.unitId.ToString());

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

        public bool Start()
        {
            const string STRLOG_MethodName = "Start";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Write(STRLOG_unitId + this.unitId.ToString());

            bool success = true;

            try
            {
                //
                // Start the lab experiment engine thread running
                //
                this.threadLabExperimentEngine.Start();
            }
            catch (ThreadStateException)
            {
                try
                {
                    //
                    // Create a new thread and start it
                    //
                    this.threadLabExperimentEngine = new Thread(new ThreadStart(LabExperimentEngineThread));
                    this.threadLabExperimentEngine.Start();
                }
                catch (ThreadStateException ex)
                {
                    Logfile.WriteError(STRERR_LabExperimentEngineFailedStart);
                    Logfile.Write(ex.Message);
                    success = false;
                }
            }

            if (success == true)
            {
                //
                // Give the thread a chance to start running and then check that it
                //
                int timeout = 5;
                while (--timeout > 0)
                {
                    Thread.Sleep(500);
                    if (this.Running == true)
                    {
                        // Lab experiment engine thread has started running
                        break;
                    }
                    Trace.Write('?');
                }
                if (timeout == 0)
                {
                    Logfile.WriteError(STRERR_LabExperimentEngineFailedStart);
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool Cancel(int experimentID, string sbName)
        {
            const string STRLOG_MethodName = "Cancel";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool cancelled = false;

            Logfile.Write(STRLOG_experimentId + experimentID.ToString());
            Logfile.Write(STRLOG_sbName + sbName);

            //
            // Check if an experiment is currently running
            //
            lock (this.statusLock)
            {
                if (this.status == StatusCodes.Running)
                {
                    //
                    // An experiment is currently running, check if it is this one
                    //
                    if (this.labExperimentInfo.experimentInfo.experimentId == experimentID &&
                        sbName != null && sbName.Equals(this.labExperimentInfo.experimentInfo.sbName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Cancel the experiment
                        this.labExperimentInfo.cancelExperiment.Cancel();
                        cancelled = true;
                    }
                }
            }

            Logfile.Write(" cancelled: " + cancelled.ToString());

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return cancelled;
        }

        //-------------------------------------------------------------------------------------------------//

        public LabExperimentStatus GetLabExperimentStatus(int experimentId, string sbName)
        {
            const string STRLOG_MethodName = "GetLabExperimentStatus";

            string logMessage = STRLOG_experimentId + experimentId.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_sbName + Logfile.STRLOG_Quote + sbName + Logfile.STRLOG_Quote;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            LabExperimentStatus labExperimentStatus = new LabExperimentStatus(
                new ExperimentStatus((int)StatusCodes.Unknown));

            int remainingRuntime = 0;
            logMessage = null;

            lock (this.statusLock)
            {
                //
                // Check if the specified experiment is running or just completed
                //
                if ((this.labExperimentInfo != null) &&
                    (labExperimentInfo.experimentInfo.experimentId == experimentId) &&
                    (labExperimentInfo.experimentInfo.sbName.Equals(sbName, StringComparison.OrdinalIgnoreCase) == true))
                {
                    //
                    // Update the status code, either StatusCodes.Running or StatusCodes.Completed or StatusCodes.Cancelled
                    //
                    labExperimentStatus.statusReport.statusCode = (int)this.status;

                    if (this.status == StatusCodes.Running)
                    {
                        //
                        // The specified experiment is currently running, fill in information
                        //
                        labExperimentStatus.statusReport.estRuntime = (double)this.labExperimentInfo.experimentInfo.estExecutionTime;

                        // Calculate time already passed for the experiment
                        remainingRuntime = (int)((TimeSpan)(DateTime.Now - this.labExperimentInfo.startDateTime)).TotalSeconds;

                        // Now calculate the time remaining for the experiment
                        remainingRuntime = this.labExperimentInfo.experimentInfo.estExecutionTime - remainingRuntime;

                        //
                        // Estimated runtime may have been underestimated. Don't say remaining runtime is zero while
                        // the experiment is still running.
                        //
                        if (remainingRuntime < 1)
                        {
                            remainingRuntime = 1;
                        }

                        labExperimentStatus.statusReport.estRemainingRuntime = (double)remainingRuntime;

                        logMessage = STRLOG_estRuntime + labExperimentStatus.statusReport.estRuntime.ToString() +
                            Logfile.STRLOG_Spacer + STRLOG_remainingRuntime + labExperimentStatus.statusReport.estRemainingRuntime.ToString();
                    }
                }
                else if (this.status == StatusCodes.Ready)
                {
                    //
                    // Experiment is not currently running, maybe waiting on the local queue ready to run
                    //
                    QueuedExperimentInfo queuedExperimentInfo = this.experimentQueue.GetExperimentInfo(experimentId, sbName);
                    if (queuedExperimentInfo != null)
                    {
                        // Set the experiment status
                        labExperimentStatus.statusReport.statusCode = (int)StatusCodes.Waiting;

                        // Get the queue position and wait time
                        labExperimentStatus.statusReport.wait =
                            new WaitEstimate(queuedExperimentInfo.position, queuedExperimentInfo.waitTime);

                        // Get the time it takes to run the experiment 
                        labExperimentStatus.statusReport.estRuntime = queuedExperimentInfo.estExecutionTime;
                        labExperimentStatus.statusReport.estRemainingRuntime = queuedExperimentInfo.estExecutionTime;

                        logMessage = STRLOG_estRuntime + labExperimentStatus.statusReport.estRuntime.ToString() +
                            Logfile.STRLOG_Spacer + STRLOG_remainingRuntime + labExperimentStatus.statusReport.estRemainingRuntime.ToString();
                    }
                }
                else
                {
                    // The specified experiment may have already completed or is unknown
                }
            }

            logMessage = STRLOG_statusCode + ((StatusCodes)labExperimentStatus.statusReport.statusCode).ToString() +
                Logfile.STRLOG_Spacer + logMessage;

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return (labExperimentStatus);
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetRemainingRuntime()
        {
            const string STRLOG_MethodName = "GetRemainingRuntime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            int remainingRuntime = 0;

            lock (this.statusLock)
            {
                //
                // Check if any experiment is currently running
                //
                if (this.status == StatusCodes.Running)
                {
                    // Calculate time already passed for the experiment
                    remainingRuntime = (int)((TimeSpan)(DateTime.Now - this.labExperimentInfo.startDateTime)).TotalSeconds;

                    // Now calculate the time remaining for the experiment
                    remainingRuntime = this.labExperimentInfo.experimentInfo.estExecutionTime - remainingRuntime;

                    // Cannot have a negative time
                    if (remainingRuntime < 0)
                    {
                        remainingRuntime = 0;
                    }
                }
                else if (this.status == StatusCodes.Waiting)
                {
                    //
                    // Experiment is not currently running, maybe waiting to run
                    //
                    ExperimentInfo experimentInfo = this.experimentQueue.Peek();
                    if (experimentInfo != null)
                    {
                        remainingRuntime = experimentInfo.estExecutionTime;
                    }
                }
                else
                {
                    // There is no experiment currently running
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return (remainingRuntime);
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual ValidationReport Validate(string xmlSpecification)
        {
            const string STRLOG_MethodName = "Validate";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            ValidationReport validationReport = new ValidationReport(false);

            //
            // Parse the XML specification string to generate a validation report
            //
            try
            {
                ExperimentSpecification experimentSpecification = new ExperimentSpecification(this.labConfiguration);
                validationReport = experimentSpecification.Parse(xmlSpecification);
            }
            catch (Exception ex)
            {
                validationReport.errorMessage = ex.Message;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return validationReport;
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Do not make this method virtual. A derived class should not be allowed to override this method.
        /// </summary>
        public void Close()
        {
            const string STRLOG_MethodName = "Close";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Write(STRLOG_unitId + this.unitId.ToString());

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
        ~LabExperimentEngine()
        {
            Trace.WriteLine("~LabExperimentEngine():");

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

                //
                // Tell LabExperimentEngineThread() that it is no longer running
                //
                if (this.Running == true)
                {
                    this.Running = false;

                    //
                    // Wait for LabExperimentEngineThread() to terminate
                    //
                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (this.threadLabExperimentEngine.Join(500) == true)
                            {
                                // Thread has terminated
                                Trace.WriteLine("LabExperimentEngineThread() has exited.");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }

                this.disposed = true;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //=================================================================================================//

        private enum States
        {
            sGetExperiment, sPrepareExperiment, sRunExperiment, sConcludeExperiment, sDone
        }

        //-------------------------------------------------------------------------------------------------//

        private void LabExperimentEngineThread()
        {
            const string STRLOG_MethodName = "LabExperimentEngineThread";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            ExperimentInfo experimentInfo = null;

            //
            // Initialise state machine
            //
            this.Running = true;
            States state = States.sGetExperiment;
            States lastState = States.sDone;

            try
            {
                //
                // State machine loop
                //
                while (this.Running == true)
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
                        case States.sGetExperiment:

                            // Get the next experiment from the queue without removing it
                            experimentInfo = this.experimentQueue.Peek();

                            // Check if there is an experiment
                            if (experimentInfo != null)
                            {
                                // Prepare experiment for running
                                state = States.sPrepareExperiment;
                                break;
                            }

                            // No experiment to run
                            this.Running = false;
                            break;

                        case States.sPrepareExperiment:

                            // Prepare experiment ready for running
                            if (PrepareExperiment(experimentInfo) == true)
                            {
                                // Status has changed from StatusCodes.Ready to StatusCodes.Running

                                // Run the experiment
                                state = States.sRunExperiment;
                                break;
                            }

                            // Preparation failed
                            state = States.sConcludeExperiment;
                            break;

                        case States.sRunExperiment:

                            // Run the experiment
                            experimentInfo = RunExperiment(experimentInfo);

                            state = States.sConcludeExperiment;
                            break;

                        case States.sConcludeExperiment:

                            // Conclude experiment after running
                            ConcludeExperiment(experimentInfo);

                            // Status has changed back to StatusCodes.Ready

                            //
                            // Remove the experiment from the queue
                            //
                            if (this.experimentQueue.Remove(experimentInfo.experimentId, experimentInfo.sbName) == false)
                            {
                                Logfile.WriteError(STRERR_QueueRemoveFailed);
                            }

                            //
                            // Tell the lab experiment manager that this experiment is completed
                            //
                            lock (this.signalCompleted)
                            {
                                Monitor.Pulse(this.signalCompleted);
                            }

                            // Experiment is finished, so exit
                            this.Running = false;
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

        private bool PrepareExperiment(ExperimentInfo experimentInfo)
        {
            const string STRLOG_MethodName = "PrepareExperiment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool ok = true;

            //
            // Check if the experiment has been cancelled
            //
            if (experimentInfo.cancelled == true)
            {
                experimentInfo.resultReport = new ResultReport((int)StatusCodes.Cancelled, STRLOG_QueuedExperimentCancelled);
                ok = false;

                Logfile.Write(STRLOG_QueuedExperimentCancelled);
            }
            else
            {
                //
                // Update the statistics
                //
                DateTime now = DateTime.Now;
                ok = this.experimentStatistics.Started(experimentInfo.experimentId, experimentInfo.sbName, this.unitId, now);

                //
                // Create an instance of LabExperimentInfo to execute the experiment
                //
                lock (this.statusLock)
                {
                    experimentInfo.unitId = this.unitId;
                    this.labExperimentInfo = new LabExperimentInfo(experimentInfo, now);
                    this.status = StatusCodes.Running;
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return ok;
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Run the experiment and fill in the result report. Override in a derived class.
        /// </summary>
        /// <param name="experimentInfo"></param>
        /// <returns></returns>
        public virtual ExperimentInfo RunExperiment(ExperimentInfo experimentInfo)
        {
            const string STRLOG_MethodName = "RunExperiment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Create a result report ready to fill in
            experimentInfo.resultReport = new ResultReport();

            try
            {
                //
                // Parse the XML specification string to generate a validation report (should be accepted!)
                //
                ExperimentSpecification experimentSpecification = new ExperimentSpecification(this.labConfiguration);
                ValidationReport validationReport = experimentSpecification.Parse(experimentInfo.xmlSpecification);
                if (validationReport.accepted == false)
                {
                    throw new ArgumentException(validationReport.errorMessage);
                }

                //
                // Create an instance of the driver, execute the experiment and return the result information
                //
                DriverGeneric driver = new DriverGeneric(this.labExperimentInfo.cancelExperiment);

                //
                // Execute the experiment and return the result information
                //
                ExperimentResultInfo experimentResultInfo = driver.Execute(experimentSpecification);

                //
                // Create an instance of LabExperimentResult to convert the experiment results to an XML string
                //
                LabExperimentResult labExperimentResult = new LabExperimentResult(
                    experimentInfo.experimentId, experimentInfo.sbName, DateTime.Now,
                    experimentSpecification.SetupId, this.unitId, this.labConfiguration);

                //
                // Fill in the result report
                //
                experimentInfo.resultReport.experimentResults = labExperimentResult.ToString();
                experimentInfo.resultReport.statusCode = (int)experimentResultInfo.statusCode;
                experimentInfo.resultReport.errorMessage = experimentResultInfo.errorMessage;
            }
            catch (Exception ex)
            {
                experimentInfo.resultReport.statusCode = (int)StatusCodes.Failed;
                experimentInfo.resultReport.errorMessage = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return experimentInfo;
        }

        //-------------------------------------------------------------------------------------------------//

        private void ConcludeExperiment(ExperimentInfo experimentInfo)
        {
            const string STRLOG_MethodName = "ConcludeExperiment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Save the experiment results
            bool ok = this.experimentResults.Save(experimentInfo);

            if (ok == true)
            {
                //
                // Experiment results have been saved successfully, experiment is completed
                //
                lock (this.statusLock)
                {
                    this.status = StatusCodes.Completed;
                }

                //
                // Notify the ServiceBroker so that the results can be retrieved
                //
                try
                {
                    LabServerToSbAPI labServerToSbAPI = new LabServerToSbAPI(this.allowedCallers);
                    labServerToSbAPI.Notify(experimentInfo.experimentId, experimentInfo.sbName);
                }
                catch (Exception ex)
                {
                    //
                    // Nothing can be done if it fails
                    //
                    if (ex.InnerException != null)
                    {
                        Logfile.WriteError(ex.InnerException.Message);
                    }
                    else
                    {
                        Logfile.WriteError(ex.Message);
                    }
                }
            }

            //
            // Update the statistics
            //
            DateTime now = DateTime.Now;
            if ((StatusCodes)experimentInfo.resultReport.statusCode == StatusCodes.Cancelled)
            {
                // Experiment was cancelled - update statistics
                this.experimentStatistics.Cancelled(experimentInfo.experimentId, experimentInfo.sbName, now);
            }
            else
            {
                //
                // Determine actual execution time of the experiment
                //
                TimeSpan timeSpan = now - this.labExperimentInfo.startDateTime;
                int executionTime = (int)timeSpan.TotalSeconds;
                Logfile.Write(STRLOG_ActualExecutionTime + executionTime.ToString() + STRLOG_seconds);

                // Experiment completed - update statistics
                this.experimentStatistics.Completed(experimentInfo.experimentId, experimentInfo.sbName, now);
            }

            //
            // Experiment is finished
            //
            lock (this.statusLock)
            {
                this.labExperimentInfo = null;
                this.status = StatusCodes.Ready;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

    }
}
