<%@ Page language="c#" Inherits="iLabs.LabServer.LabView.BEEgraph" CodeFile="BEEgraph.aspx.cs"  EnableSessionState="true"%>
<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="userNav" Src="../labNav.ascx" %>
<!-- <%@ Register TagPrefix="uc1" TagName="lvpanel" Src="../LVRemotePanel.ascx" %> -->
<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Building Energy Efficiency Lab</title> 
		<!-- 
Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
        <meta name="description" content="BEE Lab Review" />
        <meta name="keywords" content="ilab, MIT, CECI, BEE Lab" />
        <meta name="author" content="CECI" />
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<style type="text/css">@import url( ../css/main.css );
		</style>
		<script type="text/javascript" src="http://code.jquery.com/jquery-1.7.2.min.js"></script>
        <script type="text/javascript" src="scripts/vendor/underscore.js"></script>
        <script type="text/javascript" src="scripts/vendor/highstock.js"></script>
        <script type="text/javascript" src="scripts/application.js"></script>
        <link href='http://fonts.googleapis.com/css?family=Imprima' rel='stylesheet' type='text/css'/>
        <link href='styles/application.css' rel='stylesheet' type='text/css'/>
	</head>
	<body>
		<form method="post" runat="server">
			<a name="top"></a>
			<div id="outerwrapper"><uc1:banner id="Banner1" runat="server" BannerText="Building Energy Lab"></uc1:banner>
			<uc1:userNav id="UserNav1" runat="server" ></uc1:userNav>
			<br clear="all"/>
				<div id="innerwrapper">
					<div id="pageintro">
						<h1><asp:label id="lblExperimentTitle" Runat="server"><% =title %></asp:label></h1>
						<h1>Welcome back! Here's your data</h1>
						<asp:label id="lblDescription" Runat="server"></asp:label>
					</div><!-- end pageintro div -->
					<div id="pagecontent">
					    <p><asp:HyperLink id="lnkBackSB" Text="Back to InteractiveSB" runat="server" ></asp:HyperLink></p>
						 <input type="checkbox" class="series-box" id="avg-temp" checked="checked" value="1" name="Test Chamber Avg Temp"/>
      <label for="avg-temp">Test Chamber Avg Temp</label>

      <input type="checkbox" class="series-box" id="air-temp" checked="checked" value="2" name="External Air Temp" />
      <label for="air-temp">External Air Temp</label>
      <div id="container">Please wait while we load it. This may take a while.</div>
					</div><!-- end pagecontent div -->
					<br clear="all" />
				</div><!-- end innerwrapper div -->
				<div><uc1:footer id="Footer1" runat="server"></uc1:footer></div>
				<asp:HiddenField ID="hdnExperimentID" runat="server" />
			</div>
		</form>
	</body>
</html>
