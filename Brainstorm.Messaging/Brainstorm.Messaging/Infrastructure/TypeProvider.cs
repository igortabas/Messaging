using System;
using System.Collections.Generic;
using System.Linq;
using Brainstorm.Messaging.Abstractions;

namespace Brainstorm.Messaging.Infrastructure
{
    internal class TypeProvider : ITypeProvider
    {
        private const string AssemblyIdentifier = "Messaging";
        private readonly IAssemblyProxy assemblyProxy;

        public TypeProvider()
            : this(new AssemblyProxy())
        {
        }

        internal TypeProvider(IAssemblyProxy assemblyProxy)
        {
            this.assemblyProxy = assemblyProxy;
        }

        public List<Type> GetMessageTypes()
        {
            var msgTypes = this.assemblyProxy.GetAssemblies()
                .Where(x => x.FullName.Contains(AssemblyIdentifier))
                .SelectMany(x => x.GetTypes());

            return msgTypes.ToList();
        }

        public Type GetMessageType(string typeName)
        {
            var types = this.GetMessageTypes();
            var msgType = types.FirstOrDefault(x => x.FullName == typeName);

            return msgType;
        }
    }
}
