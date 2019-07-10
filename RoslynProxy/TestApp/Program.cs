using ProxyGen;
using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyGenerator.Create<IFooFactory, IFoo>(DefaultCSharpBodyEmitters.Empty);

            ProxyGenerator.Create<IBarFactory, IBar>(DefaultCSharpBodyEmitters.PassThroughTrace);
        }
    }
}
