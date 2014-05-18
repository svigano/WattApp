// angular.module is a global place for creating, registering and retrieving Angular modules
// 'wattapp' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
// 'wattapp.services' is found in services.js
// 'wattapp.controllers' is found in controllers.js
angular.module('wattapp', ['ionic','dx','wattapp.services', 'wattapp.controllers'])
    
    .run(function($ionicPlatform) {
      $ionicPlatform.ready(function() {
        if(window.StatusBar) {
          // org.apache.cordova.statusbar required
          StatusBar.styleDefault();
        }
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

            .state('tab.building', {
                url: '/building',
                views:{
                    'tab-building':{
                        templateUrl: 'templates/buildings-index.html',
                        //controller: ''
                    }
                }
            });

        // if none of the above states are matched, use this as the fallback
        $urlRouterProvider.otherwise('/tab/meter');
    });
