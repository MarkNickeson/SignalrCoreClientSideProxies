using SignalrCoreProxy.CodeGen;
using SignalrCoreProxy.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalrCoreProxy
{
    public class SignalrCoreProxyGen
    {
        public static SignalrProxyFactory<TServer, TClient> Generate<TServer, TClient>() 
            where TServer : class
            where TClient : class
        {
            // obtain types
            var serverInterfaceType = typeof(TServer);
            var clientInterfaceType = typeof(TClient);

            // validate server and client interfaces
            ValidateTServer(serverInterfaceType);
            ValidateTClient(clientInterfaceType);

            // create code generators
            var scope = new ProxyCodeGenScope();
            var cg1 = new ClientMapperProxyBuilder<TClient>(scope);
            var cg2 = new ServerProxyBuilder<TServer>(scope);
            
            // gather the code
            var code = new string[] {
                cg1.GenerateFactoryCode(),
                cg1.GenerateProxyCode(),
                cg2.GenerateFactoryCode(),
                cg2.GenerateProxyCode(),
            };

            // compile code to assembly
            var asm = ProxyCompiler.Compile(code, clientInterfaceType, serverInterfaceType);

            // create client and server factory
            var ct = asm.GetType($"{scope.Namespace}.{ClientMapperProxyBuilder<TClient>.ClientMapperFactoryClassName}");
            var st = asm.GetType($"{scope.Namespace}.{ServerProxyBuilder<TServer>.FactoryClassName}");

            var clientFactory = Activator.CreateInstance(ct) as IClientMapperProxyFactory<TClient>;
            var serverFactory = Activator.CreateInstance(st) as IServerProxyFactory<TServer>;

            return new SignalrProxyFactory<TServer, TClient>(serverFactory, clientFactory);
        }

        static void ValidateTServer(Type serverType)
        {
            // ensure serverType is interface
            if (!serverType.IsInterface)
            {
                throw new ArgumentException($"TServer is not an interface");
            }

            // ensure serverType contains methods only
            var members = serverType.GetMembers();
            foreach (var m in members)
            {
                if (m.MemberType != System.Reflection.MemberTypes.Method)
                {
                    throw new ApplicationException("TServer contains a non-method member");
                }
            }

            // ensure serverType methods all return Task or Task<T>
            var methods = serverType.GetMethods();
            foreach (var m in methods)
            {
                if (!typeof(Task).IsAssignableFrom(m.ReturnType))
                { 
                    throw new ApplicationException("TServer method does not return Task or Task<T>");
                }
            }
        }

        static void ValidateTClient(Type clientType)
        {
            // ensure clientType is interface
            if (!clientType.IsInterface)
            {
                throw new ArgumentException($"TClient is not an interface");
            }

            // ensure clientType contains methods only
            var members = clientType.GetMembers();
            foreach (var m in members)
            {
                if (m.MemberType != System.Reflection.MemberTypes.Method)
                {
                    throw new ApplicationException("TClient contains a non-method member");
                }
            }

            // ensure clientType methods all return Task (NOT Task<T> because signalr client are one way)
            var methods = clientType.GetMethods();
            foreach (var m in methods)
            {
                if (!typeof(Task).IsAssignableFrom(m.ReturnType) || m.ReturnType.IsGenericType)
                {
                    throw new ApplicationException("TClient method does not return Task");
                }
            }
        }
    }
}
