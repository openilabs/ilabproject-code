<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LabSetup.ascx.cs" Inherits="LabClientHtml.LabControls.LabSetup" %>
<table cols="2">
    <tr>
        <td class="labsetup-label">
            Source:
        </td>
        <td class="labsetup-dataright">
            <asp:DropDownList ID="ddlSource" runat="server" Width="140px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="labsetup-label">
            Absorber:
        </td>
        <td class="labsetup-dataright">
            <asp:DropDownList ID="ddlAbsorber" runat="server" Width="140px">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<table cols="3">
    <tr>
        <td class="labsetup-label">
            Distance:
        </td>
        <td class="labsetup-data160px">
            <asp:DropDownList ID="ddlDistance" runat="server" Width="66px">
            </asp:DropDownList>&nbsp;(mm)
        </td>
        <td class="labsetup-dataright">
            <asp:Button ID="btnDistanceListAdd" runat="server" Text="Add" CssClass="aspbutton"
                OnClick="btnDistanceListAdd_Click" />
        </td>
    </tr>
    <tr>
        <td class="labsetup-label">
            <asp:Label ID="lblDistanceList" runat="server" Text="Distance&nbsp;List:"></asp:Label>
        </td>
        <td class="labsetup-data160px">
            <asp:TextBox ID="txbDistanceList" runat="server" Width="134px" ReadOnly="true"></asp:TextBox>
        </td>
        <td class="labsetup-dataright">
            <asp:Button ID="btnDistanceListClear" runat="server" Text="Clear" CssClass="aspbutton"
                OnClick="btnDistanceListClear_Click" />
        </td>
    </tr>
</table>
<table cols="2">
    <tr>
        <td class="labsetup-label">
            Duration:
        </td>
        <td class="labsetup-dataright">
            <asp:TextBox ID="txbDuration" runat="server" Width="60px"></asp:TextBox>&nbsp;(secs)
        </td>
    </tr>
    <tr>
        <td class="labsetup-label">
            Trials:
        </td>
        <td class="labsetup-dataright">
            <asp:TextBox ID="txbRepeat" runat="server" Width="60px"></asp:TextBox>
        </td>
    </tr>
</table>
