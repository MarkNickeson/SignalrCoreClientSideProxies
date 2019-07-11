using System;

namespace ProxyGen.CodeGen
{
    internal class ProxyCodeGenScope
    {
        internal string Namespace { get; }

        internal ProxyCodeGenScope()
        {
            // generate a shared scope
            Namespace = $"_{Guid.NewGuid().ToString("N")}";
        }
    }
}
