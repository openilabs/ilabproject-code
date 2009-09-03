using System;

namespace Library.LabServerEngine
{
    public class Consts
    {
        //
        // Application configuration file key strings
        //
        public const string STRCFG_LabServerGuid = "LabServerGuid";
        public const string STRCFG_RootFilePath = "RootFilePath";
        public const string STRCFG_LogFilesPath = "LogFilesPath";
        public const string STRCFG_ResultsPath = "ResultsPath";
        public const string STRCFG_FarmSize = "FarmSize";
        public const string STRCFG_XmlLabConfigurationFilename = "XmlLabConfigurationFilename";
        public const string STRCFG_AllowedCaller = "AllowedCaller";
        public const string STRCFG_AuthenticateCaller = "AuthenticateCaller";
        public const string STRCFG_LogCallerIdPasskey = "LogCallerIdPasskey";

        //
        // ServiceBroker name to use when caller authentication is set to false. Used when
        // developing and running the LabServer
        public const string STR_SbNameLocalHost = "localhost";

        // Config file string splitter character
        public const char CHRCFG_SplitterChr = ',';

        //
        // XML elements in the LabConfiguration.xml file
        //
        public const string STRXML_labConfiguration = "labConfiguration";
        public const string STRXMLPARAM_title = "@title";
        public const string STRXMLPARAM_version = "@version";
        public const string STRXML_configuration = "configuration";
        public const string STRXML_setup = "setup";
        public const string STRXMLPARAM_id = "@id";
        public const string STRXML_name = "name";
        public const string STRXML_description = "description";

        public const string STRXML_experimentSpecification = "experimentSpecification";
        public const string STRXML_setupId = "setupId";

        public const string STRXML_experimentResult = "experimentResult";
        public const string STRXML_timestamp = "timestamp";
        public const string STRXML_title = "title";
        public const string STRXML_version = "version";
        public const string STRXML_experimentId = "experimentId";
        public const string STRXML_unitId = "unitId";
        public const string STRXML_setupName = "setupName";

        public const string STRXML_validation = "validation";

        public const string STRXML_simulation = "simulation";

        public const string STRXML_experimentID = "experimentID";
        public const string STRXML_sbName = "sbName";

        public const string STR_XmlFileExtension = ".xml";
    }
}
