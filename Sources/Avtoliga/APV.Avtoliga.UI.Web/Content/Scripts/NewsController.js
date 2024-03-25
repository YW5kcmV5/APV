AvtoligaApp.controller('NewsController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.init = function() {
    };

    $scope.toggle = function (id) {
        var item = $("#news_" + id);
        var likesItem = item.find("span.count");
        if ((item.length == 1) && (likesItem.length == 1)) {
            var liked = (item.hasClass("active"));
            liked = !liked;
            var likedValue = liked ? "True" : "False";
            var url = AvtoligaApp.FormatUrl("/api/news_toggle?id=" + id + "&liked=" + likedValue);
            $http({
                method: 'GET',
                url: url,
            }).then(function(response) {
                var likes = response.data;
                if (likes != -1) {
                    likesItem.text(likes);
                    if (liked) {
                        item.addClass("active");
                    } else {
                        item.removeClass("active");
                    }
                }
            });
        }
    };

}]);