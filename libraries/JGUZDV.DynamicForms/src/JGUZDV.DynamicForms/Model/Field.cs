using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JGUZDV.DynamicForms.Model;


/// <summary>
/// Represents a collection of fields.
/// </summary>
public class FieldCollection
{
    /// <summary>
    /// Gets or sets the list of fields.
    /// </summary>
    public required List<Field> Fields { get; set; }
}

/// <summary>
/// Represents a form field and provides validation functionality.
/// </summary>
[JsonConverter(typeof(FieldConverter))]
public class Field : IValidatableObject, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Field"/> class with the specified field definition.
    /// </summary>
    /// <param name="fieldDefinition">The definition of the field.</param>
    public Field(FieldDefinition fieldDefinition)
    {
        FieldDefinition = fieldDefinition;
        Value = fieldDefinition.IsList
            ? Activator.CreateInstance(typeof(List<>).MakeGenericType(ValueType.ClrType))!
            : null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Field"/> class with the specified field definition and value.
    /// </summary>
    /// <param name="fieldDefinition">The definition of the field.</param>
    /// <param name="value">The value of the field.</param>
    public Field(FieldDefinition fieldDefinition, object? value)
    {
        FieldDefinition = fieldDefinition;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the field definition.
    /// </summary>
    public FieldDefinition FieldDefinition { get; }

    private object? _value;

    /// <summary>
    /// Gets or sets the value of the field.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the value type does not match the field definition.</exception>
    public object? Value
    {
        get => _value;
        set
        {
            if (value != null)
            {
                var valueType = value.GetType();
                if (FieldDefinition.IsList)
                {
                    if (!typeof(IEnumerable).IsAssignableFrom(valueType)
                        || !valueType.IsGenericType
                        || valueType.GetGenericArguments()[0] != ValueType.ClrType)
                    {
                        throw new InvalidOperationException("Type does not match");
                    }
                }
                else if (value.GetType() != ValueType.ClrType)
                {
                    throw new InvalidOperationException("Type does not match");
                }
            }

            if (value == null && FieldDefinition.IsList)
            {
                throw new InvalidOperationException("List fields can not be null");
            }

            _value = value;
        }
    }

    /// <summary>
    /// Gets the list of values if the field is a list.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the field is not a list.</exception>
    [JsonIgnore]
    public IReadOnlyList<object> Values => FieldDefinition.IsList
        ? ((IList)Value!).OfType<object>().ToList()
        : throw new InvalidOperationException("Only list fields have multiple values");

    /// <summary>
    /// Gets the type of the field.
    /// </summary>
    [JsonIgnore]
    public FieldType ValueType => FieldDefinition.Type ?? throw new InvalidOperationException("Invalid FieldDefinition");

    /// <summary>
    /// Adds the <see cref="Value"/> of the field to the content with <see cref="FieldDefinition.Identifier"/> as default name.
    /// </summary>
    /// <param name="content">The content to add the field value to.</param>
    /// <param name="name">The name of the form field. Defaults to <see cref="FieldDefinition.Identifier"/></param>
    public virtual void AddToContent(MultipartFormDataContent content, string name = "")
        => ValueType.AddToContent(this, content, name);

    /// <summary>
    /// Determines whether the field is valid.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>True if the field is valid; otherwise, false.</returns>
    public bool IsValid(ValidationContext validationContext) => !Validate(validationContext).Any();

    /// <summary>
    /// Validates the field value based on the field definition and constraints.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Value == null)
        {
            if (FieldDefinition.IsRequired)
            {
                return new List<ValidationResult>() { new("Required field can not be null", new string[] { FieldDefinition.InputDefinition.Label.ToString()! }) };
            }
            else
            {
                return new List<ValidationResult>();
            }
        }

        List<object> val;

        if (FieldDefinition.IsList)
        {
            if (Values == null)
                return new List<ValidationResult>
                {
                    new("Value can not be null", [nameof(Value)])
                };

            val = Values.OfType<object>().ToList()!;
        }
        else
        {
            val = ((IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ValueType.ClrType))!).OfType<object>().ToList();
            val.Add(Value);
        }

        var errors = FieldDefinition.Constraints.SelectMany(x => x.ValidateConstraint(val, validationContext));

        return errors;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public void Dispose()
    {
        if (FieldDefinition.Type!.ClrType is not IDisposable)
        {
            return;
        }

        if (FieldDefinition.IsList)
        {
            foreach (var item in Values)
            {
                ((IDisposable)item).Dispose();
            }
        }
        else
        {
            ((IDisposable?)Value)?.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (FieldDefinition.Type!.ClrType is not IAsyncDisposable)
        {
            return;
        }

        if (Value == null)
        {
            return;
        }

        if (FieldDefinition.IsList)
        {
            foreach (var item in Values)
            {
                await ((IAsyncDisposable)item).DisposeAsync();
            }
        }
        else
        {
            await ((IAsyncDisposable)Value).DisposeAsync();
        }
    }
}
