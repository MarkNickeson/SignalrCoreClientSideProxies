using ClientSideProxyHelper.CodeGen;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace SignalrCoreProxyTests
{
    public class UnrollGenericTests
    {
        private readonly ITestOutputHelper output;

        public UnrollGenericTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GenericTypeToString()
        {
            var rval = TypeUtils.UnrollGenericTypeToString(typeof(IList<int>));
            Assert.Equal("System.Collections.Generic.IList<System.Int32>", rval);
        }

        [Fact]
        public void NestedGenericTypeToString()
        {
            var rval = TypeUtils.UnrollGenericTypeToString(typeof(IList<IList<int>>));
            Assert.Equal("System.Collections.Generic.IList<System.Collections.Generic.IList<System.Int32>>", rval);
        }

        [Fact]
        public void GenericTwoTypesToString()
        {
            var rval = TypeUtils.UnrollGenericTypeToString(typeof(Tuple<int, double>));
            Assert.Equal("System.Tuple<System.Int32,System.Double>", rval);
        }

        [Fact]
        public void GenericThreeTypesToString()
        {
            var rval = TypeUtils.UnrollGenericTypeToString(typeof(Tuple<int, double, string>));
            Assert.Equal("System.Tuple<System.Int32,System.Double,System.String>", rval);
        }

        [Fact]
        public void GenericNestedTwoTypesToString()
        {
            var rval = TypeUtils.UnrollGenericTypeToString(typeof(Tuple<Tuple<int, double>, Tuple<string, short>>));
            Assert.Equal("System.Tuple<System.Tuple<System.Int32,System.Double>,System.Tuple<System.String,System.Int16>>", rval);
        }        
    }
}
