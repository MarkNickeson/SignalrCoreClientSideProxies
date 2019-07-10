using ProxyGen.SignalR;
using System;
using Xunit;

namespace ProxyGenTests
{
    public class SignalrCoreProxyValidationTests
    {
        [Fact]
        public void ValidateGoodAsyncInterface()
        {
            SignalRCoreProxy.Validate(typeof(IGoodAsync));
        }

        [Fact]
        public void ValidateBadAsyncInterface()
        {
            Assert.ThrowsAny<Exception>(() => SignalRCoreProxy.Validate(typeof(IBadAsync1)));
            Assert.ThrowsAny<Exception>(() => SignalRCoreProxy.Validate(typeof(IBadAsync2)));
        }

        [Fact]
        public void ValidateNotInterface()
        {
            Assert.ThrowsAny<Exception>(() => SignalRCoreProxy.Validate(typeof(string)));
        }

        [Fact]
        public void ValidateBadInterfaceNonMethods()
        {
            Assert.ThrowsAny<Exception>(() => SignalRCoreProxy.Validate(typeof(string)));
        }
    }
}
