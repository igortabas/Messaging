using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Clients;
using Microsoft.Azure.ServiceBus.Core;
using Moq;

namespace Brainstorm.MessagingTests.Clients
{
    public class BrainstormTopicSenderTests : BaseCommunicationSenderTests
    {
        internal override BaseCommunicationSender CreateSender(IMessageSerializer serializer, ISenderClient sender)
        {
            var configuration = new Mock<ITopicClientConfiguration>();
            return new BrainstormTopicSender(configuration.Object, serializer, sender);
        }
    }
}
