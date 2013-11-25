var chart1;
(function() {
  var LoadProfile;
      
  window.LoadProfile = LoadProfile = (function() {
    console.log("window.LoadProfile called");
    LoadProfile.prototype.updateChart = function(seriesIndex) {
    	console.log("LoadProfile.prototype.updateChart called");
      var newData;
      newData = this.lab.getSerieValues({
        'index': seriesIndex
      });
      return chart1.series[seriesIndex].setData(newData, true);
    };

    var copyArray = function (array, x) {
      newArray = new Array();
      for (var i = 0; i < x; i++){
        newArray = newArray.concat(array);
      }
      return newArray;
    }

    var sunHours = 10;
    var sld = null;
    var sunChart = null;

    sunData = new Array();
     for (var i = 0; i < sunHours; i++) {
        sunData.push(100);
     }
     for (var i = 0; i < 24-sunHours; i++) {
        sunData.push(0);
     }
     sunData = copyArray(sunData, this.lab.length);

    var shiftSunGraph = function (sliderValue, sliderObj) {
        newData = new Array();
        for (var i = 0; i < sliderValue; i++) {
            newData.push(0);
        }
        for (var i = 0; i < sunHours; i++) {
            newData.push(100);
        }
        for (var i = 0; i < 24-sunHours-sliderValue; i++) {
            newData.push(0);
        }
        sunData = newData;
        //this updates the chart with the new start position of the sun
        //chart.series[0].update({data: sunData}, true);
        for (var i=0; i < chart1.series.length; i++){
          if (chart1.series[i].name === 'sunChart') {
            chart1.series[i].setData(copyArray(newData, this.lab.lenght/24));
          }
        }
        sunChart.series[0].setData(newData);
        return null;
     }
     var generateInvisibleSeries = function () {
      var invisSeries = new Array();
      for (var i=0; i < this.lab.lenght) {
        invisSeries.push(400);
      }
      return invisSeries;
     }

    $(function() {
        $("#sun_hours_select").change(function () {
            sunHours = parseInt($("#sun_hours_select").val());
            shiftSunGraph(0, null);
            sld.setMax(24-sunHours);
        })
    });

     //initialize the slider.  may not want 1500px size, need to test and see.
     sld = new dhtmlxSlider("sunslider-container", 1200, "arrow", false, 0, 24-sunHours, 0, 1);
     sld.setImagePath("scripts/dhtmlxSlider/codebase/imgs/");
     sld.attachEvent("onChange", shiftSunGraph);
     sld.init();

    LoadProfile.prototype.addToCurrentLoad = function(xPosition) {
    	console.log("LoadProfile.prototype.addToCurrentLoad called");
      var currentLoad;
      xPosition = Math.round(xPosition);
      if (xPosition < 0) {
        xPosition = 0;
      }
      if (xPosition >= this.lab.lenght) {
        xPosition = this.lab.lenght - 1;
      }
      currentLoad = BEE.activeLoad;
      if (currentLoad >= 0) {
        this.lab.toggleLoad(currentLoad, xPosition);
        return this.updateChart(currentLoad);
      } else {
        return $(".navigation").stop().effect("bounce", {
          times: 2
        }, 200);
      }
    };
    
    

    LoadProfile.prototype.buildGraph = function() {
    	console.log("LoadProfile.prototype.buildGraph called");
      var _this = this;
        chart1  = new Highcharts.Chart({
        
        chart: {
          showAxes: true,
          renderTo: 'chart-container',
          type: 'column',
          events: {
            click: function(event) {
              _this.addToCurrentLoad(event.xAxis[0].value);
              return false;
            }
          }
        },
        
        rangeSelector:{
    			enabled:false
				},
        
        title: {
          text: '  '
        },
        tooltip: {
          enabled: false
        },
        xAxis: {
          min: 0,
          max: this.lab.length - 1,
          padding: 0,
          title: {
            text: 'Test Chamber Hours'
          },
          categories: (function() {
            var hour, _i, _ref, _results;
            _results = [];
            for (hour = _i = 0, _ref = _this.lab.length; 0 <= _ref ? _i <= _ref : _i >= _ref; hour = 0 <= _ref ? ++_i : --_i) {
              _results.push(Highcharts.dateFormat("%e %H:%M", Date.UTC(2013, 1, (hour / 24) + 1, hour % 24)));
            }
            return _results;
          })()
        },
        
        scrollbar: {
            enabled: true
        },
        
        yAxis: [{
          max: 500,
          title: {
            text: 'Total Watts'
          },
          tickInterval: 100,
          showLastLabel: true,
          stackedLabels: {
            enabled: true
          }
        },
        { // Primary yAxis
          labels: {
            formatter: function() {
              return this.value +'°F';
            },
            style: {
              color: '#89A54E'
            }
          },
          title: {
              text: 'Temperature',
              style: {
                color: '#89A54E'
              },
              tooltip: {
          			enabled: true
        			}
          },
          opposite: true
    
        }        
        
        ],
        tooltip: {
          shared: true
        },        

        plotOptions: {
          series: {
            cursor: 'pointer',
            point: {
              events: {
                click: function(event) {
                  _this.addToCurrentLoad(event.point.x);
                  event.preventDefault();
                  return false;
                }
              }
            }
          },
          column: {
            borderWidth: 2,
            pointPadding: 0,
            groupPadding: 0,
            stacking: 'normal',
            dataLabels: {
              enabled: false
            }
          }
        },
        series: this._getSeries()
      });
    };    


    LoadProfile.prototype._getSeries = function() {
    	console.log("LoadProfile.prototype._getSeries called");
      var serie, _series;
      return _series = (function() {
        var _i, _len, _ref, _results;
        console.log("Before this.lab.getSeries() called");
        _ref = this.lab.getSeries();
        
        _results = [];
        for (_i = 0, _len = _ref.length; _i < _len; _i++) {
          serie = _ref[_i];
          _results.push({
            name: serie,
            data: this.lab.getSerieValues({
              'name': serie
            })
          });
          
        }
        // invisible series to raise sun series to top
        _results.push({
          name: 'invisibleSeries',
          data: generateInvisibleSeries(),
          cursor: null,
          visible: false,
          point: {
            events: {
              click: function(event) {
                return null;
              }
            }
          },
          column: {
            borderWidth: 2,
            pointPadding: 0,
            groupPadding: 0,
            stacking: 'sunStack',
            dataLabels: {
              enabled: false
            }
          }
        })
        // sunchart series
        _results.push({
          name: 'sunchart',
          data: copyArray(sunData, this.lab.lenght/24),
          cursor: null,
          point: {
            events: {
              click: function(event) {
                return null;
              }
            }
          },
          column: {
            borderWidth: 2,
            pointPadding: 0,
            groupPadding: 0,
            stacking: 'sunStack',
            dataLabels: {
              enabled: false
            }
          }
        })
        console.log("In for loop  after _results.push");
        return _results;
      }).call(this);
    };   

    	
    
    function LoadProfile(lab) {
    	console.log("LoadProfile method " );
      this.lab = lab;
      this.buildGraph();
      console.log("this.buildGraph() called " );
    }
    
    

    return LoadProfile;

  })();

}).call(this);
