AvtoligaApp.controller('FeedbackController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.feedback =
    {
        Name: "",
        Email: "",
        Phone: "",
        Text: "",
        Type: 0,
    };

    $scope._isValid = function() {
        var feedback = $scope.feedback;
        var isValid = (feedback != null);
        isValid &= (feedback.Name != null) && (feedback.Name.length > 0);
        isValid &= (feedback.Email != null) && (feedback.Email.length > 0);
        isValid &= (feedback.Text != null) && (feedback.Text.length > 0);
        isValid &= (feedback.Type != null) && ((feedback.Type == 0) || (feedback.Type == 1) || (feedback.Type == 2));
        return isValid;
    };

    $scope.init = function() {
    };

    $scope.submit = function () {
        if ($scope._isValid()) {
            var url = AvtoligaApp.FormatUrl("/api/feedback/");
            $http({
                method: 'POST',
                url: url,
                data: $scope.feedback,
            }).then(function (response) {
                var apiResult = response.data;
                if (apiResult != null) {
                    if ((apiResult.Message != null) && (apiResult.Message.length > 0)) {
                        AvtoligaApp.alert("Ошибка!<BR>" + apiResult.Message);
                    }
                    if (apiResult.SuccessValue == 1) {
                        AvtoligaApp.Redirect("/feedbacks/");
                    }
                }
            });
        }
    };

}]);