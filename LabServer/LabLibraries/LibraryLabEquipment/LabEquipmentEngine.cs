using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Library.LabEquipment
{
    public class LabEquipmentEngine : IDisposable
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "LabEquipmentEngine";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_PowerupDelayNotSpecified = "Powerup delay is not specified!";
        private const string STRLOG_InitialiseDelayNotSpecified = "Initialise delay is not specified!";
        private const string STRLOG_PowerdownTimeoutNotSpecified = "Powerdown timeout is not specified!";
        private const string STRLOG_PowerupDelay = " Powerup Delay: ";
        private const string STRLOG_InitialiseDelay = " Initialise Delay: ";
        private const string STRLOG_PowerdownTimeout = " Powerdown Timeout: ";
        private const string STRLOG_PoweroffDelay = " PowerOff Delay: ";
        private const string STRLOG_Seconds = " seconds";
        private const string STRLOG_PowerdownDisabled = " Powerdown is disabled";
        private const string STRLOG_LabEquipmentIsReady = " Lab equipment is ready. ";
        private const string STRLOG_LabEquipmentEngineThreadAlreadyRunning = " Lab equipment engine thread is already running";
        private const string STRLOG_LabEquipmentEngineThreadIsStarting = " Lab equipment engine thread is starting...";
        private const string STRLOG_LabEquipmentEngineThreadIsRunning = " Lab equipment engine thread is running";

        //
        // String constants for exception messages
        //
        private const string STRERR_NumberIsNegative = "Number cannot be negative!";
        private const string STRERR_NumberIsInvalid = "Number is invalid!";
        private const string STRERR_signalPowerup = "signalPowerup";
        private const string STRERR_statusLock = "statusLock";
        private const string STRERR_threadLabEquipmentEngine = "threadLabEquipmentEngine";
        private const string STRERR_LabEquipmentEngineThreadFailedToStart = "Lab equipment engine thread failed to start!";
        private const string STRERR_LabEquipmentFailedToBecomeReady = "Lab equipment failed to become ready!";

        //
        // Local variables
        //
        private bool disposed;
        private Object signalPowerup;
        private Object statusLock;
        private Thread threadLabEquipmentEngine;

        //
        // These need to be locked by 'statusLock'
        //
        private int slPowerupTimeRemaining;
        private int slPowerdownTimeRemaining;
        private DateTime slInitialiseStartTime;

        //
        // Constants
        //

        /// <summary>
        /// Time in seconds to wait after the equipment is powered up if not already specified.
        /// </summary>
        private const int MIN_DelayPowerup = 5;

        /// <summary>
        /// Time in seconds to wait for equipment initialisation
        /// </summary>
        private const int MIN_DelayInitialise = 15;

        /// <summary>
        /// Time in seconds to wait to powerup the equipment after it has been powered down.
        /// </summary>
        private const int MIN_DelayPoweroff = 10;

        #endregion

        #region Public Properties

        private bool powerdownEnabled;
        private int powerupDelay;
        private int initialiseDelay;
        private int powerdownTimeout;

        /// <summary>
        /// True if the powerup delay and powerdown timeout have been specified
        /// in the Application's configuration file and are valid.
        /// </summary>
        public bool PowerdownEnabled
        {
            get { return this.powerdownEnabled; }
        }

        /// <summary>
        /// Time in seconds for the equipment to become ready to initialise after power has been applied.
        /// </summary>
        public int PowerupDelay
        {
            get { return this.powerupDelay; }
            set { this.powerupDelay = value; }
        }

        /// <summary>
        /// Time in seconds for the equipment to initialise after power has been applied.
        /// </summary>
        public int InitialiseDelay
        {
            get { return this.initialiseDelay; }
            set { this.initialiseDelay = value; }
        }

        /// <summary>
        /// Time in seconds of inactivity that the equipment will wait before power is removed.
        /// </summary>
        public int PowerdownTimeout
        {
            get { return this.powerdownTimeout; }
        }

        /// <summary>
        /// Time in seconds to wait after equipment power is removed before it can be applied again.
        /// </summary>
        public int PoweroffDelay
        {
            get { return MIN_DelayPoweroff; }
        }

        /// <summary>
        /// Return the time in seconds before the equipment engine is ready to execute commands
        /// after the equipment has been powered up.
        /// </summary>
        public int TimeUntilReady
        {
            get
            {
                lock (this.statusLock)
                {
                    string logMessage = " [TimeUntilReady] ";
                    int seconds = 0;

                    if (this.slIsReady == true)
                    {
                        // Equipment is powered up and ready to use
                        logMessage += "Ready";
                    }
                    else if (this.slRunning == false)
                    {
                        // Equipment is powered down
                        seconds = this.powerupDelay + this.initialiseDelay;
                        logMessage += "Not Running";
                    }
                    else if (this.slPowerupTimeRemaining > this.powerupDelay)
                    {
                        // Equipment has powered off
                        seconds = this.slPowerupTimeRemaining + this.initialiseDelay;
                        logMessage += "Poweroff";
                    }
                    else if (this.slPowerupTimeRemaining > 0)
                    {
                        // Equipment is still powering up
                        seconds = this.slPowerupTimeRemaining + this.initialiseDelay;
                        logMessage += "Powerup";
                    }
                    else
                    {
                        //
                        // Equipment has powered up and is initialising
                        //
                        TimeSpan timeSpan = DateTime.Now - this.slInitialiseStartTime;
                        seconds = this.initialiseDelay;
                        try
                        {
                            // Get the time to complete initialisation
                            seconds -= Convert.ToInt32(timeSpan.TotalSeconds);

                            //
                            // Don't say initialisation has completed until it actually has
                            //
                            if (seconds < 1)
                            {
                                seconds = 1;
                            }
                        }
                        catch
                        {
                        }
                        logMessage += "Initialise";
                    }

                    Logfile.Write(logMessage + " Seconds: " + seconds.ToString());

                    return seconds;
                }
            }
        }

        /// <summary>
        /// Time remaining in seconds before the equipment powers down.
        /// </summary>
        public int TimeUntilPowerdown
        {
            get
            {
                lock (this.statusLock)
                {
                    return this.slPowerdownTimeRemaining;
                }
            }
        }

        #endregion

        #region Private Properties

        //
        // These need to be locked by 'statusLock'
        //
        private bool slRunning;
        private bool slIsReady;
        private bool slPowerdownSuspended;

        private bool Running
        {
            get
            {
                lock (this.statusLock)
                {
                    return this.slRunning;
                }
            }
            set
            {
                lock (this.statusLock)
                {
                    this.slRunning = value;
                }
            }
        }

        /// <summary>
        /// True if the equipment is powered up and ready to use.
        /// </summary>
        private bool IsReady
        {
            get
            {
                lock (this.statusLock)
                {
                    return this.slIsReady;
                }
            }
        }

        /// <summary>
        /// True if powerdown has been suspended
        /// </summary>
        private bool PowerdownSuspended
        {
            get
            {
                lock (this.statusLock)
                {
                    return this.slPowerdownSuspended;
                }
            }
            set
            {
                lock (this.statusLock)
                {
                    this.slPowerdownSuspended = value;
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Initialises a  new instance of the Library.LabServer.LabEquipmentPower class.
        /// </summary>
        public LabEquipmentEngine()
            : this(null, null, null)
        {
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Unit testing use only - Initialises a  new instance of the Library.LabServer.LabEquipmentPower
        /// class. Powerup delay and powerdown timeout are specified to test the exception handling.
        /// </summary>
        /// <param name="pud"></param>
        /// <param name="pdt"></param>
        public LabEquipmentEngine(string powerupDelay, string initialiseDelay, string powerdownTimeout)
        {
            const string STRLOG_MethodName = "LabEquipmentEngine";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Get powerup delay, may not be specified
            //
            try
            {
                if (powerupDelay == null)
                {
                    // Get the powerup delay from the config file
                    this.powerupDelay = Utilities.GetIntAppSetting(Consts.StrCfg_PowerupDelay);
                }
                else
                {
                    // Get the powerup delay from argument passed in
                    this.powerupDelay = Int32.Parse(powerupDelay);
                }
                if (this.powerupDelay < 0)
                {
                    // Value cannot be negative
                    throw new ArgumentException(STRERR_NumberIsNegative);
                }
                if (this.powerupDelay < MIN_DelayPowerup)
                {
                    // Set minimum powerup delay
                    this.powerupDelay = MIN_DelayPowerup;
                }
            }
            catch (ArgumentNullException)
            {
                Logfile.Write(STRLOG_PowerupDelayNotSpecified);

                // Powerup delay is not specified, set minimum powerup delay
                this.powerupDelay = MIN_DelayPowerup;
            }
            catch (FormatException)
            {
                // Value cannot be converted
                throw new ArgumentException(STRERR_NumberIsInvalid, Consts.StrCfg_PowerupDelay);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw new ArgumentException(ex.Message, Consts.StrCfg_PowerupDelay);
            }

            //
            // Get initialisation delay, may not be specified
            //
            try
            {
                if (initialiseDelay == null)
                {
                    // Get the initialisation delay from the config file
                    this.initialiseDelay = Utilities.GetIntAppSetting(Consts.StrCfg_InitialiseDelay);
                }
                else
                {
                    // Get the initialisation delay from argument passed in
                    this.initialiseDelay = Int32.Parse(initialiseDelay);
                }
                if (this.initialiseDelay < 0)
                {
                    // Value cannot be negative
                    throw new ArgumentException(STRERR_NumberIsNegative);
                }
                if (this.initialiseDelay < MIN_DelayInitialise)
                {
                    // Set minimum initialisation delay
                    this.initialiseDelay = MIN_DelayInitialise;
                }
            }
            catch (ArgumentNullException)
            {
                Logfile.Write(STRLOG_InitialiseDelayNotSpecified);

                // Initialisation delay is not specified, set minimum initialisation delay
                this.initialiseDelay = MIN_DelayInitialise;
            }
            catch (FormatException)
            {
                // Value cannot be converted
                throw new ArgumentException(STRERR_NumberIsInvalid, Consts.StrCfg_InitialiseDelay);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw new ArgumentException(ex.Message, Consts.StrCfg_InitialiseDelay);
            }

            //
            // Get powerdown timeout, may not be specified
            //
            try
            {
                if (powerdownTimeout == null)
                {
                    // Get the powerdown timeout from the config file
                    this.powerdownTimeout = Utilities.GetIntAppSetting(Consts.StrCfg_PowerdownTimeout);
                }
                else
                {
                    this.powerdownTimeout = Int32.Parse(powerdownTimeout);
                }
                if (this.powerdownTimeout < 0)
                {
                    // Value cannot be negative
                    throw new Exception(STRERR_NumberIsNegative);
                }

                // Powerdown timeout is specified so enable powerdown
                this.powerdownEnabled = true;
            }
            catch (ArgumentNullException)
            {
                Logfile.Write(STRLOG_PowerdownTimeoutNotSpecified);

                // Powerdown timeout is not specified, disable powerdown
                this.powerdownEnabled = false;
            }
            catch (FormatException)
            {
                // Value cannot be converted
                throw new ArgumentException(STRERR_NumberIsInvalid, Consts.StrCfg_PowerdownTimeout);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw new ArgumentException(ex.Message, Consts.StrCfg_PowerdownTimeout);
            }

            //
            // Log details
            //
            Logfile.Write(STRLOG_PowerupDelay + this.powerupDelay.ToString() + STRLOG_Seconds);
            Logfile.Write(STRLOG_InitialiseDelay + this.initialiseDelay.ToString() + STRLOG_Seconds);
            if (this.powerdownEnabled == true)
            {
                Logfile.Write(STRLOG_PowerdownTimeout + this.PowerdownTimeout.ToString() + STRLOG_Seconds);
                Logfile.Write(STRLOG_PoweroffDelay + this.PoweroffDelay + STRLOG_Seconds);
            }
            else
            {
                Logfile.Write(STRLOG_PowerdownDisabled);
            }

            //
            // Initialise local variables
            //
            this.disposed = false;
            this.slPowerupTimeRemaining = this.powerupDelay;
            this.slInitialiseStartTime = DateTime.Now;
            this.slPowerdownTimeRemaining = this.powerdownTimeout;

            //
            // Initialise private properties
            //
            this.slRunning = false;
            this.slIsReady = false;
            this.slPowerdownSuspended = false;

            try
            {
                //
                // Create thread objects
                //
                this.signalPowerup = new Object();
                if (this.signalPowerup == null)
                {
                    throw new ArgumentNullException(STRERR_signalPowerup);
                }
                this.statusLock = new Object();
                if (this.statusLock == null)
                {
                    throw new ArgumentNullException(STRERR_statusLock);
                }
                this.threadLabEquipmentEngine = new Thread(new ThreadStart(LabEquipmentEngineThread));
                if (this.threadLabEquipmentEngine == null)
                {
                    throw new ArgumentNullException(STRERR_threadLabEquipmentEngine);
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

        /// <summary>
        /// Start the lab experiment engine thread. Check that the thread has started and return true if
        /// successful. This method does not wait for the equipment to become ready.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            const string STRLOG_MethodName = "Start";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            try
            {
                //
                // Start the lab equipment thread running
                //
                Logfile.Write(STRLOG_LabEquipmentEngineThreadIsStarting);

                this.threadLabEquipmentEngine.Start();

                //
                // Give the thread a chance to start and then check that it has started
                //
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    if (this.Running == true)
                    {
                        Logfile.Write(STRLOG_LabEquipmentEngineThreadIsRunning);
                        success = true;
                        break;
                    }
                    Trace.Write('?');
                }
                if (success == false)
                {
                    Logfile.WriteError(STRERR_LabEquipmentEngineThreadFailedToStart);
                }
            }
            catch (ThreadStateException ex)
            {
                Logfile.Write(ex.Message);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Suspend equipment powerdown. If the equipment is already powered down, the equipment is
        /// powered up. Wait until the equipment is powered up and ready to use before returning.
        /// </summary>
        public bool SuspendPowerdown()
        {
            const string STRLOG_MethodName = "SuspendPowerdown";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            if (this.powerdownEnabled == true && this.disposed == false)
            {
                // Suspend equipment powerdown
                this.PowerdownSuspended = true;

                // Wait a bit for thread to suspend powerdown if it is still running
                Thread.Sleep(1500);

                // Check if thread is still running
                if (this.Running == true)
                {
                    // Yes it is
                    Logfile.Write(STRLOG_LabEquipmentEngineThreadAlreadyRunning);
                }
                else
                {
                    try
                    {
                        //
                        // Create a new thread and start it
                        //
                        Logfile.Write(STRLOG_LabEquipmentEngineThreadIsStarting);

                        this.threadLabEquipmentEngine = new Thread(new ThreadStart(LabEquipmentEngineThread));
                        this.threadLabEquipmentEngine.Start();
                    }
                    catch (ThreadStateException ex)
                    {
                        Logfile.Write(ex.Message);
                        success = false;
                    }
                }

                //
                // Wait for the equipment to powerup (if necessary) and become ready to use.
                // Use a timeout so that we don't wait forever if something goes wrong.
                //
                if (success == true)
                {
                    int timeout = (this.PoweroffDelay + this.PowerupDelay + this.InitialiseDelay) * 2;
                    while (--timeout > 0)
                    {
                        if (this.IsReady == true)
                        {
                            // Equipment has powered up
                            Logfile.Write(STRLOG_LabEquipmentIsReady);
                            break;
                        }

                        Thread.Sleep(1000);
                        Trace.Write("?");
                    }
                    if (timeout == 0)
                    {
                        // Equipment failed to become ready ???
                        Logfile.WriteError(STRERR_LabEquipmentFailedToBecomeReady);
                        success = false;
                    }
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Resume equipment powerdown.
        /// </summary>
        public bool ResumePowerdown()
        {
            const string STRLOG_MethodName = "ResumePowerdown";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            if (this.powerdownEnabled == true && this.disposed == false)
            {
                //
                // The equipment may already be powered down, doesn't matter
                //
                this.PowerdownSuspended = false;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return true;
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual void PowerupEquipment()
        {
            const string STRLOG_MethodName = "PowerupEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual void InitialiseEquipment()
        {
            const string STRLOG_MethodName = "InitialiseEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual void PowerdownEquipment()
        {
            const string STRLOG_MethodName = "PowerdownEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
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
        ~LabEquipmentEngine()
        {
            Trace.WriteLine("~LabEquipmentEngine():");

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
            Logfile.Write(" Dispose(" + disposing.ToString() + ")  disposed: " + this.disposed.ToString());

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
                // Tell LabEquipmentEngineThread() that it is no longer running
                //
                if (this.Running == true)
                {
                    this.Running = false;

                    //
                    // Wait for LabEquipmentThread() to terminate
                    //
                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (this.threadLabEquipmentEngine.Join(500) == true)
                            {
                                // Thread has terminated
                                Trace.WriteLine("LabEquipmentEngineThread() has exited.");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }

                    //
                    // Powerdown the equipment
                    //
                    PowerdownEquipment();
                }

                this.disposed = true;
            }
        }

        //=================================================================================================//

        private enum States
        {
            sPowerInit, sPowerUpDelay, sPowerOnInit, sPowerOnReady, sPowerSuspended, sPowerOffDelay, sPowerOff
        }

        //-------------------------------------------------------------------------------------------------//

        private void LabEquipmentEngineThread()
        {
            const string STRLOG_MethodName = "LabEquipmentEngineThread";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Initialise state machine
            //
            this.Running = true;
            States state = States.sPowerInit;
            States lastState = States.sPowerOff;

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
                        case States.sPowerInit:

                            lock (this.statusLock)
                            {
                                // Initialise powerup delay
                                this.slPowerupTimeRemaining = this.powerupDelay;
                            }

                            // Powerup the equipment 
                            PowerupEquipment();

                            state = States.sPowerUpDelay;
                            break;

                        case States.sPowerUpDelay:

                            // Wait a bit
                            Thread.Sleep(1000);

                            lock (this.statusLock)
                            {
                                // Check timeout
                                if (--this.slPowerupTimeRemaining > 0)
                                {
                                    // Equipment is still powering up
                                    Trace.Write("u");
                                    continue;
                                }
                            }

                            // Equipment is now powered up
                            state = States.sPowerOnInit;
                            break;

                        case States.sPowerOnInit:

                            // Set the intialisation start time
                            lock (this.statusLock)
                            {
                                this.slInitialiseStartTime = DateTime.Now;
                            }

                            // Initialise the equipment
                            InitialiseEquipment();

                            lock (this.statusLock)
                            {
                                // Initialise powerdown timeout
                                this.slPowerdownTimeRemaining = this.powerdownTimeout;

                                // Equipment is now ready to use
                                this.slIsReady = true;
                            }

                            Logfile.Write(STRLOG_LabEquipmentIsReady);

                            // Check if powerdown is enabled
                            if (this.powerdownEnabled == false)
                            {
                                // Nothing more to do here
                                this.Running = false;
                            }
                            else
                            {
                                // Log the time remaining before power is removed
                                LogPowerDown(this.powerdownTimeout, true);

                                state = States.sPowerOnReady;
                            }
                            break;

                        case States.sPowerOnReady:

                            // Wait a bit
                            Thread.Sleep(1000);

                            // Check if equipment powerdown is suspended
                            if (this.PowerdownSuspended == true)
                            {
                                state = States.sPowerSuspended;
                                break;
                            }

                            //
                            // Log the time remaining before power is removed
                            //
                            int timeRemaining;
                            lock (this.statusLock)
                            {
                                timeRemaining = this.slPowerdownTimeRemaining;
                            }
                            LogPowerDown(timeRemaining);

                            lock (this.statusLock)
                            {
                                // Check timeout
                                if (--this.slPowerdownTimeRemaining > 0)
                                {
                                    // Timeout is still counting down
                                    Trace.Write("t");
                                    continue;
                                }

                                //
                                // Equipment is powering down, determine the time before the equipment
                                // can be powered up again
                                //
                                this.slPowerupTimeRemaining = this.PoweroffDelay + this.PowerupDelay;
                                this.slIsReady = false;
                            }

                            // Powerdown the equipment
                            PowerdownEquipment();

                            state = States.sPowerOffDelay;
                            break;

                        case States.sPowerSuspended:

                            // Wait a bit
                            Thread.Sleep(1000);

                            // Check if equipment powerdown is resumed
                            if (this.PowerdownSuspended == false)
                            {
                                lock (this.statusLock)
                                {
                                    // Reset the powerdown timeout
                                    this.slPowerdownTimeRemaining = this.powerdownTimeout;
                                }

                                // Log the time remaining before power is removed
                                LogPowerDown(this.powerdownTimeout, true);

                                state = States.sPowerOnReady;
                            }
                            break;

                        case States.sPowerOffDelay:

                            // Wait a bit
                            Thread.Sleep(1000);

                            // Check timeout
                            lock (this.statusLock)
                            {
                                if (--this.slPowerupTimeRemaining > this.powerupDelay)
                                {
                                    // Equipment is still powering off
                                    Trace.Write("o");
                                }
                                else
                                {
                                    // Check if powerup has been requested
                                    if (this.slPowerdownSuspended == true)
                                    {
                                        state = States.sPowerInit;
                                    }
                                    else
                                    {
                                        // Powerdown has completed
                                        this.slRunning = false;
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

        private void LogPowerDown(int seconds)
        {
            LogPowerDown(seconds, false);
        }

        //-------------------------------------------------------------------------------------------------//

        private void LogPowerDown(int seconds, bool now)
        {
            const string STR_PowerdownIn = " Powerdown in ";
            const string STR_Minute = " minute";
            const string STR_And = " and ";
            const string STR_Second = " second";

            int minutes = seconds / 60;
            if (now == true && seconds > 0)
            {
                // Log message now
                string message = STR_PowerdownIn;
                seconds %= 60;
                if (minutes > 0)
                {
                    message += minutes.ToString() + STR_Minute + ((minutes != 1) ? "s" : "");
                    if (seconds != 0)
                    {
                        message += STR_And;
                    }
                }
                if (seconds != 0)
                {
                    message += seconds.ToString() + STR_Second + ((seconds != 1) ? "s" : "");
                }
                Logfile.Write(message);
            }
            else
            {
                if (minutes > 5)
                {
                    if (seconds % (5 * 60) == 0)
                    {
                        // Log message every 5 minutes
                        Logfile.Write(STR_PowerdownIn + minutes.ToString() + STR_Minute + ((minutes != 1) ? "s" : ""));
                    }
                }
                else if (seconds > 5)
                {
                    if (seconds % 60 == 0 && seconds != 0)
                    {
                        // Log message every minute
                        Logfile.Write(STR_PowerdownIn + minutes.ToString() + STR_Minute + ((minutes != 1) ? "s" : ""));
                    }
                }
                else if (seconds > 0)
                {
                    // Log message every second
                    Logfile.Write(STR_PowerdownIn + seconds.ToString() + STR_Second + ((seconds != 1) ? "s" : ""));
                }
            }
        }

    }
}
