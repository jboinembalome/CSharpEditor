using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.SignatureHelp
{
    [Export(typeof(ISignatureHelpProvider)), Shared]
    internal sealed partial class AggregateSignatureHelpProvider : ISignatureHelpProvider
    {
        #region Fields

        private ImmutableArray<Microsoft.CodeAnalysis.SignatureHelp.ISignatureHelpProvider> _providers;
        #endregion

        #region Constructors

        [ImportingConstructor]
        public AggregateSignatureHelpProvider([ImportMany] IEnumerable<Lazy<Microsoft.CodeAnalysis.SignatureHelp.ISignatureHelpProvider, 
            OrderableLanguageMetadata>> providers)
        {
            _providers = providers.Where(x => x.Metadata.Language == LanguageNames.CSharp)
                .Select(x => x.Value).ToImmutableArray();
        }
        #endregion

        #region Methods
        public bool IsTriggerCharacter(char ch) =>
            _providers.Any(p => p.IsTriggerCharacter(ch));

        public bool IsRetriggerCharacter(char ch) =>
            _providers.Any(p => p.IsRetriggerCharacter(ch));

        public async Task<SignatureHelpItems?> GetItemsAsync(Document document, int position, SignatureHelpTriggerInfo trigger,
            CancellationToken cancellationToken)
        {
            Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItems? bestItems = null;

            // TODO(cyrusn): We're calling into extensions, we need to make ourselves resilient
            // to the extension crashing.
            foreach (var provider in _providers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentItems = await provider.GetItemsAsync(document, position, trigger.Inner, cancellationToken)
                    .ConfigureAwait(false);
                if (currentItems != null && currentItems.ApplicableSpan.IntersectsWith(position))
                    // If another provider provides sig help items, then only take them if they
                    // start after the last batch of items.  i.e. we want the set of items that
                    // conceptually are closer to where the caret position is.  This way if you have:
                    //
                    //  Foo(new Bar($$
                    //
                    // Then invoking sig help will only show the items for "new Bar(" and not also
                    // the items for "Foo(..."
                    if (IsBetter(bestItems, currentItems.ApplicableSpan))
                        bestItems = currentItems;
            }

            if (bestItems != null)
            {
                var items = new SignatureHelpItems(bestItems);
                if (items.SelectedItemIndex == null)
                {
                    var selection = DefaultSignatureHelpSelector.GetSelection(items.Items, null, false, items.ArgumentIndex, 
                        items.ArgumentCount, items.ArgumentName, isCaseSensitive: true);
                    if (selection.SelectedItem != null)
                        items.SelectedItemIndex = items.Items.IndexOf(selection.SelectedItem);
                }

                return items;
            }

            return null;
        }

        private static bool IsBetter(Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItems? bestItems, TextSpan? currentTextSpan) => 
            bestItems == null || currentTextSpan?.Start > bestItems.ApplicableSpan.Start;
        #endregion
    }
}