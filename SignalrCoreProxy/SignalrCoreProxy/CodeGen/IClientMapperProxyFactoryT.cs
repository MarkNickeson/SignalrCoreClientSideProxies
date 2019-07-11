using System;

namespace SignalrCoreProxy.CodeGen
{
    public interface IClientMapperProxyFactory<T> where T: class
    {
        IDisposable Create(IHubConnectionBridge hub, T clientImplementation);
    }
}
