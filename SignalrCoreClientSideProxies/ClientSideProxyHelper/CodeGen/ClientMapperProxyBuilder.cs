using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSideProxyHelper.CodeGen
{
    internal class ClientMapperProxyBuilder<TClient> where TClient : class
    {
        internal const string ClientMapperFactoryClassName = "ClientMapperFactory";
        internal const string ClientMapperClassName = "ClientMapper";

        ProxyCodeGenScope sharedScope;
        internal string Namespace { get => sharedScope.Namespace; }
        internal Type ClientInterfaceType { get; }

        internal ClientMapperProxyBuilder(ProxyCodeGenScope sharedScope)
        {
            // generate a random namespace
            this.sharedScope = sharedScope;            
            ClientInterfaceType = typeof(TClient);
        }

        internal string GenerateFactoryCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");

            var interfaceType = TypeUtils.TypeToFullyQualifiedString(typeof(IClientMapperProxyFactory<TClient>));
            sb.AppendLine($"public class {ClientMapperFactoryClassName} : {interfaceType}");
            sb.AppendLine("{");// start of class body
            sb.Append($"public {TypeUtils.TypeToFullyQualifiedString(typeof(IDisposable))} Create({TypeUtils.TypeToFullyQualifiedString(typeof(IHubConnectionBridge))} hub, ");
            sb.Append(TypeUtils.TypeToFullyQualifiedString(ClientInterfaceType));
            sb.Append(" client)");
            // emit the body to return instance of ProxyImpl
            sb.AppendLine("{");
            sb.AppendLine($"return new {ClientMapperClassName}(hub, client);");
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
            sb.AppendLine($"public class {ClientMapperClassName} : System.IDisposable");
            sb.AppendLine("{");
            // emit ctor that corresponds to Create method on factory
            EmitFields(sb);
            EmitMethods(sb);
            EmitCtor(sb);            
            sb.AppendLine("}");// end of class
            sb.AppendLine("}");// end of namespace
            return sb.ToString();
        }

        void EmitFields(StringBuilder sb)
        {
            sb.AppendLine("bool disposedValue = false;");
            sb.AppendLine($"{TypeUtils.TypeToFullyQualifiedString(typeof(List<System.IDisposable>))} Mappings {{ get; }}");
            sb.AppendLine($"{TypeUtils.TypeToFullyQualifiedString(typeof(IHubConnectionBridge))} Hub {{ get; }}");
            sb.AppendLine($"{TypeUtils.TypeToFullyQualifiedString(ClientInterfaceType)} Client {{ get; }}");            
        }

        void EmitMethods(StringBuilder sb)
        {
            // emit one method per client interface method
            var methods = ClientInterfaceType.GetMethods();
            foreach (var m in methods)
            {
                sb.AppendLine($"async System.Threading.Tasks.Task Map{m.Name}(object[] args)");
                sb.AppendLine("{");
                sb.AppendLine($"await Client.{m.Name}(");// start of client method invoke
                // loop over parameters in current method
                var parameters = m.GetParameters();
                var paramIndex = 0;
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first) sb.AppendLine(",");
                    first = false;
                    // emit cast                    
                        sb.Append($"({TypeUtils.TypeToFullyQualifiedString(p.ParameterType)})");                    
                    // emit object[]
                    sb.Append($"args[{paramIndex}]");
                    ++paramIndex;
                }
                sb.AppendLine(");");// end of client method invoke
                sb.AppendLine("}");
            }

            sb.AppendLine("void Dispose(bool disposing)");
            sb.AppendLine("{");
            sb.AppendLine("if (!disposedValue)");
            sb.AppendLine("{");
            sb.AppendLine("if (disposing) foreach (var item in Mappings) item.Dispose();");
            sb.AppendLine("disposedValue = true;");
            sb.AppendLine("}");
            sb.AppendLine("}");

            sb.AppendLine("public void Dispose()");
            sb.AppendLine("{");
            sb.AppendLine("Dispose(true);");
            sb.AppendLine("}");
        }

        void EmitCtor(StringBuilder sb)
        {
            sb.Append($"public {ClientMapperClassName}({TypeUtils.TypeToFullyQualifiedString(typeof(IHubConnectionBridge))} hub, ");            
            sb.AppendLine($"{TypeUtils.TypeToFullyQualifiedString(ClientInterfaceType)} client)");            
            sb.AppendLine("{");
            sb.AppendLine($"Mappings = new {TypeUtils.TypeToFullyQualifiedString(typeof(List<IDisposable>))}();");
            sb.AppendLine("Hub = hub;");
            sb.AppendLine("Client = client;");

            // use HubConnectionExtentions to register a mapping for each client interface method
            var methods = ClientInterfaceType.GetMethods();
            foreach (var m in methods)
            {
                sb.AppendLine("Mappings.Add(Hub.On(");
                sb.AppendLine($"\"{m.Name}\",");
                sb.Append("new System.Type[] {");
                // emit type of each param
                var parameters = m.GetParameters();
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first) sb.Append(",");
                    first = false;
                    sb.Append($"typeof({TypeUtils.TypeToFullyQualifiedString(p.ParameterType)})");
                }
                sb.AppendLine("},"); // end of type array
                sb.AppendLine($"async (x) => await Map{m.Name}(x)));");
            }

            sb.AppendLine("}");           
        }        
    }
}