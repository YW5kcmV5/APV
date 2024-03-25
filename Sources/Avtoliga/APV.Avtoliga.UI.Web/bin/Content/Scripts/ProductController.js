AvtoligaApp.controller('ProductController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.product = null;

    $scope.init = function (productJson) {
        $scope.product = JSON.parse(productJson);
    };

    $scope.showGallery = function() {
        AvtoligaApp.ShowGallery($scope.product.ImageUrls);
    };

}]);