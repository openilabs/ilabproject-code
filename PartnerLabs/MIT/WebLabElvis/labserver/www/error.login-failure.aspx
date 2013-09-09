<%@ Page language="VBscript" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
 <head>
   <title>MIT NI-ELVIS WebLab</title>
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
<body bgcolor="white" text="black">
<center>
  <table border=0 cellpadding=10 width=95% ID="Table1">
	<tr>
		<td>
			<font class="title">
				<b>Authentication Error!</b>
			</font>
			<hr size=1>
			<font class="regular">
			<p>
				The specified username/password does not exist in our records.  If you have an account, but do not remember your password, you may retrieve it <a href="password-retrieval.aspx">here</a>.
				<p>
				If you think this may be a system error, please contact the <A href="mailto:<%=application("Admin_email")%>">system administrator</a>.
			</font>
		</td>
	</tr>
</table>
</body>
</html>