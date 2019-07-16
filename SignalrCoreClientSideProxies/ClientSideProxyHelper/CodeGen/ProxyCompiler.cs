using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ClientSideProxyHelper.CodeGen
{
    internal static class ProxyCompiler
    {
        internal static Assembly Compile(string[] sourceCode, Type clientInterfaceType, Type serverInterfaceType)
        {
            // gather references
            var references = GetAssemblyReferences(clientInterfaceType, serverInterfaceType);

            var syntaxTrees = new List<SyntaxTree>();
            // generate and add syntax trees for each block of source code
            foreach (var code in sourceCode)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(code));
            }

            // prepare compilation
            var compilation = CSharpCompilation
                .Create($"_{Guid.NewGuid().ToString("N")}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            var diags = compilation.GetDiagnostics();
            if (diags.Length!=0)
            {
                throw new ApplicationException("Diagnostic errors");
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
            var mscorlib = ResolveDllLocation("mscorlib");
            var runtime = ResolveDllLocation("System.Runtime");


            // might need to gather GetReferencedAssemblies too
            var references = new MetadataReference[]
            {                
                //MetadataReference.CreateFromFile(mscorlib),
                MetadataReference.CreateFromFile(runtime),
                MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Threading.Tasks.Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ProxyCompiler).Assembly.Location),
                MetadataReference.CreateFromFile(proxyType.Assembly.Location),
                MetadataReference.CreateFromFile(factoryType.Assembly.Location)
            };

            return references;
        }

        static string[] trustedAssembliesPaths;
        static string ResolveDllLocation(string trustedDll)
        {
            if (trustedAssembliesPaths==null)
            {
                trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            }

            return trustedAssembliesPaths
                .Where(p => 0 == string.Compare(Path.GetFileNameWithoutExtension(p), trustedDll, true))
                .FirstOrDefault();                
        }
    }
}
