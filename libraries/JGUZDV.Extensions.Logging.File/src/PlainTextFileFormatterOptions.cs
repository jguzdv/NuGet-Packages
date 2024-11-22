using Microsoft.Extensions.Configuration;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Options for the built-in plain-text file log formatter.
/// </summary>
public class PlainTextFileFormatterOptions : FileFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlainTextFileFormatterOptions"/> class.
    /// </summary>
    public PlainTextFileFormatterOptions() { }

    internal override void Configure(IConfiguration configuration) => configuration.Bind(this);
}