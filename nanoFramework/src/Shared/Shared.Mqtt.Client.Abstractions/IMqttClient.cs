using Shared.Mqtt.Client.Abstractions.Models;
using System;

namespace Shared.Mqtt.Client.Abstractions
{
    public interface IMqttClient
    {
        void Connect(
            Action onConnectedHandler,
            Action onDisconnectedHandler);
        void Publish(byte[] payload, string topic);
        void Disconnect();
        bool IsConnected { get; }
    }
}
