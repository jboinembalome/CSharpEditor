using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.DocumentationComments;
using Microsoft.CodeAnalysis.ExternalAccess.Pythia.Api;
using Microsoft.CodeAnalysis.LanguageServices;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Roslyn.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RoslynEditor.Core.QuickInfo
{
    [Export(typeof(IQuickInfoProvider)), Shared]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    internal sealed partial class QuickInfoProvider : IQuickInfoProvider
    {
        #region Fields

        private readonly IDeferredQuickInfoContentProvider _contentProvider;
        #endregion

        #region Constructors

        [ImportingConstructor]
        public QuickInfoProvider(IDeferredQuickInfoContentProvider contentProvider) => _contentProvider = contentProvider;
        #endregion

        #region Methods
        public async Task<QuickInfoItem?> GetItemAsync(Document document, int position, CancellationToken cancellationToken)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            if (tree == null)
                return null;

            var token = await tree.GetTouchingTokenAsync(position, cancellationToken, findInsideTrivia: true).ConfigureAwait(false);

            var state = await GetQuickInfoItemAsync(document, token, position, cancellationToken).ConfigureAwait(false);
            if (state != null)
                return state;

            if (ShouldCheckPreviousToken(token))
            {
                var previousToken = token.GetPreviousToken();

                if ((state = await GetQuickInfoItemAsync(document, previousToken, position,
                    cancellationToken).ConfigureAwait(false)) != null)
                    return state;
            }

            return null;
        }

        private static bool ShouldCheckPreviousToken(SyntaxToken token) => !token.Parent.IsKind(SyntaxKind.XmlCrefAttribute);

        private async Task<QuickInfoItem?> GetQuickInfoItemAsync(Document document, SyntaxToken token, int position,
            CancellationToken cancellationToken)
        {
            if (token != default && token.Span.IntersectsWith(position))
            {
                var deferredContent = await BuildContentAsync(document, token, cancellationToken).ConfigureAwait(false);
                if (deferredContent != null)
                    return new QuickInfoItem(token.Span, () => deferredContent.Create());
            }

            return null;
        }

        private async Task<IDeferredQuickInfoContent?> BuildContentAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            var linkedDocumentIds = document.GetLinkedDocumentIds();

            var modelAndSymbols = await BindTokenAsync(document, token, cancellationToken).ConfigureAwait(false);
            if ((modelAndSymbols.Item2 == null || modelAndSymbols.Item2.Count == 0) && !linkedDocumentIds.Any())
                return null;

            if (!linkedDocumentIds.Any())
                return await CreateContentAsync(document.Project.Solution.Workspace, token, modelAndSymbols.Item1,
                    modelAndSymbols.Item2!, supportedPlatforms: null, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Linked files/shared projects: imagine the following when FOO is false
            // #if FOO
            // int x = 3;
            // #endif 
            // var y = x$$;
            //
            // 'x' will bind as an error type, so we'll show incorrect information.
            // Instead, we need to find the head in which we get the best binding, 
            // which in this case is the one with no errors.

            var candidateProjects = new List<ProjectId> { document.Project.Id };
            var invalidProjects = new List<ProjectId>();

            var candidateResults = new List<Tuple<DocumentId, SemanticModel, IList<ISymbol>>>
            {
                Tuple.Create(document.Id, modelAndSymbols.Item1, modelAndSymbols.Item2!)
            };

            foreach (var link in linkedDocumentIds)
            {
                var linkedDocument = document.Project.Solution.GetDocument(link);
                var linkedToken = await FindTokenInLinkedDocument(token, linkedDocument!, cancellationToken).ConfigureAwait(false);

                if (linkedToken != default)
                {
                    // Not in an inactive region, so this file is a candidate.
                    candidateProjects.Add(link.ProjectId);
                    var linkedModelAndSymbols = await BindTokenAsync(linkedDocument!, linkedToken, cancellationToken)
                        .ConfigureAwait(false);
                    candidateResults.Add(Tuple.Create(link, linkedModelAndSymbols.Item1, linkedModelAndSymbols.Item2));
                }
            }

            // Take the first result with no errors.
            var bestBinding = candidateResults.FirstOrDefault(c => c.Item3.Count > 0 &&
            !ErrorVisitor.ContainsError(c.Item3.FirstOrDefault()));

            // Every file binds with errors. Take the first candidate, which is from the current file.
            if (bestBinding == null)
                bestBinding = candidateResults.First();

            if (bestBinding.Item3 == null || !bestBinding.Item3.Any())
                return null;

            // We calculate the set of supported projects
            candidateResults.Remove(bestBinding);
            foreach (var candidate in candidateResults)
                // Does the candidate have anything remotely equivalent?
                if (!candidate.Item3.Intersect(bestBinding.Item3, LinkedFilesSymbolEquivalenceComparer.Instance).Any())
                    invalidProjects.Add(candidate.Item1.ProjectId);

            var supportedPlatforms = new SupportedPlatformData(invalidProjects, candidateProjects,
                document.Project.Solution.Workspace);
            return await CreateContentAsync(document.Project.Solution.Workspace, token, bestBinding.Item2, bestBinding.Item3,
                supportedPlatforms, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<SyntaxToken> FindTokenInLinkedDocument(SyntaxToken token, Document linkedDocument,
            CancellationToken cancellationToken)
        {
            if (!linkedDocument.SupportsSyntaxTree)
                return default;

            var root = await linkedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Don't search trivia because we want to ignore inactive regions
            var linkedToken = root!.FindToken(token.SpanStart);

            // The new and old tokens should have the same span?
            if (token.Span == linkedToken.Span)
                return linkedToken;

            return default;
        }

        private async Task<IDeferredQuickInfoContent> CreateContentAsync(Workspace workspace, SyntaxToken token,
            SemanticModel semanticModel, IEnumerable<ISymbol> symbols, SupportedPlatformData? supportedPlatforms,
            CancellationToken cancellationToken)
        {
            var descriptionService = workspace.Services.GetLanguageServices(token.Language).GetRequiredService<ISymbolDisplayService>();

            var sections = await descriptionService.ToDescriptionGroupsAsync(workspace, semanticModel, token.SpanStart, symbols.AsImmutable(), cancellationToken).ConfigureAwait(false);

            var mainDescriptionBuilder = new List<TaggedText>();
            if (sections.ContainsKey(SymbolDescriptionGroups.MainDescription))
                mainDescriptionBuilder.AddRange(sections[SymbolDescriptionGroups.MainDescription]);

            var typeParameterMapBuilder = new List<TaggedText>();
            if (sections.ContainsKey(SymbolDescriptionGroups.TypeParameterMap))
            {
                var parts = sections[SymbolDescriptionGroups.TypeParameterMap];
                if (!parts.IsDefaultOrEmpty)
                {
                    typeParameterMapBuilder.AddLineBreak();
                    typeParameterMapBuilder.AddRange(parts);
                }
            }

            var anonymousTypesBuilder = new List<TaggedText>();
            if (sections.ContainsKey(SymbolDescriptionGroups.AnonymousTypes))
            {
                var parts = sections[SymbolDescriptionGroups.AnonymousTypes];
                if (!parts.IsDefaultOrEmpty)
                {
                    anonymousTypesBuilder.AddLineBreak();
                    anonymousTypesBuilder.AddRange(parts);
                }
            }

            var usageTextBuilder = new List<TaggedText>();
            if (sections.ContainsKey(SymbolDescriptionGroups.AwaitableUsageText))
            {
                var parts = sections[SymbolDescriptionGroups.AwaitableUsageText];
                if (!parts.IsDefaultOrEmpty)
                    usageTextBuilder.AddRange(parts);
            }

            if (supportedPlatforms != null)
                usageTextBuilder.AddRange(supportedPlatforms.ToDisplayParts().ToTaggedText());

            var exceptionsTextBuilder = new List<TaggedText>();
            if (sections.ContainsKey(SymbolDescriptionGroups.Exceptions))
            {
                var parts = sections[SymbolDescriptionGroups.Exceptions];
                if (!parts.IsDefaultOrEmpty)
                    exceptionsTextBuilder.AddRange(parts);
            }

            var formatter = workspace.Services.GetLanguageServices(semanticModel.Language).GetRequiredService<IDocumentationCommentFormattingService>();
            var syntaxFactsService = workspace.Services.GetLanguageServices(semanticModel.Language).GetRequiredService<ISyntaxFactsService>();
            var documentationContent = GetDocumentationContent(symbols, sections, semanticModel, token, formatter, syntaxFactsService, cancellationToken);
            var showWarningGlyph = supportedPlatforms != null && supportedPlatforms.HasValidAndInvalidProjects();
            var showSymbolGlyph = true;

            if (workspace.Services.GetLanguageServices(semanticModel.Language).GetRequiredService<ISyntaxFactsService>().IsAwaitKeyword(token) &&
                (symbols.First() as INamedTypeSymbol)?.SpecialType == SpecialType.System_Void)
            {
                documentationContent = _contentProvider.CreateDocumentationCommentDeferredContent(null);
                showSymbolGlyph = false;
            }

            return _contentProvider.CreateQuickInfoDisplayDeferredContent(symbol: symbols.First(), showWarningGlyph: showWarningGlyph,
                showSymbolGlyph: showSymbolGlyph, mainDescription: mainDescriptionBuilder, documentation: documentationContent,
                typeParameterMap: typeParameterMapBuilder, anonymousTypes: anonymousTypesBuilder, usageText: usageTextBuilder,
                exceptionText: exceptionsTextBuilder);
        }

        private IDeferredQuickInfoContent GetDocumentationContent(IEnumerable<ISymbol> symbols, IDictionary<SymbolDescriptionGroups,
            ImmutableArray<TaggedText>> sections, SemanticModel semanticModel, SyntaxToken token,
            IDocumentationCommentFormattingService formatter, ISyntaxFactsService syntaxFactsService,
            CancellationToken cancellationToken)
        {
            if (sections.ContainsKey(SymbolDescriptionGroups.Documentation))
            {
                var documentationBuilder = new List<TaggedText>();
                documentationBuilder.AddRange(sections[SymbolDescriptionGroups.Documentation]);
                return _contentProvider.CreateClassifiableDeferredContent(documentationBuilder);
            }
            if (symbols.Any())
            {
                var symbol = symbols.First().OriginalDefinition;

                // if generating quick info for an attribute, bind to the class instead of the constructor
                if (syntaxFactsService.IsAttributeName(token.Parent) && symbol.ContainingType?.IsAttribute() == true)
                    symbol = symbol.ContainingType;

                var documentation = symbol.GetDocumentationParts(semanticModel, token.SpanStart, formatter, cancellationToken);

                if (documentation != null)
                    return _contentProvider.CreateClassifiableDeferredContent(documentation.ToList());
            }

            return _contentProvider.CreateDocumentationCommentDeferredContent(null);
        }

        private static async Task<ValueTuple<SemanticModel, IList<ISymbol>>> BindTokenAsync(Document document, SyntaxToken token,
            CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelForNodeAsync(token.Parent, cancellationToken).ConfigureAwait(false);
            var enclosingType = semanticModel.GetEnclosingNamedType(token.SpanStart, cancellationToken);

            var symbols = semanticModel.GetSemanticInfo(token, document.Project.Solution.Workspace, cancellationToken)
                .GetSymbols(includeType: true);

            var bindableParent = document.GetLanguageService<ISyntaxFactsService>().TryGetBindableParent(token);
            if (bindableParent != null)
            {
                var overloads = semanticModel.GetMemberGroup(bindableParent, cancellationToken);

                symbols = symbols.Where(IsOk)
                    .Where(s => IsAccessible(s, enclosingType!))
                    .Concat(overloads)
                    .Distinct(SymbolEquivalenceComparer.Instance)
                    .ToImmutableArray();

                if (symbols.Any())
                {
                    return new ValueTuple<SemanticModel, IList<ISymbol>>(
                        semanticModel, symbols.First() is ITypeParameterSymbol typeParameter && typeParameter.TypeParameterKind == TypeParameterKind.Cref
                            ? SpecializedCollections.EmptyList<ISymbol>()
                            : symbols.ToList());
                }

                // Couldn't bind the token to specific symbols.  If it's an operator, see if we can at
                // least bind it to a type.
                var syntaxFacts = document.Project.LanguageServices.GetRequiredService<ISyntaxFactsService>();
                if (syntaxFacts.IsOperator(token) && token.Parent != null)
                {
                    var typeInfo = semanticModel.GetTypeInfo(token.Parent, cancellationToken);
                    if (IsOk(typeInfo.Type!))
                        return new ValueTuple<SemanticModel, IList<ISymbol>>(semanticModel, new List<ISymbol>(1) { typeInfo.Type! });
                }
            }

            return ValueTuple.Create(semanticModel, SpecializedCollections.EmptyList<ISymbol>());
        }

        private static bool IsOk(ISymbol symbol) => symbol != null && !symbol.IsErrorType() && !symbol.IsAnonymousFunction();

        private static bool IsAccessible(ISymbol symbol, INamedTypeSymbol within) =>
            within == null || symbol.IsAccessibleWithin(within);
        #endregion
    }
}
