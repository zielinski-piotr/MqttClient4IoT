using System.Drawing;
using Device.Abstractions;
using Microsoft.Extensions.Logging;
using Shared.Client;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Topics;
using static System.Threading.Tasks.Task;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

namespace Clients.Rpi.Services;

public class DeviceService : DeviceServiceBase
{
    private readonly ITemperatureSensor _temperatureSensor;
    private readonly IPressureSensor _pressureSensor;
    private readonly IHumiditySensor _humiditySensor;
    private readonly ILedMatrix _ledMatrix;

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
        ILogger<DeviceService> logger) : base(
        mqttClient,
        mqttClientOptions,
        messageDispatcher,
        messageResolver,
        logger)
    {
        _ledMatrix = ledMatrix;
        _temperatureSensor = temperatureSensor;
        _pressureSensor = pressureSensor;
        _humiditySensor = humiditySensor;
    }

    protected override async Task InnerExecuteAsync(CancellationToken cancellationToken)
    {
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

    private async Task SendTemperature(CancellationToken cancellationToken)
    {
        var message = new TemperatureMessage()
        {
            Temperature = _temperatureSensor.GetTemperature()
        };

        await PublishMeasurement(message, TopicParts.Application.Temperature, cancellationToken);
    }

    private async Task SendPressure(CancellationToken cancellationToken)
    {
        var message = new PressureMessage()
        {
            Pressure = _pressureSensor.GetPressure()
        };

        await PublishMeasurement(message, TopicParts.Application.Pressure, cancellationToken);
    }

    private async Task SendHumidity(CancellationToken cancellationToken)
    {
        var message = new HumidityMessage()
        {
            Humidity = _humiditySensor.GetRelativeHumidity()
        };

        await PublishMeasurement(message, TopicParts.Application.Humidity, cancellationToken);
    }

    protected override void SuccessState()
    {
        _ledMatrix.FillMatrix(_connectedColor);
    }

    protected override void FailedState()
    {
        _ledMatrix.FillMatrix(_disconnectedColor);
    }

    protected override void MessageSent()
    {
        _ledMatrix.DimMatrix(TimeSpan.FromMilliseconds(50), _connectedColor);
    }

    protected override void StartupState()
    {
        _ledMatrix.FillMatrix(_disconnectedColor);
    }

    protected override void ShutdownState()
    {
        _ledMatrix.FillMatrix(Color.Black);
    }
}