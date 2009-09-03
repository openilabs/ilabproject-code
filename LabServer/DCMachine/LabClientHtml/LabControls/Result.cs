using System;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml.LabControls
{
    public class Result : ExperimentResult
    {
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

        //-------------------------------------------------------------------------------------------------//

        public Result(string xmlExperimentResult)
            : base(xmlExperimentResult, new ResultInfo())
        {
            ResultInfo resultInfo = (ResultInfo)this.experimentResultInfo;
            try
            {
                //
                // Parse the experiment result
                //
                resultInfo.speedMin = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_speedMin, 0);
                resultInfo.speedMax = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_speedMax, 0);
                resultInfo.speedStep = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_speedStep, 0);
                resultInfo.fieldMin = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_fieldMin, 0);
                resultInfo.fieldMax = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_fieldMax, 0);
                resultInfo.fieldStep = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_fieldStep, 0);
                resultInfo.loadMin = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_loadMin, 0);
                resultInfo.loadMax = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_loadMax, 0);
                resultInfo.loadStep = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, STRXML_loadStep, 0);
                resultInfo.speedVector = XmlUtilities.GetXmlValue(this.xmlNodeExperimentResult, STRXML_speedVector, true);
                resultInfo.fieldVector = XmlUtilities.GetXmlValue(this.xmlNodeExperimentResult, STRXML_fieldVector, true);
                resultInfo.voltageVector = XmlUtilities.GetXmlValue(this.xmlNodeExperimentResult, STRXML_voltageVector, true);
                resultInfo.loadVector = XmlUtilities.GetXmlValue(this.xmlNodeExperimentResult, STRXML_loadVector, true);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        public ResultInfo GetResultInfo()
        {
            return (ResultInfo)this.experimentResultInfo;
        }
    }

}
