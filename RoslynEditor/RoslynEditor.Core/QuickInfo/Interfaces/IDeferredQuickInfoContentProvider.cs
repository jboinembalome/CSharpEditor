using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.QuickInfo
{
    internal interface IDeferredQuickInfoContentProvider
    {
        IDeferredQuickInfoContent CreateQuickInfoDisplayDeferredContent(
            ISymbol symbol,
            bool showWarningGlyph,
            bool showSymbolGlyph,
            IList<TaggedText> mainDescription,
            IDeferredQuickInfoContent documentation,
            IList<TaggedText> typeParameterMap,
            IList<TaggedText> anonymousTypes,
            IList<TaggedText> usageText,
            IList<TaggedText> exceptionText);

        IDeferredQuickInfoContent CreateDocumentationCommentDeferredContent(string? documentationComment);

        IDeferredQuickInfoContent CreateClassifiableDeferredContent(IList<TaggedText> content);
    }
}
