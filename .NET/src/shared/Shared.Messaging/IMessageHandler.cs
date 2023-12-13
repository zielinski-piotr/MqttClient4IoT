using System.Threading;
using System.Threading.Tasks;

namespace Shared.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage: class, IMessage
    {
        public Task Handle(TMessage message, string sourceTopic, CancellationToken cancellationToken);
    }
}