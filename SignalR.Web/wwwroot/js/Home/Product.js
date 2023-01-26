$(document).ready(() => {
    var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Information).withAutomaticReconnect([1000, 3000, 5000, 10000]).withUrl("https://localhost:7088/ProductHub").build();
    let connectionQ = connection.q;
    $("#conProductHubStatus").text(connection.q);
    $("#loadingIconProductHub").hide();
   
    $("#btnProductHubCoonection").click(() => {       
        //=========== ProductHub Bağlantısı ===================
        $("#conProductHubStatus").text(connection.q);

        connection.start().then((e) => {
            $("#conProductHubStatus").text(connection.q);
            $("#loadingIconProductHub").hide();
            connectionQ = connection.q;
            connection.on("ReceiveClientCount", (clientCount) => {                
                $("#ClientProduthubCount").text(clientCount);
            });

        }).catch((err) => {
            console.log(err);
        });
        
    });

    $("#btnProduct").click(() => {
        debugger;
        if (connectionQ == "Connected") {
            let product = { "Id": 1, "Name": "Kalem 1", "Price": 100, "Stock": 200 };
            connection.invoke("SendProductName", product);
        }
       
    });

    connection.on("ReceiveProduct", (product) => {
        debugger;
        $("#getProduct").html(JSON.stringify(product));
    });

    
})
