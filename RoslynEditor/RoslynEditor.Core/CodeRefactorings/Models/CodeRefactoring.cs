using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace RoslynEditor.Core.CodeRefactorings
{
    public sealed class CodeRefactoring
    {
        #region Properties

        public CodeRefactoringProvider Provider { get; }

        public ImmutableArray<CodeAction> Actions { get; }
        #endregion

        #region Constructors

        internal CodeRefactoring(Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoring inner)
        {
            Provider = inner.Provider;
            Actions = inner.CodeActions.Select(c => c.action).ToImmutableArray();
        }
        #endregion
    }
}