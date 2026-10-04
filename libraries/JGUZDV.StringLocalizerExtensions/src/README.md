# JGUZDV.StringLocalizerExtensions

Ein Roslyn Source Generator, der für mit `[Localized]` markierte Klassen
stark typisierte Erweiterungen für `IStringLocalizer<T>` erzeugt.

## Was macht das Paket?

Statt:

    _localizer["Title"]

schreibst du:

    _localizer.Title

Der Generator liest die zur Klasse gehörende `.resx`-Datei und erzeugt
für jeden Schlüssel eine Property. Damit erhältst du IntelliSense,
Compile-Zeit-Prüfung und Rename-Refactoring für deine Lokalisierungs-Schlüssel.

## Verwendung

1. Klasse mit `[Localized]` markieren:

    ```csharp
    [Localized]
    public class HomePage { }
    ```

2. Passende `.resx`-Datei im gleichen Projekt anlegen
   (hier: `HomePage.resx`). Der Dateiname ohne Endung muss dem
   Klassennamen entsprechen.

3. Die `.resx`-Dateien als `AdditionalFiles` einbinden, damit der Generator
   sie sieht:

    ```xml
    <ItemGroup>
        <AdditionalFiles Include="**/*.resx" />
    </ItemGroup>
    ```

4. C#-Sprachversion auf `preview` (oder `14.0`) setzen:

    ```xml
    <LangVersion>preview</LangVersion>
    ```

Nach dem Build stehen die Properties auf jedem `IStringLocalizer<HomePage>`
zur Verfügung:

    _localizer.Title

## Diagnosen

| ID     | Schwere | Beschreibung                                              |
|--------|---------|-----------------------------------------------------------|
| RSX001 | Error   | Interner Fehler im Generator.                             |
| RSX002 | Warning | `[Localized]`-Klasse ohne passende `.resx`-Datei.         |
| RSX003 | Warning | Resx-Schlüssel ist kein gültiger C#-Identifier.           |
| RSX005 | Warning | Zwei Schlüssel kollidieren nach der Bereinigung.          |