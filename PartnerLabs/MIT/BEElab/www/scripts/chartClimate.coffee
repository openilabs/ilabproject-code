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
      yAxis:
        max: 50
        title:
          text: 'Temperature'
          style:
            color: '#89A54E'
        opposite: true
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
      series: [
        name: 'hidden'
        data: []
        stack: 0
        yAxis: 0
        ,
        name: 'sunlamp'
        data: []
        stack: 0
        yAxis: 0
        ,
        name: 'Load 1'
        data: []
        yAxis: 0
        stack: 1
        events:
          click: (event) =>
            @addToCurrentLoad(event.xAxis[0].value)
            false
        ,
        name: 'Load 2'
        data: []
        yAxis: 0
        stack: 1
        events:
          click: (event) =>
            @addToCurrentLoad(event.xAxis[0].value)
            false
        ,
        name: 'Load 3'
        data: []
        yAxis: 0
        stack: 1
        events:
          click: (event) =>
            @addToCurrentLoad(event.xAxis[0].value)
            false
        ,
        name: 'Load 4'
        data: []
        yAxis: 0
        stack: 1
        events:
          click: (event) =>
            @addToCurrentLoad(event.xAxis[0].value)
            false
        ,
        name: 'Temperature'
        color: '#89A54E'
        type: 'spline'
        yAxis: 1
        data: []
        ]



  _getSeries: ->
    _series = (
      {
        name: serie
        data: @lab.getSerieValues('name': serie)
      } for serie in @lab.getSeries()
    )

  $ ->
  sunHours = 10
  sld = null
  sunChart = null
  sunData = new Array()
  i = 0

  while i < sunHours
    sunData.push 1
    i++
  i = 0

  while i < 24 - sunHours
    sunData.push 0
    i++

  shiftSunGraph = (sliderValue, sliderObj) ->
    newData = new Array()
    i = 0

    while i < sliderValue
      newData.push 0
      i++
    i = 0

    while i < sunHours
      newData.push 1
      i++
    i = 0

    while i < 24 - sunHours - sliderValue
      newData.push 0
      i++
    sunData = newData
    
    #this updates the chart with the new start position of the sun
    #chart.series[0].update({data: sunData}, true);
    @Chart.series[1].setData newData
    null

    $("#sun_hours_select").change ->
      sunHours = parseInt($("#sun_hours_select").val())
      shiftSunGraph 0, null
      sld.setMax 24 - sunHours
      return

    return

  
  #initialize the slider.  may not want 1500px size, need to test and see.
  sld = new dhtmlxSlider("sunslider-container", 1200, "arrow", false, 0, 24 - sunHours, 0, 1)
  sld.setImagePath "scripts/dhtmlxSlider/codebase/imgs/"
  sld.attachEvent "onChange", shiftSunGraph
  sld.init()
  
  
  constructor: (lab) ->
    @lab = lab
    @buildGraph()