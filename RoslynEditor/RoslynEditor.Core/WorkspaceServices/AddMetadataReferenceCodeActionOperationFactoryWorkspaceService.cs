using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeActions.WorkspaceServices;
using Microsoft.CodeAnalysis.Host.Mef;

namespace RoslynEditor.Core.WorkspaceServices
{
    [ExportWorkspaceService(typeof(IAddMetadataReferenceCodeActionOperationFactoryWorkspaceService)), Shared]
    internal sealed partial class AddMetadataReferenceCodeActionOperationFactoryWorkspaceService : 
        IAddMetadataReferenceCodeActionOperationFactoryWorkspaceService
    {
        public CodeActionOperation CreateAddMetadataReferenceOperation(ProjectId projectId, AssemblyIdentity assemblyIdentity) => 
            new AddMetadataReferenceOperation(/*projectId, assemblyIdentity*/);
    }
}