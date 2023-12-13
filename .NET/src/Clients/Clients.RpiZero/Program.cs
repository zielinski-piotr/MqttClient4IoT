using Clients.RpiZero.Services;
using Device.Mock.Extensions;
using Device.Rpi.Extensions;
using Device.Rpi.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
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
        
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>() {new UnitsNetIQuantityJsonConverter()}
        };

        services.AddSingleton(hostBuilderContext.Configuration.GetSection("MqttClientOptions")
            .Get<MqttClientOptions>());
        
        services.AddSingleton(hostBuilderContext.Configuration.GetSection("I2cConnectionSettings")
            .Get<I2CConnectionSettings>());
        
        services.AddSingleton<IMessageResolver, MessageResolver>();
        services.AddTransient<IMessageDispatcher, MessageDispatcher>();
        
        if (Runtime.RuntimeIdentifier.Contains("linux-arm") || Runtime.RuntimeIdentifier.Contains("raspbian"))
        {
            services.RegisterRpiBmp280DeviceTypes();
            services.RegisterLedDeviceTypes();
            
            services.AddSingleton(hostBuilderContext.Configuration.GetSection("SuccessLedSettings")
                .Get<SuccessLedSettings>());
            services.AddSingleton(hostBuilderContext.Configuration.GetSection("FailureLedSettings")
                .Get<FailureLedSettings>());
        }
        else
        {
            services.RegisterBmp280MockDeviceTypes();
            services.RegisterLedMockDeviceTypes();
        }

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