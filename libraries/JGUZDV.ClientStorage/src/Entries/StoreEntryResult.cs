using JGUZDV.ClientStorage.Store;

namespace JGUZDV.ClientStorage.Entries;

/// <summary>
/// Result object containing the <see cref="StoreEntry{T}"/> and <see cref="LoadingContext"/>
/// </summary>
public class StoreEntryResult<T>
{
    /// <summary>
    /// the entry
    /// </summary>
    public required StoreEntry<T> Entry { get; set; }

    /// <summary>
    /// the loading context
    /// </summary>
    public required LoadingContext Context { get; set; }
}
