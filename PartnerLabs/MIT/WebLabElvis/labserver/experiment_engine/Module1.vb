Imports System.Data.SqlClient
Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Threading
Imports OpAmpInverter.InstrumentDriverInterop.Ivi

'Author(s): James Hardison (hardison@alum.mit.edu)
'Date: 5/5/2003
'This module contains the functions and subroutines of the experiment engine created for
'WebLab Version 6.  The executable compiled from this module will be started once at
'server boot and will operate in the backround automatically.  Specifically, this module 
'checks the experiment queue for unexecuted jobs and, if they exist, dequeues the job, 
'parses the XML experiment specification, configures the test equipment with the parsed 
'data and executes the job.  When results are returned the data and or error message is 
'written, in XML, to the experiment record in the database and the the module then checks 
'the queue for more jobs.  If there are more, job execution continues.  If not, the program
'will check again in 5 seconds.

'Dependencies: 
'    1.notify.aspx must exist in the webroot of the local machine for Notify() to
'work properly.
'    2.the local database must have a properly permissioned user account with the 
'information set in line 82 of this file.

Module Module1
    Dim conWebLabLS As SqlConnection
    Dim strExpID, strExpSpec, strWarningMsg, listItem, listUnits As String
    Dim intSetupID As Integer
    Dim termInfoTable(,), udfInfoTable(,), functInfoTable(,) As String
    Dim expEngIsActive As Boolean
    Dim termNodeListLength, udfNodeListLength As Integer
    'Dim HP34970A As Therm34970.Therm34970_Session = New Therm34970.Therm34970_Session()
    ' Initialize the session with ELVIS
    Dim xmlExpSpec, xmlTemp As XmlDocument


    Dim FGEN_record As Integer = -1 '-1 is default, unassigned value, dependency: setPort
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
    Sub Main()
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand
        Dim dtrOutput As SqlDataReader
        conWebLabLS = New SqlConnection("DataBase=ELVIS_LS;Server=localhost;Integrated Security=true")
        conWebLabLS.Open()

        Debug.WriteLine("MAIN SUB STARTED")

        'checks for any abandoned jobs, recovers if found
        strDBQuery = "EXEC qm_RecoverJobs;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.ExecuteNonQuery()

        Dim strJobsQueued As String
        Dim blnLoopVar As Boolean = True
        'Initialize ELVIS
        'InitExecLoop:
        Do While blnLoopVar

            Debug.WriteLine("InitExecLoop Entered")
            'checks experiment queue for pending jobs
            strDBQuery = "SELECT dbo.qm_CheckQueue();"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            strJobsQueued = cmdDBQuery.ExecuteScalar()

            Debug.WriteLine("queue checked: result=" & strJobsQueued)
            'insert call to LoadJob() here.  LoadJob() returns a dataReader
            If strJobsQueued = "TRUE" Then
                'loads current copy of system specs
                GetCurrentSystemSpecs()
                Debug.WriteLine("System vars loaded: ExpEng is active? " & expEngIsActive)

                If expEngIsActive Then
                    'if experiment engine is set to active
                    'dequeue, load and parse an experiment
                    LoadJob(strExpID, strExpSpec)
                    
                    Debug.WriteLine("job Loaded: ID=" & strExpID)

                    ParseExperimentSpec(strExpSpec)

                    'configure lab hardware and execute experiment

                    'if present sets the switching matrix to proper channel
                    
                    'clear old warning messages
                    strWarningMsg = ""

                    'execute the experiment
                    runExperiment()

                    'if switching matrix is present, resets to default channel

                    'job is complete

                    FGEN_record = -1  'resets to default after execution
                    SCOPE_record = -1

                Else
                    Debug.WriteLine("preparing to sleep for 1 sec." & Now())
                    Thread.Sleep(1000)
                    Debug.WriteLine("waking..." & Now())
                End If
            Else
                Debug.WriteLine("preparing to sleep for 5 sec." & Now())
                Thread.Sleep(5000)
                Debug.WriteLine("waking..." & Now())
            End If

        Loop
        'GoTo InitExecLoop

    End Sub

    Private Sub GetCurrentSystemSpecs()
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand
        Dim dtrOutput As SqlDataReader

        'retrieves and sets lab system variables
        strDBQuery = "SELECT exp_eng_is_active  FROM LSSystemConfig WHERE SetupID = '1';"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        dtrOutput = cmdDBQuery.ExecuteReader(CommandBehavior.SingleRow)

        If dtrOutput.Read() Then
            If dtrOutput("exp_eng_is_active") = "True" Then
                expEngIsActive = True
            Else
                expEngIsActive = False
            End If
        End If
        dtrOutput.Close()

    End Sub

    Private Sub runExperiment()
        'Dim SMU1_setup, SMU2_setup, SMU3_setup, SMU4_setup, VSU1_setup, VSU2_setup, VMU1_setup, VMU2_setup As String
        Dim strDBQuery, strUDFName, strUDFUnits, strUDFBody, strUDF, strConnect, strXMLExpResult As String
        Dim strResult As String()
        Dim dblResult()() As Double
        Dim cmdDBQuery As SqlCommand
        Dim dtrOutput As SqlDataReader
        Dim ELVIS_Session As Inverter
        ' Restart the inverter
        Debug.WriteLine("Restarting the ELVIS session")
        
        ELVIS_Session = New Inverter
        ' get the parameters that were read on from the xml specs

        'setup the constants, VAR1, VAR1P, VAR2

        'set up the variables selected for download
        Debug.WriteLine("setting download variables")
        'listItem = RTrim(listItem)
        'listItem = Left(listItem, (Len(listItem) - 1)) 'removes extra comma from end of string
        'collates output variable units
        'listUnits = RTrim(listUnits)
        'listUnits = Left(listUnits, (Len(listUnits) - 1)) 'removes extra comma from end of string

        'Dim listItemArray() As String = Split(listItem, ",")
        'Dim listUnitArray() As String = Split(listUnits, ",")
        'Debug.WriteLine(">" & listItem & "<")

        'execute the job
        'Dim dbltime As Double
        'dbltime = Timer()
        'Debug.WriteLine("executing...")

        'flush the read buffer

        'set timeout to 5 minutes while waiting for measurement to complete
        'wait for it to finish
        'Debug.WriteLine("job completed, getting results:" & CStr(Timer() - dbltime))
        'set timeout back to 1 min

        'flush the read buffer

        ' FOR now we know there is only one setup available, so just go straight to the values

        ReDim strResult(3)
        Dim frequency As Double = functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY)
        Dim amplitude As Double = functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE)
        Dim offset As Double = functInfoTable(FGEN_FUNCT, FUNCT_OFFSET)
        Dim waveformType As Double = functInfoTable(FGEN_FUNCT, FUNCT_WAVEFORMTYPE)
        Dim samplingRate As Double = functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGRATE)
        Dim samplingTime As Double = functInfoTable(SCOPE_FUNCT, FUNCT_SAMPLINGTIME)


        dblResult = ELVIS_Session.RunExperiment(frequency, amplitude, offset, waveformType, samplingRate, samplingTime)
        'dispose the inverter
        ELVIS_Session.Dispose()
        ELVIS_Session = Nothing

        ' generate the string corresponding to the returned values
        Dim j, k As Integer

        'populate the array of time values
        For j = 0 To UBound(dblResult(j)) - 1
            strResult(0) = strResult(0) & CStr(j * 1 / samplingRate) & " " ' samplingTime / 100) & " " JLH 10/12/06
        Next

        For j = 1 To UBound(dblResult)
            For k = 0 To UBound(dblResult(j - 1)) - 1
                strResult(j) = strResult(j) & CStr(dblResult(j - 1)(k)) & " "
            Next
        Next

        Debug.WriteLine("Executing the experiment with parameters: Frequency = " & functInfoTable(FGEN_FUNCT, FUNCT_FREQUENCY) & " Amplitude = " & functInfoTable(FGEN_FUNCT, FUNCT_AMPLITUDE) & " Offset = " & functInfoTable(FGEN_FUNCT, FUNCT_OFFSET) & "WaveformType = " & functInfoTable(FGEN_FUNCT, FUNCT_WAVEFORMTYPE))
        strXMLExpResult = "<?xml version='1.0' encoding='utf-8' standalone='no' ?><!DOCTYPE experimentResult SYSTEM 'http://ilab-labview.mit.edu/labserver/xml/experimentResult.dtd'><experimentResult lab='MIT NI-ELVIS Weblab' specversion='0.1'>"
        strXMLExpResult = strXMLExpResult & "<datavector name='TIME' units='s'>" & strResult(0) & "</datavector>"
        strXMLExpResult = strXMLExpResult & "<datavector name='VIN' units='V'>" & strResult(1) & "</datavector>"
        strXMLExpResult = strXMLExpResult & "<datavector name='VOUT' units='V'>" & strResult(2) & "</datavector>"
        strXMLExpResult = strXMLExpResult & "</experimentResult>"
        'Debug.WriteLine("results captured:" & CStr(Timer() - dbltime))
        'finish the job
        Debug.WriteLine("The xml result string is " & strXMLExpResult)

        strDBQuery = "EXEC qm_FinishJob @expID, @expResult, @errorMsg, @errorBit"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@expID", strExpID)
        cmdDBQuery.Parameters.Add("@expResult", strXMLExpResult)
        cmdDBQuery.Parameters.Add("@errorMsg", strWarningMsg)
        cmdDBQuery.Parameters.Add("@errorBit", "0")
        cmdDBQuery.ExecuteNonQuery()
        Debug.WriteLine("job finished")
        'informs WebLab Lab Server of job completion
        Notify()
    End Sub

    Private Sub Notify()
        Dim objRequest As HttpWebRequest
        Dim objResponse As HttpWebResponse
        Dim srResponse As StreamReader
        Dim strResponse As String

        Try
            objRequest = CType(WebRequest.Create("http://localhost/notify.aspx?expID=" & strExpID), HttpWebRequest)
            objRequest.Method = "GET"

            objResponse = objRequest.GetResponse()
            srResponse = New StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.ASCII)
            strResponse = srResponse.ReadToEnd()
            srResponse.Close()


        Catch When Err.Number <> 0
            Exit Sub
        End Try

    End Sub

    Private Sub ParseExperimentSpec(ByRef strExpSpec As String)
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

            'load function type & process sub-nodes. Change this condition as soon as we determine how the modes will
            'work for ELVIS
            'If (instrumentType <> "VMU") And (termInfoTable(loopIdx, TERM_MODE) <> "COMM") Then 'should be <> "COMM" ?
            tempXPath = "/terminal/function/@type"
            tempNode = xmlTemp.SelectSingleNode(tempXPath)
            termInfoTable(loopIdx, TERM_FUNCTION_TYPE) = Trim(tempNode.InnerXml())
            Debug.WriteLine("function type=" & Trim(tempNode.InnerXml()))

            Select Case termInfoTable(loopIdx, TERM_FUNCTION_TYPE)
                Case "WAVEFORM"
                    'load waveformTYpe value
                    tempXPath = "/terminal/function/waveformType"
                    tempNode = xmlTemp.SelectSingleNode(tempXPath)
                    Select Case Trim(tempNode.InnerXml())
                        Case "SINE"
                            functInfoTable(instrumentConstant, FUNCT_WAVEFORMTYPE) = 0
                        Case "TRIANGULAR"
                            functInfoTable(instrumentConstant, FUNCT_WAVEFORMTYPE) = 1
                        Case "SQUARE"
                            functInfoTable(instrumentConstant, FUNCT_WAVEFORMTYPE) = 2
                    End Select
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
            'End If
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

    End Sub


    Private Sub LoadJob(ByRef strExpID As String, ByRef strExpSpec As String)
        Dim strLoadJob, strLogConfig, strDBQuery, strGroup, strXMLOutput As String
        Dim cmdLoadJob, cmdLogConfig, cmdDBQuery As SqlCommand
        Dim dtrLoadJob, dtrSetupList, dtrTermList As SqlDataReader
        Dim intBrokerID, intGroupID, intClassID As Integer
        Dim conWebLabLS2 As SqlConnection = New SqlConnection("DataBase=ELVIS_LS;Server=localhost;Integrated Security=true")

        'Load job
        strLoadJob = "EXEC qm_LoadJob;"
        cmdLoadJob = New SqlCommand(strLoadJob, conWebLabLS)

        dtrLoadJob = cmdLoadJob.ExecuteReader(CommandBehavior.SingleRow)

        If dtrLoadJob.Read() Then
            strExpID = dtrLoadJob("expID")
            strExpSpec = dtrLoadJob("exp")
            strGroup = dtrLoadJob("groups")
            intBrokerID = dtrLoadJob("provider_id")
        End If
        dtrLoadJob.Close()

        'determine if broker id or group affiliate should be used for lab config creation
        strDBQuery = "SELECT dbo.rpm_GetGroupID(@BrokerID, @GroupName);"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@BrokerID", intBrokerID)
        cmdDBQuery.Parameters.Add("@GroupName", strGroup)

        intGroupID = CInt(cmdDBQuery.ExecuteScalar())

        If intGroupID > 0 Then
            'use group id
            strDBQuery = "SELECT ClassID FROM dbo.rpm_GroupIsMemberOf(@groupID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)

            intClassID = CInt(cmdDBQuery.ExecuteScalar())
        Else
            'use broker id
            strDBQuery = "SELECT ClassID FROM dbo.rpm_BrokerIsMemberOf(@brokerID)"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", intBrokerID)

            intClassID = CInt(cmdDBQuery.ExecuteScalar())
        End If

        'begin constructing lab config 
        conWebLabLS2.Open()

        'write document header
        strXMLOutput = "<?xml version='1.0' encoding='utf-8' standalone='no' ?><!DOCTYPE labConfiguration SYSTEM 'http://web.mit.edu/weblab/xml/labConfiguration.dtd'><labConfiguration lab='MIT ELVIS Weblab' specversion='0.1'>"

        'get available setup list
        strDBQuery = "SELECT setupID, setupName, setupDesc, setupImageLoc FROM dbo.rpm_GetActiveSetupList(@ClassID) ORDER BY setupID;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@ClassID", intClassID)
        dtrSetupList = cmdDBQuery.ExecuteReader()

        While dtrSetupList.Read()

            'write experiment setup definitions to the document
            strXMLOutput = strXMLOutput & "<setup id='" & dtrSetupList("setupID") & "'>"
            strXMLOutput = strXMLOutput & "<name>" & dtrSetupList("setupName") & "</name>"
            strXMLOutput = strXMLOutput & "<description>" & dtrSetupList("setupDesc") & "</description>"
            strXMLOutput = strXMLOutput & "<imageURL>" & dtrSetupList("setupImageLoc") & "</imageURL>"

            'get setup terminal information
            strDBQuery = "SELECT termInstrument, termNumber, termName, termXLoc, termYLoc FROM dbo.rpm_GetSetupTerminalInfo(@SetupID) ORDER BY termNumber;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS2)
            cmdDBQuery.Parameters.Add("@SetupID", dtrSetupList("setupID"))
            dtrTermList = cmdDBQuery.ExecuteReader()

            While dtrTermList.Read()
                'write setup terminal info to document
                strXMLOutput = strXMLOutput & "<terminal instrumentType='" & dtrTermList("termInstrument") & "' instrumentNumber='" & dtrTermList("termNumber") & "'>"
                strXMLOutput = strXMLOutput & "<label>" & dtrTermList("termName") & "</label>"
                strXMLOutput = strXMLOutput & "<pixelLocation><x>" & dtrTermList("termXLoc") & "</x><y>" & dtrTermList("termYLoc") & "</y></pixelLocation>"
                strXMLOutput = strXMLOutput & "</terminal>"
            End While

            dtrTermList.Close()
            strXMLOutput = strXMLOutput & "</setup>"
        End While

        dtrSetupList.Close()
        conWebLabLS2.Close()

        strXMLOutput = strXMLOutput & "</labConfiguration>"

        'write lab config to exp record

        strDBQuery = "EXEC qm_LogConfigAtExec @expID, @labConfig;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@expID", strExpID)
        cmdDBQuery.Parameters.Add("@labConfig", strXMLOutput)

        cmdDBQuery.ExecuteNonQuery()
    End Sub

    Private Sub jobError(ByVal strErrMsg As String)
        Dim strFinishJob As String
        Dim cmdFinishJob As SqlCommand

        strFinishJob = "EXEC qm_FinishJob @expID, @expResults, @errorMsg, @errorBit"
        cmdFinishJob = New SqlCommand(strFinishJob, conWebLabLS)
        cmdFinishJob.Parameters.Add("@expID", strExpID)
        cmdFinishJob.Parameters.Add("@expResults", "")
        cmdFinishJob.Parameters.Add("@errorMsg", strErrMsg)
        cmdFinishJob.Parameters.Add("@errorBit", "1")
        cmdFinishJob.ExecuteNonQuery()
        Debug.WriteLine("Job Error: " & strErrMsg)
    End Sub

End Module
