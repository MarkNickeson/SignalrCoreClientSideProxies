using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public interface IFooFactory
    {
        IFoo Create(string arg);
    }

    public interface IFoo
    {
        string Foo();
        void Bar(string arg1);
    }
}
