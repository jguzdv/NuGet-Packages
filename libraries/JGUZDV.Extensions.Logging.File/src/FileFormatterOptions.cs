﻿using Microsoft.Extensions.Configuration;

using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Options for the built-in file log formatter.
/// </summary>
public class FileFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileFormatterOptions"/> class.
    /// </summary>
    public FileFormatterOptions() { }

    /// <summary>
    /// Gets or sets a value that indicates whether scopes are included.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if scopes are included.
    /// </value>
    public bool IncludeScopes { get; set; }

    /// <summary>
    /// Gets or sets the format string used to format timestamp in logging messages.
    /// </summary>
    /// <value>
    /// The default is 'yyyy-MM-dd HH:mm:ss.fff'.
    /// </value>
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    public string? TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

    /// <summary>
    /// Gets or sets a value that indicates whether or not UTC timezone should be used to format timestamps in logging messages.
    /// </summary>
    /// <value>
    /// The default is <see langword="false" />.
    /// </value>
    public bool UseUtcTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the file extension to use with this formatter - the default value is 'log' for plain text and 'log.json' for JSON.
    /// </summary>
    public virtual string FileExtension { get; set; } = "log";


    internal virtual void Configure(IConfiguration configuration) => configuration.Bind(this);
}