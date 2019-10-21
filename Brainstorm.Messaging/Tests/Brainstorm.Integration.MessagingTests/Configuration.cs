using System;
using Microsoft.Extensions.Configuration;

namespace Brainstorm.Integration.MessagingTests
{
    internal static class Configuration
    {
        public static string ServiceBusConnectionString => configRoot.Value[ServiceBusConnectionStringKey];

        public static string QueueName => configRoot.Value[BaseQueueNameKey];

        public static string TopicName => configRoot.Value[BaseTopicNameKey];

        public static int TestDurationTimeSec => Convert.ToInt32(configRoot.Value[TestDurationTimeSecKey]);

        private const string ServiceBusConnectionStringKey = "ServiceBusConnectionString";

        private const string BaseQueueNameKey = "QueueName";

        private const string BaseTopicNameKey = "TopicName";

        private const string TestDurationTimeSecKey = "TestDurationTimeSec";

        private static readonly Lazy<IConfigurationRoot> configRoot = new Lazy<IConfigurationRoot>(() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build());
    }
}
