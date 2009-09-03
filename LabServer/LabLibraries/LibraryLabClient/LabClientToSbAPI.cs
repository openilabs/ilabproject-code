using System;
using Library.Lab;

namespace Library.LabClient
{
    public class LabClientToSbAPI
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "LabClientToSbAPI";

        //
        // Local variables
        //
        private ServiceBrokerService serviceBroker;
        private string labServerGuid;

        #endregion

        //---------------------------------------------------------------------------------------//

        public LabClientToSbAPI()
            : this(0, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public LabClientToSbAPI(long couponID, string passkey)
        {
            const string STRLOG_MethodName = "LabClientToSbAPI";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Get the ServiceBroker Url and LabServer Guid from the config file
            //
            string serviceBrokerUrl = null;
            try
            {
                //
                // Get the ServiceBroker's Url
                //
                serviceBrokerUrl = Utilities.GetAppSetting(Consts.StrCfg_ServiceBrokerUrl);
                if (serviceBrokerUrl == null)
                {
                    throw new ArgumentNullException(Consts.StrCfg_ServiceBrokerUrl);
                }

                //
                // Get the LabServer's GUID
                //
                this.labServerGuid = Utilities.GetAppSetting(Consts.StrCfg_LabServerGuid);
                if (this.labServerGuid == null)
                {
                    throw new ArgumentNullException(Consts.StrCfg_LabServerGuid);
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw;
            }

            //
            // Create ServiceBroker interface
            //
            this.serviceBroker = new ServiceBrokerService();
            this.serviceBroker.Url = serviceBrokerUrl;

            //
            // Create authorisation information and fill in
            //
            sbAuthHeader sbHeader = new sbAuthHeader();
            sbHeader.couponID = couponID;
            sbHeader.couponPassKey = passkey;
            this.serviceBroker.sbAuthHeaderValue = sbHeader;

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        public bool Cancel(int experimentID)
        {
            return this.serviceBroker.Cancel(experimentID);
        }

        //---------------------------------------------------------------------------------------//

        public WaitEstimate GetEffectiveQueueLength()
        {
            return this.serviceBroker.GetEffectiveQueueLength(this.labServerGuid, 0);
        }

        //---------------------------------------------------------------------------------------//

        public WaitEstimate GetEffectiveQueueLength(int priorityHint)
        {
            return this.serviceBroker.GetEffectiveQueueLength(this.labServerGuid, priorityHint);
        }

        //---------------------------------------------------------------------------------------//

        public LabExperimentStatus GetExperimentStatus(int experimentID)
        {
            return this.serviceBroker.GetExperimentStatus(experimentID);
        }

        //---------------------------------------------------------------------------------------//

        public string GetLabConfiguration()
        {
            return this.serviceBroker.GetLabConfiguration(this.labServerGuid);
        }

        //---------------------------------------------------------------------------------------//

        public string GetLabInfo()
        {
            return this.serviceBroker.GetLabInfo(this.labServerGuid);
        }

        //---------------------------------------------------------------------------------------//

        public LabStatus GetLabStatus()
        {
            return this.serviceBroker.GetLabStatus(this.labServerGuid);
        }

        //---------------------------------------------------------------------------------------//

        public ResultReport RetrieveResult(int experimentID)
        {
            return this.serviceBroker.RetrieveResult(experimentID);
        }

        //---------------------------------------------------------------------------------------//

        public SubmissionReport Submit(string experimentSpecification)
        {
            return this.serviceBroker.Submit(this.labServerGuid, experimentSpecification, 0, false);
        }

        //---------------------------------------------------------------------------------------//

        public SubmissionReport Submit(string experimentSpecification, int priorityHint, bool emailNotification)
        {
            return this.serviceBroker.Submit(this.labServerGuid, experimentSpecification,
                priorityHint, emailNotification);
        }

        //---------------------------------------------------------------------------------------//

        public ValidationReport Validate(string experimentSpecification)
        {
            return this.serviceBroker.Validate(this.labServerGuid, experimentSpecification);
        }

    }
}
