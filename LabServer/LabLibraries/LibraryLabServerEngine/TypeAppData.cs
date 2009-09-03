using System;

namespace Library.LabServerEngine
{
    public class AppData
    {
        public AllowedCallers allowedCallers;
        public ExperimentResults experimentResults;
        public ExperimentStatistics experimentStatistics;
        public LabConfiguration labConfiguration;
        public int farmSize;
        public object signalCompleted;
        public LabExperimentEngine[] labExperimentEngines;
    }
}
