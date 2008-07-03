<%@ Reference Page="~/admin/messages.aspx" %>
<%@ Page language="c#" Inherits="iLabs.ServiceBroker.admin.manageServices" CodeFile="manageServices.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="adminNav" Src="adminNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
	<head>
		<title>MIT iLab Service Broker - Manage Services</title> 
		<!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
		<!-- $Id: manageservices.aspx,v 1.15 2007/05/08 23:08:21 pbailey Exp $ -->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<style type="text/css">@import url( ../css/main.css ); 
		</style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<a name="top"></a>
			<div id="outerwrapper">
				<uc1:banner id="Banner1" runat="server"></uc1:banner>
				<uc1:adminNav id="AdminNav1" runat="server"></uc1:adminNav>
				<br clear="all">
				<div id="innerwrapper">
					<div id="pageintro">
						<h1>Manage Process Agents</h1>
						<p>Add, remove or modify a Service below.</p>
						<p><asp:label id="lblErrorMessage" Runat="server" Visible="False"></asp:label></p>
						<!-- end pageintro div -->
						<div id="pagecontent">
							
							<div class="simpleform">
							    <form id="modifylab" action="" method="post" name="modifylab">
									<table style="WIDTH: 630px; HEIGHT: 460px" cellSpacing="0" cellPadding="5" border="0">
										<TBODY>
										    <tr><th style="width: 85px"><label for="registerBatch">Register BatchLS</label></th>
										    <td style="width: 405px"><asp:checkbox ID="cbxDoBatch" runat="server" AutoPostBack="True" OnCheckedChanged="cbxDoBatch_Changed" /></td>
										    </tr>
											<tr>
												<th style="width: 85px">
													<label for="Service">Service</label></th>
												<td style="width: 405px"><asp:dropdownlist id="ddlService" Runat="server" AutoPostBack="True" Width="360px" onselectedindexchanged="ddlService_SelectedIndexChanged"></asp:dropdownlist></td>
											</tr>
											
											<tr>
												<th style="width: 85px">
													<label for="webserviceurl">Web Service URL</label></th>
												<td style="width: 405px"><asp:textbox id="txtWebServiceURL" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr id="trPasskey" runat="server">
												<th style="width: 85px">
													<label for="outpasskey">Initial Passkey </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtOutPassKey" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 85px">
													<label for="Agentguid">Agent GUID </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtServiceGUID" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr id="trBatchIn" runat="server" visible="false">
											<th style="width: 85px">
													<label for="Passcode In">Passcode In</label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtBatchPassIn" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr id="trBatchOut" runat="server" visible="false">
											<th style="width: 85px">
													<label for="Passcode Out">PasscodeOut</label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtBatchPassOut" Runat="server" Width="500px"></asp:textbox></td>
											</tr><tr>
												<th style="width: 85px">
													<label for="AgentType">Agent Type </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtServiceType" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 85px">
													<label for="codebaseurl">Codebase URL </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtApplicationURL" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 85px">
													<label for="Servicename">Service Name </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtServiceName" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 85px">
													<label for="domainServer">Domain Server </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtDomainServer" Runat="server" Width="500px"></asp:textbox></td>
											</tr><tr>
												<th style="width: 85px">
													<label for="description">Description</label></th>
												<td style="width: 405px"><asp:textbox id="txtServiceDescription" Runat="server" Columns="20" Rows="5" TextMode="MultiLine"
														Width="500px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 85px">
													<label for="infourl">Info URL </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtInfoURL" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 85px">
													<label for="contactemail">Contact Email </label>
												</th>
												<td style="width: 405px"><asp:textbox id="txtContactEmail" Runat="server" Width="500px"></asp:textbox></td>
											</tr>
											<tr id="trAdminGroup" runat="server">
												<th style="width: 85px">
													<label id="lblAdminGroup" runat="server" for="adminGroup">
                                                        Admin Group</label>&nbsp;</th>
												<td style="width: 405px">
                                                    <asp:DropDownList ID="ddlAdminGroup" runat="server" Width="500px">
                                                    </asp:DropDownList>
                                                   </td>
											</tr>
											
											<tr id="trAssociate" runat="server">
												<th style="width: 85px">
													<label id="lblAssociate" runat="server"  for="associatedLSS">Associated LSS</label>
                                                    </th>
												<td style="width: 405px; height: 1px;">
                                                    <asp:DropDownList ID="ddlLSS" runat="server" Width="380px"></asp:DropDownList></td>
                                                    <td rowspan="2">
                                                    <asp:Button ID="btnAssociateLSS" CssClass="button"  runat="server" Text="Associate" OnClick="btnAssociateLSS_Click" />
                                                    </td>
											</tr>
											<tr id="trManage" runat="server">
												<th style="width: 85px">
													<label id="lblManage" runat="server"  for="ManageLSS">Manage on LSS</label>
                                                    </th>
												<td style="width: 405px; height: 1px;">
                                                    <asp:DropDownList ID="ddlManageLSS" runat="server" Width="377px"></asp:DropDownList>
                                                    </td>
											</tr>
                                            <tr align="center" >
												<th colspan="2" style="height: 34px">
													<asp:button id="btnSaveChanges" Runat="server" CssClass="button" Text="Save Changes" onclick="btnSaveChanges_Click"></asp:button>
													<asp:button id="btnRemove" Runat="server" CssClass="button" Text="Remove" onclick="btnRemove_Click"></asp:button>
													<asp:button id="btnNew" runat="server" CssClass="button" Text="New" onclick="btnNew_Click"></asp:button>
												</th>
											</tr>
                                            <tr align="center">
                                                <th colspan="2" >
                                                    &nbsp;<asp:button id="btnRegister" Runat="server" CssClass="button" Text="Install Domain Credentials" onclick="btnRegister_Click"></asp:button>
                                                    &nbsp;<asp:Button ID="btnAdminURLs" runat="server" CssClass="button"  Text="Domain URLs" Width="140px" /></th>
                                            </tr>
										</TBODY>
									</table>
								</form>
								</div>
							</div>
						</div>
					</div>
					</div>
				</form>
				<br clear="all" />
						<!-- end pagecontent div -->
						/div>
					<!-- end innerwrapper div --><uc1:footer id="Footer1" runat="server"></uc1:footer>
				
	</body>
</html>
