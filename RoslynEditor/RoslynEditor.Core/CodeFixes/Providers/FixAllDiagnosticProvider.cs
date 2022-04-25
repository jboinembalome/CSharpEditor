using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace RoslynEditor.Core.CodeFixes.Internal
{
    // From EditorFeatures
    internal partial class CodeFixService
    {
        private class FixAllDiagnosticProvider : FixAllContext.DiagnosticProvider
        {
            private readonly CodeFixService _codeFixService;
            private readonly ImmutableHashSet<string> _diagnosticIds;

            public FixAllDiagnosticProvider(CodeFixService codeFixService, ImmutableHashSet<string> diagnosticIds)
            {
                _codeFixService = codeFixService;
                _diagnosticIds = diagnosticIds;
            }

            public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
                => _codeFixService.GetDocumentDiagnosticsAsync(document, _diagnosticIds, cancellationToken);

            public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
                => _codeFixService.GetProjectDiagnosticsAsync(project, true, _diagnosticIds, cancellationToken);

            public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
                => _codeFixService.GetProjectDiagnosticsAsync(project, false, _diagnosticIds, cancellationToken);
        }
    }
}
