AvtoligaApp.controller('GalleryController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.images = [];
    $scope.visible = false;
    $scope.prevVisible = false;
    $scope.nextVisible = false;
    $scope.src = "#";
    $scope.index = 0;

    $scope._attach = function () {
        $("body").on("keyup", function (event) {
            if (($scope.visible) && (event.keyCode == 27)) {
                $scope.visible = false;
                $scope.$apply();
            }
        });
    };

    $scope._setImage = function(index) {
        var src = null;
        if ((index >= 0) && (index < $scope.images.length)) {
            src = $scope.images[index];
        } else {
            index = 0;
        }
        if ((src == null) || (src.length == 0)) {
            src = "#";
        }
        $scope.index = index;
        $scope.src = src;
        $scope.prevVisible = (index > 0);
        $scope.nextVisible = (index < $scope.images.length - 1);
    };

    $scope.init = function () {
        AvtoligaApp.ShowGalleryController = $scope;
        $scope._attach();
    };

    $scope.show = function (images, apply) {
        if (images == null) {
            images = [];
        } else {
            if (typeof images == "string") {
                images = JSON.parse(images);
            }
            if (!$.isArray(images)) {
                images = [images];
            }
        }
        $scope.images = images;
        $scope._setImage(0);
        $scope.visible = ($scope.src != "#");
        if (apply === true) {
            $scope.$apply();
        }
    };

    $scope.close = function() {
        $scope.visible = false;
    };

    $scope.prev = function () {
        if ($scope.index > 0) {
            $scope._setImage($scope.index - 1);
        }
    };

    $scope.next = function() {
        if ($scope.index < $scope.images.length - 1) {
            $scope._setImage($scope.index + 1);
        }
    };

    $scope.onClick = function ($event) {
        if ($event != null) {
            var target = $($event.target);
            if (target.is("div.rui-lightbox-locker")) {
                $scope.close();
            }
        }
    };

}]);