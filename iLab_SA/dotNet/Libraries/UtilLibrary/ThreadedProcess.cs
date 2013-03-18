using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace iLabs.UtilLib
{
    public delegate void ProcessDelegate();

    public class ThreadedProcess : IDisposable
    {
        protected Thread theThread;
        protected string processName = "Threaded Process";
        /// <summary>
        /// The the time to wait before running the process 
        /// in millieconds.
        /// </summary>
        protected int waitMilliseconds = 180000;

        //protected AutoResetEvent waitHandle = new AutoResetEvent(false);
        protected object syncLock = new Object();
        protected ProcessDelegate theProcess;


        /// <summary>
        /// Determines the status for the Scheduler
        /// </summary>        
        public bool Cancelled
        {
            get { return cancelled; }
            set { cancelled = value; }
        }
        protected bool cancelled = false;
        public string ProcessName
        {
            get { return processName; }
            set { processName = value; }
        }
        /// <summary>
        /// The the time to wait before running the process in seconds
        /// </summary>
        public int WaitTime
        {
            get { return waitMilliseconds / 1000; }
            set { waitMilliseconds = 1000 * value; }
        }

        public ProcessDelegate Process
        {
            get { return theProcess; }
            set { theProcess = value; }
        }

      

        public ThreadedProcess()
        {

        }

        /// <summary>
        /// Starts the background thread process       
        /// </summary>
        public void Start()
        {
            // *** Ensure that any waiting instances are shut down
            //this.WaitHandle.Set();
            lock (this)
            {
               Console.WriteLine(DateTime.Now + " Starting " + ProcessName);
                this.cancelled = false;
                theThread = new Thread(new ThreadStart(Run));
                theThread.IsBackground = true;
                theThread.Start();
            }
            //Run();
        }

        /// <summary>
        /// Starts the background thread process       
        /// </summary>
        /// <param name="waitTime">The time between DoProcess iterations in seconds</param>
        public void Start(int waitTime)
        {
            this.WaitTime = waitTime;
            Start();
        }

        /// <summary>
        /// Causes the processing to stop. If the operation is still
        /// active it will stop after the current message processing
        /// completes
        /// </summary>
        public void Stop()
        {
            lock (this.syncLock)
            {
                if (cancelled)
                    return;

                this.cancelled = true;
                //this.waitHandle.Set();
            }
        }

        /// <summary>
        /// Runs DoProcess
        /// </summary>
        private void Run()
        {
            Console.WriteLine(DateTime.Now + " Running " + ProcessName);

            // Wait one time before process
            //this.waitHandle.WaitOne(this.waitMilliseconds, true);

            while (!cancelled)
            {
                DoProcess();
                // *** Put in 
                //this.waitHandle.WaitOne(this.waitMilliseconds, true);
                Thread.Sleep(waitMilliseconds);

            }
            Console.WriteLine(DateTime.Now + " Shutting down " + ProcessName);
        }

        public virtual void DoProcess()
        {
            Console.WriteLine(DateTime.Now + " Executing " + ProcessName);
            theProcess();
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }
        #endregion
    }
}
