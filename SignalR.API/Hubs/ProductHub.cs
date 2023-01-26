using Microsoft.AspNetCore.SignalR;
using SignalR.API.DAL;
using SignalR.API.Models;


namespace SignalR.API.Hubs
{
    public class ProductHub : Hub
    {
        #region Field
        private readonly AppDbContext _contex;       
        private static int ClientCount { get; set; } = 0;

        #endregion
       
        #region Ctor
        public ProductHub(AppDbContext appDbContext)
        {
            _contex = appDbContext;           
        }
        #endregion

        #region Methods       

        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        //Hub a bağlı client oturum kapattığında tetiklenir
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }

        public async Task SendProductName(Product product)
        {
            await Clients.All.SendAsync("ReceiveProduct", product);
        }
        #endregion
    }
}
