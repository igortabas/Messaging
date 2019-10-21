using System;
using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Messaging;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Infrastructure.Abstractions;
using Brainstorm.MessagingTests.TestData;
using Microsoft.Azure.ServiceBus;
using Moq;
using Xunit;

namespace Brainstorm.MessagingTests
{
    public class MessageProcessorTests
    {
        [Fact]
        public async Task ProcessMessagesAsync_Message_VerifyVerison()
        {
            // assert
            const int testVersion = 1;
            Message testMesage = new Message();
            var deserializedMessage = new FakeMessage(testVersion);
            var handlerMock = new Mock<IMessageHandler>();
            var messageSerialzier = new Mock<IMessageSerializer>();
            messageSerialzier.Setup(x => x.Deserialize<IMessage>(It.IsAny<Message>()))
                .Returns(deserializedMessage);
            var versionChecker = new Mock<IVersionChecker>();
            versionChecker.Setup(x => x.Check(It.IsAny<int>(), It.IsAny<Type>())).Callback<int, Type>((y, t) =>
             {
             });
            var handler = new MessageProcessor(handlerMock.Object, versionChecker.Object, messageSerialzier.Object);

            // act
            await handler.ProcessMessagesAsync(testMesage, CancellationToken.None);

            // arrange
            versionChecker.Verify(x => x.Check(It.Is<int>(ver => ver == testVersion), It.Is<Type>(type => type == deserializedMessage.GetType())), Times.Once);
            messageSerialzier.Verify(x => x.Deserialize<IMessage>(It.Is<Message>(msg => msg == testMesage)), Times.Once);
        }
    }
}
