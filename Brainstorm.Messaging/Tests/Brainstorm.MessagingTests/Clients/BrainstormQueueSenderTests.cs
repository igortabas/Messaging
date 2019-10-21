using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Clients;
using Microsoft.Azure.ServiceBus.Core;
using Moq;

namespace Brainstorm.MessagingTests.Clients
{
    public class BrainstormQueueSenderTests : BaseCommunicationSenderTests
    {
        internal override BaseCommunicationSender CreateSender(IMessageSerializer serializer, ISenderClient senderClient)
        {
            var configuration = new Mock<IQueueClientConfiguration>();
            return new BrainstormQueueSender(configuration.Object, serializer, senderClient);
        }
    }
}
