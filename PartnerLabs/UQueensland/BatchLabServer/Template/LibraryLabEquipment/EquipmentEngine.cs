﻿using System;
using System.Web;
using System.Xml;
using Library.Lab;
using Library.LabEquipment.Engine;
using Library.LabEquipment.Drivers;

namespace Library.LabEquipment
{
    public class EquipmentEngine : LabEquipmentEngine
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "EquipmentEngine";

        //
        // Constants
        //

        //
        // String constants
        //

        //
        // String constants for logfile messages
        //

        //
        // String constants for error messages
        //

        //
        // Local variables
        //

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public EquipmentEngine(string rootFilePath)
            : base(rootFilePath)
        {
            const string STRLOG_MethodName = "EquipmentEngine";

            Logfile.WriteCalled(null, STRLOG_MethodName);

            try
            {
                //
                // YOUR CODE HERE
                //
            }
            catch (Exception ex)
            {
                //
                // Log the message and throw the exception back to the caller
                //
                Logfile.WriteError(ex.Message);
                throw;
            }

            Logfile.WriteCompleted(null, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Enable power to the external equipment.
        /// </summary>
        /// <returns>True if successful.</returns>
        public override bool PowerupEquipment()
        {
            const string STRLOG_MethodName = "PowerupEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            //
            // YOUR CODE HERE
            //

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Initialise the equipment after it has been powered up.
        /// </summary>
        /// <returns>True if successful.</returns>
        public override bool InitialiseEquipment()
        {
            const string STRLOG_MethodName = "InitialiseEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = false;

            try
            {
                //
                // YOUR CODE HERE
                //

                success = true;
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //---------------------------------------------------------------------------------------//

        /// <summary>
        /// Disable power to the external equipment.
        /// </summary>
        /// <returns>True if successful.</returns>
        public override bool PowerdownEquipment()
        {
            const string STRLOG_MethodName = "PowerdownEquipment";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            bool success = true;

            //
            // YOUR CODE HERE
            //

            string logMessage = STRLOG_Success + success.ToString();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return success;
        }

        //-------------------------------------------------------------------------------------------------//

        public override LabStatus GetLabEquipmentStatus()
        {
            const string STRLOG_MethodName = "GetLabEquipmentStatus";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // YOUR CODE HERE
            LabStatus labStatus = new LabStatus(true, STR_LabEquipmentIsReady);
            //

            string logMessage = STRLOG_Online + labStatus.online.ToString() +
                Logfile.STRLOG_Spacer + STRLOG_StatusMessage + Logfile.STRLOG_Quote + labStatus.labStatusMessage + Logfile.STRLOG_Quote;

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return labStatus;
        }

        //-------------------------------------------------------------------------------------------------//

        public override ExecuteCommandInfo ProcessCommand(ExecuteCommandInfo executeCommandInfo)
        {
            const string STRLOG_MethodName = "ProcessCommand";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            CommandInfo commandInfo = (CommandInfo)executeCommandInfo;

            bool success = true;
            string errorMessage = null;

            try
            {
                //
                // Process the execute command
                //
                ExecuteCommands executeCommand = (ExecuteCommands)commandInfo.command;

                switch (executeCommand)
                {
                    //
                    // YOUR CODE HERE
                    //
                    case ExecuteCommands.DoSomething:
                        // Remove this command in your implementation
                        break;

                    default:

                        //
                        // Unknown command
                        //
                        errorMessage = STRERR_UnknownCommand + executeCommand.ToString();
                        success = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
            }

            //
            // Update success of command execution
            //
            executeCommandInfo.success = success;

            string logMessage = STRLOG_Success + success.ToString();
            if (success == false)
            {
                executeCommandInfo.errorMessage = errorMessage;
                logMessage += Logfile.STRLOG_Spacer + STRLOG_ErrorMessage + errorMessage;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName, logMessage);

            return executeCommandInfo;
        }

        //-------------------------------------------------------------------------------------------------//

        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //
                // Dispose managed resources here. Anything that has a Dispose() method.
                //
                //
                // YOUR CODE HERE
                //
            }

            //
            // Release unmanaged resources here. Set large fields to null.
            //

            // Call Dispose on your base class.
            base.Dispose(disposing);
        }

    }
}
