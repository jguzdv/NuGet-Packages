namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a collection of fields.
/// </summary>
public record FieldCollection(List<Field> Fields)
{
    /// <summary>
    /// Uses the given Schema and values to create a new FieldCollection.
    /// </summary>
    public static FieldCollection CreateFromSchemaAndValues(FieldSchema schema, FieldValues fieldValues)
    {
        var schemaFieldDefinitions = schema.AsDictionary();
        var result = new List<Field>();

        foreach (var fieldValue in fieldValues.Values)
        {
            var fieldId = fieldValue.Key;
            if (!schemaFieldDefinitions.TryGetValue(fieldId, out var fieldDefinition))
            {
                throw new FieldDefinitionNotFoundException(fieldId);
            }

            var field = new Field(fieldDefinition, fieldValue.Value);
            result.Add(field);
        }

        return new(result);
    }

    /// <summary>
    /// Converts the FieldCollection to a FieldValues object.
    /// </summary>
    public FieldValues AsFieldValues()
        => new(Fields.ToDictionary(field => field.FieldDefinition.Identifier, field => field.Value));
}
