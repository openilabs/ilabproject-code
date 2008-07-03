<%@ Control Language="c#" Inherits="iLabs.LabServer.userNav" CodeFile="userNav.ascx.cs" %>
<!-- This the main navigation. The buttons that are visible depend on which page the user is on. -->
<!-- *******NOTE - The active page in the nav bar should have <a href="abc" class="topactive">.. 
The first item in the nav bar should always have <a href="abc" class="first">.
The last item in the nav bar should always have <a href="abc" class="last"> ********* -->
<div id="navbar"><div id="nav">
		<ul class="navlist" id="ulNavList" runat="server">
			<li>
				<A id="aHome" href="home.aspx" runat="server">Home</A>
			</li>
			
			<li id="liNavlistAdmin" runat="server">
				<A id="aSelfRegistration" href="selfRegistration.aspx" runat="server">Self Registration</A>
			</li>
			<li id="liNavlistMyGroups" runat="server">
				<A id="aMyGroups" href="localGroups.aspx" runat="server">Groups</A>
			</li>
			<li id="liNavlistMyLabs" runat="server">
				<A id="aMyLabs" href="groupPermissions.aspx" runat="server">Permissions</A>
			</li>
			<li id="liNavlistExperiments" runat="server">
				<A id="aMyExperiments" href="labExperiments.aspx" runat="server">Lab Experiments</A>
			</li>
			<li id="liNavlistMyAccount" runat="server">
				<A id="aMyAccount" href="manageTasks.aspx" runat="server">Tasks</A>
			</li>
			
		</ul>
	</div>
	<!-- end nav div -->
	<div id="nav2">
		<!-- This is where the help and logout buttons go. Log out only appears if the user is logged in. -->
		<ul class="navlist2">
			<li>
				<A id="aHelp" runat="server">Help</A>
			<li>
				<asp:linkbutton id="lbtnLogout" Runat="server" onclick="lbtnLogout_Click">Log out</asp:linkbutton></li></ul>
	</div> <!-- end nav2 div -->
</div> <!-- end navbar -->
