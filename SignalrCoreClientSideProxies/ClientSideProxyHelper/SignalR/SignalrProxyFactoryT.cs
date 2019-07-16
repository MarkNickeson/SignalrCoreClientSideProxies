using Microsoft.AspNetCore.SignalR.Client;
using ClientSideProxyHelper.CodeGen;
using System;

namespace ClientSideProxyHelper.SignalR
{
    public class ClientSideProxyFactory<TServer, TClient>
        where TClient : class
        where TServer : class
    {
        IClientMapperProxyFactory<TClient> clientMapperProxyFactory;
        IServerProxyFactory<TServer> serverProxyFactory;

        internal ClientSideProxyFactory(IServerProxyFactory<TServer> serverProxyFactory, IClientMapperProxyFactory<TClient> clientMapperProxyFactory)
        {
            this.clientMapperProxyFactory = clientMapperProxyFactory;
            this.serverProxyFactory = serverProxyFactory;
        }

        public IDisposable CreateClientMapperProxy(HubConnection hub, TClient clientImplementation)
        {
            // only purpose is to wrap hub with bridge and then instantiate 
            var bridge = new DefaultHubConnectionBridge(hub);
            return clientMapperProxyFactory.Create(bridge, clientImplementation);
        }

        public TServer CreateServerProxy(HubConnection hub)
        {
            // only purpose is to wrap hub with bridge and then instantiate target 
            var bridge = new DefaultHubConnectionBridge(hub);
            return serverProxyFactory.Create(bridge);
        }
    }
}
