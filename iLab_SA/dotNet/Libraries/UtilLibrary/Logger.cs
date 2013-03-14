using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace iLabs.UtilLib
{
    public class Logger
    {

        public static TraceSwitch traceSwitch = new TraceSwitch("defaultTrace", "Trace switch used by default");

        // Make this a property
        private static string logPath = null;
        private static System.IO.StreamWriter writer;
        private static bool isLogging = false;

        public static string LogPath
        {
            get
            {
                return logPath;
            }
            set
            {
                logPath = value;
                isLogging = (logPath != null && logPath.Length > 0) ? true : false;
            }
        }

        public static bool IsLogging
        {
            get{
                return isLogging;
            }
        }
            public static void WriteLine(string str)
            {
                if (isLogging)
                {
                    try
                    {
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(logPath, true);
                        writer.WriteLine(DateTime.Now.ToString() + ": \t" + str);
                        writer.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                Trace.WriteLine(DateTime.Now.ToString() + ": \t" + str);
            }

        public static void Write(string str)
        {
            if (isLogging)
            {
                try
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(logPath, true);
                    writer.WriteLine(str);
                    writer.Close();
                }
                catch (Exception ex)
                {
                }
            }
            Trace.Write(str);
        }
    }
}
