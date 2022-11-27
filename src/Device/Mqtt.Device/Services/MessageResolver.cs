using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions.Models;
using Topics;
using Topics.Extensions;

namespace Mqtt.Device.Services;

public class MessageResolver : IMessageResolver
{
    public IMessage Resolve(MqttMessage message) => message.Topic.ToTopicParts() switch
    {
        (TopicParts.Type.Sensor, _, TopicParts.Application.LedMatrix) => Serialization.Deserialize<LedMatrixMessage>(message),
        _ => throw new HandlerNotFoundException($"Message received on topic: '{message.Topic}' cannot be handled")
    };
}