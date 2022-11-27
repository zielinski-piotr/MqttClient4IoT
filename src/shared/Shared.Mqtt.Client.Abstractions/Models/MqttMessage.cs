namespace Shared.Mqtt.Client.Abstractions.Models;

public class MqttMessage
{
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public string ResponseTopic { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}