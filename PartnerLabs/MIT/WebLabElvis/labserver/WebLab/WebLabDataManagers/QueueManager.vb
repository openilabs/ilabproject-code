Imports System
Imports System.Configuration
Imports System.Data.SqlClient
Imports WebLabCustomDataTypes


'Author(s): James Hardison (hardison@alum.mit.edu)
'Date: 6/23/2003
'Date Modified: 9/20/2004
'This class provides a VB interface to the WebLab Queue Manager methods defined in the Lab server database.  This interface is intended to 
'serve the ASP.NET code that will for the backend of both the Lab Server Administration Site and the Web Services interface that will be exposed
'for use by registered Service Brokers.  To use these methods in your ASP.NET page, first, make sure a compiled copy of this file is present in 
'the /bin directory of your web application.  In your ASP.NET page, import the WebLabDataManagers namespace 
'(<%import namespace="WebLabDataManagers"%>) and, in your code, instantiate a Queue Manager object.  From this object, all of the 
'public methods listed below will be accessible.
'
'Dependency List
'Used By:
'   Lab Server Web Services Interface (/services/WebLabServices/LabServerAPI.vb, /bin/WebLabServices.dll)
'   Lab Server Queue Manager Library (/bin/QueueManager.dll (this code))
'
'Uses: 
'   WebLab Custom Data Types (/controls/WebLabCustomDataTypes/WebLabCustomDataTypes.vb, /bin/WebLabCustomDataTypes.dll)

Namespace WebLabDataManagers

    Public Class QueueManager

        Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))

        Public Function ExperimentStatus(ByVal intBrokerID As Integer, ByVal intBrokerExpID As Integer) As ExpStatusObject


            'This method retrieves the status information of an experiment specified by its BrokerID and Broker Generated Experiment ID.  The 
            'output of this method is the current queue position of the referenced job, the estimated time until the job runs, and the estimated 
            'execution time for the job.  These values are organized into an Experiment Status Object.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim esObject As ExpStatusObject
            Dim intLocalExpID As Integer

            'retrieve experiment info
            strDBQuery = "SELECT queuePosition, estTimetoRun, estExecTime FROM dbo.qm_ExperimentStatusByRemoteID(@BrokerID, @ExpID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@BrokerID", intBrokerID)
            cmdDBQuery.Parameters.AddWithValue("@ExpID", intBrokerExpID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            esObject = New ExpStatusObject(CInt(dtrDBQuery("queuePosition")), CInt(dtrDBQuery("estTimeToRun")), CInt(dtrDBQuery("estExecTime")))

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return esObject
        End Function


        Public Function ExperimentStatus(ByVal intLocalExpID As Integer) As ExpStatusObject
            'This method retrieves the status information of an experiment specified by its Locally Generated Experiment ID.  The output of 
            'this method is the current queue position of the referenced job, the estimated time until the job runs, and the estimated 
            'execution time for the job.  These values are organized into an Experiment Status Object.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim esObject As ExpStatusObject

            strDBQuery = "SELECT queuePosition, estTimetoRun, estExecTime FROM dbo.qm_ExperimentStatusByLocalID(@ExpID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@ExpID", intLocalExpID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            esObject = New ExpStatusObject(CInt(dtrDBQuery("queuePosition")), CInt(dtrDBQuery("estTimeToRun")), CInt(dtrDBQuery("estExecTime")))

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return esObject
        End Function


        Public Function QueueLength(ByVal intPriority As Integer) As QueueLengthObject
            'This method determines the current length of the experiment queue for a new job of the specified priority.  The output of this 
            'function are the length of the queue for a caller with the specified priority if a job were being submitted and the estimated 
            'time (in seconds) until that hypothetical job ran.  These values are organized into a Queue Length Object.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader
            Dim qlObject As QueueLengthObject

            strDBQuery = "SELECT queueLength, estTimeToRun FROM dbo.qm_QueueLength(@Priority);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@Priority", intPriority)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            qlObject = New QueueLengthObject(CInt(dtrDBQuery("queueLength")), CInt(dtrDBQuery("estTimeToRun")))

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return qlObject
        End Function

        Public Function CancelJob(ByVal intLocalExpID As Integer) As Boolean
            'This method attempts to cancel a job specified by its Locally Generated Experiment ID.  The output of this function is 
            'a single boolean value indicating whether or not the specified job was successfully cancelled.  A job can be cancelled 
            'at any point before its execution has begun.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC qm_CancelByLocalID @ExpID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@ExpID", intLocalExpID)

            If cmdDBQuery.ExecuteScalar() = "TRUE" Then
                conWebLabLS.Close()
                Return True
            Else
                conWebLabLS.Close()
                Return False
            End If
        End Function


        Public Function CancelJob(ByVal intBrokerID As Integer, ByVal intBrokerExpID As Integer) As Boolean
            'This method attempts to cancel a job specified by its associated Broker ID and Broker Generated Experiment ID.  The output 
            'of this function is a single boolean value indicated whether or not the specified job was successfully cancelled.  A job
            'can be cancelled at any point before its execution has begun.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand

            strDBQuery = "EXEC qm_CancelByRemoteID @BrokerID, @ExpID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@BrokerID", intBrokerID)
            cmdDBQuery.Parameters.AddWithValue("@ExpID", intBrokerExpID)

            If cmdDBQuery.ExecuteScalar() = "TRUE" Then
                conWebLabLS.Close()
                Return True
            Else
                conWebLabLS.Close()
                Return False
            End If
        End Function


        Public Function EnqueueJob(ByVal strXMLExpSpec As String, ByVal intPriority As Integer, ByVal strGroups As String, ByVal intBrokerID As Integer, ByVal intBrokerExpID As Integer, ByVal intEstExecTime As Integer, ByVal intDataPoints As Integer, ByVal intSetupID As Integer) As Integer
            'This method places the supplied experiment specification into the execution queue.  At this point, it is assumed that the experiment 
            'specification has been validated.  The output of this function is an integer value detailing the job's current position in the 
            'execution queue.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim dtrDBQuery As SqlDataReader

            strDBQuery = "EXEC qm_Enqueue @ExpSpec, @Priority, @Groups, @BrokerID, @RemoteExpID, @EstExecTime, @Points, @SetupID;"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@ExpSpec", strXMLExpSpec)
            cmdDBQuery.Parameters.AddWithValue("@Priority", intPriority)
            cmdDBQuery.Parameters.AddWithValue("@Groups", strGroups)
            cmdDBQuery.Parameters.AddWithValue("@BrokerID", intBrokerID)
            cmdDBQuery.Parameters.AddWithValue("@RemoteExpID", intBrokerExpID)
            cmdDBQuery.Parameters.AddWithValue("@EstExecTime", intEstExecTime)
            cmdDBQuery.Parameters.AddWithValue("@Points", intDataPoints)
            cmdDBQuery.Parameters.AddWithValue("@SetupID", intSetupID)
            dtrDBQuery = cmdDBQuery.ExecuteReader()

            dtrDBQuery.Read()

            Dim Output As Integer = dtrDBQuery("queuePosition")

            dtrDBQuery.Close()
            conWebLabLS.Close()

            Return Output
        End Function


        Public Function GetExperimentStatusCode(ByVal intBrokerID As Integer, ByVal intBrokerExpID As Integer) As Integer
            'This method checks the status code of a job specified by its associated Broker ID and Broker Generated Experiment ID.  
            'The output of this function is integer code describing the current status of the specified experiment.  The potential 
            'return values are described below.
            '   1 - Job/broker combo is valid and still in the queue.
            '	2 - Job/broker combo is valid and currently executing.
            '	3 - Job/broker combo is valid and terminated normally
            '	4 - Job/broker combo is valid and terminated with errors.
            '   5 - Job/broker combo is valid and was cancelled by broker.
            '	6 - Job/broker combo is invalid.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim intOutput As Integer

            strDBQuery = "SELECT dbo.qm_GetExpStatusCodeByRemoteID(@brokerID, @expID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@brokerID", intBrokerID)
            cmdDBQuery.Parameters.AddWithValue("@expID", intBrokerExpID)
            intOutput = CInt(cmdDBQuery.ExecuteScalar())

            conWebLabLS.Close()
            Return intOutput
        End Function


        Public Function GetExperimentStatusCode(ByVal intLocalExpID As Integer) As Integer
            'This method checks the status code of a job specified by its Locally Generated Experiment ID.  The output of this 
            'function is integer code describing the current status of the specified experiment.  The potential return values 
            'are described below.
            '   1 - Job/broker combo is valid and still in the queue.
            '	2 - Job/broker combo is valid and currently executing.
            '	3 - Job/broker combo is valid and terminated normally
            '	4 - Job/broker combo is valid and terminated with errors.
            '   5 - Job/broker combo is valid and was cancelled by broker.
            '	6 - Job/broker combo is invalid.
            conWebLabLS.Open()
            Dim strDBQuery As String
            Dim cmdDBQuery As SqlCommand
            Dim intOutput As Integer

            strDBQuery = "SELECT dbo.qm_GetExpStatusCodeByLocalID(@expID);"
            cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
            cmdDBQuery.Parameters.AddWithValue("@expID", intLocalExpID)
            intOutput = CInt(cmdDBQuery.ExecuteScalar())

            conWebLabLS.Close()
            Return intOutput
        End Function

    End Class

End Namespace

