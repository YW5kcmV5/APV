var AvtoligaApp = angular.module('AvtoligaApp', []);

AvtoligaApp.SiteRoot = "";
AvtoligaApp.ShowGalleryController = null;

AvtoligaApp.Redirect = function (action) {
    var top = window;
    while ((top.parent != null) && (top.parent != top)) {
        top = top.parent;
    }
    var url = AvtoligaApp.FormatUrl(action);
    top.location = url;
};

AvtoligaApp.FormatUrl = function (action) {
    var url = AvtoligaApp.SiteRoot + action;
    url = url.replace("//", "/");
    return url;
};

AvtoligaApp.ShowGallery = function (images, apply) {
    if (AvtoligaApp.ShowGalleryController != null) {
        AvtoligaApp.ShowGalleryController.show(images, apply);
    }
};