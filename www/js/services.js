angular.module('directory.services', [])

    .factory('MetersService', function($q) {

        var meters = [
            {"id": 1, "location" : "507 Michigan","name":"Main utility meter","demand": 340, "inc": -2,"firstName": "James", "lastName": "King", "managerId": 0, "managerName": "", "reports": 4, "title": "President and CEO", "department": "Corporate", "cellPhone": "617-000-0001", "officePhone": "781-000-0001", "email": "jking@fakemail.com", "city": "Boston, MA", "pic": "James_King.jpg", "twitterId": "@fakejking", "blog": "http://coenraets.org"},
            {"id": 2, "location" : "5757 Corporate","name":"Main meter","demand": 680, "inc": 0.3,"firstName": "Julie", "lastName": "Taylor", "managerId": 1, "managerName": "James King", "reports": 2, "title": "VP of Marketing", "department": "Marketing", "cellPhone": "617-000-0002", "officePhone": "781-000-0002", "email": "jtaylor@fakemail.com", "city": "Boston, MA", "pic": "Julie_Taylor.jpg", "twitterId": "@fakejtaylor", "blog": "http://coenraets.org"},
            {"id": 3, "location" : "Mke Hangar","name":"Utility meter","demand": 430, "inc": 5,"firstName": "Eugene", "lastName": "Lee", "managerId": 1, "managerName": "James King", "reports": 0, "title": "CFO", "department": "Accounting", "cellPhone": "617-000-0003", "officePhone": "781-000-0003", "email": "elee@fakemail.com", "city": "Boston, MA", "pic": "Eugene_Lee.jpg", "twitterId": "@fakeelee", "blog": "http://coenraets.org"},
            {"id": 4, "location" : "Plymouth","name":"Building 36 meter","demand": 298, "inc": -0.2,"firstName": "John", "lastName": "Williams", "managerId": 1, "managerName": "James King", "reports": 3, "title": "VP of Engineering", "department": "Engineering", "cellPhone": "617-000-0004", "officePhone": "781-000-0004", "email": "jwilliams@fakemail.com", "city": "Boston, MA", "pic": "John_Williams.jpg", "twitterId": "@fakejwilliams", "blog": "http://coenraets.org"},
            {"id": 5, "location" : "York","name":"CTU meter","demand": 1200, "inc": -2,"firstName": "Ray", "lastName": "Moore", "managerId": 1, "managerName": "James King", "reports": 2, "title": "VP of Sales", "department": "Sales", "cellPhone": "617-000-0005", "officePhone": "781-000-0005", "email": "rmoore@fakemail.com", "city": "Boston, MA", "pic": "Ray_Moore.jpg", "twitterId": "@fakermoore", "blog": "http://coenraets.org"},
            {"id": 6, "location" : "5757 Solar","name":"Solar meter","demand": 910, "inc": -3,"firstName": "Paul", "lastName": "Jones", "managerId": 4, "managerName": "John Williams", "reports": 0, "title": "QA Manager", "department": "Engineering", "cellPhone": "617-000-0006", "officePhone": "781-000-0006", "email": "pjones@fakemail.com", "city": "Boston, MA", "pic": "Paul_Jones.jpg", "twitterId": "@fakepjones", "blog": "http://coenraets.org"},
            {"id": 7, "location" : "5757 Corporate","name":"Roof Array meter","demand": 545, "inc": 0.7,"firstName": "Paula", "lastName": "Gates", "managerId": 4, "managerName": "John Williams", "reports": 0, "title": "Software Architect", "department": "Engineering", "cellPhone": "617-000-0007", "officePhone": "781-000-0007", "email": "pgates@fakemail.com", "city": "Boston, MA", "pic": "Paula_Gates.jpg", "twitterId": "@fakepgates", "blog": "http://coenraets.org"},
            {"id": 8, "location" : "5757 Corporate","name":"Pumps eletric meter","demand": 212, "inc": 1.1,"firstName": "Lisa", "lastName": "Wong", "managerId": 2, "managerName": "Julie Taylor", "reports": 0, "title": "Marketing Manager", "department": "Marketing", "cellPhone": "617-000-0008", "officePhone": "781-000-0008", "email": "lwong@fakemail.com", "city": "Boston, MA", "pic": "Lisa_Wong.jpg", "twitterId": "@fakelwong", "blog": "http://coenraets.org"}
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

            findByManager: function (managerId) {
                var deferred = $q.defer(),
                    results = meters.filter(function (element) {
                        return parseInt(managerId) === element.managerId;
                    });
                deferred.resolve(results);
                return deferred.promise;
            }

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




