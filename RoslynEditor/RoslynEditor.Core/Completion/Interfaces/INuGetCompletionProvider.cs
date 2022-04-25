using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RoslynEditor.Core.Completion
{
    public interface INuGetCompletionProvider
    {
        Task<IReadOnlyList<INuGetPackage>> SearchPackagesAsync(string searchString, bool exactMatch, 
            CancellationToken cancellationToken);
    }
}
