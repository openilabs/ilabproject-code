<%@ Page language="VBscript" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<script Runat="Server">
	Dim conWebLabLS As SqlConnection = New SqlConnection(ConfigurationSettings.AppSettings("conString"))

	Sub Page_Load
		Dim strDBQuery As String
		Dim cmdDBQuery As SqlCommand
		Dim dtrDBQuery As SqlDataReader
		conWebLabLS.Open()
		
		strDBQuery = "SELECT title, body, date_entered from SystemNotices WHERE is_displayed = 1 ORDER BY date_entered DESC;"
		cmdDBQuery = New SqlCommand(strDBQuery, conWebLabLS)
		
		dtrDBQuery = cmdDBQuery.ExecuteReader()
		rptNotices.DataSource = dtrDBQuery
		rptNotices.DataBind()
		
		dtrDBQuery.Close()
	End Sub
	
	Sub Page_PreRender
		conWebLabLS.Close()
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
	<body bgcolor="white" text="black">
	<form Runat="Server">
		<asp:Repeater
			ID="rptNotices"
			Runat="Server">
			
			<ItemTemplate>
				<font class='title'><%#Container.DataItem("title")%></font><font class='small'> - <%#Container.DataItem("date_entered")%><br>
				<%#Container.DataItem("body")%></font>
				<p>
			</ItemTemplate>
		</asp:Repeater>	
		<p>
			
		</p>
	
	<!--	<img src="blank.gif" width="1" height="1000">-->
	</form>
	</body>
</html>
