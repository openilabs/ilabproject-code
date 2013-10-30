Imports System
Imports System.Data.SqlClient
Imports System.Xml
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic
Imports WebLabDataManagers.WebLabDataManagers

Namespace WebLabSystemComponents

    'Author(s): James Hardison (hardison@alum.mit.edu)
    'Date: 6/6/2003
    'Date Modified: 9/23/2004
    'This library contains the logic used to validate incoming experiment specifications.  This process requires three steps.  First, the 
    'input XML Experiment Specification must be decoded.  Second, it must be confirmed that the agent submitting the experiment has
    'sufficient access to the experiment setups referenced in the specification.  Third, the specification itself must be checked for 
    'correctness.  Each of these procedures will be accomplished by a private function.  There will be a single public function in this
    'class that will call those individual private functions to validate the experiment.  To use this, make sure the compiled DLL is 
    'placed in the /bin directory of your ASP.NET application and, in your ASP.NET code, import the "WebLabSystemComponents" namespace.
    'This method references the WebLabDataManagers namespace.
    '
    'Dependency List
    'Used By:
    '   Lab Server Web Service Interface (/serices/WebLabServices/LabServerAPI.vb, /bin/WebLabServices.dll) 
    '   Validation Engine Library (/bin/validation_engine.dll) - this code compiled
    '
    'Uses:
    '   Resource Permission Manager (/controls/WebLabDataManagers/ResourcePermissionManager.vb, /bin/ResourcePermissionManager.dll)
    '   User Defined Function Validator (/java/src/weblab/validation/validation/java, /java/bin/weblab/validation/validation.class)

    Public Class ValidationEngine

        'Dim Debug As StreamWriter = New StreamWriter("C:\Inetpub\wwwroot\LabServer\Logs\validationLog.txt")

        Dim termInfoTable(,), udfInfoTable(,), functInfoTable(,), listItem, listUnits As String
        Dim termNodeListLength, udfNodeListLength, intSetupID, intPriority As Integer
        Dim conWebLabLS As SqlConnection
        Dim xmlExpSpec, xmlTemp As XmlDocument
        Dim rpmObject As New ResourcePermissionManager

        Private intTotalPoints As Integer = 0
        Private intPointLimit As Integer = 0
        Private intResourceID As Integer = 0


        Dim FGEN_record As Integer = -1 '-1 is default, unassigned value, dependency: parseXMLSpec
        Dim SCOPE_record As Integer = -1
        
        'terminal info fields
        Const TERM_INSTRUMENT As Integer = 0
        Const TERM_VNAME As Integer = 1
        Const TERM_FUNCTION_TYPE As Integer = 2
        
        'instrument labels for function info
        Const FGEN_FUNCT As Integer = 0
        Const SCOPE_FUNCT As Integer = 1
        
        'function information fields
        Const FUNCT_OFFSET As Integer = 0
        Const FUNCT_WAVEFORMTYPE As Integer = 1
        Const FUNCT_FREQUENCY As Integer = 2
        Const FUNCT_AMPLITUDE As Integer = 3
        Const FUNCT_SAMPLINGRATE As Integer = 4
        Const FUNCT_SAMPLINGTIME As Integer = 5

        'user defined function info fields
        Const UDF_NAME As Integer = 0
        Const UDF_UNITS As Integer = 1
        Const UDF_BODY As Integer = 2

        Public Function getTotalDataPoints() As Integer
            'This method returns the total number of datapoints in the job being validated.  Returns 0 if called 
            'before completion of the validate method.
            Return intTotalPoints
        End Function

        Public Function getSetupID() As Integer
            'This method returns the Lab Server Setup ID of the experiment setup used in the job being validated.
            'Returns 0 if called before completion of the validate method.
            Return intSetupID
        End Function

        Public Function getResourceID() As Integer
            'This method returns the Lab Server Reosurce ID of the experiment setup used in the job being validated.  
            'Returns 0 if called before completion of the validate method.
            Return intResourceID
        End Function

        Public Function validate(ByVal strXMLExpSpec As String, ByVal intBrokerID As Integer) As String
            'This method governs the job validation process.  The specified experiment specification is validated
            'withing hte permission context of the supplied Broker ID.  Private methods performing validation steps 
            'are called in the appropriate order by this method.  Upon completion, an indication of success or a 
            'validation error message is returned.
            Dim strAccessOut, strValidateOut As String
            'Debug.AutoFlush = True
            conWebLabLS = New SqlConnection("Database=ELVIS_LS;Server=localhost;Integrated Security=true")
            conWebLabLS.Open()

            'first, parse the incoming XML spec and load into memory.  Use try-catch here to handle parsing/validation errors.
            Try
                parseXMLSpec(strXMLExpSpec)
            Catch
                conWebLabLS.Close()
                Return "Error - An error was generated while parsing the Experiment Specification.  Please make sure the file is valid and well-formed. " & Err.Description()
                Exit Function
            End Try

            'next, confirm that the supplied broker has permission to use the specified experiment setup.
            strAccessOut = brokerHasSetupPermission(intBrokerID, intSetupID)

            If Not strAccessOut = "SUCCESS" Then
                conWebLabLS.Close()
                Return strAccessOut
                Exit Function
            End If

            'finally, validate the specification
            strValidateOut = experimentValidator()

            If Not strValidateOut = "SUCCESS" Then
                conWebLabLS.Close()
                Return strValidateOut
                Exit Function
            End If

            conWebLabLS.Close()
            Return "SUCCESS"
        End Function

        Public Function validate(ByVal strXMLExpSpec As String, ByVal intBrokerID As Integer, ByVal intGroupID As Integer) As String
            'This method governs the job validation process.  The specified experiment specification is validated
            'withing hte permission context of the supplied Group ID.  Private methods performing validation steps 
            'are called in the appropriate order by this method.  Upon completion, an indication of success or a 
            'validation error message is returned.
            Dim strAccessOut, strValidateOut As String
            conWebLabLS = New SqlConnection("Database=ELVIS_LS;Server=localhost;Integrated Security=true")
            conWebLabLS.Open()

            'first, parse the incoming XML spec and load into memory.  Use try-catch here to handle parsing/validation errors.
            Try
                parseXMLSpec(strXMLExpSpec)
            Catch
                conWebLabLS.Close()
                Return "Error - An error was generated while parsing the Experiment Specification.  Please make sure the file is valid and well-formed. " & Err.Description()
                Exit Function
            End Try

            'next, confirm that the supplied broker has permission to use the specified experiment setup.
            strAccessOut = groupHasSetupPermission(intGroupID, intSetupID)

            If Not strAccessOut = "SUCCESS" Then
                strAccessOut = brokerHasSetupPermission(intBrokerID, intSetupID)
                If Not strAccessOut = "SUCCESS" Then
                    conWebLabLS.Close()
                    Return strAccessOut
                    Exit Function
                End If
            End If

            'finally, validate the specification
            strValidateOut = experimentValidator()

            If Not strValidateOut = "SUCCESS" Then
                conWebLabLS.Close()
                Return strValidateOut
                Exit Function
            End If

            conWebLabLS.Close()
            Return "SUCCESS"
        End Function

        Private Function parseXMLSpec(ByVal strExpSpec As String)
            'This method parses the supplied XML Experiment Specification.  The parsed data elements are loaded into class variables for 
            'processing by other private, internal methods.
            Dim tempXPath, udfXPath, termXPath, instrumentType, termName, udfName As String
            Dim tempNode As XmlNode
            Dim termNodeList, udfNodeList As XmlNodeList
            Dim instrumentConstant, loopIdx As Integer
            listItem = ""
            listUnits = ""

            Debug.WriteLine("begin parsing Exp Spec")

            xmlExpSpec = New XmlDocument
            xmlExpSpec.LoadXml(strExpSpec)

            'loads terminal nodes 
            termXPath = "/experimentSpecification/terminal"
            termNodeList = xmlExpSpec.SelectNodes(termXPath)
            termNodeListLength = termNodeList.Count()

            'loads User Defined Function (udf) nodes
            udfXPath = "/experimentSpecification/userDefinedFunction"
            udfNodeList = xmlExpSpec.SelectNodes(udfXPath)
            udfNodeListLength = udfNodeList.Count()

            'builds 2D array, by # of terminals and then by field (reinitiallized)
            ReDim termInfoTable((termNodeListLength - 1), 5)

            'builds 2D array, by # of udfs and then by field (reinitiallized)
            ReDim udfInfoTable((udfNodeListLength - 1), 2)

            'builds 2D array, by function and then by field (reinitiallized)
            ReDim functInfoTable(5, 10)

            tempXPath = "/experimentSpecification/setupID"
            tempNode = xmlExpSpec.SelectSingleNode(tempXPath)
            intSetupID = CInt(tempNode.InnerXml())
            Debug.WriteLine("setup ID =" & CStr(intSetupID))

            'processes terminal subtrees
            For loopIdx = 0 To termNodeListLength - 1
                xmlTemp = New XmlDocument
                xmlTemp.LoadXml(termNodeList.Item(loopIdx).OuterXml())

                'instrument
                tempXPath = "/terminal/@instrumentType"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                instrumentType = Trim(tempNode.InnerXml())
                Debug.WriteLine("instrument type=" & Trim(tempNode.InnerXml()))
                tempXPath = "/terminal/@instrumentNumber"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termInfoTable(loopIdx, TERM_INSTRUMENT) = instrumentType '& Trim(tempNode.InnerXml())
                Debug.WriteLine("instrument number=" & Trim(tempNode.InnerXml()))

                Select Case termInfoTable(loopIdx, TERM_INSTRUMENT)
                    Case "FGEN"
                        instrumentConstant = FGEN_FUNCT
                        FGEN_record = loopIdx
                    Case "SCOPE"
                        instrumentConstant = SCOPE_FUNCT
                        SCOPE_record = loopIdx
                End Select

                'load vname & download attribute
                tempXPath = "/terminal/vname"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termName = Trim(tempNode.InnerXml())
                Debug.WriteLine("Vname=" & Trim(tempNode.InnerXml()))
                termInfoTable(loopIdx, TERM_VNAME) = termName

                tempXPath = "/terminal/vname/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                If Trim(tempNode.InnerXml()) = "true" Then
                    listItem = listItem & "'" & termName & "'" & ", "
                    listUnits = listUnits & "V, "
                    Debug.WriteLine(listItem)
                    Debug.WriteLine(listUnits)
                End If

                'load iname & download attribute
                'tempXPath = "/terminal/iname"
                'tempNode = xmlTemp.SelectSingleNode(tempXPath)
                'termName = Trim(tempNode.InnerXml())
                'Debug.WriteLine("Iname=" & Trim(tempNode.InnerXml()))
                'termInfoTable(loopIdx, TERM_INAME) = termName

                'tempXPath = "/terminal/iname/@download"
                'tempNode = xmlTemp.SelectSingleNode(tempXPath)
                'If Trim(tempNode.InnerXml()) = "true" Then
                'listItem = listItem & "'" & termName & "'" & ", "
                'listUnits = listUnits & "A, "
                'Debug.WriteLine(listItem)
                'Debug.WriteLine(listUnits)
                'End If

                'load mode setting
                'tempXPath = "/terminal/mode"
                'tempNode = xmlTemp.SelectSingleNode(tempXPath)
                'termInfoTable(loopIdx, TERM_MODE) = Trim(tempNode.InnerXml())
                'Debug.WriteLine("mode=" & Trim(tempNode.InnerXml()))

                'load compliance value
                'If termInfoTable(loopIdx, TERM_MODE) <> "COMM" Then     'added conditional
                'tempXPath = "/terminal/compliance"
                'tempNode = xmlTemp.SelectSingleNode(tempXPath)
                'termInfoTable(loopIdx, TERM_COMPLIANCE) = Trim(tempNode.InnerXml())
                'Debug.WriteLine("compliance=" & Trim(tempNode.InnerXml()))
                'End If

                'load function type & process sub-nodes. 

                tempXPath = "/terminal/function/@type"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                termInfoTable(loopIdx, TERM_FUNCTION_TYPE) = Trim(tempNode.InnerXml())
                Debug.WriteLine("function type=" & Trim(tempNode.InnerXml()))

                Select Case termInfoTable(loopIdx, TERM_FUNCTION_TYPE)
                    Case "WAVEFORM"
                        'load waveformTYpe value
                        tempXPath = "/terminal/function/waveformType"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_WAVEFORMTYPE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("waveformType=" & Trim(tempNode.InnerXml()))

                        'load frequency value
                        tempXPath = "/terminal/function/frequency"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_FREQUENCY) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Frequency=" & Trim(tempNode.InnerXml()))

                        'load amplitude value
                        tempXPath = "/terminal/function/amplitude"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_AMPLITUDE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Amplitude=" & Trim(tempNode.InnerXml()))

                        'load offset value
                        tempXPath = "/terminal/function/offset"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_OFFSET) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("Offset=" & Trim(tempNode.InnerXml()))

                    Case "SAMPLING"
                        'load samplingRate value
                        tempXPath = "/terminal/function/samplingRate"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_SAMPLINGRATE) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("samplingRate=" & Trim(tempNode.InnerXml()))

                        'load samplingTime value
                        tempXPath = "/terminal/function/samplingTime"
                        tempNode = xmlTemp.SelectSingleNode(tempXPath)
                        functInfoTable(instrumentConstant, FUNCT_SAMPLINGTIME) = Trim(tempNode.InnerXml())
                        Debug.WriteLine("samplingTime=" & Trim(tempNode.InnerXml()))
                End Select

            Next

            'process udf subtree
            For loopIdx = 0 To udfNodeListLength - 1
                xmlTemp = New XmlDocument
                xmlTemp.LoadXml(udfNodeList.Item(loopIdx).OuterXml())

                'load units 
                tempXPath = "/userDefinedFunction/units"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfInfoTable(loopIdx, UDF_UNITS) = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf units=" & Trim(tempNode.InnerXml()))

                'load name and download attribute
                tempXPath = "/userDefinedFunction/name"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfName = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf name=" & Trim(tempNode.InnerXml()))
                udfInfoTable(loopIdx, UDF_NAME) = udfName

                tempXPath = "/userDefinedFunction/name/@download"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                If Trim(tempNode.InnerXml()) = "true" Then
                    listItem = listItem & "'" & udfName & "'" & ", "
                    listUnits = listUnits & udfInfoTable(loopIdx, UDF_UNITS) & ", "
                    Debug.WriteLine(listItem)
                    Debug.WriteLine(listUnits)
                End If

                'load function body
                tempXPath = "/userDefinedFunction/body"
                tempNode = xmlTemp.SelectSingleNode(tempXPath)
                udfInfoTable(loopIdx, UDF_BODY) = Trim(tempNode.InnerXml())
                Debug.WriteLine("udf body=" & Trim(tempNode.InnerXml()))
            Next
        End Function

        Private Function brokerHasSetupPermission(ByVal intBrokerID As Integer, ByVal intSetupID As Integer) As String
            'this method checks if the specified broker has read access to the experiment setup referenced by intSetupID.  
            'View permission is required for setup execution.
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim blnQueryResult As Boolean

            strDBQuery = "SELECT p.resource_id FROM ActiveSetups a JOIN Setups p ON a.setup_id = p.setup_id WHERE a.setup_id = @setupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            intResourceID = CInt(dtrDBQuery("resource_id"))

            dtrDBQuery.Close()

            blnQueryResult = rpmObject.GetBrokerResourcePermission(intBrokerID, intResourceID, "canview")

            If blnQueryResult Then
                Return "SUCCESS"
            Else
                Return "Error - The specified broker does not have permission to use the specified experiment setup."
            End If
        End Function

        Private Function groupHasSetupPermission(ByVal intGroupID As Integer, ByVal intSetupID As Integer) As String
            'this method checks if the specified group has read access to the experiment Setup referenced by intSetupID.  
            'View permission is required for setup execution.
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim blnQueryResult As Boolean

            strDBQuery = "SELECT p.resource_id FROM ActiveSetups a JOIN Setups p ON a.setup_id = p.setup_id WHERE a.setup_id = @setupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            intResourceID = CInt(dtrDBQuery("resource_id"))

            dtrDBQuery.Close()

            blnQueryResult = rpmObject.GetGroupResourcePermission(intGroupID, intResourceID, "canview")

            If blnQueryResult Then
                Return "SUCCESS"
            Else
                Return "Error - The specified group does not have permission to use the specified experiment ID."
            End If
        End Function

        Private Function experimentValidator() As String
            'This function validates a parsed experiment specification.  In particular, this method is comprised of a set of rules or 
            'conditions which the spec must meet in order to be validated.  Finally, a console session will be instantiated and a 
            'server-side java component will test the specs User Defined Functions for correctness.  It is assumed that the an 
            'experiment specification has already been parsed into the defined class variables.
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim intTerminalsUsed, loopIdx, intLoopIdx As Integer
            Dim intWaveformDefRec As Integer = -1 'init value
            Dim intSamplingDefRec As Integer = -1 'init value
            Dim strUName, strLName As String
            Dim intNameLength As Integer
            Dim blnNameIsAlphaNum As Boolean


            'spec terminals match those in specififed profile
            'first, make sure the same number of terminals are specified
            strDBQuery = "SELECT terminals_used FROM Setups WHERE setup_id = @setupID"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@setupID", intSetupID)
            intTerminalsUsed = cmdDBQuery.ExecuteScalar()

            If Not intTerminalsUsed = termNodeListLength Then
                Return "Error - Experiment Specification does not match setup, terminal number mismatch."
                Exit Function
            End If

            'second, check that the individual types match
            For loopIdx = 0 To termNodeListLength - 1
                strDBQuery = "SELECT 'error' WHERE NOT EXISTS(SELECT setupterm_id FROM SetupTerminalConfig WHERE setup_id = @setupID AND instrument = @instrument)"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@setupID", intSetupID)
                cmdDBQuery.Parameters.Add("@instrument", termInfoTable(loopIdx, TERM_INSTRUMENT))

                If cmdDBQuery.ExecuteScalar() = "error" Then
                    Return "Error - Experiment Specification does not match setup, terminal type mismatch."
                    Exit Function
                End If
            Next

            'each spec terminal is well defined (has appropriate fields) for its declared instrument type.
            For loopIdx = 0 To termNodeListLength - 1
                Select Case UCase(termInfoTable(loopIdx, TERM_INSTRUMENT))
                    Case "FGEN"
                        'check or vname
                        If termInfoTable(loopIdx, TERM_VNAME) = "" Then
                            Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " has not been configured."
                            Exit Function
                            'ElseIf termInfoTable(loopIdx, TERM_INAME) = "" Then
                            'check for iname
                            'Return "Error - Port " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " requires a IName."
                            'Exit Function
                            'ElseIf Not (termInfoTable(loopIdx, TERM_MODE) = "V" Or termInfoTable(loopIdx, TERM_MODE) = "I" Or termInfoTable(loopIdx, TERM_MODE) = "COMM") Then
                            'check for mode declaration
                            'Return "Error - Port " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " requires a valid mode declaration."
                            'Exit Function
                        ElseIf termInfoTable(loopIdx, TERM_FUNCTION_TYPE) = "" Then
                            'check for function type
                            Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " requires a valid function declaration."
                            Exit Function
                            'ElseIf termInfoTable(loopIdx, TERM_COMPLIANCE) = "" And termInfoTable(loopIdx, TERM_MODE) <> "COMM" Then
                            'check for compliance value
                            'Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " requires a compliance value."
                            'Exit Function
                        End If
                    Case "SCOPE"
                        'check for vname
                        If termInfoTable(loopIdx, TERM_VNAME) = "" Then
                            Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " has not been configured."
                            Exit Function
                        ElseIf termInfoTable(loopIdx, TERM_FUNCTION_TYPE) = "" Then
                            'check for function type
                            Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " requires a valid function declaration."
                            Exit Function
                            'ElseIf termInfoTable(loopIdx, TERM_COMPLIANCE) = "" And termInfoTable(loopIdx, TERM_MODE) <> "COMM" Then
                            'check for compliance value
                            'Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " requires a compliance value."
                            'Exit Function
                        End If
                    Case Else
                        'unrecognized port type, return error
                        Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " in an unrecognized type."
                        Exit Function
                End Select
            Next

            'tests whether terminal/udf names are valid and if at least 2 but at most 8 variables are selected for download
            Dim strNameList As String

            For loopIdx = 0 To termNodeListLength - 1
                'checks vname entry against name character limit/number usage rule
                If IsNumeric(Left(termInfoTable(loopIdx, TERM_VNAME), 1)) Or Len(termInfoTable(loopIdx, TERM_VNAME)) > 6 Then
                    Return "Error - Instrument " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " VName is invalid.  String must be 6 characters or less and cannot lead with a number."
                    Exit Function
                ElseIf InStr(strNameList, termInfoTable(loopIdx, TERM_VNAME)) <> 0 Then
                    'checks name for uniqueness
                    Return "Error - Duplicate names are not allowed."
                    Exit Function
                Else
                    strLName = LCase(termInfoTable(loopIdx, TERM_VNAME))
                    strUName = UCase(termInfoTable(loopIdx, TERM_VNAME))
                    intNameLength = Len(strLName)
                    blnNameIsAlphaNum = True

                    'checks that only alphanumeric characters are used
                    For intLoopIdx = 1 To intNameLength
                        If InStr(strLName, GetChar(strUName, intLoopIdx), CompareMethod.Binary) <> 0 And Not IsNumeric(GetChar(strUName, intLoopIdx)) Then
                            blnNameIsAlphaNum = False
                            Exit For
                        End If
                    Next

                    If Not blnNameIsAlphaNum Then
                        Return "Error - Port " & termInfoTable(loopIdx, TERM_INSTRUMENT) & " VName is invalid.  String must be strictly alphanumeric."
                        Exit Function
                    Else
                        strNameList = strNameList & termInfoTable(loopIdx, TERM_VNAME) & " "
                    End If
                End If

                'Check for the current name when we start measuring currents
                
            Next

            'checks User Defined Function names if appropriate
            For loopIdx = 0 To udfNodeListLength - 1
                'checks udf name entry against name character limit/number usage rule
                If IsNumeric(Left(udfInfoTable(loopIdx, UDF_NAME), 1)) Or Len(udfInfoTable(loopIdx, UDF_NAME)) > 6 Then
                    Return "Error - User Defined Function " & udfInfoTable(loopIdx, UDF_NAME) & " Name is invalid.  String must be 6 characters or less and cannot lead with a number."
                    Exit Function
                ElseIf InStr(strNameList, udfInfoTable(loopIdx, UDF_NAME)) <> 0 Then
                    'checks name for uniqueness
                    Return "Error - Duplicate names are not allowed."
                    Exit Function
                Else
                    strLName = LCase(udfInfoTable(loopIdx, UDF_NAME))
                    strUName = UCase(udfInfoTable(loopIdx, UDF_NAME))
                    intNameLength = Len(strLName)
                    blnNameIsAlphaNum = True

                    'checks that only alphanumeric characters are used
                    For intLoopIdx = 1 To intNameLength
                        If InStr(strLName, GetChar(strUName, intLoopIdx), CompareMethod.Binary) <> 0 And Not IsNumeric(GetChar(strUName, intLoopIdx)) Then
                            blnNameIsAlphaNum = False
                            Exit For
                        End If
                    Next

                    If Not blnNameIsAlphaNum Then
                        Return "Error - User Defined Function " & udfInfoTable(loopIdx, UDF_NAME) & " Name is invalid.  String must be strictly alphanumeric."
                        Exit Function
                    Else
                        strNameList = strNameList & udfInfoTable(loopIdx, UDF_NAME) & " "
                    End If

                End If

            Next

            Dim strDLNames() As String = Split(Left(listItem, InStrRev(listItem, ",")), ", ")

            If strDLNames.Length() < 2 Then
                'checks that at least 2 variables are being downloaded
                Return "Error - At least two variables must be selected for download."
                Exit Function
            ElseIf strDLNames.Length() > 8 Then
                'checks that no more than 8 variables are being downloaded
                Return "Error - No more than eight variables may be selected for download."
                Exit Function
            End If

            'checks terminals for function types and sets up function record pointers for function checks.
            For loopIdx = 0 To termNodeListLength - 1
                Select Case UCase(termInfoTable(loopIdx, TERM_FUNCTION_TYPE))
                    Case "WAVEFORM"
                        'creates a reference to a WAVEFORM function.  Checks that only 1 terminal is set as WAVEFORM
                        If intWaveformDefRec = -1 Then
                            intWaveformDefRec = FGEN_FUNCT
                        Else
                            Return "Error - Only one terminal may be set as INPUT WAVEFORM."
                            Exit Function
                        End If
                    Case "SAMPLING"
                        'creates a reference to a SAMPLING function.  Checks that only 1 terminal is set as SAMPLING
                        If intSamplingDefRec = -1 Then
                            intSamplingDefRec = SCOPE_FUNCT
                        Else
                            Return "Error - Only one terminal may be set as OUTPUT WAVEFORM."
                            Exit Function
                        End If
                End Select
            Next

            'tests each terminal/function for compliance/max value correctness
            Dim dblCurrTermMaxAmp, dblCurrTermMaxOffset, dblCurrTermMaxI, dblCurrTermMaxF, dblCurrTermMaxSamplingRate, dblCurrTermMaxSamplingTime, dblCurrTermMaxPoints, dblSignChk, dblCurrFuncOffset As Double
            Dim foundRec As Boolean = False

            'get max datapoints value
            'strDBQuery = "SELECT max_points FROM Setups WHERE setup_id = @setupID;"
            'cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            'cmdDBQuery.Parameters.Add("@setupID", intSetupID)

            'intPointLimit = CInt(cmdDBQuery.ExecuteScalar())

            'check compliance values for each terminal
            For loopIdx = 0 To termNodeListLength - 1
                strDBQuery = "SELECT max_amplitude, max_offset, max_current,max_frequency, max_sampling_rate, max_sampling_time, max_points FROM SetupTerminalConfig WHERE setup_id = @setupID AND instrument = @instrument"
                cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
                cmdDBQuery.Parameters.Add("@setupID", intSetupID)
                cmdDBQuery.Parameters.Add("@instrument", UCase(termInfoTable(loopIdx, TERM_INSTRUMENT)))
                dtrDBQuery = cmdDBQuery.ExecuteReader()

                While dtrDBQuery.Read()
                    foundRec = True
                    dblCurrTermMaxAmp = dtrDBQuery("max_amplitude")
                    dblCurrTermMaxOffset = dtrDBQuery("max_offset")
                    dblCurrTermMaxI = dtrDBQuery("max_current")
                    dblCurrTermMaxF = dtrDBQuery("max_frequency")
                    dblCurrTermMaxSamplingRate = dtrDBQuery("max_sampling_rate")
                    dblCurrTermMaxSamplingTime = dtrDBQuery("max_sampling_time")
                    dblCurrTermMaxPoints = dtrDBQuery("max_points")
                End While

                dtrDBQuery.Close()

                If Not foundRec Then
                    Return "Error - Could not retrieve terminal compliance from server, aborting."
                    Exit Function
                End If

                Select Case UCase(termInfoTable(loopIdx, TERM_INSTRUMENT))
                    Case "FGEN"
                        Select Case UCase(termInfoTable(loopIdx, TERM_FUNCTION_TYPE))
                            Case "WAVEFORM"
                                'validate waveformType, frequency, amplitude and offset values against
                                ' values stored in the database
                                'FREQUENCY
                                If functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY) = "" Or Not IsNumeric(functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY)) Then
                                    Return "Error - A numeric frequency value for FGEN must be supplied."
                                    Exit Function
                                End If
                                If functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY) < 10 Or functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY) > dblCurrTermMaxF Then
                                    Return "Error - The frequency value " & functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY) & " provided is not within the acceptable range (10 to " & dblCurrTermMaxF & "Hz)."
                                    Exit Function
                                End If

                                'WAVEFORMTYPE
                                Dim strWaveformType As String = functInfoTable(FGEN_FUNCT, FUNCT_WAVEFORMTYPE)
                                If strWaveformType <> "SINE" And strWaveformType <> "TRIANGULAR" And strWaveformType <> "SQUARE" Then
                                    Return "Error - Invalid waveformType value " & strWaveformType & " for FGEN supplied."
                                    Exit Function
                                End If

                                'AMPLITUDE
                                If functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE) = "" Or Not IsNumeric(functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE)) Then
                                    Return "Error - A numeric amplitude value for FGEN must be supplied."
                                    Exit Function
                                End If
                                ' ensure that the amplitude is within the acceptable range
                                If functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE) < 0 Or functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE) > dblCurrTermMaxAmp Then
                                    Return "Error - The amplitude value " & functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE) & " provided is not within the acceptable range (0 to " & dblCurrTermMaxAmp & "V)."
                                    Exit Function
                                End If


                                'OFFSET
                                If functInfoTable(FGEN_FUNCT, FUNCT_OFFSET) = "" Or Not IsNumeric(functInfoTable(FGEN_FUNCT, FUNCT_OFFSET)) Then
                                    Return "Error - A numeric offset value for FGEN must be supplied."
                                    Exit Function
                                End If
                                ' ensure that the magnitude of the offset does not exceed the required limits
                                dblCurrFuncOffset = functInfoTable(FGEN_FUNCT, FUNCT_OFFSET)
                                If System.Math.Abs(dblCurrFuncOffset) > dblCurrTermMaxOffset Then
                                    Return "Error - The offset value provided is not within the acceptable range (between +/- " & dblCurrTermMaxOffset & "V)."
                                    Exit Function
                                End If
                        End Select
                    Case "SCOPE"
                        Select Case UCase(termInfoTable(loopIdx, TERM_FUNCTION_TYPE))
                            Case "SAMPLING"
                                'validate samplingRate and samplingTime values against
                                ' values stored in the database
                                'SAMPLING RATE
                                If functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE) = "" Or Not IsNumeric(functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE)) Then
                                    Return "Error - A numeric sampling rate value for SCOPE must be supplied."
                                    Exit Function
                                End If
                                If functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE) <= 0 Or functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE) > dblCurrTermMaxSamplingRate Then
                                    Return "Error - The sampling rate value " & functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE) & " provided is not within the acceptable range (1 to " & dblCurrTermMaxSamplingRate & " samples/sec)."
                                    Exit Function
                                End If

                                'SAMPLING TIME
                                If functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME) = "" Or Not IsNumeric(functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME)) Then
                                    Return "Error - A numeric sampling time value for SCOPE must be supplied."
                                    Exit Function
                                End If
                                If functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME) <= 0 Or functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME) > dblCurrTermMaxSamplingTime Then
                                    Return "Error - The sampling time value " & functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME) & " provided is not within the acceptable range (0 to " & dblCurrTermMaxSamplingTime & " sec)."
                                    Exit Function
                                End If

                                'ensure that the user plans to acquire a legal number of samples
                                Dim samples As Double = functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE) * functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME)
                                If samples < 10 Or samples > dblCurrTermMaxPoints Then
                                    Return "Error - You must specify a sampling rate and time to acquire between 10 and " & dblCurrTermMaxPoints & " samples."
                                    Exit Function
                                End If



                            Case Else
                                'unrecognized function type supplied
                                Return "Error - Unrecognized function on FGEN, aborting."
                                Exit Function
                        End Select
                End Select
            Next

            'spec test conditionals go here 
            ' SKIP UDFs For NOW!!
            Return "SUCCESS"

        End Function


    End Class

End Namespace