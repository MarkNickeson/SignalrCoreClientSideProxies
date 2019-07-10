using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly:InternalsVisibleTo("ProxyGenTests")]

namespace ProxyGen.SignalR
{
    internal interface IFactory<T> where T : class
    {
        T Create(IHubBridge hub);
    }
}
