using System.Windows.Controls;
using RoslynEditor.Core.Completion;

namespace RoslynEditor.Core.QuickInfo
{
    internal partial class DeferredQuickInfoContentProvider
    {
        private class SymbolGlyphDeferredContent : IDeferredQuickInfoContent
        {
            private Glyph Glyph { get; }

            public SymbolGlyphDeferredContent(Glyph glyph) => Glyph = glyph;

            public object Create()
            {
                var image = new Image
                {
                    Width = 16,
                    Height = 16,
                    Source = Glyph.ToImageSource()
                };

                return image;
            }
        }
    }
}