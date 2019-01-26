$("div .question").keypress(function (e) {
    if (e.which == 13) {
        CheckResult();
    }
});

$("div .question").keyup(function () {
    var text = $("#value").val();
    $("#valueText").text(text);
});

$("div .questionBtn").on("click", function () { CheckResult(); });

var CheckResult = function () {
    var text = $("#value").val();

    $.ajax({
        type: "GET",
        url: "CheckResult",
        data: { value: text },
        success: function (data) {
            AjaxResult(data);
        }
    });

    $("div .question, div .questionBtn").prop("disabled", true);
    $("div .question").addClass("active");
    $("#NextBtn").removeAttr("hidden");
}

var AjaxResult = function (result) {
    var el = $(".active");
    if (result.toLowerCase() == "true") {
        el.addClass("bg-teal text-light");
    }
    else {
        el.addClass("bg-danger text-light");
        $.ajax({
            type: "GET",
            url: "GetAnswer",
            success: function (data) {
                $("#answer").removeAttr("hidden").text(String(data))
            }
        });
    }
}