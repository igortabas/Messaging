using System;
using System.Threading.Tasks;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Clients;
using Brainstorm.MessagingTests.TestData;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Moq;
using Xunit;

namespace Brainstorm.MessagingTests.Clients
{
    public abstract class BaseCommunicationSenderTests
    {
        [Fact]
        public async Task SendAsync_Message_ShouldSerialize()
        {
            // arrange 
            var serializerMock = new Mock<IMessageSerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<IMessage>())).Returns(new Message());

            var message = new FakeMessage(1);
            var sender = this.CreateSender(serializerMock.Object, new Mock<ISenderClient>().Object);

            // act
            await sender.SendAsync(message);

            // assert
            serializerMock.Verify(x => x.Serialize(It.Is<IMessage>(msg => msg == message)), Times.Once);
        }

        [Fact]
        public async Task SendAsync_MessageWithReplyTo_ShouldSetReplyInfo()
        {
            // arrange 
            const string replyTo = "test";
            var serializerMock = new Mock<IMessageSerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<IMessage>())).Returns(new Message());
            Message actualMessage = null;
            var senderMock = new Mock<ISenderClient>();
            senderMock.Setup(x => x.SendAsync(It.IsAny<Message>())).Callback<Message>(x => actualMessage = x)
                .Returns(Task.CompletedTask);

            var message = new FakeMessage(1);
            var sender = this.CreateSender(serializerMock.Object, senderMock.Object);
            
            // act
            await sender.SendAsync(message, replyTo);

            // assert
            Assert.Equal(actualMessage.ReplyTo, replyTo);
        }

        [Fact]
        public async Task SendAsync_MessageWithReplyTo_ShouldSetCorrelationId()
        {
            // arrange 
            const string corellationId = "test";
            var serializerMock = new Mock<IMessageSerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<IMessage>())).Returns(new Message());
            Message actualMessage = null;
            var senderMock = new Mock<ISenderClient>();
            senderMock.Setup(x => x.SendAsync(It.IsAny<Message>())).Callback<Message>(x => actualMessage = x)
                .Returns(Task.CompletedTask);

            var message = new FakeMessage(1);
            var sender = this.CreateSender(serializerMock.Object, senderMock.Object);

            // act
            await sender.ReplyAsync(message, corellationId);

            // assert
            Assert.Equal(actualMessage.CorrelationId, corellationId);
        }

        [Fact]
        public async Task SendBatchAsync_Messages_ShouldSerialize()
        {
            // arrange 
            var serializerMock = new Mock<IMessageSerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<IMessage>())).Returns(new Message());
            var senderMock = new Mock<ISenderClient>();
            senderMock.Setup(x => x.SendAsync(It.IsAny<Message>()))
                .Returns(Task.CompletedTask);
            var messages = new[] { new FakeMessage(1), new FakeMessage(1) };
            var sender = this.CreateSender(serializerMock.Object, senderMock.Object);

            // act
            await sender.SendBatchAsync(messages);

            // assert
            serializerMock.Verify(x => x.Serialize(It.IsAny<FakeMessage>()), Times.Exactly(messages.Length));
        }

        [Fact]
        public async Task ScheduleMessageAsync_Message_ShouldSerializeAndSchedule()
        {
            // arrange 
            var serializerMock = new Mock<IMessageSerializer>();
            var serializeMessage = new Message();
            serializerMock.Setup(x => x.Serialize(It.IsAny<IMessage>())).Returns(serializeMessage);

            var senderMock = new Mock<ISenderClient>();
            var fakeSequenceNumber = 10L;
            senderMock.Setup(x => x.ScheduleMessageAsync(It.IsAny<Message>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(fakeSequenceNumber);

            var message = new FakeMessage(1);
            var sender = this.CreateSender(serializerMock.Object, senderMock.Object);
            DateTimeOffset scheduledTime = DateTime.UtcNow;

            // act
            await sender.ScheduleMessageAsync(message, scheduledTime);

            // assert
            serializerMock.Verify(x => x.Serialize(It.Is<IMessage>(msg => msg == message)), Times.Once);
            senderMock.Verify(x => x.ScheduleMessageAsync(It.Is<Message>(msg => msg == serializeMessage), It.Is<DateTimeOffset>(date => date == scheduledTime)), Times.Once);
        }

        [Fact]
        public async Task CancelScheduledMessageAsync_Message_ShouldCanclecheduleMessage()
        {
            // arrange 
            var senderMock = new Mock<ISenderClient>();
            var fakeSequenceNumber = 10L;
            senderMock.Setup(x => x.ScheduleMessageAsync(It.IsAny<Message>(), It.IsAny<DateTime>())).ReturnsAsync(fakeSequenceNumber);

            var sender = this.CreateSender(new Mock<IMessageSerializer>().Object, senderMock.Object);

            // act
            await sender.CancelScheduledMessageAsync(fakeSequenceNumber);

            // assert
            senderMock.Verify(x => x.CancelScheduledMessageAsync(It.Is<long>(num => num == fakeSequenceNumber)), Times.Once);
        }

        [Fact]
        public async Task CloseAsync_ShouldCallCloseAsyncOnSender()
        {
            // arrange 
            var senderMock = new Mock<ISenderClient>();
            senderMock.Setup(x => x.CloseAsync()).Returns(Task.CompletedTask);
            var sender = this.CreateSender(new Mock<IMessageSerializer>().Object, senderMock.Object);

            // act
            await sender.CloseAsync();

            // assert
            senderMock.Verify(x => x.CloseAsync(), Times.Once);
        }

        internal abstract BaseCommunicationSender CreateSender(IMessageSerializer serializer, ISenderClient sender);
    }
}
