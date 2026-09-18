using System.Linq;

using Microsoft.CodeAnalysis;

using Xunit;

namespace JGUZDV.StringLocalizerExtensions.Tests;

public class ResxLocalizerGeneratorTests
{
    [Fact]
    public void Generates_Extension_For_Localized_Class_With_Matching_Resx()
    {
        // Arrange: Quelltext mit einer [Localized]-Klasse
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        // Arrange: eine erfundene HomePage.resx mit zwei Keys
        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="Title" xml:space="preserve"><value>Willkommen</value></data>
              <data name="Subtitle" xml:space="preserve"><value>Untertitel</value></data>
            </root>
            """;

        // Act: Generator laufen lassen
        var runResult = TestHelper.RunGenerator(source, ("HomePage.resx", resx));

        // Assert: genau eine Datei generiert, mit dem erwarteten Inhalt
        var generated = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .ToList();

        Assert.Single(generated);
        Assert.Equal("HomePageLocalizerExtensions.g.cs", generated[0].HintName);

        var code = generated[0].SourceText.ToString();
        Assert.Contains("namespace MyApp", code);
        Assert.Contains("public static class HomePageLocalizerExtensions", code);
        Assert.Contains("public string Title", code);
        Assert.Contains("public string Subtitle", code);
    }
    [Fact]
    public void Reports_Warning_When_Resx_Missing()
    {
        // Arrange: [Localized]-Klasse, aber KEINE Resx-Datei
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        // Act: Generator OHNE AdditionalFiles ausführen
        var runResult = TestHelper.RunGenerator(source);

        // Assert: genau eine Diagnose mit ID RSX002
        var diagnostics = runResult.Diagnostics;
        Assert.Contains(diagnostics, d => d.Id == "RSX002");

        // Assert: keine Datei generiert
        var generated = runResult.Results.SelectMany(r => r.GeneratedSources).ToList();
        Assert.Empty(generated);
    }
    [Fact]
    public void Sanitizes_Invalid_Characters_To_Underscore()
    {
        // Arrange
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="My-Key" xml:space="preserve"><value>x</value></data>
              <data name="Hello World" xml:space="preserve"><value>x</value></data>
            </root>
            """;

        // Act
        var runResult = TestHelper.RunGenerator(source, ("HomePage.resx", resx));

        // Assert
        var generated = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .Single();

        var code = generated.SourceText.ToString();

        // Identifier wurde bereinigt
        Assert.Contains("public string My_Key", code);
        Assert.Contains("public string Hello_World", code);

        // Aber der Zugriff auf den Localizer erfolgt weiterhin mit dem Original-Key
        Assert.Contains("localizer[\"My-Key\"]", code);
        Assert.Contains("localizer[\"Hello World\"]", code);
    }
    [Fact]
    public void Generated_Code_Compiles_Without_Errors()
    {
        // Arrange: Lokalisierung-Stub + [Localized]-Klasse + Resx
        var source = TestHelper.LocalizedAttributeStub
                     + TestHelper.LocalizationStub
                     + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="Title" xml:space="preserve"><value>Willkommen</value></data>
              <data name="My-Key" xml:space="preserve"><value>x</value></data>
            </root>
            """;

        // Act: Generator ausführen UND Ergebnis kompilieren
        var (_, outputCompilation) = TestHelper.RunGeneratorAndCompile(
            source, ("HomePage.resx", resx));

        // Assert: keine Fehler in der kompilierten Ausgabe
        var errors = outputCompilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        Assert.Empty(errors);
    }
    [Fact]
    public void Generates_One_File_Per_Localized_Class()
    {
        // Arrange: Zwei [Localized]-Klassen in verschiedenen Namespaces
        var source = TestHelper.LocalizedAttributeStub + """

            namespace App.Pages
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }

            namespace App.Widgets
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class Sidebar { }
            }
            """;

        var homeResx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="Title" xml:space="preserve"><value>x</value></data>
            </root>
            """;

        var sidebarResx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="Caption" xml:space="preserve"><value>x</value></data>
            </root>
            """;

        // Act
        var runResult = TestHelper.RunGenerator(
            source,
            ("HomePage.resx", homeResx),
            ("Sidebar.resx", sidebarResx));

        // Assert: zwei Dateien generiert
        var generated = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .ToList();

        Assert.Equal(2, generated.Count);

        var names = generated.Select(g => g.HintName).OrderBy(n => n).ToList();
        Assert.Contains("HomePageLocalizerExtensions.g.cs", names);
        Assert.Contains("SidebarLocalizerExtensions.g.cs", names);

        // Beide Dateien enthalten ihre eigene Namespace-Angabe
        var homeCode = generated.Single(g => g.HintName.StartsWith("HomePage")).SourceText.ToString();
        var sidebarCode = generated.Single(g => g.HintName.StartsWith("Sidebar")).SourceText.ToString();
        Assert.Contains("namespace App.Pages", homeCode);
        Assert.Contains("namespace App.Widgets", sidebarCode);
    }
    [Fact]
    public void Reports_Warning_When_Resx_Name_Does_Not_Match_Class()
    {
        // Arrange: Klasse HomePage, aber Resx heißt OtherPage.resx
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="Title" xml:space="preserve"><value>x</value></data>
            </root>
            """;

        // Act: Resx-Dateiname passt NICHT zur Klasse
        var runResult = TestHelper.RunGenerator(source, ("OtherPage.resx", resx));

        // Assert: RSX002 gemeldet, keine Datei generiert
        Assert.Contains(runResult.Diagnostics, d => d.Id == "RSX002");
        Assert.Empty(runResult.Results.SelectMany(r => r.GeneratedSources));
    }
    [Fact]
    public void Prefixes_Leading_Digit_With_Underscore()
    {
        // Arrange
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="123" xml:space="preserve"><value>x</value></data>
              <data name="1Title" xml:space="preserve"><value>x</value></data>
            </root>
            """;

        // Act
        var runResult = TestHelper.RunGenerator(source, ("HomePage.resx", resx));

        // Assert
        var code = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .Single()
            .SourceText.ToString();

        Assert.Contains("public string _123", code);
        Assert.Contains("public string _1Title", code);
        Assert.Contains("localizer[\"123\"]", code);
        Assert.Contains("localizer[\"1Title\"]", code);
    }
    [Fact]
    public void Deduplicates_Keys_After_Sanitizing()
    {
        // Arrange: Zwei Keys, die nach Sanitizing kollidieren
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
              <data name="My-Key" xml:space="preserve"><value>a</value></data>
              <data name="My_Key" xml:space="preserve"><value>b</value></data>
            </root>
            """;

        // Act
        var runResult = TestHelper.RunGenerator(source, ("HomePage.resx", resx));

        // Assert: Identifier "My_Key" taucht nur EINMAL auf
        var code = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .Single()
            .SourceText.ToString();

        var count = System.Text.RegularExpressions.Regex.Matches(
            code, @"public string My_Key\b").Count;

        Assert.Equal(1, count);
        Assert.Contains(runResult.Diagnostics, d => d.Id == "RSX005");
    }
    [Fact]
    public void Reports_Warning_When_Resx_Has_No_Data_Entries()
    {
        // Arrange: Resx existiert, enthält aber keine <data>-Einträge
        var source = TestHelper.LocalizedAttributeStub + """

            namespace MyApp
            {
                [JGUZDV.StringLocalizerExtensions.Localized]
                public class HomePage { }
            }
            """;

        var resx = """
            <?xml version="1.0" encoding="utf-8"?>
            <root>
            </root>
            """;

        // Act
        var runResult = TestHelper.RunGenerator(source, ("HomePage.resx", resx));

        // Assert: Warnung wird gemeldet, keine Datei generiert.
        // Aktuell wird RSX002 gemeldet (semantisch nicht ideal – die Resx existiert ja).
        // Der Test dokumentiert das aktuelle Verhalten.
        Assert.Contains(runResult.Diagnostics,
            d => d.Id == "RSX002" && d.Severity == DiagnosticSeverity.Warning);

        Assert.Empty(runResult.Results.SelectMany(r => r.GeneratedSources));
    }
}