using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.QuickInfo
{
    public interface IQuickInfoProvider
    {
        Task<QuickInfoItem?> GetItemAsync(Document document, int position, CancellationToken cancellationToken);
    }
}