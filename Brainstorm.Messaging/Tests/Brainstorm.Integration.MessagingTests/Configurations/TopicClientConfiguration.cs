using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Abstractions.Metadata;
using Brainstorm.Messaging.Infrastructure;

namespace Brainstorm.Integration.MessagingTests.Configurations
{
    public class TopicClientConfiguration : ITopicClientConfiguration
    {
        public TopicClientConfiguration(string connectionString, ReceiveMode receiveMode, string topicName, string subscriptionName){
            this.ConnectionString = connectionString;
            this.ReceiveMode = receiveMode;
            this.TopicName = topicName;
            this.SubscriptionName = subscriptionName;
            this.RetryStrategy = RetryStrategy.CreateDefault();
        }

        public string ConnectionString { get; }
        public RetryStrategy RetryStrategy { get; }
        public ReceiveMode ReceiveMode { get; }
        public string TopicName { get; }
        public string SubscriptionName { get; }
    }
}
