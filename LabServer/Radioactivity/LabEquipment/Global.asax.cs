using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Library.LabEquipment;

namespace LabEquipment
{
    public class Global : System.Web.HttpApplication
    {
        private const string STRLOG_ClassName = "Global";

        public static AllowedCallers allowedCallers = null;
        public static EquipmentEngine equipmentEngine = null;

        //-------------------------------------------------------------------------------------------------//

        protected void Application_Start(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_Start";

            // Get root filepath from config file
            string rootFilePath = Utilities.GetAppSetting(Library.LabEquipment.Consts.StrCfg_RootFilePath);

            // Check for trailing backslash and append if missing
            if (rootFilePath.EndsWith("\\") == false)
            {
                rootFilePath += "\\";
            }

            // Get path for log files from config file
            string logFilesPath = Utilities.GetAppSetting(Library.LabEquipment.Consts.StrCfg_LogFilesPath);

            // Set the path for log files
            Logfile.SetFilePath(rootFilePath + logFilesPath);

            Logfile.Write("");
            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Create experiment engine and start it
            //
            allowedCallers = new AllowedCallers();
            equipmentEngine = new EquipmentEngine();
            equipmentEngine.Start();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        //-------------------------------------------------------------------------------------------------//

        protected void Application_End(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_End";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            equipmentEngine.Close();
            Logfile.Close();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }
    }
}