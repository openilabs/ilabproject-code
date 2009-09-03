using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServer.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public class DriverNetwork : DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverNetwork";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_timeofday = " timeofday: ";

        //
        // String constants for error messages
        //
        private const string STRERR_FailedToRespond = "NTP Server failed to respond!";

        //
        // Local variables
        //
        private int timeout;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverNetwork()
            : this(null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverNetwork(CancelExperiment cancelExperiment)
            : base(cancelExperiment)
        {
            this.timeout = Utilities.GetIntAppSetting(Consts.STRCFG_NtpServerTimeout);
        }

        //-------------------------------------------------------------------------------------------------//

        public override int GetExecutionTime(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "GetExecutionTime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Specification specification = (Specification)experimentSpecification;

            int executionTime = this.timeout;

            Logfile.Write(STRLOG_executionTime + executionTime.ToString());

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return executionTime;
        }

        //---------------------------------------------------------------------------------------//

        public override ExperimentResultInfo Execute(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "Execute";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Typecast the specification so that it can be used here
            Specification specification = (Specification)experimentSpecification;

            // Create an instance of the result info ready to fill in
            ResultInfo resultInfo = new ResultInfo();

            try
            {
                //
                // Get the time from the specified NTP server
                //
                NTPClient ntpClient = new NTPClient(specification.ServerUrl);
                if (ntpClient.Connect(this.timeout, false) == false)
                {
                    throw new Exception(STRERR_FailedToRespond);
                }
                resultInfo.dateTime = ntpClient.TransmitTimestamp;

                //
                // Save the timestamp string in the specified format
                //
                if (specification.FormatName.Equals(Consts.STRXML_Format_12Hour))
                {
                    resultInfo.timeofday = resultInfo.dateTime.ToString(Consts.STR_DateTimeFormat_12Hour);
                }
                else if (specification.FormatName.Equals(Consts.STRXML_Format_24Hour))
                {
                    resultInfo.timeofday = resultInfo.dateTime.ToString(Consts.STR_DateTimeFormat_24Hour);
                }

                Logfile.Write(STRLOG_timeofday + resultInfo.timeofday);

                //
                // Delay for the full execution time, unless cancelled
                //
                DateTime dateTimeEnd = resultInfo.dateTime + new TimeSpan(0, 0, this.timeout);
                while (DateTime.Now < dateTimeEnd)
                {
                    Trace.Write("N");
                    Thread.Sleep(1000);

                    //
                    // Check if the experiment is being cancelled
                    //
                    if (this.cancelExperiment != null &&
                        this.cancelExperiment.IsCancelled == true)
                    {
                        // Experiment is cancelled
                        resultInfo.statusCode = StatusCodes.Cancelled;
                        break;
                    }
                }
                Trace.WriteLine("");

                //
                // Check if the experiment was cancelled
                //
                if (resultInfo.statusCode != StatusCodes.Cancelled)
                {
                    // Successful execution
                    resultInfo.statusCode = StatusCodes.Completed;
                }
            }
            catch (Exception ex)
            {
                resultInfo.statusCode = StatusCodes.Failed;
                resultInfo.errorMessage = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            Logfile.Write(STRLOG_statusCode + resultInfo.statusCode);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return resultInfo;
        }
    }
}
