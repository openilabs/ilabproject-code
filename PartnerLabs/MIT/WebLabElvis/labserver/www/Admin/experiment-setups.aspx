<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLab.DataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim strPageState, strSetupID, strResourceID, strSetupName, strErrorMsg, strSetupImgLoc As String
	Dim blnSetupHasImage As Boolean
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnDMRead, blnDMEdit, blnDMDelete As Boolean

	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		
		blnDMRead = False
		'load user permission set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "SysRecords", "canview")
			blnDMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "SetupManagement", "canview")
			blnDMEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "SetupManagement", "canedit")
			blnDMDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassId")), "SetupManagement", "candelete")
			
			If blnDMRead Then
				If Not Request.QueryString("sid") Is Nothing And IsNumeric(Request.QueryString("sid")) Then
					strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT setup_id FROM Setups WHERE setup_id = @SetupID);"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.AddWithValue("@SetupID", Request.QueryString("sid"))
					
					If cmdDBQuery.ExecuteScalar() = "true" Then
						strPageState = "EDIT"
						strSetupID = Request.QueryString("sid")
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
					strDBQuery = "SELECT s.setup_id, r.name, r.description, s.icon_path, s.terminals_used, s.date_modified FROM Setups s JOIN Resources r ON s.resource_id = r.resource_id;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptSetup.dataSource = cmdDBquery.ExecuteReader()
					rptSetup.DataBind()	
				
				Case "EDIT"
					'strSetupID = Request.QueryString("sid")
					strDBQuery = "SELECT r.name, r.resource_id, r.description, s.icon_path, s.terminals_used, s.date_created, s.date_modified FROM Setups s JOIN Resources r ON s.resource_id = r.resource_id WHERE s.setup_id = @SetupID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.AddWithValue("@SetupID", strSetupID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() Then
						objIconPath = dtrDBQuery("icon_path")
						
						If objIconPath Is DBNull.Value Then
							strSetupImgLoc = ""
							blnSetupHasImage = False
						Else
							strSetupImgLoc = CStr(objIconPath)
							
							If Trim(strSetupImgLoc) = "" Then
								blnSetupHasImage = False
							Else
								blnSetupHasImage = True
							End If
						End If	
						
						'strSetupImgLoc = dtrDBQuery("icon_path")
										
						lblEditTitle.Text = dtrDBQuery("name")
						txtEditName.Text = dtrDBQuery("name")
						strSetupName = dtrDBQuery("name")
						strResourceID = dtrDBQuery("resource_id")
						
						txtEditDesc.Text = dtrDBQuery("description")
						
						lblEditTermNo.Text = dtrDBQuery("terminals_used")
						
						If CInt(dtrDBQuery("terminals_used")) = 0 Then
							rptSetupTerm.Visible = False
						Else
							rptSetupTerm.Visible = True
						End If
						
						lblEditDateCr.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblEditDateMod.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
						
						lblNewTermNumber.Text = "Terminal " & (CInt(lblEditTermNo.Text) + 1) & ":"
						
						lbUpdateSetup.CommandArgument = strResourceID
						lbUpdateSetup.CommandName = blnSetupHasImage
						lbRemoveImage.CommandArgument = strSetupID
						lbRemoveImage.CommandName = strSetupImgLoc
						lbCopySetup.CommandArgument = strSetupID
						lbCopySetup.CommandName = strSetupName
						lbDeleteSetup.CommandArgument = strSetupID
						lbDeleteSetup.CommandName = blnSetupHasImage
						lbCreateTerm.CommandArgument = strSetupID
					End If
					
					dtrDBQuery.Close()
					
					strDBQuery = "SELECT setupterm_id, setup_id, number, name, x_pixel_loc, y_pixel_loc, max_amplitude, max_offset, max_current, max_frequency, max_sampling_rate, max_sampling_time, max_points, instrument, date_created, date_modified FROM SetupTerminalConfig WHERE setup_id = @SetupID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.AddWithValue("@SetupID", strSetupID)
					rptSetupTerm.DataSource = cmdDBQuery.ExecuteReader()
					rptSetupTerm.DataBind()
				
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
	
	Function InstrumentSelector(ByVal strInstrument As String) As String
		'takes as input a terminals instrument designation and returns the listitem that should be selected for ddlEditTermInstrument
		Dim strOutput As String
		
		Select strInstrument
			Case = "FGEN"
				strOutput = "0"
			Case = "SCOPE"
				strOutput = "1"
		End Select
		
		Return strOutput
	End Function
	
	Sub UpdateSetup_Command(s As Object, e As CommandEventArgs)
		'updates the general information fields of the specified setup.  If a image is specified for upload, it is written to the 
		'setup record as well as the server filesystem
		If blnDMEdit Then
			Dim blnHasImage As Boolean = CBool(e.CommandName)
			Dim strImageName, strWebPath As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strResourceID = e.CommandArgument
			
			strDBQuery = "SELECT setup_id FROM Setups WHERE resource_id = @ResourceId;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@ResourceID", strResourceID)
			strSetupID = cmdDBQuery.ExecuteScalar()
			
			'check if the setup name is an appropriate value
			If Trim(txtEditName.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Setup: A Name must be provided."
				Exit Sub
			ElseIf Trim(txtEditDesc.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Setup: Setup Description must be supplied."
				Exit Sub
			End If
			
			
			strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT resource_id FROM Resources WHERE name = @NewName AND NOT resource_id = @ResourceID);"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@NewName", txtEditName.Text)
			cmdDBQuery.Parameters.AddWithValue("@ResourceID", strResourceID)
			
			If cmdDBQuery.ExecuteScalar() = "true" Then
				lblErrorMsg.Text = "Error Updating Experiment Setup: The Name """ & txtEditName.Text & """ is already in use."
				Exit Sub
			End If
			
			'update resource listing
			strDBQuery = "UPDATE Resources SET name = @Name, description = @Desc, date_modified = GETDATE() WHERE resource_id = @ResourceID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@Name", txtEditName.Text)
			cmdDBQuery.Parameters.AddWithValue("@Desc", txtEditDesc.Text)
			cmdDBQuery.Parameters.AddWithValue("@ResourceID", strResourceID)
			cmdDBQuery.ExecuteNonQuery()
			
			'update setup listing
			'strDBQuery = "UPDATE Setups SET max_points = @MaxPoints, date_modified = GETDATE() WHERE resource_id = @ResourceID;"
			'cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			'cmdDBQuery.Parameters.AddWithValue("@MaxPoints", txtMaxPoints.Text)
			'cmdDBQuery.Parameters.AddWithValue("@ResourceID", strResourceID)
			'cmdDBQuery.ExecuteNonQuery()
			
			If Not blnHasImage Then
				'check for new image, update image field.
				If inpSetupImg.PostedFile.ContentLength > 0 Then
					'image upload exists
					
					'check that the upload is of the correct type
					If InStr(inpSetupImg.PostedFile.ContentType, "image/") = 1 Then
					
						strImageName = inpSetupImg.PostedFile.FileName
						
						strImageName = Right(strImageName, (Len(strImageName) - InStrRev(strImageName, "\")))
						
					'	response.write(MapPath("../images/setups"))						
						inpSetupImg.PostedFile.SaveAs(MapPath("../images/setups/" & e.CommandArgument & strImageName))
					
						If Right(application("homepage"), 1) = "/" Then
							strWebPath = Left(application("homepage"), Len(application("homepage")) - 1) & "/images/setups/" & e.CommandArgument & strImageName
						Else
							strWebPath = application("homepage") & "/images/setups/" & e.CommandArgument & strImageName
						End If
						strDBQuery = "UPDATE Setups SET icon_path = @NewIP, date_modified = GETDATE() WHERE resource_id = @ResourceID;"
						cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
						cmdDBQuery.Parameters.AddWithValue("@NewIP", strWebPath)
						cmdDBQuery.Parameters.AddWithValue("@ResourceID", strResourceID)
						cmdDBQuery.ExecuteNonQuery()
					Else
						lblErrorMsg.Text = "Error Updating Setup: The specified file must be an image."
						Exit Sub
					End If
				End If
			End If
		End If

	End Sub
	
	Sub DeleteSetup_Command(s As Object, e As CommandEventArgs)
		'setup deletion is handled by Resource Permission Manager Method, deletion of any associated image is handled below.
		If blnDMDelete Then
			Dim strResult, strImageLoc, strLocalPath As String
			Dim blnHasImage As Boolean = CBool(e.CommandName)
			
			lblErrorMsg.Text = ""
			strImageLoc = ""
			
			If blnHasImage Then
				strDBQuery = "SELECT icon_path FROM Setups WHERE setup_id = @SetupID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@SetupID", e.CommandArgument)
				
				strImageLoc = cmdDBQuery.ExecuteScalar()
			End If
			
			strResult = rpmObject.RemoveSetup(CInt(e.CommandArgument))
			
			If strResult <> "Setup Successfully Removed." Then
				strPageState = "EDIT"
				strSetupID = e.CommandArgument
				lblErrorMsg.Text = "Error Removing Setup: " & strResult
				
			ElseIf blnHasImage
				'check if setup image is references by any other setup and, if not, remove the image from the filesystem
				strPageState = "LIST"	
					
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT setup_id FROM Setups WHERE icon_path = @ImgLoc);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@ImgLoc", strImageLoc)
				
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
	
	Sub CopySetup_Command(s As Object, e As CommandEventArgs)
		'setup copy is handled by Resource Permission Manager Method, display is se to new setup.
		If blnDMEdit Then
			Dim strResult As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			
			strResult = rpmObject.CopySetup(CInt(e.CommandArgument), "Copy of " & Left(e.CommandName, 92))
			
			If strResult <> "Setup successfully copied." Then
				strSetupID = e.CommandArgument
				lblErrorMsg.Text = "Error Copying Setup: " & strResult
			Else
				strDBQuery = "SELECT setup_id FROM Setups WHERE name = @CopyName;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@CopyName", "Copy of " & Left(e.CommandName, 92))
				
				strSetupID = cmdDBQuery.ExecuteScalar()
			End If
		End If
	End Sub
	
	Sub UpdateTerm_Command(s As Object, e As CommandEventArgs)
		If blnDMEdit Then
			Dim txtEditTermNameRef, txtEditTermXLocRef, txtEditTermYLocRef, txtEditTermMaxAmpRef, txtEditTermMaxOffsetRef, txtEditTermMaxARef, txtEditTermMaxFRef, txtEditTermMaxSamplingRateRef, txtEditTermMaxSamplingTimeRef, txtEditTermMaxPointsRef As TextBox
			Dim ddlEditTermInstrumentRef As DropDownList
			Dim intCtrlNum As Integer
			Dim strInstrumentList, strNameList As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"	
			
			strDBQuery = "SELECT setup_id FROM SetupTerminalConfig WHERE setupterm_id = @TermID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@TermID", e.commandArgument)
			strSetupID = cmdDBQuery.ExecuteScalar()
			
			strDBQuery = "SELECT name, instrument FROM SetupTerminalConfig WHERE setup_id = @SetupID AND setupterm_id <> @TermID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@SetupID", strSetupID)
			cmdDBQuery.Parameters.AddWithValue("@TermID", e.CommandArgument)
			
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			If dtrDBQuery.Read() Then
				strInstrumentList = dtrDBQuery("instrument")
				strNameList = dtrDBQuery("name")
				
				Do While dtrDBQuery.Read()
					strInstrumentList = strInstrumentList & ":" & dtrDBQuery("instrument")
					strNameList = strNameList & ":" & dtrDBQuery("name")
				Loop 
			End If
			
			dtrDBQuery.Close()
			Dim strCtrlNum As String
			intCtrlNum = (CInt(e.CommandName) * 2) - 1
			If intCtrlNum < 10 Then
				strCtrlNum = "0" & IntCtrlNum
			Else
				strCtrlNum = IntCtrlNum
			End If
			txtEditTermNameRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermName")
			txtEditTermXLocRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermXLoc")
			txtEditTermYLocRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermYLoc")
			txtEditTermMaxAmpRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxAmp")
			txtEditTermMaxOffsetRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxOffset")
			txtEditTermMaxARef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxA")
			txtEditTermMaxFRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxF")
			txtEditTermMaxSamplingRateRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxSamplingRate")
			txtEditTermMaxSamplingTimeRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxSamplingTime")
			txtEditTermMaxPointsRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$txtEditTermMaxPoints")
			
			ddlEditTermInstrumentRef = FindControl("rptSetupTerm$ctl" & strCtrlNum & "$ddlEditTermInstrument")
			
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
			
			If Not ddlEditTermInstrumentRef Is Nothing Then
				If InStr(strInstrumentList, ddlEditTermInstrumentRef.SelectedItem.Text) <> 0 Then
					lblErrorMsg.Text = "Error Updating Terminal " & e.CommandName & ": " & ddlEditTermInstrumentRef.SelectedItem.Text & " is already in use."
					Exit Sub
				End If
			Else
				lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
				Exit Sub
			End If
			
			If ddlEditTermInstrumentRef.SelectedItem.Text = "FGEN" Then
				If Not txtEditTermMaxAmpRef Is Nothing Then
					If Trim(txtEditTermMaxAmpRef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum voltage amplitude must be supplied for this terminal."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxAmpRef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The maximum voltage amplitude must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
				
				If Not txtEditTermMaxOffsetRef Is Nothing Then
					If Trim(txtEditTermMaxOffsetRef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum voltage offset must be supplied for this terminal."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxOffsetRef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The maximum voltage offset must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
						
				If Not txtEditTermMaxARef Is Nothing Then
					If Trim(txtEditTermMaxARef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum current must be supplied for this terminal."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxARef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The maximum current must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
				
				If Not txtEditTermMaxFRef Is Nothing Then
					If Trim(txtEditTermMaxFRef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum frequency must be supplied for all terminals."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxFRef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The maximum frequency must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
				' set the sampling rate and sampling time to zero
				txtEditTermMaxSamplingRateRef.text = "0"
				txtEditTermMaxSamplingTimeRef.text = "0"
				txtEditTermMaxPointsRef.text = "0"
			Else If ddlEditTermInstrumentRef.SelectedItem.Text = "SCOPE" Then
				If Not txtEditTermMaxSamplingRateRef Is Nothing Then
					If Trim(txtEditTermMaxSamplingRateRef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum sampling rate must be supplied for this terminal."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxSamplingRateRef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The sampling rate must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
				
				If Not txtEditTermMaxSamplingTimeRef Is Nothing Then
					If Trim(txtEditTermMaxSamplingTimeRef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum sampling time must be supplied for this terminal."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxSamplingTimeRef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The sampling time must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
				If Not txtEditTermMaxPointsRef Is Nothing Then
					If Trim(txtEditTermMaxPointsRef.Text) = "" Then
						lblErrorMsg.Text = "Error Updating Terminal: A maximum number of points must be supplied for this terminal."
						Exit Sub
					ElseIf Not IsNumeric(txtEditTermMaxPointsRef.Text) Then
						lblErrorMsg.Text = "Error Updating Terminal: The number of points must be specified as a numeric value."
						Exit Sub
					End If
				Else
					lblErrorMsg.Text = "Page Error While Updating Terminal.  Aborting."
					Exit Sub
				End If
				' change the maxF, maxV and maxA fields to zero
				txtEditTermMaxAmpRef.text = "0"
				txtEditTermMaxOffsetRef.text = "0"
				txtEditTermMaxARef.text = "0"
				txtEditTermMaxFRef.text = "0"
			End If
			
			strDBQuery = "UPDATE SetupTerminalConfig SET name = @Name, x_pixel_loc = @XLoc, y_pixel_loc = @YLoc, instrument = @Instrument, max_amplitude = @MaxAmplitude, max_offset = @MaxOffset, max_current = @MaxCurrent,  max_frequency = @MaxFrequency, max_sampling_rate = @MaxSamplingRate, max_sampling_time = @MaxSamplingTime, max_points = @MaxPoints,date_modified = GETDATE() WHERE setupterm_id = @TermID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@Name", txtEditTermNameRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@XLoc", txtEditTermXLocRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@YLoc", txtEditTermYLocRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxAmplitude", txtEditTermMaxAmpRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxOffset", txtEditTermMaxOffsetRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxCurrent", txtEditTermMaxARef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxFrequency", txtEditTermMaxFRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxSamplingRate", txtEditTermMaxSamplingRateRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxSamplingTime", txtEditTermMaxSamplingTimeRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@MaxPoints", txtEditTermMaxPointsRef.Text)
			cmdDBQuery.Parameters.AddWithValue("@Instrument", ddlEditTermInstrumentRef.SelectedItem.Text)
			cmdDBQuery.Parameters.AddWithValue("@TermID", e.CommandArgument)

			cmdDBQuery.ExecuteNonQuery()
			
		End If	
	End Sub
	
	Sub DeleteTerm_Command(s As Object, e As CommandEventArgs)
		'terminal deletion is handled by Resource Permission Manager Method
		If blnDMDelete Then
			Dim strResult As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strSetupID = e.CommandName
			strResult = rpmObject.RemoveSetupTerminal(e.CommandArgument)
			
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
			strSetupID = e.CommandArgument
			strResult = rpmObject.AddSetupTerminal(CInt(e.CommandArgument), txtNewTermName.Text, CInt(txtNewTermXLoc.Text), CInt(txtNewTermYLoc.Text), CInt(txtNewTermMaxAmp.Text), CInt(txtNewTermMaxOffset.Text), CInt(txtNewTermMaxA.Text), CInt(txtNewTermMaxF.Text), CInt(txtNewTermMaxSamplingRate.Text), CInt(txtNewTermMaxSamplingTime.Text), CInt(txtNewTermMaxPoints.Text), ddlNewTermInstrument.SelectedItem.Text)
			
			If strResult <> "Terminal Successfully Added." Then
				lblErrorMsg.Text = "Error Creating Terminal: " & strResult
			Else
				txtNewTermName.Text = ""
				txtNewTermXLoc.Text = ""
				txtNewTermYLoc.Text = "" 
				txtNewTermMaxAmp.Text = ""
				txtNewTermMaxOffset.Text = ""
				txtNewTermMaxA.Text = ""
				txtNewTermMaxF.Text = ""
				txtNewTermMaxSamplingRate.Text = ""
				txtNewTermMaxSamplingTime.Text = ""
				txtNewTermMaxPoints.Text = ""
				ddlNewTermInstrument.SelectedIndex = 0	
				
			End If
		End If
	End Sub
	
	Sub RemoveImage_Command(s As Object, e As CommandEventArgs)
		'removes an image associated with the specified setup.  If the image is not referenced by any other setup, it is removed from the system.
		If blnDMDelete Then
			Dim strLocalPath As String
			
			lblErrorMsg.Text = ""
			strPageState = "EDIT"
			strSetupID = e.CommandArgument
			
			'remove the reference from the specified setup
			strDBQuery = "UPDATE Setups SET icon_path = NULL WHERE setup_id = @SetupID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@SetupID", e.CommandArgument)
			cmdDBQuery.ExecuteNonQuery()
			
			'are there other setups that use this image?
			strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT setup_id FROM Setups WHERE icon_path = @ImgLoc);"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.AddWithValue("@ImgLoc", e.CommandName)
			
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
	
	Sub CreateSetup_Command(s As Object, e As CommandEventArgs)
		'creates the general information fields of a new experiment setup.  If an image is specified for upload, it is written to the setup record 
		'as well as the server filesystem.  The writing of the setup record is handled by a Resource Permission Manager Method.  Any system/input
		'errors cause the "add" page to redisplay with an appropriate error message.  If the setup is successfully created, the user is directed 
		'to the experiment setup "edit" page so that they may add terminals to the setup.
		If blnDMEdit Then
			Dim strImageName, strWebPath, strResult As String
			
			'check if the setup name is an appropriate value
			If Trim(txtAddName.Text) = "" Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Experiment Setup: A Name must be provided."
				Exit Sub
			End If
			
			If Trim(txtAddDesc.Text) = "" Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Experiment Setup: A Description must be provided."
				Exit Sub
			End If
			
			'check that, if a file is supplied, it is of an appropriate setup
			If inpAddSetupImg.PostedFile.ContentLength > 0 And InStr(inpAddSetupImg.PostedFile.ContentType, "image/") <> 1 Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Experiment Setup: The specified file must be an image."
				Exit Sub
			End If
			
			'do setup create without image
			strResult = rpmObject.AddSetup(txtAddName.Text, "", "Setup", txtAddDesc.Text)
			
			If strResult <> "Setup Successfully Created." Then
				strPageState = "ADD"
				lblInputErrorMsg.Text = "Error Creating Experiment Setup: " & strResult
			
			Else
				'create succeeded
				
				'get id of new setup
				strDBQuery = "SELECT s.setup_id FROM Setups s JOIN Resources r ON r.resource_id = s.resource_id WHERE r.name = @Name;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@Name", txtAddName.Text)
				
				strSetupID = cmdDBQuery.ExecuteScalar()
				
				'check for new image.
				If inpAddSetupImg.PostedFile.ContentLength > 0 Then
					'image upload exists
					strImageName = inpAddSetupImg.PostedFile.FileName
					
					'response.write(inpAddSetupImg.PostedFile.ContentType)
					
					strImageName = Right(strImageName, (Len(strImageName) - InStrRev(strImageName, "\")))
					
					inpAddSetupImg.PostedFile.SaveAs(MapPath("../images/setups/" & strSetupID & strImageName))
					
					If Right(application("homepage"), 1) = "/" Then
						strWebPath = Left(application("homepage"), Len(application("homepage")) - 1) & "/images/setups/" & strSetupID & strImageName
					Else
						strWebPath = application("homepage") & "/images/setups/" & strSetupID & strImageName
					End If
					
					'do update w/ image
					
					strDBQuery = "UPDATE Setups SET icon_path = @NewIP WHERE setup_id = @SetupID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.AddWithValue("@NewIP", strWebPath)
					cmdDBQuery.Parameters.AddWithValue("@SetupID", strSetupID)
					cmdDBQuery.ExecuteNonQuery()
					
				End If
				
				lblErrorMsg.Text = ""
				txtAddName.Text = ""
				lblInputErrorMsg.Text = ""
				strPageState = "EDIT"
			End If
		End If
	End Sub
	
	Sub ShowSetup_Command(s As Object, e As CommandEventArgs)
		strPageState="EDIT"
		strSetupID=e.CommandArgument
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
					Experiment Setup Management:
					</font>
					<%
					Select Case strPageState
						Case "LIST"
						'log list display code
					%>
					<font class="extra-small">
						<b>Available Experiment Setups</b>
					</font>
					
						<hr size=1>
						<div align="right" class="extra-small">
							<%If blnDMEdit Then%>
							<asp:LinkButton
								Text="Create New Setup"
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
							<asp:Repeater
								ID="rptSetup"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">Name</font>
											</th>
											<th align="left">
												<font class="regular">Description</font>
											</th>
											<th align="left">
												<font class="regular">Setup Image</font>
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
														CommandArgument='<%#Container.DataItem("setup_id")%>'
														OnCommand="ShowSetup_Command"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<%#Container.DataItem("description")%>
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
									Text="View All Setups"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%End If%>
								 <%If blnDMEdit Then %>
								 <asp:LinkButton
									Text="Create New Setup"
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
										<tr>
										<tr>
											<td colspan=2>
												<font class="regular">	
													<b>Experiment Setup Image:</b>
													<%If blnSetupHasImage Then%>
														<%If blnDMDelete Then%>
														(<asp:LinkButton
															ID="lbRemoveImage"
															Text="Remove Image"
															onCommand="RemoveImage_Command"
															Runat="Server" />)
														<%End If%>
														<br>
														<center>
														<img src="<%=strSetupImgLoc%>" border=1 width=500 height=200 alt="<%=strSetupImgLoc%>">
														</center>
													<%Else%>
														<input id="inpSetupImg" Type="File" Runat="Server">
														
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
															ID="lbUpdateSetup"
															Text="Update"
															onCommand="UpdateSetup_Command"
															Runat="Server" />
														|
														<asp:LinkButton
															ID="lbCopySetup"
															Text="Copy Setup"
															onCommand="CopySetup_Command"
															Runat="Server" />
														<%If blnDMDelete Then %>
															|
															<asp:LinkButton
																ID="lbDeleteSetup"
																Text="Delete"
																onCommand="DeleteSetup_Command"
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
										ID="rptSetupTerm"
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
															<b>Instrument:</b>
															<asp:DropDownList
																ID="ddlEditTermInstrument"
																SelectedIndex='<%#InstrumentSelector(Container.DataItem("instrument"))%>'
																Runat="Server">
																<asp:ListItem
																	Text="FGEN" />
																<asp:ListItem
																	Text="SCOPE" />	
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
													<td>
														<font class="regular">
															 Maximum Voltage Amplitude (+/- V)
															<asp:TextBox
																ID="txtEditTermMaxAmp"
																Text='<%#Container.DataItem("max_amplitude")%>'
																Columns="10"
																MaxLength="12"
																Runat="Server" />
														</font>
													</td>
													<td>
														<font class="regular">
															 Maximum Voltage Offset (+/- V)
															<asp:TextBox
																ID="txtEditTermMaxOffset"
																Text='<%#Container.DataItem("max_offset")%>'
																Columns="10"
																MaxLength="12"
																Runat="Server" />
														</font>
													</td>
												</tr>
												<tr>
													<td>
													<font class="regular">
															Maximum Frequency (Hz)
															<asp:TextBox
																ID="txtEditTermMaxF"
																Text='<%#Container.DataItem("max_frequency")%>'
																Columns="10"
																MaxLength="11"
																Runat="Server" />
														</font>
													</td>
													<td>
													<font class="regular">
															Maximum Current (+/- A)
															<asp:TextBox
																ID="txtEditTermMaxA"
																Text='<%#Container.DataItem("max_current")%>'
																Columns="10"
																MaxLength="11"
																Runat="Server" />
														</font>
													</td>
												</tr>
												<tr>
													<td>
													<font class="regular">
															Maximum Sampling Rate (For input terminals)
															<asp:TextBox
																ID="txtEditTermMaxSamplingRate"
																Text='<%#Container.DataItem("max_sampling_rate")%>'
																Columns="10"
																MaxLength="11"
																Runat="Server" />
														</font>
													</td>
													<td>
													<font class="regular">
															Maximum Sampling Time (For input terminals, in secs)
															<asp:TextBox
																ID="txtEditTermMaxSamplingTime"
																Text='<%#Container.DataItem("max_sampling_time")%>'
																Columns="10"
																MaxLength="11"
																Runat="Server" />
														</font>
													</td>
												</tr>
												<tr>
													<td colspan = 2>
													<font class="regular">
															Maximum Number of Samples (For input terminals)
															<asp:TextBox
																ID="txtEditTermMaxPoints"
																Text='<%#Container.DataItem("max_points")%>'
																Columns="10"
																MaxLength="11"
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
																	CommandArgument='<%#Container.DataItem("setupterm_id")%>'
																	CommandName='<%#Container.DataItem("number")%>'
																	onCommand="UpdateTerm_Command"
																	Runat="Server" />
																<%If blnDMDelete Then%>
																	|
																	<asp:LinkButton
																		Text="Delete Terminal"
																		CommandArgument='<%#Container.DataItem("setupterm_id")%>'
																		CommandName='<%#Container.DataItem("setup_id")%>'
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
																<b>Instrument:</b>
																<asp:DropDownList
																	ID="ddlNewTermInstrument"
																	Runat="Server">
																	<asp:ListItem
																		Text="FGEN" />
																	<asp:ListItem
																		Text="SCOPE" />	
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
														<td>
															<font class="regular">
																Maximum Voltage Amplitude (+/- V)
																<asp:TextBox
																	ID="txtNewTermMaxAmp"
																	Columns="10"
																	MaxLength="12"
																	Runat="Server" />
															</font>
														</td>
														<td>
														<font class="regular">
																Maximum Voltage Offset (+/- V)
																<asp:TextBox
																	ID="txtNewTermMaxOffset"
																	Columns="10"
																	MaxLength="11"
																	Runat="Server" />
															</font>
														</td>
													</tr>
													<tr>
														<td>
														<font class="regular">
																Maximum Frequency (+Hz)
																<asp:TextBox
																	ID="txtNewTermMaxF"
																	Columns="10"
																	MaxLength="11"
																	Runat="Server" />
															</font>
														</td>
														<td>
														<font class="regular">
																Maximum Current (+/- A)
																<asp:TextBox
																	ID="txtNewTermMaxA"
																	Columns="10"
																	MaxLength="11"
																	Runat="Server" />
															</font>
														</td>
													</tr>
													<tr>
														<td>
														<font class="regular">
																Maximum Sampling Rate (Samples/Sec)
																<asp:TextBox
																	ID="txtNewTermMaxSamplingRate"
																	Columns="10"
																	MaxLength="11"
																	Runat="Server" />
															</font>
														</td>
														<td>
														<font class="regular">
																Maximum Sampling Time(Sec)
																<asp:TextBox
																	ID="txtNewTermMaxSamplingTime"
																	Columns="10"
																	MaxLength="11"
																	Runat="Server" />
															</font>
														</td>
													</tr>
													<tr>
														<td colspan = 2>
														<font class="regular">
																Maximum No. of Samples
																<asp:TextBox
																	ID="txtNewTermMaxPoints"
																	Columns="10"
																	MaxLength="11"
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
						'experiment setup addition interface code
						If blnDMEdit Then
					%>
							<font class="extra-small">
							&nbsp;<b>Create Experiment Setup</b>
							</font>
							<hr size=1>
								<div align="right" class="extra-small">
									<%If blnDMEdit Then%>
										<asp:LinkButton
											Text="View All Experiment Setups"
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
														<b>Setup Image:</b>
															<input id="inpAddSetupImg" Type="File" Runat="Server">
													</font>
												</td>
											</tr>
											<tr>
												<td colspan=2>
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
																Text="Create Setup"
																onCommand="CreateSetup_Command"
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
