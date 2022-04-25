using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslyn.Utilities;

namespace RoslynEditor.Core.CodeFixes.Internal
{
    // From EditorFeatures
    internal partial class CodeFixService
    {
        private class FixAllPredefinedDiagnosticProvider : FixAllContext.DiagnosticProvider
        {
            private readonly ImmutableArray<Diagnostic> _diagnostics;

            public FixAllPredefinedDiagnosticProvider(ImmutableArray<Diagnostic> diagnostics) => _diagnostics = diagnostics;

            public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
                => Task.FromResult<IEnumerable<Diagnostic>>(_diagnostics);

            public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
                => Task.FromResult<IEnumerable<Diagnostic>>(_diagnostics);

            public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
                => SpecializedTasks.EmptyEnumerable<Diagnostic>();
        }
    }
}
