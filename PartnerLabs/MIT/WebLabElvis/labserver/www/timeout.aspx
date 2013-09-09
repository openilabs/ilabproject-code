<%@ Page language="VBscript" %>


<Script Runat="Server">


	Sub Page_Load
		FormsAuthentication.SignOut()
		
		Session.Remove("LoggedInAsUserName")
		Session.Remove("LoggedInUserID")
		Session.Remove("LoggedInClassID")
			
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
	<body>
	
		<script language="JavaScript">
			<!--
				eval("top.menu.location.href='login.aspx'");
				eval("top.main.location.href='main.aspx'");
			//-->
		</script>
	
	</body>
</html>
