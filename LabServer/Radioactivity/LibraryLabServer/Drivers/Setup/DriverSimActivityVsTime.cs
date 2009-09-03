using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServer.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public class DriverSimActivityVsTime : DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverSimActivityVsTime";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_StartingMeasurements = " Starting measurements";
        private const string STRLOG_FinishedMeasurements = " Finished measurements";
        private const string STRLOG_Distance = " Distance: ";
        private const string STRLOG_Duration = " Duration: ";
        private const string STRLOG_Repeat = " Repeat: ";
        private const string STRLOG_VectorLength = " VectorLength: ";
        private const string STRLOG_MovingTube = " Moving tube to ";
        private const string STRLOG_Millimetres = " mm";
        private const string STRLOG_SelectingAbsorber = " Selecting absorber: ";
        private const string STRLOG_SelectingSource = " Selecting source: ";
        private const string STRLOG_ReturningSource = "Returning source to home position";
        private const string STRLOG_ReturningAbsorber = "Returning absorber to home position";
        private const string STRLOG_ReturningTube = "Returning tube to home position";

        //
        // Local variables
        //
        private SimActivity simActivity;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverSimActivityVsTime()
            : this(null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverSimActivityVsTime(CancelExperiment cancelExperiment)
            : base(cancelExperiment)
        {
            // Create driver class
            this.simActivity = new SimActivity();
        }

        //-------------------------------------------------------------------------------------------------//

        public override int GetExecutionTime(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "GetExecutionTime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Typecast the specification so that it can be used here
            Specification specification = (Specification)experimentSpecification;

            int executionTime = 0;
            int tubeHomeDistance = this.simActivity.GetTubeHomeDistance();

            //
            // Determine pre-measurement time
            //
            executionTime += this.simActivity.GetTubeMoveTime(tubeHomeDistance, specification.Distance);
            executionTime += this.simActivity.GetAbsorberSelectTime(specification.AbsorberLocation);
            executionTime += this.simActivity.GetSourceSelectTime(specification.SourceLocation);

            //
            // Determine measurement time
            //
            executionTime += specification.Duration * specification.Repeat;

            //
            // Determine post-measurement time
            //
            executionTime += this.simActivity.GetSourceReturnTime(specification.SourceLocation);
            executionTime += this.simActivity.GetAbsorberReturnTime(specification.AbsorberLocation);
            executionTime += this.simActivity.GetTubeMoveTime(specification.Distance, tubeHomeDistance);

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

            //
            // Create array to hold the results
            //
            int vectorLength = specification.Repeat;
            resultInfo.dataVectors = new int[1,vectorLength];

            Logfile.Write(STRLOG_Distance + specification.Distance.ToString());
            Logfile.Write(STRLOG_Duration + specification.Duration.ToString());
            Logfile.Write(STRLOG_Repeat + specification.Repeat.ToString());
            Logfile.Write(STRLOG_VectorLength + vectorLength.ToString());

            try
            {
                //
                // Move tube into position
                //
                Logfile.Write(STRLOG_MovingTube + specification.Distance.ToString() + STRLOG_Millimetres);
                this.simActivity.SetTubeDistance(specification.Distance);

                //
                // Move absorber into position
                //
                Logfile.Write(STRLOG_SelectingAbsorber + specification.AbsorberName);
                this.simActivity.SetAbsorberLocation(specification.AbsorberLocation);

                //
                // Move source into position
                //
                Logfile.Write(STRLOG_SelectingSource + specification.SourceName);
                this.simActivity.SetSourceLocation(specification.SourceLocation);

                //
                // Start data capture
                //
                Logfile.Write(STRLOG_StartingMeasurements);

                //
                // Generate simulated data
                //
                int[] data = this.simActivity.GenerateData(specification.Distance, specification.Duration, specification.Repeat);

                //
                // Run simulated activity capture
                //
                for (int i = 0; i < specification.Repeat; i++)
                {
                    if (this.simActivity.SimulateDelays == true)
                    {
                        // Delay for specified duration
                        for (int j = 0; j < specification.Duration; j++)
                        {
                            Thread.Sleep(1000);
                            Trace.Write("C");

                            // Check if the experiment is being cancelled
                            if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                            {
                                // Experiment is cancelled
                                break;
                            }
                        }
                        Trace.WriteLine("");
                    }

                    //
                    // Store the data
                    //
                    resultInfo.dataVectors[0, i] = data[i];

                    //
                    // Check if the experiment is being cancelled
                    //
                    if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                    {
                        // Experiment is cancelled
                        resultInfo.statusCode = StatusCodes.Cancelled;
                        break;
                    }
                }

                //
                // Data capture is finished
                //
                Logfile.Write(STRLOG_FinishedMeasurements);

                //
                // Return source to home position
                //
                Logfile.Write(STRLOG_ReturningSource);
                this.simActivity.SetSourceLocation(this.simActivity.GetSourceHomeLocation());

                //
                // Return absorber to home position
                //
                Logfile.Write(STRLOG_ReturningAbsorber);
                this.simActivity.SetAbsorberLocation(this.simActivity.GetAbsorberHomeLocation());

                //
                // Return tube to home position
                //
                Logfile.Write(STRLOG_ReturningTube);
                this.simActivity.SetTubeDistance(this.simActivity.GetTubeHomeDistance());

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
