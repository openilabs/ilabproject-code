<%@ Page language="VBscript" %>
<html>
	<head>
		<link rel="stylesheet" type="text/css" href="weblab.css"></head>
	<body bgcolor="white" text="black" link="blue" vlink="blue" alink="blue" topmargin="0" marginheight="0">
		<table border="0" cellspacing="0" cellpadding="0" width="100%">
			<tr>
				<td colspan="2" background="darkgray.gif"><img src="darkgray.gif" width="1" height="1"></td>
			</tr>
			<tr>
				<td valign="top"><font class='extra-small'><b>Current Sponsors:</b> <a href="http://www.carnegie.org" target="_none">Carnegie Corporation</a>
								, <a href="http://www.ni.com" target="_none">National Instruments</a></font></td>
				<td align="right" valign="top" nowrap><font class='extra-small'><b>System Manager:</b> <a href="mailto:<%=application("Admin_email")%>">
							<%=application("Admin_Name")%></a>&nbsp;</font></td>
			</tr>
		</table>
	</body>
</html>
