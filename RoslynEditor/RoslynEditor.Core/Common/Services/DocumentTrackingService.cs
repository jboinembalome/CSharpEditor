using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core
{
    internal sealed partial class DocumentTrackingServiceFactory
    {
        private class DocumentTrackingService : Microsoft.CodeAnalysis.IDocumentTrackingService
        {
            #region Fields

            private readonly IDocumentTrackingService _inner;
            #endregion

            #region Constructors

            public DocumentTrackingService(IDocumentTrackingService inner) =>
               _inner = inner;
            #endregion

            #region Events

            public event EventHandler<DocumentId> ActiveDocumentChanged
            {
                add => _inner.ActiveDocumentChanged += value;
                remove => _inner.ActiveDocumentChanged -= value;
            }

            public event EventHandler<EventArgs> NonRoslynBufferTextChanged
            {
                add => _inner.NonRoslynBufferTextChanged += value;
                remove => _inner.NonRoslynBufferTextChanged -= value;
            }
            #endregion

            #region Methods

            public DocumentId GetActiveDocument() => _inner.GetActiveDocument();

            public ImmutableArray<DocumentId> GetVisibleDocuments() => _inner.GetVisibleDocuments();

            public DocumentId? TryGetActiveDocument() => _inner.TryGetActiveDocument();
            #endregion
        }
    }
}