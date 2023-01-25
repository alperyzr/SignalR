using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.DAL;
using SignalR.API.Models;

namespace SignalR.API.Hubs
{
    public class MyHub:Hub
    {
        private readonly AppDbContext _context;

        public MyHub(AppDbContext context)
        {
            _context = context;
        }


        //MyHub a her istek atıldığında yeni bir nesne örneği oluşturulur.
        //Bu nedenle static list olarak tanımlarsak API ayakta olduğu sürece bu liste sıfırdan oluşmaz.
        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 7;

        //Clientlar her SendMessage methoduna istek yaptığında çalışır
        //Server a isimleri göndermek için kullanılacak olan method
        public async Task SendName(string name)
        {
            if (Names.Count >= TeamCount)
            {
                //Caller methodu sadece belirtilen takım üyesi sayısının üzerinde bir isim eklemeye çalışan Client a ilgili uyarı mesajını gösterir
                await Clients.Caller.SendAsync("Error", $"Takım En fazla {TeamCount} kişiden oluşmalıdır.");
            }
            else
            {
                Names.Add(name);

                //Server Tarafındaki method İsmi
                await Clients.All.SendAsync("ReceiveName", name);
            }
           
        }

        //Servderdan isimleri çekmek için kullanalacak method
        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }

        public async Task GetAllNamesAsync()
        {
            var result =  _context.Teams.Include(x=>x.Users).ToList();
            await Clients.All.SendAsync("ReceiveAllNamesAsync", result);
        }

        //Takıma Ekleme
        public async Task AddToGroup(string TeamName)
        {
            //Db üzerinden Takım eklemek için kullanıyoruz
            await Groups.AddToGroupAsync(Context.ConnectionId, TeamName);
        }

        //Takımdan Silme
        public async Task RemoveToGroup(string TeamName)
        {
            //Db üzerinden gruptan çıkartma yani silme için kullanıyoruz
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, TeamName);

        }

        //Takıma İsim Gönderme
        public async Task SendNameByGroup(string Name, string TeamName)
        {
            var team = await _context.Teams.Where(x => x.Name.Equals(TeamName)).FirstOrDefaultAsync();
            if (team != null)
            {
                team.Users.Add(new User() { Name = Name });
            }
            else
            {
                var newTeam = new Team() { Name = TeamName };
                newTeam.Users.Add(new User() { Name=Name });
                _context.Teams.Add(newTeam);
                
            }
            await _context.SaveChangesAsync();

            //Sadece bu gruba üye olan clientlar bu mesajı alacak
            await Clients.Group(TeamName).SendAsync("ReceiveMessageByGroup",Name, team.Name);
        }

        //Takımdan İsimleri Alma
        public async Task GetNamesByGroup()
        {
            var teams = _context.Teams.Include(x => x.Users).Select(x => new
            {
                TeamName = x.Name,
                Users = x.Users.ToList()
            });

            await Clients.All.SendAsync("ReceiveNamesByGroup",teams);
        }

        //Hub a bağlı clienatları listelemek için override edilen methodlar
        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiceClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiceClientCount", ClientCount);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
