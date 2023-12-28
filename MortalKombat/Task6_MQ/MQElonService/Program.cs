using System.Reflection;
using Contracts.Interfaces;
using MassTransit;
using MQElonService;
using MQPlayerRoom;
using Nsu.MortalKombat.Players;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPlayer, Elon>();
builder.Services.AddMassTransit(x =>
{   
    x.AddConsumer<ElonPlayer>();
    
    var mvc = x.AddControllers();
    mvc.AddApplicationPart(typeof(MQPickCardController).Assembly);
    mvc.AddControllersAsServices();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        var hostCfg = builder.Configuration;
        var ip = hostCfg.GetValue<string>("MQHost");
        if (ip == null)
        {
            throw new InvalidDataException("No \"MQHost\" set in appsettings.json!");
        }
        
        cfg.Host(ip, "/", h =>
        {
            h.Username(hostCfg.GetValue<string>("MQUser"));
            h.Password(hostCfg.GetValue<string>("MQPassword"));
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
