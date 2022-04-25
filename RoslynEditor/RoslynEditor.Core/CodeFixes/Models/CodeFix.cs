using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace RoslynEditor.Core.CodeFixes
{
    public sealed class CodeFix
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.CodeFixes.CodeFix _inner;
        #endregion

        #region Properties

        public Project Project => _inner.Project;

        public CodeAction Action => _inner.Action;

        public ImmutableArray<Diagnostic> Diagnostics => _inner.Diagnostics;

        public Diagnostic PrimaryDiagnostic => _inner.PrimaryDiagnostic;
        #endregion

        #region Contructors

        internal CodeFix(Microsoft.CodeAnalysis.CodeFixes.CodeFix inner)
        {
            _inner = inner;
        }
        #endregion


    }
}