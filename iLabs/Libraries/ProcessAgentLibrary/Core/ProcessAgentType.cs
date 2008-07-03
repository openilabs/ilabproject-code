using System;

namespace iLabs.Core
{

    public class ProcessAgentType
    {
        public enum AgentType : int
        {
            SERVICE_BROKER = 4, BATCH_SERVICE_BROKER = 5, REMOTE_SERVICE_BROKER = 6,
            LAB_SERVER = 8, BATCH_LAB_SERVER = 9,
            EXPERIMENT_STORAGE_SERVER = 16,
            SCHEDULING_SERVER = 32, LAB_SCHEDULING_SERVER = 64
        }

        // Standard String types for the ProcessAgent Types
        public const string SERVICE_BROKER = "SERVICE BROKER";
        public const string BATCH_SERVICE_BROKER = "BATCH SERVICE BROKER";
        public const string REMOTE_SERVICE_BROKER = "REMOTE SERVICE BROKER";
        public const string LAB_SERVER = "LAB SERVER";
        public const string BATCH_LAB_SERVER = "BATCH LAB SERVER";
        public const string EXPERIMENT_STORAGE_SERVER = "EXPERIMENT STORAGE SERVER";
        public const string SCHEDULING_SERVER = "SCHEDULING SERVER";
        public const string LAB_SCHEDULING_SERVER = "LAB SCHEDULING SERVER";

        public static string ToTypeName(AgentType type)
        {
            string typeName = null;
            switch (type)
            {
                case AgentType.SERVICE_BROKER:
                    typeName = ProcessAgentType.SERVICE_BROKER;
                    break;
                case AgentType.BATCH_SERVICE_BROKER:
                    typeName = ProcessAgentType.BATCH_SERVICE_BROKER;
                    break;

                case AgentType.REMOTE_SERVICE_BROKER:
                    typeName = ProcessAgentType.REMOTE_SERVICE_BROKER;
                    break;
                case AgentType.LAB_SERVER:
                    typeName = ProcessAgentType.LAB_SERVER;
                    break;
                case AgentType.BATCH_LAB_SERVER:
                    typeName = ProcessAgentType.BATCH_LAB_SERVER;
                    break;
                case AgentType.EXPERIMENT_STORAGE_SERVER:
                    typeName = ProcessAgentType.EXPERIMENT_STORAGE_SERVER;
                    break;
                case AgentType.SCHEDULING_SERVER:
                    typeName = ProcessAgentType.SCHEDULING_SERVER;
                    break;
                case AgentType.LAB_SCHEDULING_SERVER:
                    typeName = ProcessAgentType.LAB_SCHEDULING_SERVER;
                    break;
                default:
                    break;
            }
            return typeName;
        }


    }
}