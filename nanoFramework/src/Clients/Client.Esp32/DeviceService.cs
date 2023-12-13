using Device.Abstractions;
using Shared.Mqtt.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace Clients.Esp32
{
    public class DeviceService
    {
        private readonly ITemperatureSensor _temperatureSensor;
        private readonly IPressureSensor _pressureSensor;
        private readonly ILed _successLed;
        private readonly ILed _failureLed;
        private readonly Shared.Mqtt.Client.Abstractions.IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttClientOptions;

        public DeviceService(
            Shared.Mqtt.Client.Abstractions.IMqttClient mqttClient,
            MqttClientOptions mqttClientOptions,
            ITemperatureSensor temperatureSensor,
            IPressureSensor pressureSensor,
            ILed successLed,
            ILed failureLed)
        {
            _temperatureSensor = temperatureSensor;
            _pressureSensor = pressureSensor;
            _successLed = successLed;
            _failureLed = failureLed;
            _mqttClient = mqttClient;
            _mqttClientOptions = mqttClientOptions;
        }

        public void Execute()
        {
            try
            {
                StartupState();

                Debug.WriteLine("Starting device.");

                _mqttClient.Connect(
                    onConnectedHandler: OnConnected,
                    onDisconnectedHandler: OnDisconnected);

                SendMeasurements();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Application Critical Exception: '{e}'");
                throw;
            }
            finally
            {
                _mqttClient.Disconnect();
                ShutdownState();
            }
        }

        private void SendMeasurements()
        {
            while (true)
            {
                SendTemperature();
                Thread.Sleep(1000);
                SendPressure();
                Thread.Sleep(5000);
            }
        }

        private void SendTemperature()
        {
            var measurement = _temperatureSensor.GetTemperature();
            var serializedMessage = @"{""Temperature"":{""Unit"":""TemperatureUnit.DegreeCelsius"",""Value"":" + measurement.DegreesCelsius + "}}";
            PublishMeasurement(Encoding.UTF8.GetBytes(serializedMessage), $"Sensor/{_mqttClientOptions.DeviceId}/Temperature");
        }

        private void SendPressure()
        {
            var measurement = _pressureSensor.GetPressure();
            var serializedMessage = @"{""Pressure"":{""Unit"":""PressureUnit.Hectopascal"",""Value"":" + measurement.Hectopascals + "}}";
            PublishMeasurement(Encoding.UTF8.GetBytes(serializedMessage), $"Sensor/{_mqttClientOptions.DeviceId}/Pressure");
        }

        private void OnDisconnected() => FailedState();

        private void OnConnected() => SuccessState();

        private void PublishMeasurement(byte[] message, string topic)
        {
            try
            {
                if (_mqttClient.IsConnected)
                {
                    Debug.WriteLine($"Publishing message on topic '{topic}'");
                                        
                    _mqttClient.Publish(message, topic);
                    MessageSent();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Publishing message on topic '{topic}' failed with Exception:'{e}'");
            }
        }

        private void SuccessState()
        {
            _failureLed.Off();
            _successLed.On();
        }

        private void FailedState()
        {
            _failureLed.On();
            _successLed.Off();
        }

        private void MessageSent()
        {
            _successLed.Off(TimeSpan.FromMilliseconds(50));
        }

        private void ShutdownState()
        {
            _failureLed.Off();
            _successLed.Off();
        }

        private void StartupState()
        {
            _failureLed.On();
            _successLed.Off();
        }
    }
}
