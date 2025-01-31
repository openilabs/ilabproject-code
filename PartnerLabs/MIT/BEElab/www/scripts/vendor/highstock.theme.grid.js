/**
 * Grid theme for Highcharts JS
 * @author Torstein Hønsi
 */

Highcharts.theme = {
  colors: ['#A84B3A', '#FF9F67', '#323138', '#4C646B', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4'],
  chart: {
    backgroundColor: {
      linearGradient: { x1: 0, y1: 0, x2: 1, y2: 1 },
      stops: [
        [0, 'rgb(255, 255, 255)'],
        [1, 'rgb(240, 240, 255)']
      ]
    },
    borderWidth: 1,
    plotBackgroundColor: 'rgba(255, 255, 255, .9)',
    plotShadow: false,
    plotBorderWidth: 1
  },
  title: {
    style: {
      color: '#000',
      font: 'bold 16px "Trebuchet MS", Verdana, sans-serif'
    }
  },
  subtitle: {
    style: {
      color: '#666666',
      font: 'bold 12px "Trebuchet MS", Verdana, sans-serif'
    }
  },
  xAxis: {
    gridLineWidth: 1,
    tickInterval: 1,
    minorTickInterval: 0,
    lineColor: '#000',
    tickColor: '#000',
    labels: {
      style: {
        color: '#000',
        font: '11px Trebuchet MS, Verdana, sans-serif'
      }
    },
    title: {
      style: {
        color: '#333',
        fontWeight: 'bold',
        fontSize: '12px',
        fontFamily: 'Trebuchet MS, Verdana, sans-serif'

      }
    }
  },
  yAxis: {
    minorTickInterval: 'auto',
    lineColor: '#000',
    lineWidth: 0,
    tickWidth: 1,
    tickColor: '#000',
    labels: {
      style: {
        color: '#000',
        font: '11px Trebuchet MS, Verdana, sans-serif'
      }
    },
    title: {
      style: {
        color: '#333',
        fontWeight: 'bold',
        fontSize: '12px',
        fontFamily: 'Trebuchet MS, Verdana, sans-serif'
      }
    }
  },
  legend: {
    itemStyle: {
      font: '9pt Trebuchet MS, Verdana, sans-serif',
      color: 'black'

    },
    itemHoverStyle: {
      color: '#039'
    },
    itemHiddenStyle: {
      color: 'gray'
    }
  },
  labels: {
    style: {
      color: '#99b'
    }
  }
};

// Apply the theme
var highchartsOptions = Highcharts.setOptions(Highcharts.theme);
