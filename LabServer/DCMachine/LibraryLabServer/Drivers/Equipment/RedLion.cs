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

        public struct DCDriveMutConfiguration
        {
            public int speed;          // RPM
            public int torque;         // Percent
            public int minSpeedLimit;  // RPM
            public int maxSpeedLimit;  // RPM
            public int minTorqueLimit; // Percent
            public int maxTorqueLimit; // Percent
            public int speedRampTime;  // Seconds
            public int field;          // Percent

            public DCDriveMutConfiguration(int speed, int torque, int minSpeedLimit, int maxSpeedLimit,
                int minTorqueLimit, int maxTorqueLimit, int speedRampTime, int field)
            {
                this.speed = speed;
                this.torque = torque;
                this.minSpeedLimit = minSpeedLimit;
                this.maxSpeedLimit = maxSpeedLimit;
                this.minTorqueLimit = minTorqueLimit;
                this.maxTorqueLimit = maxTorqueLimit;
                this.speedRampTime = speedRampTime;
                this.field = field;
            }
        }

        public struct Measurements
        {
            public int speed;          // RPM
            public int voltage;        // Volts
            public float fieldCurrent; // Amps
            public int load;         // Percent
        }

        //
        // Start DC Drive Mut modes
        //
        public enum StartDCDriveMutModes
        {
            EnableOnly, Speed, Torque
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
        private const string STRLOG_Speed = " Speed: ";
        private const string STRLOG_Voltage = " Voltage: ";
        private const string STRLOG_Torque = " Torque: ";
        private const string STRLOG_Field = " Field: ";
        private const string STRLOG_FieldCurrent = " FieldCurrent: ";
        private const string STRLOG_Rpm = " RPM";
        private const string STRLOG_Volts = " Volts";
        private const string STRLOG_Percent = " %";
        private const string STRLOG_Amps = " Amps";

        //
        // String constants for error messages
        //
        private const string STRERR_MachineIPNotSpecifiedForUnit = "Machine IP not specified for unit #";
        private const string STRERR_NumberIsNegative = "Number is negative!";
        private const string STRERR_NumberIsInvalid = "Number is invalid!";
        private const string STRERR_ActiveFaultDetectedACDrive = "Active fault detected on AC drive!";

        //
        // Local variables
        //
        private string machineIP;
        private int machinePort;
        private TcpClient tcpClient;
        private ModbusIpMaster master;
        private ACDrive acDrive;
        private DCDriveMut dcDriveMut;

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
                this.dcDriveMut = new DCDriveMut(this.master);
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

        public bool CheckActiveFaultACDrive()
        {
            const string STRLOG_MethodName = "CheckActiveFaultACDrive";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                int faultCode = acDrive.ReadActiveFault();

                if (faultCode != 0)
                {
                    throw new Exception(STRERR_ActiveFaultDetectedACDrive + STRLOG_FaultCode + faultCode.ToString());
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
                acDrive.ReadActiveFault();
                acDrive.ResetDrive();
                this.CheckActiveFaultACDrive();
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

        public bool ResetDCDriveMut()
        {
            const string STRLOG_MethodName = "ResetDCDriveMut";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Reset DC drive
                //
                dcDriveMut.ResetDriveFault();
                dcDriveMut.ResetDrive();
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

        public int ResetDCDriveMutExecutionTime()
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_ResetDriveFault;
            executionTime += DCDriveMut.DELAY_ResetDrive;

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

        public bool ConfigureDCDriveMut(DCDriveMutConfiguration config)
        {
            const string STRLOG_MethodName = "ConfigureDCDriveMut";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Configure DC drive, set non-zero speed last
                //
                if (config.speed == 0)
                {
                    dcDriveMut.SetSpeed(0);
                }
                dcDriveMut.SetTorque(config.torque);
                dcDriveMut.SetMinSpeedLimit(config.minSpeedLimit);
                dcDriveMut.SetMaxSpeedLimit(config.maxSpeedLimit);
                dcDriveMut.SetMinTorqueLimit(config.minTorqueLimit);
                dcDriveMut.SetMaxTorqueLimit(config.maxTorqueLimit);
                dcDriveMut.SetSpeedRampTime(config.speedRampTime);
                dcDriveMut.SetField(config.field);
                if (config.speed != 0)
                {
                    dcDriveMut.SetSpeed(config.speed);
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

        public int ConfigureDCDriveMutExecutionTime()
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_SetSpeed;
            executionTime += DCDriveMut.DELAY_SetTorque;
            executionTime += DCDriveMut.DELAY_SetMinSpeedLimit;
            executionTime += DCDriveMut.DELAY_SetMaxSpeedLimit;
            executionTime += DCDriveMut.DELAY_SetMinTorqueLimit;
            executionTime += DCDriveMut.DELAY_SetMaxTorqueLimit;
            executionTime += DCDriveMut.DELAY_SetSpeedRampTime;
            executionTime += DCDriveMut.DELAY_SetField;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StartACDrive()
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
                acDrive.StartDrive();
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

        public int StartACDriveExecutionTime()
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_StartDrive;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StartDCDriveMut(StartDCDriveMutModes mode)
        {
            const string STRLOG_MethodName = "StartDCDriveMut";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Start DC drive
                //
                dcDriveMut.SetMainContactorOn();
                if (mode == StartDCDriveMutModes.EnableOnly)
                {
                    // Don't start the drive
                }
                else if (mode == StartDCDriveMutModes.Speed)
                {
                    dcDriveMut.StartDrive();
                }
                else if (mode == StartDCDriveMutModes.Torque)
                {
                    dcDriveMut.StartDriveTorque();
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

        public int StartDCDriveMutExecutionTime(StartDCDriveMutModes mode)
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_SetMainContactorOn;
            if (mode == StartDCDriveMutModes.EnableOnly)
            {
                // Don't start the drive
            }
            else if (mode == StartDCDriveMutModes.Speed)
            {
                executionTime += DCDriveMut.DELAY_StartDrive;
            }
            else if (mode == StartDCDriveMutModes.Torque)
            {
                executionTime += DCDriveMut.DELAY_StartDriveTorque;
            }

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StopDCDriveMut()
        {
            const string STRLOG_MethodName = "StopDCDriveMut";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Stop DC drive
                //
                dcDriveMut.ResetDrive();
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

        public int StopDCDriveMutExecutionTime()
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_ResetDrive;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool StopACDrive()
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
                acDrive.StopDrive();
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

        public int StopACDriveExecutionTime()
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_StopDrive;
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
                dcDriveMut.WaitDelay(delay);

                //
                // Take the measurement
                //
                measurement.speed = dcDriveMut.ReadDriveSpeed();
                measurement.voltage = dcDriveMut.ReadArmatureVoltage();
                measurement.fieldCurrent = dcDriveMut.ReadFieldCurrent();
                measurement.load = dcDriveMut.ReadTorque();
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed) +
                Logfile.STRLOG_Spacer + STRLOG_Speed + measurement.speed.ToString() + STRLOG_Rpm +
                Logfile.STRLOG_Spacer + STRLOG_Voltage + measurement.voltage + STRLOG_Volts +
                Logfile.STRLOG_Spacer + STRLOG_FieldCurrent + measurement.fieldCurrent.ToString() + STRLOG_Amps;

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetSpeedACDrive(int speed)
        {
            const string STRLOG_MethodName = "SetSpeedACDrive";

            string logMessage = STRLOG_Speed + speed.ToString() + STRLOG_Rpm;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Set AC drive speed
                //
                acDrive.SetSpeed(speed);
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int SetSpeedACDriveExecutionTime()
        {
            int executionTime = 0;

            executionTime += ACDrive.DELAY_SetSpeed;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetTorqueDCDriveMut(int percent)
        {
            const string STRLOG_MethodName = "SetTorqueDCDriveMut";

            string logMessage = STRLOG_Torque + percent.ToString() + STRLOG_Percent;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Set DC drive torque
                //
                dcDriveMut.SetTorque(percent);
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int SetTorqueDCDriveMutExecutionTime()
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_SetTorque;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetSpeedDCDriveMut(int speed)
        {
            const string STRLOG_MethodName = "SetSpeedDCDriveMut";

            string logMessage = STRLOG_Speed + speed.ToString() + STRLOG_Rpm;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Set DC drive speed
                //
                dcDriveMut.SetSpeed(speed);
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int SetSpeedDCDriveMutExecutionTime()
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_SetSpeed;

            return executionTime;
        }

        //-------------------------------------------------------------------------------------------------//

        public bool SetFieldDCDriveMut(int percent)
        {
            const string STRLOG_MethodName = "SetFieldDCDriveMut";

            string logMessage = STRLOG_Field + percent.ToString() + STRLOG_Percent;

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            this.lastError = null;
            bool success = true;
            try
            {
                //
                // Set DC drive field
                //
                dcDriveMut.SetField(percent);
            }
            catch (Exception ex)
            {
                success = false;
                this.lastError = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            logMessage = STRLOG_Result + (success ? STRLOG_Ok : STRLOG_Failed);

            Logfile.WriteCompleted(null, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public int SetFieldDCDriveMutExecutionTime()
        {
            int executionTime = 0;

            executionTime += DCDriveMut.DELAY_SetField;

            return executionTime;
        }

    }
}
