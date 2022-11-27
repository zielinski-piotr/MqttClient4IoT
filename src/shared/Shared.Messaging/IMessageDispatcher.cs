using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Messaging
{
    public interface IMessageDispatcher
    {
        Task Dispatch(IMessage message, string sourceTopic, CancellationToken cancellationToken);
    }
}