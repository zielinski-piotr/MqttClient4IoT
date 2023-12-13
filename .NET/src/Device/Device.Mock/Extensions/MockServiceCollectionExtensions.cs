using Device.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Mock.Extensions;

public static class MockServiceCollectionExtensions
{
    public static IServiceCollection RegisterRpiSenseHatMockDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SenseHatMock>();

        serviceCollection.AddSingleton<ILedMatrix>(serviceProvider => serviceProvider.GetRequiredService<SenseHatMock>());
        serviceCollection.AddSingleton<ITemperatureSensor>(serviceProvider => serviceProvider.GetRequiredService<SenseHatMock>());
        serviceCollection.AddSingleton<IHumiditySensor>(serviceProvider => serviceProvider.GetRequiredService<SenseHatMock>());
        serviceCollection.AddSingleton<IPressureSensor>(serviceProvider => serviceProvider.GetRequiredService<SenseHatMock>());

        return serviceCollection;
    }
    
    public static IServiceCollection RegisterBmp280MockDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<Bmp280Mock>();

        serviceCollection.AddSingleton<ITemperatureSensor>(serviceProvider => serviceProvider.GetRequiredService<Bmp280Mock>());
        serviceCollection.AddSingleton<IPressureSensor>(serviceProvider => serviceProvider.GetRequiredService<Bmp280Mock>());

        return serviceCollection;
    }

    public static IServiceCollection RegisterLedMockDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SuccessLed>();
        serviceCollection.AddSingleton<FailureLed>();
        
        return serviceCollection;
    }
}
