angular.module('wattapp.building_controllers', [])

    .controller('BuildingsIndexCtrl', function ($scope, BuildingService) {

        $scope.searchKey = "";

        $scope.clearSearch = function () {
            $scope.searchKey = "";
            findAllbuilding();
        }

        $scope.search = function () {
            BuildingService.findByName($scope.searchKey).then(function (b) {
                $scope.buildings = b;
            });
        }

        var findAllbuilding = function() {
            BuildingService.findAll().then(function (b) {
                $scope.buildings = b;
                console.log(b)
            });
        }

        findAllbuilding();

    })

    .controller('BuildingsDetailCtrl', function ($scope, $stateParams, BuildingService) {
        BuildingService.findById($stateParams.buildingId).then(function(b) {
            $scope.building = b;
        });
    });

