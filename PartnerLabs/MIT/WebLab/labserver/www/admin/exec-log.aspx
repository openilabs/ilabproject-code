<%@ Page Language="VBScript" ValidateRequest="False"%>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers" %>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->


<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim intStartIdx, intInterval, intEndIdx As Integer
	Dim strStartIdx, strLastIdx As String
	Dim strPageState, strDetailID As String
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead As Boolean
	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		blnSRRead = False
		
		'load user permissions set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			
			If blnSRRead Then
		
				strDBQuery = "SELECT COUNT(*) FROM JobRecord WHERE NOT job_status = 'IN PROGRESS' AND NOT job_status = 'QUEUED';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				
				intEndIdx = CInt(cmdDBQuery.ExecuteScalar())
				strPageState = "LIST"
				
				If not Page.IsPostBack then
					'get initial record set
					intStartIdx = 1
					intInterval = dropInterval.SelectedItem.Value
					
					
					'dtrDBQuery = GetLogRecords(intStartIdx, intInterval)
					
					'rptWSIntRecs.dataSource = dtrDBQuery
					'rptWSIntRecs.DataBind()		
				Else
					intStartIdx = Request.Form("intStartIdx")
					intInterval = dropInterval.SelectedItem.Value		
				End If
			
			End If
		End If
		
	End Sub

	Sub Page_PreRender
		If blnSRRead Then
	
			'write info into display fields
			
			If strPageState = "LIST" Then
				dtrDBQuery = GetLogRecords(intStartIdx, intInterval)
					
				rptExecRecs.dataSource = dtrDBQuery
				rptExecRecs.DataBind()
				dtrDBQuery.Close()	
				
				If intInterval = 0 Then
					strStartIdx = "1"
					strLastIdx = CStr(intEndIdx)
				Else
					strStartIdx = CStr(intStartIdx)
					strLastIdx = CStr(intStartIdx + intInterval - 1)
				End If
			End If
		End If		
			
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	Function GetLogRecords(ByVal intStartIdx AS Integer, ByVal intInterval As Integer) As SqlDataReader
		'function dedicated to calling record filter procedure on DB
		Dim strDBQuery As String
		Dim cmdDBQuery As SqlCommand
		
		strDBQuery = "EXEC rm_ReturnJobLogSubset @StartIdx, @Interval;"
		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
		cmdDBQuery.Parameters.Add("@StartIdx", intStartIdx)
		cmdDBQuery.Parameters.Add("@Interval", intInterval)
		
		Return cmdDBQuery.ExecuteReader()	
	End Function
	
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
	
	Function DisplayErrorResult(ByVal strErrorVal As String) As String
		'takes a string cast of a boolean value (true or false) and represents it as either "Success" or "Error" where "True" indicates that an error occured.
		Dim strOutput As String = ""
		
		If UCase(strErrorVal) = "TRUE" Then
			strOutput = "Error"
		ElseIf UCase(strErrorVal) = "FALSE" Then
			strOutput = "Success"
		Else
			strOutput = strErrorVal
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
	
	Function CheckGroup(ByVal objGroup As Object) As String
	'this method checks for a DBNull in the argument database return object.  This object is a group name value, which should be displayed in
	'parens if not null.  Only an empty string should be returned if the argument is a DBNull or an empty string
		Dim strOutput As String = ""
		
		If objGroup.GetType.ToString = "DBNull" Then
			strOutput = ""
		ElseIf Trim(CStr(objGroup)) = "" Then
			strOutput = ""
		Else
			strOutput = "(" & Trim(CStr(objGroup)) & ")"
		End If
		
		Return strOutput
	End Function
	
	Sub First_Click(s As Object, e As EventArgs)
		intStartIdx = 1
		
	End Sub

	Sub Next_Click(s As Object, e As EventArgs)
		intStartIdx = (intStartIdx + intInterval)
		
	End Sub
	
	Sub Previous_Click(s As Object, e As EventArgs)
		intStartIdx = intStartIdx - intInterval
		
	End Sub
	
	Sub Last_Click(s As Object, e As EventArgs)
		intStartIdx = (intEndIdx - intInterval) + 1
		
	End Sub	
	
	Sub ShowDetails_Command(s As Object, e As CommandEventArgs)
		If blnSRRead Then
			strPageState = "DETAILS"
			strDetailID = e.CommandArgument
			
			strDBQuery = "SELECT j.exp_id, j.provider_id, b.name As broker_name, j.broker_assigned_id, j.groups, j.priority, j.job_status, j.submit_time, j.exec_time, j.end_time, j.exec_elapsed, j.job_elapsed, j.est_exec_time, j.queue_at_insert, j.datapoints, j.device_profile_used, r.name As device_name, j.lab_config_at_exec, j.experiment_vector, j.experiment_results, j.error_report, j.error_occurred, j.downloaded FROM JobRecord j LEFT JOIN Brokers b ON j.provider_id = b.broker_id LEFT JOIN DeviceProfiles d ON j.device_profile_used = d.profile_id LEFT JOIN Resources r ON d.resource_id = r.resource_id WHERE j.exp_id = @ExpID;"		
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@ExpID", strDetailID)
			
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			If dtrDBQuery.Read() Then
				lblLocExpId.text = dtrDBQuery("exp_id")
				lblResult.Text = DisplayErrorResult(dtrDBQuery("error_occurred"))
				lblDeviceName.Text = CheckForDBNull(dtrDBQuery("device_name"))
				lblQueueStat.Text = dtrDBQuery("queue_at_insert")
				lblBroker.Text = CheckBrokerName(Cint(dtrDbQuery("provider_id")), dtrDBQuery("broker_name"))
				lblGroup.Text = CheckForDBNull(dtrDBQuery("groups"))
				lblBrokerJobId.Text = dtrDBQuery("broker_assigned_id")
				lblPriority.Text = dtrDBQuery("priority")
				lblDownloaded.Text = dtrDbQuery("downloaded")
				lblStatus.Text = StrConv(dtrDBQuery("job_status"), VBStrConv.ProperCase)
				lblDatapoints.Text = dtrDBQuery("datapoints")
				lblSubmitTime.Text = dtrDBQuery("submit_time")
				lblExecTime.Text = CheckForDBNull(dtrDBQuery("exec_time"))
				lblEndTime.Text = CheckForDBNull(dtrDBQuery("end_time"))
				
				Dim strActExec, strEstExec, strJob As String
				strActExec = CheckForDBNull(dtrDBQuery("exec_elapsed"))
				strEstExec = CheckForDBNull(dtrDBQuery("est_exec_time"))
				strJob = CheckForDBNull(dtrDBQuery("job_elapsed"))
				
				If IsNumeric(strActExec) Then
					strActExec = strActExec & " Seconds"
				End If
				
				If IsNumeric(strEstExec) Then
					strEstExec = strEstExec & " Seconds"
				End If
				
				If IsNumeric(strJob) Then
					strJob = strJob & " Seconds"
				End If
				
				lblEstExecElapsed.Text = strEstExec
				lblActExecElapsed.Text = strActExec
				lblJobElapsed.Text = strJob
				
				txtErrorRpt.Text = CheckForDBNull(dtrDBQuery("error_report"))
				txtExpSpec.Text = CheckForDBNull(dtrDBQuery("experiment_vector"))
				txtLabConfig.Text = CheckForDBNull(dtrDBQuery("lab_config_at_exec"))
				txtExpResults.Text = CheckForDBNull(dtrDBQuery("experiment_results"))
			
			End If
			
			dtrDBQuery.Close()
		End If	
	End Sub
	
	Sub ShowList_Click(s As Object, e As EventArgs)
		If blnSRRead Then
			strPageState = "LIST"
			intInterval = Request.Form("intInterval")
		End If
	End Sub
	
	
	
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
					Experiment Execution Log:
					</font>
					<%
					Select Case strPageState
						Case "LIST"
						'log list display code
					%>
						<font class="extra-small">
						&nbsp;Records <b><%=strStartIdx%></b> through <b><%=Math.Min(Cint(strLastIdx), intEndIdx)%></b> out of <b><%=intEndIdx%></b>
						</font>
						<hr size=1>
						<p>
						<center>
						<table border=0 cellpadding=3 cellspacing=0 width=100%>
							<tr bgcolor="#e0e0e0">
								<th align=left><font class="regular">Broker (Group)</font></th><th align=left><font class="regular">Submission Time</font></th><th align=left><font class="regular">Device</font></th><th align=left><font class="regular">Queue <br>at insert</font></th><th align=left><font class="regular">Exec. <br>Elapsed</font></th><th align=left><font class="regular">Result</font></th>
							</tr>
							<asp:Repeater
								ID="rptExecRecs"
								Runat="Server">
								<AlternatingItemTemplate>
									<tr BGCOLOR="#EEEEEE">
										<td><font class="regular"><%#CheckBrokerName(CInt(Container.DataItem("provider_id")), Container.DataItem("provider_name"))%><br><%#CheckGroup(Container.DataItem("groups"))%></font></td><td><font class="regular"><%#DisplayDate(Container.DataItem("submit_time"))%>&nbsp;<%#DisplayTime(Container.DataItem("submit_time"))%></font></td><td><font class="regular"><%#Container.DataItem("profile_name")%></font></td><td><font class="regular"><%#Container.DataItem("queue_at_insert")%></font></td><td><font class="regular"><%#Container.DataItem("exec_elapsed")%></font></td><td><font class="regular"><asp:LinkButton Text='<%#DisplayErrorResult(Container.DataItem("error_occurred"))%>' CommandArgument='<%#Container.DataItem("exp_id")%>' OnCommand="ShowDetails_Command" Runat="Server" /></font></td>
									</tr>
								</AlternatingItemTemplate>
								<ItemTemplate>
									<tr>
										<td><font class="regular"><%#CheckBrokerName(CInt(Container.DataItem("provider_id")), Container.DataItem("provider_name"))%><br><%#CheckGroup(Container.DataItem("groups"))%></font></td><td><font class="regular"><%#DisplayDate(Container.DataItem("submit_time"))%>&nbsp;<%#DisplayTime(Container.DataItem("submit_time"))%></font></td><td><font class="regular"><%#Container.DataItem("profile_name")%></font></td><td><font class="regular"><%#Container.DataItem("queue_at_insert")%></font></td><td><font class="regular"><%#Container.DataItem("exec_elapsed")%></font></td><td><font class="regular"><asp:LinkButton Text='<%#DisplayErrorResult(Container.DataItem("error_occurred"))%>' CommandArgument='<%#Container.DataItem("exp_id")%>' OnCommand="ShowDetails_Command" Runat="Server" /></font></td>
									</tr>
								</ItemTemplate>
		
							</asp:Repeater>
							<tr bgcolor="#e0e0e0">
								<td align=left colspan=3>
									<font class="regular">
										<%
										If intInterval <> 0 Then
											If (intInterval - intStartIdx) <= 0 Then
											%>
											<asp:LinkButton Text="First" OnClick="First_Click" Runat="Server" />
											|
											<asp:LinkButton Text="Previous" OnClick="Previous_Click" Runat="Server" />
											|
											<%
											Else
											%>
											First
											|
											Previous
											|
											<%
											End If
											If (intEndIdx - intStartIdx) > intInterval Then
											%>
											<asp:LinkButton Text="Next" OnClick="Next_Click" Runat="Server" />
											|
											<asp:LinkButton Text="Last" OnClick="Last_Click" Runat="Server" />
											<%Else%>
											Next
											|
											Last
											<%End If%>
										<%Else%>
											&nbsp;
										<%End If%>
									</font>
								</td>
								<td align=right colspan=3>
									<font class="regular">Records Per Page:
										<asp:DropDownList 
											ID="dropInterval" 
											AutoPostBack="True"
											Runat="Server" >
											<asp:ListItem 
												Text="10" 
												Value="10" />
											<asp:ListItem 
												Text="25" 
												Value="25"
												Selected="True" />
											<asp:ListItem 
												Text="50" 
												Value="50" />
											<asp:ListItem 
												Text="100" 
												Value="100" />
											<asp:ListItem 
												Text="All" 
												Value="0" />
										</asp:DropDownList>
									</font>
								</td>
							</tr>
						</table>
						</center>
					<%
						CASE "DETAILS"
						'experiment details display code
					%>
						<font class="extra-small">
						&nbsp;<b>Experiment Details</b>
						</font>
						<hr size=1>

						<center>
						<table border=0 cellpadding=3 cellspacing=0 width=100%>
							<tr>
								<td width=25%><font class="extra-small">&nbsp;</font></td>
								<td width=25%><font class="extra-small">&nbsp;</font></td>
								<td width=25%><font class="extra-small">&nbsp;</font></td>
								<td width=25%><font class="extra-small">&nbsp;</font></td>
							</tr>
							<tr bgcolor="#e0e0e0">
								<th colspan=4 align=left><font class="regular">General Information</font></th>
							</tr>
							<tr>
								<td colspan=2 align=left><font class="regular"><b>Local Experiment ID:</b>
									<asp:Label
										ID="lblLocExpId"
										Runat="Server" /></font>
								</td>												
								<td colspan=2 align=left><font class="regular"><b>Result:</b>
									<asp:Label
										ID="lblResult"
										Runat="Server" /></font>
								</td>												
							</tr>
							<tr>
								<td colspan=2 align=left><font class="regular"><b>Device Used:</b>
									<asp:Label
										ID="lblDeviceName"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left><font class="regular"><b>Queue Status At Submit:</b>
									<asp:Label
										ID="lblQueueStat"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=4><p>&nbsp;</p></td>
							</tr>
							<tr bgcolor="#e0e0e0">
								<th colspan=4 align=left><font class="regular">Broker Information</font></th>
							</tr>
							<tr>
								<td colspan=2 align=left valign=top><font class="regular"><b>Broker:</b>
									<asp:Label
										ID="lblBroker"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left><font class="regular"><b>Effective Group:</b>
									<asp:Label
										ID="lblGroup"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=2 align=left valign=top><font class="regular"><b>Broker Assigned ID:</b>
									<asp:Label
										ID="lblBrokerJobId"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left><font class="regular"><b>Job Priority:</b>
									<asp:Label
										ID="lblPriority"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=4 align=left valign=top><font class="regular"><b>Experiment Data Retrieved by Broker:</b>
									<asp:Label
										ID="lblDownloaded"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=4><p>&nbsp;</p></td>
							</tr>
							<tr bgcolor="#e0e0e0">
								<td colspan=6 align=left><font class="regular"><b>Execution Notes:</b></font></td>
							</tr>
							<tr>
								<td colspan=2 align=left valign=top><font class="regular"><b>Job Status:</b>
									<asp:Label
										ID="lblStatus"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left valign=top><font class="regular"><b>DataPoints:</b>
									<asp:Label
										ID="lblDatapoints"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=2 align=left valign=top><font class="regular"><b>Submit Time:</b>
									<asp:Label
										ID="lblSubmitTime"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left><font class="regular"><b>Elapsed Exec. Time (estimate):</b>
									<asp:Label
										ID="lblEstExecElapsed"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=2 align=left valign=top><font class="regular"><b>Execution Time:</b>
									<asp:Label
										ID="lblExecTime"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left><font class="regular"><b>Elapsed Exec. Time (actual):</b>
									<asp:Label
										ID="lblActExecElapsed"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=2 align=left valign=top><font class="regular"><b>Completion Time:</b>
									<asp:Label
										ID="lblEndTime"
										Runat="Server" /></font>
								</td>
								<td colspan=2 align=left><font class="regular"><b>Elapsed Job Time:</b>
									<asp:Label
										ID="lblJobElapsed"
										Runat="Server" /></font>
								</td>
							</tr>
							<%
							If UCase(lblResult.Text) = "ERROR" Then
							%>
							<tr>
								<td colspan=4 align=left><font class="regular"><b>Error Report:</b></font></td>
							</tr>
							
							<tr>
								<td colspan=4 align=left><font class="regular">
									<asp:TextBox
										ID="txtErrorRpt"
										TextMode="MultiLine"
										Wrap="True"
										Columns="65"
										Rows="5"
										ReadOnly = "True"
										Runat="Server" /></font>
								</td>
							</tr>
							<%
							End If
							%>
							<tr>
								<td colspan=4><p>&nbsp;</p></td>
							</tr>
							<tr bgcolor="#e0e0e0">
								<td colspan=4 align=left><font class="regular"><b>Experiment Description Objects:</b></font></td>
							</tr>
							<tr>
								<td colspan=4 align=left><font class="regular"><b>Experiment Specification:</b></font></td>
							</tr>
							
							<tr>
								<td colspan=4 align=left><font class="regular">
									<asp:TextBox
										ID="txtExpSpec"
										TextMode="MultiLine"
										Wrap="True"
										Columns="65"
										Rows="5"
										ReadOnly = "True"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=4><p>&nbsp;</p></td>
							</tr>
							<tr>
								<td colspan=4 align=left><font class="regular"><b>Lab Configuration at Execution:</b></font></td>
							</tr>
							
							<tr>
								<td colspan=4 align=left><font class="regular">
									<asp:TextBox
										ID="txtLabConfig"
										TextMode="MultiLine"
										Wrap="True"
										Columns="65"
										Rows="5"
										ReadOnly = "True"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=4><p>&nbsp;</p></td>
							</tr>
							<tr>
								<td colspan=4 align=left><font class="regular"><b>Experiment Results:</b></font></td>
							</tr>
							
							<tr>
								<td colspan=4 align=left><font class="regular">
									<asp:TextBox
										ID="txtExpResults"
										TextMode="MultiLine"
										Wrap="True"
										Columns="65"
										Rows="5"
										ReadOnly = "True"
										Runat="Server" /></font>
								</td>
							</tr>
							
							
																	
							<tr bgcolor="#e0e0e0">
								<td align=center colspan=6>
									<font class="regular">
										<asp:LinkButton
											Text="Back"
											OnClick="ShowList_Click"
											Runat="Server" />
									</font>
								</td>
								
							</tr>
						</table>
					
					
					
						</center>
						<input type="hidden" name="intInterval" value="<%=intInterval%>">
					<%
					End Select
					%>
 				</td>
			</tr>
			<tr>
				<td>
				<center>
					<font class="small">
						<%If blnSRRead Then%>
							<a href="/admin/main.aspx" target="main">Return to Main</a>
						<%Else%>
							<a href="/main.aspx" target="main">Return to Main</a>
						<%End If%>
					</font>
				</center>
				</td>
			</tr>
		</table>
		<input type="hidden" name="intStartIdx" value="<%=intStartIdx%>">
		
    </form>
  <%End If%>
  </body>
</html>
