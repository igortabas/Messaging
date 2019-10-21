using System.Collections.Generic;
using System.Reflection;

namespace Brainstorm.Messaging.Abstractions
{
    internal interface IAssemblyProxy
    {
        IEnumerable<Assembly> GetAssemblies();
    }
}
