var BuildUrl = function () {
    var searchData = $("#searchData").val();
    var urlSize = $("#rowsperpage option:selected").data('url');
    var url;
    if (searchData) {
        var urlSearch = $("#search").data('url');
        urlSize = urlSize.substring(urlSize.indexOf("?") + 1);
        urlSearch += "?search=" + searchData;
        url = urlSearch + "&" + urlSize;
    }
    else {
        url = urlSize;
    }
    document.location = url;
}

var WordInLearningChange = function (data) {
    var clas = $("#cb_" + data).attr('class');
    var val1 = "badge badge-secondary";
    var val2 = "badge badge-success";
    if (clas === val1) {
        $("#cb_" + data).attr('class', val2);
        $("#cb_" + data).text("На вивченні");
    }
    else {
        $("#cb_" + data).attr('class', val1);
        $("#cb_" + data).text("Додати слово");
    }
}

var MessageShow = function (data) {
    $("#messages").html("<div class='alert alert-warning alert-dismissible fade show' role='alert'>"
        + "Ваш словник було успішно змінено"
        + "<button type = 'button' class= 'close' data-dismiss='alert' aria-label='Close' >"
        + "<span aria-hidden='true'>&times;</span></button ></div >");
    var link = $("#cb_" + data).parent().attr("href");
    var arr = link.split('check=');
    var replace = arr[1].toLowerCase() === "true" ? "false" : "true";
    var newLink = link.replace(arr[1], replace);
    $("#cb_" + data).parent().attr("href", newLink);
}

var setPaginationStyle = function () {
    $("tfoot a").addClass('btn btn-secondary mb-1');
    $("tfoot td")
        .contents()
        .filter(function () {
            if (this.nodeType === 3 && this.length > 1) {
                return this.nodeType
            }
        })
        .wrap('<span class="btn btn-primary mx-1 mb-1" />');
}

var setGridChangeListener = function () {

    var targetNode = document.getElementById('gridChange');
    var config = { attributes: false, childList: true, subtree: false };
    var callback = function (mutationsList, observer) {
        for (var mutation of mutationsList) {
            if (mutation.type == 'childList') {
                setPaginationStyle();
            }
        }
    };
    var observer = new MutationObserver(callback);
    observer.observe(targetNode, config);
}

window.onload =
    $(document).ready(function () {

        setPaginationStyle();

        var value = $("#rowsperpage").attr("val");
        $("#rowsperpage").val(value);

        $("#rowsperpage").change(function () { BuildUrl(); });
        //    function () {
        //    var url = $("#rowsperpage option:selected").data('url');
        //    document.location = url;
        //    }
        //);

        $("#search").on("click", function () { BuildUrl(); });

        $("#searchData").change(function () {
            if (!$("#searchData").val()) { BuildUrl(); }
        });

        setGridChangeListener();
    }
    )