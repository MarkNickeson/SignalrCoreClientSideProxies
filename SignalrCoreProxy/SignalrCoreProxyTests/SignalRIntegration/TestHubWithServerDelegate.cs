using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalrCoreProxyTests.SignalRIntegration
{
    public class TestHubWithServerDelegate : Hub<ITestClientMethods>, ITestServerMethods
    {
        ITestServerMethods serverDelegate;

        public TestHubWithServerDelegate(ITestServerMethods serverDelegate)
        {
            if (serverDelegate == null) throw new ArgumentNullException("serverDelegate");
            this.serverDelegate = serverDelegate;
        }

        public async Task Bar1()
        {
            await serverDelegate.Bar1();
        }

        public async Task Bar2(int arg1)
        {
            await serverDelegate.Bar2(arg1);
        }

        public async Task Bar3(Custom<int> arg1)
        {
            await serverDelegate.Bar3(arg1);
        }

        public async Task<int> Bar4()
        {
            return await serverDelegate.Bar4();
        }

        public async Task<int> Bar5(int arg1)
        {
            return await serverDelegate.Bar5(arg1);
        }

        public async Task<Custom<int>> Bar6(int arg1)
        {
            return await serverDelegate.Bar6(arg1);
        }
    }
}
