﻿using System;
using System.Xml;
using Library.Lab;
using Library.LabEquipment.Engine;
using Library.LabEquipment.Drivers;

namespace Library.LabEquipment
{
    public class EquipmentManager : LabEquipmentManager
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "EquipmentManager";

        #endregion

        //-------------------------------------------------------------------------------------------------//

        public EquipmentManager(string rootFilePath)
            : base(rootFilePath)
        {
        }

        //-------------------------------------------------------------------------------------------------//

        public override void Create()
        {
            const string STRLOG_MethodName = "Create";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // Create an instance of lab equipment engine
            //
            this.labEquipmentEngine = new EquipmentEngine(this.rootFilePath);

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //-------------------------------------------------------------------------------------------------//

        public override LabStatus GetLabEquipmentStatus()
        {
            const string STRLOG_MethodName = "GetLabEquipmentStatus";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            LabStatus labStatus = this.labEquipmentEngine.GetLabEquipmentStatus();

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return labStatus;
        }

        //-------------------------------------------------------------------------------------------------//

        public override string ExecuteXmlRequest(string xmlRequest)
        {
            string strXmlResponse = string.Empty;

            const string STRLOG_MethodName = "ExecuteXmlRequest";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            EquipmentEngine equipmentEngine = (EquipmentEngine)this.labEquipmentEngine;

            try
            {
                bool success = true;
                string errorMessage = string.Empty;
                CommandInfo commandInfo = null;
                CommandInfo resultInfo = null;

                //
                // Create the XML response
                //
                XmlDocument xmlResponseDocument = new XmlDocument();
                XmlElement xmlElement = xmlResponseDocument.CreateElement(Engine.Consts.STRXML_Response);
                xmlResponseDocument.AppendChild(xmlElement);

                //
                // Add success of command execution and update later
                //
                xmlElement = xmlResponseDocument.CreateElement(Engine.Consts.STRXML_RspSuccess);
                xmlElement.InnerText = success.ToString();
                xmlResponseDocument.DocumentElement.AppendChild(xmlElement);

                //
                // Parse XML request for the command
                //
                XmlDocument xmlRequestDocument = new XmlDocument();
                xmlRequestDocument.LoadXml(xmlRequest);
                XmlNode xmlRequestNode = XmlUtilities.GetXmlRootNode(xmlRequestDocument, Engine.Consts.STRXML_Request);
                string strCommand = XmlUtilities.GetXmlValue(xmlRequestNode, Engine.Consts.STRXML_Command, false);

                NonExecuteCommands nonExecuteCommand = (NonExecuteCommands)(-1);
                ExecuteCommands executeCommand = (ExecuteCommands)(-1);
                try
                {
                    //
                    // Try to convert to a non-execute command type that doesn't require powerdown to be suspended
                    //
                    nonExecuteCommand = (NonExecuteCommands)Enum.Parse(typeof(NonExecuteCommands), strCommand);
                    Logfile.Write(STRLOG_Command + nonExecuteCommand.ToString());
                }
                catch
                {
                    try
                    {
                        //
                        // Try to convert to an execute command type that does require powerdown to be suspended
                        //
                        executeCommand = (ExecuteCommands)Enum.Parse(typeof(ExecuteCommands), strCommand);
                        Logfile.Write(STRLOG_Command + executeCommand.ToString());

                        //
                        // Check that powerdown has been suspended before executing the command
                        //
                        if (this.labEquipmentEngine.IsPowerdownSuspended == false)
                        {
                            //
                            // Suspend the powerdown before executing the command
                            //
                            this.labEquipmentEngine.SuspendPowerdown();
                        }
                    }
                    catch
                    {
                        //
                        // Unknown command
                        //
                        success = false;
                        errorMessage = STRERR_UnknownCommand + strCommand;
                    }
                }

                if (success == true && nonExecuteCommand != (NonExecuteCommands)(-1))
                {
                    //
                    // Process the non-execute command
                    //
                    switch (nonExecuteCommand)
                    {
                        case NonExecuteCommands.GetTimeOfDayTime:

                            //
                            // Get the time to reset the AC drive
                            //
                            int timeOfDayTime = equipmentEngine.GetTimeOfDayTime();

                            //
                            // Add the time to the response
                            //
                            xmlElement = xmlResponseDocument.CreateElement(Consts.STRXML_RspTimeOfDayTime);
                            xmlElement.InnerText = timeOfDayTime.ToString();
                            xmlResponseDocument.DocumentElement.AppendChild(xmlElement);
                            break;
                    }
                }
                else if (success == true && executeCommand != (ExecuteCommands)(-1))
                {
                    //
                    // Only one execute command allowed at a time
                    //
                    lock (this.managerLock)
                    {
                        //
                        // Create an instance of the command info ready to fill in
                        //
                        commandInfo = new CommandInfo(executeCommand);

                        //
                        // Process the execute command
                        //
                        switch (executeCommand)
                        {
                            case ExecuteCommands.GetTimeOfDay:

                                //
                                // Get the server URL from request
                                //
                                string strServerUrl = XmlUtilities.GetXmlValue(xmlRequestNode, Consts.STRXML_ReqServerUrl, false);

                                //
                                // Get the time-of-day from the specified server
                                //
                                commandInfo.parameters = new object[] { strServerUrl };
                                resultInfo = (CommandInfo)equipmentEngine.ExecuteCommand((ExecuteCommandInfo)commandInfo);
                                if (commandInfo.success == true)
                                {
                                    //
                                    // Get time-of-day from the result
                                    //
                                    DateTime timeOfDay = (DateTime)commandInfo.results[0];

                                    //
                                    // Add the time-of-day to the response
                                    //
                                    xmlElement = xmlResponseDocument.CreateElement(Consts.STRXML_RspTimeOfDay);
                                    xmlElement.InnerText = timeOfDay.ToBinary().ToString();
                                    xmlResponseDocument.DocumentElement.AppendChild(xmlElement);
                                }
                                break;
                        }

                        success = resultInfo.success;
                        errorMessage = resultInfo.errorMessage;
                    }
                }

                //
                // Update success of command execution
                //
                XmlNode xmlResponseNode = XmlUtilities.GetXmlRootNode(xmlResponseDocument, Engine.Consts.STRXML_Response);
                XmlUtilities.SetXmlValue(xmlResponseNode, Engine.Consts.STRXML_RspSuccess, success.ToString(), false);
                if (success == false)
                {
                    //
                    // Create error response
                    //
                    xmlElement = xmlResponseDocument.CreateElement(Engine.Consts.STRXML_RspErrorMessage);
                    xmlElement.InnerText = errorMessage;
                    xmlResponseDocument.DocumentElement.AppendChild(xmlElement);
                }
                strXmlResponse = xmlResponseDocument.InnerXml;
            }
            catch (ArgumentException)
            {
                //
                // Unknown command so pass to base class to process
                //
                strXmlResponse = base.ExecuteXmlRequest(xmlRequest);
            }
            catch (Exception ex)
            {
                Logfile.Write(ex.Message);
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return strXmlResponse;
        }

    }
}
