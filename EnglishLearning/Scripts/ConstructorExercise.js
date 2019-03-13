$(".questionBtn").on("click", function () {
    $(".questionBtn").prop("hidden", true);
    $(".letter").off("click").addClass("disabled").addClass("active").prop("draggable", false);
    $("#NextBtn").removeAttr("hidden");

    var text = $("#value").text();
    $.ajax({
        type: "POST",
        url: "CheckResult",
        data: { id: $("#value").attr("data"), value: text },
        success: function (data) {
            AjaxResult(data);
        }
    });
});

var AjaxResult = function (result) {
    var el = $(".active");
    el.removeClass("btn-purple");
    if (result.toLowerCase() == "true") {
        el.addClass("btn-success");
    }
    else {
        el.addClass("btn-danger");
        $.ajax({
            type: "GET",
            url: "GetAnswer",
            success: function (data) {
                $("#answer").removeAttr("hidden").text(String(data))
            }
        });
    }
}

$(".letter").on("click", function () {
    var id = $(this).parent().attr("id");
    if (id === "value") $("#question").append($(this));
    else $("#value").append($(this));
})

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

function drop(ev, el) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    el.appendChild(document.getElementById(data));
}