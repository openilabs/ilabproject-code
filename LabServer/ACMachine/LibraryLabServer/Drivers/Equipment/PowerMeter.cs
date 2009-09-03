using System;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using Modbus.Device;

namespace Library.LabServer.Drivers.Equipment
{
    class PowerMeter
    {
        //
        // Power meter register addresses (read only)
        //
        private const ushort REGADDR_RW_PM_VoltagePhaseToPhase = 2072;
        private const ushort REGADDR_RW_PM_CurrentPhaseToPhase = 2074;
        private const ushort REGADDR_RW_PM_PowerFactorAverage = 2082;

        // Local variables
        private ModbusIpMaster master;

        //-------------------------------------------------------------------------------------------------//

        //
        // Constructor
        //
        public PowerMeter(ModbusIpMaster master)
        {
            this.master = master;
        }

        //-------------------------------------------------------------------------------------------------//

        public float ReadVoltagePhaseToPhase()
        {
            Trace.Write("PowerMeter.ReadVoltagePhaseToPhase():");

            float fvalue;
            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RW_PM_VoltagePhaseToPhase, (ushort)2);
                int ivalue = (regs[1] << 16) | regs[0];

                fvalue = Conversion.ToFloat(ivalue);
                Trace.WriteLine(" => " + fvalue.ToString("F04"));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return fvalue;
        }

        //-------------------------------------------------------------------------------------------------//

        public float ReadCurrentPhase()
        {
            Trace.Write("PowerMeter.ReadCurrentPhase():");

            float fvalue;
            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RW_PM_CurrentPhaseToPhase, (ushort)2);
                int ivalue = (regs[1] << 16) | regs[0];

                fvalue = Conversion.ToFloat(ivalue);
                Trace.WriteLine(" => " + fvalue.ToString("F04"));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return fvalue;
        }

        //-------------------------------------------------------------------------------------------------//

        public float ReadPowerFactorAverage()
        {
            Trace.Write("PowerMeter.ReadPowerFactorAverage():");

            float fvalue;
            try
            {
                // Read register
                ushort[] regs = this.master.ReadHoldingRegisters(REGADDR_RW_PM_PowerFactorAverage, (ushort)2);
                int ivalue = (regs[1] << 16) | regs[0];

                fvalue = Conversion.ToFloat(ivalue);
                Trace.WriteLine(" => " + fvalue.ToString("F04"));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw (e);
            }

            return fvalue;
        }



    }
}
