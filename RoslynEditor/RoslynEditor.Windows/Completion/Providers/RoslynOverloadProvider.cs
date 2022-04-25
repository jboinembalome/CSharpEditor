using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using CommonFontWeights = System.Windows.FontWeights;
using RoslynEditor.Core;
using RoslynEditor.Core.SignatureHelp;

namespace RoslynEditor.Windows
{
    internal sealed class RoslynOverloadProvider : NotificationObject, IOverloadProviderEx
    {
        #region Fields

        private readonly SignatureHelpItems _signatureHelp;
        private readonly IList<SignatureHelpItem> _items;

        private int _selectedIndex;
        private SignatureHelpItem? _item;
        private object? _currentHeader;
        private object? _currentContent;
        private string? _currentIndexText;
        #endregion

        #region Properties

        public int SelectedIndex
        {
            get => _selectedIndex; set
            {
                if (SetProperty(ref _selectedIndex, value))
                    Refresh();
            }
        }

        public int Count => _items.Count;

        // ReSharper disable once UnusedMember.Local
        public string? CurrentIndexText
        {
            get => _currentIndexText;
            private set => SetProperty(ref _currentIndexText, value);
        }

        public object? CurrentHeader
        {
            get => _currentHeader;
            private set => SetProperty(ref _currentHeader, value);
        }

        public object? CurrentContent
        {
            get => _currentContent;
            private set => SetProperty(ref _currentContent, value);
        }
        #endregion

        #region Constructors

        public RoslynOverloadProvider(SignatureHelpItems signatureHelp)
        {
            _signatureHelp = signatureHelp;
            _items = signatureHelp.Items;

            if (signatureHelp.SelectedItemIndex != null)
                _selectedIndex = signatureHelp.SelectedItemIndex.Value;
        }
        #endregion

        #region Methods

        public void Refresh()
        {
            _item = _items[_selectedIndex];
            var headerPanel = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    _item.PrefixDisplayParts.ToTextBlock(),
                }
            };
            var contentPanel = new StackPanel();

            var docText = _item.DocumentationFactory(CancellationToken.None).ToTextBlock();
            if (HasContent(docText))
                contentPanel.Children.Add(docText);

            if (!_item.Parameters.IsDefault)
                for (var index = 0; index < _item.Parameters.Length; index++)
                {
                    var param = _item.Parameters[index];
                    AddParameterSignatureHelp(_item, index, param, headerPanel, contentPanel);
                }

            headerPanel.Children.Add(_item.SuffixDisplayParts.ToTextBlock());
            CurrentHeader = headerPanel;
            CurrentContent = contentPanel;
            CurrentIndexText = $" {_selectedIndex + 1} of {_items.Count} ";
        }

        private bool HasContent(TextBlock textBlock) => textBlock?.Inlines.Count > 0;

        private void AddParameterSignatureHelp(SignatureHelpItem item, int index, SignatureHelpParameter param, Panel headerPanel, 
            Panel contentPanel)
        {
            var isSelected = _signatureHelp.ArgumentIndex == index;
            headerPanel.Children.Add(param.DisplayParts.ToTextBlock(isBold: isSelected));

            if (index != item.Parameters.Length - 1)
                headerPanel.Children.Add(item.SeparatorDisplayParts.ToTextBlock());

            if (isSelected)
            {
                var textBlock = param.DocumentationFactory(CancellationToken.None).ToTextBlock();
                if (HasContent(textBlock))
                    contentPanel.Children.Add(new WrapPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock { Text = param.Name + ": ", FontWeight = CommonFontWeights.Bold },
                            textBlock
                        }
                    });
            }
        }
        #endregion        
    }
}