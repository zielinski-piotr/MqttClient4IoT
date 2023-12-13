using Device.Esp32.Settings;
using Device.Esp32;
using Shared.Mqtt.Client;
using System.Device.I2c;

namespace Clients.Esp32
{
    internal static class Settings
    {
        public static MqttClientOptions BrokerSettings => new()
        {
            Address = "0.0.0.0",
            AutoReconnect = true,
            CleanSession = true,
            ClientId = "Esp32",
            DeviceId = "Esp32",
            Port = 1883,
            SubscribedTopics = new string[] { }
        };

        public static WifiSettings WifiSettings => new()
        {
            SSID = "Put your SSID here",
            Password = "Put your wifi password here",
        };

        public static I2cConnectionSettings I2CConnectionSettings = new I2cConnectionSettings(1, 0x76);

        public static LedSettings SuccessLedSettings = new()
        {
            Pin = 18,
            ResistorType = ResistorType.PullDown
        };

        public static LedSettings DebugLedSettings = new()
        {
            Pin = 17,
            ResistorType = ResistorType.PullDown
        };

        public static LedSettings FailureLedSettings = new()
        {
            Pin = 19,
            ResistorType = ResistorType.PullDown
        };
    }
}
