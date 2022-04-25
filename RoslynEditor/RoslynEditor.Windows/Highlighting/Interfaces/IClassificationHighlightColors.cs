using ICSharpCode.AvalonEdit.Highlighting;

namespace RoslynEditor.Windows
{
    public interface IClassificationHighlightColors
    {
        HighlightingColor DefaultBrush { get; }

        HighlightingColor GetBrush(string classificationTypeName);
    }
}