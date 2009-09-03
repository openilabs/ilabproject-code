using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Library.LabEquipment;
using LabEquipment.Drivers;

namespace LabEquipment
{
    /// <summary>
    /// AuthHeader - Class that defines the Authentication Header object. For each WebMethod call, an instance of
    /// this class, containing the caller's server ID and passkey will be passed in the header of the SOAP Request.
    /// </summary>
    [XmlType(Namespace = "http://ilab.uq.edu.au/")]
    [XmlRoot(Namespace = "http://ilab.uq.edu.au/", IsNullable = false)]
    public class AuthHeader : SoapHeader
    {
        public string identifier;
        public string passKey;
    }

    /// <summary>
    /// Summary description for RadioactivityService
    /// </summary>
    [WebService(Namespace = "http://ilab.uq.edu.au/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class RadioactivityService : System.Web.Services.WebService
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "RadioactivityService";

        //
        // String constants for logfile messages
        //
        private const string STRLOG_AccessDenied = "Access Denied!";
        private const string STRLOG_UnauthorisedAccess = "Unauthorised Access!";
        private const string STRLOG_EquipmentCommand = " Equipment Command: ";
        private const string STRLOG_UnknownCommand = "Unknown Command";
        private const string STRLOG_Distance = " Distance: ";
        private const string STRLOG_From = " From: ";
        private const string STRLOG_To = " To: ";
        private const string STRLOG_Duration = " Duration: ";
        private const string STRLOG_Location = " Location: ";
        private const string STRLOG_LcdLineNo = " Line No: ";
        private const string STRLOG_LcdMessage = " Message: ";
        private const string STRLOG_CaptureTimeout = " Capture timeout!";
        private const string STRLOG_InvalidData = " Invalid data!";
        private const string STRLOG_Result = " Result: ";
        private const string STRLOG_Spacer = " | ";

        //
        // Local variables
        //
        private EquipmentEngine equipmentEngine;
        private FlexMotion flexMotion;
        private SerialLcd serialLcd;

        public enum EquipmentCommands
        {
            GetTimeUntilReady,
            GetTubeHomeDistance, GetTubeMoveTime,
            GetSourceHomeLocation, GetSourceSelectTime, GetSourceReturnTime,
            GetAbsorberHomeLocation, GetAbsorberSelectTime, GetAbsorberReturnTime,
            SuspendPowerdown, ResumePowerdown,
            SetTubeDistance, SetSourceLocation, SetAbsorberLocation,
            GetCaptureData,
            WriteLcdLine
        }

        public struct EquipmentResult
        {
            public bool boolValue;
            public int intValue;
            public string errorMessage;
        }

        public AuthHeader authHeader;

        #endregion

        //---------------------------------------------------------------------------------------//

        public RadioactivityService()
        {
            authHeader = new AuthHeader();

            this.equipmentEngine = Global.equipmentEngine;
            this.flexMotion = Global.equipmentEngine.DeviceFlexMotion;
            this.serialLcd = Global.equipmentEngine.DeviceSerialLcd;
        }

        //---------------------------------------------------------------------------------------//

        [WebMethod]
        [SoapHeader("authHeader", Direction = SoapHeaderDirection.In)]
        public EquipmentResult ExecuteCommand(EquipmentCommands command, int param1, int param2, string param3)
        {
            //const string STRLOG_MethodName = "ExecuteCommand";

            //Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            EquipmentResult result = new EquipmentResult();

            //
            // Check caller access is authorised
            //
            if (Authorised(authHeader) == false)
            {
                result.errorMessage = STRLOG_AccessDenied;
                result.boolValue = false;
                return result;
            }

            string logMessage = " " + command.ToString() + STRLOG_Spacer;

            //
            // Execute the command
            //
            switch ((EquipmentCommands)command)
            {
                case EquipmentCommands.GetTimeUntilReady:

                    result.intValue = this.equipmentEngine.TimeUntilReady;
                    result.boolValue = true;

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.GetTubeHomeDistance:

                    result.intValue = this.flexMotion.TubeHomeDistance;

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.GetTubeMoveTime:

                    logMessage += STRLOG_From + param1.ToString() + STRLOG_Spacer;
                    logMessage += STRLOG_To + param2.ToString() + STRLOG_Spacer;

                    result.intValue = this.flexMotion.GetTubeMoveTime(param1, param2);

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.GetSourceHomeLocation:

                    result.intValue = this.flexMotion.SourceHomeLocation;

                    logMessage += STRLOG_Result + ((char)result.intValue).ToString();
                    break;

                case EquipmentCommands.GetSourceSelectTime:

                    logMessage += STRLOG_Location + ((char)param1).ToString() + STRLOG_Spacer;

                    result.intValue = this.flexMotion.GetSourceSelectTime((char)param1);

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.GetSourceReturnTime:

                    logMessage += STRLOG_Location + ((char)param1).ToString() + STRLOG_Spacer;

                    result.intValue = this.flexMotion.GetSourceReturnTime((char)param1);

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.GetAbsorberHomeLocation:

                    result.intValue = this.flexMotion.AbsorberHomeLocation;

                    logMessage += STRLOG_Result + ((char)result.intValue).ToString();
                    break;

                case EquipmentCommands.GetAbsorberSelectTime:

                    logMessage += STRLOG_Location + ((char)param1).ToString() + STRLOG_Spacer;

                    result.intValue = this.flexMotion.GetAbsorberSelectTime((char)param1);

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.GetAbsorberReturnTime:

                    logMessage += STRLOG_Location + ((char)param1).ToString() + STRLOG_Spacer;

                    result.intValue = this.flexMotion.GetAbsorberReturnTime((char)param1);

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.SuspendPowerdown:

                    result.boolValue = this.equipmentEngine.SuspendPowerdown();

                    logMessage += STRLOG_Result + result.boolValue.ToString();
                    break;

                case EquipmentCommands.ResumePowerdown:

                    result.boolValue = this.equipmentEngine.ResumePowerdown();

                    logMessage += STRLOG_Result + result.boolValue.ToString();
                    break;

                case EquipmentCommands.SetTubeDistance:

                    logMessage += STRLOG_Distance+ param1.ToString() + STRLOG_Spacer;

                    result.boolValue = this.flexMotion.SetTubeDistance(param1);

                    logMessage += STRLOG_Result + result.boolValue.ToString();
                    break;

                case EquipmentCommands.SetSourceLocation:

                    logMessage += STRLOG_Location + ((char)param1).ToString() + STRLOG_Spacer;

                    result.boolValue = this.flexMotion.SetSourceLocation((char)param1);

                    logMessage += STRLOG_Result + result.boolValue.ToString();
                    break;

                case EquipmentCommands.SetAbsorberLocation:

                    logMessage += STRLOG_Location + ((char)param1).ToString() + STRLOG_Spacer;

                    result.boolValue = this.flexMotion.SetAbsorberLocation((char)param1);

                    logMessage += STRLOG_Result + result.boolValue.ToString();
                    break;

                case EquipmentCommands.GetCaptureData:

                    logMessage += STRLOG_Duration + param1.ToString() + STRLOG_Spacer;

                    result.intValue = CaptureData(param1);

                    logMessage += STRLOG_Result + result.intValue.ToString();
                    break;

                case EquipmentCommands.WriteLcdLine:
                    logMessage += STRLOG_LcdLineNo + param1.ToString() + STRLOG_Spacer;
                    logMessage += STRLOG_LcdMessage + param3 + STRLOG_Spacer;

                    result.boolValue = this.serialLcd.WriteLine(param1, param3);

                    logMessage += STRLOG_Result + result.boolValue.ToString();
                    break;

                default:
                    result.errorMessage = STRLOG_UnknownCommand;
                    break;
            }

            if (result.errorMessage == null)
            {
                Logfile.Write(logMessage);
            }
            else
            {
                Logfile.WriteError(result.errorMessage);
            }

            //Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);

            return result;
        }

        //---------------------------------------------------------------------------------------//

        private int CaptureData(int duration)
        {
            this.serialLcd.StartCapture(duration);

            //
            // Use a timeout and retries
            //
            int retries = 3;
            int data = -1;

            for (int j = 0; j < retries; j++)
            {
                //
                // Check for data, but use a timeout
                //
                int timeout = (duration + 3) * 2;
                while (true)
                {
                    // Get capture data from serial LCD
                    data = this.serialLcd.CaptureData;

                    // Check if data received
                    if (data >= 0)
                    {
                        // Data received
                        break;
                    }

                    // Not data yet, check timeout
                    if (--timeout == 0)
                    {
                        // Timed out
                        break;
                    }

                    //
                    // Wait for data
                    //
                    Thread.Sleep(500);
                    Trace.Write(".");
                }

                if (timeout == 0)
                {
                    Logfile.Write(STRLOG_CaptureTimeout);
                }
                else if (data < 0)
                {
                    Logfile.Write(STRLOG_InvalidData);
                }
                else
                {
                    // Data captured successfully
                    break;
                }
            }

            this.serialLcd.StopCapture();

            return data;
        }

        private bool Authorised(AuthHeader authHeader)
        {
            bool ok = true;

            if (Global.allowedCallers.IsAuthenticating == true)
            {
                //
                // Check if caller is specified
                //
                if (authHeader == null || authHeader.identifier == null || authHeader.passKey == null)
                {
                    // Caller is not specified
                    ok = false;
                    Logfile.Write(STRLOG_AccessDenied);
                }

                //
                // Check if access is authorised for this caller
                //
                else if (Global.allowedCallers.Authentication(authHeader.identifier, authHeader.passKey) == null)
                {
                    // Caller is not allowed access to this Web Method
                    ok = false;
                    Logfile.Write(STRLOG_UnauthorisedAccess);
                }
            }

            return ok;
        }

    }
}
