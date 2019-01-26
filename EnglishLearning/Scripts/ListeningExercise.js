//$("div .question").on("click", function () {
//    $("div .question").addClass("disabled");
//    $(this).addClass("active");
//    $("#NextBtn").removeAttr("hidden")//.removeClass("disabled").off("click");//.prop("disabled", false).off("click");
//});

$("div .question").keypress(function (e) {
    if (e.which == 13) {
        CheckResult();
    }
});

$("div .questionBtn").on("click", function () { CheckResult(); });

var CheckResult = function () {
    var text = $("#value").val();

    $.ajax({
        type: "POST",
        url: "CheckResult",
        data: { id: $("#value").attr("data"), value: text
},
    success: function (data) {
        AjaxResult(data);
                    }
                });

$("div .question, div .questionBtn").prop("disabled", true);
$("div .question").addClass("active");
$("#NextBtn").removeAttr("hidden");
        }

//var ShowNext = function () {
//    //$(this).addClass("active");
//    $("#NextBtn").removeAttr("hidden");
//}

var AjaxResult = function (result) {
    var el = $(".active");
    //el.removeClass("btn-purple");
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
        //$("#answer").removeAttr("hidden").text(@Model[ViewBag.Index].Word1);
    }
}