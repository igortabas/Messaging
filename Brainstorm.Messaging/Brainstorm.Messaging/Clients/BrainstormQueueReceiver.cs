using System;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Brainstorm.Messaging.Clients
{
    public class BrainstormQueueReceiver : BaseCommunicationReceiver
    {
        private readonly Lazy<IQueueClient> queueClient;

        public BrainstormQueueReceiver(IQueueClientConfiguration configuration)
            : base(configuration)
        {
            this.queueClient = new Lazy<IQueueClient>(() => new QueueClient(
                configuration.ConnectionString,
                configuration.QueueName,
                ReceiveModeConverter.Convert(configuration.ReceiveMode),
                RetryConfiguration.Create(configuration.RetryStrategy)));

            this.Receiver = this.queueClient.Value;
        }

        internal BrainstormQueueReceiver(
            IQueueClientConfiguration configuration,
            IReceiverClient receiverClient)
            : base(configuration, receiverClient)
        {
        }
    }
}
