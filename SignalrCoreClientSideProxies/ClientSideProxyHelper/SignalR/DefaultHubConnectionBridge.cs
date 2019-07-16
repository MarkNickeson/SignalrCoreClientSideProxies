using Microsoft.AspNetCore.SignalR.Client;
using ClientSideProxyHelper.CodeGen;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientSideProxyHelper.SignalR
{
    public class DefaultHubConnectionBridge : IHubConnectionBridge
    {
        HubConnection hubConnection;

        public DefaultHubConnectionBridge(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
        }

        public async Task<object> InvokeCoreAsync(string methodName, Type returnType, object[] args, CancellationToken cancellationToken = default)
        {
            return await hubConnection.InvokeCoreAsync(methodName, returnType, args, cancellationToken);
        }

        public async Task SendCoreAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
        {
            await hubConnection.SendCoreAsync(methodName, args, cancellationToken);
        }

        public IDisposable On(string methodName, Type[] parameterTypes, Func<object[], Task> handler)
        {
            return hubConnection.On(methodName, parameterTypes, handler);
        }
    }
}
