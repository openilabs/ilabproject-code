using System;
using Library.LabServerEngine;

namespace Library.LabServer
{
    public class ResultInfo : ExperimentResultInfo
    {
        public int[,] dataVectors;

        //-------------------------------------------------------------------------------------------------//

        public ResultInfo()
            : base()
        {
        }
    }

}
