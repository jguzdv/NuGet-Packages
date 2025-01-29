using System.ComponentModel.DataAnnotations;

using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.Localization;

namespace JGUZDV.DynamicForms.Model;

public class InputDefinition : IValidatableObject
{
    public InputDefinition()
    {
        Name = Guid.NewGuid().ToString();
        Id = Name;
    }

    public L10nString Label { get; set; } = new();

    public string Name { get; set; }
    public string Id { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var service = (ISupportedCultureService)validationContext.GetService(typeof(ISupportedCultureService))!;
        var SL = (IStringLocalizer<Validations>)validationContext.GetService(typeof(IStringLocalizer<Validations>))!;

        foreach (var lang in service?.GetSupportedCultures() ?? new())
        {
            if (string.IsNullOrWhiteSpace(Label[lang])) yield return new ValidationResult(SL[$"{nameof(InputDefinition)}.{nameof(Label)}", lang], new string[] { nameof(Label) });
        }

        //if (string.IsNullOrWhiteSpace(Label["en"]) || string.IsNullOrWhiteSpace(Label["de"])) yield return new ValidationResult("Name muss gesetzt sein", new string[] { nameof(Label) });

    }
}


public class ChoiceOption : IValidatableObject
{
    public string? Value { get; set; }

    public L10nString Name { get; set; } = new();

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

public class FieldDefinition : IValidatableObject
{
    public string Identifier { get; set; } = Guid.NewGuid().ToString();

    public InputDefinition InputDefinition { get; set; } = new();

    public List<ChoiceOption> ChoiceOptions { get; set; } = new();

    public L10nString Description { get; set; } = new();

    public bool IsList { get; set; }

    public int SortKey { get; set; }

    public List<Constraint> Constraints { get; set; } = new();
    public bool IsRequired { get; set; }
    public FieldType? Type { get; set; }

    /// <summary>
    /// Validates without checking the optional <see cref="Constraints"/> and <see cref="ChoiceOptions"/>
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
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
