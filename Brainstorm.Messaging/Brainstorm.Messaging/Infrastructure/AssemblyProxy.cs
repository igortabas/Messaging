using System;
using System.Collections.Generic;
using System.Reflection;
using Brainstorm.Messaging.Abstractions;

namespace Brainstorm.Messaging.Infrastructure
{
    internal sealed class AssemblyProxy : IAssemblyProxy
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
