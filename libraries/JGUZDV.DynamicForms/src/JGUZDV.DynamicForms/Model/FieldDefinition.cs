using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.Localization;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents the definition of a field.
/// </summary>
public class FieldDefinition : IValidatableObject
{
    /// <summary>
    /// Gets or sets the identifier of the field.
    /// </summary>
    public string Identifier { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the input definition of the field.
    /// </summary>
    public InputDefinition InputDefinition { get; set; } = new();

    /// <summary>
    /// Gets or sets the choice options for the field.
    /// </summary>
    public List<ChoiceOption> ChoiceOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the description of the field.
    /// </summary>
    public L10nString Description { get; set; } = new();

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is a list.
    /// </summary>
    public bool IsList { get; set; }

    /// <summary>
    /// Gets or sets the sort key of the field.
    /// </summary>
    public int SortKey { get; set; }

    /// <summary>
    /// Gets or sets the constraints for the field.
    /// </summary>
    public List<Constraint> Constraints { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the field is required.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the type of the field.
    /// </summary>
    public FieldType? Type { get; set; }


    /// <summary>
    /// Copies the properties from another <see cref="FieldDefinition"/> instance to this instance.
    /// </summary>
    /// <param name="other"></param>
    public void CopyFrom(FieldDefinition other)
    {
        foreach (var prop in GetType().GetProperties())
        {
            if (prop.CanWrite && prop.CanRead)
            {
                var value = prop.GetValue(other);
                prop.SetValue(this, value);
            }
        }
    }

    /// <summary>
    /// Deep Copy of the field definition.
    /// </summary>
    /// <returns></returns>
    public FieldDefinition Copy()
    {
        return JsonSerializer.Deserialize<FieldDefinition>(
            JsonSerializer.Serialize(this, DynamicFormsConfiguration.JsonSerializerOptions),
            DynamicFormsConfiguration.JsonSerializerOptions)!;
    }

    /// <summary>
    /// Validates the field definition without checking the optional constraints and choice options.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> ValidateBase(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        errors.AddRange(InputDefinition.Validate(validationContext).ToList());

        var service = (ISupportedCultureService)validationContext.GetService(typeof(ISupportedCultureService))!;
        var SL = (IStringLocalizer<Validations>)validationContext.GetService(typeof(IStringLocalizer<Validations>))!;

        foreach (var lang in service?.GetSupportedCultures() ?? new())
        {
            if (string.IsNullOrWhiteSpace(Description[lang]))
                errors.Add(new ValidationResult(SL[$"{nameof(FieldDefinition)}.{nameof(Description)}", lang], new string[] { nameof(Description) }));
        }

        if (SortKey < 0)
            errors = errors.Append(new ValidationResult(SL[$"{nameof(FieldDefinition)}.{nameof(SortKey)}"], new string[] { nameof(SortKey) })).ToList();

        if (string.IsNullOrWhiteSpace(Identifier))
        {
            errors.Add(new(SL[$"{nameof(FieldDefinition)}.{nameof(Identifier)}"], new string[] { nameof(Identifier) }));
        }

        if (null == Type)
            errors.Add(new ValidationResult(SL[$"{nameof(FieldDefinition)}.{nameof(Type)}"], new string[] { nameof(Type) }));

        return errors;
    }

    /// <summary>
    /// Validates the field definition.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        errors.AddRange(ValidateBase(validationContext));

        foreach (var choice in ChoiceOptions)
        {
            errors.AddRange(choice.Validate(validationContext));
        }

        foreach (var constraint in Constraints)
        {
            errors.AddRange(constraint.Validate(validationContext));
        }

        return errors;
    }
}
