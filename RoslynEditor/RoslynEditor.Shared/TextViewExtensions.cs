using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace RoslynEditor.Windows
{
    internal static class TextViewExtensions
    {
        public static Point GetPosition(this TextView textView, int line, int column)
        {
            var visualPosition = textView.GetVisualPosition(
                new TextViewPosition(line, column), VisualYPosition.LineBottom) - textView.ScrollOffset;
            return visualPosition;
        }
    }
}