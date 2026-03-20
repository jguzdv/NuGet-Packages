using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Blazor.ChoicOptionComponents;

public class ChoiceOptionEditModel
{
    public L10nString Name { get; set; } = new();
    public string? Value { get; set; }
    public int Index { get; set; }
}
