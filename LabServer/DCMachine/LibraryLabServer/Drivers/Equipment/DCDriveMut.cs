using System;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using Modbus.Device;

namespace Library.LabServer.Drivers.Equipment
{
    public class DCDriveMut
    {
        #region Class Constants and Variables

        //
        // Command execution delay constants
        //
        public const int DELAY_ResetDriveFault = 5;
        public const int DELAY_ResetDrive = 5;
        public const int DELAY_SetMainContactorOn = 5;
        public const int DELAY_StartDrive = 5;
        public const int DELAY_StartDriveTorque = 5;
        public const int DELAY_SetSpeed = DEFAULT_SpeedRampTime;
        public const int DELAY_SetTorque = 1;
        public const int DELAY_SetMinSpeedLimit = 0;
        public const int DELAY_SetMaxSpeedLimit = 0;
        public const int DELAY_SetMinTorqueLimit = 0;
        public const int DELAY_SetMaxTorqueLimit = 0;
        public const int DELAY_SetSpeedRampTime = 0;
        public const int DELAY_SetField = 1;
        public const int DELAY_SetFieldTrim = 1;

        //
        // Default values for control registers
        //
        public const int DEFAULT_Speed = 0;
        public const int DEFAULT_Torque = 0;
        public const int DEFAULT_MinSpeedLimit = -1500;
        public const int DEFAULT_MaxSpeedLimit = 1500;
        public const int DEFAULT_MinTorqueLimit = -100;
        public const int DEFAULT_MaxTorqueLimit = 100;
        public const int DEFAULT_SpeedRampTime = 5;
        public const int DEFAULT_Field = 100;
        public const int DEFAULT_FieldTrim = 0;

        #endregion

        #region Properties

        private string lastError;

        public string LastError
        {
            get
            {
                string errorMsg = lastError;
                lastError = null;
                return errorMsg;
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        //
        // DC drive register addresses (read/write)
        //
        private const ushort REGADDR_RW_DC_ControlWord = 4200;
        private const ushort REGADDR_RW_DC_ControlSpeed = 4201;
        private const ushort REGADDR_RW_DC_ControlTorque = 4202;
        private const ushort REGADDR_RW_DC_MinSpeedLimit = 4203;
        private const ushort REGADDR_RW_DC_MaxSpeedLimit = 4204;
        private const ushort REGADDR_RW_DC_MaxTorqueLimit = 4205;
        private const ushort REGADDR_RW_DC_MinTorqueLimit = 4206;
        private const ushort REGADDR_RW_DC_SpeedRampTime = 4207;
        private const ushort REGADDR_RW_DC_FieldLimit = 4208;
        private const ushort REGADDR_RW_DC_FieldTrim = 4209;

        //
        // DC drive register addresses (read only)
        //
        private const ushort REGADDR_RO_DC_SpeedEncoder = 4002;
        private const ushort REGADDR_RO_DC_Torque = 4007;
        private const ushort REGADDR_RO_DC_ArmatureVoltage = 4013;
        private const ushort REGADDR_RO_DC_FieldCurrent = 4024;

        // Local variables
        private ModbusIpMaster master;

        //-------------------------------------------------------------------------------------------------//

        //
        // Constructor
        //
        public DCDriveMut(ModbusIpMaster master)
        {
            this.master = master;
        }

        //-------------------------------------------------------------------------------------------------//

        #region DC Drive Control

        public void ResetDriveFault()
        {
            Trace.Write("dcDriveMut.ResetDriveFault():");

            try
            {
                // Write multiple holding registers
                ushort[] regs = new ushort[1] { 0x04f6 };

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X4"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlWord, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X4"));

                WaitDelay(DELAY_ResetDriveFault);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void ResetDrive()
        {
            Trace.Write("dcDriveMut.ResetDrive():");

            try
            {
                // Write multiple holding registers
                ushort[] regs = new ushort[1] { 0x0476 };

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X4"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlWord, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X4"));

                WaitDelay(DELAY_ResetDrive);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMainContactorOn()
        {
            Trace.Write("dcDriveMut.SetMainContactorOn():");

            try
            {
                ushort[] regs = new ushort[1] { 0x0477 };

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X4"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlWord, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X4"));

                WaitDelay(DELAY_SetMainContactorOn);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void StartDrive()
        {
            Trace.Write("dcDriveMut.StartDrive():");

            try
            {
                ushort[] regs = new ushort[1] { 0x047F };

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X4"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlWord, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X4"));

                WaitDelay(DELAY_StartDrive);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void StartDriveTorque()
        {
            Trace.Write("dcDriveMut.StartDriveTorque():");

            try
            {
                ushort[] regs = new ushort[1] { 0x147F };

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X4"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlWord, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X4"));

                WaitDelay(DELAY_StartDriveTorque);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetSpeed(int speed)
        {
            Trace.Write("dcDriveMut.SetSpeed(): " + speed.ToString() + " rpm");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert speed to register values: -1500 to 1500 => -20,000 to 20,000
                speed = (speed * 20000) / 1500;
                regs[0] = (ushort)speed;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlSpeed.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlSpeed, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlSpeed, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetSpeed);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetTorque(int percent)
        {
            Trace.Write("dcDriveMut.SetTorque(): " + percent.ToString() + " percent");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert percent to register values: -327 to 327 => -32700 to 32700
                percent *= 100;
                regs[0] = (ushort)percent;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_ControlTorque.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_ControlTorque, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlTorque, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetTorque);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMinSpeedLimit(int speed)
        {
            Trace.Write("dcDriveMut.SetMinSpeedLimit(): " + speed.ToString() + " rpm");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert speed to register values: -10,000 to 0 => -10,000 to 0
                regs[0] = (ushort)speed;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_MinSpeedLimit.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_MinSpeedLimit, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MinSpeedLimit, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMinSpeedLimit);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMaxSpeedLimit(int speed)
        {
            Trace.Write("dcDriveMut.SetMaxSpeedLimit(): " + speed.ToString() + " rpm");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert speed to register values: 0 to 10,000 => 0 to 10,000
                regs[0] = (ushort)speed;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_MaxSpeedLimit.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_MaxSpeedLimit, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MaxSpeedLimit, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMaxSpeedLimit);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMinTorqueLimit(int percent)
        {
            Trace.Write("dcDriveMut.SetMinTorqueLimit(): " + percent.ToString() + " percent");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert percent to register values: -325 to 0 => -32500 to 0
                percent *= 100;
                regs[0] = (ushort)percent;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_MinTorqueLimit.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_MinTorqueLimit, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MinTorqueLimit, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMinTorqueLimit);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMaxTorqueLimit(int percent)
        {
            Trace.Write("dcDriveMut.SetMaxTorqueLimit(): " + percent.ToString() + " percent");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert percent to register values: 0 to 325 => 0 to 32500
                percent *= 100;
                regs[0] = (ushort)percent;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_MaxTorqueLimit.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_MaxTorqueLimit, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MaxTorqueLimit, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMaxTorqueLimit);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetSpeedRampTime(int time)
        {
            Trace.Write("dcDriveMut.SetSpeedRampTime(): " + time.ToString() + " seconds");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert time to register values: 0 to 300 => 0 to 30,000
                time *= 100;
                regs[0] = (ushort)time;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_SpeedRampTime.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_SpeedRampTime, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_SpeedRampTime, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetSpeedRampTime);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetField(int percent)
        {
            Trace.Write("dcDriveMut.SetField():");

            try
            {
                ushort[] regs = new ushort[1] { 0 };

                // Convert percent to register values: 0 to 100 => 0 to 10,000
                percent *= 100;
                regs[0] = (ushort)percent;

                int value = regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_DC_FieldLimit.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_DC_FieldLimit, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_FieldLimit, (ushort)1);
                value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetField);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void WaitDelay(int seconds)
        {
            try
            {
                for (int i = 0; i < seconds; i++)
                {
                    ReadControlWord(false);
                    Trace.Write(".");
                    Thread.Sleep(1000);
                }
                Trace.WriteLine("");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        #region DC Drive Status

        public ushort ReadControlWord(bool show)
        {
            if (show == true)
            {
                Trace.Write("dcDriveMut.ReadControlWord(): ");
            }

            ushort controlWord = 0;

            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlWord, (ushort)1);

                controlWord = regs[0];

                if (show == true)
                {
                    Trace.WriteLine(" Reading: 0x" + controlWord.ToString("x4"));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return controlWord;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadControlSpeed()
        {
            Trace.Write("dcDriveMut.ReadControlSpeed(): ");

            int speed = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlSpeed, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to speed: -20,000 to 20,000 => -1500 to 1500
                speed = (value * 1500) / 20000;
                Trace.WriteLine(" => " + speed.ToString() + " rpm");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return speed;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadControlTorque()
        {
            Trace.Write("dcDriveMut.ReadControlTorque(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_ControlTorque, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to percent: -32,768 to 32,767 => -327.68 to 327.67 
                percent = value / 100;
                Trace.WriteLine(" => " + percent.ToString() + " percent");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return percent;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadMinSpeedLimit()
        {
            Trace.Write("dcDriveMut.ReadMinSpeedLimit(): ");

            int speed = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MinSpeedLimit, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert speed to register values: -10,000 to 0 => -10,000 to 0
                speed = value;
                speed = (speed > 32767) ? speed - 65536 : speed;
                Trace.WriteLine(" => " + speed.ToString() + " rpm");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return speed;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadMaxSpeedLimit()
        {
            Trace.Write("dcDriveMut.ReadMaxSpeedLimit(): ");

            int speed = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MaxSpeedLimit, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert speed to register values: 0 to 10,000 => 0 to 10,000
                speed = value;
                Trace.WriteLine(" => " + speed.ToString() + " rpm");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return speed;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadMinTorqueLimit()
        {
            Trace.Write("dcDriveMut.ReadMinTorqueLimit(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MinTorqueLimit, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to percent: -32500 to 0 => -325 to 0
                percent = value;
                percent = (percent > 32767) ? percent - 65536 : percent;
                percent = percent / 100;
                Trace.WriteLine(" => " + percent.ToString() + " percent");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return percent;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadMaxTorqueLimit()
        {
            Trace.Write("dcDriveMut.ReadMaxTorqueLimit(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_MaxTorqueLimit, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to percent: 0 to 32500 => 0 to 325
                percent = value / 100;
                Trace.WriteLine(" => " + percent.ToString() + " percent");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return percent;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadSpeedRampTime()
        {
            Trace.Write("dcDriveMut.ReadSpeedRampTime(): ");

            int time = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_SpeedRampTime, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to time: 0 to 30,000 => 0 to 300
                time = value / 100;
                Trace.WriteLine(" => " + time.ToString() + " seconds");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return time;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadFieldLimit()
        {
            Trace.Write("dcDriveMut.ReadFieldLimit(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_FieldLimit, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to percent: 0 to 10,000 => 0 to 100
                percent = value / 100;
                Trace.WriteLine(" => " + percent.ToString() + " percent");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return percent;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadFieldTrim()
        {
            Trace.Write("dcDriveMut.ReadFieldTrim(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_DC_FieldTrim, (ushort)1);
                int value = inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register value to percent: -200 to 200 => -20 to 20
                percent = value / 10;
                Trace.WriteLine(" => " + percent.ToString() + " percent");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return percent;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadDriveSpeed()
        {
            Trace.Write("dcDriveMut.ReadDriveSpeed():");

            int speed = 0;

            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RO_DC_SpeedEncoder, (ushort)1);

                // Convert the speed to an integer value
                speed = regs[0];
                speed = (speed > 32767) ? speed - 65536 : speed;

                Trace.WriteLine(" => " + speed.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return speed;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadArmatureVoltage()
        {
            Trace.Write("dcDriveMut.ReadArmatureVoltage():");

            int voltage = 0;

            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RO_DC_ArmatureVoltage, (ushort)1);

                // Convert the voltage to an integer value
                voltage = regs[0];
                voltage = (voltage > 32767) ? voltage - 65536 : voltage;

                Trace.WriteLine(" => " + voltage.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return voltage;
        }

        //-------------------------------------------------------------------------------------------------//

        public float ReadFieldCurrent()
        {
            Trace.Write("dcDriveMut.ReadFieldCurrent():");

            float current = 0;

            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RO_DC_FieldCurrent, (ushort)1);

                // Convert current to an integer value
                current = regs[0];
                current /= 100;

                Trace.WriteLine(" => " + current.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return current;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadTorque()
        {
            Trace.Write("dcDriveMut.ReadTorque():");

            int percent = 0;

            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RO_DC_Torque, (ushort)1);

                // Convert percentage to an integer value
                percent = regs[0];
                percent /= 100;

                Trace.WriteLine(" => " + percent.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return percent;
        }

        #endregion

    }
}
