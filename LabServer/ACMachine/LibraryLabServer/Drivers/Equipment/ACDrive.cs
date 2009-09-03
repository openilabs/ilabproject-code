using System;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using Modbus.Device;

namespace Library.LabServer.Drivers.Equipment
{
    public class ACDrive
    {
        //
        // Command execution delay constants
        //
        public const int DELAY_EnableDrivePower = 5;
        public const int DELAY_DisableDrivePower = 10;
        public const int DELAY_ResetDrive = 5;
        public const int DELAY_StartDrive = 5;
        public const int DELAY_StopDrive = 10;
        public const int DELAY_SetSpeed = 1;
        public const int DELAY_SetTorque = 1;
        public const int DELAY_SetSpeedRampTime = 0;
        public const int DELAY_SetMaximumCurrent = 0;
        public const int DELAY_SetMaximumTorque = 0;
        public const int DELAY_SetMinimumTorque = 0;

        //
        // Default values for control registers
        //
        public const int DEFAULT_Speed = 0;
        public const int DEFAULT_Torque = 0;
        public const int DEFAULT_SpeedRampTime = 3;
        public const int DEFAULT_MaximumCurrent = 5500;
        public const int DEFAULT_MaximumTorque = 100;
        public const int DEFAULT_MinimumTorque = -100;

        //
        // Maximum and minimum values for control registers
        //
        public const int MAXIMUM_Speed = 1500;
        public const int MAXIMUM_MaximumCurrent = 10000;

        //
        // Properties
        //
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

        //-------------------------------------------------------------------------------------------------//

        //
        // AC Drive register addresses (read/write)
        //
        private const ushort REGADDR_RW_AC_ControlWord = 3200;
        private const ushort REGADDR_RW_AC_ControlSpeed = 3202;
        private const ushort REGADDR_RW_AC_ControlTorque = 3204;
        private const ushort REGADDR_RW_AC_SpeedRampTime = 3206;
        private const ushort REGADDR_RW_AC_MaximumCurrent = 3208;
        private const ushort REGADDR_RW_AC_MaximumTorque = 3210;
        private const ushort REGADDR_RW_AC_MinimumTorque = 3212;

        //
        // AC Drive register addresses (read only)
        //
        private const ushort REGADDR_RO_AC_DriveSpeed = 3000;
        private const ushort REGADDR_RO_AC_DriveTorque = 3006;
        private const ushort REGADDR_RO_AC_ActiveFault = 3050;

        // Local variables
        private ModbusIpMaster master;

        //-------------------------------------------------------------------------------------------------//

        //
        // Constructor
        //
        public ACDrive(ModbusIpMaster master)
        {
            this.master = master;
        }

        //-------------------------------------------------------------------------------------------------//

        #region AC Drive Control

        public void EnableDrivePower()
        {
            Trace.Write("ACDrive.EnableDrivePower():");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x4000 };

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X8"));

                // Write to register
                this.master.WriteMultipleRegisters(0, REGADDR_RW_AC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlWord, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X8"));

                WaitDelay(DELAY_EnableDrivePower);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void DisableDrivePower()
        {
            Trace.Write("ACDrive.DisableDrivePower():");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X8"));

                // Write to register
                this.master.WriteMultipleRegisters(0, REGADDR_RW_AC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlWord, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X8"));

                WaitDelay(DELAY_DisableDrivePower);
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
            Trace.Write("ACDrive.ResetDrive():");

            try
            {
                ushort[] regs = new ushort[2] { 0x09a1, 0x4000 };

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X8"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlWord, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X8"));

                WaitDelay(DELAY_ResetDrive);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void StartDriveLockedRotor()
        {
            ushort[] regs = new ushort[2] { 0x08a2, 0x5000 };
            StartDrive(regs);
        }

        public void StartDriveSyncSpeed()
        {
            ushort[] regs = new ushort[2] { 0x08a2, 0x6000 };
            StartDrive(regs);
        }

        public void StartDriveNoLoad()
        {
            ushort[] regs = new ushort[2] { 0x08a1, 0x6000 };
            StartDrive(regs);
        }

        public void StartDriveFullLoad()
        {
            ushort[] regs = new ushort[2] { 0x08a2, 0x6000 };
            StartDrive(regs);
        }

        //-------------------------------------------------------------------------------------------------//

        private void StartDrive(ushort[] regs)
        {
            Trace.Write("ACDrive.StartDrive():");

            try
            {
                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X8"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlWord, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X8"));

                WaitDelay(DELAY_StartDrive);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void StopDriveFullLoad()
        {
            ushort[] regs = new ushort[2] { 0x0821, 0x6000 };
            StopDrive(regs);
        }

        public void StopDriveOther()
        {
            ushort[] regs = new ushort[2] { 0x08A1, 0x4000 };
            StopDrive(regs);
        }

        //-------------------------------------------------------------------------------------------------//

        private void StopDrive(ushort[] regs)
        {
            Trace.Write("ACDrive.StopDrive():");

            try
            {
                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlWord.ToString());
                Trace.Write(" Writing: 0x" + value.ToString("X8"));

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_ControlWord, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlWord, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: 0x" + value.ToString("X8"));

                WaitDelay(DELAY_StopDrive);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetSpeed(int rpm)
        {
            Trace.Write("ACDrive.SetSpeed(): " + rpm.ToString() + " rpm");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                // Convert speed to register values: -3000 to 3000 => -200,000,000 to 200,000,000
                int speed = (rpm * 200000) / 3;
                regs[0] = (ushort)speed;
                regs[1] = (ushort)(speed >> 16);

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlSpeed.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_ControlSpeed, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlSpeed, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
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
            Trace.Write("ACDrive.SetTorque(): " + percent.ToString() + " percent");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                // Convert torque to register values: -2000 to 2000 => -200,000,000 to 200,000,000
                int torque = percent * 100000;
                regs[0] = (ushort)torque;
                regs[1] = (ushort)(torque >> 16);

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_ControlTorque.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_ControlTorque, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlTorque, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
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

        public void SetSpeedRampTime(int time)
        {
            Trace.Write("ACDrive.SetSpeedRampTime(): " + time.ToString() + " seconds");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                // Convert time to register values: 0 to 1,800 => 0 to 1,800,000
                time *= 1000;
                regs[0] = (ushort)time;
                regs[1] = (ushort)(time >> 16);

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_SpeedRampTime.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_SpeedRampTime, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_SpeedRampTime, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
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

        public void SetMaximumCurrent(int current)
        {
            Trace.Write("ACDrive.SetMaximumCurrent(): " + current.ToString() + " milliAmps");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                // Convert current to register values: 0 to 30,000,000 => 0 to 3,000,000
                current = current / 10;
                regs[0] = (ushort)current;
                regs[1] = (ushort)(current >> 16);

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_MaximumCurrent.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_MaximumCurrent, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_MaximumCurrent, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMaximumCurrent);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMaximumTorque(int percent)
        {
            Trace.Write("ACDrive.SetMaximumTorque(): " + percent.ToString() + " percent");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                // Convert percent to register values: 0 to 100 => 0 to 1,000
                percent *= 10;
                regs[0] = (ushort)percent;
                regs[1] = (ushort)(percent >> 16);

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_MaximumTorque.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_MaximumTorque, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_MaximumTorque, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMaximumTorque);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public void SetMinimumTorque(int percent)
        {
            Trace.Write("ACDrive.SetMinimumTorque(): " + percent.ToString() + " percent");

            try
            {
                ushort[] regs = new ushort[2] { 0x0000, 0x0000 };

                // Convert percent to register values: -1600 to -0.1 => 49536 to 65535
                percent = 65535 - ((65536 - 49536) * -percent / 1600);
                regs[0] = (ushort)percent;
                regs[1] = (ushort)(percent >> 16);

                int value = (regs[1] << 16) | regs[0];
                Trace.Write(" Reg: " + REGADDR_RW_AC_MinimumTorque.ToString());
                Trace.Write(" Writing: " + value.ToString());

                // Write to register
                this.master.WriteMultipleRegisters(REGADDR_RW_AC_MinimumTorque, regs);

                // Read back register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_MinimumTorque, (ushort)2);
                value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                WaitDelay(DELAY_SetMaximumTorque);
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

        #region AC Drive Status

        public uint ReadControlWord(bool show)
        {
            if (show == true)
            {
                Trace.Write("ACDrive.ReadControlWord(): ");
            }

            uint controlWord = 0;

            try
            {
                // Read registers
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlWord, (ushort)2);

                controlWord = (uint)((regs[1] << 16) | regs[0]);

                if (show == true)
                {
                    Trace.WriteLine(" Reading: 0x" + regs[1].ToString("x4") + "-" + regs[0].ToString("x4"));
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

        public int ReadActiveFault()
        {
            Trace.Write("ACDrive.ReadActiveFault():");

            int value = 0;
            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RO_AC_ActiveFault, (ushort)2);
                value = (regs[1] << 16) | regs[0];

                Trace.WriteLine(" => " + value.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return value;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadControlSpeed()
        {
            Trace.Write("ACDrive.ReadControlSpeed(): ");

            int speed = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlSpeed, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to speed: -200,000,000 to 200,000,000 => -3000 to 3000
                speed = value * 3 / 200000;
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
            Trace.Write("ACDrive.ReadControlTorque(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_ControlTorque, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to torque: -200,000,000 to 200,000,000 => -2000 to 2000
                percent = value / 100000;
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
            Trace.Write("ACDrive.ReadSpeedRampTime(): ");

            int seconds = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_SpeedRampTime, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to time: 0 to 1,800,000 => 0 to 1,800
                seconds = value / 1000;
                Trace.WriteLine(" => " + seconds.ToString() + " seconds");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadMaximumCurrent()
        {
            Trace.Write("ACDrive.ReadMaximumCurrent(): ");

            int current = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_MaximumCurrent, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to current: 0 to 3,000,000 => 0 to 30,000,000
                current = value * 10;
                Trace.WriteLine(" => " + current.ToString() + " milliAmps");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return current;
        }

        //-------------------------------------------------------------------------------------------------//

        public int ReadMinimumTorque()
        {
            Trace.Write("ACDrive.ReadMinimumTorque():");

            int percent = 0;
            try
            {
                // Convert percent to register values: -1600 to -0.1 => 49536 to 65535
                //percent = 65535 - ((65536 - 49536) * -percent / 1600);
                //regs[0] = (ushort)percent;
                //regs[1] = (ushort)(percent >> 16);

                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_MinimumTorque, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to percent: 49536 to 65535 => -1600 to -0.1
                percent = -(65535 - value) * 1600 / (65536 - 49536);
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

        public int ReadMaximumTorque()
        {
            Trace.Write("ACDrive.ReadMaximumTorque(): ");

            int percent = 0;
            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RW_AC_MaximumTorque, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to percent: 0 to 1,000 => 0 to 100
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
            Trace.Write("ACDrive.ReadDriveSpeed(): ");

            int speed = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RO_AC_DriveSpeed, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to speed: -3,000,000 to 3,000,000 => -30,000 to 30,000
                speed = value / 100;
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

        public int ReadDriveTorque()
        {
            Trace.Write("ACDrive.ReadDriveTorque(): ");

            int percent = 0;

            try
            {
                // Read register
                ushort[] inregs = this.master.ReadHoldingRegisters(REGADDR_RO_AC_DriveTorque, (ushort)2);
                int value = (inregs[1] << 16) | inregs[0];
                Trace.Write(" Reading: " + value.ToString());

                // Convert register values to percent: 0 to 16,000 => 0 to 1,600
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

        #endregion

    }
}
