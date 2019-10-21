using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Messaging.Infrastructure.Abstractions;

namespace Brainstorm.Messaging.Abstractions
{
   public interface IMessageHandler
    {
        Task ProcessMessagesAsync(IReceivedMessage message, CancellationToken token);

        IExceptionReceivedHandler ExceptionHandler { get; }

        INotifier Notifier { get; }
    }
}
