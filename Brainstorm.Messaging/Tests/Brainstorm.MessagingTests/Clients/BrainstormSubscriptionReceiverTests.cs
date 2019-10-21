using Brainstorm.Messaging.Abstractions.Configurations;
using Brainstorm.Messaging.Clients;
using Microsoft.Azure.ServiceBus.Core;
using Moq;

namespace Brainstorm.MessagingTests.Clients
{
    public class BrainstormSubscriptionReceiverTests : BaseCommunicationReceiverTests
    {
        protected override BaseCommunicationReceiver CreateReceiver(IReceiverClient receiverClient)
        {
            var configuration = new Mock<IQueueClientConfiguration>();
            return new BrainstormSubscriptionReceiver(configuration.Object, receiverClient);
        }
    }
}
