using System;
//using System.Diagnostics;
//using System.Threading;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServer.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public class DriverSpeedVsField : DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverSpeedVsField";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_FieldMin = " Field Min: ";
        private const string STRLOG_FieldMax = " Field Max: ";
        private const string STRLOG_FieldStep = " Field Step: ";
        private const string STRLOG_VectorLength = " VectorLength: ";
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
            sResetACDrive, sResetDCDriveMut, sConfigureACDrive, sConfigureDCDriveMut, sStartACDrive, sStartDCDriveMut,
            sTakeMeasurements, sTakeMeasurement,
            sStopDCDriveMut, sStopACDrive, sRestoreDefaultsDCDriveMut, sRestoreDefaultsACDrive,
            sCloseConnection
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverSpeedVsField(int unitId)
            : this(unitId, null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverSpeedVsField(int unitId, CancelExperiment cancelExperiment)
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
            executionTime += this.redLion.ResetDCDriveMutExecutionTime();
            executionTime += this.redLion.ConfigureACDriveExecutionTime();
            executionTime += this.redLion.ConfigureDCDriveMutExecutionTime();
            executionTime += this.redLion.StartACDriveExecutionTime();
            executionTime += this.redLion.StartDCDriveMutExecutionTime(RedLion.StartDCDriveMutModes.Speed);

            //
            // Determine measurement time
            //
            int vectorLength = ((specification.Field.max - specification.Field.min) / specification.Field.step) + 1;
            int measurementsTime = this.redLion.SetFieldDCDriveMutExecutionTime();
            measurementsTime += this.redLion.MeasurementCount * this.redLion.MeasurementDelay;
            executionTime += (vectorLength * measurementsTime) + this.redLion.SetFieldDCDriveMutExecutionTime();

            //
            // Determine post-measurement time
            //
            executionTime += this.redLion.StopDCDriveMutExecutionTime();
            executionTime += this.redLion.StopACDriveExecutionTime();
            executionTime += this.redLion.ConfigureDCDriveMutExecutionTime();
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

            // Create an instance of the result info ready to fill in
            ResultInfo resultInfo = new ResultInfo();

            //
            // Create arrays to hold the results
            //
            int vectorLength = (specification.Field.max - specification.Field.min) / specification.Field.step + 1;
            resultInfo.speedVector = new int[vectorLength];
            resultInfo.voltageVector = new int[vectorLength];
            resultInfo.loadVector = new int[vectorLength];
            resultInfo.fieldVector = new float[vectorLength];

            string logMessage = STRLOG_FieldMin + specification.Field.min.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_FieldMax + specification.Field.max.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_FieldStep + specification.Field.step.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_VectorLength + vectorLength.ToString();
            Logfile.Write(logMessage);

            //
            // Initialise measurements
            //
            int vectorIndex = 0;
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
                            state = States.sResetDCDriveMut;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sCloseConnection;
                        break;

                    case States.sResetDCDriveMut:

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
                        // Reset DC drive
                        //
                        if (this.redLion.ResetDCDriveMut() == true)
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
                        RedLion.ACDriveConfiguration setACConfig = new RedLion.ACDriveConfiguration(
                            0, 0, ACDrive.DEFAULT_SpeedRampTime, ACDrive.DEFAULT_MaximumCurrent, 10, -10);
                        if (this.redLion.ConfigureACDrive(setACConfig) == true)
                        {
                            state = States.sConfigureDCDriveMut;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sRestoreDefaultsACDrive;
                        break;

                    case States.sConfigureDCDriveMut:

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
                        // Configure DC drive
                        //
                        RedLion.DCDriveMutConfiguration setDCConfig = new RedLion.DCDriveMutConfiguration(
                            0, 0, DCDriveMut.DEFAULT_MinSpeedLimit, DCDriveMut.DEFAULT_MaxSpeedLimit,
                            DCDriveMut.DEFAULT_MinTorqueLimit, DCDriveMut.DEFAULT_MaxTorqueLimit,
                            DCDriveMut.DEFAULT_SpeedRampTime, DCDriveMut.DEFAULT_Field);
                        if (this.redLion.ConfigureDCDriveMut(setDCConfig) == true)
                        {
                            state = States.sStartACDrive;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sRestoreDefaultsDCDriveMut;
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
                        if (this.redLion.StartACDrive() == true)
                        {
                            state = States.sStartDCDriveMut;
                            break;
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sStopACDrive;
                        break;

                    case States.sStartDCDriveMut:

                        //
                        // Check if experiment is cancelled
                        //
                        if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                        {
                            resultInfo.statusCode = StatusCodes.Cancelled;
                            state = States.sStopACDrive;
                            break;
                        }

                        //
                        // Start DC drive
                        //
                        if (this.redLion.StartDCDriveMut(RedLion.StartDCDriveMutModes.Speed) == true)
                        {
                            if (this.redLion.SetSpeedDCDriveMut(750) == true)
                            {
                                state = States.sTakeMeasurements;
                                break;
                            }
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sStopDCDriveMut;
                        break;

                    case States.sTakeMeasurements:

                        if (vectorIndex == 0)
                        {
                            Logfile.Write(STRLOG_StartingMeasurements);
                        }

                        //
                        // Check if all measurements steps have been taken
                        //
                        if (vectorIndex == vectorLength)
                        {
                            Logfile.Write(STRLOG_FinishedMeasurements);

                            if (this.redLion.SetFieldDCDriveMut(DCDriveMut.DEFAULT_Field) == false)
                            {
                                //
                                // Failed
                                //
                                resultInfo.statusCode = StatusCodes.Failed;
                                resultInfo.errorMessage = this.redLion.LastError;
                            }

                            state = States.sStopDCDriveMut;
                            break;
                        }

                        //
                        // Set the field percentage
                        //
                        int percent = specification.Field.max - (vectorIndex * specification.Field.step);
                        if (this.redLion.SetFieldDCDriveMut(percent) == true)
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
                        state = States.sStopDCDriveMut;
                        break;

                    case States.sTakeMeasurement:

                        //
                        // Check if experiment is cancelled
                        //
                        if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true)
                        {
                            resultInfo.statusCode = StatusCodes.Cancelled;
                            state = States.sStopDCDriveMut;
                            break;
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
                                resultInfo.speedVector[vectorIndex] += measurements[i].speed;
                                resultInfo.voltageVector[vectorIndex] += measurements[i].voltage;
                                resultInfo.fieldVector[vectorIndex] += measurements[i].fieldCurrent;
                                resultInfo.loadVector[vectorIndex] += measurements[i].load;
                            }
                            resultInfo.speedVector[vectorIndex] /= measurementCount;
                            resultInfo.voltageVector[vectorIndex] /= measurementCount;
                            resultInfo.fieldVector[vectorIndex] /= measurementCount;
                            resultInfo.loadVector[vectorIndex] /= measurementCount;

                            vectorIndex++;
                            state = States.sTakeMeasurements;
                            break;
                        }

                        //
                        // Check fault status
                        //
                        if (this.redLion.CheckActiveFaultACDrive() == true)
                        {
                            //
                            // Take a measurement
                            //
                            if (this.redLion.TakeMeasurement(this.redLion.MeasurementDelay, ref measurements[measurementNumber]) == true)
                            {
                                // Measurement taken successfully
                                measurementNumber++;
                                break;
                            }
                        }

                        //
                        // Failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = this.redLion.LastError;
                        state = States.sStopDCDriveMut;
                        break;

                    case States.sStopDCDriveMut:

                        //
                        // Stop the DC drive
                        //
                        if (this.redLion.StopDCDriveMut() == false)
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
                        state = States.sStopACDrive;
                        break;

                    case States.sStopACDrive:

                        //
                        // Stop the AC drive
                        //
                        if (this.redLion.StopACDrive() == false)
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
                        state = States.sRestoreDefaultsDCDriveMut;
                        break;

                    case States.sRestoreDefaultsDCDriveMut:

                        //
                        // Restore the DC drive configuration
                        //
                        RedLion.DCDriveMutConfiguration resetDCConfig = new RedLion.DCDriveMutConfiguration(
                            0, 0, DCDriveMut.DEFAULT_MinSpeedLimit, DCDriveMut.DEFAULT_MaxSpeedLimit,
                            DCDriveMut.DEFAULT_MinTorqueLimit, DCDriveMut.DEFAULT_MaxTorqueLimit,
                            DCDriveMut.DEFAULT_SpeedRampTime, DCDriveMut.DEFAULT_Field);
                        if (this.redLion.ConfigureDCDriveMut(resetDCConfig) == false)
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

            Logfile.Write(STRLOG_statusCode + resultInfo.statusCode);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return resultInfo;
        }
    }
}
