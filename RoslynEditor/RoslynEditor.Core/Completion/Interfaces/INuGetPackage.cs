using System.Collections.Generic;

namespace RoslynEditor.Core.Completion
{
    public interface INuGetPackage
    {
        string Id { get; }
        IEnumerable<string> Versions { get; }
    }
}
