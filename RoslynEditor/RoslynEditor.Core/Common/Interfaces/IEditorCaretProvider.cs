namespace RoslynEditor.Core
{
    public interface IEditorCaretProvider
    {
        int CaretPosition { get; }

        bool TryMoveCaret(int position);
    }
}
