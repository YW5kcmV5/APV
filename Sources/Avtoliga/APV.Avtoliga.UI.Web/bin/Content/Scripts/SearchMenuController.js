AvtoligaApp.controller('SearchMenuController', ['$scope', '$http', '$timeout', '$filter', function ($scope, $http, $timeout, $filter) {

    $scope.trademarks = null;
    $scope.models = null;
    $scope.groups = null;
    $scope.trademark = null;
    $scope.model = null;
    $scope.group = null;
    $scope.producer = null;
    $scope.itemsContainer = null;
    $scope.itemsPointers = null;
    $scope.itemsMode = null;
    $scope.itemsVisible = false;
    $scope.itemsExtended = false;
    $scope.items = [];

    $scope._selectTrademark = function(trademarkId) {
        var trademark = null;
        if (($scope.trademarks != null) && ($scope.trademarks.length > 0)) {
            if (trademarkId != null) {
                var length = $scope.trademarks.length;
                for (var i = 0; i < length; i++) {
                    if ($scope.trademarks[i].TrademarkId == trademarkId) {
                        trademark = $scope.trademarks[i];
                        break;
                    }
                }
            }
            if (trademark == null) {
                trademark = $scope.trademarks[0];
            }
        }
        if ($scope.trademark != trademark) {
            $scope.trademark = trademark;
            $scope.onTrademarkChange();
        }
    };

    $scope._selectModel = function (modelId) {
        var model = null;
        if (($scope.models != null) && ($scope.models.length > 0)) {
            if (modelId != null) {
                var length = $scope.models.length;
                for (var i = 0; i < length; i++) {
                    if ($scope.models[i].ModelId == modelId) {
                        model = $scope.models[i];
                        break;
                    }
                }
            }
            if (model == null) {
                model = $scope.models[0];
            }
        }
        if ($scope.model = model) {
            $scope.model = model;
        }
    };

    $scope._selectGroup = function (productGroupId) {
        var group = null;
        if (($scope.groups != null) && ($scope.groups.length > 0)) {
            if (productGroupId != null) {
                var length = $scope.groups.length;
                for (var i = 0; i < length; i++) {
                    if ($scope.groups[i].ProductGroupId == productGroupId) {
                        group = $scope.groups[i];
                        break;
                    }
                }
            }
            if (group == null) {
                group = $scope.groups[0];
            }
        }
        if ($scope.group = group) {
            $scope.group = group;
        }
    };

    $scope._setMode = function (itemsMode) {
        if ($scope.itemsMode != itemsMode) {
            var extended = false;
            var items = [];
            var length;
            var selected;
            var item;
            var i;
            if ((itemsMode == 0) && ($scope.trademarks != null)) {
                //Trademarks
                extended = true;
                length = $scope.trademarks.length;
                for (i = 0; i < length; i++) {
                    var trademark = $scope.trademarks[i];
                    selected = (trademark == $scope.trademark);
                    item = { Id: trademark.TrademarkId, Name: trademark.Name, Selected: selected };
                    items.push(item);
                }
            } else if ((itemsMode == 1) && ($scope.models != null)) {
                //Models
                length = $scope.models.length;
                for (i = 0; i < length; i++) {
                    var model = $scope.models[i];
                    selected = (model == $scope.model);
                    item = { Id: model.ModelId, Name: model.DisplayName, Selected: selected };
                    items.push(item);
                }
            } else if ((itemsMode == 2) && ($scope.groups != null)) {
                //Groups
                length = $scope.groups.length;
                for (i = 0; i < length; i++) {
                    var group = $scope.groups[i];
                    selected = (group == $scope.group);
                    item = { Id: group.ProductGroupId, Name: group.Name, Selected: selected };
                    items.push(item);
                }
            }

            var container = $($scope.itemsPointers[itemsMode]).parent();
            var pointerPosition = container.position();
            var pointerTop = pointerPosition.top;
            var pointerLeft = pointerPosition.left;

            var rows = (extended) ? (items.length / 3) : (items.length / 2);
            var expectedHeight = (rows <= 12) ? 200 : 200 + 15 * (rows - 12);
            var middle = expectedHeight / 2;

            var top = (pointerTop - middle);
            var left = (pointerLeft + container.width() + 50);

            $scope.itemsMode = itemsMode;
            $scope.items = items;
            $scope.itemsExtended = extended;
            $scope.itemsContainer.css({ top: top, left: left });
        }
    };

    $scope._setPosition = function() {
        var items = $("div.ui_select");
        var length = items.length;
        for (var i = 0; i < length; i++) {
            var item = $(items[i]);
            var prev = item.prev();
            var position = prev.position();
            var top = position.top + 4;
            var left = position.left;
            item.css("top", top + "px");
            item.css("left", left + "px");
        }
    };

    $scope._attach = function() {
        $("body").on("click", function (event) {
            if ($scope.itemsVisible) {
                var target = $(event.target);
                var parent = target.closest("div.ui_select_list, div.ui_select, div.option");
                if (parent.length == 0) {
                    $scope.itemsVisible = false;
                    $scope.$apply();
                }
            }
        });
        $("body").on("keyup", function (event) {
            if (($scope.itemsVisible) && (event.keyCode == 27)) {
                $scope.itemsVisible = false;
                $scope.$apply();
            }
        });
    };

    $scope.init = function (modelJson) {
        var model = JSON.parse(modelJson);
        $scope.trademarks = model.Trademarks;
        $scope.groups = model.Groups;
        $scope.producer = model.Producer;

        $scope.itemsContainer = $("div.ui_select_list");
        $scope.itemsPointers = $("div.pointer");

        $scope._selectGroup(model.ProductGroupId);
        $scope._selectTrademark(model.TrademarkId);
        $scope._selectModel(model.ModelId);

        $scope._setPosition();
        $scope._attach();
    };

    $scope.onTrademarkChange = function () {
        var models = ($scope.trademark != null) ? $scope.trademark.Models : null;
        if ($scope.models != models) {
            $scope.models = models;
            $scope._selectModel();
        }
    };

    $scope.openItems = function (event, itemsMode) {
        if (($scope.itemsVisible) && ($scope.itemsMode == itemsMode)) {
            $scope.itemsVisible = false;
        } else {
            $scope._setMode(itemsMode);
            $scope.itemsVisible = true;
        }
    };

    $scope.onSelectItem = function (item) {
        if ($scope.itemsMode == 0) {
            $scope._selectTrademark(item.Id);
            $scope._setMode(1);
        } else if ($scope.itemsMode == 1) {
            $scope._selectModel(item.Id);
            $scope._setMode(2);
        } else {
            $scope._selectGroup(item.Id);
            $scope.itemsVisible = false;
        }
    };

    $scope.submit = function () {
        //search/{trademarkId}/{modelId}/{group}/{producerId}
        var trademarkId = ($scope.trademark != null) ? $scope.trademark.TrademarkId : "0";
        var modelId = ($scope.model != null) ? $scope.model.ModelId : "0";
        var groupId = ($scope.group != null) ? $scope.group.ProductGroupId : "0";
        var producerId = ($scope.producer != null) ? $scope.producer.ProducerId : "";
        var url = "/search/" + trademarkId + "/" + modelId + "/" + groupId + "/" + producerId;
        AvtoligaApp.Redirect(url);
    };

}]);