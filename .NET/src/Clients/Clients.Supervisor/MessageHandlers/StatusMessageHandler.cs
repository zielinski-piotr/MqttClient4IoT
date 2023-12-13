using Microsoft.Extensions.Logging;
using Shared.Contracts;
using Shared.Messaging;
using Shared.Mqtt.Topics.Extensions;

namespace Clients.Supervisor.MessageHandlers;

public class StatusMessageHandler: IMessageHandler<StatusMessage>
{
    private readonly ILogger<StatusMessageHandler> _logger;

    public StatusMessageHandler(ILogger<StatusMessageHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(StatusMessage message, string sourceTopic, CancellationToken cancellationToken)
    {
        var (_, sourceDeviceId, _) = sourceTopic.ToTopicParts();
        _logger.LogInformation("Received Status from Device '{SourceDeviceId}' Value: {Value}", sourceDeviceId, message.Status);
        
        return Task.CompletedTask;
    }
    
}