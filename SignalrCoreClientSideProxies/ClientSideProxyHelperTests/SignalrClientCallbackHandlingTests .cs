using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ClientSideProxyHelper;
using ClientSideProxyHelperTests.SignalRIntegrationFixtures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ClientSideProxyHelperTests
{
    public class SignalrClientCallbackHandlingTests
    {
        private readonly ITestOutputHelper output;

        public SignalrClientCallbackHandlingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task GoodTest()
        {
            var serverCheck = new TestServerMethodsFailOnCall();
            serverCheck.ResetToFailAll(); // for client test make sure all server methods fail

            var app = Host.CreateDefaultBuilder()
                .ConfigureServices((sc) =>
                {
                    sc.AddSingleton<ITestServerMethods, TestServerMethodsFailOnCall>((x) => serverCheck);
                    sc.AddSignalR();
                })
                .ConfigureWebHost((wh) =>
                {
                    wh.Configure((app) =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<TestHubWithServerDelegate>("TestHub");
                        });
                    });
                    wh.UseTestServer();
                })
                .Build();

            app.Start();

            var server = app.GetTestServer();

            // resolve server-side hub context
            var serverHubContext = app.Services.GetRequiredService<IHubContext<TestHubWithServerDelegate, ITestClientMethods>>();

            var hub = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/TestHub",
                    x => x.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            await hub.StartAsync();

            var proxyFactory = ClientSideProxies.GenerateFor<ITestServerMethods, ITestClientMethods>();

            // create client implementation
            var clientImpl = new TestClientMethodsFailOnCall();

            // create client mapper proxy
            proxyFactory.CreateClientMapperProxy(hub, clientImpl);

            int successCount = 0;

            clientImpl.ResetToFailAll();
            clientImpl.Signal.Reset();
            clientImpl.FailOnFoo1 = false;
            await serverHubContext.Clients.All.Foo1();
            clientImpl.Signal.WaitOne();
            successCount++;
           
            clientImpl.ResetToFailAll();
            clientImpl.Signal.Reset();
            clientImpl.FailOnFoo2 = false;   
            await serverHubContext.Clients.All.Foo2(97);
            clientImpl.Signal.WaitOne();
            successCount++;   

            clientImpl.ResetToFailAll();
            clientImpl.Signal.Reset();
            clientImpl.FailOnFoo3 = false;
            await serverHubContext.Clients.All.Foo3(new Custom<int>() { Value = 96 });
            clientImpl.Signal.WaitOne();
            successCount++;
            
            Assert.Equal(3, successCount);
        }
    }
}
