using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;

namespace Library.LabServerEngine
{
    public class DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverGeneric";

        //
        // String constants for logfile messages
        //
        protected const string STRLOG_executionTime = " executionTime: ";
        protected const string STRLOG_statusCode = " statusCode: ";

        //
        // Constants
        //
        private const int EXECUTION_TIME = 15;

        //
        // Local variables available to a derived class
        //
        protected CancelExperiment cancelExperiment;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverGeneric()
            : this(null)
        {
        }

        //-------------------------------------------------------------------------------------------------//

        public DriverGeneric(CancelExperiment cancelExperiment)
        {
            this.cancelExperiment = cancelExperiment;
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual int GetExecutionTime(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "GetExecutionTime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            int executionTime = EXECUTION_TIME;

            Logfile.Write(STRLOG_executionTime + executionTime.ToString());

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return EXECUTION_TIME;
        }

        //-------------------------------------------------------------------------------------------------//

        public virtual ExperimentResultInfo Execute(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "Execute";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Create an instance of the experiment result info ready to fill in
            ExperimentResultInfo experimentResultInfo = new ExperimentResultInfo();

            try
            {
                // Determine time to finish execution
                DateTime dateTimeEnd = DateTime.Now + new TimeSpan(0, 0, EXECUTION_TIME);

                //
                // Delay for the full execution time, unless cancelled
                //
                while (DateTime.Now < dateTimeEnd)
                {
                    Trace.Write("G");
                    Thread.Sleep(1000);

                    //
                    // Check if the experiment is being cancelled
                    //
                    if (this.cancelExperiment != null &&
                        this.cancelExperiment.IsCancelled == true)
                    {
                        // Experiment is cancelled
                        experimentResultInfo.statusCode = StatusCodes.Cancelled;
                        break;
                    }
                }
                Trace.WriteLine("");

                //
                // Check if the experiment was cancelled
                //
                if (experimentResultInfo.statusCode != StatusCodes.Cancelled)
                {
                    // Successful execution
                    experimentResultInfo.statusCode = StatusCodes.Completed;
                }
            }
            catch (Exception ex)
            {
                experimentResultInfo.statusCode = StatusCodes.Failed;
                experimentResultInfo.errorMessage = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            Logfile.Write(STRLOG_statusCode + experimentResultInfo.statusCode);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return experimentResultInfo;
        }
    }
}
