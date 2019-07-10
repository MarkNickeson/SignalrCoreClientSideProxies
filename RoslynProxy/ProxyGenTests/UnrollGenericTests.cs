﻿using ProxyGen;
using ProxyGen.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ProxyGenTests
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
            var rval = GenericTypeUtils.UnrollGenericType(typeof(IList<int>));
            Assert.Equal("System.Collections.Generic.IList<System.Int32>", rval);
        }

        [Fact]
        public void NestedGenericTypeToString()
        {
            var rval = GenericTypeUtils.UnrollGenericType(typeof(IList<IList<int>>));
            Assert.Equal("System.Collections.Generic.IList<System.Collections.Generic.IList<System.Int32>>", rval);
        }

        [Fact]
        public void GenericTwoTypesToString()
        {
            var rval = GenericTypeUtils.UnrollGenericType(typeof(Tuple<int, double>));
            Assert.Equal("System.Tuple<System.Int32,System.Double>", rval);
        }

        [Fact]
        public void GenericThreeTypesToString()
        {
            var rval = GenericTypeUtils.UnrollGenericType(typeof(Tuple<int, double, string>));
            Assert.Equal("System.Tuple<System.Int32,System.Double,System.String>", rval);
        }

        [Fact]
        public void GenericNestedTwoTypesToString()
        {
            var rval = GenericTypeUtils.UnrollGenericType(typeof(Tuple<Tuple<int, double>, Tuple<string, short>>));
            Assert.Equal("System.Tuple<System.Tuple<System.Int32,System.Double>,System.Tuple<System.String,System.Int16>>", rval);
        }        
    }
}