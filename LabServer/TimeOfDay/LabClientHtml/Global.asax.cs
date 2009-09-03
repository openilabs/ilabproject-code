using System;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml
{
    public class Global : System.Web.HttpApplication
    {
        private const string STRLOG_ClassName = "Global";

        //---------------------------------------------------------------------------------------//

        protected void Application_Start(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_Start";

            // Get root filepath from config file
            string rootFilePath = Utilities.GetAppSetting(Consts.StrCfg_RootFilePath);

            // Check for trailing backslash and append if missing
            if (rootFilePath.EndsWith("\\") == false)
            {
                rootFilePath += "\\";
            }

            // Get path for log files from config file
            string logFilesPath = Utilities.GetAppSetting(Consts.StrCfg_LogFilesPath);

            // Set the path for log files
            Logfile.SetFilePath(rootFilePath + logFilesPath);

            Logfile.Write("");
            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Nothing to do here

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        protected void Application_End(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_End";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Logfile.Close();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }
    }
}