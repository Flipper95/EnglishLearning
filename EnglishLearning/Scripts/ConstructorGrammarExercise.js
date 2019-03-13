
$(".questionBtn").on("click", function () {
    $(".questionBtn").prop("hidden", true);
    $("#NextBtn").removeAttr("hidden");

    var text = $("#value").text();
    text = text.trimRight();
    $.ajax({
        type: "GET",
        url: "CheckResult",
        data: { value: text },
        success: function (data) {
            AjaxResult(data);
        }
    });
});

var AjaxResult = function (result) {
    var el = $("#answer");
    el.removeAttr("hidden");
    if (result.toLowerCase() == "true") {
        el.removeClass("alert-warning").addClass("alert-success");
        el.text($("#value").text());
    }
    else {
        el.removeClass("alert-warning").addClass("alert-danger");
        $.ajax({
            type: "GET",
            url: "GetAnswer",
            success: function (data) {
                el.text(String(data))
            }
        });
    }
}

$("#value").on("click", ".letter:last-child", function () {
    WordClick($(this));
});

$("#question").on("click", ".letter", function () {
    WordClick($(this));
});

var WordClick = function (el) {
    var id = el.parent().attr("id");
    var wordIndex = el.parent().attr('data-id');
    var link = el.parent().attr('data');
    if (id === "value") {
        $("#question").append(el);
        wordIndex = parseInt(wordIndex) - 1;
    }
    else {
        $("#value").append(el);
        wordIndex = 1 + parseInt(wordIndex);
    }
    $("#value").off("click", ".letter:not(:last-child)");
    $("#value .letter").addClass("disabled").prop("draggable", false);
    $("#value .letter:last-child").removeClass("disabled").prop("draggable", true);
    var sentenceIndex = $("#sentence").attr('data');
    $.ajax({
        type: "GET",
        url: link,
        data: { sentenceIndex: sentenceIndex, wordIndex: wordIndex },
        success: function (partialViewResult) {
            $("#question").html(partialViewResult);
        }
    });
}