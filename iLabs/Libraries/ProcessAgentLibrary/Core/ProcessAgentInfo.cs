using System;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;


namespace iLabs.Core
{
	/// <summary>
	/// Summary description for ProcessAgentInfo.
	/// </summary>
	public class ProcessAgentInfo
	{
        public int agentId;
        public string agentName;
        public string agentGuid;
        public ProcessAgentType.AgentType agentType;
        public string domainGuid;
        public string codeBaseUrl;
        public string webServiceUrl;
        public string issuerGuid;
        public Coupon identIn = null;
        public Coupon identOut = null;

		public ProcessAgentInfo(){}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="codeBase"></param>
        /// <param name="servicePage"></param>
        /// <param name="inCoupon"></param>
        /// <param name="outCoupon"></param>
        public ProcessAgentInfo(int id, string guid, string name, int type,
             string domainGuid, string codeBase, string servicePage,
            string issuerGuid, Coupon inCoupon, Coupon outCoupon)
		{
			agentId = id;
            agentGuid = guid;
			agentName = name;
			agentType = (ProcessAgentType.AgentType) type;
            this.domainGuid = domainGuid;
            codeBaseUrl = codeBase;
			webServiceUrl = servicePage;
            this.issuerGuid = issuerGuid;
			identIn = inCoupon;
			identOut = outCoupon;
		}

		public string AgentGuid
		{
				get
				{
					return agentGuid;
				}

                set {
                    agentGuid = value; 
                }
			}	
		public int AgentType
		{
			get
			{
				return (int) agentType;
			}

		}
		public string AgentTypeName
		{
			get
			{
                return ProcessAgentType.ToTypeName(agentType);
			}

		}
		public string AgentName
		{
			get
			{
				return agentName;
			}

		}	
		public string ServiceUrl
		{
			get
			{
				return webServiceUrl;
			}

		}
        public string CodeBaseUrl
        {
            get
            {
                return codeBaseUrl;
            }

        }
		public int AgentId
		{
			get
			{
				return agentId;
			}

		}	

		


	}

	
}
