using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Provides the metadata for a field type
/// </summary>
public interface IFieldTypeMetadataProvider
{
    /// <summary>
    /// Get the display name for the field types metadata.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    L10nString GetMetadataDisplayName(FieldType type);

    /// <summary>
    /// Gets the allowed metadata values for the field type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Task<(bool HandlesType, List<ChoiceOption> AllowedValues)> TryGetValues(FieldType type);

    /// <summary>
    /// Gets the allowed metadata values for the field type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Task<List<ChoiceOption>> GetValues(FieldType type);
}
