using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Text;
using RoslynEditor.Core;
using RoslynEditor.Core.CodeActions;
using RoslynEditor.Core.CodeFixes;
using RoslynEditor.Core.CodeRefactorings;

namespace RoslynEditor.Windows
{
    public sealed partial class RoslynContextActionProvider : IContextActionProvider
    {
        #region Fields

        private static readonly ImmutableArray<string> ExcludedRefactoringProviders =
            ImmutableArray.Create("ExtractInterface");

        private readonly DocumentId _documentId;
        private readonly IRoslynHost _roslynHost;
        private readonly ICodeFixService _codeFixService;
        #endregion

        #region Constructors

        public RoslynContextActionProvider(DocumentId documentId, IRoslynHost roslynHost)
        {
            _documentId = documentId;
            _roslynHost = roslynHost;
            _codeFixService = _roslynHost.GetService<ICodeFixService>();
        }
        #endregion

        #region Methods

        public async Task<IEnumerable<object>> GetActions(int offset, int length, CancellationToken cancellationToken)
        {
            var textSpan = new TextSpan(offset, length);

            var document = _roslynHost.GetDocument(_documentId);
            if (document == null)
                return Array.Empty<object>();

            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            if (textSpan.End >= text.Length) 
                return Array.Empty<object>();

            var codeFixes = await _codeFixService.GetFixesAsync(document,textSpan, false, cancellationToken).ConfigureAwait(false);
            var codeRefactorings = await _roslynHost.GetService<ICodeRefactoringService>()
                .GetRefactoringsAsync(document, textSpan, cancellationToken).ConfigureAwait(false);

            return ((IEnumerable<object>)codeFixes.SelectMany(x => x.Fixes))
                .Concat(codeRefactorings
                .Where(x => ExcludedRefactoringProviders.All(p => !x.Provider.GetType().Name.Contains(p)))
                .SelectMany(x => x.Actions));
        }

        public ICommand? GetActionCommand(object action) => 
            action is not CodeFix codeFix || codeFix.Action.HasCodeActions() 
            ? (global::System.Windows.Input.ICommand)null 
            : (global::System.Windows.Input.ICommand)(action is CodeAction codeAction 
                ? new CodeActionCommand(this, codeAction) 
                : new CodeActionCommand(this, codeFix.Action));

        private async Task ExecuteCodeAction(CodeAction codeAction)
        {
            var operations = await codeAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(true);
            foreach (var operation in operations)
            {
                var document = _roslynHost.GetDocument(_documentId);
                if (document != null)
                    operation.Apply(document.Project.Solution.Workspace, CancellationToken.None);
            }
        }

        #endregion
    }
}