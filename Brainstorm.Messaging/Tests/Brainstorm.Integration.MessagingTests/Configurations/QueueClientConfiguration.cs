using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Abstractions.Metadata;
using Brainstorm.Messaging.Infrastructure;

namespace Brainstorm.Integration.MessagingTests.Configurations
{
    internal class QueueClientConfiguration : IQueueClientConfiguration
    {
        public QueueClientConfiguration(string connectionString, ReceiveMode receiveMode, string queueName)
        {
            this.ConnectionString = connectionString;
            this.ReceiveMode = receiveMode;
            this.QueueName = queueName;
            this.RetryStrategy = RetryStrategy.CreateDefault();
        }

        public string ConnectionString { get; }
        public RetryStrategy RetryStrategy { get; }
        public ReceiveMode ReceiveMode { get; }
        public string QueueName { get; }
    }
}
