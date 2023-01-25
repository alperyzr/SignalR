using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.API.Hubs;
using System.Runtime.CompilerServices;

namespace SignalR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        //Hub classı üzerinden değilde Dependency ile Controller tarafından methodlara erişip aşağıdaki örnek gibi canlı takım sayısı dönmek istiyorsak
        //IHubContext interface ini implemente etmemiz gerekiyot
        private readonly IHubContext<MyHub> _hubContext;

        public NotificationController(IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }

       
        [HttpGet("{TeamCount}")]
        public async Task<IActionResult> SetTeamCount(int TeamCount)
        {
            //Notify Subscribe adı, diğeri ise ekrana gösterceğimiz metin
            await _hubContext.Clients.All.SendAsync("Notify", $"Takım maksimum {TeamCount} kişi olmalıdır");
            return Ok();
        }

    }
}
