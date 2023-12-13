using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Clients.Supervisor.MessageHandlers;
using Clients.Supervisor.Services;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Client.Extensions;
using UnitsNet.Serialization.JsonNet;

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
        services.AddSingleton<IHostedService, SupervisorService>();

        services.RegisterMqttService();

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>() { new UnitsNetIQuantityJsonConverter() }
        };

        services.AddSingleton(hostBuilderContext.Configuration.GetSection("MqttClientOptions").Get<MqttClientOptions>());
        services.AddSingleton<IMessageResolver, MessageResolver>();
        services.AddTransient<IMessageHandler<TemperatureMessage>, TemperatureMessageHandler>();
        services.AddTransient<IMessageHandler<HumidityMessage>, HumidityMessageHandler>();
        services.AddTransient<IMessageHandler<PressureMessage>, PressureMessageHandler>();
        services.AddTransient<IMessageHandler<StatusMessage>, StatusMessageHandler>();
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