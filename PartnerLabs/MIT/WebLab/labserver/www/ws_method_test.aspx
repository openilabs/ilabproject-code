<%@ Page validateRequest="false" Strict="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="LabServerAPI" %>
<%@ Import Namespace="SiteComponents" %>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->


<script Runat="server">



Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
Dim strDBQuery, strPageState, strResponse, strExpSpec, strGroup, strBrokerId, strBrokerPasskey As String
Dim intExpID, intPriorityHint As Integer
dim intBroker, intExp, loopIdx As Integer
Dim cmdDBQuery As SQLCommand
Dim dtrDBQuery As SQLDataReader
Dim LSAPI As New LabServerAPI()
Dim AuthHeader As New AuthHeader()
'Dim LabConfig As String
'Dim LabStatus As Object = New Object()
'Dim ResultReport As Object = New Object()
Dim SubmitReport As Object = New Object() 'New SubmissionReport()
Dim ValidReport As Object = New Object() 'New ValidationReport()
'Dim WaitReport As Object = New Object() 'New WaitEstimate()

Sub Page_Load
	conWebLabLS.Open()
	'get initial info
	
	'System.Net.ServicePointManager.CertificatePolicy = New WebLabCertPolicy()
	
	LSAPI.URL = "http://ekahi.mit.edu/services/WebLabService.asmx"
	
	intBroker = 1
	
	strDBQuery = "SELECT broker_server_id, broker_passkey FROM Brokers WHERE broker_id = " & intBroker & ";"
	cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	dtrDBQuery = cmdDBquery.ExecuteReader()
	
	If dtrDBQuery.Read() Then
		strBrokerId = dtrDBQuery("broker_server_id")
		strBrokerPasskey = dtrDBQuery("broker_passkey")
	End If
	dtrDBQuery.Close()	
	
	'For Submit call test
	strDBQuery = "SELECT MAX(exp_id) FROM JobRecord WHERE provider_id = @LSID;"
    cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
    cmdDBQuery.Parameters.Add("@LSID", intBroker)
   
    Dim objExpID As Object = cmdDBQuery.ExecuteScalar()
   
    If objExpID Is DBNull.Value Then
        intExpID = 1
    Else
        intExpID = CInt(objExpID) + 1
    End If
    	
    conWebLabLS.Close()
	
	
	AuthHeader.identifier = strBrokerId
	AuthHeader.passkey = strBrokerPasskey 
		
	LSAPI.AuthHeaderValue = AuthHeader
	
        'Submit TEST
        Try


            strExpSpec = "<?xml version=""1.0"" encoding=""utf-8"" standalone=""no"" ?><!DOCTYPE experimentSpecification SYSTEM ""http://weblab2.mit.edu/xml/experimentSpecification.dtd""><experimentSpecification lab=""MIT Microelectronics Weblab"" specversion=""0.1""><deviceID>1</deviceID><terminal portType=""SMU"" portNumber=""1""><vname download=""true"">V1</vname><iname download=""true"">I1</iname><mode>V</mode><function type=""VAR1""><scale>LIN</scale><start>0.000</start><stop>2.000</stop><step>0.01000</step></function><compliance>0.1</compliance></terminal><terminal portType=""SMU"" portNumber=""2""><vname download=""false"">V2</vname><iname download=""false"">I2</iname><mode>V</mode><function type=""CONS""><value>0</value></function><compliance>0.1</compliance></terminal></experimentSpecification>"

            SubmitReport = LSAPI.Submit(intExpID, strExpSpec, "", 0)

            strResponse = "Submit completed : expID = " & intExpID & "<br>"
            strResponse = strResponse & "Returned Exp_id " & SubmitReport.experimentID.ToString() & "<br>"
        Catch e As Exception
            strResponse = e.Message() & "<br>" & e.GetBaseException.ToString()
        End Try
    
        ''Validate TEST

        'Try
        '    strExpSpec = "<?xml version=""1.0"" encoding=""utf-8"" standalone=""no"" ?><!DOCTYPE experimentSpecification SYSTEM ""http://weblab2.mit.edu/xml/experimentSpecification.dtd""><experimentSpecification lab=""MIT Microelectronics Weblab"" specversion=""0.1""><deviceID>1</deviceID><terminal portType=""SMU"" portNumber=""1""><vname download=""true"">V1</vname><iname download=""true"">I1</iname><mode>V</mode><function type=""VAR1""><scale>LIN</scale><start>0.000</start><stop>2.000</stop><step>0.01000</step></function><compliance>0.1</compliance></terminal><terminal portType=""SMU"" portNumber=""2""><vname download=""false"">V2</vname><iname download=""false"">I2</iname><mode>V</mode><function type=""CONS""><value>0</value></function><compliance>0.1</compliance></terminal></experimentSpecification>"

        '    ValidReport = LSAPI.Validate(strExpSpec, "")

        '    strResponse = "Validate completed : expID = " & intExpID & "<br>"
        '    strResponse = strResponse & "Accepted? " & ValidReport.accepted.ToString() & "<br>"
        'Catch e As Exception
        '   strResponse = e.Message() & "<br>" & e.GetBaseException.ToString()
        'End Try
	
	
	''GetLabConfig TEST
	'Try
 '       LabConfig = LSAPI.GetLabConfiguration("")

 '       strResponse = LabConfig
 '   Catch e As Exception
 '       strResponse = e.Message() & "<br>" & e.GetBaseException.ToString()
 '   End Try
	
	
	'GetLabStatus TEST
	'Try
	'	LabStatus = LSAPI.GetLabStatus()

	'	strResponse = "<b>Lab is Online? </b>" & LabStatus.online & "<br>"
	'	strResponse = strResponse & "<b>Lab Status Message: </b>" & LabStatus.labStatusMessage & "<br>"
	'Catch e As Exception
	'	strResponse = e.Message() & "<br>" & e.GetBaseException.ToString()
	'End Try
	
	
	'RESULT REPORT TEST
	'intExp = "50700"	
		
	'Try
	'	ResultReport = LSAPI.RetrieveResult(intExp)
		
	'	strResponse = "<b>Status Code:</b> " & ResultReport.statusCode & "<br>"
	'	strResponse = strResponse & "<b>Error Message:</b> " & ResultReport.errorMessage & "<br>"
	'	strResponse = strResponse & "<b>Warning Messages:</b> " & ResultReport.warningMessages.Length() & "<br>"
	'	For loopIdx = 0 To ResultReport.warningMessages.Length() - 1
	'		strResponse = strResponse & "<b>Warning Message " & loopIdx & ":</b> -" & ResultReport.warningMessages(loopIdx) & "-<br>"
	'	Next
		
	'	'strResponse = strResponse & "<b>Warning Messages:</b> " & ResultReport.warningMessages.ToString() & "<br>"
	'	strResponse = strResponse & "<b>Experiment Results:</b> " & ResultReport.experimentResults & "<br>"
	'	strResponse = strResponse & "<b>XML Result Extension:</b> " & ResultReport.xmlResultExtension & "<br>"
		
	'Catch e As Exception
	'	strResponse = e.Message() & "<br>" & e.GetBaseException.ToString()
	'End Try
		

	
End Sub

Sub Page_PreRender
	'closes database connection
	'conWebLabLS.Close()

End Sub








 
</script>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>WebLab Lab Server Web Service Method Test Page</title>
  </head>
  
  <body>
    <form Runat="server">
    <%
		'Response.Write("Result for RetrieveResult for Job ID = " & intExp & " (Broker ID = " & intBroker & "): <p>" & strResponse & "<p>")			
		'Response.Write("Result for GetLabStatus for Broker ID = " & intBroker & ": <p>" & strResponse & "<p>")			
		'Response.Write("Result for GetLabConfig for Broker ID = " & intBroker & ": <p>" & strResponse & "<p>")			
        Response.Write("Result for Submit for Broker ID = " & intBroker & ": <p>" & strResponse & "<p>")
        'Response.Write("Result for Validate for Broker ID = " & intBroker & ": <p>" & strResponse & "<p>")			
	%>

    

    </form>
  </body>
</html>
