namespace JGUZDV.ActiveDirectory;

/// <summary>
/// Represents a property in Active Directory with its name and type.
/// </summary>
public class ADPropertyInfo(string propertyName, Type nativeType)
{
    /// <summary>
    /// The name of the property in Active Directory.
    /// </summary>
    public string PropertyName { get; } = propertyName;

    /// <summary>
    /// The native type of the property in Active Directory.
    /// </summary>
    public Type PropertyType { get; } = nativeType;
}