namespace Brainstorm.Messaging.Abstractions.Configurations
{
    public interface ITopicClientConfiguration : ICommunicationConfiguration
    {
        string TopicName { get; }

        string SubscriptionName { get; }
    }
}
