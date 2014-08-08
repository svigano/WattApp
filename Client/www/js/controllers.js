angular.module('wattapp.controllers', [])

    .controller('MetersIndexCtrl', function ($scope, MetersService, SettingsService) {

        $scope.searchKey = "";

        $scope.clearSearch = function () {
            $scope.searchKey = "";
            findAllmeters();
        }

        $scope.search = function () {
            // TO DO
        }

        var findAllmeters = function() {
            var customerGuid = SettingsService.getSelectedCustomer();
            MetersService.findAll(customerGuid).then(function(payload){
                $scope.meters = payload;
            });
            console.log("findAllmeters " + $scope.meters)
        }

        findAllmeters();

    })

    .controller('MetersDetailCtrl', function ($scope, $stateParams, MetersService, SettingsService) {
        var customerGuid = SettingsService.getSelectedCustomer();
        MetersService.findById(customerGuid, $stateParams.meterId).then(function(payload) { 
            console.log(payload);    
            $scope.meter = payload;
        });
    })

    .controller('MetersReportsConsumptionCtrl', function ($scope, $stateParams, MeterHistoryService, SettingsService) {
            var customerGuid = SettingsService.getSelectedCustomer();

            $scope.chartSettings = {

                dataSource: {
                    load: function(){
                        return MeterHistoryService.getLastWeekConsumption(customerGuid, $stateParams.meterId);
                    }
                },
                margin:{right:20},
                legend: {
                    visible:false,
                    horizontalAlignment:"center"
                },
                series: {
                  argumentField: "t",
                  valueField: "val",
                  type: "bar",
                  color: '#ffa500'
                }
            }
    })

    .controller('MetersReportsCtrl', function ($scope, $http, $q, $stateParams, getDemandTodayVsYesterdaySync, MeterHistoryService, SettingsService) {

            var calculateMinMaxChartScale = function(data){
                var minvaluePair = _.min(data, function(d){return Math.min(d.val1, d.val2)});
                var maxvaluePair =  _.max(data, function(d){return Math.max(d.val1, d.val2)});
                var min = Math.min(minvaluePair.val1, minvaluePair.val2);
                var max = Math.max(maxvaluePair.val1, maxvaluePair.val2);
                min = ((min - 300 < 0) ? 0 : min - 300)
                return {'min':min, 'max':max};
            }

            var data = getDemandTodayVsYesterdaySync;
            console.log(data[23])
            for (index = 0; index < data.length; ++index){
                console.log(data[index].t)
            }

            var minMax = calculateMinMaxChartScale(data);
            console.log(minMax)
            $scope.chartSettings = {
                dataSource: data,
                legend: {
                    visible:true,
                    horizontalAlignment:"center"
                },
                size: { height: 400 },
                margin:{right:15},
                palette: ['#ffae00', '#ff7700', '#fa6a63'],
                commonSeriesSettings: {
                    argumentField: 't',
                    opacity: 0.4,
                    type: 'splinearea',
                },
                series: [
                    { valueField: 'val1', name: 'today'},
                    { valueField: 'val2', name: 'yesterday'}
                    ],
                argumentAxis: { 
                    valueMarginsEnabled: false,
                    label: {format:'shortTime'},
                    tickInterval: { hours: 3 },
                    //setTicksAtUnitBeginning: true
                },
                valueAxis:{ 
                            min: minMax.min,
                            max: minMax.max,
                          }
            }
    })

    .controller('MetersReportsWeatherCtrl', function ($scope, $http, $q, $stateParams, MeterHistoryService, SettingsService) {
            var customerGuid = SettingsService.getSelectedCustomer();
            var data = [];
                $scope.chartSettings = {

                    dataSource: {
                        load: function(){
                            return MeterHistoryService.getTodayWeather(customerGuid, $stateParams.meterId);
                        }
                    },
                    legend: {
                        visible:true,
                        horizontalAlignment:"center"
                    },
                    scale: {
                        minorTickInterval: "hour",
                        placeholderHeight: 20,
                        format: "hour"
                    },
                    //size: { height: 400 },
                    margin:{right:5},
                    palette: ['#ffae00', '#ff7700', '#fa6a63'],
                    commonSeriesSettings: {
                        argumentField: 't',
                        opacity: 0.4,
                        type: 'splinearea',
                    },
                    series: [
                        { valueField: 'val1', name: 'Demand' },
                        { valueField: 'val2', name: 'OAT', axis: "OAT", type: "spline" },
                        ],
                    argumentAxis: { valueMarginsEnabled: false },
                    sliderMarker: {
                        placeholderSize: {
                            width: 65,
                            height: 30
                        },
                        format: "hour"
                    },
                    valueAxis: [{
                        grid:{
                            visible: true
                            }
                        },{
                        name: "OAT",
                        position: "right",
                        grid: {
                            visible: true
                        },
                        title: {
                            text: "OutSide Temperature"
                        },
                        label: {
                            format: "largeNumber"
                        }
                    }]

                }
    })

    .controller('SettingsCtrl', function ($scope) {
        $scope.realdata = 1;

        var loadSettings = function(){
            $scope.realdata = 0;   
            if (window.localStorage['WattAppSettings.Realdata'] == 'true')
                 $scope.realdata = 1;

            console.log('load RealData check ' + $scope.realdata + ' local ' + window.localStorage['WattAppSettings.Realdata'] );
        }
        $scope.saveSettings = function(realdata){
            console.log('Saved RealData check ' + realdata);
            // TO DO 
            // Save a JSON format
            window.localStorage['WattAppSettings.Realdata'] = realdata;
        };

        loadSettings();
        console.log('value RealData check ' + $scope.realdata );
    })

        .controller('PeaksCtrl', function ($scope, $stateParams, getPeaksDemandDataSync, MeterHistoryService, SettingsService) {
            $scope.chartSettings = {

                dataSource: getPeaksDemandDataSync.WeeklyPeaks,
                margin:{right:20},
                legend: {
                    visible:false,
                },
                series: {
                  argumentField: "t",
                  valueField: "val",
                  type: "bar",
                  color: '#ffa500'
                },
                customizePoint: function() {
                                if(this.value > getPeaksDemandDataSync.YearToDatePeak) {
                                    return { color: '#ff4500' };
                                } else if(this.value < getPeaksDemandDataSync.AverageWeeklyDemand) {
                                    return { color: '#00ced1' };
                                }
                 },
                 argumentAxis: {
                    label: {
                        overlappingBehavior: { mode: 'auto'}
                    }
                },
                valueAxis: {
                    valueType: 'numeric',
                    max: getPeaksDemandDataSync.YearToDatePeak + 100,
                    constantLines: [{
                        value: getPeaksDemandDataSync.YearToDatePeak,
                        width: 2,
                        dashStyle: 'dash',
                        color: '#FF0000',
                        label: {
                            text: 'Highest',
                            position: 'outside',
                            horizontalAlignment: 'right'
                        }
                    },
                    {
                        value: getPeaksDemandDataSync.AverageWeeklyDemand,
                        width: 2,
                        dashStyle: 'dash',
                        color: '#00ced1',
                        label: {
                            text: 'Average',
                            position: 'outside',
                            horizontalAlignment: 'right'
                        }
                    }]
                }
            }
    });


