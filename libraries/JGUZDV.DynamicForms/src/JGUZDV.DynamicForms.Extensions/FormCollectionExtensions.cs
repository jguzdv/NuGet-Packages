using JGUZDV.DynamicForms.Extensions.Models;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.DynamicForms.Extensions;

/// <summary>
/// Provides extension methods for extracting form fields from an IFormCollection.
/// </summary>
public static class FormCollectionExtensions
{
    /// <summary>
    /// Extracts form fields and file fields from the specified form collection.
    /// </summary>
    /// <param name="form">The form collection to extract fields from.</param>
    /// <returns>The extracted form fields.</returns>
    public static FormFields ExtractFields(this IFormCollection form)
    {
        return FormFieldsExtractor.ExtractFields(form);
    }
}

