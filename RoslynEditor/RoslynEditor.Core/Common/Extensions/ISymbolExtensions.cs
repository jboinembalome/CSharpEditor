using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core
{
    // ReSharper disable once InconsistentNaming
    public static class ISymbolExtensions
    {
        public static Completion.Glyph GetGlyph(this ISymbol symbol) => 
            (Completion.Glyph)Microsoft.CodeAnalysis.Shared.Extensions.ISymbolExtensions2.GetGlyph(symbol);
    }
}