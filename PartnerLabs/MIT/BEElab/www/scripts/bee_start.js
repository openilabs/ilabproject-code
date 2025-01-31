﻿(function() {
  var Lab;

  window.BEE = {
    activeLoad: -1,
    VERSION: '2.0.beta',
    EMPTY_LOAD: {
      from: 0,
      to: 0
    }
  };

  Lab = (function() {

    function Lab(_length) {
      this.length = _length;
      this._buildProfile();
    }

    Lab.prototype.length = Lab.length;

    Lab.prototype.toggleLoad = function(loadIndex, hour) {
      var hourWatts, toggleItAsInt;
      hourWatts = parseInt(this._loadMap[loadIndex]);
      toggleItAsInt = +(!this.profile[hour][loadIndex]);
      return this.profile[hour][loadIndex] = hourWatts * toggleItAsInt;
    };

    Lab.prototype.getSerieValues = function(serieInfo) {
      var hourInterval, loadIndex, time, _i, _len, _ref, _results;
      if (serieInfo['name']) {
        loadIndex = this._loadMap[serieInfo['name']];
      } else {
        loadIndex = serieInfo['index'];
      }
      _ref = this.profile;
      _results = [];
      for (time = _i = 0, _len = _ref.length; _i < _len; time = ++_i) {
        hourInterval = _ref[time];
        _results.push(hourInterval[loadIndex]);
      }
      return _results;
    };

    Lab.prototype.getSeries = function() {
      return ['Load 1', 'Load 2', 'Load 3', 'Load 4'];
    };

    Lab.prototype._buildProfile = function() {
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
      'Load 1': 0,
      'Load 2': 1,
      'Load 3': 2,
      'Load 4': 3,
      0: '100',
      1: '100',
      2: '100',
      3: '100'
    };

    return Lab;

  })();

  window.app = {
    setup: function() {
      var _ref;
      if ((_ref = window.labLength) == null) {
        window.labLength = 24;
      }
      this.lab = new Lab(window.labLength);
      $("#chart-container").css("width", window.labLength * 42);
      this._drawGraph();
      this._setupListeners();
      this._prepareLaunch();
      return this._moveGraphElements();
    },
    setCurrentLoad: function(event) {
      var loadIndex, myLabel;
      $('label.button.active').removeClass('active');
      loadIndex = parseInt($(this).val());
      myLabel = $(this).parent().find('label').addClass('active');
      return BEE.activeLoad = loadIndex;
    },
    removeCurrentLoad: function(event) {
      BEE.activeLoad = -1;
      $('label.button.active').removeClass('active');
      $('.js-add-load').prop('checked', false);
      return event.preventDefault();
    },
    launchLab: function(event) {
      var launchString;
      launchString = window.launchPad.launch();
      if (typeof console !== "undefined" && console !== null) {
        console.log(launchString, "Launching lab!");
      }
      $("#hdnProfile").val(launchString);
      return $("#btnGo").click();
    },
    _moveGraphElements: function() {
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
      var css;
      css = {
        margin: '10px auto',
        width: '250px'
      };
      return $(".highcharts-legend").attr("transform", "").css(css);
    },
    _prepareLaunch: function() {
      if (window.launchPad != null) {
        window.launchPad = null;
      }
      return window.launchPad = new LaunchPad(this.lab);
    },
    _drawGraph: function() {
      if (window.chart != null) {
        window.chart = null;
      }
      return window.chart = new LoadProfile(this.lab);
    },
    _setupListeners: function() {
      var _this = this;
      $(window).resize(function() {
        return _this._moveGraphElements();
      });
      $('body').on('change', '.js-add-load', this.setCurrentLoad);
      $('body').on('click', 'label.button.active', this.removeCurrentLoad);
      return $('body').on('click', '#js-launch-experiment', this.launchLab);
    }
  };

  jQuery(function($) {
    return app.setup();
  });

}).call(this);
