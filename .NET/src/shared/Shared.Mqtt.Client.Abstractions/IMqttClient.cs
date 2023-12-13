using Shared.Mqtt.Client.Abstractions.Models;

namespace Shared.Mqtt.Client.Abstractions;

public interface IMqttClient
{
    Task ConnectAsync(
        MqttClientOptions mqttClientOptions,
        Func<MqttMessage, CancellationToken, Task> onMessageReceivedHandlerAsync,
        Func<CancellationToken, Task>? onConnectedHandlerAsync,
        Func<CancellationToken, Task>? onDisconnectedHandlerAsync,
        CancellationToken cancellationToken);
    Task PublishAsync(byte[] payload, string topic, CancellationToken cancellationToken);
    Task DisconnectAsync(CancellationToken cancellationToken);
    bool IsConnected { get; }
}