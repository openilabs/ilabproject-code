using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServer.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    class DriverFullLoad : DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverFullLoad";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_StartingMeasurements = " Starting measurements";
        private const string STRLOG_FinishedMeasurements = " Finished measurements";

        //
        // Local variables
        //
        private RedLion redLion;

        //
        // States for executing the experiment
        //
        private enum States
        {
            sCreateConnection,
            sResetACDrive, sConfigureACDrive, sStartACDrive,
            sTakeMeasurement,
            sStopACDrive, sRestoreDefaultsACDrive,
            sCloseConnection
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverFullLoad(int unitId)
            : this(unitId, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverFullLoad(int unitId, CancelExperiment cancelExperiment)
            : base(cancelExperiment)
        {
            // Create equipment driver class
            this.redLion = new RedLion(unitId);
        }

        //-------------------------------------------------------------------------------------------------//

        public override int GetExecutionTime(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "GetExecutionTime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            Specification specification = (Specification)experimentSpecification;

            int executionTime = 0;

            //
            // Determine pre-measurement time
            //
            executionTime += this.redLion.ResetACDriveExecutionTime();
            executionTime += this.redLion.ConfigureACDriveExecutionTime();
            executionTime += this.redLion.StartACDriveExecutionTime(RedLion.ACDriveModes.FullLoad);

            //
            // Determine measurement time
            //
            int measurementsTime = this.redLion.MeasurementCount * this.redLion.MeasurementDelay;
            executionTime += measurementsTime;

            //
            // Determine post-measurement time
            //
            executionTime += this.redLion.StopACDriveExecutionTime(RedLion.ACDriveModes.FullLoad);
            executionTime += this.redLion.ConfigureACDriveExecutionTime();

            string logMessage = STRLOG_executionTime + executionTime.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return executionTime;
        }

        //---------------------------------------------------------------------------------------//

        public override ExperimentResultInfo Execute(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "Execute";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Typecast the specification so that it can be used here
            Specification specification = (Specification)experimentSpecification;

            //
            // Create an instance of the result info ready to fill in
            //
            ResultInfo resultInfo = new ResultInfo();
            resultInfo.statusCode = StatusCodes.Running;

            //
            // Initialise measurements
            //
            int measurementCount = this.redLion.MeasurementCount;
            RedLion.Measurements[] measurements = new RedLion.Measurements[measurementCount];
            int measurementNumber = 0;

            //
            // Run a state machine which allows the execution to be cancelled or terminate due to an error
            //
            States state = States.sCreateConnection;
            bool done = false;
            while (true)
            {
                switch (state)
                {
                    case States.sCreateConnection:

                        //
                        // Create a connection to the RedLion controller
                        //
                        if (this.redLion.CreateConnection() == true)
                        {
                            state = States.sResetACDrive;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        done = true;
                        break;

                    case States.sResetACDrive:

                        //
                        // Check if experiment is cancelled
                        //
                        if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                        {
                            resultInfo.statusCode = StatusCodes.Cancelled;
                            state = States.sCloseConnection;
                            break;
                        }

                        //
                        // Reset AC drive
                        //
                        if (this.redLion.ResetACDrive() == true)
                        {
                            state = States.sConfigureACDrive;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sCloseConnection;
                        break;

                    case States.sConfigureACDrive:

                        //
                        // Check if experiment is cancelled
                        //
                        if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                        {
                            resultInfo.statusCode = StatusCodes.Cancelled;
                            done = true;
                            break;
                        }

                        //
                        // Configure AC drive
                        //
                        RedLion.ACDriveConfiguration setConfig = new RedLion.ACDriveConfiguration(
                            0, 0, ACDrive.DEFAULT_SpeedRampTime,
                            ACDrive.MAXIMUM_MaximumCurrent, ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);
                        if (this.redLion.ConfigureACDrive(setConfig) == true)
                        {
                            state = States.sStartACDrive;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sRestoreDefaultsACDrive;
                        break;

                    case States.sStartACDrive:

                        //
                        // Check if experiment is cancelled
                        //
                        if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                        {
                            resultInfo.statusCode = StatusCodes.Cancelled;
                            state = States.sRestoreDefaultsACDrive;
                            break;
                        }

                        //
                        // Start AC drive
                        //
                        if (this.redLion.StartACDrive(RedLion.ACDriveModes.FullLoad) == true)
                        {
                            measurementNumber = 0;
                            state = States.sTakeMeasurement;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sStopACDrive;
                        break;

                    case States.sTakeMeasurement:

                        //
                        // Check if experiment is cancelled
                        //
                        if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                        {
                            resultInfo.statusCode = StatusCodes.Cancelled;
                            state = States.sStopACDrive;
                            break;
                        }

                        if (measurementNumber == 0)
                        {
                            Logfile.Write(STRLOG_StartingMeasurements);
                        }

                        //
                        // Check if all measurements have been taken
                        //
                        if (measurementNumber == measurementCount)
                        {
                            //
                            // Average the measurements
                            //
                            for (int i = 0; i < measurementCount; i++)
                            {
                                resultInfo.voltage += measurements[i].voltage;
                                resultInfo.current += measurements[i].current;
                                resultInfo.powerFactor += measurements[i].powerFactor;
                                resultInfo.speed += measurements[i].speed;
                                resultInfo.torque += measurements[i].torque;
                            }
                            resultInfo.voltage /= measurementCount;
                            resultInfo.current /= measurementCount;
                            resultInfo.powerFactor /= measurementCount;
                            resultInfo.speed /= measurementCount;
                            resultInfo.torque /= measurementCount;

                            Logfile.Write(STRLOG_FinishedMeasurements);

                            state = States.sStopACDrive;
                            break;
                        }

                        //
                        // Take a measurement
                        //
                        if (this.redLion.TakeMeasurement(this.redLion.MeasurementDelay, ref measurements[measurementNumber]) == true)
                        {
                            // Measurement taken successfully
                            measurementNumber++;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sStopACDrive;
                        break;

                    case States.sStopACDrive:

                        //
                        // Stop the AC drive
                        //
                        if (this.redLion.StopACDrive(RedLion.ACDriveModes.FullLoad) == false)
                        {
                            //
                            // Failed
                            //
                            if (resultInfo.statusCode == StatusCodes.Running)
                            {
                                resultInfo.statusCode = StatusCodes.Failed;
                                resultInfo.errorMessage = this.redLion.LastError;
                            }
                        }
                        state = States.sRestoreDefaultsACDrive;
                        break;

                    case States.sRestoreDefaultsACDrive:

                        //
                        // Restore the AC drive configuration
                        //
                        RedLion.ACDriveConfiguration resetConfig = new RedLion.ACDriveConfiguration(
                            0, 0, ACDrive.DEFAULT_SpeedRampTime,
                            ACDrive.DEFAULT_MaximumCurrent, ACDrive.DEFAULT_MaximumTorque, ACDrive.DEFAULT_MinimumTorque);
                        if (this.redLion.ConfigureACDrive(resetConfig) == false)
                        {
                            //
                            // Failed
                            //
                            if (resultInfo.statusCode == StatusCodes.Running)
                            {
                                resultInfo.statusCode = StatusCodes.Failed;
                                resultInfo.errorMessage = this.redLion.LastError;
                            }
                        }
                        else
                        {
                            //
                            // Finished
                            //
                            if (resultInfo.statusCode == StatusCodes.Running)
                            {
                                resultInfo.statusCode = StatusCodes.Completed;
                            }
                        }
                        state = States.sCloseConnection;
                        break;

                    case States.sCloseConnection:

                        //
                        // Close the connection to the RedLion controller
                        //
                        this.redLion.CloseConnection();
                        done = true;
                        break;
                }

                //
                // Check if state machine is finished
                //
                if (done == true)
                {
                    // Exit state machine
                    break;
                }
            }

            string logMessage = STRLOG_statusCode + resultInfo.statusCode;

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return resultInfo;
        }

    }
}
