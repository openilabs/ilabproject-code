using System;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml.LabControls
{
    public class Result : ExperimentResult
    {

        //-------------------------------------------------------------------------------------------------//

        public Result(string xmlExperimentResult)
            : base(xmlExperimentResult, new ResultInfo())
        {
            ResultInfo resultInfo = (ResultInfo)this.experimentResultInfo;

            //
            // Parse the experiment result
            //
            try
            {
                resultInfo.voltage = (float)XmlUtilities.GetRealValue(this.xmlNodeExperimentResult, LabConsts.STRXML_voltage, 0);
                resultInfo.current = (float)XmlUtilities.GetRealValue(this.xmlNodeExperimentResult, LabConsts.STRXML_current, 0);
                resultInfo.powerFactor = (float)XmlUtilities.GetRealValue(this.xmlNodeExperimentResult, LabConsts.STRXML_powerFactor, 0);
                resultInfo.speed = XmlUtilities.GetIntValue(this.xmlNodeExperimentResult, LabConsts.STRXML_speed, 0);
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
