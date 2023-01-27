$(document).ready(() => {
    var covidChartList = new Array();
    covidChartList.push(["Tarih", "İstanbul", "Ankara", "İzmir", "Antalya", "Muğla"]);

    var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7088/CovidHub").build();

    connection.start().then((e) => {
        connection.invoke("GetCovidListAsync");
    }).catch((err) => {
        debugger;
        console.log(err);
    });


    connection.on("ReceiveCovidListAsync", (covidList) => {
        covidChartList = covidChartList.splice(0, 1);
        covidList.forEach((item) => {
            covidChartList.push([item.covidDate, item.counts[0], item.counts[1], item.counts[2], item.counts[3], item.counts[4]])
        });
        debugger;
        google.charts.load('current', { 'packages': ['corechart'] });
        google.charts.setOnLoadCallback(drawChart);
    });

    function drawChart() {       
        var data = google.visualization.arrayToDataTable(covidChartList);

        var options = {
            title: 'Covid Chart',
            curveType: 'function',
            legend: { position: 'bottom' }
        };

        var chart = new google.visualization.LineChart(document.getElementById('curve_chart'));

        chart.draw(data, options);
    }

});

