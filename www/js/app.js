// angular.module is a global place for creating, registering and retrieving Angular modules
// 'directory' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
// 'directory.services' is found in services.js
// 'directory.controllers' is found in controllers.js
angular.module('directory', ['ionic','dx','directory.services', 'directory.controllers'])
    
    // .run(function($ionicPlatform, PushProcessingService){
    //     $ionicPlatform.ready(function(){
    //         if (parseFloat(window.device.version) === 7.0) {
    //           document.body.style.marginTop = "20px";
    //           console.log("statusbar margin adjusted")
    //         }
    //         else
    //           console.log("statusbar margin NOT adjusted")

    //     })
    // });


    .config(function ($stateProvider, $urlRouterProvider) {

        // Ionic uses AngularUI Router which uses the concept of states
        // Learn more here: https://github.com/angular-ui/ui-router
        // Set up the various states which the app can be in.
        // Each state's controller can be found in controllers.js
        $stateProvider

            .state('meter-index', {
                url: '/',
                templateUrl: 'templates/meters-index.html',
                controller: 'MetersIndexCtrl'
            })

            .state('meter-detail', {
                url: '/meter/:meterId',
                templateUrl: 'templates/meters-detail.html',
                controller: 'MetersDetailCtrl'
            })

            .state('meter-reports', {
                url: '/meter/:meterId/reports',
                templateUrl: 'templates/meters-reports.html',
                controller: 'MetersReportsCtrl'
            })

            .state('meters-reports-consumption', {
                url: '/meter/:meterId/reports/consumption',
                templateUrl: 'templates/meters-reports-consumption.html',
                controller: 'MetersReportsConsumptionCtrl'
            });

        // if none of the above states are matched, use this as the fallback
        $urlRouterProvider.otherwise('/');

    });
