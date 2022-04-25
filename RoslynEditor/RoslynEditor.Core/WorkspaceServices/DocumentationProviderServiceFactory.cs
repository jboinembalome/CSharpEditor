using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using System.Composition;

namespace RoslynEditor.Core.WorkspaceServices
{
    [ExportWorkspaceServiceFactory(typeof(IDocumentationProviderService), ServiceLayer.Host), Shared]
    internal sealed class DocumentationProviderServiceFactory : IWorkspaceServiceFactory
    {
        private readonly IDocumentationProviderService _service;

        [ImportingConstructor]
        public DocumentationProviderServiceFactory(IDocumentationProviderService service) => _service = service;

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices) => _service;
    }
}