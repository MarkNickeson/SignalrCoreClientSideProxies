using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideProxyHelperTests.Fixtures
{
    public interface IOneParamReturnTaskT
    {
        Task<int> Foo(int test);
    }
}
