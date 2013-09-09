<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Math" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery, strSetupNameList, strSetupStatList As String
	Dim dcmInSuccess, dcmOutSuccess As Decimal
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim intQueuedJobs, intDoneJobs, intTotalJobs, intSetups, loopIdx, intRows As Integer
	Dim dblRows As Double
	Dim blnJobInProgress As Boolean = False
	Dim blnHaveSetups As Boolean = False
	DIm blnDisplaySec As Boolean = False
	Dim strSetupNameArray, strSetupStatArray As String()
	
	Sub Page_Load
		conWebLabLS.Open()
		
				
		intQueuedJobs = 0
		intTotalJobs = 0 
		intDoneJobs = 0
		
		'lab status info
		If application("ws_int_is_active") = "True" And application("exp_eng_is_active") = "True" Then
			lblLabStatus.Text = "Active"	
		ElseIf application("ws_int_is_active") = "False" And application("exp_eng_is_active") = "True" Then
			lblLabStatus.Text = "Interface is Disabled" 
		ElseIf application("ws_int_is_active") = "True" And application("exp_eng_is_active") = "False" Then
			lblLabStatus.Text = "Experiment Engine is Disabled"
		Else
			lblLabStatus.Text = "Lab is Offline"
		End If
		
		
			
		
		'active Setup info
			strDBQuery = "SELECT r.name, a.is_active FROM ActiveSetups a LEFT JOIN Setups s ON a.setup_id = s.setup_id LEFT JOIN Resources r ON s.resource_id = r.resource_id ORDER BY a.active_id;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			'starts list, first row
			If dtrDBQuery.Read()
				If dtrDBQuery("name") Is DBNull.Value Then
					strSetupNameList = "No Setup"
				Else			
					strSetupNameList = dtrDBQuery("name")	
				End If
				
				strSetupStatList = dtrDBQuery("is_active")
				blnHaveSetups = True
			End If 
			
			'appends remaining rows
			While dtrDBQuery.Read()
				If dtrDBQuery("name") Is DBNull.Value Then
					strSetupNameList = strSetupNameList & ":!:" & "No Setup"
				Else			
					strSetupNameList = strSetupNameList & ":!:" & dtrDBQuery("name")
				End If
				
				strSetupStatList = strSetupStatList & ":!:" & dtrDBQuery("is_active")
			End While
			
			dtrDBQuery.Close()
			
			'split lists into arrays, measure and prep for table creation in render section
			strSetupNameArray = Split(strSetupNameList, ":!:")
			strSetupStatArray = Split(strSetupStatList, ":!:")
			
			intSetups = UBound(strSetupNameArray)
		
		
		'queue/exec subsys info
			strDBQuery = "SELECT COUNT(*) FROM JobRecord WHERE job_status = 'QUEUED';"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			
			intQueuedJobs = CInt(cmdDBQuery.ExecuteScalar())
		
		
			strDBQuery = "SELECT COUNT(*) As job_count, job_status FROM JobRecord WHERE submit_time BETWEEN DATEADD(hour, -1, GETDATE()) AND GETDATE() GROUP BY job_status;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			While dtrDBQuery.Read()
				Select Case dtrDBQuery("job_status")
				
					Case "QUEUED"
						'intQueuedJobs = CInt(dtrDBQuery("job_count"))
						intTotalJobs = intTotalJobs + CInt(dtrDBQuery("job_count"))
					Case "IN PROGRESS"
						intTotalJobs = intTotalJobs + CInt(dtrDBQuery("job_count")) 'should always be in {0,1}
						blnJobInProgress = True
					
					Case "CANCELLED"
						intTotalJobs = intTotalJobs + CInt(dtrDBQuery("job_count"))
					Case "COMPLETE"
						intDoneJobs = intDoneJobs + CInt(dtrDBQuery("job_count"))
						intTotalJobs = intTotalJobs + CInt(dtrDBQuery("job_count"))
				End Select
			
			End While
		
			dtrDBQuery.Close()
			
			lblQJobSubsPH.Text = intTotalJobs
			lblQJobExecsPH.Text = intDoneJobs
			lblQLength.Text = intQueuedJobs
			
			
			
			If application("exp_eng_is_active") = "True" Then
				lblQStatus.Text = "Idle"
				If blnJobInProgress Then
					strDBQuery = "SELECT DATEDIFF(second, exec_time, GETDATE()) AS time_since_dequeue FROM JobRecord WHERE job_status = 'IN PROGRESS';"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					lblQCurrentExecElapsed.Text = cmdDBQuery.ExecuteScalar()
					lblQStatus.Text = "Executing..."
					blnDisplaySec = True
				ElseIf intQueuedJobs > 0 Then
					'case where jobs are queued but exec engine is idle (normally between executions), termed "resetting" or "initiallizing"
					strDBQuery = "SELECT TOP 1 DATEDIFF(second, exec_time, GETDATE()) FROM JobRecord WHERE job_status = 'COMPLETE' ORDER BY submit_time DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					lblQCurrentExecElapsed.Text = cmdDBquery.ExecuteScalar()
					lblQStatus.Text = "Initiallizing..."
					blnDisplaySec = True
				End If
			Else
				lblQStatus.Text = "Disabled by Admin"
			end If
				
			
			'CONSTRUCT STATUS INDICATOR, THEN MOST RECENT JOB LISTING
			strDBQuery = "SELECT TOP 4 b.name AS BrokerName, r.name AS SetupName, j.exec_time, j.exec_elapsed, j.error_occurred FROM JobRecord j JOIN Brokers b ON j.provider_id = b.broker_id JOIN Setups p ON j.setup_used = p.setup_id JOIN Resources r ON p.resource_id = r.resource_id WHERE job_status = 'COMPLETE' ORDER BY exec_time DESC;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			rptJobList.DataSource = dtrDBQuery
			rptJobList.DataBind()
			
			dtrDBQuery.Close()
			
		
		'web method interface info
			strDBQuery = "SELECT type, COUNT(*) As Total, (SUM(CAST(completed_successfully AS decimal)) / COUNT(*)) * 100 As SuccessRate FROM WebMethodRequestLog WHERE transaction_time BETWEEN DATEADD(hour, -1, GETDATE()) AND GETDATE() GROUP BY type;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			lblInReqsPH.Text = "0"
			lblInReqsPHSuccessRate.Text = "0"
			lblOutReqsPH.Text = "0"
			lblOutReqsPHSuccessRate.Text = "0"
			
			While dtrDBQuery.Read()
				If dtrDBQuery("type") = "INCOMING" Then
					'have data for INCOMING requests
					lblInReqsPH.Text = dtrDBQuery("Total")
					dcmInSuccess = dtrDBQuery("SuccessRate")
								
					lblInReqsPHSuccessRate.Text = CStr(Round(dcmInSuccess, 0))
				ElseIf dtrDBQuery("type") = "OUTGOING" Then
					'have data for OUTGOING requests
					lblOutReqsPH.Text = dtrDBQuery("Total")
					dcmOutSuccess = dtrDBQuery("SuccessRate")
					
					lblOutReqsPHSuccessRate.Text = CStr(Round(dcmOutSuccess, 0))
				End If
			End While
			
			dtrDBQuery.Close()
			
			strDBQuery = "SELECT TOP 2 type, source_name, dest_name, method_name, transaction_time, completed_successfully FROM WebMethodRequestLog ORDER BY transaction_time DESC;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
		
			'use data to fill in labels
			If dtrDBQuery.Read() Then
				'read/process the first record.
				If dtrDBQuery("type") = "INCOMING" Then
					lblReqDir1.Text = "In"
					lblAgent1.Text = Trim(dtrDBQuery("source_name"))
					
					'rest of labels are type independent
					lblName1.Text = Trim(dtrDBQuery("method_name"))
					lblDate1.Text = FormatDateTime(dtrDBQuery("transaction_time"))
					
					If dtrDBQuery("completed_successfully") = "1" Then
						lblStatus1.Text = "Success"
					Else
						lblStatus1.Text = "Error"
					End If
									
				ElseIf dtrDBQuery("type") = "OUTGOING" Then
					lblReqDir1.Text = "Out"
					lblAgent1.Text = Trim(dtrDBQuery("dest_name"))
					
					'rest of labels are type independent
					lblName1.Text = Trim(dtrDBQuery("method_name"))
					lblDate1.Text = FormatDateTime(dtrDBQuery("transaction_time"))
					
					If dtrDBQuery("completed_successfully") = "1" Then
						lblStatus1.Text = "Success"
					Else
						lblStatus1.Text = "Error"
					End If
				End If
				If dtrDBQuery.Read() Then
					'read/process the second record
					If dtrDBQuery("type") = "INCOMING" Then
					lblReqDir2.Text = "In"
					lblAgent2.Text = Trim(dtrDBQuery("source_name"))
					
					'rest of labels are type independent
					lblName2.Text = Trim(dtrDBQuery("method_name"))
					lblDate2.Text = FormatDateTime(dtrDBQuery("transaction_time"))
					
					If dtrDBQuery("completed_successfully") = "1" Then
						lblStatus2.Text = "Success"
					Else
						lblStatus2.Text = "Error"
					End If
									
					ElseIf dtrDBQuery("type") = "OUTGOING" Then
						lblReqDir2.Text = "Out"
						lblAgent2.Text = Trim(dtrDBQuery("dest_name"))
						
						'rest of labels are type independent
						lblName2.Text = Trim(dtrDBQuery("method_name"))
						lblDate2.Text = FormatDateTime(dtrDBQuery("transaction_time"))
						
						If dtrDBQuery("completed_successfully") = "1" Then
							lblStatus2.Text = "Success"
						Else
							lblStatus2.Text = "Error"
						End If
					End If
				End If
				
			
			End IF
			
			dtrDBQuery.Close()
	
	End Sub

	Sub Page_PreRender
		conWebLabLS.Close()
	End Sub

	Function FormatDateTime(ByVal strDateTime As String) As String
		Dim strReturnVal, strMinutes, strSeconds As String
		
		strMinutes = Minute(strDateTime)
		
		If CInt(strMinutes) < 10 Then
			strMinutes = "0" & strMinutes
		End If
		
		strSeconds = Second(strDateTime)
		
		If CInt(strSeconds) < 10 Then
			strSeconds = "0" & strSeconds
		End If
		
		strReturnVal = Month(strDateTime) & "/" & Day(strDateTime) & " " & Hour(strDateTime) & ":" & strMinutes & ":" & strSeconds
		Return strReturnVal
	End Function
	
	Function ErrorOutputConvert(ByVal strBitInput As String, ByVal blnNoErrorState As Boolean) As String
		If blnNoErrorState Then
			'case where true or "1" should be interpreted as success
			If UCase(strBitInput) = "TRUE" Then
				Return "Success"
			Else
				Return "Error"
			End If
		Else
			'case where false or "0" should be interpreted as success
			If UCase(strBitInput) = "TRUE" Then
				Return "Error"
			Else
				Return "Success"
			End If
		End If
	End Function


</script>




<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
	<%
	if not (InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), UCase("win")) = 0 and InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), UCase("mac")) = 0) then
	%>
      <link rel="stylesheet" type="text/css" href="/weblabwin.css">
    <%
    else
    %>
	  <link rel="stylesheet" type="text/css" href="/weblabathena.css">
	<%end if%>
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio.NET 7.0">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  
  <body>
  <% If Not Session("LoggedInAsUserName") Is Nothing Then  %>
    <form Runat="server">
		<table cellspacing=0 cellpadding=0 border=0 width=100%>
			<tr>
				<td><font class="regular"><b>Lab Status:</b>&nbsp;
						<asp:Label
							ID="lblLabStatus"
							Runat="Server" />
					<br><hr size="1" noshade>
					</font>
				</td>
			</tr>
			<%If blnHaveSetups Then%>
			<tr>
				<td>
					<table cellspacing=0 cellpadding=0 border=0 width=100%>
						<tr>
							<td colspan=2>
								<font class="regular"><b>Setup Status:</b></font>
							</td>
						</tr>
						<tr>
						<%
						dblRows = (intSetups + 1) / 2
						intRows =  CInt(Ceiling(dblRows))
						
						For loopIdx = 0 To intRows - 1
							If loopIdx <= intSetups Then
						%>
							
							<td width=50%><font class="small">
								<%=strSetupNameArray(loopIdx)%>
								(<%
								If strSetupStatArray(loopIdx) = "True" Then 
									Response.Write("active") 
								Else 
									Response.Write("<font color=red>inactive</font>") 
								End If%>)
								</font>
							</td>
							<%
							End If
							%>
							<td>
							<%
							If (loopIdx + intRows) <= intSetups Then
							%>
								<font class="small">
									 <%=strSetupNameArray(loopIdx + intRows)%> 
									(<%
									If strSetupStatArray(loopIdx + intRows) = "True" Then 
										Response.Write("active") 
									Else 
										Response.Write("<font color=red>inactive</font>") 
									End If%>)
								</font>
							<%Else%>
							<font class="small">
								&nbsp;
							</font>
							<%End If%>
							</td>
						</tr>
						<%Next%>
					</table>
					<hr size="1" noshade>
				</td>
			</tr>
			<%End If%>
			<tr>
				<td>
					<table cellspacing=0 cellpadding=0 border=0 width=100%>
						<tr>
							<td><font class="regular"><b>Queuing & Execution System:</b></font></td>
							<td align=center><font class="small">Most Recent Jobs:&nbsp;&nbsp; <font class="extra-small">(<a href="/labserver/admin/exec-log.aspx" target="main">View All</a>)</font></font></td>
						</tr>
						<tr>
							<td width=37%>
								<font class="small">
									Current Status:&nbsp; 
									<b>
									<asp:Label
										ID="lblQStatus"
										Runat="Server" />
									<%If blnDisplaySec Then 'lblQStatus.Text = "Executing..." OR CInt(lblQLength.Text) > 0 Then %>
										(<asp:Label
											ID="lblQCurrentExecElapsed"
											Runat="Server" />
										sec.)
									<%end if%>
									</b><br>
									Current Queue Length:&nbsp;
									<em>
									<asp:Label
										ID="lblQLength"
										Runat="Server" />
									</em>
									<br>
									In the Past Hour:<br>
									&nbsp;&nbsp;&nbsp;&nbsp;Job Submissions:&nbsp; 
									<em>
									<asp:Label
										ID="lblQJobSubsPH"
										Runat="Server" />
									</em>
									<br>
									&nbsp;&nbsp;&nbsp;&nbsp;Job Executions:&nbsp;
									<em>
									<asp:Label
										ID="lblQJobExecsPH"
										Runat="Server" />
									</em>
								</font>
							</td>
							<td>
								<table cellspacing=0 cellpadding=0 border=0 align=right width=100%>
									<tr>
										<th align=left><font class="extra-small">Broker</font></th>
										<th align=left><font class="extra-small">Setup</font></th>
										<th align=left><font class="extra-small">Exec. Time</font></th>
										<th align=left><font class="extra-small">Exec. Elapsed</font></th>
										<th align=left><font class="extra-small">Status</font></th>
									</tr>
									<asp:Repeater
										ID="rptJobList"
										Runat="Server">
										<ItemTemplate>
											<tr>
												<td><font class="extra-small"><%#Container.DataItem("BrokerName")%></font></td>
												<td><font class="extra-small"><%#Container.DataItem("SetupName")%></font></td>
												<td><font class="extra-small"><%#FormatDateTime(Container.DataItem("exec_time"))%></font></td>
												<td><font class="extra-small"><%#Container.DataItem("exec_elapsed")%>&nbsp;sec.<font></td>
												<td><font class="extra-small"><%#ErrorOutputConvert(Container.DataItem("error_occurred"), False)%></font></td>
											</tr>	
										
										</ItemTemplate>
									</asp:Repeater>
								<%'WRITE ME%>
								</table>
							</td>
						</tr>	
					</table>
					<hr size="1" noshade>
				</td>
			</tr>
			<tr>
				<td>
					<table cellspacing=0 cellpadding=0 border=0 width=100%>
						<tr>
							<td><font class="regular"><b>Web Service Interface Traffic:</b></font></td>
							<td align=center><font class="small">Most Recent Requests:&nbsp;&nbsp; <font class="extra-small">(<a href="/labserver/admin/wsint-log.aspx" target="main">View All</a>)</font></font></td>
						</tr>
						<tr>
							<td width=37%>
								<font class="small">
									In the Past Hour:<br>
									&nbsp;&nbsp;&nbsp;&nbsp;Incoming: 
									<em>
									<asp:Label
										ID="lblInReqsPH"
										Runat="Server" />
									&nbsp;
									<%If lblInReqsPH.Text <> "0" Then %>
									(<asp:Label
										ID="lblInReqsPHSuccessRate"
										Runat="Server" />
									% successful)
									<%End If%>
									</em>
									<br>
									&nbsp;&nbsp;&nbsp;&nbsp;Outgoing: 
									<em>
									<asp:Label
										ID="lblOutReqsPH"
										Runat="Server" />
									&nbsp;
									<%If lblOutReqsPH.Text <> "0" Then%>
									(<asp:Label
										ID="lblOutReqsPHSuccessRate"
										Runat="Server" />
									% successful)
									<%End If%>
									</em>	
								</font>
							</td>
							<td>
	
									
									<table cellpadding=0 cellspacing=0 border=0 align=right width=100%>
										<tr>
											<th align=left><font class="extra-small">In/Out</font></th>
											<th align=left><font class="extra-small">Remote Agent</font></th>
											<th align=left><font class="extra-small">Method Name</font></th>
											<th align=left><font class="extra-small">Date</font></th>
											<th align=left><font class="extra-small">Status</font></th>
										</tr>
										<tr>
											<td><font class="extra-small"><asp:Label ID="lblReqDir1" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblAgent1" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblName1" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblDate1" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblStatus1" Runat="Server" /></font></td>
										</tr>
										<tr>
											<td><font class="extra-small"><asp:Label ID="lblReqDir2" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblAgent2" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblName2" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblDate2" Runat="Server" /></font></td>
											<td><font class="extra-small"><asp:Label ID="lblStatus2" Runat="Server" /></font></td>
										</tr>
									<table>
									<%'since fixed amount, encode as labels.%>
								</font>
							</td>
						</tr>	
					</table>
				</td>
			</tr>
		</table>
		<hr size="1" noshade>
    </form>
  <%End If%>
  </body>
</html>
