<%@ Page Language="VBScript" ValidateRequest="False" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers.WebLabDataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery, strJobsInQueue, strUpdated As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead As Boolean
	
	Dim blnInProgress As Boolean = False
	Dim blnQueued As Boolean = False

	Sub Page_Load
		conWebLabLS.Open()
		'get initial info	
		
		'load user permission set for this page
		blnSRRead = False
		
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
		
		End If
	End Sub

	Sub Page_PreRender
		If blnSRRead Then
			'write info into display fields
			strUpdated = "Updated " & CStr(Now())
			
			'retrieve information on job currently being executed
			strDBQuery = "SELECT j.exp_id, j.provider_id, b.name, j.priority, j.submit_time, j.exec_time, j.est_exec_time, j.queue_at_insert, GETDATE() AS 'current_time' FROM JobRecord j JOIN Brokers b ON j.provider_id = b.broker_id WHERE j.job_status = 'IN PROGRESS'"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()	
			
			If dtrDBQuery.Read() Then
				blnInProgress = True
								
				lblExpInProgress.Text = "Job In Progress (" & DateDiff("s", dtrDBQuery("exec_time"), dtrDBQuery("current_time")) & "sec.)"
				lblIPExpID.Text = dtrDBQuery("exp_id")
				lblIPPriority.Text = dtrDBQuery("priority")
				lblIPName.Text = CheckBrokerName(dtrDBQuery("provider_id"), dtrDBQuery("name"))
				lblIPSubmitTime.Text = dtrDBQuery("submit_time")
				lblIPQueue.Text = dtrDBQuery("queue_at_insert")
				lblIPExecTime.Text = dtrDBQuery("exec_time")
				lblIPEstExec.Text = dtrDBQuery("est_exec_time")
				'current executing job info
			Else
				'if exec engine is idle
				blnInProgress = False
				
				lblIPNoneTag.Text = "No jobs are currently being executed."
				lblExpInProgress.Text = "Idle"
			End If	
			
			dtrDbQuery.Close()
			
			
			'check if there are jobs on the queue
			strDBQuery = "SELECT dbo.qm_CheckQueue() AS 'jobs_in_queue';"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			strJobsInQueue = cmdDBQuery.ExecuteScalar()
			
			If strJobsInQueue = "TRUE" Then
				blnQueued = True
				lblQStatus.Text = "Jobs Queued..."
			Else
				blnQueued = False
				lblQStatus.Text = "Empty"
				lblQNoneTag.Text = "No jobs are currently in the queue."
			End If
			
			'retrieve and write queue information
			strDBQuery = "SELECT q.exp_id, q.provider_id, b.name, q.priority, q.submit_time, q.est_exec_time, q.queue_at_insert FROM dbo.qm_QueueSnapshot() q JOIN Brokers b ON q.provider_id = b.broker_id;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			rptQueuedJobs.DataSource = cmdDBQuery.ExecuteReader()
			rptQueuedJobs.DataBind()
		
		End If	
			
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	Function DisplayDate(ByVal strDateVal As String) As String
		'takes as input a datetime literal and returns a display friendly version of the date
		Dim strOutput As String = ""
		
		If Not strDateVal Is Nothing Then
			'strOutput = MonthName(Month(strDateVal), True) & ". " & Day(strDateVal) & ", " & Year(strDateVal)
			strOutput = Month(strDateVal) & "/" & Day(strDateVal) & "/" & Year(strDateVal)
		End If
		
		Return strOutput		
	End Function
	
	Function DisplayTime(ByVal strDateVal As String) As String
		'takes as input a datetime literal and returns a dispaly friendly version of the time
		Dim strOutput As String = ""
		Dim intSec, intMin As Integer
		
		If Not strDateVal Is Nothing Then
			strOutput = Hour(strDateVal)
			
			intMin = Minute(strDateVal)
			intSec = Second(strDateVal)
			
			If intMin < 10 Then
				strOutput = strOutput & ":0" & intMin
			Else
				strOutput = strOutput & ":" & intMin
			End If
			
			If intSec < 10 Then
				strOutput = strOutput & ":0" & intSec
			Else
				strOutput = strOutput & ":" & intSec
			End If
			
		End If
		
		Return strOutput		
	End Function
	
	Function CheckBrokerName(ByVal intBrokerId As Integer, ByVal objBrokerName As Object) As String
		'this method checks that there is a non-empty name value displayed for this page.  Proper jobs will have their names in objBrokerName
		'at invocation.  A job with an intBrokerId of 0 denotes an job issued from an LS component.  These do not resolve to names on their own
		'so they are assigned the name "Internal" here.  Jobs with a nonzero broker ID and no name are assigned "Unknown Caller".
		Dim strOutput As String = ""
		
		If  intBrokerId = 0 Then 
			strOutput = "Internal"
		ElseIf intBrokerId > 0 AND Not objBrokerName Is DBNull.Value Then
			strOutput = CStr(objBrokerName)
		ElseIf intBrokerId > 0 And Not objBrokerName Is DBNull.Value Then
			strOutput = "Unknown"
		End If
		
		Return strOutput
	End Function
	
	Function CheckForDBNull(ByVal objData As Object) As String
		'this method checks for a DBnull in the argument database return object, if the value is Null, the string "None" is returned. Otherwise,
		'the string representation of the input value is returned.
		Dim strOutput As String = ""
		
		If objData Is DBNull.Value Then
			strOutput = "None"
		ElseIf Trim(CStr(objData)) = "" Then
			strOutput = "None"
		Else	
			strOutput = Trim(CStr(objdata))
		End If
		
		Return strOutput
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnSRRead Then  %>
    <form Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					Experiment Currently Running:
					</font>
					<b>
					<font class="extra-small">
					&nbsp;<asp:Label
							ID="lblExpInProgress"
							Runat="Server" />
						</b>
						-
						<%=strUpdated%>
					</font>
					<hr size=1>
					<p>
					
					<%
					If blnInProgress Then
					%>
					<center>
					<table border=0 cellpadding=3 cellspacing=0 width=100%>
						<tr>
							<td align="left">
								<font class="regular">
									<b>Experiment ID:</b>
									<asp:Label
										ID="lblIPExpID"
										Runat="Server" />
								</font>
							</td>
							<td align="left">
								<font class="regular">
									<b>Job Priority:</b>
									<asp:Label
										ID="lblIPPriority"
										Runat="Server" />
								</font>
							</td>
						</tr>
						
						<tr>
							<td align="left">
								<font class="regular">
									<b>Broker Name:</b>
									<asp:Label
										ID="lblIPName"				
										Runat="Server" />
								</font>
							</td>
							<td align="left">
								<font class="regular">
									<b>Submit Time:</b>
									<asp:Label
										ID="lblIPSubmitTime"
										Runat="Server" />
								</font>
							</td>
						</tr>
						<tr>
							<td align="left">
								<font class="regular">
									<b>Queue At Insert:</b>
									<asp:Label
										ID="lblIPQueue"
										Runat="Server" />
								</font>
							</td>
							<td align="left">
								<font class="regular">
									<b>Execution Time:</b>
									<asp:Label 
										ID="lblIPExecTime"
										Runat="Server" />
								</font>
							</td>
						</tr>
						<tr>
							<td align="left" colspan="2">
								<font class="regular">
									<b>Estimated Elapsed Exececution Time:</b>
									<asp:Label
										ID="lblIPEstExec"
										Runat="Server" />
								</font>
							</td>
						</tr>
					</table>
					</center>
					<%
					Else
					%>
					<font class="extra-small"><em>
						<asp:Label 
							ID="lblIPNoneTag"
							Runat="Server" /></em></font>
					
					<%
					End If
					%>
					
					
				</td>
			</tr>
			
			<tr>
				<td>
					<font class="title">
					Experiment Queue Status:
					</font>
					<b>
					<font class="extra-small">
					&nbsp;<asp:Label
							ID="lblQStatus"
							Runat="Server" />
						</b>

						-
						<%=strUpdated%>
					</font>
					<hr size=1>
					<p>
					
					<%
					If blnQueued Then
					%>
					<center>
					<table border=0 cellpadding=3 cellspacing=0 width=100%>
						<tr bgcolor="#e0e0e0">
							<th align=left><font class="regular">Experiment ID</font></th><th align=left><font class="regular">Broker Name</font></th><th align=left><font class="regular">Priority</font></th><th align=left><font class="regular">Queue <br>at insert</font></th><th align=left><font class="regular">Submisssion Time</font></th><th align=left><font class="regular">Estimated <br>Exec. Time</font></th>
						</tr>
						<asp:Repeater
							ID="rptQueuedJobs"
							Runat="Server">
							<ItemTemplate>
								<tr>
									<td><font class="regular"><%#Container.DataItem("exp_id")%></font></td><td><font class="regular"><%#CheckBrokerName(Container.DataItem("provider_id"), Container.DataItem("name"))%></font></td><td><font class="regular"><%#Container.DataItem("priority")%></font></td><td><font class="regular"><%#Container.DataItem("queue_at_insert")%></font></td><td><font class="regular"><%#DisplayDate(Container.DataItem("submit_time"))%>&nbsp;<%#DisplayTime(Container.DataItem("submit_time"))%></font></td><td><font class="regular"><%#Container.DataItem("est_exec_time")%></font></td>
								</tr>
							</ItemTemplate>
	
						</asp:Repeater>
						<tr bgcolor="#e0e0e0">
							<td align=left colspan=3>
								
										&nbsp;
								
								
							</td>
							<td align=right colspan=3>
								
							</td>
						</tr>
					</table>
					</center>
					<%
					Else
					%>
					<font class="extra-small"><em>
						<asp:Label 
							ID="lblQNoneTag"
							Runat="Server" /></em></font>
					<%
					End If
					%>
 				</td>
			</tr>
			<tr>
				<td>
				<center>
					<font class="small">
						<%If blnSRRead Then%>
							<a href="main.aspx" target="main">Return to Main</a>
						<%Else%>
							<a href="../main.aspx" target="main">Return to Main</a>
						<%End If%>
					</font>
				</center>
				</td>
			</tr>
		</table>

    </form>
  <%End If%>
  </body>
</html>
