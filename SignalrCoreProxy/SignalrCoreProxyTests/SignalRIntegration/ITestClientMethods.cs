using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalrCoreProxyTests.SignalRIntegration
{
    // invokable on client
    // - use for creating strongly typed hub on the server 
    // - implement on client and map with proxy
    public interface ITestClientMethods
    {
        Task Foo1();
        Task Foo2(int arg1);
        Task Foo3(Custom<int> arg1);
    }
}
