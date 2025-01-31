window.LoadProfile = class LoadProfile

  updateChart: (seriesIndex) ->
    newData = @lab.getSerieValues('index': seriesIndex)
    @chart.series[seriesIndex].setData(newData, true)

  addToCurrentLoad: (xPosition) ->
    xPosition = Math.round(xPosition)
    xPosition = 0  if xPosition < 0
    xPosition = @lab.lenght - 1 if xPosition >= @lab.lenght
    currentLoad = BEE.activeLoad
    if currentLoad >= 0
      @lab.toggleLoad(currentLoad, xPosition)
      @updateChart(currentLoad)
    else
      $(".navigation").stop().effect("bounce", { times: 2 }, 200)


  buildGraph: ->
    @chart = new Highcharts.Chart
      chart:
        showAxes: true
        renderTo: 'chart-container'
        type: 'column'
        events:
          click: (event) =>
            @addToCurrentLoad(event.xAxis[0].value)
            false
      title:
        text: '  '
      tooltip:
        enabled: false
      xAxis:
        min: 0
        max: @lab.length - 1
        padding: 0
        title:
          text: 'Test Chamber Hours'
        categories: (=> Highcharts.dateFormat("%e %H:%M", Date.UTC(2013, 1, (hour / 24) + 1, hour % 24)) for hour in [0..@lab.length])()
      yAxis:
        max: 500
        title:
          text: 'Total Watts'
        stackedLabels:
          enabled: true
      plotOptions:
        series:
          cursor: 'pointer'
          point:
            events:
              click: (event) =>
                @addToCurrentLoad(event.point.x)
                event.preventDefault()
                false
        column:
          borderWidth: 2
          pointPadding: 0
          groupPadding: 0
          stacking: 'normal'
          dataLabels:
            enabled: false
      series: @_getSeries()


  _getSeries: ->
    _series = (
      {
        name: serie
        data: @lab.getSerieValues('name': serie)
      } for serie in @lab.getSeries()
    )

  constructor: (lab) ->
    @lab = lab
    @buildGraph()