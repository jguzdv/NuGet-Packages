namespace JGUZDV.DynamicForms.Extensions.Models;

/// <summary>
/// Represents the extracted form fields.
/// </summary>
public class FormFields
{
    /// <summary>
    /// Gets or sets the list of form fields.
    /// </summary>
    public required List<FormField> Fields { get; set; }

    /// <summary>
    /// Gets or sets the list of file form fields.
    /// </summary>
    public required List<FileFormField> FileFields { get; set; }
}
