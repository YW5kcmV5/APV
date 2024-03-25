AvtoligaApp.controller('TrademarkController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.trademark = null;

    $scope.init = function (trademarkJson) {
        $scope.trademark = JSON.parse(trademarkJson);
    };
    
}]);