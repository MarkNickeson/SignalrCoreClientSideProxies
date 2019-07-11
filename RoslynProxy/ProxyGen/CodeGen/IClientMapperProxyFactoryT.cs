using System;

namespace ProxyGen.CodeGen
{
    public interface IClientMapperProxyFactory<T> where T: class
    {
        IDisposable Create(IHubConnectionBridge hub, T clientImplementation);
    }
}
