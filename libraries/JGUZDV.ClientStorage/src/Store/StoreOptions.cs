namespace JGUZDV.ClientStorage.Store;

/// <summary>
/// options to configure loading and caching of an entry
/// </summary>
/// <typeparam name="T">type of the value</typeparam>
public class StoreOptions<T>
{
    /// <summary>
    /// the duration a loaded value is valid
    /// </summary>
    public TimeSpan ValueExpiry { get; set; }

    /// <summary>
    /// function for loading the value
    /// </summary>
    public required Func<CancellationToken, Task<T>> LoadFunc { get; set; }

    /// <summary>
    /// if true, the entry is automatically refreshed in the background
    /// </summary>
    public bool UsesBackgroundRefresh { get; set; }
}
