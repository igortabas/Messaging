using System;
using System.Collections.Generic;
using System.Linq;
using Brainstorm.Messaging.Abstraction;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Infrastructure;
using Brainstorm.Messaging.Metadata;
using Microsoft.Azure.ServiceBus;

namespace Brainstorm.Messaging
{
    public class MessageSerializer : IMessageSerializer
    {
        private readonly ITypeProvider typeProvider;

        private readonly Serializer serializer;

        public MessageSerializer()
            : this(new TypeProvider())
        {
        }

        internal MessageSerializer(ITypeProvider typeProvider)
        {
            this.typeProvider = typeProvider;

            this.serializer = new Serializer();
        }

        public List<Message> SerializeBatch<TMessage>(ICollection<TMessage> messages)
            where TMessage : IMessage
        {
            return messages.Select(message => this.Serialize(message)).ToList();
        }

        public List<TMessage> DeserializeBatch<TMessage>(ICollection<Message> messages)
            where TMessage : IMessage
        {
            List<TMessage> result = new List<TMessage>(messages.Count);
            foreach (var message in messages)
            {
                result.Add(this.Deserialize<TMessage>(message));
            }

            return result;
        }

        public TMessage Deserialize<TMessage>(Message message)
            where TMessage : IMessage
        {
            var typeName = (string)message.UserProperties[CustomMessageProperties.MessageTypeProperty];
            Type bodyType = this.typeProvider.GetMessageType(typeName);

            return this.serializer.Deserialize<TMessage>(message.Body, bodyType);
        }

        public Message Serialize<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var messageBody = this.serializer.Serialize(message);
            var messageType = message.GetType().FullName;
            var sbMessage = new Message(messageBody);
            sbMessage.UserProperties.Add(CustomMessageProperties.MessageTypeProperty, messageType);
            return sbMessage;
        }
    }
}
