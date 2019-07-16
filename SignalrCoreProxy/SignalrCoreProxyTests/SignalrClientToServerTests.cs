using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ClientSideProxyHelper;
using ClientSideProxyHelperTests.SignalRIntegrationFixtures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ClientSideProxyHelperTests
{
    public class SignalrClientToServerTests
    {
        private readonly ITestOutputHelper output;

        public SignalrClientToServerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task GoodTest()
        {
            var serverCheck = new TestServerMethodsFailOnCall();

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

            var hub = new HubConnectionBuilder()
                .WithUrl(
                    "http://localhost/TestHub",
                    x => x.HttpMessageHandlerFactory = _ => server.CreateHandler())
                .Build();

            await hub.StartAsync();

            var proxyFactory = ClientSideProxies.GenerateFor<ITestServerMethods, ITestClientMethods>();

            var serverProxy = proxyFactory.CreateServerProxy(hub);

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar1 = false;
            await serverProxy.Bar1();

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar2 = false;
            await serverProxy.Bar2(1);

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar3 = false;
            await serverProxy.Bar3(new Custom<int>() { Value = 2 });

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar4 = false;
            await serverProxy.Bar4();

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar5 = false;
            await serverProxy.Bar5(3);

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar6 = false;
            var rval = await serverProxy.Bar6(1234);
            Assert.Equal(1234, rval.Value);
        }
    }
}
