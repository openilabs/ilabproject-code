<%@ Page Language="VBScript" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="WebLabDataManagers" %>
<%@ Import Namespace="WebLabCustomDataTypes" %>
<%@ Import Namespace="ILabStandardTypes" %>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
<script runat="server">
	Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
	Dim rmObject As RecordManager = New RecordManager()
	'Dim rrOutput As Object = New ResultReport()
	Dim strEstOut, strResponse as String
	Dim resultObject As ResultObject
	
	Dim strResultExtension As String
	
	
	
	Sub Page_Load
	
	strResponse = ""
	
	'TEST RPM-AuthenticateBroker
	Dim intAuthBroker As Integer
	strResponse = strResponse & "<p>running RPM-AuthenticateBroker <br>"
	
	Try 
	    intAuthBroker = rpmObject.AuthenticateBRoker("2EFDCF244544C38DA8BBED61E808C","538392404164608")
	    strResponse = strResponse & "method return: " & intAuthBroker & "<br>"
	Catch e As Exception
	    strResponse = strResponse & e.Message() & "<br>" & e.GetBaseException.ToString()
	End Try
	
	
	'TEST RPM GetBrokerResourcePermission
	Dim blnHasPermission As Boolean
	strResponse = strResponse & "<p>running RPM-GetBrokerResourcePermission <br>"
	
	Try
	    blnHasPermission = rpmObject.GetBrokerResourcePermission(intAuthBroker, "WSInterface", "canview")
	    strResponse = strResponse & "method return: " & blnHasPermission & "<br>"
	Catch e As Exception
	    strResponse = strResponse & e.Message() & "<br>" & e.GetBaseException.ToString()
	End Try
	
	
	'TEST RPM GetWSInterfaceStatus
	Dim blnWSStatus As Boolean
	strResponse = strResponse & "<p>running RPM-GetWSInterfaceStatus <br>"
	
	Try
	    blnWSStatus = rpmObject.GetWSInterfaceStatus()
	    strResponse = strResponse & "method return: " & blnWSStatus & "<br>"
	Catch e As Exception
	    strResponse = strResponse & e.Message() & "<br>" & e.GetBaseException.ToString()
	End Try
	    
	'TEST RPM GetGroupID
	Dim intGroupID As Integer
	strResponse = strResponse & "<p>running RPM-GetGroupID <br>"
	
	Try
	    intGroupID = rpmObject.GetGroupID(intAuthBroker, "")
	    strResponse = strResponse & "method return: " & intGroupID & "<br>"
	Catch e As Exception
	    strResponse = strResponse & e.Message() & "<br>" & e.GetBaseException.ToString()
	End Try
	
	'TEST RM ExperimentRuntimeEst
	Dim intRuntimeEst As Integer
	strResponse = strResponse & "<p>running RM-ExperimentRuntimeEst <br>"
	
	Try
	    intRuntimeEst = rmObject.ExperimentRuntimeEst("201", "1")
	    strResponse = strResponse & "method return: " & intRuntimeEst & "<br>"
	Catch e As Exception
	    strResponse = strResponse & e.Message() & "<br>" & e.GetBaseException.ToString()
	End Try
	<%--
	'resultObject = rmObject.RetrieveResult(4, 1047)
	'rmObject.LogIncomingWebRequest(2, "Test Method", False, False, "this is a test")
	'strEstOut = ""
	
	'Dim loopIdx as Integer
	'For loopIdx = 0 To UBound(strOutput, 1) - 1
'		strEstOut = strEstOut & "<>" & strOutput(loopIdx)
'	Next
	
	'strEstOut = strOutput(0) & "<>" & strOutput(1) & "<>" & strOutput(2)
	
	'This method returns the results of the specified experiment.  Data will only be returned if the specified job was submitted by the same
        'service broker as the caller. 
    Dim experimentID As Integer
    
    Dim rmObject As RecordManager = New RecordManager()
    Dim qmObject As QueueManager = New QueueManager()
   
    Dim eriObject As ExpRecordInfoObject
    Dim intBrokerID, intExpStatCode As Integer
    
    Dim blnHasPermission As Boolean = False

	intBrokerID = 1
	experimentID = 37993
	
           
    intExpStatCode = qmObject.GetExperimentStatusCode(intBrokerID, experimentID)

    If intExpStatCode <> 3 And intExpStatCode <> 4 And intExpStatCode <> 5 Then  'case where expID/brokerID combo is invalid or job is still in the queue.
        'rrOutput.statusCode = intExpStatCode

 
        Exit Sub
    End If


    resultObject = rmObject.RetrieveResult(intBrokerID, experimentID)
    eriObject = rmObject.GetExperimentRecordInfo(intBrokerID, experimentID)
    strResultExtension = eriObject.SubmitTime()

    'rrOutput.statusCode = resultObject.ExperimentStatus()
    'rrOutput.experimentResults = resultObject.ExperimentResults()
    ''rrOutput.labConfiguration = resultObject.LabConfig()  - lab configuration removed from ResultReport object (2/16/05)

    Dim strWarning() As String = Split(resultObject.WarningMessages(), ";;")
    'rrOutput.warningMessages = strWarning

    'rrOutput.errorMessage = resultObject.ErrorMessages()

    'If eriObject.SubmitTime() <> "" Then 'checks submit time field for data
     '   strResultExtension = "Group=" & eriObject.UserGroup()
      '  strResultExtension = strResultExtension & ",SubmitTime=" & eriObject.SubmitTime()
       ' strResultExtension = strResultExtension & ",ExecutionTime=" & eriObject.ExecTime()
        'strResultExtension = strResultExtension & ",EndTime=" & eriObject.EndTime()
      '  strResultExtension = strResultExtension & ",ElapsedExecutionTime=" & eriObject.ExecElapsed()
      '  strResultExtension = strResultExtension & ",ElapsedJobTime=" & eriObject.JobElapsed()
      '  strResultExtension = strResultExtension & ",DeviceName=" & eriObject.DeviceName()

       ' rrOutput.xmlResultExtension = strResultExtension
    'End If
    --%>


	
	End Sub
	
  
  </script>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>Method Test</title>

  </head>
  
  <body>
    <form runat="server">
		<%
		Response.Write(strResponse)
		'Response.Write("Experiment Status = " & resultObject.ExperimentStatus() & "<br>")
		'Response.Write("Result Extension = " & strResultExtension & "<br>")
		'Response.Write("Warning Messages = " & resultObject.WarningMessages() & "<br>")
		'Response.Write("Experiment Results = " & resultObject.ExperimentResults() & "<br>")
		'Response.Write("Lab Config = " & resultObject.LabConfig() & "<br>")
		%>

    </form>
  </body>
</html>
