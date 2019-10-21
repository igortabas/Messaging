using System.Collections.Generic;
using Brainstorm.Messaging.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace Brainstorm.Messaging
{
    internal sealed class ReceivedMessage : IReceivedMessage
    {
        internal ReceivedMessage(IMessage message)
        {
            this.Message = message;
        }

        internal ReceivedMessage(IMessage message, string replyTo, string messageId)
        {
            this.Message = message;
            this.MessageId = messageId;
            this.ReplyTo = replyTo;
        }

        internal ReceivedMessage(Message message, IMessage payloadMessage)
            : this(payloadMessage, message.ReplyTo, message.MessageId)
        {
            this.CorrelationId = message.CorrelationId;
            this.LockToken = message.SystemProperties.IsLockTokenSet ?
                message.SystemProperties.LockToken
                : string.Empty;
            this.UserProperties = message.UserProperties;
        }

        public IDictionary<string, object> UserProperties { get; }

        public string LockToken { get; }

        public string CorrelationId { get; }

        public string MessageId { get; }

        public string ReplyTo { get; }

        public IMessage Message { get; }
    }
}
