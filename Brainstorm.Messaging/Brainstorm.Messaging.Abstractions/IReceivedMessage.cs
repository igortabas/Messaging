using System.Collections.Generic;

namespace Brainstorm.Messaging.Abstractions
{
    public interface IReceivedMessage
    {
        IDictionary<string, object> UserProperties { get; }

        string LockToken { get; }

        string CorrelationId { get; }

        string MessageId { get; }

        string ReplyTo { get; }

        IMessage Message { get; }
    }
}
