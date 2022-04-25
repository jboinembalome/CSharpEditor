using System.Composition;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.Diagnostics
{
    [Export(typeof(IAnalyzerAssemblyLoader))]
    internal class SimpleAnalyzerAssemblyLoader : AnalyzerAssemblyLoader
    {
        public static IAnalyzerAssemblyLoader Instance { get; } = new SimpleAnalyzerAssemblyLoader();

        protected override Assembly LoadFromPathImpl(string fullPath) => Assembly.Load(AssemblyName.GetAssemblyName(fullPath));
    }
}
