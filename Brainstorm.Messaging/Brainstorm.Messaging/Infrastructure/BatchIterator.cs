using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;

namespace Brainstorm.Messaging.Infrastructure
{
    internal static class BatchIterator
    {
        public static IEnumerable<List<Message>> Batch(this IEnumerable<Message> messages, BatchOptions options)
        {
            return messages.Batch(options, msg => msg.Body?.Length ?? 0);
        }
    }
}
