<%@ Page Language="VBScript" ValidateRequest="False" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim intStartIdx, intInterval, intEndIdx As Integer
	Dim strStartIdx, strLastIdx As String
	Dim strPageState, strEditID, strErrorMsg As String
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnSCRead, blnSCEdit, blnSCDelete As Boolean
	

	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		
		'load user permission set for this page
		blnSCRead = False
		
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			blnSCRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysConfig", "canview")
			blnSCEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysConfig", "canedit")
			blnSCDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysConfig", "candelete")
		
			If blnSCRead Then
		
				If not Page.IsPostBack then
					strPageState = "LIST"
				End If
			End If
		End If
	End Sub

	Sub Page_PreRender
		If blnSCRead Then
			'write info into display fields
			
			If strPageState = "LIST" Then
				strDBQuery = "SELECT n.notice_id, n.title, n.body, n.date_entered, n.is_displayed, n.author_id, s.username FROM SystemNotices n JOIN SiteUsers s ON n.author_id = s.user_id;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				
				rptSysNotices.dataSource = cmdDBquery.ExecuteReader()
				rptSysNotices.DataBind()	
				
			End If
		End If	
				
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	Function DisplayDate(ByVal strDateVal As String) As String
		'takes as input a datetime literal and returns a display friendly version of the date
		Dim strOutput As String = ""
		
		If Not strDateVal Is Nothing Then
			strOutput = MonthName(Month(strDateVal), False) & " " & Day(strDateVal) & ", " & Year(strDateVal)
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
			'intSec = Second(strDateVal)
			
			If intMin < 10 Then
				strOutput = strOutput & ":0" & intMin
			Else
				strOutput = strOutput & ":" & intMin
			End If
			
			'If intSec < 10 Then
			'	strOutput = strOutput & ":0" & intSec
			'Else
			'	strOutput = strOutput & ":" & intSec
			'End If
			
		End If
		
		Return strOutput		
	End Function
	
	Function DisplayNoticeStatus(ByVal blnIsDisplayed As Boolean) As String
		Dim strOutput As String
		
		If blnIsDisplayed Then
			strOutput = "Displayed"
		Else
			strOutput = "Not Displayed"
		End If

		Return strOutput
	End Function
	
	Function ChangeStatusLinkText(ByVal blnIsDisplayed As Boolean) As String
		Dim strOutput As String
		
		If blnIsDisplayed Then
			strOutput = "Hide"
		Else
			strOutput = "Show"
		End If

		Return strOutput
	End Function
	
	Function ChangeStatusComName(ByVal blnIsDisplayed As Boolean) As String
		Dim strOutput As String
		
		If blnIsDisplayed Then
			strOutput = "HIDE"
		Else
			strOutput = "SHOW"
		End If

		Return strOutput
	End Function
	
	Function TruncateNotice(ByVal strNoticeBody As String) As String
		Dim intCharLimit As Integer
		Dim strOutput As String
		
		'set character limit
		intCharLimit = 330
		
		If Len(strNoticeBody) > intCharLimit Then
			strOutput  = Left(strNoticeBody, intCharLimit) & "..."
		Else
			strOutput = strNoticeBody
		End If
		
		Return strOutput
	End Function
	
	Sub ChangeStatus_Command(s As Object, e As CommandEventArgs)
		'this method changes the display status of a specified notice.
		If blnSCEdit Then
			strPageState = "LIST"
			
			If e.CommandName = "HIDE" Then
				strDBQuery = "UPDATE SystemNotices SET is_displayed = '0' WHERE notice_id = @NoticeID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@NoticeID", e.CommandArgument)
				cmdDBQuery.ExecuteNonQuery()
			ElseIf e.CommandName = "SHOW" Then
				strDBQuery = "UPDATE SystemNotices SET is_displayed = '1' WHERE notice_id = @NoticeID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@NoticeID", e.CommandArgument)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	Sub DeleteNotice_Command(s As Object, e As CommandEventArgs)
		If blnSCDelete Then
			'this method removes the specified notice from the system database
			strPageState = "LIST"
			
			strDBQuery = "DELETE FROM SystemNotices WHERE notice_id = @NoticeID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@NoticeID", e.CommandArgument)
			cmdDBQuery.ExecuteNonQuery()
		End If
	End Sub
	
	Sub UpdateNotice_Command(s As Object, e As CommandEventArgs)
		If blnSCEdit Then
			'this method commits any editions made by the user to the specified notice	
			strPageState = "EDIT"
			strEditID = e.CommandArgument
			strErrorMsg = ""
			
			If Trim(txtBody.Text) = "" Then
				strErrorMsg = "Error - Message body must be a non-empty string.  Submission Aborted."
			Else
				strDBQuery = "UPDATE SystemNotices SET title = @Title, is_displayed = @Displayed, body = @Body WHERE notice_id = @NoticeID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@Title", txtTitle.Text)
				cmdDBQuery.Parameters.AddWithValue("@Displayed", ddlDisplayed.SelectedItem.Value)
				cmdDBQuery.Parameters.AddWithValue("@Body", txtBody.Text)
				cmdDBQuery.Parameters.AddWithValue("@NoticeID", strEditID)
				cmdDBQuery.ExecuteNonQuery()
			End If
			
			lblErrorMsg.Text = strErrorMsg
			
			strDBQuery = "SELECT n.notice_id, n.title, n.body, n.date_entered, n.is_displayed, n.author_id, s.username FROM SystemNotices n JOIN SiteUsers s ON n.author_id = s.user_id WHERE n.notice_id = @NoticeID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@NoticeID", strEditID)
			
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			If dtrDBQuery.Read() Then
				If Not dtrDBQuery("title") Is DBNull.Value Then
					txtTitle.Text = dtrDBQuery("title")
				End If
				
				If CBool(dtrDBQuery("is_displayed")) Then
					ddlDisplayed.SelectedIndex = 0
				Else
					ddlDisplayed.SelectedIndex = 1
				End If
				
				txtBody.Text = dtrDBQuery("body")
				
				If Not dtrDBQuery("username") Is DBNull.Value Then
					lblAuthor.Text = dtrDBQuery("username")
				Else
					lblAuthor.Text = "Unknown User"
				End If
				
				lblDate.Text = DisplayDate(dtrDBQuery("date_entered")) & " " & DisplayTime(dtrDBQuery("date_entered"))
			End If
			
			dtrDBQuery.Close()
			lbUpdate.CommandArgument = strEditID
		End If		
	End Sub
	
	Sub CreateNotice_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			'takes as input the user's message title, body and display status and creates a new record in the SystemNoticed table.
			strErrorMsg = ""
			
			If Trim(txtBodyInput.Text) = "" Then
				strErrorMsg = "Error - Message body must be a non-empty string.  Submission Aborted."
				strPageState = "ADD"
				lblInputErrorMsg.Text = strErrorMsg
			Else
				strDBQuery = "INSERT INTO SystemNotices (title, body, is_displayed, author_id, date_entered) VALUES (@Title, @Body, @IsDisplayed, @AuthorID, GETDATE());"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@Title", txtTitleInput.Text)
				cmdDBQuery.Parameters.AddWithValue("@Body", txtBodyInput.Text)
				cmdDBQuery.Parameters.AddWithValue("@IsDisplayed", ddlDisplayedInput.SelectedItem.Value)
				cmdDBQuery.Parameters.AddWithValue("@AuthorID", Session("LoggedInUserID"))
				
				cmdDBQuery.ExecuteNonQuery()
				strPageState = "LIST"
			End If		
		End If
	End Sub

	Sub ShowEdit_Command(s As Object, e As CommandEventArgs)
		If blnSCEdit Then
			strErrorMsg = ""
			strPageState = "EDIT"
			strEditID = e.CommandArgument
			
			strDBQuery = "SELECT n.notice_id, n.title, n.body, n.date_entered, n.is_displayed, n.author_id, s.username FROM SystemNotices n JOIN SiteUsers s ON n.author_id = s.user_id WHERE n.notice_id = @NoticeID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@NoticeID", strEditID)
			
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			If dtrDBQuery.Read() Then
				If Not dtrDBQuery("title") Is DBNull.Value Then
					txtTitle.Text = dtrDBQuery("title")
				End If
				
				If CBool(dtrDBQuery("is_displayed")) Then
					ddlDisplayed.SelectedIndex = 0
				Else
					ddlDisplayed.SelectedIndex = 1
				End If
				
				txtBody.Text = dtrDBQuery("body")
				
				If Not dtrDBQuery("username") Is DBNull.Value Then
					lblAuthor.Text = dtrDBQuery("username")
				Else
					lblAuthor.Text = "Unknown User"
				End If
				
				lblDate.Text = DisplayDate(dtrDBQuery("date_entered")) & " " & DisplayTime(dtrDBQuery("date_entered"))
			End If
			
			dtrDBQuery.Close()
			
			lblErrorMsg.Text = strErrorMsg
			lbUpdate.CommandArgument = strEditID
		End If	
	End Sub
	
	Sub ShowList_Click(s As Object, e As EventArgs)
		If blnSCRead Then
			strPageState = "LIST"
		End If
	End Sub
	
	Sub ShowAdd_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			strPageState = "ADD"
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnSCRead Then  %>
    <form Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					System Notices:
					</font>
					<%
					Select Case strPageState
						Case "LIST"
						'log list display code
					%>
					<font class="extra-small">
						<b>Current Notices</b>
					</font>
					
						<hr size=1>
						<div align="right" class="extra-small">
							<%If blnSCEdit Then%>
								<asp:LinkButton
									Text="Create New"
									onClick="ShowAdd_Click"
									Runat="Server" />
								|
							<%End If%>
							<%If blnSRread Then%>
								<a href="main.aspx" target="main">Return to Main</a>
							<%Else%>
								<a href="../main.aspx" target="main">Return to Main</a>
							<%End If%>					
							
						</div>
						<p>
							<asp:Repeater
								ID="rptSysNotices"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
								</HeaderTemplate>
								
								<ItemTemplate>
									<tr>
										<td>
											<table cellpadding=0 cellspacing=0 border=0 width=100%>
												<tr>
													<td>
														<font class="regular"><b>Title:</b>
															<%If blnSCEdit Then%>
															<asp:LinkButton
																Text='<%#Container.DataItem("title")%>'
																CommandArgument='<%#Container.DataItem("notice_id")%>'
																OnCommand="ShowEdit_Command" 
																Runat="Server" />	
															<%Else%>
																<u><%#Container.DataItem("title")%></u>
															<%End If%>
														</font>
														
													</td>
													<td align=right>
														<font class="regular">
															<b>Status:</b>
															<%#DisplayNoticeStatus(Cbool(Container.DataItem("is_displayed")))%>
															
														</font>
														<font class="extra-small">
															<%If blnSCEdit Then 'since edit is a prereq for delete%>
																(<asp:LinkButton
																	Text='<%#ChangeStatusLinkText(CBool(Container.DataItem("is_displayed")))%>'
																	CommandName='<%#ChangeStatusComName(CBool(Container.DataItem("is_displayed")))%>'
																	CommandArgument='<%#Container.DataItem("notice_id")%>'
																	OnCommand="ChangeStatus_Command"
																	Runat="Server" /><%If blnSCDelete Then%>
																						|
																						<asp:LinkButton
																							Text="Delete"
																							CommandArgument='<%#Container.DataItem("notice_id")%>'
																							OnCommand="DeleteNotice_Command" 
																							Runat="Server" /><%End If%>)															
															<%End If%>
														</font>
														
													</td>
												</tr>
												<tr>
													<td colspan=2>
														<font class="regular"><b>Body:</b>
														<%#TruncateNotice(Container.DataItem("body"))%>
														
														</font>
													</td>
												</tr>
												<tr>
													<td>
														<font class="regular"><b>Posted By:</b>
															<%#Container.DataItem("username")%>
														</font>
													</td>
													<td align=right>
														<font class="regular"><b>Posted On:</b>
															<%#DisplayDate(Container.DataItem("date_entered"))%>&nbsp;<%#DisplayTime(Container.DataItem("date_entered"))%>
															
														</font>
													</td>
												</tr>
											</table>
										</td>
									</tr>
								</ItemTemplate>
								
								<SeparatorTemplate>
									<tr>
										<td>
											&nbsp;
										</td>
									</tr>
								</SeparatorTemplate>
								
								<FooterTemplate>
									</table>
									</center>
								</FooterTemplate>
		
							</asp:Repeater>
						

					<%
						CASE "EDIT"
						'system notice edit interface code
							If blnSCEdit Then
					%>
								<font class="extra-small">
								&nbsp;<b>Edit Notice</b>
								</font>
								<hr size=1>
									<div align="right" class="extra-small">
										<%If blnSCRead Then %>
											<asp:LinkButton
												Text="View All"
												onClick="ShowList_Click"
												Runat="Server" />
											|
										<%End If%>
										<%If blnSCEdit Then%>
											<asp:LinkButton
												Text="Create New"
												onClick="ShowAdd_Click"
												Runat="Server" />
											|
										<%End If%>
										<%If blnSRRead Then%>
											<a href="main.aspx" target="main">Return to Main</a>
										<%Else%>
											<a href="../main.aspx" target="main">Return to Main</a>
										<%End If%>
										
									</div>
								<p>
								<center>
								<font class="regular">
									<asp:Label 
										ID="lblErrorMsg"
										ForeColor="Red"
										Runat="Server" />
								</font>
								<table border=0 cellpadding=3 cellspacing=0 width=100%>
									<tr>
										<td>
											<table border=0 cellpadding=0 cellspacing=3 width=100%>
												<tr>
													<th align=right>
														<font class="regular">
															Title:	
														</font>
													</th>
													<td>
														<font class="regular">	
														<asp:TextBox
															ID="txtTitle"
															Columns="45"
															MaxLength="75"
															Runat="Server" />
														</font>
													</td>
												</tr>
												<tr>
													<th align=right>
														<font class="regular">
															Displayed:	
														</font>
													</th>
													<td>
														<font class="regular">	
														<asp:DropDownList
															ID="ddlDisplayed"
															Runat="Server">
															
															<asp:ListItem 
																Text="Yes" 
																Value="1" />
															<asp:ListItem 
																Text="No" 
																Value="0" />
														</asp:DropDownList>
														</font>
													</td>
												</tr>
												<tr>
													<th align=right valign=top>
														<font class="regular">
															Body:	
														</font>
													</th>					
													<td>
														<font class="regular">
														<asp:TextBox
															ID="txtBody"
															MaxLength="1500"
															Columns="52"
															Rows="20"
															TextMode="MultiLine"
															Wrap="True"
															Runat="Server" />
														</font>					
												</tr>
											</table>
											
										</td>
									</tr>
									<tr>
										<table border=0 cellspacing=0 cellpadding=0 width=100%>
											<tr>
												<th>
													<font class="regular">Created By:</font>
												</th>
												<td>
													<font class="regular"><asp:Label ID="lblAuthor" Runat="Server" /></font>
												</td>
												<td rowspan=2 align=center valign=middle width=50%>
													<font class="regular">
														<%If blnSCEdit Then%>
															<asp:LinkButton ID="lbUpdate" Text="Update Notice" onCommand="UpdateNotice_Command" Runat="Server" />
														<%Else%>
															&nbsp;
														<%End If%>
													</font>
												</td>
											</tr>
											<tr>
												<th>
													<font class="regular">Created On:</font>
												</th>
												<td>
													<font class="regular"><asp:Label ID="lblDate" Runat="Server" /></font>
												</td>
											</tr>
										</table>
									</tr>
								</table>

								</center>
					<%
							End If
						CASE "ADD"
						'system notice edit interface code
							If blnSCEdit Then
					%>
								<font class="extra-small">
								&nbsp;<b>Create Notice</b>
								</font>
								<hr size=1>
									<div align="right" class="extra-small">
										<%If blnSCRead Then%>
										<asp:LinkButton
											Text="View All"
											onClick="ShowList_Click"
											Runat="Server" />
										|
										<%End If%>
										<%If blnSRRead Then%>
											<a href="main.aspx" target="main">Return to Main</a>
										<%Else%>
											<a href="../main.aspx" target="main">Return to Main</a>
										<%End If%>
									</div>
								<p>
								<center>
								<font class="regular">
									<asp:Label 
										ID="lblInputErrorMsg"
										ForeColor="Red"
										Runat="Server" />
								</font>
								<table border=0 cellpadding=3 cellspacing=0 width=100%>
									<tr>
										<td>
											<table border=0 cellpadding=0 cellspacing=3 width=100%>
												<tr>
													<th align=right>
														<font class="regular">
															Title:	
														</font>
													</th>
													<td>
														<font class="regular">	
														<asp:TextBox
															ID="txtTitleInput"
															Columns="45"
															MaxLength="75"
															Runat="Server" />
														</font>
													</td>
												</tr>
												<tr>
													<th align=right>
														<font class="regular">
															Displayed:	
														</font>
													</th>
													<td>
														<font class="regular">	
														<asp:DropDownList
															ID="ddlDisplayedInput"
															Runat="Server">
															
															<asp:ListItem 
																Text="Yes" 
																Value="1" />
															<asp:ListItem 
																Text="No" 
																Value="0" />
														</asp:DropDownList>
														</font>
													</td>
												</tr>
												<tr>
													<th align=right valign=top>
														<font class="regular">
															Body:	
														</font>
													</th>					
													<td>
														<font class="regular">
														<asp:TextBox
															ID="txtBodyInput"
															MaxLength="1500"
															Columns="52"
															Rows="20"
															TextMode="MultiLine"
															Wrap="True"
															Runat="Server" />
														</font>					
												</tr>
											</table>
											<center>
											<font class="regular">
												<%If blnSCEdit Then%>
													<asp:LinkButton Text="Create Notice" onClick="CreateNotice_Click" Runat="Server" />
												<%Else%>
													&nbsp;
												<%End If%>
											</font>
											</center>
										</td>
									</tr>
								
								</table>

								</center>

					<%
							End If
							
					End Select
					%>
 				</td>
			</tr>
			<tr>
				<td>
				<center>
				<!--	<font class="small">
						<a href="main.aspx" target="main">Return to Main</a>
					</font>-->
				</center>
				</td>
			</tr>
		</table>
    </form>
  <%End If%>
  </body>
</html>
