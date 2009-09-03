using System;

namespace Library.LabServer
{
    public class Consts
    {
        //
        // Configuration
        //
        public const string STRXML_SetupId_VoltageVsSpeed = "VoltageVsSpeed";
        public const string STRXML_SetupId_VoltageVsField = "VoltageVsField";
        public const string STRXML_SetupId_VoltageVsLoad = "VoltageVsLoad";
        public const string STRXML_SetupId_SpeedVsVoltage = "SpeedVsVoltage";
        public const string STRXML_SetupId_SpeedVsField = "SpeedVsField";

        //
        // Validation
        //
        public const string STRXML_vdnSpeed = "vdnSpeed";
        public const string STRXML_vdnField = "vdnField";
        public const string STRXML_vdnLoad = "vdnLoad";
        public const string STRXML_minimum = "minimum";
        public const string STRXML_maximum = "maximum";
        public const string STRXML_stepMin = "stepMin";
        public const string STRXML_stepMax = "stepMax";

        //
        // Specification and ExperimentResult
        //
        public const string STRXML_speedMin = "speedMin";
        public const string STRXML_speedMax = "speedMax";
        public const string STRXML_speedStep = "speedStep";
        public const string STRXML_fieldMin = "fieldMin";
        public const string STRXML_fieldMax = "fieldMax";
        public const string STRXML_fieldStep = "fieldStep";
        public const string STRXML_loadMin = "loadMin";
        public const string STRXML_loadMax = "loadMax";
        public const string STRXML_loadStep = "loadStep";

        //
        // ExperimentResult
        //
        public const string STRXML_speedVector = "speedVector";
        public const string STRXML_fieldVector = "fieldVector";
        public const string STRXML_voltageVector = "voltageVector";
        public const string STRXML_loadVector = "loadVector";
        public const char CHR_Splitter = ',';

        //
        // Application configuration file key strings
        //
        public const string STRCFG_MachineIP = "MachineIP";
        public const string STRCFG_MachinePort = "MachinePort";
        public const string STRCFG_MeasurementCount = "MeasurementCount";
        public const string STRCFG_MeasurementDelay = "MeasurementDelay";
    }
}
