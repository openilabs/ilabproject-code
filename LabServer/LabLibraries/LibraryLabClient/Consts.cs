using System;

namespace Library.LabClient
{
    public class Consts
    {
        //
        // Config file key strings
        //
        public const string STRCFG_ServiceBrokerUrl = "ServiceBrokerUrl";
        public const string STRCFG_LabServerGuid = "LabServerGuid";
        public const string STRCFG_LogFilesPath = "LogFilesPath";
        public const string STRCFG_FeedbackEmail = "FeedbackEmail";
        public const string STRCFG_MultipleSubmit = "MultipleSubmit";

        //
        // Session variables
        //
        public const string STRSSN_SubmittedID = "SubmittedID";
        public const string STRSSN_CompletedID = "CompletedID";
        public const string STRSSN_SubmittedIDs = "SubmittedIDs";
        public const string STRSSN_CompletedIDs = "CompletedIDs";

        //
        // XML elements in the lab configuration string
        //
        public const string STRXML_labConfiguration = "labConfiguration";
        public const string STRXMLPARAM_title = "@title";
        public const string STRXMLPARAM_version = "@version";
        public const string STRXML_navmenuPhoto_image = "navmenuPhoto/image";
        public const string STRXML_labInfo_text = "labInfo/text";
        public const string STRXML_labInfo_url = "labInfo/url";
        public const string STRXML_configuration = "configuration";
        public const string STRXML_setup = "setup";
        public const string STRXMLPARAM_id = "@id";
        public const string STRXML_name = "name";
        public const string STRXML_description = "description";

        //
        // XML elements in the specification string
        //
        public const string STRXML_setupId = "setupId";
        public const string STRXML_experimentSpecification = "experimentSpecification";
        public const string STRXML_validation = "validation";

        //
        // XML elements in the experiment results string
        //
        public const string STRXML_experimentResult = "experimentResult";
        public const string STRXML_timestamp = "timestamp";
        public const string STRXML_title = "title";
        public const string STRXML_version = "version";
        public const string STRXML_experimentId = "experimentId";
        public const string STRXML_unitId = "unitId";
        public const string STRXML_setupName = "setupName";
        public const string STRXML_dataType = "dataType";

        //
        // XML elements for the Java applets
        //
        public const string STRXML_resultsApplet_archive = "resultsApplet/archive";
        public const string STRXML_resultsApplet_code = "resultsApplet/code";

        //
        // Result string download response
        //
        public const string StrRsp_ContentTypeCsv = "Application/x-msexcel";
        public const string StrRsp_Disposition = "content-disposition";
        public const string StrRsp_AttachmentCsv = "attachment; filename=\"result.csv\"";
    }
}
