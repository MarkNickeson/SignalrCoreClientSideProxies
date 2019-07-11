using CodeGenHelper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGen
{
    internal class ClientMapperProxyBuilder
    {
        internal const string ClientMapperFactoryClassName = "ClientMapperFactory";
        internal const string ClientMapperClassName = "ClientMapper";
        internal string Namespace { get; }
        internal Type ClientInterfaceType { get; }

        internal ClientMapperProxyBuilder(Type clientInterfaceType)
        {
            // generate a random namespace
            Namespace = $"_{Guid.NewGuid().ToString("N")}";
            ClientInterfaceType = clientInterfaceType;
        }

        internal string GenerateFactoryCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");

            sb.AppendLine($"public class {ClientMapperFactoryClassName}");
            sb.AppendLine("{");

            sb.Append("public ClientMapper Create(Microsoft.AspNetCore.SignalR.Client.HubConnection hub, ");

            if (ClientInterfaceType.IsGenericType)
            {
                sb.Append(GenericTypeUtils.UnrollGenericTypeToString(ClientInterfaceType));
            }
            else
            {
                sb.Append(ClientInterfaceType.FullName);
            }

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
            sb.AppendLine("System.Collections.Generic.List<System.IDisposable> Mappings { get; }");
            sb.AppendLine("Microsoft.AspNetCore.SignalR.Client.HubConnection Hub { get; }");
            if (ClientInterfaceType.IsGenericType)
            {
                sb.AppendLine($"{GenericTypeUtils.UnrollGenericTypeToString(ClientInterfaceType)} Client {{ get; }}");
            }
            else
            {
                sb.AppendLine($"{ClientInterfaceType.FullName} Client {{ get; }}");
            }            
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
                    if (p.ParameterType.IsGenericType)
                    {
                        sb.Append($"({GenericTypeUtils.UnrollGenericTypeToString(p.ParameterType)})");
                    }
                    else
                    {
                        sb.Append($"({p.ParameterType.FullName})");
                    }
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
            sb.Append($"public {ClientMapperClassName}(Microsoft.AspNetCore.SignalR.Client.HubConnection hub, ");
            if (ClientInterfaceType.IsGenericType)
            {
                sb.AppendLine($"{GenericTypeUtils.UnrollGenericTypeToString(ClientInterfaceType)} client)");
            }
            else
            {
                sb.AppendLine($"{ClientInterfaceType.FullName} client)");
            }

            sb.AppendLine("{");
            sb.AppendLine("Mappings = new System.Collections.Generic.List<System.IDisposable>();");
            sb.AppendLine("Hub = hub;");
            sb.AppendLine("Client = client;");

            // use HubConnectionExtentions to register a mapping for each client interface method
            var methods = ClientInterfaceType.GetMethods();
            foreach (var m in methods)
            {
                sb.AppendLine("Mappings.Add(Microsoft.AspNetCore.SignalR.Client.HubConnectionExtensions.On(");
                sb.AppendLine("Hub,");
                sb.AppendLine($"\"{m.Name}\",");
                sb.Append("new System.Type[] {");
                // emit type of each param
                var parameters = m.GetParameters();
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first) sb.Append(",");
                    first = false;
                    if (p.ParameterType.IsGenericType)
                    {
                        sb.Append($"typeof({GenericTypeUtils.UnrollGenericTypeToString(p.ParameterType)})");
                    }
                    else
                    {
                        sb.Append($"typeof({p.ParameterType.FullName})");
                    }
                }
                sb.AppendLine("},"); // end of type array
                sb.AppendLine($"async (x) => await Map{m.Name}(x)));");
            }

            sb.AppendLine("}");           
        }        
    }
}