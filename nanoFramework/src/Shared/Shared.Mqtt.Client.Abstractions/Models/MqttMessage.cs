namespace Shared.Mqtt.Client.Abstractions.Models
{
    public class MqttMessage
    {
        public byte[] Payload { get; set; } = new byte[] { };
        public string ResponseTopic { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
    }
}
