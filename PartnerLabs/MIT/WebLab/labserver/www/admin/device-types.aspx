<%@ Page Language="VBScript"%>
<%@ Import Namespace="System.IO" %>
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
	Dim strPageState, strTypeID, strTypeName, strErrorMsg, strDevImgLoc As String
	Dim blnTypeHasImage As Boolean
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnDMRead, blnDMEdit, blnDMDelete As Boolean

	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		
		blnDMRead = False
		'load user permission set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "SysRecords", "canview")
			blnDMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "DeviceManagement", "canview")
			blnDMEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "DeviceManagement", "canedit")
			blnDMDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "DeviceManagement", "candelete")
			
			If blnDMRead Then
				If Not Request.QueryString("tid") Is Nothing And IsNumeric(Request.QueryString("tid")) Then
					strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT type_id FROM DeviceTypes WHERE type_id = @TypeID);"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@TypeID", Request.QueryString("tid"))
					
					If cmdDBQuery.ExecuteScalar() = "true" Then
						strPageState = "EDIT"
						strTypeID = Request.QueryString("tid")
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
			Dim objIconPath As Object
			
			Select strPageState
				Case "LIST" 
					strDBQuery = "SELECT type_id, name, icon_path, terminals_used, date_modified FROM DeviceTypes;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptDevType.dataSource = cmdDBquery.ExecuteReader()
					rptDevType.DataBind()	
				
				Case "EDIT"
					strDBQuery = "SELECT name, icon_path, terminals_used, date_created, date_modified FROM DeviceTypes WHERE type_id = @TypeID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@TypeID", strTypeID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() Then
						objIconPath = dtrDBQuery("icon_path")
						
						If objIconPath Is DBNull.Value Then
							strDevImgLoc = ""
							blnTypeHasImage = False
						Else
							strDevImgLoc = CStr(objIconPath)
							
							If Trim(strDevImgLoc) = "" Then
								blnTypeHasImage = False
							Else
								blnTypeHasImage = True
							End If
						End If	
						
						'strDevImgLoc = dtrDBQuery("icon_path")
										
						lblEditTitle.Text = dtrDBQuery("name")
						txtEditName.Text = dtrDBQuery("name")
						strTypeName = dtrDBQuery("name")
						lblEditTermNo.Text = dtrDBQuery("terminals_used")
						
						If CInt(dtrDBQuery("terminals_used")) = 0 Then
							rptDevTypeTerm.Visible = False
						Else
							rptDevTypeTerm.Visible = True
						End If
						
						lblEditDateCr.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblEditDateMod.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
						
						lblNewTermNumber.Text = "Terminal " & (CInt(lblEditTermNo.Text) + 1) & ":"
						
						lbUpdateType.CommandArgument = strTypeID
						lbUpdateType.CommandName = blnTypeHasImage
						lbRemoveImage.CommandArgument = strTypeID
						lbRemoveImage.CommandName = strDevImgLoc
						lbCopyType.CommandArgument = strTypeID
						lbCopyType.CommandName = strTypeName
						lbDeleteType.CommandArgument = strTypeID
						lbDeleteType.CommandName = blnTypeHasImage
						lbCreateTerm.CommandArgument = strTypeID
					End If
					
					dtrDBQuery.Close()
					
					strDBQuery = "SELECT typeterm_id, type_id, number, name, x_pixel_loc, y_pixel_loc, port, date_created, date_modified FROM DeviceTypeTerminalConfig WHERE type_id = @TypeID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@TypeID", strTypeID)
					rptDevTypeTerm.DataSource = cmdDBQuery.ExecuteReader()
					rptDevTypeTerm.DataBind()
					
				Case "ADD"
				
				
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
	
	Function DisplayIconPath(ByVal objPath As Object) As String
		'takes as input a string representing the path to an image file, if the string is empty, the string "No Image Specified" is returned, else, a link 
		'tag to the specified location is returned
		Dim strOutput As String
		
		If objPath Is DBNull.Value Then
			strOutput = "No Image Specified"
		Else
			If Trim(CStr(objPath)) = "" Then
				strOutput = "No Image Specified"
			Else
				strOutput = "<a href=""" & CStr(objPath) & """ target=""_none"">View Image</a>"
			End If
		End If
		
		Return strOutput
	End Function
	
	Function PortSelector(ByVal strPort As String) As String
		'takes as input a terminals port designation and returns the listitem that should be selected for ddlEditTermPort
		Dim strOutput As String
		
		Select strPort
			Case = "SMU1"
				strOutput = "0"
			Case = "SMU2"
				strOutput = "1"
			Case = "SMU3"
				strOutput = "2"
			Case = "SMU4"
				strOutput = "3"
			Case = "SMU5"
				strOutput = "4"
			Case = "VSU1"
				strOutput = "5"
			Case = "VSU2"
				strOutput = "6"
			Case = "VMU1"
				strOutput = "7"
			Case = "VMU2"
				strOutput = "8"
		End Select
		
		Return strOutput
	End Function
	
	Sub UpdateType_Command(s As Object, e As CommandEventArgs)
		'updates the general information fields of the specified device type.  If a image is specified for upload, it is written to the 
		'type record as well as the server filesystem
		If blnDMEdit Then
			Dim blnHasImage As Boolean = CBool(e.CommandName)
			Dim strImageName, strWebPath As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strTypeID = e.CommandArgument
			
			'check if the device name is an appropriate value
			If Trim(txtEditName.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Device Type: A Name must be provided."
				Exit Sub
			End If
			
			strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT type_id FROM DeviceTypes WHERE name = @NewName AND NOT type_id = @TypeID);"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@NewName", txtEditName.Text)
			cmdDBQuery.Parameters.Add("@TypeID", e.CommandArgument)
			
			If cmdDBQuery.ExecuteScalar() = "true" Then
				lblErrorMsg.Text = "Error Updating Device Type: The Name """ & txtEditName.Text & """ is already in use."
				Exit Sub
			End If
			
			If blnHasImage Then
				'only update name field
				strDBQuery = "UPDATE DeviceTypes SET name = @NewName, date_modified = GETDATE() WHERE type_id = @TypeID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@NewName", txtEditName.Text)
				cmdDBQuery.Parameters.Add("@TypeID", e.CommandArgument)
				cmdDBQuery.ExecuteNonQuery()
			Else
				'check for new image, update image and name fields.
				If inpDevImg.PostedFile.ContentLength > 0 Then
					'image upload exists
					
					'check that the upload is of the correct type
					If InStr(inpDevImg.PostedFile.ContentType, "image/") = 1 Then
					
						strImageName = inpDevImg.PostedFile.FileName
						
						strImageName = Right(strImageName, (Len(strImageName) - InStrRev(strImageName, "\")))
						
						inpDevImg.PostedFile.SaveAs(MapPath("/images/devices/" & e.CommandArgument & strImageName))
						
						If Right(application("homepage"), 1) = "/" Then
							strWebPath = Left(application("homepage"), Len(application("homepage")) - 1) & "/images/devices/" & e.CommandArgument & strImageName
						Else
							strWebPath = application("homepage") & "/images/devices/" & e.CommandArgument & strImageName
						End If
						
						strDBQuery = "UPDATE DeviceTypes SET name = @NewName, icon_path = @NewIP, date_modified = GETDATE() WHERE type_id = @TypeID;"
						cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
						cmdDBQuery.Parameters.Add("@NewName", txtEditName.Text)
						cmdDBQuery.Parameters.Add("@NewIP", strWebPath)
						cmdDBQuery.Parameters.Add("@TypeID", e.CommandArgument)
						cmdDBQuery.ExecuteNonQuery()
					Else
						lblErrorMsg.Text = "Error Updating Device Type: The specified file must be an image."
						Exit Sub
					End If
					
				Else
					'image not specified
					strDBQuery = "UPDATE DeviceTypes SET name = @NewName, date_modified = GETDATE() WHERE type_id = @TypeID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@NewName", txtEditName.Text)
					cmdDBQuery.Parameters.Add("@TypeID", e.CommandArgument)
					cmdDBQuery.ExecuteNonQuery()
				End If
			End If
		End If

	End Sub
	
	Sub DeleteType_Command(s As Object, e As CommandEventArgs)
		'type deletion is handled by Resource Permission Manager Method, deletion of any associated image is handled below.
		If blnDMDelete Then
			Dim strResult, strImageLoc, strLocalPath As String
			Dim blnHasImage As Boolean = CBool(e.CommandName)
			
			lblErrorMsg.Text = ""
			strImageLoc = ""
			
			If blnHasImage Then
				strDBQuery = "SELECT icon_path FROM DeviceTypes WHERE type_id = @TypeID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@TypeID", e.CommandArgument)
				
				strImageLoc = cmdDBQuery.ExecuteScalar()
			End If
			
			strResult = rpmObject.RemoveDeviceType(CInt(e.CommandArgument))
			
			If strResult <> "Device type successfully removed." Then
				strPageState = "EDIT"
				strTypeID = e.CommandArgument
				lblErrorMsg.Text = "Error Removing Device Type: " & strResult
				
			ElseIf blnHasImage
				'check if type image is references by any other type and, if not, remove the image from the filesystem
				strPageState = "LIST"	
					
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT type_id FROM DeviceTypes WHERE icon_path = @ImgLoc);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@ImgLoc", strImageLoc)
				
				'if there are no other references, proceed with file removal
				If cmdDBQuery.ExecuteScalar() <> "true" Then
					strLocalPath = Replace(strImageLoc, application("homepage"), "")
					
					If InStr(strLocalPath, "/") <> 1 Then
						strLocalPath = "/" & strLocalPath
					End If
					
					'makes sure the file exists before deleting (if not, the pointer is considered bad and not restored)
					If File.Exists(MapPath(strLocalPath)) Then
						File.Delete(MapPath(strLocalPath))
					End If
				End If	
			Else
				strPageState = "LIST"
			End If
		End If
	End Sub
	
	Sub CopyType_Command(s As Object, e As CommandEventArgs)
		'type copy is handled by Resource Permission Manager Method, display is se to new type.
		If blnDMEdit Then
			Dim strResult As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			
			strResult = rpmObject.CopyDeviceType(CInt(e.CommandArgument), "Copy of " & Left(e.CommandName, 92))
			
			If strResult <> "Device type successfully copied." Then
				strTypeID = e.CommandArgument
				lblErrorMsg.Text = "Error Copying Device Type: " & strResult
			Else
				strDBQuery = "SELECT type_id FROM DeviceTypes WHERE name = @CopyName;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@CopyName", "Copy of " & Left(e.CommandName, 92))
				
				strTypeID = cmdDBQuery.ExecuteScalar()
			End If
		End If
	End Sub
	
	Sub UpdateTerm_Command(s As Object, e As CommandEventArgs)
		If blnDMEdit Then
			Dim txtEditTermNameRef, txtEditTermXLocRef, txtEditTermYLocRef  As TextBox
			Dim ddlEditTermPortRef As DropDownList
			Dim intCtrlNum As Integer
			Dim strPortList, strNameList As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"	
			
			strDBQuery = "SELECT type_id, name, port FROM DeviceTypeTerminalConfig WHERE type_id = (SELECT type_id FROM DeviceTypeTerminalConfig WHERE typeterm_id = @TermID) AND typeterm_id <> @TermID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@TermID", e.CommandArgument)
			
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			If dtrDBQuery.Read() Then
				strTypeID = dtrDBQuery("type_id")
				strPortList = dtrDBQuery("port")
				strNameList = dtrDBQuery("name")
				
				Do While dtrDBQuery.Read()
					strPortList = strPortList & ":" & dtrDBQuery("port")
					strNameList = strNameList & ":" & dtrDBQuery("name")
				Loop 
			End If
			
			dtrDBQuery.Close()
			
			intCtrlNum = (CInt(e.CommandName) * 2) - 1
			
			txtEditTermNameRef = FindControl("rptDevTypeTerm:_ctl" & intCtrlNum & ":txtEditTermName")
			txtEditTermXLocRef = FindControl("rptDevTypeTerm:_ctl" & intCtrlNum & ":txtEditTermXLoc")
			txtEditTermYLocRef = FindControl("rptDevTypeTerm:_ctl" & intCtrlNum & ":txtEditTermYLoc")
			ddlEditTermPortRef = FindControl("rptDevTypeTerm:_ctl" & intCtrlNum & ":ddlEditTermPort")
			
			If Not txtEditTermNameRef Is Nothing Then
				If Trim(txtEditTermNameRef.Text) = "" Then
					lblErrorMsg.Text = "Error Updating Terminal " & e.CommandName & ": Name field must not be empty."
					Exit Sub
				ElseIf InStr(strNameList, txtEditTermNameRef.Text) <> 0 Then
					lblErrorMsg.Text = "Error Updating Terminal " & e.CommandName & ": The Name " & txtEditTermNameRef.Text & " is already in use."
					Exit Sub
				End If
			Else
				lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
				Exit Sub
			End If
			
			If Not txtEditTermXLocRef Is Nothing Then
				If Trim(txtEditTermXLocRef.Text) = "" Then
					lblErrorMsg.Text = "Error Updating Terminal " & e.CommandName & ": Horizontal Location field must not be empty."
					Exit Sub
				End If
			Else
				lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
				Exit Sub
			End If
			
			If Not txtEditTermYLocRef Is Nothing Then
				If Trim(txtEditTermYLocRef.Text) = "" Then
					lblErrorMsg.Text = "Error Updating Terminal " & e.CommandName & ": Vertical Location field must not be empty."
					Exit Sub
				End If
			Else
				lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
				Exit Sub
			End If
			
			If Not ddlEditTermPortRef Is Nothing Then
				If InStr(strPortList, ddlEditTermPortRef.SelectedItem.Text) <> 0 Then
					lblErrorMsg.Text = "Error Updating Terminal " & e.CommandName & ": " & ddlEditTermPortRef.SelectedItem.Text & " is already in use."
					Exit Sub
				End If
			Else
				lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
				Exit Sub
			End If
			
			strDBQuery = "UPDATE DeviceTypeTerminalConfig SET name = @Name, x_pixel_loc = @XLoc, y_pixel_loc = @YLoc, port = @Port, date_modified = GETDATE() WHERE typeterm_id = @TermID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtEditTermNameRef.Text)
			cmdDBQuery.Parameters.Add("@XLoc", txtEditTermXLocRef.Text)
			cmdDBQuery.Parameters.Add("@YLoc", txtEditTermYLocRef.Text)
			cmdDBQuery.Parameters.Add("@Port", ddlEditTermPortRef.SelectedItem.Text)
			cmdDBQuery.Parameters.Add("@TermID", e.CommandArgument)
		
			cmdDBQuery.ExecuteNonQuery()
		End If	
	End Sub
	
	Sub DeleteTerm_Command(s As Object, e As CommandEventArgs)
		'terminal deletion is handled by Resource Permission Manager Method
		If blnDMDelete Then
			Dim strResult As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strTypeID = e.CommandName
			strResult = rpmObject.RemoveDeviceTypeTerminal(e.CommandArgument)
			
			If strResult <> "Terminal Successfully Deleted." Then
				lblErrorMsg.Text = "Error Deleting Terminal: " & strResult
			End If
		End If
	End Sub
	
	Sub CreateTerm_Command(s As Object, e As CommandEventArgs)
		'terminal creation is handled by Resource Permission Manager Method
		If blnDMEdit Then
			Dim strResult As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strTypeID = e.CommandArgument
			
			strResult = rpmObject.AddDeviceTypeTerminal(CInt(e.CommandArgument), txtNewTermName.Text, CInt(txtNewTermXLoc.Text), CInt(txtNewTermYLoc.Text), ddlNewTermPort.SelectedItem.Text)
			
			If strResult <> "Terminal Successfully Added." Then
				lblErrorMsg.Text = "Error Creating Terminal: " & strResult
			Else
				txtNewTermName.Text = ""
				txtNewTermXLoc.Text = ""
				txtNewTermYLoc.Text = "" 
				ddlNewTermPort.SelectedIndex = 0	
				
			End If
		End If
	End Sub
	
	Sub RemoveImage_Command(s As Object, e As CommandEventArgs)
		'removes an image associated with the specified type.  If the image is not referenced by any other type, it is removed from the system.
		If blnDMDelete Then
			Dim strLocalPath As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strTypeID = e.CommandArgument
			
			'remove the reference from the specified type
			strDBQuery = "UPDATE DeviceTypes SET icon_path = NULL WHERE type_id = @TypeID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@TypeID", e.CommandArgument)
			cmdDBQuery.ExecuteNonQuery()
			
			'are there other types that use this image?
			strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT type_id FROM DeviceTypes WHERE icon_path = @ImgLoc);"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@ImgLoc", e.CommandName)
			
			'if there are no other references, proceed with file removal
			If cmdDBQuery.ExecuteScalar() <> "true" Then
				strLocalPath = Replace(e.CommandName, application("homepage"), "")
				
				If InStr(strLocalPath, "/") <> 1 Then
					strLocalPath = "/" & strLocalPath
				End If
				
				'makes sure the file exists before deleting (if not, the pointer is considered bad and not restored)
				If File.Exists(MapPath(strLocalPath)) Then
					File.Delete(MapPath(strLocalPath))
				End If
			End If	
		End If
	End Sub
	
	Sub CreateType_Command(s As Object, e As CommandEventArgs)
		'creates the general information fields of a new device type.  If an image is specified for upload, it is written to the type record 
		'as well as the server filesystem.  The writing of the type record is handled by a Resource Permission Manager Method.  Any system/input
		'errors cause the "add" page to redisplay with an appropriate error message.  If the type is successfully created, the user is directed 
		'to the device type "edit" page so that they may add terminals to the type.
		If blnDMEdit Then
			Dim strImageName, strWebPath, strResult As String
			
			'check if the device name is an appropriate value
			If Trim(txtAddName.Text) = "" Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Type: A Name must be provided."
				Exit Sub
			End If
			
			'check that, if a file is supplied, it is of an appropriate type
			If inpAddDevImg.PostedFile.ContentLength > 0 And InStr(inpAddDevImg.PostedFile.ContentType, "image/") <> 1 Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Type: The specified file must be an image."
				Exit Sub
			End If
			
			'do type create without image
			strResult = rpmObject.AddDeviceType(txtAddName.Text, "")
			
			If strResult <> "Device type successfully added." Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Device Type: " & strResult
			
			Else
				'create succeeded
				
				'get type id of new type
				strDBQuery = "SELECT type_id FROM DeviceTypes WHERE name = @Name;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@Name", txtAddName.Text)
				
				strTypeID = cmdDBQuery.ExecuteScalar()
				
				'check for new image.
				If inpAddDevImg.PostedFile.ContentLength > 0 Then
					'image upload exists
					strImageName = inpAddDevImg.PostedFile.FileName
					
					'response.write(inpAddDevImg.PostedFile.ContentType)
					
					strImageName = Right(strImageName, (Len(strImageName) - InStrRev(strImageName, "\")))
					
					inpAddDevImg.PostedFile.SaveAs(MapPath("/images/devices/" & strTypeID & strImageName))
					
					If Right(application("homepage"), 1) = "/" Then
						strWebPath = Left(application("homepage"), Len(application("homepage")) - 1) & "/images/devices/" & strTypeID & strImageName
					Else
						strWebPath = application("homepage") & "/images/devices/" & strTypeID & strImageName
					End If
					
					'do update w/ image
					
					strDBQuery = "UPDATE DeviceTypes SET icon_path = @NewIP WHERE type_id = @TypeID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@NewIP", strWebPath)
					cmdDBQuery.Parameters.Add("@TypeID", strTypeID)
					cmdDBQuery.ExecuteNonQuery()
					
				End If
				
				lblErrorMsg.Text = ""
				txtAddName.Text = ""
				lblInputErrorMsg.Text = ""
				strPageState = "EDIT"
			End If
		End If
	End Sub
	
	Sub ShowType_Command(s As Object, e As CommandEventArgs)
		strPageState="EDIT"
		strTypeID=e.CommandArgument
	End Sub
		
	Sub ShowList_Click(s As Object, e As EventArgs)
		strPageState = "LIST"
	End Sub
	
	Sub ShowAdd_Click(s As Object, e As EventArgs)
		strPageState = "ADD"
		lblInputErrorMsg.Text = ""
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnDMRead Then  %>
    <form EncType="multipart/form-data" Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					Device Type Management:
					</font>
					<%
					Select Case strPageState
						Case "LIST"
						'log list display code
					%>
					<font class="extra-small">
						<b>Available Types</b>
					</font>
					
						<hr size=1>
						<div align="right" class="extra-small">
							<%If blnDMEdit Then%>
							<asp:LinkButton
								Text="Create New Type"
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
								ID="rptDevType"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">Name</font>
											</th>
											<th align="left">
												<font class="regular">Device Image</font>
											</th>
											<th align="left">
												<font class="regular">Terminals Used</font>
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
														Text='<%#Container.DataItem("name")%>'
														CommandArgument='<%#Container.DataItem("type_id")%>'
														OnCommand="ShowType_Command"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<%#DisplayIconPath(Container.DataItem("icon_path"))%>
												</font>
											</td>
											<td>
												<font class="regular">
													<%#Container.DataItem("terminals_used")%>
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
						'system notice edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEditTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<%If blnDMRead Then %>
								<asp:LinkButton
									Text="View All Types"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%End If%>
								 <%If blnDMEdit Then %>
								 <asp:LinkButton
									Text="Create New Type"
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
								<th align=left><font class="regular">General Information</font></th>
							</tr>
							<tr>
								<td>
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td>
												<font class="regular">
													<b>Name:</b>
													<asp:TextBox
														ID="txtEditName"
														Columns="25"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
											<td align=right width=50%>
												<font class="regular">
													<b>Terminals Used:</b>
													<asp:Label 
														ID="lblEditTermNo"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">	
													<b>Device Image:</b>
													<%If blnTypeHasImage Then%>
														<%If blnDMDelete Then%>
														(<asp:LinkButton
															ID="lbRemoveImage"
															Text="Remove Image"
															onCommand="RemoveImage_Command"
															Runat="Server" />)
														<%End If%>
														<br>
														<center>
														<img src="<%=strDevImgLoc%>" border=0 alt="<%=strDevImgLoc%>">
														</center>
													<%Else%>
														<input id="inpDevImg" Type="File" Runat="Server">
														
													<%End If%>
												</font>
											</td>
										</tr>
										<tr>
											<td align="center">
												<font class="extra-small">
													<b>Date Created:</b>
													<asp:Label
														ID="lblEditDateCr"
														Runat="Server" />	
												</font>
											</td>					
											<td align="center">
												<font class="extra-small">
													<b>Date Modified:</b>
													<asp:Label
														ID="lblEditDateMod"
														Runat="Server" />
												</font>
											</td>					
										</tr>
										<tr>
											<td colspan=2 align=center>
												<font class="regular">
													<%If blnDMEdit Then  'since edit is a prereq for delete%>
														<asp:LinkButton
															ID="lbUpdateType"
															Text="Update"
															onCommand="UpdateType_Command"
															Runat="Server" />
														|
														<asp:LinkButton
															ID="lbCopyType"
															Text="Copy Type"
															onCommand="CopyType_Command"
															Runat="Server" />
														<%If blnDMDelete Then %>
															|
															<asp:LinkButton
																ID="lbDeleteType"
																Text="Delete"
																onCommand="DeleteType_Command"
																Runat="Server" />
														<%End If%>
													<%End If%>
											
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
							
									<asp:Repeater
										ID="rptDevTypeTerm"
										Runat="Server">
										<HeaderTemplate>
											<tr bgcolor="#e0e0e0">
												<th align=left><font class="regular">Terminal Definitions</font></th>
											</tr>
											<tr>
												<td>
										</HeaderTemplate>
										
										<ItemTemplate>
											 
											<table border=0 cellspacing=0 cellpadding=3 width=100%>
												<tr>
													<td colspan=2>
														<font class="title">
															Terminal <%#Container.DataItem("number")%>:
														</font>
													</td>
												</tr>
												<tr>
													<td>
														<font class="regular">
															<b>Name:</b>
															<asp:TextBox
																ID="txtEditTermName"
																Text='<%#Container.DataItem("name")%>'
																Columns="25"
																MaxLength="50"
																Runat="Server" />
														</font>
													</td>
													<td>
														<font class="regular">
															<b>Port:</b>
															<asp:DropDownList
																ID="ddlEditTermPort"
																SelectedIndex='<%#PortSelector(Container.DataItem("port"))%>'
																Runat="Server">
																<asp:ListItem
																	Text="SMU1" />
																<asp:ListItem
																	Text="SMU2" />	
																<asp:ListItem
																	Text="SMU3" />
																<asp:ListItem
																	Text="SMU4" />
																<asp:ListItem
																	Text="SMU5" />
																<asp:ListItem
																	Text="VSU1" />
																<asp:ListItem
																	Text="VSU2" />
																<asp:ListItem
																	Text="VMU1" />
																<asp:ListItem
																	Text="VMU2" />
															</asp:DropDownList>
															
														</font>
													</td>
												</tr>
												<tr>
													<td>
														<font class="regular">
															<b>Horizontal Location (pixels):</b>
															<asp:TextBox
																ID="txtEditTermXLoc"
																Text='<%#Container.DataItem("x_pixel_loc")%>'
																Columns="3"
																MaxLength="5"
																Runat="Server" />
														</font>
													</td>
													<td>
														<font class="regular">
															<b>Vertical Location (pixels):</b>
															<asp:TextBox
																ID="txtEditTermYLoc"
																Text='<%#Container.DataItem("y_pixel_loc")%>'
																Columns="3"
																MaxLength="5"
																Runat="Server" />
														</font>
													</td>
												</tr>
												<tr>
													<td align="center">
														<font class="extra-small">
															<b>Date Created:</b>
															<%#DisplayDate(Container.DataItem("date_created"))%>&nbsp;<%#DisplayTime(Container.DataItem("date_created"))%>
														</font>
													</td>					
													<td align="center">
														<font class="extra-small">
															<b>Date Modified:</b>
															<%#DisplayDate(Container.DataItem("date_modified"))%>&nbsp;<%#DisplayTime(Container.DataItem("date_modified"))%>
														</font>
													</td>	
												</tr>
												<tr>
													<td colspan=2 align=center>
														<font class="regular">
															<%If blnDMEdit Then 'since edit is a prereq for delete %>
																<asp:LinkButton
																	Text="Update Terminal"
																	CommandArgument='<%#Container.DataItem("typeterm_id")%>'
																	CommandName='<%#Container.DataItem("number")%>'
																	onCommand="UpdateTerm_Command"
																	Runat="Server" />
																<%If blnDMDelete Then%>
																	|
																	<asp:LinkButton
																		Text="Delete Terminal"
																		CommandArgument='<%#Container.DataItem("typeterm_id")%>'
																		CommandName='<%#Container.DataItem("type_id")%>'
																		onCommand="DeleteTerm_Command"
																		Runat="Server" />
																<%End If%>
															<%End If%>
														</font>
													</td>
												</tr>
											</table>
										</ItemTemplate>
										
										<SeparatorTemplate>
											<p>
										</SeparatorTemplate>
										
										<FooterTemplate>
												</td>
											</tr>
											<tr>
												<td><p>&nbsp;</p></td>
											</tr>
										</FooterTemplate>
				
									</asp:Repeater>						
							
							<tr bgcolor="#e0e0e0">
								<th align=left><font class="regular">Add New Terminal</font></th>
							</tr>
							<tr>
								<td>
									<table border=0 cellspacing=0 cellpadding=0 width=100%>
										<tr>
											<td>
												<table border=0 cellspacing=0 cellpadding=3 width=100%>
													<tr>
														<td colspan=2>
															<font class="title">
																<asp:Label
																	ID="lblNewTermNumber"
																	Runat="Server" />
															</font>
														</td>
													</tr>
													<tr>
														<td>
															<font class="regular">
																<b>Name:</b>
																<asp:TextBox
																	ID="txtNewTermName"
																	Columns="25"
																	MaxLength="50"
																	Runat="Server" />
															</font>
														</td>
														<td>
															<font class="regular">
																<b>Port:</b>
																<asp:DropDownList
																	ID="ddlNewTermPort"
																	Runat="Server">
																	<asp:ListItem
																		Text="SMU1" />
																	<asp:ListItem
																		Text="SMU2" />	
																	<asp:ListItem
																		Text="SMU3" />
																	<asp:ListItem
																		Text="SMU4" />
																	<asp:ListItem
																		Text="SMU5" />
																	<asp:ListItem
																		Text="VSU1" />
																	<asp:ListItem
																		Text="VSU2" />
																	<asp:ListItem
																		Text="VMU1" />
																	<asp:ListItem
																		Text="VMU2" />
																</asp:DropDownList>
																
															</font>
														</td>
													</tr>
													<tr>
														<td>
															<font class="regular">
																<b>Horizontal Location (pixels):</b>
																<asp:TextBox
																	ID="txtNewTermXLoc"
																	Columns="3"
																	MaxLength="5"
																	Runat="Server" />
															</font>
														</td>
														<td>
															<font class="regular">
																<b>Vertical Location (pixels):</b>
																<asp:TextBox
																	ID="txtNewTermYLoc"
																	Columns="3"
																	MaxLength="5"
																	Runat="Server" />
															</font>
														</td>
													</tr>
													<tr>
														<td colspan=2 align=center>
															<font class="regular">
																<%If blnDMEdit Then%>
																	<asp:LinkButton
																		ID="lbCreateTerm"
																		Text="Create Terminal"
																		onCommand="CreateTerm_Command"
																		Runat="Server" />
																<%End If%>
															</font>
														</td>
													</tr>
												</table>
											
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>

						</center>
					<%
						CASE "ADD"
						'device type addition interface code
						If blnDMEdit Then
					%>
							<font class="extra-small">
							&nbsp;<b>Create Device Type</b>
							</font>
							<hr size=1>
								<div align="right" class="extra-small">
									<%If blnDMEdit Then%>
										<asp:LinkButton
											Text="View All Types"
											onClick="ShowList_Click"
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
									<th align=left><font class="regular">General Information</font></th>
								</tr>
								<tr>
									<td>
										<table border=0 cellpadding=0 cellspacing=3 width=100%>
											<tr>
												<td align=left>
													<font class="regular">
														<b>Name:</b>
														<asp:TextBox
															ID="txtAddName"
															Columns="25"
															MaxLength="100"
															Runat="Server" />
													</font>
												</td>
		
												<td align=right>
													<font class="regular">	
														<b>Device Image:</b>
															<input id="inpAddDevImg" Type="File" Runat="Server">
													</font>
												</td>
											</tr>
											<tr>
												<td colspan=2 align=center>
													<font class="regular">  <!--want this here?-->
														<asp:Label 
															ID="lblInputErrorMsg"
															ForeColor="Red"
															Runat="Server" />
													</font>
												</td>
											</tr>
											<tr>
												<td colspan=2 align=center>
													<font class="regular">
														<%If blnDMEdit Then%>
															<asp:LinkButton
																Text="Create Type"
																onCommand="CreateType_Command"
																Runat="Server" />
														<%End If%>
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
