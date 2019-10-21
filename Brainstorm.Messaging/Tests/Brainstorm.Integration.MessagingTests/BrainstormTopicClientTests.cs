using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brainstorm.Integration.MessagingTests.Configurations;
using Brainstorm.Integration.MessagingTests.Handlers;
using Brainstorm.Messaging.Clients;
using Microsoft.ServiceBus;
using Xunit;
using ReceiveMode = Brainstorm.Messaging.Abstractions.Metadata.ReceiveMode;

namespace Brainstorm.Integration.MessagingTests
{
    public class BrainstormTopicClientTests : IDisposable
    {
        private static readonly int TestRunTime = Configuration.TestDurationTimeSec * 1000;
        private readonly NamespaceManager namespaceManager;
        private readonly string topicName = "bsitoptest-" + Configuration.TopicName;
        private readonly string subscriptionName = "bsiqsbcest-" + Configuration.TopicName;
        private readonly TopicClientConfiguration configs;
        private bool disposed = false;

        public BrainstormTopicClientTests()
        {
            var connectionString = Configuration.ServiceBusConnectionString;
            namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (namespaceManager.TopicExists(topicName))
            {
                namespaceManager.DeleteTopic(topicName);
            }
            namespaceManager.CreateTopic(topicName);

            if (namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                namespaceManager.DeleteSubscription(topicName, subscriptionName);
            }

            namespaceManager.CreateSubscription(topicName, subscriptionName);

            this.configs = new TopicClientConfiguration(connectionString, ReceiveMode.PeekLock, topicName, subscriptionName);
        }

        [Fact]
        public async Task BrainstormTopicClient_SendAndReceiveMessage()
        {
            var brainstormSenderClient = new BrainstormTopicSender(this.configs);
            var receiver = new BrainstormSubscriptionReceiver(this.configs);
            var handler = new MessageHandler(new Notifier(),new ExceptionReceivedHandler());
            receiver.RegisterHandler(handler);
            var message = new Message { Data = "test" };
            var messageId = await brainstormSenderClient.SendAsync(message);
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

        ~BrainstormTopicClientTests()
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

            if (namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                namespaceManager.DeleteSubscription(topicName, subscriptionName);
            }

            if (namespaceManager.TopicExists(topicName))
            {
                namespaceManager.DeleteTopic(topicName);
            }
        }
    }
}
