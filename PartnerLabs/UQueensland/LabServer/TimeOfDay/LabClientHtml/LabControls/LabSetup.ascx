<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LabSetup.ascx.cs" Inherits="LabClientHtml.LabControls.LabSetup" %>
<table cols="2">
    <tr>
        <td class="labsetup-label">
            <asp:Label ID="lblTimeServerUrl" runat="server" Text="<nobr>Server Url:</nobr>"></asp:Label>
        </td>
        <td class="labsetup-dataright">
            <asp:DropDownList ID="ddlTimeServerUrl" runat="server" Width="160px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="labsetup-label">
            Time&nbsp;format:
        </td>
        <td class="labsetup-dataright">
            <asp:DropDownList ID="ddlTimeFormat" runat="server" Width="100px">
            </asp:DropDownList>
        </td>
    </tr>
</table>
