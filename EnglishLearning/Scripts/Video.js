$(".video_select").on("click", ".video", function (e) {
    var child = $(this).find(".embed-responsive");
    var id = child.prop("id");
    if ($('#' + id).children().length == 0) {
        var link = $(this).attr("data");
        $.ajax({
            type: "GET",
            url: link,
            success: function (data) {
                $("#" + id).html(data);
            }
        });
    }
});