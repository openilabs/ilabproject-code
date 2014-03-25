window.BEE =
  activeLoad: 2
  VERSION: '2.0.beta'
  EMPTY_LOAD:
    from: 0
    to: 0

# Lab class holds the main logic for the experiment. It is the controller of
# everything.
class Lab
  constructor: (_length) ->
    @length  = _length
    @_buildProfile()

  length: @length

  PhoenixData = [91.04, 86, 84.02, 82.04, 80.96, 80.96, 80.06, 84.92, 89.96, 91.94, 95, 100.04, 102.02, 102.02, 105.08, 107.06, 107.96, 107.06, 107.06, 105.98, 102.92, 100.04, 98.06, 96.98, 93.92, 91.94, 89.06, 87.98, 84.92, 82.94, 84.02, 84.92, 89.96, 91.94, 96.08, 98.06, 100.04, 102.02, 104, 105.98, 105.98, 107.96, 107.06, 105.08, 102.02, 95, 93.02, 89.96, 89.96, 89.06, 84.92, 84.92, 86, 86, 84.02, 86, 89.06, 91.94, 93.92, 96.98, 100.04, 100.04, 100.94, 102.92, 104, 102.92, 104, 102.02, 100.04, 96.98, 96.08, 96.08, 93.92, 89.06, 89.06, 87.98, 87.08, 86, 82.04, 82.94, 86, 86, 87.98, 95, 98.96, 102.02, 102.02, 105.08, 105.98, 105.98, 105.98, 104, 102.02, 100.94, 98.96, 93.92, 91.04, 87.08, 86, 84.02, 82.04, 80.96, 84.02, 84.02, 89.06, 93.02, 96.08, 98.96, 104, 107.06, 111.02, 111.92, 111.92, 113, 111.92, 109.04, 100.04, 96.98, 98.06, 96.08, 89.06, 86, 84.02, 84.02, 82.94, 82.04, 82.04, 84.02, 87.08, 89.96, 93.02, 96.98, 98.96, 102.02, 102.92, 104, 105.98, 104, 104, 102.02, 98.96, 96.98, 95, 93.02, 91.04, 89.96, 87.08, 86, 82.04, 80.96, 82.04, 84.92, 89.06, 89.96, 91.94, 93.92, 95, 96.98, 100.04, 100.94, 102.02, 102.92, 102.92, 100.94, 98.96, 96.98, 93.02, 91.94]
  AtlantaData = [71.96, 71.06, 73.04, 71.96, 71.06, 71.06, 71.96, 73.04, 77, 80.96, 82.94, 86, 87.08, 77, 82.94, 73.94, 75.92, 84.02, 82.94, 80.96, 78.08, 75.92, 75.02, 75.02, 75.02, 73.04, 73.04, 71.96, 71.06, 69.98, 69.98, 73.04, 75.92, 78.98, 82.94, 84.92, 87.08, 86, 87.08, 82.94, 82.94, 82.94, 80.96, 78.98, 78.08, 73.94, 73.04, 71.96, 71.96, 69.08, 69.98, 66.92, 69.08, 66.92, 62.96, 66.02, 73.04, 75.92, 78.98, 80.06, 82.04, 84.02, 84.92, 87.08, 87.08, 87.08, 84.92, 84.02, 80.96, 77, 73.94, 73.04, 73.04, 71.06, 71.06, 68, 66.02, 66.92, 66.92, 71.06, 73.04, 77, 82.04, 84.02, 87.08, 84.92, 87.08, 89.96, 87.98, 87.98, 87.98, 84.92, 82.94, 78.08, 77, 73.94, 73.04, 71.96, 71.06, 69.08, 69.98, 69.08, 69.08, 69.98, 71.06, 75.92, 78.98, 82.94, 84.92, 87.98, 87.98, 84.92, 84.92, 77, 75.02, 80.06, 75.92, 73.04, 73.04, 73.04, 71.06, 71.06, 69.98, 71.06, 71.06, 69.98, 69.98, 71.06, 73.94, 77, 77, 77, 78.08, 80.06, 84.02, 84.92, 86, 86, 86, 84.02, 78.98, 75.92, 75.02, 71.06, 69.98, 69.98, 69.08, 69.08, 69.08, 69.08, 69.08, 69.98, 71.06, 71.96, 71.96, 73.04, 75.92, 77, 75.02, 75.92, 73.04, 75.92, 73.94, 73.94, 71.96, 71.96, 71.06, 69.98]

  SeriesToBeDisplayed = [0]
  dummyProfile = [0]

# THIS NEEDS TO BE CHANGED!!!!!
  current_temperature = 70
 
 

  # Turns a load on/off at a specific hour
  #
  # It will not update the chart
  toggleLoad: (loadIndex, hour) ->
    hourWatts = parseInt @_loadMap[loadIndex]
    toggleItAsInt = + !@profile[hour][loadIndex] # returns 1 or 0 as integers
    @profile[hour][loadIndex] = hourWatts * toggleItAsInt


  # Given object with either an index or a name, this method returns the
  # profile for that specific series.
  #
  # Returns the array of data.
  #
  # Examples:
  #
  # getSeriesValues({'name': '100 Watts'})
  #   => returns an array with the values for the 100 Watts series
  #
  # getSeriesValues({'index': 0})
  #   => returns an array with the values for the serie at index 0
  getSerieValues: (serieInfo) ->
    if serieInfo['name']
      loadIndex = @_loadMap[serieInfo['name']]
    else
      loadIndex = serieInfo['index']
    hourInterval[loadIndex] for hourInterval, time in @profile

  # Returns an array with the loads of each serie
  getSeries: -> ['Load 1', 'Load 2', 'Load 3', 'Load 4']


  # Private Functions ---------------------------------------------------------

  # Private Function
  # Builds a `@maxLength` * matrix that will hold what loads are on
  # at a specific time.
  #
  # The @profile[i] represents the loads that are turned on at time i
  #
  # Returns the matrix
  _buildProfile: -> @profile = ([0,0,0,0,0,0,0] for [1..@length])

  # Private object
  # Map that holds the labels and indexes for each load
  # The index is the one in the `@profile` matrix
  _loadMap:
    'hidden'  : 0
    'sunlamp' : 1
    'Load 1'  : 2
    'Load 2'  : 3
    'Load 3'  : 4
    'Load 4'  : 5
    0         : '450'
    1         : '50'
    2         : '100'
    3         : '100'
    4         : '100'
    5         : '1'


# End of Lab Class ------------------------------------------------------------


window.app =
  setup: ->

    # this should eventually take in the number of hours from input.
    sunHours = 12
    sunChart = null
    sunSlider = null
    sunData = null
    window.labLength ?= 72
    @lab = new Lab(window.labLength) # Max lab lenght
    $("#chart-container").css("width", window.labLength * 42)
    @_setupSunSlider()
    @_drawGraph()
    @_setupListeners()
    @_prepareLaunch()
    @_moveGraphElements()

  # jQuery Listener
  # This will set the `window.currentLoad` to the one pointed by the radio
  # button that was just clicked.
  #
  # This will also toggle the label to an 'active' state
  setCurrentLoad: (event) ->
    $('label.button.active').removeClass('active')
    loadIndex = parseInt($(this).val())
    myLabel = $(this).parent().find('label').addClass('active')
    BEE.activeLoad = loadIndex

  # jQuery Listener
  # This will set the BEE.activeLoad to `-1` and also will remove any active
  # state from any label.
  removeCurrentLoad: (event) ->
    BEE.activeLoad = -1
    $('label.button.active').removeClass('active')
    $('.js-add-load').prop('checked', false)
    event.preventDefault()

  # jQuery Listener
  # Launches the experiment
  # TODO: Test this on the server
  launchLab: (event) ->
    launchString = window.launchPad.launch()
    console?.log(launchString, "Launching lab!")
    $("#hdnProfile").val(launchString)
    $("#btnGo").click() # Use the C# button

  _moveGraphElements: ->
    holder = $("#holder")
    if holder.find(".highcharts-legend").length > 0
      svg = $('<svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="27"></svg>')
      css = {
        display: "block"
        width: "300px"
        margin: "10px auto"
      }
      $(".legend").html svg.css(css).append $(".highcharts-legend")
    @_moveLegend()

  _moveLegend: ->
    css = {
      margin: '10px auto'
      width: '250px'
    }
    $(".highcharts-legend").attr("transform", "").css(css)

  _prepareLaunch: ->
    window.launchPad = null if window.launchPad?
    window.launchPad = new LaunchPad(@lab)

  _drawGraph: ->
    window.chart = null if window.chart?
    window.chart = new LoadProfile(@lab)

  _setupListeners: ->
    $(window).resize( => @_moveGraphElements() )
    $('body').on 'change', '.js-add-load',        @setCurrentLoad
    $('body').on 'click',  'label.button.active', @removeCurrentLoad
    $('body').on 'change', '#sun_hours_select', @changeHours
    $('body').on 'change', '#profile-list', @profileSelector
    $('body').on 'click', '#js-launch-experiment', @launchLab

  profileSelector: (event) ->
    if (typeof console != "undefined" && console != null)
      console.log("Profile selection changed !!")
    selectedProfile = $('#profile-list :selected').index()
    drawGraphWithProfile(selectedProfile)

  # initialize the slider.  may not want 1500px size, need to test and see.
  _setupSunSlider: ->
    sunSlider = new dhtmlxSlider("divSunSlider",180, "arrow", false, 0, 24 - sunHours, 0, 1)
    sunSlider.setImagePath("scripts/dhtmlxSlider/codebase/imgs/")
    sunSlider.attachEvent("onChange", shiftSunGraph)
    sunSlider.init()
  
  hoursSelector: (event) ->
    if (typeof console != "undefined" && console != null)
      console.log("Hours selection changed !!")
    hours = $('#sun_hours_select :selected').val()
    drawGraphWithProfile(selectedProfile)

  shiftSunGraph: (sliderValue, sliderObj) ->
    newData = new Array()
    i = 0
    while i < sliderValue
      newData.push 1
      i++
    i = 0
    while i < 24 - sunHours - sliderValue - sunHours
      newData.push 1
      i++
    i = 0
    while i < sunHours
      newData.push 1
      i++
    
    #sunData = newData
    #this updates the chart with the new start position of the sun
    @chart.series[1].update({data: newData}, true)
    #window.chart.series['sunlamp'].setData(newData)
   
  sunHoursChange: (event) ->
    sunHours = parseInt($("#sun_hours_select").val())
    shiftSunGraph(0, null)
    sunSlider.setMax(24-sunHours)
  

jQuery ($) ->
  app.setup()