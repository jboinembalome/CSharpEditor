using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.SignatureHelp
{
    internal sealed partial class AggregateSignatureHelpProvider
    {
        private static class DefaultSignatureHelpSelector
        {
            public static SignatureHelpSelection GetSelection(IList<SignatureHelpItem> items, SignatureHelpItem? selectedItem,
                bool userSelected, int argumentIndex, int argumentCount, string argumentName, bool isCaseSensitive)
            {
                selectedItem = SelectBestItem(selectedItem, ref userSelected, items, argumentIndex, argumentCount, argumentName,
                    isCaseSensitive);
                var selectedParameter = GetSelectedParameter(selectedItem, argumentIndex, argumentName, isCaseSensitive);

                return new SignatureHelpSelection(selectedItem, userSelected, selectedParameter);
            }

            private static int GetSelectedParameter(SignatureHelpItem bestItem, int parameterIndex, string parameterName,
                bool isCaseSensitive)
            {
                if (!string.IsNullOrEmpty(parameterName))
                {
                    var comparer = isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
                    var selected = bestItem.Parameters.Select((p, index) => (p, index))
                        .FirstOrDefault(p => comparer.Equals(p.p.Name, parameterName));

                    if (selected.p != null)
                        return selected.index;
                }

                return parameterIndex;
            }

            private static SignatureHelpItem SelectBestItem(SignatureHelpItem? currentItem, ref bool userSelected,
                IList<SignatureHelpItem> filteredItems, int selectedParameter, int argumentCount, string? name, bool isCaseSensitive)
            {
                // If the current item is still applicable, then just keep it.
                if (currentItem != null && filteredItems.Contains(currentItem) &&
                    IsApplicable(currentItem, argumentCount, name, isCaseSensitive))
                    // If the current item was user-selected, we keep it as such.
                    return currentItem;

                // If the current item is no longer applicable, we'll be choosing a new one,
                // which was definitely not previously user-selected.
                userSelected = false;

                // Try to find the first applicable item.  If there is none, then that means the
                // selected parameter was outside the bounds of all methods.  i.e. all methods only
                // went up to 3 parameters, and selected parameter is 3 or higher.  In that case,
                // just pick the very last item as it is closest in parameter count.
                var result = filteredItems.FirstOrDefault(i => IsApplicable(i, argumentCount, name, isCaseSensitive));
                if (result != null)
                {
                    currentItem = result;
                    return currentItem;
                }

                // if we couldn't find a best item, and they provided a name, then try again without
                // a name.
                if (name != null)
                    return SelectBestItem(currentItem, ref userSelected, filteredItems, selectedParameter, argumentCount, null, 
                        isCaseSensitive);

                // If we don't have an item that can take that number of parameters, then just pick
                // the last item.  Or stick with the current item if the last item isn't any better.
                var lastItem = filteredItems.Last();
                if (currentItem != null && (currentItem.IsVariadic || currentItem.Parameters.Length == lastItem.Parameters.Length))
                    return currentItem;

                return lastItem;
            }

            private static bool IsApplicable(SignatureHelpItem item, int argumentCount, string? name, bool isCaseSensitive)
            {
                // If they provided a name, then the item is only valid if it has a parameter that
                // matches that name.
                if (name != null)
                {
                    var comparer = isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
                    return item.Parameters.Any(p => comparer.Equals(p.Name, name));
                }

                // An item is applicable if it has at least as many parameters as the selected
                // parameter index.  i.e. if it has 2 parameters and we're at index 0 or 1 then it's
                // applicable. If it has 2 parameters and we're at index 2, then it's not
                // applicable. However, if it is variadic then it is applicable as it can take any number of
                // items.
                if (item.Parameters.Length >= argumentCount || item.IsVariadic)
                    return true;

                // Also, we special case 0.  that's because if the user has "Goo(" and goo takes no
                // arguments, then we'll see that it's arg count is 0.  We still want to consider
                // any item applicable here though.
                return argumentCount == 0;
            }
        }
    }
}