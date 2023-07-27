namespace JGUZDV.ClientStorage;

/// <summary>
/// interface for a simple storage
/// </summary>
public interface IKeyValueStorage
{
    /// <summary>
    /// Save a key value pair
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">the key</param>
    /// <param name="value">the value</param>
    /// <param name="expiresAt">the expiration of the entry</param>
    /// <returns></returns>
    public Task SetItem<T>(string key, T value, DateTimeOffset? expiresAt = null);

    /// <summary>
    /// Gets a item from the storage
    /// </summary>
    /// <typeparam name="T">the type of the item</typeparam>
    /// <param name="key">the key of the entry</param>
    /// <returns>the value</returns>
    public Task<T?> GetItem<T>(string key) where T : class;

    /// <summary>
    /// Remove an entry from storage
    /// </summary>
    /// <param name="key">the key</param>
    /// <returns></returns>
    public Task RemoveItem(string key);

    /// <summary>
    /// Clear the storage
    /// </summary>
    /// <returns></returns>
    public Task Clear();
}