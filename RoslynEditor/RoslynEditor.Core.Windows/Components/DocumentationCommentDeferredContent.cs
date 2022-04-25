using System.Windows;
using System.Windows.Controls;

namespace RoslynEditor.Core.QuickInfo
{
    internal partial class DeferredQuickInfoContentProvider
    {
        private class DocumentationCommentDeferredContent : IDeferredQuickInfoContent
        {
            #region Fields

            private readonly string? _documentationComment;
            #endregion

            #region Constructors

            public DocumentationCommentDeferredContent(string? documentationComment) => _documentationComment = documentationComment;
            #endregion

            #region Methods

            public object Create()
            {
                var documentationTextBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap
                };

                UpdateDocumentationTextBlock(documentationTextBlock);

                return documentationTextBlock;
            }

            private void UpdateDocumentationTextBlock(TextBlock documentationTextBlock)
            {
                if (!string.IsNullOrEmpty(_documentationComment))
                    documentationTextBlock.Text = _documentationComment;
                else
                {
                    documentationTextBlock.Text = string.Empty;
                    documentationTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            #endregion
        }
    }
}