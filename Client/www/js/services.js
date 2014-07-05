angular.module('wattapp.services', [])

    .factory('MetersService', function($q) {

        var meters = [
            {"Id": 1, "Location" : "507 Michigan","Name":"Main utility meter","Demand": 340, "Inc": -2},
            {"Id": 2, "Location" : "5757 Corporate","Name":"Main meter","Demand": 680, "Inc": +0.3},
            {"Id": 3, "Location" : "Mke Hangar","Name":"Utility meter","Demand": 430, "Inc": +5},
            {"Id": 4, "Location" : "Plymouth","Name":"Building 36 meter","Demand": 298, "Inc": -0.2},
            {"Id": 5, "Location" : "York","Name":"CTU meter","Demand": 1200, "Inc": -2},
            {"Id": 6, "Location" : "5757 Solar","Name":"Solar meter","Demand": 910, "Inc": -3},
            {"Id": 7, "Location" : "5757 Corporate","Name":"Roof Array meter","Demand": 545, "Inc": +0.7},
            {"Id": 8, "location" : "5757 Corporate","Name":"Pumps eletric meter","Demand": 212, "Inc": +1.1}
        ];

        // We use promises to make this api asynchronous. This is clearly not necessary when using in-memory data
        // but it makes this service more flexible and plug-and-play. For example, you can now easily replace this
        // service with a JSON service that gets its data from a remote server without having to changes anything
        // in the modules invoking the data service since the api is already async.

        return {
            findAll: function() {
                console.log("----------- InMemory -------------")                
                return meters;
            },

            findById: function(meterId) {
                console.log("----------- InMemory -------------")
                var meter = meters[meterId - 1];
                console.log(meter);
                return meter;
            },

            findByName: function(searchKey) {
                var results = meters.filter(function(element) {
                    var fullName = element.firstName + " " + element.lastName;
                    return fullName.toLowerCase().indexOf(searchKey.toLowerCase()) > -1;
                });
                return results;
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
              { t: "Monday", val: 500},
              { t: "Tuesday", val: 550},
              { t: "Wednesday", val: 480},
              { t: "Thursday", val: 600},
              { t: "Friday", val: 560},
              { t: "Saturday", val: 320},
              { t: "Sunday", val: 380}
              ];

        return {
            getDemandTodayVsYesterday: function(meterId) {
                console.log("----------- InMemory -------------")
                var deferred = $q.defer();
                deferred.resolve(dataSamplesDemandTodayVsYesterday);
                //return deferred.promise;
                return dataSamplesDemandTodayVsYesterday;
            },

            getLastWeekConsumption: function(meterId) {
                console.log("----------- InMemory -------------")
                return dataSamplesLastWeekConsumption;
            },

        }

    });




