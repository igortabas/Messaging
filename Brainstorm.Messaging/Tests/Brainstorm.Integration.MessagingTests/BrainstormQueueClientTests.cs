using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Integration.MessagingTests.Configurations;
using Brainstorm.Integration.MessagingTests.Handlers;
using Brainstorm.Messaging.Abstractions.Metadata;
using Brainstorm.Messaging.Clients;
using Microsoft.ServiceBus;
using Xunit;

namespace Brainstorm.Integration.MessagingTests
{
    public class BrainstormQueueClientTests : IDisposable
    {
        private static readonly int TestRunTime = Configuration.TestDurationTimeSec * 1000;
        private readonly NamespaceManager namespaceManager;
        private readonly string queueName = "bsiqctest-" + Configuration.QueueName;
        private readonly QueueClientConfiguration configs;
        private bool disposed = false;

        public BrainstormQueueClientTests()
        {
            var connectionString = Configuration.ServiceBusConnectionString;
            namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (namespaceManager.QueueExists(queueName))
            {
                namespaceManager.DeleteQueue(queueName);
            }

            namespaceManager.CreateQueue(queueName);

            this.configs = new QueueClientConfiguration(connectionString, ReceiveMode.PeekLock, queueName);
        }


        [Fact]
        public async Task BrainstormQueueClient_SendAndReceiveMessage()
        {
            var brainstormSenderClient = new BrainstormQueueSender(this.configs);
            var receiver = new BrainstormQueueReceiver(this.configs);
            var handler = new MessageHandler(new Notifier(),new ExceptionReceivedHandler());
            receiver.RegisterHandler(handler);
            var message = new Message { Data = "test" };
            var messageId =   await brainstormSenderClient.SendAsync(message);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TestRunTime);
            while (true)
            {
                if (cts.IsCancellationRequested)
                {
                    throw new InvalidOperationException("Duration time exceeded");
                }

                if (handler.ReceivedMessages.Count > 0)
                {
                    var receivedMessage = handler.ReceivedMessages.Single();
                    Assert.IsType<Message>(receivedMessage.Message);
                    Assert.Equal(message.Data, ((Message)receivedMessage.Message).Data);
                    break;
                }
            }
            Assert.NotEmpty(messageId);
        }

        ~BrainstormQueueClientTests()
        {
            this.Dispose();
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (namespaceManager.QueueExists(queueName))
            {
                namespaceManager.DeleteQueue(queueName);
            }
        }
    }
}
