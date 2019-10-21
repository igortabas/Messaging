using Brainstorm.Messaging.Abstractions.Metadata;
using Brainstorm.Messaging.Infrastructure;

namespace Brainstorm.Messaging.Abstractions.Configurations
{
   public interface ICommunicationConfiguration
    {
        string ConnectionString { get; }

        RetryStrategy RetryStrategy { get; }

        ReceiveMode ReceiveMode { get; }
    }
}
