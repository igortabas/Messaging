using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Infrastructure;

namespace Brainstorm.Integration.MessagingTests
{
    [MessageVersion(1)]
    internal class Message : IMessage
    {
        public string Data { get; set; }

        public int Version => 1;
    }
}
