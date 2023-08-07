using JGUZDV.ActiveDirectory.ClaimProvider.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class PropertyConverterFactory : IPropertyConverterFactory
{
    private record ConverterInfo(Type Type, string? OutputFormat);

    private readonly ILogger<PropertyConverterFactory> _logger;

    private readonly Dictionary<ConverterInfo, IPropertyConverter> _converters;
    private readonly Dictionary<string, ADPropertyInfo> _propertyInfos;

    public PropertyConverterFactory(
        IOptions<ActiveDirectoryOptions> options,
        ILogger<PropertyConverterFactory> logger, 
        IEnumerable<IPropertyConverter> converters)
    {
        _logger = logger;
        
        _propertyInfos = options.Value.Properties.ToDictionary(x => x.PropertyName, StringComparer.OrdinalIgnoreCase);
        
        _converters = new();
        foreach(var c in converters)
        {
            _converters.Add(new(c.PropertyType, null), c);

            foreach (var f in c.OutputFormats)
                _converters.Add(new(c.PropertyType, f), c);
        }
    }


    public IPropertyConverter GetConverter(string propertyName, string? outputFormat)
    {
        if (!_propertyInfos.TryGetValue(propertyName, out var propertyInfo)) {
            _logger.LogInformation("Property {propertyName} has no registered mapper. Fallback to String", propertyName);
            propertyInfo = new(propertyName, typeof(string));
        }

        var converterInfo = new ConverterInfo(propertyInfo.PropertyType, outputFormat);
        if(!_converters.TryGetValue(converterInfo, out var converter)) {
            _logger.LogError("Converter for {property}, with source type {type} and format {format} has not been found.",
                propertyName, converterInfo.Type, converterInfo.OutputFormat);
            throw new InvalidOperationException("Converter could not be found.");
        }

        return converter;
    }
}
