<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers.WebLabDataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim strPageState, strErrorMsg, strResourceID, strSetupID As String
	Dim blnIsSetup As Boolean
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnACRead, blnACEdit, blnACDelete, blnACGrant, blnDMRead, blnDMEdit As Boolean
	
	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		
		blnACRead = False
		'load user permission set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			blnACRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "canview")
			blnACEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "canedit")
			blnACDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "candelete")
			blnACGrant = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "cangrant")
			blnDMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SetupManagement", "canview")
			blnDMEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SetupManagement", "canedit")
			
			If blnACRead Then
				
				If Session("PermListViewPref") Is Nothing Then
					Session("PermListViewPref") = "all"
				End If

				If Not Request.QueryString("rid") Is Nothing And IsNumeric(Request.QueryString("rid")) Then
					strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT resource_id FROM Resources WHERE resource_id = @ResourceID);"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ResourceID", Request.QueryString("rid"))
					
					If cmdDBQuery.ExecuteScalar() = "true" THen
						strPageState = "EDIT"
						strResourceID = Request.QueryString("rid")
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
		If blnACRead Then		
			'write info into display fields
			
			Select strPageState
				Case "LIST" 
					strDBQuery = "SELECT resource_id, name, type, category, date_modified FROM Resources ORDER BY type, date_created;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptResources.DataSource = cmdDBquery.ExecuteReader()
					rptResources.DataBind()	
				
				Case "EDIT"
					strDBQuery = "SELECT r.resource_id, r.name, r.type, r.category, r.description, r.date_created, r.date_modified, p.setup_id FROM Resources r LEFT JOIN Setups p ON r.resource_id = p.resource_id WHERE r.resource_id = @ResourceID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ResourceID", strResourceID)
					
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() Then
						Select dtrDBQuery("type")
							Case "SETUP"
								blnIsSetup = True
								strSetupID = dtrDBQuery("setup_id")
							Case "FUNCTION"
								blnIsSetup = False
								ddlEditType.SelectedIndex = "0"
							Case "OBJECT"
								blnIsSetup = False
								ddlEditType.SelectedIndex = "1"
						End Select
							
						lblEditTitle.Text = dtrDBQuery("name")
						txtEditName.Text = dtrDBQuery("name")
						
						If dtrDBQuery("category") Is DBNull.Value Then
							txtEditCat.Text = ""
						Else
							txtEditCat.Text = dtrDBQuery("category")
						End If
						
						txtEditDesc.Text = dtrDBQuery("description")
						lblDateCreated.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblDateModified.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
						
						lbExpandView.CommandArgument = strResourceID
						lbLimitView.CommandArgument = strResourceID
						lbUpdate.CommandArgument = strResourceID
						lbUpdate.CommandName = dtrDBQuery("type")
						lbDelete.CommandArgument = strResourceID
						lbDelete.CommandName = dtrDBQuery("type")
					
					End If
					dtrDBQuery.Close()	
				
					If Session("PermListViewPref") = "all" Then
						strDBQuery = "SELECT c.name, c.class_id, m.mapping_id, m.can_view, m.can_edit, m.can_delete, m.can_grant, m.priority FROM UsageClasses c LEFT JOIN ClassToResourceMapping m ON c.class_id = m.class_id AND m.resource_id = @ResourceID;"				
					ElseIf Session("PermListViewPref") = "limit" Then
						strDBQuery = "SELECT c.name, c.class_id, m.mapping_id, m.can_view, m.can_edit, m.can_delete, m.can_grant, m.priority FROM ClassToResourceMapping m JOIN UsageClasses c ON m.class_id = c.class_id WHERE m.resource_id = @ResourceID;"
					End If
									
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ResourceID", strResourceID)
					
					If blnIsSetup Then
						rptSetupPerms.DataSource = cmdDBQuery.ExecuteReader()
						rptSetupPerms.DataBind()
					Else
						rptNonSetupPerms.DataSource = cmdDBQuery.ExecuteReader()
						rptNonSetupPerms.DataBind()
					End If
				
				Case "ADD"
					strDBQuery = "SELECT class_id, name FROM UsageClasses WHERE NOT class_id = 1;"		'exclude administrator class, permissions automatically applied.		
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					rptAddPerms.DataSource = cmdDBQuery.ExecuteReader()
					rptAddPerms.DataBind()
					
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
	
	Sub UpdateResource_Command(s As Object, e As CommandEventArgs)
		If blnACGrant Then
			strPageState = "EDIT"
			strResourceID = e.CommandArgument
			lblErrorMsg.Text = ""
			
			'check/update general resource information first
			If Trim(txtEditName.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Resource: A Resource name must be specified."
				Exit Sub
			Else
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT resource_id FROM Resources WHERE name = @Name AND NOT resource_id = @ResourceID);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@Name", txtEditName.Text)
				cmdDBQuery.Parameters.Add("@ResourceID", e.CommandArgument)
				
				If cmdDBQuery.ExecuteScalar() = "true" Then
					lblErrorMsg.Text = "Error Updating Resource: The specified Resource name is in use, please select another."
					Exit Sub
				End If
			End If 
			
			If Trim(txtEditCat.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Resource: A Resource category must be specified."
				Exit Sub
			End If
			
			If Trim(txtEditDesc.Text) = "" Then
				lblErrorMsg.Text = "Error Updating Resource: A Resource description must be specified."
				Exit Sub
			End If
			
			'inputs verified, updating
			strDBQuery = "UPDATE Resources SET name = @Name, category = @Category, description = @Desc, date_modified = GETDATE() WHERE resource_id = @ResourceID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtEditName.Text)
			cmdDBQuery.Parameters.Add("@Category", txtEditCat.Text)
			cmdDBQuery.Parameters.Add("@Desc", txtEditDesc.Text)
			cmdDBQuery.Parameters.Add("@ResourceID", e.CommandArgument)
			
			cmdDBQuery.ExecuteNonQuery()
			
			
			Dim loopIdx, intCtrlNum As Integer
			Dim strResult As String
			Dim lblMappingIDRef, lblClassIDRef As Label
			Dim chkCanReadRef, chkCanEditRef, chkCanDeleteRef, chkCanGrantRef As CheckBox
			Dim txtPriorityRef As TextBox
			
			If Session("PermListViewPref") = "limit" Then
				'only need to handle mapping updates/removals
				
				If e.CommandName = "SETUP" Then
					For loopIdx = 0 To Cint(Request.Form("Gct")) - 1 'CInt(lblGroupCT.Text) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMappingIDRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":lblMappingID")
						chkCanReadRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanRead")
						chkCanEditRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanEdit")
						chkCanDeleteRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanDelete")
						chkCanGrantRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanGrant")
						txtPriorityRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":txtPriority")
						
						If lblMappingIDRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If	
						
						If chkCanReadRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanEditRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanDeleteRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanGrantRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If txtPriorityRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						Else
							If Not IsNumeric(txtPriorityRef.Text) Then
								lblErrorMsg.Text = "Error Updating Resource: Priority values must be expressed numerically."
								Exit Sub
							ElseIf Math.Abs(CInt(txtPriorityRef.Text)) > 20 Then
								lblErrorMsg.Text = "Error Updating Resource: Priority value must be between -20 and +20."
								Exit Sub
							End If
						End If
						
						If Not (chkCanReadRef.Checked Or chkCanEditRef.Checked Or chkCanDeleteRef.Checked Or chkCanGrantRef.Checked) Then
							'case where permissions have been removed.
							strResult = rpmObject.RemoveResourceMapping(CInt(lblMappingIDRef.Text))
							
							If strResult <> "Mapping successfully deleted." Then
								lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						Else
							'case where permissions are updated
							strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked, CInt(txtPriorityRef.Text))
							
							If strResult <> "Mapping successfully updated." Then
								lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						End If 
						
					Next
				Else
					For loopIdx = 0 To Cint(Request.Form("Gct")) - 1 'CInt(lblGroupCT.Text) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMappingIDRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":lblMappingID")
						chkCanReadRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanRead")
						chkCanEditRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanEdit")
						chkCanDeleteRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanDelete")
						chkCanGrantRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanGrant")
						
						If lblMappingIDRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If	
						
						If chkCanReadRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanEditRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanDeleteRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanGrantRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If Not (chkCanReadRef.Checked Or chkCanEditRef.Checked Or chkCanDeleteRef.Checked Or chkCanGrantRef.Checked) Then
							'case where permissions have been removed.
							strResult = rpmObject.RemoveResourceMapping(CInt(lblMappingIDRef.Text))
							
							If strResult <> "Mapping successfully deleted." Then
								lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						Else
							'case where permissions are updated
							strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked)
							
							If strResult <> "Mapping successfully updated." Then
								lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						End If 
						
					Next
				
				End If
				
			ElseIf Session("PermListViewPref") = "all" Then
				'need to handle add cases as well
				
				If e.CommandName = "SETUP" Then
					For loopIdx = 0 To Cint(Request.Form("Gct")) - 1 'CInt(lblGroupCT.Text) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMappingIDRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":lblMappingID")
						lblClassIDRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":lblClassID")
						chkCanReadRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanRead")
						chkCanEditRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanEdit")
						chkCanDeleteRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanDelete")
						chkCanGrantRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":chkCanGrant")
						txtPriorityRef = FindControl("rptSetupPerms:_ctl" & intCtrlNum & ":txtPriority")
						
						If lblMappingIDRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If	
						
						If lblClassIDRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanReadRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanEditRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanDeleteRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanGrantRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If txtPriorityRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						Else
							If Not IsNumeric(txtPriorityRef.Text) Then
								lblErrorMsg.Text = "Error Updating Resource: Priority values must be expressed numerically."
								Exit Sub
							ElseIf Math.Abs(CInt(txtPriorityRef.Text)) > 20 Then
								lblErrorMsg.Text = "Error Updating Resource: Priority value must be between -20 and +20."
								Exit Sub
							End If
						End If
						
						If Not (chkCanReadRef.Checked Or chkCanEditRef.Checked Or chkCanDeleteRef.Checked Or chkCanGrantRef.Checked) Then
							If lblMappingIDRef.Text <> "0" Then
								'case where mapping has should be removed (permissions removed)
								strResult = rpmObject.RemoveResourceMapping(CInt(lblMappingIDRef.Text))
								
								If strResult <> "Mapping successfully deleted." Then
									lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							End If
								'else case is where non-mapped class has not been given permissions, do nothing.
						Else
							If lblMappingIDRef.Text <> "0" Then
								'case where non-removal edits have been made to pre-existing mapping, update.					
								strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked, CInt(txtPriorityRef.Text))
								
								If strResult <> "Mapping successfully updated." Then
									lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							Else 
								'case where non-mapped class has been given permissions, add
								strResult = rpmObject.MapClassToResource(CInt(e.CommandArgument), CInt(lblClassIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked, CInt(txtPriorityRef.Text))
								
								If strResult <> "Mapping successfully added." Then
									lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							End If
						End If 
						
					Next
				Else
				'non-setup case
					For loopIdx = 0 To Cint(Request.Form("Gct")) - 1 'CInt(lblGroupCT.Text) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMappingIDRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":lblMappingID")
						lblClassIDRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":lblClassID")
						chkCanReadRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanRead")
						chkCanEditRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanEdit")
						chkCanDeleteRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanDelete")
						chkCanGrantRef = FindControl("rptNonSetupPerms:_ctl" & intCtrlNum & ":chkCanGrant")
						
						If lblMappingIDRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If	
						
						If lblClassIDRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanReadRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanEditRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanDeleteRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If chkCanGrantRef Is Nothing Then
							lblErrorMsg.Text = "Page Error While Updating Resource.  Aborting."
							Exit Sub
						End If
						
						If Not (chkCanReadRef.Checked Or chkCanEditRef.Checked Or chkCanDeleteRef.Checked Or chkCanGrantRef.Checked) Then
							If lblMappingIDRef.Text <> "0" Then
								'case where mapping has should be removed (permissions removed)
								strResult = rpmObject.RemoveResourceMapping(CInt(lblMappingIDRef.Text))
								
								If strResult <> "Mapping successfully deleted." Then
									lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							End If
								'else case is where non-mapped class has not been given permissions, do nothing.
						Else
							If lblMappingIDRef.Text <> "0" Then
								'case where non-removal edits have been made to pre-existing mapping, update.					
								strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked)
								
								If strResult <> "Mapping successfully updated." Then
									lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							Else 
								'case where non-mapped class has been given permissions, add
								strResult = rpmObject.MapClassToResource(CInt(e.CommandArgument), CInt(lblClassIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked)
								
								If strResult <> "Mapping successfully added." Then
									lblErrorMsg.Text = "Error Updating Resource: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							End If
						End If 
						
					Next
				
				End If
			End If
		Else
			strPageState = "LIST"
		End If
		
	End Sub

	Sub DeleteResource_Command(s As Object, e As CommandEventArgs)
		'this method is handled by a Resource Permission Manager Method
		If blnACDelete Then
			lblErrorMsg.Text = ""
			
			Dim strResult As String
				
			strResult = rpmObject.RemoveResource(CInt(e.CommandArgument))
			
			If strResult <> "Resource Successfully Removed." Then
				strPageState = "EDIT"
				strResourceID = e.CommandArgument
				
				lblErrorMsg.Text = "Error Removing Resource: " & strResult
				Exit Sub
			Else
				strPageState = "LIST"
			End If
		Else
			strPageState = "LIST"
		End If
	End Sub
	
	Sub CreateResource_Command(s As Object, e As CommandEventArgs)
		'initial creation is handled by the Resource Permission Manager method AddResource.  Assigning class permissions is then performed.
		If blnACEdit Then
			lblErrorMsg.Text = ""
			lblErrorOnAddMsg.Text = ""
			Dim strResult As String
			
			'create resource
			strResult = rpmObject.AddResource(txtAddName.Text, "FUNCTION", "Software Component", txtAddDesc.Text)
			
			If strResult <> "Resource successfully added." Then
				strPageState = "ADD"
				
				lblErrorOnAddMsg.Text = "Error Creating Resource: " & strResult
				Exit Sub
			End If
						
			'creation successfull, get new resource id, set page state and assign permission settings.
			strDBQuery = "SELECT resource_id FROM Resources WHERE name = @NewName;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@NewName", txtAddName.Text)
			
			strResourceID = cmdDBQuery.ExecuteScalar()
			strPageState = "EDIT"
			
			If blnACGrant Then
				Dim loopIdx, intCtrlNum As Integer
				Dim chkAddCanReadRef, chkAddCanEditRef, chkAddCanDeleteRef, chkAddCanGrantRef As CheckBox
				Dim lblAddClassIDRef As Label
				
				For loopIdx = 0 To CInt(Request.Form("Gct")) - 1
					intCtrlNum = (loopIdx * 2) + 1
					
					lblAddClassIDRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":lblAddClassID")
					chkAddCanReadRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanRead")
					chkAddCanEditRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanEdit")
					chkAddCanDeleteRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanDelete")
					chkAddCanGrantRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanGrant")
					
					If lblAddClassIDRef Is Nothing Then
						lblErrorOnAddMsg.Text = "Page Error While Assigning Permissions.  Aborting."
						strPageState = "ADD"
						Exit Sub
					End If
					
					If chkAddCanReadRef Is Nothing Then
						lblErrorOnAddMsg.Text = "Page Error While Assigning Permissions.  Aborting."
						strPageState = "ADD"
						Exit Sub
					End If
				
					If chkAddCanEditRef Is Nothing Then
						lblErrorOnAddMsg.Text = "Page Error While Assigning Permissions.  Aborting."
						strPageState = "ADD"
						Exit Sub
					End If
					
					If chkAddCanDeleteRef Is Nothing Then
						lblErrorOnAddMsg.Text = "Page Error While Assigning Permissions.  Aborting."
						strPageState = "ADD"
						Exit Sub
					End If
					
					If chkAddCanGrantRef Is Nothing Then
						lblErrorMsg.Text = "Page Error While Assigning Permissions.  Aborting."
						Exit Sub
					End If
					
					'use RPM method MapClassToResource to assign resource permissions.
					
					strResult = rpmObject.MapClassToResource(CInt(strResourceID), CInt(lblAddClassIDRef.Text), chkAddCanReadRef.Checked, chkAddCanEditRef.Checked, chkAddCanGrantRef.Checked, chkAddCanDeleteRef.Checked)
					
					If strResult <> "Mapping successfully added." Then
						lblErrorMsg.Text = "Error Assigning Permissions: " & strResult
						Exit Sub
					End If
					
				Next
			End If
		Else
			strPageState = "LIST"
		End If

	End Sub

	Sub ExpandView_Command(s As Object, e As CommandEventArgs)
		strPageState="EDIT"
		strResourceID = e.CommandArgument
		Session("PermListViewPref") = "all"
	End Sub
	
	Sub LimitView_Command(s As Object, e As CommandEventArgs)
		strPageState="EDIT"
		strResourceID = e.CommandArgument
		Session("PermListViewPref") = "limit"
	End Sub
	
	Sub ShowResource_Command(s As Object, e As CommandEventArgs)
		strPageState="EDIT"
		lblErrorMsg.Text = ""
		strResourceID=e.CommandArgument
	End Sub
		
	Sub ShowList_Click(s As Object, e As EventArgs)
		strPageState = "LIST"
	End Sub
	
	Sub ShowAdd_Click(s As Object, e As EventArgs)
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnACRead Then  %>
    <form EncType="multipart/form-data" Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					System Resources:
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
							<%If blnACEdit Then%>
								<asp:LinkButton
									Text="Create New Resource"
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
								ID="rptResources"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">Resource</font>
											</th>
											<th align="left">
												<font class="regular">Category</font>
											</th>
											<th align="left">
												<font class="regular">Type</font>
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
														CommandArgument='<%#Container.DataItem("resource_id")%>'
														OnCommand="ShowResource_Command"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular"> 
													<%#Container.DataItem("category")%>
												</font>
											</td>
											<td>
												<font class="regular">
													<%#Container.DataItem("Type")%>
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
						'resource edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEditTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Resources"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnACEdit Then%>
									<asp:LinkButton
										Text="Create New Resource"
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
									<%If blnIsSetup And blnDMRead Then%>
									<font class="extra-small">(<a href="/labserver/admin/experiment-setups.aspx?sid=<%=strSetupID%>" target=main>View Experiment Setup</a>)</font>
									<%End If%>
								</td>
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
														Columns="30"
														MaxLength="250"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Category:</b>
													<asp:TextBox
														ID="txtEditCat"
														Columns="20"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">
													<b>Type:</b>
													<%If blnIsSetup Then%>
														Setup
													<%Else%>
														Function
														<!--
														<asp:DropDownList
															ID="ddlEditType"
															Visible="False"
															Runat="Server">
															<asp:ListItem
																Text="Function"
																Value="function" />
															<asp:ListItem
																Text="Object"
																Value="object" />
														</asp:DropDownList>
														-->
													<%End If%>
													<br>&nbsp;
												</font>
												
											</td>
										</tr>
										<tr>
											<td colspan=2>
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
																	Columns="45"
																	MaxLength="1000"
																	Wrap="True"
																	TextMode="MultiLine"
																	Runat="Server" />
															</font>
														</td>
													</tr>
												</table>
												<font class="regular">&nbsp;</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">
													<b>Permissions:</b>
													<%If Session("PermListViewPref") = "limit" Then%>
														(<asp:LinkButton
															ID="lbExpandView"
															Text="View All Classes"
															onCommand="ExpandView_Command"
															Runat="Server" />)
													<%Else 'default is view all%>
														(<asp:LinkButton
															ID="lbLimitView"
															Text="View Only Permissioned Classes"
															onCommand="LimitView_Command"
															Runat="Server" />)
													<%End If %>
													
													<%If blnIsSetup Then%>
														<!--if resource is an experiment setup-->
														<asp:Repeater
															ID="rptSetupPerms"
															Runat="Server">
															<HeaderTemplate>
																<table border=1 cellspacing=0 cellpadding=0 width=100%>
																	<tr>
																		<th>
																			<font class="regular">
																				Usage Class
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				View
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Edit
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Delete
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Grant
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Priority
																			</font>
																		</th>
																	</tr>
															</HeaderTemplate>
															
															<ItemTemplate>
											 						<tr>
											 							<td>
											 								<font class="regular">
											 									&nbsp;<a href="/labserver/admin/usage-classes.aspx?cid=<%#Container.DataItem("class_id")%>" target=main><%#Container.DataItem("name")%></a>
											 									<asp:Label
											 										ID="lblMappingID"
											 										Visible="False"
											 										Text='<%#DBNullFilter(Container.DataItem("mapping_id"), "0")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblClassID"
											 										Visible="False"
											 										Text='<%#Container.DataItem("class_id")%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanRead"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_view"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanEdit"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_edit"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanDelete"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_delete"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanGrant"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_grant"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:TextBox
											 										ID="txtPriority"
											 										Columns="3"
											 										MaxLength="3"
											 										Text='<%#DBNullFilter(Container.DataItem("priority"), "0")%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 						</tr>
															</ItemTemplate>
															
															<SeparatorTemplate>
															</SeparatorTemplate>
															
															<FooterTemplate>			
																	</table>
															</FooterTemplate>
									
														</asp:Repeater>
														
														<input type=hidden name=Gct value=<%=rptSetupPerms.Items.Count()%>
													<%Else%>
														<!--if resource is not an experiment setup-->
														<asp:Repeater
															ID="rptNonSetupPerms"
															Runat="Server">
															<HeaderTemplate>
																<table border=1 cellspacing=0 cellpadding=0 width=100%>
																	<tr>
																		<th>
																			<font class="regular">
																				Usage Class
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				View
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Edit
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Delete
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Grant
																			</font>
																		</th>
																	</tr>
															</HeaderTemplate>
															
															<ItemTemplate>
											 						<tr>
											 							<td>
											 								<font class="regular">
											 									&nbsp;<a href="/labserver/admin/usage-classes.aspx?cid=<%#Container.DataItem("class_id")%>" target=main><%#Container.DataItem("name")%></a>
											 									<asp:Label
											 										ID="lblMappingID"
											 										Visible="False"
											 										Text='<%#DBNullFilter(Container.DataItem("mapping_id"), "0")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblClassID"
											 										Visible="False"
											 										Text='<%#Container.DataItem("class_id")%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanRead"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_view"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanEdit"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_edit"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanDelete"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_delete"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkCanGrant"
											 										Checked='<%#DisplayCheck(Container.DataItem("can_grant"))%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 						</tr>
															</ItemTemplate>
															
															<SeparatorTemplate>
															</SeparatorTemplate>
															
															<FooterTemplate>			
																	</table>
															</FooterTemplate>
									
														</asp:Repeater>
														
														<input type=hidden name=Gct value=<%=rptNonSetupPerms.Items.Count()%>
													<%End If%>
													&nbsp;
													<asp:Label 
														ID="lblErrorMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2 align=center>
												<font class="regular">
													<%If blnACGrant Then%>
														<asp:LinkButton 
															ID="lbUpdate"
															Text="Update Information"
															onCommand="UpdateResource_Command"
															Runat="Server" />
													<%End If%>
													<%If blnACGrant And blnACDelete Then%>
													|
													<%End If%>
													<%If blnACDelete Then%>
														<asp:LinkButton
															ID="lbDelete"
															Text="Remove Resource"
															onCommand="DeleteResource_Command"
															Runat="Server" />
													<%End If%>
												</font>
											</td>
										</tr>
										<tr>
											<td align=center>
												<font class="small">
													<b>Date Created:</b>
													<asp:Label
														ID="lblDateCreated"
														Runat="Server" />
												</font>
											</td>
											<td align=center>
												<font class="small">
													<b>Date Modified:</b>
													<asp:Label
														ID="lblDateModified"
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
						'system resource addition interface code
						If blnACEdit Then
					%>
							<font class="extra-small">
							&nbsp;<b>Create Resource</b>
							</font>
							<hr size=1>
								<div align="right" class="extra-small">
									<asp:LinkButton
										Text="View All Resources"
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
															ID="txtAddName"
															Columns="30"
															MaxLength="250"
															Runat="Server" />
													</font>
												</td>
												<td>
													<font class="regular">
														<b>Type:</b>
														Function
													</font>
												</td>
											</tr>
											<tr>
												<td colspan=2>
													<font class="regular">
														Note: This form is to be used for creating software resources only.  
														<%If blnDMEdit Then%>
															Experiment Setup resources can be added via the <a href="/labserver/admin/experiment-setups.aspx?mode=add" target=main>Create Experiment Setup form</a>.
														<%End If%>
														<br>&nbsp;
													</font>
													
												</td>
											</tr>
											<tr>
												<td colspan=2>
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
																		Columns="45"
																		MaxLength="1000"
																		Wrap="True"
																		TextMode="MultiLine"
																		Runat="Server" />
																</font>
															</td>
														</tr>
													</table>
													<font class="regular">&nbsp;</font>
												</td>
											</tr>
											<tr>
												<td colspan=2>
													<font class="regular">
														<b>Permissions:</b>
														The Administrator Class will automatically be granted full permissions on this resource.
														<%
														If Not blnACGrant Then
															rptAddPerms.Visible = False
														End If
														%>
														<asp:Repeater
															ID="rptAddPerms"
															Runat="Server">
															<HeaderTemplate>
																<table border=1 cellspacing=0 cellpadding=0 width=100%>
																	<tr>
																		<th>
																			<font class="regular">
																				Usage Class
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				View
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Edit
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Delete
																			</font>
																		</th>
																		<th>
																			<font class="regular">
																				Grant
																			</font>
																		</th>
																	</tr>
															</HeaderTemplate>
															
															<ItemTemplate>
											 						<tr>
											 							<td>
											 								<font class="regular">
											 									&nbsp;<a href="/labserver/admin/usage-classes.aspx?cid=<%#Container.DataItem("class_id")%>" target=main><%#Container.DataItem("name")%></a>
											 									<asp:Label
											 										ID="lblAddClassID"
											 										Visible="False"
											 										Text='<%#Container.DataItem("class_id")%>'
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkAddCanRead"
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkAddCanEdit"
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkAddCanDelete"
											 										Runat="Server" />
											 								</font>
											 							</td>
											 							<td align=center>
											 								<font class="regular">
											 									<asp:CheckBox
											 										ID="chkAddCanGrant"
											 										Runat="Server" />
											 								</font>
											 							</td>
											 						</tr>
															</ItemTemplate>
															
															<SeparatorTemplate>
															</SeparatorTemplate>
															
															<FooterTemplate>			
																	</table>
															</FooterTemplate>
									
														</asp:Repeater>
														
														<input type=hidden name=Gct value=<%=rptAddPerms.Items.Count()%>
													
														&nbsp;
														<asp:Label 
															ID="lblErrorOnAddMsg"
															ForeColor="Red"
															Runat="Server" />
													</font>
												</td>
											</tr>
											<tr>
												<td colspan=2 align=center>
													<font class="regular">
														<%If blnACEdit Then%>
															<asp:LinkButton 
																ID="lbCreate"
																Text="Create Resource"
																onCommand="CreateResource_Command"
																Runat="Server" />
														<%End If%>
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
