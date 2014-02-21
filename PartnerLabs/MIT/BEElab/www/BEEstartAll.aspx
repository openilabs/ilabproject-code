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
		
  <title>BEE Lab Launch</title>
  
<!-- [if lt IE 9]>
  
<script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
  
<![endif] -->
  <link href='styles/bee_start.css' rel='stylesheet' type='text/css'/>
<script type="text/javascript" src="http://code.jquery.com/jquery-1.7.2.min.js"></script>
  <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.8.23/jquery-ui.min.js"></script>
  <script type="text/javascript" src="scripts/vendor/highcharts.js"></script>
<script type="text/javascript" src="scripts/vendor/underscore.js"></script>
  <script type="text/javascript" src="scripts/vendor/exporting.src.js"></script>
  <script type="text/javascript" src="scripts/vendor/highstock.theme.grid.js"></script>
  <script type="text/javascript" src="scripts/dhtmlxSlider/codebase/dhtmlxcommon.js"></script>
<script type="text/javascript" src="scripts/dhtmlxSlider/codebase/dhtmlxslider.js"></script>
<script type="text/javascript" src="scripts/dhtmlxSlider/codebase/ext/dhtmlxslider_start.js"></script>
<script type="text/javascript" src="scripts/beeStartClimate.js"></script>
<script type="text/javascript" src="scripts/chartClimate.js"></script>
<script type="text/javascript" src="scripts/launchAll.js?v=565757"></script>
<link rel="stylesheet" type="text/css" href="scripts/dhtmlxSlider/codebase/dhtmlxslider.css"/>
<style type="text/css">@import url(css/main.css );</style>

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
  <h3>Please select the load profile</h3>
<div class="description">
  For each load, you can choose when it will be turned on. You can manage them
  by clicking the buttons for each specific wattage.
</div>
<br />

<div class='main'>
  <h1 class='chart-title'>Load Profiles</h1>
<div id='holder'>
     <div id="sun-container"></div>
    <div id="sunslider-container"></div>
    <div id="chart-container"></div>	
  </div>
 
  <!-- <div class='legend'></div> -->

<nav>
  <ul class="navigation">
    <li id='load-label'>
      <strong>Select a heat source to manage:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</strong>
    </li>
    <li id='profile-label'>
      <strong>Select a climate profile and cycles to run:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</strong>
    </li>
    <li><strong>Select how many hours per day the sun lamp should be on:</strong></li>
  </ul>
   <ul class="navigation">
    <li>
      <label for='load-1' style='background-color:#A84B3A' class='button'>Load 1</label>
      <input type='radio' class='js-add-load button'  name='js-active-load' value='0' id='load-1'/>
    </li>
    <li>
      <label for='load-2' style='background-color:#FF9F67' class='button'>Load 2</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='1' id='load-2'/>
    </li>
    <li>
      <label for='load-3' style='background-color:#323138' class='button'>Load 3</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='2' id='load-3'/>
    </li>
    <li>
      <label for='load-4' style='background-color:#4C646B' class='button'>Load 4</label>
      <input type='radio' class='js-add-load button' name='js-active-load' value='3' id='load-4'/>
    </li>
    <li id = 'profiles' >
	    <select id="profile-list">
				   <option value = "0" selected="selected">No Climate Profile</option>
				    <option value = "1">Phoenix</option>
				    <option value = "2">Atlanta</option>
	     <!--          <option value = "3">Singapore</option>
	               <option value = "4">Custom 2</option> -->
	    </select>
    </li> 
<li><select name="Select number of hours" id="sun_hours_select">
  <option value="0">0 (off)</option>
  <option value="4">4 hours</option>
  <option value="5">5 hours</option>
  <option value="6">6 hours</option>
  <option value="7">7 hours</option>
  <option value="8">8 hours</option>
  <option value="9">9 hours</option>
  <option value="10" selected="selected">10 hours</option>
  <option value="11">11 hours</option>
  <option value="12">12 hours</option>
  <option value="13">13 hours</option>
  <option value="14">14 hours</option>
  <option value="15">15 hours</option>
</select></li>
<li><div  id="divSunSlider" align="center"></div></li>
 </ul>
</nav>
</div>
<a class='button' id='js-launch-experiment' href='#'>
  Start Experiment
</a>

<!-- <a class='js-add-load button' href='#' value='2'>Load 3</a> -->
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
				<asp:HiddenField ID="hdnExpId" runat="server" />
				<asp:HiddenField ID="hdnClimateProfile" runat="server" />
				<asp:HiddenField ID="hdnProfile" runat="server" value="120,0,2,3,6,6,6"/>
				<asp:HiddenField ID="hdnSunLamp" runat="server" />
				<asp:HiddenField ID="hdnExpLength" runat="server" value="24"/>
				<asp:HiddenField ID="hdnTimeUnit" runat="server" value="60"/>
				<asp:HiddenField ID="hdnSampleRate" runat="server" value="30"/>
			</div>
		</form>
	</body>
</html>
