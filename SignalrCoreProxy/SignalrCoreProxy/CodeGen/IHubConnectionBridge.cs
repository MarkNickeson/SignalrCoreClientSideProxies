using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalrCoreClientHelper.CodeGen
{
    public interface IHubConnectionBridge
    {
        Task<object> InvokeCoreAsync(string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default);
        Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default);
        IDisposable On(string methodName, Type[] parameterTypes, Func<object[], Task> handler);
    }
}
