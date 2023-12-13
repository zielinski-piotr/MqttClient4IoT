using System;

namespace Shared.Mqtt.Client
{
    public class MqttClientOptions
    {
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; } = 1883;
        public string ClientId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public bool CleanSession { get; set; } = true;
        public bool AutoReconnect { get; set; } = true;
        public string[] SubscribedTopics { get; set; } = new string[] { };
    }
}
