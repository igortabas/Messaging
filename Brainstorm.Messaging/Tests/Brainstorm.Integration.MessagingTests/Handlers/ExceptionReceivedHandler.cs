using System;
using System.Threading.Tasks;
using Brainstorm.Messaging.Abstractions;

namespace Brainstorm.Integration.MessagingTests.Handlers
{
    internal class ExceptionReceivedHandler:IExceptionReceivedHandler
    {
        public Task ProcessException(EventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
