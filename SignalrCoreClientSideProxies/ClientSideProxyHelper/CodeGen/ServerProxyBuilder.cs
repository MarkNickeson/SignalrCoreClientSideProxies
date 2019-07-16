using System;
using System.Reflection;
using System.Text;

namespace ClientSideProxyHelper.CodeGen
{
    internal class ServerProxyBuilder<TServer> where TServer : class
    {
        internal const string FactoryClassName = "ServerProxyFactoryImpl";
        internal const string ProxyClassName = "ProxyImpl";
        internal string Namespace { get => sharedScope.Namespace; }
        internal Type ServerInterfaceType { get; }
        ProxyCodeGenScope sharedScope;

        internal ServerProxyBuilder(
            ProxyCodeGenScope sharedScope)
        {
            this.sharedScope = sharedScope;
            ServerInterfaceType = typeof(TServer);
        }

        internal string GenerateFactoryCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"public class {FactoryClassName} : {TypeUtils.TypeToFullyQualifiedString(typeof(IServerProxyFactory<TServer>))}");
            sb.AppendLine("{");
            sb.Append($"public {TypeUtils.TypeToFullyQualifiedString(ServerInterfaceType)} Create({TypeUtils.TypeToFullyQualifiedString(typeof(IHubConnectionBridge))} hub)");
                        
            // emit the body to return instance of ProxyImpl
            sb.AppendLine("{");
            sb.Append($"return new {ProxyClassName}(hub);");           
            sb.AppendLine("}");// end of create method
            sb.AppendLine("}");// end of impl
            sb.AppendLine("}");// end of namespace

            return sb.ToString();
        }

        internal string GenerateProxyCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"public class {ProxyClassName} : {TypeUtils.TypeToFullyQualifiedString(ServerInterfaceType)}");            
            sb.AppendLine("{");
            // emit ctor that corresponds to Create method on factory
            EmitFields(sb);
            EmitCtor(sb);
            EmitMethods(sb);
            sb.AppendLine("}");// end of proxy impl class
            sb.AppendLine("}");// end of namespace
            return sb.ToString();
        }

        void EmitFields(StringBuilder sb)
        {
            sb.AppendLine($"{TypeUtils.TypeToFullyQualifiedString(typeof(IHubConnectionBridge))} Hub {{ get; }}");
        }

        void EmitCtor(StringBuilder sb)
        {
            sb.Append($"public {ProxyClassName}({TypeUtils.TypeToFullyQualifiedString(typeof(IHubConnectionBridge))} hub)");            
            // emit the ctor body to assign paremters to fields
            sb.AppendLine("{");            
            sb.AppendLine($"Hub = hub;");  // fully qualified parameter type            
            sb.AppendLine("}");
        }



        void EmitMethods(StringBuilder sb)
        {
            var methods = ServerInterfaceType.GetMethods();
            foreach (var m in methods)
            {
                // all methods are public because interface impls
                sb.Append($"public async {TypeUtils.TypeToFullyQualifiedString(m.ReturnType)} {m.Name}(");
                var parameters = m.GetParameters();
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first) sb.Append(",");
                    first = false;
                    sb.Append($"{TypeUtils.TypeToFullyQualifiedString(p.ParameterType)} {p.Name}");
                }
                sb.AppendLine($")");

                // for a body, just emit default(x) unless void
                sb.AppendLine("{");
                MethodBodyCodeGen(sb, m);
                sb.AppendLine("}");
            }
        }

        void MethodBodyCodeGen(StringBuilder builder, MethodInfo methodInfo)
        {
            // for each method want to emit code as:
            // public async {Method.ReturnType} {Method.Name}({ params })
            // {
            //     return await hub.InvokeAsyncCore(methodName, returnType, args, ct);
            //     or
            //     await hubConnection.SendAsyncCore(methodName, args, ct);
            // }

            if (!methodInfo.ReturnType.IsGenericType) // Task
            {
                // emit SendAsyncCore
                builder.Append($"await Hub.SendCoreAsync(");
                builder.Append($"\"{methodInfo.Name}\"");
                builder.Append(",new object[]{");
                bool first = true;
                foreach (var p in methodInfo.GetParameters())
                {
                    if (!first) builder.Append(",");
                    first = false;
                    builder.Append(p.Name);
                }
                builder.Append("}");// end of object[]
                // ignoring cancellation token
                builder.Append(");"); // end of SendCoreAsync
            }
            else // Task<T>
            {
                // emit InvokeAsyncCore
                // difference here is that InvokeAsyncCore returns Task<object> so
                // must await to capture object then cast and return
                builder.Append($"var temp = await Hub.InvokeCoreAsync(\"");
                builder.Append(methodInfo.Name);
                // Return type is assumed to be Task<T>, but T can still be generic so unroll it
                var taskT = methodInfo.ReturnType.GetGenericArguments()[0];
                builder.Append($"\",typeof({TypeUtils.TypeToFullyQualifiedString(taskT)})");

                // box the method parameters
                builder.Append(",new object[]{");
                bool first = true;
                foreach (var p in methodInfo.GetParameters())
                {
                    if (!first) builder.Append(",");
                    first = false;
                    builder.Append(p.Name);
                }
                builder.Append("}");// end of object[]
                // ignore cancellation token
                builder.AppendLine(");"); // end of Invoke

                builder.Append($"return ({TypeUtils.TypeToFullyQualifiedString(methodInfo.ReturnType.GetGenericArguments()[0])})temp;");
            }
        }
    }
}