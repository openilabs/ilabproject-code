using System;
using System.Diagnostics;
using Library.Lab;

namespace Library.LabServer.Drivers.Equipment
{
    public class LabServerToRadioactivityAPI
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "LabServerToRadioactivityAPI";

        //
        // Public properties
        //
        private RadioactivityService radioactivityService;

        public RadioactivityService Radioactivity
        {
            get { return this.radioactivityService; }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public LabServerToRadioactivityAPI()
        {
            const string STRLOG_MethodName = "LabServerToRadioactivityAPI";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            try
            {
                // Get LabServer identifier
                string labServerGuid = Utilities.GetAppSetting(Library.LabServerEngine.Consts.STRCFG_LabServerGuid);

                //
                // Get radioactivity service Url and passkey
                //
                string radioactivityUrl = Utilities.GetAppSetting(Consts.StrCfg_RadioactivityService);
                string radioactivityPasskey = Utilities.GetAppSetting(Consts.StrCfg_RadioactivityPasskey);

                //
                // Create equipment service interface
                //
                this.radioactivityService = new RadioactivityService();
                this.radioactivityService.Url = radioactivityUrl;

                //
                // Create and fill in authorisation information
                //
                AuthHeader authHeader = new AuthHeader();
                authHeader.identifier = labServerGuid;
                authHeader.passKey = radioactivityPasskey;
                this.radioactivityService.AuthHeaderValue = authHeader;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

    }
}
