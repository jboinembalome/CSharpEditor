using System.Collections.Generic;

namespace RoslynEditor.Core.Snippets
{
    public interface ISnippetInfoService
    {
        IEnumerable<SnippetInfo> GetSnippets();
    }
}