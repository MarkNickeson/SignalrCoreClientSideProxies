using ProxyGen;
using ProxyGen.SignalR;
using ProxyGenTests.Support;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ProxyGenTests
{
    public class ClientMapperProxyBuilderCodeGenTests
    {
        private readonly ITestOutputHelper output;

        public ClientMapperProxyBuilderCodeGenTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void NoParamsReturnTask()
        {
            var b = new ClientMapperProxyBuilder(typeof(ITestClientInterface));

            output.WriteLine(b.GenerateFactoryCode());

            output.WriteLine(b.GenerateProxyCode());
        }       
    }
}
