using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SolarEdge.Monitor
{
    class Program
    {

        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables("SolarEdge:Monitor:");
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions<Inverter.Config>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Inverter"));
                    services.AddSingleton<Inverter.IFactory, Inverter.Factory>();
                    services.AddOptions<Message.Config>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("MQTT"));
                    services.AddSingleton<Message.IFactory, Message.Factory>();
                    services.AddSingleton<Message.IPublisher, Message.Publisher>();
                    services.AddSingleton<Message.IFacade, Message.Facade>();
                    services.AddOptions<Service.Config>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Service"));
                    services.AddSingleton<Service.State.IMachine, Service.State.Machine>();
                    services.AddSingleton<Service.State.IFactory, Service.State.Factory>();
                    services.AddSingleton<Service.State.Transition.IFactory, Service.State.Transition.Factory>();
                    services.AddSingleton<IHostedService, Service.Instance>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConsole();
                });

            try
            {
                await builder
                    .UseConsoleLifetime()
                    .Build()
                    .ValidateConfiguration<Inverter.Config, Message.Config, Service.Config>()
                    .PrintConfiguration<Inverter.Config, Message.Config, Service.Config>()
                    .RunAsync();
            }
            catch (ConfigurationValidationException e)
            {
                Console.WriteLine($"One or more configuration errors occured:{Environment.NewLine}{e.Message}");
                Console.ReadLine();
            }
        }
    }
}
