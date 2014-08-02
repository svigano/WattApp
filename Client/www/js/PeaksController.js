angular.module('wattapp.PeaksController', [])

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

