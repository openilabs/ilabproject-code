<%@ Page language="c#" Inherits="iLabs.LabServer.BEE.BEEstart" CodeFile="BEEstart.aspx.cs"  EnableSessionState="true"%>
<%@ Register TagPrefix="uc1" TagName="banner" Src="banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="userNav" Src="labNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="footer.ascx" %>

<!DOCTYPE HTML >
<html lang="en">
	<head>
		<!-- 
Copyright (c) 2012 The Massachusetts Institute of Technology. All rights reserved.
Please see license.txt in top level directory for full license. 
-->
 <meta charset="UTF-8" />
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		
  <title>BEE Lab • Launch</title>
  <!-- [if lt IE 9]>
  <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
  <![endif] -->
  <link href='styles/bee_start.css' rel='stylesheet' type='text/css'/>
  <script type="text/javascript" src="http://code.jquery.com/jquery-1.7.2.min.js"></script>
  <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.8.23/jquery-ui.min.js"></script>

  <script type="text/javascript" src="scripts/vendor/highcharts.js"></script>
  <script type="text/javascript" src="scripts/vendor/exporting.src.js"></script>
  <script type="text/javascript" src="scripts/vendor/highstock.theme.grid.js"></script>

  <script type="text/javascript" src="scripts/bee_start.js"></script>
  <script type="text/javascript" src="scripts/chart.js"></script>
  <script type="text/javascript" src="scripts/launching.js"></script>
<style type="text/css">@import url(css/main.css );	</style>	
	</head>
	<body>
		<form method="post" runat="server">
			<a name="top"></a>
			<div id="outerwrapper">
			<uc1:banner id="Banner1" runat="server" BannerText="Building Energy Lab"></uc1:banner><br clear="all"/>
			<uc1:userNav ID="UserNav1" runat="server" ></uc1:userNav>
				<div id="innerwrapper">
					<div id="pageintro">
						<h1><asp:label id="lblExperimentTitle" Runat="server"><% =title %></asp:label></h1>
						<asp:label id="lblDescription" Runat="server"></asp:label>
						<p id="pResponse" runat="server"><asp:label id="lblResponse" Runat="server" Visible="False"></asp:label></p>
					</div><!-- end pageintro div -->
					<div id="pagecontent">
					 
  <h1>Please select the load profile</h1>

<div class="description">
  For each load, you can choose when it will be turned on. You can manage them
  by clicking the buttons for each specific wattage.
</div>

<nav>
  <ul class="navigation">
    <li id='load-label'>
      <strong>Please select a load to manage:</strong>
    </li>
    <li>
      <label for='load-1' class='button'>Load 1</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='0' id='load-1'/>
    </li>

    <li>
      <label for='load-2' class='button'>Load 2</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='1' id='load-2'/>
    </li>

    <li>
      <label for='load-3' class='button'>Load 3</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='2' id='load-3'/>
    </li>

    <li>
      <label for='load-4' class='button'>Load 4</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='3' id='load-4'/>
    </li>
  </ul>
</nav>

<div id="chart-container"></div>
<a class="button" id="js-launch-experiment" href="#">
  Start Experiment
</a>
<asp:button runat="server" cssClass="button" id="btnGo" Text="Run Experiment" Visible="true" onCLick="goButton_Click"/>

<div id="js-load-schedule"></div>


<!-- <a class='js-add-load button' href='#' value='2'>Load 3</a> -->   
					</div><!-- end pagecontent div -->
					<br clear="all" />
				</div><!-- end innerwrapper div -->
				<div><uc1:footer id="Footer1" runat="server"></uc1:footer></div>
				<asp:HiddenField ID="hdnCoupon" runat="server" />
				<asp:HiddenField ID="hdnPasscode" runat="server" />
				<asp:HiddenField ID="hdnIssuer" runat="server" />
				<asp:HiddenField ID="hdnAppkey" runat="server" />
				<asp:HiddenField ID="hdnSbUrl" runat="server" />
				<asp:HiddenField ID="hdnExpID" runat="server" />
				<asp:HiddenField ID="hdnProfile" runat="server" />
				<asp:HiddenField ID="hdnTask" runat="server" />
			</div>
		</form>
	</body>
</html>
