using System.Windows.Media;
using RoslynEditor.Core.Completion;

namespace RoslynEditor.Core
{
    public static partial class GlyphExtensions
    {
        private static readonly GlyphService _service = new();

        public static ImageSource? ToImageSource(this Glyph glyph) => _service.GetGlyphImage(glyph);
    }
}