angular.module('wattapp.rest-services', ['ngResource'])

    .factory('MetersService', function($resource, $q) {

        // We use promises to make this api asynchronous. This is clearly not necessary when using in-memory data
        // but it makes this service more flexible and plug-and-play. For example, you can now easily replace this
        // service with a JSON service that gets its data from a remote server without having to changes anything
        // in the modules invoking the data service since the api is already async.
        var baseAPIRoot = "http://wattappbackend.azurewebsites.net/api"
        
        // Real data
        // /customer/7iULAhT9vUuLr9A8r2Eb5g/dashboard

        return {
            findAll: function() {
                // Remark -> $resource return an promise....
                var endpoint = baseAPIRoot +'/mockmeters';
                var meters = $resource(endpoint).query();
                return meters;
            },

            findById: function(meterId) {
                console.log("Request meter detail");
                var meter = $resource(baseAPIRoot+'/mockmeters/:Id').get({Id:1});
                return meter;
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


    .factory('MeterHistoryService', function($http, $q) {

        var baseAPIRoot = "http://wattappbackend.azurewebsites.net/api"

        var convertToDatetime = function(data, headers){
            var data = JSON.parse(data);
            if (data.length){
              data = _.map(data, function(sample){
                return {t: moment(sample.t).toDate(), val1: sample.val1, val2: sample.val2}
              });
            }
            return data;                    
        }

        var convertToDay = function(data, headers){
            var data = JSON.parse(data);
            if (data.length){
              data = _.map(data, function(sample){
                return {t: moment(sample.t).day(), val: sample.val}
              });
            }
            return data;                    
        }

        return {
                getDemandTodayVsYesterday: function(meterId) {
                    var def = $.Deferred();
                    $http({ method: 'GET', url: baseAPIRoot+'/MockMeterHistory', transformResponse: convertToDatetime }).success(function (data) {
                    def.resolve(data);
                    console.log(data);
                  });
                  return def.promise();
                },

                getLastWeekConsumption: function(meterId) {
                    var def = $.Deferred();
                    $http({ method: 'GET', url: baseAPIRoot+'/consumption', transformResponse: convertToDay }).success(function (data) {
                    def.resolve(data);
                    console.log(data);
                  });
                  return def.promise();
                  },

        }
  });




