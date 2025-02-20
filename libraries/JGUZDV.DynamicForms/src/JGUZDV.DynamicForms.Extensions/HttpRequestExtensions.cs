using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Extensions.Models;

namespace JGUZDV.DynamicForms.Extensions;

/// <summary>
/// Provides extension methods for extracting form fields from an HTTP request.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Extracts form fields and file fields from the specified HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request to extract fields from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the extracted form fields.</returns>
    public static async Task<FormFields> ExtractFields(this HttpRequest request)
    {
        var form = await request.ReadFormAsync();
        return FormFieldsExtractor.ExtractFields(form);
    }
}
