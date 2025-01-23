using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

public abstract record FieldType
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
            TypeInfoResolver = new DefaultResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    public static FieldType FromJson(string json)
    {
        var options = new JsonSerializerOptions()
        {
            TypeInfoResolver = new DefaultResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        return JsonSerializer.Deserialize<FieldType>(json, options) ?? throw new InvalidOperationException($"Could not parse json: {json}");
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

public record DateOnlyFieldType : FieldType
{
    public override Type ClrType => typeof(DateOnly);

    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Datum",
        ["en"] = "Date"
    };
}

public record IntFieldType : FieldType
{
    public override Type ClrType => typeof(int);

    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Ganzzahl",
        ["en"] = "Integer"
    };
}

public record StringFieldType : FieldType
{
    public override Type ClrType => typeof(string);

    public override string ConvertFromValue(object value)
    {
        return (string)value;
    }

    public override object ConvertToValue(string stringValue)
    {
        return stringValue;
    }

    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Text",
        ["en"] = "Text"
    };
}
