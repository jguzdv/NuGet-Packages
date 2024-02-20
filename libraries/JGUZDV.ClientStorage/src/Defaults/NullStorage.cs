namespace JGUZDV.ClientStorage.Defaults;

/// <summary>
/// Null storage if no client side persistence is needed
/// </summary>
public class NullStorage : IKeyValueStorage
{
    /// <inheritdoc/>
    public Task Clear()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<T?> GetItem<T>(string key) where T : class
    {
        return Task.FromResult<T?>(null);
    }

    /// <inheritdoc/>
    public Task RemoveItem(string key)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SetItem<T>(string key, T value, DateTimeOffset? expiresAt = null)
    {
        return Task.CompletedTask;
    }
}
