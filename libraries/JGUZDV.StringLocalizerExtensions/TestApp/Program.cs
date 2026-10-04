using Microsoft.Extensions.DependencyInjection; // Stellt BuildServiceProvider() bereit
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;             // Stellt AddLogging() bereit

namespace TestApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("TestApp wird gestartet...\n");

        var services = new ServiceCollection();

        // Benötigt Microsoft.Extensions.Logging
        services.AddLogging();

        // Benötigt Microsoft.Extensions.Localization
        services.AddLocalization();

        // Benötigt Microsoft.Extensions.DependencyInjection
        var provider = services.BuildServiceProvider();

        // Instanz von IStringLocalizer<HomePage> auflösen
        var localizer = provider.GetRequiredService<IStringLocalizer<HomePage>>();

        // Extension-Properties abrufen
        Console.WriteLine($"Title:    {localizer.Title}");
        Console.WriteLine($"Greeting: {localizer.Greeting}");
        Console.WriteLine($"Greeting: {localizer.Greeting_Variant1}");
        Console.WriteLine($"Greeting: {localizer.Hallo_Tschüß}");
        Console.WriteLine("\nDrücke eine Taste zum Beenden.");
        Console.ReadKey();
    }
}