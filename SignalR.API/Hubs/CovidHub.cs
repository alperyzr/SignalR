using Microsoft.AspNetCore.SignalR;
using SignalR.API.DAL;
using SignalR.API.Services.Interfaces;

namespace SignalR.API.Hubs
{
    public class CovidHub:Hub
    {
        private readonly ICovidService _covidService;

        public CovidHub(ICovidService covidService)
        {
            _covidService= covidService;
        }

        public async Task GetCovidListAsync()
        {           
            await Clients.All.SendAsync("ReceiveCovidListAsync",_covidService.GetCovidChartList() );
        }
    }
}
