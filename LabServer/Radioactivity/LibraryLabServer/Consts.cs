using System;

namespace Library.LabServer
{
    public class Consts
    {
        //
        // Application configuration file key strings
        //
        public const string StrCfg_RadioactivityService = "RadioactivityService";
        public const string StrCfg_RadioactivityPasskey = "RadioactivityPasskey";
        public const string StrCfg_SimulationParameters = "SimulationParameters";
        public const string StrCfg_SimulateDelays = "SimulateDelays";

        //
        // XML Configuration
        //
        public const string STRXML_SetupId_RadioactivityVsTime = "RadioactivityVsTime";
        public const string STRXML_SetupId_RadioactivityVsDistance = "RadioactivityVsDistance";
        public const string STRXML_SetupId_SimActivityVsTime = "SimActivityVsTime";
        public const string STRXML_SetupId_SimActivityVsDistance = "SimActivityVsDistance";

        //
        // XML Specification and ExperimentResult
        //
        public const string STRXML_sourceName = "sourceName";
        public const string STRXML_absorberName = "absorberName";
        public const string STRXML_distance = "distance";
        public const string STRXML_duration = "duration";
        public const string STRXML_repeat = "repeat";
        public const char CHR_CsvSplitter = ',';

        //
        // XML Validation
        //
        public const string STRXML_vdnDistance = "vdnDistance";
        public const string STRXML_vdnDuration = "vdnDuration";
        public const string STRXML_vdnRepeat = "vdnRepeat";
        public const string STRXML_vdnTotaltime = "vdnTotaltime";
        public const string STRXML_minimum = "minimum";
        public const string STRXML_maximum = "maximum";

        //
        // XML ExperimentResult
        //
        public const string STRXML_dataVector = "dataVector";
    }
}
