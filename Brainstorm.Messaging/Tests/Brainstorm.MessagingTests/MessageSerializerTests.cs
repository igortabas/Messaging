using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Brainstorm.Messaging;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Metadata;
using Brainstorm.MessagingTests.TestData;
using Microsoft.Azure.ServiceBus;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Brainstorm.MessagingTests
{
    public class MessageSerializerTests
    {
        [Fact]
        public void Serialize_Message_ReturnsConfiguratedMessage()
        {
            // arrange
            var serializer = new MessageSerializer();
            var message = new FakeMessage(1);

            // act
            var brokeredMessage = serializer.Serialize(message);
            var body = new MemoryStream(brokeredMessage.Body);

            // assert
            Assert.Equal(brokeredMessage.UserProperties[CustomMessageProperties.MessageTypeProperty], message.GetType().FullName);
            Assert.NotNull(body);
        }

        [Fact]
        public void Deserialize_EmptyMessageWithoutMessageType_ThrowsException()
        {
            // arrange
            var serializer = new MessageSerializer();
            var emptyBrokeredMessage = new Message();

            // act/assert
            Assert.Throws<KeyNotFoundException>(() => serializer.Deserialize<IMessage>(emptyBrokeredMessage));
        }

        [Fact]
        public void Deserialize_EmptyMessage_ThrowsException()
        {
            // arrange
            var typeProvide = new Mock<ITypeProvider>();
            typeProvide.Setup(x => x.GetMessageType(It.IsAny<string>())).Returns(typeof(FakeMessage));
            var serializer = new MessageSerializer(typeProvide.Object);
            var emptyBrokeredMessage = new Message();
            emptyBrokeredMessage.UserProperties.Add(CustomMessageProperties.MessageTypeProperty, "test");

            // act/assert
            Assert.Throws<ArgumentNullException>(() => serializer.Deserialize<IMessage>(emptyBrokeredMessage));
        }

        [Fact]
        public void Deserialize_Message_ReturnsMessage()
        {
            // arrange
            var typeProvide = new Mock<ITypeProvider>();
            typeProvide.Setup(x => x.GetMessageType(It.IsAny<string>())).Returns(typeof(FakeMessage));
            var serializer = new MessageSerializer(typeProvide.Object);
            var expctedMesage = new FakeMessage(2);
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expctedMesage)));
            var msgType = typeof(FakeMessage).FullName;
            message.UserProperties.Add(CustomMessageProperties.MessageTypeProperty, msgType);

            // act
            var actual = serializer.Deserialize<FakeMessage>(message);

            // assert
            typeProvide.Verify(x => x.GetMessageType(It.Is<string>(t => t == msgType)), Times.Once);
            Assert.Equal(actual.Version, expctedMesage.Version);
        }

        [Fact]
        public void DeserializeBatch_Messages_ReturnsMessage()
        {
            // arrange
            var typeProvide = new Mock<ITypeProvider>();
            typeProvide.Setup(x => x.GetMessageType(It.IsAny<string>())).Returns(typeof(FakeMessage));

            var serializer = new MessageSerializer(typeProvide.Object);
            var expctedMesages = new List<FakeMessage> { new FakeMessage(2), new FakeMessage(1) };
            var msgType = typeof(FakeMessage).FullName;
            var messages = expctedMesages.Select(msg =>
            {
                var res = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)));
                res.UserProperties.Add(CustomMessageProperties.MessageTypeProperty, msgType);
                return res;
            }).ToList();

            // act
            var actualMessages = serializer.DeserializeBatch<FakeMessage>(messages);

            // assert
            typeProvide.Verify(x => x.GetMessageType(It.Is<string>(t => t == msgType)), Times.Exactly(expctedMesages.Count));

            for (int i = 0; i < actualMessages.Count; i++)
            {
                var actual = actualMessages[i];
                var expctedMesage = expctedMesages[i];
                Assert.Equal(actual.Version, expctedMesage.Version);
            }
        }

        [Fact]
        public void DeserializeBatch_EmptyCollectionOfMessages_ReturnEmptyCollection()
        {
            // arrange
            var typeProvide = new Mock<ITypeProvider>();
            var serializer = new MessageSerializer(typeProvide.Object);

            // act/
            var actualMessages = serializer.DeserializeBatch<FakeMessage>(new List<Message>());

            // assert
            Assert.Empty(actualMessages);
        }

        [Fact]
        public void SerializeBatch_Message_ReturnConfiguratedMessage()
        {
            // arrange
            var serializer = new MessageSerializer();
            var messages = new List<FakeMessage> { new FakeMessage(1), new FakeMessage(1) };

            // act
            var actualMessages = serializer.SerializeBatch(messages);

            // assert
            for (int i = 0; i < actualMessages.Count; i++)
            {
                var message = actualMessages[i];
                var expMessage = messages[i];
                Assert.Equal(
                    message.UserProperties[CustomMessageProperties.MessageTypeProperty],
                    expMessage.GetType().FullName);
                Assert.NotNull(message.Body);
            }
        }

        [Fact]
        public void SerializeBatch_EmptyCollectionOfMessages_ReturnEmptyCollection()
        {
            // arrange
            var serializer = new MessageSerializer();

            // act
            var actualMessages = serializer.SerializeBatch(new List<FakeMessage>());

            // assert
            Assert.Empty(actualMessages);
        }
    }
}
