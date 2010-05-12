using System;

namespace Library.LabServerEngine
{
    public class AppData
    {
        public AllowedCallers allowedCallers;
        public ExperimentQueueDB experimentQueue;
        public ExperimentResults experimentResults;
        public ExperimentStatistics experimentStatistics;
        public LabConfiguration labConfiguration;
        public int farmSize;
        public LabExperimentEngine[] labExperimentEngines;
        public Object signalCompleted;
    }
}
