namespace JGUZDV.ClientStorage.Store;

/// <summary>
/// Data object for <see cref="ClientStore.ValueChanged"/>, <see cref="ClientStore.KeyRegistered"/>, <see cref="ClientStore.KeyUnregistered"/>
/// </summary>
public class StoreChangedEvent
{
    /// <summary>
    /// The key of the changed entry
    /// </summary>
    public required string Key { get; set; }
}
