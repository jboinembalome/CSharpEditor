using ICSharpCode.AvalonEdit.CodeCompletion;

namespace RoslynEditor.Windows
{
    public interface ICompletionDataEx : ICompletionData
    {
        bool IsSelected { get; }

        string SortText { get; }
    }
}