<%@ Page Language="C#" MasterPageFile="~/LabClient.Master" AutoEventWireup="true"
    CodeBehind="Setup.aspx.cs" Inherits="LabClientHtml.Setup" Title="Setup" %>

<%@ Register TagPrefix="uc" TagName="LabSetup" Src="~/LabControls/LabSetup.ascx" %>
<%@ MasterType VirtualPath="~/LabClient.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="styles/LabControls.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="sectiontitle">
        Experiment Setups</div>
    <table cols="2">
        <tr>
            <td class="setup-label">
                Setup:
            </td>
            <td class="dataright">
                <asp:DropDownList ID="ddlExperimentSetups" runat="server" CssClass="setup-dropdownlist"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlExperimentSetups_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:DropDownList ID="ddlExperimentSetupIds" runat="server" Visible="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="setup-label">
                &nbsp;
            </td>
            <td class="setup-description">
                <asp:Label ID="lblSetupDescription" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <uc:LabSetup ID="labSetup" runat="server" />
    <table cols="2">
        <tr>
            <td class="label">
                Message:
            </td>
            <td class="message-data">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>&nbsp;
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td class="button">
                <asp:Button ID="btnValidate" runat="server" Text="Validate" CssClass="aspbutton"
                    OnClick="btnValidate_Click1" />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="aspbutton" OnClick="btnSubmit_Click1" />
            </td>
        </tr>
    </table>
</asp:Content>
