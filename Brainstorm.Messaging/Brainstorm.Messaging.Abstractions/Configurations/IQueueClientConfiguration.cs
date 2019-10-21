namespace Brainstorm.Messaging.Abstractions.Configurations
{
    public interface IQueueClientConfiguration : ICommunicationConfiguration
    {
        string QueueName { get; }
    }
}
