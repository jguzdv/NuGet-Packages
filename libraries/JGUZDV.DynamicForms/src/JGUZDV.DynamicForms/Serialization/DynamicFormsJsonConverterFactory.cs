using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;

namespace JGUZDV.DynamicForms.Serialization;

public sealed class DynamicFormsJsonConverterFactory(
    FieldTypeRegistry fieldTypeRegistry,
    ConstraintTypeRegistry constraintTypeRegistry) : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(FieldType) || typeToConvert == typeof(IConstraint);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return typeToConvert == typeof(FieldType)
            ? new FieldTypeConverter(fieldTypeRegistry)
            : typeToConvert == typeof(IConstraint)
                ? new ConstraintConverter(constraintTypeRegistry)
                : throw new NotSupportedException($"The type {typeToConvert} is not supported.");
    }
}