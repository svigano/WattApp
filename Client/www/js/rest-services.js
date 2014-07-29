angular.module('wattapp.rest-services', ['ngResource', 'wattapp.app-services'])

    .factory('MetersService', function(DSCacheFactory, $resource, $http,$q) {

        // Create a new cache 
        var networkDataCache = DSCacheFactory('networkDataCache', {
            //maxAge: 3600000,
            //recycleFreq: 1000000, // ms
            cacheFlushInterval: 1200000,
            deleteOnExpire: 'aggressive',
            storageMode: 'localStorage'
            }
        );

        var baseAPIRoot = "http://wattappbackend.azurewebsites.net/api"

        // Takes an ISO time and returns a string representing how
        // long ago the date represents.
        var prettyDate = function (date){
            diff = (((new Date()).getTime() - date.getTime()) / 1000),
            day_diff = Math.floor(diff / 86400);
            
            if ( isNaN(day_diff) || day_diff < 0 || day_diff >= 31 )
                return;
            
            return day_diff == 0 && (
                    diff < 60 && "just now" ||
                    diff < 120 && "1 minute ago" ||
                    diff < 3600 && Math.floor( diff / 60 ) + " minutes ago" ||
                    diff < 7200 && "1 hour ago" ||
                    diff < 86400 && Math.floor( diff / 3600 ) + " hours ago") ||
                day_diff == 1 && "Yesterday" ||
                day_diff < 7 && day_diff + " days ago" ||
                day_diff < 31 && Math.ceil( day_diff / 7 ) + " weeks ago";
        }

        var convertToLocalTime = function(data, headers){
            var meterInfo = JSON.parse(data);
            console.log('convertToLocalTime ' + meterInfo.lastUpdate + ' local:' +moment.utc(meterInfo.lastUpdate).toDate());
            meterInfo.lastUpdate = prettyDate(moment.utc(meterInfo.lastUpdate).toDate());
            return meterInfo;                    
        }



        return {
            findAll: function(customerGuid) {

                var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard';
                // Remark -> $resource return an promise....
                //var meters = $resource(endpoint).query();
                var def = $q.defer();
                var startRequestTime = new Date().getTime();
                $http({ method: 'GET', url: endpoint, cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertToLocalTime }).success(function (data) {
                    console.log(endpoint)
                    console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                    def.resolve(data);
                });

                return def.promise;
            },

            findById: function(customerGuid, meterId) {
                console.log("Request meter detail " + url);
                var url = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId;
                var def = $q.defer();
                var startRequestTime = new Date().getTime();
                $http({ method: 'GET', url: url, cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertToLocalTime }).success(function (data) {
                    console.log(url)
                    console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                    def.resolve(data);
                });

                return def.promise;
            },
        }

    })


    .factory('MeterHistoryService', function(DSCacheFactory, $http, $q) {

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
                return {t: moment(moment(sample.t).toDate()).format('ddd'), val: sample.val}
              });
            }
            return data;                    
        }

        return {
                getDemandTodayVsYesterday: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var startRequestTime = new Date().getTime();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId+'/DemandVsYesterday';
                    $http({ method: 'GET', url: endpoint, cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertToDatetime }).success(function (data) {
                        console.log(endpoint)
                        console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                        def.resolve(data);
                  });
                  return def.promise();
                },

                getTodayWeather: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var startRequestTime = new Date().getTime();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId+'/demandAndWeather';
                    $http({ method: 'GET', url: endpoint, cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertToDatetime }).success(function (data) {
                        console.log(endpoint)
                        console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                        def.resolve(data);
                  });
                  return def.promise();
                  },

                getLastWeekConsumption: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId;
                    var startRequestTime = new Date().getTime();
                    $http({ method: 'GET', url: endpoint+'/lastweekConsumption', cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertToDay }).success(function (data) {
                        console.log(endpoint)
                        console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                        def.resolve(data);
                    console.log(data);
                  });
                  return def.promise();
                  },

        }
    });




