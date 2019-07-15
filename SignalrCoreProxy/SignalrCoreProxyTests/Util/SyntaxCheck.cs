using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SignalrCoreClientHelper.CodeGen;
using System;

namespace SignalrCoreProxyTests.Util
{
    public static class SyntaxCheck
    {
        public static bool Check(string source, Type referenceInference)
        {
            // gather references
            var references = GetReferences(referenceInference);

            // prepare compilation
            var compilation = CSharpCompilation
                .Create($"_{Guid.NewGuid().ToString("N")}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references);

            compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source));

            var diags = compilation.GetDiagnostics();

            return diags.Length == 0;
        }

        static MetadataReference[] GetReferences(Type referenceInference)
        {
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ProxyCompiler).Assembly.Location),
                MetadataReference.CreateFromFile(referenceInference.Assembly.Location)
            };

            return references;
        }
    }
}
