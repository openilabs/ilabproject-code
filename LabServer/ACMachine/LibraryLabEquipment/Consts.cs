using System;

namespace Library.LabEquipment
{
    public class Consts
    {
        //
        // XML elements in the equipment request strings
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
        // XML elements in the EquipmentConfig.xml file
        //
        public const string STRXML_machineIP = "machineIP";
        public const string STRXML_machinePort = "machinePort";
        public const string STRXML_initialiseEquipment = "initialiseEquipment";
        public const string STRXML_measurementDelay = "measurementDelay";

    }
}
