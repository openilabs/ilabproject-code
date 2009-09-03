using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;
using Library.LabServerEngine;

namespace Library.LabServer.Drivers.Setup
{
    public class DriverLocal : DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverLocal";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_timeofday = " timeofday: ";

        //
        // String constants for error messages
        //

        //
        // Constants
        //
        private const int EXECUTION_TIME = 10;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverLocal()
            : this(null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverLocal(CancelExperiment cancelExperiment)
            : base(cancelExperiment)
        {
        }

        //-------------------------------------------------------------------------------------------------//

        public override int GetExecutionTime(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "GetExecutionTime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Specification specification = (Specification)experimentSpecification;

            int executionTime = EXECUTION_TIME;

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
                // Get the LabServer's local clock time
                //
                resultInfo.dateTime = DateTime.Now;

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
                DateTime dateTimeEnd = resultInfo.dateTime + new TimeSpan(0, 0, EXECUTION_TIME);
                while (DateTime.Now < dateTimeEnd)
                {
                    Trace.Write("L");
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
