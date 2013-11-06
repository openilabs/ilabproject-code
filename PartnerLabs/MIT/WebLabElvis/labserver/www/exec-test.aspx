<%@ Page validateRequest="false" Strict="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="ILab.ILabStandardTypes" %>
<%@ Import Namespace="WebLab.LabServer" %>
<%@ Import Namespace="WebLab" %>

<script Runat="server">

Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
Dim strDBQuery, strPageState, strResponse, strExpSpec, strGroup, strBrokerId, strBrokerPasskey As String
Dim intExpID, intPriorityHint As Integer
Dim cmdDBQuery As SQLCommand
Dim dtrDBQuery As SQLDataReader
Dim LSAPI As New LabServerAPI()
Dim AuthHeader As New AuthHeader()
Dim SubmitReport As Object = New Object() 'New SubmissionReport()
Dim ValidReport As Object = New Object() 'New ValidationReport()
Dim WaitReport As Object = New Object() 'New WaitEstimate()

Sub Page_Load
	conWebLabLS.Open()
	'get initial info
	
	System.Net.ServicePointManager.CertificatePolicy = New WebLabCertPolicy()
	
        LSAPI.Site = "https://eeilabs.mit.edu/elvis5/services/WebLabService.asmx"
	
	strDBQuery = "SELECT TOP 1 broker_assigned_id FROM JobRecord WHERE provider_id = 2 ORDER BY submit_time DESC;"
	cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	Dim objResult As Object = cmdDBquery.ExecuteScalar()
	
	If objResult Is DBNull.Value Then
		intExpID = 1
	ElseIf IsNumeric(CStr(objResult)) Then
		intExpID = Cint(objResult) + 1
	Else  'if the result is neither null nor numeric (empty string)
		intExpID = 1
	End If
	
	strDBQuery = "SELECT broker_server_id, broker_passkey FROM Brokers WHERE broker_id = 2;"
	cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	dtrDBQuery = cmdDBquery.ExecuteReader()
	
	If dtrDBQuery.Read() Then
		strBrokerId = dtrDBQuery("broker_server_id")
		strBrokerPasskey = dtrDBQuery("broker_passkey")
	End If
	dtrDBQuery.Close()	

	If not Page.IsPostBack then
		strPageState = "NEWLOAD"
	End If
End Sub

Sub Page_PreRender
	'closes database connection
	conWebLabLS.Close()
	
	If strPageState = "NEWLOAD" Then
		lblNewExpId.Text = intExpId
	End If 
End Sub


Sub btnSubmit_Click(s As Object, e As EventArgs)
	strExpSpec = Trim(txtExpSpec.Text)
	strGroup = Trim(txtGroup.Text)
	If IsNumeric(txtPriorityHint.Text) Then
		intPriorityHint = Cint(txtPriorityHint.Text)
	Else
		intPriorityHint = 0
	End If
		
	If strExpSpec = "" Then
		strResponse = "You must enter an experiment specification."
	Else
		AuthHeader.identifier = strBrokerId
		AuthHeader.passkey = strBrokerPasskey 
		
            LSAPI.BrokerCred = AuthHeader
		
		SubmitReport = LSAPI.Submit(intExpID, strExpSpec, strGroup, intPriorityHint)
		ValidReport = SubmitReport.vReport
		WaitReport = SubmitReport.wait
		
		strResponse = "<b>MinTimeToLive:</b> " & SubmitReport.minTimetoLive & "<br>"
		strResponse = strResponse & "<b>Job Accepted:</b> " & ValidReport.accepted & "<br>"
		strResponse = strResponse & "<b>Estimated Run Time:</b> " & ValidReport.estRuntime & "<br>"
		strResponse = strResponse & "<b>Validation Error Message:</b> " & ValidReport.validationErrorMessage & "<br>"
		strResponse = strResponse & "<b>Validation Warning Message:</b> " & ValidReport.validationWarningMessages & "<br>"
		If ValidReport.accepted = "True" Then
			strResponse = strResponse & "<b>Effective Queue Length:</b> " & WaitReport.effectiveQueueLength & "<br>"
			strResponse = strResponse & "<b>Estimated Wait:</b> " & WaitReport.estWait & "<br>"
		Else
			strResponse = strResponse & "Job Number " & intExpId & " not executed.<br>"
		End If
		
		lblNewExpId2.Text = CStr(intExpId + 1)
		txtGroup2.Text = strGroup
		txtPriorityHint2.Text = CStr(intPriorityHint)
		txtExpSpec2.Text = strExpSpec
	End If

	strPageState = "SUBMITTED"

End Sub

Sub btnReSubmit_Click(s As Object, e As EventArgs)
	strExpSpec = Trim(txtExpSpec2.Text)
	strGroup = Trim(txtGroup2.Text)
	If IsNumeric(txtPriorityHint2.Text) Then
		intPriorityHint = Cint(txtPriorityHint2.Text)
	Else
		intPriorityHint = 0
	End If
		
	If strExpSpec = "" Then
		strResponse = "You must enter an experiment specification."
	Else
		AuthHeader.identifier = strBrokerId 
		AuthHeader.passkey = strBrokerPasskey 
		
            LSAPI.BrokerCred = AuthHeader
		
		SubmitReport = LSAPI.Submit(intExpID, strExpSpec, strGroup, intPriorityHint)
		ValidReport = SubmitReport.vReport
		WaitReport = SubmitReport.wait
		
		strResponse = "<b>MinTimeToLive:</b> " & SubmitReport.minTimetoLive & "<br>"
		strResponse = strResponse & "<b>Job Accepted:</b> " & ValidReport.accepted & "<br>"
		strResponse = strResponse & "<b>Estimated Run Time:</b> " & ValidReport.estRuntime & "<br>"
		strResponse = strResponse & "<b>Validation Error Message:</b> " & ValidReport.validationErrorMessage & "<br>"
		strResponse = strResponse & "<b>Validation Warning Message:</b> " & ValidReport.validationWarningMessages & "<br>"
		If ValidReport.accepted = "True" Then
			strResponse = strResponse & "<b>Effective Queue Length:</b> " & WaitReport.effectiveQueueLength & "<br>"
			strResponse = strResponse & "<b>Estimated Wait:</b> " & WaitReport.estWait & "<br>"
		Else
			strResponse = strResponse & "Job Number " & intExpId & " not executed.<br>"
		End If
		
		lblNewExpId2.Text = CStr(intExpId + 1)
		txtGroup2.Text = strGroup
		txtPriorityHint2.Text = CStr(intPriorityHint)
		txtExpSpec2.Text = strExpSpec
	End If

	strPageState = "SUBMITTED"

End Sub



 
</script>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>WebLab Lab Server Experiment Engine Test Page</title>
  </head>
  
  <body>
    <form Runat="server">
    <%
    Select Case strPageState
		Case "NEWLOAD"

    %>
		<p>
		This page serves as an inteface to test the job submission functionality of the WebLab v. 6.0 Lab Server.  Jobs submitted via this page are routed through the 
		Web Service Interface.  Thus, from the perspective of the Lab Server, this page is similar to a service broker submitting job requests.  The experiment specification itself, along with 
		some string user group name and an integer priority hint, must by supplied.  Upon submitting that information, the submission reciept is displayed and 
		the option to resubmit the job is given.  The results of the experiment itself will not be displayed by this page.
		<p>
		<table border=0 cellpadding=0 cellspacing=0>
			<tr>
				<td colspance=2><b>This submission's experiment ID:</b>
					<asp:Label
						id="lblNewExpId"	
						Runat="Server" />
				</td>
			</tr>
			
			<tr>
				<th>Enter a group Name (optional)</th>
				<td>
					<asp:TextBox
						id="txtGroup"
						Columns="5"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Enter your priority hint (optional):</th>
				<td>
					<asp:TextBox
						id="txtPriorityHint"
						Columns="5"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Insert XML Experiment Specification here (required):</th>
				<td>
					<asp:TextBox
						id="txtExpSpec"
						TextMode="Multiline"
						Columns="70"
						Rows="30"
						Wrap="True"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<td colspan=2>
					<asp:Button
						id="btnSubmit"
						text="Submit"
						OnClick="btnSubmit_Click"
						Runat="Server" />
				</td>
			</tr>
				
				
				
		</table>
	<%
		Case "SUBMITTED"
			Response.write("<b>Submission Response for ExpId " & intExpID & ":</b> <br>" & strResponse & "<p>")			
	%>
		<b>Resubmit Job:</b>
		<table border=0 cellpadding=0 cellspacing=0>
			<tr>
				<td colspance=2><b>This submission's experiment ID:</b>
					<asp:Label
						id="lblNewExpId2"	
						Runat="Server" />
				</td>
			</tr>
			
			<tr>
				<th>Group Name (optional)</th>
				<td>
					<asp:TextBox
						id="txtGroup2"
						Columns="5"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Priority Hint (optional):</th>
				<td>
					<asp:TextBox
						id="txtPriorityHint2"
						Columns="5"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<th>Experiment Specification (required):</th>
				<td>
					<asp:TextBox
						id="txtExpSpec2"
						TextMode="Multiline"
						Columns="70"
						Rows="30"
						Wrap="True"
						Runat="Server" />
				</td>
			</tr>
			<tr>
				<td colspan=2>
					<asp:Button
						id="btnReSubmit"
						text="ReSubmit"
						OnClick="btnReSubmit_Click"
						Runat="Server" />
				</td>
			</tr>
				
				
				
		</table>
    
    
    <%
    End Select
    %>
    

    </form>
  </body>
</html>
