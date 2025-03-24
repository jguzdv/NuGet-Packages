using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JGUZDV.CQRS.Analyzer.CodeFix
{
    /// <summary>
    /// 
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CQRSAnalyzerCodeFixProvider)), Shared]
    public class CQRSAnalyzerCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// A list of diagnostic IDs that this provider can provide fixes for.
        /// </summary>
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CQRSAnalyzer.DiagnosticId);

        /// <summary>
        /// Gets an optional Microsoft.CodeAnalysis.CodeFixes.FixAllProvider that can fix
        /// all/multiple occurrences of diagnostics fixed by this code fix provider.
        /// </summary>
        /// <returns>BatchFixer from well known fix all providers in Microsoft.CodeAnalysis.CodeFixes.WellKnownFixAllProviders.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        /// Computes one or more fixes for the specified Microsoft.CodeAnalysis.CodeFixes.CodeFixContext.
        /// </summary>
        /// <param name="context">A Microsoft.CodeAnalysis.CodeFixes.CodeFixContext containing context information
        /// about the diagnostics to fix.</param>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var recDeclaration = root.FindToken(context.Span.Start).Parent;
            var node = recDeclaration.Parent.Parent.Parent as RecordDeclarationSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.Title,
                    createChangedDocument: c => UpdateQueryDefinition(context.Document, node, c),
                    equivalenceKey: nameof(CodeFixResources.Title)),
                diagnostic);
        }

        /// <summary>
        /// Updates the query record to use the correct base class and removes the QueryResult declaration.
        /// </summary>
        /// <param name="document">The document in which the code fix is being run.</param>
        /// <param name="node">The syntax node of the record to be changed.</param>
        /// <param name="c">Cancellation token.</param>
        /// <returns></returns>
        private async Task<Document> UpdateQueryDefinition(Document document, RecordDeclarationSyntax node, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync();

            var typeArguments = (node.BaseList.Types.OfType<SimpleBaseTypeSyntax>().First().Type as GenericNameSyntax).TypeArgumentList;

            var newQuery = node.WithBaseList(
                SyntaxFactory.BaseList().AddTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("QueryBase"),
                            typeArguments))));

            var queryResultProperty = newQuery.Members
                .OfType<PropertyDeclarationSyntax>()
                .First(pds =>
                    (pds.Type is NullableTypeSyntax npType
                        && npType.ElementType is GenericNameSyntax npName
                        && npName.Identifier.Text.Equals("QueryResult"))
                    || (pds.Type is GenericNameSyntax pName
                        && pName.Identifier.Text.Equals("QueryResult")));

            newQuery = newQuery.RemoveNode(queryResultProperty, SyntaxRemoveOptions.KeepNoTrivia);

            return document.WithSyntaxRoot(root.ReplaceNode(node, newQuery));
        }
    }
}
