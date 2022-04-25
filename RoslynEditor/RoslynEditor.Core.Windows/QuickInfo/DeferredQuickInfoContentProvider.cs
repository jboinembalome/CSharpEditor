using System.Collections.Generic;
using System.Composition;
using Microsoft.CodeAnalysis;
using RoslynEditor.Core.Completion;

namespace RoslynEditor.Core.QuickInfo
{
    [Export(typeof(IDeferredQuickInfoContentProvider))]
    internal partial class DeferredQuickInfoContentProvider : IDeferredQuickInfoContentProvider
    {
        public IDeferredQuickInfoContent CreateQuickInfoDisplayDeferredContent(ISymbol symbol, bool showWarningGlyph, 
            bool showSymbolGlyph, IList<TaggedText> mainDescription, IDeferredQuickInfoContent documentation,
            IList<TaggedText> typeParameterMap, IList<TaggedText> anonymousTypes, IList<TaggedText> usageText,
            IList<TaggedText> exceptionText)
        {
            return new QuickInfoDisplayDeferredContent(
                symbolGlyph: showSymbolGlyph ? CreateGlyphDeferredContent(symbol) : null,
                warningGlyph: showWarningGlyph ? CreateWarningGlyph() : null,
                mainDescription: CreateClassifiableDeferredContent(mainDescription),
                documentation: documentation,
                typeParameterMap: CreateClassifiableDeferredContent(typeParameterMap),
                anonymousTypes: CreateClassifiableDeferredContent(anonymousTypes),
                usageText: CreateClassifiableDeferredContent(usageText),
                exceptionText: CreateClassifiableDeferredContent(exceptionText));
        }

        private static IDeferredQuickInfoContent CreateGlyphDeferredContent(ISymbol symbol) =>
            new SymbolGlyphDeferredContent(symbol.GetGlyph());

        private static IDeferredQuickInfoContent CreateWarningGlyph() =>
            new SymbolGlyphDeferredContent(Glyph.CompletionWarning);

        public IDeferredQuickInfoContent CreateDocumentationCommentDeferredContent(string? documentationComment) =>
            new DocumentationCommentDeferredContent(documentationComment);

        public IDeferredQuickInfoContent CreateClassifiableDeferredContent(IList<TaggedText> content) =>
            new ClassifiableDeferredContent(content);
    }
}