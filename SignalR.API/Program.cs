using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SignalR.API.DAL;
using SignalR.API.Hubs;
using SignalR.API.Services;
using SignalR.API.Services.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(x =>
{    
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        //Web projesi bu port üzerinden ayağa kalkıyor. değişiklik gösterebilir
        //Headers içerebilir
        //Methodlara istek atabilir
        builder.WithOrigins("https://localhost:7297")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
              
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<ICovidService, CovidService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Server a bağlanacak clientların yetkilendirilmesi

//SignaR kütüphanesi implementasonu
builder.Services.AddSignalR();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

//Yukarıda yazdığımız CorsPolicy isimli servisi extention olarak belirtiyoruz
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

//http://localhost:4400/MyHub clientlar, api url i üzerinden hub a ulaşabilir
app.MapHub<MyHub>("/MyHub");
app.MapHub<ProductHub>("/ProductHub");
app.MapHub<CovidHub>("/CovidHub");


app.Run();
