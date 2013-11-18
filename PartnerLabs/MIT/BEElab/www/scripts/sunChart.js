$(function () {
var sunHours = 10;
var sld = null;
var sunChart = null;

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
    sunChart.series[0].setData(newData);
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
 sld = new dhtmlxSlider("sunslider-container", 1200, "arrow", false, 0, 24-sunHours, 0, 1);
 sld.setImagePath("scripts/dhtmlxSlider/codebase/imgs/");
 sld.attachEvent("onChange", shiftSunGraph);
 sld.init();



 // create initial data with lamp starting at hour 0.
 var colors = Highcharts.getOptions().colors;

 sunChart = new Highcharts.Chart({
    chart: {
        renderTo: 'sun-container',
        type: 'column',
        height: 200

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
});