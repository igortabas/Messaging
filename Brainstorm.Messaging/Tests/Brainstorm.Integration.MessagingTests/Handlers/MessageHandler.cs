using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Infrastructure.Abstractions;

namespace Brainstorm.Integration.MessagingTests.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        public List<IReceivedMessage> ReceivedMessages = new List<IReceivedMessage>();

        public IExceptionReceivedHandler ExceptionHandler { get; }

        public INotifier Notifier { get; }

        public MessageHandler(INotifier notifier, IExceptionReceivedHandler exceptionHandler)
        {
            Notifier = notifier;
            ExceptionHandler = exceptionHandler;
        }

        public Task ProcessMessagesAsync(IReceivedMessage message, CancellationToken token)
        {
            ReceivedMessages.Add(message);
            return Task.CompletedTask;
        }
    }
}
