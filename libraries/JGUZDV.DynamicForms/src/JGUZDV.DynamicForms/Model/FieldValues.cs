using System.Text.Json.Serialization;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents a collection of field values.
/// This is meant for transport via JSON
/// Use <see cref="FieldCollection.AsFieldValues"/> to create a <see cref="FieldValues"/> object from a <see cref="FieldCollection"/>
/// Use <see cref="FieldCollection.CreateFromSchemaAndValues"/> to create a <see cref="FieldCollection"/> from a <see cref="FieldSchema"/> and a <see cref="FieldValues"/>
/// </summary>
[JsonConverter(typeof(FieldValuesConverter))]
public record FieldValues(Dictionary<FieldId, object?> Values);