using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using System;
using System.Collections.Immutable;

namespace RoslynEditor.Core
{
    public interface IDocumentTrackingService : IWorkspaceService
    {
        event EventHandler<DocumentId> ActiveDocumentChanged;
        event EventHandler<EventArgs> NonRoslynBufferTextChanged;
        DocumentId GetActiveDocument();
        DocumentId? TryGetActiveDocument();
        ImmutableArray<DocumentId> GetVisibleDocuments();
    }
}
