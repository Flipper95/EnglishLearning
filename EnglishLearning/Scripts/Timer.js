var diff = $("#timer").attr("data");
var count = new Date(new Date().getTime() + diff * 60000).getTime();

var x = setInterval(function () {

    var now = new Date().getTime();

    var distance = count - now;

    var min = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var sec = Math.floor((distance % (1000 * 60)) / 1000);

    document.getElementById("timer").innerHTML = min + "м " + sec + "с ";

    if (distance < 0) {
        clearInterval(x);
        $("#timer").addClass("text-danger");
        document.getElementById("timer").innerHTML = "Час вийшов. Результат не буде зараховано";
    }
}, 1000);