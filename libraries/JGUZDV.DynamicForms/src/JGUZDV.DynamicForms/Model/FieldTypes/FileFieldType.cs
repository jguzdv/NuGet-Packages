using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for file values.
/// </summary>
public class FileFieldType : FieldType
{
    private FileFieldType() : base(
        typeof(FileInfo),
        new L10nString()
        {
            ["de"] = "Datei",
            ["en"] = "File"
        },
        [FileSizeConstraint.ConstraintId])
    { }


    /// <summary>
    /// Gets the singleton instance of the <see cref="FileFieldType"/>.
    /// </summary>
    public static FileFieldType Instance { get; } = new FileFieldType();


    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException"></exception>
    public override void AddToContent(Field field, MultipartFormDataContent content, string name = "")
    {
        //skip null values
        if (field.Value == null)
        {
            return;
        }

        List<FileInfo> files = field.FieldDefinition.IsList
            ? field.Values.OfType<FileInfo>().ToList()
            : [(FileInfo)field.Value];

        name = string.IsNullOrWhiteSpace(name)
            ? $"{DynamicFormsConfiguration.FormFieldPrefix}{field.FieldDefinition.Identifier}"
            : name;

        foreach (var value in files)
        {
            if (value.Stream == null)
            {
                throw new InvalidOperationException("File stream is null.");
            }

            var fileStreamContent = new StreamContent(value.Stream!);
            content.Add(fileStreamContent, name, value.FileName);
        }
    }

    /// <summary>
    /// CLR type for the <see cref="FileFieldType"/>
    /// </summary>
    public record FileInfo : IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public required string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the stream of the file.
        /// </summary>
        [JsonConverter(typeof(StreamConverter))]
        public Stream? Stream { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose() => Stream?.Dispose();
    }
}
