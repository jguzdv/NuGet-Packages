using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Options for the built-in plain-text file log formatter.
/// </summary>
public class PlainTextFileFormatterOptions : FileFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConsoleFormatterOptions"/> class.
    /// </summary>
    public PlainTextFileFormatterOptions() { }

    /// <summary>
    /// Gets or sets a value that indicates whether the entire message is logged in a single line.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the entire message is logged in a single line.
    /// </value>
    public bool SingleLine { get; set; }

    internal override void Configure(IConfiguration configuration) => configuration.Bind(this);
}