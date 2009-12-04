using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Library.Lab;
using Modbus.Device;

namespace Library.LabEquipment.Drivers
{
    public class RedLion
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "RedLion";

        //
        // Constants
        //
        public const int DELAY_POWERUP = 0;
        public const int DELAY_INITIALISE = 0;
        public const int DELAY_MEASUREMENT = 5;

        //
        // String constants for logfile messages
        //
        private const string STRLOG_NotInitialised = "Not Initialised!";
        private const string STRLOG_Initialising = "Initialising...";
        private const string STRLOG_MachineIP = " MachineIP: ";
        private const string STRLOG_MachinePort = " MachinePort: ";
        private const string STRLOG_Online = " Online: ";
        private const string STRLOG_Result = " Result: ";
        private const string STRLOG_Success = " Success: ";
        private const string STRLOG_FaultCode = " FaultCode: ";

        private const string STRLOG_ACDriveMode = "ACDriveMode: ";

        //
        // String constants for error messages
        //
        private const string STRERR_MachineIPNotSpecified = "Machine IP not specified!";
        private const string STRERR_NumberIsNegative = "Number is negative!";
        private const string STRERR_NumberIsInvalid = "Number is invalid!";
        private const string STRERR_FailedToInitialise = "Failed to initialise!";
        private const string STRERR_FailedToResetACDrive = "Failed to reset AC drive!";

        //
        // Local variables
        //
        private bool initialised;
        private string lastError;
        private string machineIP;
        private int machinePort;
        private TcpClient tcpClient;
        private ModbusIpMaster master;
        private ACDrive acDrive;
        private PowerMeter powerMeter;

        #endregion

        #region Properties

        private bool online;
        private string statusMessage;
        private bool initialiseEquipment;
        private int measurementDelay;

        /// <summary>
        /// Returns the time (in seconds) that it takes for the equipment to initialise.
        /// </summary>
        public int InitialiseDelay
        {
            get { return (this.initialised == false) ? this.GetInitialiseTime() : 0; }
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

        public bool InitialiseEquipment
        {
            get { return this.initialiseEquipment; }
            set { this.initialiseEquipment = value; }
        }

        public int MeasurementDelay
        {
            get { return this.measurementDelay; }
            set { this.measurementDelay = value; }
        }

        #endregion

        //
        // Start AC Drive modes
        //
        public enum ACDriveModes
        {
            NoLoad, FullLoad, LockedRotor, SynchronousSpeed
        }

        //
        // Measurements to take
        //
        public struct Measurements
        {
            public float voltage;
            public float current;
            public float powerFactor;
            public int speed;
            public int torque;
        }

        //-------------------------------------------------------------------------------------------------//

        public RedLion(XmlNode xmlNodeEquipmentConfig)
        {
            const string STRLOG_MethodName = "RedLion";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Initialise local variables
            //
            this.initialised = false;
            this.lastError = string.Empty;

            //
            // Initialise properties
            //
            this.online = false;
            this.statusMessage = STRLOG_NotInitialised;
            this.initialiseEquipment = true;
            this.measurementDelay = DELAY_MEASUREMENT;

            //
            // Get the IP address of the RedLion unit
            //
            this.machineIP = XmlUtilities.GetXmlValue(xmlNodeEquipmentConfig, Consts.STRXML_machineIP, false);
            IPAddress machineIP = IPAddress.Parse(this.machineIP);
            Logfile.Write(STRLOG_MachineIP + this.machineIP.ToString());

            //
            // Get the port number to use with the RedLion unit
            //
            this.machinePort = XmlUtilities.GetIntValue(xmlNodeEquipmentConfig, Consts.STRXML_machinePort);
            if (this.machinePort < 0)
            {
                throw new Exception(STRERR_NumberIsNegative);
            }
            Logfile.Write(STRLOG_MachinePort + this.machinePort.ToString());

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
            const string STRLOG_MethodName = "Initialise";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            if (this.initialised == false)
            {
                this.statusMessage = STRLOG_Initialising;

                try
                {
                    if (this.initialiseEquipment == true)
                    {
                        //
                        // Create a connection to the RedLion controller
                        //
                        if (this.CreateConnection() == false)
                        {
                            throw new Exception(this.GetLastError());
                        }

                        try
                        {
                            //
                            // Reset the AC drive
                            //
                            if (this.ResetACDrive() == false)
                            {
                                throw new Exception(this.GetLastError());
                            }

                            //
                            // Configure the AC drive with default values
                            //
                            this.ConfigureACDrive(0, 0,
                                ACDrive.DEFAULT_SpeedRampTime, ACDrive.DEFAULT_MaximumCurrent,
                                ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);

                            //
                            // Disable drive power
                            //
                            acDrive.DisableDrivePower();
                        }
                        finally
                        {
                            this.CloseConnection();
                        }
                    }

                    //
                    // Initialisation is complete
                    //
                    this.initialised = true;
                    this.online = true;
                    this.statusMessage = StatusCodes.Ready.ToString();
                }
                catch (Exception ex)
                {
                    Logfile.WriteError(ex.Message);
                    this.statusMessage = STRERR_FailedToInitialise;
                    this.lastError = ex.Message;
                }
            }

            string logMessage = STRLOG_Online + this.online.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return this.online;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetResetACDriveTime()
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_EnableDrivePower;
            executionTime += ACDrive.DELAY_ResetDrive;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetStartACDriveTime(ACDriveModes acDriveMode)
        {
            int executionTime = 0;

            executionTime += this.GetConfigureACDriveTime();

            switch (acDriveMode)
            {
                case ACDriveModes.NoLoad:
                    executionTime += ACDrive.DELAY_StartDrive;
                    break;

                case ACDriveModes.FullLoad:
                    executionTime += ACDrive.DELAY_StartDriveFullLoad;
                    break;

                case ACDriveModes.LockedRotor:
                    executionTime += ACDrive.DELAY_StartDrive;
                    break;

                case ACDriveModes.SynchronousSpeed:
                    executionTime += ACDrive.DELAY_StartDrive;
                    break;
            }

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetStopACDriveTime(ACDriveModes acDriveMode)
        {
            int executionTime = 0;

            switch (acDriveMode)
            {
                case ACDriveModes.NoLoad:
                    executionTime += ACDrive.DELAY_StopDrive;
                    break;

                case ACDriveModes.FullLoad:
                    executionTime += ACDrive.DELAY_StopDriveFullLoad;
                    break;

                case ACDriveModes.LockedRotor:
                    executionTime += ACDrive.DELAY_StopDrive;
                    break;

                case ACDriveModes.SynchronousSpeed:
                    executionTime += ACDrive.DELAY_StopDrive;
                    break;
            }

            executionTime += this.GetConfigureACDriveTime();
            executionTime += ACDrive.DELAY_DisableDrivePower;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetTakeMeasurementTime()
        {
            int executionTime = 0;

            executionTime += this.measurementDelay;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool CreateConnection()
        {
            const string STRLOG_MethodName = "CreateConnection";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;
            try
            {
                this.tcpClient = new TcpClient(this.machineIP, this.machinePort);
                this.master = ModbusIpMaster.CreateTcp(this.tcpClient);
                this.acDrive = new ACDrive(this.master);
                this.powerMeter = new PowerMeter(this.master);

                success = true;
                this.lastError = null;
            }
            catch (Exception ex)
            {
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool CloseConnection()
        {
            const string STRLOG_MethodName = "CloseConnection";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            if (this.tcpClient != null)
            {
                try
                {
                    NetworkStream networkStream = this.tcpClient.GetStream();
                    if (networkStream != null)
                    {
                        networkStream.Close();
                    }
                    this.tcpClient.Close();

                    success = true;
                    this.lastError = null;
                }
                catch (Exception ex)
                {
                    this.lastError = ex.Message;
                    Logfile.WriteError(ex.Message);
                }
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool ResetACDrive()
        {
            const string STRLOG_MethodName = "ResetACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = false;

            try
            {
                //
                // Enable drive power and reset AC drive
                //
                acDrive.EnableDrivePower();
                int faultCode = acDrive.ReadActiveFault();
                acDrive.ResetDrive();
                faultCode = acDrive.ReadActiveFault();
                if (faultCode != 0)
                {
                    throw new Exception(STRERR_FailedToResetACDrive + STRLOG_FaultCode + faultCode.ToString());
                }

                success = true;
            }
            catch (Exception ex)
            {
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StartACDrive(ACDriveModes acDriveMode)
        {
            const string STRLOG_MethodName = "StartACDrive";

            string logMessage = STRLOG_ACDriveMode + acDriveMode.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            this.lastError = null;
            bool success = false;

            try
            {
                //
                // Configure and then start AC drive
                //
                switch (acDriveMode)
                {
                    case ACDriveModes.NoLoad:

                        this.ConfigureACDrive(ACDrive.MAXIMUM_Speed, 0,
                            ACDrive.DEFAULT_SpeedRampTime, ACDrive.MAXIMUM_MaximumCurrent,
                            ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);
                        acDrive.StartDriveNoLoad();
                        break;

                    case ACDriveModes.FullLoad:

                        this.ConfigureACDrive(0, 0,
                            ACDrive.DEFAULT_SpeedRampTime, ACDrive.MAXIMUM_MaximumCurrent,
                            ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);
                        acDrive.StartDriveFullLoad();
                        break;

                    case ACDriveModes.LockedRotor:

                        this.ConfigureACDrive(0, 0,
                            ACDrive.DEFAULT_SpeedRampTime, ACDrive.MAXIMUM_MaximumCurrent,
                            ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);
                        acDrive.StartDriveLockedRotor();
                        break;

                    case ACDriveModes.SynchronousSpeed:

                        this.ConfigureACDrive(ACDrive.MAXIMUM_Speed, 0,
                            ACDrive.DEFAULT_SpeedRampTime, ACDrive.MAXIMUM_MaximumCurrent,
                            ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);
                        acDrive.StartDriveSyncSpeed();
                        break;
                }

                success = true;
            }
            catch (Exception ex)
            {
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StopACDrive(ACDriveModes acDriveMode)
        {
            const string STRLOG_MethodName = "StopACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = false;

            try
            {
                //
                // Stop AC drive
                //
                switch (acDriveMode)
                {
                    case ACDriveModes.NoLoad:
                        acDrive.StopDriveNoLoad();
                        break;

                    case ACDriveModes.FullLoad:
                        acDrive.StopDriveFullLoad();
                        break;

                    case ACDriveModes.LockedRotor:
                        acDrive.StopDriveLockedRotor();
                        break;

                    case ACDriveModes.SynchronousSpeed:
                        acDrive.StopDriveSyncSpeed();
                        break;
                }

                //
                // Reconfigure the AC drive with default values
                //
                this.ConfigureACDrive(0, 0,
                    ACDrive.DEFAULT_SpeedRampTime, ACDrive.DEFAULT_MaximumCurrent,
                    ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);

                //
                // Disable drive power
                //
                acDrive.DisableDrivePower();

                success = true;
            }
            catch (Exception ex)
            {
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool TakeMeasurement(ref Measurements measurement)
        {
            const string STRLOG_MethodName = "TakeMeasurement";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = false;

            try
            {
                // Wait before taking the measurement
                acDrive.WaitDelay(this.measurementDelay);

                //
                // Take the measurement
                //
                measurement.voltage = this.powerMeter.ReadVoltagePhaseToPhase();
                measurement.current = this.powerMeter.ReadCurrentPhase();
                measurement.powerFactor = this.powerMeter.ReadPowerFactorAverage();
                measurement.speed = this.acDrive.ReadDriveSpeed();
                measurement.torque = this.acDrive.ReadDriveTorque();

                success = true;
            }
            catch (Exception ex)
            {
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //=================================================================================================//

        private int GetInitialiseTime()
        {
            int intialiseTime = 1;

            intialiseTime += this.GetResetACDriveTime();
            intialiseTime += this.GetConfigureACDriveTime();
            intialiseTime += ACDrive.DELAY_DisableDrivePower;

            return intialiseTime;
        }

        //-------------------------------------------------------------------------------------------------//

        private int GetConfigureACDriveTime()
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_SetSpeed;
            executionTime += ACDrive.DELAY_SetTorque;
            executionTime += ACDrive.DELAY_SetSpeedRampTime;
            executionTime += ACDrive.DELAY_SetMaximumCurrent;
            executionTime += ACDrive.DELAY_SetMaximumTorque;
            executionTime += ACDrive.DELAY_SetMinimumTorque;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        private void ConfigureACDrive(int speed, int torque, int speedRampTime, int maxCurrent, int maxTorque, int minTorque)
        {
            //
            // Set non-zero speed last
            //
            if (speed == 0)
            {
                acDrive.SetSpeed(0);
            }
            acDrive.SetTorque(torque);
            acDrive.SetSpeedRampTime(speedRampTime);
            acDrive.SetMaximumCurrent(maxCurrent);
            acDrive.SetMaximumTorque(maxTorque);
            acDrive.SetMinimumTorque(minTorque);
            if (speed != 0)
            {
                acDrive.SetSpeed(speed);
            }
        }

    }
}
