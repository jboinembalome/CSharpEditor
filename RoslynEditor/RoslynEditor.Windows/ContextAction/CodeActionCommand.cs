using System;
using System.Windows.Input;
using Microsoft.CodeAnalysis.CodeActions;

namespace RoslynEditor.Windows
{
    public sealed partial class RoslynContextActionProvider
    {
        private class CodeActionCommand : ICommand
        {
            private readonly RoslynContextActionProvider _provider;
            private readonly CodeAction _codeAction;

            public CodeActionCommand(RoslynContextActionProvider provider, CodeAction codeAction)
            {
                _provider = provider;
                _codeAction = codeAction;
            }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            public bool CanExecute(object parameter) => true;

            public async void Execute(object parameter)
            {
                await _provider.ExecuteCodeAction(_codeAction).ConfigureAwait(true);
            }
        }
    }
}