using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.Json.Serialization;

namespace JGUZDV.L10n;

[JsonConverter(typeof(L10nStringJsonConverter))]
public class L10nString
{
    /// <summary>
    /// The default culture name.
    /// Will be derived from the EntryAssemblys (or ExecutingAssemblys, if null) <see cref="System.Resources.NeutralResourceLanguageAttribute"/> CultureName
    /// If not set, it'll use "de" as default.
    /// </summary>
    public static string DefaultCultureName { get; set; } =
        (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
            .GetCustomAttribute<NeutralResourcesLanguageAttribute>()?.CultureName 
        ?? "de";

    /// <summary>
    /// If set, the list will be used as preference list during <see cref="ToString"/>.
    /// </summary>
    public static List<string>? CulturePreferences { get; set; }


    protected readonly Dictionary<string, string> _values;
    public IReadOnlyDictionary<string, string> Values => _values;

    /// <summary>
    /// Initializes an empty instance.
    /// </summary>
    public L10nString()
    {
        _values = new (StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Initializes a copy of the original
    /// </summary>
    public L10nString(L10nString original)
        : this(original.Values)
    { }

    /// <summary>
    /// Initializes the instance from a dictionary.
    /// </summary>
    public L10nString(IEnumerable<KeyValuePair<string, string>> values)
        :this()
    {
        foreach (var (k, v) in values)
            SetValue(k, v);
    }

    /// <summary>
    /// Wraps <see cref="GetValue(string)"/> and <see cref="SetValue(string, string?)"/> into an indexer.
    /// </summary>
    /// <returns>The string matching the language or null, if it does not exist.</returns>
    public string? this[string cultureName]
    {
        get => GetValue(cultureName);
        set => SetValue(cultureName, value);
    }

    /// <summary>
    /// Get's the value indicated by the cultureName parameter, or null, if not present.
    /// </summary>
    public string? GetValue(string cultureName)
        => _values.GetValueOrDefault(cultureName);

    /// <summary>
    /// Sets the content to value, if not null or whitespace.
    /// Removes the key, if null or whitespace
    /// </summary>
    public void SetValue(string cultureName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _values.Remove(cultureName);
        }
        else
        {
            _values[cultureName] = value;
        }
    }

    /// <summary>
    /// Tries to match the requested culture and returns the content of this object.
    /// If the requested culture is not contained in this instance, the following sequence will be used:
    /// - Match language2 without region, if region was given
    /// - Match <see cref="DefaultCultureName"/>, if it was not already requested.
    /// - Match by culture preference from <see cref="CulturePreferences"/> if set (does not fallback to language2 if given as language2-region2 code)
    /// - return First()
    /// </summary>
    public string? GetBestAvailableMatch(string cultureName)
    {
        // We're shortcutting, if this is empty.
        if (_values.Count == 0)
            return null;

        string? result;

        // The best match will always be the one with the original requested language
        if (_values.TryGetValue(cultureName, out result))
            return result;

        // Dropping region suffix, if exists
        var languagePrefix = cultureName.Split('-', 2).First();
        if(languagePrefix != cultureName)
            if (_values.TryGetValue(languagePrefix, out result))
                return result;

        // If both did fail and the default language is not the requested language
        if (!EqualsCI(cultureName, DefaultCultureName) && !EqualsCI(languagePrefix, DefaultCultureName))
            return GetBestAvailableMatch(DefaultCultureName);


        // We'll get here, if we skipped the DefaultCulture, so we'll return by static preference list
        if (CulturePreferences != null) {
            foreach (var cn in CulturePreferences)
                if (_values.TryGetValue(cn, out result))
                    return result;
        }

        // If we still did not return anything by now, we'll return the first value
        return _values.First().Value;


        bool EqualsCI(string? s1, string? s2) => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Converts the L10nString instance to a string by calling <see cref="GetBestAvailableMatch(string)"/>
    /// with <see cref="CultureInfo.CurrentUICulture.Name"/> as parameter.
    /// </summary>
    public override string? ToString()
        => GetBestAvailableMatch(CultureInfo.CurrentUICulture.Name);

    /// <summary>
    /// Convenience function for contexts where ?. is forbidden;
    /// Calls <see cref="ToString"/>() internally, when l10n is not null.
    /// </summary>
    public static string? ToString(L10nString l10n)
        => l10n?.ToString();

    /// <summary>
    /// Explicit cast to string, since we're going to loose some data in that conversion.
    /// Calls <see cref="ToString"/>() internally, when l10n is not null.
    /// </summary>
    public static explicit operator string?(L10nString? l10n)
        => l10n?.ToString();
}
