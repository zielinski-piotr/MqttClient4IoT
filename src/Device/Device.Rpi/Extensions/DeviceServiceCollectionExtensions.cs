using Device.Abstractions;
using Iot.Device.SenseHat;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Rpi.Extensions;

public static class DeviceServiceCollectionExtensions
{
    public static IServiceCollection RegisterRpiDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SenseHat>();
        serviceCollection.AddSingleton<RpiSenseHat>();

        serviceCollection.AddSingleton<ILedMatrix>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());
        serviceCollection.AddSingleton<ITemperatureSensor>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());
        serviceCollection.AddSingleton<IHumiditySensor>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());
        serviceCollection.AddSingleton<IPressureSensor>(serviceProvider => serviceProvider.GetRequiredService<RpiSenseHat>());

        return serviceCollection;
    }
}