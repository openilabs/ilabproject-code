using System;

namespace Library.LabServerEngine
{
    public class Consts
    {
        //
        // Application configuration file key strings
        //
        public const string STRCFG_LabServerGuid = "LabServerGuid";
        public const string STRCFG_LogFilesPath = "LogFilesPath";
        public const string STRCFG_XmlLabConfigurationFilename = "XmlLabConfigurationFilename";
        public const string STRCFG_AllowedCaller = "AllowedCaller";
        public const string STRCFG_AuthenticateCaller = "AuthenticateCaller";
        public const string STRCFG_LogCallerIdPasskey = "LogCallerIdPasskey";
        public const string STRCFG_SqlConnection = "SqlConnection";
        public const string STRCFG_FarmSize = "FarmSize";
        public const string STRCFG_EquipmentService = "EquipmentService";

        //
        // ServiceBroker name to use when caller authentication is set to false. Used when
        // developing and running the LabServer
        public const string STR_SbNameLocalHost = "localhost";

        //
        // Application configuration file key strings for LabServer administration
        //

        // Comma-seperated-value string splitter character
        public const char CHR_CsvSplitterChar = ',';

        //
        // XML elements in the LabConfiguration.xml file
        //
        public const string STRXML_labConfiguration = "labConfiguration";
        public const string STRXMLPARAM_title = "@title";
        public const string STRXMLPARAM_version = "@version";
        public const string STRXML_navmenuPhoto_image = "navmenuPhoto/image";
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

        //
        // XML elements in the equipment request and response strings
        //
        public const string STRXML_Request = "Request";
        public const string STRXML_Command = "Command";
        public const string STRXML_Response = "Response";
        public const string STRXML_RspSuccess = "Success";
        public const string STRXML_RspErrorMessage = "ErrorMessage";

        //
        // Results XML download response
        //
        public const string StrRsp_ContentType_TextXml = "text/xml";
        public const string StrRsp_Disposition = "content-disposition";
        public const string StrRsp_Attachment_ExperimentQueueXml = "attachment; filename=\"ExperimentQueue.xml\"";
        public const string StrRsp_Attachment_ExperimentResultsXml = "attachment; filename=\"ExperimentResults.xml\"";
        public const string StrRsp_Attachment_ExperimentStatisticsXml = "attachment; filename=\"ExperimentStatistics.xml\"";

        //
        // XML Configuration only used for development
        //
        public const string STRXML_SetupId_EquipmentGeneric = "EquipmentGeneric";
        public const string STRXML_SetupId_ModuleGeneric = "ModuleGeneric";
    }
}
