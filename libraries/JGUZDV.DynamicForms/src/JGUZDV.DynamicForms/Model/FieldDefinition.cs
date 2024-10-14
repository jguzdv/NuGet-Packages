using System.ComponentModel.DataAnnotations;

namespace JGUZDV.DynamicForms.Model;

public class InputDefinition : IValidatableObject
{
    public InputDefinition()
    {
        Name = Guid.NewGuid().ToString();
        Id = Name;
    }

    public string Label { get; set; } = "";
    //public L10nString Label { get; set; } = new();

    public string Name { get; set; }
    public string Id { get; set; }
    public string Type { get; set; } = "";
    public string InputType { get; set; } = "text";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //if (string.IsNullOrWhiteSpace(Label["en"]) || string.IsNullOrWhiteSpace(Label["de"])) yield return new ValidationResult("Name muss gesetzt sein", new string[] { nameof(Label) });
        if (string.IsNullOrWhiteSpace(Type)) yield return new ValidationResult("Typ muss gesetzt sein", new string[] { nameof(Type) });
    }
}


public class ChoiceOption : IValidatableObject
{
    public string? Value { get; set; }
    //public L10nString Name { get; set; } = new();
    public string Name { get; set; } = "new()";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //if (string.IsNullOrWhiteSpace(Name["de"]) || string.IsNullOrWhiteSpace(Name["en"])) yield return new ValidationResult("Name muss gesetzt sein", new string[] { nameof(Name) });
        if (string.IsNullOrWhiteSpace(Value)) yield return new ValidationResult("Wert muss gesetzt sein", new string[] { nameof(Value) });
    }
}

public class FieldDefinition : IValidatableObject
{
    public string Identifier { get; set; } = Guid.NewGuid().ToString();

    public InputDefinition InputDefinition { get; set; } = new();

    public List<ChoiceOption> ChoiceOptions { get; set; } = new();

    //public L10nString Description { get; set; } = new();
    public string Description { get; set; } = "";

    public bool IsList { get; set; }

    public int SortKey { get; set; }

    public List<Constraint> Constraints { get; set; } = new();
    public bool IsRequired { get; set; }

    private bool _isResource;
    public bool IsResource
    {
        get => _isResource;
        set
        {
            _isResource = value;
            if (_isResource)
                InputDefinition.Type = typeof(string).Name;
        }
    }

    public int? ResourceTypeId { get; set; }

    // das muss auch eine resource sein können
    // wir brauchen constraints, typ abhhängig "optional,max,min,default value, validateSet",
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        errors.AddRange(InputDefinition.Validate(validationContext).ToList());

        if (IsResource && !ResourceTypeId.HasValue)
            errors.Add(new ValidationResult("ResourceType muss gesetzt sein, wenn das Feld eine Ressource definiert", new string[] { nameof(ResourceTypeId) }));

        if (SortKey < 0)
            errors = errors.Append(new ValidationResult("SortKey muss positiv sein", new string[] { nameof(SortKey) })).ToList();

        if (string.IsNullOrWhiteSpace(Identifier))
        {
            errors.Add(new($"{nameof(Identifier)} muss gesetzt sein", new string[] { nameof(Identifier) }));
        }

        foreach (var choice in ChoiceOptions)
        {
            errors.AddRange(choice.Validate(validationContext));
        }

        return errors;
    }
}
