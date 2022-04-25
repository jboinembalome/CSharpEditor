using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using System.Globalization;

namespace RoslynEditor.Core.Completion
{
    public sealed class CompletionHelper
    {
        #region Fields

        private readonly Microsoft.CodeAnalysis.Completion.CompletionHelper _inner;
        #endregion

        #region Constructors

        private CompletionHelper(Microsoft.CodeAnalysis.Completion.CompletionHelper inner) => _inner = inner;
        #endregion

        #region Methods

        public static CompletionHelper GetHelper(Document document /*,CompletionService service*/) => 
            new(Microsoft.CodeAnalysis.Completion.CompletionHelper.GetHelper(document));

        public bool MatchesFilterText(CompletionItem item, string filterText) =>
            _inner.MatchesPattern(item.FilterText, filterText, CultureInfo.InvariantCulture);
        #endregion
    }
}