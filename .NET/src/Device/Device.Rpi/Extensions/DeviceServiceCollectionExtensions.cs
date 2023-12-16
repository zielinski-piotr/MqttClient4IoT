using System.Device.Gpio;
using Device.Abstractions;
using Iot.Device.SenseHat;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Rpi.Extensions;

public static class DeviceServiceCollectionExtensions
{
    public static IServiceCollection RegisterRpiSenseHatDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SenseHat>();
        serviceCollection.AddSingleton<RpiSenseHat>();

        serviceCollection.AddSingleton<ILedMatrix>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());
        serviceCollection.AddSingleton<ITemperatureSensor>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());
        serviceCollection.AddSingleton<IHumiditySensor>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());
        serviceCollection.AddSingleton<IPressureSensor>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());

        return serviceCollection;
    }
    
    public static IServiceCollection RegisterRpiBmp280DeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<RpiBmp280>();

        serviceCollection.AddSingleton<ITemperatureSensor>(serviceProvider => serviceProvider.GetRequiredService<RpiBmp280>());
        serviceCollection.AddSingleton<IPressureSensor>(serviceProvider => serviceProvider.GetRequiredService<RpiBmp280>());

        return serviceCollection;
    }

    public static IServiceCollection RegisterLedDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GpioController>();
        serviceCollection.AddSingleton<ISuccessLed, SuccessLed>();
        serviceCollection.AddSingleton<IFailureLed, FailureLed>();

        return serviceCollection;
    }
    
    
    
}