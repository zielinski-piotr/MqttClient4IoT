using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Topics;
using Shared.Mqtt.Topics.Extensions;

namespace Clients.Rpi.Services;

public class MessageResolver : IMessageResolver
{
    public IMessage Resolve(MqttMessage message) => message.Topic.ToTopicParts() switch
    {
        (TopicParts.Type.Sensor, _, TopicParts.Application.LedMatrix) => Serialization.Deserialize<LedMatrixMessage>(message),
        _ => throw new HandlerNotFoundException($"Message received on topic: '{message.Topic}' cannot be handled")
    };
}