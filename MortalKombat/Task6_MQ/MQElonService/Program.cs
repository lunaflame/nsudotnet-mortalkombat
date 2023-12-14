using System.Reflection;
using Contracts.Interfaces;
using MassTransit;
using MQElonService;
using Nsu.MortalKombat.Players;
using PlayerRoom.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();

Assembly[] externalControllers = {
	typeof(PlayerController).Assembly,
	/* more controllers... */
};

var mvc = builder.Services.AddMvc();
builder.Services.AddScoped<IPlayer, Elon>();

builder.Services.AddMassTransit(x =>
{   
    x.AddConsumer<ElonPlayer>();
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

        /*var queueName = hostCfg.GetValue<string>("MQQueueName");
        if (queueName == null)
        {
            throw new InvalidDataException("No \"MQQueueName\" set in appsettings.json!");
        }*/
    });
});

// builder.Services.AddHostedService<GodClient.Client>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
