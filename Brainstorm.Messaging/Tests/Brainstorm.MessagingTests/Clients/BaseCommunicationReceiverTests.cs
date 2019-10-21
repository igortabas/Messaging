using System;
using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Clients;
using Brainstorm.Messaging.Infrastructure.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Moq;
using Xunit;

namespace Brainstorm.MessagingTests.Clients
{
    public abstract class BaseCommunicationReceiverTests
    {
        [Fact]
        public void RegisterHandler_MessageHandler_ShouldRegisterMessageHandler()
        {
            // arrange 
            var receiverMock = new Mock<IReceiverClient>();

            var handlerMock = new Mock<IMessageHandler>();
            handlerMock.Setup(x => x.ExceptionHandler).Returns(new Mock<IExceptionReceivedHandler>().Object);
            handlerMock.Setup(x => x.Notifier).Returns(new Mock<INotifier>().Object);
            var receiver = this.CreateReceiver(receiverMock.Object);
            receiverMock.Setup(x => x.RegisterMessageHandler(
                It.IsAny<Func<Message, CancellationToken, Task>>(),
                It.IsAny<Func<ExceptionReceivedEventArgs, Task>>()));
          
            // act
            receiver.RegisterHandler(handlerMock.Object);

            // assert
            receiverMock.Verify(
                x => x.RegisterMessageHandler(
                It.IsAny<Func<Message, CancellationToken, Task>>(),
                It.IsAny<Func<ExceptionReceivedEventArgs, Task>>()), Times.Once);
        }

        [Fact]
        public void CompleteAsync_LockToken_CompleteMessageWihtCorrespondingToken()
        {
            // arrange 
            const string lockToken = "test";
            var receiverMock = new Mock<IReceiverClient>();
            var receiver = this.CreateReceiver(receiverMock.Object);
            receiverMock.Setup(x => x.CompleteAsync(It.IsAny<string>()));

            // act
            receiver.CompleteAsync(lockToken);

            // assert
            receiverMock.Verify(x => x.CompleteAsync(It.Is<string>(token => token == lockToken)), Times.Once);
        }

        [Fact]
        public void Abandon_LockToken_AbandonMessageWihtCorrespondingToken()
        {
            // arrange 
            const string lockToken = "test";
            var receiverMock = new Mock<IReceiverClient>();
            var receiver = this.CreateReceiver(receiverMock.Object);
            receiverMock.Setup(x => x.AbandonAsync(It.IsAny<string>(), null));

            // act
            receiver.AbandonAsync(lockToken);

            // assert
            receiverMock.Verify(x => x.AbandonAsync(It.Is<string>(token => token == lockToken), null), Times.Once);
        }

        [Fact]
        public void DeadLetter_LockToken_DeadLetterMessageWihtCorrespondingToken()
        {
            // arrange 
            const string lockToken = "test";
            var receiverMock = new Mock<IReceiverClient>();
            var receiver = this.CreateReceiver(receiverMock.Object);
            receiverMock.Setup(x => x.DeadLetterAsync(It.IsAny<string>(), null));

            // act
            receiver.DeadLetterAsync(lockToken);

            // assert
            receiverMock.Verify(x => x.DeadLetterAsync(It.Is<string>(token => token == lockToken), null), Times.Once);
        }

        protected abstract BaseCommunicationReceiver CreateReceiver(IReceiverClient receiverClient);
    }
}
