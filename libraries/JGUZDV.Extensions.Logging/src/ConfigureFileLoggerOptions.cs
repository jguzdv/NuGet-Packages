using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace Microsoft.Extensions.DependencyInjection;

internal class ConfigureFileLoggerOptions : IPostConfigureOptions<FileLoggerOptions>, IValidateOptions<FileLoggerOptions>
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IConfiguration _configuration;
    private Dictionary<string, string>? _placeholders;

    public ConfigureFileLoggerOptions(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
    }

    public void PostConfigure(string? name, FileLoggerOptions options)
    {
        options.PathPlaceholderResolver = ResolvePlaceholders;
    }

    public ValidateOptionsResult Validate(string? name, FileLoggerOptions options)
    {
        return options.Files?.Length > 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("At least one entry is required in Logging:File:Files.");
    }


    private string? ResolvePlaceholders(string placeholder, string? inlineFormat, ILogFilePathFormatContext context)
    {
        if (_placeholders == null)
        {
            _placeholders = new(StringComparer.OrdinalIgnoreCase)
            {
                { "ApplicationName", _hostEnvironment.ApplicationName },
                { "Discriminator", _configuration.GetValue<string?>("Logging:File:Discriminator") ?? "" },
                { "MachineName", Environment.MachineName }
            };
        }

        return _placeholders.TryGetValue(placeholder, out var value) ? value : null;
    }
}
