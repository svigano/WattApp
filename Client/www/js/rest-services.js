angular.module('wattapp.rest-services', ['ngResource'])

    .factory('MetersService', function($resource, $http,$q) {

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
                return {t: moment(moment(sample.t).toDate()).format('ddd'), val: sample.val}
              });
            }
            return data;                    
        }

        return {
                getDemandTodayVsYesterday: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId;
                    $http({ method: 'GET', url: endpoint+'/DemandVsYesterday', transformResponse: convertToDatetime }).success(function (data) {
                    def.resolve(data);
                  });
                  return def.promise();
                },

                getTodayWeather: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId;
                    $http({ method: 'GET', url: endpoint+'/demandAndWeather', transformResponse: convertToDatetime }).success(function (data) {
                    def.resolve(data);
                  });
                  return def.promise();
                  },

                getLastWeekConsumption: function(customerGuid, meterId) {
                    var def = $.Deferred();
                    var endpoint = baseAPIRoot+'/customer/'+customerGuid+'/dashboard/'+meterId;
                    $http({ method: 'GET', url: endpoint+'/lastweekConsumption', transformResponse: convertToDay }).success(function (data) {
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




