<%@ Page Language="VBScript" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="WebLab.DataTypes" %>
<%@ Import Namespace="WebLab.DataManagers" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>Method Test</title>

  </head>
  <script runat="Server">
	Dim rpmObject As ResourcePermissionManager = New ResourcePermissionManager()
	Dim rmObject As RecordManager = New RecordManager()
	Dim strEstOut as String = ""
	
	Sub Page_Load
	
	
	Dim resOutput as ResultObject = rmObject.RetrieveResult(1, 12)	
	<%--
	    strEstOut = resOutput.ExperimentStatus()
		strEstOut = strEstOut & "<>" & resOutput.LabConfig
		strEstOut = strEstOut & "<>" & resOutput.ExperimentResults
		strEstOut = strEstOut & "<>" & resOutput.ErrorMessages
		
	--%>
	
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
