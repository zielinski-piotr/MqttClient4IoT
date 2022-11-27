using Shared.Mqtt.Client.Abstractions.Models;

namespace Shared.Messaging
{
    public interface IMessageResolver
    {
        IMessage Resolve(MqttMessage message);
    }
}