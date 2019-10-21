using System;
using System.Collections.Generic;

namespace Brainstorm.Messaging.Abstractions
{
    internal interface ITypeProvider
    {
        List<Type> GetMessageTypes();

        Type GetMessageType(string typeName);
    }
}
