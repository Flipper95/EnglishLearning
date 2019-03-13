$("#myTabContent").on("click", ".close-btn", function (e) {
    e.stopPropagation();
    var el = $(this);
    var parent = $(this).parent(".alert");
    var link = parent.attr("data");
    var id = parent.attr("id");
    $.ajax({
        type: "POST",
        url: link,
        data: { id: id },
        beforeSend: function () {
            return confirm("Ви точно хочете видалити завдання?");
        },
        success: function () {
            parent.remove();
        }
    })
});

$("#myTabContent").on("click", ".fa-edit", function (e) {
    actionClick($(this), e, "#editBody", "#editModal");
});

var actionClick = function (el, event, bodyId, modalId) {
    event.stopPropagation();
    var link = el.attr("data");
    $.ajax({
        type: "GET",
        url: link,
        beforeSend: function () {
            el.addClass("fa-spin disabled");
            el.attr("disabled");
        },
        success: function (data) {
            el.removeClass("fa-spin disabled");
            el.removeAttr("disabled");
            $(bodyId).html(data);
            $(modalId).modal('show');
        }
    })
}

var hideModal = function (formId, modalId) {
    $(formId).submit();
    $(modalId).modal('hide');
    $('body').removeClass('modal-open');
    $(modalId).remove();
    $('.modal-backdrop').remove();
}

var AjaxResultRequest = function (formId, modalId, funcResult) {
    var id = $("#TaskId").prop("value");
    var elem = document.getElementById(formId);
    var data = new FormData(elem);
    $.ajax({
        type: "POST",
        url: $("#" + formId).prop("action"),
        data: data,
        success: function (data) {
            $("#" + modalId).modal('hide');
            $('body').removeClass('modal-open');
            $("#" + modalId).remove();
            $('.modal-backdrop').remove();
            funcResult(data, id);
        },
        cache: false,
        contentType: false,
        processData: false
    })
}

var CheckDone = function (done, taskId) {
    if (done == "True" || done == "true") {
        $("#" + taskId).remove();
    }
}

var UpdateName = function (Name, taskId) {
    if (Name) {
        $("#" + taskId).children(".TaskName").text(Name);
    }
}

$("#editBody").on("click", "#EditSubmit", function () {

    AjaxResultRequest("EditForm", "editModal", UpdateName)
});

$("#myTabContent").on("click", "#create", function (e) {
    actionClick($(this), e, "#createBody", "#createModal");
});

$("#createBody").on("click", "#CreateSubmit", function () {
    AjaxResultRequest("CreateForm", "createModal", function () { });
    setTimeout(function () {
        location.reload();
    }, 2000);
});

$("#myTabContent").on("click", "#userTaskEdit", function (e) {
    actionClick($(this), e, "#editUTBody", "#editUTModal");
});

$("#editUTBody").on("click", "#EditUTSubmit", function () {
    AjaxResultRequest("EditUTForm", "editUTModal", CheckDone);
});

$("#myTabContent").on("click", ".TaskName", function (e) {
    el = $(this);
    e.stopPropagation();
    var link = el.attr("data-show");
    $.ajax({
        type: "GET",
        url: link,
        beforeSend: function () {
            el.addClass("fa-spin disabled");
            el.attr("disabled");
        },
        success: function (data) {
            el.removeClass("fa-spin disabled");
            el.removeAttr("disabled");
            $("#DetailedModal").html(data);
            $("#ShowModal").modal('show');
        }
    })
});