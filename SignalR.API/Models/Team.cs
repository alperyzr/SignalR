namespace SignalR.API.Models
{
    public class Team
    {
        public Team()
        {
            Users = new List<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        
        //ICollection methodunun kendi içerisinde add,clear,contains, copyTo, remove methodları mevcut
        //virtual ise LazyLoading desteğide olsun diye belirtildi
        public virtual ICollection<User> Users { get; set; }
    }
}
