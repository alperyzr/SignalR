using SignalR.API.Models;

namespace SignalR.API.Services.Interfaces
{
    public interface ICovidService
    {
        IQueryable<Covid> GetList();
        Task SaveCovidAsync(Covid covid);    
        List<CovidChart> GetCovidChartList();
    }
}
