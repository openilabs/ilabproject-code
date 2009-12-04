using System;
using System.Diagnostics;
using System.Xml;
using Library.Lab;
using Library.LabServerEngine;
using Library.LabServerEngine.Drivers.Setup;
using Library.LabServerEngine.Drivers.Equipment;

namespace Library.LabServer.Drivers.Setup
{
    public partial class DriverEquipment : DriverEquipmentGeneric
    {
        #region Class Constants and Variables

        //
        // String constants for logfile messages
        //

        //
        // String constants for error messages
        //

        //
        // Data driven state machine for Execute()
        //
        private enum States_Execute
        {
            sSuspendPowerdown,
            //
            // YOUR CODE HERE
            //
            sResumePowerdown,
            sCompleted,
        }
        private struct SMTableEntry_Execute
        {
            public States_Execute currentState;
            public States_Execute nextState;
            public States_Execute exitState;
            public string equipmentCommand;
            public string[,] commandArguments;

            public SMTableEntry_Execute(States_Execute currentState, States_Execute nextState, States_Execute exitState,
                string equipmentCommand, string[,] commandArguments)
            {
                this.currentState = currentState;
                this.nextState = nextState;
                this.exitState = exitState;
                this.equipmentCommand = equipmentCommand;
                this.commandArguments = commandArguments;
            }
        }
        private SMTableEntry_Execute[] smTable_Execute = new SMTableEntry_Execute[] {
            //
            // Suspend powerdown
            //
            new SMTableEntry_Execute(States_Execute.sSuspendPowerdown, States_Execute.sResumePowerdown, States_Execute.sResumePowerdown,
                null, null),

            //
            // YOUR CODE HERE
            //

            //
            // Resume powerdown
            //
            new SMTableEntry_Execute(States_Execute.sResumePowerdown, States_Execute.sCompleted, States_Execute.sCompleted,
                null, null),
        };

        #endregion

        //---------------------------------------------------------------------------------------//

        public override ExperimentResultInfo Execute(ExperimentSpecification experimentSpecification)
        {
            const string STRLOG_MethodName = "Execute";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Determine how long it actually take to execute
            //
            DateTime startDateTime = DateTime.Now;

            // Typecast the specification so that it can be used here
            Specification specification = (Specification)experimentSpecification;

            //
            // Log the specification
            //
            string logMessage = string.Empty;
            //
            // YOUR CODE HERE
            //
            Logfile.Write(logMessage);

            //
            // Create an instance of the result info ready to fill in
            //
            ResultInfo resultInfo = new ResultInfo();
            resultInfo.statusCode = StatusCodes.Running;

            //
            // Create data structures to hold the results
            //
            //
            // YOUR CODE HERE
            //

            //
            // Initialise variables used in the state machine
            //
            //
            // YOUR CODE HERE
            //

            try
            {
                //
                // First, check to see if the LabEquipment is online
                //
                LabEquipmentStatus labEquipmentStatus = this.equipmentServiceProxy.GetLabEquipmentStatus();
                if (labEquipmentStatus.online == false)
                {
                    throw new Exception(labEquipmentStatus.statusMessage);
                }

                //
                // Run the state machine to execute the experiment specification
                //
                States_Execute state = States_Execute.sCompleted;
                if (smTable_Execute.Length > 0)
                {
                    state = smTable_Execute[0].currentState;
                }
                while (state != States_Execute.sCompleted)
                {
                    //
                    // Find table entry
                    //
                    int index = -1;
                    for (int i = 0; i < smTable_Execute.Length; i++)
                    {
                        if (smTable_Execute[i].currentState == state)
                        {
                            // Entry found
                            index = i;
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        throw new ArgumentOutOfRangeException(state.ToString(), STRERR_StateNotFound);
                    }

                    //
                    // Get table entry and save next state
                    //
                    SMTableEntry_Execute entry = smTable_Execute[index];
                    States_Execute nextState = entry.nextState;

                    logMessage = " [ " + STRLOG_MethodName + ": " + entry.currentState.ToString() + " ]";
                    Logfile.Write(logMessage);

                    Trace.WriteLine(logMessage);

                    //
                    // Check if experiment was cancelled
                    //
                    if (this.cancelExperiment != null && this.cancelExperiment.IsCancelled == true &&
                        resultInfo.statusCode == StatusCodes.Running)
                    {
                        //
                        // Experiment was cancelled
                        //
                        resultInfo.statusCode = StatusCodes.Cancelled;
                        state = entry.exitState;
                        continue;
                    }

                    //
                    // Process non-XML commands
                    //
                    switch (entry.currentState)
                    {
                        case States_Execute.sSuspendPowerdown:
                            if (this.equipmentServiceProxy.SuspendPowerdown() == false)
                            {
                                //
                                // Command execution failed
                                //
                                resultInfo.statusCode = StatusCodes.Failed;
                                resultInfo.errorMessage = STRERR_SuspendPowerdown;
                                state = entry.exitState;
                            }
                            else
                            {
                                state = nextState;
                            }
                            continue;

                        case States_Execute.sResumePowerdown:
                            if (this.equipmentServiceProxy.ResumePowerdown() == false)
                            {
                                //
                                // Command execution failed
                                //
                                resultInfo.statusCode = StatusCodes.Failed;
                                resultInfo.errorMessage = STRERR_ResumePowerdown;
                                state = entry.exitState;
                            }
                            else
                            {
                                state = nextState;
                            }
                            continue;

                        default:
                            break;
                    }

                    //
                    // Add command arguments where required
                    //
                    switch (entry.currentState)
                    {
                        //
                        // YOUR CODE HERE
                        //

                        default:
                            break;
                    }

                    //
                    // Execute command and check response success
                    //
                    XmlDocument xmlRequestDocument = CreateXmlRequestDocument(entry.equipmentCommand, entry.commandArguments);
                    string xmlResponse = this.equipmentServiceProxy.ExecuteRequest(xmlRequestDocument.InnerXml);
                    XmlNode xmlResponseNode = CreateXmlResponseNode(xmlResponse);
                    if (XmlUtilities.GetBoolValue(xmlResponseNode, LabServerEngine.Consts.STRXML_RspSuccess, false) == false)
                    {
                        //
                        // Command execution failed
                        //
                        resultInfo.statusCode = StatusCodes.Failed;
                        resultInfo.errorMessage = XmlUtilities.GetXmlValue(xmlResponseNode, LabServerEngine.Consts.STRXML_RspErrorMessage, true);
                        state = entry.exitState;
                        continue;
                    }

                    //
                    // Extract response values where required
                    //
                    switch (entry.currentState)
                    {
                        //
                        // YOUR CODE HERE
                        //

                        default:
                            break;
                    }

                    //
                    // Next state
                    //
                    state = nextState;
                }

                //
                // Update status code
                //
                if (resultInfo.statusCode == StatusCodes.Running)
                {
                    resultInfo.statusCode = StatusCodes.Completed;
                }
            }
            catch (Exception ex)
            {
                resultInfo.statusCode = StatusCodes.Failed;
                resultInfo.errorMessage = ex.Message;
                Logfile.WriteError(ex.Message);
            }

            //
            // Calculate actual execution time
            //
            TimeSpan timeSpan = DateTime.Now - startDateTime;

            logMessage = STRLOG_StatusCode + resultInfo.statusCode
                + Logfile.STRLOG_Spacer + STRLOG_ExecutionTime + timeSpan.TotalSeconds.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return resultInfo;
        }

    }
}
