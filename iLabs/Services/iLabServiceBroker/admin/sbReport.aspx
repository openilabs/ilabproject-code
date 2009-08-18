<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="adminNav" Src="adminNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Page language="c#" Inherits="iLabs.ServiceBroker.admin.sbReport" CodeFile="sbReport.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Service Broker Reports</title>
        <!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<style type="text/css">@import url( ../css/main.css );
		</style>

</head>
<body>
		<form id="Form1" method="post" runat="server">
			<a name="top"></a>
			<div id="outerwrapper">
				<uc1:banner id="Banner1" runat="server"></uc1:banner>
				<uc1:adminNav id="AdminNav1" runat="server"></uc1:adminNav>
                <br clear="all" />
				<div id="innerwrapper">
					<div id="pageintro">
						<h1>Service Broker Reports
						</h1>
						<p>Group Specific Reports.
						</p>
						<asp:label id="lblResponse" EnableViewState="False" Visible="False" Runat="server"></asp:label>
					</div>
					<!-- end pageintro div -->
					<div id="pagecontent">
					<div class="simpleform">
					    <table border=1>
					        <tr>
							<td><label for="messagetype">Select the groupname</label> <br>
							<asp:dropdownlist id="ddlGroupTarget" Runat="server" Width="248px"></asp:dropdownlist></SELECT-->
							</td>
							<td><label for="messagetype">Select the report</label>							<br>
							<asp:dropdownlist id="ddlReportTarget" Runat="server" Width="248px"></asp:dropdownlist></SELECT-->
							</td>
							<td><asp:button id="btnSubmit" Runat="server" Width="45px" CssClass="button" Text="Submit" onclick="btnSubmit_Click"></asp:button></td>
							</tr>
                        </table>
					</div>
					<asp:Literal runat="server" EnableViewState="False" Visible="False" ID="ReportDisplayArea"> </asp:Literal>
					<br clear="all">

					<!-- end pagecontent div-->
				</div>
				<!-- end innerwrapper div -->
			</div>
		</form>
</body>
</html>
