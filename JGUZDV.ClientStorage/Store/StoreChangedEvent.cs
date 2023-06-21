namespace JGUZDV.ClientStorage.Store;

/// <summary>
/// Data object for <see cref="ClientStore.ValueChanged"/>, <see cref="ClientStore.KeyRegistered"/>, <see cref="ClientStore.KeyUnregistered"/>
/// </summary>
public class StoreChangedEvent
{
    public required string Key { get; set; }
}
