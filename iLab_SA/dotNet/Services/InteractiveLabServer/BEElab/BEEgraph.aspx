<%@ Page language="c#" Inherits="iLabs.LabServer.LabView.BEEgraph" CodeFile="BEEgraph.aspx.cs"  EnableSessionState="true"%>
<%@ Register TagPrefix="uc1" TagName="banner" Src="../banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="userNav" Src="../labNav.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="../footer.ascx" %>

<!DOCTYPE html >
<html lang="en">
	<head>
		<title>Building Energy Efficiency Lab</title> 
		<!-- 
Copyright (c) 2012 The Massachusetts Institute of Technology. All rights reserved.
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
		
        <!--[if lt IE 9]>
            <script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script>
            <![endif]-->
		<script type="text/javascript" src="http://code.jquery.com/jquery-1.7.2.min.js"></script>
        <script type="text/javascript" src="scripts/vendor/underscore.js"></script>
        <script type="text/javascript" src="scripts/vendor/highstock.js"></script>
        <script type="text/javascript" src="scripts/vendor/exporting.src.js"></script>
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
					    <!-- <input type="checkbox" class="series-box" id="cbx_RecNum" value="0" data-array-index="0" name="RecNum"/><label for="cbx_RecNum"></label> -->
						<!--		<input type="checkbox" class="series-box" id="cbx_x" value="21" data-array-index="21" name="x"/><label for="cbx_"></label> -->
						<!--		<input type="checkbox" class="series-box" id="cbx_XX" value="29" data-array-index="29" name="XX"/><label for="cbx_XX">XX</label> -->
						
						
                  <table>
                  <tr><th>Exterior</th><th>Chamber</th><th>Floor</th><th>Ceiling</th></tr>
                  <tr><td>
                        <input type="checkbox" class="series-box" id="cbx_XN1" value="15" data-array-index="15" name="XN1"/><label for="cbx_XN1">XN1</label>
						<input type="checkbox" class="series-box" id="cbx_XN2" value="16" data-array-index="16" name="XN2"/><label for="cbx_XN2">XN2</label>
                        <input type="checkbox" class="series-box" id="cbx_XE1" value="5" data-array-index="5" checked="checked" name="XE1"/><label for="cbx_XE1">XE1</label>
						<input type="checkbox" class="series-box" id="cbx_XE2" value="6" data-array-index="6" name="XE2"/><label for="cbx_XE2">XE2</label>
						<input type="checkbox" class="series-box" id="cbx_XE3" value="7" data-array-index="7" name="XE3"/><label for="cbx_XE3">XE3</label>
						<input type="checkbox" class="series-box" id="cbx_XE4" value="8" data-array-index="8" name="XE4"/><label for="cbx_XE4">XE4</label>
						<input type="checkbox" class="series-box" id="cbx_XS1" value="9" data-array-index="9" name="XS1"/><label for="cbx_XS1">XS1</label>
						<input type="checkbox" class="series-box" id="cbx_XS2" value="10" data-array-index="10" name="XS2"/><label for="cbx_XS2">XS2</label>
						<input type="checkbox" class="series-box" id="cbx_XW1" value="11" data-array-index="11" name="XW1"/><label for="cbx_XW1">XW1</label>
						<input type="checkbox" class="series-box" id="cbx_XW2" value="12" data-array-index="12" name="XW2"/><label for="cbx_XW2">XW2</label>
						<input type="checkbox" class="series-box" id="cbx_XW3" value="13" data-array-index="13" name="XW3"/><label for="cbx_XW3">XW3</label>
						<input type="checkbox" class="series-box" id="cbx_XW4" value="14" data-array-index="14" name="XW4"/><label for="cbx_XW4">XW4</label>
						<input type="checkbox" class="series-box" id="cbx_XF2" value="22" data-array-index="22" name="XF2"/><label for="cbx_XF2">XF2</label>
						<input type="checkbox" class="series-box" id="cbx_XF3" value="23" data-array-index="23" name="XF3"/><label for="cbx_XF3">XF3</label>
						<input type="checkbox" class="series-box" id="cbx_XF4" value="24" data-array-index="24" name="XF4"/><label for="cbx_XF4">XF4</label>
						<input type="checkbox" class="series-box" id="cbx_XF5" value="25" data-array-index="25" name="XF5"/><label for="cbx_XF5">XF5</label>
						<input type="checkbox" class="series-box" id="cbx_XR1" value="19" data-array-index="19" name="XR1"/><label for="cbx_XR1">XR1</label>
						<input type="checkbox" class="series-box" id="cbx_XR3" value="20" data-array-index="20" name="XR3"/><label for="cbx_XR3">XR3</label>
				        <input type="checkbox" class="series-box" id="cbx_XR1S" value="17" data-array-index="17" name="XR1S"/><label for="cbx_XR1S">XR1S</label>
						<input type="checkbox" class="series-box" id="cbx_XR3S" value="18" data-array-index="18" name="XR3S"/><label for="cbx_XR3S">XR3S</label>
                  </td>
                  <td>
                        <input type="checkbox" class="series-box" id="cbx_Tref1" value="2" data-array-index="2" name="AvgTemp 1"/><label for="cbx_Tref1">Avg Temp 1</label>
						<input type="checkbox" class="series-box" id="cbx_Tref2" value="3" data-array-index="3" checked="checked" name="AvgTemp 2"/><label for="cbx_Tref1">Avg Temp 2</label>
					    <input type="checkbox" class="series-box" id="cbx_Tref3" value="4" data-array-index="4" name="AvgTemp 3"/><label for="cbx_Tref1">Avg Temp 3</label>
						<input type="checkbox" class="series-box" id="cbx_N1" value="58" data-array-index="58" name="N1"/><label for="cbx_N1">N1</label>
						<input type="checkbox" class="series-box" id="cbx_N2" value="59" data-array-index="59" name="N2"/><label for="cbx_N2">N2</label>
						<input type="checkbox" class="series-box" id="cbx_E1" value="30" data-array-index="30" name="E1"/><label for="cbx_E1">E1</label>
						<input type="checkbox" class="series-box" id="cbx_E2" value="31" data-array-index="31" name="E2"/><label for="cbx_E2">E2</label>
						<input type="checkbox" class="series-box" id="cbx_E3" value="32" data-array-index="32" name="E3"/><label for="cbx_E3">E3</label>
						<input type="checkbox" class="series-box" id="cbx_E4" value="33" data-array-index="33" name="E4"/><label for="cbx_E4">E4</label>
						<input type="checkbox" class="series-box" id="cbx_S1" value="34" data-array-index="34" name="S1"/><label for="cbx_S1">S1</label>
						<input type="checkbox" class="series-box" id="cbx_S2" value="35" data-array-index="35" name="S2"/><label for="cbx_S2">S2</label>
						<input type="checkbox" class="series-box" id="cbx_W1" value="54" data-array-index="54" name="W1"/><label for="cbx_W1">W1</label>
						<input type="checkbox" class="series-box" id="cbx_W2" value="55" data-array-index="55" checked="checked" name="W2"/><label for="cbx_W2">W2</label>
						<input type="checkbox" class="series-box" id="cbx_W3" value="56" data-array-index="56" name="W3"/><label for="cbx_W3">W3</label>
						<input type="checkbox" class="series-box" id="cbx_W4" value="57" data-array-index="57" name="W4"/><label for="cbx_W4">W4</label>
						<input type="checkbox" class="series-box" id="cbx_CN1" value="66" data-array-index="66" name="CN1"/><label for="cbx_CN1">CN1</label>
						<input type="checkbox" class="series-box" id="cbx_CN2" value="67" data-array-index="67" name="CN2"/><label for="cbx_CN2">CN2</label>
						<input type="checkbox" class="series-box" id="cbx_CN3" value="68" data-array-index="68" name="CN3"/><label for="cbx_CN3">CN3</label>
						<input type="checkbox" class="series-box" id="cbx_CS1" value="42" data-array-index="42" name="CS1"/><label for="cbx_CS1">CS1</label>
						<input type="checkbox" class="series-box" id="cbx_CS2" value="43" data-array-index="43" name="CS2"/><label for="cbx_CS2">CS2</label>
						<input type="checkbox" class="series-box" id="cbx_CS3" value="44" data-array-index="44" name="CS3"/><label for="cbx_CS3">CS3</label>
						<input type="checkbox" class="series-box" id="cbx_N1S" value="69" data-array-index="69" name="N1S"/><label for="cbx_N1S">N1S</label>
						<input type="checkbox" class="series-box" id="cbx_N2S" value="70" data-array-index="70" name="N2S"/><label for="cbx_N2S">N2S</label>
						<input type="checkbox" class="series-box" id="cbx_E2S" value="46" data-array-index="46" name="E2S"/><label for="cbx_E2S">E2S</label>
						<input type="checkbox" class="series-box" id="cbx_E4S" value="47" data-array-index="47" name="E4S"/><label for="cbx_E4S">E4S</label>
						<input type="checkbox" class="series-box" id="cbx_S1S" value="45" data-array-index="45" name="S1S"/><label for="cbx_S1S">S1S</label>
						<input type="checkbox" class="series-box" id="cbx_W2S" value="71" data-array-index="71" name="W2S"/><label for="cbx_W2S">W2S</label>
						<input type="checkbox" class="series-box" id="cbx_W4S" value="72" data-array-index="72" name="W4S"/><label for="cbx_W4S">W4S</label>
                  </td>
                  <td>
                        <input type="checkbox" class="series-box" id="cbx_F1" value="63" data-array-index="63" name="F1"/><label for="cbx_F1">F1</label>
						<input type="checkbox" class="series-box" id="cbx_F2" value="64" data-array-index="64" name="F2"/><label for="cbx_F2">F2</label>
						<input type="checkbox" class="series-box" id="cbx_F3" value="39" data-array-index="39" name="F3"/><label for="cbx_F3">F3</label>
						<input type="checkbox" class="series-box" id="cbx_F4" value="65" data-array-index="65" name="F4"/><label for="cbx_F4">F4</label>
					    <input type="checkbox" class="series-box" id="cbx_F5" value="40" data-array-index="40" name="F5"/><label for="cbx_F5">F5</label>
						<input type="checkbox" class="series-box" id="cbx_F6" value="41" data-array-index="41" name="F6"/><label for="cbx_F6">F6</label>
						<input type="checkbox" class="series-box" id="cbx_F1S" value="76" data-array-index="76" name="F1S"/><label for="cbx_F1S">F1S</label>
						<input type="checkbox" class="series-box" id="cbx_F2S" value="77" data-array-index="77" name="F2S"/><label for="cbx_F2S">F2S</label>
						<input type="checkbox" class="series-box" id="cbx_F3S" value="51" data-array-index="51" name="F3S"/><label for="cbx_F3S">F3S</label>
						<input type="checkbox" class="series-box" id="cbx_F5S" value="52" data-array-index="52" name="F5S"/><label for="cbx_F5S">F5S</label>
						<input type="checkbox" class="series-box" id="cbx_F6S" value="53" data-array-index="53" checked="checked" name="F6S"/><label for="cbx_F6S">F6S</label>
						<input type="checkbox" class="series-box" id="cbx_F3S1" value="26" data-array-index="26" name="F3S1"/><label for="cbx_F3S1">F3S1</label>
						<input type="checkbox" class="series-box" id="cbx_F3S2" value="27" data-array-index="27" name="F3S2"/><label for="cbx_F3S2">F3S2</label>
						<input type="checkbox" class="series-box" id="cbx_F3S3" value="28" data-array-index="28" name="F3S3"/><label for="cbx_F3S3">F3S3</label>
                  </td>
                  <td>
                        <input type="checkbox" class="series-box" id="cbx_R1" value="60" data-array-index="60" name="R1"/><label for="cbx_R1">R1</label>
						<input type="checkbox" class="series-box" id="cbx_R2" value="61" data-array-index="61" checked="checked" name="R2"/><label for="cbx_R2">R2</label>
						<input type="checkbox" class="series-box" id="cbx_R3" value="62" data-array-index="62" name="R3"/><label for="cbx_R3">R3</label>
						<input type="checkbox" class="series-box" id="cbx_R4" value="36" data-array-index="36" name="R4"/><label for="cbx_R4">RW</label>
						<input type="checkbox" class="series-box" id="cbx_R5" value="37" data-array-index="37" name="R5"/><label for="cbx_R5">R5</label>
						<input type="checkbox" class="series-box" id="cbx_R6" value="38" data-array-index="38" name="R6"/><label for="cbx_R6">R6</label>
                        <input type="checkbox" class="series-box" id="cbx_R1S" value="73" data-array-index="73" name="R1S"/><label for="cbx_R1S">R1S</label>
						<input type="checkbox" class="series-box" id="cbx_R2S" value="74" data-array-index="74" name="R2S"/><label for="cbx_R2S">R2S</label>
						<input type="checkbox" class="series-box" id="cbx_R3S" value="75" data-array-index="75" name="R3S"/><label for="cbx_R3S">R3S</label>
						<input type="checkbox" class="series-box" id="cbx_R4S" value="48" data-array-index="48" name="R4S"/><label for="cbx_R4S">R4S</label>
						<input type="checkbox" class="series-box" id="cbx_R5S" value="49" data-array-index="49" name="R5S"/><label for="cbx_R5S">R5S</label>
						<input type="checkbox" class="series-box" id="cbx_R6S" value="50" data-array-index="50" name="R6S"/><label for="cbx_R6S">R6S</label>
						
                  </td></tr>
                  <tr><td>
                        <input type="checkbox" class="series-box" id="Checkbox1" value="80" data-array-index="80" name="Pump"/><label for="cbx_Pump">Pump</label>
						<input type="checkbox" class="series-box" id="Checkbox2" value="81" data-array-index="81" name="Load"/><label for="cbx_Load">Load Wattage</label>
                  </td></tr>
                  </table>
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
