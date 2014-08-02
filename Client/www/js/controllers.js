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

    .controller('MetersReportsCtrl', function ($scope, $http, $q, $stateParams, MeterHistoryService, SettingsService) {
            var customerGuid = SettingsService.getSelectedCustomer();
            var data = [];
                $scope.chartSettings = {
                    dataSource: {
                        load: function(){
                            return MeterHistoryService.getDemandTodayVsYesterday(customerGuid, $stateParams.meterId);
                        }
                    },
                    legend: {
                        visible:true,
                        horizontalAlignment:"center"
                    },
                    size: { height: 400 },
                    margin:{right:10},
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
                        label: {format:'shortTime'}
                    },
                    valueAxis:{ min: 200}
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
                    size: { height: 400 },
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
        var customers = [{"name":"ACME","guid":"123mock123"},{"name":"JCI","guid":"uOKheQeUJ067n4UyVPeMVw"}]
        $scope.customers = customers;
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
            var customerGuid = SettingsService.getSelectedCustomer();
            console.log('Resolved data ' + getPeaksDemandDataSync.length)
            console.log(getPeaksDemandDataSync)
            $scope.chartSettings = {

                dataSource: getPeaksDemandDataSync.items,
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
                },
                customizePoint: function() {
                                if(this.value > getPeaksDemandDataSync.peakTd) {
                                    return { color: '#ff4500' };
                                } else if(this.value < getPeaksDemandDataSync.average) {
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
                    max: getPeaksDemandDataSync.peakTd + 100,
                    constantLines: [{
                        value: getPeaksDemandDataSync.peakTd,
                        width: 2,
                        dashStyle: 'dash',
                        color: '#FF0000',
                        label: {
                            text: 'Highest',
                            position: 'outside'
                        }
                    },
                    {
                        value: getPeaksDemandDataSync.average,
                        width: 2,
                        dashStyle: 'dash',
                        color: '#00ced1',
                        label: {
                            text: 'Average',
                            position: 'outside'
                        }
                    }]
                }
            }
    });


