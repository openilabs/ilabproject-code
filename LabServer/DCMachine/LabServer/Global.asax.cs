using System;
using Library.Lab;
using Library.LabServer;
using Library.LabServerEngine;

namespace LabServer
{
    public class Global : System.Web.HttpApplication
    {
        private const string STRLOG_ClassName = "Global";

        public static AllowedCallers allowedCallers = null;
        public static ExperimentManager experimentManager = null;

        //-------------------------------------------------------------------------------------------------//

        protected void Application_Start(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_Start";

            // Get root filepath from config file
            string rootFilePath = Utilities.GetAppSetting(Library.LabServerEngine.Consts.STRCFG_RootFilePath);

            // Check for trailing backslash and append if missing
            if (rootFilePath.EndsWith("\\") == false)
            {
                rootFilePath += "\\";
            }

            // Get path for log files from config file
            string logFilesPath = Utilities.GetAppSetting(Library.LabServerEngine.Consts.STRCFG_LogFilesPath);

            // Set the path for log files
            Logfile.SetFilePath(rootFilePath + logFilesPath);

            Logfile.Write("");
            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Create the experiment manager
            //
            allowedCallers = new AllowedCallers();
            Configuration configuration = new Configuration();
            experimentManager = new ExperimentManager(allowedCallers, configuration);
            experimentManager.Create();

            //
            // Now start the experiment manager
            //
            experimentManager.Start();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        protected void Application_Error(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_Error";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Need to do something here

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        protected void Application_End(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Application_End";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Close experiment manager
            //
            if (experimentManager != null)
            {
                experimentManager.Close();
            }

            // Close logfile class
            Logfile.Close();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }
    }
}