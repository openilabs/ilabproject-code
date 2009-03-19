using System;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;


namespace iLabs.Core
{
	
	public class SystemSupport
	{
        public int agentId;
        public string contactEmail;
        public string infoUrl;
        public string description;
      

		public SystemSupport(){}
       
        public SystemSupport(int id, string contactEmail, string infoUrl, string description)
		{
            this.contactEmail = contactEmail;
            this.infoUrl = infoUrl;
            this.description = description;
		}
	}
}
