//export class App {
//    private static instance: App = new App();
//    public signalRConnection: any;
//    public static getInstance(): App {
//        return App.instance;
//    }
//    constructor() {
//        debugger;
//        $(document).ready(() => {
//            this.signalRConnection = new signalR.HubConnectionBuilder()
//                .withUrl("https://localhost:7088/CovidHub")
//                .build();
//            debugger;
//            this.signalRConnection.start().then((e) => {
//                debugger;
//                this.signalRConnection.invoke("GetCovidListAsync");
//            }).catch((err: any) => {
//                debugger;
//                document.write(err)
//            });
//            this.signalRConnection.on("ReceiveCovidListAsync", (covidList) => {
//                debugger;
//            });
//        });
//    }
//}
$(document).ready(function () {
    debugger;
    alert("geldi");
    var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7088/CovidHub").build();
    connection.start().then(function (e) {
        connection.invoke("GetCovidListAsync");
    }).catch(function (err) {
        debugger;
        console.log(err);
    });
    connection.on("ReceiveCovidListAsync", function (covidList) {
        debugger;
    });
});
//# sourceMappingURL=app.js.map