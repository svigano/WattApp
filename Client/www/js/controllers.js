angular.module('wattapp.controllers', [])

    .controller('MetersIndexCtrl', function ($scope, MetersService) {

        $scope.searchKey = "";

        $scope.clearSearch = function () {
            $scope.searchKey = "";
            findAllmeters();
        }

        $scope.search = function () {
            MetersService.findByName($scope.searchKey).then(function (meters) {
                $scope.meters = meters;
            });
        }

        var findAllmeters = function() {
            MetersService.findAll().then(function (meters) {
                $scope.meters = meters;
            });
            console.log("This is a test")
        }

        findAllmeters();

    })

    .controller('MetersDetailCtrl', function ($scope, $stateParams, MetersService) {
        MetersService.findById($stateParams.meterId).then(function(meter) {
            $scope.meter = meter;
        });
    })

    .controller('MetersReportsConsumptionCtrl', function ($scope, $stateParams, MeterHistoryService) {
            var data = MeterHistoryService.getLastWeekConsumption($stateParams.meterId)
            console.log(data.length)
            console.log(data)
            $scope.chartSettings = {

                dataSource: data,
                margin:{right:20},
                legend: {
                    visible:false,
                    horizontalAlignment:"center"
                },
                series: {
                  argumentField: "day",
                  valueField: "consumption",
                  type: "bar",
                  color: '#ffa500'
                }
            }
    })

    .controller('MetersReportsCtrl', function ($scope, $stateParams, MeterHistoryService) {
            console.log("first this one");
           // MeterHistoryService.getDemandTodayVsYesterday($stateParams.meterId).then(function(data) {
            var data = MeterHistoryService.getDemandTodayVsYesterday($stateParams.meterId)
            console.log(data.length);
            console.log(data);

            $scope.chartSettings = {

                dataSource: data,
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
                margin:{right:20},
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
                },
                selectedRange: {
                    startValue: new Date(2014, 04, 28,9,0,0),
                    endValue: new Date(2014, 04, 28,15,0,0)
                }
            }
        //});
    });
