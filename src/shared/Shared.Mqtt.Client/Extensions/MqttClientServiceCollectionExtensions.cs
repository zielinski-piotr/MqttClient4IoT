using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using Shared.Mqtt.Client.Abstractions;

namespace Shared.Mqtt.Client.Extensions;

public static class MqttClientServiceCollectionExtensions
{
    public static IServiceCollection RegisterMqttService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMqttClient, MqttClient>();
        serviceCollection.AddTransient<MqttFactory>();

        return serviceCollection;
    }
}