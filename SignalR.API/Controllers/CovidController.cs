using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.API.Models;
using SignalR.API.Services.Interfaces;

namespace SignalR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CovidController : ControllerBase
    {
        private readonly ICovidService _covidService;

        public CovidController(ICovidService covidService)
        {
            _covidService = covidService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCovidAsync(Covid covid)
        {
            await _covidService.SaveCovidAsync(covid); 
            var covidChart = _covidService.GetCovidChartList();
            return Ok(covidChart);

        }

        [HttpGet]
        public IActionResult InitializeCovidAsync()
        {
            //Rasgetle Sayı üretir
            Random rnd = new Random();

            //Enumerable.Range methodu başlangıç ve bitiş değeri verdiğimiz bir liste oluşturur
            Enumerable.Range(1, 10).ToList().ForEach(x =>
            {
                //Enum.GetValues methodu tip belirterek istediğimiz classı ve ya enum ı belirtirsek içerisinde dönüyor ve yine o tipte dönüş sağlıyor
                foreach (ECity item in Enum.GetValues(typeof(ECity)))
                {
                    var newCovid = new Covid
                    {
                        City = item,
                        //Rasgele sayının 100 ile 1000 arasında olacağı belirtiliyor
                        Count = rnd.Next(100,1000),
                        CovidDate= DateTime.Now.AddDays(x),
                    };

                    //Wait() methodu await ile aynı işlemi yapar
                    _covidService.SaveCovidAsync(newCovid).Wait();
                    
                    //Her kayıt işleminden sonra sistemi 1sn bekletiyoruz
                    System.Threading.Thread.Sleep(300);
                }
            });

            return Ok("İşlem başarıyla Gerçekleştirildi");
        }
    }
}
