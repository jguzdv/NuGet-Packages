namespace JGUZDV.ClientStorage.Entries;

/// <summary>
/// Interface for an entry
/// </summary>
public interface IStoreEntry
{
    /// <summary>
    /// Timestamp for creation
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The expiration of the entry
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

}
