using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    public interface IClient
    {
        Task Foo2();
        Task<int> Foo3();
        Task Foo5(int arg);
        Task<int> Foo6(int arg);
    }

    public class MapperFactory
    {
        public Mapper Create(HubConnection hub, IClient client)
        {
            return new Mapper(hub, client);
        }
    }

    public class Mapper : IDisposable
    {
        HubConnection hub;
        IClient client;        
        List<IDisposable> handlers = new List<IDisposable>();

        public Mapper(HubConnection hub, IClient client)
        {
            this.client = client;
            this.hub = hub;

            // register On handlers for client methods
            // use the non extension base impl
            // public IDisposable On(string methodName, Type[] parameterTypes, Func<object[], object, Task> handler, object state);

            handlers.Add(HubConnectionExtensions.On(
                hub, 
                "Foo2", 
                new Type[] {
                }, 
                async (x) => await Foo2Handler(x)
                ));

            handlers.Add(HubConnectionExtensions.On(hub, "Foo3", new Type[] { }, async (x) => await Foo3Handler(x)));
            handlers.Add(HubConnectionExtensions.On(hub, "Foo5", new Type[] { typeof(int) }, async (x) => await Foo5Handler(x)));
            handlers.Add(HubConnectionExtensions.On(hub, "Foo6", new Type[] { typeof(int) }, async (x) => await Foo6Handler(x)));
        }

        async Task Foo2Handler(object[] args)
        {
            await client.Foo2();
        }

        async Task Foo3Handler(object[] args)
        {
            await client.Foo3();
        }

        async Task Foo5Handler(object[] args)
        {
            await client.Foo5((int)args[0]);
        }

        async Task Foo6Handler(object[] args)
        {
            await client.Foo6((int)args[0]); 
        }

        bool disposedValue = false;

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}