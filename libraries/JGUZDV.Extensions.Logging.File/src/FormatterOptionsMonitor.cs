using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.Extensions.Logging.File;

internal sealed class FormatterOptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions> :
        IOptionsMonitor<TOptions>
        where TOptions : FileFormatterOptions
{
    private readonly TOptions _options;

    public FormatterOptionsMonitor(TOptions options)
    {
        _options = options;
    }

    public TOptions Get(string? name) => _options;

    public IDisposable? OnChange(Action<TOptions, string> listener)
    {
        return null;
    }

    public TOptions CurrentValue => _options;
}
