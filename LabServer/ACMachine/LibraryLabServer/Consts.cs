using System;

namespace Library.LabServer
{
    public class Consts
    {
        //
        // Application configuration file key strings
        //

        //
        // XML Configuration
        //
        public const string STRXML_SetupId_LockedRotor = "LockedRotor";
        public const string STRXML_SetupId_NoLoad = "NoLoad";
        public const string STRXML_SetupId_SynchronousSpeed = "SynchronousSpeed";
        public const string STRXML_SetupId_FullLoad = "FullLoad";
        public const string STRXML_measurementCount = "measurementCount";

        //
        // XML Specification and ExperimentResult
        //

        //
        // XML Validation
        //

        //
        // XML elements for the commands in the equipment request strings
        //
        public const string STRXML_CmdGetResetACDriveTime = "GetResetACDriveTime";
        public const string STRXML_CmdGetStartACDriveTime = "GetStartACDriveTime";
        public const string STRXML_CmdGetStopACDriveTime = "GetStopACDriveTime";
        public const string STRXML_CmdGetTakeMeasurementTime = "GetTakeMeasurementTime";
        public const string STRXML_CmdCreateConnection = "CreateConnection";
        public const string STRXML_CmdCloseConnection = "CloseConnection";
        public const string STRXML_CmdResetACDrive = "ResetACDrive";
        public const string STRXML_CmdStartACDrive = "StartACDrive";
        public const string STRXML_CmdStopACDrive = "StopACDrive";
        public const string STRXML_CmdTakeMeasurement = "TakeMeasurement";

        //
        // XML elements for the command arguments in the equipment request strings
        //
        public const string STRXML_ReqACDriveMode = "ACDriveMode";

        //
        // XML elements in the equipment response strings
        //
        public const string STRXML_RspResetACDriveTime = "ResetACDriveTime";
        public const string STRXML_RspStartACDriveTime = "StartACDriveTime";
        public const string STRXML_RspStopACDriveTime = "StopACDriveTime";
        public const string STRXML_RspTakeMeasurementTime = "TakeMeasurementTime";
        public const string STRXML_RspVoltage = "Voltage";
        public const string STRXML_RspCurrent = "Current";
        public const string STRXML_RspPowerFactor = "PowerFactor";
        public const string STRXML_RspSpeed = "Speed";
        public const string STRXML_RspTorque = "Torque";

        //
        // XML ExperimentResult
        //
        public const string STRXML_voltage = "voltage";
        public const string STRXML_current = "current";
        public const string STRXML_powerFactor = "powerFactor";
        public const string STRXML_speed = "speed";

        //
        // String constants
        //

    }
}
