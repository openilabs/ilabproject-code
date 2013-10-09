<%@ Page Language="VBScript"%>
<%@ Import Namespace="System.Data" %>
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
	Dim dstClasses, dstNewMembers As DataSet
	Dim dadClasses, dadNewMembers As SqlDataAdapter
	Dim strPageState, strErrorMsg, strClassID, strResourceID As String
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnACRead, blnACEdit, blnACDelete, blnACGrant, blnAMRead As Boolean

	
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
			blnAMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AcctManagement", "canview")		
			
			If blnACRead Then
			
				If Session("PermListViewPref") Is Nothing Then
					Session("PermListViewPref") = "all"
				End If

				If Not Request.QueryString("cid") Is Nothing And IsNumeric(Request.QueryString("cid")) Then
					strClassID = Request.QueryString("cid")
					strPageState = "EDITPERM"
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
				
					strDBQuery = "SELECT class_id, name, amt_member_brokers, amt_member_groups, amt_member_susers, date_modified FROM UsageClasses;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptClasses.DataSource = cmdDBquery.ExecuteReader()
					rptClasses.DataBind()	
				
				Case "EDITPERM"
					
					strDBQuery = "SELECT class_id, name, description, date_created, date_modified FROM UsageClasses WHERE class_id = @ClassID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() THen
						lblEditPermTitle.Text = dtrDBQuery("name")
						txtEditPermName.Text = dtrDBQuery("name")
						txtEditPermDesc.Text = dtrDBQuery("description")
						lblDateCreated.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblDateModified.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
					
						lbShowMem.CommandArgument = dtrDBQuery("class_id")
						lbShowMem.CommandName = "BROKERS"
						lbNavShowMem.CommandArgument = dtrDBQuery("class_id")
						lbNavShowMem.CommandName = "BROKERS"
						
						lbExpandView.CommandArgument = dtrDBQuery("class_id")
						lbLimitView.CommandArgument = dtrDBQuery("class_id")
						lbUpdate.CommandArgument = dtrDBQuery("class_id")
						lbUpdate.CommandName = strPageState
						lbDelete.CommandArgument = dtrDBQuery("class_id")
						lbDelete.CommandName = strPageState
						
					
					End If
					dtrDBQuery.Close()
					
					If Session("PermListViewPref") = "all" Then
						strDBQuery = "SELECT r.name, r.resource_id, r.type, m.mapping_id, m.can_view, m.can_edit, m.can_delete, m.can_grant, m.priority FROM Resources r LEFT JOIN ClassToResourceMapping m ON r.resource_id = m.resource_id AND m.class_id = @ClassID;"				
					ElseIf Session("PermListViewPref") = "limit" Then
						strDBQuery = "SELECT r.name, r.resource_id, r.type, m.mapping_id, m.can_view, m.can_edit, m.can_delete, m.can_grant, m.priority FROM Resources r JOIN ClassToResourceMapping m ON r.resource_id = m.resource_id AND m.class_id = @ClassID;"				
					End If
				
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					rptClassPerms.DataSource = cmdDBQuery.ExecuteReader()
					rptClassPerms.DataBind()
					
				Case "EDITMEMB"
					strDBQuery = "SELECT name, amt_member_brokers FROM UsageClasses WHERE class_id = @ClassID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read()
						lblBMemberTitle.Text = dtrDBQuery("name")
						lblBMemberCnt.Text = dtrDBQuery("amt_member_brokers")
					End If
					dtrDBQuery.Close()
					
					lbBNavShowPerms.CommandArgument = strClassID
					lbBToGMembers.CommandArgument = strClassID
					lbBToGMembers.CommandName = "GROUPS"
					lbBToUMembers.CommandArgument = strClassID
					lbBtoUMembers.CommandName = "SUSERS"
					lbBMemUpdate.CommandArgument = strClassID
					lbBMemUpdate.CommandName = strPageState
					lbBMemDelete.CommandArgument = strClassID
					lbBMemDelete.CommandName = strPageState
					lbBMemAssign.CommandArgument = strClassID
					lbBMemAssign.CommandName = strPageState
					
					dstClasses = New DataSet()
					strDBQuery = "SELECT 0 As class_id, 'Select Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses WHERE NOT class_id = @ClassID ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dadClasses = New SqlDataAdapter(cmdDBQuery)
					dadClasses.Fill(dstClasses, "Classes")
					
					dstNewMembers = New DataSet()
					strDBQuery = "SELECT 0 As broker_id, 'Select Service Broker' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT broker_id, name FROM Brokers WHERE NOT class_id = @ClassID ORDER BY broker_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dadNewMembers = New SqlDataAdapter(cmdDBQuery)
					dadNewMembers.Fill(dstNewMembers, "NewMembers")
					
					ddlBNonMemList.DataSource = dstNewMembers
					ddlBNonMemList.DataTextField = "name"
					ddlBNonMemList.DataValueField = "broker_id"
					ddlBNonMemList.DataBind()
					
					strDBQuery = "SELECT broker_id, name, contact_first_name, contact_last_name, contact_email, is_active FROM Brokers WHERE class_id = @ClassID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					rptBMembers.DataSource = cmdDBQuery.ExecuteReader()
					rptBMembers.DataBind()
				
				Case "EDITMEMG"
					strDBQuery = "SELECT name, amt_member_groups FROM UsageClasses WHERE class_id = @ClassID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read()
						lblGMemberTitle.Text = dtrDBQuery("name")
						lblGMemberCnt.Text = dtrDBQuery("amt_member_groups")
					End If
					dtrDBQuery.Close()
					
					lbGNavShowPerms.CommandArgument = strClassID
					lbGToBMembers.CommandArgument = strClassID
					lbGToBMembers.CommandName = "BROKERS"
					lbGToUMembers.CommandArgument = strClassID
					lbGToUMembers.CommandName = "SUSERS"
					lbGMemUpdate.CommandArgument = strClassID
					lbGMemUpdate.CommandName = strPageState
					lbGMemDelete.CommandArgument = strClassID
					lbGMemDelete.CommandName = strPageState
					lbGMemAssign.CommandArgument = strClassID
					lbGMemAssign.CommandName = strPageState
					
					dstClasses = New DataSet()
					strDBQuery = "SELECT 0 As class_id, 'Select Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses WHERE NOT class_id = @ClassID ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dadClasses = New SqlDataAdapter(cmdDBQuery)
					dadClasses.Fill(dstClasses, "Classes")
					
					dstNewMembers = New DataSet()
					strDBQuery = "SELECT 0 As group_id, 'Select Broker Group' As group_name UNION SELECT 0 As group_id, '-----' As group_name UNION SELECT g.group_id, g.name + ' (' + b.name + ')' AS group_name FROM Groups g JOIN Brokers b ON g.owner_id = b.broker_id WHERE NOT g.class_id = @ClassID ORDER BY group_id, group_name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dadNewMembers = New SqlDataAdapter(cmdDBQuery)
					dadNewMembers.Fill(dstNewMembers, "NewMembers")
					
					ddlGNonMemList.DataSource = dstNewMembers
					ddlGNonMemList.DataTextField = "group_name"
					ddlGNonMemList.DataValueField = "group_id"
					ddlGNonMemList.DataBind()
					
					strDBQuery = "SELECT g.group_id, g.name AS group_name, g.owner_id AS broker_id, b.name AS broker_name, g.is_active FROM Groups g JOIN Brokers b ON g.owner_id = b.broker_id WHERE g.class_id = @ClassID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					rptGMembers.DataSource = cmdDBQuery.ExecuteReader()
					rptGMembers.DataBind()
					
				Case "EDITMEMU"
					strDBQuery = "SELECT name, amt_member_susers FROM UsageClasses WHERE class_id = @ClassID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read()
						lblUMemberTitle.Text = dtrDBQuery("name")
						lblUMemberCnt.Text = dtrDBQuery("amt_member_susers")
					End If
					dtrDBQuery.Close()
					
					lbUNavShowPerms.CommandArgument = strClassID
					lbUToGMembers.CommandArgument = strClassID
					lbUToGMembers.CommandName = "GROUPS"
					lbUToBMembers.CommandArgument = strClassID
					lbUtoBMembers.CommandName = "BROKERS"
					lbUMemUpdate.CommandArgument = strClassID
					lbUMemUpdate.CommandName = strPageState
					lbUMemDelete.CommandArgument = strClassID
					lbUMemDelete.CommandName = strPageState
					lbUMemAssign.CommandArgument = strClassID
					lbUMemAssign.CommandName = strPageState
					
					dstClasses = New DataSet()
					strDBQuery = "SELECT 0 As class_id, 'Select Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses WHERE NOT class_id = @ClassID ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dadClasses = New SqlDataAdapter(cmdDBQuery)
					dadClasses.Fill(dstClasses, "Classes")
					
					dstNewMembers = New DataSet()
					strDBQuery = "SELECT 0 As user_id, 'Select Site User' As name, '-01' AS last_name UNION SELECT 0 As user_id, '-----' As name, '-02' AS last_name UNION SELECT user_id, first_name + ' ' + last_name, last_name AS name FROM SiteUsers WHERE NOT class_id = @ClassID ORDER BY last_name;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dadNewMembers = New SqlDataAdapter(cmdDBQuery)
					dadNewMembers.Fill(dstNewMembers, "NewMembers")
					
					ddlUNonMemList.DataSource = dstNewMembers
					ddlUNonMemList.DataTextField = "name"
					ddlUNonMemList.DataValueField = "user_id"
					ddlUNonMemList.DataBind()
					
					strDBQuery = "SELECT user_id, username, first_name, last_name, is_active FROM SiteUsers WHERE class_id = @ClassID ORDER BY last_name;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					
					rptUMembers.DataSource = cmdDBQuery.ExecuteReader()
					rptUMembers.DataBind()
				
				Case "ADD"
					
					strDBQuery = "SELECT resource_id, name, type FROM Resources;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					rptAddPerms.DataSource = cmdDBQuery.ExecuteReader()
					rptAddPerms.DataBind()
					
					'example
					'strDBQuery = "SELECT class_id, name FROM UsageClasses WHERE NOT class_id = 1;"		'exclude administrator class, permissions automatically applied.		
					'cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					'rptAddPerms.DataSource = cmdDBQuery.ExecuteReader()
					'rptAddPerms.DataBind()
					
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
	
	Sub RemoveClass_Command(s As Object, e As CommandEventArgs)
		'this routine removes the specified Usage Class from the system.  All orphaned brokers/groups/users are reassigned to the Guest class.  
		'The reassignment/removal process is handled by a Resource Permission Manager method.
		'commandargument = class_id
		'commandname = strPageState
		If blnACDelete Then
			Dim strResult As String
			
			If e.CommandName = "EDITPERM" Or e.CommandName = "EDITMEMB" Or e.CommandName = "EDITMEMG" Or e.CommandName = "EDITMEMU" Then
				'pagestate recognized, proceed.
				
				strResult = rpmObject.RemoveClass(CInt(e.CommandArgument))
				
				If strResult <> "Class successfully deleted." Then
					'error during removal
					
					Select e.CommandName
						Case "EDITPERM"
							'came from permissions page
							lblErrorOnEditPermMsg.Text = "Error Removing Class: " & strResult	
							strPageState = "EDITPERM"
							
						Case "EDITMEMB"
							'came from Broker membership page
							lblErrorOnEditBMemMsg.Text = "Error Removing Class: " & strResult
							strPageState = "EDITMEMB"
							
						Case "EDITMEMG"
							'came from group membership page
							lblErrorOnEditGMemMsg.Text = "Error Removing Class: " & strResult
							strPageState = "EDITMEMG"
						
						Case "EDITMEMU"
							'came from user membership page
							lblErrorOnEditUMemMsg.Text = "Error Removing Class: " & strResult
							strPageState = "EDITMEMU"
				
					End Select
				Else
					'removal completed successfully
					strPageState = "LIST"
				End If
			
				'request came from unkown pagestate, do nothing
				strPageState = "LIST"
			End If	
		Else
			strPageState = "LIST"
		End If
	End Sub
	
	Sub UpdateClass_Command(s As Object, e As CommandEventArgs)
		If blnACEdit Then
			'updates class properties/permission settings, outputs to source pagestate (from EDITPERM)
			strPageState = e.CommandName
			strClassID = e.CommandArgument
			lblErrorOnEditPermMsg.Text = ""
			
			'check/update general information
			If Trim(txtEditPermName.Text) = "" Then
				lblErrorOnEditPermMsg.Text = "Error Updating Class: A Class name must be specified."
				Exit Sub
			Else
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT class_id FROM UsageClasses WHERE name = @Name AND NOT class_id = @ClassID);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@Name", txtEditPermName.Text)
				cmdDBQuery.Parameters.Add("@ClassID", strClassID)
				
				If cmdDBQuery.ExecuteScalar() = "true" Then
					lblErrorOnEditPermMsg.Text = "Error Updating Class: The specified Class name is in use, please select another."
					Exit Sub
				End If
			End If
			
			If Trim(txtEditPermDesc.Text) = "" Then
				lblErrorOnEditPermMsg.Text = "Error Updating Class: A Class description must be specified."
				Exit Sub
			End If
			
			'inputs verified, updating
			strDBQuery = "UPDATE UsageClasses SET name = @Name, description = @Desc, date_modified = GETDATE() WHERE class_id = @ClassID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtEditPermName.Text)
			cmdDBQuery.Parameters.Add("@Desc", txtEditPermDesc.Text)
			cmdDBQuery.Parameters.Add("@ClassID", strClassID)
			
			cmdDBQuery.ExecuteNonQuery()
			
			If blnACGrant Then
				'prepare to update permission mapping records.
				Dim loopIdx, intCtrlNum As Integer
				Dim strResult As String
				Dim lblMappingIDRef, lblResourceIDRef, lblResourceTypeRef As Label
				Dim chkCanReadRef, chkCanEditRef, chkCanDeleteRef, chkCanGrantRef As CheckBox
				Dim txtPriorityRef As TextBox
				
				If Session("PermListViewPref") = "limit" Then
					'only need to handle mapping update/removals
				
					For loopIdx = 0 To Cint(Request.Form("Gct")) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMappingIDRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":lblMappingID")
						lblResourceTypeRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":lblResourceType")
						chkCanReadRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanRead")
						chkCanEditRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanEdit")
						chkCanDeleteRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanDelete")
						chkCanGrantRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanGrant")
						txtPriorityRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":txtPriority")
						
						If lblMappingIDRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If	
						
						If lblResourceTypeRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanReadRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanEditRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanDeleteRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanGrantRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If txtPriorityRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						Else
							If Not IsNumeric(txtPriorityRef.Text) Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: Priority values must be expressed numerically."
								Exit Sub
							ElseIf Math.Abs(CInt(txtPriorityRef.Text)) > 20 Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: Priority value must be between -20 and +20."
								Exit Sub
							End If
						End If
					
						If Not (chkCanReadRef.Checked Or chkCanEditRef.Checked Or chkCanDeleteRef.Checked Or chkCanGrantRef.Checked) Then
							'case where permissions have been removed.
							strResult = rpmObject.RemoveResourceMapping(CInt(lblMappingIDRef.Text))
							
							If strResult <> "Mapping successfully deleted." Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						Else
							'case where permissions are updated
							If lblResourceTypeRef.Text = "DEVICE" Then
								strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked, CInt(txtPriorityRef.Text))
							Else
								strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked)
							End If
							
							If strResult <> "Mapping successfully updated." Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						End If 
					Next
				Else
					For loopIdx = 0 To Cint(Request.Form("Gct")) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMappingIDRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":lblMappingID")
						lblResourceIDRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":lblResourceID")
						lblResourceTypeRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":lblResourceType")
						chkCanReadRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanRead")
						chkCanEditRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanEdit")
						chkCanDeleteRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanDelete")
						chkCanGrantRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":chkCanGrant")
						txtPriorityRef = FindControl("rptClassPerms:_ctl" & intCtrlNum & ":txtPriority")
						
						If lblMappingIDRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If	
						
						If lblResourceIDRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If lblResourceTypeRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanReadRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanEditRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanDeleteRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If chkCanGrantRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						End If
						
						If txtPriorityRef Is Nothing Then
							lblErrorOnEditPermMsg.Text = "Page Error While Updating Class.  Aborting."
							Exit Sub
						Else
							If Not IsNumeric(txtPriorityRef.Text) Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: Priority values must be expressed numerically."
								Exit Sub
							ElseIf Math.Abs(CInt(txtPriorityRef.Text)) > 20 Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: Priority value must be between -20 and +20."
								Exit Sub
							End If
						End If
					
						If Not (chkCanReadRef.Checked Or chkCanEditRef.Checked Or chkCanDeleteRef.Checked Or chkCanGrantRef.Checked) And lblMappingIDRef.Text <> "0" Then
							'case where permissions have been removed. (else case is non-mapped resource has not been given permissions, do nothing)
							strResult = rpmObject.RemoveResourceMapping(CInt(lblMappingIDRef.Text))
							
							If strResult <> "Mapping successfully deleted." Then
								lblErrorOnEditPermMsg.Text = "Error Updating Class: " & strResult & " (Item " & (loopIdx + 1) & ")"
								Exit Sub
							End If
						
						Else
							If lblMappingIDRef.Text <> "0" Then
							'case where non-removal edits have been made to pre-existing permissions, do nothing.
								If lblResourceTypeRef.Text = "DEVICE" Then
									strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked, CInt(txtPriorityRef.Text))
								Else
									strResult = rpmObject.EditResourceMapping(CInt(lblMappingIDRef.Text), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked)
								End If
								
								If strResult <> "Mapping successfully updated." Then
									lblErrorOnEditPermMsg.Text = "Error Updating Class: " & strResult & " (Item " & (loopIdx + 1) & ")"
									Exit Sub
								End If
							Else
								'case where non-mapped resource has been given permissions, add.
								If lblResourceTypeRef.Text = "DEVICE" Then
									strResult = rpmObject.MapClassToResource(CInt(lblResourceIDRef.Text), CInt(strClassID), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked, CInt(txtPriorityRef.Text))
								Else
									strResult = rpmObject.MapClassToResource(CInt(lblResourceIDRef.Text), CInt(strClassID), chkCanReadRef.Checked, chkCanEditRef.Checked, chkCanGrantRef.Checked, chkCanDeleteRef.Checked)
								End If
								
								If strResult <> "Mapping successfully added." Then
									lblErrorOnEditPermMsg.Text = "Error Updating Class: " & strResult & " (Item " & (loopIdx + 1) & ")"
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
	
	Sub UpdateClassMembership_Command(s As Object, e As CommandEventArgs)
		If blnACGrant Then
			'reassigns class members to specified other classes (or no action, if none is specified), outputs to source pagestate (from EDITMEMB, EDITMEMG, and EDITMEMU)
			strPageState = e.CommandName
			strClassID = e.CommandArgument
			
			Dim strResult As String
			Dim loopIdx, intCtrlNum As Integer
			Dim lblMemberIDRef As Label
			Dim ddlNewClassRef As DropDownList
			
			Select strPageState
				Case "EDITMEMB"
					For loopIdx = 0 To CInt(Request.Form("Gct")) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMemberIDRef = FindControl("rptBMembers:_ctl" & intCtrlNum & ":lblBrokerID")
						ddlNewClassRef = FindControl("rptBMembers:_ctl" & intCtrlNum & ":ddlBMemOC")
						
						If lblMemberIDRef Is Nothing Then
							lblErrorOnEditBMemMsg.Text = "Page Error While Updating Class Membership.  Aborting."
							Exit Sub
						End If
						
						If ddlNewClassRef Is Nothing Then
							lblErrorOnEditBMemMsg.Text = "Page Error While Updating Class Membership.  Aborting."
							Exit Sub
						End If
						
						If ddlNewClassRef.SelectedItem.Value <> "0" Then
							'case where other group has been selected, else do nothing.
							'member reassignment is handled by a resource permission manager method.
							strResult = rpmObject.MapBrokerToClass(CInt(lblMemberIDRef.Text), CInt(ddlNewClassRef.SelectedItem.Value))
						
							If strResult <> "Mapping successfully updated." Then
								lblErrorOnEditBMemMsg.Text = "Error Updating Class Membership: " & strResult
								Exit Sub
							End If
						End If
					Next
				Case "EDITMEMG"
					For loopIdx = 0 To CInt(Request.Form("Gct")) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMemberIDRef = FindControl("rptGMembers:_ctl" & intCtrlNum & ":lblGroupID")
						ddlNewClassRef = FindControl("rptGMembers:_ctl" & intCtrlNum & ":ddlGMemOC")
						
						If lblMemberIDRef Is Nothing Then
							lblErrorOnEditGMemMsg.Text = "Page Error While Updating Class Membership.  Aborting."
							Exit Sub
						End If
						
						If ddlNewClassRef Is Nothing Then
							lblErrorOnEditGMemMsg.Text = "Page Error While Updating Class Membership.  Aborting."
							Exit Sub
						End If
						
						If ddlNewClassRef.SelectedItem.Value <> "0" Then
							'case where other group has been selected, else do nothing.
							'member reassignment is handled by a resource permission manager method.
							strResult = rpmObject.MapGroupToClass(CInt(lblMemberIDRef.Text), CInt(ddlNewClassRef.SelectedItem.Value))
						
							If strResult <> "Mapping successfully updated." Then
								lblErrorOnEditGMemMsg.Text = "Error Updating Class Membership: " & strResult
								Exit Sub
							End If
						End If
					Next
				Case "EDITMEMU"
					For loopIdx = 0 To CInt(Request.Form("Gct")) - 1
						intCtrlNum = (loopIdx * 2) + 1
						
						lblMemberIDRef = FindControl("rptUMembers:_ctl" & intCtrlNum & ":lblUserID")
						ddlNewClassRef = FindControl("rptUMembers:_ctl" & intCtrlNum & ":ddlUMemOC")
						
						If lblMemberIDRef Is Nothing Then
							lblErrorOnEditUMemMsg.Text = "Page Error While Updating Class Membership.  Aborting."
							Exit Sub
						End If
						
						If ddlNewClassRef Is Nothing Then
							lblErrorOnEditUMemMsg.Text = "Page Error While Updating Class Membership.  Aborting."
							Exit Sub
						End If
						
						If ddlNewClassRef.SelectedItem.Value <> "0" Then
							'case where other group has been selected, else do nothing.
							'member reassignment is handled by a resource permission manager method.
							strResult = rpmObject.MapSiteUserToClass(CInt(lblMemberIDRef.Text), CInt(ddlNewClassRef.SelectedItem.Value))
						
							If strResult <> "Mapping successfully updated." Then
								lblErrorOnEditUMemMsg.Text = "Error Updating Class Membership: " & strResult
								Exit Sub
							End If
						End If
					Next
				Case Else
					strPageState = "LIST"
			End Select
			
		Else
			strPageState = "LIST"	
		End If
	End Sub

	Sub AssignMemToClass_Command(s As Object, e As CommandEventArgs)
		If blnACGrant Then
			'assigns a specified nonmember to the specified class, outputs to source pagestate (from EDITMEMB, EDITMEMG, and EDITMEMU)
			strPageState = e.CommandName
			strClassID = e.CommandArgument
			
			Dim strResult As String
			
			Select Case strPageState
				Case "EDITMEMB"
					If ddlBNonMemList.SelectedItem.Value <> "0" Then
						'case where non-member broker is selected, else do nothing.
						'broker assignmnent is handled by resource permission manager method.
						strResult = rpmObject.MapBrokerToClass(CInt(ddlBNonMemList.SelectedItem.Value), CInt(strClassID))
						
						If strResult <> "Mapping successfully updated." Then
							lblErrorOnEditBMemMsg.Text = "Error Updating Class Membership: " & strResult
							Exit Sub
						End If
					End If
				Case "EDITMEMG"
					If ddlGNonMemList.SelectedItem.Value <> "0" Then
						'case where non-member group is selected, else do nothing.
						'group assignmnent is handled by resource permission manager method.
						strResult = rpmObject.MapGroupToClass(CInt(ddlGNonMemList.SelectedItem.Value), CInt(strClassID))
						
						If strResult <> "Mapping successfully updated." Then
							lblErrorOnEditGMemMsg.Text = "Error Updating Class Membership: " & strResult
							Exit Sub
						End If
					End If
				Case "EDITMEMU"
					If ddlUNonMemList.SelectedItem.Value <> "0" Then
						'case where non-member site user is selected, else do nothing.
						'site user assignmnent is handled by resource permission manager method.
						strResult = rpmObject.MapSiteUserToClass(CInt(ddlUNonMemList.SelectedItem.Value), CInt(strClassID))
						
						If strResult <> "Mapping successfully updated." Then
							lblErrorOnEditUMemMsg.Text = "Error Updating Class Membership: " & strResult
							Exit Sub
						End If
					End If
				Case Else
					strPageState = "LIST"
					
			End Select
		Else
			strPageState = "LIST"
		End If 
	End Sub
	
	Sub CreateClass_Click(s As Object, e As EventArgs)
		If blnACEdit Then
			'creates new class, outputs to EDITPERM for new class (from ADD)
			Dim strResult As String
			
			If Trim(txtAddName.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Class: A Class name must be supplied."
				strPageState = "ADD"
				Exit Sub
			End If
			
			If Trim(txtAddDesc.Text) = "" Then
				lblErrorOnAddMsg.Text = "Error Creating Class: A Class description must be supplied."
				strPageState = "ADD"
				Exit Sub
			End If
			
			'create class record.  Procedure performed by Resource Permission Manager method.
			strResult = rpmObject.AddClass(txtAddName.Text, txtAddDesc.Text)
			
			If strResult <> "Usage Class successfully added" Then
				lblErrorOnAddMsg.Text = "Error Creating Class: " & strResult
				strPageState = "ADD"
				Exit Sub
			End If
			
			'class created, get new ID and add user supplied permissions 
			strDBQuery = "SELECT class_id FROM UsageClasses WHERE name = @Name;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtAddName.Text)
			
			strClassID = cmdDBQuery.ExecuteScalar()
			strPageState = "EDITPERM"
			
			If blnACGrant Then
				Dim loopIdx, intCtrlNum As Integer
				Dim lblAddResourceIDRef, lblAddResourceTypeRef As Label
				Dim chkAddCanReadRef, chkAddCanEditRef, chkAddCanDeleteRef, chkAddCanGrantRef As Checkbox
				Dim txtAddPriorityRef As TextBox
				
				For loopIdx = 0 To CInt(Request.Form("Gct")) - 1
					intCtrlNum = (loopIdx * 2) + 1
				
					lblAddResourceIDRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":lblAddResourceID")
					lblAddResourceTypeRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":lblAddResourceType")
					chkAddCanReadRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanRead")
					chkAddCanEditRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanEdit")
					chkAddCanDeleteRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanDelete")
					chkAddCanGrantRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":chkAddCanGrant")
					txtAddPriorityRef = FindControl("rptAddPerms:_ctl" & intCtrlNum & ":txtAddPriority")
					
					If lblAddResourceIDRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [1]"
						Exit Sub
					End If
					
					If lblAddResourceTypeRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [2]"
						Exit Sub
					End If
					
					If chkAddCanReadRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [3]"
						Exit Sub
					End If
					
					If chkAddCanEditRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [4]"
						Exit Sub
					End If
					
					If chkAddCanDeleteRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [5]"
						Exit Sub
					End If
					
					If chkAddCanGrantRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [6]"
						Exit Sub
					End If
					
					If txtAddPriorityRef Is Nothing Then
						lblErrorOnEditPermMsg.Text = "Page Error While Creating Class Permissions.  Aborting. [7]"
						Exit Sub
					Else
						If Not IsNumeric(txtAddPriorityRef.Text) Then
							lblErrorOnEditPermMsg.Text = "Error Updating Class: Priority values must be expressed numerically."
							Exit Sub
						ElseIf Math.Abs(CInt(txtAddPriorityRef.Text)) > 20 Then
							lblErrorOnEditPermMsg.Text = "Error Updating Class: Priority value must be between -20 and +20."
							Exit Sub
						End If
					End If
					
					If chkAddCanReadRef.Checked Or chkAddCanEditRef.Checked Or chkAddCanDeleteRef.Checked Or chkAddCanGrantRef.Checked Then
						'case where some permissions have been set (create mapping), else do nothing
						'mapping creation is handled by Resource Permission Manager method
						
						If lblAddResourceTypeRef.Text = "DEVICE" Then
							'case where resource is a device (include priority)
							strResult = rpmObject.MapClassToResource(CInt(lblAddResourceIDRef.Text), CInt(strClassID), chkAddCanReadRef.Checked, chkAddCanEditRef.Checked, chkAddCanGrantRef.Checked, chkAddCanDeleteRef.Checked, CInt(txtAddPriorityRef.Text))
						Else
							strResult = rpmObject.MapClassToResource(CInt(lblAddResourceIDRef.Text), CInt(strClassID), chkAddCanReadRef.Checked, chkAddCanEditRef.Checked, chkAddCanGrantRef.Checked, chkAddCanDeleteRef.Checked)
						End If
						
						If strResult <> "Mapping successfully added." Then
							lblErrorOnEditPermMsg.Text = "Error Updating Class: " & strResult & " (Item " & (loopIdx + 1) & ")"
							Exit Sub
						End If
					End If
				Next
			End If
		Else
			strPageState = "LIST"
		End If
	End Sub

	Sub ExpandView_Command(s As Object, e As CommandEventArgs)
		strPageState="EDITPERM"
		strClassID = e.CommandArgument
		Session("PermListViewPref") = "all"
	End Sub
	
	Sub LimitView_Command(s As Object, e As CommandEventArgs)
		strPageState="EDITPERM"
		strClassID = e.CommandArgument
		Session("PermListViewPref") = "limit"
	End Sub
	
	Sub ShowEditMem_Command(s As Object, e As CommandEventArgs)
		strClassID = e.CommandArgument
		
		Select e.CommandName
			Case "BROKERS"
				strPageState = "EDITMEMB"
				lblErrorOnEditBMemMsg.Text = ""
			Case "GROUPS"
				strPageState = "EDITMEMG"
				lblErrorOnEditGMemMsg.Text = ""
			Case "SUSERS"
				strPageState = "EDITMEMU"
				lblErrorOnEditUMemMsg.Text = ""
			Case Else
				strPageState = "LIST"
		End Select
	End Sub
	
	Sub ShowClassPerm_Command(s As Object, e As CommandEventArgs)
		strPageState="EDITPERM"
		lblErrorOnEditPermMsg.Text = ""
		strClassID=e.CommandArgument
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnACRead Then  %>
    <form EncType="multipart/form-data" Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					Usage Classes:
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
									Text="Create New Class"
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
								ID="rptClasses"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">Usage Class</font>
											</th>
											<th align="center">
												<font class="regular">Members</font><br>
												<font class="extra-small">(Brokers/Groups/Site Users)</font>
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
														CommandArgument='<%#Container.DataItem("class_id")%>'
														OnCommand="ShowClassPerm_Command"
														Runat="Server" />
												</font>
											</td>
											<td align="center">
												<font class="regular"> 
													<%#Container.DataItem("amt_member_brokers")%>
													/
													<%#Container.DataItem("amt_member_groups")%>
													/
													<%#Container.DataItem("amt_member_susers")%>
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
						CASE "EDITPERM"
						'usage class infor and permission edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEditPermTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									ID="lbNavShowMem"
									Text="View Class Membership"
									OnCommand="ShowEditMem_Command"
									Runat="Server"/>
								|
								<asp:LinkButton
									Text="View All Classes"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnACEdit Then%>
									<asp:LinkButton
										Text="Create New Class"
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
														ID="txtEditPermName"
														Columns="30"
														MaxLength="100"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<asp:LinkButton
														ID="lbShowMem"
														Text="Membership Information"
														OnCommand="ShowEditMem_Command"
														Runat="Server"/>
												
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<table border=0 cellspacing=0 cellpadding=0 width=100%>
													<tr>
														<td valign="top">
															<font class="regular">
																<b>Description:</b>
															</font>	
														</td>
														<td>
															<font class="regular">
																<asp:TextBox
																	ID="txtEditPermDesc"
																	TextMode="MultiLine"
																	Columns="45"
																	Rows="3"
																	MaxLength="1000"
																	Wrap="True"
																	Runat="Server" />
																<br>&nbsp;
															</font>
														</td>
													
													</tr>
												</table>	
												
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
													
													
														<!--if resource is not a device-->
														<asp:Repeater
															ID="rptClassPerms"
															Runat="Server">
															<HeaderTemplate>
																<table border=1 cellspacing=0 cellpadding=0 width=100%>
																	<tr>
																		<th>
																			<font class="regular">
																				Resource
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
											 									&nbsp;<a href="/admin/system-resources.aspx?rid=<%#Container.DataItem("resource_id")%>" target=main><%#Container.DataItem("name")%></a>
											 									<asp:Label
											 										ID="lblMappingID"
											 										Visible="False"
											 										Text='<%#DBNullFilter(Container.DataItem("mapping_id"), "0")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblResourceType"
											 										Visible="False"
											 										Text='<%#Container.DataItem("type")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblResourceID"
											 										Visible="False"
											 										Text='<%#Container.DataItem("resource_id")%>'
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
											 										Visible='<%#ValueCheck(Container.DataItem("type"), "DEVICE", "True", "False")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblNoPriority"
											 										Text="N/A"
											 										Visible='<%#ValueCheck(Container.DataItem("type"), "DEVICE", "False", "True")%>' 
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
														
														<input type=hidden name=Gct value=<%=rptClassPerms.Items.Count()%>
												
													&nbsp;
													<asp:Label 
														ID="lblErrorOnEditPermMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2 align=center>
												<font class="regular">
													<%If blnACEdit Then 'since edit is prereq for delete%>
														<asp:LinkButton 
															ID="lbUpdate"
															Text="Update Information"
															onCommand="UpdateClass_Command"
															Runat="Server" />
														<%If Not (strClassID = "1" Or strClassID = "2") And blnACDelete Then%>
														<!--current class is not Administrator or Guests-->
														|
														<asp:LinkButton
															ID="lbDelete"
															Text="Remove Class"
															onCommand="RemoveClass_Command"
															Runat="Server" />
														<%End If%>
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
						CASE "EDITMEMB"
						'class membership (broker) edit interface code
					%>	
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblBMemberTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									ID="lbBNavShowPerms"
									Text="View Class Permissions"
									OnCommand="ShowClassPerm_Command"
									Runat="Server"/>
								|
								<asp:LinkButton
									Text="View All Classes"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnACEdit Then%>
									<asp:LinkButton
										Text="Create New Class"
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
									<font class="regular"><b>Class Membership</b></font>
								</td>
							</tr>
							<tr>
								<td align="center">
									<font class="small">
										Service Brokers
										|
										<asp:LinkButton
											ID="lbBToGMembers"
											Text="Broker Groups"
											onCommand="ShowEditMem_Command"
											Runat="Server" />
										|
										<asp:LinkButton
											ID="lbBToUMembers"
											Text="Site Users"
											onCommand="ShowEditMem_Command"
											Runat="Server" />
										<br>&nbsp;	
									</font>
								</td>
							</tr>
							<tr>
								<td>								
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td align="left">
												<font class="regular">
													<b>Service Brokers:</b>
												</font>
											</td>
											<td align="right" width=40%>
												<font class="regular">
													Members:
													<asp:Label
														ID="lblBMemberCnt"
														Runat="Server" />	
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">																			
												
													<asp:Repeater
														ID="rptBMembers"
														Runat="Server">
														<HeaderTemplate>
															<table border=1 cellpadding=2 cellspacing=0 width=100%>
																<tr>
																	<th align="left">
																		<font class="regular">
																			Name
																		</font>
																	</th>
																	<th align="left">
																		<font class="regular">
																			Contact Name
																		</font>
																	</th>
																	<th align="left">
																		<font class="regular">
																			Status
																		</font>
																	</th>
																	<%If blnACGrant Then%>
																		<th align="left">
																			<font class="regular">
																				Reassign To...
																			</font>
																		</th>
																	<%End If%>
																</tr>
														</HeaderTemplate>
														
														<ItemTemplate>
											 					<tr>
											 						<td>
											 							<font class="regular">
											 								<a href="/admin/service-brokers.aspx?bid=<%#Container.DataItem("broker_id")%>" target="main"><%#Container.DataItem("name")%></a>
											 								<asp:Label
											 									ID="lblBrokerID"
											 									Text='<%#Container.DataItem("broker_id")%>'
											 									Visible="False"
											 									Runat="Server" />
											 							</font>
											 						</td>
											 						<td>
											 							<font class="regular">
											 								<a href="mailto:<%#Container.DataItem("contact_email")%>"><%#Container.DataItem("contact_first_name")%>&nbsp;<%#Container.DataItem("contact_last_name")%></a>
											 							</font>
											 						</td>
																	<td>
																		<font class="regular">
																			<%#DisplayStatus(Container.DataItem("is_active"))%>
																		</font>
																	</td>
																	<%If blnACGrant Then%>
																		<td>
																			<font class="regular">
																				<asp:DropDownList
																					ID="ddlBMemOC"
																					DataSource='<%#dstClasses%>'
																					DataTextField="name"
																					DataValueField="class_id"
																					Runat="Server" />
																					
																			</font>
																		</td>
																	<%End If%>
											 					</tr>
											 					
														</ItemTemplate>
													
														<SeparatorTemplate>
														</SeparatorTemplate>
														
														<FooterTemplate>			
															</table>
														</FooterTemplate>
								
													</asp:Repeater>
													
													<input type=hidden name=Gct value=<%=rptBMembers.Items.Count()%>
												
													&nbsp;
													<asp:Label 
														ID="lblErrorOnEditBMemMsg"
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
															ID="lbBMemUpdate"
															Text="Update Information"
															onCommand="UpdateClassMembership_Command"
															Runat="Server" />
													<%End If%>
													<%If blnACGrant And blnACDelete And Not (strClassID = "1" Or strClassID = "2") Then%>
													|
													<%End If%>
													<%If Not (strClassID = "1" Or strClassID = "2") And blnACDelete Then%>
													<!--current class is not Administrator or Guests-->
														<asp:LinkButton
															ID="lbBMemDelete"
															Text="Remove Class"
															onCommand="RemoveClass_Command"
															Runat="Server" />
													<%End If%>
													<br>
													&nbsp;
												</font>
											</td>
										</tr>
										<%If blnACGrant Then%>
											<tr>
												<td align="center">
													<font class="regular">
														<b>Add New Member:</b>
														<asp:DropDownList
															ID="ddlBNonMemList"
															Runat="Server" />
													</font>
												</td>
												<td>
													<font class="regular">
														<asp:LinkButton
															ID="lbBMemAssign"
															Text="Assign to Class"
															OnCommand="AssignMemToClass_Command"
															Runat="Server" />
													</font>
												</td>
											</tr>
										<%End If%>
									</table>
								</td>
							</tr>
						</table>
					
						</center>
					<%
						CASE "EDITMEMG"
						'class membership (group) edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblGMemberTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									ID="lbGNavShowPerms"
									Text="View Class Permissions"
									OnCommand="ShowClassPerm_Command"
									Runat="Server"/>
								|
								<asp:LinkButton
									Text="View All Classes"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnACEdit Then%>
									<asp:LinkButton
										Text="Create New Class"
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
									<font class="regular"><b>Class Membership</b></font>
								</td>
							</tr>
							<tr>
								<td align="center">
									<font class="small">
										<asp:LinkButton
											ID="lbGToBMembers"
											Text="Service Brokers"
											onCommand="ShowEditMem_Command"
											Runat="Server" />
										|
										Broker Groups
										|
										<asp:LinkButton
											ID="lbGToUMembers"
											Text="Site Users"
											onCommand="ShowEditMem_Command"
											Runat="Server" />
										<br>&nbsp;	
									</font>
								</td>
							</tr>
							<tr>
								<td>								
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td align="left">
												<font class="regular">
													<b>Broker Defined Groups:</b>
												</font>
											</td>
											<td align="right" width=40%>
												<font class="regular">
													Members:
													<asp:Label
														ID="lblGMemberCnt"
														Runat="Server" />	
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">																			
												
													<asp:Repeater
														ID="rptGMembers"
														Runat="Server">
														<HeaderTemplate>
															<table border=1 cellpadding=2 cellspacing=0 width=100%>
																<tr>
																	<th align="left">
																		<font class="regular">
																			Name
																		</font>
																	</th>
																	<th align="left">
																		<font class="regular">
																			Group Owner
																		</font>
																	</th>
																	<th align="left">
																		<font class="regular">
																			Status
																		</font>
																	</th>
																	<%If blnACGrant Then%>
																		<th align="left">
																			<font class="regular">
																				Reassign To...
																			</font>
																		</th>
																	<%End If%>
																</tr>
														</HeaderTemplate>
														
														<ItemTemplate>
											 					<tr>
											 						<td>
											 							<font class="regular">
											 								<a href="/admin/service-brokers.aspx?gid=<%#Container.DataItem("group_id")%>" target="main"><%#Container.DataItem("group_name")%></a>
											 								<asp:Label
											 									ID="lblGroupID"
											 									Text='<%#Container.DataItem("group_id")%>'
											 									Visible="False"
											 									Runat="Server" />
											 							</font>
											 						</td>
											 						<td>
											 							<font class="regular">
											 								<a href="/admin/service-brokers.aspx?bid=<%#Container.DataItem("broker_id")%>" target="main"><%#Container.DataItem("broker_name")%></a>
											 							</font>
											 						</td>
																	<td>
																		<font class="regular">
																			<%#DisplayStatus(Container.DataItem("is_active"))%>
																		</font>
																	</td>
																	<%If blnACGrant Then%>
																		<td>
																			<font class="regular">
																				<asp:DropDownList
																					ID="ddlGMemOC"
																					DataSource='<%#dstClasses%>'
																					DataTextField="name"
																					DataValueField="class_id" 
																					Runat="Server" />																				
																			</font>
																		</td>
																	<%End If%>
											 					</tr>
											 					
														</ItemTemplate>
													
														<SeparatorTemplate>
														</SeparatorTemplate>
														
														<FooterTemplate>			
															</table>
														</FooterTemplate>
								
													</asp:Repeater>
													
													<input type=hidden name=Gct value=<%=rptGMembers.Items.Count()%>
												
													&nbsp;
													<asp:Label 
														ID="lblErrorOnEditGMemMsg"
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
															ID="lbGMemUpdate"
															Text="Update Information"
															onCommand="UpdateClassMembership_Command"
															Runat="Server" />
													<%End If%>
													<%If blnACGrant And blnACDelete And Not (strClassID = "1" Or strClassID = "2") Then %>
													|
													<%End If%>
													<%If Not (strClassID = "1" Or strClassID = "2") And blnACDelete Then%>
													<!--current class is not Administrator or Guests-->
													<asp:LinkButton
														ID="lbGMemDelete"
														Text="Remove Class"
														onCommand="RemoveClass_Command"
														Runat="Server" />
													<%End If%>
													<br>
													&nbsp;
												</font>
											</td>
										</tr>
										<%If blnACGrant Then%>
											<tr>
												<td align="center">
													<font class="regular">
														<b>Add New Member:</b>
														<asp:DropDownList
															ID="ddlGNonMemList"
															Runat="Server" />
													</font>
												</td>
												<td>
													<font class="regular">
														<asp:LinkButton
															ID="lbGMemAssign"
															Text="Assign to Class"
															OnCommand="AssignMemToClass_Command"
															Runat="Server" />
													</font>
												</td>
											</tr>
										<%End If%>
									</table>
								</td>
							</tr>
						</table>
					
						</center>
					<%
						CASE "EDITMEMU"
						'class membership (site users) edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblUMemberTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									ID="lbUNavShowPerms"
									Text="View Class Permissions"
									OnCommand="ShowClassPerm_Command"
									Runat="Server"/>
								|
								<asp:LinkButton
									Text="View All Classes"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnACEdit Then%>
									<asp:LinkButton
										Text="Create New Class"
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
									<font class="regular"><b>Class Membership</b></font>
								</td>
							</tr>
							<tr>
								<td align="center">
									<font class="small">
										<asp:LinkButton
											ID="lbUToBMembers"
											Text="Service Brokers"
											onCommand="ShowEditMem_Command"
											Runat="Server" />
										|
										<asp:LinkButton
											ID="lbUToGMembers"
											Text="Broker Groups"
											onCommand="ShowEditMem_Command"
											Runat="Server" />
										|
										Site Users
										<br>&nbsp;	
									</font>
								</td>
							</tr>
							<tr>
								<td>								
									<table border=0 cellpadding=0 cellspacing=3 width=100%>
										<tr>
											<td align="left">
												<font class="regular">
													<b>Site Users:</b>
												</font>
											</td>
											<td align="right" widht=40%>
												<font class="regular">
													Members:
													<asp:Label
														ID="lblUMemberCnt"
														Runat="Server" />	
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">																			
												
													<asp:Repeater
														ID="rptUMembers"
														Runat="Server">
														<HeaderTemplate>
															<table border=1 cellpadding=2 cellspacing=0 width=100%>
																<tr>
																	<th align="left">
																		<font class="regular">
																			Name
																		</font>
																	</th>
																	<th align="left">
																		<font class="regular">
																			UserName
																		</font>
																	</th>
																	<th align="left">
																		<font class="regular">
																			Status
																		</font>
																	</th>
																	<%If blnACGrant Then%>
																		<th align="left">
																			<font class="regular">
																				Reassign To...
																			</font>
																		</th>
																	<%End If%>
																</tr>
														</HeaderTemplate>
														
														<ItemTemplate>
											 					<tr>
											 						<td>
											 							<font class="regular">
											 								<a href="/admin/site-users.aspx?uid=<%#Container.DataItem("user_id")%>" target="main"><%#Container.DataItem("first_name")%>&nbsp;<%#Container.DataItem("last_name")%></a>
											 								<asp:Label
											 									ID="lblUserID"
											 									Text='<%#Container.DataItem("user_id")%>'
											 									Visible="False"
											 									Runat="Server" />
											 							</font>
											 						</td>
											 						<td>
											 							<font class="regular">
											 								<%#Container.DataItem("username")%>
											 							</font>
											 						</td>
																	<td>
																		<font class="regular">
																			<%#DisplayStatus(Container.DataItem("is_active"))%>
																		</font>
																	</td>
																	<%If blnACGrant Then%>
																		<td>
																			<font class="regular">
																				<asp:DropDownList
																					ID="ddlUMemOC"
																					DataSource='<%#dstClasses%>'
																					DataTextField="name"
																					DataValueField="class_id" 
																					Runat="Server" />
																			</font>
																		</td>
																	<%End If%>
											 					</tr>
											 					
														</ItemTemplate>
													
														<SeparatorTemplate>
														</SeparatorTemplate>
														
														<FooterTemplate>			
															</table>
														</FooterTemplate>
								
													</asp:Repeater>
													
													<input type=hidden name=Gct value=<%=rptUMembers.Items.Count()%>
												
													&nbsp;
													<asp:Label 
														ID="lblErrorOnEditUMemMsg"
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
															ID="lbUMemUpdate"
															Text="Update Information"
															onCommand="UpdateClassMembership_Command"
															Runat="Server" />
													<%End If%>
													<%If blnACGrant And blnACDelete And Not (strClassID = "1" Or strClassID = "2") Then %> 
													|
													<%End If%>
													<%If Not (strClassID = "1" Or strClassID = "2") And blnACDelete Then%>
													<!--current class is not Administrator or Guests-->
													<asp:LinkButton
														ID="lbUMemDelete"
														Text="Remove Class"
														onCommand="RemoveClass_Command"
														Runat="Server" />
													<%End If%>
													<br>
													&nbsp;
												</font>
											</td>
										</tr>
										<%If blnACGrant Then%>
											<tr>
												<td align="center">
													<font class="regular">
														<b>Add New Member:</b>
														<asp:DropDownList
															ID="ddlUNonMemList"
															Runat="Server" />
													</font>
												</td>
												<td>
													<font class="regular">
														<asp:LinkButton
															ID="lbUMemAssign"
															Text="Assign to Class"
															OnCommand="AssignMemToClass_Command"
															Runat="Server" />
													</font>
												</td>
											</tr>
										<%End If%>
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
							&nbsp;<b>Create Usage Class</b>
							</font>
							<hr size=1>
								<div align="right" class="extra-small">
									<asp:LinkButton
										Text="View All Classes"
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
													<font class="regular">
														<b>Name:</b>
														<asp:TextBox
															ID="txtAddName"
															Columns="30"
															MaxLength="100"
															Runat="Server" />
													</font>
												</td>
											</tr>
											<tr>
												<td>
													<font class="regular">
														<table border=0 cellspacing=0 cellpadding=0 width=100%>
															<tr>
																<td valign="top">
																	<font class="regular">
																		<b>Description:</b>
																	</font>
																</td>
																<td>
																	<font class="regular">
																		<asp:TextBox
																			ID="txtAddDesc"
																			TextMode="MultiLine"
																			Columns="45"
																			Rows="3"
																			MaxLength="1000"
																			Wrap="True"
																			Runat="Server" />
																	</font>
																</td>
															</tr>
														</table>
														&nbsp;
													</font>
													
												</td>
											</tr>
											<tr>
												<td colspan=2>
													<font class="regular">
														<%If blnACGrant Then%>
															<b>Permissions:</b>
														<%Else
															rptAddPerms.Visible = False
														End If%>
														<asp:Repeater
															ID="rptAddPerms"
															Runat="Server">
															<HeaderTemplate>
																<table border=1 cellspacing=0 cellpadding=0 width=100%>
																	<tr>
																		<th>
																			<font class="regular">
																				Resource
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
											 									&nbsp;<a href="/admin/system-resources.aspx?rid=<%#Container.DataItem("resource_id")%>" target=main><%#Container.DataItem("name")%></a>
											 									<asp:Label
											 										ID="lblAddResourceID"
											 										Visible="False"
											 										Text='<%#Container.DataItem("resource_id")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblAddResourceType"
											 										Visible="False"
											 										Text='<%#Container.DataItem("type")%>'
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
											 							<td align=center>
											 								<font class="regular">
											 									<asp:TextBox
											 										ID="txtAddPriority"
											 										Text="0"
											 										Columns="3"
											 										MaxLength="3"
											 										Visible='<%#ValueCheck(Container.DataItem("type"), "DEVICE", "True", "False")%>'
											 										Runat="Server" />
											 									<asp:Label
											 										ID="lblAddNoPriority"
											 										Text="N/A"
											 										Visible='<%#ValueCheck(Container.DataItem("type"), "DEVICE", "False", "True")%>'
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
																Text="Create Usage Class"
																onClick="CreateClass_Click"
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
