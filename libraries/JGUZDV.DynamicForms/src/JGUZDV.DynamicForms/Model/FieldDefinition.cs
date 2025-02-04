using System.ComponentModel.DataAnnotations;

using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.Localization;

namespace JGUZDV.DynamicForms.Model;

/// <summary>
/// Represents the definition of an input field.
/// </summary>
public class InputDefinition : IValidatableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputDefinition"/> class.
    /// </summary>
    public InputDefinition()
    {


        Name = Guid.NewGuid().ToString();
        Id = Name;
    }

    /// <summary>
    /// Gets or sets the label of the input field.
    /// </summary>
    public L10nString Label { get; set; } = new();

    /// <summary>
    /// Gets or sets the name of the input field.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the input field.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Validates the input field.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var service = (ISupportedCultureService)validationContext.GetService(typeof(ISupportedCultureService))!;
        var SL = (IStringLocalizer<Validations>)validationContext.GetService(typeof(IStringLocalizer<Validations>))!;

        foreach (var lang in service?.GetSupportedCultures() ?? new())
        {
            if (string.IsNullOrWhiteSpace(Label[lang])) yield return new ValidationResult(SL[$"{nameof(InputDefinition)}.{nameof(Label)}", lang], new string[] { nameof(Label) });
        }
    }
}


/// <summary>
/// Represents a choice option for a field.
/// </summary>
public class ChoiceOption : IValidatableObject
{
    /// <summary>
    /// Gets or sets the value of the choice option.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the name of the choice option.
    /// </summary>
    public L10nString Name { get; set; } = new();

    /// <summary>
    /// Validates the choice option.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var service = (ISupportedCultureService)validationContext.GetService(typeof(ISupportedCultureService))!;
        var SL = (IStringLocalizer<Validations>)validationContext.GetService(typeof(IStringLocalizer<Validations>))!;

        foreach (var lang in service?.GetSupportedCultures() ?? new())
        {
            if (string.IsNullOrWhiteSpace(Name[lang])) yield return new ValidationResult(SL[$"{nameof(ChoiceOption)}.{nameof(Name)}", lang], new string[] { nameof(Name) });
        }

        if (string.IsNullOrWhiteSpace(Value)) yield return new ValidationResult(SL[$"{nameof(ChoiceOption)}.{nameof(Value)}"], new string[] { nameof(Value) });
    }
}

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
