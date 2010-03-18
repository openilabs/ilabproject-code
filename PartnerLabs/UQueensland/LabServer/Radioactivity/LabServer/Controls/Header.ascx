<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="LabServer.Controls.Header" %>
<table cols="2" cellspacing="2">
    <tr>
        <td class="header-logo">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/level2-arrow.gif" Width="28px"
                Height="28px" />
        <td class="header-title">
            <asp:Label ID="lblTitle" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Image ID="Image2" runat="server" ImageUrl="~/images/level2-underline.gif" Width="395px"
                Height="1px" />
        </td>
    </tr>
</table>
