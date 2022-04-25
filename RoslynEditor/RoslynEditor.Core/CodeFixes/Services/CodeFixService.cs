using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.CodeFixes
{
    [Export(typeof(ICodeFixService)), Shared]
    internal sealed class CodeFixService : ICodeFixService
    {
        private readonly Internal.CodeFixService _inner;

        [ImportingConstructor]
        public CodeFixService(Internal.CodeFixService inner) => _inner = inner;

        public async Task<IEnumerable<CodeFixCollection>> GetFixesAsync(Document document, TextSpan textSpan, 
            bool includeSuppressionFixes, CancellationToken cancellationToken)
        {
            var result = await _inner.GetFixesAsync(document, textSpan, includeSuppressionFixes, cancellationToken)
                .ConfigureAwait(false);

            return result.Select(x => new CodeFixCollection(x)).ToImmutableArray();
        }

        public CodeFixProvider? GetSuppressionFixer(string language, IEnumerable<string> diagnosticIds) => 
            _inner.GetSuppressionFixer(language, diagnosticIds);
    }
}
