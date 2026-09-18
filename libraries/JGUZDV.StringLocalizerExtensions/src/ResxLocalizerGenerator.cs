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
    /// eine Extension-Klasse mit stark typisierten Properties für
    /// <c>IStringLocalizer&lt;T&gt;</c> generiert.
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

        private static readonly DiagnosticDescriptor InvalidKeyWarning = new(
            "RSX003",
            "Invalid resource key",
            "Resource key '{0}' in class '{1}' is not a valid C# identifier and will be skipped",
            "Localization",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        private static readonly DiagnosticDescriptor KeyCollisionWarning = new(
            "RSX005",
            "Duplicate resource key after sanitizing",
            "Resource key '{0}' in class '{1}' collides with another key after sanitizing (both map to '{2}') and will be skipped",
            "Localization",
            DiagnosticSeverity.Warning,
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

            context.RegisterSourceOutput(combined, static (spc, pair) =>
            {
                var classes = pair.Left;
                var allResxFiles = pair.Right;

                foreach (var classInfo in classes)
                {
                    spc.CancellationToken.ThrowIfCancellationRequested();
                    if (classInfo is not null)
                    {
                        ProcessLocalizedClass(spc, classInfo, allResxFiles);
                    }
                }
            });
        }

        private static bool IsLocalizedClass(SyntaxNode node)
        {
            if (node is not ClassDeclarationSyntax classDeclaration)
                return false;

            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var name = attribute.Name.ToString();
                    if (name == "Localized"
                        || name == "LocalizedAttribute"
                        || name.EndsWith(".Localized", StringComparison.Ordinal)
                        || name.EndsWith(".LocalizedAttribute", StringComparison.Ordinal))
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

            // Semantische Verifikation: Hat die Klasse wirklich [Localized]?
            var hasLocalized = symbol.GetAttributes().Any(a =>
                a.AttributeClass?.Name is "LocalizedAttribute" or "Localized");

            if (!hasLocalized)
                return null;

            return new ClassInfo
            {
                ClassName = symbol.Name,
                Namespace = symbol.ContainingNamespace.ToDisplayString(),
                Location = classDeclaration.GetLocation()
            };
        }

        private static void ProcessLocalizedClass(
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

        private static AdditionalText? FindMatchingResxFile(
            ClassInfo classInfo,
            IEnumerable<AdditionalText> allResxFiles)
        {
            return allResxFiles.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f.Path)
                    .Equals(classInfo.ClassName, StringComparison.OrdinalIgnoreCase));
        }

        private static void ReportMissingResxWarning(
            SourceProductionContext spc,
            ClassInfo classInfo)
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                MissingResxWarning,
                classInfo.Location,
                classInfo.ClassName));
        }
        private static void ReportKeyCollisionWarning(
            SourceProductionContext spc,
            ClassInfo classInfo,
            string key,
            string identifier)
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                KeyCollisionWarning,
                classInfo.Location,
                key,
                classInfo.ClassName,
                identifier));
        }

        private static List<string> ParseResxKeys(AdditionalText resxFile)
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

        private static void GenerateExtensions(
            SourceProductionContext spc,
            ClassInfo classInfo,
            AdditionalText resxFile)
        {
            try
            {
                var keys = ParseResxKeys(resxFile);

                if (keys.Count == 0)
                {
                    // Keine Resourcen => keine Extension generieren.
                    // Statt einer leeren Datei eine Diagnose ausgeben, damit der Nutzer es merkt.
                    ReportMissingResxWarning(spc, classInfo);
                    return;
                }

                var properties = new List<string>();
                var usedNames = new HashSet<string>(StringComparer.Ordinal);

                foreach (var key in keys)
                {
                    spc.CancellationToken.ThrowIfCancellationRequested();

                    var identifier = SanitizeKey(key);

                    if (identifier is null)
                    {
                        ReportInvalidKeyWarning(spc, classInfo, key);
                        continue;
                    }

                    if (!usedNames.Add(identifier))
                    {
                        ReportKeyCollisionWarning(spc, classInfo, key, identifier);
                        continue;
                    }

                    properties.Add(
                        $"\t\t\t/// <summary>Gets the localized string for '{key}'.</summary>\n" +
                        $"\t\t\tpublic string {identifier} => localizer[\"{key}\"].ToString();");
                }

                if (properties.Count == 0)
                {
                    // Alle Keys waren ungültig oder doppelt.
                    return;
                }

                var propertiesCode = string.Join("\n", properties);

                var sourceCode = $$"""
                // <auto-generated />
                #pragma warning disable CS1591
                #nullable enable
                using Microsoft.Extensions.Localization;

                namespace {{classInfo.Namespace}}
                {
                    public static class {{classInfo.ClassName}}LocalizerExtensions
                    {
                        extension(IStringLocalizer<{{classInfo.ClassName}}> localizer)
                        {
                {{propertiesCode}}
                        }
                    }
                }
                """;

                sourceCode = NormalizeLineEndings(sourceCode);

                spc.AddSource(
                    $"{classInfo.ClassName}LocalizerExtensions.g.cs",
                    SourceText.From(sourceCode, Encoding.UTF8));
            }
            catch (OperationCanceledException)
            {
                throw;
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

        /// <summary>
        /// Wandelt einen Resx-Key in einen gültigen C#-Identifier um.
        /// Erlaubte Zeichen: Buchstaben, Ziffern, Unterstrich.
        /// Alle anderen Zeichen werden durch '_' ersetzt.
        /// Führende Ziffern werden mit '_' präfixiert.
        /// </summary>
        private static string? SanitizeKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var builder = new StringBuilder(key.Length);

            foreach (var c in key)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    builder.Append(c);
                else
                    builder.Append('_');
            }

            // Führende Ziffer? Präfix mit '_'.
            if (builder.Length > 0 && char.IsDigit(builder[0]))
                builder.Insert(0, '_');

            var result = builder.ToString();

            // Wenn nach dem Sanitizing immer noch kein gültiger Identifier rauskommt
            // (z. B. weil das Ergebnis ein Keyword ist), mit '@' escapen.
            if (!SyntaxFacts.IsValidIdentifier(result))
            {
                var escaped = "@" + result;
                if (SyntaxFacts.IsValidIdentifier(escaped))
                    return escaped;

                return null;
            }

            return result;
        }

        private static void ReportInvalidKeyWarning(
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
        public string ClassName { get; init; } = string.Empty;
        public string Namespace { get; init; } = string.Empty;
        public Location? Location { get; init; }
    }
}