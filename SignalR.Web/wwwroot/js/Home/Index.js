//$(document).ready(function(){---}) ile aynı kullanım
$(document).ready(() => {

    //#region Bağlantı Kısmı

    //Hub bağlantıı için kullanılır (API url üzerindeki hub methoduna gönderilir)
    //configureLogging(signalR.LogLevel.Debug) console ekranına logları yazmak için kullanılır
    //withUrl belirtilen URl e istek atması için kullanılır
    //build bağlantıyı inşa etmek için kullanılır
    //withAutomaticReconnect() bağlantı kesilirse belirli periotlarlarla tekrar bağlanmaya çalışmasını sağlar. Şuan 1sn, 3sn, 5sn ve 10sn periotlarında 4 defa bağlanmaya çalışır.
    var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Information).withAutomaticReconnect([1000, 3000, 5000, 10000]).withUrl("https://localhost:7088/MyHub").build();

    //BAĞLANTI DURUMUNU EKRANA YAZDIRMA
    $("#conStatus").text(connection.q);

    //Bağlantı start verilir. Then methodu success, catch methodu ise fail durumunda karşılayacağımız method olacak
    connection.start().then((e) => {
        $("#conStatus").text(connection.q);
        $("#loadingIcon").hide();

        //Sayfa ilk açılırken static listede veri varsa çağırmak için kullanılır       
        connection.invoke("GetNames");
        connection.invoke("GetAllNamesAsync");
        connection.invoke("SetTeamCount");
    }).catch((err) => {
        console.log(err);
    });

    //#endregion

    //#region Bağlantı Sorunlaru
    //Girişten itibaren bağlantı yoksa belirli aralıklarla bağlanmaya çalışması için
    function Start() {
        connection.start().then((e) => {
            $("#conStatus").text(connection.q);
            $("#loadingIcon").hide();

            //Sayfa ilk açılırken static listede veri varsa çağırmak için kullanılır
            connection.invoke("GetNames");
            connection.invoke("GetAllNamesAsync");
            connection.invoke("SetTeamCount");
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

    //#region Buton Clickler
    $("#btnNameSave").click(function () {

        //Server üzerindeki methodları çağırmak için kullanılır
        //txtName i parametre olarak gönderiyoruz
        connection.invoke("SendName", $("#txtName").val()).then(() => {

        }).catch((err) => {
            console.log(err);
        });
    });

    $("#btnNameTeam").click(() => {
        let name = $("#txtName").val();
        let teamId = $("input[type=radio]:checked").val();
        if (teamId != null) {
            connection.invoke("SendNameByGroup", name, teamId);
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Lütfen Önce Takım Seçiniz',
            })
        }

    });

    $("input[type=radio]").change(() => {
        let value = $(`input[type=radio]:checked`).val();
        if (value == "Team A") {
            //İlgili Takıma Ekler
            connection.invoke("AddToGroup", value);
            connection.invoke("RemoveToGroup", "Team B");
        }
        else {
            connection.invoke("AddToGroup", value);
            connection.invoke("RemoveToGroup", "Team A");
        }
    });

   
    //#endregion

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

    connection.on("Error", (errorText) => {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: errorText,
        })
    });

    //Bağlantı ilk açıldığında static NameListesinde dava varsa db den okuma mantığı gibi okuyup ekrana basıyor
    connection.on("ReceiveNames", (names) => {
        $("#namesList").empty();
        names.forEach((item, index) => {
            $("#namesList").append(`<li class="list-group-item"> ${item}</li>`);
        });
    });

    connection.on("ReceiveMessageByGroup", (name, teamName) => {
        if (teamName == "Team A") {
            $("#ATeamList").append(`<li class="list-group-item"> ${name}</li>`);
            connection.invoke("GetAllNamesAsync");
        }
        if (teamName == "Team B") {
            $("#BTeamList").append(`<li class="list-group-item"> ${name}</li>`);
            connection.invoke("GetAllNamesAsync");

        }
    });

    connection.on("ReceiveAllNamesAsync", (result) => {

        if (result != null) {
            $("#GetAllListTeamAAsync").empty();
            $("#GetAllListTeamBAsync").empty();
            result.forEach((item, index) => {
                if (item.teamName == "Team A" && item.userName != null) {
                    $("#GetAllListTeamAAsync").append(`<li class="list-group-item"> ${item.userName}</li>`);
                }
                else if (item.teamName == "Team B" && item.userName != null) {
                    $("#GetAllListTeamBAsync").append(`<li class="list-group-item"> ${item.userName}</li>`);
                }
            });
        }
    });

    connection.on("ErrorTeamCount", (err) => {
        Swal.fire({
            title: err,
            input: 'text',
            inputAttributes: {
                autocapitalize: 'off'
            },
            showCancelButton: true,
            confirmButtonText: 'Gönder',
            showLoaderOnConfirm: true,
            preConfirm: (login) => {

                //TeamCout inputuna girilen değerin sayı olup olmadığı kontrol edilir
                let isnum = /^\d+$/.test(login);
                if (isnum) {
                    if (login <= 0) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Takım Sayısı sıfırdan küçük ve ya eşit olamaz',
                        });
                    }
                    else {
                        $.ajax({
                            url: "https://localhost:7088/api/notification/" + login,
                            type: "GET",
                            success: function (data, textStatus, jqXHR) {
                                Swal.fire({
                                    icon: 'success',
                                    text: 'Takım Sayısı ' + login + ' Kişi Olarak belirlenmiştir',
                                });
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Oops...',
                                    text: 'Takım Sayısı Eklenemedi',
                                });
                            }
                        });
                    }
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Yalnızca Sayı Girilmlidir',
                    });
                }
            },
            allowOutsideClick: () => !Swal.isLoading()
        })
    });

    //#endregion
});