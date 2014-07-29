angular.module('wattapp.app-services', ['ngResource'])

    .factory('LocalStorage', ['$window', function($window) {
      return {
        set: function(key, value) {
          $window.localStorage[key] = value;
        },
        get: function(key, defaultValue) {
          return $window.localStorage[key] || defaultValue;
        },
        setObject: function(key, value) {
          $window.localStorage[key] = JSON.stringify(value);
        },
        getObject: function(key) {
          return JSON.parse($window.localStorage[key] || '{}');
        }
      }
    }])

    .factory('SettingsService', function($q, LocalStorage) {

        return {
            getSelectedCustomer: function() {
                // TO DO
                // Read guid from network and storage....
                var customerGuid = '123mock123';

                //if (window.localStorage['WattAppSettings.Realdata'] == 'true')
                if (LocalStorage.get('WattAppSettings.Realdata') == 'true')
                    customerGuid = 'uOKheQeUJ067n4UyVPeMVw'

                return customerGuid;
            },
        }

});




