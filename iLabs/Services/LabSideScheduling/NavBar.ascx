<%@ Control Language="c#" Inherits="iLabs.Scheduling.LabSide.NavBar" CodeFile="NavBar.ascx.cs" %>
<div id="navbar"><div id="nav">
	
	<ul class="navlist" id="ulNavList" runat="server">
	<li>
				    <asp:HyperLink ID="HLRegisterUSS" runat="server" NavigateUrl="~/Administer.aspx">Register USS</asp:HyperLink>
			</li>
			<li>
                    <asp:HyperLink ID="HLRegisterGroup" runat="server" NavigateUrl="~/RegisterGroup.aspx">Register Group</asp:HyperLink>
				</li>
				<li>
				    <asp:HyperLink ID="HLexpInfoManagement"
                        runat="server" NavigateUrl="~/Manage.aspx">Experiment Information Management</asp:HyperLink>
				</li>
				<li>
                    <asp:HyperLink ID="HLReservationInfo" runat="server" NavigateUrl="~/ReservationInfo.aspx">Reservation Information</asp:HyperLink>
               </li>
               <li>
				    <asp:HyperLink ID="HLRevokeReservation" runat="server" NavigateUrl="~/RevokeReservation.aspx">Revoke Reservation</asp:HyperLink>
				</li>
				<li>
				<asp:HyperLink ID="HLTimeBlockManage" runat="server" NavigateUrl="~/TimeBlockManagement.aspx">TimeBlock Management</asp:HyperLink>
				</li>
				<li>
				<asp:HyperLink ID="HLBackToSB" runat="server">Back to Service Broker</asp:HyperLink>
				</li>
		
</ul>
	</div>
</div>
