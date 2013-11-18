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
            text: 'Total Wattz'
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
