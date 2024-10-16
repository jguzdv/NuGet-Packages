namespace JGUZDV.ActiveDirectory.Configuration;

/// <summary>
/// Property reader configuration options.
/// </summary>
public class PropertyReaderOptions
{
    /// <summary>
    /// A list of known properties in Active Directory. It's initialized with <c>Defaults.KnownProperties</c>.
    /// </summary>
    public Dictionary<string, ADPropertyInfo> PropertyInfos { get; set; } = Defaults.KnownProperties.ToDictionary(x => x.PropertyName, StringComparer.OrdinalIgnoreCase);
}