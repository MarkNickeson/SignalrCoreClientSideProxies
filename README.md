# Stongly-Typed SignalR Core Client-side Proxies

[SignalR Core](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR) is awesome! But currently strongly-typed contracts are not supported for .NET clients. This project fills this gap by providing the following capabilities:
- Strongly-typed proxy for Client -> Server calls
- Strongly-typed proxy for Server -> Client callbacks
- Automatic runtime proxy generation based on Roslyn (Microsoft.CodeAnalysis.*)

## Basic Usage

### Generate proxy factory given server and client contracts
```csharp
var proxyFactory = ClientSideProxies.GenerateFor<IServerContract, IClientContract>();
```
### Create and start a hub connection
```csharp
var hubConnection = new HubConnectionBuilder()
                            .WithUrl(...)
                            .Build();

await hubConnection.StartAsync();
```
### Strongly-type client -> server
```csharp
// use proxyFactory and hubConnection to create a server proxy
var clientToServerProxy = proxyFactory.CreateServerProxy(hubConnection);

// invoke server-side hub methods via strongly-typed client proxy
var rval = await clientToServerProxy.SampleMethod("A String",DateTime.Now);
```

### Strongly-type server -> client callbacks
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

## Limitations

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
