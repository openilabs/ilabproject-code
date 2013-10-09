Imports System.ServiceProcess
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Threading
Imports WebLabSystemComponents
Imports WebLabCustomDataTypes
Imports MATRIX.Matrix_SessionClass
Imports HP4155.HP4155_SessionClass
Imports Therm34970.Therm34970_SessionClass

'Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
'Please see license.txt in top level directory for full license. 

'Author(s): James Hardison (hardison@alum.mit.edu)
'Date: 9/14/2005
'This module contains the functions and subroutines of the Experiment Execution Engine created for the v6.1 revision of the Microelectronics
'WebLab Lab Server.  This is primarily a rework of the original, 6.0, lab server experiment execution engine.  The methodologies and 
'mechanisms are largely untouched.  This process will be started automatically at server boot and will run constantly in the background.  
'Specifically, this module checks the experiment queue for unexecuted jobs and, if they exist, dequeues the job, parses the XML experiment 
'specification, configures the test equipment with the parsed data and executes the job.  when results are returned, the data and/or error 
'message is written, in XML, to the experiment record in the Lab Server database.  The procedure then checks the queue for more jobs.  If 
'there are more, job execution continues without interuption.  If there are no jobs, the program returns to its initial wait state - polling
'the job queue every 5 seconds for new jobs.
'
'This module contains all updates to the 6.0 execution engine up until 9/14/2005.  The original lab instrument-level drivers are this 
'program's interface to the lab hardware.  The primary operational differences between the 6.0
'version and this code are as follows:
'
'1. This module is developed/compiled as a windows service.  Runs in a dedicated background process.  Should provide better stability,
'performance, managability as well as the ability to output status messages to the Windows Event Log.
'
'2. This code takes advantage of the work done on the Experiment Validation Engine and employs the ExperimentParser module to parse
'XML experiment specifications.  This is in contrast to previous version where identical but duplicate parsing code existed in both
'the Experiment Execution Engine and the Validation Engine. 
'
'Dependencies:
'   1. ExperimentParser:  This module governs the parsing of an XML encoded Experiment Specification document into executable lab
'instrumentation code. 
'       Namespace: WebLabSystemComponents
'       Source - /labserver/controls/SiteComponents/ExperimentParser/ExperimentParser.vb
'       Compiled DLL - /labserver/bin/ExperimentParser.dll
'
'   2. WebLabCustomDataTypes:  This module defines a set of custom data structures used by the WebLab Lab Server.  They are used here
'to facilitage experiment parsing.
'       Namespace: WebLabCustomDataTypes
'       Source - /labserver/controls/WebLabCustomDataTypes/WebLabCustomDataTypes.vb
'       Compiled DLL - /labserver/bin/WebLabCustomDataTypes.dll
'
'   2. Lab Instrumentation drivers (HP4155.dll, MATRIX.dll, Therm34970.dll): COM object type DLLs used to interface to the lab hardware
'via the system GBIP interface.  These drivers govern actual GBIP communication and low-level instrument command execution.
'       Compiled DLLs - /labserver/ExperimentExecEngine/obj
'
'   3. Notify.aspx:  This ASP.NET file performs the Notify WebService call on behalf of the execution engine.  This is performed at the
'end of each job execution.  This file must exist in the web root (/labserver/) of the Lab Server Web Application.
'
'   4. This implementation assumes that the Lab Server Database is a MS SQL Server (service launch dependency in Install method in 
'ProjectInstaller.vb in this project).  The SQL connection string in this class (line 64) and the specific DB client class used may 
'need to be changed in a different data store is used.  The Lab Server database must be permissioned such that this process can access the necessary data tables and stored procedures.  
'This module also requires that the lab server database be named WebLabServicesLS and be accessible by the server's LocalSystem account
'([domain]\[machinename]$).



Public Class ExpExecEngine
    Inherits System.ServiceProcess.ServiceBase

    'global variables
    Dim conWebLabLS As SqlConnection = New SqlConnection("Server=localhost;Database=WebLabServicesLS;Trusted_Connection=True")
    Dim HP4155_GPIBBusAddr, HPE5250A_GPIBBusAddr, HP34970A_GPIBBusAddr, HP34970A_dataChannel, visaName As String
    Dim intExpID As Integer
    Dim intTotalRunCount, intErrorCount As Integer
    Dim dtProcStart As DateTime
    Dim tsUpTime As TimeSpan
    Dim strUpTimeRpt, strHomepage, strErrorMsg, strWarningMsg, listNames, listUnits As String
    Dim HPE5250A_isPresent, HP34970A_isPresent, expEngIsActive As Boolean
    Dim HP4155 As HP4155.HP4155_Session = New HP4155.HP4155_Session()
    Dim HPE5250A As MATRIX.Matrix_Session = New MATRIX.Matrix_Session()
    Dim HP34970A As Therm34970.Therm34970_Session = New Therm34970.Therm34970_Session()

    'measurement time/range adjust enabled
    Dim RangeParams As Boolean = False
    Dim TimeParams As Boolean = True

    'job execution state variables
    Dim blnJobLoaded As Boolean = False
    Dim bln4155Connected As Boolean = False
    Dim bln34970AConnected As Boolean = False

    'set timer interval settings
    Dim intActiveLoopTime As Integer = 100 '.1 seconds
    Dim intIdleLoopTime As Integer = 5000 ' 5 seconds


#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()

        ' This call is required by the Component Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call

        ''Is this needed for creating a new log (or is that in the InitializeComponent Sub sufficient)?
        'If Not System.Diagnostics.EventLog.SourceExists("WebLab Lab Server") Then
        'System.Diagnostics.EventLog.CreateEventSource("ExpExecEngine", "WebLab Lab Server")
        ''End If

    End Sub

    'UserService overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' The main entry point for the process
    <MTAThread()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' More than one NT Service may run within the same process. To add
        ' another service to this process, change the following line to
        ' create a second service object. For example,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New ExpExecEngine()}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    Friend WithEvents EventLogInterface As System.Diagnostics.EventLog
    Friend WithEvents SleepTimer As System.Timers.Timer
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.EventLogInterface = New System.Diagnostics.EventLog()
        Me.SleepTimer = New System.Timers.Timer()
        CType(Me.EventLogInterface, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SleepTimer, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'EventLogInterface
        '
        'Me.EventLogInterface.Log = "WebLab Lab Server"
        Me.EventLogInterface.Source = "ExpExecEngine"
        '
        'SleepTimer
        '
        Me.SleepTimer.Enabled = False 'started by onStart after DB connection is established
        Me.SleepTimer.Interval = intActiveLoopTime
        '
        'ExpExecEngine
        '
        Me.CanShutdown = True
        Me.ServiceName = "ExpExecEngine"
        CType(Me.EventLogInterface, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SleepTimer, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

#End Region

    '----Service Controller Event Override Methods----

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand

        intTotalRunCount = 0
        intErrorCount = 0

        Debug.WriteLine("Process Started, opening database connection")

        Try
            conWebLabLS.Open()

        Catch err As Exception
            'catch in order to log error, but re-throw to cause start to timeout (so process doesn't run without an open database connection)

            EventLogInterface.WriteEntry("An error occurred while starting service: " & err.ToString)
            Debug.WriteLine("An error occurred while starting service: " & err.ToString)

            Throw New Exception(err.ToString())
        End Try

        Debug.WriteLine("Database connection opened, recovering abandoned jobs")

        Try
            'check for jobs abandoned during shutdown/process stop (jobs in progress at stop, marks as queued and gives highest priority)
            RecoverAbandonedJobs()

            Debug.WriteLine("abandoned jobs recovered, logging start event")

            EventLogInterface.WriteEntry("Experiment Execution Engine Started: Abandoned jobs recovered.")

        Catch err As Exception
            EventLogInterface.WriteEntry("An error occurred while starting service: " & err.ToString)
            Debug.WriteLine("An error occurred while starting service: " & err.ToString)
        End Try

        'start timer.  set by default to active loop interval.  Job execution should begine in 0.1 sec.

        'REMOVED 2/24/06 to prevent service from failing due to long job execution. - JH
        'make initial queue check/run without waiting one timer cycle
        'Debug.WriteLine("launching ExecuteQueuedJob")

        'Try
        '    'only catches pre- and post-job errors.  mid-job errors (where message should be returned as experiment result) handled at lower levels

        '    ExecuteQueuedJob()

        'Catch e As Exception
        '    EventLogInterface.WriteEntry("An error occurred while executing job during service start: " & e.ToString)

        '    Debug.WriteLine("An error occurred while starting service: " & e.ToString)
        'End Try

        dtProcStart = Date.Now()

        'start job loop timer
        Me.SleepTimer.Start()

    End Sub

    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        Me.SleepTimer.Stop()

        Debug.WriteLine("stopping service")

        conWebLabLS.Close()
        Debug.Close()

        tsUpTime = Date.Now().Subtract(dtProcStart)
        strUpTimeRpt = tsUpTime.Days() & " days, " & tsUpTime.Hours() & " hours, " & tsUpTime.Minutes() & " minutes, " & tsUpTime.Seconds() & "." & tsUpTime.Milliseconds() & " seconds"

        EventLogInterface.WriteEntry("Experiment Execution Engine service stopped.  " & intTotalRunCount & " jobs run during process lifetime (" & strUpTimeRpt & ").  " & intErrorCount & " jobs ended with errors.")
    End Sub

    Protected Overrides Sub OnShutdown()
        'Governs cleanup in preparation for a system shutdown
        Me.SleepTimer.Stop()

        Debug.WriteLine("shutting down service")

        conWebLabLS.Close()
        Debug.Close()

        tsUpTime = Date.Now().Subtract(dtProcStart)
        strUpTimeRpt = tsUpTime.Days() & " days, " & tsUpTime.Hours() & " hours, " & tsUpTime.Minutes() & " minutes, " & tsUpTime.Seconds() & "." & tsUpTime.Milliseconds() & " seconds"

        EventLogInterface.WriteEntry("Experiment Execution Engine service shutting down.  " & intTotalRunCount & " jobs run during process lifetime (" & strUpTimeRpt & ").  " & intErrorCount & " jobs ended with errors.")
    End Sub




    '----Private Event Handlers----

    Private Sub SleepTimer_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles SleepTimer.Elapsed
        'event handler for the Sleep Timer object for the Interval Elapsed event.
        SleepTimer.Stop()

        'run job
        Debug.WriteLine("launching ExecuteQueuedJob")

        Try
            'only catches pre- and post-job errors.  mid-job errors (where message should be returned as experiment result) handled at lower levels
            ExecuteQueuedJob()
        Catch err As Exception
            EventLogInterface.WriteEntry("An error occurred while executing job: " & err.ToString)

            Debug.WriteLine("An error occurred during job execution: " & err.ToString)
        End Try

        SleepTimer.Start()
    End Sub


    '----Private Execution Engine Helper Methods----

    Private Sub RecoverAbandonedJobs()
        'this method executes a SQL stored procedure that changes the job status of any job marked as "IN PROGRESS" to "QUEUED" and gives it the 
        'maximum job priority.  Used in the case that an error or process termination results in a job being started but not completed (forces the 
        'job to eb re-run)
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand

        strDBQuery = "EXEC qm_RecoverJobs;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.ExecuteNonQuery()
    End Sub

    Private Sub ExecuteQueuedJob()
        Dim strConnect As String
        Dim jrCurrJob As JobRecord
        Dim eoParserOut As ExperimentObject
        Dim epObject As New ExperimentParser()

        'reset intExpID
        intExpID = 0

        'This method is the top level method for the execution of lab experiments.  Queue checking, job loading and execution are all
        'mananaged by this method.

        If CheckQueue() Then
            'prepare to load and execute job
            'loads current copy of system specs
            GetCurrentSystemSpecs()

            Debug.WriteLine("System vars loaded: 4155GBA=" & HP4155_GPIBBusAddr & ", ExpEng is active? " & expEngIsActive)

            If expEngIsActive Then
                'if experiment engine is set to active
                'dequeue, load and parse an experiment
                jrCurrJob = LoadJob()


                Try
                    'Begin top-level experiment run Try Catch statement (catch all for procedural errors while an experiment record is being 
                    'worked on.  Ensures that no jobs are left cycling through the engine because of a procedural error.  Does not supercede
                    'case-specific error handling lower in the process.

                    'create and record current lab configuration for the job owner
                    RecordLabConfig(jrCurrJob)

                    'set job state varialble
                    blnJobLoaded = True '(both vars re-initialized by runExperiment)
                    'intExpID = jrCurrJob.experimentID

                    intTotalRunCount = intTotalRunCount + 1
                    'increment job counter

                    'set intExpID
                    intExpID = jrCurrJob.experimentID

                    Debug.WriteLine("Job Loaded: ID=" & intExpID)

                    'parse XML experiment specification
                    Try
                        Debug.WriteLine("parsing job")

                        eoParserOut = epObject.parseXMLExpSpec(jrCurrJob.experimentSpec)
                    Catch e As Exception
                        strErrorMsg = "System Error: An error was thrown during experiment parsing.  Aborting job.  Underlying error: " & e.GetBaseException.Message() & " in " & e.GetBaseException.Source() & ".  Stack Trace: " & e.GetBaseException.StackTrace()
                        jobError(strErrorMsg, jrCurrJob.experimentID)
                        'wrap up job and prepare to start active loopback
                        SleepTimer.Interval = intActiveLoopTime
                        Exit Sub
                    End Try

                    'configure lab hardware and execute experiment

                    'if present, sets the switching matrix to the proper channel
                    If HPE5250A_isPresent Then
                        Debug.WriteLine("connecting to HP5250A.  Setting matrix to channel " & eoParserOut.Item("DeviceNum") & ".")

                        strConnect = deviceConnect(eoParserOut.Item("DeviceNum"), HPE5250A_GPIBBusAddr, visaName)
                        If strConnect <> "SUCCESS" Then
                            strErrorMsg = "Error connecting to switching matrix: " & strConnect
                            jobError(strErrorMsg, jrCurrJob.experimentID)

                            'wrap up job and prepare to start active loopback
                            SleepTimer.Interval = intActiveLoopTime
                            Exit Sub
                        End If
                    End If

                    'if present, sets the thermometer connection and read units to Kelvin
                    If HP34970A_isPresent Then
                        Debug.WriteLine("Connecting tp HP34970A.")

                        strConnect = connectTherm(HP34970A_GPIBBusAddr, HP34970A_dataChannel)
                        If strConnect <> "SUCCESS" Then
                            strErrorMsg = "Error connecting to the thermometer: " & strConnect
                            jobError(strErrorMsg, jrCurrJob.experimentID)

                            'wrap up job and prepare to start active loopback
                            SleepTimer.Interval = intActiveLoopTime
                            Exit Sub
                        End If

                        'set state variable
                        bln34970AConnected = True

                        HP34970A.SetUnits("K")
                    End If

                    'clear old warning messages
                    strWarningMsg = ""

                    Debug.WriteLine("Calling runExperiment...")

                    'configure the 4155 and execute the experiment
                    runExperiment(eoParserOut, jrCurrJob.experimentID)

                Catch e As Exception
                    strErrorMsg = "System Error: An error was thrown during experiment execution.  Aborting job.  Underlying error: " & e.GetBaseException.Message() & " in " & e.GetBaseException.Source() & ".  Stack Trace: " & e.GetBaseException.StackTrace()
                    jobError(strErrorMsg, jrCurrJob.experimentID)
                    'wrap up job and prepare to start active loopback
                    SleepTimer.Interval = intActiveLoopTime

                    'allow remainder of sub to complete.  If not effected, will clean up system state.  Otherwise, will trip higher level, job
                    'independent catch.  But set timer interval, just in case
                End Try

                'informs WebLab Lab Server Web Server process of job completion (run in separate thread)
                Dim notifyThread As New Thread(AddressOf Notify)
                notifyThread.Start()
                'Notify(jrCurrJob.experimentID)

                'if present, disconnect from thermometer
                If HP34970A_isPresent And bln34970AConnected Then
                    Debug.WriteLine("disconnecting from HP34970A.")
                    disconnectTherm()

                    'set state variable
                    bln34970AConnected = False
                End If

                'if present, reset switching matrix to default channel
                If HPE5250A_isPresent Then
                    Debug.WriteLine("resetting HP5250A to default channel (0).")
                    strConnect = deviceConnect("0", HPE5250A_GPIBBusAddr, visaName)
                    If strConnect <> "SUCCESS" Then
                        strErrorMsg = "Error connecting to switching matrix: " & strConnect
                        'jobError(strErrorMsg, jrCurrJob.experimentID, False)

                        'don't worry about this, since job has already completed?  REVIEW
                    End If
                End If

                'job is complete

                'any cleanup methods go here



                'set timer interval to .1 seconds (in case there are more jobs in the queue (speed up poll interval)
                SleepTimer.Interval = intActiveLoopTime

            End If
        Else
            'set timer interval to 5 seconds (slow down poll interval for non-consecutive jobs)
            SleepTimer.Interval = intIdleLoopTime
        End If


    End Sub

    Private Function CheckQueue() As Boolean
        'This method performs the CheckQueue query on the lab database.  If there are jobs in the queue, the boolean value "True" is 
        'returned.  Else, "False" is returned.
        Dim strDBQuery, strResult As String
        Dim cmdDBQuery As SqlCommand

        Debug.WriteLine("checking queue...")

        strDBQuery = "SELECT dbo.qm_CheckQueue();"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)

        strResult = cmdDBQuery.ExecuteScalar()

        If UCase(strResult) = "TRUE" Then
            Debug.WriteLine("Jobs found in the queue")
            Return True
        Else
            Debug.WriteLine("Queue empty")
            Return False
        End If
    End Function

    Private Function LoadJob() As JobRecord
        Dim strLoadJob As String
        Dim cmdLoadJob As SqlCommand
        Dim dtrLoadJob As SqlDataReader
        Dim jrCurrJob As JobRecord
        'This method loads the information for the first experiment on the lab server execution queue

        'Load Job
        strLoadJob = "EXEC qm_LoadJob;"
        cmdLoadJob = New SqlCommand(strLoadJob, conWebLabLS)

        dtrLoadJob = cmdLoadJob.ExecuteReader(CommandBehavior.SingleRow)

        If dtrLoadJob.Read() Then
            jrCurrJob.experimentID = dtrLoadJob("expID")
            jrCurrJob.experimentSpec = dtrLoadJob("exp")
            jrCurrJob.groupName = dtrLoadJob("groups")
            jrCurrJob.brokerID = dtrLoadJob("provider_id")
        End If
        dtrLoadJob.Close()


        'return JobRecord struct as function output
        Return jrCurrJob

    End Function

    Private Function getTemp() As String
        'gets temperature data from HP34970A
        Return HP34970A.GetTemp()
    End Function

    Private Function getUnits() As String
        'gets current unit of measure from the HP34970
        Return Replace(HP34970A.GetUnits(), Chr(10), "")
    End Function

    Private Function deviceConnect(ByRef strChannel As String, ByRef GPIBBusAddr As String, ByRef visaName As String) As String
        'Opens a connection to the HP5250A (switching matrix) and instructs the instrument to switch to the specified device
        Dim strResult

        strResult = HPE5250A.connect(visaName, GPIBBusAddr) 'connects to the switching matrix

        If strResult <> "SUCCESS" Then
            Return "Error connecting to HP5250A.  " & strResult
            Exit Function
        End If

        HPE5250A.DeviceChoose(strChannel)
        HPE5250A.waitUntilDone()
        HPE5250A.close_session()

        Return "SUCCESS"
    End Function

    Private Function connectTherm(ByRef GPIBBusAddr As String, ByRef dataChannel As String)
        'opens a GPIB connection to the HP34970A
        Return HP34970A.connect(GPIBBusAddr, dataChannel)
    End Function

    Private Function setPort(ByRef htTerminal As Hashtable) As String
        'This method creates the SCPI commands for setting channel assignment variables (vname, iname, function type, mode) for each terminal
        'and sends these commands to the 4155. 
        Dim strPort, strVName, strIName, strMode, strFunction, strCommand, strResult As String

        strPort = UCase(htTerminal.Item("port"))

        'define channel parameters based on the type of terminal (SMU, VMU, VSU)
        Select Case Left(strPort, 3)
            Case "SMU"
                strVName = htTerminal.Item("vname")
                strIName = htTerminal.Item("iname")
                strMode = UCase(htTerminal.Item("mode"))

                If htTerminal("dlVTerm") = "true" Then
                    'adds data vector to download list
                    listNames = listNames & "'" & strVName & "'" & ", "
                    listUnits = listUnits & "V, "
                End If

                If htTerminal("dlITerm") = "true" Then
                    'adds data vector to download list
                    listNames = listNames & "'" & strIName & "'" & ", "
                    listUnits = listUnits & "A, "
                End If

                'this conditional constructs terminal configuration command with "CONS" function declaration (COMM mode is always a CONS function)
                If strMode = "COMM" Then
                    strCommand = ":PAGE:CHAN:" & strPort & ":VNAM '" & strVName & "';"
                    strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":INAM '" & strIName & "';"
                    strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":MODE " & strMode & ";"
                    strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":FUNC CONS"
                Else
                    strFunction = UCase(htTerminal("functionType"))
                    If strFunction = "VAR1P" Then
                        strFunction = "VARD" 'applet encoding to command conversion for Var prime case
                    End If

                    strCommand = ":PAGE:CHAN:" & strPort & ":VNAM '" & strVName & "';"
                    strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":INAM '" & strIName & "';"
                    strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":MODE " & strMode & ";"
                    strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":FUNC " & strFunction
                End If
            Case "VSU"
                strVName = htTerminal.Item("vname")
                strFunction = UCase(htTerminal.Item("functionType"))

                If strFunction = "VAR1P" Then
                    strFunction = "VARD" 'applet encoding to command conversion for Var prime case
                End If

                If htTerminal("dlVTerm") = "true" Then
                    'adds data vector to download list
                    listNames = listNames & "'" & strVName & "'" & ", "
                    listUnits = listUnits & "V, "
                End If

                strCommand = ":PAGE:CHAN:" & strPort & ":VNAM '" & strVName & "';"
                strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":FUNC " & strFunction
            Case "VMU"
                strVName = htTerminal.Item("vname")
                strMode = UCase(htTerminal.Item("mode"))

                If htTerminal("dlVTerm") = "true" Then
                    'adds data vector to download list
                    listNames = listNames & "'" & strVName & "'" & ", "
                    listUnits = listUnits & "V, "
                End If

                strCommand = ":PAGE:CHAN:" & strPort & ":VNAM '" & strVName & "';"
                strCommand = strCommand & ":PAGE:CHAN:" & strPort & ":MODE " & strMode
            Case Else
                strErrorMsg = "Experiment Error: """ & strPort & """ is an invalid port type, aborting."

                Return strErrorMsg
                Exit Function
        End Select

        'send terminal configuration command to the 4155
        Debug.WriteLine("Sending command: " & strCommand)

        strResult = HP4155.command(strCommand)
        If strResult <> "SUCCESS" Then
            strErrorMsg = "System Error: Error setting port " & strPort & ".  Change the setup for this port and try again.  (" & strResult & ")"

            Return strErrorMsg
            Exit Function
        End If

        'if enabled, set additional rage settings for each port
        If RangeParams And Left(strPort, 3) = "SMU" Then
            'for SMU in I mode (measuring voltage)
            If strMode = "I" Then
                'hard code ranging mod temporarily
                strResult = HP4155.command(":PAGE:MEAS:MSET:" & strPort & ":RANG:MODE FIX")
                Debug.WriteLine("Setting the ranging mode for the port " & strPort & ": " & strResult)

                strResult = HP4155.command(":PAGE:MEAS:MSET:" & strPort & ":RANG 20")
                Debug.WriteLine("Setting range of this port: " & strResult)

            ElseIf strMode = "V" Then
                'for SMU in V mode (measuing current)
                strResult = HP4155.command("PAGE:MEAS:MSET:" & strPort & ":RANG:MODE FIX")
                Debug.WriteLine("Setting the ranging mode for the port " & strPort & ": " & strResult)

                strResult = HP4155.command("PAGE:MEAS:MSET:" & strPort & ":RANG 1")
                Debug.WriteLine("Setting range of this port: " & strResult)
            End If
        End If

        Return "SUCCESS"

    End Function

    Private Function setFnct(ByRef htTerminal As Hashtable) As String
        'This method creates the terminal function setup commands and sends them to the HP4155.  For the terminal function being setup, the 
        'corresponding terminal definition must be setup using setPort prior to execution of this method.  If an error occurs, this method 
        'returns an error message string.  Otherwise, the string "SUCCESS" is returned.

        Dim strPort, strMode, strFunction, strStart, strStep, strStop, strScale, strCompliance, strRatio, strOffset, strSource, strPoints, strCommand, strResult As String
        Dim sngStart, sngStep, sngStop As Single
        Dim intPoints As Integer

        strPort = UCase(htTerminal.Item("port"))
        strMode = UCase(htTerminal.Item("mode"))

        'create SCPI string setting function-specific parameters for SMU and VSU terminals
        If Left(strPort, 3) <> "VMU" Then
            'setup the VAR1, VAR2, VAR1P, CONS or COMM command line for this terminal
            If strMode = "COMM" Then
                'no function command is sent, return SUCCESS
                Return "SUCCESS"
                Exit Function

            Else
                strFunction = UCase(htTerminal("functionType"))
                Select Case strFunction
                    Case "VAR1"

                        strStart = ":PAGE:MEAS:VAR1:START " & htTerminal("start") & ";"
                        strStop = ":PAGE:MEAS:VAR1:STOP " & htTerminal("stop") & ";"

                        strStep = ""

                        Select Case UCase(htTerminal("scale"))
                            Case "LOG10"
                                strScale = ":PAGE:MEAS:VAR1:SPAC L10;"
                            Case "LOG25"
                                strScale = ":PAGE:MEAS:VAR1:SPAC L25;"
                            Case "LOG50"
                                strScale = ":PAGE:MEAS:VAR1:SPAC L50;"
                            Case ("LIN")
                                strScale = ":PAGE:MEAS:VAR1:SPAC " & UCase(htTerminal("scale")) & ";"
                                'also set step command (not needed for log cases)
                                strStep = ":PAGE:MEAS:VAR1:STEP " & htTerminal("step") & ";"
                            Case Else
                                'unrecognized scale
                                Return "Unrecognized scale (""" & htTerminal("scale") & """) on " & strPort
                                Exit Function
                        End Select

                        strCompliance = ":PAGE:MEAS:VAR1:COMP " & htTerminal("compliance")

                        strCommand = strScale & strStart & strStop & strStep & strCompliance

                    Case "VAR2"
                        strStart = ":PAGE:MEAS:VAR2:START " & htTerminal("start") & ";"
                        strStep = ":PAGE:MEAS:VAR2:STEP " & htTerminal("step") & ";"
                        strPoints = ":PAGE:MEAS:VAR2:POINTS " & CStr(Int((CSng(htTerminal("stop")) - CSng(htTerminal("start"))) / CSng(htTerminal("step"))) + 1) & ";"
                        strCompliance = ":PAGE:MEAS:VAR2:COMP " & htTerminal("compliance")

                        strCommand = strStart & strStep & strPoints & strCompliance

                    Case "VAR1P"
                        'var1 prime case.  weblab = VAR1P, HP4155 = VARD
                        strOffset = ":PAGE:MEAS:VARD:OFFS " & htTerminal("offset") & ";"
                        strRatio = ":PAGE:MEAS:VARD:RAT " & htTerminal("ratio") & ";"
                        strCompliance = ":PAGE:MEAS:VARD:COMP " & htTerminal("compliance")

                        strCommand = strOffset & strRatio & strCompliance

                    Case "CONS"
                        strSource = ":PAGE:MEAS:CONS:" & strPort & " " & htTerminal("value") & ";"
                        strCompliance = ":PAGE:MEAS:CONS:" & strPort & ":COMP " & htTerminal("compliance")

                        strCommand = strSource & strCompliance

                    Case Else
                        'unrecognized function
                        Return "Unrecognized function (""" & strFunction & """) on " & strPort
                        Exit Function
                End Select
            End If
            Debug.WriteLine("sending function configuration command: " & strCommand)

            strResult = HP4155.command(strCommand)

            If strResult <> "SUCCESS" Then
                Return "The following message was returned when configuring port " & strPort & " for function type " & UCase(htTerminal("functionType")) & ": """ & strResult & """.  Command: """ & strCommand & """"
            Else
                Return "SUCCESS"
            End If
        Else
            'no op
            Return "SUCCESS"
        End If

    End Function

    Private Function setUserDefinedFunctions(ByRef eusUDFTable As ExpUnitSetObject) As String
        'This method creates the SCPI configuration commands needed to set each UDF in the experiment specification on the 4155.  If this setup
        'procedure completes successfully, the string "SUCCESS" is returned.  Otherwise, a string detailing the failure is returned.
        Dim loopIdx As Integer
        Dim strUDF, strCurrentKey, strUDFKeyArray(), strResult, strUDFName, strUDFUnits, STRUDFBody As String

        ReDim strUDFKeyArray(eusUDFTable.Count() - 1)
        strUDFKeyArray = eusUDFTable.EnumerateUnits()

        For loopIdx = 0 To eusUDFTable.Count() - 1
            strCurrentKey = strUDFKeyArray(loopIdx)

            strUDFName = eusUDFTable.UnitItem(strCurrentKey, "name")
            strUDFUnits = eusUDFTable.UnitItem(strCurrentKey, "units")
            STRUDFBody = eusUDFTable.UnitItem(strCurrentKey, "body")

            If eusUDFTable.UnitItem(strCurrentKey, "dlUDF") = "true" Then
                'adds data vector to download list
                listNames = listNames & "'" & strUDFName & "'" & ", "
                listUnits = listUnits & strUDFUnits & ", "
            End If

            strUDF = "'" & strUDFName & "','" & strUDFUnits & "','" & STRUDFBody & "'"

            Debug.WriteLine("sending UDF configuration command: " & strUDF)

            strResult = HP4155.command(":PAGE:CHAN:UFUN:DEF " & strUDF)

            If strResult <> "SUCCESS" Then
                Return "The following message was returned while attempting to setup User Defined Function (Name: """ & strUDFName & """, Units: """ & strUDFUnits & """, Body: """ & STRUDFBody & """): " & strResult & "."
                Exit Function
            End If

        Next

        'if no error occurs
        Return "SUCCESS"

    End Function

    Private Sub RecordLabConfig(ByVal jrCurrJob As JobRecord)
        'this method uses the current experiment's owner information to build a lab configuration document depicting the state of the kab
        'at experiment run-time.  
        Dim strLogConfig, strDBQuery, strGroup, strXMLOutput As String
        Dim cmdLogConfig, cmdDBQuery As SqlCommand
        Dim dtrDevTermList As SqlDataReader
        Dim intBrokerID, intGroupID, intClassID, intCurrDevID, intCurrMaxDP As Integer

        'determine if brokerID or group affiliation should be used for lab config creation
        strDBQuery = "SELECT dbo.rpm_GetGroupID(@BrokerID, @GroupName);"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@BrokerID", jrCurrJob.brokerID)
        cmdDBQuery.Parameters.Add("@GroupName", jrCurrJob.groupName)

        intGroupID = CInt(cmdDBQuery.ExecuteScalar())

        If intGroupID > 0 Then
            'use group ID to determine usage class
            strDBQuery = "SELECT ClassID FROM dbo.rpm_GroupIsMemberOf(@groupID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@groupID", intGroupID)

            intClassID = CInt(cmdDBQuery.ExecuteScalar())
        Else
            'use broker ID to determine usage class
            strDBQuery = "SELECT ClassID FROM dbo.rpm_BrokerIsMemberOf(@brokerID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.Add("@brokerID", jrCurrJob.brokerID)

            intClassID = CInt(cmdDBQuery.ExecuteScalar())
        End If

        'begin constructing lab configuration
        strXMLOutput = "<?xml version='1.0' encoding='utf-8' standalone='no' ?><!DOCTYPE labConfiguration SYSTEM 'http://weblab2.mit.edu/xml/labConfiguration.dtd'><labConfiguration lab='MIT Microelectronics Weblab' specversion='0.1'>"

        strDBQuery = "SELECT d.deviceID, d.devNumber, d.devName, d.devType, d.devDesc, d.devImageLoc, d.devMaxDataPoints, t.port As termPort, t.name AS termName, t.x_pixel_loc As termXLoc, t.y_pixel_loc As termYLoc, p.max_voltage As termMaxV, p.max_current As termMaxA FROM dbo.rpm_GetActiveDeviceList(@ClassID) d JOIN DeviceProfileTerminalConfig p ON d.deviceID = p.profile_id JOIN DeviceTypeTerminalConfig t ON p.typeterm_id = t.typeterm_id ORDER BY d.devNumber, t.number;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@ClassID", intClassID)
        dtrDevTermList = cmdDBQuery.ExecuteReader()

        intCurrDevID = 0
        intCurrMaxDP = 0

        While dtrDevTermList.Read()
            If intCurrDevID <> dtrDevTermList("deviceID") Then
                'case where terminal record belongs to the next device in the list
                If intCurrDevID <> 0 Then
                    'writes end of device tag after all terminals of a device have been written.  Uses Max data point info from previous loop iteration
                    'so we want this to run on every execution of the parent conditional EXCEPT the first one.
                    strXMLOutput = strXMLOutput & "<maxDataPoints>" & intCurrMaxDP & "</maxDataPoints>"
                    strXMLOutput = strXMLOutput & "</device>"
                End If

                strXMLOutput = strXMLOutput & "<device id='" & dtrDevTermList("devNumber") & "' type='" & dtrDevTermList("devType") & "'>"
                strXMLOutput = strXMLOutput & "<name>" & dtrDevTermList("devName") & "</name>"
                strXMLOutput = strXMLOutput & "<description>" & dtrDevTermList("devDesc") & "</description>"
                strXMLOutput = strXMLOutput & "<imageURL>" & dtrDevTermList("devImageLoc") & "</imageURL>"

            End If

            'write current terminal record for this device
            strXMLOutput = strXMLOutput & "<terminal portType='" & Left(dtrDevTermList("termPort"), 3) & "' portNumber='" & Right(dtrDevTermList("termPort"), 1) & "'>"
            strXMLOutput = strXMLOutput & "<label>" & dtrDevTermList("termName") & "</label>"
            strXMLOutput = strXMLOutput & "<pixelLocation><x>" & dtrDevTermList("termXLoc") & "</x><y>" & dtrDevTermList("termYLoc") & "</y></pixelLocation>"
            strXMLOutput = strXMLOutput & "<maxVoltage>" & dtrDevTermList("termMaxV") & "</maxVoltage>"
            strXMLOutput = strXMLOutput & "<maxCurrent>" & dtrDevTermList("termMaxA") & "</maxCurrent>"
            strXMLOutput = strXMLOutput & "</terminal>"

            'set state variables for next loop iteration
            intCurrDevID = dtrDevTermList("deviceID")
            intCurrMaxDP = dtrDevTermList("devMaxDataPoints")
        End While

        'finalize last device tag
        strXMLOutput = strXMLOutput & "<maxDataPoints>" & intCurrMaxDP & "</maxDataPoints>"
        strXMLOutput = strXMLOutput & "</device>"

        dtrDevTermList.Close()

        strXMLOutput = strXMLOutput & "</labConfiguration>"

        'write lab config to exp record
        strDBQuery = "EXEC qm_LogConfigAtExec @expID, @labConfig;"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@expId", jrCurrJob.experimentID)
        cmdDBQuery.Parameters.Add("@labConfig", strXMLOutput)
        cmdDBQuery.ExecuteNonQuery()

    End Sub

    Private Sub GetCurrentSystemSpecs()
        Dim strDBQuery As String
        Dim cmdDBQuery As SqlCommand
        Dim dtrOutput As SqlDataReader

        'retrieves and sets lab system variables

        strDBQuery = "SELECT HP4155_ID, HPE5250A_ID, HP34970A_ID, HP34970A_chan, VISA_Name, homepage, HPE5250A_present, HP34970A_present, exp_eng_is_active FROM LSSystemConfig WHERE SetupID = '1';"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        dtrOutput = cmdDBQuery.ExecuteReader(CommandBehavior.SingleRow)

        If dtrOutput.Read() Then
            HP4155_GPIBBusAddr = dtrOutput("HP4155_ID")
            HPE5250A_GPIBBusAddr = dtrOutput("HPE5250A_ID")
            HP34970A_GPIBBusAddr = dtrOutput("HP34970A_ID")
            HP34970A_dataChannel = dtrOutput("HP34970A_chan")
            visaName = dtrOutput("VISA_Name")
            strHomepage = dtrOutput("homepage")
            HPE5250A_isPresent = dtrOutput("HPE5250A_present")
            HP34970A_isPresent = dtrOutput("HP34970A_present")
            If dtrOutput("exp_eng_is_active") = "True" Then
                expEngIsActive = True
            Else
                expEngIsActive = False
            End If
        End If

        dtrOutput.Close()

    End Sub

    Private Sub runExperiment(ByRef eoExpSpec As ExperimentObject, ByRef intExpID As Integer)
        Dim eusTermTable, eusUDFTable As ExpUnitSetObject
        Dim SMU1_setup, SMU2_setup, SMU3_setup, SMU4_setup, SMU5_setup, VSU1_setup, VSU2_setup, VMU1_setup, VMU2_setup As String
        Dim strDBQuery, strResult, strErrorMsg, strXMLExpResult, strTermKeyArray() As String
        Dim currName, currUnit, currResultItem As String
        Dim intDeviceNum, loopIdx As Integer
        Dim cmdDBQuery As SqlCommand
        Dim dtrDBQuery As SqlDataReader

        Dim VAR1_record As String = ""

        listNames = ""
        listUnits = ""
        'This subroutine performs tasks related to the configuration and execution of an experiment on the 4155 hardware

        Debug.WriteLine("beginning runExperiment on job " & intExpID)

        eusTermTable = eoExpSpec.Item("TermTable")
        eusUDFTable = eoExpSpec.Item("UDFTable")
        intDeviceNum = eoExpSpec.Item("DeviceNum")

        Debug.WriteLine("spec decomposed into tables, Device Number is  " & intDeviceNum)

        'connect to the HP4155
        strResult = HP4155.connect(visaName, HP4155_GPIBBusAddr)
        If strResult <> "SUCCESS" Then
            strErrorMsg = "System Error:  Error on HP4155 connect (" & strResult & ")"
            jobError(strErrorMsg, intExpID)
            Exit Sub
        End If

        'set state variable 
        bln4155Connected = True

        Debug.WriteLine("connected to hp4155")

        HP4155.setTimeOut(3000)

        Debug.WriteLine("set 4155 timeout to 3 seconds, reseting 4155...")

        'Reset the HP4155 (try this twice before sending an error)
        strResult = HP4155.reset()
        If strResult <> "SUCCESS" Then
            strResult = HP4155.reset()
            If strResult <> "SUCCESS" Then
                strErrorMsg = "System Error: Error reseting HP4155, try running your measurement again (" & strErrorMsg & ")"
                jobError(strErrorMsg, intExpID)
                Exit Sub
            End If
        End If

        Debug.WriteLine("4155 reset")


        'cycle through terminal table to match terminal records to specific ports on the 4155
        ReDim strTermKeyArray(eusTermTable.Count() - 1)
        strTermKeyArray = eusTermTable.EnumerateUnits()

        Debug.WriteLine("enumerating terminal Items")


        For loopIdx = 0 To eusTermTable.Count() - 1

            Debug.WriteLine("items for terminal " & strTermKeyArray(loopIdx))
            Debug.WriteLine("this terminal has " & eusTermTable.EnumerateUnitItems(strTermKeyArray(loopIdx)).Length() & " items.")

            'find terminal configured to be VAR1 
            If UCase(UCase(eusTermTable.UnitItem(strTermKeyArray(loopIdx), "functionType"))) = "VAR1" Then
                VAR1_record = strTermKeyArray(loopIdx)
                Exit For
            End If

        Next

        'setup terminals - VAR1 terminal gets set first.  Others can be set in any order (4155 restriction)
        Debug.WriteLine("setting up terminals")

        'VAR1 terminal will be either SMU or VSU, can assume in flow control - existence of VAR1 in experiment specification is required by the HP4155
        'and assured by the Experiment Validation component (at submit).
        strResult = setPort(eusTermTable.Unit(VAR1_record))
        If strResult <> "SUCCESS" Then
            jobError(strResult, intExpID)
            Exit Sub
        End If

        strResult = setFnct(eusTermTable.Unit(VAR1_record))
        If strResult <> "SUCCESS" Then
            strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
            jobError(strErrorMsg, intExpID)
            Exit Sub
        End If

        For loopIdx = 0 To eusTermTable.Count() - 1
            If strTermKeyArray(loopIdx) <> VAR1_record Then
                Select Case UCase(Left(eusTermTable.UnitItem(strTermKeyArray(loopIdx), "port"), 3))
                    Case "SMU", "VSU"
                        strResult = setPort(eusTermTable.Unit(strTermKeyArray(loopIdx)))
                        If strResult <> "SUCCESS" Then
                            jobError(strResult, intExpID)
                            Exit Sub
                        End If

                        strResult = setFnct(eusTermTable.Unit(strTermKeyArray(loopIdx)))
                        If strResult <> "SUCCESS" Then
                            strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
                            jobError(strErrorMsg, intExpID)
                            Exit Sub
                        End If
                    Case "VMU"
                        strResult = setPort(eusTermTable.Unit(strTermKeyArray(loopIdx)))
                        If strResult <> "SUCCESS" Then
                            jobError(strResult, intExpID)
                            Exit Sub
                        End If

                        'no function definition for VMU
                    Case Else
                        strErrorMsg = "Experiment Error: Unrecognized port """ & eusTermTable.UnitItem(strTermKeyArray(loopIdx), "port") & """, aborting."
                        jobError(strErrorMsg, intExpID)
                        Exit Sub
                End Select
            End If

        Next


        'If SMU1_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(SMU1_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(SMU1_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If SMU2_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(SMU2_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(SMU2_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If SMU3_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(SMU3_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(SMU3_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If SMU4_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(SMU4_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(SMU4_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If SMU5_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(SMU5_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(SMU5_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If VSU1_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(VSU1_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(VSU1_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If VSU2_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(VSU2_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    strResult = setFnct(eusTermTable.Unit(VSU2_record))
        '    If strResult <> "SUCCESS" Then
        '        strErrorMsg = "System Error: An error occurred while setting a terminal function.  " & strResult
        '        jobError(strErrorMsg, intExpID)
        '        Exit Sub
        '    End If
        'End If
        'If VMU1_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(VMU1_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    'no function definition for VMU
        'End If
        'If VMU2_record <> "" Then 'only set port if defined in the experiment specification
        '    strResult = setPort(eusTermTable.Unit(VMU2_record))
        '    If strResult <> "SUCCESS" Then
        '        jobError(strResult, intExpID)
        '        Exit Sub
        '    End If

        '    'no function definition for VMU
        'End If

        'Set up user defined functions (originally set between terminal definition and terminal functio definition, does this matter?)
        Debug.WriteLine("setting up user defined functions")

        strResult = setUserDefinedFunctions(eusUDFTable)
        If strResult <> "SUCCESS" Then
            strErrorMsg = "Experiment Error: Error setting up User Defined Function.  " & strResult
            jobError(strErrorMsg, intExpID)
            Exit Sub
        End If

        'parse name and unit lists for data vectors to be downloaded 
        Debug.WriteLine("setting download variables")
        listNames = RTrim(listNames)
        listNames = Left(listNames, (Len(listNames) - 1)) 'removes extra comma from end of string

        listUnits = RTrim(listUnits)
        listUnits = Left(listUnits, (Len(listUnits) - 1)) 'removes extra comma from end of string

        Dim listNameArray() As String = Split(listNames, ", ")
        Dim listUnitArray() As String = Split(listUnits, ", ")
        Debug.WriteLine(">" & listNames & "<")

        strResult = HP4155.command(":PAGE:DISP:LIST " & listNames)
        If strResult <> "SUCCESS" Then
            strErrorMsg = "System Error: Error setting download variables, check variable settings and try again.  " & strResult
            jobError(strErrorMsg, intExpID)
            Exit Sub
        End If

        'send additional measurement paramaeters for waiting and integration time (governs all experiments) if desired
        If TimeParams Then
            strResult = HP4155.command(":PAGE:MEAS:MSET:WTIM 0")
            Debug.WriteLine("Setting HP4155 wait time: " & strResult)

            strResult = HP4155.command(":PAGE:MEAS:MSET:ITIM:SHORT 8E-5")
            Debug.WriteLine("Setting HP4155 integration time: " & strResult)
        End If

        'execute the job
        strResult = HP4155.command(":PAGE:SCON:SING")
        If strResult <> "SUCCESS" Then
            strErrorMsg = "System Error: Error executing experiment.  Change your setup and try again.  " & strResult
            jobError(strErrorMsg, intExpID)
            Exit Sub
        End If

        'flush the read buffer
        HP4155.flush()

        'set hardware timeout to 5 minutes while waiting for measurement to complete
        HP4155.setTimeOut(300000)

        'wait for job to finish
        strResult = HP4155.waitUntilDone()
        If strResult <> "SUCCESS" Then
            If InStr(strResult, "WARNING") <> 0 Then
                'each warning message must be appended to the contents of strWarningMsg and be terminated with ";;"
                strWarningMsg = strWarningMsg & strResult & ": One or more ports may have reached their compliance value.;;"
            Else
                strErrorMsg = "Timed out while waiting, use fewer data points and try again: " & strResult
                jobError(strErrorMsg, intExpID)
                Exit Sub
            End If
        End If

        'set timeout back to 1 minute
        HP4155.setTimeOut(6000)

        'flush the read buffer
        HP4155.flush()

        'begin constructing Experiment Result XML document
        strXMLExpResult = "<?xml version='1.0' encoding='utf-8' standalone='no' ?><!DOCTYPE experimentResult SYSTEM 'http://weblab2.mit.edu/xml/experimentResult.dtd'><experimentResult lab='MIT Microelectronics Weblab' specversion='0.1'>"

        If HP34970A_isPresent Then
            strXMLExpResult = strXMLExpResult & "<temp units='" & getUnits() & "'>" & getTemp() & "</temp>"
        End If

        For loopIdx = 0 To UBound(listNameArray)
            currName = Trim(listNameArray(loopIdx))
            currUnit = Trim(listUnitArray(loopIdx))
            currResultItem = HP4155.query(":DATA? " & currName)

            Debug.WriteLine(currName)
            Debug.WriteLine(currResultItem)

            strXMLExpResult = strXMLExpResult & "<datavector name='" & Replace(currName, "'", "") & "' units='" & currUnit & "'>" & Replace(Left(currResultItem, (InStrRev(currResultItem, "E") + 4)), ",", " ") & "</datavector>"
        Next

        strXMLExpResult = strXMLExpResult & "</experimentResult>"

        'finish the job
        strDBQuery = "EXEC qm_FinishJob @expID, @expResult, @errorMsg, @errorBit"
        cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
        cmdDBQuery.Parameters.Add("@expID", intExpID)
        cmdDBQuery.Parameters.Add("@expResult", strXMLExpResult)
        cmdDBQuery.Parameters.Add("@errorMsg", strWarningMsg)
        cmdDBQuery.Parameters.Add("@errorBit", "0")
        cmdDBQuery.ExecuteNonQuery()

        'set job state variable
        blnJobLoaded = False

        Debug.WriteLine("job finished")
        HP4155.close_session()

        'set state variable
        bln4155Connected = False

    End Sub


    Private Sub Notify()
        'a method signature for use as a delegate for ThreadStart (so that Notfy can be called in a separate
        'thread and not hold up job execution).
        'Uses intExpID instantiated in top level class declarations and written by ExecuteQueuedJob()

        Notify(intExpID)
    End Sub


    Private Sub Notify(ByRef intExpID As Integer)
        'this method initiates a job complete notification by instructing the Lab Server web server process to make the Notify
        'web service call on the appropriate Service Broker.  
        Dim objRequest As HttpWebRequest
        Dim objResponse As HttpWebResponse
        Dim srResponse As StreamReader
        Dim strResponse As String

        strHomepage = Trim(strHomepage)

        If InStrRev(strHomepage, "/") <> Len(strHomepage) Then
            'homepage URL does not have a backslash at the end, add one
            strHomepage = strHomepage & "/"
        End If

        Try
            objRequest = CType(WebRequest.Create(strHomepage & "notify.aspx?expID=" & intExpID), HttpWebRequest)
            objRequest.Method = "GET"

            objResponse = objRequest.GetResponse()
            srResponse = New StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.ASCII)
            strResponse = srResponse.ReadToEnd()
            srResponse.Close()

        Catch e As Exception
            Debug.WriteLine(e.GetBaseException.Message())
            Exit Sub
        End Try

    End Sub


    Private Sub jobError(ByVal strErrMsg As String, ByRef intExpID As Integer)
        'Generic error notification subroutine (closes current experiment)

        Dim strFinishJob As String
        Dim cmdFinishJob As SqlCommand

        strFinishJob = "EXEC qm_FinishJob @expID, @expResults, @errorMsg, @errorBit"
        cmdFinishJob = New SqlCommand(strFinishJob, conWebLabLS)
        cmdFinishJob.Parameters.Add("@expID", CStr(intExpID))
        cmdFinishJob.Parameters.Add("@expResults", "")
        cmdFinishJob.Parameters.Add("@errorMsg", strErrMsg)
        cmdFinishJob.Parameters.Add("@errorBit", "1")
        cmdFinishJob.ExecuteNonQuery()

        'increment error counter & write error report to event log
        intErrorCount = intErrorCount + 1
        EventLogInterface.WriteEntry("Error executing job #" & intExpID & ".  " & strErrMsg)

        Debug.WriteLine("job Error: " & strErrMsg)

        If bln4155Connected Then
            HP4155.close_session()
        End If

    End Sub

    Private Sub disconnectTherm()
        'closes GPIB connection to HP34970A
        HP34970A.ClosePort()
    End Sub



    '---Private Custom Data Types
    Private Structure JobRecord
        Public experimentID As Integer
        Public brokerID As Integer
        Public groupName As String
        Public experimentSpec As String
        Public labConfiguration As String
        Public experimentResult As String
        'else?
    End Structure

End Class



