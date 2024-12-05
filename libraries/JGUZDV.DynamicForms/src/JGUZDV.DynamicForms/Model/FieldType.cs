using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

public abstract class FieldType
{
    [JsonIgnore]
    public abstract Type ClrType { get; }

    [JsonIgnore]
    public abstract L10nString DisplayName { get; }

    public virtual string ConvertFromValue(object value)
    {
        return JsonSerializer.Serialize(value);
    }

    public virtual object ConvertToValue(string stringValue)
    {
        return JsonSerializer.Deserialize(stringValue, ClrType) ?? throw new InvalidOperationException($"Could not parse json: {stringValue} into target type: {ClrType.Name}"); ;
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions()
        {
            TypeInfoResolver = new FieldTypeResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public static FieldType FromJson(string json)
    {
        var options = new JsonSerializerOptions()
        {
            TypeInfoResolver = new FieldTypeResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        return JsonSerializer.Deserialize<FieldType>(json, options) ?? throw new InvalidOperationException($"Could not parse json: {json}");
    }

    public static List<FieldType> KnownFieldTypes = new()
    {
        new DateOnlyFieldType(),
        new IntFieldType(),
        new StringFieldType(),
    };

    private static JsonPolymorphismOptions BuildJsonPolymorphismOptions()
    {
        var options = new JsonPolymorphismOptions
        {
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
        };

        foreach (var fieldType in KnownFieldTypes)
        {
            options.DerivedTypes.Add(new JsonDerivedType(fieldType.GetType(), fieldType.GetType().Name));
        }

        return options;
    }

    private class FieldTypeResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var typeInfo = base.GetTypeInfo(type, options);

            if (typeInfo.Type == typeof(FieldType))
                typeInfo.PolymorphismOptions = BuildJsonPolymorphismOptions();

            if (typeInfo.Type.IsAssignableTo(typeof(FieldType)))
            {
                typeInfo.Properties.Remove(typeInfo.Properties.FirstOrDefault(x => x.Name == "ClrType")!);
            }

            return typeInfo;
        }
    }
}

//this should allow to inject at runtime dynamically allowed values e.g. resources in ResourceProvisioning
public class ValueProvider
{
    private readonly Dictionary<FieldType, Func<Task<List<ChoiceOption>>>> _funcs = new();

    public async Task<(bool HandlesType, List<ChoiceOption> AllowedValues)> GetValues(FieldType type)
    {
        if (_funcs.TryGetValue(type, out var func))
        {
            return (true, await func());
        }

        return (false, []);
    }
}

public class DateOnlyFieldType : FieldType
{
    public override Type ClrType => typeof(DateOnly);

    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Datum",
        ["en"] = "Date"
    };
}

public class IntFieldType : FieldType
{
    public override Type ClrType => typeof(int);

    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Ganzzahl",
        ["en"] = "Integer"
    };
}

public class StringFieldType : FieldType
{
    public override Type ClrType => typeof(string);

    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Text",
        ["en"] = "Text"
    };
}
