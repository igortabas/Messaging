using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Brainstorm.Messaging.Infrastructure;

namespace Brainstorm.Messaging.Abstractions
{
    public interface ICommunicationSender
    {
        Task<string> SendAsync<TMessage>(TMessage message)
            where TMessage : IMessage;

        Task<string> SendAsync<TMessage>(TMessage message, string replyTo)
            where TMessage : IMessage;

        Task<string> SendAsync<TMessage>(TMessage message, string replyTo, IDictionary<string, object> userProperties)
            where TMessage : IMessage;

        Task<string> ReplyAsync<TMessage>(TMessage message, string corellationId)
            where TMessage : IMessage;
        
        Task SendBatchAsync<TMessage>(ICollection<TMessage> message)
            where TMessage : IMessage;

        Task SendBatchAsync<TMessage>(ICollection<TMessage> message, BatchOptions options, int parallelSendersCount)
         where TMessage : IMessage;

        Task<long> ScheduleMessageAsync<TMessage>(TMessage message, DateTimeOffset scheduleEnqueueTimeUtc)
            where TMessage : IMessage;

        Task CancelScheduledMessageAsync(long sequenceNumber);

        Task CloseAsync();
    }
}
