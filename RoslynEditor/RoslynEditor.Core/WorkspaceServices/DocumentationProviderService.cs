using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.Host;
using System.Composition;
using System.IO;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.WorkspaceServices
{
    [Export(typeof(IDocumentationProviderService)), Shared]
    internal sealed class DocumentationProviderService : IDocumentationProviderService
    {
        private readonly ConcurrentDictionary<string, DocumentationProvider?> _assemblyPathToDocumentationProviderMap
            = new();

        public DocumentationProvider? GetDocumentationProvider(string location)
        {
            string? finalPath = Path.ChangeExtension(location, "xml");

            return _assemblyPathToDocumentationProviderMap.GetOrAdd(location,
                _ =>
                {
                    if (!File.Exists(finalPath))
                        finalPath = GetFilePath(RoslynHostReferences.ReferenceAssembliesPath.docPath, finalPath) ??
                                    GetFilePath(RoslynHostReferences.ReferenceAssembliesPath.assemblyPath, finalPath);

                    return finalPath == null ? null : XmlDocumentationProvider.CreateFromFile(finalPath);
                });
        }

        private static string? GetFilePath(string? path, string location)
        {
            if (path != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var referenceLocation = Path.Combine(path, Path.GetFileName(location));
                if (File.Exists(referenceLocation))
                    return referenceLocation;
            }

            return null;
        }
    }

}