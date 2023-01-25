using Microsoft.EntityFrameworkCore;
using SignalR.API.Models;

namespace SignalR.API.DAL
{
    public class AppDbContext: DbContext
    {
        //options ı Base e göndermek için kullanılır
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
