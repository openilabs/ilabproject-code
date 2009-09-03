using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace Library.LabEquipment
{
    public class Logfile
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "Logfile";

        //
        // String constants
        //
        private const string DEFAULT_EXT = ".log";

        private const string STRLOG_CreateThreadObjectsFailed = "Create thread objects failed!";
        private const string STR_Error = "***ERROR***\r\n";

        private static string logfilePath = null;
        private static Queue logfileQueue = null;

        //
        // Thread related local variables and properties
        //
        private static bool running;
        private static Object statusLock;
        private static Thread threadLogfile;

        //
        // Private properties
        //
        private static bool Running
        {
            get
            {
                lock (statusLock)
                {
                    return running;
                }
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public static void SetFilePath(string path)
        {
            // Remove any leading and trailing whitespace
            logfilePath = path.Trim();

            // Append trailing backslash if missing
            if (logfilePath.EndsWith("\\") == false)
            {
                logfilePath += "\\";
            }

            //
            // Attempt to write to logfile
            //
            string filename = CreateDatedFilename();
            try
            {
                StreamWriter sw = new StreamWriter(filename, true);
                sw.WriteLine(STRLOG_ClassName + " -> logfilePath='" + logfilePath + "'");
                sw.Close();
            }
            catch (Exception)
            {
                // Unable to write to logfile
                throw;
            }

            // Create queue for logfile messages
            if (logfileQueue == null)
            {
                logfileQueue = Queue.Synchronized(new Queue());
            }

            //
            // Thread related initialisation
            //
            statusLock = new Object();
            threadLogfile = new Thread(new ThreadStart(LogfileThread));
            if (statusLock == null || threadLogfile == null)
            {
                throw new ArgumentNullException(STRLOG_CreateThreadObjectsFailed);
            }

            //
            // Start the thread running
            //
            running = true;
            threadLogfile.Start();
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Terminate the logging thread. Messages can still be written but the logfile
        /// will be opened, written and closed for each message.
        /// </summary>
        public static void Close()
        {
            //
            // Tell LogfileThread() that it is no longer running
            //
            lock (statusLock)
            {
                running = false;
            }

            //
            // Wait for LabEquipmentThread() to terminate
            //
            for (int i = 0; i < 3; i++)
            {
                if (threadLogfile.Join(1000) == true)
                {
                    // Thread has terminated
                    Trace.WriteLine(" LogfileThread terminated OK");
                    break;
                }
            }

            //
            // Delete thread related objects
            //
            logfileQueue = null;
            statusLock = null;
            threadLogfile = null;
        }

        //---------------------------------------------------------------------------------------//

        public static void Write(string message)
        {
            if (message != null)
            {
                //
                // Add timestamp to message
                //
                DateTime now = DateTime.Now;
                message = now.ToString("T") + ":\t" + message;

                //Trace.WriteLine(message);

                if (running == true)
                {
                    //
                    // Add the message to the queue
                    //
                    lock (logfileQueue.SyncRoot)
                    {
                        logfileQueue.Enqueue(message);
                    }
                }
                else
                {
                    try
                    {
                        //
                        // Write the message to the log file
                        //
                        string filename = CreateDatedFilename();
                        string fullpath = Path.GetFullPath(filename);
                        StreamWriter sw = new StreamWriter(filename, true);
                        sw.WriteLine(message);
                        sw.Close();
                    }
                    catch
                    {
                        // Message not written to log file
                    }
                }
            }
        }

        //---------------------------------------------------------------------------------------//

        public static void Write()
        {
            Write("");
        }

        //---------------------------------------------------------------------------------------//

        public static void WriteError(string message)
        {
            if (message != null)
            {
                Write(STR_Error + message);
            }
        }

        //---------------------------------------------------------------------------------------//

        public static void WriteError(string methodName, string message)
        {
            if (methodName != null)
            {
                message = methodName + "(): " + message;
            }
            WriteError(message);
        }

        //---------------------------------------------------------------------------------------//

        public static void WriteError(string className, string methodName, string message)
        {
            if (methodName != null)
            {
                message = methodName + "(): " + message;
            }
            if (className != null && methodName != null)
            {
                message = className + "." + message;
            }
            WriteError(message);
        }

        //---------------------------------------------------------------------------------------//

        public static void WriteCalled(string className, string methodName)
        {
            string message = null;
            if (methodName != null)
            {
                message = methodName;
            }
            if (className != null && methodName != null)
            {
                message = className + "." + message;
            }
            if (message != null)
            {
                message = message + "(): Called";
                Write(message);
            }
        }

        //---------------------------------------------------------------------------------------//

        public static void WriteCompleted(string className, string methodName)
        {
            string message = null;
            if (methodName != null)
            {
                message = methodName;
            }
            if (className != null && methodName != null)
            {
                message = className + "." + message;
            }
            if (message != null)
            {
                message = message + "(): Completed";
                Write(message);
            }
        }

        //---------------------------------------------------------------------------------------//

        private static void LogfileThread()
        {
            const string STRLOG_MethodName = "LogfileThread";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            while (Running == true)
            {
                // Wait a bit before checking the experiment queue
                Thread.Sleep(1000);

                //
                // Check for queued messages
                //
                lock (logfileQueue.SyncRoot)
                {
                    if (logfileQueue.Count == 0)
                    {
                        continue;
                    }

                    //Trace.WriteLine(STRLOG_ClassName + ": logfileQueue.Count => " + logfileQueue.Count.ToString());

                    //
                    // Write queued messages to logfile
                    //
                    try
                    {
                        // Open logfile for writing
                        StreamWriter sw = new StreamWriter(CreateDatedFilename(), true);

                        // Write queued messages to the log file
                        while (logfileQueue.Count > 0)
                        {
                            // Get the next message off the queue
                            string message = (string)logfileQueue.Dequeue();

                            // Write the message to the logfile
                            sw.WriteLine(message);
                        }

                        // Close the log file
                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }

                    //Trace.WriteLine("Logfile closed.");
                }
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        private static string CreateDatedFilename()
        {
            //
            // The current date becomes the name of the file
            //
            DateTime now = DateTime.Now;
            string filename = logfilePath +
                now.ToString("yyyy") + now.ToString("MM") + now.ToString("dd") +
                DEFAULT_EXT;

            return filename;
        }
    }
}
