using Microsoft.Extensions.Logging;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Topics.Extensions;

namespace Clients.Supervisor.MessageHandlers;

public class TemperatureMessageHandler : IMessageHandler<TemperatureMessage>
{
    private readonly ILogger<TemperatureMessageHandler> _logger;

    public TemperatureMessageHandler(ILogger<TemperatureMessageHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(TemperatureMessage message, string sourceTopic, CancellationToken cancellationToken)
    {
        var (_, sourceDeviceId, _) = sourceTopic.ToTopicParts();
        _logger.LogInformation("Received Temperature from Device '{SourceDeviceId}' Value: {Value}", sourceDeviceId, message.Temperature);
        
        return Task.CompletedTask;
    }
}