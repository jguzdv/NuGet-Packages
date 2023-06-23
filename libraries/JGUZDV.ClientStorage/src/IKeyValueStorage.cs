namespace JGUZDV.ClientStorage;

public interface IKeyValueStorage
{
    public Task SetItem<T>(string key, T value, DateTimeOffset? expiresAt = null);

    public Task<T?> GetItem<T>(string key) where T : class;
    public Task RemoveItem(string key);

    public Task Clear();
}