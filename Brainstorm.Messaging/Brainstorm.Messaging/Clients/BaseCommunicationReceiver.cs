using System.Threading.Tasks;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Abstractions.Configurations;
using Microsoft.Azure.ServiceBus.Core;

namespace Brainstorm.Messaging.Clients
{
    public abstract class BaseCommunicationReceiver : ICommunicationReceiver
    {
        protected readonly ICommunicationConfiguration Configuration;

        internal BaseCommunicationReceiver(ICommunicationConfiguration configuration, IReceiverClient receiverClient)
        {
            this.Configuration = configuration;
            this.Receiver = receiverClient;
        }

        protected BaseCommunicationReceiver(ICommunicationConfiguration configuration) 
            : this(configuration, null)
        {
        }
        
        protected IReceiverClient Receiver { get; set; }

        public virtual void RegisterHandler(IMessageHandler handler)
        {
            var proccesser = new MessageProcessor(handler);
            this.Receiver.RegisterMessageHandler(proccesser.ProcessMessagesAsync, proccesser.ProcessExceptionAsync);
        }

        public Task CompleteAsync(string lockToken)
        {
            return this.Receiver.CompleteAsync(lockToken);
        }

        public Task AbandonAsync(string lockToken)
        {
            return this.Receiver.AbandonAsync(lockToken);
        }

        public Task DeadLetterAsync(string lockToken)
        {
            return this.Receiver.DeadLetterAsync(lockToken);
        }
    }
}
