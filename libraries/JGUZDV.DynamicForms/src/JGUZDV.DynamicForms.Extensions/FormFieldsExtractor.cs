using JGUZDV.DynamicForms.Extensions.Models;
using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Extensions;

/// <summary>
/// </summary>
public static class FormFieldsExtractor
{
    /// <summary>
    /// Extracts form fields and file fields from the specified form collection.
    /// </summary>
    /// <param name="form">The form collection to extract fields from.</param>
    /// <returns>The extracted form fields.</returns>
    public static FormFields ExtractFields(IFormCollection form)
    {
        List<FileFormField> fileFields = new();
        foreach (var group in form.Files
            .Where(x => x.Name.StartsWith(DynamicFormsConfiguration.FormFieldPrefix))
            .GroupBy(x => x.Name))
        {
            var identifier = group.First().Name.Replace(DynamicFormsConfiguration.FormFieldPrefix, "");
            var files = new List<FileFieldType.FileType>();

            foreach (var file in group)
            {
                var stream = file.OpenReadStream();
                var filename = file.FileName;
                var size = stream.Length;

                files.Add(new FileFieldType.FileType
                {
                    FileName = filename,
                    FileSize = size,
                    Stream = stream
                });
            }

            fileFields.Add(new FileFormField
            {
                FieldIdentifier = identifier,
                Files = files
            });
        }

        List<FormField> jsonFields = new();
        foreach (var formField in form
            .Where(x => x.Key.StartsWith(DynamicFormsConfiguration.FormFieldPrefix)))
        {
            if (form.Files.Any(x => x.Name == formField.Key))
            {
                continue;
            }

            jsonFields.Add(new FormField
            {
                FieldIdentifier = formField.Key.Replace(DynamicFormsConfiguration.FormFieldPrefix, ""),
                Json = formField.Value.ToString()
            });
        }

        return new FormFields
        {
            Fields = jsonFields,
            FileFields = fileFields
        };
    }
}

