# Stongly-Typed Client-side Proxies for SignalR Core
## Overview

[SignalR Core](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR) is awesome! A current limitation, however, is that .NET clients are not strongly-typed.

This project adds the following strongly-typed client capabilities:
- Automatic, runtime generation of:
    - strongly-typed proxies for Client -> Server calls
    - strongly-typed mapper proxies for Server -> Client callback mapping
- Runtime proxy factory generation based on Roslyn (Microsoft.CodeAnalysis.*)

## Basic Usage

### Generating a proxy factory given server and client contracts
```csharp
var proxyFactory = ClientSideProxies.GenerateFor<IServerContract, IClientContract>();
```
### Creating and starting a hub connection
```csharp
var hubConnection = new HubConnectionBuilder()
                            .WithUrl(...)
                            .Build();

await hubConnection.StartAsync();
```
### Creating a strongly-typed client -> server helper
```csharp
// use proxyFactory and hubConnection to create a server proxy
var clientToServerProxy = proxyFactory.CreateServerProxy(hubConnection);

// invoke server-side hub methods via strongly-typed client proxy
var rval = await clientToServerProxy.SampleMethod("A String",DateTime.Now);
```

### Creating strongly-typed server -> client callback helper
```csharp
// create your client callback implementation
IClientContract clientImpl = new MyClientCallbacks();

// use proxyFactory to create a client mapper proxy
var callbackProxy = proxyFactory.CreateClientMapperProxy(hubConnection, client);

// at this point server-side callbacks via HubContext.Clients.* will route 
// automatically to clientImpl

// to "unsubscribe", dispose of callbackProxy
callbackProxy.Dispose();
```

## Info, Caveats & Limitations
- Based on .NET Core 3.0 preview 6
- Developed with Visual Studio 2019 Community Preview (Version 16.2.0 Preview 3.0)
- Only async/await enabled contracts are supported

    ```csharp
    public interface IServer
    {
        Task ImOk();
        Task<int> ImOkToo();
        // int ImNotSupported();
        // void MeEither();
    }

    public interface IClient
    {
        Task ImOk();
        // Task<int> ImOkToo();  // client callback return values not supported by SignalR
        // int ImNotSupported();
        // void MeEither();
    }
    ```
