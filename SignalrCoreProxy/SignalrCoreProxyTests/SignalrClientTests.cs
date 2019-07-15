using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalrCoreClientHelper;
using SignalrCoreProxyTests.SignalRIntegration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SignalrCoreProxyTests
{
    public class SignalrClientTests
    {
        private readonly ITestOutputHelper output;

        public SignalrClientTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        // what does client test entail?
        // - host running server api
        // - server mechanism to poke client
        // - server mechanism to ack client initiated callbacks

        [Fact]
        public async Task Foo()
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
                            endpoints.MapHub<TestHub>("TestHub");
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

            var proxyFactory = ClientSideProxies.Generate<ITestServerMethods, ITestClientMethods>();

            var serverProxy = proxyFactory.CreateServerProxy(hub);

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar1 = false;
            await serverProxy.Bar1();

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar2 = false;
            await serverProxy.Bar2(1);

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar3 = false;
            await serverProxy.Bar3(Tuple.Create(2));

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar4 = false;
            await serverProxy.Bar4();

            serverCheck.ResetToFailAll();
            serverCheck.FailOnBar5 = false;
            await serverProxy.Bar5(3);
        }
    }
}
