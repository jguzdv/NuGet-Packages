using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace StringLocalizerGenerator
{
    /// <summary>
    /// Source Generator, der für mit <c>[Localized]</c> markierte Klassen
    /// eine Extension-Klasse mit stark typisierten Properties für <c>IStringLocalizer&lt;T&gt;</c> generiert.
    /// </summary>
    [Generator]
    public class ResxLocalizerGenerator : IIncrementalGenerator
    {
        private static readonly DiagnosticDescriptor MissingResxWarning = new(
            "RSX002",
            "Missing resource file",
            "Class '{0}' is marked with [Localized] but no '{0}.resx' file was found",
            "Localization",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor GeneratorError = new(
            "RSX001",
            "Generator Error",
            "Error processing {0}: {1}",
            "Localization",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <summary>
        /// Registriert die inkrementellen Pipeline-Schritte des Generators.
        /// </summary>
        /// <param name="context">Der Kontext des inkrementellen Generators.</param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Alle RESX-Dateien sammeln
            var resxFiles = context.AdditionalTextsProvider
                .Where(f => f.Path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
                .Collect();

            // Markierte Klassen mit [Localized] finden
            var localizedClasses = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsLocalizedClass(s),
                    transform: static (ctx, _) => GetClassInfo(ctx))
                .Where(c => c is not null)
                .Collect();

            // Kombinieren und verarbeiten
            var combined = localizedClasses.Combine(resxFiles);

            context.RegisterSourceOutput(combined, (spc, pair) =>
            {
                var classes = pair.Left;
                var allResxFiles = pair.Right;

                foreach (var classInfo in classes)
                {
                    if (classInfo is not null)
                    {
                        ProcessLocalizedClass(spc, classInfo, allResxFiles);
                    }
                }
            });
        }

        private static bool IsLocalizedClass(SyntaxNode node)
        {
            // Nur Klassendeklarationen betrachten
            if (node is not ClassDeclarationSyntax classDeclaration)
                return false;

            // Prüfen ob Attribute vorhanden sind
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeName = attribute.Name.ToString();
                    if (attributeName == "Localized" || attributeName == "LocalizedAttribute")
                        return true;
                }
            }

            return false;
        }

        private static ClassInfo? GetClassInfo(GeneratorSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

            if (symbol is null)
                return null;

            return new ClassInfo
            {
                ClassName = symbol.Name,
                Namespace = symbol.ContainingNamespace.ToDisplayString(),
                FilePath = classDeclaration.SyntaxTree.FilePath,
                Location = classDeclaration.GetLocation()
            };
        }

        private void ProcessLocalizedClass(
            SourceProductionContext spc,
            ClassInfo classInfo,
            IEnumerable<AdditionalText> allResxFiles)
        {
            var matchingResx = FindMatchingResxFile(classInfo, allResxFiles);

            if (matchingResx is null)
            {
                ReportMissingResxWarning(spc, classInfo);
                return;
            }

            GenerateExtensions(spc, classInfo, matchingResx);
        }

        private AdditionalText? FindMatchingResxFile(
            ClassInfo classInfo,
            IEnumerable<AdditionalText> allResxFiles)
        {
            return allResxFiles.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f.Path)
                    .Equals(classInfo.ClassName, StringComparison.OrdinalIgnoreCase));
        }

        private void ReportMissingResxWarning(
            SourceProductionContext spc,
            ClassInfo classInfo)
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                MissingResxWarning,
                classInfo.Location,
                classInfo.ClassName));
        }

        private List<string> ParseResxKeys(AdditionalText resxFile)
        {
            var content = resxFile.GetText()?.ToString();
            if (string.IsNullOrEmpty(content))
                return new List<string>();

            var doc = XDocument.Parse(content);
            return doc.Descendants("data")
                .Select(d => d.Attribute("name")?.Value)
                .OfType<string>()
                .Where(name => name.Length > 0)
                .ToList();
        }

        private void GenerateExtensions(
            SourceProductionContext spc,
            ClassInfo classInfo,
            AdditionalText resxFile)
        {
            try
            {
                var keys = ParseResxKeys(resxFile);

                if (!keys.Any())
                {
                    GenerateEmptyExtensions(spc, classInfo);
                    return;
                }

                // Properties generieren (mit Validierung)
                var validKeys = new List<string>();
                foreach (var key in keys)
                {
                    if (SyntaxFacts.IsValidIdentifier(key))
                    {
                        validKeys.Add(key);
                    }
                    else
                    {
                        ReportInvalidKeyWarning(spc, classInfo, key);
                    }
                }

                if (!validKeys.Any())
                {
                    GenerateEmptyExtensions(spc, classInfo);
                    return;
                }

                var properties = string.Join("\n", validKeys.Select(key =>
                    $"\t\t\tpublic string {key} => localizer[\"{key}\"].ToString();"
                ));

                var sourceCode = $$"""
                // <auto-generated />
                #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                #nullable enable
                using Microsoft.Extensions.Localization;

                namespace {{classInfo.Namespace}}
                {
                    public static class {{classInfo.ClassName}}LocalizerExtensions
                    {
                        extension(IStringLocalizer<{{classInfo.ClassName}}> localizer)
                        {
                            {{properties}}
                        }
                    }
                }
                """;

                sourceCode = NormalizeLineEndings(sourceCode);

                spc.AddSource(
                    $"{classInfo.ClassName}LocalizerExtensions.g.cs",
                    SourceText.From(sourceCode, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    GeneratorError,
                    classInfo.Location,
                    classInfo.ClassName,
                    ex.Message));
            }
        }

        private void GenerateEmptyExtensions(
            SourceProductionContext spc,
            ClassInfo classInfo)
        {
            var sourceCode = $$"""
            // <auto-generated />
            #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            #nullable enable
            using Microsoft.Extensions.Localization;

            namespace {{classInfo.Namespace}}
            {
                public static class {{classInfo.ClassName}}LocalizerExtensions
                {
                    extension(IStringLocalizer<{{classInfo.ClassName}}> localizer)
                    {
                        // No resources found
                    }
                }
            }
            """;
            sourceCode = NormalizeLineEndings(sourceCode);
            spc.AddSource(
                $"{classInfo.ClassName}LocalizerExtensions.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
        }

        private static readonly DiagnosticDescriptor InvalidKeyWarning = new(
            "RSX003",
            "Invalid resource key",
            "Resource key '{0}' in class '{1}' is not a valid C# identifier and will be skipped",
            "Localization",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private void ReportInvalidKeyWarning(
            SourceProductionContext spc,
            ClassInfo classInfo,
            string key)
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                InvalidKeyWarning,
                classInfo.Location,
                key,
                classInfo.ClassName));
        }
        private static string NormalizeLineEndings(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }

    internal class ClassInfo
    {
        public string ClassName { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public Location? Location { get; set; }
    }
}