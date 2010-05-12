<%@ Page Language="C#" MasterPageFile="~/LabClient.Master" AutoEventWireup="true"
    CodeBehind="Results.aspx.cs" Inherits="LabClientHtml.Results" Title="Results" %>

<%@ Register TagPrefix="uc" TagName="LabResults" Src="~/LabControls/LabResults.ascx" %>
<%@ MasterType VirtualPath="~/LabClient.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="styles/LabControls.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="sectiontitle">
        Experiment Results</div>
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
                <asp:Label ID="lblResultMessage" runat="server"></asp:Label>&nbsp;
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td class="button">
                <asp:Button ID="btnRetrieve" runat="server" Text="Retrieve" CssClass="aspbutton"
                    OnClick="btnRetrieve_Click" />
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="aspbutton" OnClick="btnSave_Click" />
                <asp:Button ID="btnDisplay" runat="server" Text="Display" CssClass="aspbutton" OnClick="btnDisplay_Click" />
            </td>
        </tr>
    </table>
    <asp:PlaceHolder ID="phResultsTable" runat="server"></asp:PlaceHolder>
    <table>
        <tr>
            <td>
                <asp:Label ID="lblHiddenResults" runat="server"></asp:Label>&nbsp;
                <asp:Label ID="lblHiddenApplet" runat="server"></asp:Label>&nbsp;
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                &nbsp;
            </td>
        </tr>
    </table>
    <uc:LabResults ID="labResults" runat="server" />
</asp:Content>
