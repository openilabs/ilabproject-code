 var PhoenixData = [91.04, 86, 84.02, 82.04, 80.96, 80.96, 80.06, 84.92, 89.96, 91.94, 95, 100.04, 102.02, 102.02, 105.08, 107.06, 107.96, 107.06, 107.06, 105.98, 102.92, 100.04, 98.06, 96.98, 93.92, 91.94, 89.06, 87.98, 84.92, 82.94, 84.02, 84.92, 89.96, 91.94, 96.08, 98.06, 100.04, 102.02, 104, 105.98, 105.98, 107.96, 107.06, 105.08, 102.02, 95, 93.02, 89.96, 89.96, 89.06, 84.92, 84.92, 86, 86, 84.02, 86, 89.06, 91.94, 93.92, 96.98, 100.04, 100.04, 100.94, 102.92, 104, 102.92, 104, 102.02, 100.04, 96.98, 96.08, 96.08, 93.92, 89.06, 89.06, 87.98, 87.08, 86, 82.04, 82.94, 86, 86, 87.98, 95, 98.96, 102.02, 102.02, 105.08, 105.98, 105.98, 105.98, 104, 102.02, 100.94, 98.96, 93.92, 91.04, 87.08, 86, 84.02, 82.04, 80.96, 84.02, 84.02, 89.06, 93.02, 96.08, 98.96, 104, 107.06, 111.02, 111.92, 111.92, 113, 111.92, 109.04, 100.04, 96.98, 98.06, 96.08, 89.06, 86, 84.02, 84.02, 82.94, 82.04, 82.04, 84.02, 87.08, 89.96, 93.02, 96.98, 98.96, 102.02, 102.92, 104, 105.98, 104, 104, 102.02, 98.96, 96.98, 95, 93.02, 91.04, 89.96, 87.08, 86, 82.04, 80.96, 82.04, 84.92, 89.06, 89.96, 91.94, 93.92, 95, 96.98, 100.04, 100.94, 102.02, 102.92, 102.92, 100.94, 98.96, 96.98, 93.02, 91.94];
 var AtlantaData = [71.96, 71.06, 73.04, 71.96, 71.06, 71.06, 71.96, 73.04, 77, 80.96, 82.94, 86, 87.08, 77, 82.94, 73.94, 75.92, 84.02, 82.94, 80.96, 78.08, 75.92, 75.02, 75.02, 75.02, 73.04, 73.04, 71.96, 71.06, 69.98, 69.98, 73.04, 75.92, 78.98, 82.94, 84.92, 87.08, 86, 87.08, 82.94, 82.94, 82.94, 80.96, 78.98, 78.08, 73.94, 73.04, 71.96, 71.96, 69.08, 69.98, 66.92, 69.08, 66.92, 62.96, 66.02, 73.04, 75.92, 78.98, 80.06, 82.04, 84.02, 84.92, 87.08, 87.08, 87.08, 84.92, 84.02, 80.96, 77, 73.94, 73.04, 73.04, 71.06, 71.06, 68, 66.02, 66.92, 66.92, 71.06, 73.04, 77, 82.04, 84.02, 87.08, 84.92, 87.08, 89.96, 87.98, 87.98, 87.98, 84.92, 82.94, 78.08, 77, 73.94, 73.04, 71.96, 71.06, 69.08, 69.98, 69.08, 69.08, 69.98, 71.06, 75.92, 78.98, 82.94, 84.92, 87.98, 87.98, 84.92, 84.92, 77, 75.02, 80.06, 75.92, 73.04, 73.04, 73.04, 71.06, 71.06, 69.98, 71.06, 71.06, 69.98, 69.98, 71.06, 73.94, 77, 77, 77, 78.08, 80.06, 84.02, 84.92, 86, 86, 86, 84.02, 78.98, 75.92, 75.02, 71.06, 69.98, 69.98, 69.08, 69.08, 69.08, 69.08, 69.08, 69.98, 71.06, 71.96, 71.96, 73.04, 75.92, 77, 75.02, 75.92, 73.04, 75.92, 73.94, 73.94, 71.96, 71.96, 71.06, 69.98];
 var SeriesToBeDisplayed = [];
 var dummyProfile = [];

 // THIS NEEDS TO BE CHANGED!!!!!
 current_temperature = 70
 
 //this should eventually take in the number of hours from input.
//var sunHours = 12;
//var sunChart = null;
//var sunSlider = null;
//var sunData = null;

(function() {
	console.log("In bee_all.js");
  var Lab;
  var load;

  window.BEE = {
  	
    activeLoad: -1,
    VERSION: '2.0.beta',
    EMPTY_LOAD: {
      from: 0,
      to: 0
    },    
  };
  console.log("window.BEE called");

  Lab = (function() {
  	
  	console.log("Lab is called");
    
    function Lab(_length) {
    	console.log("Lab(_length) is called");
      this.length = _length;
      this._buildProfile();
    }

    Lab.prototype.length = Lab.length;

    Lab.prototype.toggleLoad = function(loadIndex, hour) {
    	console.log("Lab.prototype.toggleLoad is called");
      var hourWatts, toggleItAsInt;
      hourWatts = parseInt(this._loadMap[loadIndex]);
      toggleItAsInt = +(!this.profile[hour][loadIndex]);
      return this.profile[hour][loadIndex] = hourWatts * toggleItAsInt;
    };

    Lab.prototype.getSerieValues = function(serieInfo) {
    	console.log("Lab.prototype.getSerieValues is called");
      var hourInterval, loadIndex, time, _i, _len, _ref, _results;
      if (serieInfo['name']) {
      	console.log("In if block");
        loadIndex = this._loadMap[serieInfo['name']];
        console.log("After loadIndex " + loadIndex);
      } else {
        loadIndex = serieInfo['index'];
      }
      _ref = this.profile;
      _results = [];
      for (time = _i = 0, _len = _ref.length; _i < _len; time = ++_i) {
        hourInterval = _ref[time];
        _results.push(hourInterval[loadIndex]);
      }
      console.log("Before return " +_results);
      return _results;
    };

    Lab.prototype.getSeries = function() {
    	console.log("Lab.prototype.getSeries is called");
      return ['Load 1', 'Load 2', 'Load 3', 'Load 4'];
    };

    Lab.prototype._buildProfile = function() {
    	console.log("Lab.prototype._buildProfile is called");
      return this.profile = (function() {
        var _i, _ref, _results;
        _results = [];
        for (_i = 1, _ref = this.length; 1 <= _ref ? _i <= _ref : _i >= _ref; 1 <= _ref ? _i++ : _i--) {
          _results.push([0, 0, 0, 0]);
        }
        return _results;
      }).call(this);
    };

    Lab.prototype._loadMap = {
    	'Load 1': 2,
      'Load 2': 3,
      'Load 3': 4,
      'Load 4': 5,
      2: '100',
      3: '100',
      4: '100',
      5: '100'
    };
    console.log("Lab.prototype._loadMap is called");
    return Lab;

  })();

  window.app = {
  	
    setup: function() {
    	console.log("window.app is called");
    	console.log("setup: function() is called");
      var _ref;
      if ((_ref = window.labLength) == null) {
        window.labLength = 24;
      }
      
   
    
     //initialize the slider.  may not want 1500px size, need to test and see.
   sunSlider = new dhtmlxSlider("divSunSlider",180, "arrow", false, 0, 24-sunHours, 0, 1);
   sunSlider.setImagePath("scripts/dhtmlxSlider/codebase/imgs/");
   sunSlider.attachEvent("onChange", shiftSunGraph);
   sunSlider.init();

   
    sunData = new Array();
     for (var i = 0; i < sunHours; i++) {
        sunData.push(1);
    }
    for (var i = 0; i < 24-sunHours; i++) {
       sunData.push(0);
   }
    
      this.lab = new Lab(window.labLength);
      $("#chart-container").css("width", window.labLength * 42);
      this._drawGraph();
      console.log("Before this._setupListeners() is called");
      this._setupListeners();
      console.log("After this._setupListeners() is called");
      this._prepareLaunch();
      return this._moveGraphElements();
    },
    setCurrentLoad: function(event) {
      console.log("setCurrentLoad: is called");
      var loadIndex, myLabel;
      $('label.button.active').removeClass('active');
      loadIndex = parseInt($(this).val());
      myLabel = $(this).parent().find('label').addClass('active');
      return BEE.activeLoad = loadIndex;
    },
    removeCurrentLoad: function(event) {
      console.log("removeCurrentLoad: is called");
      BEE.activeLoad = -1;
      $('label.button.active').removeClass('active');
      $('.js-add-load').prop('checked', false);
      return event.preventDefault();
    },
    launchLab: function(event) {
      console.log("launchLab: is called");
      var launchString;
      launchString = window.launchPad.launch();
      if (typeof console !== "undefined" && console !== null) {
        console.log(launchString, "Launching lab!!!!");
      }
      $("#hdnProfile").val(launchString);
	  console.log("Hidden Values are " + $('#hdnProfile').val());
      return $("#btnGo").click();
    },
    
    hoursSelector: function(event) {
      if (typeof console !== "undefined" && console !== null) {
        console.log("Hours selection changed !!");
      }
	  var hours = $('#sun_hours_select :selected').val();
	    drawGraphWithProfile(selectedProfile);
	},		

    
    profileSelector: function(event) {
      if (typeof console !== "undefined" && console !== null) {
        console.log("Profile selection changed !!");
      }
	  var selectedProfile = $('#profile-list :selected').index();
	    drawGraphWithProfile(selectedProfile);
	},		

    _moveGraphElements: function() {
      console.log("_moveGraphElements: is called");
      var css, holder, svg;
      holder = $("#holder");
      if (holder.find(".highcharts-legend").length > 0) {
        svg = $('<svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="27"></svg>');
        css = {
          display: "block",
          width: "300px",
          margin: "10px auto"
        };
        $(".legend").html(svg.css(css).append($(".highcharts-legend")));
      }
      return this._moveLegend();
    },
    _moveLegend: function() {
      console.log("_moveLegend: is called");
      var css;
      css = {
        margin: '10px auto',
        width: '250px'
      }; 
      return $(".highcharts-legend").attr("transform", "").css(css);
    },
    _prepareLaunch: function() {
      console.log("_prepareLaunch: is called");
      if (window.launchPad != null) {
        window.launchPad = null;
      }
      return window.launchPad = new LaunchPad(this.lab);
    },
    
    _drawGraph: function() {
    	
      if (window.chart != null) {
        window.chart = null;
      }
      console.log("_drawGraph: is called before load");
      load = new LoadProfile(this.lab);
      console.log("_drawGraph: is called after load");
      return window.chart = LoadProfile;
    }, 
    
    _setupListeners: function() {
    	console.log("_setupListeners: is called");
      var _this = this;
      $(window).resize(function() {
        return _this._moveGraphElements();
      });
      $('body').on('change', '#profile-list', this.profileSelector);
       $('body').on('change', '#sun_hours_select', this.changeHours);
      $('body').on('change', '.js-add-load', this.setCurrentLoad);
      $('body').on('click', 'label.button.active', this.removeCurrentLoad);
      return $('body').on('click', '#js-launch-experiment', this.launchLab);
    }
  };

  jQuery(function($) {
  	console.log("jQuery(function($) is called");
    return app.setup();
  });

}).call(this);

setUpTimeAlert = function (profile) {
  if (profile === null || profile.length === 0) {
    return
  }
  hours_it_takes = Math.abs(current_temperature-SeriesToBeDisplayed)/5
  minutes_it_takes = Math.round((Math.abs(current_temperature-SeriesToBeDisplayed)/5.0 - hours_it_takes)*60)
  if (minutes_it_takes === 0) {
    alert("The current temperature is "+current_temperature+", so it will take approximately "+hours_it_takes+" hours to start the lab.");
  }
  else {
    alert("The current temperature is "+current_temperature+", so it will take approximately "+hours_it_takes+" hours and "+minutes_it_takes+" minutes to start the lab.");
  }
}

function drawGraphWithProfile(profileSelected) {
  
  console.log("_drawGraphWithProfile: is called");
  if (window.chart != null) {
  	window.chart = null;
  }
  
  if(profileSelected == 0){
    SeriesToBeDisplayed = dummyProfile;
  }  
  else if(profileSelected == 1){
    SeriesToBeDisplayed = PhoenixData;
  }
  else if(profileSelected == 2){
   	SeriesToBeDisplayed = AtlantaData;
  }
  else{
    alert('Profile data is not set')
    SeriesToBeDisplayed = dummyProfile;
  }
  setUpTimeAlert(SeriesToBeDisplayed);
  redrawChart();
}


function redrawChart() {
  
  if (chart1.series.length < 7) {  
    chart1.addSeries({
        data: SeriesToBeDisplayed,
        name: 'Temperature',
        color: '#89A54E',
        type: 'spline',
        yAxis: 1,
   });
   }
   else{
        chart1.series[6].setData(SeriesToBeDisplayed);
    }
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
   sunChart.series[0].setData(newData);
   return null;
}

$(function() {
   $("#sun_hours_select").change(function () {
       sunHours = parseInt($("#sun_hours_select").val());
       shiftSunGraph(0, null);
       sunSlider.setMax(24-sunHours);
   })
});


// //initialize the slider.  may not want 1500px size, need to test and see.
// sld = new dhtmlxSlider("slider", 1500, "arrow", false, 0, 24-sunHours, 0, 1);
// sld.setImagePath("dhtmlxSlider/codebase/imgs/");
// sld.attachEvent("onChange", shiftSunGraph);
// sld.init();



 // create initial data with lamp starting at hour 0.
var colors = Highcharts.getOptions().colors;

sunChart = new Highcharts.Chart({
   chart: {
       renderTo: 'sun-container',
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
sunChart.series[0].setData(sunData);
  



