﻿using ProxyGen.CodeGen;
using ProxyGenTests.Fixtures;
using ProxyGenTests.Util;
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
        public void TestClientInterface()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ClientMapperProxyBuilder<ITestClientInterface>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(ITestClientInterface)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(ITestClientInterface)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void GenericTestClientInterface()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ClientMapperProxyBuilder<ITestGenericClientInterface<int>>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(ITestGenericClientInterface<int>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(ITestGenericClientInterface<int>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void GenericTestClientInterface2()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ClientMapperProxyBuilder<ITestGenericClientInterface<Tuple<int>>>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(ITestGenericClientInterface<Tuple<int>>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(ITestGenericClientInterface<Tuple<int>>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }
    }
}
