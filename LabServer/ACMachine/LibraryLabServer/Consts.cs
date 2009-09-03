using System;

namespace Library.LabServer
{
    public class Consts
    {
        //
        // Configuration
        //
        public const string STRXML_SetupId_LockedRotor = "LockedRotor";
        public const string STRXML_SetupId_NoLoad = "NoLoad";
        public const string STRXML_SetupId_SynchronousSpeed = "SynchronousSpeed";
        public const string STRXML_SetupId_FullLoad = "FullLoad";

        //
        // ExperimentResult
        //
        public const string STRXML_voltage = "voltage";
        public const string STRXML_current = "current";
        public const string STRXML_powerFactor = "powerFactor";
        public const string STRXML_speed = "speed";

        //
        // Application configuration file key strings
        //
        public const string STRCFG_MachineIP = "MachineIP";
        public const string STRCFG_MachinePort = "MachinePort";
        public const string STRCFG_MeasurementCount = "MeasurementCount";
        public const string STRCFG_MeasurementDelay = "MeasurementDelay";

    }
}
