using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.CodeFixes
{
    public sealed class CodeFixCollection
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.CodeFixes.CodeFixCollection _inner;
        #endregion

        #region Properties

        public object Provider => _inner.Provider;

        public TextSpan TextSpan => _inner.TextSpan;

        public ImmutableArray<CodeFix> Fixes { get; }
        #endregion

        #region Constructors

        internal CodeFixCollection(Microsoft.CodeAnalysis.CodeFixes.CodeFixCollection inner)
        {
            _inner = inner;
            Fixes = inner.Fixes.Select(x => new CodeFix(x)).ToImmutableArray();
        }
        #endregion
    }
}