using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.Diagnostics
{
    public class DiagnosticsUpdatedArgs : UpdatedEventArgs
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.Diagnostics.DiagnosticsUpdatedArgs _inner;
        #endregion

        #region Properties

        public DiagnosticsUpdatedKind Kind { get; }
        public Solution? Solution { get; }
        public ImmutableArray<DiagnosticData> Diagnostics { get; private set; }

        #endregion

        #region Constructors

        internal DiagnosticsUpdatedArgs(Microsoft.CodeAnalysis.Diagnostics.DiagnosticsUpdatedArgs inner, ImmutableArray<DiagnosticData>? diagnostics = null) : base(inner)
        {
            _inner = inner;
            Solution = inner.Solution;
            Diagnostics = diagnostics ?? inner.Diagnostics.Select(x => new DiagnosticData(x)).ToImmutableArray();
            Kind = (DiagnosticsUpdatedKind)inner.Kind;
        }
        #endregion

        #region Methods

        public DiagnosticsUpdatedArgs WithDiagnostics(ImmutableArray<DiagnosticData> diagnostics) => 
            new(_inner, diagnostics);
        #endregion

    }
}