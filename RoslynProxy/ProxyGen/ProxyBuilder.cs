using CodeGenHelper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProxyGen
{
    internal class ProxyBuilder
    {
        internal string Namespace { get; }
        internal Type FactoryType { get; }
        internal Type ProxyType { get; }

        Action<StringBuilder, MethodInfo> emitCSharpBody;

        internal ProxyBuilder(Type factoryType, Type proxyType) 
            : this(factoryType, proxyType, DefaultCSharpBodyEmitters.Empty)
        {
        }

        internal ProxyBuilder(
            Type factoryType, 
            Type proxyType, 
            Action<StringBuilder, MethodInfo> emitCSharpBody)
        {
            // generate a random namespace
            Namespace = $"_{Guid.NewGuid().ToString("N")}";
            FactoryType = factoryType;
            ProxyType = proxyType;
            this.emitCSharpBody = emitCSharpBody;
        }

        internal string GenerateFactoryCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            // check if factory type is generic interface
            if (FactoryType.IsGenericType)
            {
                var genericType = GenericTypeUtils.UnrollGenericTypeToString(FactoryType);
                sb.AppendLine($"public class FactoryImpl : {genericType}");
            }
            else
            {
                sb.AppendLine($"public class FactoryImpl : {FactoryType.FullName}");
            }
            sb.AppendLine("{");

            // get the method
            var methodInfo = FactoryType.GetMethod("Create");

            sb.Append($"public {methodInfo.ReturnType.FullName} Create(");

            // get args
            var parameters = methodInfo.GetParameters();
            bool first = true;
            foreach (var p in parameters)
            {
                if (!first) sb.Append(",");
                first = false;
                sb.Append(p.ParameterType.FullName);  // fully qualified parameter type
                sb.Append(" ");
                sb.Append(p.Name); // parameter name
            }
            sb.AppendLine(")");

            // emit the body to return instance of ProxyImpl
            sb.AppendLine("{");
            sb.Append("return new ProxyImpl(");
            first = true;
            foreach (var p in parameters)
            {
                if (!first) sb.Append(",");
                sb.Append($"{p.Name}");
            }
            sb.AppendLine(");");
            sb.AppendLine("}");// end of create method
            sb.AppendLine("}");// end of impl
            sb.AppendLine("}");// end of namespace

            return sb.ToString();
        }

        internal string GenerateProxyCode(bool asyncWhenReturnsTask)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"public class ProxyImpl : {ProxyType.FullName}");
            sb.AppendLine("{");
            // emit ctor that corresponds to Create method on factory
            EmitFields(sb);
            EmitCtor(sb);
            EmitMethods(sb, asyncWhenReturnsTask);
            sb.AppendLine("}");// end of proxy impl class
            sb.AppendLine("}");// end of namespace
            return sb.ToString();
        }

        void EmitFields(StringBuilder sb)
        {
            var createMethod = FactoryType.GetMethod("Create");

            // define a field for each ctor parameter.  use _ prefix for easy disambiguate
            var parameters = createMethod.GetParameters();

            foreach (var p in parameters)
            {
                sb.AppendLine($"{p.ParameterType.FullName} _{p.Name};");
            }
        }

        void EmitCtor(StringBuilder sb)
        {
            var createMethod = FactoryType.GetMethod("Create");

            sb.Append("public ProxyImpl(");

            // get args
            var parameters = createMethod.GetParameters();
            bool first = true;
            foreach (var p in parameters)
            {
                if (!first) sb.Append(",");
                first = false;
                sb.Append(p.ParameterType.FullName);  // fully qualified parameter type
                sb.Append(" ");
                sb.Append(p.Name); // parameter name
            }
            sb.AppendLine(")");

            // emit the ctor body to assign paremters to fields
            sb.AppendLine("{");
            foreach (var p in parameters)
            {
                sb.AppendLine($"_{p.Name} = {p.Name};");  // fully qualified parameter type
            }
            sb.AppendLine("}");
        }

        static Type taskType = typeof(Task);

        void EmitMethods(StringBuilder sb, bool asyncWhenReturnsTask)
        {
            var methods = ProxyType.GetMethods();
            foreach (var m in methods)
            {
                // all methods are public because interface impls
                if (taskType.IsAssignableFrom(m.ReturnType))
                    sb.Append("public async ");
                else
                    sb.Append("public ");

                if (m.ReturnType == typeof(void))
                {
                    sb.Append($"void ");
                }
                else
                {
                    if (m.ReturnType.IsGenericType)
                    {
                        var genericType = GenericTypeUtils.UnrollGenericTypeToString(m.ReturnType);
                        sb.Append(genericType);
                    }
                    else
                    {
                        sb.Append(m.ReturnType.FullName);
                    }
                }

                sb.Append($" {m.Name}(");
                var parameters = m.GetParameters();
                bool first = true;
                foreach (var p in parameters)
                {
                    if (!first) sb.Append(",");
                    first = false;
                    sb.Append(p.ParameterType.FullName);  // fully qualified parameter type
                    sb.Append(" ");
                    sb.Append(p.Name); // parameter name
                }
                sb.AppendLine($")");

                // for a body, just emit default(x) unless void
                sb.AppendLine("{");
                emitCSharpBody(sb, m);
                sb.AppendLine("}");
            }
        }
    }
}