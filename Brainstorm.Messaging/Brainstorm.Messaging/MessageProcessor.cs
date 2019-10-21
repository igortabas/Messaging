using System;
using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Infrastructure.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace Brainstorm.Messaging
{
    internal class MessageProcessor 
    {
        private readonly IMessageSerializer serializer;
        private readonly IVersionChecker versionChecker;

        internal MessageProcessor(
            IMessageHandler handler,
            IVersionChecker versionChecker,
            IMessageSerializer serializer)
        {
            this.Handler = handler;
            this.versionChecker = versionChecker;
            this.serializer = serializer;
        }

        internal MessageProcessor(
            IMessageHandler handler)
            : this(handler, new VersionChecker(handler.Notifier), new MessageSerializer())
        {
        }

        private IMessageHandler Handler { get; }

        internal virtual Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var payloadMessage = this.serializer.Deserialize<IMessage>(message);
            var receivedMessage = new ReceivedMessage(message, payloadMessage);
            this.versionChecker.Check(payloadMessage.Version, payloadMessage.GetType());

            return this.Handler.ProcessMessagesAsync(receivedMessage, token);
        }

        internal virtual Task ProcessExceptionAsync(EventArgs args)
        {
            return this.Handler.ExceptionHandler.ProcessException(args);
        }
    }
}
