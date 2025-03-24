using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JGUZDV.CQRS.Analyzer
{
    /// <summary>
    /// Anakyzer class that defines all neccessary parameters of a diagnostic and analyzes files.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CQRSAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Diagnostic Id given to diagnostics reported by this analyzer.
        /// </summary>
        public const string DiagnosticId = "CQRS01";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.Title),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Desciption = new LocalizableResourceString(
            nameof(Resources.Description),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Messsage = new LocalizableResourceString(
            nameof(Resources.MessageFromat),
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "CQRSAnalysisDesign";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            Messsage,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Desciption);

        /// <summary>
        /// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        /// <summary>
        /// Called once at session start to register actions in the analysis context.
        /// </summary>
        /// <param name="context">Analysis context</param>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeQuerys, SyntaxKind.RecordDeclaration);
        }

        /// <summary>
        /// Checks if the base class of the class declaration node is IQuery and if it has a nullable or non-nullable Property of type QueryResult.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        private void AnalyzeQuerys(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = context.Node as RecordDeclarationSyntax;

            if (classDeclaration.BaseList == null)
                return;

            if (classDeclaration.BaseList.Types.First() is SimpleBaseTypeSyntax baseClass
                && baseClass.Type is GenericNameSyntax name
                && name.Identifier.Text.Equals("IQuery"))
            {
                if (classDeclaration.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Any(pds =>
                        (pds.Type is NullableTypeSyntax npType
                            && npType.ElementType is GenericNameSyntax npName
                            && npName.Identifier.Text.Equals("QueryResult"))
                        || (pds.Type is GenericNameSyntax pName
                            && pName.Identifier.Text.Equals("QueryResult"))))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, baseClass.GetLocation()));
                }
            }
        }
    }
}
