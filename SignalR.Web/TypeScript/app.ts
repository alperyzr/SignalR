export class App {
    

    public signalRConnection: any;

    constructor() {
        debugger;
        $(document).ready(() => {
            this.signalRConnection = new signalR.HubConnectionBuilder()
                .withUrl("https://localhost:7088/CovidHub")
                .build();

            debugger;
            this.signalRConnection.start().then((e) => {
                debugger;
                this.signalRConnection.invoke("GetCovidListAsync");
            }).catch((err: any) => {
                debugger;
                document.write(err)
            });

            this.signalRConnection.on("ReceiveCovidListAsync", (covidList) => {
                debugger;
            });
        });


    }
}



