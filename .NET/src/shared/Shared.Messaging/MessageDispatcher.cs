using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageDispatcher(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Dispatch(IMessage message, string sourceTopic, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var handlerType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());

            if (handlerType is null)
                throw new HandlerNotFoundException(
                    $"Handler for message type : {message.GetType().FullName} not found");

            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            return (Task)handlerType
                .GetMethod(nameof(IMessageHandler<IMessage>.Handle))!
                .Invoke(handler, new object[] { message, sourceTopic, cancellationToken });
        }
    }
}