using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.CodeRefactorings
{
    [Export(typeof(ICodeRefactoringService)), Shared]
    internal sealed class CodeRefactoringService : ICodeRefactoringService
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.CodeRefactorings.ICodeRefactoringService _inner;
        #endregion

        #region Constructors

        [ImportingConstructor]
        public CodeRefactoringService(Microsoft.CodeAnalysis.CodeRefactorings.ICodeRefactoringService inner)
        {
            _inner = inner;
        }
        #endregion

        #region Methods

        public Task<bool> HasRefactoringsAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken) => 
            _inner.HasRefactoringsAsync(document, textSpan, cancellationToken);

        public async Task<IEnumerable<CodeRefactoring>> GetRefactoringsAsync(Document document, TextSpan textSpan, CancellationToken cancellationToken)
        {
            var result = await _inner.GetRefactoringsAsync(document, textSpan, cancellationToken).ConfigureAwait(false);
            return result.Select(x => new CodeRefactoring(x)).ToArray();
        }
        #endregion



    }
}