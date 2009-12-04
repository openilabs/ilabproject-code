using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;

namespace Library.LabEquipment.Drivers
{
    public class RadiationCounter
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "RadiationCounter";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_Success = " Success: ";
        private const string STRLOG_CaptureTimeout = " Capture timeout!";
        private const string STRLOG_InvalidData = " Invalid data!";

        //
        // Local variables
        //
        private bool initialised;
        private SerialLcd serialLcd;

        public const int DELAY_CAPTURE_DATA = 1; // seconds

        #endregion

        #region Properties

        public const int DELAY_INITIALISE = 0;

        private double adjustDuration;

        /// <summary>
        /// Returns the time (in seconds) that it takes for the equipment to initialise.
        /// </summary>
        public int InitialiseDelay
        {
            get { return (this.initialised == false) ? DELAY_INITIALISE : 0; }
        }

        public double AdjustDuration
        {
            get { return this.adjustDuration; }
            set { this.adjustDuration = value; }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public RadiationCounter(SerialLcd serialLcd)
        {
            const string STRLOG_MethodName = "RadiationCounter";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Initialise local variables
            //
            this.initialised = false;
            this.serialLcd = serialLcd;

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        public bool Initialise()
        {
            const string STRLOG_MethodName = "Initialise";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            //
            // Nothing to do here
            //

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public double GetCaptureDataTime(int duration)
        {
            double seconds = 0.0;

            seconds = duration + this.adjustDuration;

            return seconds;
        }

        //---------------------------------------------------------------------------------------//

        public int CaptureData(int duration)
        {
#if NO_HARDWARE
            int data = 235;
#else
            int data = -1;

            if (this.serialLcd != null)
            {
                this.serialLcd.StartCapture(duration);

                //
                // Use a timeout and retries
                //
                int retries = 3;
                for (int i = 0; i < retries; i++)
                {
                    //
                    // Check for data, but use a timeout
                    //
                    int timeout = (duration + 3) * 2;
                    while (true)
                    {
                        // Get capture data from serial LCD
                        data = this.serialLcd.CaptureData;

                        // Check if data received
                        if (data >= 0)
                        {
                            // Data received
                            break;
                        }

                        // Not data yet, check timeout
                        if (--timeout == 0)
                        {
                            // Timed out
                            break;
                        }

                        //
                        // Wait for data
                        //
                        Thread.Sleep(500);
                        Trace.Write(".");
                    }

                    if (timeout == 0)
                    {
                        Logfile.Write(STRLOG_CaptureTimeout);
                    }
                    else if (data < 0)
                    {
                        Logfile.Write(STRLOG_InvalidData);
                    }
                    else
                    {
                        // Data captured successfully
                        break;
                    }
                }

                this.serialLcd.StopCapture();
            }
#endif

            return data;
        }

    }
}
