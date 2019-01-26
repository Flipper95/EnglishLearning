//$("#NextBtn").on("click", function () {
//    var text = $("#value").text();
//    text = text.trimRight();
//    document.getElementById("result").value = text;
//    //$("#result").val(text);
//});
$(".questionBtn").on("click", function () {
    $(".questionBtn").prop("hidden", true);
    //$(".letter").off("click").addClass("disabled").addClass("active").prop("draggable", false);
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
    //var el = $(".active");
    //el.removeClass("btn-purple");
    var el = $("#answer");
    el.removeAttr("hidden");
    if (result.toLowerCase() == "true") {
        el.removeClass("alert-warning").addClass("alert-success");
        el.text($("#value").text());
    }
    else {
        //el.addClass("btn-danger");
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
    //var text = $(this).text();
    if (id === "value") {
        $("#question").append(el);
        wordIndex = parseInt(wordIndex) - 1;
        //var newText = document.getElementById("result").value;
        //newText = newText.replace(new RegExp(text + '$'), '');
        //document.getElementById("result").value = newText;
    }
    else {
        $("#value").append(el);
        wordIndex = 1 + parseInt(wordIndex);
        //document.getElementById("result").value = text;
    }
    $("#value").off("click", ".letter:not(:last-child)");
    //$("#value").on("click", ".letter:last-child", function () {
    //    WordClick($(this));
    //});
    $("#value .letter").addClass("disabled").prop("draggable", false);
    $("#value .letter:last-child").removeClass("disabled").prop("draggable", true);
    var sentenceIndex = $("#sentence").attr('data');
    $.ajax({
        type: "GET",
        url: link,
        data: { sentenceIndex: sentenceIndex, wordIndex: wordIndex },
        success: function (partialViewResult) {
            $("#question").html(partialViewResult);
            //$(".letter").on("click", function () {
            //    WordClick($(this));
            //});
            //$(".next_link").on("click", function () { GetNewAnswers($(this)); });
            //$("#EndTest").on("click", function () { EndTest(); });
        }
    });
}

        //function allowDrop(ev) {
        //    ev.preventDefault();
        //}

        //function drag(ev) {
        //    ev.dataTransfer.setData("text", ev.target.id);
        //}

        //function drop(ev, el) {
        //    ev.preventDefault();
        //    var data = ev.dataTransfer.getData("text");
        //    WordClick($(String(data)));
        //    //var element = document.getElementById(data);
        //    // WordClick(element);
        //    //el.appendChild(document.getElementById(data));
        //}