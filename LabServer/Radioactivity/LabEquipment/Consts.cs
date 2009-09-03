using System;

namespace LabEquipment
{
    public class Consts
    {
        //
        // Application configuration file key strings
        //
        public const string StrCfg_LabEquipmentTitle = "LabEquipmentTitle";
        public const string StrCfg_FlexMotionBoardID = "FlexMotionBoardID";
        public const string StrCfg_SerialPort = "SerialPort";
        public const string StrCfg_BaudRate = "BaudRate";
        public const string StrCfg_TubeOffsetDistance = "TubeOffsetDistance";
        public const string StrCfg_TubeHomeDistance = "TubeHomeDistance";
        public const string StrCfg_InitTubeAxis = "InitTubeAxis";

        public const string StrCfg_AllowedCaller = "AllowedCaller";
        public const string StrCfg_AuthenticateCaller = "AuthenticateCaller";
        public const string StrCfg_LogCallerIdPasskey = "LogCallerIdPasskey";

        // Config file string splitter character
        public const char ChrCfg_SplitterChr = ',';
    }
}
