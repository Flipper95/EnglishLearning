﻿$("div a.question").on("click", function () {
    $("div a.question").addClass("disabled");
    $(this).addClass("active");
    $("#NextBtn").removeAttr("hidden")
});

var AjaxResult = function (result) {
    var el = $(".active");
    el.removeClass("btn-purple");
    if (result.toLowerCase() == "true") {
        el.addClass("btn-success");
    }
    else {
        el.addClass("btn-danger");
    }
}