using System.Reflection;
using Contracts.Interfaces;
using Nsu.MortalKombat.Players;
using PlayerRoom.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Assembly[] externalControllers = {
	typeof(PlayerController).Assembly,
	/* more controllers... */
};

var mvc = builder.Services.AddMvc();
builder.Services.AddScoped<IPlayer, Elon>();

foreach (Assembly controller in externalControllers)
{
	mvc.AddApplicationPart(controller);
}
mvc.AddControllersAsServices();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();