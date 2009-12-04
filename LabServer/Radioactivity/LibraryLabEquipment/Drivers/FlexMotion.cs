using System;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using Library.Lab;
using Library.LabEquipment.Engine;

namespace Library.LabEquipment.Drivers
{
    public class FlexMotion
    {
        #region DLL Import

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_initialize_controller(byte boardID, byte[] settings);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_read_csr_rtn(byte boardID, ref ushort csr);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_find_reference(byte boardID, byte axisOrVectorSpace, ushort axisOrVSMap, byte searchType);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_check_reference(byte boardID, byte axisOrVectorSpace, ushort axisOrVSMap, ref ushort found, ref ushort finding);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_reset_pos(byte boardID, byte axis, int position1, int position2, byte inputVector);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_read_pos_rtn(byte boardID, byte axis, ref int position);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_set_op_mode(byte boardID, byte axis, ushort operationMode);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_load_target_pos(byte boardID, byte axis, int targetPosition, byte inputVector);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_start(byte boardID, byte axisOrVectorSpace, ushort axisOrVSMap);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_stop_motion(byte boardID, byte axisOrVectorSpace, ushort stopType, ushort axisOrVSMap);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_read_axis_status_rtn(byte boardID, byte axis, ref ushort axisStatus);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_read_error_msg_rtn(byte boardID, ref ushort commandID, ref ushort resourceID, ref int errorCode);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_get_error_description(ushort descriptionType, int errorCode, ushort commandID, ushort resourceID, char[] charArray, ref int sizeOfArray);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_set_breakpoint_output_momo(byte boardID, byte axisOrEncoder, ushort mustOn, ushort mustOff, byte inputVector);

        [DllImport("FlexMotion32.dll")]
        private static extern int flex_enable_breakpoint(byte boardID, byte axisOrEncoder, byte enable);

        #endregion

        #region Class Constants and Variables

        private const string STRLOG_ClassName = "FlexMotion";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_NotInitialised = " Not Initialised!";
        private const string STRLOG_Initialising = " Initialising...";
        private const string STRLOG_FlexMotionBoardID = " FlexMotionBoardID: ";
        private const string STRLOG_TubeOffsetDistance = " TubeOffsetDistance: ";
        private const string STRLOG_TubeHomeDistance = " TubeHomeDistance: ";
        private const string STRLOG_TubeMoveRate = " TubeMoveRate: ";
        private const string STRLOG_TubeInitAxis = " TubeInitAxis: ";
        private const string STRLOG_FindingTubeForwardLimit = " Finding tube forward limit...";
        private const string STRLOG_FindingTubeReverseLimit = " Finding tube reverse limit...";
        private const string STRLOG_ResetingTubePosition = " Reseting tube position...";
        private const string STRLOG_SettingTubeDistanceToHomePosition = " Setting tube distance to home position...";
        private const string STRLOG_SettingSourceToHomeLocation = " Setting source to home location...";
        private const string STRLOG_SettingAbsorberToHomeLocation = " Setting absorber to home location...";
        private const string STRLOG_StartingRadiationCounter = " Starting radiation counter...";
        private const string STRLOG_Location = " Location: ";
        private const string STRLOG_Success = " Success: ";

        //
        // String constants for error messages
        //
        private const string STRERR_FlexMotionBoardNotPresent = "FlexMotion board is not present! ";
        private const string STRERR_PowerEnableBreakpointFailed = "Failed to set PowerEnable breakpoint! ";
        private const string STRERR_CounterStartBreakpointFailed = "Failed to set CounterStart breakpoint! ";
        private const string STRERR_FindTubeReverseLimitFailed = "FindTubeReverseLimit Failed! ";
        private const string STRERR_ResetTubePositionFailed = "ResetTubePosition Failed! ";
        private const string STRERR_FindTubeForwardLimitFailed = "FindTubeForwardLimit Failed! ";
        private const string STRERR_SetTubeDistanceFailed = "SetTubeDistance Failed! ";
        private const string STRERR_SetSourceLocationFailed = "SetSourceLocation Failed! ";
        private const string STRERR_SetAbsorberLocationFailed = "SetAbsorberLocation Failed! ";
        private const string STRERR_InvalidLocation = " Invalid Location: ";

        //
        // Power-up and power-down delays in seconds
        //
        private const int DELAY_POWERUP = 5;
        private const int DELAY_INITIALISE_WITH_ABSORBERS = 40;
        private const int DELAY_INITIALISE_NO_ABSORBERS = 27;

        // Tube axis encoder counts per mm distance moved
        private const int ENCODER_COUNTS_PER_MM = 43000;

        //
        // Local variables
        //
        private bool isPresent;
        private bool powerupReset;
        private bool initialised;
        private string lastError;
        private byte boardID;
        private bool hasAbsorberTable;
        private byte tubeAxis;
        private byte sourceAxis;
        private byte absorberAxis;
        private int tubeOffsetDistance;
        private double tubeMoveRate;
        private bool tubeInitAxis;
        private byte powerEnableBreakpoint;
        private byte counterStartBreakpoint;

        private struct AxisInfo
        {
            public int[] encoderPositions;
            public double[] selectTimes;
            public double[] returnTimes;
        }

        private AxisInfo sourceAxisInfo;
        private AxisInfo absorberAxisInfo;

        #endregion

        #region Properties

        private int tubeHomeDistance;
        private char sourceFirstLocation;
        private char sourceLastLocation;
        private char sourceHomeLocation;
        private char absorberFirstLocation;
        private char absorberLastLocation;
        private char absorberHomeLocation;
        private bool online;
        private string statusMessage;

        /// <summary>
        /// Returns the time (in seconds) that it takes for the equipment to power-up.
        /// </summary>
        public int PowerupDelay
        {
            get { return DELAY_POWERUP; }
        }

        /// <summary>
        /// Returns the time (in seconds) that it takes for the equipment to initialise.
        /// </summary>
        public int InitialiseDelay
        {
            get
            {
                int delay = 1;

                if (this.initialised == false)
                {
                    if (this.hasAbsorberTable == true)
                    {
                        delay = DELAY_INITIALISE_WITH_ABSORBERS;
                    }
                    else
                    {
                        delay = DELAY_INITIALISE_NO_ABSORBERS;
                    }
                }

                return delay;
            }
        }

        public int TubeHomeDistance
        {
            get { return this.tubeHomeDistance; }
            set { this.tubeHomeDistance = value; }
        }

        public char SourceFirstLocation
        {
            get { return this.sourceFirstLocation; }
        }

        public char SourceLastLocation
        {
            get { return this.sourceLastLocation; }
        }

        public char SourceHomeLocation
        {
            get { return this.sourceHomeLocation; }
        }

        public char AbsorberFirstLocation
        {
            get { return this.absorberFirstLocation; }
        }

        public char AbsorberLastLocation
        {
            get { return this.absorberLastLocation; }
        }

        public char AbsorberHomeLocation
        {
            get { return this.absorberHomeLocation; }
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

        //---------------------------------------------------------------------------------------//

        public FlexMotion(XmlNode xmlNodeEquipmentConfig)
        {
            const string STRLOG_MethodName = "FlexMotion";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Initialise local variables
            //
            this.initialised = false;
            this.lastError = string.Empty;
            this.isPresent = false;
            this.powerupReset = false;
            this.tubeAxis = Nimc.AXIS1;
            this.powerEnableBreakpoint = this.tubeAxis;
            this.sourceAxis = Nimc.AXIS2;
            this.counterStartBreakpoint = this.sourceAxis;
            this.absorberAxis = Nimc.AXIS3;
            this.hasAbsorberTable = false;

            //
            // Initialise properties
            //
            this.online = false;
            this.statusMessage = STRLOG_NotInitialised;

            //
            // Get NI FlexMotion controller card board ID
            //
            this.boardID = (byte)XmlUtilities.GetIntValue(xmlNodeEquipmentConfig, Consts.STRXML_flexMotionBoardID);
            Logfile.Write(STRLOG_FlexMotionBoardID + boardID.ToString());

            //
            // Initialise tube settings
            //
            XmlNode xmlNode = XmlUtilities.GetXmlNode(xmlNodeEquipmentConfig, Consts.STRXML_tube);
            this.tubeOffsetDistance = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_offsetDistance);
            this.tubeHomeDistance = XmlUtilities.GetIntValue(xmlNode, Consts.STRXML_homeDistance);
            this.tubeMoveRate = XmlUtilities.GetRealValue(xmlNode, Consts.STRXML_moveRate);
            this.tubeInitAxis = XmlUtilities.GetBoolValue(xmlNode, Consts.STRXML_initAxis, false);
            Logfile.Write(STRLOG_TubeOffsetDistance + tubeOffsetDistance.ToString());
            Logfile.Write(STRLOG_TubeHomeDistance + tubeHomeDistance.ToString());
            Logfile.Write(STRLOG_TubeMoveRate + tubeMoveRate.ToString());

            //
            // Initialise source settings
            //
            xmlNode = XmlUtilities.GetXmlNode(xmlNodeEquipmentConfig, Consts.STRXML_sources);
            this.sourceFirstLocation = XmlUtilities.GetCharValue(xmlNode, Consts.STRXML_firstLocation);
            this.sourceHomeLocation = XmlUtilities.GetCharValue(xmlNode, Consts.STRXML_homeLocation);
            this.sourceLastLocation = this.sourceFirstLocation;

            //
            // Initialise source encoder positions array
            //
            string sourceEncoderPositions = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_encoderPositions, false);
            string[] strSplit = sourceEncoderPositions.Split(new char[] { Engine.Consts.CHR_CsvSplitterChar });
            this.sourceAxisInfo.encoderPositions = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.encoderPositions[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise source select times array
            //
            string sourceSelectTimes = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_selectTimes, false);
            strSplit = sourceSelectTimes.Split(new char[] { Engine.Consts.CHR_CsvSplitterChar });
            this.sourceAxisInfo.selectTimes = new double[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.selectTimes[i] = Double.Parse(strSplit[i]);
            }
            this.sourceLastLocation = (char)(this.sourceFirstLocation + this.sourceAxisInfo.selectTimes.Length - 1);

            //
            // Initialise source return times array
            //
            string sourceReturnTimes = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_returnTimes, false);
            strSplit = sourceReturnTimes.Split(new char[] { Engine.Consts.CHR_CsvSplitterChar });
            this.sourceAxisInfo.returnTimes = new double[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.returnTimes[i] = Double.Parse(strSplit[i]);
            }

            //
            // Initialise absorber settings
            //
            xmlNode = XmlUtilities.GetXmlNode(xmlNodeEquipmentConfig, Consts.STRXML_absorbers);
            this.absorberFirstLocation = XmlUtilities.GetCharValue(xmlNode, Consts.STRXML_firstLocation);
            this.absorberHomeLocation = XmlUtilities.GetCharValue(xmlNode, Consts.STRXML_homeLocation);
            this.absorberLastLocation = this.absorberFirstLocation;

            try
            {
                //
                // Initialise absorber encoder positions array
                //
                string absorberEncoderPositions = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_encoderPositions, false);
                strSplit = absorberEncoderPositions.Split(new char[] { Engine.Consts.CHR_CsvSplitterChar });
                this.absorberAxisInfo.encoderPositions = new int[strSplit.Length];
                for (int i = 0; i < strSplit.Length; i++)
                {
                    this.absorberAxisInfo.encoderPositions[i] = Int32.Parse(strSplit[i]);
                }

                //
                // Initialise absorber select times array
                //
                string absorberSelectTimes = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_selectTimes, false);
                strSplit = absorberSelectTimes.Split(new char[] { Engine.Consts.CHR_CsvSplitterChar });
                this.absorberAxisInfo.selectTimes = new double[strSplit.Length];
                for (int i = 0; i < strSplit.Length; i++)
                {
                    this.absorberAxisInfo.selectTimes[i] = Double.Parse(strSplit[i]);
                }
                this.absorberLastLocation = (char)(this.absorberFirstLocation + this.absorberAxisInfo.selectTimes.Length - 1);

                //
                // Initialise absorber return times array
                //
                string absorberReturnTimes = XmlUtilities.GetXmlValue(xmlNode, Consts.STRXML_returnTimes, false);
                strSplit = absorberReturnTimes.Split(new char[] { Engine.Consts.CHR_CsvSplitterChar });
                this.absorberAxisInfo.returnTimes = new double[strSplit.Length];
                for (int i = 0; i < strSplit.Length; i++)
                {
                    this.absorberAxisInfo.returnTimes[i] = Double.Parse(strSplit[i]);
                }

                //
                // There is an absorber table
                //
                this.hasAbsorberTable = true;
            }
            catch
            {
                // No absorber table
            }

            //
            // Initialise the Flexmotion controller card. Must be done here because a breakpoint
            // on the controller card is used to powerup the equipment and initialisation is
            // carried out after the equipment is powered up.
            //
            this.isPresent = InitialiseController();
            if (this.isPresent == false)
            {
                Logfile.Write(STRERR_FlexMotionBoardNotPresent);
            }
            else
            {
                //
                // Initialisation is complete
                //
                this.online = true;
                this.statusMessage = StatusCodes.Ready.ToString();
            }

            if (this.powerupReset == true)
            {
                this.initialised = false;
            }

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public string GetLastError()
        {
            string lastError = this.lastError;
            this.lastError = string.Empty;
            return lastError;
        }

        //---------------------------------------------------------------------------------------//

        public bool EnablePower()
        {
            const string STRLOG_MethodName = "EnablePower";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            try
            {
                //
                // Make sure FlexMotion controller is present
                //
                if (this.isPresent == false)
                {
                    throw new ArgumentException(STRERR_FlexMotionBoardNotPresent);
                }

                //
                // Ensure power-enable and counter-start signals are inactive
                //
                if (this.SetBreakpoint(this.boardID, this.powerEnableBreakpoint, false) == false)
                {
                    throw new ArgumentException(STRERR_PowerEnableBreakpointFailed + this.GetLastError());
                }
                if (this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false) == false)
                {
                    throw new ArgumentException(STRERR_CounterStartBreakpointFailed + this.GetLastError());
                }

                //
                // Toggle the counter-start signal to enable both signals
                //
                if (this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, true) == false)
                {
                    throw new ArgumentException(STRERR_CounterStartBreakpointFailed + this.GetLastError());
                }
                if (this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false) == false)
                {
                    throw new ArgumentException(STRERR_CounterStartBreakpointFailed + this.GetLastError());
                }

                //
                // Enable the power
                //
                if (this.SetBreakpoint(this.boardID, this.powerEnableBreakpoint, true) == false)
                {
                    throw new ArgumentException(STRERR_PowerEnableBreakpointFailed + this.GetLastError());
                }

                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public bool DisablePower()
        {
            const string STRLOG_MethodName = "DisablePower";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            try
            {
                //
                // Make sure FlexMotion controller is present
                //
                if (this.isPresent == false)
                {
                    throw new ArgumentException(STRERR_FlexMotionBoardNotPresent);
                }

                //
                // Make the counter-start and power-enable signals inactive
                //
                if (this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false) == false)
                {
                    throw new ArgumentException(STRERR_CounterStartBreakpointFailed + this.GetLastError());
                }
                if (this.SetBreakpoint(this.boardID, this.powerEnableBreakpoint, false) == false)
                {
                    throw new ArgumentException(STRERR_PowerEnableBreakpointFailed + this.GetLastError());
                }

                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public bool Initialise()
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

                try
                {
                    //
                    // Make sure FlexMotion controller is present
                    //
                    if (this.isPresent == false)
                    {
                        throw new ArgumentException(STRERR_FlexMotionBoardNotPresent);
                    }

                    this.statusMessage = STRLOG_Initialising;

                    //if ((this.powerupReset == true) || (this.tubeInitAxis == true))
                    //{
                    //    //
                    //    // Find forward limit switch position in encoder counts (4,283,880)
                    //    //
                    //    Logfile.Write(STRLOG_FindingTubeForwardLimit);
                    //    int forwardPosition = 0;
                    //    if (FindTubeForwardLimit(ref forwardPosition) == false)
                    //    {
                    //        throw new ArgumentException(STRERR_FindTubeForwardLimitFailed + this.GetLastError());
                    //    }
                    //}

                    //
                    // Find the reverse limit switch and set tube position to zero
                    //
                    Logfile.Write(STRLOG_FindingTubeReverseLimit);
                    int reversePosition = 0;
                    if (FindTubeReverseLimit(ref reversePosition) == false)
                    {
                        throw new ArgumentException(STRERR_FindTubeReverseLimitFailed + this.GetLastError());
                    }

                    Logfile.Write(STRLOG_ResetingTubePosition);
                    if (ResetTubePosition() == false)
                    {
                        throw new ArgumentException(STRERR_ResetTubePositionFailed + this.GetLastError());
                    }

                    //
                    // Set tube to its home position
                    //
                    Logfile.Write(STRLOG_SettingTubeDistanceToHomePosition);
                    if (SetTubeDistance(this.tubeHomeDistance) == false)
                    {
                        throw new ArgumentException(STRERR_SetTubeDistanceFailed + this.GetLastError());
                    }

                    //
                    // Set source to its home location
                    //
                    Logfile.Write(STRLOG_SettingSourceToHomeLocation);
                    if (SetSourceLocation(this.sourceHomeLocation) == false)
                    {
                        throw new ArgumentException(STRERR_SetSourceLocationFailed + this.GetLastError());
                    }

                    //
                    // Set absorber to its home location
                    //
                    if (this.hasAbsorberTable == true)
                    {
                        Logfile.Write(STRLOG_SettingAbsorberToHomeLocation);
                        if (SetAbsorberLocation(this.absorberHomeLocation) == false)
                        {
                            throw new ArgumentException(STRERR_SetAbsorberLocationFailed + this.GetLastError());
                        }
                    }

                    //
                    // First-time initialisation is complete
                    //
                    this.initialised = true;

                    success = true;
                }
                catch (Exception ex)
                {
                    Logfile.WriteError(ex.Message);
                }
            }

            //
            // Initialisation that must be done each time the equipment is powered up
            //
            try
            {
                success = false;

                //
                // Toggle the counter-start signal to start the radiation counter
                //
                Logfile.Write(STRLOG_StartingRadiationCounter);
                if (this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, true) == false)
                {
                    throw new ArgumentException(STRERR_CounterStartBreakpointFailed + this.GetLastError());
                }
                if (this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false) == false)
                {
                    throw new ArgumentException(STRERR_CounterStartBreakpointFailed + this.GetLastError());
                }

                //
                // Initialisation is complete
                //
                this.online = true;
                this.statusMessage = null;

                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public double GetTubeMoveTime(int startDistance, int endDistance)
        {
            // Get absolute distance
            int distance = endDistance - startDistance;
            if (distance < 0)
            {
                distance = -distance;
            }

            // Tube move rate is in ms per mm
            double seconds = distance * this.tubeMoveRate;

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public double GetSourceSelectTime(char toLocation)
        {
            double seconds = 0;

            int index = toLocation - this.sourceFirstLocation;
            if (index >= 0 && index < this.sourceAxisInfo.selectTimes.Length)
            {
                seconds = this.sourceAxisInfo.selectTimes[index];
            }

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public double GetSourceReturnTime(char fromLocation)
        {
            double seconds = 0;

            int index = fromLocation - this.sourceFirstLocation;
            if (index >= 0 && index < this.sourceAxisInfo.returnTimes.Length)
            {
                seconds = this.sourceAxisInfo.returnTimes[index];
            }

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public double GetAbsorberSelectTime(char toLocation)
        {
            double seconds = 0;

            if (this.hasAbsorberTable == true)
            {
                int index = toLocation - this.absorberFirstLocation;
                if (index >= 0 && index < this.absorberAxisInfo.selectTimes.Length)
                {
                    seconds = this.absorberAxisInfo.selectTimes[index];
                }
            }

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public double GetAbsorberReturnTime(char fromLocation)
        {
            double seconds = 0;

            if (this.hasAbsorberTable == true)
            {
                int index = fromLocation - this.absorberFirstLocation;
                if (index >= 0 && index < this.absorberAxisInfo.returnTimes.Length)
                {
                    if (this.hasAbsorberTable == true)
                    {
                        seconds = this.absorberAxisInfo.returnTimes[index];
                    }
                }
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public bool SetTubeDistance(int targetDistance)
        {
            // Convert target position in millimetres to encoder counts
            int targetPosition = (targetDistance - this.tubeOffsetDistance) * ENCODER_COUNTS_PER_MM;

            // Move tube to target position
            return SetTubePosition(targetPosition);
        }

        //---------------------------------------------------------------------------------------//

        public bool SetSourceLocation(char location)
        {
            const string STRLOG_MethodName = "SetSourceLocation";

            string logMessage = STRLOG_Location + location.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = false;

            int index = location - this.sourceFirstLocation;
            if (index >= 0 && index < this.sourceAxisInfo.encoderPositions.Length)
            {
                //
                // Move source table to specified location
                //
                if ((success = this.SetSourceEncoderPosition(this.sourceAxisInfo.encoderPositions[index])) == false)
                {
                    Logfile.WriteError(this.lastError);
                }
            }
            else
            {
                Logfile.WriteError(STRERR_InvalidLocation + location.ToString());
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        public bool SetAbsorberLocation(char location)
        {
            const string STRLOG_MethodName = "SetAbsorberLocation";

            string logMessage = STRLOG_Location + location.ToString();

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            bool success = false;

            if (this.hasAbsorberTable == true)
            {
                int index = location - this.absorberFirstLocation;
                if (index >= 0 && index < this.absorberAxisInfo.encoderPositions.Length)
                {
                    if (this.hasAbsorberTable == true)
                    {
                        //
                        // Move absorber table to specified location
                        //
                        if ((success = this.SetAbsorberEncoderPosition(this.absorberAxisInfo.encoderPositions[index])) == false)
                        {
                            Logfile.WriteError(this.lastError);
                        }
                    }
                    else
                    {
                        Logfile.WriteError(STRERR_InvalidLocation + location.ToString());
                    }
                }
            }
            else
            {
                success = true;
            }

            logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //=======================================================================================//

        private bool InitialiseController()
        {
            int err = 0;
            ushort csr = 0;
            int state = 0;

            try
            {
                ClearErrors();

                for (; ; )
                {
                    switch (state)
                    {
                        case 0:
                            // Get communication status register
                            err = flex_read_csr_rtn(boardID, ref csr);
                            break;

                        case 1:
                            // Check if the board is in power up reset condition
                            if ((csr & Nimc.POWER_UP_RESET) != 0)
                            {
                                err = flex_initialize_controller(boardID, null);

                                // Tube axis must be initialised after powerup reset
                                this.powerupReset = true;
                            }
                            break;

                        case 2:
                            // Inhibit the tube axis motor
                            err = flex_stop_motion(this.boardID, this.tubeAxis, Nimc.KILL_STOP, 0);
                            break;

                        case 3:
                            // Inhibit the source table motor
                            err = flex_stop_motion(this.boardID, this.sourceAxis, Nimc.KILL_STOP, 0);
                            break;

                        case 4:
                            if (this.hasAbsorberTable == true)
                            {
                                // Inhibit the absorber table motor
                                err = flex_stop_motion(this.boardID, this.absorberAxis, Nimc.KILL_STOP, 0);
                            }
                            break;

                        case 5:
                            // Initialisation successful
                            return true;
                    }

                    // Check for errors
                    if (err != 0)
                    {
                        // Initialisation failed
                        ProcessError(boardID, err);
                        return false;
                    }

                    // Next state
                    state++;
                }
            }
            catch
            {
                return false;
            }
        }

        //---------------------------------------------------------------------------------------//

        private bool SetSourceEncoderPosition(int targetPosition)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            ushort csr = 0;
            ushort found = 0;
            ushort finding = 0;
            int position = 0;
            ushort axisStatus = 0;
            int state = 0;

            while (true)
            {
                switch (state)
                {
                    case 0:
                        err = flex_find_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << sourceAxis),
                            Nimc.FIND_HOME_REFERENCE);
                        break;

                    case 1:
                        err = flex_check_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << sourceAxis),
                            ref found, ref finding);
                        break;

                    case 2:
                        if (finding != 0)
                        {
                            state = 1;
                            continue;
                        }
                        break;

                    case 3:
                        err = flex_read_pos_rtn(boardID, sourceAxis, ref position);
                        break;

                    case 4:
                        err = flex_reset_pos(boardID, sourceAxis, 0, 0, 0xFF);
                        break;

                    case 5:
                        err = flex_read_pos_rtn(boardID, sourceAxis, ref position);
                        break;

                    case 6:
                        err = flex_load_target_pos(boardID, sourceAxis, targetPosition, 0xFF);
                        break;

                    case 7:
                        err = flex_start(boardID, sourceAxis, 0);
                        break;

                    case 8:
                        err = flex_read_pos_rtn(boardID, sourceAxis, ref position);
                        break;

                    case 9:
                        err = flex_read_axis_status_rtn(boardID, sourceAxis, ref axisStatus);
                        break;

                    case 10:
                        // Check the modal errors
                        err = flex_read_csr_rtn(boardID, ref csr);
                        break;

                    case 11:
                        if ((csr & Nimc.MODAL_ERROR_MSG) != 0)
                        {
                            // Stop the Motion
                            flex_stop_motion(boardID, sourceAxis, Nimc.DECEL_STOP, 0);
                            err = (short)(csr & Nimc.MODAL_ERROR_MSG);
                        }
                        break;

                    case 12:
                        // Test against the move complete bit
                        if ((axisStatus & (Nimc.MOVE_COMPLETE_BIT | Nimc.AXIS_OFF_BIT)) == 0)
                        {
                            // Not finished yet
                            state = 8;
                            continue;
                        }
                        break;

                    case 13:
                        // Inhibit the motor
                        err = flex_stop_motion(this.boardID, this.sourceAxis, Nimc.KILL_STOP, 0);
                        break;

                    case 14:
                        // Successful
                        return (true);
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);

                    Logfile.WriteError("state: " + state.ToString() + "  err: " + err.ToString());

                    return (false);
                }

                state++;
            }
        }

        //---------------------------------------------------------------------------------------//

        private bool SetAbsorberEncoderPosition(int targetPosition)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            ushort csr = 0;
            ushort found = 0;
            ushort finding = 0;
            int position = 0;
            ushort axisStatus = 0;
            int state = 0;

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        err = flex_find_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << absorberAxis),
                            Nimc.FIND_HOME_REFERENCE);
                        break;

                    case 1:
                        err = flex_check_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << absorberAxis),
                            ref found, ref finding);
                        break;

                    case 2:
                        if (finding != 0)
                        {
                            state = 1;
                            continue;
                        }
                        break;

                    case 3:
                        err = flex_read_pos_rtn(boardID, absorberAxis, ref position);
                        break;

                    case 4:
                        err = flex_reset_pos(boardID, absorberAxis, 0, 0, 0xFF);
                        break;

                    case 5:
                        err = flex_read_pos_rtn(boardID, absorberAxis, ref position);
                        break;

                    case 6:
                        err = flex_load_target_pos(boardID, absorberAxis, targetPosition, 0xFF);
                        break;

                    case 7:
                        err = flex_start(boardID, absorberAxis, 0);
                        break;

                    case 8:
                        err = flex_read_pos_rtn(boardID, absorberAxis, ref position);
                        break;

                    case 9:
                        err = flex_read_axis_status_rtn(boardID, absorberAxis, ref axisStatus);
                        break;

                    case 10:
                        // Check the modal errors
                        err = flex_read_csr_rtn(boardID, ref csr);
                        break;

                    case 11:
                        if ((csr & Nimc.MODAL_ERROR_MSG) != 0)
                        {
                            // Stop the Motion
                            flex_stop_motion(boardID, absorberAxis, Nimc.DECEL_STOP, 0);
                            err = (short)(csr & Nimc.MODAL_ERROR_MSG);
                        }
                        break;

                    case 12:
                        // Test against the move complete bit
                        if ((axisStatus & (Nimc.MOVE_COMPLETE_BIT | Nimc.AXIS_OFF_BIT)) == 0)
                        {
                            // Not finished yet
                            state = 8;
                            continue;
                        }
                        break;

                    case 13:
                        // Inhibit the motor
                        err = flex_stop_motion(this.boardID, this.absorberAxis, Nimc.KILL_STOP, 0);
                        break;

                    case 14:
                        // Successful
                        return (true);
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return (false);
                }

                state++;
            }
        }

        //---------------------------------------------------------------------------------------//

        private bool FindTubeForwardLimit(ref int position)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            ushort found = 0;
            ushort finding = 0;
            int state = 0;

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        // Find the reverse limit switch
                        err = flex_find_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << tubeAxis),
                            Nimc.FIND_FORWARD_LIMIT_REFERENCE);
                        break;

                    case 1:
                        err = flex_check_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << tubeAxis),
                            ref found, ref finding);
                        break;

                    case 2:
                        if (finding != 0)
                        {
                            state = 1;
                            continue;
                        }
                        break;

                    case 3:
                        // Read the current position of the tube
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 4:
                        // Inhibit the motor
                        err = flex_stop_motion(boardID, tubeAxis, Nimc.KILL_STOP, 0);
                        break;

                    case 5:
                        // Successful
                        return true;
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return false;
                }

                // Next state
                state++;

            }
        }

        //---------------------------------------------------------------------------------------//

        private bool FindTubeReverseLimit(ref int position)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            ushort found = 0;
            ushort finding = 0;
            int state = 0;

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        // Find the reverse limit switch
                        err = flex_find_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << tubeAxis),
                            Nimc.FIND_REVERSE_LIMIT_REFERENCE);
                        break;

                    case 1:
                        err = flex_check_reference(boardID, Nimc.AXIS_CTRL, (ushort)(1 << tubeAxis),
                            ref found, ref finding);
                        break;

                    case 2:
                        if (finding != 0)
                        {
                            state = 1;
                            continue;
                        }
                        break;

                    case 3:
                        // Read the current position of the tube
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 4:
                        // Inhibit the motor
                        err = flex_stop_motion(boardID, tubeAxis, Nimc.KILL_STOP, 0);
                        break;

                    case 5:
                        // Successful
                        return true;
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return false;
                }

                // Next state
                state++;

            }
        }

        //---------------------------------------------------------------------------------------//

        private bool ResetTubePosition()
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            int position = 0;
            int state = 0;

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        // Read the position of the reverse limit switch
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 1:
                        // Reset the position to 0
                        err = flex_reset_pos(boardID, tubeAxis, 0, 0, 0xFF);
                        break;

                    case 2:
                        // Read the position again
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 3:
                        // Successful
                        return true;
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return false;
                }

                // Next state
                state++;
            }
        }

        //---------------------------------------------------------------------------------------//

        private bool GetTubePosition(ref int position)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            int state = 0;

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        // Read the current position of the tube
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 1:
                        // Successful
                        return true;
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return false;
                }

                // Next state
                state++;
            }
        }

        //---------------------------------------------------------------------------------------//

        private bool SetTubePosition(int targetPosition)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            ushort csr = 0;
            int position = 0;
            ushort axisStatus = 0;
            int state = 0;

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        err = flex_set_op_mode(boardID, tubeAxis, Nimc.ABSOLUTE_POSITION);
                        break;

                    case 1:
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 2:
                        err = flex_load_target_pos(boardID, tubeAxis, targetPosition, 0xFF);
                        break;

                    case 3:
                        err = flex_start(boardID, tubeAxis, 0);
                        break;

                    case 4:
                        err = flex_read_pos_rtn(boardID, tubeAxis, ref position);
                        break;

                    case 5:
                        err = flex_read_axis_status_rtn(boardID, tubeAxis, ref axisStatus);
                        break;

                    case 6:
                        // Check the modal errors
                        err = flex_read_csr_rtn(boardID, ref csr);
                        break;

                    case 7:
                        if ((csr & Nimc.MODAL_ERROR_MSG) != 0)
                        {
                            // Stop the Motion
                            err = flex_stop_motion(boardID, tubeAxis, Nimc.DECEL_STOP, 0);
                        }
                        break;

                    case 8:
                        // Test against the move complete bit
                        if ((axisStatus & (Nimc.MOVE_COMPLETE_BIT | Nimc.AXIS_OFF_BIT)) == 0)
                        {
                            // Not finished yet
                            state = 4;
                            continue;
                        }

                        // Inhibit the motor
                        err = flex_stop_motion(boardID, tubeAxis, Nimc.KILL_STOP, 0);
                        break;

                    case 9:
                        // Successful
                        return true;
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return false;
                }

                state++;
            }
        }

        //---------------------------------------------------------------------------------------//

        private bool SetBreakpoint(byte boardID, byte axis, bool enable)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                this.lastError = STRERR_FlexMotionBoardNotPresent;
                return false;
            }

            int err = 0;
            int state = 0;

            ushort muston = 0, mustoff = 0;

            if (enable)
            {
                mustoff = (ushort)(1 << axis);
            }
            else
            {
                muston = (ushort)(1 << axis);
            }

            for (; ; )
            {
                switch (state)
                {
                    case 0:
                        // Disable breakpoint to allow direct control of I/O
                        err = flex_enable_breakpoint(boardID, axis, 0);
                        break;

                    case 1:
                        err = flex_set_breakpoint_output_momo(boardID, axis, muston, mustoff, 0xFF);
                        break;

                    case 2:
                        err = flex_set_breakpoint_output_momo(boardID, axis, 0, 0, 0xFF);
                        break;

                    case 3:
                        // Successful
                        return true;
                }

                // Check for errors
                if (err != 0)
                {
                    // Failed
                    ProcessError(boardID, err);
                    return false;
                }

                // Next state
                state++;
            }
        }

        //---------------------------------------------------------------------------------------//

        private void ClearErrors()
        {
            int err = 0;
            ushort csr = 0;
            ushort commandID = 0;
            ushort resourceID = 0;
            int errorCode = 0;

            try
            {
                for (;;)
                {
                    err = flex_read_csr_rtn(boardID, ref csr);
                    if ((csr & Nimc.MODAL_ERROR_MSG) == 0)
                    {
                        return;
                    }

                    flex_read_error_msg_rtn(boardID, ref commandID, ref resourceID, ref errorCode);
                }
            }
            catch
            {
                throw new Exception("FlexMotion controller access failed");
            }
        }

        //---------------------------------------------------------------------------------------//

        private void ProcessError(byte boardID, int error)
        {
            int err = 0;
            ushort csr = 0;
            ushort commandID = 0;
            ushort resourceID = 0;
            int errorCode = 0;

            err = flex_read_csr_rtn(boardID, ref csr);
            if ((csr & Nimc.MODAL_ERROR_MSG) != 0)
            {
                do
                {
                    //
                    // Get the command ID, resource and the error code of
                    // the modal error from the error stack on the board.
                    //
                    err = flex_read_error_msg_rtn(boardID, ref commandID, ref resourceID, ref errorCode);
                    this.lastError = GetErrorDescription(errorCode, commandID, resourceID);

                    err = flex_read_csr_rtn(boardID, ref csr);
                } while ((csr & Nimc.MODAL_ERROR_MSG) != 0);
            }
            else
            {
                lastError = GetErrorDescription(error, 0, 0);
            }
        }

        //---------------------------------------------------------------------------------------//

        private string GetErrorDescription(int errorCode, ushort commandID, ushort resourceID)
        {
            char[] errorDescription = null;
            int sizeOfArray = 0;
            ushort descriptionType;

            descriptionType = (commandID == 0) ? Nimc.ERROR_ONLY : Nimc.COMBINED_DESCRIPTION;

            // First, get the size for the error description
            flex_get_error_description(descriptionType, errorCode, commandID, resourceID,
                                errorDescription, ref sizeOfArray);

            // sizeOfArray is size of description + NULL character
            sizeOfArray++;

            // Allocate char array for the description
            errorDescription = new char[sizeOfArray];

            // Get error description
            flex_get_error_description(descriptionType, errorCode, commandID, resourceID,
                                    errorDescription, ref sizeOfArray);

            return new string(errorDescription);
        }

    }
}
