<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="LabServer.Controls.Login" %>
<table cols="3" cellspacing="2" width="300px">
    <tr>
        <td>
            Username:
        </td>
        <td>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="login-textbox"></asp:TextBox>
            <%--
                        <asp:RegularExpressionValidator ID="rexValidator" runat="server" ControlToValidate="txtUsername" ValidationExpression="\d{1,40}"
                ErrorMessage="Username cannot be longer than 40 characters"></asp:RegularExpressionValidator>
--%>
        </td>
        <td width="100%">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            Password:
        </td>
        <td>
            <asp:TextBox ID="txtPassword" runat="server"  CssClass="login-textbox" TextMode="Password"></asp:TextBox>
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            <asp:Button ID="btnLogIn" runat="server" Text="Login" CssClass="login-button" OnClick="btnLogIn_Click">
            </asp:Button>
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
    <tr>
        <td colspan="3">
            <!-- Error message for login-->
            <asp:Label ID="lblLoginErrorMessage" runat="server" Visible="False"></asp:Label>
        </td>
    </tr>
</table>
