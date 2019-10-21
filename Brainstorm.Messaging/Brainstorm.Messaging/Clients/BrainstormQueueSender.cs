using System;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Brainstorm.Messaging.Clients
{
    public class BrainstormQueueSender : BaseCommunicationSender
    {
        private readonly Lazy<IQueueClient> queueClient;

        public BrainstormQueueSender(IQueueClientConfiguration configuration) 
            : base(configuration)
        {
            this.queueClient = new Lazy<IQueueClient>(() => new QueueClient(
                configuration.ConnectionString,
                configuration.QueueName,
                ReceiveModeConverter.Convert(configuration.ReceiveMode),
                RetryConfiguration.Create(configuration.RetryStrategy)));

            this.Sender = this.queueClient.Value;
        }

        internal BrainstormQueueSender(
            IQueueClientConfiguration configuration,
            IMessageSerializer serializer, ISenderClient senderClient)
            : base(configuration, serializer, senderClient)
        {
        }
    }
}
