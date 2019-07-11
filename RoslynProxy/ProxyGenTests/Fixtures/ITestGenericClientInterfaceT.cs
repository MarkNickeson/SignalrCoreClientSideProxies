using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGenTests.Fixtures
{
    public interface ITestGenericClientInterface<T>
    {
        Task Foo1();        
        Task Foo2(T arg1);
        Task Foo3(T arg1, int arg2, Tuple<int,double> arg3);
    }
}
