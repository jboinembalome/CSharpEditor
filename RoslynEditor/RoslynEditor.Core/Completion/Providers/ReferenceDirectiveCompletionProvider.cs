using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynEditor.Core.Completion.Providers
{
    [ExportCompletionProvider("ReferenceDirectiveCompletionProvider", LanguageNames.CSharp)]
    internal class ReferenceDirectiveCompletionProvider : AbstractReferenceDirectiveCompletionProvider
    {
        #region Fields

        const string NuGetPrefix = "nuget:";

        private static readonly CompletionItemRules s_rules = CompletionItemRules.Create(
            filterCharacterRules: ImmutableArray<CharacterSetModificationRule>.Empty,
            commitCharacterRules: ImmutableArray<CharacterSetModificationRule>.Empty,
            enterKeyRule: EnterKeyRule.Never,
            selectionBehavior: CompletionItemSelectionBehavior.SoftSelection);

        private readonly INuGetCompletionProvider _nuGetCompletionProvider;
        #endregion

        #region Constructors

        [ImportingConstructor]
        public ReferenceDirectiveCompletionProvider([Import(AllowDefault = true)] INuGetCompletionProvider nuGetCompletionProvider) =>
            _nuGetCompletionProvider = nuGetCompletionProvider;

        #endregion

        #region Methods

        protected override Task ProvideCompletionsAsync(CompletionContext context, string pathThroughLastSlash)
        {
            if (_nuGetCompletionProvider != null &&
                pathThroughLastSlash.StartsWith(NuGetPrefix, StringComparison.InvariantCultureIgnoreCase))
                return ProvideNuGetCompletionsAsync(context, pathThroughLastSlash);

            if (string.IsNullOrEmpty(pathThroughLastSlash))
                context.AddItem(CreateNuGetRoot());

            return base.ProvideCompletionsAsync(context, pathThroughLastSlash);
        }

        protected override bool TryGetStringLiteralToken(SyntaxTree tree, int position, out SyntaxToken stringLiteral,
            CancellationToken cancellationToken) => tree.TryGetStringLiteralToken(position, SyntaxKind.ReferenceDirectiveTrivia, 
                out stringLiteral, cancellationToken);

        private static CompletionItem CreateNuGetRoot() => CommonCompletionItem.Create(displayText: NuGetPrefix, displayTextSuffix: "",
            rules: s_rules, glyph: Microsoft.CodeAnalysis.Glyph.NuGet, sortText: "");

        private async Task ProvideNuGetCompletionsAsync(CompletionContext context, string packageIdAndVersion)
        {
            var (id, version) = ParseNuGetReference(packageIdAndVersion);
            var packages = await Task.Run(() => _nuGetCompletionProvider.SearchPackagesAsync(id, exactMatch: version != null, context.CancellationToken), context.CancellationToken).ConfigureAwait(false);

            if (version != null)
            {
                if (packages.Count > 0)
                {
                    var package = packages[0];
                    var versions = package.Versions;
                    if (!string.IsNullOrWhiteSpace(version))
                        versions = versions.Where(v => v.StartsWith(version, StringComparison.InvariantCultureIgnoreCase));

                    context.AddItems(versions.Select((v, i) =>
                        CommonCompletionItem.Create(v, "", s_rules, Microsoft.CodeAnalysis.Glyph.NuGet,
                        sortText: i.ToString("0000"))));
                }
            }
            else
            {
                context.AddItems(packages.Select((p, i) =>
                    CommonCompletionItem.Create(NuGetPrefix + p.Id + "/", "", s_rules, Microsoft.CodeAnalysis.Glyph.NuGet,
                    sortText: i.ToString("0000"))));
            }
        }

        private static (string id, string? version) ParseNuGetReference(string value)
        {
            string id;
            string? version;

            var indexOfSlash = value.IndexOf('/');
            if (indexOfSlash >= 0)
            {
                id = value[NuGetPrefix.Length..indexOfSlash];
                version = indexOfSlash != value.Length - 1 ? value[(indexOfSlash + 1)..] : string.Empty;
            }
            else
            {
                id = value[NuGetPrefix.Length..];
                version = null;
            }

            return (id, version);
        }
        #endregion
    }
}