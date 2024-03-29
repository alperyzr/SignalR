﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.API.Hubs;
using System.Runtime.CompilerServices;

namespace SignalR.API.Controllers
{
    [Route("api/[controller]/[action]")]
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
            
                MyHub.TeamCount = TeamCount;
                //Notify Subscribe adı, diğeri ise ekrana gösterceğimiz metin
                await _hubContext.Clients.All.SendAsync("Notify", $"Takım {TeamCount} kişiden oluşacaktır");
                return Ok();
            
            
        }

    }
}
