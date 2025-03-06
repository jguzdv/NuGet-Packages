using Microsoft.Extensions.Logging;

using static JGUZDV.Extensions.Logging.File.FileLogger;

namespace JGUZDV.Extensions.Logging.File;

/// <summary>
/// Scope provider that does nothing.
/// </summary>
internal sealed class NullExternalScopeProvider : IExternalScopeProvider
{
    private NullExternalScopeProvider()
    {
    }

    /// <summary>
    /// Returns a cached instance of <see cref="NullExternalScopeProvider"/>.
    /// </summary>
    public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

    /// <inheritdoc />
    void IExternalScopeProvider.ForEachScope<TState>(Action<object?, TState> callback, TState state)
    {
    }

    /// <inheritdoc />
    IDisposable IExternalScopeProvider.Push(object? state)
    {
        return NullScope.Instance;
    }
}

internal sealed class NullScope : IDisposable
{
    public static NullScope Instance { get; } = new NullScope();

    private NullScope()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}