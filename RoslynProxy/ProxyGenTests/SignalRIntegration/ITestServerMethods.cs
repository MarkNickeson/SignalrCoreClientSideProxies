﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGenTests.SignalRIntegration
{
    // implement with signalr hub
    // - given a HubConnection, signalr client creates a proxy to the server
    public interface ITestServerMethods
    {
        Task Bar1();
        Task Bar2(int arg1);
        Task Bar3(Tuple<int> arg1);
        Task<int> Bar4();
        Task<int> Bar5(int arg1);
        Task<Tuple<int>> Bar6(Tuple<int> arg1);
    }
}