using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents the base class for field types.
/// </summary>
[JsonConverter(typeof(FieldTypeConverter))]
public abstract record FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public abstract Type ClrType { get; }

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    [JsonIgnore]
    public abstract L10nString DisplayName { get; }

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    [JsonIgnore]
    public virtual string InputType { get; } = "text";

    /// <summary>
    /// Converts the specified value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representation of the value.</returns>
    public virtual string ConvertFromValue(object value)
    {
        return JsonSerializer.Serialize(value, DynamicFormsConfiguration.JsonSerializerOptions);
    }

    /// <summary>
    /// Converts the specified string to an object of the CLR type.
    /// </summary>
    /// <param name="stringValue">The string to convert.</param>
    /// <returns>An object of the CLR type.</returns>
    public virtual object ConvertToValue(string stringValue)
    {
        return JsonSerializer.Deserialize(stringValue, ClrType, DynamicFormsConfiguration.JsonSerializerOptions) ?? throw new InvalidOperationException($"Could not parse json: {stringValue} into target type: {ClrType.Name}");
    }

    /// <summary>
    /// Converts the field type to a JSON string.
    /// </summary>
    /// <returns>A JSON string representation of the field type.</returns>
    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, DynamicFormsConfiguration.JsonSerializerOptions);
    }

    /// <summary>
    /// Creates a field type from the specified JSON string.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>A field type object.</returns>
    public static FieldType FromJson(string json)
    {
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        return JsonSerializer.Deserialize<FieldType>(json, options) ?? throw new InvalidOperationException($"Could not parse json: {json}");
    }

    /// <summary>
    /// Adds the field value to the content.
    /// </summary>
    public virtual void AddToContent(Field field, MultipartFormDataContent content, string name = "")
    {
        var json = JsonSerializer.Serialize(field.Value, DynamicFormsConfiguration.JsonSerializerOptions);

        name = string.IsNullOrWhiteSpace(name)
            ? $"{DynamicFormsConfiguration.FormFieldPrefix}{field.FieldDefinition.Identifier}"
            : name;

        content.Add(new StringContent(json), name);
    }
}

/// <summary>
/// Provides the values for a field type at runtime. E.g. if allowed values are loaded from a database.
/// </summary>
public interface IFieldTypeValueProvider
{
    public Task<(bool HandlesType, List<ChoiceOption> AllowedValues)> TryGetValues(FieldType type, string? metadata = null);

    public Task<List<ChoiceOption>> GetValues(FieldType type, string? metadata = null);
}

public interface IFieldTypeMetadataProvider
{
    L10nString GetMetadataDisplayName(FieldType type);

    public Task<(bool HandlesType, List<ChoiceOption> AllowedValues)> TryGetValues(FieldType type);
    public Task<List<ChoiceOption>> GetValues(FieldType type);
}


/// <summary>
/// Represents a field type for DateOnly values.
/// </summary>
public record DateOnlyFieldType : FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public override Type ClrType => typeof(DateOnly);

    /// <summary>
    /// Gets the input type of the field.
    /// </summary>
    [JsonIgnore]
    public override string InputType => "date";

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Datum",
        ["en"] = "Date"
    };

    /// <summary>
    /// Converts the specified value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representation of the value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the value type is invalid.</exception>
    public override string ConvertFromValue(object value)
    {
        if (value is DateOnly dateOnly)
        {
            return dateOnly.ToString("O");
        }
        throw new InvalidOperationException($"Invalid value type: {value.GetType().Name}. Expected DateOnly.");
    }

    /// <summary>
    /// Converts the specified string to a DateOnly object.
    /// </summary>
    /// <param name="stringValue">The string to convert.</param>
    /// <returns>A DateOnly object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the string cannot be parsed.</exception>
    public override object ConvertToValue(string stringValue)
    {
        if (DateOnly.TryParse(stringValue, out var dateOnly))
        {
            return dateOnly;
        }
        throw new InvalidOperationException($"Could not parse string: {stringValue} into DateOnly.");
    }
}

/// <summary>
/// Represents a field type for integer values.
/// </summary>
public record IntFieldType : FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public override Type ClrType => typeof(int);

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Ganzzahl",
        ["en"] = "Integer"
    };
}

/// <summary>
/// Represents a field type for string values.
/// </summary>
public record StringFieldType : FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public override Type ClrType => typeof(string);

    /// <summary>
    /// Converts the specified value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representation of the value.</returns>
    public override string ConvertFromValue(object value)
    {
        return (string)value;
    }

    /// <summary>
    /// Converts the specified string to a string object.
    /// </summary>
    /// <param name="stringValue">The string to convert.</param>
    /// <returns>A string object.</returns>
    public override object ConvertToValue(string stringValue)
    {
        return stringValue;
    }

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Text",
        ["en"] = "Text"
    };
}

/// <summary>
/// Represents a field type for file values.
/// </summary>
public record FileFieldType : FieldType
{
    /// <summary>
    /// Gets the CLR type of the field.
    /// </summary>
    [JsonIgnore]
    public override Type ClrType => typeof(FileType);

    /// <summary>
    /// Gets the display name of the field type.
    /// </summary>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Datei",
        ["en"] = "File"
    };

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public override void AddToContent(Field field, MultipartFormDataContent content, string name = "")
    {
        //skip null values
        if (field.Value == null)
        {
            return;
        }

        List<FileType> files = field.FieldDefinition.IsList
            ? field.Values.OfType<FileType>().ToList()
            : [(FileType)field.Value];

        name = string.IsNullOrWhiteSpace(name)
            ? $"{DynamicFormsConfiguration.FormFieldPrefix}{field.FieldDefinition.Identifier}"
            : name;

        foreach (var value in files)
        {
            if (value.Stream == null)
            {
                throw new InvalidOperationException("File stream is null.");
            }

            var fileStreamContent = new StreamContent(value.Stream!);
            content.Add(fileStreamContent, name, value.FileName);
        }
    }

    /// <summary>
    /// CLR type for the <see cref="FileFieldType"/>
    /// </summary>
    public record FileType : IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public required string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the stream of the file.
        /// </summary>
        [JsonConverter(typeof(StreamConverter))]
        public Stream? Stream { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
