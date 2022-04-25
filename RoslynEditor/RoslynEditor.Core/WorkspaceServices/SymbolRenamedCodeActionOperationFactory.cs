using System;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeActions.WorkspaceServices;
using Microsoft.CodeAnalysis.Host.Mef;

namespace RoslynEditor.Core.WorkspaceServices
{
    [ExportWorkspaceService(typeof(ISymbolRenamedCodeActionOperationFactoryWorkspaceService), ServiceLayer.Host), Shared]
    internal sealed partial class SymbolRenamedCodeActionOperationFactory : ISymbolRenamedCodeActionOperationFactoryWorkspaceService
    {
        public CodeActionOperation CreateSymbolRenamedOperation(ISymbol symbol, string newName, Solution startingSolution,
            Solution updatedSolution) =>
            // this action does nothing - but Roslyn requires it for some Code Fixes
            new RenameSymbolOperation(symbol ?? throw new ArgumentNullException(nameof(symbol)),
                newName ?? throw new ArgumentNullException(nameof(newName)));
    }
}
