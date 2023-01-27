using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.DAL;
using SignalR.API.Hubs;
using SignalR.API.Models;
using SignalR.API.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace SignalR.API.Services
{
    public class CovidService : ICovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext contex,
            IHubContext<CovidHub> hubContext)
        {
            _context = contex;
            _hubContext = hubContext;
        }

        public IQueryable<Covid> GetList()
        {

            //AsQuerable bir veri sorgulanacağında farklı farklı sorgular varsa hepsini birleştirir.
            //En son ToList, FirstOrDefault gibi çağrıldığı zaman tek seferde halleder ve db ye yük bindirmemiş olur
            return _context.Covids.AsQueryable();
        }

        public async Task SaveCovidAsync(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidListAsync", GetCovidChartList());
        }

        public List<CovidChart> GetCovidChartList()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();

            //Bir bağlantı parçacığı oluşturduk.
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                //Ardından grafiğe basmak için direkt olarak yazdığımız sql sorgusnu commondText olarak verdik.
                command.CommandText = @"select Tarih,[1],[2],[3],[4],[5] from
                                        (select [City], [Count], Cast([CovidDate] as date) as Tarih from Covids) as covidT
                                        PIVOT
                                        (Sum(Count) for City IN ([1],[2],[3],[4],[5])) as pivotTable
                                        order by Tarih asc";

                command.CommandType = System.Data.CommandType.Text;

                //Bağlantıyı açtık
                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart cc = new CovidChart()
                        {
                            CovidDate = reader.GetDateTime(0).ToShortDateString(),
                        };

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (System.DBNull.Value.Equals(reader[x]))
                            {
                                cc.Counts.Add(0);
                            }
                            else
                            {
                                cc.Counts.Add(reader.GetInt32(x));
                            }
                        });

                        covidCharts.Add(cc);
                    }
                }

                _context.Database.CloseConnection();
                return covidCharts;

            }
        }

    }
}
