using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Brainstorm.Messaging.Clients
{
    public abstract class BaseCommunicationSender : ICommunicationSender
    {
        protected readonly ICommunicationConfiguration Configuration;
        private readonly IMessageSerializer serializer;

        internal BaseCommunicationSender(
            ICommunicationConfiguration configuration,
            IMessageSerializer serializer,
            ISenderClient senderClient)
        {
            this.Configuration = configuration;
            this.serializer = serializer;
            this.Sender = senderClient;
        }

        protected BaseCommunicationSender(
            ICommunicationConfiguration configuration)
            : this(configuration, new MessageSerializer(), null)
        {
        }

        protected ISenderClient Sender { get; set; }

        public virtual Task<string> SendAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            return this.SendAsync(message, string.Empty, null);
        }

        public virtual Task<string> SendAsync<TMessage>(TMessage message, string replyTo)
            where TMessage : IMessage
        {
            return this.SendAsync(message, replyTo, null);
        }

        public Task<string> SendAsync<TMessage>(TMessage message, string replyTo, IDictionary<string, object> userProperties) 
            where TMessage : IMessage
        {
            var serviceBusMessage = this.serializer.Serialize(message);

            if (userProperties != null)
            {
                foreach (var property in userProperties)
                {
                    serviceBusMessage.UserProperties.Add(property);
                }
            }

            if (!string.IsNullOrEmpty(replyTo))
            {
                serviceBusMessage.ReplyTo = replyTo;
            }

            return this.SendAsyncInternal(serviceBusMessage);
        }

        public Task<string> ReplyAsync<TMessage>(TMessage message, string corellationId)
            where TMessage : IMessage
        {
            var serviceBusMessage = this.serializer.Serialize(message);
            serviceBusMessage.CorrelationId = corellationId;
            return this.SendAsyncInternal(serviceBusMessage);
        }

        public Task SendBatchAsync<TMessage>(ICollection<TMessage> message, BatchOptions options, int parallelSendersCount)
            where TMessage : IMessage
        {
            return this.SendBatchAsync(message, parallelSendersCount, options);
        }

        /// <summary>
        /// The send batch with standard SB batch configuration.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendBatchAsync<TMessage>(ICollection<TMessage> message)
            where TMessage : IMessage
        {
            const int parallelSendersCount = 10;
            return this.SendBatchAsync(message, parallelSendersCount, new StandardBatchOptions());
        }

        public virtual Task<long> ScheduleMessageAsync<TMessage>(TMessage message, DateTimeOffset scheduleEnqueueTimeUtc)
            where TMessage : IMessage
        {
            var serviceBusMessage = this.serializer.Serialize(message);
            return this.Sender.ScheduleMessageAsync(serviceBusMessage, scheduleEnqueueTimeUtc);
        }

        public virtual Task CancelScheduledMessageAsync(long sequenceNumber)
        {
            return this.Sender.CancelScheduledMessageAsync(sequenceNumber);
        }

        public Task CloseAsync()
        {
            return this.Sender.CloseAsync();
        }

        private async Task SendBatchAsync<TMessage>(ICollection<TMessage> messages, int parallelSendersCount, BatchOptions batchOptions)
            where TMessage : IMessage
        {
            var serviceBusMessagBatches = messages
                .Select(message => this.serializer.Serialize(message)).Batch(batchOptions);

            var senders = new List<Task>(parallelSendersCount);
            foreach (var batch in serviceBusMessagBatches)
            {
                if (senders.Count >= parallelSendersCount)
                {
                    await Task.WhenAll(senders);
                    senders.Clear();
                }

                senders.Add(this.Sender.SendAsync(batch));
            }
        }

        private async Task<string> SendAsyncInternal(Message serviceBusMessage)
        {
            serviceBusMessage.MessageId = Guid.NewGuid().ToString("N");
            await this.Sender.SendAsync(serviceBusMessage);
            return serviceBusMessage.MessageId;
        }
    }
}