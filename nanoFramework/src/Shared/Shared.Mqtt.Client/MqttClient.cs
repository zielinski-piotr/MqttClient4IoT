using nanoFramework.M2Mqtt.Messages;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Shared.Mqtt.Client
{
    public class MqttClient : Abstractions.IMqttClient, IDisposable
    {
        public bool IsConnected => _client.IsConnected;

        public bool AutoReconnect { get; private set; }

        private readonly nanoFramework.M2Mqtt.MqttClient _client;
        private readonly MqttClientOptions _mqttClientOptions;

        public MqttClient(MqttClientOptions mqttClientOptions)
        {
            _mqttClientOptions = mqttClientOptions;
            _client = new nanoFramework.M2Mqtt.MqttClient(_mqttClientOptions.Address);
        }

        public void Connect(Action onConnectedHandler, Action onDisconnectedHandler)
        {
            SetupReconnectOnDisconnected(_mqttClientOptions.AutoReconnect, onDisconnectedHandler);
            SetupSubscriptionsOnConnected(_mqttClientOptions.SubscribedTopics, onConnectedHandler);

            Connect();
        }

        private void SetupSubscriptionsOnConnected(string[] topics, Action onConnectedHandler)
        {
            _client.ConnectionOpened += (_, _) =>
                       {
                           if (onConnectedHandler is not null)
                           {
                               try
                               {
                                   onConnectedHandler();
                                   Debug.WriteLine($"Invoked OnConnectedHandlerAsync user handler");
                               }
                               catch (Exception e)
                               {
                                   Debug.WriteLine($"OnConnectedHandlerAsync user handler reported an exception. Exception was suppressed. Exception: '{e}'");
                               }
                           }

                           if (topics != null && topics.Length <= 0) return;

                           var qos = new MqttQoSLevel[topics.Length];

                           for (var i = 0; i < topics.Length; i++)
                           {
                               qos[i] = MqttQoSLevel.ExactlyOnce;
                           }

                           _client.Subscribe(topics, qos);
                       };
        }

        private void SetupReconnectOnDisconnected(bool autoReconnect, Action onDisconnectedHandler)
        {
            AutoReconnect = autoReconnect;

            _client.ConnectionClosed += (_, _) =>
            {
                Debug.WriteLine($"Client disconnected from the broker.");

                if (onDisconnectedHandler is not null)
                {
                    try
                    {
                        onDisconnectedHandler();
                        Debug.WriteLine($"Invoked OnDisconnectedHandler user handler");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"OnDisconnectedHandler user handler reported an exception. Exception was suppressed. Exception: '{e}'");
                    }
                }

                if (!AutoReconnect) return;

                Debug.WriteLine($"The client is reconnecting in 5 seconds ...");
                Thread.Sleep(5000);
                Connect();
            };
        }

        private void Connect()
        {
            while (true)
            {
                var result = MqttReasonCode.UnspecifiedError;

                Debug.WriteLine($"Connecting to the broker :'{_mqttClientOptions.Address}'");

                try
                {
                    result = _client.Connect(
                        clientId: _mqttClientOptions.ClientId, 
                        username: string.Empty, 
                        password: string.Empty, 
                        willRetain: false, 
                        willQosLevel: MqttQoSLevel.ExactlyOnce, 
                        willFlag: true, 
                        willTopic: $"Sensor/{_mqttClientOptions.DeviceId}/Status", 
                        willMessage: @"{""Status"": ""Disconnected""}", 
                        cleanSession: _mqttClientOptions.CleanSession, 
                        keepAlivePeriod: 60);

                    Debug.WriteLine($"Client connection result: '{result}'");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Client connection error result: '{result}', exception: '{e}'");
                }

                if (result != MqttReasonCode.Success)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    continue;
                }
                break;
            }
        }

        public void Disconnect()
        {
            if (_client.IsConnected)
            {
                AutoReconnect = false;

                _client.Disconnect();
            }
        }

        public void Publish(byte[] payload, string topic)
        {
            {
                if (_client.IsConnected)
                {
                    try
                    {
                        _client.Publish(
                            topic,
                            payload);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"An error has been thrown when trying to publish message to topic '{topic}' with exception: '{e.Message}'");
                    }
                }
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
