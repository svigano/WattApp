angular.module('wattapp.rest-services', ['ngResource', 'wattapp.app-services'])

    .factory('MetersService', function(DSCacheFactory, $resource, $http,$q) {

        // Create the a new network cache 
        // Every other service will try to use this cache.
        // TO DO Find a better place to initialize this cache: What if an other service will execute before this one ?
        var networkDataCache = DSCacheFactory('networkDataCache');

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
            console.log(meterInfo);
            console.log('convertToLocalTime ' + meterInfo.lastUpdate + ' local:' +moment.utc(meterInfo.lastUpdate).toDate());
            meterInfo.lastUpdate = prettyDate(moment.utc(meterInfo.lastUpdate).toDate());
            console.log(meterInfo);
            return meterInfo;                    
        }



        return {
            findAll: function(customerGuid) {

                var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard';
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

        var demandData = { 
                        YearToDatePeak: 590,
                        AverageWeeklyDemand: 450,
                        items : [
                              { t: "Monday", val: 500},
                              { t: "Tuesday", val: 550},
                              { t: "Wednesday", val: 480},
                              { t: "Thursday", val: 600},
                              { t: "Friday", val: 560},
                              { t: "Saturday", val: 320},
                              { t: "Sunday", val: 380}
                          ]
                      };

        var convertToDatetime = function(data, headers){
            var data = JSON.parse(data);
            if (data.length){
              data = _.map(data, function(sample){
                console.log(sample.t)
                return {t: moment.utc(sample.t).toDate(), val1: sample.val1, val2: sample.val2}
              });
            }
            return data;                    
        }

        var convertToDay = function(data, headers){
            var data = JSON.parse(data);
            if (data.length){
              data = _.map(data, function(sample){
                return {t: moment.utc(moment(sample.t).toDate()).format('ddd'), val: sample.val}
              });
            }
            return data;                    
        }

        var convertWeeklyPeakTimeToDay = function(data, headers){
            var data = JSON.parse(data);
            for (index = 0; index < data.WeeklyPeaks.length; ++index)
                data.WeeklyPeaks[index].t = moment(moment(data.WeeklyPeaks[index].t).toDate()).format('ddd')
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
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId+'/lastweekConsumption';
                    var startRequestTime = new Date().getTime();
                    $http({ method: 'GET', url: endpoint, cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertToDay }).success(function (data) {
                        console.log(endpoint)
                        console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                        def.resolve(data);
                        console.log(data);
                    });
                  return def.promise();
                  },

                getPeaksDemandData: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId+'/peakDemandSummary';
                    var startRequestTime = new Date().getTime();
                    $http({ method: 'GET', url: endpoint, cache: DSCacheFactory.get('networkDataCache'), transformResponse: convertWeeklyPeakTimeToDay }).success(function (data) {
                        console.log(endpoint)
                        console.log('time taken for request: ' + (new Date().getTime() - startRequestTime) + 'ms');
                        def.resolve(data);
                    });
                  return def.promise();
                  },
        }
    });




