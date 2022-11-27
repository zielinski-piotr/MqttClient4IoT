using Microsoft.Extensions.Logging;
using Shared.Contracts;
using Shared.Messaging;
using Topics.Extensions;

namespace Mqtt.Supervisor.MessageHandlers;

public class PressureMessageHandler : IMessageHandler<PressureMessage>
{
    private readonly ILogger<PressureMessageHandler> _logger;

    public PressureMessageHandler(ILogger<PressureMessageHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(PressureMessage message, string sourceTopic, CancellationToken cancellationToken)
    {
        var (_, sourceDeviceId, _) = sourceTopic.ToTopicParts();
        _logger.LogInformation("Received Pressure from Device '{SourceDeviceId}' Value: {Value}", sourceDeviceId, message.Pressure);
        
        return Task.CompletedTask;
    }
}