namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Defines the output format for the log files.
/// </summary>
public enum LoggingFileFormat
{
    /// <summary>
    /// Use JSONL files
    /// </summary>
    Json,

    /// <summary>
    /// Use plain text files
    /// </summary>
    PlainText
}