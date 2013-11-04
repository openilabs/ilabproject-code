 //this should eventually take in the number of hours from input.
 var sunHours = 8;
 skldajgsklajgklasjdglkjasgj
 // create initial data with lamp starting at hour 0.
 var sunData = new Array();
 for (int i = 0; i < sunHours; i++) {
    sunData.push(1);
 }
 for (int i = 0; i < 24-sunHours; i++) {
    sunData.push(0);
 }
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
            enabled: false
        }
    },
    scrollbar: {
        enabled: true
    },
    yAxis: {
        gridLineWidth: 0,
        minorGridLineWidth: 0,
        lineColor: 'transparent',        
        labels: {
            enabled: false
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
        align: 'center',
        //x: -100,
        verticalAlign: 'top',
        //y: 20,
        floating: true,
        backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColorSolid) || 'white',
        borderColor: '#CCC',
        borderWidth: 1,
        shadow: false
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
            pointPadding: 0,
            pointRange : 2.5,
            borderWidth: 0,
            pointPlacement: 'between'
        }
    },
    
    series: [{
        data: sunData
        }]
    
    
});