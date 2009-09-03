<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navmenu.ascx.cs" Inherits="LabClientHtml.Controls.Navmenu" %>
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
                <li class="unselected"><a id="aLabClient" href="~/LabClient.aspx" runat="server">&#187; Home</a></li>
                <li class="unselected"><a id="aSetup" href="~/Setup.aspx" runat="server">&#187; Setup</a></li>
                <li class="unselected"><a id="aStatus" href="~/Status.aspx" runat="server">&#187; Status</a></li>
                <li class="unselected"><a id="aResults" href="~/Results.aspx" runat="server">&#187; Results</a></li>
            </ul>
        </td>
    </tr>
</table>
