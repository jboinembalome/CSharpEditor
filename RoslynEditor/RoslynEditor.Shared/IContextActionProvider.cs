using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoslynEditor.Windows
{
    public interface IContextActionProvider
    {
        Task<IEnumerable<object>> GetActions(int offset, int length, CancellationToken cancellationToken);

        ICommand? GetActionCommand(object action);
    }
}