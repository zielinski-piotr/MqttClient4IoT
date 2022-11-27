using System.Drawing;
using Device.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions;
using Shared.Mqtt.Client.Abstractions.Models;
using Topics;
using static System.Threading.Tasks.Task;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

namespace Mqtt.Device.Services;

public class DeviceService : IHostedService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;
    private readonly IMessageDispatcher _messageDispatcher;
    private readonly IMessageResolver _messageResolver;
    private readonly ITemperatureSensor _temperatureSensor;
    private readonly IPressureSensor _pressureSensor;
    private readonly IHumiditySensor _humiditySensor;
    private readonly ILedMatrix _ledMatrix;
    private readonly ILogger<DeviceService> _logger;

    private readonly Color _connectedColor = Color.ForestGreen;
    private readonly Color _disconnectedColor = Color.DarkRed;

    public DeviceService(
        IMqttClient mqttClient,
        MqttClientOptions mqttClientOptions,
        IMessageDispatcher messageDispatcher,
        IMessageResolver messageResolver,
        ITemperatureSensor temperatureSensor,
        IPressureSensor pressureSensor,
        IHumiditySensor humiditySensor,
        ILedMatrix ledMatrix,
        ILogger<DeviceService> logger)
    {
        _mqttClient = mqttClient;
        _mqttClientOptions = mqttClientOptions;
        _messageDispatcher = messageDispatcher;
        _messageResolver = messageResolver;
        _temperatureSensor = temperatureSensor;
        _pressureSensor = pressureSensor;
        _humiditySensor = humiditySensor;
        _ledMatrix = ledMatrix;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting device. Running at {RuntimeIdentifier}", Runtime.RuntimeIdentifier);
        
        _ledMatrix.FillMatrix(_disconnectedColor);

        await _mqttClient.ConnectAsync(_mqttClientOptions,
            onMessageReceivedHandlerAsync: (mqttMessage, token) =>
            {
                var message = _messageResolver.Resolve(mqttMessage);
                return _messageDispatcher.Dispatch(message, mqttMessage.Topic, token);
            },
            onConnectedHandlerAsync: OnConnected,
            onDisconnectedHandlerAsync: OnDisconnected,
            cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            await SendTemperature(cancellationToken);
            await Delay(1000, cancellationToken);
            await SendPressure(cancellationToken);
            await Delay(1000, cancellationToken);
            await SendHumidity(cancellationToken);
            await Delay(5000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _mqttClient.DisconnectAsync(cancellationToken);
    }

    private async Task SendTemperature(CancellationToken cancellationToken)
    {
        var message = new Shared.Contracts.TemperatureMessage()
        {
            Temperature = _temperatureSensor.GetTemperature()
        };

        await PublishMeasurement(message, TopicParts.Application.Temperature, cancellationToken);
    }

    private async Task SendPressure(CancellationToken cancellationToken)
    {
        var message = new Shared.Contracts.PressureMessage()
        {
            Pressure = _pressureSensor.GetPressure()
        };

        await PublishMeasurement(message, TopicParts.Application.Pressure, cancellationToken);
    }

    private async Task SendHumidity(CancellationToken cancellationToken)
    {
        var message = new Shared.Contracts.HumidityMessage()
        {
            Humidity = _humiditySensor.GetRelativeHumidity()
        };

        await PublishMeasurement(message, TopicParts.Application.Humidity, cancellationToken);
    }

    private async Task PublishMeasurement<T>(T message, TopicParts.Application topicApplicationPart,
        CancellationToken cancellationToken)
        where T : class
    {
        var topic = TopicBuilder
            .Create()
            .WithType(TopicParts.Type.Sensor)
            .WithDevice(_mqttClientOptions.DeviceId)
            .WithApplication(topicApplicationPart)
            .Build();

        try
        {
            if (_mqttClient.IsConnected)
            {
                _logger.LogInformation("Publishing message on topic '{Topic}'", topic);
                await _mqttClient.PublishAsync(Serialization.Serialize(message), topic, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Publishing message on topic '{Topic}' failed", topic);
        }
    }
    
    private Task OnDisconnected(CancellationToken token)
    {
        _ledMatrix.FillMatrix(_disconnectedColor);
        
        return CompletedTask;
    }

    private Task OnConnected(CancellationToken token)
    {
        _ledMatrix.FillMatrix(_connectedColor);
        
        return CompletedTask;
    }
}