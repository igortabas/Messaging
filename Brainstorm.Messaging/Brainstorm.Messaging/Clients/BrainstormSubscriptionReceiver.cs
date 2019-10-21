using System;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Brainstorm.Messaging.Clients
{
    public class BrainstormSubscriptionReceiver : BaseCommunicationReceiver
    {
        private readonly Lazy<ISubscriptionClient> subscriptionClient;

        public BrainstormSubscriptionReceiver(ITopicClientConfiguration configuration)
            : base(configuration)
        {
            this.subscriptionClient = new Lazy<ISubscriptionClient>(() => new SubscriptionClient(
                configuration.ConnectionString, 
                configuration.TopicName,
                configuration.SubscriptionName,
                ReceiveModeConverter.Convert(configuration.ReceiveMode),
                RetryConfiguration.Create(configuration.RetryStrategy)));

            this.Receiver = this.subscriptionClient.Value;
        }

        internal BrainstormSubscriptionReceiver(
            IQueueClientConfiguration configuration,
            IReceiverClient receiverClient)
            : base(configuration, receiverClient)
        {
        }
    }
}
