using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Shared.Mqtt.Client.Abstractions.Models;
using IMqttClient = Shared.Mqtt.Client.Abstractions.IMqttClient;
using IMqttLibraryClient = MQTTnet.Client.IMqttClient;
using MqttClientOptions = MQTTnet.Client.MqttClientOptions;

namespace Shared.Mqtt.Client;

public class MqttClient : IMqttClient, IDisposable
{
    private readonly IMqttLibraryClient _client;
    private readonly ILogger<MqttClient> _logger;
    private CancellationToken _cancellationToken;

    private MqttClientOptions? Options { get; set; }
    private bool AutoReconnect { get; set; }

    public bool IsConnected => _client.IsConnected;

    public MqttClient(MqttFactory factory, ILogger<MqttClient> logger)
    {
        if (factory is null) throw new ArgumentNullException(nameof(factory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _client = factory.CreateMqttClient();
    }

    public async Task ConnectAsync(
        Abstractions.Models.MqttClientOptions mqttClientOptions,
        Func<MqttMessage, CancellationToken, Task> onMessageReceivedHandlerAsync,
        Func<CancellationToken, Task>? onConnectedHandlerAsync,
        Func<CancellationToken, Task>? onDisconnectedHandlerAsync,
        CancellationToken cancellationToken)
    {
        Options = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttClientOptions.Address, mqttClientOptions.Port)
            .WithClientId(mqttClientOptions.ClientId)
            .WithCleanSession(mqttClientOptions.CleanSession)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
            .Build();

        _cancellationToken = cancellationToken;

        SetupReconnectOnDisconnected(mqttClientOptions.AutoReconnect, onDisconnectedHandlerAsync);
        SetupSubscriptionsOnConnected(mqttClientOptions.SubscribedTopics, onConnectedHandlerAsync);
        SetupOnMessageReceivedHandler(onMessageReceivedHandlerAsync);

        await ConnectAsync();
    }

    public async Task PublishAsync(byte[] payload, string topic, CancellationToken cancellationToken)
    {
        if (_client.IsConnected)
        {
            var mqttMessage = new MqttApplicationMessage
            {
                Payload = payload,
                Topic = topic,
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
            };

            try
            {
                await _client.PublishAsync(mqttMessage, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "An error has been thrown when trying to publish message to topic '{Topic}'",
                    mqttMessage.Topic);
                throw;
            }
        }
    }

    private void SetupOnMessageReceivedHandler(Func<MqttMessage, CancellationToken, Task> onMessageReceivedHandlerAsync)
    {
        _client.ApplicationMessageReceivedAsync += async args =>
        {
            try
            {
                var message = new MqttMessage
                {
                    Payload = args.ApplicationMessage.Payload,
                    Topic = args.ApplicationMessage.Topic,
                    ResponseTopic = args.ApplicationMessage.ResponseTopic
                };

                await onMessageReceivedHandlerAsync(message, _cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "An exception '{Exception}' has been thrown when handling the message received on Topic '{Topic}'",
                    e.Message,
                    args.ApplicationMessage.Topic);

                args.ReasonCode = MqttApplicationMessageReceivedReasonCode.ImplementationSpecificError;
                args.ResponseReasonString = "An exception has been thrown when handling the message";
            }
        };
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken)
    {
        if (_client.IsConnected)
        {
            AutoReconnect = false;

            await _client.DisconnectAsync(new MqttClientDisconnectOptions(), cancellationToken);
        }
    }

    private void SetupReconnectOnDisconnected(bool autoReconnect, Func<CancellationToken, Task>? onDisconnectedHandlerAsync)
    {
        AutoReconnect = autoReconnect;

        _client.DisconnectedAsync += async args =>
        {
            _logger.LogWarning("Client disconnected from the broker. Reason: '{Reason}'", args.Reason);

            if (onDisconnectedHandlerAsync is not null)
            {
                try
                {
                    await onDisconnectedHandlerAsync.Invoke(_cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    //Suppress
                }
            }

            if (AutoReconnect)
            {
                _logger.LogInformation("The client is reconnecting in 5 seconds ...");
                await Task.Delay(5000, _cancellationToken);
                await ConnectAsync();
            }
        };
    }

    private void SetupSubscriptionsOnConnected(IEnumerable<string> subscribedTopics, Func<CancellationToken, Task>? onConnectedHandlerAsync)
    {
        var enumerable = subscribedTopics as string[] ?? subscribedTopics.ToArray();

        if (!enumerable.Any()) return;

        var options = new MqttClientSubscribeOptions
        {
            TopicFilters = new List<MqttTopicFilter>()
        };

        foreach (var topic in enumerable)
        {
            options.TopicFilters.Add(new MqttTopicFilter()
            {
                Topic = topic,
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
            });
        }

        _client.ConnectedAsync += async _ =>
        {
            if (onConnectedHandlerAsync is not null)
            {
                try
                {
                    await onConnectedHandlerAsync.Invoke(_cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    //Suppress
                }
            }
            
            _logger.LogInformation("Trying to subscribe to topics: {Topics}",
                string.Join(", ", enumerable.Select(x => $"'{x}'")));

            var subscriptionResult = await _client.SubscribeAsync(options, _cancellationToken);
            var unsuccessfulResultItems =
                subscriptionResult.Items.Where(x => x.ResultCode != MqttClientSubscribeResultCode.GrantedQoS2).ToList();

            if (unsuccessfulResultItems.Any())
            {
                _logger.LogError("Subscribing to topics failed:{Reasons}",
                    string.Join(Environment.NewLine,
                        unsuccessfulResultItems.Select(x => $"Topic: '{x.TopicFilter.Topic}' Reason: '{x.ResultCode}'")));
                return;
            }

            _logger.LogInformation("Successfully subscribed to topics: {Topics}",
                string.Join(", ", subscriptionResult.Items.Select(x => $"'{x.TopicFilter.Topic}'")));
        };
    }

    private async Task ConnectAsync()
    {
        _logger.LogInformation("Connecting to the broker '{Address}'", Options?.ChannelOptions);

        MqttClientConnectResult? result = null;

        try
        {
            result = await _client.ConnectAsync(Options, _cancellationToken);

            _logger.LogInformation("Client connection result: '{Result}'", result.ResultCode);
        }
        catch
        {
            _logger.LogError("Client connection error result: '{Result}'", result?.ResultCode ?? MqttClientConnectResultCode.UnspecifiedError);
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}