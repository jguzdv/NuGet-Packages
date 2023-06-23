namespace JGUZDV.ClientStorage.Entries;

public interface IStoreEntry
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public object Value { get; set; }
}
