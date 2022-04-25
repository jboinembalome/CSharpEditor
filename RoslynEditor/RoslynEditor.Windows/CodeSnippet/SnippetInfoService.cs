using System.Collections.Generic;
using System.Composition;
using System.Linq;
using RoslynEditor.Core.Snippets;

namespace RoslynEditor.Windows
{
    [Export(typeof(ISnippetInfoService)), Shared]
    internal sealed class SnippetInfoService : ISnippetInfoService
    {
        public SnippetManager SnippetManager { get; } = new SnippetManager();

        public IEnumerable<SnippetInfo> GetSnippets() => 
            SnippetManager.Snippets.Select(x => new SnippetInfo(x.Name, x.Name, x.Description));
    }
}