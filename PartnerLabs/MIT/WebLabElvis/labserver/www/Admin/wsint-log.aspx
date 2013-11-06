<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLab.DataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
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
		
		'load user permission set for this page
		blnSRRead = False
		
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			
			If blnSRRead Then
		
				strDBQuery = "SELECT COUNT(*) FROM WebMethodRequestLog;"
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
					
				rptWSIntRecs.dataSource = dtrDBQuery
				rptWSIntRecs.DataBind()
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
		
		strDBQuery = "EXEC rm_ReturnWSIntLogSubset @StartIdx, @Interval;"
		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
		cmdDBQuery.Parameters.AddWithValue("@StartIdx", intStartIdx)
		cmdDBQuery.Parameters.AddWithValue("@Interval", intInterval)
		
		Return cmdDBQuery.ExecuteReader()	
	End Function
	
	Function DisplayDate(ByVal strDateVal As String) As String
		'takes as input a datetime literal and returns a display friendly version of the date
		Dim strOutput As String = ""
		
		If Not strDateVal Is Nothing Then
			strOutput = MonthName(Month(strDateVal), True) & ". " & Day(strDateVal) & ", " & Year(strDateVal)
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
			
			strDBQuery = "SELECT type, source_name, dest_name, method_name, out_dest_URL, in_has_permission, completion_status, transaction_time FROM WebMethodRequestLog WHERE request_id = @ReqID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@ReqID", strDetailID)
			
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			If dtrDBQuery.Read() Then
				lblMethod.Text = dtrDBQuery("method_name")
				lblType.Text = dtrDBQuery("type")
				lblDate.Text = DisplayDate(dtrDBQuery("transaction_time")) & " " & DisplayTime(dtrDBQuery("transaction_time"))
				lblSource.Text = dtrDBQuery("source_name")
				lblDest.Text = dtrDBQuery("dest_name")
				txtNotes.Text = dtrDBQuery("completion_status")
			
				If lblType.Text = "INCOMING" Then
					lblSourcePerm.Text = dtrDBQuery("in_has_permission")
				Else If lblType.Text = "OUTGOING" Then
					txtDestURL.Text = dtrDBQuery("out_dest_URL")
				End If
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
	
	'Sub Interval_SelectedIndexChange(s As Object, e as EventArgs)
		'response.write(dropInterval.SelectedItem.Value)
		
	'	intInterval = dropInterval.SelectedItem.Value
		
		'dtrDBQuery = GetLogRecords(intStartIdx, intInterval)
			
		'rptWSIntRecs.dataSource = dtrDBQuery
		'rptWSIntRecs.DataBind()	
	'End Sub
	
	
	
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
					Web Method Interface Traffic Log:
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
								<th align=left><font class="regular">Method</font></th><th align=left><font class="regular">Date</font></th><th align=left><font class="regular">Time</font></th><th align=left><font class="regular">Source</font></th><th align=left><font class="regular">Destination</font></th><th align=left><font class="regular">Success</font></th>
							</tr>
							<asp:Repeater
								ID="rptWSIntRecs"
								Runat="Server">
								<ItemTemplate>
									<tr>
										<td><font class="regular"><asp:LinkButton Text='<%#Container.DataItem("method_name")%>' CommandArgument='<%#Container.DataItem("request_id")%>' OnCommand="ShowDetails_Command" Runat="Server" /></font></td><td><font class="regular"><%#DisplayDate(Container.DataItem("transaction_time"))%></font></td><td><font class="regular"><%#DisplayTime(Container.DataItem("transaction_time"))%></font></td><td><font class="regular"><%#Container.DataItem("source_name")%></font></td><td><font class="regular"><%#Container.DataItem("dest_name")%></font></td><td><font class="regular"><%#Container.DataItem("completed_successfully")%></font></td>
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
						'transaction details display code
					%>
						<font class="extra-small">
						&nbsp;<b>Transaction Details</b>
						</font>
						<hr size=1>

						<center>
						<table border=0 cellpadding=3 cellspacing=0 width=100%>
							<tr>
								<td width=16%><font class="extra-small">&nbsp;</font></td>
								<td width=17%><font class="extra-small">&nbsp;</font></td>
								<td width=16%><font class="extra-small">&nbsp;</font></td>
								<td width=17%><font class="extra-small">&nbsp;</font></td>
								<td width=17%><font class="extra-small">&nbsp;</font></td>
								<td width=17%><font class="extra-small">&nbsp;</font></td>
							</tr>
							<tr bgcolor="#e0e0e0">
								<th colspan=6 align=left><font class="regular">General Information</font></th>
							</tr>
							<tr>
								<td colspan=2 align=left><font class="regular"><b>Method:</b>
									<asp:Label
										ID="lblMethod"
										Runat="Server" /></font>
								</td>												
								<td colspan=2 align=center><font class="regular"><b>Type:</b>
									<asp:Label
										ID="lblType"
										Runat="Server" /></font>
								</td>												
								<td colspan=2 align=right><font class="regular"><b>Date:</b>
									<asp:Label
										ID="lblDate"
										Runat="Server" /></font>
								</td>
							</tr>
							<tr>
								<td colspan=3 align=left><font class="regular"><b>Source:</b>
									<asp:Label
										ID="lblSource"
										Runat="Server" /></font>
								</td>
								<%
								If UCase(lblType.Text) = "INCOMING" Then
								%>
								<td colspan=3 align=left><font class="regular"><b>Caller Has Appropriate Permissions:</b>
									<asp:Label
										ID="lblSourcePerm"
										Runat="Server" /></font>
								</td>
								<%Else%>
								<td colspan=3>&nbsp;</td>
								<%End If%>
							</tr>
							<tr>
								<td colspan=3 align=left valign=top><font class="regular"><b>Destination:</b>
									<asp:Label
										ID="lblDest"
										Runat="Server" /></font>
								</td>
								<%
								If UCase(lblType.Text) = "OUTGOING" Then
								%>
								<td colspan=3 align=left><font class="regular"><b>Remote Interface Location:</b><br>
									<asp:TextBox
										ID="txtDestURL"
										Runat="Server" /></font>
								</td>
								<%Else%>
								<td colspan=3>&nbsp;</td>
								<%End If%>
							</tr>
							<tr>
								<td colspan=6 align=left><font class="regular"><b>Execution Notes:</b></font></td>
							</tr>
							<tr>
								<td colspan=6 align=left><font class="regular">
									<asp:TextBox
										ID="txtNotes"
										TextMode="MultiLine"
										Wrap="True"
										Columns="65"
										Rows="5"
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
							<a href="main.aspx" target="main">Return to Main</a>
						<%Else%>
							<a href="../main.aspx" target="main">Return to Main</a>
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
