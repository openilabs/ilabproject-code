<%@ Page Language="VBScript" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="WebLabDataManagers.WebLabDataManagers" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>Method Test</title>

  </head>
  <script runat="Server">
	Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
	Dim rmObject As RecordManager = New RecordManager()
	Dim strEstOut as String
	
	Sub Page_Load
	
	'strEstOut = rpmObject.GetLabConfig(1)
	Dim strOutput() as String = rmObject.RetrieveResult(1, 12)
	strEstOut = ""
	
	Dim loopIdx as Integer
	For loopIdx = 0 To UBound(strOutput, 1) - 1
		strEstOut = strEstOut & "<>" & strOutput(loopIdx)
	Next
	
	'strEstOut = strOutput(0) & "<>" & strOutput(1) & "<>" & strOutput(2)
	End Sub
	
  
  </script>
  <body>
    <form runat="server">
		<%
		Response.Write(strEstOut)
		%>

    </form>
  </body>
</html>
