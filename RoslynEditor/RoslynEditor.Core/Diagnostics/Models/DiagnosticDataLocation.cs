using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.Diagnostics
{
    public sealed class DiagnosticDataLocation
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.Diagnostics.DiagnosticDataLocation _inner;
        #endregion

        #region Properties

        public DocumentId? DocumentId => _inner.DocumentId;

        public TextSpan? SourceSpan => _inner.SourceSpan;

        public string? MappedFilePath => _inner.MappedFilePath;
        public int MappedStartLine => _inner.MappedStartLine;
        public int MappedStartColumn => _inner.MappedStartColumn;
        public int MappedEndLine => _inner.MappedEndLine;
        public int MappedEndColumn => _inner.MappedEndColumn;
        public string? OriginalFilePath => _inner.OriginalFilePath;
        public int OriginalStartLine => _inner.OriginalStartLine;
        public int OriginalStartColumn => _inner.OriginalStartColumn;
        public int OriginalEndLine => _inner.OriginalEndLine;
        public int OriginalEndColumn => _inner.OriginalEndColumn;
        #endregion

        #region Constructors

        internal DiagnosticDataLocation(Microsoft.CodeAnalysis.Diagnostics.DiagnosticDataLocation inner) => _inner = inner;
        #endregion
    }
}