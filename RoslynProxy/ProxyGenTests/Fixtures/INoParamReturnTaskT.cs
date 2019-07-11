using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGenTests.Fixtures
{
    public interface INoParamReturnTaskT
    {
        Task<int> Foo();
    }
}
