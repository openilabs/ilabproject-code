<%@ Page Language="VBScript" %>
<%@ Import Namespace="System.Math" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="WebLabDataManagers" %>

<!-- 
Copyright (c) 2005 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->


<script Runat="Server">

	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strDBQuery, strLabStatusMsg, strLabServerID, strWebServIntEnabled, strExpEngEnabled, str4155BusAddr, str5250AEnabled, str5250ABusAddr, str34970AEnabled, str34970ABusAddr, str34970AThermChannel, strVISAIntName, strSysManID, strHomepage As String
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
			
				strDBQuery = "SELECT HP4155_ID, HPE5250A_present, HPE5250A_ID, HP34970A_present, HP34970A_ID, HP34970A_chan, VISA_Name, homepage, Admin_ID, ws_int_is_active, exp_eng_is_active, lab_server_id, lab_status_msg FROM LSSystemConfig WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				dtrDBQuery = cmdDBQuery.ExecuteReader()
				
				If dtrDBQuery.Read() Then
					strWebServIntEnabled = dtrDBQuery("ws_int_is_active")
					strExpEngEnabled = dtrDBQuery("exp_eng_is_active")
					str4155BusAddr = dtrDBQuery("HP4155_ID")
					str5250AEnabled = dtrDBQuery("HPE5250A_present")
					str5250ABusAddr = dtrDBQuery("HPE5250A_ID")
					str34970AEnabled = dtrDBQuery("HP34970A_present")
					str34970ABusAddr = dtrDBQuery("HP34970A_ID")
					str34970AThermChannel = dtrDBQuery("HP34970A_chan")
					strVISAIntName = dtrDBQuery("VISA_Name")
					strSysManID = dtrDBQuery("Admin_ID")
					
					
					If dtrDBQuery("homepage") Is DBNull.Value Then
						strHomepage = ""
					Else
						strHomepage = dtrDBQuery("homepage")
					End If
					
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
				End If
				
				dtrDBQuery.Close()		
			End If
		End If
	End Sub

	Sub Page_PreRender
		If blnSCRead Then
		
			'populate display fields (done in prerender to take into account changes made by "update" methods)
			txt4155BusAddr.Text = str4155BusAddr
			txt5250ABusAddr.Text = str5250ABusAddr
			txt34970ABusAddr.Text = str34970ABusAddr
			txt34970AThermChannel.Text = str34970AThermChannel
			txtVISAIntName.Text = strVISAIntName
			txtHomepage.Text = strHomepage
			txtStatusMessage.Text = strLabStatusMsg
			txtServerID.Text = strLabServerID
			
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
			drop5250AEnabled.Items.Clear()
			
			'populates list with current info
			drop5250AEnabled.Items.Add(New ListItem("No", "0"))
			drop5250AEnabled.Items.Add(New ListItem("Yes", "1"))
			
			If str5250AEnabled = "True" Then
				drop5250AEnabled.SelectedIndex = 1
			End If
			
			'purges list from previous page state
			drop34970AEnabled.Items.Clear()
			
			'populates list with current info
			drop34970AEnabled.Items.Add(New ListItem("No", "0"))
			drop34970AEnabled.Items.Add(New ListItem("Yes", "1"))
			
			If str34970AEnabled = "True" Then
				drop34970AEnabled.SelectedIndex = 1
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
	
	
	Sub HomepageUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue As String
			
			strNewValue = txtHomepage.Text
			
			If Not Trim(strNewValue) = "" Then
				strHomepage = strNewValue
				application("homepage") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET homepage = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				
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
				cmdDBQuery.Parameters.Add("@adminID", strNewValue)
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
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
				
			End If
		End If
					
	End Sub
	
	
	Sub VISAIntNameUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
		
			Dim strNewValue As String
			
			strNewValue = txtVISAIntName.Text
			
			If Not Trim(strNewValue) = "" Then
				strVISAIntName = strNewValue
				application("VISA_Name") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET VISA_Name = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub HP34970AThermChannelUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue As String
			strNewValue = txt34970AThermChannel.Text
			
			If Not Trim(strNewValue) = "" And IsNumeric(strNewValue) Then
				str34970AThermChannel = strNewValue
				application("HP34970A_chan") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET HP34970A_chan = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub HP34970ABusAddrUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit
			Dim strNewValue As String
			strNewValue = txt34970ABusAddr.Text
			
			If Not Trim(strNewValue) = "" And IsNumeric(strNewValue) Then
				str34970ABusAddr = strNewValue
				application("HP34970A_ID") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET HP34970A_ID = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub HP34970AEnabledUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue, strNewText As String
			strNewValue = drop34970AEnabled.SelectedItem.Value
			
			If Not Trim(strNewValue) = "" Then
				If strNewValue = "1" Then
					strNewText = "True"
				Else
					strNewText = "False"
				End If
			
				str34970AEnabled = strNewText
				application("HP34970A_present")	= strNewText
				
				strDBQuery = "UPDATE LSSystemConfig SET HP34970A_present = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub HP5250ABusAddrUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue As String
			strNewValue = txt5250ABusAddr.Text
			
			If Not Trim(strNewValue) = "" And IsNumeric(strNewValue) Then
				str5250ABusAddr = strNewValue
				application("HPE5250A_ID") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET HPE5250A_ID = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub HP5250AEnabledUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue, strNewText As String
			strNewValue = drop5250AEnabled.SelectedItem.Value
			
			If Not Trim(strNewValue) = "" Then
				If strNewValue = "1" Then
					strNewText = "True"
				Else
					strNewText = "False"
				End If
				
				str5250AEnabled = strNewText
				application("HPE5250A_present") = strNewText
				
				strDBQuery = "UPDATE LSSystemConfig SET HPE5250A_present = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
				cmdDBQuery.ExecuteNonQuery()
			End If
		End If
	End Sub
	
	
	Sub HP4155BusAddrUpdate_Click(s As Object, e As EventArgs)
		If blnSCEdit Then
			Dim strNewValue As String
			strNewValue = txt4155BusAddr.Text
			
			If Not Trim(strNewValue) = "" And IsNumeric(strNewValue) Then
				str4155BusAddr = strNewValue
				application("HP4155_ID") = strNewValue
				
				strDBQuery = "UPDATE LSSystemConfig SET HP4155_ID = @newValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQUery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
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
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
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
				cmdDBQuery.Parameters.Add("@newValue", strNewValue)
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
				cmdDBQuery.Parameters.Add("@newStatusMsg", strNewStatusMsg)
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
				cmdDBQuery.Parameters.Add("@newEngValue", strNewEngValue)
				cmdDBQuery.Parameters.Add("@newIntValue", strNewIntValue)
				cmdDBQuery.Parameters.Add("@newStatusMsg", strNewStatusMsg)
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
			Dim strNew4155BusAddr, strNew5250ABusAddr, strNew5250AEnVal, strNew34970ABusAddr, strNew34970AEnVal, strNew34970AThermChan, strNewVISAIntName, strNewSysManVal, strNewHomepage As String
			
			strNewHomepage = txtHomepage.Text
			strNewSysManVal = dropSysMan.SelectedItem.Value
			strNewVISAIntName = txtVISAIntName.Text
			strNew34970AThermChan = txt34970AThermChannel.Text
			strNew34970ABusAddr = txt34970ABusAddr.Text
			strNew34970AEnVal = drop34970AEnabled.SelectedItem.Value
			strNew5250ABusAddr = txt5250ABusAddr.Text
			strNew5250AEnVal = drop5250AEnabled.SelectedItem.Value
			strNew4155BusAddr = txt4155BusAddr.Text
			
			If (IsNumeric(strNew4155BusAddr) And IsNumeric(strNew5250ABusAddr) And IsNumeric(strNew34970ABusAddr) And IsNumeric(strNew34970AThermChan)) And Not (Trim(strNewHomepage) = "" Or Trim(strNewSysManVal) = "" Or Trim(strNewVISAIntName) = "" Or Trim(strNew34970AThermChan) = "" Or Trim(strNew34970ABusAddr) = "" Or Trim(strNew34970AEnVal) = "" Or Trim(strNew5250ABusAddr) = "" Or Trim(strNew5250AEnVal) = "" Or Trim(strNew4155BusAddr) = "") Then
				'edit page/site setting variables
				strHomepage = strNewHomepage
				strVISAIntName = strNewVISAIntName
				str34970AThermChannel = strNew34970AThermChan
				str5250ABusAddr = strNew5250ABusAddr
				str4155BusAddr = strNew4155BusAddr
				str34970ABusAddr = strNew34970ABusAddr
				
				application("homepage") = strNewHomepage
				application("HP34970A_chan") = strNew34970AThermChan
				application("HP34970A_ID") = strNew34970ABusAddr
				application("HP4155_ID") = strNew4155BusAddr
				application("HPE5250A_ID") = strNew5250ABusAddr
				application("VISA_Name") = strNewVISAIntName
				
				If strNew34970AEnVal = "1" Then
					str34970AEnabled = "True"
					application("HP34970A_present") = "True"
				Else
					str34970AEnabled = "False"
					application("HP34970A_present") = "False"
				End If
				
				If strNew5250AEnVal = "1" Then
					str5250AEnabled = "True"
					application("HPE5250A_present") = "True"
				Else
					str5250AEnabled = "False"
					application("HPE5250A_present") = "False"
				End If
				
				strDBQuery = "SELECT first_name, last_name, email FROM SiteUsers WHERE user_id = @adminID;"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@adminID", strNewSysManVal)
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
				strDBQuery = "UPDATE LSSystemConfig SET homepage = @newHomepageValue, Admin_ID = @newSysManValue, VISA_Name = @newVISAIntValue, HP34970A_chan = @new34970ChanValue, HP34970A_ID = @new34970IDValue, HP34970A_present = @new34970EnValue, HPE5250A_ID = @new5250IDValue, HPE5250A_present = @new5250EnValue, HP4155_ID = @new4155IDValue WHERE SetupID = '1';"
				cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
				cmdDBQuery.Parameters.Add("@newHomepageValue", strNewHomepage)
				cmdDBQuery.Parameters.Add("@newSysManValue", strNewSysManVal)
				cmdDBQuery.Parameters.Add("@newVISAIntValue", strNewVISAIntName)
				cmdDBQuery.Parameters.Add("@new34970ChanValue", strNew34970AThermChan)
				cmdDBQuery.Parameters.Add("@new34970IDValue", strNew34970ABusAddr)
				cmdDBQuery.Parameters.Add("@new34970EnValue", strNew34970AEnVal)
				cmdDBQuery.Parameters.Add("@new5250IDValue", strNew5250ABusAddr)
				cmdDBQuery.Parameters.Add("@new5250EnValue", strNew5250AEnVal)
				cmdDBQuery.Parameters.Add("@new4155IDValue", strNew4155BusAddr)
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
					<td><font class="regular">HP4155B GPIB Bus Address:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txt4155BusAddr"
								Columns="20"
								MaxLength="10"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>
					<td><font class="regular">
						<%If blnSCEdit Then%>
							<asp:LinkButton
								Text="Update"
								OnClick="HP4155BusAddrUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>
				
				<tr>
					<td><font class="regular">HPE5250A Enabled:</font></td>
					<td><font class="regular">
							<asp:DropDownList
								ID="drop5250AEnabled"
								Runat="Server" />
						</font></td>

					<td><font class="regular">
						<%If blnSCEdit Then  %>
							<asp:LinkButton
								Text="Update"
								OnClick="HP5250AEnabledUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>

				<tr>
					<td><font class="regular">HPE5250A GPIB Bus Address:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txt5250ABusAddr"
								Columns="20"
								MaxLength="10"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>

					<td><font class="regular">
						<%If blnSCEdit Then%>
							<asp:LinkButton
								Text="Update"
								OnClick="HP5250ABusAddrUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>

				<tr>
					<td><font class="regular">HP34970A Enabled:</font></td>
					<td><font class="regular">
							<asp:DropDownList
								ID="drop34970AEnabled"
								Runat="Server" />
						</font></td>

					<td><font class="regular">
						<%If blnSCEdit Then %>
							<asp:LinkButton
								Text="Update"
								OnClick="HP34970AEnabledUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>

				<tr>
					<td><font class="regular">HP34970A GPIB Bus Address:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txt34970ABusAddr"
								Columns="20"
								MaxLength="10"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>
					<td><font class="regular">
						<%If blnSCEdit Then %> 
							<asp:LinkButton
								Text="Update"
								OnClick="HP34970ABusAddrUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>

				<tr>
					<td><font class="regular">HP34970A Configured <br>Thermocouple Channel:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txt34970AThermChannel"
								Columns="20"
								MaxLength="10"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>
					<td><font class="regular">
						<%If blnSCEdit Then%>
							<asp:LinkButton 
								Text="Update"
								OnClick="HP34970AThermChannelUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>

				<tr>
					<td><font class="regular">GPIB VISA Interface Name:</font></td>
					<td><font class="regular">
							<asp:TextBox
								ID="txtVISAIntName"
								Columns="20"
								MaxLength="10"
								TextMode="SingleLine"
								Runat="Server" />
						</font></td>
					<td><font class="regular">
						<%If blnSCEdit Then %>
							<asp:LinkButton
								Text="Update"
								OnClick="VISAIntNameUpdate_Click"
								Runat="Server" />
						<%Else%>
							&nbsp;
						<%End If%>
						</font></td>
				</tr>
				
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
							<a href="/admin/main.aspx" target="main">Return to Main</a>
						<%Else%>
							<a href="/main.aspx" target="main">Return to Main</a>
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
