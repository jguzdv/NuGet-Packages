namespace JGUZDV.ClientStorage.Store;

/// <summary>
/// Additional information how the entry was retrieved
/// </summary>
public class LoadingContext
{
    /// <summary>
    /// Flag to indicate if result comes from the persistent storage
    /// </summary>
    public bool LoadedFromStorage { get; set; }

    /// <summary>
    /// Flag to indiciate if result comes from the cache
    /// </summary>
    public bool LoadedFromCache { get; set; }

    /// <summary>
    /// Flag to indicate if result was loaded from source
    /// </summary>
    public bool LoadedHot { get; set; }

    /// <summary>
    /// The key of the loaded entry
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Exception that occured during loading
    /// </summary>
    public Exception? Exception { get; set; }
}
