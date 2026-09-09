using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

namespace JGUZDV.DynamicForms.Model.FieldTypes;

/// <summary>
/// Represents a field type for file values.
/// </summary>
public record FileFieldType : FieldType
{
    /// <inheritdoc/>
    public override string TypeDiscriminator => "File";

    /// <inheritdoc/>
    public override Type ClrType => typeof(FileInfo);

    /// <inheritdoc/>
    public override L10nString DisplayName => new L10nString()
    {
        ["de"] = "Datei",
        ["en"] = "File"
    };

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
        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
