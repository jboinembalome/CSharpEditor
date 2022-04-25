using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace RoslynEditor.Core.WorkspaceServices
{
    internal sealed partial class SymbolRenamedCodeActionOperationFactory
    {
        private class RenameSymbolOperation : CodeActionOperation
        {
            private readonly ISymbol _symbol;
            private readonly string _newName;

            public RenameSymbolOperation(ISymbol symbol, string newName)
            {
                _symbol = symbol;
                _newName = newName;
            }

            public override string Title => $"Rename {_symbol.Name} to {_newName}";
        }
    }
}
