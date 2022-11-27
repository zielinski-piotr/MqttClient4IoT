using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions.Models;
using Topics;
using Topics.Extensions;

namespace Mqtt.Supervisor.Services;

public class MessageResolver : IMessageResolver
{
    public IMessage Resolve(MqttMessage message) => message.Topic.ToTopicParts() switch
    {
        (TopicParts.Type.Sensor, _, TopicParts.Application.Humidity) => Serialization.Deserialize<HumidityMessage>(message),
        (TopicParts.Type.Sensor, _, TopicParts.Application.Pressure) => Serialization.Deserialize<PressureMessage>(message),
        (TopicParts.Type.Sensor, _, TopicParts.Application.Temperature) => Serialization.Deserialize<TemperatureMessage>(message),
        _ => throw new HandlerNotFoundException($"Message received on topic: '{message.Topic}' cannot be handled")
    };
}