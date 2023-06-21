namespace JGUZDV.ClientStorage.Entries;

/// <summary>
/// the stored entry
/// </summary>
public class StoreEntry<T> : IStoreEntry
{
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public required T Value { get; set; }

    object IStoreEntry.Value
    {
        get => Value!;
        set => Value = (T)value;
    }
}
