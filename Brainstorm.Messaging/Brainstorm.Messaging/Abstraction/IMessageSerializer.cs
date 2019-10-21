using System.Collections.Generic;
using Brainstorm.Messaging.Abstractions;
using Microsoft.Azure.ServiceBus;

namespace Brainstorm.Messaging.Abstraction
{
    internal interface IMessageSerializer
    {
        Message Serialize<TMessage>(TMessage message)
            where TMessage : IMessage;

        TMessage Deserialize<TMessage>(Message message)
            where TMessage : IMessage;

        List<Message> SerializeBatch<TMessage>(ICollection<TMessage> messages)
            where TMessage : IMessage;

        List<TMessage> DeserializeBatch<TMessage>(ICollection<Message> messages)
            where TMessage : IMessage;
    }
}
