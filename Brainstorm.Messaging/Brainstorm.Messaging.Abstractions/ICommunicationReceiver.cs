using System.Threading.Tasks;

namespace Brainstorm.Messaging.Abstractions
{
    public interface ICommunicationReceiver
    {
        void RegisterHandler(IMessageHandler handler);

        Task CompleteAsync(string lockToken);

        Task AbandonAsync(string lockToken);

        Task DeadLetterAsync(string lockToken);
    }
}
