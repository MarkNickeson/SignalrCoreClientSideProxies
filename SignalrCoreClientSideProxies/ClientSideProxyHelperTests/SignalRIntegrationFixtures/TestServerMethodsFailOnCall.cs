using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideProxyHelperTests.SignalRIntegrationFixtures
{
    public class TestServerMethodsFailOnCall : ITestServerMethods
    {
        public bool FailOnBar1 { get; set; }
        public bool FailOnBar2 { get; set; }
        public bool FailOnBar3 { get; set; }
        public bool FailOnBar4 { get; set; }
        public bool FailOnBar5 { get; set; }
        public bool FailOnBar6 { get; set; }

        public void ResetToFailAll()
        {
            FailOnBar1 = true;
            FailOnBar2 = true;
            FailOnBar3 = true;
            FailOnBar4 = true;
            FailOnBar5 = true;
            FailOnBar6 = true;
        }

        public Task Bar1()
        {
            if (FailOnBar1) throw new ApplicationException("Bar1");
            return Task.CompletedTask;
        }

        public Task Bar2(int arg1)
        {
            if (FailOnBar2) throw new ApplicationException("Bar2");
            return Task.CompletedTask;
        }

        public Task Bar3(Custom<int> arg1)
        {
            if (FailOnBar3) throw new ApplicationException("Bar3");
            return Task.CompletedTask;
        }

        public Task<int> Bar4()
        {
            if (FailOnBar4) throw new ApplicationException("Bar4");
            return Task.FromResult<int>(999);
        }

        public Task<int> Bar5(int arg1)
        {
            if (FailOnBar5) throw new ApplicationException("Bar5");
            return Task.FromResult<int>(998);
        }

        public Task<Custom<int>> Bar6(int arg1)
        {
            if (FailOnBar6) throw new ApplicationException("Bar6");
            return Task.FromResult<Custom<int>>(new Custom<int>() { Value = arg1});
        }
    }
}
