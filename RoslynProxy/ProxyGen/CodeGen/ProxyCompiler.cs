using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ProxyGen.CodeGen
{
    internal static class ProxyCompiler
    {
        internal static Assembly Compile(string[] sourceCode, Type clientInterfaceType, Type serverInterfaceType)
        {
            // gather references
            var references = GetAssemblyReferences(clientInterfaceType, serverInterfaceType);

            // prepare compilation
            var compilation = CSharpCompilation
                .Create($"_{Guid.NewGuid().ToString("N")}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references);

            // generate and add syntax trees for each block of source code
            foreach(var code in sourceCode)
            {
                compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
            }
           
            using (var ms = new MemoryStream())
            {
                var er = compilation.Emit(ms);

                if (!er.Success) throw new ApplicationException("Failure compiling generated code");

                // rewind stream
                ms.Position = 0;

                return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(ms);
            }
        }
        
        static IEnumerable<MetadataReference> GetAssemblyReferences(Type factoryType, Type proxyType)
        {
            // might need to gather GetReferencedAssemblies too
            var references = new MetadataReference[]
            {                
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ProxyCompiler).Assembly.Location),
                MetadataReference.CreateFromFile(proxyType.Assembly.Location),
                MetadataReference.CreateFromFile(factoryType.Assembly.Location)
            };

            return references;
        }
    }
}
