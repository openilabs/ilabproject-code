using System;
using Library.LabEquipment;
using LabEquipment.Drivers;

namespace LabEquipment
{
    public class EquipmentEngine : LabEquipmentEngine
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "EquipmentEngine";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_SerialPort = " SerialPort: ";
        private const string STRLOG_BaudRate = " BaudRate: ";
        private const string STRLOG_TubeOffsetDistance = " TubeOffsetDistance: ";
        private const string STRLOG_TubeHomeDistance = " TubeHomeDistance: ";
        private const string STRLOG_FlexMotionBoardID = " FlexMotionBoardID: ";
        private const string STRLOG_FlexMotionInitialising = " Initialising FlexMotion controller ...";
        private const string STRLOG_FlexMotionPresentInitialised = " FlexMotion controller is present and initialised";
        private const string STRLOG_FlexMotionNotPresent = " FlexMotion controller is not present!";
        private const string STRLOG_SerialLcdNotPresent = " Serial LCD is not present!";
        private const string STRLOG_SerialLcdPresent = " Serial LCD is present";
        private const string STRLOG_StartingRadiationCounter = " Starting radiation counter ...";

        #endregion

        #region Properties

        private FlexMotion flexMotion;
        private SerialLcd serialLcd;

        public FlexMotion DeviceFlexMotion
        {
            get { return this.flexMotion; }
        }

        public SerialLcd DeviceSerialLcd
        {
            get { return this.serialLcd; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public EquipmentEngine()
            : base()
        {
            const string STRLOG_MethodName = "EquipmentEngine";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Get the serial port to use and the baud rate
            //
            string serialport = Utilities.GetAppSetting(Consts.StrCfg_SerialPort);
            int baudrate = Utilities.GetIntAppSetting(Consts.StrCfg_BaudRate);
            Logfile.Write(STRLOG_SerialPort + serialport);
            Logfile.Write(STRLOG_BaudRate + baudrate.ToString());

            //
            // Create an instance of the SerialLcd class
            //
            this.serialLcd = new SerialLcd(serialport, baudrate);

            //
            // Get NI FlexMotion controller card board ID
            //
            int boardID = Utilities.GetIntAppSetting(Consts.StrCfg_FlexMotionBoardID);
            Logfile.Write(STRLOG_FlexMotionBoardID + boardID.ToString());

            //
            // Get tube offset distance and home position
            //
            int tubeOffsetDistance = Utilities.GetIntAppSetting(Consts.StrCfg_TubeOffsetDistance);
            int tubeHomeDistance = Utilities.GetIntAppSetting(Consts.StrCfg_TubeHomeDistance);
            bool initTubeAxis = Utilities.GetBoolAppSetting(Consts.StrCfg_InitTubeAxis);
            Logfile.Write(STRLOG_TubeOffsetDistance + tubeOffsetDistance.ToString());
            Logfile.Write(STRLOG_TubeHomeDistance + tubeHomeDistance.ToString());

            //
            // Create an instance of the FlexMotion class
            //
            this.flexMotion = new FlexMotion(boardID, tubeOffsetDistance, tubeHomeDistance, initTubeAxis);

            //
            // Determine powerup delay
            //
            int powerupDelay = this.flexMotion.PowerupDelay;
            if (powerupDelay > this.PowerupDelay)
            {
                this.PowerupDelay = powerupDelay;
            }

            // Set initialisation delay
            this.InitialiseDelay = this.flexMotion.InitialiseDelay;

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        public override void PowerupEquipment()
        {
            const string STRLOG_MethodName = "PowerupEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Enable power to the external equipment
            //
            this.flexMotion.EnablePower();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        public override void InitialiseEquipment()
        {
            const string STRLOG_MethodName = "InitialiseEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Get serial LCD version if possible
            //
            string firmwareVersion = this.serialLcd.GetHardwareFirmwareVersion();
            if (firmwareVersion == null)
            {
                Logfile.WriteError(STRLOG_SerialLcdNotPresent);
            }
            else
            {
                Logfile.Write(STRLOG_SerialLcdPresent);

                //
                // Get LabEquipment service title and display
                //
                string title = Utilities.GetAppSetting(Consts.StrCfg_LabEquipmentTitle);
                this.serialLcd.WriteLine(1, title);

                // Display serial LCD firmware version
                this.serialLcd.WriteLine(2, firmwareVersion);

                // Ensure data capture is stopped
                this.serialLcd.StopCapture();
            }

            //
            // Initialise the FlexMotion controller
            //
            Logfile.Write(STRLOG_FlexMotionInitialising);
            if (this.flexMotion.Initialise() == false)
            {
                Logfile.WriteError(STRLOG_FlexMotionNotPresent);
            }
            else
            {
                Logfile.Write(STRLOG_FlexMotionPresentInitialised);
            }

            //
            // Reset initialisation delay
            //
            this.InitialiseDelay = this.flexMotion.InitialiseDelay;

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        public override void PowerdownEquipment()
        {
            const string STRLOG_MethodName = "PowerdownEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Disable power to the external equipment
            //
            this.flexMotion.DisablePower();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                // Dispose managed resources here. Anything that has a Dispose() method.
                if (this.serialLcd != null)
                {
                    this.serialLcd.Close();
                }
            }

            //
            // Release unmanaged resources here. Set large fields to null.
            //

            // Call Dispose on your base class.
            base.Dispose(disposing);
        }
    }
}
