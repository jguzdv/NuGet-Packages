using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class PropertyConverterFactory : IPropertyConverterFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<ActiveDirectoryOptions> _options;
    private readonly ILogger<PropertyConverterFactory> _logger;

    private readonly IEnumerable<IPropertyConverter> _converters;

    public PropertyConverterFactory(IServiceProvider serviceProvider, 
        IOptions<ActiveDirectoryOptions> options,
        ILogger<PropertyConverterFactory> logger, 
        IEnumerable<IPropertyConverter> converters)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;
        _converters = converters;
    }


    public IPropertyConverter GetConverter(string propertyName)
    {
        if (!_options.Value.PropertyConverters.ContainsKey(propertyName))
            _logger.LogInformation($"Property {propertyName} has no registered mapper. Fallback to String");

        return _converters.FirstOrDefault(x => x.ConverterName.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) ??
            _converters.First(x => x.ConverterName.Equals(nameof(StringConverter), StringComparison.OrdinalIgnoreCase));
    }
}
