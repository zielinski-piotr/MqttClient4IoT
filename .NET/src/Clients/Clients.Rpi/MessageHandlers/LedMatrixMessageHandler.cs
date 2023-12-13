using Device.Abstractions;
using Shared.Contracts;
using Shared.Messaging;

namespace Clients.Rpi.MessageHandlers;

public class LedMatrixMessageHandler : IMessageHandler<LedMatrixMessage>
{
    private readonly ILedMatrix _ledMatrix;

    public LedMatrixMessageHandler(ILedMatrix ledMatrix)
    {
        _ledMatrix = ledMatrix ?? throw new ArgumentNullException(nameof(ledMatrix));
    }
    
    public Task Handle(LedMatrixMessage message, string sourceTopic, CancellationToken cancellationToken)
    {
        if (message.Colors.Length != 64)
        {
            throw new InvalidOperationException($"Matrix is 8x8=64 pixels but {message.Colors.Length} sent");
        }
        
        _ledMatrix.SetLedMatrix(message.Colors);
        
        return Task.CompletedTask;
    }
}