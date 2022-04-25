using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;

namespace RoslynEditor.Core
{
    [ExportWorkspaceServiceFactory(typeof(Microsoft.CodeAnalysis.IDocumentTrackingService))]
    internal sealed partial class DocumentTrackingServiceFactory : IWorkspaceServiceFactory
    {
        public IWorkspaceService? CreateService(HostWorkspaceServices workspaceServices)
        {
            var innerService = workspaceServices.GetService<IDocumentTrackingService>();
            return innerService != null ? new DocumentTrackingService(innerService) : null;
        }
    }
}
