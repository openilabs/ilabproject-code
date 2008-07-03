<%@ Page language="c#" Inherits="iLabs.Scheduling.UserSide.SelectTimeSlots" CodeFile="SelectTimeSlots.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Select Time Slots</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<style type="text/css">@import url( css/popup.css ); 
		</style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div id="wrapper" style="padding-bottom: 15px; padding-top: 15px">
			<div id="pageintro">
                    <h3>
                       <asp:label id="lblTitleofSchedule"  Runat="server" 
							text="Please Check sequential  time slots, then click the make reservation button."></asp:label>
                        <asp:Label ID="lblTimeSlotsPolicy" runat="server" ></asp:Label>
                    </h3>
					<p><asp:label id="lblErrorMessage" EnableViewState="False" Visible="False" Runat="server"></asp:label></p></div>
				<div id="pagecontent">		
			<table>
			<tr>
					<td style="height: 15px"><asp:button id="btnMakeReservation1" runat="server" Text="Make Reservation" CssClass="button" onclick="btnMakeReservation_Click"></asp:button></td>
				</tr>
				<tr>
					<td style="height: 3px"><asp:repeater id="repTimeSlotsInfo" runat="server"></asp:repeater></td>
				</tr>
				<tr>
					<td style="height: 15px"><asp:button id="btnMakeReservation" runat="server" Text="Make Reservation" CssClass="button" onclick="btnMakeReservation_Click" TabIndex="1"></asp:button></td>
				</tr>
			
			</table>
			</div>
			</div>
		</form>
	</body>
</HTML>



