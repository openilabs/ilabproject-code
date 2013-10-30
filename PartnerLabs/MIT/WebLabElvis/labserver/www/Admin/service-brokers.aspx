<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="WebLabDataManagers.WebLabDataManagers" %>
<%@ Import Namespace="WebLabCustomDataTypes.WebLabCustomDataTypes" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery As String
	Dim loopIdx As Integer
	DIm htClassIDOrder As New HashTable()
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim dstClasses As DataSet
	Dim dadClasses As SqlDataAdapter
	Dim strPageState, strErrorMsg, strBrokerID, strGroupID, strClassID, strClassName As String
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
			
				If Not Request.QueryString("bid") Is Nothing And IsNumeric(Request.QueryString("bid")) Then
					strBrokerID = Request.QueryString("bid")
					strPageState = "EDITBROKER"
					
				ElseIf Not Request.QueryString("gid") Is Nothing And IsNumeric(Request.QueryString("gid")) Then
					strGroupID = Request.QueryString("gid")
					strPageState = "EDITGROUP"
					
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
					strDBQuery = "SELECT b.broker_id, b.class_id, b.name, b.is_active, b.date_modified, c.name AS class_name FROM Brokers b JOIN UsageClasses c ON b.class_id = c.class_id;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					
					rptBrokers.DataSource = cmdDBQuery.ExecuteReader()
					rptBrokers.DataBind()
				
				Case "EDITBROKER"
					ddlEditBrokerOCList.Items.Clear()
				
					strDBQuery = "SELECT b.broker_id, b.class_id, b.name, b.broker_server_id, b.broker_passkey, b.server_passkey, b.description, b.contact_first_name, b.contact_last_name, b.contact_email, b.is_active, b.notify_location, b.date_created, b.date_modified, c.name AS class_name FROM Brokers b JOIN UsageClasses c ON b.class_id = c.class_id WHERE b.broker_id = @BrokerID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() Then
						lblEditBrokerTitle.Text = dtrDBQuery("name")
						txtEditBrokerName.Text = dtrDBQuery("name")
						If CBool(dtrDBQuery("is_active")) Then
							ddlEditBrokerStatus.SelectedIndex = 0
						Else
							ddlEditBrokerStatus.SelectedIndex = 1
						End If
						
						strClassID = dtrDBQuery("class_id")
						strClassName = dtrDBquery("class_name")
						txtEditBrokerDesc.Text = dtrDBQuery("description")
						txtEditBrokerCFName.Text = dtrDBQuery("contact_first_name")
						txtEditBrokerCLName.Text = dtrDBQuery("contact_last_name")
						txtEditBrokerCEmail.Text = dtrDBQuery("contact_email")
						txtEditBrokerID.Text = dtrDBQuery("broker_server_id")
						txtEditBrokerPassKey.Text = dtrDBQuery("broker_passkey")
						txtEditBrokerLSPassKey.Text = DBNullFilter(dtrDBQuery("server_passkey"), "")
						txtEditBrokerWSLoc.Text = dtrDBQuery("notify_location")
						
						lblEditBrokerDateCreated.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblEditBrokerDateMod.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
						
						lbAddGroup.CommandArgument = dtrDBQuery("broker_id")
						lbEditBrokerUpdate.CommandArgument = dtrDBQuery("broker_id")
						lbEditBrokerDelete.CommandArgument = dtrDBQuery("broker_id")
							
					End If
					dtrDBQuery.Close()
					
					strDBQuery = "SELECT 0 As class_id, 'Select New Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					loopIdx = 0
					Do While dtrDBQuery.Read()
						If dtrDBQuery("class_id") <> strClassID Then
							ddlEditBrokerOCList.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("class_id")))	
						End If
						
						If dtrDBQuery("class_id") <> "0" Then
							htClassIDOrder.Item(dtrDBQuery("class_id")) = loopIdx
							loopIdx = loopIdx + 1		
						End If
					Loop
					
					dtrDBQuery.Close()
					
					dstClasses = New DataSet()
					strDBQuery = "SELECT class_id, name FROM UsageClasses ORDER BY class_id, name DESC;"
					dadClasses = New SqlDataAdapter(strDBQuery, conWebLabLS)
					dadClasses.Fill(dstClasses, "Classes")
					
					strDBQuery = "SELECT COUNT(*) FROM Groups WHERE owner_id = @BrokerID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
					
					If CInt(cmdDBQuery.ExecuteScalar()) = 0 Then
						rptGroups.Visible = False
					Else
						rptGroups.Visible = True
						strDBQuery = "SELECT g.group_id, g.name, g.class_id, c.name AS class_name, g.is_active, g.date_modified FROM Groups g JOIN UsageClasses c ON g.class_id = c.class_id WHERE owner_id = @BrokerID;"
						cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
						cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
						rptGroups.DataSource = cmdDBQuery.ExecuteReader()
						rptGroups.DataBind()
					End If
					
				Case "EDITGROUP"
					ddlEGOCList.Items.Clear()
				
					strDBQuery = "SELECT g.group_id, g.name, g.owner_id, b.name AS broker_name, g.class_id, c.name AS class_name, g.description, g.is_active, g.date_created, g.date_modified FROM Groups g JOIN Brokers b ON g.owner_id = b.broker_id JOIN UsageClasses c ON g.class_id = c.class_id WHERE g.group_id = @GroupID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@GroupID", strGroupID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					If dtrDBQuery.Read() Then
						lblEGBTitle.text = dtrDBQuery("broker_name")
						lblEGGTitle.Text = dtrDBQuery("name")
						
						lbEGViewBroker.CommandArgument = dtrDBQuery("owner_id")
						lbEGAddGroup.CommandArgument = dtrDBQuery("owner_id")
						
						txtEGName.Text = dtrDBQuery("name")
						If CBool(dtrDBQuery("is_active")) Then
							ddlEGStatus.SelectedIndex = 0
						Else
							ddlEGStatus.SelectedIndex = 1
						End If

						strClassID = dtrDBQuery("class_id")
						strClassName = dtrDBQuery("class_name")
						txtEGDesc.Text = dtrDBQuery("description")
						lblEGDateCreated.Text = DisplayDate(dtrDBQuery("date_created")) & " " & DisplayTime(dtrDBQuery("date_created"))
						lblEGDateMod.Text = DisplayDate(dtrDBQuery("date_modified")) & " " & DisplayTime(dtrDBQuery("date_modified"))
						
						lbEGUpdate.CommandArgument = dtrDBQuery("group_id")
						lbEGDelete.CommandArgument = dtrDBQuery("group_id")
						lbEGDelete.CommandName = dtrDBQuery("owner_id")
											
					End If
					dtrDBQuery.Close()
					
					strDBQuery = "SELECT 0 As class_id, 'Select New Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses WHERE NOT class_id = @ClassID ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@ClassID", strClassID)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					Do While dtrDBQuery.Read()
						ddlEGOCList.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("class_id")))	
					Loop
					dtrDBQuery.Close()
					
				
				Case "ADDGROUP"
					ddlAGClass.Items.Clear()
					
					strDBQuery = "SELECT name FROM Brokers WHERE broker_id = @BrokerID;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
					
					lblAGBTitle.Text = cmdDBQuery.ExecuteScalar()
					lbAGViewBroker.CommandArgument = strBrokerID
					lbAGCreate.CommandArgument = strBrokerID
					
					strDBQuery = "SELECT 0 As class_id, 'Select Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					Do While dtrDBQuery.Read()
						ddlAGClass.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("class_id")))	
					Loop
					dtrDBQuery.Close()
					
						
				Case "ADDBROKER"
					ddlABClass.Items.Clear()
					
					strDBQuery = "SELECT 0 As class_id, 'Select Class' As name UNION SELECT 0 As class_id, '-----' As name UNION SELECT class_id, name FROM UsageClasses ORDER BY class_id, name DESC;"
					cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
					dtrDBQuery = cmdDBQuery.ExecuteReader()
					
					Do While dtrDBQuery.Read()
						ddlABClass.Items.Add(New ListItem(dtrDBQuery("name"), dtrDBQuery("class_id")))	
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
	
	Sub UpdateBroker_Command(s As Object, e As CommandEventArgs)
		'updates information on Edit Broker page
		If blnAMEdit Then
			Dim strResult As String
			Dim loopIdx, intCtrlNum As Integer
			
			strBrokerID = e.CommandArgument
			strPageState = "EDITBROKER"
			
			'validate broker inputs
			If Trim(txtEditBrokerName.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A Name must be provided."
				Exit Sub
			Else
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT broker_id FROM Brokers WHERE name = @Name AND NOT broker_id = @BrokerID);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@Name", txtEditBrokerName.Text)
				cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
				
				If cmdDBQuery.ExecuteScalar() = "true" Then
					lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: The specified name is already in use.  Please select another."
					Exit Sub
				End If
			End If
			
			If Trim(txtEditBrokerDesc.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A Description must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditBrokerWSLoc.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A Broker Web Service URL must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditBrokerCFName.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A full Contact Name must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditBrokerCLName.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A full Contact Name must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditBrokerCEmail.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A Contact Email address must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditBrokerID.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A Broker Authentication ID must be provided."
				Exit Sub
			End If
			
			If Trim(txtEditBrokerLSPassKey.Text) = "" Then
				lblErrorOnEditBrokerMsg.Text = "Error Updating Broker: A Lab Server Passkey must be provided."
				Exit Sub
			End If
			
			'update non-class assignment broker information
			strDBQuery = "UPDATE Brokers SET name = @Name, description = @Desc, notify_location = @WSURL, contact_first_name = @CFName, contact_last_name = @CLName, contact_email = @CEmail, broker_server_id = @BAuthID, server_passkey = @LSPasskey, is_active = @Status, date_modified = GETDATE() WHERE broker_id = @BrokerID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtEditBrokerName.Text)
			cmdDBQuery.Parameters.Add("@Desc", txtEditBrokerDesc.Text)
			cmdDBQuery.Parameters.Add("@WSURL", txtEditBrokerWSLoc.Text)
			cmdDBQuery.Parameters.Add("@CFName", txtEditBrokerCFName.Text)
			cmdDBQuery.Parameters.Add("@CLName", txtEditBrokerCLName.Text)
			cmdDBQuery.Parameters.Add("@CEmail", txtEditBrokerCEmail.Text)
			cmdDBQuery.Parameters.Add("@BAuthID", txtEditBrokerID.Text)
			cmdDBQuery.Parameters.Add("@LSPasskey", txtEditBrokerLSPassKey.Text)
			cmdDBQuery.Parameters.Add("@Status", ddlEditBrokerStatus.SelectedItem.Value)
			cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
			
			Try
				cmdDBQuery.ExecuteNonQuery()
			Catch
				lblErrorOnEditBrokerMsg.Text = "Data Error While Updating Broker.  Aborting."
				Exit Sub
			End Try
			
			'update broker class assignment, if necessary and allowed
			If ddlEditBrokerOCList.SelectedItem.Value <> "0" And blnACGrant Then
				strResult = rpmObject.MapBrokerToClass(cInt(strBrokerID), CInt(ddlEditBrokerOCList.SelectedItem.Value))
				
				If strResult <> "Mapping successfully updated." Then 
					lblErrorOnEditBrokerMsg.Text = "Error While Updating Broker: " & strResult
					Exit Sub
				End If
			End If
			
			'group information update.
			Dim  lblEBGroupStatusRef As HiddenField
			Dim lblEBGroupIDRef, lblEBGClassIDRef As HiddenField
			Dim ddlEBOCListRef, ddlEBGStatusRef As DropDownList
			
			Dim strCtrlNum As String
			For loopIdx = 0 To cInt(Request.Form("Gct")) - 1
				intCtrlNum = (loopIdx * 2) + 1
				If intCtrlNum < 10 Then
				    strCtrlNum = "0" & IntCtrlNum
				Else
				     strCtrlNum = IntCtrlNum
				End If
				lblEBGroupIDRef = FindControl("rptGroups$ctl" & strCtrlNum & "$lblEditBrokerGroupID")
				lblEBGroupStatusRef = FindControl("rptGroups$ctl" & strCtrlNum & "$lblEditBrokerGroupStatus")
				lblEBGClassIDRef = FindControl("rptGroups$ctl" & strCtrlNum & "$lblEditBrokerGClassID")
				ddlEBOCListRef = FindControl("rptGroups$ctl" & strCtrlNum & "$ddlEditBrokerOCList")
				ddlEBGStatusRef = FindControl("rptGroups$ctl" & strCtrlNum & "$ddlEditBrokerGStatus")
				
				If lblEBGroupIDRef Is Nothing Then
					lblErrorOnEditBrokerMsg.Text = "Page Error While Updating Broker.  Aborting."
					Exit Sub
				End If
				
				If lblEBGroupStatusRef Is Nothing Then
					lblErrorOnEditBrokerMsg.Text = "Page Error While Updating Broker.  Aborting."
					Exit Sub
				End If
				
				If lblEBGClassIDRef Is Nothing Then
					lblErrorOnEditBrokerMsg.Text = "Page Error While Updating Broker.  Aborting."
					Exit Sub
				End If
				
				If ddlEBOCListRef Is Nothing Then
					lblErrorOnEditBrokerMsg.Text = "Page Error While Updating Broker.  Aborting."
					Exit Sub
				End If
				
				If ddlEBGStatusRef Is Nothing Then
					lblErrorOnEditBrokerMsg.Text = "Page Error While Updating Broker.  Aborting."
					Exit Sub
				End If
				
				'check and update class membership, if allowed.
				If ddlEBOCListRef.SelectedItem.Value <> lblEBGClassIDRef.Value And blnACGrant Then
					strResult = rpmObject.MapGroupToClass(CInt(lblEBGroupIDRef.Value), CInt(ddlEBOCListRef.SelectedItem.Value))
					
					If strResult <> "Mapping successfully updated." Then
						lblErrorOnEditBrokerMsg.Text = "Error Updating Broker Group: " & strResult
						Exit Sub
					End If
				End If
				
				'check and update status
				If lblEBGroupStatusRef.Value = "True" And ddlEBGStatusRef.SelectedItem.Value = "0" Then
					strResult = rpmObject.DeactivateGroup(CInt(lblEBGroupIDRef.Value))
					
					If strResult <> "SUCCESS" Then
						lblErrorOnEditBrokerMsg.Text = "Error Updating Broker Group Status.  Aborting."
						Exit Sub
					End If
					
				ElseIf lblEBGroupStatusRef.Value = "False" And ddlEBGStatusRef.SelectedItem.Value = "1" Then
					strResult = rpmObject.ActivateGroup(CInt(lblEBGroupIDRef.Value))
					
					If strResult <> "SUCCESS" Then
						lblErrorOnEditBrokerMsg.Text = "Error Updating Broker Group Status.  Aborting."
						Exit Sub
					End If
				End If
			Next
		Else
			strPageState = "LIST"
		End If
	End Sub
		
	Sub UpdateGroup_Command(s As Object, e As CommandEventArgs)
		'updates information on Edit Group page
		If blnAMEdit Then
			Dim strResult As String
			lblErrorOnEGMsg.Text = ""
			strGroupID = e.CommandArgument
			strPageState = "EDITGROUP"
			
			'validate inputs 
			If Trim(txtEGName.Text) = "" Then
				lblErrorOnEGMsg.Text = "Error Updating Broker Group: A Name must be provided."
				Exit Sub
			Else
				strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT group_id FROM Groups WHERE name = @Name AND NOT group_id = @GroupID);"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@Name", txtEGName.Text)
				cmdDBQuery.Parameters.Add("@GroupID", strGroupID)
				
				If cmdDBQuery.ExecuteScalar() = "true" Then
					lblErrorOnEGMsg.Text = "Error Updating Broker Group: The specified name is already in use.  Please select another."
					Exit Sub
				End If
			End If
			
			If Trim(txtEGDesc.Text) = "" Then
				lblErrorOnEGMsg.Text = "Error Updating Broker Group: A Description must be provided."
				Exit Sub
			End If
			
			'update non-class assignment information
			strDBQuery = "UPDATE Groups SET name = @Name, description = @Desc, is_active = @Status, date_modified = GETDATE() WHERE group_id = @GroupID;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@Name", txtEGName.Text)
			cmdDBQuery.Parameters.Add("@Desc", txtEGDesc.Text)
			cmdDBQuery.Parameters.Add("@Status", ddlEGStatus.SelectedItem.Value)
			cmdDBQuery.Parameters.Add("@GroupID", strGroupID)
			
			Try
				cmdDBQuery.ExecuteNonQuery()
			Catch
				lblErrorOnEGMsg.Text = "Data Error While Updating Broker Group.  Aborting."
				Exit Sub
			End Try
			
			'update class assignment, if allowed
			If ddlEGOCList.SelectedItem.Value <> "0" And blnACGrant Then
				strResult = rpmObject.MapGroupToClass(CInt(strGroupID), CInt(ddlEGOCList.SelectedItem.Value))
				
				If strResult <> "Mapping successfully updated." Then
					lblErrorOnEGMsg.Text = "Error Updating Broker Group: " & strResult
					Exit Sub
				End If
			End If
		Else
			strPageState = "LIST"
		End If
	End Sub
	
	Sub RemoveBroker_Command(s As Object, e As CommandEventArgs)
		'removes a broker, as well as dependent groups, from system, adjusts class membership tallies.
		If blnAMDelete Then
			Dim strResult As String
			lblErrorOnEditBrokerMsg.Text = ""
			
			strBrokerID = e.CommandArgument
			
			strResult = rpmObject.RemoveBroker(cInt(strBrokerID))
			
			If strResult <> "Broker successfully deleted." Then
				lblErrorOnEditBrokerMsg.Text = "Error Removing Broker: " & strResult
				strPageState = "EDITBROKER"
			Else
				strPageState = "LIST"
			End If	
		Else
			strPageState = "LIST"
		End If
	End Sub

	Sub RemoveGroup_Command(s As Object, e As CommandEventArgs)
		'removes a broker group from system and adjusts class membership tallies.
		If blnAMDelete Then
			Dim strResult As String
			lblErrorOnEGMsg.Text = ""
			
			strGroupID = e.CommandArgument
			strBrokerID = e.CommandName
			
			strResult = rpmObject.RemoveGroup(Cint(strGroupID))
			
			If strResult <> "Group successfully deleted." Then
				lblErrorOnEGMsg.Text = "Error Removing Broker Group: " & strResult
				strPageState = "EDITGROUP"
			Else
				strPageState = "EDITBROKER"
			End If
		Else
			strPageState = "LIST"
		End If
			
	End Sub
	
	Sub CreateGroup_Command(s As Object, e As CommandEventArgs)
		'creates a new broker group
		If blnAMEdit Then
			Dim strResult As String
			Dim intNewClassID As Integer
			lblErrorOnAGMsg.Text = ""
			strBrokerID = e.CommandArgument
			
			'validate inputs 
			If Trim(txtAGName.Text) = "" Then
				lblErrorOnAGMsg.Text = "Error Creating Broker Group: A Name must be specified."
				strPageState = "ADDGROUP"
				Exit Sub
			End If
			
			If blnACGrant Then
				If ddlAGClass.SelectedItem.Value = 0 Then
					lblErrorOnAGMsg.Text = "Error Creating Broker Group: A Usage Class must be speciifed."
					strPageState = "ADDGROUP"
					Exit Sub
				Else
					intNewClassID = CInt(ddlAGClass.SelectedItem.Value)
				End If
			Else
				intNewClassID = 2 'automaticall assign to the default Guest class
			End If
			
			If Trim(txtAGDesc.Text) = "" Then
				lblErrorOnAGMsg.Text = "Error Creating Broker Group: A Description must be provided."
				strPageState = "ADDGROUP"
				Exit Sub
			End If
			
			'inputs validated, creating group.
			strResult = rpmObject.AddGroup(cInt(strBrokerID), intNewClassID, txtAGName.Text,txtAGDesc.Text, True)
			
			If strResult <> "Group successully added." Then
				lblErrorOnAGMsg.Text = "Error Creating Broker Group: " & strResult
				strPageState = "ADDGROUP"
				Exit Sub
			End If
			
			strDBQuery = "SELECT group_id FROM Groups WHERE owner_id = @BrokerID AND name = @Name;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			cmdDBQuery.Parameters.Add("@BrokerID", strBrokerID)
			cmdDBQuery.Parameters.Add("@Name", txtAGName.Text)
			
			strGroupID = cmdDBQuery.ExecuteScalar()
			strPageState = "EDITGROUP"
			
		Else
			strPageState = "LIST"
		End If
					
	End Sub
	
	Sub CreateBroker_Click(s As Object, e As EventArgs)
		'creates a new service broker, creates broker passkey.
		If blnAMEdit Then
			Dim strNewPassKey As String
			Dim intNewClassID As Integer
			Dim brokerConf As NewBrokerConf
			lblErrorOnABMsg.Text = ""
			
			'validate inputs
			If Trim(txtABName.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A Name must be specified."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If blnACGrant Then
				If ddlABClass.SelectedItem.Value = 0 Then
					lblErrorOnABMsg.Text = "Error Creating Broker: A Usage Class must be selected."
					strPageState = "ADDBROKER"
					Exit Sub
				Else
					intNewClassID = CInt(ddlABClass.SelectedItem.Value)
				End If
			Else
				intNewClassID = 2 'automatically assigned to the default guest class.
			End If
			
			If Trim(txtABDesc.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A Description must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If Trim(txtABWSLoc.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A Web Service Interface URL must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If Trim(txtABCFName.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A full Contact Name must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If Trim(txtABCLName.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A full Contact Name must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If Trim(txtABCEmail.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A Contact Email address must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If Trim(txtABID.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A Service Broker ID must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			If Trim(txtABPassKey.Text) = "" Then
				lblErrorOnABMsg.Text = "Error Creating Broker: A Lab Server PassKey must be provided."
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			'inputs validated, creating broker
			strNewPassKey = rpmObject.CreateForeignPasskey(CInt(strBrokerID))
			
			brokerConf = rpmObject.AddBroker(txtABName.Text, txtABID.Text, strNewPassKey, txtABPassKey.Text, intNewClassID, txtABDesc.Text, txtABCFName.Text, txtABCLName.Text, txtABCEmail.Text, txtABWSLoc.Text, True) 
			'item 0 = new record ID
			'item 1 = execution comments
			
			If brokerConf.Comments() <> "Broker successfully added." Then
				lblErrorOnABMsg.Text = "Error Creating Broker: " & brokerConf.Comments()
				strPageState = "ADDBROKER"
				Exit Sub
			End If
			
			strBrokerID = brokerConf.BrokerID()
			strPageState = "EDITBROKER"
		Else
			strPageState = "LIST"
		End If
		
	End Sub
	
	Sub ShowEditBroker_Command(s As Object, e As CommandEventArgs)
		strBrokerID = e.CommandArgument
		strPageState = "EDITBROKER"
		lblErrorOnEditBrokerMsg.Text = ""
		
	End Sub
	
	Sub ShowEditGroup_Command(s As Object, e As CommandEventArgs)
		strGroupID = e.CommandArgument
		strPageState = "EDITGROUP"
		lblErrorOnEGMsg.Text = ""
	End Sub
		
	Sub ShowList_Click(s As Object, e As EventArgs)
		strPageState = "LIST"
	End Sub
	
	Sub ShowAddBroker_Click(s As Object, e As EventArgs)
		strPageState = "ADDBROKER"
		lblErrorOnABMsg.Text = ""
	End Sub
	
	Sub ShowAddGroup_Command(s As Object, e As CommandEventArgs)
		strPageState = "ADDGROUP"
		strBrokerID = e.CommandArgument
		lblErrorOnAGMsg.Text = ""
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
					Service Brokers:
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
									Text="Create New Broker"
									onClick="ShowAddBroker_Click"
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
								ID="rptBrokers"
								Runat="Server">
								<HeaderTemplate>
									<center>
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											<th align="left">
												<font class="regular">Service Broker</font>
											</th>
											<th align="left">
												<font class="regular">Usage Class</font>
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
														Text='<%#Container.DataItem("name")%>'
														CommandArgument='<%#Container.DataItem("broker_id")%>'
														OnCommand="ShowEditBroker_Command"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular"> 
													<%If blnACRead Then%>
														<a href="usage-classes.aspx?cid=<%#Container.DataItem("class_id")%>" target="main"><%#Container.DataItem("class_name")%></a>
													<%Else%>
														<%#Container.DataItem("class_name")%>
													<%End If%>
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
						CASE "EDITBROKER"
						'Service Broker information edit interface code
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEditBrokerTitle"
									Runat="Server" /></b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Brokers"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnAMEdit Then%>
									<asp:LinkButton
										Text="Create New Broker"
										onClick="ShowAddBroker_Click"
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
								<td align=left>
									<font class="regular"><b>General Information</b></font>
								</td>
							</tr>
							<tr>
								<td align="center">
									<font class="small">
										<%If blnAMEdit Then%>
											View Broker Information
											|
											<asp:LinkButton
												ID="lbAddGroup"
												Text="Create Broker Group"
												onCommand="ShowAddGroup_Command"
												Runat="Server" />
										<%End If%>
									</font>
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
														ID="txtEditBrokerName"
														Columns="30"
														MaxLength="250"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Status:</b>
													<asp:DropDownList
														ID="ddlEditBrokerStatus"
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
													<b>Class Membership:</b>
													<%If blnACRead Then%>
														<a href="usage-classes.aspx?cid=<%=strClassID%>" target="main"><%=strClassName%></a>
													<%Else
														Response.Write(strClassName)
													End If%>
												</font>
											</td>
											<td>
												<font class="regular">
													<%If blnACGrant Then%>
														<b>Reassign to:</b>
													<%Else
														ddlEditBrokerOCList.Visible = False
													End If%>
													<asp:DropDownList
														ID="ddlEditBrokerOCList"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												&nbsp;<br>
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
																	ID="txtEditBrokerDesc"
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
													<b>Broker Web Service URL:</b>
													<asp:TextBox
														ID="txtEditBrokerWSLoc"
														Columns="40"
														MaxLength="100"
														Runat="Server" />
												</font>
												<br>&nbsp;
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Contact First Name:</b><br>
													<asp:TextBox
														ID="txtEditBrokerCFName"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Broker ID:</b><br>
													<asp:TextBox
														ID="txtEditBrokerID"
														Columns="30"
														MaxLength="36"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Contact Last Name:</b><br>
													<asp:TextBox
														ID="txtEditBrokerCLName"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Broker Passkey:</b><br>
													<asp:TextBox
														ID="txtEditBrokerPassKey"
														Columns="30"
														MaxLength="50"
														ReadOnly="True"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Contact Email:</b><br>
													<asp:TextBox
														ID="txtEditBrokerCEmail"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Lab Server Passkey:</b><br>
													<asp:TextBox
														ID="txtEditBrokerLSPassKey"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												&nbsp;<br>
												<font class="regular">
													
													<asp:Repeater
														ID="rptGroups"
														Runat="Server">
														<HeaderTemplate>
															<b>Broker Groups:</b>
															<table border=1 cellspacing=0 cellpadding=0 width=100%>
																<tr>
																	<th>
																		<font class="regular">
																			Group
																		</font>
																	</th>
																	<th>
																		<font class="regular">
																			Usage Class
																		</font>
																	</th>
																	<th>
																		<font class="regular">
																			Status
																		</font>
																	</th>
																	<th>
																		<font class="regular">
																			Date Modified
																		</font>
																	</th>
																</tr>
														</HeaderTemplate>
														
														<ItemTemplate>
											 					<tr>
											 						<td>
											 							<font class="regular">
											 								&nbsp;<asp:LinkButton
											 										Text='<%#Container.DataItem("name")%>'
											 										onCommand="ShowEditGroup_Command"
											 										CommandArgument='<%#Container.DataItem("group_id")%>'
											 										Runat="Server" />
											 								<asp:HiddenField
											 									ID="lblEditBrokerGroupID"
											 									Value='<%#Container.DataItem("group_id")%>'
											 									Runat="Server" />
											 								<asp:HiddenField
											 									ID="lblEditBrokerGroupStatus"
											 									Value='<%#Container.DataItem("is_active")%>'
											 									Runat="Server" />
											 								<asp:HiddenField
											 									ID="lblEditBrokerGClassID"
											 									Value='<%#Container.DataItem("class_id")%>'
											 									Runat="Server" />
											 							</font>
											 						</td>
											 						<td>
											 							<font class="regular">
											 								<%If blnACGrant Then%>
											 									<asp:DropDownList
											 										ID="ddlEditBrokerOCList"
											 										DataSource='<%#dstClasses%>'
											 										DataTextField="name"
											 										DataValueField="class_id"
											 										SelectedIndex='<%#htClassIDOrder.Item(Container.DataItem("class_id"))%>'
											 										Runat="Server" />
											 								<%ElseIf blnACRead Then%>
											 									&nbsp;<a href="usage-classes.aspx?cid=<%#Container.DataItem("class_id")%>" target="main"><%#Container.DataItem("class_name")%></a>
											 								<%Else%>
											 									&nbsp;<%#Container.DataItem("class_name")%>
											 								<%End If%>
											 								
											 								
											 							</font>
											 						</td>
											 						<td>
											 							<font class="regular">
											 								<asp:DropDownList
											 									ID="ddlEditBrokerGStatus"
											 									SelectedIndex='<%#ValueCheck(DisplayStatus(Container.DataItem("is_active")), "Active", "0", "1")%>'
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
														</FooterTemplate>
								
													</asp:Repeater>
													
													<input type=hidden name=Gct value=<%=rptGroups.Items.Count()%>
												
													&nbsp;
													<asp:Label 
														ID="lblErrorOnEditBrokerMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2 align=center>
												<font class="regular">
													<%If blnAMEdit Then 'since edit is prereq for delete%>
														<asp:LinkButton 
															ID="lbEditBrokerUpdate"
															Text="Update Information"
															onCommand="UpdateBroker_Command"
															Runat="Server" />
														<%If blnAMDelete Then%>
														|
														<asp:LinkButton 
															ID="lbEditBrokerDelete"
															Text="Remove Broker"
															onCommand="RemoveBroker_Command"
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
														ID="lblEditBrokerDateCreated"
														Runat="Server" />
												</font>
											</td>
											<td align=center>
												<font class="small">
													<b>Date Modified:</b>
													<asp:Label
														ID="lblEditBrokerDateMod"
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
						CASE "EDITGROUP"
						'class membership (broker) edit interface code
					%>	
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblEGBTitle"
									Runat="Server" />
								-
								<asp:Label
									ID="lblEGGTitle"
									Runat="Server" />	
							</b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Brokers"
									onClick="ShowList_Click"
									Runat="Server" />
								 |
								 <%If blnAMEdit Then%>
								 <asp:LinkButton
									Text="Create New Broker"
									onClick="ShowAddBroker_Click"
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
								<td align=left>
									<font class="regular"><b>Broker Group Information</b></font>
								</td>
							</tr>
							<tr>
								<td align="center">
									<font class="small">
										<asp:LinkButton
											ID="lbEGViewBroker"
											Text="View Broker Information"
											onCommand="ShowEditBroker_Command"
											Runat="Server" />
										<%If blnAMEdit Then%>
											|	
											<asp:LinkButton
												ID="lbEGAddGroup"
												Text="Create Broker Group"
												onCommand="ShowAddGroup_Command"
												Runat="Server" />
										<%End If%>
									</font>
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
														ID="txtEGName"
														Columns="30"
														MaxLength="250"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Status:</b>
													<asp:DropDownList
														ID="ddlEGStatus"
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
													<b>Class Membership:</b>
													<%If blnACRead Then%>
														<a href="usage-classes.aspx?cid=<%=strClassID%>" target="main"><%=strClassName%></a>
													<%Else
														Response.Write(strClassName)
													End If%>
												</font>
											</td>
											<td>
												<font class="regular">
													<%If blnACGrant Then%>
														<b>Reassign to:</b>
													<%Else
														ddlEGOCList.Visible = False
													End If%>
													<asp:DropDownList
														ID="ddlEGOCList"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												&nbsp;<br>
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
																	ID="txtEGDesc"
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
												
												<font class="regular">
													&nbsp;
													<asp:Label 
														ID="lblErrorOnEGMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2 align=center>
												
											
												<font class="regular">
													<%If blnAMEdit Then 'edit is prereq for delete%>
														<asp:LinkButton 
															ID="lbEGUpdate"
															Text="Update Information"
															onCommand="UpdateGroup_Command"
															Runat="Server" />
														<%If blnAMDelete Then%>
															|
															<asp:LinkButton 
																ID="lbEGDelete"
																Text="Remove Group"
																onCommand="RemoveGroup_Command"
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
														ID="lblEGDateCreated"
														Runat="Server" />
												</font>
											</td>
											<td align=center>
												<font class="small">
													<b>Date Modified:</b>
													<asp:Label
														ID="lblEGDateMod"
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
						CASE "ADDGROUP"
						'broker group addition interface code
						If blnAMEdit Then
					%>
						<font class="extra-small">
						&nbsp;<b><asp:Label
									ID="lblAGBTitle"
									Runat="Server" />
								-
								Create Broker Group
							</b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Brokers"
									onClick="ShowList_Click"
									Runat="Server" />
								|
								<%If blnAMEdit Then%>
									<asp:LinkButton
										Text="Create New Broker"
										onClick="ShowAddBroker_Click"
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
								<th align=left><font class="regular">New Broker Group Information</font></th>
							</tr>
							<tr>
								<td align="center">
									<font class="small">
										<asp:LinkButton
											ID="lbAGViewBroker"
											Text="View Broker Information"
											onCommand="ShowEditBroker_Command"
											Runat="Server" />
										|
										Create Broker Group
									</font>
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
														ID="txtAGName"
														Columns="30"
														MaxLength="250"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Usage Class:</b>
													<%If Not blnACGrant Then
														ddlAGClass.Visible = False%>
														Guests
													<%End If%>
													<asp:DropDownList
														ID="ddlAGClass"
														Runat="Server" />

												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
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
																		ID="txtAGDesc"
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
													<asp:Label 
														ID="lblErrorOnAGMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
												
											</td>
										</tr>
										<tr>
											<td colspan=2 align=center>
												<font class="regular">
													<%If blnAMEdit Then%>
														<asp:LinkButton 
															ID="lbAGCreate"
															Text="Create Broker Group"
															onCommand="CreateGroup_Command"
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
					
					CASE "ADDBROKER"
					'broker addition interface code
					If blnAMEdit Then
					%>
						<font class="extra-small">
						&nbsp;<b>Create Service Broker</b>
						</font>
						<hr size=1>
							<div align="right" class="extra-small">
								<asp:LinkButton
									Text="View All Brokers"
									onClick="ShowList_Click"
									Runat="Server" />
								|
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
														ID="txtABName"
														Columns="30"
														MaxLength="250"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Usage Class:</b>
													<%If Not blnACGrant Then
														ddlABClass.Visible = False%>
														Guests
													<%End If%>
													<asp:DropDownList
														ID="ddlABClass"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td colspan=2>
												<font class="regular">
												&nbsp;<br>
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
																		ID="txtABDesc"
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
													<b>Broker Web Service URL:</b>
													<asp:TextBox
														ID="txtABWSLoc"
														Columns="40"
														MaxLength="100"
														Runat="Server" />
													<br>&nbsp;
												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Contact First Name:</b><br>
													<asp:TextBox
														ID="txtABCFName"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Broker ID:</b><br>
													<asp:TextBox
														ID="txtABID"
														Columns="30"
														MaxLength="36"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td>
												<font class="regular">
													<b>Contact Last Name:</b><br>
													<asp:TextBox
														ID="txtABCLName"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
											<td>
												<font class="regular">
													<b>Lab Server Passkey:</b><br>
													<asp:TextBox
														ID="txtABPassKey"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
												</font>
											</td>
										</tr>
										<tr>
											<td align=left>
												<font class="regular">
													<b>Contact Email:</b><br>
													<asp:TextBox
														ID="txtABCEmail"
														Columns="30"
														MaxLength="50"
														Runat="Server" />
													<br>&nbsp;
													<asp:Label 
														ID="lblErrorOnABMsg"
														ForeColor="Red"
														Runat="Server" />
												</font>
											</td>
											<td align=center>
												<font class="regular">
													<%If blnAMEdit Then%>
														<asp:LinkButton 
															ID="lbABCreate"
															Text="Create Service Broker"
															onClick="CreateBroker_Click"
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
