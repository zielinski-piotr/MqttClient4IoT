using System;
using System.Text;
using Newtonsoft.Json;
using Shared.Mqtt.Client.Abstractions.Models;

namespace Shared.Messaging
{
    public static class Serialization
    {
        public static IMessage Deserialize<T>(MqttMessage message) where T : class, IMessage
        {
            var payload = Encoding.UTF8.GetString(message.Payload);

            return JsonConvert.DeserializeObject<T>(payload) ?? throw new InvalidOperationException($"Unable to deserialize {payload} to {typeof(T)} type");
        }

        public static byte[] Serialize<T>(T message) where T : class
        {
            var messageString = JsonConvert.SerializeObject(message);
            
            return Encoding.UTF8.GetBytes(messageString);
        }
    }
}