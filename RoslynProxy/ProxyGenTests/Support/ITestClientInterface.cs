using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGenTests.Support
{
    public interface ITestClientInterface
    {
        Task Foo1();        
        Task Foo2(int arg1);
        Task Foo3(int arg1, int arg2, Tuple<int,double> arg3);
    }
}
