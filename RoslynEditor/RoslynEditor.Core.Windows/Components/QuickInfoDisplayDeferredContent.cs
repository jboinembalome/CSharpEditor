using System.Windows;

namespace RoslynEditor.Core.QuickInfo
{
    internal partial class DeferredQuickInfoContentProvider
    {
        private class QuickInfoDisplayDeferredContent : IDeferredQuickInfoContent
        {
            #region Fields

            private readonly IDeferredQuickInfoContent? _symbolGlyph;
            private readonly IDeferredQuickInfoContent? _warningGlyph;
            private readonly IDeferredQuickInfoContent _mainDescription;
            private readonly IDeferredQuickInfoContent _documentation;
            private readonly IDeferredQuickInfoContent _typeParameterMap;
            private readonly IDeferredQuickInfoContent _anonymousTypes;
            private readonly IDeferredQuickInfoContent _usageText;
            private readonly IDeferredQuickInfoContent _exceptionText;
            #endregion

            #region Constructors

            public QuickInfoDisplayDeferredContent(IDeferredQuickInfoContent? symbolGlyph, IDeferredQuickInfoContent? warningGlyph, 
                IDeferredQuickInfoContent mainDescription, IDeferredQuickInfoContent documentation, 
                IDeferredQuickInfoContent typeParameterMap, IDeferredQuickInfoContent anonymousTypes, 
                IDeferredQuickInfoContent usageText, IDeferredQuickInfoContent exceptionText)
            {
                _symbolGlyph = symbolGlyph;
                _warningGlyph = warningGlyph;
                _mainDescription = mainDescription;
                _documentation = documentation;
                _typeParameterMap = typeParameterMap;
                _anonymousTypes = anonymousTypes;
                _usageText = usageText;
                _exceptionText = exceptionText;
            }
            #endregion

            #region Methods
            public object Create()
            {
                object? warningGlyph = null;
                if (_warningGlyph != null)
                    warningGlyph = _warningGlyph.Create();

                object? symbolGlyph = null;
                if (_symbolGlyph != null)
                    symbolGlyph = _symbolGlyph.Create();

                return new QuickInfoDisplayPanel(symbolGlyph as FrameworkElement, warningGlyph as FrameworkElement,
                    (FrameworkElement)_mainDescription.Create(), (FrameworkElement)_documentation.Create(),
                    (FrameworkElement)_typeParameterMap.Create(), (FrameworkElement)_anonymousTypes.Create(),
                    (FrameworkElement)_usageText.Create(), (FrameworkElement)_exceptionText.Create());
            }
            #endregion
        }
    }
}