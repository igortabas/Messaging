using System.Collections.Generic;
using System.Linq;
using Brainstorm.Messaging.Abstractions.Metadata;
using AzureReceiveMode = Microsoft.Azure.ServiceBus.ReceiveMode;

namespace Brainstorm.Messaging
{
    internal static class ReceiveModeConverter
    {
        private static Dictionary<ReceiveMode, AzureReceiveMode> mapping = new Dictionary<ReceiveMode, AzureReceiveMode>
        {
            { ReceiveMode.PeekLock, AzureReceiveMode.PeekLock },
            { ReceiveMode.ReceiveAndDelete, AzureReceiveMode.ReceiveAndDelete },
        };

        public static AzureReceiveMode Convert(ReceiveMode receive)
        {
            return mapping[receive];
        }

        public static ReceiveMode ConvertBack(AzureReceiveMode receive)
        {
            return mapping.Single(x => x.Value == receive).Key;
        }
    }
}
