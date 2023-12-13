using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Topics;
using static System.Threading.Tasks.Task;
using Runtime = System.Runtime.InteropServices.RuntimeInformation;

namespace Shared.Client;

public class DeviceServiceBase : BackgroundService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;
    private readonly IMessageDispatcher _messageDispatcher;
    private readonly IMessageResolver _messageResolver;
    private readonly ILogger<DeviceServiceBase> _logger;

    protected DeviceServiceBase(
        IMqttClient mqttClient,
        MqttClientOptions mqttClientOptions,
        IMessageDispatcher messageDispatcher,
        IMessageResolver messageResolver,
        ILogger<DeviceServiceBase> logger)
    {
        _mqttClient = mqttClient;
        _mqttClientOptions = mqttClientOptions;
        _messageDispatcher = messageDispatcher;
        _messageResolver = messageResolver;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            StartupState();

            _logger.LogInformation("Starting device. Running at {RuntimeIdentifier}", Runtime.RuntimeIdentifier);

            await _mqttClient.ConnectAsync(_mqttClientOptions,
                onMessageReceivedHandlerAsync: (mqttMessage, token) =>
                {
                    var message = _messageResolver.Resolve(mqttMessage);
                    return _messageDispatcher.Dispatch(message, mqttMessage.Topic, token);
                },
                onConnectedHandlerAsync: OnConnected,
                onDisconnectedHandlerAsync: OnDisconnected,
                cancellationToken);

            await InnerExecuteAsync(cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // We do not throw if the application shuts down.
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Application Critical Exception");
            throw;
        }
        finally
        {
            await _mqttClient.DisconnectAsync(cancellationToken);
            ShutdownState();
        }
    }

    protected async Task PublishMeasurement<T>(T message, TopicParts.Application topicApplicationPart,
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
            if (_mqttClient.IsConnected && !cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Publishing message on topic '{Topic}'", topic);
                await _mqttClient.PublishAsync(Serialization.Serialize(message), topic, cancellationToken);
                MessageSent();
            }
        }
        catch (TaskCanceledException e)
        {
            _logger.LogError(e, "Publishing message on topic '{Topic}' failed: Cancelled", topic);
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(e, "Publishing message on topic '{Topic}' failed: Cancelled", topic);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Publishing message on topic '{Topic}' failed", topic);
        }
    }

    private Task OnDisconnected(CancellationToken token)
    {
        FailedState();

        return CompletedTask;
    }

    private Task OnConnected(CancellationToken token)
    {
        SuccessState();

        return CompletedTask;
    }
    
    protected virtual Task InnerExecuteAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    protected virtual void SuccessState() => throw new NotImplementedException();
    
    protected virtual void FailedState() => throw new NotImplementedException();
    
    protected virtual void MessageSent() => throw new NotImplementedException();
    
    protected virtual void StartupState() => throw new NotImplementedException();
    
    protected virtual void ShutdownState() => throw new NotImplementedException();
}