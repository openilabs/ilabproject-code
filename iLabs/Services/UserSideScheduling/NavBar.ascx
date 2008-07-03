
<%@ Control Language="c#" Inherits="iLabs.Scheduling.UserSide.NavBar" CodeFile="NavBar.ascx.cs" %>

<div id="navbar"><div id="nav">
	
	<ul class="navlist" id="ulNavList" runat="server">
	<li>
				 <asp:HyperLink ID="HLRegisterLSS" runat="server" NavigateUrl="~/Administer.aspx">RegisterLSS</asp:HyperLink>
			</li>
			<li >
				<asp:HyperLink ID="HLRegisterExperiment" runat="server" NavigateUrl="~/RegisterExperimentInfo.aspx">Register Experiment</asp:HyperLink>
			</li>
			<li >
				<asp:HyperLink ID="HLPolicyManagement" runat="server" NavigateUrl="~/Manage.aspx">Policy Management</asp:HyperLink>
			</li>
			<li >
				<asp:HyperLink ID="HLReservationInfo" runat="server" NavigateUrl="~/ReservationManagement.aspx">Reservation Information</asp:HyperLink>
			</li>
			<li >
			 <asp:HyperLink ID="HLBackTOSB" runat="server">Back to Service Broker</asp:HyperLink>
			</li>
			
	</ul>
	</div>
</div> <!-- end mainnav div -->

