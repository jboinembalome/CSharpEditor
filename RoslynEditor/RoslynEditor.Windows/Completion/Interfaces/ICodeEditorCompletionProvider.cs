using System.Threading.Tasks;

namespace RoslynEditor.Windows
{
    public interface ICodeEditorCompletionProvider
    {
        Task<CompletionResult> GetCompletionData(int position, char? triggerChar, bool useSignatureHelp);
    }
}