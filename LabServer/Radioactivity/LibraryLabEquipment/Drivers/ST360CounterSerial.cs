using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using Library.Lab;

namespace Library.LabEquipment.Drivers
{
    public class ST360Counter : IDisposable
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "ST360Counter";

        //
        // Value ranges and defaults
        //
        public const int MIN_HighVoltage = 0;
        public const int MAX_HighVoltage = 450;
        public const int DEFAULT_HighVoltage = 400;

        public const int MIN_SpeakerVolume = 0;
        public const int MAX_SpeakerVolume = 5;
        public const int DEFAULT_SpeakerVolume = 0;

        public const int MIN_PresetTime = 1;
        public const int MAX_PresetTime = 10;

        //
        // Delays are in millisecs
        //
        private const int DELAY_DISPLAY_MS = 1000;
        private const int DELAY_CAPTURE_DATA = DELAY_DISPLAY_MS / 1000;
        private const int DELAY_ISCOUNTING_MS = 1000;

        private const int MAX_RECEIVEDATA_WAITTIME_MS = 2000;


        //
        // Display selections
        //
        public enum Display
        {
            Counts, Time, Rate, HighVoltage, AlarmPoint, SpeakerVolume
        }

        //
        // String constants for logfile messages
        //
        private const string STRLOG_NotInitialised = " Not Initialised!";
        private const string STRLOG_Initialising = " Initialising...";
        private const string STRLOG_Online = " Online: ";
        private const string STRLOG_disposing = " disposing: ";
        private const string STRLOG_disposed = " disposed: ";
        private const string STRLOG_SerialPort = " SerialPort: ";
        private const string STRLOG_BaudRate = " BaudRate: ";
        private const string STRLOG_InterfaceMode = " InterfaceMode: ";
        private const string STRLOG_Selection = " Selection: ";
        private const string STRLOG_HighVoltage = " HighVoltage: ";
        private const string STRLOG_SpeakerVolume = " SpeakerVolume: ";
        private const string STRLOG_PresetTime = " PresetTime: ";
        private const string STRLOG_Duration = " Duration: ";
        private const string STRLOG_CaptureDataTime = " CaptureDataTime: ";
        private const string STRLOG_IsCounting = " Is Counting: ";
        private const string STRLOG_Counts = " Counts: ";
        private const string STRLOG_Success = " Success: ";
        private const string STRLOG_OpeningSerialPort = " Opening serial port...";
        private const string STRLOG_ReceiveHandlerThreadIsStarting = " ReceiveHandler thread is starting...";
        private const string STRLOG_ReceiveHandlerThreadIsRunning = " ReceiveHandler thread is running.";
        //
        // String constants for error messages
        //
        private const string STRERR_ReceiveHandlerThreadFailedToStart = "ReceiveHandler thread failed to start!";
        private const string STRERR_InvalidCommand = "Invalid command!";
        private const string STRERR_FailedToSetInterfaceMode = "Failed to set Interface Mode: ";
        private const string STRERR_FailedToSetDisplaySelection = "Failed to set Display Selection: ";
        private const string STRERR_FailedToSetHighVoltage = "Failed to set High Voltage: ";
        private const string STRERR_FailedToSetSpeakerVolume = "Failed to set Speaker Volume: ";
        private const string STRERR_FailedToSetPresetTime = "Failed to set Preset Time: ";
        private const string STRERR_FailedToPushDisplaySelectSwitch = "Failed to push Display Select switch";
        private const string STRERR_FailedToPushStartSwitch = "Failed to push Start switch";
        private const string STRERR_FailedToPushStopSwitch = "Failed to push Stop switch";
        private const string STRERR_FailedToPushResetSwitch = "Failed to push Reset switch";
        private const string STRERR_FailedToReadCountingStatus = "Failed to read Counting Status";
        private const string STRERR_FailedToReadCounts = "Failed to read Counts";
        private const string STRERR_CaptureTimedOut = "Capture timed out!";

        //
        // Single byte commands that return 5 bytes which is an echo of the command
        // followed by 4 data bytes
        //
        private const byte CMD_ReadCounts = 0x40;
        private const byte CMD_ReadPresetCounts = 0x41;
        private const byte CMD_ReadElapsedTime = 0x42;
        private const byte CMD_ReadPresetTime = 0x43;
        private const byte CMD_ReadCountsPerSec = 0x44;
        private const byte CMD_ReadCountsPerMin = 0x45;
        private const byte CMD_ReadHighVoltage = 0x46;
        private const byte CMD_ReadAlarmSetPoint = 0x47;
        private const byte CMD_ReadIsCounting = 0x48;
        private const byte CMD_ReadSpeakerVolume = 0x49;
        private const byte CMD_ReadDisplaySelection = 0x4A;
        private const byte CMD_ReadAnalyserInfo = 0x4B;

        private const int DATALEN_CMD_Read = 5;

        //
        // Single byte commands that return 1 byte which is an echo of the command
        //
        private const byte CMD_InterfaceNone = 0x50;
        private const byte CMD_InterfaceSerial = 0x51;
        private const byte CMD_InterfaceUsb = 0x52;

        public enum Commands
        {
            InterfaceNone = 0x50, InterfaceSerial = 0x51, InterfaceUsb = 0x52
        }

        private const int DATALEN_CMD_Interface = 1;

        //
        // Two byte commands that return 2 bytes which is an echo of the command.
        // Write and read the first byte before writing and reading the second byte
        //
        private const byte CMD_SetHighVoltage = 0x80;
        private const byte CMD_SetAlarmRate = 0x82;
        private const byte CMD_SetPresetTime = 0x83;
        private const byte CMD_SetSpeakerVolume = 0x84;
        private const byte CMD_SetCPMRateDisplay = 0x86;
        private const byte CMD_SetCPSRateDisplay = 0x87;

        private const int DATALEN_CMD_Set = 1;

        //
        // Single byte commands that return 1 byte which is an echo of the command
        //
        private const byte CMD_PushDisplaySelectSwitch = 0xDF;
        private const byte CMD_PushDownSwitch = 0xEF;
        private const byte CMD_PushUpSwitch = 0xF7;
        private const byte CMD_PushResetSwitch = 0xFB;
        private const byte CMD_PushStopSwitch = 0xFD;
        private const byte CMD_PushStartSwitch = 0xFE;

        private const int DATALEN_CMD_Push = 1;

        //
        // Local variables
        //
        private bool disposed;
        private bool initialised;
        private string lastError;
        private bool configured;
        private int geigerTubeVoltage;
        private int speakerVolume;
        private SerialPort serialPort;
        private Queue receiveQueue;
        private Object receiveSignal;
        private Thread receiveThread;
        private bool receiveRunning;

        #endregion

        #region Public Properties

        //
        // Minimum power-up and initialise delays in seconds
        //
        public const int DELAY_POWERUP = 5;
        public const int DELAY_INITIALISE = DELAY_DISPLAY_MS * 2 / 1000 + 2;

        private double adjustDuration;
        private bool online;
        private string statusMessage;

        /// <summary>
        /// Returns the time (in seconds) that it takes for the equipment to initialise.
        /// </summary>
        public int InitialiseDelay
        {
            get { return DELAY_INITIALISE; }
        }

        public double AdjustDuration
        {
            get { return this.adjustDuration; }
            set { this.adjustDuration = value; }
        }

        /// <summary>
        /// Returns true if the hardware has been initialised successfully and is ready for use.
        /// </summary>
        public bool Online
        {
            get { return this.online; }
        }

        public string StatusMessage
        {
            get { return this.statusMessage; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public ST360Counter(XmlNode xmlNodeEquipmentConfig)
        {
            const string STRLOG_MethodName = "ST360Counter";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Initialise local variables
            //
            this.disposed = true;
            this.initialised = false;
            this.lastError = null;
            this.configured = false;
            this.receiveRunning = false;

            //
            // Initialise properties
            //
            this.online = false;
            this.statusMessage = STRLOG_NotInitialised;

            //
            // Get the serial port to use and the baud rate
            //
            XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlNodeEquipmentConfig, Consts.STRXML_st360Counter);
            string serialport = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_port, false);
            int baudrate = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_baud);
            Logfile.Write(STRLOG_SerialPort + serialport);
            Logfile.Write(STRLOG_BaudRate + baudrate.ToString());

            //
            // Get Geiger tube voltage from application's configuration file
            //
            this.geigerTubeVoltage = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_voltage, DEFAULT_HighVoltage);

            //
            // Make sure that the high voltage is within range
            //
            if (this.geigerTubeVoltage < MIN_HighVoltage)
            {
                this.geigerTubeVoltage = MIN_HighVoltage;
            }
            else if (this.geigerTubeVoltage > MAX_HighVoltage)
            {
                this.geigerTubeVoltage = MAX_HighVoltage;
            }
            Logfile.Write(STRLOG_HighVoltage + this.geigerTubeVoltage.ToString());

            //
            // Get speaker volume from application's configuration file
            //
            this.speakerVolume = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_volume, MIN_SpeakerVolume);

            //
            // Make sure that the speaker volume is within range
            //
            if (this.speakerVolume < MIN_SpeakerVolume)
            {
                this.speakerVolume = MIN_SpeakerVolume;
            }
            else if (this.speakerVolume > MAX_SpeakerVolume)
            {
                this.speakerVolume = MAX_SpeakerVolume;
            }
            Logfile.Write(STRLOG_SpeakerVolume + this.speakerVolume.ToString());

            //
            // Create an instance of the serial port, set read and write timeouts
            //
            this.serialPort = new SerialPort(serialport, baudrate);
            this.serialPort.ReadTimeout = 1000;
            this.serialPort.WriteTimeout = 1000;

            //
            // Create the receive queue and receive signal object
            //
            this.receiveQueue = new Queue();
            this.receiveSignal = new Object();

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public string GetLastError()
        {
            string lastError = this.lastError;
            this.lastError = null;
            return lastError;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool Initialise()
        {
            return Initialise(true);
        }

        //-------------------------------------------------------------------------------------------------//

        public bool Initialise(bool configure)
        {
            const string STRLOG_MethodName = "Initialise";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            //
            // Check if this is first-time initialisation
            //
            if (this.initialised == false)
            {
                this.statusMessage = STRLOG_Initialising;

                //
                // Nothing to do here
                //

                //
                // First-time initialisation is complete
                //
                this.initialised = true;
            }

            //
            // Initialisation that must be done each time the equipment is powered up
            //
            try
            {
                this.disposed = false;

                //
                // Open the serial
                //
                Logfile.Write(STRLOG_OpeningSerialPort);
                this.serialPort.Open();

                //
                // Create and start the receive thread
                //
                Logfile.Write(STRLOG_ReceiveHandlerThreadIsStarting);
                this.receiveThread = new Thread(new ThreadStart(ReceiveHandler));
                this.receiveThread.Start();

                //
                // Give the thread a chance to start and then check that it has started
                //
                for (int i = 0; i < 5; i++)
                {
                    if ((success = this.receiveRunning) == true)
                    {
                        Logfile.Write(STRLOG_ReceiveHandlerThreadIsRunning);
                        break;
                    }

                    Thread.Sleep(500);
                    Trace.Write('?');
                }
                if (success == false)
                {
                    throw new ArgumentException(STRERR_ReceiveHandlerThreadFailedToStart);
                }

                //
                // Set interface to Serial mode, retry if necessary
                //
                for (int i = 0; i < 5; i++)
                {
                    if ((success = this.SetInterfaceMode(Commands.InterfaceSerial)) == true)
                    {
                        break;
                    }

                    Thread.Sleep(500);
                    Trace.Write('?');
                }
                if (success == false)
                {
                    throw new Exception(this.GetLastError());
                }

                //
                // Check if full configuration is required, always will be unless developing/debugging
                //
                if (configure == true)
                {
                    //
                    // Display the high voltage and set it
                    //
                    if (this.SetDisplay(ST360Counter.Display.HighVoltage) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }
                    if (this.SetHighVoltage(this.geigerTubeVoltage) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }
                    Thread.Sleep(DELAY_DISPLAY_MS);

                    //
                    // Display the speaker volume and set it
                    //
                    if (this.SetDisplay(ST360Counter.Display.SpeakerVolume) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }
                    if (this.SetSpeakerVolume(this.speakerVolume) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }
                    Thread.Sleep(DELAY_DISPLAY_MS);

                    //
                    // Set display to counts and clear time and counts
                    //
                    if (this.SetDisplay(Display.Counts) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }
                    if (this.PushResetSwitch() == false)
                    {
                        throw new Exception(this.GetLastError());
                    }

                    //
                    // Configuration is complete
                    //
                    this.configured = true;
                }

                //
                // Initialisation is complete
                //
                this.online = true;
                this.statusMessage = StatusCodes.Ready.ToString();

                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                this.Close();
            }

            string logMessage = STRLOG_Online + this.online.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return this.online;
        }

        //-------------------------------------------------------------------------------------------------//

        public double GetCaptureDataTime(int duration)
        {
            double seconds = 0;

            seconds = duration + (double)(DELAY_DISPLAY_MS * 2) / 1000 + this.adjustDuration;

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetInterfaceMode(Commands command)
        {
            const string STRLOG_MethodName = "SetInterfaceMode";

            string logMessage = STRLOG_InterfaceMode + command.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = true;

            try
            {
                //
                // Check for valid interface command
                //
                if (command != Commands.InterfaceNone &&
                    command != Commands.InterfaceSerial)
                {
                    throw new ArgumentException(STRERR_InvalidCommand, command.ToString());
                }

                //
                // Write the command and get the received data, should be the command echoed back
                //
                byte[] readData = WriteReadData(new byte[] { (byte)command }, 1, DATALEN_CMD_Interface);
                if (readData == null || readData.Length != DATALEN_CMD_Interface || readData[0] != (byte)command)
                {
                    throw new Exception(STRERR_FailedToSetInterfaceMode + command.ToString());
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool CaptureData(int duration, int[] counts)
        {
            const string STRLOG_MethodName = "CaptureData";

            string logMessage = STRLOG_Duration + duration.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = true;

            try
            {
                //
                // Use timeout with retries
                //
                int retries = 3;
                for (int i = 0; i < retries; i++)
                {
                    //
                    // Set the duration
                    //
                    if (this.SetPresetTime(duration) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }

                    //
                    // Reset the time and count before starting the counter
                    //
                    if (this.PushResetSwitch() == false)
                    {
                        throw new Exception(this.GetLastError());
                    }

                    //
                    // Set the display to time so that we can see the progress
                    //
                    if (this.SetDisplay(ST360Counter.Display.Time) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }

                    //
                    // Start the counter
                    //
                    if (this.StartCounter() == false)
                    {
                        throw new Exception(this.GetLastError());
                    }

                    //
                    // Set a timeout so that we don't wait forever if something goes wrong
                    //
                    int timeout = duration + 5;
                    while (--timeout > 0)
                    {
                        //
                        // Wait a bit and then check if still counting
                        //
                        Thread.Sleep(DELAY_ISCOUNTING_MS);

                        bool[] isCounting = new bool[1];
                        if (this.IsCounting(isCounting) == false)
                        {
                            throw new Exception(this.GetLastError());
                        }
                        if (isCounting[0] == false)
                        {
                            break;
                        }
                    }
                    if (timeout == 0)
                    {
                        Logfile.WriteError(STRERR_CaptureTimedOut);

                        //
                        // Stop the counter
                        //
                        if (this.StopCounter() == false)
                        {
                            throw new Exception(this.GetLastError());
                        }

                        //
                        // Retry
                        //
                        continue;
                    }

                    //
                    // Wait a moment before changing the display
                    //
                    Thread.Sleep(DELAY_DISPLAY_MS);

                    //
                    // Set the display to counts so that we can see how many counts there were
                    //
                    if (this.SetDisplay(ST360Counter.Display.Counts) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }
                    Thread.Sleep(DELAY_DISPLAY_MS);

                    //
                    // Get the counts and check for error
                    //
                    if (this.GetCounts(counts) == false)
                    {
                        throw new Exception(this.GetLastError());
                    }

                    //
                    // Data captured successfully
                    //
                    break;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_Counts + counts[0].ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetDisplay(Display selection)
        {
            const string STRLOG_MethodName = "SetDisplay";

            string logMessage = STRLOG_Selection + selection.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = true;

            try
            {
                while (true)
                {
                    //
                    // Get the current display selection
                    //
                    byte[] readData = WriteReadData(new byte[] { CMD_ReadDisplaySelection }, 1, DATALEN_CMD_Read);
                    if (readData == null || readData.Length != DATALEN_CMD_Read || readData[0] != CMD_ReadDisplaySelection)
                    {
                        throw new Exception(STRERR_FailedToSetDisplaySelection + selection.ToString());
                    }
                    Display currentSelection = (Display)readData[DATALEN_CMD_Read - 1];

                    //
                    // Check if this is the desired selection
                    //
                    if (currentSelection == selection)
                    {
                        break;
                    }

                    //
                    // Move the display selection down by one
                    //
                    readData = WriteReadData(new byte[] { CMD_PushDisplaySelectSwitch }, 1, DATALEN_CMD_Push);
                    if (readData == null || readData.Length != DATALEN_CMD_Push || readData[0] != CMD_PushDisplaySelectSwitch)
                    {
                        throw new Exception(STRERR_FailedToPushDisplaySelectSwitch);
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetHighVoltage(int highVoltage)
        {
            const string STRLOG_MethodName = "SetHighVoltage";

            string logMessage = STRLOG_HighVoltage + highVoltage.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = true;

            try
            {
                //
                // Make sure high voltage is within range
                //
                if (highVoltage < MIN_HighVoltage && highVoltage > MAX_HighVoltage)
                {
                    throw new ArgumentOutOfRangeException("SetHighVoltage", "Not in range");
                }

                //
                // Determine value to write for desired high voltage 
                //
                byte highVoltageValue = (byte)(highVoltage / 5);

                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_SetHighVoltage }, 1, DATALEN_CMD_Set);
                if (readData == null || readData.Length != DATALEN_CMD_Set || readData[0] != CMD_SetHighVoltage)
                {
                    throw new Exception(STRERR_FailedToSetHighVoltage + highVoltage.ToString());
                }

                //
                // Write the high voltage value
                //
                readData = WriteReadData(new byte[] { highVoltageValue }, 1, DATALEN_CMD_Set);
                if (readData == null || readData.Length != DATALEN_CMD_Set || readData[0] != highVoltageValue)
                {
                    throw new Exception(STRERR_FailedToSetHighVoltage + highVoltage.ToString());
                }

                //
                // Read the high voltage back
                //
                readData = WriteReadData(new byte[] { CMD_ReadHighVoltage }, 1, DATALEN_CMD_Read);
                if (readData == null || readData.Length != DATALEN_CMD_Read || readData[0] != CMD_ReadHighVoltage)
                {
                    throw new Exception(STRERR_FailedToSetHighVoltage + highVoltage.ToString());
                }

                //
                // Extract high voltage value from byte array and compare
                //
                int readHighVoltage = 0;
                for (int i = 1; i < DATALEN_CMD_Read; i++)
                {
                    readHighVoltage = readHighVoltage * 256 + (int)readData[i];
                }
                if (readHighVoltage != highVoltage)
                {
                    throw new Exception(STRERR_FailedToSetHighVoltage + highVoltage.ToString());
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetSpeakerVolume(int speakerVolume)
        {
            const string STRLOG_MethodName = "SetSpeakerVolume";

            string logMessage = STRLOG_SpeakerVolume + speakerVolume.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = true;

            try
            {
                //
                //
                // Make sure speaker volume is within range
                //
                if (speakerVolume < MIN_SpeakerVolume && speakerVolume > MAX_SpeakerVolume)
                {
                    throw new ArgumentOutOfRangeException("SetSpeakerVolume", "Not in range");
                }

                //
                // Determine value to write for desired speaker volume
                //
                byte speakerVolumeValue = (byte)speakerVolume;

                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_SetSpeakerVolume }, 1, DATALEN_CMD_Set);
                if (readData == null || readData.Length != DATALEN_CMD_Set || readData[0] != CMD_SetSpeakerVolume)
                {
                    throw new Exception(STRERR_FailedToSetSpeakerVolume + speakerVolumeValue.ToString());
                }

                //
                // Write the speaker volume value
                //
                readData = WriteReadData(new byte[] { speakerVolumeValue }, 1, DATALEN_CMD_Set);
                if (readData == null || readData.Length != DATALEN_CMD_Set || readData[0] != speakerVolumeValue)
                {
                    throw new Exception(STRERR_FailedToSetSpeakerVolume + speakerVolumeValue.ToString());
                }

                //
                // Read the speaker volume back
                //
                readData = WriteReadData(new byte[] { CMD_ReadSpeakerVolume }, 1, DATALEN_CMD_Read);
                if (readData == null || readData.Length != DATALEN_CMD_Read || readData[0] != CMD_ReadSpeakerVolume ||
                    readData[DATALEN_CMD_Read - 1] != speakerVolumeValue)
                {
                    throw new Exception(STRERR_FailedToSetSpeakerVolume + speakerVolumeValue.ToString());
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetPresetTime(int seconds)
        {
            const string STRLOG_MethodName = "SetPresetTime";

            string logMessage = STRLOG_PresetTime + seconds.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = true;

            try
            {
                //
                // Make sure preset time is within range
                //
                if (seconds < MIN_PresetTime || seconds > MAX_PresetTime)
                {
                    throw new ArgumentOutOfRangeException("SetPresetTime", "Not in range");
                }

                //
                // Determine value to write for desired preset time
                //
                byte secondsValue = (byte)seconds;

                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_SetPresetTime }, 1, DATALEN_CMD_Set);
                if (readData == null || readData.Length != DATALEN_CMD_Set || readData[0] != CMD_SetPresetTime)
                {
                    throw new Exception(STRERR_FailedToSetPresetTime + secondsValue.ToString());
                }

                //
                // Write the preset time value
                //
                readData = WriteReadData(new byte[] { secondsValue }, 1, DATALEN_CMD_Set);
                if (readData == null || readData.Length != DATALEN_CMD_Set || readData[0] != secondsValue)
                {
                    throw new Exception(STRERR_FailedToSetPresetTime + secondsValue.ToString());
                }

                //
                // Read the preset time back
                //
                readData = WriteReadData(new byte[] { CMD_ReadPresetTime }, 1, DATALEN_CMD_Read);
                if (readData == null || readData.Length != DATALEN_CMD_Read || readData[0] != CMD_ReadPresetTime ||
                    readData[DATALEN_CMD_Read - 1] != secondsValue)
                {
                    throw new Exception(STRERR_FailedToSetPresetTime + secondsValue.ToString());
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StartCounter()
        {
            const string STRLOG_MethodName = "StartCounter";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            try
            {
                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_PushStartSwitch }, 1, DATALEN_CMD_Push);
                if (readData == null || readData.Length != DATALEN_CMD_Push || readData[0] != CMD_PushStartSwitch)
                {
                    throw new Exception(STRERR_FailedToPushStartSwitch);
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StopCounter()
        {
            const string STRLOG_MethodName = "StopCounter";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            try
            {
                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_PushStopSwitch }, 1, DATALEN_CMD_Push);
                if (readData == null || readData.Length != DATALEN_CMD_Push || readData[0] != CMD_PushStopSwitch)
                {
                    throw new Exception(STRERR_FailedToPushStopSwitch);
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool IsCounting(bool[] isCounting)
        {
            //const string STRLOG_MethodName = "IsCounting";

            //Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            try
            {
                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_ReadIsCounting }, 1, DATALEN_CMD_Read);
                if (readData == null || readData.Length != DATALEN_CMD_Read || readData[0] != CMD_ReadIsCounting)
                {
                    throw new Exception(STRERR_FailedToReadCountingStatus);
                }

                isCounting[0] = (readData[DATALEN_CMD_Read - 1] != 0);
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            //string logMessage = STRLOG_Success + success.ToString() +
            //    Logfile.STRLOG_Spacer + STRLOG_IsCounting + isCounting[0].ToString();

            //Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool GetCounts(int[] counts)
        {
            const string STRLOG_MethodName = "GetCounts";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            try
            {
                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_ReadCounts }, 1, DATALEN_CMD_Read);
                if (readData == null || readData.Length != DATALEN_CMD_Read || readData[0] != CMD_ReadCounts)
                {
                    throw new Exception(STRERR_FailedToReadCounts);
                }

                //
                // Extract count value from byte array
                //
                for (int i = 1; i < DATALEN_CMD_Read; i++)
                {
                    counts[0] = counts[0] * 256 + (int)readData[i];
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            string logMessage = counts.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool PushResetSwitch()
        {
            const string STRLOG_MethodName = "PushResetSwitch";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            try
            {
                //
                // Write the command
                //
                byte[] readData = WriteReadData(new byte[] { CMD_PushResetSwitch }, 1, DATALEN_CMD_Push);
                if (readData == null || readData.Length != DATALEN_CMD_Push || readData[0] != CMD_PushResetSwitch)
                {
                    throw new Exception(STRERR_FailedToPushResetSwitch);
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        private byte[] WriteReadData(byte[] writeData, int writeCount, int readCount)
        {
            byte[] receivedData = null;
            int receivedCount = 0;

            try
            {
                lock (this.receiveSignal)
                {
                    this.serialPort.Write(writeData, 0, writeCount);

                    //
                    // Create storage for received data
                    //
                    while (receivedCount < readCount)
                    {
                        //
                        // Wait for received data
                        //
                        if (Monitor.Wait(receiveSignal, MAX_RECEIVEDATA_WAITTIME_MS))
                        {
                            //
                            // Check for sufficient received data
                            //
                            lock (this.receiveQueue)
                            {
                                receivedCount = this.receiveQueue.Count;
                                //Trace.WriteLine(" WriteReadData: receiveQueue.Count = " + receivedCount.ToString());
                            }
                        }
                        else
                        {
                            // Timeout waiting for received data
                            break;
                        }
                    }

                    //
                    // Check if sufficient data has been received
                    //
                    if (receivedCount >= readCount)
                    {
                        //
                        // Get the data from the receive queue
                        //
                        receivedData = new byte[readCount];
                        lock (this.receiveQueue)
                        {
                            for (int i = 0; i < readCount; i++)
                            {
                                receivedData[i] = (byte)this.receiveQueue.Dequeue();
                            }

                            //
                            // Clear the queue for the next transaction
                            //
                            if (this.receiveQueue.Count > 0)
                            {
                                this.receiveQueue.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return receivedData;
        }

        //-------------------------------------------------------------------------------------------------//

        private void ReceiveHandler()
        {
            try
            {
                byte[] receiveBuffer = new byte[8];
                int bytesRead;

                this.receiveRunning = true;
                while (this.receiveRunning == true)
                {
                    try
                    {
                        //
                        // Get some received data
                        //
                        bytesRead = this.serialPort.Read(receiveBuffer, 0, receiveBuffer.Length);
                        //Trace.Write("ReceiveHandler: bytesRead=" + bytesRead.ToString());

                        lock (this.receiveSignal)
                        {
                            //
                            // Some data has been received, save it
                            //
                            for (int i = 0; i < bytesRead; i++)
                            {
                                this.receiveQueue.Enqueue(receiveBuffer[i]);
                                //Trace.Write(" 0x" + receiveBuffer[i].ToString("X2"));
                            }

                            //
                            // Tell someone that some data has been received
                            //
                            Monitor.Pulse(receiveSignal);
                        }
                        //Trace.WriteLine("");
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }
            catch (IOException)
            {
                this.receiveRunning = false;
                Thread.CurrentThread.Abort();
            }
            catch (ObjectDisposedException)
            {
                this.receiveRunning = false;
                if (receiveThread != null)
                {
                    receiveThread = null;
                }
            }
            Trace.WriteLine("ReceiveHandler(): Exiting");
        }

        //-------------------------------------------------------------------------------------------------//

        #region Close and Dispose

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
            Dispose(true);

            // Take yourself off the Finalization queue to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Use C# destructor syntax for finalization code. This destructor will run only if the Dispose
        /// method does not get called. It gives your base class the opportunity to finalize. Do not provide
        /// destructors in types derived from this class.
        /// </summary>
        ~ST360Counter()
        {
            Trace.WriteLine("~ST360Counter():");

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

            string logMessage = STRLOG_disposing + disposing.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_disposed + this.disposed.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

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

                if (this.configured == true)
                {
                    //
                    // Reset everything
                    //
                    bool[] isCounting = new bool[1];
                    if (this.IsCounting(isCounting) == false)
                    {
                        Logfile.WriteError(this.GetLastError());
                    }
                    else
                    {
                        if (isCounting[0] == true)
                        {
                            if (this.StopCounter() == false)
                            {
                                Logfile.WriteError(this.GetLastError());
                            }
                        }
                    }
                    if (this.SetHighVoltage(0) == false)
                    {
                        Logfile.WriteError(this.GetLastError());
                    }
                    if (this.SetSpeakerVolume(MIN_SpeakerVolume) == false)
                    {
                        Logfile.WriteError(this.GetLastError());
                    }
                    if (this.SetDisplay(Display.Counts) == false)
                    {
                        Logfile.WriteError(this.GetLastError());
                    }
                    if (this.PushResetSwitch() == false)
                    {
                        Logfile.WriteError(this.GetLastError());
                    }
                }

                if (this.initialised == true)
                {
                    //
                    // Set interface type back to none
                    //
                    if (this.SetInterfaceMode(Commands.InterfaceNone) == false)
                    {
                        Logfile.WriteError(this.GetLastError());
                    }
                }

                //
                // Stop the receive thread and close the serial port
                //
                if (this.receiveRunning == true)
                {
                    this.receiveRunning = false;
                    this.receiveThread.Join();
                }
                if (this.serialPort != null && this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }

                this.disposed = true;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        #endregion

    }
}
