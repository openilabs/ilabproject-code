<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientMetadata.aspx.cs" Inherits="iLabs.ServiceBroker.admin.ClientMetadata" validateRequest="false" %>

<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="adminNav" Src="adminNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<!-- 
Copyright (c) 2011 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
<!-- $Id$ -->
<head>
    <title>MIT iLab Service Broker - Client Metadata</title>
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
        <asp:HiddenField ID="hdnMetaId" runat="server" />
        <div id="outerwrapper">
            <uc1:banner ID="Banner1" runat="server"></uc1:banner>
            <uc1:adminNav ID="nav1" runat="server"></uc1:adminNav>
            <br clear="all" />
            <div id="innerwrapper">
                <div id="pageintro">
                    <h1>
                        Client Metadata
                    </h1>
                    <p>
                        Create, modify and publish client metadata. Note: This page is not integrated into the
                        ServiceBroker it currently is used to create a ClientAuthorization Ticket and
                        a simple XML document of the Metadata required to call the WebService method 'LaunchLabClient'.
                        The Ticket &amp; metadata are stored in the database. To generate a new set of Metadata
                        for the selected client, clear the CouponID &amp; Passkey fields before registering. Currently only one metadata record per client is supported.</p>
                    <p>
                        <asp:Label ID="lblResponse" runat="server" Visible="False"></asp:Label></p>
                </div>
                <div id="pagecontent">
                    <div class="simpleform">
                        <table cellspacing="5" cellpadding="0" border="0" style="width: 780px">
                            <tr>
                                <th style="width: 50px"><label for="client">Client </label></th>
                                <td colspan="3"><asp:DropDownList CssClass="i18n" ID="ddlClient" runat="server"  AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlClient_SelectedIndexChanged" Width="754px"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <th style="width: 50px"><label for="groups">Client Groups</label></th>
                                <td style="width: 420px"><asp:DropDownList CssClass="i18n" ID="ddlGroups" runat="server"  AutoPostBack="True" Width="320px"
                                OnSelectedIndexChanged="ddlGroups_SelectedIndexChanged"></asp:DropDownList></td>
                                <th style="width: 100px"><label for="ModificationTime">Modification Time</label></th>
                                <td style="width: 200px"><asp:Textbox CssClass="i18n" ID="txtModTime" runat="server" Width="330px"></asp:Textbox></td>
                            </tr>
                            <tr>
                                <td align="center" colspan="4"><label for="client">Client Authorization Coupon</label></td>
                            </tr>
                            <tr>
                            <th style="width: 50px"><label for="CouponID">CouponID</label></th>
                             <td style="width: 420px"><asp:Textbox CssClass="i18n" ID="txtCouponID" runat="server" Width="330px"></asp:Textbox></td>
                             <th style="width: 100px"><label for="Issuer">Coupon Issuer</label></th>
                             <td style="width: 200px"><asp:Textbox CssClass="i18n" ID="txtIssuer" runat="server" Columns="50"></asp:Textbox></td>
                                
                            </tr>
                            <tr>
                            <th style="width: 50px"><label for="Passkey">Passkey</label></th>
                             <td style="width: 420px"><asp:Textbox CssClass="i18n" ID="txtPasscode" runat="server" Columns="50"></asp:Textbox></td>
                             <td align="center" colspan="2"><asp:Button ID="btnGuid" runat="server" CssClass="button" Text="Generate Passkey" OnClick="btnGuid_Click" /></td>   
                            </tr>
                            <tr>
                                <th style="width: 50px"><label for="client">Metadata </label></th>
                                <td colspan="3"><asp:Textbox CssClass="i18n" ID="txtMetadata" runat="server" Width="750px" Rows="8" TextMode="MultiLine" Enabled="false"></asp:Textbox></td>
                            </tr>
                            <tr>
                                <th style="width: 50px"><label for="client">Client SCORM </label></th>
                                <td colspan="3"><asp:Textbox CssClass="i18n" ID="txtScorm" runat="server" Width="750px" Rows="8" TextMode="MultiLine" Enabled="false"></asp:Textbox></td>
                            </tr>
                            <tr>
                                <th style="width: 50px"><label for="client">Metadata Format </label></th>
                                <td colspan="3"><asp:Textbox CssClass="i18n" ID="txtFormat" runat="server" Width="750px" Rows="3" TextMode="MultiLine" Enabled="false"></asp:Textbox></td>
                            </tr>
                            <tr>
                                <td style="width: 50px">&nbsp;</td>
                                <td colspan="3">
                                <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="button" OnClick="btnRegister_Click" />
                                <asp:Button ID="btnNew" runat="server" Text="Clear" CssClass="button" OnClick="btnNew_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <uc1:footer ID="Footer1" runat="server"></uc1:footer>
        </div>
    </form>
</body>
</html>
