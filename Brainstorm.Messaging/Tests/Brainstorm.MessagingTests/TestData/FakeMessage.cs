using Brainstorm.Messaging.Abstractions;

namespace Brainstorm.MessagingTests.TestData
{
    public class FakeMessage : IMessage
    {
        public FakeMessage(int version)
        {
            this.Version = version;
        }

        public int Version { get; set; }
    }
}
