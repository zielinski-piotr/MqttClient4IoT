using Device.Abstractions;
using Microsoft.Extensions.Logging;
using Shared.Client;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Topics;
using static System.Threading.Tasks.Task;

namespace Clients.RpiZero.Services;

public class DeviceService : DeviceServiceBase
{
    private readonly ITemperatureSensor _temperatureSensor;
    private readonly IPressureSensor _pressureSensor;
    private readonly ISuccessLed _successLed;
    private readonly IFailureLed _failureLed;

    public DeviceService(
        IMqttClient mqttClient,
        MqttClientOptions mqttClientOptions,
        IMessageDispatcher messageDispatcher,
        IMessageResolver messageResolver,
        ITemperatureSensor temperatureSensor,
        IPressureSensor pressureSensor,
        ISuccessLed successLed,
        IFailureLed failureLed,
        ILogger<DeviceService> logger) : base(
        mqttClient,
        mqttClientOptions,
        messageDispatcher,
        messageResolver,
        logger)
    {
        _temperatureSensor = temperatureSensor;
        _pressureSensor = pressureSensor;
        _successLed = successLed;
        _failureLed = failureLed;
    }

    protected override async Task InnerExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await SendTemperature(cancellationToken);
            await Delay(1000, cancellationToken);
            await SendPressure(cancellationToken);
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

    protected override void SuccessState()
    {
        _failureLed.Off();
        _successLed.On();
    }

    protected override void FailedState()
    {
        _failureLed.On();
        _successLed.Off();
    }

    protected override void MessageSent()
    {
        _successLed.Off(TimeSpan.FromMilliseconds(50));
    }

    protected override void ShutdownState()
    {
        _failureLed.Off();
        _successLed.Off();
    }

    protected override void StartupState()
    {
        _failureLed.On();
        _successLed.Off();
    }
}