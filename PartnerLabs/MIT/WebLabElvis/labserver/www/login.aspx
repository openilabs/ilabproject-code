<%@ Page language="VBscript" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Security" %>
<%@ Import Namespace="WebLabDataManagers.WebLabDataManagers" %>

<Script Runat="Server">
	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))
	Dim strPageState, strErrorTrigger as String
	Dim RecMan As New RecordManager()
	Dim RPMan As New ResourcePermissionManager()

	Sub Page_Load
		conWebLabLS.Open()
		strErrorTrigger = "NONE"
		
		If Session("LoggedInAsUserName") Is Nothing Then
			If User.Identity.IsAuthenticated 
				PerformLogin(User.Identity.Name, True, False)
			Else
				strPageState = "LOGIN"
			End If
		Else
			strPageState = "MENU"
		End If
			
	End Sub
	
	Sub Page_PreRender
		conWebLabLS.Close()
	End Sub
	
	Sub LoginButton_Click(s As Object, e As ImageClickEventArgs)
		If IsValid Then
			If IsValidAdmin(txtUserName.Text, txtPassword.Text) Then
				PerformLogin(txtUserName.Text, chkDoPersistentLogin.Checked, True)
			Else
				strPageState = "LOGIN"
				strErrorTrigger = "BADLOGIN"
				
				'to reset form variables
				txtUserName.Text = ""
				txtPassword.Text = ""
				chkDoPersistentLogin.Checked = False	
			End If
		End If 
	End Sub
	
	Sub LogoutButton_Click(s As Object, e As Eventargs)
		FormsAuthentication.SignOut()
		
		Session.Remove("LoggedInAsUserName")
		Session.Remove("LoggedInUserID")
		Session.Remove("LoggedInClassID")
		strPageState = "LOGIN"
	End Sub
	
	Function IsValidAdmin(strUserName As String, strPassword As String) As Boolean
		Dim strDBQuery as String
		Dim cmdDBQuery As SqlCommand
		Dim blnResult as Boolean
				
		strDBQuery = "SELECT 'true' WHERE EXISTS(SELECT user_id FROM SiteUsers WHERE username = @username AND password = @password);"
		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
		cmdDBQuery.Parameters.Add("@username", strUserName)
		cmdDBQuery.Parameters.Add("@password", strPassword)
		
		If cmdDBQuery.ExecuteScalar() = "true" Then
			blnResult = True
		Else
			blnResult = False
		End If
		
		Return blnResult
	End Function
	
	Sub PerformLogin(strUserName As String, blnLoginIsPersistent As Boolean, blnIsNewLogin As Boolean)
		Dim strDBQuery as String
		Dim cmdDBQuery As SqlCommand
		Dim dtrDBQuery As SqlDataReader
		Dim intAuthVal As integer
		
		strDBQuery = "SELECT user_id, username, class_id FROM SiteUsers WHERE username = @username;"
		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
		cmdDBQuery.Parameters.Add("@username", strUserName)
		
		dtrDBQuery = cmdDBQuery.ExecuteReader()
		
		If dtrDBQuery.Read()
			Session("LoggedInUserID") = dtrDBQuery("user_id")
			Session("LoggedInAsUserName") = dtrDBQuery("username")
			Session("LoggedInClassID") = dtrDBQuery("class_id")
			
			Dim objCookie As New HttpCookie ("WebLabUser", dtrDBQuery("user_id"))
			objCookie.Expires = DateTime.MaxValue
			Response.Cookies.Add(objCookie)
			FormsAuthentication.SetAuthCookie(Session("LoggedInAsUserName"), blnLoginIsPersistent)
			
			dtrDBQuery.Close()
			
			RecMan.LogSiteLogon(Session("LoggedInUserID"), Request.UserHostAddress(), Request.UserAgent(), blnIsNewLogin)
			
			strPageState = "MENU"
		Else
			dtrDBQuery.Close()
			
			strPageState = "LOGIN"
			strErrorTrigger = "BADLOGIN"
				
			'to reset form variables
			txtUserName.Text = ""
			txtPassword.Text = ""
			chkDoPersistentLogin.Checked = False
		End If
	
	End Sub
	

</script>
<html>
	<head>
	<%
	if not (InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), UCase("win")) = 0 and InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), UCase("mac")) = 0) then
	%>
      <link rel="stylesheet" type="text/css" href="weblabwin.css">
    <%
    else
    %>
	  <link rel="stylesheet" type="text/css" href="weblabathena.css">
	<%end if%>
	</head>
	<body background="button_bg.gif" bgcolor="white" text="black" marginheight="0" marginwidth="0" topmargin="0" leftmargin="0">
		<form Runat="Server">	
	<%
	Select Case strPageState
	
		Case "LOGIN"
	%>
		<script language="JavaScript">
		<!--
		var w=183;
		var agt = navigator.userAgent.toLowerCase();
		if((agt.indexOf('mozilla')!=-1) && (agt.indexOf('compatible') == -1) && (agt.indexOf('hotjava')==-1)) {
		w-=2;
		}

		document.write("<table border=0 cellpadding=0 cellspacing=0>");
		document.write("<tr>");
		document.write("    <td background=\"login_bg.gif\"><img src=\"darkgray.gif\" width="+w+" height=1><br>");

		//'top.main.location.href.indexOf("main.aspx") == "-1" && ' was in line below, removed to refresh main frame when /labserver/admin/main.aspx is present in it)
		if(top.main.location.href.indexOf("error.no-account.aspx") == "-1" && top.main.location.href.indexOf("error.not-authorized.aspx") == "-1" && top.main.location.href.indexOf("error.not-active.aspx") == "-1" && top.main.location.href.indexOf("error.wrong-password.aspx") == "-1") {
			top.main.location.href = "main.aspx";
		}
		//-->
		</script>
		<%
		If strErrorTrigger = "BADLOGIN" Then
		%>
		<script language="JavaScript">
			<!--
				eval("top.main.location.href='error.login-failure.aspx'");
			//-->
		</script>
		<%
		End If
		%>
		<noscript>
			<table border="0" cellpadding="0" cellspacing="0">
				<tr> <!--<td width=40 background="button_bg.gif" valign=top rowspan=2><img src="button_bg.gif" width=33></td>
    <td width=1 rowspan=2 background="darkgray.gif" valign=top><img src="darkgray.gif" width=1 height=1></td>-->
					<td background="login_bg.gif"><img src="darkgray.gif" width="181" height="1"><br>
		</noscript>
		
		
			&nbsp;&nbsp;&nbsp;<font class='regular'><b>Username:</b></font><br>
			&nbsp;&nbsp;&nbsp;<asp:TextBox
									ID="txtUserName"
									Columns="18"
									Runat="Server"
							  />
							  <asp:RequiredFieldValidator
									ControlToValidate="txtUserName"
									Display="Dynamic"
									Text="You must enter your username."
									Runat="Server"
							  /><br>
			&nbsp;&nbsp;&nbsp;<font class='regular'><b>Password:</b></font><br>
			&nbsp;&nbsp;&nbsp;<asp:TextBox
									ID="txtPassword"
									Columns="18"
									TextMode="Password"
									Runat="Server"
							  />
							  <asp:RequiredFieldValidator
									ControlToValidate="txtPassword"
									Display="Dynamic"
									Text="You must enter your password."
									Runat="Server"
							  /><br>
			&nbsp;&nbsp;&nbsp;<font class='small'><b>Remember my login:</b></font>
								<asp:CheckBox
									ID="chkDoPersistentLogin"
									Runat="Server"
								/>	
			<p>
			<center><asp:ImageButton
						ImageURL="menu_login_off.gif"
						AlternativeText="Login"
						CausesValidation="True"
						OnClick="LoginButton_Click"
						Runat="Server"
					/>
			</center>
	
			<br>
				<img src="child.gif" width="17" height="15"><img src="arrow-redblack.gif"> <font class='small'>
					<a href="password-retrieval.aspx" class='offblue' target="main"><b>Lost Your Password?</b></a></font>
			<br>&nbsp;
			<!--
         <font class='small'><a href="" class='offblue'><img src="forgotpassword.gif" border=0 vspace=2></a></font>
         <br>
         <font class='small'><a href="" class='offblue'><img src="register.gif" border=0 vspace=2></a></font>
         -->
			</center>

		</td> 
		<!--<td width=1 rowspan=2 background="darkgray.gif" valign=top><img src="darkgray.gif" width=1></td>-->
		</tr>
		<tr> <!-- bg strip continues -->
			<!-- left line continues -->
			<td background="button_bg.gif">
				<img src="button_separator.gif" width="181"><br>
				<font class="small"><font class="title">&nbsp;Documentation</font>
					<br>
					<img src="child.gif" width="17" height="15"><img src="arrow-redblack.gif" width="6" height="11">
					<a href="system_faq.asp?referrer=Troubleshooting" target="main" class="menu">Link 1</a><br>
					<img src="child.gif" width="17" height="15"><img src="arrow-redblack.gif" width="6" height="11">
					<a href="user_manual.asp?referrer=UserManual" target="main" class="menu">Link 2</a><br>
					<br>
					<font class="title">&nbsp;General</font>
					<br>
					<img src="child.gif" width="17" height="15"><img src="arrow-redblack.gif"> <a href="bug_reporter.asp?referrer=main.asp" target="main" class="menu">
						Link 3</a><br>
					<img src="child.gif" width="17" height="15"><img src="arrow-redblack.gif"> <a href="construction.html?referrer=AboutThisProject" target="main" class="menu">
						Link 4</a><br>
					<br>
				</font>
				<!-- copyright -->
				<center><font class='small'>&copy; 2002 <a href="http://www.mit.edu" target="_none"><font color="#7F0531">
								<b>MIT</b></font></a></font></center>
			</td>
			<!-- right line continues -->
		</tr>
		</table>
	<%Case "MENU"%>
		<%
		If Not Session("LoggedInClassID") Is Nothing And IsNumeric(Session("LoggedInClassID")) Then
		
			If RPMan.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
		%>
	
			<script language="JavaScript">
				<!--
					eval("top.main.location.href='main-sub-frames.aspx'");
				//-->
			</script>
		<%Else%>
			<script language="JavaScript">
				<!--
					eval("top.main.location.href='main.aspx'");
				//-->
			</script>
		<%End If%>
		<noscript>
		<table border=0 cellpadding=0 cellspacing=0>
		<td background="button_bg.gif" valign=top><img src="darkgray.gif" width=181 height=1><br>
		</noscript>
			<font class="small">
			

	<%
		'do resource permission checking (by function group) here
		
		If RPMan.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AcctManagement", "canview")
	%>

			<font class="title">&nbsp;Accounts</font>
			<br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/service-brokers.aspx" class="menu" target="main">Broker Management</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/site-users.aspx" class="menu" target="main">Site User Accounts </a><br>
			<br>
        <%
		End If
			
		If RPMan.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "AccessControl", "canview")
        %>
    
			<font class="title">&nbsp;Access Control</font>
			<br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/usage-classes.aspx" class="menu" target="main">Usage Classes</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/system-resources.aspx" class="menu" target="main">System Resources</a><br>
			<br>
    
        <%
		End If
			
		If RPMan.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SetupManagement", "canview")
        %>
        
			<font class="title">&nbsp;Experiment Setup Management</font>
			<br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/active-setups.aspx" class="menu" target="main">Active Setups</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/experiment-setups.aspx" class="menu" target="main">Experiment Setups</a><br>
			<br>
    
        <%
		End If
			
		If RPMan.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysRecords", "canview")
        %>
    
			<font class="title">&nbsp;System Records</font>
			<br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/wsint-log.aspx" class="menu" target="main">Web Method Activity</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/queue-stat.aspx" class="menu" target="main">Queue Status</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/exec-log.aspx" class="menu" target="main">Execution Records</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/login-log.aspx" class="menu" target="main">Site Logins</a><br>
			<br>
	    
        <%
		End If
			
		If RPMan.GetClassResourcePermission(CInt(Session("LoggedInClassID")), "SysConfig", "canview")
        %>
        
			<font class="title">&nbsp;System Status</font>
			<br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/system-config.aspx" class="menu" target="main">Configuration Settings</a><br>
			<img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="admin/system-notices.aspx" class="menu" target="main">Notices</a><br>
			<br>
    <%
		End If
    %>

        <font class="title">&nbsp;Documentation</font>
        <br>
        <img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="system_faq.asp?referrer=Troubleshooting" target="main" class="menu">Troubleshooting</a><br>
        <img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11> <a href="user_manual.asp?referrer=UserManual" target="main" class="menu">User Manual</a><br>
        <br>

        <font class="title">&nbsp;General</font>
        <br>
        <img src="child.gif" width=17 height=15><img src="arrow-redblack.gif" width=6 height=11>
			<asp:LinkButton
				Text="Logout"
				OnClick="LogoutButton_Click"
				CausesValidation="False"
				ForeColor = "Black"
				Runat="Server" />
		<br>

        </p>
        <!-- copyright --> <center><font class='small'>&copy; 2002 <a href="http://web.mit.edu" target="_none"><font color="#7F0531"><b>MIT</b></font></a></font></center>
        </font>


        
    </td>
</tr>

</table>
		
	<%
		End If
				
	Case Else
	
	End Select
	%>
	

	</form>
	</body>
</html>
