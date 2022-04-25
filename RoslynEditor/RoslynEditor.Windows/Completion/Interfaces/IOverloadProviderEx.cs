using ICSharpCode.AvalonEdit.CodeCompletion;

namespace RoslynEditor.Windows
{
    public interface IOverloadProviderEx : IOverloadProvider
    {
        void Refresh();
    }
}