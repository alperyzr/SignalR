//$(document).ready(function(){---}) ile aynı kullanım
$(document).ready(() => {

    //#region Bağlantı Kısmı

    //Hub bağlantıı için kullanılır (API url üzerindeki hub methoduna gönderilir)
    //configureLogging(signalR.LogLevel.Debug) console ekranına logları yazmak için kullanılır
    //withUrl belirtilen URl e istek atması için kullanılır
    //build bağlantıyı inşa etmek için kullanılır
    //withAutomaticReconnect() bağlantı kesilirse belirli periotlarlarla tekrar bağlanmaya çalışmasını sağlar
    var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Debug).withAutomaticReconnect().withUrl("https://localhost:7088/MyHub").build();

    //BAĞLANTI DURUMUNU EKRANA YAZDIRMA
    $("#conStatus").text(connection.q);
    /* $("#conStatus").text(connection.q);*/

    //Bağlantı start verilir. Then methodu success, catch methodu ise fail durumunda karşılayacağımız method olacak
    connection.start().then((e) => {
        $("#conStatus").text(connection.q);
        $("#loadingIcon").hide();
    }).catch((err) => {
        console.log(err);
    });

    $("#conStatus").text(connection.q);

    //#endregion
   
    //#region Bağlantı Sorunlaru
    //Girişten itibaren bağlantı yoksa belirli aralıklarla bağlanmaya çalışması için
    function Start() {
        connection.start().then((e) => {
            $("#conStatus").text(connection.q);
            $("#loadingIcon").hide();
        }).catch((err) => {
            console.log(err);
            //Başlangıçta bağlantıda bir sorun varsa 2sn sonra tekrar dener
            setTimeout(() => Start(), 2000)
        });
    }

    //Bu kısımlar bağlantı arada koparsa devreye girecektir ve tekrar bağlanmaya çalışığ bağlanamazsa kapatacaktır
    //Bağlanmya çalışma, bağlanma ve çbağlantı kapatılma durumlarında loader gösterip console hataları yazdırır
    connection.onreconnecting(err => {
        $("#loadingIcon").show();
        $("#conStatus").text(connection.q);
        console.log("onrecoonnection: " + err);
    });
    connection.onreconnected(connectionId => {
        $("#loadingIcon").hide();
        $("#conStatus").text(connection.q);
        console.log("ConnectionId: " + connectionId);
    });
    connection.onclose(() => {
        $("#loadingIcon").hide();
        $("#conStatus").text(connection.q); 
        // 4 denemesinde Bağlantı tamamen kapandıktan sonra tekrar başlatılması sağlanabilir
        Start();
    });

    //#endregion

    $("#btnNameSave").click(function () {

        //Server üzerindeki methodları çağırmak için kullanılır
        //txtName i parametre olarak gönderiyoruz
        connection.invoke("SendName", $("#txtName").val()).then(() => {
            
        }).catch((err) => {           
            console.log(err);
        });
    });

    //#region Methodlara Subscribe Olma

    //Server tarafındaki methodlara subscribe olabilmek için kullanılır
    //Aynı zamanda İsim Ekle butonuna tıklanıldıktan sonra inputtaki text te girilen name i methoda gönderir 
    connection.on("ReceiveName", (name) => {      
        $("#namesList").append(`<li class="list-group-item"> ${name}</li>`); 
    });

    connection.on("ReceiceClientCount", (clientCount) => {
        $("#ClientCount").text(clientCount);
    });
    connection.on("Notify", (countText) => {        
        $("#Notify").html(`<div class="alert alert-success">${countText}</div>`);
    });
    //#endregion
});