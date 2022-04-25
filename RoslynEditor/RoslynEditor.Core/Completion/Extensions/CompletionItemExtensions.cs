using Microsoft.CodeAnalysis.Completion;
using RoslynEditor.Core.CodeActions;

namespace RoslynEditor.Core.Completion
{
    public static class CompletionItemExtensions
    {
        public static Glyph GetGlyph(this CompletionItem completionItem) =>
            CodeActionExtensions.GetGlyph(completionItem.Tags);

        public static CompletionDescription GetDescription(this CompletionItem completionItem) =>
            CommonCompletionItem.GetDescription(completionItem);
    }
}