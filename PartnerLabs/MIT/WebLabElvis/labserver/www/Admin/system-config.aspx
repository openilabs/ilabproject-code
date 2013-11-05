<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Math" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers" %>

<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationManager.AppSettings("conString"))
	Dim strDBQuery, strLabStatusMsg, strLabServerID, strWebServIntEnabled, strExpEngEnabled, strSysManID, strHomepage, strDevName As String
	Dim cmdDBQuery As SqlCommand
	Dim dtrDBQuery As SqlDataReader
	Dim rpmObject As New ResourcePermissionManager()
	Dim blnSRRead, blnSCRead, blnSCEdit As Boolean
	
	Sub Page_Load
		conWebLabLS.Open()
		blnSCRead = False
		
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
			blnSRRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
			blnSCRead = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysConfig", "canview")
			blnSCEdit = rpmObject.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysConfig", "canedit")
			
			If blnSCRead Then
			
				strDBQuery = "SELECT homepage, Admin_ID, ws_int_is_active, exp_eng_is_active, lab_server_id, lab_status_msg, elvis_dev_name FROM LSSystemConfig WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				dtrDBQuery = cmdDBQuery.ExecuteReader()
				
				If dtrDBQuery.Read() Then
					strWebServIntEnabled = dtrDBQuery("ws_int_is_active")
					strExpEngEnabled = dtrDBQuery("exp_eng_is_active")
					strSysManID = dtrDBQuery("Admin_ID")
					
					If dtrDBQuery("lab_server_id") Is DBNull.Value Then
						strLabServerID = ""
					Else
						strLabServerID = dtrDBQuery("lab_server_id")
					End If
					
					If dtrDBQuery("lab_status_msg") Is DBNull.Value Then
						strLabStatusMsg = ""
					Else
						strLabStatusMsg = dtrDBQuery("lab_status_msg")
					End If
					If dtrDBQuery("homepage") Is DBNull.Value Then
						strHomepage = ""
					Else
						strHomepage = dtrDBQuery("homepage")
					End If
					
					If dtrDBQuery("elvis_dev_name") Is DBNull.Value Then
						strDevName = ""
					Else
						strDevName = dtrDBQuery("elvis_dev_name")
					End If
				End If
				
				dtrDBQuery.Close()		
			End If
		End If
	End Sub

	Sub Page_PreRender
		If blnSCRead Then
		
			'populate display fields (done in prerender to take into account changes made by "update" methods)
			txtHomepage.Text = strHomepage
			txtStatusMessage.Text = strLabStatusMsg
			txtServerID.Text = strLabServerID
			txtDevName.Text = strDevName
			
			'resets drop down list item collection (purges data from previous page state)
			dropWebServIntEnabled.Items.Clear()
			
			'populates drop down list with current info
			dropWebServIntEnabled.Items.Add(New ListItem("Disabled", "0"))
			dropWebServIntEnabled.Items.Add(New ListItem("Enabled", "1"))
			
			If strWebServIntEnabled = "True" Then
				dropWebServIntEnabled.SelectedIndex = 1
			End If
			
			'purges list from previous page state
			dropExpEngEnabled.Items.Clear()
			
			'populates list with current info
			dropExpEngEnabled.Items.Add(New ListItem("Disabled", "0"))
			dropExpEngEnabled.Items.Add(New ListItem("Enabled", "1"))
			
			If strExpEngEnabled = "True" Then
				dropExpEngEnabled.SelectedIndex = 1
			End If
			
			'purges list from previous page state
			dropSysMan.Items.Clear()
			
			'populates list with current info
			strDBQuery = "SELECT user_id, first_name, last_name FROM SiteUsers WHERE class_id = 1;"
			cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
			dtrDBQuery = cmdDBQuery.ExecuteReader()
			
			Dim loopIdx As Integer = 0
			
			While dtrDBQuery.Read()
				dropSysMan.Items.Add(New ListItem(dtrDBQuery("first_name") & " " & dtrDBQuery("last_name"), dtrDBQuery("user_id")))
				
				If dtrDBQuery("user_id") = strSysManID Then
					dropSysMan.SelectedIndex = loopIdx
				End If
				
				loopIdx = loopIdx + 1
			
			End While	
			
			dtrDBQuery.Close()
		End If
		
		
		'closes database connection
		conWebLabLS.Close()
	End Sub
	
	
	Sub DevNameUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue As String
			
			strNewValue = txtDevName.Text
			
			If Not Trim(strNewValue) = "" Then
				strDevName = strNewValue
				application("device_name") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET elvis_dev_name = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newValue", strNewValue)
				
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	Sub HomepageUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue As String
			
			strNewValue = txtHomepage.Text
			
			If Not Trim(strNewValue) = "" Then
				strHomepage = strNewValue
				application("homepage") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET homepage = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newValue", strNewValue)
				
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub SysManUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			
			Dim strNewValue As String
			
			strNewValue = dropSysMan.SelectedItem.Value
			
			If Not Trim(strNewValue) = "" Then
							
				strDBQuery = "SELECT first_name, last_name, email FROM SiteUsers WHERE user_id = @adminID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@adminID", strNewValue)
				dtrDBQuery = cmdDBQuery.ExecuteReader()
				
				If dtrDBQuery.Read() Then
					application("Admin_Name") = Trim(dtrDBQuery("first_name")) & " " & Trim(dtrDBQuery("last_name"))
					application("Admin_email") = Trim(dtrDBQuery("email"))
				Else
					'case where submitted id is not in the database
					dtrDBQuery.Close()
					Exit Sub
				End If
				dtrDBQuery.Close()
				
				strSysManID = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET Admin_ID = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
				
			End If
		End If
					
	End Sub
	
	
	
	Sub ExpEngEnabledUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue, strNewText As String
			strNewValue = dropExpEngEnabled.SelectedItem.Value
			
			If Not Trim(strNewValue) = "" Then
				If strNewValue = "1" Then
					strNewText = "True"
				Else
					strNewText = "False"
				End If
				
				strExpEngEnabled = strNewText
				application("exp_eng_is_active") = strNewText
				
				strDBQuery = "UPDATE LSSystemConfig SET exp_eng_is_active = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub WebServIntEnabledUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue, strNewText As String
			strNewValue = dropWebServIntEnabled.SelectedItem.Value
			
			If Not Trim(strNewValue) = "" Then
				If strNewValue = "1" Then
					strNewText = "True"
				Else
					strNewText = "False"
				End If
				
				strWebServIntEnabled = strNewText
				application("ws_int_is_active") = strNewText
				
				strDBQuery = "UPDATE LSSystemConfig SET ws_int_is_active = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub StatusMessageUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit
			Dim strNewStatusMsg As String
			strNewStatusMsg = txtStatusMessage.Text
		
			If Not Trim(strNewStatusMsg) = ""  Then
				strLabStatusMsg = strNewStatusMsg
				
				strDBQuery = "UPDATE LSSystemConfig SET lab_status_msg = @newStatusMsg WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newStatusMsg", strNewStatusMsg)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub


	Sub LStatUpdateAll_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewIntValue, strNewIntText, strNewEngValue, strNewEngText, strNewStatusMsg As String
			strNewIntValue = dropWebServIntEnabled.SelectedItem.Value
			strNewEngValue = dropExpEngEnabled.SelectedItem.Value
			strNewStatusMsg = txtStatusMessage.Text
			
			If Not (Trim(strNewIntValue) = "" OR Trim(strNewEngValue) = "" OR Trim(strNewStatusMsg) = "") Then
				If strNewIntValue = "1" Then
					strNewIntText = "True"
				Else
					strNewIntText = "False"
				End If
				
				If strNewEngValue = "1" Then
					strNewEngText = "True"
				Else
					strNewEngText = "False"
				End If
				
				strWebServIntEnabled = strNewIntText
				application("ws_int_is_active") = strNewIntText
				strExpEngEnabled = strNewEngText
				application("exp_eng_is_active") = strNewEngText
				strLabStatusMsg = strNewStatusMsg
				
				strDBQuery = "UPDATE LSSystemConfig SET exp_eng_is_active = @newEngValue, ws_int_is_active = @newIntValue, lab_status_msg = @newStatusMsg WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newEngValue", strNewEngValue)
				cmdDBQuery.Parameters.AddWithValue("@newIntValue", strNewIntValue)
				cmdDBQuery.Parameters.AddWithValue("@newStatusMsg", strNewStatusMsg)
				cmdDBQuery.ExecuteNonQuery()
				
			End If
		End If
	End Sub
	
	
	Sub NewServerID_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
			
			'will only execute if current id value is DBNull.value
			strLabServerID = rpmObject.CreateServerID()
		End If
	End Sub
	
	
	Sub	ConfigUpdateAll_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			'load all new settings
			Dim strNewSysManVal, strNewHomepage, strNewDevName As String
			
			strNewHomepage = txtHomepage.Text
			strNewSysManVal = dropSysMan.SelectedItem.Value
			strNewDevName = txtDevName.Text
			
			If  Not (Trim(strNewHomepage) = "" Or Trim(strNewSysManVal) = "" Or Trim(strNewDevName) = "") Then
				'edit page/site setting variables
				strHomepage = strNewHomepage
				application("homepage") = strNewHomepage
				
				strDevName = strNewDevName
				application("device_name") = strNewDevName
				
				strDBQuery = "SELECT first_name, last_name, email FROM SiteUsers WHERE user_id = @adminID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@adminID", strNewSysManVal)
				dtrDBQuery = cmdDBQuery.ExecuteReader()
				
				If dtrDBQuery.Read() Then
					application("Admin_Name") = Trim(dtrDBQuery("first_name")) & " " & Trim(dtrDBQuery("last_name"))
					application("Admin_email") = Trim(dtrDBQuery("email"))
				Else
					'case where submitted id is not in the database
					dtrDBQuery.Close()
					Exit Sub
				End If
				dtrDBQuery.Close()
				
				strSysManID = strNewSysManVal
				
				'update setup record in database
				strDBQuery = "UPDATE LSSystemConfig SET homepage = @newHomepageValue, Admin_ID = @newSysManValue, elvis_dev_name = @newDevName WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.AddWithValue("@newHomepageValue", strNewHomepage)
				cmdDBQuery.Parameters.AddWithValue("@newSysManValue", strNewSysManVal)
				cmdDBQuery.Parameters.AddWithValue("@newDevName", strNewDevName)
				cmdDBQuery.ExecuteNonQuery()
			End If
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
					Lab Status Settings:
				</font>
				<hr size=1>
				<center>
					
					<table border=0 cellpadding=2 cellspacing=0 width=95%>
						<tr bgcolor="#e0e0e0">
							<td><font class="regular"><b>Setting</b></font></td>
							<td width=180><font class="regular"><b>Value</b></font></td>
							<td><font class="regular"><b>&nbsp;</b></font></td>
						</tr>
						<tr>
							<td><font class="regular">WebLab Web Service Interface:</font></td>
							<td><font class="regular">
									<asp:DropDownList
										ID="dropWebServIntEnabled"
										Runat="Server" />
										</font></td>
							<td><font class="regular">
								<%If blnSCEdit Then%>
									<asp:LinkButton
										Text="Update"
										OnClick="WebServIntEnabledUpdate_Click"
										Runat="Server" />
								<%Else%>
									&nbsp;
								<%End If%>
								</font></td>
						</tr>
						<tr>
							<td><font class="regular">Experiment Execution Engine:</font></td>
							<td><font class="regular">
									<asp:DropDownList
										ID="dropExpEngEnabled"
										Runat="Server" />
										</font></td>
							<td><font class="regular">
								<%If blnSCEdit Then%>
									<asp:LinkButton
										Text="Update"
										OnClick="ExpEngEnabledUpdate_Click"
										Runat="Server" />
								<%Else%>
									&nbsp;
								<%End If%>
								</font></td>
						</tr>
						<tr>
							<td><font class="regular">Lab Status Message:</font></td>
							<td><font class="regular">
									<asp:TextBox
										ID="txtStatusMessage"
										Columns="15"
										Rows="3"
										TextMode="MultiLine"
										Wrap="True"
										Runat="Server" />
								</font></td>
							<td><font class="regular">
								<%If blnSCEdit Then%>
									<asp:LinkButton
										Text="Update"
										OnClick="StatusMessageUpdate_Click"
										Runat="Server" />
								<%Else%>
									&nbsp;
								<%End If%>
								</font></td>
						</tr>
						<tr>
							<td colspan=3 align=center>
								<font class="regular">
								<p>
								<%If blnSCEdit Then%>
									<asp:LinkButton
										Text = "Update All Lab Status Settings"
										OnClick="LStatUpdateAll_Click"
										Runat="Server" />
								<%Else%>
									&nbsp;
								<%End If%>
								</font>
							</td>
						</tr>
					</table>
				<p>
				</center>
			</td>
		</tr>
		<tr>
			<td>
				<font class="title">
					Configuration Settings:
				</font>
				
				<hr size=1>

				<center>


				<table border=0 cellpadding=2 cellspacing=0 width=95%>
				<tr bgcolor="#e0e0e0">
					<td><font class="regular"><b>Setting</b></font></td>
					<td width=180><font class="regular"><b>Value</b></font></td>
					<td><font class="regular"><b>&nbsp;</b></font></td>
				</tr>

				<% 
				%>
				<tr>
					<td><font class="regular">System Manager:</font></td>
					<td><font class="regular">
							<asp:DropDownList
								ID="dropSysMan"
								Runat="Server" />
						</font></td>
						
					<td><font class="regular">
						<%If blnSCEdit Then %>
							<asp:LinkButton
								Text="Update"
								OnClick="SysManUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>
				
				<tr>
					<td><font class="regular">WebLab System URL:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txtHomepage"
								Columns="20"
								MaxLength="100"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>
					<td><font class="regular">
						<%If blnSCEdit Then%>
							<asp:LinkButton
								Text="Update"
								OnClick="HomepageUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>
				
				<tr>
					<td><font class="regular">NI-ELVIS Device Name:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txtDevName"
								Columns="20"
								MaxLength="100"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>
					<td><font class="regular">
						<%If blnSCEdit Then%>
							<asp:LinkButton
								Text="Update"
								OnClick="DevNameUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>
				
				<tr>
					<td colspan=3 align=center>
						<font class="regular">
						<p>
						<%If blnSCEdit Then%>
							<asp:LinkButton
								Text = "Update All Configuration Settings"
								OnClick="ConfigUpdateAll_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font>
					</td>
				</tr>
				<tr>
					<td colspan=3>
						<p>&nbsp;
					</td>
				</tr>
				<tr>
					<td><font class="regular">Server ID
							<%
							If strLabServerID = "" And blnSCEdit Then
							%> (<asp:LinkButton
									Text="Generate New ID"
									OnClick="NewServerID_Click"
									Runat="Server" />)
							<%
							End If
							%>:
						</font></td>
					<td colspan=2><font class="regular">
							<asp:TextBox
								ID="txtServerID"
								Columns="32"
								MaxLength="100"
								TextMode="SingleLine"
								ReadOnly = "True"
								Runat="Server" />
						</font></td>
				
				</tr>
				</table>
				</p>

				</center>

 				</td>
			</tr>
			<tr>
				<td>
				<center>
					<font class="small">
						<%If blnSRRead Then%>
							<a href="main.aspx" target="main">Return to Main</a>						<%Else%>
							<a href="../main.aspx" target="main">Return to Main</a>
						<%End If%>
					</font>
				</center>
				</td>
			</tr>
		</table>
		
    </form>
  <%End If%>
  </body>
</html>
