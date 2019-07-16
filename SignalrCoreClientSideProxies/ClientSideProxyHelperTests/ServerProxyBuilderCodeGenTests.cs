using ClientSideProxyHelper.CodeGen;
using ClientSideProxyHelperTests.Fixtures;
using ClientSideProxyHelperTests.Util;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ClientSideProxyHelperTests
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
            var scope = new ProxyCodeGenScope();
            var b = new ServerProxyBuilder<INoParamReturnTask>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(INoParamReturnTask)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(INoParamReturnTask)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void NoParamsReturnTaskT()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ServerProxyBuilder<INoParamReturnTaskT>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(INoParamReturnTaskT)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(INoParamReturnTaskT)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void OneParamReturnTask()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ServerProxyBuilder<IOneParamReturnTask>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(IOneParamReturnTask)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(IOneParamReturnTask)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void OneParamReturnTaskT()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ServerProxyBuilder<IOneParamReturnTaskT>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(IOneParamReturnTaskT)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(IOneParamReturnTaskT)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void GenericNoParamsReturnTaskT()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ServerProxyBuilder<IGenericNoParamReturnTask<int>>(scope);
            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(IGenericNoParamReturnTask<int>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(IGenericNoParamReturnTask<int>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }

        [Fact]
        public void GenericOneParamReturnTaskT()
        {
            var scope = new ProxyCodeGenScope();
            var b = new ServerProxyBuilder<IGenericOneParamReturnTask<Tuple<int>>>(scope);

            var s = b.GenerateProxyCode();
            if (!SyntaxCheck.Check(s, typeof(IGenericOneParamReturnTask<Tuple<int>>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }

            s = b.GenerateFactoryCode();
            if (!SyntaxCheck.Check(s, typeof(IGenericOneParamReturnTask<Tuple<int>>)))
            {
                output.WriteLine(s);
                Assert.True(false);
            }
        }
    }
}
