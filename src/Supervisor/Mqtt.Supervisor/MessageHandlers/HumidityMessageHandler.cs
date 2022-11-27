using Microsoft.Extensions.Logging;
using Shared.Contracts;
using Shared.Messaging;
using Topics.Extensions;

namespace Mqtt.Supervisor.MessageHandlers;

public class HumidityMessageHandler : IMessageHandler<HumidityMessage>
{
    private readonly ILogger<HumidityMessageHandler> _logger;

    public HumidityMessageHandler(ILogger<HumidityMessageHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(HumidityMessage message, string sourceTopic, CancellationToken cancellationToken)
    {
        var (_, sourceDeviceId, _) = sourceTopic.ToTopicParts();
        _logger.LogInformation("Received Humidity from Device '{SourceDeviceId}' Value: {Value}", sourceDeviceId, message.Humidity);
        
        return Task.CompletedTask;
    }
}