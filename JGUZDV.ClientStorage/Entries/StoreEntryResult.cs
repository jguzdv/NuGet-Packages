using JGUZDV.ClientStorage.Store;

namespace JGUZDV.ClientStorage.Entries;

/// <summary>
/// Result object containing the <see cref="StoreEntry{T}"/> and <see cref="LoadingContext"/>
/// </summary>
public class StoreEntryResult<T>
{
    public required StoreEntry<T> Entry { get; set; }
    public required LoadingContext Context { get; set; }
}
