using System;
using System.Threading.Tasks;

namespace Brainstorm.Messaging.Abstractions
{
   public interface IExceptionReceivedHandler
    {
        Task ProcessException(EventArgs args);
    }
}
