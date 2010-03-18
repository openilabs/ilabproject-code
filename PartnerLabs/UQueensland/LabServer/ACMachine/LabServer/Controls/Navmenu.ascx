<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navmenu.ascx.cs" Inherits="LabServer.Controls.Navmenu" %>
<table cols="1" cellspacing="0">
    <tr>
        <td class="navmenu-photo">
            <asp:Image ID="NavmenuPhoto" runat="server" Width="200px" />
        </td>
    </tr>
    <tr>
        <td class="navmenu-header">
            ON THIS SITE
        </td>
    </tr>
    <tr>
        <td class="navmenu-item">
            <ul>
                <li class="unselected"><a id="aHome" href="~/Administration.aspx" runat="server">&#187; Administration</a></li>
            </ul>
        </td>
    </tr>
</table>
