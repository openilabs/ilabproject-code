<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="adminNav" Src="adminNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Page language="c#" Inherits="iLabs.ServiceBroker.admin.experimentRecords" validateRequest="false" CodeFile="experimentRecords.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
<!-- $Id: experimentrecords.aspx,v 1.5 2008/04/11 19:53:33 pbailey Exp $ -->
	<head>
		<title>MIT iLab Service Broker - Experiment Records</title>
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
				<uc1:adminnav id="AdminNav1" runat="server"></uc1:adminnav>
				<br clear="all" />
				<div id="innerwrapper">
					<div id="pageintro">
						<h1>Experiments</h1>
						<p>View experiments below.</p>
						<!-- Error message here --><asp:label id="lblResponse" EnableViewState="False" Visible="False" Runat="server"></asp:label></div>
					<!-- end pageintro div -->
					<div id="pagecontent">
						<table cols="2">
						<tr>
						<td valign="top">
						<div class="simpleform">
							<table cols="4">
								<tr>
									<th>
										<label for="timeis">Username</label></th>
									<td colspan="3"><asp:textbox id="txtUsername" Runat="server"></asp:textbox>
									</td>
										
								</tr>
									<!-- the following field uses the class "noneditable" if the user does not select between from the drop-down list -->
								<tr>
									<th>
										<label for="timeis">Groupname</label></th>
									<td colspan="3"><asp:textbox id="txtGroupname" Runat="server"></asp:textbox></td>
									<!-- the following field uses the class "noneditable" if the user does not select between from the drop-down list --></tr>
								<tr>
									<th style="HEIGHT: 11px">
										<label for="timeis">Time </label>
									</th>
									<td style="HEIGHT: 11px" colspan="3"><asp:dropdownlist id="ddlTimeAttribute" Runat="server" Width="112px" AutoPostBack="True" onselectedindexchanged="ddlTimeAttribute_SelectedIndexChanged">
											<asp:ListItem Value="-- Select one --">-- Select one --</asp:ListItem>
											<asp:ListItem Value="equal to">equal to</asp:ListItem>
											<asp:ListItem Value="before">before</asp:ListItem>
											<asp:ListItem Value="after">after</asp:ListItem>
											<asp:ListItem Value="between">between</asp:ListItem>
											<asp:ListItem Value="on date">on date</asp:ListItem>
										</asp:dropdownlist></td>
									<!-- the following field uses the class "noneditable" if the user does not select between from the drop-down list --></tr>
								<tr>
									<th>
										&nbsp;</th>
									<td style="WIDTH: 78px"><asp:textbox id="txtTime1" Runat="server" Width="96px"></asp:textbox>
									</td>
									<td style="WIDTH: 78px"><asp:textbox id="txtTime2" Runat="server" ReadOnly="true" Width="96px" BackColor="Lavender"></asp:textbox>
									</td>
									<td>&nbsp;
									</td>
								</tr>
								<tr>
								<th>
										&nbsp;</th>
									<td colspan="3"><asp:button id="btnGo" Runat="server" CssClass="button" Text="Search Experiments" onclick="btnGo_Click"></asp:button>
									</td>
								</tr>
							</table>
						</div>
						<div class="simpleform"><label for="selectexperiment">Select experiment</label>
							<br />
							<asp:listbox id="lbxSelectExperiment" Runat="server" Rows="20" Width="312px" AutoPostBack="True"
								Height="354px" onselectedindexchanged="lbxSelectExperiment_SelectedIndexChanged"></asp:listbox>
						</div>
						</td>
						<td valign="top">
						<div id="itemdisplay">
							<h4>Selected Experiment</h4>
							<div class="simpleform">
								<table>
									<tr>
										<th>
											<label for="experimentid">Experiment ID</label></th>
										<td><asp:textbox id="txtExperimentID" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th style="HEIGHT: 24px">
											<label for="username">User Name </label>
										</th>
										<td style="HEIGHT: 24px"><asp:textbox id="txtUserName1" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="groupname">Effective Group Name </label>
										</th>
										<td><asp:textbox id="txtGroupName1" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="clientname">Client Name</label></th>
										<td><asp:textbox id="txtClientName" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="labservername">Lab Server Name</label></th>
										<td><asp:textbox id="txtLabServerName" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="status">Status</label></th>
										<td><asp:textbox id="txtStatus" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="subtime">Submission Time </label>
										</th>
										<td><asp:textbox id="txtSubTime" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="comtime">Completion Time </label>
										</th>
										<td><asp:textbox id="txtComtime" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="recordCount">Total Records </label>
										</th>
										<td><asp:textbox id="txtRecordCount" Runat="server" ReadOnly="true"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th>
											<label for="annotation">Annotation</label></th>
										<td><asp:textbox id="txtAnnotation" Runat="server" ReadOnly="true" Columns="20" Rows="3" TextMode="MultiLine"></asp:textbox>
										</td>
									</tr>
									<tr>
										<th colspan="2">
											<asp:button id="btnSaveAnnotation" Runat="server" Text="Save Annotation" CssClass="buttonright" onclick="btnSaveAnnotation_Click"></asp:button>
										</th>
									</tr>							
									<tr>
										<th colspan="2">
											<asp:button id="btnDeleteExperiment" Runat="server" Text="Delete Experiment" CssClass="buttonright" onclick="btnDeleteExperiment_Click"></asp:button>
										</th>
									</tr>
									<tr id="showExperiment">
										<th colspan="2">
											<asp:button id="btnShowExperiment" Runat="server" Text="Display Experiment Records" Enabled="false" CssClass="buttonright" onclick="btnShowExperiment_Click"></asp:button>
										</th>
									</tr>
								</table>
							</div>
						</div>
						</td>
						</tr>
						</table>
						<p>&nbsp;</p>
					</div>
					<br clear="all" />
					<!-- end pagecontent div --></div> <!-- end innerwrapper div --><uc1:footer id="Footer1" runat="server"></uc1:footer></div>
		</form>
	</body>
</html>
