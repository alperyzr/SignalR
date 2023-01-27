namespace SignalR.API.Models
{
    public enum ECity
    {
        Istanbul = 1,
        Ankara = 2,
        Izmır = 3,
        Antalya = 4,
        Muğla =5
    }
    public class Covid
    {
        public int Id { get; set; }
        public ECity City { get; set; }
        public DateTime CovidDate { get; set; }
        public int Count { get; set; }
    }
}
