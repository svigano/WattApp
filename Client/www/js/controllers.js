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
            $scope.meters = MetersService.findAll(customerGuid);
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

    .controller('MetersReportsConsumptionCtrl', function ($scope, $stateParams, MeterHistoryService) {
            $scope.chartSettings = {

                dataSource: {
                    load: function(){
                        return MeterHistoryService.getLastWeekConsumption($stateParams.meterId);
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

    .controller('MetersReportsCtrl', function ($scope, $http, $q, $stateParams, MeterHistoryService) {
            console.log("first this one");
            var data = [];
                $scope.chartSettings = {

                    dataSource: {
                        load: function(){
                            return MeterHistoryService.getDemandTodayVsYesterday($stateParams.meterId);
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
                    margin:{right:10},
                    palette: ['#ffae00', '#ff7700', '#fa6a63'],
                    commonSeriesSettings: {
                        argumentField: 't',
                        opacity: 0.4,
                        type: 'splinearea',
                    },
                    series: [
                        { valueField: 'val1', name: 'yesterday' },
                        { valueField: 'val2', name: 'today' }
                        ],
                    argumentAxis: { valueMarginsEnabled: false },
                    sliderMarker: {
                        placeholderSize: {
                            width: 65,
                            height: 30
                        },
                        format: "hour"
                    }
                }
            console.log(data.length);
            console.log($scope.chartSettings.dataSource);
    })

    .controller('MetersReportsWeatherCtrl', function ($scope, $http, $q, $stateParams, MeterHistoryService) {
            console.log("first this one");
            var data = [];
                $scope.chartSettings = {

                    dataSource: {
                        load: function(){
                            return MeterHistoryService.getTodayWeather();
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
                    margin:{right:10},
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
    });

