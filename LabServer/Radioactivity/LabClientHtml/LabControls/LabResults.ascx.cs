using System;
using System.IO;
using System.Xml;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml.LabControls
{
    public partial class LabResults : System.Web.UI.UserControl
    {
        #region Class Constants and Variables

        //
        // String constants
        //
        public const string STR_Source = "Source";
        public const string STR_Absorber = "Absorber";
        public const string STR_Distance = "Distance (mm)";
        public const string STR_DistanceList = "Distance List (mm)";
        public const string STR_Duration = "Duration (secs)";
        public const string STR_Trials = "Trials";
        public const string STR_CountsAt = "Counts @ ";
        public const string STR_Millimetres = "mm";

        #endregion

        //-------------------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Create a string which represents the experiment specification. Each line contains two fields
        /// which are the name of the field and its value. The format of the string will be different
        /// for comma-seperated-values and applet parameters.
        /// </summary>
        /// <param name="xmlNodeExperimentResult"></param>
        /// <param name="swArgument"></param>
        /// <returns></returns>
        public string CreateSpecificationString(ResultInfo resultInfo, string swArgument)
        {
            StringWriter sw = new StringWriter();
            try
            {
                // Write the source name
                sw.WriteLine(swArgument, STR_Source, resultInfo.sourceName);

                // Write the absorber name
                sw.WriteLine(swArgument, STR_Absorber, resultInfo.absorberName);

                //
                // Create a CSV string of distances from the distance list
                //
                string strDistances = "";
                for (int i = 0; i < resultInfo.distances.Length; i++)
                {
                    if (i > 0)
                    {
                        strDistances += ",";
                    }
                    strDistances += resultInfo.distances[i].ToString();
                }

                // Write the distances
                if (resultInfo.distances.Length > 1)
                {
                    // Write as a distance list
                    sw.WriteLine(swArgument, STR_DistanceList, strDistances);
                }
                else
                {
                    // Write as a single distance
                    sw.WriteLine(swArgument, STR_Distance, strDistances);
                }

                // Write the duration
                sw.WriteLine(swArgument, STR_Duration, resultInfo.duration);

                // Write the repeat count (number of trials)
                sw.WriteLine(swArgument, STR_Trials, resultInfo.repeat);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return sw.ToString();
        }

        //-------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Create a string which represents the experiment result. Each line contains two fields
        /// which are the name of the field and its value. The format of the string will be different
        /// for comma-seperated-values and applet parameters.
        /// </summary>
        /// <param name="xmlNodeExperimentResult"></param>
        /// <param name="swArgument"></param>
        /// <returns></returns>
        public string CreateResultsString(ResultInfo resultInfo, string swArgument)
        {
            StringWriter sw = new StringWriter();
            try
            {
                for (int i = 0; i < resultInfo.datavectors.GetLength(0); i++)
                {
                    //
                    // Create a CSV string of radioactivity counts from the data vector
                    //
                    string strDistances = "";
                    for (int j = 0; j < resultInfo.datavectors.GetLength(1); j++)
                    {
                        if (j > 0)
                        {
                            strDistances += ",";
                        }
                        strDistances += resultInfo.datavectors[i, j].ToString();
                    }

                    // Write the string of counts also showing the distance
                    sw.WriteLine(swArgument, STR_CountsAt + resultInfo.distances[i].ToString() + STR_Millimetres, strDistances);
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return sw.ToString();
        }
    }

}