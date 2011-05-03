<%@ Register TagPrefix="uc1" TagName="NavBar" Src="NavBar.ascx" %>
<%@ Register TagPrefix="uc1" TagName="banner" Src="banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="footer.ascx" %>

<%@ Page Language="c#" Inherits="iLabs.Scheduling.LabSide.RevokeReservation" CodeFile="RevokeReservation.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>RevokeReservation</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <style type="text/css">@import url( css/main.css ); 
		</style>
</head>
<body>
    <div id="outerwrapper">
        <uc1:banner ID="Banner1" runat="server"></uc1:banner>
        <uc1:NavBar ID="NavBar1" runat="server"></uc1:NavBar>
        <br clear="all">
        <div id="innerwrapper">
            <div id="pageintro">
                <h1>
                    Revoke Reservation
                </h1>
                <p>
                    <asp:Label ID="lblDescription" runat="server"></asp:Label></p>
                <!-- Administer Groups Error message here: <div class="errormessage"><p>Error message here.</p></div> End error message -->
                <p>
                    <asp:Label ID="lblErrorMessage" runat="server" EnableViewState="False" Visible="False"></asp:Label></p>
            </div>
            <div id="pagecontent">
                    <form id="Form1" method="post" runat="server">
                      <div class="simpleform">
                        <table>
                            <tr>
                                <th style="height: 1px; width: 100px" />
                                <td style="height: 1px; width: 125px" />
                                <td style="height: 1px; width: 50px" />
                                <td style="height: 1px; width: 28px" />
                                <td style="height: 1px; width: 50px" />
                                <td style="height: 1px; width: 200px" />
                            </tr>
                            <tr>
                                <th>
                                    <label for="userMessage">Message to Users</label>
                                </th>
                                <td colspan="5">
                                    <asp:TextBox ID="txtMessage" runat="server" Columns="20" Rows="5" TextMode="MultiLine"
                                        Width="540px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label for="Lab Resource">Lab Resource</label>
                                </th>
                                <td colspan="5">
                                    &nbsp;<asp:DropDownList CssClass="i18n" ID="ddlLabResource" runat="server" AutoPostBack="True"
                                        Width="540px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label for="Experiment">Experiment</label>
                                </th>
                                <td colspan="5">
                                    <asp:DropDownList CssClass="i18n" ID="ddlExperiment" runat="server" AutoPostBack="True"
                                        Width="540px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label for="Group">Group</label>
                                </th>
                                <td colspan="5">
                                    <asp:DropDownList CssClass="i18n" ID="ddlGroup" runat="server" Width="540px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label for="startTime">Start Time</label>
                                </th>
                                <td>
                                    <a href="javascript:;" onclick="window.open('datePickerPopup.aspx?date=start','cal','width=250,height=225,left=270,top=180')">
                                        <img src="calendar.gif" border="0" class="normal" /></a><asp:TextBox ID="txtStartDate"
                                            runat="server" Width="100px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList CssClass="i18n" ID="ddlStartHour" runat="server" Width="48px">
                                        <asp:ListItem>12</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                        <asp:ListItem>3</asp:ListItem>
                                        <asp:ListItem>4</asp:ListItem>
                                        <asp:ListItem>5</asp:ListItem>
                                        <asp:ListItem>6</asp:ListItem>
                                        <asp:ListItem>7</asp:ListItem>
                                        <asp:ListItem>8</asp:ListItem>
                                        <asp:ListItem>9</asp:ListItem>
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>11</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtStartMin" runat="server" Width="24px">00</asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList CssClass="i18n" ID="ddlStartAM" runat="server" Width="48px">
                                        <asp:ListItem>AM</asp:ListItem>
                                        <asp:ListItem>PM</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <th>
                                    <label for="endTime">
                                        End Time</label>
                                </th>
                                <td>
                                    <a href="javascript:;" onclick="window.open('datePickerPopup.aspx?date=end','cal','width=250,height=225,left=270,top=180')">
                                        <img src="calendar.gif" border="0" class="normal" /></a><asp:TextBox ID="txtEndDate"
                                            runat="server" Width="100px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList CssClass="i18n" ID="ddlEndHour" runat="server" Width="48px">
                                        <asp:ListItem>12</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                        <asp:ListItem>3</asp:ListItem>
                                        <asp:ListItem>4</asp:ListItem>
                                        <asp:ListItem>5</asp:ListItem>
                                        <asp:ListItem>6</asp:ListItem>
                                        <asp:ListItem>7</asp:ListItem>
                                        <asp:ListItem>8</asp:ListItem>
                                        <asp:ListItem>9</asp:ListItem>
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>11</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtEndMin" runat="server" Width="24px">00</asp:TextBox>
                                </td>
                                <td>
                                    <asp:DropDownList CssClass="i18n" ID="ddlEndAM" runat="server" Width="48px">
                                        <asp:ListItem>AM</asp:ListItem>
                                        <asp:ListItem>PM</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td align="center"><asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button" OnClick="btnSearch_Click">
                                    </asp:Button></td>
                            </tr>
                            <tr>
                                <td colspan="6">
                                    <asp:TextBox ID="txtDisplay" runat="server" Width="640px" Height="288px" TextMode="MultiLine"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>
                                </th>
                                <td colspan="4" id="tdRevoke" runat="server" visible="false">
                                    <asp:Button ID="btnRevoke" runat="server" Text="Revoke Reservation" CssClass="button"
                                        OnClick="btnRevoke_Click"></asp:Button>&nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnReserve" runat="server" Text="Revoke & Reserve" CssClass="button"
                                        OnClick="btnReserve_Click"></asp:Button>
                                </td>
                            </tr>
                        </table>
                        </div>
                    </form>
            </div>
        </div>
        <uc1:footer ID="Footer1" runat="server"></uc1:footer>
    </div>
</body>
</html>
