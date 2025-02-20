using System.Collections.Generic;
using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Extensions.Models;

/// <summary>
/// Represents a file form field.
/// </summary>
public class FileFormField
{
    /// <summary>
    /// Gets or sets the field identifier.
    /// </summary>
    public required string FieldIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the list of files associated with the field.
    /// </summary>
    public required List<FileFieldType.FileType> Files { get; set; }
}
