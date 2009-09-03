using System;
using System.Diagnostics;
using System.Threading;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServer.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public class DriverRadioactivityVsTime : DriverGeneric
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "DriverRadioactivityVsTime";

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
        private const string STRLOG_ReturningSource = " Returning source to home position";
        private const string STRLOG_ReturningAbsorber = " Returning absorber to home position";
        private const string STRLOG_ReturningTube = " Returning tube to home position";

        //
        // String constants for serial LCD messages
        //
        private const string STRLCD_PreExecute = "PreExecute: ";
        private const string STRLCD_MovingTube = "Move tube ";
        private const string STRLCD_Millimetres = "mm";
        private const string STRLCD_SelectAbsorber = "Select absorber";
        private const string STRLCD_SelectSource = "Select source";
        private const string STRLCD_Execute = "Execute: ";
        private const string STRLCD_PostExecute = "PostExecute: ";
        private const string STRLCD_ReturnSource = "Return source";
        private const string STRLCD_ReturnAbsorber = "Return absorber";
        private const string STRLCD_ReturnTube = "Return tube";
        private const string STRLCD_Seconds = "sec";
        private const string STRLCD_Break = "-";
        private const string STRLCD_Of = "/";
        private const string STRLCD_EmptyString = "";

        //
        // Local variables
        //
        private LabServerToRadioactivityAPI labServerToRadioactivityAPI;
        private RadioactivityService radioactivity;

        #endregion

        //---------------------------------------------------------------------------------------//

        public DriverRadioactivityVsTime()
            : this(null)
        {
        }

        //---------------------------------------------------------------------------------------//

        public DriverRadioactivityVsTime(CancelExperiment cancelExperiment)
            : base(cancelExperiment)
        {
            const string STRLOG_MethodName = "DriverRadioactivityVsTime";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            //
            // Create an interface to radioactivity equipment service
            //
            this.labServerToRadioactivityAPI = new LabServerToRadioactivityAPI();
            this.radioactivity = this.labServerToRadioactivityAPI.Radioactivity;

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public override int GetExecutionTime(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "GetExecutionTime";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            // Typecast the specification so that it can be used here
            Specification specification = (Specification)experimentSpecification;

            int executionTime = 0;

            //
            // Determine time until equipment is ready to use
            //
            EquipmentResult result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetTimeUntilReady, 0, 0, null);
            executionTime += result.intValue;

            //
            // Determine pre-measurement time
            //
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetTubeHomeDistance, 0, 0, null);
            int tubeHomeDistance = result.intValue;
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetTubeMoveTime, tubeHomeDistance, specification.Distance, null);
            executionTime += result.intValue;
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetAbsorberSelectTime, specification.AbsorberLocation, 0, null);
            executionTime += result.intValue;
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetSourceSelectTime, specification.SourceLocation, 0, null);
            executionTime += result.intValue;

            //
            // Determine measurement time
            //
            executionTime += specification.Duration * specification.Repeat;

            //
            // Determine post-measurement time
            //
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetSourceReturnTime, specification.SourceLocation, 0, null);
            executionTime += result.intValue;
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetAbsorberReturnTime, specification.AbsorberLocation, 0, null);
            executionTime += result.intValue;
            result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetTubeMoveTime, specification.Distance, tubeHomeDistance, null);
            executionTime += result.intValue;

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
            resultInfo.dataVectors = new int[1, vectorLength];

            Logfile.Write(STRLOG_Distance + specification.Distance.ToString());
            Logfile.Write(STRLOG_Duration + specification.Duration.ToString());
            Logfile.Write(STRLOG_Repeat + specification.Repeat.ToString());
            Logfile.Write(STRLOG_VectorLength + vectorLength.ToString());

            //
            // Suspend equipment powerdown
            //
            EquipmentResult result = this.radioactivity.ExecuteCommand(EquipmentCommands.SuspendPowerdown, 0, 0, null);

            try
            {
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 1, 0, STRLCD_PreExecute);

                //
                // Move tube into position
                //
                Logfile.Write(STRLOG_MovingTube + specification.Distance.ToString() + STRLOG_Millimetres);
                string lcdMessage = STRLCD_MovingTube + specification.Distance.ToString() + STRLCD_Millimetres;
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, lcdMessage);
                this.radioactivity.ExecuteCommand(EquipmentCommands.SetTubeDistance, specification.Distance, 0, null);

                //
                // Move absorber into position
                //
                Logfile.Write(STRLOG_SelectingAbsorber + specification.AbsorberName);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, STRLCD_SelectAbsorber);
                this.radioactivity.ExecuteCommand(EquipmentCommands.SetAbsorberLocation, specification.AbsorberLocation, 0, null);

                //
                // Move source into position
                //
                Logfile.Write(STRLOG_SelectingSource + specification.SourceName);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, STRLCD_SelectSource);
                this.radioactivity.ExecuteCommand(EquipmentCommands.SetSourceLocation, specification.SourceLocation, 0, null);

                //
                // Start data capture
                //
                Logfile.Write(STRLOG_StartingMeasurements);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 1, 0, STRLCD_Execute);

                for (int i = 0; i < specification.Repeat; i++)
                {
                    //
                    // Display details on the LCD
                    //
                    lcdMessage = specification.Distance.ToString() + STRLCD_Millimetres;
                    lcdMessage += STRLCD_Break + specification.Duration.ToString() + STRLCD_Seconds;
                    lcdMessage += STRLCD_Break + (i + 1).ToString() + STRLCD_Of + specification.Repeat.ToString();
                    this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, lcdMessage);
                    Trace.WriteLine(lcdMessage);

                    //
                    // Store the data
                    //
                    result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetCaptureData, specification.Duration, 0, null);
                    resultInfo.dataVectors[0, i] = result.intValue;

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

                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 1, 0, STRLCD_PostExecute);

                //
                // Return source to home position
                //
                Logfile.Write(STRLOG_ReturningSource);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, STRLCD_ReturnSource);
                result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetSourceHomeLocation, 0, 0, null);
                this.radioactivity.ExecuteCommand(EquipmentCommands.SetSourceLocation, result.intValue, 0, null);

                //
                // Return absorber to home position
                //
                Logfile.Write(STRLOG_ReturningAbsorber);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, STRLCD_ReturnAbsorber);
                result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetAbsorberHomeLocation, 0, 0, null);
                this.radioactivity.ExecuteCommand(EquipmentCommands.SetAbsorberLocation, result.intValue, 0, null);

                //
                // Return tube to home position
                //
                Logfile.Write(STRLOG_ReturningTube);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, STRLCD_ReturnTube);
                result = this.radioactivity.ExecuteCommand(EquipmentCommands.GetTubeHomeDistance, 0, 0, null);
                this.radioactivity.ExecuteCommand(EquipmentCommands.SetTubeDistance, result.intValue, 0, null);

                //
                // Clear display
                //
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 1, 0, STRLCD_EmptyString);
                this.radioactivity.ExecuteCommand(EquipmentCommands.WriteLcdLine, 2, 0, STRLCD_EmptyString);

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

            //
            // Resume equipment powerdown
            //
            this.radioactivity.ExecuteCommand(EquipmentCommands.ResumePowerdown, 0, 0, null);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return resultInfo;
        }
    }
}
