using Microsoft.Extensions.Configuration;

using System.Text.Json;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Options for the built-in JSON console log formatter.
/// </summary>
public class JsonFileFormatterOptions : FileFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileFormatterOptions"/> class.
    /// </summary>
    public JsonFileFormatterOptions() {
        FileExtension = "log.json";
    }

    /// <summary>
    /// Gets or sets JsonWriterOptions.
    /// </summary>
    public JsonWriterOptions JsonWriterOptions { get; set; }

    internal override void Configure(IConfiguration configuration) => configuration.Bind(this);
}
