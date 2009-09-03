using System;
using System.Net.Sockets;
using Library.Lab;
using Modbus.Device;

namespace Library.LabServer.Drivers.Equipment
{
    public class RedLion
    {
        public struct ACDriveConfiguration
        {
            public int speed;          // RPM
            public int torque;         // Percent
            public int speedRampTime;  // Seconds
            public int maxCurrent;     // Milliamps
            public int maxTorque;      // Percent
            public int minTorque;      // Percent

            public ACDriveConfiguration(int speed, int torque, int speedRampTime, int maxCurrent, int maxTorque, int minTorque)
            {
                this.speed = speed;
                this.torque = torque;
                this.speedRampTime = speedRampTime;
                this.maxCurrent = maxCurrent;
                this.maxTorque = maxTorque;
                this.minTorque = minTorque;
            }
        }

        public struct Measurements
        {
            public float voltage;
            public float current;
            public float powerFactor;
            public int speed;
            public int torque;
        }

        //
        // Start AC Drive modes
        //
        public enum ACDriveModes
        {
            NoLoad, FullLoad, LockedRotor, SynchronousSpeed
        }

        #region Class Constants and Variables

        private const string STRLOG_ClassName = "RedLion";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_UnitId = " UnitId: ";
        private const string STRLOG_MachineIP = " MachineIP: ";
        private const string STRLOG_MachinePort = " MachinePort: ";
        private const string STRLOG_MeasurementCount = " MeasurementCount: ";
        private const string STRLOG_MeasurementDelay = " MeasurementDelay: ";
        private const string STRLOG_Result = " Result: ";
        private const string STRLOG_Ok = "Ok";
        private const string STRLOG_Failed = "Failed";
        private const string STRLOG_FaultCode = " FaultCode: ";

        //
        // String constants for error messages
        //
        private const string STRERR_MachineIPNotSpecifiedForUnit = "Machine IP not specified for unit #";
        private const string STRERR_NumberIsNegative = "Number is negative!";
        private const string STRERR_NumberIsInvalid = "Number is invalid!";
        private const string STRERR_FailedToResetACDrive = "Failed to reset AC drive!";

        //
        // Local variables
        //
        private string machineIP;
        private int machinePort;
        private TcpClient tcpClient;
        private ModbusIpMaster master;
        private ACDrive acDrive;
        private PowerMeter powerMeter;

        #endregion

        #region Properties

        private int measurementCount;
        private int measurementDelay;
        private string lastError;

        public int MeasurementCount
        {
            get { return this.measurementCount; }
        }

        public int MeasurementDelay
        {
            get { return this.measurementDelay; }
        }

        public string LastError
        {
            get { return this.lastError; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public RedLion(int unitId)
        {
            const string STRLOG_MethodName = "RedLion";

            string logMessage = STRLOG_UnitId + unitId.ToString();

            Logfile.WriteCalled(null, STRLOG_MethodName, logMessage);

            this.lastError = null;

            try
            {
                //
                // Get the IP address of the RedLion unit
                //
                string strMachineIP = Utilities.GetAppSetting(Consts.STRCFG_MachineIP);
                string[] strSplit = strMachineIP.Split(new char[] { Library.LabServerEngine.Consts.CHRCFG_SplitterChr });
                if (strSplit.Length < unitId + 1)
                {
                    throw new ArgumentException(STRERR_MachineIPNotSpecifiedForUnit + unitId.ToString(), Consts.STRCFG_MachineIP);
                }
                this.machineIP = strSplit[unitId].Trim();
                Logfile.Write(STRLOG_MachineIP + this.machineIP);

                //
                // Get the port number to use with the RedLion unit
                //
                this.machinePort = Utilities.GetIntAppSetting(Consts.STRCFG_MachinePort);
                if (this.machinePort < 0)
                {
                    throw new Exception(STRERR_NumberIsNegative);
                }
                Logfile.Write(STRLOG_MachinePort + this.machinePort.ToString());

                //
                // Get the number of measurements to take
                //
                this.measurementCount = Utilities.GetIntAppSetting(Consts.STRCFG_MeasurementCount);
                if (this.measurementCount < 1)
                {
                    throw new Exception(STRERR_NumberIsInvalid);
                }
                Logfile.Write(STRLOG_MeasurementCount + this.measurementCount.ToString());

                //
                // Get the delay in seconds between each measurement
                //
                this.measurementDelay = Utilities.GetIntAppSetting(Consts.STRCFG_MeasurementDelay);
                if (this.measurementDelay < 1)
                {
                    throw new Exception(STRERR_NumberIsInvalid);
                }
                Logfile.Write(STRLOG_MeasurementDelay + this.measurementDelay.ToString());

            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw;
            }

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public bool CreateConnection()
        {
            const string STRLOG_MethodName = "CreateConnection";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                this.tcpClient = new TcpClient(this.machineIP, this.machinePort);
                this.master = ModbusIpMaster.CreateTcp(this.tcpClient);
                this.acDrive = new ACDrive(this.master);
                this.powerMeter = new PowerMeter(this.master);
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public void CloseConnection()
        {
            if (this.tcpClient != null)
            {
                NetworkStream networkStream = this.tcpClient.GetStream();
                if (networkStream != null)
                {
                    networkStream.Close();
                }
                this.tcpClient.Close();
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public bool ResetACDrive()
        {
            const string STRLOG_MethodName = "ResetACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Reset AC drive
                //
                acDrive.EnableDrivePower();
                int faultCode = acDrive.ReadActiveFault();
                acDrive.ResetDrive();
                faultCode = acDrive.ReadActiveFault();
                if (faultCode != 0)
                {
                    throw new Exception(STRERR_FailedToResetACDrive + STRLOG_FaultCode + faultCode.ToString());
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ResetACDriveExecutionTime()
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_EnableDrivePower;
            executionTime += ACDrive.DELAY_ResetDrive;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool ConfigureACDrive(ACDriveConfiguration config)
        {
            const string STRLOG_MethodName = "ConfigureACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Configure AC drive, set non-zero speed last
                //
                if (config.speed == 0)
                {
                    acDrive.SetSpeed(0);
                }
                acDrive.SetTorque(config.torque);
                acDrive.SetSpeedRampTime(config.speedRampTime);
                acDrive.SetMaximumCurrent(config.maxCurrent);
                acDrive.SetMaximumTorque(config.maxTorque);
                acDrive.SetMinimumTorque(config.minTorque);
                if (config.speed != 0)
                {
                    acDrive.SetSpeed(config.speed);
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ConfigureACDriveExecutionTime()
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

        public bool StartACDrive(ACDriveModes mode)
        {
            const string STRLOG_MethodName = "StartACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Start AC drive
                //
                if (mode == ACDriveModes.NoLoad)
                {
                    acDrive.StartDriveNoLoad();
                }
                else if (mode == ACDriveModes.FullLoad)
                {
                    acDrive.StartDriveNoLoad();
                    acDrive.StartDriveFullLoad();
                }
                else if (mode == ACDriveModes.LockedRotor)
                {
                    acDrive.StartDriveLockedRotor();
                }
                else if (mode == ACDriveModes.SynchronousSpeed)
                {
                    acDrive.StartDriveSyncSpeed();
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int StartACDriveExecutionTime(ACDriveModes mode)
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_StartDrive;
            if (mode == ACDriveModes.FullLoad)
            {
                executionTime += ACDrive.DELAY_StartDrive;
            }

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StopACDrive(ACDriveModes mode)
        {
            const string STRLOG_MethodName = "StopACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Stop AC drive
                //
                if (mode == ACDriveModes.FullLoad)
                {
                    acDrive.StopDriveFullLoad();
                }
                acDrive.StopDriveOther();
                acDrive.DisableDrivePower();
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int StopACDriveExecutionTime(ACDriveModes mode)
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_StopDrive;
            if (mode == ACDriveModes.FullLoad)
            {
                executionTime += ACDrive.DELAY_StopDrive;
            }
            executionTime += ACDrive.DELAY_DisableDrivePower;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool TakeMeasurement(int delay, ref Measurements measurement)
        {
            const string STRLOG_MethodName = "TakeMeasurement";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                // Wait before taking the measurement
                acDrive.WaitDelay(delay);

                //
                // Take the measurement
                //
                measurement.voltage = this.powerMeter.ReadVoltagePhaseToPhase();
                measurement.current = this.powerMeter.ReadCurrentPhase();
                measurement.powerFactor = this.powerMeter.ReadPowerFactorAverage();
                measurement.speed = this.acDrive.ReadDriveSpeed();
                measurement.torque = this.acDrive.ReadDriveTorque();
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

    }
}
