angular.module('wattapp.services', [])

    .factory('MetersService', function($q) {

        var meters = [
            {"id": 1, "location" : "507 Michigan","name":"Main utility meter","demand": 340, "inc": -2},
            {"id": 2, "location" : "5757 Corporate","name":"Main meter","demand": 680, "inc": +0.3},
            {"id": 3, "location" : "Mke Hangar","name":"Utility meter","demand": 430, "inc": +5},
            {"id": 4, "location" : "Plymouth","name":"Building 36 meter","demand": 298, "inc": -0.2},
            {"id": 5, "location" : "York","name":"CTU meter","demand": 1200, "inc": -2},
            {"id": 6, "location" : "5757 Solar","name":"Solar meter","demand": 910, "inc": -3},
            {"id": 7, "location" : "5757 Corporate","name":"Roof Array meter","demand": 545, "inc": +0.7},
            {"id": 8, "location" : "5757 Corporate","name":"Pumps eletric meter","demand": 212, "inc": +1.1}
        ];

        // We use promises to make this api asynchronous. This is clearly not necessary when using in-memory data
        // but it makes this service more flexible and plug-and-play. For example, you can now easily replace this
        // service with a JSON service that gets its data from a remote server without having to changes anything
        // in the modules invoking the data service since the api is already async.

        return {
            findAll: function() {
                var deferred = $q.defer();
                deferred.resolve(meters);
                return deferred.promise;
            },

            findById: function(meterId) {
                var deferred = $q.defer();
                var meter = meters[meterId - 1];
                deferred.resolve(meter);
                return deferred.promise;
            },

            findByName: function(searchKey) {
                var deferred = $q.defer();
                var results = meters.filter(function(element) {
                    var fullName = element.firstName + " " + element.lastName;
                    return fullName.toLowerCase().indexOf(searchKey.toLowerCase()) > -1;
                });
                deferred.resolve(results);
                return deferred.promise;
            },
        }

    })


    .factory('MeterHistoryService', function($q) {

        var dataSamplesDemandTodayVsYesterday = [
              { t: new Date(2014, 04, 28,0,0,0), val1: 19, val2: 18 },
              { t: new Date(2014, 04, 28,1,0,0), val1: 23, val2: 17 },
              { t: new Date(2014, 04, 28,2,0,0), val1: 24, val2: 19 },
              { t: new Date(2014, 04, 28,3,0,0), val1: 23, val2: 24 },
              { t: new Date(2014, 04, 28,4,0,0), val1: 20, val2: 25 },
              { t: new Date(2014, 04, 28,5,0,0), val1: 19, val2: 24 },
              { t: new Date(2014, 04, 28,6,0,0), val1: 16, val2: 24 },
              { t: new Date(2014, 04, 28,7,0,0), val1: 16, val2: 20 },
              { t: new Date(2014, 04, 28,8,0,0), val1: 16, val2: 14 },
              { t: new Date(2014, 04, 28,9,0,0), val1: 12, val2: 12 },
              { t: new Date(2014, 04, 28,10,0,0), val1: 12, val2: 13 },
              { t: new Date(2014, 04, 28,11,0,0), val1: 16, val2: 13 },
              { t: new Date(2014, 04, 28,12,0,0), val1: 18, val2: 16 },
              { t: new Date(2014, 04, 28,13,0,0), val1: 18, val2: 16 },
              { t: new Date(2014, 04, 28,14,0,0), val1: 23, val2: 24 },
              { t: new Date(2014, 04, 28,15,0,0), val1: 20, val2: 25 },
              { t: new Date(2014, 04, 28,16,0,0), val1: 19, val2: 24 },
              { t: new Date(2014, 04, 28,17,0,0), val1: 16, val2: 24 },
              { t: new Date(2014, 04, 28,18,0,0), val1: 16, val2: 20 },
              { t: new Date(2014, 04, 28,19,0,0), val1: 16, val2: 14 },
              { t: new Date(2014, 04, 28,20,0,0), val1: 12, val2: 12 },
              { t: new Date(2014, 04, 28,21,0,0), val1: 12, val2: 13 },
              { t: new Date(2014, 04, 28,22,0,0), val1: 16, val2: 13 },
              { t: new Date(2014, 04, 28,23,0,0), val1: 18, val2: 16 },
              { t: new Date(2014, 04, 29,0,0,0), val1: 18, val2: 16 }
              ];

        var dataSamplesLastWeekConsumption = [
              { day: "Monday", consumption: 500},
              { day: "Tuesday", consumption: 550},
              { day: "Wednesday", consumption: 480},
              { day: "Thursday", consumption: 600},
              { day: "Friday", consumption: 560},
              { day: "Saturday", consumption: 320},
              { day: "Sunday", consumption: 380}
              ];

        return {
            getDemandTodayVsYesterday: function(meterId) {
                var deferred = $q.defer();
                deferred.resolve(dataSamplesDemandTodayVsYesterday);
                //return deferred.promise;
                return dataSamplesDemandTodayVsYesterday;
            },

            getLastWeekConsumption: function(meterId) {
                return dataSamplesLastWeekConsumption;
            },

        }

    });




