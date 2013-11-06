<%@ Page Language="VBScript" ValidateRequest="False" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLab.DataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim intStartIdx, intInterval, intEndIdx As Integer
	Dim strStartIdx, strLastIdx As String
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
				
				strDBQuery = "SELECT COUNT(*) FROM LoginRecord;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				
				intEndIdx = CInt(cmdDBQuery.ExecuteScalar())
				
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
			
			dtrDBQuery = GetLogRecords(intStartIdx, intInterval)
				
			rptLoginRecs.dataSource = dtrDBQuery
			rptLoginRecs.DataBind()
			dtrDBQuery.Close()	
			
			If intInterval = 0 Then
				strStartIdx = "1"
				strLastIdx = CStr(intEndIdx)
			Else
				strStartIdx = CStr(intStartIdx)
				strLastIdx = CStr(intStartIdx + intInterval - 1)
			End If
		End If		
		
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	Function GetLogRecords(ByVal intStartIdx AS Integer, ByVal intInterval As Integer) As SqlDataReader
		'function dedicated to calling record filter procedure on DB
		Dim strDBQuery As String
		Dim cmdDBQuery As SqlCommand
		
		strDBQuery = "EXEC rm_ReturnLoginLogSubset @StartIdx, @Interval;"
		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
		cmdDBQuery.Parameters.AddWithValue("@StartIdx", intStartIdx)
		cmdDBQuery.Parameters.AddWithValue("@Interval", intInterval)
		
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
					Administration Site Login Log:
					</font>
					<font class="extra-small">
					&nbsp;Records <b><%=strStartIdx%></b> through <b><%=Math.Min(Cint(strLastIdx), intEndIdx)%></b> out of <b><%=intEndIdx%></b>
					</font>
					<hr size=1>
					<p>
					<center>
					<table border=0 cellpadding=3 cellspacing=0 width=100%>
						<tr bgcolor="#e0e0e0">
							<th align=left width=12%><font class="regular">User</font></th>
							<th align=left><font class="regular">Login Method</font></th>
							<th align=left><font class="regular">Remote Address</font></th>
							<th align=left width=35%><font class="regular">Client Platform</font></th>
							<th align=left><font class="regular">Login Time</font></th>
						</tr>
						<asp:Repeater
							ID="rptLoginRecs"
							Runat="Server">
							<ItemTemplate>
								<tr>
									<td valign="top"><font class="regular"><%#CheckForDBNull(Container.DataItem("username"))%></font></td>
									<td valign="top"><font class="regular"><%#StrConv(Container.DataItem("login_type"), VbStrConv.ProperCase)%></font></td>
									<td valign="top"><font class="regular"><%#Container.DataItem("remote_ip")%></font></td>
									<td valign="top"><font class="extra-small"><%#Container.DataItem("user_agent")%></font></td>
									<td valign="top"><font class="regular"><%#DisplayDate(Container.DataItem("login_time"))%>&nbsp;<%#DisplayTime(Container.DataItem("login_time"))%></font></td>
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
