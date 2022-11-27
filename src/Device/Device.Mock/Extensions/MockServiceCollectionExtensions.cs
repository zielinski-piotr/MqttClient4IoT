using Device.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Mock.Extensions;

public static class MockServiceCollectionExtensions
{
    public static IServiceCollection RegisterMockDeviceTypes(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<MockSenseHat>();

        serviceCollection.AddSingleton<ILedMatrix>(serviceProvider => serviceProvider.GetRequiredService<MockSenseHat>());
        serviceCollection.AddSingleton<ITemperatureSensor>(serviceProvider => serviceProvider.GetRequiredService<MockSenseHat>());
        serviceCollection.AddSingleton<IHumiditySensor>(serviceProvider => serviceProvider.GetRequiredService<MockSenseHat>());
        serviceCollection.AddSingleton<IPressureSensor>(serviceProvider => serviceProvider.GetRequiredService<MockSenseHat>());

        return serviceCollection;
    }
}
