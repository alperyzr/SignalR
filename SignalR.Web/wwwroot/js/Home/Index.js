//$(document).ready(function(){---}) ile aynı kullanım
$(document).ready(() => {
    debugger;
    //Hub bağlantıı için kullanılır (API url üzerindeki hub methoduna gönderilir)
    var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Debug).withUrl("https://localhost:7088/MyHub").build();
   
    //BAĞLANTI DURUMUNU EKRANA YAZDIRMA
    $("#conStatus").text(connection.q);
    /* $("#conStatus").text(connection.q);*/

    //Bağlantı start verilir. Then methodu success, catch methodu ise fail durumunda karşılayacağımız method olacak
    connection.start().then((e) => {       
        $("#conStatus").text(connection.q);
    }).catch((err) => {       
        console.log(err);
    });

    $("#conStatus").text(connection.q);

    $("#btnNameSave").click(function () {

        //Server üzerindeki methodları çağırmak için kullanılır
        //txtName i parametre olarak gönderiyoruz
        connection.invoke("SendName", $("#txtName").val()).then(() => {
            
        }).catch((err) => {           
            console.log(err);
        });
    });

    //Server tarafındaki methodlara subscribe olabilmek için kullanılır
    //Aynı zamanda İsim Ekle butonuna tıklanıldıktan sonra inputtaki text te girilen name i methoda gönderir 
    connection.on("ReceiveName", (name) => {      
        $("#namesList").append(`<li class="list-group-item"> ${name}</li>`); 
    });
});