angular.module('wattapp.rest-services', ['ngResource'])

    .factory('MetersService', function($resource, $http,$q) {

        // We use promises to make this api asynchronous. This is clearly not necessary when using in-memory data
        // but it makes this service more flexible and plug-and-play. For example, you can now easily replace this
        // service with a JSON service that gets its data from a remote server without having to changes anything
        // in the modules invoking the data service since the api is already async.
        var baseAPIRoot = "http://wattappbackend.azurewebsites.net/api"
        

        var convertToLocalTime = function(data, headers){
            console.log('convertToLocalTime' + meterInfo);
            var meterInfo = JSON.parse(data);
            console.log('convertToLocalTime ' + meterInfo.lastUpdate + ' local:' +moment.utc(meterInfo.lastUpdate).toDate());
            meterInfo.lastUpdate = moment(moment.utc(meterInfo.lastUpdate).toDate()).format('LLL')
            return meterInfo;                    
        }


        return {
            findAll: function(customerGuid) {
                var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard';
                // Remark -> $resource return an promise....
                var meters = $resource(endpoint).query();
                return meters;
            },

            findById: function(customerGuid, meterId) {
                console.log("Request meter detail " + url);
                var url = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId;
                var def = $q.defer();
                $http({ method: 'GET', url: url, transformResponse: convertToLocalTime }).success(function (data) {
                    def.resolve(data);
                    console.log(data);
                });
                console.log(def);

                return def.promise;
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

        var todayDemandWeather = [
              { t: new Date(2014, 04, 28,0,0,0), val1: 190, val2: 59 },
              { t: new Date(2014, 04, 28,1,0,0), val1: 230, val2: 60 },
              { t: new Date(2014, 04, 28,2,0,0), val1: 240, val2: 61 },
              { t: new Date(2014, 04, 28,3,0,0), val1: 230, val2: 63 },
              { t: new Date(2014, 04, 28,4,0,0), val1: 200, val2: 64 },
              { t: new Date(2014, 04, 28,5,0,0), val1: 190, val2: 64 },
              { t: new Date(2014, 04, 28,6,0,0), val1: 160, val2: 64 },
              { t: new Date(2014, 04, 28,7,0,0), val1: 160, val2: 68 },
              { t: new Date(2014, 04, 28,8,0,0), val1: 160, val2: 68 },
              { t: new Date(2014, 04, 28,9,0,0), val1: 120, val2: 70 },
              { t: new Date(2014, 04, 28,10,0,0), val1: 120, val2: 70 },
              { t: new Date(2014, 04, 28,11,0,0), val1: 160, val2: 71 },
              { t: new Date(2014, 04, 28,12,0,0), val1: 180, val2: 71 },
              { t: new Date(2014, 04, 28,13,0,0), val1: 180, val2: 71 },
              { t: new Date(2014, 04, 28,14,0,0), val1: 230, val2: 71 },
              { t: new Date(2014, 04, 28,15,0,0), val1: 200, val2: 70 },
              { t: new Date(2014, 04, 28,16,0,0), val1: 190, val2: 70 },
              { t: new Date(2014, 04, 28,17,0,0), val1: 160, val2: 68 },
              { t: new Date(2014, 04, 28,18,0,0), val1: 160, val2: 68 },
              { t: new Date(2014, 04, 28,19,0,0), val1: 160, val2: 68 },
              { t: new Date(2014, 04, 28,20,0,0), val1: 120, val2: 66 },
              { t: new Date(2014, 04, 28,21,0,0), val1: 120, val2: 63 },
              { t: new Date(2014, 04, 28,22,0,0), val1: 160, val2: 60 },
              { t: new Date(2014, 04, 28,23,0,0), val1: 180, val2: 60 },
              { t: new Date(2014, 04, 29,0,0,0), val1: 180, val2: 59 }
              ];

        return {
                getDemandTodayVsYesterday: function(meterId) {
                    var def = $.Deferred();
                    $http({ method: 'GET', url: baseAPIRoot+'/MockMeterHistory', transformResponse: convertToDatetime }).success(function (data) {
                    def.resolve(data);
                    console.log(data);
                  });
                  return def.promise();
                },

                getTodayWeather: function(meterId) {
                  return todayDemandWeather;
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
    })

    .factory('SettingsService', function($q) {

        return {
            getSelectedCustomer: function() {
                // TO DO
                // Read guid from network and storage....
                var customerGuid = '123mock123';

                if (window.localStorage['WattAppSettings.Realdata'] == 'true')
                    customerGuid = 'uOKheQeUJ067n4UyVPeMVw'

                return customerGuid;
            },

        }

});




