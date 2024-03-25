AvtoligaApp.controller('HeaderController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.init = function() {
    };

    $scope.login = function () {

        var loginAction = function(username, password) {
            var url = AvtoligaApp.FormatUrl("/api/login/", true);
            var data = { username: username, password: password };
            $http({
                method: 'POST',
                url: url,
                data: data,
            }).then(function(response) {
                if ((response.data == null) || (response.data != "true")) {
                    AvtoligaApp.alert("Ошибка авторизации!<BR>Неверные имя пользователя или пароль.");
                } else {
                    AvtoligaApp.reload();
                }
            });
        };

        AvtoligaApp.login(loginAction);
    };

    $scope.logout = function () {
        var url = AvtoligaApp.FormatUrl("/api/logout/");
        $http({
            method: 'POST',
            url: url,
        }).then(function () {
            AvtoligaApp.reload();
        });
    };

}]);