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
<script type="text/javascript" src="../www/scripts/vendor/underscore.js"></script>
  <script type="text/javascript" src="scripts/vendor/exporting.src.js"></script>
  <script type="text/javascript" src="scripts/vendor/highstock.theme.grid.js"></script>
  <script type="text/javascript" src="scripts/bee_start.js"></script>
  <script type="text/javascript" src="scripts/chart.js"></script>
  <script type="text/javascript" src="scripts/launching.js?v=565757"></script>
<link rel="stylesheet" type="text/css" href="dhtmlxSlider/codebase/dhtmlxslider.css">
<script src="dhtmlxSlider/codebase/dhtmlxcommon.js"></script>
<script src="dhtmlxSlider/codebase/dhtmlxslider.js"></script>
<style type="text/css">@import url(css/main.css );</style>
<script>
//this should eventually take in the number of hours from input.
var sunHours = 0;
var sld = null;

sunData = new Array();
 for (var i = 0; i < sunHours; i++) {
    sunData.push(1);
 }
 for (var i = 0; i < 24-sunHours; i++) {
    sunData.push(0);
 }

var shiftSunGraph = function (sliderValue, sliderObj) {
    newData = new Array();
    for (var i = 0; i < sliderValue; i++) {
        newData.push(0);
    }
    for (var i = 0; i < sunHours; i++) {
        newData.push(1);
    }
    for (var i = 0; i < 24-sunHours-sliderValue; i++) {
        newData.push(0);
    }
    sunData = newData;
    //this updates the chart with the new start position of the sun
    //chart.series[0].update({data: sunData}, true);
    chart.series[0].setData(newData);
    return null;
 }

$(function() {
    $("#sun_hours_select").change(function () {
        sunHours = parseInt($("#sun_hours_select").val());
        shiftSunGraph(0, null);
        sld.setMax(24-sunHours);
    })
});

 //initialize the slider.  may not want 1500px size, need to test and see.
 sld = new dhtmlxSlider("slider", 1500, "arrow", false, 0, 24-sunHours, 0, 1);
 sld.setImagePath("dhtmlxSlider/codebase/imgs/");
 sld.attachEvent("onChange", shiftSunGraph);
 sld.init();


 // create initial data with lamp starting at hour 0.
 var colors = Highcharts.getOptions().colors;

 chart = new Highcharts.Chart({
    chart: {
        renderTo: 'container',
        type: 'column'

    },
    
    xAxis: {
        min:0,
        max:23,
        id:"my xaxis",
        tickInterval: 1,
        title: {
            enabled: true,
            margin: 15
        },
        title: {
            text: 'time'
        },
        labels: {
            enabled: true,
            x: -35
        },
        tickmarkPlacement: 'on',
        showFirstLabel: false,
        tickLength: 0
    },
    
    yAxis: {
        gridLineWidth: 0,
        minorGridLineWidth: 0,
        lineColor: 'transparent',        
        labels: {
            formatter: function() {
                if (this.value == 1) {
                  return "On";
                }
                else if (this.value == 0) {
                    return "Off";
                }
                return "";
            },
            step: 1
        },
        minorTickLength: 0,
        tickLength: 0,
        title: {
            text: 'Sun Lamp'
        },
        max: 1,
        min: 0,
        stackLabels: {
            enabled: true,
            style: {
                fontWeight: 'bold',
                color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
            }
        }
    },
    legend: {
        enabled: false
    },
    title: {
            text: "Sun Lamp"
    },
    tooltip: {
        formatter: function () {
            return '<b>' + this.x + '</b><br/>' + this.series.name + ': ' + this.y + '<br/>'; //+
            //'Total: '+ this.point.stackTotal;
        }
    },
    plotOptions: {
        column: {
            stacking: 'normal',
            point: {
                events: {
                    click: function () {
                        console.log('this.x,y=' + this.x + "," + this.y + " category=" + this.category);

                    }
                }
            },
            dataLabels: {
                enabled: true,
                color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white'
            }
        }
    },
    
    plotOptions: {
        series: {
            marker: {
                enabled: false,
                id: "my scrollbar"
            }
        },
        column: {
            pointPlacement: "between",
            pointStart: 0,
            pointPadding: -(1.0/3),
            borderWidth: 0
        }
    },
    
    series: [{
        data: sunData
        }]
    
    
 });
 chart.series[0].setData(sunData);
</script>	
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
<div class='main'>
  <h2 class='chart-title'>Load Profiles</h2>
  <div id='holder'>
    <div id="chart-container"></div>
  </div>
<nav>
  <ul class="navigation">
    <li id='load-label'>
      <strong>Please select a load to manage:</strong>
    </li>
      <li><input type='radio' class='js-add-load button' name='js-active-load' style='background-color:#A84B3A'  title="Load 1" value='0' id='load-1'></li>
    <li><input type='radio' class='js-add-load button' name='js-active-load'  style='background-color:#FF9F67'  title="Load 2" value='1' id='load-2'></li>
    <li><input type='radio' class='js-add-load button' name='js-active-load' style='background-color:#323138'  title="Load 3" value='2' id='load-3'></li>
    <li><input type='radio' class='js-add-load button' name='js-active-load' style='background-color:#4C646B'  title="Load 4" value='3' id='load-4'></li>
  </ul>
</nav>
  <div class='legend'></div>
<div id="slider" align="center"></div>
Select how many hours per day the sun lamp should be on:

<select name="Select number of hours" id="sun_hours_select">
  <option value="0">0 (off)</option>
  <option value="4">4 hours</option>
  <option value="5">5 hours</option>
  <option value="6">6 hours</option>
  <option value="7">7 hours</option>
  <option value="8">8 hours</option>
  <option value="9">9 hours</option>
  <option value="10">10 hours</option>
  <option value="11">11 hours</option>
  <option value="12">12 hours</option>
  <option value="13">13 hours</option>
  <option value="14">14 hours</option>
  <option value="15">15 hours</option>
</select>
<nav>
  <ul class="navigation">
   <li id='profile-label'>
      <strong>Please select profile and cycles to run:</strong>
    </li>
    <li id = 'profiles' >
	    <select id = "profile-list">
				   <option value = "0">Profiles</option>
	               <option value = "1">Phoenix</option>
	               <option value = "2">Atlanta</option>
	               <option value = "3">Custom 1</option>
	               <option value = "4">Custom 2</option>
	    </select>
    </li> 
  </ul>
</nav>
</div>
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
				<asp:HiddenField ID="hdnExpId" runat="server" />
				<asp:HiddenField ID="hdnProfile" runat="server" value="120,0,2,3,6,6,6"/>
				<asp:HiddenField ID="hdnExpLength" runat="server" value="24"/>
				<asp:HiddenField ID="hdnTimeUnit" runat="server" value="60"/>
				<asp:HiddenField ID="hdnSampleRate" runat="server" value="30"/>
			</div>
		</form>
	</body>
</html>
