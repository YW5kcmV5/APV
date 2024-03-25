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

AvtoligaApp.FormatUrl = function (action, ssl) {
    var prefix = AvtoligaApp.SiteRoot;
    while (prefix.endsWith("/")) {
        prefix = prefix.substr(0, prefix.length - 1);
    }
    while (action.startsWith("/")) {
        action = action.substr(1);
    }
    var url = prefix + "/" + action;
    if (ssl === true) {
        url = url.replace("http:", "https:");
    }
    return url;
};

AvtoligaApp.ShowGallery = function (images, apply) {
    if (AvtoligaApp.ShowGalleryController != null) {
        AvtoligaApp.ShowGalleryController.show(images, apply);
    }
};

AvtoligaApp.reload = function () {
    location.reload();
};

AvtoligaApp.alert = function (message, error) {
    var style = (error == true) ? "alarm-body alarm-error" : "alarm-body";
    message = $.trim(message);
    var header = "";
    message = message.replace("\\r", "\r");
    message = message.replace("\\n", "\n");
    message = message.replace("\r\n", "\n");
    message = message.replace("\r\n", "\n");
    message = message.replace("\n", "<BR>");
    var index = message.indexOf("<BR>");
    if (index != -1) {
        header = "<h4>" + message.substring(0, index) + "</h4>";
        message = message.substring(index + 4, message.length);
    }

    $.magnificPopup.open({
            items: {
                src: "<div class='alarm'><div class='" + style + "'>" + header + "<label>" + message + "</label></div><div class='alarm-footer'><input type='button' class='submit' value='Ok' onclick='$.magnificPopup.close();'></div></div>",
            },
            type: 'inline',
            showCloseBtn: false,
            enableEscapeKey: true,
            closeOnBgClick: false,
            callbacks: {
                open: function() {
                    $(".mfp-content").css("width", "auto");
                },
            }
        });
};

AvtoligaApp.showFlayout = function (header, message, error) {
    var showTimeoutInMlsec = 5000;
    var fadeOutTimeoutInMlsec = 1000;

    var style = (error == true) ? "flyout error" : "flyout";
    message = $.trim(message);
    message = message.replace("\\r", "\r");
    message = message.replace("\\n", "\n");
    message = message.replace("\r\n", "\n");
    message = message.replace("\r\n", "\n");
    message = message.replace("\n", "<BR>");
    if (header == null) {
        var index = message.indexOf("<BR>");
        if (index != -1) {
            header = message.substring(0, index);
            message = message.substring(index + 4, message.length);
        } else {
            header = "";
        }
    }
    var html = "<div class='" + style + "'><span>x</span><span>" + header + "</span><span>" + message + "</span></div>";

    var flayout = $(".flyout");
    flayout.remove();
    flayout = $(html);
    flayout.appendTo("body");

    var timer = null;
    var closeHandler = function () {
        if (timer != null) {
            clearTimeout(timer);
            timer = null;
        }
        flayout.fadeOut(fadeOutTimeoutInMlsec, "linear", function () {
            flayout.remove();
        });
    };

    timer = setTimeout(closeHandler, showTimeoutInMlsec);
    flayout.find("span:first").one("click", closeHandler);
};

AvtoligaApp.confirm = function (message, action) {
    if ((message == null) || (typeof message != "string") || (action == null) || (typeof action != "function")) {
        return;
    }

    message = $.trim(message);
    var headerTitle = "Подтверждение.";
    var cancelTitle = "Нет";
    var okTitle = "Да";
    message = message.replace("\\r", "\r");
    message = message.replace("\\n", "\n");
    message = message.replace("\r\n", "\n");
    message = message.replace("\r\n", "\n");
    message = message.replace("\n", "<BR>");
    var index = message.indexOf("<BR>");
    if (index != -1) {
        headerTitle = message.substring(0, index);
        message = message.substring(index + 4, message.length);
    }
    var html = $("<div class='confirm'><div class='confirm-body'><h4>" + headerTitle + "</h4><label>" + message + "</label></div><div class='confirm-footer'><input type='button' value='" + cancelTitle + "' onclick='$.magnificPopup.close();'><input type='button' class='submit' value='" + okTitle + "'></div></div>");
    var buttonOk = html.find("input.submit");

    var actionEvent = function () {
        try {
            action();
        } catch (e) {
        }
        $.magnificPopup.close();
    };
    buttonOk.one("click", actionEvent);

    $.magnificPopup.open({
        items: {
            src: html,
        },
        type: 'inline',
        showCloseBtn: false,
        enableEscapeKey: true,
        closeOnBgClick: false,
        callbacks: {
            open: function () {
                $(".mfp-content").css("width", "auto");
            },
        }
    });
};

AvtoligaApp.login = function (action) {
    if ((action == null) || (typeof action != "function")) {
        return;
    }

    var html = $("<div class='confirm'><div class='confirm-body form'><h4>Авторизация пользователя.</h4><label for='login_username'>Имя пользователя<sup>*</sup></label><input type='text' id='login_username' name='username' /><br/><label for='login_password'>Пароль<sup>*</sup></label><input type='text' id='login_password' name='password' /></div><div class='confirm-footer'><input type='button' class='submit' value='Login'></div></div>");
    var buttonOk = html.find("input.submit");
    var usernameInput = html.find("#login_username");
    var passwordInput = html.find("#login_password");

    var onChangeEvent = function (item) {
        var target = $(item.target);
        var value = target.val();
        item = $(target.prev().children()[0]);
        if ((value == null) || (value.length == 0)) {
            item.show();
        } else {
            item.hide();
        }
    };

    usernameInput.on("change keyup", onChangeEvent);
    passwordInput.on("change keyup", onChangeEvent);

    var actionEvent = function () {
        try {
            var username = usernameInput.val();
            var password = passwordInput.val();
            if ((username == null) || (username.length == 0)) {
                usernameInput.focus();
            } else if ((password == null) || (password.length == 0)) {
                passwordInput.focus();
            } else {
                $.magnificPopup.close();
                action(username, password);
            }
        } catch (e) {
        }
    };
    buttonOk.on("click", actionEvent);

    $.magnificPopup.open({
        items: {
            src: html,
        },
        type: 'inline',
        showCloseBtn: false,
        enableEscapeKey: true,
        closeOnBgClick: false,
        focus: "#login_username",
        callbacks: {
            open: function () {
                $(".mfp-content").css("width", "auto");
            },
        }
    });
};


