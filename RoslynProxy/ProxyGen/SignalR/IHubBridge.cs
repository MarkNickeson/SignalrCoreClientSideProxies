using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyGen.SignalR
{
    public interface IHubBridge
    {
        Task<object> InvokeCoreAsync(string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default);
        Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default);
    }
}
