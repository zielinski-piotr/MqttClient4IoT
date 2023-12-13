using Shared.Messaging;

namespace Shared.Contracts
{
    public class StatusMessage : IMessage
    {
        public ConnectionStatus Status { get; set; }
    }
}