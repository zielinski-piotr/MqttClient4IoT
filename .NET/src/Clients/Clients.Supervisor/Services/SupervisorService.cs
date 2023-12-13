using Microsoft.Extensions.Hosting;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Client.Abstractions;
using Shared.Mqtt.Client.Abstractions.Models;
using Shared.Mqtt.Topics;

namespace Clients.Supervisor.Services;

public class SupervisorService : IHostedService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;
    private readonly IMessageDispatcher _messageDispatcher;
    private readonly IMessageResolver _messageResolver;

    public SupervisorService(
        IMqttClient mqttClient,
        MqttClientOptions mqttClientOptions,
        IMessageDispatcher messageDispatcher,
        IMessageResolver messageResolver)
    {
        _mqttClient = mqttClient;
        _mqttClientOptions = mqttClientOptions;
        _messageDispatcher = messageDispatcher;
        _messageResolver = messageResolver;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.ConnectAsync(_mqttClientOptions, OnMessageReceivedHandlerAsync, null, null, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(5000, cancellationToken);
            await PublishFace(cancellationToken);
        }
    }

    private async Task PublishFace(CancellationToken cancellationToken)
    {
        if (_mqttClient.IsConnected)
        {
            try
            {
                var topic = TopicBuilder.Create()
                    .WithType(TopicParts.Type.Sensor)
                    .WithDevice("RaspberryPi") // update this to "123" if you want to send emoji to mocked device
                    .WithApplication(TopicParts.Application.LedMatrix)
                    .Build();
                
                var message = Serialization.Serialize(new LedMatrixMessage
                {
                    Colors = Emoji.Emoji.PickFace(true)
                });

                await _mqttClient.PublishAsync(message, topic, cancellationToken);
            }
            catch
            {
                // We don't care
            }
        }
    }

    private Task OnMessageReceivedHandlerAsync(MqttMessage mqttMessage, CancellationToken cancellationToken)
    {
        var message = _messageResolver.Resolve(mqttMessage);
        return _messageDispatcher.Dispatch(message, mqttMessage.Topic, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _mqttClient.DisconnectAsync(cancellationToken);
    }
}