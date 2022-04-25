using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.Diagnostics
{
    [Export(typeof(IDiagnosticService)), Shared]
    internal sealed class DiagnosticsService : IDiagnosticService
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.Diagnostics.IDiagnosticService _inner;
        public event EventHandler<DiagnosticsUpdatedArgs>? DiagnosticsUpdated;
        #endregion

        #region Constructors

        [ImportingConstructor]
        public DiagnosticsService(Microsoft.CodeAnalysis.Diagnostics.IDiagnosticService inner)
        {
            _inner = inner;
            inner.DiagnosticsUpdated += OnDiagnosticsUpdated;
        }
        #endregion

        #region Methods

        public IEnumerable<DiagnosticData> GetDiagnostics(Workspace workspace, ProjectId projectId, DocumentId documentId, object id,
            bool includeSuppressedDiagnostics, CancellationToken cancellationToken) => 
            _inner.GetDiagnostics(workspace, projectId, documentId, id, includeSuppressedDiagnostics,
                cancellationToken).Select(x => new DiagnosticData(x));

        public IEnumerable<UpdatedEventArgs> GetDiagnosticsUpdatedEventArgs(Workspace workspace, ProjectId projectId, DocumentId documentId,
            CancellationToken cancellationToken) =>
            _inner.GetDiagnosticsUpdatedEventArgs(workspace, projectId, documentId, cancellationToken)
            .Select(x => new UpdatedEventArgs(x));

        // ReSharper disable once UnusedParameter.Local
        private void OnDiagnosticsUpdated(object sender, Microsoft.CodeAnalysis.Diagnostics.DiagnosticsUpdatedArgs e) => 
            DiagnosticsUpdated?.Invoke(this, new DiagnosticsUpdatedArgs(e));
        #endregion
    }
}