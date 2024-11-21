using System.Runtime.Versioning;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Configures a ConsoleLoggerOptions object from an IConfiguration.
/// </summary>
/// <remarks>
/// Doesn't use ConfigurationBinder in order to allow ConfigurationBinder, and all its dependencies,
/// to be trimmed. This improves app size and startup.
/// </remarks>
internal sealed class FileLoggerConfigureOptions : IConfigureOptions<FileLoggerOptions>
{
    private readonly IConfiguration _configuration;

    [UnsupportedOSPlatform("browser")]
    public FileLoggerConfigureOptions(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
    {
        _configuration = providerConfiguration.Configuration;
    }

    public void Configure(FileLoggerOptions options) => _configuration.Bind(options);
}