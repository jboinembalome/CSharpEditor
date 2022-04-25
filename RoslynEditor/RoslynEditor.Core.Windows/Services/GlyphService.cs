using System.Windows.Media;
using RoslynEditor.Core.Completion;
using RoslynEditor.Core.Resources;

namespace RoslynEditor.Core
{
    public static partial class GlyphExtensions
    {
        private class GlyphService
        {
            private readonly Glyphs _glyphs;

            public GlyphService() => _glyphs = new Glyphs();

            public ImageSource? GetGlyphImage(Glyph glyph) => _glyphs[glyph] as ImageSource;
        }
    }
}