using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace ProxyGen
{
    public static class ProxyGenerator
    {
        public static TFactory Create<TFactory, TProxy>(Action<StringBuilder, MethodInfo> emitCSharpBody)
            where TFactory : class
            where TProxy : class
        {
            var factoryType = typeof(TFactory);
            var proxyType = typeof(TProxy);            

            Validate(factoryType, proxyType);

            var builder = new ProxyBuilder(factoryType, proxyType, emitCSharpBody);

            var proxySyntaxTree = GenerateProxySyntaxTree(builder);
            var factorySyntaxTree = GenerateFactorySyntaxTree(builder);

            var references = GetAssemblyReferences(factoryType, proxyType);

            var compilation = CSharpCompilation
                .Create($"_{Guid.NewGuid().ToString("N")}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(proxySyntaxTree, factorySyntaxTree);

            Assembly asm;

            using (var ms = new MemoryStream())
            {
                var er = compilation.Emit(ms);

                // rewind stream
                ms.Position = 0;

                asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(ms);
            }

            // reflect over asm to find TFactory, create and return instance

            var ft = asm.GetType($"{builder.Namespace}.FactoryImpl");

            return Activator.CreateInstance(ft) as TFactory;
        }

        static SyntaxTree GenerateProxySyntaxTree(ProxyBuilder builder)
        {
            var code = builder.GenerateProxyCode(true);
            return CSharpSyntaxTree.ParseText(code);
        }

        static SyntaxTree GenerateFactorySyntaxTree(ProxyBuilder builder)
        {
            var code =  builder.GenerateFactoryCode();
            return CSharpSyntaxTree.ParseText(code);
        }

        static IEnumerable<MetadataReference> GetAssemblyReferences(Type factoryType, Type proxyType)
        {
            // might need to gather GetReferencedAssemblies too
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(proxyType.Assembly.Location),
                MetadataReference.CreateFromFile(factoryType.Assembly.Location)
            };

            return references;
        }

        static void Validate(Type factoryType, Type proxyType)
        {
            // ensure factoryType is an interface
            if (!factoryType.IsInterface)
            {
                throw new ArgumentException("TFactory is not an interface");
            }

            // ensure proxy type is an interface
            if (!proxyType.IsInterface)
            {
                throw new ArgumentException("TProxy is not an interface");
            }

            // ensure proxy type is methods only
            var proxyMembers = proxyType.GetMembers();
            foreach(var member in proxyMembers)
            {
                if (member.MemberType != System.Reflection.MemberTypes.Method)
                {
                    throw new ApplicationException("TProxy defines a non-method member");
                }
            }

            // ensure factor interface has
            // one method
            // named create
            // returning proxy interface type
            var factoryMembers = factoryType.GetMembers();
            if (factoryMembers.Length != 1)
            {
                throw new ArgumentException($"TFactory has invalid number of members. One expected but {factoryMembers.Length} found");
            }

            // get Create method
            var createMethodInfo = factoryType.GetMethod("Create");
            if (createMethodInfo == null)
            {
                throw new ArgumentException($"TFactory does not define Create method");
            }

            // check create method return value
            if (createMethodInfo.ReturnType != proxyType)
            {
                throw new ArgumentException($"TFactory.Create does not return TProxy");
            }
        }
    }
}
