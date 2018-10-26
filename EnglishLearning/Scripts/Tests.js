$(".quest_link").on("click", function () {
    GetNewAnswers($(this));
    });

$(".next_link").on("click", function () {
    GetNewAnswers($(this));
});

var GetNewAnswers = function (el) {
    var link = el.attr('data');

    var answers = [];
    answers = GetChecked();

    var choosenId = el.attr("id");
    var prevId = $(".act").attr("id");
    var nextId = $(".quest_link#" + choosenId).next(".quest_link").attr("id");

    if (answers.length > 0) {
        $(".act").addClass("btn-purple").removeClass("act btn-info");
    }
    else $(".act").addClass("btn-secondary").removeClass("act btn-info");
    $(".quest_link#" + choosenId).addClass("act btn-info").removeClass("btn-secondary btn-purple");

    $.ajax({
        type: "POST",
        url: link,
        data: { prevId: prevId, nextId: nextId, choosenId: choosenId, answers: answers },
        success: function (partialViewResult) {
            $("#answer_content").html(partialViewResult);
            $(".next_link").on("click", function () { GetNewAnswers($(this)); });
            $("#EndTest").on("click", function () { EndTest(); });
        }
    });
}

var EndTest = function () {
    var link = $("#EndTest").attr('data');
    var id = $(".act").attr("id");
    var answers = GetChecked();
    $.ajax({
        type: "POST",
        url: link,
        data: { questId: id, answers: answers },
        success: function () {
            document.getElementById('End').click();
        }
    });
}

var GetChecked = function () {
    var answers = [];
    $(".answer").each(function () {
        if ($(this).is(":checked")) {
            answers.push(parseInt($(this).attr("id")));
        }
    });
    return answers;
}