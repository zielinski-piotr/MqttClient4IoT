using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Topics;
using Shared.Mqtt.Topics.Extensions;

namespace Clients.Supervisor.Services;

public class MessageResolver : IMessageResolver
{
    public IMessage Resolve(MqttMessage message) => message.Topic.ToTopicParts() switch
    {
        (TopicParts.Type.Sensor, _, TopicParts.Application.Humidity) => Serialization.Deserialize<HumidityMessage>(message),
        (TopicParts.Type.Sensor, _, TopicParts.Application.Pressure) => Serialization.Deserialize<PressureMessage>(message),
        (TopicParts.Type.Sensor, _, TopicParts.Application.Temperature) => Serialization.Deserialize<TemperatureMessage>(message),
        (TopicParts.Type.Sensor, _, TopicParts.Application.Status)  => Serialization.Deserialize<StatusMessage>(message),
        _ => throw new HandlerNotFoundException($"Message received on topic: '{message.Topic}' cannot be handled")
    };
}