using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using TextDocument = ICSharpCode.AvalonEdit.Document.TextDocument;
using Microsoft.CodeAnalysis;
using RoslynEditor.Core;

namespace RoslynEditor.Windows
{
    public sealed class RoslynHighlightingColorizer : HighlightingColorizer
    {
        private readonly DocumentId _documentId;
        private readonly IRoslynHost _roslynHost;
        private readonly IClassificationHighlightColors _highlightColors;

        public RoslynHighlightingColorizer(DocumentId documentId, IRoslynHost roslynHost, 
            IClassificationHighlightColors highlightColors)
        {
            _documentId = documentId;
            _roslynHost = roslynHost;
            _highlightColors = highlightColors;
        }

        protected override IHighlighter CreateHighlighter(TextView textView, TextDocument document) => 
            new RoslynSemanticHighlighter(textView, document, _documentId, _roslynHost, _highlightColors);
    }
}