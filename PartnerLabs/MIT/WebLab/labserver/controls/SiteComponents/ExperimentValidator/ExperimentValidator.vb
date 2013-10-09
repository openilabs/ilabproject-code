Imports System
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading
Imports System.Diagnostics
Imports System.Collections
Imports Microsoft.VisualBasic
Imports WebLabDataManagers
Imports WebLabSystemComponents
Imports WebLabCustomDataTypes

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

Namespace WebLabSystemComponents

    'Author(s): James Hardison (hardison@alum.mit.edu)
    'Date: 4/13/2005
    'Date Modified: 4/13/2005
    'This class library is a rebuilt version of the original experiment_validator class in WebLab 6.0.  This library contains the logic
    'used to validate incoming experiment specifications.  This process requires three steps.  First, the input XML Experiment Specification
    'document must be decoded.  The ExperimentParser class in the WebLabSystemComponents Namespace performs this funtion.  Second, it must be 
    'confirmed tha the agent submitting the experiment has sufficient access to the device profile referenced in the specification.  This is 
    'performed using the ResourcePermissionManager class in the WebLabDataManagers Namespace.  Finally, the specification itself must be
    'checked for correctness.  This final step will be performed by a series of private methods.  There will be a single public function 
    'in this class that will, in turn, delegate work to individual private and/or external methods.  The syntax of 4155 User Defined 
    'Functions are checked by an external java console application.  
    '
    'To use this class, make sure the compiled DLL is placed in the /bin directory of your ASP.NET application and, in your ASP.NET code, 
    'import the "WebLabSystemComponents" namespace.  This method references the WebLabDataManagers namespace, the ExperimentParser 
    'class in the WebLabSystem Components namespace and the WebLabCustomDataTypes namespace.
    '
    'Used By:
    '   Lab Server Web Service Interface (/services/WebLabServices/LabServerAPI.vb, /bin/WebLabServices.dll)
    '   Validation Engine Library (/bin/ExperimentValidator.dll) - this code compiled
    '
    'Uses:
    '   Resource Permission Manager (/controls/WebLabDataManagers/ResourcePermissionManager.vb, /bin/ResourcePermissionManager.dll)
    '   Experiment Parser (/controls/WebLabSystemComponents/ExperimentParser/ExperimentParser.vb, /bin/ExperimentParser.dll)
    '   User Defined Function Validator (/java/src/weblab/validation/validation/java, /java/bin/weblab/validation/validation.class)
    '   WebLab Custom Data Types (/controls/WebLabCustomDataTypes/WebLabCustomDataTypes.vb, /bin/WebLabCustomTypes.dll)

    Public Class ExperimentValidator


        Dim strNameList, strSitePath As String
        Dim intDeviceNumber, intPriority As Integer
        Dim rpmObject As New ResourcePermissionManager()
        Dim conWebLabLS As SqlConnection

        Private intDeviceProfileID As Integer = 0
        Private intResourceID As Integer = 0
        Private strVar1TermKey As String = ""
        Private strVar2TermKey As String = ""
        Private strVar1PTermKey As String = ""


        Public Function validate(ByVal strXMLExpSpec As String, ByVal intBrokerID As Integer, ByVal strCallerPath As String) As ValidationObject
            'This method governs the job validation process.  the specified experiment specification is validated within the
            'permission context of the supplied Broker ID.  strCallerPath is the location of the webroot of the caller web process.  
            'Private methods performing validation steps are called in the appropriate order by this method.  Upon completion, 
            'a validation object is returned.  This object contians the following fields:
            '
            'isValid - a boolean field indicating whether the job was successfully validated
            'errorMessage - a string field containing any error messages attending a validation failure. If isValid is True, 
            '   this field will be empty.
            'dataPoints - an integer value containing the calculated datapoints the job will generate.  The value will be 
            '   0 if isValid is False.
            'deviceProfileID - an integer value indicating the Lab Server Device Profile ID of the device to be tested by the 
            '   experiment specification.  If isValid is False, this value will be 0.
            'resourceID - an integer value indicating the Lab Server Resource ID of the device to be tested by the experiment
            '   specification.  If isValid is False, this value will be 0.

            Dim strAccessOut, strErrorMsg As String
            Dim voValidateOut As ValidationObject
            Dim eoParserOut As ExperimentObject
            Dim epObject As New ExperimentParser()

            strSitePath = strCallerPath
            conWebLabLS = New SqlConnection("Server=localhost;Database=WebLabServicesLS;Trusted_Connection=True")

            'first, parse the incoming XML spec and get the parsed data object.  Use try-catch here to handle parsing errors.
            Try
                eoParserOut = epObject.parseXMLExpSpec(strXMLExpSpec)
            Catch
                strErrorMsg = "Error - An error was generated while parsing the Experiment Specification.  Please make sure the document is valid and well formed.  (Source: " & Err.Source() & ", Description: " & Err.Description() & ")"
                voValidateOut = New ValidationObject(False, strErrorMsg, 0, 0, 0)
                Return voValidateOut
                Exit Function
            End Try

            'open database connection
            conWebLabLS.Open()

            'next, confirm that the supplied broker has permission to use the specified device
            strAccessOut = brokerHasDevicePermission(intBrokerID, CInt(eoParserOut.Item("DeviceNum"))) '<-NEED TO CODE THIS!!!

            If Not strAccessOut = "SUCCESS" Then
                conWebLabLS.Close()
                voValidateOut = New ValidationObject(False, strAccessOut, 0, 0, 0)
                Return voValidateOut
                Exit Function
            End If

            'finally, validate the specification
            voValidateOut = ExperimentValidator(eoParserOut) '<-- NEED TO CODE THIS!!!

            conWebLabLS.Close()
            Return voValidateOut
        End Function

        Public Function validate(ByVal strXMLExpSpec As String, ByVal intBrokerID As Integer, ByVal intGroupID As Integer, ByVal strCallerPath As String) As ValidationObject
            'This method governs the job validation process.  the specified experiment specification is validated within the
            'permission context of the supplied Broker and Group IDs.  strCallerPath is the location of the webroot of the 
            'caller web process.  Private methods performing validation steps are called in the appropriate order by this 
            'method.  Upon completion, a validation object is returned.  This object contians the following fields:
            '
            'isValid - a boolean field indicating whether the job was successfully validated
            'errorMessage - a string field containing any error messages attending a validation failure. If isValid is True, 
            '   this field will be empty.
            'dataPoints - an integer value containing the calculated datapoints the job will generate.  The value will be 
            '   0 if isValid is False.
            'deviceProfileID - an integer value indicating the Lab Server Device Profile ID of the device to be tested by the 
            '   experiment specification.  If isValid is False, this value will be 0.
            'resourceID - an integer value indicating the Lab Server Resource ID of the device to be tested by the experiment
            '   specification.  If isValid is False, this value will be 0.

            Dim strAccessOut, strErrorMsg As String
            Dim voValidateOut As ValidationObject
            Dim eoParserOut As ExperimentObject
            Dim epObject As New ExperimentParser()

            strSitePath = strCallerPath
            conWebLabLS = New SqlConnection("Server=localhost;Database=WebLabServicesLS;Trusted_Connection=True")

            'first, parse the incoming XML spec and get the parsed data object.  Use try-catch here to handle parsing errors.
            Try
                eoParserOut = epObject.parseXMLExpSpec(strXMLExpSpec)
            Catch
                strErrorMsg = "Error - An error was generated while parsing the Experiment Specification.  Please make sure the document is valid and well formed.  (Source: " & Err.Source() & ", Description: " & Err.Description() & ")"
                voValidateOut = New ValidationObject(False, strErrorMsg, 0, 0, 0)
                Return voValidateOut
                Exit Function
            End Try

            'open database connection
            conWebLabLS.Open()

            'next, confirm that the supplied broker has permission to use the specified device
            strAccessOut = groupHasDevicePermission(intGroupID, CInt(eoParserOut.Item("DeviceNum"))) '<-- NEED TO CODE THIS!!!

            If Not strAccessOut = "SUCCESS" Then
                strAccessOut = brokerHasDevicePermission(intBrokerID, CInt(eoParserOut.Item("DeviceNum"))) '<-NEED TO CODE THIS!!!

                If Not strAccessOut = "SUCCESS" Then
                    conWebLabLS.Close()
                    voValidateOut = New ValidationObject(False, strAccessOut, 0, 0, 0)
                    Return voValidateOut
                    Exit Function
                End If
            End If

            'finally, validate the specification
            voValidateOut = ExperimentValidator(eoParserOut) '<-- NEED TO CODE THIS!!!

            conWebLabLS.Close()
            Return voValidateOut

        End Function

        '---private class methods---

        Private Function brokerHasDevicePermission(ByVal intBrokerID As Integer, ByVal intDeviceNum As Integer) As String
            'This method checks if the specified broker has read access to the device profile referenced by intDeviceNumber
            'View permission is required for device execution.
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim blnQueryResult As Boolean

            strDBQuery = "SELECT a.profile_id, p.resource_id FROM ActiveDevices a JOIN DeviceProfiles p ON a.profile_id = p.profile_id WHERE a.number = @deviceNumber"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@deviceNumber", intDeviceNum + 1) '+1 makes number 1-based for DB
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            intDeviceProfileID = CInt(dtrDBQuery("profile_id"))
            intResourceID = CInt(dtrDBQuery("resource_id"))

            dtrDBQuery.Close()

            blnQueryResult = rpmObject.GetBrokerResourcePermission(intBrokerID, intResourceID, "canview")

            If blnQueryResult Then
                Return "SUCCESS"
            Else
                Return "Error - The specified broker does not have permission to use the specified device."
            End If
        End Function

        Private Function groupHasDevicePermission(ByVal intGroupID As Integer, ByVal intDeviceNum As Integer) As String
            'This method checks if the specified group has read access to the device profile referenced by intDeviceNumber.
            'View permission is required for device execution.
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim blnQueryResult As Boolean

            strDBQuery = "SELECT a.profile_id, p.resource_id FROM ActiveDevices a JOIN DeviceProfiles p ON a.profile_id = p.profile_id WHERE a.number = @deviceNumber"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@deviceNumber", intDeviceNum + 1) '+1 makes number 1-based for DB
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            intDeviceProfileID = CInt(dtrDBQuery("profile_id"))
            intResourceID = CInt(dtrDBQuery("resource_id"))

            dtrDBQuery.Close()

            blnQueryResult = rpmObject.GetGroupResourcePermission(intGroupID, intResourceID, "canview")

            If blnQueryResult Then
                Return "SUCCESS"
            Else
                Return "Error - The specified group does not have permission to use the specified device."
            End If
        End Function

        Private Function ExperimentValidator(ByVal eoParsedExp As ExperimentObject) As ValidationObject
            'This function validates a parsed experiment specification.  In particular, this method validates the input specification either by checking
            'it directly (in the case of one time checks or those that must have a validation-global scope) or by delegating to helper methods (in the case of
            'complex or commonly repeated checks).  The purpose of this method is to check a submitted specification for known hardware error cases as well as
            'system-imposed contraints.  The input specification must meet all of the following encoded rules and conditions in order to be validated and 
            'placed into the Experiment Queue.  The input to this method is an Experiment Object (see WebLabCustomDataTypes) with top-level keys of "DeviceNum" 
            '(a string indicating the device number being addressed by the specification), "TermTable" (an ExpUnitSetObject (see WebLabCustomDataTypes) which is 
            'a object that contains all of the terminal/function specifications, keyed first on an integer index and then on individual data item), and "UDFTable" 
            '(an ExpUnitSetObject that contains all of the user defined function specifications, keyed first on and integer index and then on individual data 
            'item(units And body)).
            '
            'The output of this function is a validation object contianing the following fields:
            '
            'isValid - a boolean field indicating whether the job was successfully validated
            'errorMessage - a string field containing any error messages attending a validation failure. If isValid is True, 
            '   this field will be empty.
            'dataPoints - an integer value containing the calculated datapoints the job will generate.  The value will be 
            '   0 if isValid is False.
            'deviceProfileID - an integer value indicating the Lab Server Device Profile ID of the device to be tested by the 
            '   experiment specification.  If isValid is False, this value will be 0.  At the time this method is called, this value is held in the class 
            '   variable "intDeviceProfileID".
            'resourceID - an integer value indicating the Lab Server Resource ID of the device to be tested by the experiment
            '   specification.  If isValid is False, this value will be 0.  At the time this method is called, this value is held in the class variable
            '   "intResourceID".

            Dim strDBQuery, strCheckResult As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim intDataPoints As Integer
            Dim hstTermCheckResult As Hashtable

            'Check for consistency between specification and basic device terminal configuration (includes name checking for UDFs).
            strCheckResult = DeviceTypeCheck(eoParsedExp.Item("TermTable"))

            If strCheckResult <> "SUCCESS" Then
                Return New ValidationObject(False, strCheckResult, 0, 0, 0)
                Exit Function
            End If


            'Perform self-consistency and error checks on the specification as a whole.
            strCheckResult = SpecificationCheck(eoParsedExp.Item("TermTable"), eoParsedExp.Item("UDFTable"))

            If strCheckResult <> "SUCCESS" Then
                Return New ValidationObject(False, strCheckResult, 0, 0, 0)
                Exit Function
            End If


            'Perform individual terminal checks.
            hstTermCheckResult = TerminalCheck(eoParsedExp.Item("TermTable"))

            If hstTermCheckResult.Item("result") <> "SUCCESS" Then
                Return New ValidationObject(False, hstTermCheckResult.Item("result"), 0, 0, 0)
                Exit Function
            Else
                intDataPoints = hstTermCheckResult.Item("datapoints")
            End If


            'Perform in-depth UDF checks
            If Not eoParsedExp.Item("UDFTable") Is Nothing Then
                strCheckResult = UDFCheck(eoParsedExp.Item("UDFTable"))
            End If

            If strCheckResult <> "SUCCESS" Then
                Return New ValidationObject(False, strCheckResult, 0, 0, 0)
                Exit Function
            End If

            'create and return validation object (success)
            Return New ValidationObject(True, "", intDataPoints, intDeviceProfileID, intResourceID)

        End Function

        Private Function DeviceTypeCheck(ByVal TermTable As ExpUnitSetObject) As String
            'This method checks the device type configuration specified in the experimentSpecification with that stored in the lab server database.  
            Dim strDBQuery, strTermKeyArray() As String
            Dim cmdDBQuery As SqlCommand
            Dim intDeviceTypeID, loopIdx As Integer

            'get reference to Device Type associated with the specified Device Profile ID (private property set by public validate governor method).
            strDBQuery = "SELECT t.type_id FROM DeviceTypes t JOIN DeviceProfiles p ON t.type_id = p.type_id WHERE p.profile_id = @profileID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@profileID", intDeviceProfileID)
            intDeviceTypeID = CInt(cmdDBQuery.ExecuteScalar())

            'Check that the specified terminals match those in the specified profile/type.
            'First, make sure that the appropriate number of terminals are specified.
            strDBQuery = "SELECT terminals_used FROM DeviceTypes WHERE type_id = @typeID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@typeID", intDeviceTypeID)

            If CInt(cmdDBQuery.ExecuteScalar()) <> TermTable.Count() Then
                Return "Error - Experiment Specification does not match profile, terminal number mismatch."
                Exit Function
            End If

            'Next, check that each terminal matches a terminal type record for the specified device.
            ReDim strTermKeyArray(TermTable.Count() - 1)
            strTermKeyArray = TermTable.EnumerateUnits()

            For loopIdx = 0 To TermTable.Count() - 1
                'check that the current terminal matches a terminal type record for the specified type
                strDBQuery = "SELECT 'error' WHERE  NOT EXISTS(SELECT typeterm_id FROM DeviceTypeTerminalConfig WHERE type_ID = @typeID AND port = @port);"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@typeID", intDeviceTypeID)
                cmdDBQuery.Parameters.Add("@port", TermTable.UnitItem(strTermKeyArray(loopIdx), "port"))

                If cmdDBQuery.ExecuteScalar() = "error" Then
                    Return "Error = Experiment Specification does not match profile, terminal type mismatch."
                    Exit Function
                End If
            Next

            'if no errors have been returned...
            Return "SUCCESS"

        End Function

        Private Function SpecificationCheck(ByVal TermTable As ExpUnitSetObject, ByVal UDFTable As ExpUnitSetObject) As String
            'This method performs checks on the correctness of individual terminal information where the context of the entire specification
            'is necessary (such as checking for terminal name uniqueness).
            Dim strDBQuery, strTermKeyArray(), strUDFKeyArray(), strCurrentKey, strCurrentPort, strCurrentPortType, strCheckResult As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim loopIdx, intDLCount As Integer

            intDLCount = 0
            strNameList = "|"

            ReDim strTermKeyArray(TermTable.Count() - 1)
            strTermKeyArray = TermTable.EnumerateUnits()

            For loopIdx = 0 To TermTable.Count() - 1
                strCurrentKey = strTermKeyArray(loopIdx)

                'retreive port type info for this terminal
                strCurrentPort = TermTable.UnitItem(strCurrentKey, "port")
                strCurrentPortType = Left(strCurrentPort, 3)

                'First, check that each specified terminal is well defined (has the appropriate fields) for its declared port type.
                'Port types: SMU, VSU, VMU
                'Below is a listing of terminal definition properties (as keyed in the TermTable structure) along with the port types 
                'that use them.
                '-vname: SMU, VSU, VMU
                '-dlVTerm: SMU, VSU, VMU
                '-iname: SMU
                '-dlITerm: SMU
                '-mode: SMU
                '-functionType: SMU, VSU
                '-compliance: SMU

                'check for the existence of the other properties on a per-port-type basis (select implicitly checks port value.
                Select Case UCase(strCurrentPortType)
                    Case "SMU"
                        'Check that a vname exists, and that a download value is specified (all port types have a vname field).
                        If Trim(TermTable.UnitItem(strCurrentKey, "vname")) = "" Then
                            Return "Error - Port " & strCurrentPort & "requires a VName."
                            Exit Function

                        ElseIf Trim(TermTable.UnitItem(strCurrentKey, "dlVTerm")) = "" Then
                            Return "Error - The download status of the " & strCurrentPort & " voltage measurement must be specified."
                            Exit Function

                            'check that iname parameter exists
                        ElseIf Trim(TermTable.UnitItem(strCurrentKey, "iname")) = "" Then
                            Return "Error - Port " & strCurrentPort & " requires an IName."
                            Exit Function

                            'check that iname download value is specified
                        ElseIf Trim(TermTable.UnitItem(strCurrentKey, "dlITerm")) = "" Then
                            Return "Error - The download status of the " & strCurrentPort & " current measurement must be specified."
                            Exit Function

                            'check that proper terminal mode is specified
                        ElseIf Not (UCase(Trim(TermTable.UnitItem(strCurrentKey, "mode"))) = "V" Or UCase(Trim(TermTable.UnitItem(strCurrentKey, "mode"))) = "I" Or UCase(Trim(TermTable.UnitItem(strCurrentKey, "mode"))) = "COMM") Then
                            Return "Error - Port " & strCurrentPort & " requires a valid mode declaration."
                            Exit Function

                        ElseIf UCase(Trim(TermTable.UnitItem(strCurrentKey, "mode"))) <> "COMM" Then

                            'check that a function type is specified (if mode is not "COMM")
                            If Trim(TermTable.UnitItem(strCurrentKey, "functionType")) = "" Then
                                Return "Error - Port " & strCurrentPort & " requires a valid function declaration."
                                Exit Function

                                'check that a terminal compliance is specified (if mode is not "COMM")
                            ElseIf Trim(TermTable.UnitItem(strCurrentKey, "compliance")) = "" Or Not IsNumeric(TermTable.UnitItem(strCurrentKey, "compliance")) Then
                                Return "Error - Port " & strCurrentPort & " requires a numeric compliance value."
                                Exit Function

                            End If


                        End If
                    Case "VSU"
                        'Check that a vname exists, and that a download value is specified (all port types have a vname field).
                        If Trim(TermTable.UnitItem(strCurrentKey, "vname")) = "" Then
                            Return "Error - Port " & strCurrentPort & "requires a VName."
                            Exit Function

                        ElseIf Trim(TermTable.UnitItem(strCurrentKey, "dlVTerm")) = "" Then
                            Return "Error - The download status of the " & strCurrentPort & " voltage measurement must be specified."
                            Exit Function

                            'check that a function type is specified
                        ElseIf Trim(TermTable.UnitItem(strCurrentKey, "functionType")) = "" Then
                            Return "Error - Port " & strCurrentPort & " requires a valid function declaration."
                            Exit Function
                        End If
                    Case "VMU"
                        'Check that a vname exists, and that a download value is specified (all port types have a vname field).
                        If Trim(TermTable.UnitItem(strCurrentKey, "vname")) = "" Then
                            Return "Error - Port " & strCurrentPort & "requires a VName."
                            Exit Function

                        ElseIf Trim(TermTable.UnitItem(strCurrentKey, "dlVTerm")) = "" Then
                            Return "Error - The download status of the " & strCurrentPort & " voltage measurement must be specified."
                            Exit Function
                        End If
                    Case Else
                        Return "Error - Port " & strCurrentPort & " is an unrecognized type."
                        Exit Function
                End Select

                'begin checking terminal data for correctness (based on known hardware/software rules and self-consistency)

                'First, check that terminal names are properly formed, unique

                'check vname
                strCheckResult = NameCheck(TermTable.UnitItem(strCurrentKey, "vname"), strNameList, strCurrentPort, False)
                If strCheckResult <> "SUCCESS" Then
                    Return strCheckResult
                    Exit Function
                Else
                    strNameList = "|" & TermTable.UnitItem(strCurrentKey, "vname") & strNameList
                    'check if terminal is for DL, increment counter if so
                    If UCase(TermTable.UnitItem(strCurrentKey, "dlVTerm")) = "TRUE" Then
                        intDLCount = intDLCount + 1
                    End If
                End If

                'if port is an SMU, check iname too
                If strCurrentPortType = "SMU" Then
                    'check iname
                    strCheckResult = NameCheck(TermTable.UnitItem(strCurrentKey, "iname"), strNameList, strCurrentPort, False)
                    If strCheckResult <> "SUCCESS" Then
                        Return strCheckResult
                        Exit Function
                    Else
                        strNameList = "|" & TermTable.UnitItem(strCurrentKey, "iname") & strNameList
                        'check if terminal is for DL, increment counter if so
                        If UCase(TermTable.UnitItem(strCurrentKey, "dlITerm")) = "TRUE" Then
                            intDLCount = intDLCount + 1
                        End If
                    End If
                End If


                'sets up var1, var2, var1p record key pointers for TerminalCheck
                Select Case UCase(TermTable.UnitItem(strCurrentKey, "functionType"))
                    Case "VAR1"
                        'creates a reference to the var1 port key
                        If strVar1TermKey = "" Then
                            strVar1TermKey = strCurrentKey
                        Else
                            'case where more than one terminal is set to var1
                            Return "Error - Only one terminal may be set to VAR1."
                            Exit Function
                        End If
                    Case "VAR2"
                        'creates a reference to the var2 port key
                        If strVar2TermKey = "" Then
                            strVar2TermKey = strCurrentKey
                        Else
                            'case where more than one terminal is set to var1
                            Return "Error - Only one terminal may be set to VAR2."
                            Exit Function
                        End If
                    Case "VAR1P"
                        'creates a reference to the var1p port key
                        If strVar1PTermKey = "" Then
                            strVar1PTermKey = strCurrentKey
                        Else
                            'case where more than one terminal is set to var1p
                            Return "Error - Only one terminal may be set to VAR1P"
                            Exit Function
                        End If
                End Select
            Next


            'check var1, var2, var1p dependencies.
            'first, check that there is a terminal configured as VAR1
            If strVar1TermKey = "" Then
                Return "Error - Exactly one terminal must be set as VAR1."
                Exit Function
            End If

            'next, if var1p is specififed check that var1 and var1p have the same modes
            If strVar1PTermKey <> "" Then
                If TermTable.UnitItem(strVar1TermKey, "mode") <> TermTable.UnitItem(strVar1PTermKey, "mode") Then
                    Return "Error - The VAR1 and VAR1P terminals must have the same mode setting."
                    Exit Function
                End If
            End If

            'do UDF name checking and DL tally checks 
            ReDim strUDFKeyArray(UDFTable.Count() - 1)
            strUDFKeyArray = UDFTable.EnumerateUnits()


            For loopIdx = 0 To UDFTable.Count() - 1
                strCurrentKey = strUDFKeyArray(loopIdx)

                'check UDF names are specified and properly formed.
                If Trim(UDFTable.UnitItem(strCurrentKey, "name")) = "" Then
                    Return "Error - User Defined Functions must be named."
                    Exit Function
                Else
                    strCheckResult = NameCheck(UDFTable.UnitItem(strCurrentKey, "name"), strNameList, "", True)
                    If strCheckResult <> "SUCCESS" Then
                        Return strCheckResult
                        Exit Function
                    Else
                        strNameList = "|" & UDFTable.UnitItem(strCurrentKey, "name") & strNameList
                        'check if UDF is for DL, increment counter if so
                        If UCase(UDFTable.UnitItem(strCurrentKey, "dlUDF")) = "TRUE" Then
                            intDLCount = intDLCount + 1
                        End If
                    End If
                End If

            Next

            If intDLCount < 2 Then
                'checks that at least 2 variables are being downloaded
                Return "Error - At least two variables must be selected for download."
                Exit Function
            ElseIf intDLCount > 8 Then
                'checks that no more than 8 variables are deing downloaded
                Return "Error - No more than eight variables may be selected for download."
                Exit Function
            End If

            'if no errors have been returned...
            Return "SUCCESS"

        End Function

        Private Function TerminalCheck(ByVal TermTable As ExpUnitSetObject) As Hashtable
            'This method perfoms specific data correctness checks on individual terminals/functions.  The output of this method is 
            'a hashable containing two keys.  The first "result" is a string indicating the success of this validation process or 
            'an error message indicating the exact nature of its failure.  The second "datapoints" is an integer value indicating the 
            'total datapoints the specified job will generate.  This second field is only non-zero if the "result" string indicates
            'success.
            Dim strDBQuery, strTermKeyArray(), strCurrentKey, strCurrentPort, strCurrentPortType, strCheckResult, strFunctScale As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim loopIdx, intPointLimit As Integer
            Dim dblCurrentTermMaxV, dblCurrentTermMaxI, dblVar1PStart, dblVar1PStop, dblCompliance As Double
            Dim htOutput As New Hashtable()
            Dim foundRec As Boolean

            Dim intVar1Points As Integer = 0
            Dim intVar2Points As Integer = 0
            Dim intTotalPoints As Integer = 0

            ReDim strTermKeyArray(TermTable.Count() - 1)
            strTermKeyArray = TermTable.EnumerateUnits()

            For loopIdx = 0 To TermTable.Count() - 1
                strCurrentKey = strTermKeyArray(loopIdx)

                'retrieve port type info for this terminal
                strCurrentPort = TermTable.UnitItem(strCurrentKey, "port")
                strCurrentPortType = Left(strCurrentPort, 3)

                If strCurrentPortType = "SMU" Or strCurrentPortType = "VSU" Then

                    'retrieve max datapoints value
                    strDBQuery = "SELECT max_points FROM DeviceProfiles WHERE profile_id = @profileID;"
                    cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                    cmdDBQuery.Parameters.Add("@profileID", intDeviceProfileID)

                    intPointLimit = CInt(cmdDBQuery.ExecuteScalar())

                    'retrieve lab server compliance values for this terminal
                    strDBQuery = "SELECT p.max_voltage, p.max_current FROM DeviceProfileTerminalConfig p JOIN DeviceTypeTerminalConfig t ON p.typeterm_id = t.typeterm_id WHERE p.profile_id = @profileID AND t.port = @port;"
                    cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                    cmdDBQuery.Parameters.Add("@profileID", intDeviceProfileID)
                    cmdDBQuery.Parameters.Add("@port", UCase(strCurrentPort))
                    dtrDBQuery = cmdDBQuery.ExecuteReader()

                    foundRec = False
                    While dtrDBQuery.Read()
                        foundRec = True
                        dblCurrentTermMaxV = dtrDBQuery("max_voltage")
                        dblCurrentTermMaxI = dtrDBQuery("max_current")
                    End While

                    dtrDBQuery.Close()

                    If Not foundRec Then
                        htOutput.Add("result", "Error - Could not retrieve terminal compliance from server, aborting.")
                        htOutput.Add("datapoints", 0)
                        Return htOutput
                        Exit Function
                    End If

                    'set up terminal compliance (if SMU, use supplied.  If VSU, use 100mA)
                    If Left(strCurrentPort, 3) = "VSU" Then
                        dblCompliance = 0.1
                    Else
                        dblCompliance = Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "compliance")))
                    End If

                    'check terminal function settings
                    Select Case UCase(TermTable.UnitItem(strCurrentKey, "functionType"))
                        Case "VAR1"
                            strFunctScale = UCase(TermTable.UnitItem(strCurrentKey, "scale"))

                            If InStr(strFunctScale, "LOG") <> 0 Then
                                strCheckResult = CheckVar1LogFunction(TermTable.Unit(strCurrentKey), strCurrentPort)

                                If strCheckResult <> "SUCCESS" Then
                                    htOutput.Add("result", strCheckResult)
                                    htOutput.Add("datapoints", 0)
                                    Return htOutput
                                    Exit Function
                                Else
                                    'calculate data points for this terminal (log case)
                                    Select Case strFunctScale
                                        Case "LOG10"
                                            intVar1Points = CInt(Math.Ceiling(10 * (Math.Log10(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "stop")))) - Math.Log10(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "start"))))))) + 1
                                        Case "LOG25"
                                            intVar1Points = CInt(Math.Ceiling(25 * (Math.Log10(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "stop")))) - Math.Log10(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "start"))))))) + 1
                                        Case "LOG50"
                                            intVar1Points = CInt(Math.Ceiling(50 * (Math.Log10(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "stop")))) - Math.Log10(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "start"))))))) + 1
                                        Case Else
                                            'unrecognized logarithmic scale
                                            htOutput.Add("result", "Error - Unrecognized scale on " & strCurrentPort & ", aborting.")
                                            htOutput.Add("datapoints", 0)
                                            Return htOutput
                                            Exit Function
                                    End Select
                                End If
                            ElseIf InStr(strFunctScale, "LIN") <> 0 Then
                                strCheckResult = CheckVar1LinFunction(TermTable.Unit(strCurrentKey), strCurrentPort)

                                If strCheckResult <> "SUCCESS" Then
                                    htOutput.Add("result", strCheckResult)
                                    htOutput.Add("datapoints", 0)
                                    Return htOutput
                                    Exit Function
                                Else
                                    'calculate data points for this terminal (linear case)
                                    intVar1Points = CInt(Math.Floor((CDbl(TermTable.UnitItem(strCurrentKey, "stop")) - CDbl(TermTable.UnitItem(strCurrentKey, "start"))) / CDbl(TermTable.UnitItem(strCurrentKey, "step")))) + 1

                                    'check against minimum instrument resolution rules & software device datapoint limit
                                    If intVar1Points > intPointLimit Then
                                        htOutput.Add("result", "Error - The specified number of datapoints is greater than that allowed by the server (" & intPointLimit & "), aborting.")
                                        htOutput.Add("datapoints", 0)
                                        Return htOutput
                                        Exit Function
                                    End If

                                    If intVar1Points > 1001 Then
                                        htOutput.Add("result", "Error - Step resolution too small for " & strCurrentPort & " sweep range.  Reduce the sweep range (stop - start) or increase the step value and try again (Max datapoints for Var1 = 1001).")
                                        htOutput.Add("datapoints", 0)
                                        Return htOutput
                                        Exit Function
                                    End If
                                End If
                            Else
                                'case where function type is unrecognized
                                htOutput.Add("result", "Error - A valid function scale for " & strCurrentPort & " must be specified.")
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If


                            strCheckResult = CheckVarSoftwareRules(TermTable.Unit(strCurrentKey), strCurrentPort, dblCurrentTermMaxV, dblCurrentTermMaxI)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If


                            strCheckResult = CheckVarHardwareRules(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "start"))), Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "stop"))), dblCompliance, UCase(TermTable.UnitItem(strCurrentKey, "mode")), UCase(TermTable.UnitItem(strCurrentKey, "scale")), strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                        Case "VAR2"
                            'var2 only operates in linear scale
                            strCheckResult = CheckVar2Function(TermTable.Unit(strCurrentKey), strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                            'calculate and check var2 datapoints here
                            intVar2Points = CInt(Math.Floor((CDbl(TermTable.UnitItem(strCurrentKey, "stop")) - CDbl(TermTable.UnitItem(strCurrentKey, "start"))) / CDbl(TermTable.UnitItem(strCurrentKey, "step")))) + 1

                            'checks for var2 sweep range limitation compliance
                            If intVar2Points > intPointLimit Then
                                htOutput.Add("result", "Error - The specified number of datapoints is greater than that allowed by the server (" & intPointLimit & " datapoints), aborting.")
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            ElseIf intVar2Points > 128 Then
                                htOutput.Add("result", "Error - The specified sweep range for " & strCurrentPort & " is too large.  Maximum range for VAR2 is 128 datapoints.")
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                            'check software compliance rules
                            strCheckResult = CheckVarSoftwareRules(TermTable.Unit(strCurrentKey), strCurrentPort, dblCurrentTermMaxV, dblCurrentTermMaxI)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                            'check hardware compliance, resolution rules (VAR2 is always in LIN scale)
                            strCheckResult = CheckVarHardwareRules(Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "start"))), Math.Abs(CDbl(TermTable.UnitItem(strCurrentKey, "stop"))), dblCompliance, UCase(TermTable.UnitItem(strCurrentKey, "mode")), "LIN", strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                        Case "VAR1P"
                            'var1p follows var1 sweep (must validate factor and offset values only
                            strCheckResult = CheckVar1PFunction(TermTable.Unit(strCurrentKey), strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If


                            'first calculate aggregate start/stop values created when VAR1 values are adjusted by VAR1P ratio and offset
                            dblVar1PStart = Math.Abs((CDbl(TermTable.UnitItem(strVar1TermKey, "start")) * CDbl(TermTable.UnitItem(strCurrentKey, "ratio"))) + CDbl(TermTable.UnitItem(strCurrentKey, "offset")))
                            dblVar1PStop = Math.Abs((CDbl(TermTable.UnitItem(strVar1TermKey, "stop")) * CDbl(TermTable.UnitItem(strCurrentKey, "ratio"))) + CDbl(TermTable.UnitItem(strCurrentKey, "offset")))

                            strCheckResult = CheckVar1PSoftwareRules(dblVar1PStart, dblVar1PStop, dblCompliance, strCurrentPort, UCase(TermTable.UnitItem(strVar1TermKey, "mode")), dblCurrentTermMaxV, dblCurrentTermMaxI)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                            'check hardware imposed rules.
                            'Use aggregate source values, use VAR1P values for port compliance, use VAR1 values for mode and scale 
                            strCheckResult = CheckVarHardwareRules(dblVar1PStart, dblVar1PStop, dblCompliance, UCase(TermTable.UnitItem(strVar1TermKey, "mode")), UCase(TermTable.UnitItem(strVar1TermKey, "scale")), strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                        Case "CONS"
                            'only need to check value field
                            strCheckResult = CheckConsFunction(TermTable.Unit(strCurrentKey), strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If

                            strCheckResult = CheckConsSoftwareRules(TermTable.Unit(strCurrentKey), dblCompliance, strCurrentPort, dblCurrentTermMaxV, dblCurrentTermMaxI)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If


                            strCheckResult = CheckConsHardwareRules(TermTable.Unit(strCurrentKey), dblCompliance, strCurrentPort)

                            If strCheckResult <> "SUCCESS" Then
                                htOutput.Add("result", strCheckResult)
                                htOutput.Add("datapoints", 0)
                                Return htOutput
                                Exit Function
                            End If
                        Case Else
                    End Select

                End If
            Next

            'function wrap up
            'calculate total job datapoints and check against limit.
            If intVar2Points = 0 Then   'case where spec. only uses var1
                intTotalPoints = intVar1Points
            Else
                intTotalPoints = intVar1Points * intVar2Points
            End If

            'check that total datapoints does not exceed max allowed
            If intTotalPoints > intPointLimit Then
                htOutput.Add("result", "Error - The specified number of datapoints is greater than that allowed by the server (" & intPointLimit & " datapoints), aborting.")
                htOutput.Add("datapoints", 0)
                Return htOutput
                Exit Function
            End If


            'If all checks are passed...
            htOutput.Add("result", "SUCCESS")
            htOutput.Add("datapoints", intTotalPoints)
            Return htOutput

        End Function

        Private Function NameCheck(ByVal termName As String, ByVal usedNameList As String, ByVal port As String, ByVal isUDF As Boolean) As String
            'This method checks the supplied terminal name against a number of correctness rules to determine is the name if 4155-legal.  
            'Additionally, the name is checked against the supplied used name list to determine if the supplied name is unique.
            'If the name is determined correct and unique, the string "SUCCESS" is returned.  Otherwise, an error message is returned
            'to the caller.
            Dim strLName, strUName, strEMLeadin As String
            Dim intNameLength, loopIdx As Integer
            Dim blnNameIsAlphaNum As Boolean

            'setup Port/UDF specific error message lead-in
            If isUDF Then
                strEMLeadin = "Error - User Defined Function """ & termName & """"
            Else
                strEMLeadin = "Error - The name """ & termName & """ on port " & port
            End If

            'begin checks
            'check that name is less than 6 characters long (4155)
            If Len(termName) > 6 Then
                Return strEMLeadin & " is invalid.  Names must be 6 characters or less."
                Exit Function
            End If

            'check that the leading character is not a number (4155)
            If IsNumeric(Left(termName, 1)) Then
                Return strEMLeadin & " is invalid.  Names cannot lead with a numeric character."
                Exit Function
            End If

            'check that only alphanumeric characters are used
            strLName = LCase(termName)
            strUName = UCase(termName)
            intNameLength = Len(termName)
            blnNameIsAlphaNum = True

            For loopIdx = 1 To intNameLength
                If InStr(strLName, GetChar(strUName, loopIdx), CompareMethod.Binary) <> 0 And Not IsNumeric(GetChar(strUName, loopIdx)) Then
                    blnNameIsAlphaNum = False
                    Exit For
                End If
            Next

            If Not blnNameIsAlphaNum Then
                Return strEMLeadin & " is invalid.  Names must be strictly alphanumeric."
                Exit Function
            End If

            'checks that the provided name is unique (delimiters used to avoid name - name substring false positives - e.g. "Vterm" and "term")
            If InStr(usedNameList, "|" & termName & "|") <> 0 Then
                Return strEMLeadin & " is invalid.  Duplicate names are not allowed."
                Exit Function
            End If

            'case where name passes all conditions
            Return "SUCCESS"

        End Function

        Private Function CheckVar1LogFunction(ByVal TermInfo As Hashtable, ByVal strCurrentPort As String) As String
            'This function checks that the terminal information encoded in the specified hashtable is approprate for configuring a logarithmic scale 
            'VAR1 function on the 4155 hardware.  This includes basic, function-specific field format and dependency checks.

            'check that a start value exists and is numeric
            If Trim(TermInfo.Item("start")) = "" Or Not IsNumeric(TermInfo.Item("start")) Then
                Return "Error - A numeric start value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that a stop value exists and is numeric
            If Trim(TermInfo.Item("stop")) = "" Or Not IsNumeric(TermInfo.Item("stop")) Then
                Return "Error - A numeric stop value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the start value for this function is positive and non-zero.
            If CDbl(TermInfo.Item("start")) <= 0 Then
                Return "Error - A positive, non-zero start value is required for " & strCurrentPort & ", aborting."
                Exit Function
            End If

            'check that the stop value for this function is positive and non-zero
            If CDbl(TermInfo.Item("stop")) <= 0 Then
                Return "Error - A positive, non-zero stop value is required for " & strCurrentPort & ", aborting."
                Exit Function
            End If

            'check that the stop value is greater than the start value. (depends on previous checks)
            If CDbl(TermInfo.Item("stop")) - CDbl(TermInfo.Item("start")) = 0 Then
                Return "Error - The stop value for " & strCurrentPort & " must be greater than the start value, aborting."
                Exit Function
            End If

            'case where all conditions are passed
            Return "SUCCESS"

        End Function

        Private Function CheckVar1LinFunction(ByVal TermInfo As Hashtable, ByVal strCurrentPort As String) As String
            'This function checks that the terminal information encoded in the specified hashtable is approprate for configuring a linear scale 
            'VAR1 function on the 4155 hardware.  This includes basic, function-specific field format and dependency checks.
            Dim dblSignCheck As Double

            'check that the start value exists and is numeric
            If Trim(TermInfo.Item("start")) = "" Or Not IsNumeric(TermInfo.Item("start")) Then
                Return "Error - A numeric start value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the stop value exists and is numeric
            If Trim(TermInfo.Item("stop")) = "" Or Not IsNumeric(TermInfo.Item("stop")) Then
                Return "Error - A numeric stop value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the step value exists and is numeric
            If Trim(TermInfo.Item("step")) = "" Or Not IsNumeric(TermInfo.Item("step")) Then
                Return "Error - A numeric step value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the step value is non-zero.
            If CDbl(TermInfo.Item("step")) = 0 Then
                Return "Error - The step value for " & strCurrentPort & " must be non-zero, aborting."
                Exit Function
            End If

            'check that the step value is properly signed (if stop is greater than start, should be positive.  if start is greater than stop, 
            'should be negative.)
            dblSignCheck = (CDbl(TermInfo.Item("stop")) - CDbl(TermInfo.Item("start"))) * CDbl(TermInfo.Item("step"))

            If dblSignCheck <= 0 Then
                Return "Error - The step value for " & strCurrentPort & " must have the same sign as (stop - start), aborting."
                Exit Function
            End If

            'check that the step value is smaller than the specified sweep range.
            If Math.Abs(CDbl(TermInfo.Item("step"))) > Math.Abs(CDbl(TermInfo.Item("stop")) - CDbl(TermInfo.Item("start"))) Then
                Return "Error - The step value for " & strCurrentPort & " cannot be greater than the specified sweep range (stop - start), aborting."
                Exit Function
            End If

            'if passes all conditions
            Return "SUCCESS"

        End Function

        Private Function CheckVarSoftwareRules(ByVal TermInfo As Hashtable, ByVal strCurrentPort As String, ByVal dblCurrentTermMaxV As Double, ByVal dblCurrentTermMaxI As Double) As String
            'This function checks that the specified terminal conforms to the compliance rules set for it by the Lab Server System.  The output of 
            'this funciton is a string that will either indicate success by returning "SUCCESS" or failure by returning an error-specific message.
            Dim strCurrentPortType As String
            Dim dblCompliance As Double

            'get port type from supplied port
            strCurrentPortType = Left(strCurrentPort, 3)

            If strCurrentPortType = "VSU" Then
                dblCompliance = 0.1
            Else
                dblCompliance = CDbl(TermInfo.Item("compliance"))
            End If

            If strCurrentPortType = "VSU" Or (strCurrentPortType = "SMU" And UCase(TermInfo.Item("mode")) = "V") Then
                'check software imposed limits in case that this is a voltage terminal

                'check that terminal compliance (current value) is within bounds.
                If dblCompliance > dblCurrentTermMaxI Then
                    Return "Error - Specified compliance value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum compliance = " & CStr(dblCurrentTermMaxI) & " A."
                    Exit Function
                End If

                'check that source values (voltages) are within bounds.
                If Math.Abs(CDbl(TermInfo.Item("stop"))) > dblCurrentTermMaxV Or Math.Abs(CDbl(TermInfo.Item("start"))) > dblCurrentTermMaxV Then
                    Return "Error - Specified source values for " & strCurrentPort & " are larger than those permitted by server, aborting.  Maximum source voltage = " & CStr(dblCurrentTermMaxV) & " V."
                    Exit Function
                End If
            ElseIf strCurrentPortType = "SMU" And UCase(TermInfo.Item("mode")) = "I" Then
                'check software imposed limits in case that this is a current terminal

                'check that terminal compliance (voltage value) is within bounds.
                If dblCompliance > dblCurrentTermMaxV Then
                    Return "Error - Specified compliance value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum compliance = " & CStr(dblCurrentTermMaxV) & " V."
                    Exit Function
                End If

                'check that source values (current) are within bounds.
                If Math.Abs(CDbl(TermInfo.Item("stop"))) > dblCurrentTermMaxI Or Math.Abs(CDbl(TermInfo.Item("start"))) > dblCurrentTermMaxI Then
                    Return "Error - Specified source values for " & strCurrentPort & " are larger than those permitted by server, aborting.  Maximum source voltage = " & CStr(dblCurrentTermMaxI) & " A."
                    Exit Function
                End If
            Else
                'unrecognized mode for the specified port, aborting
                Return "Error - Unrecognized mode specified for " & strCurrentPort & ", aborting."
                Exit Function
            End If

            'if all conditions met...
            Return "SUCCESS"

        End Function

        Private Function CheckVarHardwareRules(ByVal dblStart As Double, ByVal dblStop As Double, ByVal dblCompliance As Double, ByVal strMode As String, ByVal strScale As String, ByVal strCurrentPort As String) As String
            'This function checks the supplied terminal information against the hardware compliance information for that type of terminal.  This
            'method has separate checks for VSUs, MPSMUs and HPSMUs (encoded as port = "SMU5").  Double arguments should be absolute values.  
            'String inputs should be all upper case characters.  The output of this function is a string.  If all 
            'conditions are met, the value "SUCCESS" is returned.  Otherwise, a description of the specific failure condition is returned.
            Dim strCurrentPortType As String
            Dim dblSweepRange As Double
            Dim intSROrder As Integer

            strCurrentPortType = Left(strCurrentPort, 3)
            dblSweepRange = dblStop - dblStart

            'specific check routine based on port type/mode
            'port type check already performed, can assume correct
            Select Case strCurrentPortType
                Case "SMU"
                    'mode check already performed, can assume correct
                    If strMode = "V" Then
                        If strCurrentPort = "SMU5" Then
                            'check against absolute hardware upper bound
                            If dblStart > 200 Or dblStop > 200 Then
                                Return "Error - The specified source values for " & strCurrentPort & " are larger than those permitted by hardware, aborting.  Maximum source voltage = 200V."
                                Exit Function
                            End If

                            'check resolution WRT specified sweep range
                            Select Case dblSweepRange
                                Case Is <= 2
                                    If dblCompliance > 1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 1A"
                                        Exit Function
                                    ElseIf dblStart < 0.0001 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 2V, |start| >= 100uV."
                                        Exit Function
                                    End If
                                Case Is <= 20
                                    If dblCompliance > 1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 1A"
                                        Exit Function
                                    ElseIf dblStart < 0.001 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 20V, |start| >= 1mV."
                                        Exit Function
                                    End If
                                Case Is <= 40
                                    If dblCompliance > 0.5 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 40V, maximum compliance = 500mA."
                                        Exit Function
                                    ElseIf dblStart < 0.002 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 40V, |start| >= 2mV."
                                        Exit Function
                                    End If
                                Case Is <= 100
                                    If dblCompliance > 0.125 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 100V, maximum compliance = 125mA."
                                        Exit Function
                                    ElseIf dblStart < 0.005 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 100V, |start| >= 5mV."
                                        Exit Function
                                    End If
                                Case Is <= 200
                                    If dblCompliance > 0.05 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 200V, maximum compliance = 50mA."
                                        Exit Function
                                    ElseIf dblStart < 0.01 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 200V, |start| >= 10mV."
                                        Exit Function
                                    End If
                                Case Else
                                    Return "Error - The specified sweep range for " & strCurrentPort & " exceeds hardware capabilities, aborting."
                                    Exit Function
                            End Select
                        Else
                            'check against absolute hardware upper bound
                            If dblStart > 100 Or dblStop > 100 Then
                                Return "Error - The specified source values for " & strCurrentPort & " are larger than those permitted by hardware, aborting.  Maximum source voltage = 100V."
                            End If

                            'check resolution WRT specified sweep range
                            Select Case dblSweepRange
                                Case Is <= 2
                                    If dblCompliance > 0.1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 100mA"
                                        Exit Function
                                    ElseIf dblStart < 0.0001 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 2V, |start| >= 100uV."
                                        Exit Function
                                    End If
                                Case Is <= 20
                                    If dblCompliance > 0.1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 100mA"
                                        Exit Function
                                    ElseIf dblStart < 0.001 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 20V, |start| >= 1mV."
                                        Exit Function
                                    End If
                                Case Is <= 40
                                    If dblCompliance > 0.05 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 40V, maximum compliance = 50mA."
                                        Exit Function
                                    ElseIf dblStart < 0.002 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 40V, |start| >= 2mV."
                                        Exit Function
                                    End If
                                Case Is <= 100
                                    If dblCompliance > 0.02 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 100V, maximum compliance = 20mA."
                                        Exit Function
                                    ElseIf dblStart < 0.005 And InStr(strScale, "LOG") <> 0 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for this sweep range.  For |stop - start| <= 100V, |start| >= 5mV."
                                        Exit Function
                                    End If
                                Case Else
                                    Return "Error - The specified sweep range for " & strCurrentPort & " exceeds hardware capabilities, aborting."
                                    Exit Function
                            End Select
                        End If

                    ElseIf strMode = "I" Then
                        If strCurrentPort = "SMU5" Then
                            'check against absolute hardware upper-bound
                            If dblStart > 1 Or dblStop > 1 Then
                                Return "Error - The specified source values for " & strCurrentPort & " are larger than those permitted by hardware, aborting.  Maximum source current = 1A."
                                Exit Function
                            End If

                            'check resolution WRT specified sweep range
                            If dblStop <= 0.05 And dblStart <= 0.05 And dblCompliance > 200 Then
                                Return "Error - The Specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum compliance = 200V."
                                Exit Function
                            End If

                            If ((dblStop > 0.05 And dblStop <= 0.1) Or (dblStart > 0.05 And dblStart <= 0.1)) And dblSweepRange <= 0.1 And dblCompliance > 100 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 100V."
                                Exit Function
                            End If

                            If ((dblStop > 0.05 And dblStop <= 0.125) Or (dblStart > 0.05 And dblStart <= 0.125)) And dblSweepRange <= 1 And dblCompliance > 100 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 100V."
                                Exit Function
                            End If

                            If ((dblStop > 0.125 And dblStop <= 0.5) Or (dblStart > 0.125 And dblStart <= 0.5)) And dblCompliance > 40 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 40V."
                                Exit Function
                            End If

                            If ((dblStop > 0.5 And dblStop <= 1) Or (dblStart > 0.5 And dblStart <= 1)) And dblCompliance > 20 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 20V."
                                Exit Function
                            End If

                            'check against linear case min nonzero start/stop value rules for instrument
                            If InStr(strScale, "LIN") <> 0 Then
                                intSROrder = CInt(Math.Ceiling(Math.Log10(dblSweepRange))) 

                                If dblStart <> 0 Then
                                    If intSROrder - CInt(Math.Ceiling(Math.Log10(dblStart))) > 4 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for the specified sweep range.  For  |stop - start| <= 1E" & intSROrder & " A, |start| >= 1E" & intSROrder - 4 & " A if non-zero."
                                        Exit Function
                                    End If
                                End If

                                If dblStop <> 0 Then
                                    If intSROrder - CInt(Math.Ceiling(Math.Log10(dblStop))) > 4 Then
                                        Return "Error - The stop value for " & strCurrentPort & " is too small for the specified sweep range.  For  |stop - start| <= 1E" & intSROrder & " A, |stop| >= 1E" & intSROrder - 4 & " A if non-zero."
                                        Exit Function
                                    End If
                                End If
                            End If

                        Else
                            'check against absolute hardware upper-bound
                            If dblStart > 0.1 Or dblStop > 0.1 Then
                                Return "Error - The specified source values for " & strCurrentPort & " are larger than those permitted by hardware, aborting.  Maximum source current = 100mA."
                                Exit Function
                            End If

                            'check resolution WRT specified sweep range
                            If dblStop <= 0.02 And dblStart <= 0.02 And dblCompliance > 100 Then
                                Return "Error - The Specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum compliance = 100V."
                                Exit Function
                            End If

                            If ((dblStop > 0.02 And dblStop <= 0.05) Or (dblStart > 0.02 And dblStart <= 0.05)) And dblCompliance > 40 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 40V."
                                Exit Function
                            End If

                            If ((dblStop > 0.05 And dblStop <= 0.1) Or (dblStart > 0.05 And dblStart <= 0.1)) And dblCompliance > 20 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 20V."
                                Exit Function
                            End If

                            'check against linear case min nonzero start/stop value rules for instrument
                            If InStr(strScale, "LIN") <> 0 Then
                                intSROrder = CInt(Math.Ceiling(Math.Log10(dblSweepRange)))

                                If dblStart <> 0 Then
                                    If intSROrder - CInt(Math.Ceiling(Math.Log10(dblStart))) > 4 Then
                                        Return "Error - The start value for " & strCurrentPort & " is too small for the specified sweep range.  For  |stop - start| <= 1E" & intSROrder & " A, |start| >= 1E" & intSROrder - 4 & " A if non-zero."
                                        Exit Function
                                    End If
                                End If

                                If dblStop <> 0 Then
                                    If intSROrder - CInt(Math.Ceiling(Math.Log10(dblStop))) > 4 Then
                                        Return "Error - The stop value for " & strCurrentPort & " is too small for the specified sweep range.  For  |stop - start| <= 1E" & intSROrder & " A, |stop| >= 1E" & intSROrder - 4 & " A if non-zero."
                                        Exit Function
                                    End If
                                End If
                            End If

                            End If

                        End If
                Case "VSU"
                        'validate function against hardware compliance information (v mode assumed, max compliance = 100mA, max volts = +/-20 V)
                        If dblCompliance > 0.1 Then
                            Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum compliance = 100 mA"
                            Exit Function
                        End If

                        If dblStop > 20 Or dblStart > 20 Then
                            Return "Error - The specified source values for " & strCurrentPort & " are larger than those permitted by hardware, aborting.  Maximum source voltage = 20 V."
                            Exit Function
                        End If
            End Select

            'if all conditions have been met...
            Return "SUCCESS"

        End Function

        Private Function CheckVar2Function(ByVal TermInfo As Hashtable, ByVal strCurrentPort As String) As String
            'This function checks that the terminal information encoded in the specified hashtable is approprate for configuring a linear scale 
            'VAR2 function on the 4155 hardware.  This includes basic, function-specific field format and dependency checks.
            'if this portion of the specification passes all test conditions, the string "SUCCESS" is returned.  Otherwise, an error message 
            'is returned to the caller.
            Dim dblSignCheck As Double

            'check that the supplied start value is numeric
            If Trim(TermInfo("start")) = "" Or Not IsNumeric(TermInfo("start")) Then
                Return "Error - A numeric start value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the supplied stop value is numeric
            If Trim(TermInfo("stop")) = "" Or Not IsNumeric(TermInfo("stop")) Then
                Return "Error - A numeric stop value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the supplied step value is numeric
            If Trim(TermInfo("step")) = "" Or Not IsNumeric(TermInfo("step")) Then
                Return "Error - A numeric step value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the supplied step value is non-zero
            If CDbl(TermInfo("step")) = 0 Then
                Return "Error - Step value for " & strCurrentPort & " must be non-zero, aborting."
                Exit Function
            End If

            'check that the step value is properly signed
            dblSignCheck = (CDbl(TermInfo("stop")) - CDbl(TermInfo("start"))) * CDbl(TermInfo("step"))

            If dblSignCheck <= 0 Then
                Return "Error - The step value for " & strCurrentPort & " must have the same sign as (stop - start), aborting."
                Exit Function
            End If

            'check that the supplied step value is within the defined sweep range
            If Math.Abs(CDbl(TermInfo("step"))) > Math.Abs(CDbl(TermInfo("stop")) - CDbl(TermInfo("start"))) Then
                Return "Error - the step value for " & strCurrentPort & " cannot be greater than the specified sweep range (stop - start), aborting."
                Exit Function
            End If

            'if passed all conditions...

            Return "SUCCESS"

        End Function

        Private Function CheckVar1PFunction(ByVal TermInfo As Hashtable, ByVal strCurrentPort As String) As String
            'This function checks that the terminal information encoded in the specified hashtable is approprate for configuring a VAR1P 
            'function on the 4155 hardware (var1p must be linear).  This includes basic, function-specific field format and dependency checks.
            'if this portion of the specification passes all test conditions, the string "SUCCESS" is returned.  Otherwise, an error message 
            'is returned to the caller.

            'check that the supplied ratio value is numeric
            If Trim(TermInfo("ratio")) = "" Or Not IsNumeric(TermInfo("ratio")) Then
                Return "Error - A numeric ratio value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'check that the supplied offset value is numeric
            If Trim(TermInfo("offset")) = "" Or Not IsNumeric(TermInfo("offset")) Then
                Return "Error - A numeric offset value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'if passed all conditions...

            Return "SUCCESS"

        End Function

        Private Function CheckVar1PSoftwareRules(ByVal dblVar1PStart As Double, ByVal dblVar1PStop As Double, ByVal dblCompliance As Double, ByVal strCurrentPort As String, ByVal strMode As String, ByVal dblCurrentTermMaxV As Double, ByVal dblCurrentTermMaxI As Double) As String
            'This function checks that the specified terminal conforms to the compliance rules set for it by the Lab Server System.  In particular,
            'this method checks the aggregate voltage/current values to be produced by a VAR1P function on the 4155 (that is, the VAR1 start and stop 
            'values scaled by the VAR1P ratio and offset values.  These aggregate values are to be supplied as arguments to this function as dblVar1PStart
            'and dblVar1PStop.  The output of this funciton is a string that will either indicate success by returning "SUCCESS" or failure by returning an 
            'error-specific message.
            Dim strCurrentPortType As String

            'get port type from supplied port
            strCurrentPortType = Left(strCurrentPort, 3)

            If strCurrentPortType = "VSU" Or (strCurrentPortType = "SMU" And strMode = "V") Then
                'check software imposed limits in case that this is a voltage terminal

                'check that terminal compliance (current value) is within bounds.
                If dblCompliance > dblCurrentTermMaxI Then
                    Return "Error - Specified compliance value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum compliance = " & CStr(dblCurrentTermMaxI) & " A."
                    Exit Function
                End If

                'check that source values (voltages) are within bounds.
                If dblVar1PStop > dblCurrentTermMaxV Or dblVar1PStart > dblCurrentTermMaxV Then
                    Return "Error - Specified source values for " & strCurrentPort & " are larger than those permitted by server, aborting.  Maximum source voltage = " & CStr(dblCurrentTermMaxV) & " V."
                    Exit Function
                End If
            ElseIf strCurrentPortType = "SMU" And strMode = "I" Then
                'check software imposed limits in case that this is a current terminal

                'check that terminal compliance (voltage value) is within bounds.
                If dblCompliance > dblCurrentTermMaxV Then
                    Return "Error - Specified compliance value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum compliance = " & CStr(dblCurrentTermMaxV) & " V."
                    Exit Function
                End If

                'check that source values (current) are within bounds.
                If dblVar1PStop > dblCurrentTermMaxI Or dblVar1PStart > dblCurrentTermMaxI Then
                    Return "Error - Specified source values for " & strCurrentPort & " are larger than those permitted by server, aborting.  Maximum source voltage = " & CStr(dblCurrentTermMaxI) & " A."
                    Exit Function
                End If
            Else
                'unrecognized mode for the specified port, aborting
                Return "Error - Unrecognized mode specified for " & strCurrentPort & ", aborting."
                Exit Function
            End If

            'if all conditions met...
            Return "SUCCESS"

        End Function

        Private Function CheckConsFunction(ByVal TermInfo As Hashtable, ByVal strCurrentPort As String) As String
            'This function checks that the terminal information encoded in the specified hashtable is approprate for configuring a CONS 
            'function on the 4155 hardware.  This includes basic, function-specific field format and dependency checks.

            'check that the supplied constant value is numeric.
            If Trim(TermInfo.Item("value")) = "" Or Not IsNumeric(TermInfo.Item("value")) Then
                Return "Error - A numeric value for " & strCurrentPort & " must be supplied."
                Exit Function
            End If

            'if passes all conditions
            Return "SUCCESS"

        End Function

        Private Function CheckConsSoftwareRules(ByVal TermInfo As Hashtable, ByVal dblCompliance As Double, ByVal strCurrentPort As String, ByVal dblCurrentTermMaxV As Double, ByVal dblCurrentTermMaxI As Double) As String
            'This function checks that the specified terminal conforms to the compliance rules set for it by the Lab Server System.  In particular,
            'this method checks the constant values to be produced by a CONS function on the 4155.  The output of this funciton is a string that will 
            'either indicate success by returning "SUCCESS" or failure by returning an error-specific message.
            Dim strCurrentPortType As String

            'get port type from supplied port
            strCurrentPortType = Left(strCurrentPort, 3)

            If strCurrentPortType = "VSU" Or (strCurrentPortType = "SMU" And UCase(TermInfo.Item("mode")) = "V") Then
                'check software imposed limits in case that this is a voltage terminal

                'check that terminal compliance (current value) is within bounds.
                If dblCompliance > dblCurrentTermMaxI Then
                    Return "Error - Specified compliance value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum compliance = " & CStr(dblCurrentTermMaxI) & " A."
                    Exit Function
                End If

                'check that source values (voltages) are within bounds.
                If Math.Abs(CDbl(TermInfo.Item("value"))) > dblCurrentTermMaxV Then
                    Return "Error - Specified source value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum source voltage = " & CStr(dblCurrentTermMaxV) & " V."
                    Exit Function
                End If
            ElseIf strCurrentPortType = "SMU" And UCase(TermInfo.Item("mode")) = "I" Then
                'check software imposed limits in case that this is a current terminal

                'check that terminal compliance (voltage value) is within bounds.
                If dblCompliance > dblCurrentTermMaxV Then
                    Return "Error - Specified compliance value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum compliance = " & CStr(dblCurrentTermMaxV) & " V."
                    Exit Function
                End If

                'check that source values (current) are within bounds.
                If Math.Abs(CDbl(TermInfo.Item("value"))) > dblCurrentTermMaxI Then
                    Return "Error - Specified source value for " & strCurrentPort & " is larger than that permitted by server, aborting.  Maximum source voltage = " & CStr(dblCurrentTermMaxI) & " A."
                    Exit Function
                End If
            Else
                'unrecognized mode for the specified port, aborting
                Return "Error - Unrecognized mode specified for " & strCurrentPort & ", aborting."
                Exit Function
            End If

            'if all conditions met...
            Return "SUCCESS"
        End Function

        Private Function CheckConsHardwareRules(ByVal TermInfo As Hashtable, ByVal dblCompliance As Double, ByVal strCurrentPort As String) As String
            'This function checks the supplied terminal information against the hardware compliance information for that type of terminal.  This
            'method has separate checks for VSUs, MPSMUs and HPSMUs (encoded as port = "SMU5").  Double arguments should be absolute values.  
            'String inputs should be all upper case characters.  The output of this function is a string.  If all 
            'conditions are met, the value "SUCCESS" is returned.  Otherwise, a description of the specific failure condition is returned.
            Dim strCurrentPortType, strMode As String
            Dim dblValue As Double

            strCurrentPortType = Left(strCurrentPort, 3)
            strMode = UCase(TermInfo.Item("mode"))
            dblValue = Math.Abs(CDbl(TermInfo.Item("value")))
            'specific check routine based on port type/mode
            'port type check already performed, can assume correct
            Select Case strCurrentPortType
                Case "SMU"
                    'mode check already performed, can assume correct
                    If strMode = "V" Then
                        If strCurrentPort = "SMU5" Then
                            'check against absolute hardware upper bound
                            If dblValue > 200 Then
                                Return "Error - The specified source value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum source voltage = 200V."
                                Exit Function
                            End If

                            'check resolution WRT specified CONS value
                            Select Case dblValue
                                Case Is <= 2
                                    If dblCompliance > 1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 1A"
                                        Exit Function
                                    End If
                                Case Is <= 20
                                    If dblCompliance > 1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 1A"
                                        Exit Function
                                    End If
                                Case Is <= 40
                                    If dblCompliance > 0.5 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 40V, maximum compliance = 500mA."
                                        Exit Function
                                    End If
                                Case Is <= 100
                                    If dblCompliance > 0.125 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 100V, maximum compliance = 125mA."
                                        Exit Function
                                    End If
                                Case Is <= 200
                                    If dblCompliance > 0.05 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 200V, maximum compliance = 50mA."
                                        Exit Function
                                    End If
                                Case Else
                                    Return "Error - The specified source value for " & strCurrentPort & " exceeds hardware capabilities, aborting."
                                    Exit Function
                            End Select
                        Else
                            'check against absolute hardware upper bound
                            If dblValue > 100 Then
                                Return "Error - The specified source value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum source voltage = 100V."
                            End If

                            'check resolution WRT specified sweep range
                            Select Case dblValue
                                Case Is <= 2
                                    If dblCompliance > 0.1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 100mA"
                                        Exit Function
                                    End If
                                Case Is <= 20
                                    If dblCompliance > 0.1 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum complance = 100mA"
                                        Exit Function
                                    End If
                                Case Is <= 40
                                    If dblCompliance > 0.05 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 40V, maximum compliance = 50mA."
                                        Exit Function
                                    End If
                                Case Is <= 100
                                    If dblCompliance > 0.02 Then
                                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  For |stop - start| = 100V, maximum compliance = 20mA."
                                        Exit Function
                                    End If
                                Case Else
                                    Return "Error - The specified source value for " & strCurrentPort & " exceeds hardware capabilities, aborting."
                                    Exit Function
                            End Select
                        End If

                    ElseIf strMode = "I" Then
                        If strCurrentPort = "SMU5" Then
                            'check against absolute hardware upper-bound
                            If dblValue > 1 Then
                                Return "Error - The specified source value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum source current = 1A."
                                Exit Function
                            End If

                            'check resolution WRT specified sweep range
                            If dblValue <= 0.05 And dblCompliance > 200 Then
                                Return "Error - The Specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum compliance = 200V."
                                Exit Function
                            End If

                            If (dblValue > 0.05 And dblValue <= 0.1) And dblCompliance > 100 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 100V."
                                Exit Function
                            End If

                            If (dblValue > 0.05 And dblValue <= 0.125) And dblCompliance > 100 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 100V."
                                Exit Function
                            End If

                            If (dblValue > 0.125 And dblValue <= 0.5) And dblCompliance > 40 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 40V."
                                Exit Function
                            End If

                            If (dblValue > 0.5 And dblValue <= 1) And dblCompliance > 20 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 20V."
                                Exit Function
                            End If


                        Else
                            'check against absolute hardware upper-bound
                            If dblValue > 0.1 Then
                                Return "Error - The specified source value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum source current = 100mA."
                                Exit Function
                            End If

                            'check resolution WRT specified sweep range
                            If dblValue <= 0.02 And dblCompliance > 100 Then
                                Return "Error - The Specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum compliance = 100V."
                                Exit Function
                            End If

                            If (dblValue > 0.02 And dblValue <= 0.05) And dblCompliance > 40 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 40V."
                                Exit Function
                            End If

                            If (dblValue > 0.05 And dblValue <= 0.1) And dblCompliance > 20 Then
                                Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware for the specified sweep range, aborting.  Maximum compliance for this range is 20V."
                                Exit Function
                            End If

                        End If

                    End If
                Case "VSU"
                    'validate function against hardware compliance information (v mode assumed, max compliance = 100mA, max volts = +/-20 V)
                    If dblCompliance > 0.1 Then
                        Return "Error - The specified compliance value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum compliance = 100 mA"
                        Exit Function
                    End If

                    If dblValue > 20 Then
                        Return "Error - The specified source value for " & strCurrentPort & " is larger than that permitted by hardware, aborting.  Maximum source voltage = 20 V."
                        Exit Function
                    End If
            End Select

            'if all conditions have been met...
            Return "SUCCESS"

        End Function

        Private Function UDFCheck(ByVal UDFTable As ExpUnitSetObject) As String
            'This method checks the integrity of the User Defined Functions specified in the experiment specification.  In particular,
            'a Java console application is used to verify the syntactical correctness of each UDF given the complete set of variable names
            'in the specification.  If a UDF is found to be improperly formed, a string describing the error is returned.  Otherwise, the
            'string "SUCCESS" is returned.

            'This method depends on strNameList (class variable defined by SpecificationCheck method) and strSitePath (class variable defined
            'by public accessor).
            Dim UDFCheckProcess As Process = New Process()
            Dim strChkOut, strErrOut, strFinalList As String
            Dim loopIdx As Integer
            Dim sIn As StreamWriter
            Dim sOut, sErr As StreamReader

            'Open console
            UDFCheckProcess.StartInfo.FileName = "cmd.exe"
            UDFCheckProcess.StartInfo.UseShellExecute = False
            UDFCheckProcess.StartInfo.CreateNoWindow = True
            UDFCheckProcess.StartInfo.RedirectStandardInput = True
            UDFCheckProcess.StartInfo.RedirectStandardOutput = True
            UDFCheckProcess.StartInfo.RedirectStandardError = True
            UDFCheckProcess.Start()
            sIn = UDFCheckProcess.StandardInput
            sOut = UDFCheckProcess.StandardOutput
            sErr = UDFCheckProcess.StandardError
            sIn.AutoFlush = True

            'Navigate to UDF checker bin directory (parically supplied by agent invoking validaton
            sIn.Write("cd " & strSitePath & "\java" & Environment.NewLine)

            'creates a process block scenario.  Standard error not checked in this release.
            'strErrOut = sErr.ReadToEnd() 'check for operational error
            'If Trim(strErrOut) <> "" Then
            '    Return "Error Parsing User-Defined Function - Error invoking check application (" & strErrOut & ").  Please try again and contact a system administrator if the problem persists."

            '    sOut.DiscardBufferedData()
            '    sIn.Close()
            '    sOut.Close()
            '    sErr.Close()
            '    UDFCheckProcess.Close()
            '    Exit Function
            'End If

            For loopIdx = 0 To UDFTable.Count() - 1
                'set up variable list for check (omits this UDF)
                strFinalList = Replace(strNameList, "|" & UDFTable.UnitItem(loopIdx, "name"), "")
                strFinalList = Right(strFinalList, Len(strFinalList) - 1) 'remove leading "|"
                strFinalList = Replace(strFinalList, "|", ",")

                Thread.Sleep(1000) 'waits for previous commands to complete
                sOut.ReadLine()
                sOut.DiscardBufferedData() 'removes initial terminal output

                'executes check command
                sIn.Write("java -classpath bin weblab.validation.Validation -v """ & strFinalList & """ """ & UDFTable.UnitItem(loopIdx, "body") & """" & Environment.NewLine)

                Thread.Sleep(1000) 'waits until check is done

                'check for operational error
                'strErrOut = sErr.ReadLine()
                'If Trim(strErrOut) <> "" Then
                '    Return "Error Parsing User-Defined Function - Error invoking check application (" & strErrOut & ").  Please try again and contact a system administrator if the problem persists."

                '    sIn.Close()
                '    sOut.Close()
                '    sErr.Close()
                '    UDFCheckProcess.Close()
                '    Exit Function
                'End If

                sOut.ReadLine() 'advance past the command echo line

                strChkOut = sOut.ReadLine()
                If Trim(strChkOut) <> "OK" Then
                    If Not UDFCheckProcess.HasExited Then
                        UDFCheckProcess.Kill()
                    End If

                    Return "Error Parsing User-Defined Function - " & strChkOut & " vars:" & strFinalList & ", body:" & UDFTable.UnitItem(loopIdx, "body")

                    sIn.Close()
                    sOut.Close()
                    sErr.Close()
                    UDFCheckProcess.Close()
                    Exit Function
                End If
            Next

            If Not UDFCheckProcess.HasExited Then
                UDFCheckProcess.Kill()
            End If

            sIn.Close()
            sOut.Close()
            sErr.Close()
            UDFCheckProcess.Close()

            Return "SUCCESS"

        End Function

    End Class

End Namespace

