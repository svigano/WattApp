
// REMARK
// To use the in-memory meter service inject -> 'wattapp.services'
// To use the local wattApp mock server (node) for the meter service inject -> 'wattapp.rest-services'
// The server is locacate in the server folder

angular.module('wattapp', ['ionic','dx','wattapp.rest-services', 'wattapp.controllers', 'wattapp.building_services','wattapp.building_controllers'])
    
    .run(function($ionicPlatform) {
      $ionicPlatform.ready(function() {
        if(window.StatusBar) {
          // org.apache.cordova.statusbar required
          StatusBar.styleDefault();
        }
        console.log("This is a test: $ionicPlatform.ready");
      });
    })

    .config(function ($stateProvider, $urlRouterProvider) {

        // Ionic uses AngularUI Router which uses the concept of states
        // Learn more here: https://github.com/angular-ui/ui-router
        // Set up the various states which the app can be in.
        // Each state's controller can be found in controllers.js
        $stateProvider

            .state('tab', {
                url: '/tab',
                abstract: true,
                templateUrl: 'templates/tabs.html',
            })

            // Each tab has its own nav history stack:

            .state('tab.meter', {
                url: '/meter',
                views:{
                    'tab-meter':{
                        templateUrl: 'templates/meters-index.html',
                        controller: 'MetersIndexCtrl'
                    }
                }
            })

            .state('tab.meter-detail', {
                url: '/meter/:meterId',
                views:{
                    'tab-meter':{
                        templateUrl: 'templates/meters-detail.html',
                        controller: 'MetersDetailCtrl'
                    }
                }
            })

            .state('tab.meter-reports', {
                url: '/meter/:meterId/reports',
                views:{
                    'tab-meter':{
                        templateUrl: 'templates/meters-reports.html',
                        controller: 'MetersReportsCtrl'
                    }
                }
            })

            .state('tab.meters-reports-consumption', {
                url: '/meter/:meterId/reports/consumption',
                views:{
                    'tab-meter':{
                        templateUrl: 'templates/meters-reports-consumption.html',
                        controller: 'MetersReportsConsumptionCtrl'
                    }
                }
            })

            .state('tab.settings', {
                url: '/settings',
                views:{
                    'tab-settings':{
                        templateUrl: 'templates/settings.html',
                        controller: 'SettingsCtrl'
                    }
                }
            });


        // if none of the above states are matched, use this as the fallback
        $urlRouterProvider.otherwise('/tab/meter');
    });
