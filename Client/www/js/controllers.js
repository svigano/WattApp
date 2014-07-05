angular.module('wattapp.controllers', [])

    .controller('MetersIndexCtrl', function ($scope, MetersService) {

        $scope.searchKey = "";

        $scope.clearSearch = function () {
            $scope.searchKey = "";
            findAllmeters();
        }

        $scope.search = function () {
            $scope.meters = MetersService.findByName($scope.searchKey);
        }

        var findAllmeters = function() {
            $scope.meters = MetersService.findAll();
            console.log("This is a test " + $scope.meters)
        }

        findAllmeters();

    })

    .controller('MetersDetailCtrl', function ($scope, $stateParams, MetersService) {
        $scope.meter = MetersService.findById($stateParams.meterId);
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
    });
