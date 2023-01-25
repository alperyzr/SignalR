using Microsoft.AspNetCore.SignalR;

namespace SignalR.API.Hubs
{
    public class MyHub:Hub
    {
        //MyHub a her istek atıldığında yeni bir nesne örneği oluşturulur.
        //Bu nedenle static list olarak tanımlarsak API ayakta olduğu sürece bu liste sıfırdan oluşmaz.
        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;

        //Clientlar her SendMessage methoduna istek yaptığında çalışır
        //Server a isimleri göndermek için kullanılacak olan method
        public async Task SendName(string name)
        {
            Names.Add(name);

            //Server Tarafındaki method İsmi
           await Clients.All.SendAsync("ReceiveName", name);
        }


        //Servderdan isimleri çekmek için kullanalacak method
        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }


        //Hub a bağlı clienatları listelemek için override edilen methodlar
        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiceClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiceClientCount", ClientCount);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
