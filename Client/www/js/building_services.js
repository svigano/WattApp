angular.module('wattapp.building_services', [])

    .factory('BuildingService', function($q) {

        var buildings = [
            {"id": 1, "name" : "507 Michigan", "inc": -3},
            {"id": 2, "name" : "5757 Corporate", "inc": 2},
            {"id": 3, "name" : "Mke Hangar", "inc": 5},
            {"id": 4, "name" : "Plymouth", "inc": -7},
            {"id": 5, "name" : "York", "inc": -10},
            {"id": 6, "name" : "5757 Solar", "inc": -12},
            {"id": 7, "name" : "5757 Corporate", "inc": 1},
            {"id": 8, "name" : "5757 Corporate", "inc": 9}
        ];

        // We use promises to make this api asynchronous. This is clearly not necessary when using in-memory data
        // but it makes this service more flexible and plug-and-play. For example, you can now easily replace this
        // service with a JSON service that gets its data from a remote server without having to changes anything
        // in the modules invoking the data service since the api is already async.

        return {
            findAll: function() {
                var deferred = $q.defer();
                deferred.resolve(buildings);
                return deferred.promise;
            },

            findById: function(meterId) {
                var deferred = $q.defer();
                var building = buildings[meterId - 1];
                deferred.resolve(building);
                return deferred.promise;
            },

            findByName: function(searchKey) {
                var deferred = $q.defer();
                var results = buildings.filter(function(element) {
                    var fullName = element.firstName + " " + element.lastName;
                    return fullName.toLowerCase().indexOf(searchKey.toLowerCase()) > -1;
                });
                deferred.resolve(results);
                return deferred.promise;
            },

        }

    });


    
