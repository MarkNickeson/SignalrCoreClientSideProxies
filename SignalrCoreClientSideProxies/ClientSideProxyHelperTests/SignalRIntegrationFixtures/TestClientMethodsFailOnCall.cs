using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientSideProxyHelperTests.SignalRIntegrationFixtures
{
    public class TestClientMethodsFailOnCall : ITestClientMethods
    {


        public ManualResetEvent Signal { get; }
        public bool FailOnFoo1 { get; set; }
        public bool FailOnFoo2 { get; set; }
        public bool FailOnFoo3 { get; set; }

        public TestClientMethodsFailOnCall()
        {
            Signal = new ManualResetEvent(false);
        }

        public void ResetToFailAll()
        {
            FailOnFoo1 = true;
            FailOnFoo2 = true;
            FailOnFoo3 = true;
        }

        public Task Foo1()
        {
            try
            {
                Trace.WriteLine("TestClientMethodsFailOnCall.Foo1");
                if (FailOnFoo1) throw new ApplicationException("Foo1");
                return Task.CompletedTask;
            }
            finally
            {
                Trace.WriteLine("Signalling TestClientMethodsFailOnCall.Foo1");
                Signal.Set();
            }
        }

        public Task Foo2(int arg1)
        {
            try
            {
                Trace.WriteLine("Enter TestClientMethodsFailOnCall.Foo2");
                if (FailOnFoo2) throw new ApplicationException("Foo2");
                return Task.CompletedTask;
            }
            finally
            {
                Trace.WriteLine("Signalling TestClientMethodsFailOnCall.Foo2");
                Signal.Set();
            }
        }

        public Task Foo3(Custom<int> arg1)
        {
            try
            {
                Trace.WriteLine("TestClientMethodsFailOnCall.Foo3");
                if (FailOnFoo3) throw new ApplicationException("Foo3");
                return Task.CompletedTask;
            }
            finally
            {
                Trace.WriteLine("Signalling TestClientMethodsFailOnCall.Foo3");
                Signal.Set();
            }
        }
    }
}
