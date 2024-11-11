using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace JGUZDV.DynamicForms.Model;

public abstract class FieldType
{
    [JsonIgnore]
    public abstract Type ClrType { get; }

    public virtual string ConvertFromValue(object value)
    {
        return JsonSerializer.Serialize(value);
    }

    public virtual object ConvertToValue(string stringValue)
    {
        return JsonSerializer.Deserialize(stringValue, ClrType);
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions();
        var typeInfo = new JsonSerializerOptions().GetTypeInfo(typeof(FieldType));
        typeInfo.PolymorphismOptions = JsonPolymorphismOptions;

        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public FieldType FromJson(string json)
    {
        var options = new JsonSerializerOptions();
        var typeInfo = new JsonSerializerOptions().GetTypeInfo(typeof(FieldType));
        typeInfo.PolymorphismOptions = JsonPolymorphismOptions;

        return JsonSerializer.Deserialize<FieldType>(json, options);
    }

    public static JsonPolymorphismOptions JsonPolymorphismOptions =
        new JsonPolymorphismOptions()
        {
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            DerivedTypes =
            {
                new JsonDerivedType(typeof(DateOnlyFieldType), nameof(DateOnlyFieldType)),
            }
        };

    public static List<FieldType> KnownFieldTypes = new()
    {
        new DateOnlyFieldType(),
        new IntFieldType(),
        new StringFieldType(),
    };
}

public class DateOnlyFieldType : FieldType
{
    public override Type ClrType => typeof(DateOnly);


}

public class IntFieldType : FieldType
{
    public override Type ClrType => typeof(int);

    
}

public class StringFieldType : FieldType
{
    public override Type ClrType => typeof(string);
}
