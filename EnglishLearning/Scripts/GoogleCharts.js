google.charts.load('current', { 'packages': ['corechart', 'bar'] });
google.charts.setOnLoadCallback(drawStuff);

function drawStuff() {

    var chartDiv = document.getElementById('columnchart2_material');

    var from = $("#fromDate").val();
    var to = $("#toDate").val();

    var jsonObject = {
        "xName": "Дата",
        "colName": "Кількість тестів",
        "col2Name": "Кількість вивчених слів",
        "from": from,
        "to": to
    }

    var jsonData = $.ajax({
        url: $("#ChartCountLink").attr("href"),
        data: jsonObject,
        dataType: "json",
        async: false
    }).responseText;

    var data = new google.visualization.DataTable(jsonData);

    var materialOptions = {
        chart: {
            title: 'Прогрес',
            subtitle: 'кількість тестів зліва, кількість слів праворуч'
        },
        series: {
            0: { axis: 'tests' },
            1: { axis: 'words' }
        },
        axes: {
            y: {
                tests: { label: 'тестів' },
                words: { side: 'right', label: 'слів' }
            }
        }
    };

    var materialChart = new google.charts.Bar(chartDiv);
    materialChart.draw(data, google.charts.Bar.convertOptions(materialOptions));

    jsonObject = {
        "xName": "Кількість тестів",
        "colName": "% успішності",
    }

    jsonData = $.ajax({
        url: $("#ChartTestLink").attr("href"),
        data: jsonObject,
        dataType: "json",
        async: false
    }).responseText;

    //hAxis: {
    //    ticks: [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100]
    //},
    var options = {
        legend: { position: 'none' },
    };
    data = new google.visualization.DataTable(jsonData);
    var chart = new google.visualization.Histogram(document.getElementById('histogramchart_material'));
    chart.draw(data, google.charts.Bar.convertOptions(options));
};