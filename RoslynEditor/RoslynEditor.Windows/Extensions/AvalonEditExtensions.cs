using ICSharpCode.AvalonEdit.CodeCompletion;

namespace RoslynEditor.Windows
{
    public static class AvalonEditExtensions
    {
        public static bool IsOpen(this CompletionWindowBase window) => window?.IsVisible == true;
    }
}