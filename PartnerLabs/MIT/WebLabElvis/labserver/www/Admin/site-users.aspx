<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers.WebLabDataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery As String
	Dim loopIdx As Integer
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim dstClasses As DataSet
	Dim dadClasses As SqlDataAdapter
	Dim strPageState, strErrorMsg, strUserID, strClassID, strClassName As String
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnAMRead, blnAMEdit, blnAMDelete, blnACRead, blnACGrant As Boolean
	
	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		
		blnAMRead = False
		'load user permission set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			blnAMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AcctManagement", "canview")
			blnAMEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AcctManagement", "canedit")
			blnAMDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AcctManagement", "candelete")
			blnACRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "canview")
			blnACGrant = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "cangrant")
			
			If blnAMRead Then

				If Not Request.QueryString("uid") Is Nothing And IsNumeric(Request.QueryString("uid")) Then
					strUserID = Request.QueryString("uid")
					strPageState = "EDIT"
					
				ElseIf not Page.IsPostBack then
					strPageState = "LIST"
				End If
			End If
		End If
	End Sub

	Sub Page_PreRender
		If blnAMRead Then
			'write info into display fields
			
			Select strPageState
				Case "LIST" 
					strDBQuery = "SELECT user_id, first_name, last_name, email, username, is_active, date_modified FROM SiteUsers;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptUsers.DataSource = cmdDBQuery.ExecuteReader()
					rptUsers.DataBind()
				
				Case "EDIT"
					ddlEditOCList.Items.Clear()
					
					strDBQuery = "SELECT u.user_id, u.first_name, u.last_name, u.email, u.username, u.password, u.class_id, c.name AS class_name, u.is_active, u.date_created, u.date_modified FROM SiteUsers u JOIN UsageClasses c ON u.class_id = c.class_id WHERE u.user_id = @UserID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@UserID", strUserID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() Then
						lblEditTitle.Text = dtrDBQuery("first_name") & " " & dtrDBQuery("last_name")
						txtEditFName.Text = dtrDBQuery("first_name")
						txtEditLName.Text = dtrDBQuery("last_name")
						txtEditUName.Text = dtrDBQuery("username")
						txtEditEmail.Text = dtrDBQuery("email")
						
						If CBool(dtrDBQuery("is_active")) Then
							ddlEditStatus.SelectedIndex = 0
						Else
							ddlEditStatus.SelectedIndex = 1
						End If
						
						strClassID = dtrDBQuery("class_id")
						strClassName = dtrDBQuery("class_name")
						lbEditUpdate.CommandArgument = dtrDBQuery("user_id")
						lbEditDelete.CommandArgument = dtrDBQuery("user_id")
						lblEditDateCreated.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblEditDateMod.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))	
					End If
					dtrDBQuery.Close()
					
					strDBQuery = "SELECT 0 As class_id, 'Select New Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses WHERE NOT class_id = @ClassID ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
				
					Do While dtrDBQuery.Read()
						ddlEditOCList.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("class_id")))
					Loop 
					dtrDBQuery.Close()
					
				
				Case "ADD"
					ddlAddClass.Items.Clear()
					
					strDBQuery = "SELECT 0 As class_id, 'Select Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					Do While dtrDBQuery.Read()
						ddlAddClass.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("class_id")))	
					Loop
					dtrDBQuery.Close()
					
			End Select
			
		End If
			
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	Function DisplayDate(ByVal strDateVal As String) As String
		'takes as input a datetime literal and returns a display friendly version of the date
		Dim strOutput As String = ""
		
		If Not strDateVal Is Nothing Then
			strOutput = MonthName(Month(strDateVal), True) & " " & Day(strDateVal) & ", " & Year(strDateVal)
		End If
		
		Return strOutput		
	End Function
	
	Function DisplayTime(ByVal strDateVal As String) As String
		'takes as input a datetime literal and returns a display friendly version of the time
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
	
	Function DisplayCheck(ByVal objCheck As Object) As String
		'this method takes as input a database bit value.  If the value is a DBNull or "False", then the string "False" is returned.  
		'If the value is "True", then the string "True" is returned.  All other inputs generate "False"
		Dim strOutput As String

		If objCheck Is DBNull.Value Then
			strOutput = "False"
		ElseIf CStr(objCheck) = "False" Or CStr(objCheck) = "True" Then
			strOutput = CStr(objCheck)
		Else
			strOutput = "False"
		End If
		
		Return strOutput
	End Function

	Function DBNullFilter(ByVal objData As Object, ByVal objAlternate As Object) As Object
		'this function checks if the input is a DBNull value.  If not, the object is passed, else objAlternate is returned
		If objData Is DBNull.Value Then
			Return objAlternate
		Else
			Return objData
		End If
	End Function
	
	Function DisplayStatus(ByVal objData As Object) As String
		'this function checks the input value.  If it is boolean true, the string "Active" is returned.  Else "Inactive" is returned.
		If objData Is DBNull.Value Then
			Return "Inactive"
		ElseIf CBool(objData) Then
			Return "Active"
		Else
			Return "Inactive"
		End If
	End Function
	
	Function ValueCheck(ByVal objVal As Object, ByVal strRefVal As String, ByVal strPosResponse As String, ByVal strNegResponse As String) As String
		'this function checks objVal for string equality to a reference value (strRefVal).  If they are equal, strPosResponse is returned,
		'else strNegResponse is returned.
		If objVal Is DBNull.Value Then
			Return strNegResponse
		ElseIf CStr(objVal) = strRefVal Then
			Return strPosResponse
		Else	
			Return strNegResponse
		End If
	End Function
	
	Sub UpdateUser_Command(s As Object, e As CommandEventArgs)
		'updates site user information.  triggered from the "edit" pagestate
		'all information except class membership is validated and updated directly by this method.
		'The specified user's password will be updated IFF the new password field is not empty and matches
		'the entry in the confirm password field.  Class membership will be updated via a call to a 
		'Resource Permission Manager method.
		If blnAMEdit Then
			Dim strResult as String
			strPageState = "EDIT"
			strUserID = e.CommandArgument
			lblErrorOnEditMsg.Text = ""
			
			'validate inputs
			If Trim(txtEditFName.Text) = "" Then
				lblErrorOnEditMsg.Text = "Error Updating Site User Account: A First Name must be provided."
				Exit Sub
			End If	
			
			If Trim(txtEditLName.Text) = "" Then
				lblErrorOnEditMsg.Text = "Error Updating Site User Account: A Last Name must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditEmail.Text) = "" Then
				lblErrorOnEditMsg.Text = "Error Updating Site User Account: An Email address must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditUName.Text) = "" Then
				lblErrorOnEditMsg.Text = "Error Updating Site User Account: A username must be provided."
				Exit Sub
			Else
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT user_id FROM SiteUsers WHERE username = @UName AND NOT useR_id = @UserID);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@UName", txtEditUName.Text)
				cmdDBQuery.Parameters.Add("@UserID", strUserID)
				
				If cmdDBQuery.ExecuteScalar() = "true" Then
					lblErrorOnEditMsg.Text = "Error Updating Site User Account: The specified username is already in use.  Please select another."
					Exit Sub
				End If
			End If
			
			If Trim(txtEditNewPass.Text) = "" And Trim(txtEditPassConf.Text) = "" Then
				'process update without pwd change
				strDBQuery = "UPDATE SiteUsers SET first_name = @FName, last_name = @LName, email = @Email, username = @UName, is_active = @Status, date_modified = GETDATE() WHERE user_id = @UserID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@FName", txtEditFName.Text)
				cmdDBQuery.Parameters.Add("@LName", txtEditLName.Text)
				cmdDBQuery.Parameters.Add("@Email", txtEditEmail.Text)
				cmdDBQuery.Parameters.Add("@UName", txtEditUName.Text)
				cmdDBQuery.Parameters.Add("@Status", ddlEditStatus.SelectedItem.Value)
				cmdDBQuery.Parameters.Add("@UserID", strUserID)
				
				Try
					cmdDBQuery.ExecuteNonQuery()
				Catch
					lblErrorOnEditMsg.Text = "Page Error While Updating Site User Account.  Aborting."
					Exit Sub
				End Try
				
			ElseIf Trim(txtEditNewPass.Text) = Trim(txtEditPassConf.Text) Then
				'process update with pwd change
				strDBQuery = "UPDATE SiteUsers SET first_name = @FName, last_name = @LName, email = @Email, username = @UName, password = @Pass, is_active = @Status, date_modified = GETDATE() WHERE user_id = @UserID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@FName", txtEditFName.Text)
				cmdDBQuery.Parameters.Add("@LName", txtEditLName.Text)
				cmdDBQuery.Parameters.Add("@Email", txtEditEmail.Text)
				cmdDBQuery.Parameters.Add("@UName", txtEditUName.Text)
				cmdDBQuery.Parameters.Add("@Pass", txtEditNewPass.Text)
				cmdDBQuery.Parameters.Add("@Status", ddlEditStatus.SelectedItem.Value)
				cmdDBQuery.Parameters.Add("@UserID", strUserID)
				
				Try
					cmdDBQuery.ExecuteNonQuery()
				Catch
					lblErrorOnEditMsg.Text = "Page Error While Updating Site User Account.  Aborting."
					Exit Sub
				End Try
			Else
				'password mismatch error case
				lblErrorOnEditMsg.Text = "Error Updating Site User Account: Password fields do not match, please try again."
				Exit Sub
			End If 
			
			If blnACGrant Then
				'perform class reassignment, if necessary
				If ddlEditOCList.SelectedItem.Value <> "0" Then
					strResult = rpmObject.MapSiteUserToClass(CInt(strUserID), CInt(ddlEditOCList.SelectedItem.Value))
					
					If strResult <> "Mapping successfully updated." Then
						lblErrorOnEditMsg.Text = "Error Updating Class Membership: " & strResult
					End If
				End If
			End If
		Else
			strPageState = "LIST"
		End If
	End Sub
	
	Sub RemoveUser_Command(s As Object, e As CommandEventArgs)
		'removes the specified site user from the system.  Triggered from the "edit" pagestate.
		'The procedure is performed by a Resource Permission Manager Method
		If blnAMDelete Then
			Dim strResult As String
						
			strResult = rpmObject.RemoveSiteUser(CInt(e.CommandArgument))
			
			If strResult <> "Site User successfully deleted." Then
				lblErrorOnEditMsg.Text = "Error Removing Site User Account: " & strResult
				strPageState = "EDIT"
				strUserID = e.CommandArgument
			Else
				strPageState = "LIST"
			End If
		Else
			strPageState = "LIST"
		End If
		
	End Sub
	
	Sub CreateUser_Click(s As Object, e As EventArgs)
		'creates a new site user.  Triggered from the "add" pagestate.  The inputs are checked to be not-null here.
		'Password confirmation is also performed locally.  Further checks and command execution are handled by a resource permission manager method
		If blnAMEdit Then
			Dim strResult As String
			Dim intNewClassID As Integer
			lblErrorOnAddMsg.Text = ""
			
			'validate inputs
			If Trim(txtAddFName.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: A First Name must be provided."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If Trim(txtAddLName.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: A Last Name must be provided."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If Trim(txtAddUName.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: A username must be provided."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If Trim(txtAddEmail.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: A Email address must be provided."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If Trim(txtAddPass.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: A Password must be provided."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If txtAddPass.Text <> txtAddPassConf.Text Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: Password fields do not match, please try again."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If blnACGrant Then
				If ddlAddClass.SelectedItem.Value = "0" Then
					lblErrorOnAddMsg.Text = "Error Creating Site User Account: A Usage Class must be selected."
					strPageState = "ADD"
					Exit Sub
				Else
					intNewClassID = CInt(ddlAddClass.SelectedItem.Value)
				End If
			Else
				intNewClassID = 2 'automatically assign to the default Guest class
			End If
				
			
			'inputs validated, perform create
			strResult = rpmObject.AddSiteUser(txtAddFName.Text, txtAddLName.Text, txtAddEmail.Text, txtAddUName.Text, txtAddPass.Text, intNewClassID, True)
			
			If Not IsNumeric(strResult) Then
				lblErrorOnAddMsg.Text = "Error Creating Site User Account: " & strResult
				strPageState = "ADD"
			Else
				strUserID = strResult
				strPageState = "EDIT"
			End If		
		Else
			strPageState = "LIST"
		End If
	End Sub
	
	Sub ShowEdit_Command(s As Object, e As CommandEventArgs)
		strUserID = e.CommandArgument
		strPageState = "EDIT"
		lblErrorOnEditMsg.Text = ""
	End Sub
		
	Sub ShowList_Click(s As Object, e As EventArgs)
		strPageState = "LIST"
	End Sub
	
	Sub ShowAdd_Click(s As Object, e As EventArgs)
		strPageState = "ADD"
		lblErrorOnAddMsg.Text = ""
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnAMRead Then  %>
    <form EncType="multipart/form-data" Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					Site Users:
					</font>
					<%
					Select Case strPageState
						Case "LIST"
						'list display code
					%>
					<font class="extra-small">
						<b></b>
					</font>
					
						<hr size=1>
						<div align="right" class="extra-small">
							<%If blnAMEdit Then%>
								<asp:LinkButton
									Text="Create New User"
									onClick="ShowAdd_Click"
									Runat="Server" />
								|
							<%End If%>
							<%If blnSRRead Then%>
								<a href="/labserver/admin/main.aspx" target="main">Return to Main</a>
							<%Else%>
								<a href="/labserver/main.aspx" target="main">Return to Main</a>
							<%End If%>
						</div>
						<p>
							<asp:Repeater
								ID="rptUsers"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">User</font>
											</th>
											<th align="left">
												<font class="regular">Username</font>
											</th>
											<th align="left">
												<font class="regular">Status</font>
											</th>
											<th align="left">
												<font class="regular">Date Modified</font>
											</th>
										</tr>
								</HeaderTemplate>
								
								<ItemTemplate>
										<tr>
											<td>
												<font class="regular">
													<asp:LinkButton 
														Text='<%#Container.DataItem("first_name") & " " & Container.DataItem("last_name")%>'
														CommandArgument='<%#Container.DataItem("user_id")%>'
														OnCommand="ShowEdit_Command"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular"> 
													<a href="mailto:<%#Container.DataItem("email")%>" target="main"><%#Container.DataItem("username")%></a>
												</font>
											</td>
											<td>
												<font class="regular"> 
													<%#DisplayStatus(Container.DataItem("is_active"))%>
												</font>
											</td>
											<td>
												<font class="regular">
													<%#DisplayDate(Container.DataItem("date_modified"))%>&nbsp;<%#DisplayTime(Container.DataItem("date_modified"))%>
												</font>
											</td>
										</tr>
								</ItemTemplate>
								
								<SeparatorTemplate>
									
								</SeparatorTemplate>
								
								<FooterTemplate>
									</table>
									</center>
								</FooterTemplate>
		
							</asp:Repeater>
						

					<%
						CASE "EDIT"
						'Site User information edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEditTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Users"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnAMEdit Then %>
									<asp:LinkButton
										Text="Create New User"
										onClick="ShowAdd_Click"
										Runat="Server" />
									|
								<%End If%>
								<%If blnSRRead Then%>
									<a href="/labserver/admin/main.aspx" target="main">Return to Main</a>
								<%Else%>
									<a href="/labserver/main.aspx" target="main">Return to Main</a>
								<%End If%>
							</div>
						<p>
						<center>

						<table border=0 cellpadding=3 cellspacing=0 width=100%>
							<tr bgcolor="#e0e0e0">
								<td align=left>
									<font class="regular"><b>General Information</b></font>
								</td>
							</tr>
							<tr>
								<td>
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td>
												<font class="regular">
													<b>First Name:</b>
													<asp:TextBox
														ID="txtEditFName"
														Columns="30"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Status:</b>
													<asp:DropDownList
														ID="ddlEditStatus"
														Runat="Server">
														<asp:ListItem
															Text="Active"
															Value="1" />
														<asp:ListItem
															Text="Inactive"
															Value="0" />
													</asp:DropDownList>
												</font>
											</td>
										</tr>
										<tr>
											<td> 
												<font class="regular">
													<b>Last Name:</b>
													<asp:TextBox
														ID="txtEditLName"
														Columns="30"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Username:</b>
													<asp:TextBox
														ID="txtEditUName"
														Columns="30"
														MaxLength="20"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2 align=left>
												<font class="regular">
													<b>Email Address:</b><br>
													<asp:TextBox
														ID="txtEditEmail"
														Columns="30"
														MaxLength="150"
														Runat="Server" />
													<br>&nbsp;
												</font>
											</td>											
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Class Membership:</b>
													<%If blnACRead Then%>
														<a href="/labserver/admin/usage-classes.aspx?cid=<%=strClassID%>" target="main"><%=strClassName%></a>
													<%Else
														Response.Write(strClassName)
													End If%>
												</font>
											</td>
											<td>
												<font class="regular">
													<b>New Password:</b>
													<asp:TextBox
														ID="txtEditNewPass"
														TextMode="Password"
														Columns="15"
														MaxLength="20"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td> 
												<font class="regular">
													<%If blnACGrant Then%>
														<b>Reassign to:</b>
														<asp:DropDownList
															ID="ddlEditOCList"
															Runat="Server" />
													<%Else%>
														&nbsp;
													<%End If%>
												</font>
												
											</td>
											<td>
												<font class="regular">
													<b>Confirm Password:</b>
													<asp:TextBox
														ID="txtEditPassConf"
														TextMode="Password"
														Columns="15"
														MaxLength="20"
														Runat="Server" />
												</font>
											</td>
										</tr>
										
										<tr>
											<td colspan=2>
												<font class="regular">
													<asp:Label 
														ID="lblErrorOnEditMsg"
														ForeColor="Red"
														Runat="Server" />
													<p>
													<center>
														<%If blnAMEdit Then 'since edit is prereq for delete%>
															<asp:LinkButton 
																ID="lbEditUpdate"
																Text="Update Information"
																onCommand="UpdateUser_Command"
																Runat="Server" />
															<%If blnAMDelete Then%>
																|
																<asp:LinkButton 
																	ID="lbEditDelete"
																	Text="Remove User"
																	onCommand="RemoveUser_Command"
																	Runat="Server" />
															<%End If%>
														<%End If%>
													</center>	
												</font>
											</td>
										</tr>
										<tr>
											<td align=center>
												<font class="small">
													<b>Date Created:</b>
													<asp:Label
														ID="lblEditDateCreated"
														Runat="Server" />
												</font>
											</td>
											<td align=center>
												<font class="small">
													<b>Date Modified:</b>
													<asp:Label
														ID="lblEditDateMod"
														Runat="Server" />
												</font>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					
						</center>
					
					<%
						CASE "ADD"
						'site user addition interface code
						If blnAMEdit Then
					%>
						<font class="extra-small">
						&nbsp;<b>Create New User</b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Users"
									onClick="ShowList_Click"
									Runat="Server" />
								|
								<%If blnSRRead Then%>
									<a href="/labserver/admin/main.aspx" target="main">Return to Main</a>
								<%Else%>
									<a href="/labserver/main.aspx" target="main">Return to Main</a>
								<%End If%>
							</div>
						<p>
						<center>
						<table border=0 cellpadding=3 cellspacing=0 width=100%>
							<tr bgcolor="#e0e0e0">
								<th align=left><font class="regular">New Site User Information</font></th>
							</tr>
							<tr>
								<td>
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td>
												<font class="regular">
													<b>First Name:</b><br>
													<asp:TextBox
														ID="txtAddFName"
														Columns="30"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Usage Class:</b>
													<%If blnACGrant Then%>
														<asp:DropDownList
															ID="ddlAddClass"
															Runat="Server" />
													<%Else
														ddlAddClass.Visible = False%>
														Guests
													<%End If%>
												

												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Last Name:</b><br>
													<asp:TextBox
														ID="txtAddLName"
														Columns="30"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Password:</b>
													<asp:TextBox
														ID="txtAddPass"
														TextMode="Password"
														Columns="15"
														MaxLength="20"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Username:</b><br>
													<asp:TextBox
														ID="txtAddUName"
														Columns="30"
														MaxLength="20"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Confirm Password:</b>
													<asp:TextBox
														ID="txtAddPassConf"
														TextMode="Password"
														Columns="15"
														MaxLength="20"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Email Address:</b><br>
													<asp:TextBox
														ID="txtAddEmail"
														Columns="30"
														MaxLength="150"
														Runat="Server" />
												</font>
											</td>
											<td align="center">
												<font class="regular">
													<%If blnAMEdit Then%>
														<asp:LinkButton
															ID="lbAddUser"
															Text="Create User"
															onClick="CreateUser_Click"
															Runat="Server" />
													<%End If%>
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">
												&nbsp;<br>
													<asp:Label 
														ID="lblErrorOnAddMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
											</td>
										</tr>
										
									</table>
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
						<a href="/labserver/admin/main.aspx" target="main">Return to Main</a>
					</font>-->
				</center>
				</td>
			</tr>
		</table>
    </form>
  <%End If%>
  </body>
</html>
