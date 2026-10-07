namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Provides the values for a field type at runtime. E.g. if allowed values are loaded from a database.
/// </summary>
public interface IFieldTypeValueProvider
{
    /// <summary>
    /// Gets the allowed values for the field type based on its metadata and type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public Task<(bool HandlesType, List<ChoiceOption> AllowedValues)> TryGetValues(FieldType type, string? metadata = null);

    /// <summary>
    /// Gets the allowed values for the field type based on its metadata and type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public Task<List<ChoiceOption>> GetValues(FieldType type, string? metadata = null);
}
