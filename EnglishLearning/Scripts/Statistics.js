$(function () {
    $('tbody tr').hover(function () {
        $(this).toggleClass('clickable');
    }).on('click', function () {
        location.href = $(this).find('td:first div').attr("data");
    });
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