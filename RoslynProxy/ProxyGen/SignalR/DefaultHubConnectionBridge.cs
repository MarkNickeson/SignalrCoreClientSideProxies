using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyGen.SignalR
{
    public class DefaultHubConnectionBridge : IHubBridge
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
    }
}
