using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace LabEquipment.Drivers
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

        // Power-up and power-down delays in seconds
        const int DELAY_POWERUP = 5;
        const int DELAY_INITIALISE = 17;
        //const int DELAY_INITIALISE = 30;

        // Source table encoder counts for positions A thru F
        private const string STRCSV_SourceEncoderPositions = "75200,100,10900,32450,43100,59150";

        //
        // Source table select and return times (secs) for positions A thru F
        //
        private const string STRCSV_SourceSelectTimes = "14,4,6,8,10,12";
        private const string STRCSV_SourceReturnTimes = "10,19,17,15,14,12";
        private const char CHR_SourceIndexStart = 'A';
        private const char CHR_SourceHomeLocation = 'F';

        // Absorber table encoder counts - ony one position is valid (None)
        private const string STRCSV_AbsorberEncoderPositions = "0";

        //
        // Absorber table select and return times (secs) for position A
        //
        private const string STRCSV_AbsorberSelectTimes = "0";
        private const string STRCSV_AbsorberReturnTimes = "0";
        private const char CHR_AbsorberIndexStart = 'A';
        private const char CHR_AbsorberHomeLocation = 'A';

        // Comma-seperated value string splitter character
        private const char CHR_CsvSplitterChar = ',';

        // Rate at which the Geiger tube moves (milleseconds per millimeter)
        private const int TUBE_MOVE_RATE_MS_PER_MM = 520;

        // Tube axis encoder counts per mm distance moved
        private const int ENCODER_COUNTS_PER_MM = 43000;

        //
        // Local variables
        //
        private bool isPresent;
        private bool initialised;
        private byte boardID;
        private byte tubeAxis;
        private byte sourceAxis;
        private byte absorberAxis;
        private byte powerEnableBreakpoint;
        private byte counterStartBreakpoint;
        private bool powerupReset;

        private struct AxisInfo
        {
            public int[] encoderPositions;
            public int[] selectTimes;
            public int[] returnTimes;
        }

        private AxisInfo sourceAxisInfo;
        private AxisInfo absorberAxisInfo;

        #endregion

        #region Properties

        private int tubeOffsetDistance;
        private int tubeHomeDistance;
        private char sourceHomeLocation;
        private char absorberHomeLocation;
        private bool initTubeAxis;
        private string lastError;

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
            get { return (this.initialised == false) ? DELAY_INITIALISE : 0; }
        }

        public int TubeHomeDistance
        {
            get { return this.tubeHomeDistance; }
            set { this.tubeHomeDistance = value; }
        }

        public char SourceHomeLocation
        {
            get { return this.sourceHomeLocation; }
        }

        public char AbsorberHomeLocation
        {
            get { return this.absorberHomeLocation; }
        }

        public string LastError
        {
            get
            {
                string errorMsg = this.lastError;
                this.lastError = null;
                return errorMsg;
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public FlexMotion(int boardID, int tubeOffsetDistance, int tubeHomeDistance, bool initTubeAxis)
        {
            //
            // Initialise local variables
            //
            this.isPresent = false;
            this.initialised = false;
            this.boardID = (byte)boardID;
            this.tubeAxis = Nimc.AXIS1;
            this.powerEnableBreakpoint = this.tubeAxis;
            this.sourceAxis = Nimc.AXIS2;
            this.counterStartBreakpoint = this.sourceAxis;
            this.absorberAxis = Nimc.AXIS3;
            this.tubeOffsetDistance = tubeOffsetDistance;
            this.tubeHomeDistance = tubeHomeDistance;
            this.sourceHomeLocation = CHR_SourceHomeLocation;
            this.absorberHomeLocation = CHR_AbsorberHomeLocation;
            this.initTubeAxis = initTubeAxis;

            //
            // Initialise properties
            //
            this.lastError = null;
            this.powerupReset = false;

            //
            // Initialise source axis info
            //
            this.sourceAxisInfo = new AxisInfo();

            //
            // Initialise source encoder positions array
            //
            string[] strSplit = STRCSV_SourceEncoderPositions.Split(new char[] { CHR_CsvSplitterChar });
            this.sourceAxisInfo.encoderPositions = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.encoderPositions[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise source select times array
            //
            strSplit = STRCSV_SourceSelectTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.sourceAxisInfo.selectTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.selectTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise source return times array
            //
            strSplit = STRCSV_SourceReturnTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.sourceAxisInfo.returnTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.sourceAxisInfo.returnTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise absorber axis info
            //
            this.absorberAxisInfo = new AxisInfo();

            //
            // Initialise absorber encoder positions array
            //
            strSplit = STRCSV_AbsorberEncoderPositions.Split(new char[] { CHR_CsvSplitterChar });
            this.absorberAxisInfo.encoderPositions = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.absorberAxisInfo.encoderPositions[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise absorber select times array
            //
            strSplit = STRCSV_AbsorberSelectTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.absorberAxisInfo.selectTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.absorberAxisInfo.selectTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise absorber return times array
            //
            strSplit = STRCSV_AbsorberReturnTimes.Split(new char[] { CHR_CsvSplitterChar });
            this.absorberAxisInfo.returnTimes = new int[strSplit.Length];
            for (int i = 0; i < strSplit.Length; i++)
            {
                this.absorberAxisInfo.returnTimes[i] = Int32.Parse(strSplit[i]);
            }

            //
            // Initialise the Flexmotion controller card. Must be done here because a breakpoint
            // on the controller card is used to powerup the equipment and initialisation is
            // carried out after the equipment is powered up.
            //
            this.isPresent = InitialiseController();
        }

        //---------------------------------------------------------------------------------------//

        public bool EnablePower()
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                return false;
            }

            //
            // Ensure power-enable and counter-start signals are inactive
            //
            this.SetBreakpoint(this.boardID, this.powerEnableBreakpoint, false);
            this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false);

            //
            // Toggle the counter-start signal to enable both signals
            //
            this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, true);
            this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false);

            //
            // Enable the power
            //
            this.SetBreakpoint(this.boardID, this.powerEnableBreakpoint, true);

            return true;
        }

        //---------------------------------------------------------------------------------------//

        public bool DisablePower()
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                return false;
            }

            //
            // Make the counter-start and power-enable signals inactive
            //
            this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false);
            this.SetBreakpoint(this.boardID, this.powerEnableBreakpoint, false);

            return true;
        }

        //---------------------------------------------------------------------------------------//

        public bool Initialise()
        {
#if SIMULATE_FLEXMOTION
            for (int i = 0; i < this.InitialiseDelay; i++)
            {
                Trace.Write("i");
                Thread.Sleep(1000);
            }
            Trace.WriteLine("");
#else
            //
            // Initialise geiger tube axis
            //
            if (this.initialised == false)
            {
                if (this.isPresent == true)
                {
                    bool success = true;

                    if ((this.powerupReset == true) || (this.initTubeAxis == true))
                    {
                        int reversePosition = 0;

                        //
                        // Find the reverse limit switch and set tube position to zero
                        //
                        if ((FindTubeReverseLimit(ref reversePosition) == false) ||
                            (ResetTubePosition() == false))
                        {
                            success = false;
                        }

                        //
                        // ***Debugging info only, uncomment and recompile to run***
                        // Find forward limit switch position in encoder counts (4,283,880)
                        //
                        //int forwardPosition = 0;
                        //if (FindTubeForwardLimit(ref forwardPosition) == true)
                        //{
                        //    FindTubeReverseLimit(ref reversePosition);
                        //}
                    }

                    //
                    // Set tube, absorber and source to their home positions
                    //
                    if (success == true)
                    {
                        success = SetTubeDistance(this.tubeHomeDistance);
                    }
                    if (success == true)
                    {
                        success = SetSourceLocation(this.sourceHomeLocation);
                    }
                    if (success == true)
                    {
                        success = SetAbsorberLocation(this.absorberHomeLocation);
                    }
                }

                this.initialised = true;
            }

            //
            // Toggle the counter-start signal to start the radiation counter
            //
            this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, true);
            this.SetBreakpoint(this.boardID, this.counterStartBreakpoint, false);
#endif

            return this.isPresent;
        }

        //---------------------------------------------------------------------------------------//

        public int GetTubeMoveTime(int startDistance, int endDistance)
        {
            // Get absolute distance
            int distance = endDistance - startDistance;
            if (distance < 0)
            {
                distance *= -1;
            }

            int seconds = (distance * TUBE_MOVE_RATE_MS_PER_MM) / 1000 + 1;

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public int GetSourceSelectTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_SourceIndexStart;
            if (index >= 0 && index < this.sourceAxisInfo.selectTimes.Length)
            {
                seconds = this.sourceAxisInfo.selectTimes[index] + 1;
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public int GetSourceReturnTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_SourceIndexStart;
            if (index >= 0 && index < this.sourceAxisInfo.returnTimes.Length)
            {
                seconds = this.sourceAxisInfo.returnTimes[index] + 1;
            }

            return seconds;
        }

        //-------------------------------------------------------------------------------------------------//

        public int GetAbsorberSelectTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_AbsorberHomeLocation;
            if (index >= 0 && index < this.absorberAxisInfo.selectTimes.Length)
            {
                seconds = this.absorberAxisInfo.selectTimes[index] + 1;
            }

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public int GetAbsorberReturnTime(char location)
        {
            int seconds = 0;

            int index = location - CHR_AbsorberHomeLocation;
            if (index >= 0 && index < this.absorberAxisInfo.returnTimes.Length)
            {
                seconds = this.absorberAxisInfo.returnTimes[index] + 1;
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
            bool ok = true;

            int index = location - CHR_SourceIndexStart;
            if (index >= 0 && index < this.sourceAxisInfo.encoderPositions.Length)
            {
                // Move source table to position
                ok = this.SetSourceEncoderPosition(this.sourceAxisInfo.encoderPositions[index]);
            }

            return ok;
        }

        //---------------------------------------------------------------------------------------//

        public bool SetAbsorberLocation(char location)
        {
            bool ok = true;

            int index = location - CHR_AbsorberHomeLocation;
            if (index >= 0 && index < this.absorberAxisInfo.encoderPositions.Length)
            {
                // Move absorber table to position
                ok = this.SetAbsorberEncoderPosition(this.absorberAxisInfo.encoderPositions[index]);
            }

            return ok;
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

        public bool InitSourcePosition()
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                return false;
            }

            int err = 0;
            ushort found = 0;
            ushort finding = 0;
            int position = 0;
            int state = 0;

            for (;;)
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
                        // Inhibit the motor
                        err = flex_stop_motion(boardID, sourceAxis, Nimc.KILL_STOP, 0);
                        break;
                    case 12:
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

        private bool SetSourceEncoderPosition(int targetPosition)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                return false;
            }

            int err = 0;
            ushort csr = 0;
            ushort found = 0;
            ushort finding = 0;
            int position = 0;
            ushort axisStatus = 0;
            int state = 0;

            for (;;)
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

        private bool SetAbsorberEncoderPosition(int targetPosition)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
                return false;
            }

            return true;
        }

        //---------------------------------------------------------------------------------------//

        private bool FindTubeForwardLimit(ref int position)
        {
            //
            // Make sure flexmotion controller is present
            //
            if (this.isPresent == false)
            {
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
                    lastError = GetErrorDescription(errorCode, commandID, resourceID);

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
