using System.Collections.Generic;
using System.Windows;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.QuickInfo
{
    internal partial class DeferredQuickInfoContentProvider
    {
        private class ClassifiableDeferredContent : IDeferredQuickInfoContent
        {
            private readonly IList<TaggedText> _classifiableContent;

            public ClassifiableDeferredContent(IList<TaggedText> content)
            {
                _classifiableContent = content;
            }

            public object Create()
            {
                var textBlock = _classifiableContent.ToTextBlock();
                if (textBlock.Inlines.Count == 0)
                    textBlock.Visibility = Visibility.Collapsed;
                return textBlock;
            }
        }
    }
}