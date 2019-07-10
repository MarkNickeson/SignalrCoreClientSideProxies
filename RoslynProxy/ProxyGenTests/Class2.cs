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

    public class Unwrapper : IDisposable
    {
        IClient client;
        HubConnection hub;
        List<IDisposable> handlers = new List<IDisposable>();

        public Unwrapper(IClient client, HubConnection hub)
        {
            this.client = client;
            this.hub = hub;

            // register On handlers for client methods
            // use the non extension base impl
            // public IDisposable On(string methodName, Type[] parameterTypes, Func<object[], object, Task> handler, object state);
            object state = null;
            handlers.Add(hub.On("Foo2", new Type[] { }, Foo2Handler, state));
            handlers.Add(hub.On("Foo3", new Type[] { }, Foo3Handler, state));
            handlers.Add(hub.On("Foo5", new Type[] { typeof(int) }, Foo5Handler, state));
            handlers.Add(hub.On("Foo6", new Type[] { typeof(int) }, Foo6Handler, state));
        }

        async Task Foo2Handler(object[] args, object state)
        {
            await client.Foo2();
            return;
        }

        async Task Foo3Handler(object[] args, object state)
        {
            var rval = await client.Foo3();
            // assume state has to be return value handler, maybe exception 
        }

        async Task Foo5Handler(object[] args, object state)
        {
            await client.Foo5((int)args[0]);
            return;
        }

        async Task Foo6Handler(object[] args, object state)
        {
            var rval = await client.Foo6((int)args[0]);
            // state must be return value handler, maybe exception 
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