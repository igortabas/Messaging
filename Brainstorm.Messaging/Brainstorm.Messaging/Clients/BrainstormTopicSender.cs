using System;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Brainstorm.Messaging.Clients
{
    public class BrainstormTopicSender : BaseCommunicationSender
    {
        private readonly Lazy<ITopicClient> topicClient;

        public BrainstormTopicSender(ITopicClientConfiguration configuration)
            : base(configuration)
        {
            this.topicClient = new Lazy<ITopicClient>(() => new TopicClient(
                configuration.ConnectionString,
                configuration.TopicName,
                RetryConfiguration.Create(configuration.RetryStrategy)));

            this.Sender = this.topicClient.Value;
        }

        internal BrainstormTopicSender(
            ITopicClientConfiguration configuration, IMessageSerializer serializer, 
            ISenderClient senderClient)
            : base(configuration, serializer, senderClient)
        {
        }
    }
}
