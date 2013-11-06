<%@ Page Language="VBScript"%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLab.DataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
	Dim strDBQuery As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim dstProfiles As DataSet
	Dim dadProfiles As SqlDataAdapter
	Dim strProfIDList As String
	Dim intLocCT As Integer
	Dim rpmObject As New ResourcePermissionManager()
	Dim strProfIDArray() As String 
	Dim blnDMRead, blnDMEdit, blnDMDelete, blnSRRead As Boolean
	
	Sub Page_Load
		conWebLabLS.Open()
		'get initial info
		
		blnDMRead = False
		'load user permission set for this page
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			blnDMRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SetupManagement", "canview")
			blnDMEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SetupManagement", "canedit")
			blnDMDelete = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SetupManagement", "candelete")
			
			If blnDMRead Then
		
				lblErrorMsg.Text = ""
				
				If not Page.IsPostBack then
					
				End If
			End If
		End If
	End Sub

	Sub Page_PreRender
		If blnDMRead Then
		
			'write info into display fields
			
			dstProfiles = New DataSet()
			strDBQuery = "SELECT p.setup_id, r.name FROM Setups p JOIN Resources r ON p.resource_id = r.resource_id ORDER BY p.setup_id;"
			dadProfiles = New SqlDataAdapter(strDBQuery, conWebLabLS)
			dadProfiles.Fill(dstProfiles, "Profiles")
			
			strDBQuery = "SELECT setup_id FROM Setups ORDER BY setup_id;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDbQuery = cmdDBquery.ExecuteReader
			
			strProfIDList = ""
			
			If dtrDBQuery.Read() Then
				strProfIDList = dtrDBQuery("setup_id")
				
				Do While dtrDBQuery.Read()
					strProfIDList = strProfIDList & ":" & dtrDBQuery("setup_id")
				Loop
			End If
			dtrDBQuery.Close()
			
			strProfIDArray = Split(strProfIDList, ":")
			
			strDBQuery = "SELECT COUNT(*) FROM ActiveSetups;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			intLocCT = CInt(cmdDBQuery.ExecuteScalar())

					
			'Active Device List Information
			strDBQuery = "SELECT a.active_id, a.is_active, a.setup_id, r.name As setup_name FROM ActiveSetups a LEFT JOIN Setups p ON a.setup_id = p.setup_id LEFT JOIN Resources r ON p.resource_id = r.resource_id;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			rptActives.DataSource = dtrDBQuery
			rptActives.DataBind()
			
			dtrDBQuery.Close()
			
			'lbChange.CommandArgument = intLocCT
		End If
			
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	Function SelectIndex(ByVal objProfileID As Object) As Integer
		'detects the index in an ordered dataset where the specified id occurs and outputs that value
		Dim intProfileID, loopIdx, arrayLen, ReturnIdx As Integer
		
		If objProfileID Is DBNull.Value Then
			intProfileID = 0
		Else
			intProfileID = CInt(objProfileID)
		End If
		
		arrayLen = strProfIDArray.Length()
		ReturnIdx = 0
		
		For loopIdx = 0 To arrayLen - 1
			If intProfileID = strProfIDArray(loopIdx) Then
				ReturnIdx = loopIdx
			End If
		
		Next
		
		Return ReturnIdx
	End Function
	
	Sub CreatePort_Click(s As Object, e As EventArgs)
	'	'this routine creates a new active device location on the system.  The process is handled by a Resource Permission Manager Method.
	'	If blnDMEdit Then
	'		Dim strResult As String
	'		
	'		strResult = rpmObject.CreateDevicePosition()
	'		
	'		If strResult <> "Device Position Successfully Added." Then
	'			lblErrorMsg.Text = "Error Creating Location: " & strResult
	'		End If
	'	End If
	End Sub
	
	Sub RemovePort_Click(s As Object, e As EventArgs)
	'	'this routine removes the most recently created location on the system.  The process is handled by a Resource Permission Manager Method.
	'	If blnDMDelete Then
	'		Dim strResult As String
	'		
	'		strResult = rpmObject.RemoveDevicePosition()
	'		
	'		If strResult <> "Device position successfully removed." Then
	'			lblErrorMsg.Text = "Error Removing Location: " & strResult
	'		End If
	'	End If
	End Sub
	
	Sub ChangeAssn_Command(s As Object, e As CommandEventArgs)
		'this routine changes the profile assigned to the specified device location (ddlLocations) to the profile specified by the user (ddlProfiles).
		'The process is handled by a Resource Permission Manager Method.
		If blnDMEdit Then
			Dim strResult As String
			Dim loopIdx, intCtrlNum As Integer
			Dim ddlProfilesRef As DropDownList
			Dim chkStatusRef As CheckBox
			Dim lblIDContainerRef As HiddenField

			Dim strCtrlNum As String
			For loopIdx = 0 To e.CommandArgument - 1
				intCtrlNum = (loopIdx * 2) + 1
				
				If intCtrlNum < 10 Then
				    strCtrlNum = "0" &IntCtrlNum
				Else
				     strCtrlNum = IntCtrlNum
				End If
				
				ddlProfilesRef = FindControl("rptActives$ctl" & strCtrlNum & "$ddlProfiles")
				chkStatusRef = FindControl("rptActives$ctl" & strCtrlNum & "$chkStatus")
				lblIDContainerRef = FindControl("rptActives$ctl" & strCtrlNum & "$lblIDContainer")
				
				If ddlProfilesRef Is Nothing Then
					lblErrorMsg.Text = "Page Error While Changing Assignments.  Aborting. (ddlProfiles, " & loopIdx & ")"
					Exit Sub 	
				End If
			
				If chkStatusRef Is Nothing Then
					lblErrorMsg.Text = "Page Error While Changing Assignments.  Aborting. (chkStatusRef, " & loopIdx & ")"
					Exit Sub
				End If
				
				If lblIDContainerRef Is Nothing Then
					lblErrorMsg.Text = "Page Error While Changing Assignments.  Aborting. (lblIDContainer, " & loopIdx & ")"
					Exit Sub
				End If
				
				If ddlProfilesRef.SelectedItem.Value = "0" Then
					'strResult = rpmObject.SetActiveDevice(CInt(lblIDContainerRef.Value), CBool(chkStatusRef.Checked))
					
					'If strResult <> "Device position successfully set." Then
					'	lblErrorMsg.Text = "Error Assigning Profile: " & strResult
					'End If
				Else
					strResult = rpmObject.SetActiveSetup(CInt(lblIDContainerRef.Value), CInt(ddlProfilesRef.SelectedItem.Value), CBool(chkStatusRef.Checked))
					
					If strResult <> "Device position successfully set." Then
						lblErrorMsg.Text = "Error Assigning Profile: " & strResult
					End If
				End If
			Next
		End If
	End Sub
	
	'Sub ChangeStatus_Command(s As Object, e As CommandEventArgs)
		'this function performs the action specified by the commandname argument {Activate, Deactivate}, on the device location
		'specified by commandArgument.  If the action is Activate, the is_active field is set to "1".  Deactivate sets the 
		'is_active field to "0".  Any other value of commandName returns an error and no action is performed.
		
	'	If e.CommandName = "Activate" Then
	'		strDBQuery = "UPDATE ActiveDevices SET is_active = 1 WHERE active_id = @LocID;"
	'		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	'		cmdDBQuery.Parameters.AddWithValue("@LocID", e.CommandArgument)
	'		cmdDBQuery.ExecuteNonQuery()
	'		
	'	ElseIf e.CommandName = "Deactivate" Then
	'		strDBQuery = "UPDATE ActiveDevices SET is_active = 0 WHERE active_id = @LocID;"
	'		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
	'		cmdDBQuery.Parameters.AddWithValue("@LocID", e.CommandArgument)
	'		cmdDBQuery.ExecuteNonQuery()
	'	
	'	Else
	'		lblErrorMsg.Text = "Error Changing Location Status: Unrecognized Action."
	'	End If
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
  <% If Not Session("LoggedInAsUserName") Is Nothing And blnDMRead Then  %>
    <form EncType="multipart/form-data" Runat="server">
		<table border=0 cellpadding=10 width=95%>
			<tr>
				<td>
					<font class="title">
					Active Setups:
					</font>
			
					<font class="extra-small">
						<b><!--subtitle text--></b>
					</font>
					
						<hr size=1>
						<div align="right" class="extra-small">
							<%If blnSRRead Then%>
								<a href="main.aspx" target="main">Return to Main</a>
							<%Else%>
								<a href="../main.aspx" target="main">Return to Main</a>
							<%End If%>
						</div>
						<p>
						<center>
							<asp:Repeater
								ID="rptActives"
								Runat="Server">
								<HeaderTemplate>
									
									<table border=0 cellpadding=3 cellspacing=0 width=100%>
										<tr bgcolor="#e0e0e0">
											
											<th align="left">
												<font class="regular">Assigned Circuit</font>
											</th>
											<th>
												<font class="regular">Circuit Is Active</font>
											</th>
										</tr>
								</HeaderTemplate>
								
								<ItemTemplate>
										<tr>
											
											<td>
												<font class="regular">
												<asp:HiddenField
														ID="lblIDContainer"
														Value='<%#Container.DataItem("active_id")%>'
														Runat="Server" />
													<asp:DropDownList
														ID="ddlProfiles"
														DataSource='<%#dstProfiles%>'
														DataTextField="name"
														DataValueField="setup_id"
														SelectedIndex='<%#SelectIndex(Container.DataItem("setup_id"))%>'
														Runat="Server" />
													
												</font>
											</td>
											<td align=center>
												<font class="regular">
													<asp:CheckBox
														ID="chkStatus"
														Checked='<%#Container.DataItem("is_active")%>'
														Runat="Server" />
												
												</font>
											</td>
										</tr>
								</ItemTemplate>
								
								<SeparatorTemplate>
									
								</SeparatorTemplate>
								
								<FooterTemplate>
										<tr>
											<td colspan=2 align=center>
												<font class="regular">
													<%If blnDMEdit Then%>
														<asp:LinkButton
															ID="lbChange"
															Text="Commit Assignment"
															CommandArgument='<%#intLocCT%>'
															onCommand="ChangeAssn_Command"
															Runat="Server" />
													<%End If%>
													<!--	
													|
													<asp:LinkButton
														Text="Create New Location"
														OnClick="CreatePort_Click"
														Runat="Server" />
													| 
													<asp:LinkButton
														Text="Remove Location"
														OnClick="RemovePort_Click"
														Runat="Server" />
													-->
												</font>
											</td>
										</tr>
									</table>
									
								</FooterTemplate>
		
							</asp:Repeater>
							<p>
							<font class="regular">
								<asp:Label
									ID="lblErrorMsg"
									ForeColor="Red"
									Runat="Server" />
							</font>
	
							</center>
				
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
