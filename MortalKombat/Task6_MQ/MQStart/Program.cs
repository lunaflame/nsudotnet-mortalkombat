using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MQStart
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            return builder.ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<GodClient.PlayerExperimentQuerier.PlayerListener>();
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var hostCfg = hostContext.Configuration;
                        var ip = hostCfg.GetValue<string>("MQHost");

                        cfg.Host(ip, "/", h =>
                        {
                            h.Username(hostCfg.GetValue<string>("MQUser"));
                            h.Password(hostCfg.GetValue<string>("MQPassword"));
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });
                services.AddHostedService<GodClient.Client>();
            });
        }
    }
}
