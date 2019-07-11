using ProxyGen;
using ProxyGen.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ProxyGenTests
{
    public class ServerProxyBuilderCodeGenTests
    {
        private readonly ITestOutputHelper output;

        public ServerProxyBuilderCodeGenTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void NoParamsReturnTask()
        {
            var b = new ServerProxyBuilder(typeof(IFactory<INoParamReturnTask>), typeof(INoParamReturnTask),
                SignalRCoreProxy.ServerProxyCodeGen);

            output.WriteLine(b.GenerateProxyCode(true));

            output.WriteLine(b.GenerateFactoryCode());
        }

        [Fact]
        public void NoParamsReturnTaskT()
        {
            var b = new ServerProxyBuilder(typeof(IFactory<INoParamReturnTaskT>), typeof(INoParamReturnTaskT),
                SignalRCoreProxy.ServerProxyCodeGen);

            output.WriteLine(b.GenerateProxyCode(true));

            output.WriteLine(b.GenerateFactoryCode());
        }

        [Fact]
        public void OneParamReturnTask()
        {
            var b = new ServerProxyBuilder(typeof(IFactory<IOneParamReturnTask>), typeof(IOneParamReturnTask),
                SignalRCoreProxy.ServerProxyCodeGen);

            output.WriteLine(b.GenerateProxyCode(true));

            output.WriteLine(b.GenerateFactoryCode());
        }

        [Fact]
        public void OneParamReturnTaskT()
        {
            var b = new ServerProxyBuilder(typeof(IFactory<IOneParamReturnTaskT>), typeof(IOneParamReturnTaskT),
                SignalRCoreProxy.ServerProxyCodeGen);

            output.WriteLine(b.GenerateProxyCode(true));

            output.WriteLine(b.GenerateFactoryCode());
        }
    }
}
