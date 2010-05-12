<%@ Page Language="C#" MasterPageFile="~/LabClient.Master" AutoEventWireup="true"
    CodeBehind="Status.aspx.cs" Inherits="LabClientHtml.Status" Title="Status" %>

<%@ MasterType VirtualPath="~/LabClient.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="sectiontitle">
        LabServer Status</div>
    <table cols="2">
        <tr>
            <td class="label">
                Status:
            </td>
            <td class="dataright">
                <asp:Label ID="lblOnline" runat="server"></asp:Label>&nbsp;
            </td>
        </tr>
        <tr>
            <td class="label">
                Message:
            </td>
            <td class="message-data">
                <asp:Label ID="lblLabServerStatusMsg" runat="server"></asp:Label>&nbsp;
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td class="button">
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="aspbutton" OnClick="btnRefresh_Click1" />
            </td>
        </tr>
    </table>
    <p>
    </p>
    <div class="sectiontitle">
        Experiment Status</div>
    <table cols="2">
        <tr>
            <td class="label">
                Experiment:
            </td>
            <td class="dataright">
                <asp:TextBox ID="txbExperimentID" runat="server" Width="60px"></asp:TextBox>
                <asp:DropDownList ID="ddlExperimentIDs" runat="server" Width="66px" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlExperimentIDs_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="label">
                Message:
            </td>
            <td class="message-data">
                <asp:Label ID="lblExperimentStatusMsg" runat="server"></asp:Label>&nbsp;
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td class="button">
                <asp:Button ID="btnCheck" runat="server" Text="Check" CssClass="aspbutton" OnClick="btnCheck_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="aspbutton" OnClick="btnCancel_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
