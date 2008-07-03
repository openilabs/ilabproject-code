<%@ Page language="c#" Inherits="iLabs.Services.SelfRegistration" CodeFile="SelfRegistration.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="adminNav" Src="adminNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>selfRegistration</title> 
		<!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<style type="text/css">@import url( ../css/main.css );
		</style>
	</HEAD>
	<body>
		<form method="post" runat="server">
			<a name="top"></a>
			<div id="outerwrapper"><uc1:banner id="Banner1" runat="server"></uc1:banner>
			<uc1:adminNav id="AdminNav1" runat="server"></uc1:adminNav>
				<div id="innerwrapper">
					<div id="pageintro">
						<h1><asp:label id="lblTitle" Runat="server"></asp:label></h1>
						<asp:label id="lblDescription" Runat="server"></asp:label>
						<p><asp:label id="lblResponse" Runat="server" Visible="False"></asp:label></p>
					</div>
					<div id="Div1"  runat="server">
					<asp:CustomValidator ID="valGuid" ControlToValidate="txtServiceGuid" OnServerValidate="checkGuid" 
                    Text="A Guid must be unique and no longer than 50 characters" runat="server"/>
					
					</div>
					<!-- end pageintro div -->
					<div id="pagecontent">
						<p><asp:HyperLink id="lnkBackSB" Text="Back to InteractiveSB" runat="server" ></asp:HyperLink></p>
						
						<div class="simpleform">
						    <form id="serviceInfo" action="" method="post" name="serviceInfo">
									<table style="WIDTH: 564px; HEIGHT: 460px" cellSpacing="0" cellPadding="5" border="0">
										<TBODY>
											<tr>
												<th style="width: 480px">
													<label for="serviceType">Service Type</label></th>
													<% // Change this to the type of ProcessAgent %>
												<td style="width: 484px"><asp:label id="lblServiceType" Runat="server" Width="360px"></asp:label></td>
											</tr>
											<tr>
												<th style="width: 480px">
													<label for="serviceName">Service Name</label></th>
												<td style="width: 484px"><asp:textbox id="txtServiceName" Runat="server" Width="360px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 480px">
													<label for="agentGuid">Service GUID</label></th>
												<td style="width: 484px"><asp:textbox id="txtServiceGUID" Runat="server" Width="360px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 480px">
													<label for="codebaseUrl">Codebase URL</label></th>
												<td style="width: 484px"><asp:textbox id="txtCodebaseUrl" Runat="server" Width="360px"></asp:textbox></td>
											</tr>
											<tr>
												<th style="width: 480px">
													<label for="webServiceUrl">Web Service URL</label></th>
												<td style="width: 484px"><asp:textbox id="txtWebServiceUrl" Runat="server" Width="360px"></asp:textbox></td>
											</tr>
											<tr id="trPasskey" runat="server">
												<th style="width: 480px">
													<label for="outpasskey">Initial Passkey </label></th>
												<td style="width: 484px"><asp:textbox id="txtOutPassKey" Runat="server" Width="360px"></asp:textbox></td>
											</tr>
                                            <tr id="trDomainSB" runat="server">
												<th style="width: 480px">
													<label for="DomainServer">Domain ServiceBroker </label></th>
												<td style="width: 484px"><asp:textbox id="txtDomainServer" Runat="server" Width="360px" ReadOnly="true"></asp:textbox></td>
											</tr>
											<tr>
												<th colspan="2">
												<asp:button id="btnGuid" Runat="server" CssClass="button" Text="Create GUID" onclick="btnGuid_Click"></asp:button>
                                                    &nbsp;&nbsp;<asp:button id="btnSaveChanges" Runat="server" CssClass="button" Text="Save Changes" onclick="btnSaveChanges_Click"></asp:button>
                                                    &nbsp;&nbsp;<asp:button id="btnRefresh" Runat="server" CssClass="button" Text="Refresh" onclick="btnRefresh_Click"></asp:button>
                                                    &nbsp;&nbsp;<asp:button id="btnNew" Runat="server" CssClass="button" Text="Clear" onclick="btnNew_Click"></asp:button>
                                                 </th>
                                            </tr>
											<tr>
												<th colspan="2"></th>
											</tr>
										</TBODY>
									</table>
								</form>
						 </div>
					<!-- end pagecontent div --></div>
				<!-- end innerwrapper div --><uc1:footer id="Footer1" runat="server"></uc1:footer></div>
				</div>
				<br clear="all"/>
				</form>
				
	</body>
</HTML>
