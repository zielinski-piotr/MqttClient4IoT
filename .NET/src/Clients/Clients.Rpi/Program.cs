using Device.Mock.Extensions;
using Device.Rpi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Clients.Rpi.MessageHandlers;
using Clients.Rpi.Services;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Client.Extensions;
using UnitsNet.Serialization.JsonNet;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
        config.AddEnvironmentVariables();

        if (args.Any())
        {
            config.AddCommandLine(args);
        }
    })
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.AddOptions();
        services.AddHostedService<DeviceService>();

        services.RegisterMqttService();

        if (Runtime.RuntimeIdentifier is "linux-arm" or "linux-arm64" or "raspbian.11-arm")
        {
            services.RegisterRpiSenseHatDeviceTypes();
        }
        else
        {
            services.RegisterRpiSenseHatMockDeviceTypes();
        }
        
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>() {new UnitsNetIQuantityJsonConverter()}
        };

        services.AddSingleton(hostBuilderContext.Configuration.GetSection("MqttClientOptions").Get<MqttClientOptions>());
        services.AddSingleton<IMessageResolver, MessageResolver>();
        services.AddTransient<IMessageHandler<LedMatrixMessage>, LedMatrixMessageHandler>();
        services.AddTransient<IMessageDispatcher, MessageDispatcher>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();
        
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddSerilog();
    });

await builder.RunConsoleAsync();