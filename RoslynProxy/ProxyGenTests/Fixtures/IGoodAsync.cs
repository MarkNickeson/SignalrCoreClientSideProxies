﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGenTests.Fixtures
{
    public interface IGoodAsync
    {
        Task Foo1();
        Task<string> Foo2();
    }
}