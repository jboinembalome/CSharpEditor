using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace RoslynEditor.Core.WorkspaceServices
{
    internal sealed partial class AddMetadataReferenceCodeActionOperationFactoryWorkspaceService
    {
        private class AddMetadataReferenceOperation : CodeActionOperation
        {
            //private readonly AssemblyIdentity _assemblyIdentity;
            //private readonly ProjectId _projectId;

            //public AddMetadataReferenceOperation(ProjectId projectId, AssemblyIdentity assemblyIdentity)
            //{
            //    //_projectId = projectId;
            //    //_assemblyIdentity = assemblyIdentity;
            //}

            public override void Apply(Workspace workspace, CancellationToken cancellationToken)
            {
                //var roslynEditorWorkspace = workspace as RoslynWorkspace;
                //roslynEditorWorkspace?.RoslynHost?.AddMetadataReference(_projectId, _assemblyIdentity);

                //RoslynHost.AddMetadataReference(_projectId, _assemblyIdentity);
            }

            public override string Title => "Add Reference";
        }
    }
}