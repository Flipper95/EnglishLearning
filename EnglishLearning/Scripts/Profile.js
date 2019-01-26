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
            //el.prop("data-dismiss", "alert");
            //el.prop("aria-label", "Close");
            //el.parent(".alert").alert('close');
            parent.remove();
            //parent.alert('close');
        }
    })
});

$("#myTabContent").on("click", ".fa-edit", function (e) {
    //e.stopPropagation();
    //var id = $(this).parent(".alert").attr("data-target");
    //var link = $(this).attr("data");
    //var el = $(this);
    ////$(id).modal('hide');
    //$.ajax({
    //    type: "GET",
    //    url: link,
    //    //data: { id: id },
    //    beforeSend: function () {
    //        el.addClass("fa-spin disabled");
    //        el.attr("disabled");
    //    },
    //    success: function (data) {
    //        el.removeClass("fa-spin disabled");
    //        el.removeAttr("disabled");
    //        $("#editBody").html(data);
    //        $("#editModal").modal('show');
    //    }
    //})
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
            //if (data) {
            //    $("#" + id).children(".TaskName").text(data);
            //}
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
    //$("#EditForm").submit();
    //$("#editModal").modal('hide');
    //$('body').removeClass('modal-open');
    //$("#editModal").remove();
    //$('.modal-backdrop').remove();
    ////$("#editModal").remove();
    //var id = $("#TaskId").prop("value");
    //var elem = document.getElementById("EditForm");
    //var data = new FormData(elem);
    //data.append("file", $("#file")[0].files[0]);
    //data.append("Name", $("#Name")[0].prop("text"));
    //data.append("Description", $("#Description")[0].prop("text"));
    //data.append("Text", $("#Text")[0].prop("text"));
    //data.append("Group", $("#Group")[0].prop("text"));
    //data.append("DocumentPath", $("#DocumentPath")[0].prop("text"));
    //data.append("Difficult", $("#Difficult")[0].prop("value"));
    //$.ajax({
    //    type: "POST",
    //    url: $("#EditForm").prop("action"),
    //    data: data,
    //    success: function (data) {
    //        $("#editModal").modal('hide');
    //        $('body').removeClass('modal-open');
    //        $("#editModal").remove();
    //        $('.modal-backdrop').remove();
    //        if (data) {
    //            $("#"+id).children(".TaskName").text(data);
    //        }
    //    },
    //    cache: false,
    //    contentType: false,
    //    processData: false
    //})

    AjaxResultRequest("EditForm", "editModal", UpdateName)
    //hideModal("#EditForm", "#editModal");
});

$("#myTabContent").on("click", "#create", function (e) {
    //e.stopPropagation();
    //var link = $(this).attr("data");
    //var el = $(this);

    //$.ajax({
    //    type: "GET",
    //    url: link,
    //    beforeSend: function () {
    //        el.addClass("fa-spin disabled").attr("disabled", true);
    //    },
    //    success: function (data) {
    //        el.removeClass("fa-spin disabled").removeAttr("disabled");
    //        $("#createBody").html(data);
    //        $("#createModal").modal('show');
    //    }
    //})
    actionClick($(this), e, "#createBody", "#createModal");
});

$("#createBody").on("click", "#CreateSubmit", function () {
    //$("#CreateForm").submit();
    //$("#createModal").modal('hide');
    //$('body').removeClass('modal-open');
    //$("#createModal").remove();
    //$('.modal-backdrop').remove();
    //var id = $("#TaskId").prop("value");
    //var elem = document.getElementById("CreateForm");
    //var data = new FormData(elem);
    //$.ajax({
    //    type: "POST",
    //    url: $("#CreateForm").prop("action"),
    //    data: data,
    //    success: function (data) {
    //        $("#createModal").modal('hide');
    //        $('body').removeClass('modal-open');
    //        $("#createModal").remove();
    //        $('.modal-backdrop').remove();
    //        //if (data) {
    //        //    $("#" + id).children(".TaskName").text(data);
    //        //}
    //    },
    //    cache: false,
    //    contentType: false,
    //    processData: false
    //})
    AjaxResultRequest("CreateForm", "createModal", function () { });
    //hideModal("#CreateForm", "#createModal");
    setTimeout(function () {
        location.reload();
    }, 2000);
    // or can use ajax to reload, but not sure it helps
    //location.reload();
});

$("#myTabContent").on("click", "#userTaskEdit", function (e) {
    //e.stopPropagation();
    //var link = $(this).attr("data");
    //var el = $(this);

    //$.ajax({
    //    type: "GET",
    //    url: link,
    //    beforeSend: function () {
    //        el.addClass("fa-spin disabled").attr("disabled", true);
    //    },
    //    success: function (data) {
    //        el.removeClass("fa-spin disabled").removeAttr("disabled");
    //        $("#editUTBody").html(data);
    //        $("#editUTModal").modal('show');
    //    }
    //})
    actionClick($(this), e, "#editUTBody", "#editUTModal");
});

$("#editUTBody").on("click", "#EditUTSubmit", function () {
    //$("#EditUTForm").submit();
    //$("#editUTModal").modal('hide');
    //$('body').removeClass('modal-open');
    //$("#editUTModal").remove();
    //$('.modal-backdrop').remove();
    //var id = $("#TaskId").prop("value");
    //var elem = document.getElementById("EditUTForm");
    //var data = new FormData(elem);
    //$.ajax({
    //    type: "POST",
    //    url: $("#EditUTForm").prop("action"),
    //    data: data,
    //    success: function (data) {
    //        $("#editUTModal").modal('hide');
    //        $('body').removeClass('modal-open');
    //        $("#editUTModal").remove();
    //        $('.modal-backdrop').remove();
    //        if (data == "True" || data == "true") {
    //            $("#" + id).remove();//.children(".TaskName").text(data);
    //        }
    //    },
    //    cache: false,
    //    contentType: false,
    //    processData: false
    //})
    AjaxResultRequest("EditUTForm", "editUTModal", CheckDone);
    //hideModal("#EditUTForm", "#editUTModal");
    //setTimeout(function () {
    //    location.reload();
    //}, 2000);
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