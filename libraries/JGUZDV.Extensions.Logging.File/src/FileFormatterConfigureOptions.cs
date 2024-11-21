using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Configures a FileFormatterOptions object from an IConfiguration.
/// </summary>
/// <remarks>
/// Doesn't use ConfigurationBinder in order to allow ConfigurationBinder, and all its dependencies,
/// to be trimmed. This improves app size and startup.
/// </remarks>
internal sealed class FileFormatterConfigureOptions : IConfigureOptions<FileFormatterOptions>
{
    private readonly IConfiguration _configuration;

    [UnsupportedOSPlatform("browser")]
    public FileFormatterConfigureOptions(ILoggerProviderConfiguration<FileLoggerProvider> providerConfiguration)
    {
        _configuration = providerConfiguration.GetFormatterOptionsSection();
    }

    public void Configure(FileFormatterOptions options) => options.Configure(_configuration);
}
