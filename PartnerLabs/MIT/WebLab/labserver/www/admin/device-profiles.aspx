<%@ Page Language="VBScript"%>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers" %>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->


<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery As String
	Dim blnIsItemChange As Boolean
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim strPageState, strErrorMsg, strTypeID, strTypeName, strProfileID, strResourceID As String
	Dim intTermCT As Integer
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnDMRead, blnDMEdit, blnDMDelete, blnACRead As Boolean
	
	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info

		blnDMRead = False
		'load user permission set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			blnDMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "DeviceManagement", "canview")
			blnDMEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "DeviceManagement", "canedit")
			blnDMDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "DeviceManagement", "candelete")
			blnACRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "canview")
		
			If blnDMRead Then
		
				If Not Request.QueryString("pid") Is Nothing And IsNumeric(Request.QueryString("pid")) Then
					strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT profile_id FROM DeviceProfiles WHERE profile_id = @ProfileID);"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ProfileID", Request.QueryString("pid"))
					
					If cmdDBQuery.ExecuteScalar() = "true" THen
						strPageState = "EDIT"
						strProfileID = Request.QueryString("pid")
					Else
						strPageState = "LIST"
					End If
					
				ElseIf Not Request.QueryString("mode") Is Nothing Then
					If LCase(Request.QueryString("mode")) = "add" Then
						strPageState = "ADD"
					Else
						strPageState = "LIST"
					End If
					
				ElseIf not Page.IsPostBack then
					strPageState = "LIST"
				End If
			End If
		End If
	End Sub

	Sub Page_PreRender
		If blnDMRead Then
			'write info into display fields
			
			Select strPageState
				Case "LIST" 
					strDBQuery = "SELECT p.profile_id, p.resource_id, p.type_id, r.name As profile_name, t.name As type_name, p.date_modified FROM DeviceProfiles p JOIN Resources r ON p.resource_id = r.resource_id JOIN DeviceTypes t ON p.type_id = t.type_id;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptDevProfile.dataSource = cmdDBquery.ExecuteReader()
					rptDevProfile.DataBind()	
				
				Case "EDIT"
					strDBQuery = "SELECT r.resource_id, r.name AS profile_name, r.description, t.type_id, t.name AS type_name, t.terminals_used, p.max_points, p.date_created, p.date_modified FROM DeviceProfiles p JOIN Resources r ON p.resource_id = r.resource_id JOIN DeviceTypes t ON p.type_id = t.type_id WHERE p.profile_id = @ProfileID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ProfileID", strProfileID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()				
				
					
					If dtrDBQuery.Read() Then
						txtEditName.Text = dtrDBQuery("profile_name")
						lblEditTitle.Text = dtrDBQuery("profile_name")
						txtEditDesc.Text = dtrDBQuery("description")
						txtMaxPoints.Text = dtrDBQuery("max_points")
						
						strTypeID = dtrDBQuery("type_id")
						strTypeName = dtrDBQuery("type_name")
						intTermCT = CInt(dtrDbQuery("terminals_used"))
						strResourceID = dtrDBQuery("resource_id")
						
						If intTermCT = 0 Then
							rptDevProfileTerm.Visible = False
						Else
							rptdevProfileTerm.Visible = True
						End If
						
						lblEditDateCr.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblEditDateMod.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
					
						lbUpdateProfile.CommandArgument = strProfileID
						lbUpdateProfile.CommandName = dtrDBQuery("resource_id")
						lbCopyProfile.CommandArgument = strProfileID
						lbCopyProfile.CommandName = dtrDBQuery("profile_name")
						lbDelProfile.CommandArgument = strProfileID
						lbDelProfile.CommandName = dtrDBQuery("resource_id")
						'lbUpdateAll.CommandArgument = strProfileID
						'lbUpdateAll.CommandName = intTermCT
					End If
					
					dtrDBQuery.Close()
					
					strDBQuery = "SELECT termPID, termNumber, termPort, termName, termMaxV, termMaxA FROM dbo.rpm_GetDeviceTerminalInfo(@ProfileID);"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ProfileID", strProfileID)
					rptDevProfileTerm.DataSource = cmdDBQuery.ExecuteReader()
					rptDevProfileTerm.DataBind()
					
					
				Case "ADD"
					If Not blnIsItemChange Then
						ddlAddDevType.Items.Clear() 'reinitiallized control items
						
						strDBQuery = "SELECT type_id, name FROM DeviceTypes;"
						cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
						dtrDBQuery = cmdDBQuery.ExecuteReader()
						
						Do While dtrDBquery.Read()
							ddlAddDevType.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("type_id")))
						Loop
						dtrDBQuery.Close()
					
					End If
					
					'ddlAddDevType.DataSource = cmdDBQuery.ExecuteReader()
					'ddlAddDevType.DataTextField = "name"
					'ddlAddDevType.DataValueField = "type_id"
					'ddlAddDevType.DataBind()
					strDBQuery = "SELECT terminals_used FROM DeviceTypes WHERE type_id = @TypeID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@TypeID", ddlAddDevType.SelectedItem.Value)
					
					If CInt(cmdDBQuery.ExecuteScalar()) = 0 Then
						rptAddProfileTerm.Visible = False
					Else
						rptAddProfileTerm.Visible = True
						
						strDBQuery = "SELECT name, port FROM DeviceTypeTerminalConfig WHERE type_id = @TypeID ORDER BY number;" 			
						cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
						cmdDBQuery.Parameters.Add("@TypeID", ddlAddDevType.SelectedItem.Value)
						
						rptAddProfileTerm.DataSource = cmdDBQuery.ExecuteReader()
						rptAddProfileTerm.DataBind()
					End If
				
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
	
	Sub UpdateProfile_Command(s As Object, e As CommandEventArgs)
		'this method updates the general information associated with the specified profile.
		If blnDMEdit Then
			Dim loopIdx, termCT, intCtrlNum As Integer
			Dim strPIDList, strNumList, strMaxVList, strMaxAList, strNewMVList, strNewMAList As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strProfileID = e.CommandArgument
			
			'check validity of  general info inputs
			If Trim(txtEditName.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Device Profile: Profile Name must be supplied."
				Exit Sub
			ElseIf Trim(txtEditDesc.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Device Profile: Profile Description must be supplied."
				Exit Sub
			ElseIf Trim(txtMaxPoints.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Device Profile: A Measurement Datapoint Limit must be specified."
				Exit Sub
			ElseIf Not IsNumeric(txtMaxPoints.Text) Then
				lblErrorMsg.Text = "Error Updating Device Profile: A numeric value must be supplied for the Measurement Datapoint Limit."
				Exit Sub
			End If
			
			'obtain/check terminal info
			strDBQuery = "SELECT termPID, termNumber, termMaxV, termMaxA FROM dbo.rpm_GetDeviceTerminalInfo(@ProfileID);"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@ProfileID", strProfileID)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			termCT = 0
			strPIDList = ""
			strNumList = ""
			strMaxVList = ""
			strMaxAList = ""
			
			Do While dtrDBQuery.Read()
				strPIDList = strPIDList & ":" & dtrDBQuery("termPID")
				strNumList = strNumList & ":" & dtrDBQuery("termNumber")
				strMaxVList = strMaxVList & ":" & dtrDBQuery("termMaxV")
				strMaxAList = strMaxAList & ":" & dtrDBQuery("termMaxA")
				
				termCT = termCT + 1		
			Loop
			dtrDBQuery.Close()
			
			If termCt > 0 Then
				Dim txtEditTermMaxVRef, txtEditTermMaxARef As TextBox
				
				strPIDList = Right(strPIDList, (Len(strPIDList) - 1))
				strNumList = Right(strNumList, (Len(strNumList) - 1))
				strMaxVList = Right(strMaxVList, (Len(strMaxVList) - 1))
				strMaxAList = Right(strMaxAList, (Len(strMaxAList) - 1))
				
				Dim strPIDArray() As String = Split(strPIDList, ":")
				Dim strNumArray() As String = Split(strNumList, ":")
				Dim strMaxVArray() As String = Split(strMaxVList, ":")
				Dim strMaxAArray() As String = Split(strMaxAList, ":")
				
				strNewMVList = ""
				strNewMAList = ""
				
				For loopIdx = 0 To termCT - 1
					intCtrlNum = (CInt(strNumArray(loopIdx)) * 2) - 1
					
					txtEditTermMaxVRef = FindControl("rptDevProfileTerm:_ctl" & intCtrlNum & ":txtEditTermMaxV")
					txtEditTermMaxARef = FindControl("rptDevProfileTerm:_ctl" & intCtrlNum & ":txtEditTermMaxA")
					
					If Not txtEditTermMaxVRef Is Nothing Then
						If Trim(txtEditTermMaxVRef.Text) = "" Then
							lblErrorMsg.Text = "Error Updating Device Terminal: A maximum voltage must be supplied for all terminals."
							Exit Sub
						ElseIf Not IsNumeric(txtEditTermMaxVRef.Text) Then
							lblErrorMsg.Text = "Error Updating Device Terminal: The maximum voltage must be specified as a numeric value."
							Exit Sub
						End If
					Else
						lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
						Exit Sub
					End If
					
					If Not txtEditTermMaxARef Is Nothing Then
						If Trim(txtEditTermMaxARef.Text) = "" Then
							lblErrorMsg.Text = "Error Updating Device Terminal: A maximum current must be supplied for all terminals."
							Exit Sub
						ElseIf Not IsNumeric(txtEditTermMaxARef.Text) Then
							lblErrorMsg.Text = "Error Updating Device Terminal: The maximum current must be specified as a numeric value."
							Exit Sub
						End If
					Else
						lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
						Exit Sub
					End If
					
					'response.write("Terminal:" & strNumArray(loopIdx) & " - PID:" & strPIDArray(loopIdx) & " - NewV:" & txtEditTermMaxVRef.Text & " - NewA:" & txtEditTermMaxARef.Text)
					
					strNewMVList = strNewMVList & ":" & txtEditTermMaxVRef.Text
					strNewMAList = strNewMAList & ":" & txtEditTermMaxARef.Text
					
				Next
				
				'begin commit of checked values
				'update terminal compliances
				Dim strNewMVArray() As String = Split(Right(strNewMVList, (Len(strNewMVList) - 1)), ":")
				Dim strNewMAArray() As String = Split(Right(strNewMAList, (Len(strNewMAList) - 1)), ":")
				
				For loopIdx = 0 To termCT - 1
					If (strNewMVArray(loopIdx) <> strMaxVArray(loopIdx)) Or (strNewMAArray(loopIdx) <> strMaxAArray(loopIdx)) Then
						strDbQuery = "UPDATE DeviceProfileTerminalConfig SET max_voltage = @MaxVolts, max_current = @MaxAmps, date_modified = GETDATE() WHERE profileterm_id = @PTermID;"
						cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
						cmdDBQuery.Parameters.Add("@MaxVolts", strNewMVArray(loopIdx))
						cmdDBQuery.Parameters.Add("@MaxAmps", strNewMAArray(loopIdx))
						cmdDBQuery.Parameters.Add("@PTermID", strPIDArray(loopIdx))
						cmdDBQuery.ExecuteNonQuery()
					
					End If
				Next
				
			End If
			
			'update resource listing
			strDBQuery = "UPDATE Resources SET name = @Name, description = @Desc, date_modified = GETDATE() WHERE resource_id = @ResourceID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtEditName.Text)
			cmdDBQuery.Parameters.Add("@Desc", txtEditDesc.Text)
			cmdDBQuery.Parameters.Add("@ResourceID", e.CommandName)
			cmdDBQuery.ExecuteNonQuery()
			
			'update profile listing
			strDBQuery = "UPDATE DeviceProfiles SET max_points = @MaxPoints, date_modified = GETDATE() WHERE profile_id = @ProfileID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@MaxPoints", txtMaxPoints.Text)
			cmdDBQuery.Parameters.Add("@ProfileID", strProfileID)
			cmdDBQuery.ExecuteNonQuery()
		End if
		
	End Sub

	Sub CopyProfile_Command(s As Object, e As CommandEventArgs)
		'this method creates a new device profile from the one specified.  The copy process is performed
		'by a Resource Permission Manager method.
		If blnDMEdit Then
			Dim strResult, strNewName As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strNewName = "Copy of " & Left(e.CommandName, 242)
		
			strResult = rpmObject.CopyDeviceProfile(CInt(e.CommandArgument), strNewName)
			
			If strResult <> "Device Profile Successfully Copied." Then
				strProfileID = e.CommandArgument
				lblErrorMsg.Text = "Error Copying Device Profile: " & strResult
			Else
				strDBQuery = "SELECT p.profile_id FROM DeviceProfiles p JOIN Resources r ON p.resource_id = r.resource_id WHERE r.name = @CopyName;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@CopyName", strNewName)
				
				strProfileID = cmdDBQuery.ExecuteScalar()
			End If
		End If
	End Sub

	Sub DeleteProfile_Command(s As Object, e As CommandEventArgs)
		'this method removes a device profile, along with any other dependent objects, from the system.  This
		'process is performed by a Resource Permission Manager method.
		If blnDMDelete Then
			Dim strResult As String
			
			lblErrorMsg.Text = ""
			
			strResult = rpmObject.RemoveDeviceProfile(CInt(e.CommandArgument))
			
			If strResult <> "Device Profile Successfully Removed." Then
				lblErrorMsg.Text = "Error Removing Device Profile: " & strResult
				strProfileID = e.CommandArgument
				strPageState = "EDIT"
			Else
				strPageState = "LIST"
			End If
		End If
	End Sub
	
	Sub CreateProfile_Command(s As Object, e As CommandEventArgs)
		'this method creates a new device profile (and requesite support items) in the system.  This process
		'is performed by a Resource Permission Manager method.
		If blnDMEdit Then
			Dim strResult, strMaxVList, strMaxAList As String
			Dim intTermCT, loopIdx, intCtrlNum As Integer
			
			'validate input
			If Trim(txtAddName.Text) = "" Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Profile: A Name must be provided."
				Exit Sub
			End If
			
			If Trim(txtAddDesc.Text) = "" Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Profile: A Description must be provided."
				Exit Sub
			End If
			
			If Trim(txtAddMaxPoints.Text) = "" Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Profile: A Measurement Datapoint Limit must be provided."
				Exit Sub
			ElseIf Not IsNumeric(txtAddMaxPoints.Text) Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Profile: The Measurement Datapoint Limit must be a numeric value."
				Exit Sub
			End If
			
			'retrieve and check compliance information
			strDBQuery = "SELECT terminals_used FROM DeviceTypes WHERE type_id = @TypeID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@TypeID", ddlAddDevType.SelectedItem.Value)
			intTermCT = cmdDBQuery.ExecuteScalar()
			
			strMaxVList = ""
			strMaxAList = ""
			
			If intTermCT > 0 Then
				Dim txtAddTermMaxVRef, txtAddTermMaxARef As TextBox
						
				For loopIdx = 0 To intTermCT - 1
					intCtrlNum = (loopIdx * 2) + 1
					
					txtAddTermMaxVRef = FindControl("rptAddProfileTerm:_ctl" & intCtrlNum & ":txtAddTermMaxV")
					txtAddTermMaxARef = FindControl("rptAddProfileTerm:_ctl" & intCtrlNum & ":txtAddTermMaxA")
					
					If Not txtAddTermMaxVRef Is Nothing Then
						If Trim(txtAddTermMaxVRef.Text) = "" Then
							lblInputErrorMsg.Text = "Error Creating Device Profile: A maximum voltage must be supplied for all terminals."
							Exit Sub
						ElseIf Not IsNumeric(txtAddTermMaxVRef.Text) Then
							lblInputErrorMsg.Text = "Error Creating Device Profile: The maximum voltage must be specified as a numeric value."
							Exit Sub
						End If
					Else
						lblInputErrorMsg.Text = "Page Error While Creating Profile.  Aborting."
						Exit Sub
					End If
					
					If Not txtAddTermMaxARef Is Nothing Then
						If Trim(txtAddTermMaxARef.Text) = "" Then
							lblInputErrorMsg.Text = "Error Creating Device Profile: A maximum current must be supplied for all terminals."
							Exit Sub
						ElseIf Not IsNumeric(txtAddTermMaxARef.Text) Then
							lblInputErrorMsg.Text = "Error Creating Device Profile: The maximum current must be specified as a numeric value."
							Exit Sub
						End If
					Else
						lblInputErrorMsg.Text = "Page Error While Creating Profile.  Aborting."
						Exit Sub
					End If
					
					'response.write("Terminal:" & strNumArray(loopIdx) & " - PID:" & strPIDArray(loopIdx) & " - NewV:" & txtEditTermMaxVRef.Text & " - NewA:" & txtEditTermMaxARef.Text)
					
					strMaxVList = strMaxVList & ":" & txtAddTermMaxVRef.Text
					strMaxAList = strMaxAList & ":" & txtAddTermMaxARef.Text
					
				Next
				
			End If
			
			Dim strMaxVArray() As String = Split(Right(strMaxVList, Math.Max((Len(strMaxVList) - 1), 0)), ":")
			Dim strMaxAArray() As String = Split(Right(strMaxAList, Math.Max((Len(strMaxAList) - 1), 0)), ":")
			
			Dim dblMaxVArray(Math.Max(intTermCT - 1, 0)) As Double
			Dim dblMaxAArray(Math.Max(intTermCT - 1, 0)) As Double
			
			For loopIdx = 0 To intTermCT - 1
				dblMaxVArray(loopIdx) = CDbl(strMaxVArray(loopIdx))
				dblMaxAArray(loopIdx) = CDbl(strMaxAArray(loopIdx))
			Next
			
			'do create
			strResult = rpmObject.AddDeviceProfile(txtAddName.Text, ddlAddDevType.SelectedItem.Value, "Device", txtAddDesc.Text, CInt(txtAddMaxPoints.Text), dblMaxVArray, dblMaxAArray)
					
			If strResult <> "Device Profile Successfully Created." Then	
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Profile: " & strResult
				Exit Sub
			Else
				'create succeeded	
			
				'get profile id of new profile
				strDBQuery = "SELECT p.profile_id FROM DeviceProfiles p JOIN Resources r ON p.resource_id = r.resource_id WHERE r.name = @Name;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@Name", txtAddName.Text)
				
				strProfileID = cmdDBQuery.ExecuteScalar()
				
				lblErrorMsg.Text = ""
				lblInputErrorMsg.Text = ""
				strPageState = "EDIT"
			End If
		End If
	End Sub

	Sub ShowProfile_Command(s As Object, e As CommandEventArgs)
		strPageState="EDIT"
		strProfileID=e.CommandArgument
	End Sub
		
	Sub ShowList_Click(s As Object, e As EventArgs)
		strPageState = "LIST"
	End Sub
	
	Sub ShowAdd_Click(s As Object, e As EventArgs)
		blnIsItemChange = False
		strPageState = "ADD"
	End Sub
	
	Sub TypeChanged_SelectedItemChange(s As Object, e As EventArgs)
		blnIsItemChange = True
		strPageState = "ADD"
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnDMRead Then%>
    <form EncType="multipart/form-data" Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					Device Profile Management:
					</font>
					<%
					Select Case strPageState
						Case "LIST"
						'list display code
					%>
					<font class="extra-small">
						<b>Available Profiles</b>
					</font>
					
						<hr size=1>
						<div align="right" class="extra-small">
							<%If blnDMEdit Then %>
								<asp:LinkButton
									Text="Create New Profile"
									onClick="ShowAdd_Click"
									Runat="Server" />
								|
							<%End If%>
							<%If blnSRRead Then%>
								<a href="/admin/main.aspx" target="main">Return to Main</a>
							<%Else%>
								<a href="/main.aspx" target="main">Return to Main</a>
							<%End If%>
						</div>
						<p>
							<asp:Repeater
								ID="rptDevProfile"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">Profile Name</font>
											</th>
											<th align="left">
												<font class="regular">Type</font>
											</th>
											<th align="left">
												<font class="regular">Date Modified</font>
											</th>
											<%If blnACRead Then%>
												<th align="left">
													<font class="regular">Permissions</font>
												</th>
											<%End If%>
										</tr>
								</HeaderTemplate>
								
								<ItemTemplate>
										<tr>
											<td>
												<font class="regular">
													<asp:LinkButton 
														Text='<%#Container.DataItem("profile_name")%>'
														CommandArgument='<%#Container.DataItem("profile_id")%>'
														OnCommand="ShowProfile_Command"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular"> <!--code hook into device types page-->
													<a href="/admin/device-types.aspx?tid=<%#Container.DataItem("type_id")%>" target="main"><%#Container.DataItem("type_name")%></a>
												</font>
											</td>
											<td>
												<font class="regular">
													<%#DisplayDate(Container.DataItem("date_modified"))%>&nbsp;<%#DisplayTime(Container.DataItem("date_modified"))%>
												</font>
											</td>
											<%If blnACRead Then%>
												<td>
													<font class="regular"> <!--code hook into resources page-->
														<a href="/admin/system-resources.aspx?rid=<%#Container.DataItem("resource_id")%>" target="main">View</a>
													</font>
												</td>
											<%End If%>
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
						'system notice edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEditTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
							
								<asp:LinkButton
									Text="View All Profiles"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnDMEdit Then%>
									<asp:LinkButton
										Text="Create New Profile"
										onClick="ShowAdd_Click"
										Runat="Server" />
									|
								<%End If%>
								<%If blnSRRead Then%>
									<a href="/admin/main.aspx" target="main">Return to Main</a>
								<%Else%>
									<a href="/main.aspx" target="main">Return to Main</a>
								<%End If%>
							</div>
						<p>
						<center>

						<table border=0 cellpadding=3 cellspacing=0 width=100%>
							<tr bgcolor="#e0e0e0">
								<td align=left>
									<font class="regular"><b>General Information</b></font>
									<%If blnACRead Then%>
										<font class="extra-small">(<a href="/admin/system-resources.aspx?rid=<%=strResourceID%>" target="main">View Device Permissions</a>)</font>								
									<%End If%>
								</td>
							</tr>
							<tr>
								<td>
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td>
												<table border=0 cellpadding=0 cellspacing=0 width=100%>
													<tr>
														<td valign="top">
															<font class="regular">
																<b>Profile Name:</b>
																<asp:TextBox
																	ID="txtEditName"
																	Columns="25"
																	MaxLength="100"
																	Runat="Server" />
															</font>
														</td>
														<td align=right>
															<font class="regular">
																<b>Device Type:</b>
																<a href="/admin/device-types.aspx?tid=<%=strTypeID%>" target="main"><%=strTypeName%></a>
																<!--<br>-->
																
															</font>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												&nbsp;
												<table border=0 cellspacing=0 cellpadding=0 width=100%>
													<tr>
														<td valign=top>
															<font class="regular">	
																<b>Description:</b>
															</font>
														</td>
														<td>
															<font class="regular">
																<asp:TextBox 
																	ID="txtEditDesc"													
																	Rows="3"
																	Columns="40"
																	MaxLength="1000"
																	TextMode="MultiLine"
																	Wrap="True"
																	Runat="Server" />
															</font>
														</td>
													</tr>
												</table>
												&nbsp;
											</td>
										</tr>
										
												<asp:Repeater
													ID="rptDevProfileTerm"
													Runat="Server">
													<HeaderTemplate>
														<tr>
															<td>
																<table border=1 cellspacing=0 cellpadding=2 width=90% align=center>
																	<tr>
																		<th>
																			<font class="regular">
																				Terminal
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Max. Voltage (V)
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Max. Current (A)
																			</font>
																		</th>
																	</tr>
													</HeaderTemplate>
													
													<ItemTemplate>
											 				<tr>
																<td>
																	<font class="regular">
																		<%#Container.DataItem("termName")%>&nbsp;(<%#Container.DataItem("termPort")%>)
																	</font>
																</td>
																<td align=center>
																	<font class="regular">
																		+/-
																		<asp:TextBox
																			ID="txtEditTermMaxV"
																			Text='<%#Container.DataItem("termMaxV")%>'
																			Columns="10"
																			MaxLength="12"
																			Runat="Server" />
																	</font>
																</td>
																<td align=center>
																	<font class="regular">
																		+/-
																		<asp:TextBox
																			ID="txtEditTermMaxA"
																			Text='<%#Container.DataItem("termMaxA")%>'
																			Columns="10"
																			MaxLength="11"
																			Runat="Server" />
																	</font>
																</td>
															</tr>
													</ItemTemplate>
													
													<SeparatorTemplate>
													</SeparatorTemplate>
													
													<FooterTemplate>			
																</table>
																&nbsp;
															</td>
														</tr>
													</FooterTemplate>
							
												</asp:Repeater>						
						
										
										<tr>
											<td>
												<table border=0 cellspacing=0 cellpadding=0 width=100%>
													<tr>
														<td>
															<font class="regular">
																<b>Max. Measurement Datapoints:</b>
																<asp:TextBox
																	ID="txtMaxPoints"
																	Columns="4"
																	MaxLength="4"
																	Runat="Server" />	
															</font>
														</td>
														<td align=center width=45%>
															<font class="regular">
																<%If blnDMEdit Then 'since edit is prereq for delete%>
																	<asp:LinkButton
																		ID="lbUpdateProfile"
																		Text="Update"
																		onCommand="UpdateProfile_Command"
																		Runat="Server" />
																	|
																	<asp:LinkButton
																		ID="lbCopyProfile"
																		Text="Copy"
																		onCommand="CopyProfile_Command"
																		Runat="Server" />
																	<%If blnDMDelete Then%>
																		|
																		<asp:LinkButton
																			ID="lbDelProfile"
																			Text="Delete"
																			onCommand="DeleteProfile_Command"
																			Runat="Server" />
																	<%End If%>
																<%End If%>
															</font>
														</td>
													</tr>
												</table>
												
											</td>									
										</tr>
										<tr>
											<td>
												<table border=0 cellspacing=0 cellpadding=0 width=100%>
													<tr>
														<td align=center>
															<font class="extra-small">
																<b>Date Created:</b>
																<asp:Label
																	ID="lblEditDateCr"
																	Runat="Server" />
															</font>
														</td>
														<td align=center>
															<font class="extra-small">
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
									<!--<p>-->
									<font class="regular">  <!--want this here?-->
										<asp:Label 
											ID="lblErrorMsg"
											ForeColor="Red"
											Runat="Server" />
									</font>
								</td>
							</tr>
							<tr>
								<td><p>&nbsp;</p></td>
							</tr>
							
						</table>		

						</center>
					<%
						CASE "ADD"
						'device profile addition interface code
							If blnDMEdit Then
					%>
							<font class="extra-small">
							&nbsp;<b>Create Device Profile</b>
							</font>
							<hr size=1>
								<div align="right" class="extra-small">
									<asp:LinkButton
										Text="View All Profiles"
										onClick="ShowList_Click"
										Runat="Server" />
									|
									<%If blnSRRead Then%>
										<a href="/admin/main.aspx" target="main">Return to Main</a>
									<%Else%>
										<a href="/main.aspx" target="main">Return to Main</a>
									<%End If%>
								</div>
							<p>
							<center>
							<table border=0 cellpadding=3 cellspacing=0 width=100%>
								<tr bgcolor="#e0e0e0">
									<th align=left><font class="regular">General Information</font></th>
								</tr>
								<tr>
									<td>
										<table border=0 cellpadding=0 cellspacing=3 width=100%>
											<tr>
												<td>
													<table border=0 cellpadding=0 cellspacing=0 width=100%>
														<tr>
															<td>
																<font class="regular">
																	<b>Profile Name:</b>
																	<asp:TextBox
																		ID="txtAddName"
																		Columns="25"
																		MaxLength="100"
																		Runat="Server" />
																</font>
															</td>
															<td align=right>
																<font class="regular">
																	<b>Device Type:</b>
																	<asp:DropDownList
																		ID="ddlAddDevType"
																		AutoPostBack="True"
																		onSelectedIndexChanged="TypeChanged_SelectedItemChange"
																		Runat="Server" />
																</font>
															</td>
														</tr>
													</table>
													&nbsp;
												</td>
											</tr>
											<tr>
												<td>
													<table border=0 cellspacing=0 cellpadding=0 width=100%>
														<tr>
															<td valign=top>
																<font class="regular">	
																	<b>Description:</b>
																</font>
															</td>
															<td>
																<font class="regular">
																	<asp:TextBox 
																		ID="txtAddDesc"													
																		Rows="3"
																		Columns="40"
																		MaxLength="1000"
																		TextMode="MultiLine"
																		Wrap="True"
																		Runat="Server" />
																</font>
															</td>
														</tr>
													</table>
													&nbsp;
												</td>
											</tr>
											<asp:Repeater
												ID="rptAddProfileTerm"
												Runat="Server">
												<HeaderTemplate>
													<tr>
														<td>
															<table border=1 cellspacing=0 cellpadding=2 width=90% align=center>
																<tr>
																	<th>
																		<font class="regular">
																			Terminal
																		</font>
																	</th>
																	<th>
																		<font class="regular">
																			Max. Voltage (V)
																		</font>
																	</th>
																	<th>
																		<font class="regular">
																			Max. Current (A)
																		</font>
																	</th>
																</tr>
												</HeaderTemplate>
												
												<ItemTemplate>
											 			<tr>
															<td>
																<font class="regular">
																	<%#Container.DataItem("name")%>&nbsp;(<%#Container.DataItem("port")%>)
																</font>
															</td>
															<td align=center>
																<font class="regular">
																	+/-
																	<asp:TextBox
																		ID="txtAddTermMaxV"
																		Columns="10"
																		MaxLength="12"
																		Runat="Server" />
																</font>
															</td>
															<td align=center>
																<font class="regular">
																	+/-
																	<asp:TextBox
																		ID="txtAddTermMaxA"
																		Columns="10"
																		MaxLength="11"
																		Runat="Server" />
																</font>
															</td>
														</tr>
												</ItemTemplate>
												
												<SeparatorTemplate>
												</SeparatorTemplate>
												
												<FooterTemplate>			
															</table>
															&nbsp;
														</td>
													</tr>
												</FooterTemplate>
						
											</asp:Repeater>
											
											<tr>
												<td>
													<table border=0 cellspacing=0 cellpadding=0 width=100%>
														<tr>
															<td>
																<font class="regular">
																	<b>Max. Measurement Datapoints:</b>
																	<asp:TextBox
																		ID="txtAddMaxPoints"
																		Columns="4"
																		MaxLength="4"
																		Runat="Server" />	
																</font>
															</td>
															<td align=center width=45%>
																<font class="regular">
																	<asp:LinkButton
																		Text="Create Device Profile"
																		onCommand="CreateProfile_Command"
																		Runat="Server" />
																</font>
															</td>
														</tr>
													</table>
													
												</td>									
											</tr>
											<tr>
												<td>
													<font class="regular">  <!--want this here?-->
														<asp:Label 
															ID="lblInputErrorMsg"
															ForeColor="Red"
															Runat="Server" />
													</font>
												</td>
											</tr>
										</table>
										<p>
									
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
						<a href="/admin/main.aspx" target="main">Return to Main</a>
					</font>-->
				</center>
				</td>
			</tr>
		</table>
    </form>
  <%End If%>
  </body>
</html>
