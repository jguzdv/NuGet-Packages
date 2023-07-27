namespace JGUZDV.ClientStorage.Entries;

/// <summary>
/// the stored entry
/// </summary>
public class StoreEntry<T> : IStoreEntry
{

    /// <summary>
    /// Timestamp for creation
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The expiration of the entry
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// The value
    /// </summary>
    public required T Value { get; set; }

    object IStoreEntry.Value
    {
        get => Value!;
        set => Value = (T)value;
    }
}
