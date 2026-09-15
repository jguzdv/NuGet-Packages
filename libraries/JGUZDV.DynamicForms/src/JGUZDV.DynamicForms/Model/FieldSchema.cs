namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents the schema of a form, containing a list of field definitions.
/// </summary>
public record FieldSchema(List<FieldDefinition> FieldDefinitions)
{
    /// <summary>
    /// Converts the list of field definitions into a dictionary, where the key is the field identifier and the value is the corresponding field definition.
    /// </summary>
    public Dictionary<FieldId, FieldDefinition> AsDictionary() 
        => FieldDefinitions.ToDictionary(fd => fd.Identifier);
}
