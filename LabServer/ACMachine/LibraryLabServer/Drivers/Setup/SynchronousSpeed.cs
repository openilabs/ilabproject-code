using System;
using System.Diagnostics;
using System.Net.Sockets;
using Modbus.Device;


namespace LabServer
{
    class SynchronousSpeed
    {
        //
        // Constants
        //
        private const int DELAY_Measurement = 10;

        //
        // Local variables
        //
        private string ipAddress = null;
        private int portNum = 0;

        //
        // Properties
        //
        private string lastError = null;
        private float voltage;
        private float current;
        private float powerFactor;

        public string LastError
        {
            get
            {
                string errorMsg = lastError;
                lastError = null;
                return errorMsg;
            }
        }

        public float Voltage
        {
            get
            {
                return this.voltage;
            }
        }

        public float Current
        {
            get
            {
                return this.current;
            }
        }

        public float PowerFactor
        {
            get
            {
                return this.powerFactor;
            }
        }

        //-------------------------------------------------------------------------------------------------//

        //
        // Constructor
        //
        public SynchronousSpeed(string ipAddress, int portNum)
        {
            Trace.WriteLine("SynchronousSpeed(): Called");
            Trace.WriteLine("ipAddress=" + ipAddress + "; portNum=" + portNum.ToString());

            // Initialise local variables
            this.ipAddress = ipAddress;
            this.portNum = portNum;
        }

        //-------------------------------------------------------------------------------------------------//

        public void Execute()
        {
            Trace.WriteLine("SynchronousSpeed.Execute(): Called");

#if NO_HARDWARE
            // Empty
#else
            try
            {
                using (TcpClient client = new TcpClient(this.ipAddress, this.portNum))
                {
                    ModbusIpMaster master = ModbusIpMaster.CreateTcp(client);

                    ACDrive acDrive = new ACDrive(master);
                    PowerMeter powerMeter = new PowerMeter(master);

                    //
                    // Reset AC drive
                    //
                    acDrive.EnableDrivePower();
                    acDrive.ReadActiveFault();
                    acDrive.ResetDrive();
                    acDrive.ReadActiveFault();

                    //
                    // Configure AC drive
                    //
                    acDrive.SetSpeed(1500);
                    acDrive.SetTorque(0);
                    acDrive.SetSpeedRampTime(ACDrive.DEFAULT_SpeedRampTime);
                    acDrive.SetMaximumCurrent(10000);
                    acDrive.SetMaximumTorque(ACDrive.DEFAULT_MaximumTorque);
                    acDrive.SetMinimumTorque(ACDrive.DEFAULT_MinimumTorque);

                    //
                    // Start AC drive
                    //
                    acDrive.StartDriveSyncSpeed();

                    Trace.WriteLine("Starting measurements...");

                    // Wait before taking measurements
                    acDrive.WaitDelay(DELAY_Measurement);

                    //
                    // Take measurements
                    //
                    this.voltage = powerMeter.ReadVoltagePhaseToPhase();
                    this.current = powerMeter.ReadCurrentPhase();
                    this.powerFactor = powerMeter.ReadPowerFactorAverage();

                    Trace.WriteLine("Finished measurements");

                    //
                    // Stop AC drive
                    //
                    acDrive.StopDrive();
                    acDrive.DisableDrivePower();

                    //
                    // Restore defaults
                    //
                    acDrive.SetSpeed(0);
                    acDrive.SetMaximumCurrent(ACDrive.DEFAULT_MaximumCurrent);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                lastError = e.Message;

                //
                // An exception has been thrown - attempt to shut down the drives
                //
                using (TcpClient client = new TcpClient(this.ipAddress, this.portNum))
                {
                    ModbusIpMaster master = ModbusIpMaster.CreateTcp(client);

                    ACDrive acDrive = new ACDrive(master);

                    //
                    // Stop AC drive
                    //
                    acDrive.StopDrive();
                    acDrive.DisableDrivePower();
                }
            }
#endif

            Trace.WriteLine("SynchronousSpeed.Execute(): Completed ");

        }

        //-------------------------------------------------------------------------------------------------//

        public int ExecutionTime()
        {
            Trace.WriteLine("SynchronousSpeed.ExecutionTime():");

            int execTime = 0;

            // Determine pre-measurement time
            execTime += ACDrive.DELAY_EnableDrivePower;
            execTime += ACDrive.DELAY_ResetDrive;

            execTime += ACDrive.DELAY_SetSpeed;
            execTime += ACDrive.DELAY_SetTorque;
            execTime += ACDrive.DELAY_SetSpeedRampTime;
            execTime += ACDrive.DELAY_SetMaximumCurrent;
            execTime += ACDrive.DELAY_SetMaximumTorque;
            execTime += ACDrive.DELAY_SetMinimumTorque;

            execTime += ACDrive.DELAY_StartDrive;

            // Determine measurement time
            execTime += DELAY_Measurement;

            // Determine post-measurement time
            execTime += ACDrive.DELAY_StopDrive;
            execTime += ACDrive.DELAY_DisableDrivePower;

            execTime += ACDrive.DELAY_SetSpeed;
            execTime += ACDrive.DELAY_SetMaximumCurrent;

            return execTime;
        }


    }
}
