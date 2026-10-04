using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace JGUZDV.StringLocalizerExtensions.Tests;

/// <summary>
/// Hilfsklasse, um den Source Generator in Tests auszuführen.
/// </summary>
internal static class TestHelper
{
    /// <summary>
    /// Quelltext-Stub für das [Localized]-Attribut. Wird in Tests an den
    /// eigentlichen Testquelltext angehängt, damit die Semantikprüfung greift.
    /// </summary>
    public const string LocalizedAttributeStub = """
        namespace JGUZDV.StringLocalizerExtensions
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public sealed class LocalizedAttribute : System.Attribute { }
        }
        """;

    /// <summary>
    /// Quelltext-Stub für Microsoft.Extensions.Localization, damit der generierte
    /// Code kompiliert werden kann.
    /// </summary>
    /// <summary>
    /// Quelltext-Stub für Microsoft.Extensions.Localization.
    /// </summary>
    public const string LocalizationStub = """
        namespace Microsoft.Extensions.Localization
        {
            public interface IStringLocalizer<T>
            {
                LocalizedString this[string name] { get; }
            }

            public readonly struct LocalizedString
            {
                private readonly string _value;
                public LocalizedString(string value) { _value = value; }
                public override string ToString() => _value;
            }
        }
        """;

    /// <summary>
    /// Führt den Generator aus und liefert das GeneratorDriverRunResult zurück.
    /// </summary>
    public static GeneratorDriverRunResult RunGenerator(
        string source,
        params (string path, string content)[] additionalFiles)
    {
        var compilation = CreateCompilation(source);

        var driver = CSharpGeneratorDriver.Create(
            generators: new[] { new StringLocalizerGenerator.ResxLocalizerGenerator().AsSourceGenerator() },
            additionalTexts: additionalFiles
                .Select(f => (AdditionalText)new InMemoryAdditionalText(f.path, f.content))
                .ToArray(),
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview));

        return driver.RunGenerators(compilation).GetRunResult();
    }

    /// <summary>
    /// Führt den Generator aus, kompiliert das Ergebnis und liefert beides zurück.
    /// Nützlich, um zu prüfen, ob der generierte Code fehlerfrei kompiliert.
    /// </summary>
    public static (GeneratorDriverRunResult runResult, Compilation outputCompilation) RunGeneratorAndCompile(
        string source,
        params (string path, string content)[] additionalFiles)
    {
        var compilation = CreateCompilation(source);

        var driver = CSharpGeneratorDriver.Create(
            generators: new[] { new StringLocalizerGenerator.ResxLocalizerGenerator().AsSourceGenerator() },
            additionalTexts: additionalFiles
                .Select(f => (AdditionalText)new InMemoryAdditionalText(f.path, f.content))
                .ToArray(),
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview));

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var outputCompilation, out _);

        return (driver.GetRunResult(), outputCompilation);
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            SourceText.From(source),
            new CSharpParseOptions(LanguageVersion.Preview));

        return CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: GetDefaultReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static IEnumerable<MetadataReference> GetDefaultReferences()
    {
        // Alle Assemblies, die im aktuellen Testprozess geladen sind, als Referenz verwenden.
        // Reicht für einfache Tests aus, in denen der generierte Code nur
        // Standardtypen und unsere Stubs verwendet.
        var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? "";
        return trusted
            .Split(Path.PathSeparator)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p));
    }
}

/// <summary>
/// Ein AdditionalText, der den Inhalt im Speicher hält – für Tests, in denen
/// keine echte Resx-Datei existiert.
/// </summary>
internal sealed class InMemoryAdditionalText : AdditionalText
{
    private readonly SourceText _text;

    public InMemoryAdditionalText(string path, string content)
    {
        Path = path;
        _text = SourceText.From(content);
    }

    public override string Path { get; }

    public override SourceText? GetText(CancellationToken cancellationToken = default) => _text;
}